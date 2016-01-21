using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Units;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Common.Audio;

namespace ZeldaOracle.Game.Entities.Monsters {
	
	//-----------------------------------------------------------------------------
	// Interaction Type
	//-----------------------------------------------------------------------------

	public enum InteractionType {
		None = -1,

		EmberSeed = 0,			// Hit by an ember seed.
		ScentSeed,				// Hit by a scent seed.
		PegasusSeed,			// Hit by a pegasus seed.
		GaleSeed,				// Hit by a gale seed.
		MysterySeed,			// Hit by a mystery seed.
		Fire,					// Touches fire.
		Gale,					// Touches gale.
		Arrow,					// Hit by an arrow.
		SwordBeam,				// Hit by a sword beam projectile.
		RodFire,				// Hit by a projectile from the fire-rod.
		Sword,					// Hit by a sword.
		SwordSpin,				// Hit by a spinning sword.
		BiggoronSword,			// Hit by a biggoron sword.
		Boomerang,				// Hit by a boomerang.
		BombExplosion,			// Hit by a bomb explosion.
		Shield,					// Hit by a shield.
		SwitchHook,				// Hit by the switch hook.
		Shovel,					// Hit by a shovel being used.
		Pickup,					// Attempt to use the bracelet to pickup.
		ButtonAction,			// The A button is pressed while colliding.
		
		Parry,
		PlayerContact,

		ThrownObject,			// Hit by a thrown object (thrown tiles, not bombs).
		MineCart,				// Hit by a minecart.
		Block,					// Hit by a block (either moving or spawned on top of).

		// UNUSED:
		SwordHitShield,			// Their sword hits my shield.
		BiggoronSwordHitShield,	// Their biggoron sword hits my shield.
		ShieldHitShield,		// Their shield hits my shield.

		Count,
	};

		
	//-----------------------------------------------------------------------------
	// Interaction Event Arguments
	//-----------------------------------------------------------------------------
	
	public class InteractionArgs : EventArgs {
        public Vector2F ContactPoint { get; set; }
	}

	public class WeaponInteractionEventArgs : EventArgs {
        public ItemWeapon Weapon { get; set; }
	}
	
	public class ParryInteractionArgs : InteractionArgs {
        public UnitTool SenderTool { get; set; }
        public UnitTool MonsterTool { get; set; }
	}


	public partial class Monster : Unit {

		public static InteractionType GetSeedInteractionType(SeedType seedType) {
			return (InteractionType) ((int) InteractionType.EmberSeed + (int) seedType);
		}

		
		//-----------------------------------------------------------------------------
		// Interaction Delegates
		//-----------------------------------------------------------------------------

		public delegate void InteractionMemberDelegate(Entity sender, EventArgs args);

		public delegate void InteractionStaticDelegate(Monster monster, Entity sender, EventArgs args);
		
		private static InteractionStaticDelegate ToStaticInteractionDelegate(InteractionMemberDelegate memberDelegate) {
			return delegate(Monster monster, Entity sender, EventArgs args) {
				memberDelegate.Invoke(sender, args);
			};
		}
		

		//-----------------------------------------------------------------------------
		// Interaction Handler
		//-----------------------------------------------------------------------------

		public class InteractionHandler {
			private InteractionStaticDelegate handler;
			
			public InteractionHandler Clear() {
				handler = null;
				return this;
			}
			
			public InteractionHandler Set(InteractionMemberDelegate reaction) {
				handler = ToStaticInteractionDelegate(reaction);
				return this;
			}
			
			public InteractionHandler Set(InteractionStaticDelegate reaction) {
				handler = reaction;
				return this;
			}
			
			public InteractionHandler Add(InteractionMemberDelegate reaction) {
				return Add(ToStaticInteractionDelegate(reaction));
			}
			
			public InteractionHandler Add(InteractionStaticDelegate reaction) {
				if (handler == null)
					handler = reaction;
				else
					handler += reaction;
				return this;
			}

			public void Trigger(Monster monster, Entity sender, EventArgs args) {
				if (handler != null)
					handler.Invoke(monster, sender, args);
			}
		}


		//-----------------------------------------------------------------------------
		// Monster Reactions
		//-----------------------------------------------------------------------------

		public static class Reactions {
			
			
			//-----------------------------------------------------------------------------
			// Basic Monster Reactions
			//-----------------------------------------------------------------------------

			// Do nothing.
			public const InteractionStaticDelegate None = null;

			// Instantly kill the monster.
			public static void Kill(Monster monster, Entity sender, EventArgs args) {
				monster.Kill();
			}
			
			// Instantly kill the monster.
			public static void SoftKill(Monster monster, Entity sender, EventArgs args) {
				monster.Kill(); // TODO: Soft Kill
			}

			// Damage the monster for 1 damage.
			public static void Damage(Monster monster, Entity sender, EventArgs args) {
				monster.Hurt(1, sender.Center);
			}
			
			// Damage the monster for 2 damage.
			public static void Damage2(Monster monster, Entity sender, EventArgs args) {
				monster.Hurt(2, sender.Center);
			}
			
			// Damage the monster for 3 damage.
			public static void Damage3(Monster monster, Entity sender, EventArgs args) {
				monster.Hurt(3, sender.Center);
			}
			
			// Damage the monster for 4 damage.
			public static void Damage4(Monster monster, Entity sender, EventArgs args) {
				monster.Hurt(4, sender.Center);
			}

			// Bump the monster.
			public static void Bump(Monster monster, Entity sender, EventArgs args) {
				AudioSystem.PlaySound(GameData.SOUND_BOMB_BOUNCE);
				monster.Bump(sender.Center, true);
			}

			// Bump the monster and sender.
			public static void Parry(Monster monster, Entity sender, EventArgs args) {
				monster.Bump(sender.Center, false);
				if (sender is Unit)
					(sender as Unit).Bump(monster.Center, false);
				AudioSystem.PlaySound(GameData.SOUND_EFFECT_CLING);

				AudioSystem.PlaySound(GameData.SOUND_EFFECT_CLING);
			}
			
			// Burn the monster for 1 damage.
			public static void Burn(Monster monster, Entity sender, EventArgs args) {
				monster.Burn(1);
				if (sender is Fire)
					sender.Destroy();
			}
			
			// Stun the monster.
			public static void Stun(Monster monster, Entity sender, EventArgs args) {
				monster.Stun();
			}
			
			// Send the monster in a gale.
			public static void Gale(Monster monster, Entity sender, EventArgs args) {
				monster.EnterGale((EffectGale) sender);
			}
			
			// Trigger a random seed effect.
			public static void MysterySeed(Monster monster, Entity sender, EventArgs args) {
				// Random: burn, stun, damage, gale
				int rand = GRandom.NextInt(4);
				SeedEntity seed = (SeedEntity) sender;

				if (rand == 0)
					seed.SeedType = SeedType.Ember;
				else if (rand == 1)
					seed.SeedType = SeedType.Scent;
				else if (rand == 2)
					seed.SeedType = SeedType.Pegasus;
				else
					seed.SeedType = SeedType.Gale;

				monster.TriggerInteraction(Monster.GetSeedInteractionType(seed.SeedType), sender, args);
			}
			
			// Switch places with the monster (Only for switch-hook interactions).
			public static void SwitchHook(Monster monster, Entity sender, EventArgs args) {
				SwitchHookProjectile hook = sender as SwitchHookProjectile;
				hook.SwitchWithEntity(monster);
			}
			
			// Stun the monster.
			public static void ClingEffect(Monster monster, Entity sender, EventArgs args) {
				Effect effect = new Effect(GameData.ANIM_EFFECT_CLING, DepthLayer.EffectCling);
				
				Vector2F effectPos = (monster.Center + sender.Center) * 0.5f;
				if (args is InteractionArgs)
					effectPos = (args as InteractionArgs).ContactPoint;

				monster.RoomControl.SpawnEntity(effect, effectPos);
				AudioSystem.PlaySound(GameData.SOUND_EFFECT_CLING);
			}
			

			//-----------------------------------------------------------------------------
			// Customizable Monster Reactions
			//-----------------------------------------------------------------------------

			// Damage the monster for the given amount.
			public static InteractionStaticDelegate Damage(int amount) {
				return delegate(Monster monster, Entity sender, EventArgs args) {
					monster.Hurt(amount, sender.Center);
				};
			}

			// Damage the monster for the given amount based on the used item's level.
			public static InteractionStaticDelegate DamageByLevel(int amountLevel1, int amountLevel2, int amountLevel3) {
				return delegate(Monster monster, Entity sender, EventArgs args) {
					int level = (args as WeaponInteractionEventArgs).Weapon.Level;
					int amount = 0;
					if (level == Item.Level1)
						amount = amountLevel1;
					else if (level == Item.Level2)
						amount = amountLevel2;
					else if (level == Item.Level3)
						amount = amountLevel3;
					monster.Hurt(amount, sender.Center);
				};
			}

			public static InteractionStaticDelegate ContactEffect(Effect effect) {
				return delegate(Monster monster, Entity sender, EventArgs args) {
					Effect clonedEffect = effect.Clone();
					InteractionArgs interactionArgs = args as InteractionArgs;
					monster.RoomControl.SpawnEntity(clonedEffect, interactionArgs.ContactPoint);
				};
			}
			

			//-----------------------------------------------------------------------------
			// Conditions
			//-----------------------------------------------------------------------------
			/*
			// Damage the monster for the given amount based on the used item's level.
			public static InteractionDelegate IfLevel(int level) {
				return delegate(Monster monster) {

				};
			}*/
			
		}
		
			
		//-----------------------------------------------------------------------------
		// Sender Reactions
		//-----------------------------------------------------------------------------

		public static class SenderReactions {

			// Do nothing.
			public const Delegate None = null;

			
			//-----------------------------------------------------------------------------
			// Basic Sender Reactions
			//-----------------------------------------------------------------------------

			public static void Destroy(Monster monster, Entity sender, EventArgs args) {
				sender.Destroy();
			}

			public static void Intercept(Monster monster, Entity sender, EventArgs args) {
				if (sender is IInterceptable)
					(sender as IInterceptable).Intercept();
			}
			
			public static void Bump(Monster monster, Entity sender, EventArgs args) {
				if (sender is Unit)
					(sender as Unit).Bump(monster.Center, true);
			}
			
			public static void Damage(Monster monster, Entity sender, EventArgs args) {
				if (sender is Unit)
					(sender as Unit).Hurt(1, monster.Center);
			}

			public static void Kill(Monster monster, Entity sender, EventArgs args) {
				if (sender is Unit)
					(sender as Unit).Kill();
				else
					sender.Destroy();
			}

			//-----------------------------------------------------------------------------
			// Customizable Sender Reactions
			//-----------------------------------------------------------------------------

		}
	}
}
