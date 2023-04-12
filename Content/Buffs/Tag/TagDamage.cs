using dimaPlayground.Content.Globals;
using Terraria;
using Terraria.ModLoader;

namespace dimaPlayground.Content.Buffs.Tag
{
    public class BasicTagDamage : ModBuff
    {
        public int mTagDamage;
        public override void SetStaticDefaults()
        {
            mTagDamage = 0;
            DisplayName.SetDefault("Tag Damage");
            Description.SetDefault("Taking increased damage from minions.");
            Main.debuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<GNPC>().summonTag += mTagDamage;
        }
    }
    public class WeakTagDamage : BasicTagDamage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            mTagDamage = 3;
        }
    }

    public class StrongTagDamage : BasicTagDamage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            mTagDamage = 30;
        }
    }
}