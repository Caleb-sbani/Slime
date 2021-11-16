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
        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = mod.GetPacket();
            packet.Write(SlimeKills);
            packet.Send(toWho, fromWho);
        }
        public override TagCompound Save()
        {
            return new TagCompound
            {
                { "SlimeKills", SlimeKills }
            };
        }
        public override void Load(TagCompound tag)
        {
            SlimeKills = tag.GetInt("SlimeKills");
        }
        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if (target.netID == (NPCID.BlueSlime) || target.netID == NPCID.GreenSlime) {
                if (damage >= target.life) {
                    SlimeKills += 1;
                }
            }
        }
    }
}
