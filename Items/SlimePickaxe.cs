using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;
using Slime;

namespace Slime.Items
{
    class SlimePickaxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slime Pickaxe");
            Tooltip.SetDefault("Ewww.... Sticky");
        }
        public override void SetDefaults()
        {
            item.damage = 10;
            item.useTime = 20;
            item.useAnimation = 13;
            item.pick = 65;
            item.autoReuse = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
        }
        public override void ModifyWeaponDamage(Terraria.Player player, ref float add, ref float mult)
        {
            item.useTime = 20 - player.GetModPlayer<Player>().SlimeKills/450;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Gel, 15);
            recipe.AddIngredient(ItemID.GoldBar, 10);
            recipe.AddIngredient(ItemID.Ruby, 1);
            recipe.AddTile(TileID.Solidifier);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
