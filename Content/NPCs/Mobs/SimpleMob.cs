using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace dimaPlayground.Content.NPCs.Mobs
{
    class SimpleMob : ModNPC
    {
        private enum ActionState
        {
            Asleep,
            Notice,
            Jump,
            Hover,
            Fall,
            Falling
        }
        private enum Frame
        {
            Asleep,
            Notice,
            Falling,
            Flutter1,
            Flutter2,
            Flutter3
        }
        public ref float AI_State => ref NPC.ai[0];
        public ref float AI_Timer => ref NPC.ai[1];
        public ref float AI_FlutterTime => ref NPC.ai[2];

        protected float hover_speed;
        protected float hover_acc;
        protected float lookup_range;
        static private float RageLookupRange = 10000f;
        protected float attack_range;
        protected int windup_time;
        protected bool isEnraged;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Slime"); // Automatic from localization files
            Main.npcFrameCount[NPC.type] = 6; // make sure to set this for your modnpcs.

            // Specify the debuffs it is immune to
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
					//BuffID.Poisoned // This NPC will be immune to the Poisoned debuff.
				}
            });
        }

        public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit)
        {
            base.OnHitByItem(player, item, damage, knockback, crit);
            isEnraged = true;
            lookup_range = RageLookupRange;
        }

        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
            base.OnHitByProjectile(projectile, damage, knockback, crit);
            isEnraged = true;
            lookup_range = RageLookupRange;
        }

        public override void SetDefaults()
        {
            NPC.width = 36; // The width of the npc's hitbox (in pixels)
            NPC.height = 36; // The height of the npc's hitbox (in pixels)
            NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
            NPC.damage = 17; // The amount of damage that this npc deals
            NPC.defense = 5; // The amount of defense that this npc has
            NPC.lifeMax = 100; // The amount of health that this npc has
            NPC.HitSound = SoundID.NPCHit1; // The sound the NPC will make when being hit.
            NPC.DeathSound = SoundID.NPCDeath1; // The sound the NPC will make when it dies.
            NPC.value = 25f; // How many copper coins the NPC will drop when killed.
            NPC.knockBackResist = 0.1f;
            isEnraged = false;
            hover_speed = 20f;
            hover_acc = .05f;
            lookup_range = 500f;
            attack_range = 250f;
            windup_time = 10;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            // we would like this npc to spawn in the overworld.
            float mult = 0.1f;

            if (NPC.downedQueenSlime)
            {
                mult *= 4f;
            }
            else if (NPC.downedSlimeKing)
            {
                mult *= 3f;
            }

            if (!Main.dayTime || spawnInfo.SpawnTileY > Main.worldSurface || spawnInfo.PlayerInTown)
                return 0.0f;

            if (spawnInfo.Player.ZoneForest || spawnInfo.Player.ZonePurity)
                return mult;
            if (spawnInfo.Player.ZoneJungle)
                return mult * 0.5f;
            if (spawnInfo.Player.ZoneHallow)
                return mult * 0.3f;
            return mult * 0.2f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
            npcLoot.Add(ItemDropRule.Common(ItemID.Gel, 1, 1, 2));
            npcLoot.Add(ItemDropRule.Common(ItemID.SlimeStaff, 50));
            npcLoot.Add(ItemDropRule.Coins(1000, false));
        }
        // Our AI here makes our NPC sit waiting for a player to enter range, jumps to attack, flutter mid-fall to stay afloat a little longer, then falls to the ground. Note that animation should happen in FindFrame
        public override void AI()
        {
            // The npc starts in the asleep state, waiting for a player to enter range
            switch (AI_State)
            {
                case (float)ActionState.Asleep:
                    FallAsleep();
                    break;
                case (float)ActionState.Notice:
                    Notice();
                    break;
                case (float)ActionState.Jump:
                    Jump();
                    break;
                case (float)ActionState.Hover:
                    Hover();
                    break;
                case (float)ActionState.Fall:
                    Fall();
                    if (AI_Timer == windup_time)
                    {
                        AI_State = (float)ActionState.Falling;
                    }
                    break;
                case (float)ActionState.Falling:
                    Fall();
                    if (NPC.velocity.Y == 0)
                    {
                        NPC.velocity.X = 0;
                        AI_State = (float)ActionState.Asleep;
                        AI_Timer = 0;
                    }

                    break;
            }
        }

        // Here in FindFrame, we want to set the animation frame our npc will use depending on what it is doing.
        // We set npc.frame.Y to x * frameHeight where x is the xth frame in our spritesheet, counting from 0. For convenience, we have defined a enum above.
        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            NPC.spriteDirection = NPC.direction;

            // For the most part, our animation matches up with our states.
            switch (AI_State)
            {
                case (float)ActionState.Asleep:
                    // npc.frame.Y is the goto way of changing animation frames. npc.frame starts from the top left corner in pixel coordinates, so keep that in mind.
                    NPC.frame.Y = (int)Frame.Asleep * frameHeight;
                    break;
                case (float)ActionState.Notice:
                    // Going from Notice to Asleep makes our npc look like it's crouching to jump.
                    if (AI_Timer < 10)
                    {
                        NPC.frame.Y = (int)Frame.Notice * frameHeight;
                    }
                    else
                    {
                        NPC.frame.Y = (int)Frame.Asleep * frameHeight;
                    }

                    break;
                case (float)ActionState.Jump:
                    NPC.frame.Y = (int)Frame.Falling * frameHeight;
                    break;
                case (float)ActionState.Hover:
                    // Here we have 3 frames that we want to cycle through.
                    NPC.frameCounter++;

                    if (NPC.frameCounter < 10)
                    {
                        NPC.frame.Y = (int)Frame.Flutter1 * frameHeight;
                    }
                    else if (NPC.frameCounter < 20)
                    {
                        NPC.frame.Y = (int)Frame.Flutter2 * frameHeight;
                    }
                    else if (NPC.frameCounter < 30)
                    {
                        NPC.frame.Y = (int)Frame.Flutter3 * frameHeight;
                    }
                    else
                    {
                        NPC.frameCounter = 0;
                    }

                    break;
                case (float)ActionState.Fall:
                case (float)ActionState.Falling:
                    NPC.frame.Y = (int)Frame.Falling * frameHeight;
                    break;
            }
        }

        // Here, because we use custom AI (aiStyle not set to a suitable vanilla value), we should manually decide when Flutter Slime can fall through platforms
        public override bool? CanFallThroughPlatforms()
        {
            if (AI_State == (float)ActionState.Falling && NPC.HasValidTarget && Main.player[NPC.target].Top.Y > NPC.Bottom.Y)
            {
                // If Flutter Slime is currently falling, we want it to keep falling through platforms as long as it's above the player
                return true;
            }

            return false;
            // You could also return null here to apply vanilla behavior (which is the same as false for custom AI)
        }

        private void FallAsleep()
        {
            // TargetClosest sets npc.target to the player.whoAmI of the closest player.
            // The faceTarget parameter means that npc.direction will automatically be 1 or -1 if the targeted player is to the right or left.
            // This is also automatically flipped if npc.confused.
            NPC.TargetClosest(true);

            if (NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) < lookup_range)
            {
                // Since we have a target in range, we change to the Notice state. (and zero out the Timer for good measure)
                AI_State = (float)ActionState.Notice;
                AI_Timer = 0;
                lookup_range = 1000f;
                if(isEnraged)
                {
                    lookup_range = RageLookupRange;
                }
            }
        }

        private void Notice()
        {
            if (Main.player[NPC.target].Distance(NPC.Center) < attack_range || isEnraged)
            {
                // Here we use our Timer to wait .33 seconds before actually jumping. In FindFrame you'll notice AI_Timer also being used to animate the pre-jump crouch
                AI_Timer++;
                attack_range = 500f;
                if (isEnraged)
                {
                    lookup_range = RageLookupRange;
                }
                if (AI_Timer >= 20)
                {
                    AI_State = (float)ActionState.Jump;
                    AI_Timer = 0;
                }
            }
            else
            {
                NPC.TargetClosest(true);

                if (!NPC.HasValidTarget || Main.player[NPC.target].Distance(NPC.Center) > lookup_range)
                {
                    // Out targeted player seems to have left our range, so we'll go back to sleep.
                    AI_State = (float)ActionState.Asleep;
                    AI_Timer = 0;
                }
            }
        }

        private void Jump()
        {
            AI_Timer++;

            if (AI_Timer == 1)
            {
                // We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
                NPC.velocity = new Vector2(NPC.direction * 2, -10f);
            }
            else if (AI_Timer > 40)
            {
                // after .66 seconds, we go to the hover state. //TODO, gravity?
                AI_State = (float)ActionState.Hover;
                AI_Timer = 0;
            }
        }

        private void Hover()
        {
            AI_Timer++;

            // Here we make a decision on how long this flutter will last. We check netmode != 1 to prevent Multiplayer Clients from running this code. (similarly, spawning projectiles should also be wrapped like this)
            // netMode == 0 is SP, netMode == 1 is MP Client, netMode == 2 is MP Server.
            // Typically in MP, Client and Server maintain the same state by running deterministic code individually. When we want to do something random, we must do that on the server and then inform MP Clients.
            if (AI_Timer == 1 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                // For reference: without proper syncing: https://gfycat.com/BackAnxiousFerret and with proper syncing: https://gfycat.com/TatteredKindlyDalmatian
                AI_FlutterTime = Main.rand.NextBool() ? 100 : 50;

                // Informing MP Clients is done automatically by syncing the npc.ai array over the network whenever npc.netUpdate is set.
                // Don't set netUpdate unless you do something non-deterministic ("random")
                NPC.netUpdate = true;
            }

            // Here we add a tiny bit of upward velocity to our npc.
            NPC.velocity += new Vector2(0, -.35f);

            // ... and some additional X velocity when traveling slow.
            if (Math.Abs(NPC.velocity.X) < hover_speed)
            {
                NPC.velocity += new Vector2(NPC.direction * hover_acc, 0);
            }
            bool IsOnTopOfTarget = (int)Math.Abs(Main.player[NPC.target].position.X - NPC.position.X) < 10f;
            // after fluttering for 100 ticks (1.66 seconds), our Flutter Slime is tired, so he decides to go into the Fall state.
            if (AI_Timer > AI_FlutterTime || IsOnTopOfTarget)
            {
                AI_State = (float)ActionState.Fall;
                AI_Timer = 0;
            }
        }
        private void Fall()
        {
            if (AI_Timer < windup_time)
            {
                //Freeze in air for a little while
                AI_Timer++;
                NPC.velocity = new Vector2(0, 0);
            }
            else if (AI_Timer == windup_time)
            {
                //fall down super fast!
                AI_Timer++;
                NPC.velocity += new Vector2(Main.player[NPC.target].velocity.X, 5f);
            }
        }
    }
}