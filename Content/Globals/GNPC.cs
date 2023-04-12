using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace dimaPlayground.Content.Globals
{
    public class GNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public int summonTag;


        public override void ResetEffects(NPC npc)
        {
            summonTag = 0;
        }

        public override void ModifyHitByProjectile(NPC target, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            bool summon = (projectile.minion || ProjectileID.Sets.MinionShot[projectile.type] || ProjectileID.Sets.SentryShot[projectile.type] || projectile.sentry);
            if (summon)
                damage += summonTag;
        }
    }
}