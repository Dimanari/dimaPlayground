using Terraria.ModLoader;

namespace dimaPlayground.Content.Globals
{
    public class GPlayer : ModPlayer
    {
        public int minionlifesteal;
        public float minionlifestealScale;
        public bool watcher_minion;
        public bool wrath_minion;
        public override void UpdateEquips()
        {
            minionlifesteal = 0;
            minionlifestealScale = 0.0f;
        }

        public override void Initialize()
        {
            minionlifesteal = 0;
            minionlifestealScale = 0.0f;
            watcher_minion = false;
            wrath_minion = false;
        }
    }
}