using dimaPlayground.Content.Buffs.General;
using dimaPlayground.Content.Buffs.Special;
using Terraria;
using Terraria.ModLoader;

namespace dimaPlayground.Content.Globals
{
    public class GProjectile : GlobalProjectile
    {
        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            MinionEchoDamage.ActivateEcho(projectile, target, damage);
            MinionHealCoolDown.ActivateHeal(projectile, damage);
        }
    }
}