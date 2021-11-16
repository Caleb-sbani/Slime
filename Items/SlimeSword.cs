using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Slime;

namespace Slime.Items
{
    class SlimeSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slime Sword");
            Tooltip.SetDefault("Does more damage the more slimes you kills");
        }
        public override void SetDefaults()
        {
            item.damage = 20;
            item.crit = 15;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
        }
        public override void ModifyWeaponDamage(Terraria.Player player, ref float add, ref float mult)
        {
            item.damage = 20 + player.GetModPlayer<Player>().SlimeKills / 90;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Gel, 50);
            recipe.AddIngredient(ItemID.Ruby, 4);
            recipe.AddIngredient(ItemID.Wood, 10);
            recipe.AddTile(TileID.Solidifier);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
