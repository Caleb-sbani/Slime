using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Slime;

namespace Slime.NPCs
{
    class RevengerSlime : ModNPC
    {
        //this AI is highly inspired by tModLoader's Example Mod's Flutter Slime
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 6; //number of animation frames
        }
        public override void SetDefaults()
        {
            npc.width = 32;
            npc.height = 32;
            npc.aiStyle = -1; //-1 == unique ai style
            npc.damage = 15;
            npc.defense = 5;
            npc.knockBackResist = 0.3f;
            npc.lifeMax = 50;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.value = 25f;
            npc.buffImmune[BuffID.Poisoned] = true;
            npc.buffImmune[BuffID.Confused] = false;
            npc.netID = NPCID.GreenSlime;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.GetModPlayer<Player>().KingSlimeKilled) 
            {
                return SpawnCondition.OverworldDaySlime.Chance * 1f ;
            }
            else
            {
                return 0;
            }
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life > 0)
            {
                for (int i = 0; i < damage / npc.lifeMax * 100; i++)
                {
                    Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, 192, hitDirection, -1f, 100, new Color(200, 40, 40, 100), 1f);
                    dust.noGravity = true;
                }
                return;
            }
            for (int i = 0; i < 50; i++)
            {
                Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, 192, 2 * hitDirection, -2f, 100, new Color(200, 40, 40, 100), 1f);
                dust.noGravity = true;
            }
        }

        public override void NPCLoot()
        {//ruby Drops!
            if (Main.rand.Next(10) == 1)
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Ruby);
            }
        }

        #region State Values
        //these const ints are for readability of the programmer
        private const int AI_State_Slot = 0;
        private const int AI_Timer_Slot = 1;
        private const int AI_Flutter_Time_Slot = 2;
        private const int AI_Unused_Slot_3 = 3;
        //

        //npc.lcoalAI will also have 4 float variables, using these local class-member variables will have the same effect.
        private const int Local_AI_Unused_Slot_0 = 0;
        private const int Local_AI_Unused_Slot_1 = 1;
        private const int Local_AI_Unused_Slot_2 = 2;
        private const int Local_AI_Unused_Slot_3 = 3;

        //State slot values:
        private const int State_Asleep = 0;
        private const int State_Notice = 1;
        private const int State_Jump = 2;
        private const int State_Hover = 3;
        private const int State_Fall = 4;

        public float AI_State
        {
            get => npc.ai[AI_State_Slot];
            set => npc.ai[AI_State_Slot] = value;
        }
        public float AI_Timer
        {
            get => npc.ai[AI_Timer_Slot];
            set => npc.ai[AI_Timer_Slot] = value;
        }
        public float AI_FlutterTime
        {
            get => npc.ai[AI_Flutter_Time_Slot];
            set => npc.ai[AI_Flutter_Time_Slot] = value;
        }
        #endregion

        //NPC waits for a player to enter range, floats for a few seconds and then jumps to attack 
        //attacking requires fluttering mid-attack
        public override void AI()
        {
            #region Asleep
            if (AI_State == State_Asleep)
            {
                npc.TargetClosest(true);
                //the above line sets the target of the NPC to the closest player, findTarget then returns a 1 or -1 for right or left
                if (npc.HasValidTarget && Main.player[npc.target].Distance(npc.Center) < 500)
                {
                    AI_State = State_Notice;
                    AI_Timer = 0;
                }
            }
            #endregion

            #region PlayerTargeted
            else if (AI_State == State_Notice)
            { //if the targeted player is in attack range
                if (Main.player[npc.target].Distance(npc.Center) < 250f)
                {
                    //counts 20 frames before jump (also used to animate pre-jump crouch)
                    AI_Timer++;
                    if(AI_Timer >= 20)
                    {
                        AI_State = State_Jump;
                        AI_Timer = 0;
                    }
                }
                else
                {
                    npc.TargetClosest(true);
                    if (!npc.HasValidTarget || Main.player[npc.target].Distance(npc.Center) > 500f)
                    {
                        AI_State = State_Asleep;
                        AI_Timer = 0;
                    }
                }
            }
            #endregion

            #region Jumping
            else if (AI_State == State_Jump)
            {
                AI_Timer++;
                if (AI_Timer == 1) 
                {
                    npc.velocity = new Vector2(npc.direction * 2, -10f);
                }
                //40 frames pass by after the initial jump to begin hovering
                else if (AI_Timer > 40)
                {
                    AI_State = State_Hover;
                    AI_Timer = 0;
                }
            }
            #endregion
            
            #region Fluttering
            else if (AI_State == State_Hover)
            {
                AI_Timer++;
                //netCode for multiplayerServers is implemented
                //This is because of the use of Random which is non-deterministic - will run differently based on the NET client
                if (AI_Timer == 1 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    AI_FlutterTime = Main.rand.NextBool() ? 100 : 50;
                    npc.netUpdate = true;
                }
                //adds small velocity (the upwards part)
                npc.velocity += new Vector2(0, -.35f);
                if(Math.Abs(npc.velocity.X) < 2)
                {
                    npc.velocity += new Vector2(npc.direction * 0.5f, 0);
                    //^ used to keep a slight horizontal velocity to create a smoother flutter
                }
                if (AI_Timer > AI_FlutterTime)
                {
                    //initiates after fluttering for 100 frames
                    AI_State = State_Fall;
                    AI_Timer = 0;
                }
            }
            #endregion

            #region Falling
            else if (AI_State == State_Fall)
            {
                if (npc.velocity.Y == 0) 
                {
                    //creates infinite friction when landing, to prevent sliding
                    npc.velocity.X = 0;
                    //defaults to state asleep
                    AI_State = State_Asleep;
                    //resets timer
                    AI_Timer = 0;
                }
            }
            #endregion
        }

        #region Animation Frames
        //readability const
        private const int Frame_Asleep = 0;
        private const int Frame_Notice = 1;
        private const int Frame_Falling = 2;
        private const int Frame_Flutter_1 = 3;
        private const int Frame_Flutter_2 = 4;
        private const int Frame_Flutter_3 = 5;
        #endregion

        //depending on the current MODE of the AI, we need to use Find Frame to find the correct animation frames
        public override void FindFrame(int frameHeight)
        {
            //frameHeight = npc.height
            //flips horizontally to face where necessary
            npc.spriteDirection = npc.direction;

            //for the most part, the animation matches with states, so we will use those
            if (AI_State == State_Asleep)
            {
                npc.frame.Y = Frame_Asleep * frameHeight;
            }
            else if (AI_State == State_Notice)
            {
                if (AI_Timer < 10)
                {
                    npc.frame.Y = Frame_Notice * frameHeight;
                }
                else 
                {//uses Frame Asleep like a crouch frame for 10 in-game frames
                    npc.frame.Y = Frame_Asleep * frameHeight;
                }
            }
            else if (AI_State == State_Jump)
            {
                //we use the same frame for falling as it's rising, it becomes more terraria esque
                npc.frame.Y = Frame_Falling * frameHeight;
            }
            //we don't reset the frame for falling, because it needs to first flutter
            else if (AI_State == State_Hover) 
            {
                npc.frameCounter++;
                if (npc.frameCounter < 10)
                {
                    npc.frame.Y = Frame_Flutter_1 * frameHeight;
                }
                else if (npc.frameCounter < 20)
                {
                    npc.frame.Y = Frame_Flutter_2 * frameHeight;
                }
                else if (npc.frameCounter < 30) 
                {
                    npc.frame.Y = Frame_Flutter_3 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
            //this code is set to reset it after the flutter effect
            else if (AI_State == State_Fall) {
                npc.frame.Y = Frame_Falling * frameHeight;
            }

        }

    }
}
