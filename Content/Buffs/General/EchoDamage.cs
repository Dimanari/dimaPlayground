using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace dimaPlayground.Content.Buffs.General
{
    public class MinionEchoDamage : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hurt Me More");
            Description.SetDefault("Taking increased damage from minions.");
            Main.debuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
        }
        static public void ActivateEcho(Projectile projectile, NPC target, int damage)
        {
            bool summon = projectile.minion || ProjectileID.Sets.MinionShot[projectile.type] || ProjectileID.Sets.SentryShot[projectile.type] || projectile.sentry;
            if (summon && target.HasBuff(ModContent.BuffType<MinionEchoDamage>()))
            {
                int myDamage = (int)(damage * 4.5f);
                //apply damage
                target.StrikeNPC(myDamage, 0f, 0, false);

                //consume dubuff
                var index = target.FindBuffIndex(ModContent.BuffType<MinionEchoDamage>());
                target.DelBuff(index);
            }
        }
    }
}
