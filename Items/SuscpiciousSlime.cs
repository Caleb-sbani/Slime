using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Slime;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Slime.NPCs.bosses;

namespace Slime.Items
{
    class SuscpiciousSlime : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Suspicious Looking Slime");
            Tooltip.SetDefault("Summons the Slime King's avenger");
        }
        public override void SetDefaults()
        {
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 30;
            item.useAnimation = 20;
        }
        public override bool UseItem(Terraria.Player player)
        {
            NPC.SpawnOnPlayer(Main.myPlayer, ModContent.NPCType<SlimeOfCthulu>());
            return true;
        }
    }
}
