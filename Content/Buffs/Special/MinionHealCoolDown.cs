using dimaPlayground.Content.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace dimaPlayground.Content.Buffs.Special
{
    public class MinionHealCoolDown : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Minion Regen Cooldown");
            Description.SetDefault("Cannot heal using minions.");
            Main.debuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
        }

        public static void ActivateHeal(Projectile projectile, int Damage)
        {
            bool summon = projectile.minion || ProjectileID.Sets.MinionShot[projectile.type] || ProjectileID.Sets.SentryShot[projectile.type] || projectile.sentry;

            Player p = Main.player[projectile.owner];
            if (summon && p.GetModPlayer<GPlayer>().minionlifesteal > 0)
            {
                if (!p.HasBuff<MinionHealCoolDown>())
                {
                    int HealAmount = (int)(Damage * p.GetModPlayer<GPlayer>().minionlifestealScale) + p.GetModPlayer<GPlayer>().minionlifesteal;
                    p.HealEffect(HealAmount);
                    p.statLife += HealAmount;
                    p.AddBuff(ModContent.BuffType<MinionHealCoolDown>(), 20);
                }
            }
        }
    }
}