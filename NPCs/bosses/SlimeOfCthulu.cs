using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Slime.NPCs.bosses
{
    class SlimeOfCthulu : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slime Of Cthulu");
        }
        public override void SetDefaults()
        {
            npc.boss = true;
            npc.width = 160;
            npc.height = 160;
            npc.aiStyle = -2; //-2 == unique ai style
            npc.damage = 15;
            npc.defense = 5;
            npc.knockBackResist = 20f;
            npc.lifeMax = 5000;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.value = 25f;
            npc.buffImmune[BuffID.Poisoned] = true;
            npc.buffImmune[BuffID.Confused] = false;
            npc.netID = NPCID.KingSlime;
            npc.noGravity = true;
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

        #region State values
        private const int AI_State_Slot = 0;
        private const int AI_Timer_Slot = 1;

        private const int State_Asleep = 0;
        private const int State_Neutral = 1;
        private const int State_Launch = 2;
        private const int State_Spin = 3;
        private const int State_Agressive_Launch = 4;
        private bool Has_Spun = false;

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
        #endregion
        public override void AI()
        {
            #region Player Search

            if (AI_State == State_Asleep)
            {
                npc.TargetClosest(true);
                AI_Timer++;

                if (AI_Timer > 20)
                {
                    AI_Timer = 0;
                    if (npc.life <= npc.lifeMax / 2 && Has_Spun == false)
                    {
                        AI_Timer = 0;
                        AI_State = State_Spin;
                    }
                    else if (Has_Spun == false)
                    {
                        AI_State = State_Launch;
                    }
                    else
                    {
                        AI_Timer = 0;
                        AI_State = State_Agressive_Launch;
                    }

                }
            }
            #endregion
            #region Phase-1-launch
            else if (AI_State == State_Launch)
            {
                Vector2 moveTo = new Vector2(npc.targetRect.X, npc.targetRect.Y);
                float speed = 20f; //make this whatever you want
                Vector2 move = moveTo - npc.Center; //this is how much your boss wants to move
                float magnitude = (float)Math.Sqrt(move.X * move.X + move.Y * move.Y); //fun with the Pythagorean Theorem
                move.X *= speed / magnitude; //this adjusts your boss's speed so that its speed is always constant
                move.Y *= speed / magnitude;
                npc.velocity = move;
                AI_Timer = 0;
                AI_State = State_Neutral;
            }
            #endregion
            #region Neutral
            else if (AI_State == State_Neutral)
            {
                npc.velocity *= 0.95f;
                AI_Timer++;
                if (AI_Timer > 60)
                {
                    AI_State = State_Asleep;
                }
            }
            #endregion
            #region Agressive Launch
            else if (AI_State == State_Agressive_Launch)
            {
                AI_Timer++;
                npc.velocity *= 0.95f;
                if (AI_Timer == 1)
                {
                    Vector2 moveTo = new Vector2(npc.targetRect.X, npc.targetRect.Y);
                    float speed = 20f; //make this whatever you want
                    Vector2 move = moveTo - npc.Center; //this is how much your boss wants to move
                    float magnitude = (float)Math.Sqrt(move.X * move.X + move.Y * move.Y); //fun with the Pythagorean Theorem
                    move.X *= speed / magnitude; //this adjusts your boss's speed so that its speed is always constant
                    move.Y *= speed / magnitude;
                    npc.velocity = move;
                }
                else if (AI_Timer == 10)
                {
                    Vector2 moveTo = new Vector2(npc.targetRect.X, npc.targetRect.Y);
                    float speed = 30f; //make this whatever you want
                    Vector2 move = moveTo - npc.Center; //this is how much your boss wants to move
                    float magnitude = (float)Math.Sqrt(move.X * move.X + move.Y * move.Y); //fun with the Pythagorean Theorem
                    move.X *= speed / magnitude; //this adjusts your boss's speed so that its speed is always constant
                    move.Y *= speed / magnitude;
                    npc.velocity = move;
                }
                else if (AI_Timer == 20)
                {
                    Vector2 moveTo = new Vector2(npc.targetRect.X, npc.targetRect.Y);
                    float speed = 40f; //make this whatever you want
                    Vector2 move = moveTo - npc.Center; //this is how much your boss wants to move
                    float magnitude = (float)Math.Sqrt(move.X * move.X + move.Y * move.Y); //fun with the Pythagorean Theorem
                    move.X *= speed / magnitude; //this adjusts your boss's speed so that its speed is always constant
                    move.Y *= speed / magnitude;
                    npc.velocity = move;
                    AI_State = State_Neutral;
                }
            }
            #endregion
            #region Spin
            else if (AI_State == State_Spin)
            {
                AI_Timer++;
                npc.rotation += AI_Timer * 0.5f;
                if (AI_Timer >= 120)
                {
                    Has_Spun = true;
                    AI_State = State_Asleep;
                }
            }
            #endregion
        }
        public override void FindFrame(int frameHeight)
        {
            npc.spriteDirection = -npc.direction;
            double direction = Math.Asin((npc.velocity.Y / Math.Sqrt(npc.velocity.X * npc.velocity.X + npc.velocity.Y * npc.velocity.Y)));
            npc.rotation = (float)direction;
        }

    }
}
