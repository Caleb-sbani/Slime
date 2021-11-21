using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.IO;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

namespace Slime
{
    class Player : ModPlayer
    {
        public const int maxSlimeKills = 9000;
        public int SlimeKills;
        public bool KingSlimeKilled = false;
        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet1 = mod.GetPacket();
            packet1.Write(SlimeKills);
            packet1.Send(toWho, fromWho);
            ModPacket packet2 = mod.GetPacket();
            packet2.Write(KingSlimeKilled);
            packet2.Send(toWho, fromWho);

        }
        public override TagCompound Save()
        {
            return new TagCompound
            {
                { "SlimeKills", SlimeKills },
                { "KingSlimeKilled", KingSlimeKilled}
            };
        }
        public override void Load(TagCompound tag)
        {
            SlimeKills = tag.GetInt("SlimeKills");
            KingSlimeKilled = tag.GetBool("KingSlimeKilled");
        }
        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if (target.netID == (NPCID.BlueSlime) || target.netID == NPCID.GreenSlime) {
                if (damage >= target.life) {
                    SlimeKills += 1;
                }
            }
            if (target.netID == NPCID.KingSlime)
            {
                if (damage >= target.life)
                {
                    SlimeKills += 90;
                    KingSlimeKilled = true;
                }
            }
            if(SlimeKills >= maxSlimeKills)
            {
                SlimeKills = maxSlimeKills;
            }
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (target.netID == (NPCID.BlueSlime) || target.netID == NPCID.GreenSlime)
            {
                if (damage >= target.life)
                {
                    SlimeKills += 1;
                }
            }
            if (target.netID == NPCID.KingSlime)
            {
                if (damage >= target.life)
                {
                    SlimeKills += 90;
                    KingSlimeKilled = true;
                }
            }
        }
    }
}
