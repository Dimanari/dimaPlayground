using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using dimaPlayground.Content.Projectiles.UltraWhipProjectile;

namespace dimaPlayground.Content.Items.Weapons.UltraWhip
{
    public class UltraWhip : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            Tooltip.SetDefault("30 summon tag damage\nMinion Strike will echo significant damage\nYour summons will focus struck enemies\n\'Ultimate destruction\'");
            DisplayName.SetDefault("Ultra Whip");
        }

        public override void SetDefaults()
        {
            // This method quickly sets the whip's properties.
            // Mouse over to see its parameters.
            Item.DefaultToWhip(ModContent.ProjectileType<UltraWhipProjectile>(), 100, 1f, 10f);

            Item.rare = ItemRarityID.Red;

            Item.channel = false; // example whip's channel is not vanilla+
        }
        public override bool? CanAutoReuseItem(Player player)
        {
            return player.autoReuseGlove || Item.autoReuse;
        }

        public override void AddRecipes()
        {
            //The recipe
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddIngredient(ItemID.FragmentStardust, 20);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();

            recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DirtBlock, 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}