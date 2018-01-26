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
using ZeldaOracle.Game.Entities.Monsters.States;
using ZeldaOracle.Game.Entities.Projectiles.Seeds;
using ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles;

namespace ZeldaOracle.Game.Entities.Monsters {
	
	//-----------------------------------------------------------------------------
	// Interaction Type
	//-----------------------------------------------------------------------------

	public enum InteractionType {
		None = -1,
		
		
		//-----------------------------------------------------------------------------
		// Weapon Interactions
		//-----------------------------------------------------------------------------

		/// <summary>Hit by a sword.
		/// <para/>Default = SenderReactions.Intercept,
		/// Reactions.DamageByLevel(1, 2, 3)</summary>
		Sword,
		/// <summary>Hit by a spinning sword.
		/// <para/>Default = Reactions.Damage2</summary>
		SwordSpin,
		/// <summary>Hit by a biggoron sword.
		/// <para/>Default = Reactions.Damage3</summary>
		BiggoronSword,
		/// <summary>Hit by a shield.
		/// <para/>Default = SenderReactions.Bump, Reactions.Bump</summary>
		Shield,
		/// <summary>Hit by a shovel being used.
		/// <para/>Default = Reactions.Bump</summary>
		Shovel,
		/// <summary>TODO.
		/// <para/>Default = Reactions.None</summary>
		Parry,
		/// <summary>Attempt to use the bracelet to pickup.
		/// <para/>Default = Reactions.None</summary>
		Pickup,
		
		//-----------------------------------------------------------------------------
		// Seed Interactions
		//-----------------------------------------------------------------------------

		/// <summary>Hit by an ember seed.
		/// <para/>Default = SenderReactions.Intercept</summary>
		EmberSeed = 0,
		/// <summary>Hit by a scent seed.
		/// <para/>Default = SenderReactions.Intercept, Reactions.Damage</summary>
		ScentSeed,
		/// <summary>Hit by a pegasus seed.
		/// <para/>Default = SenderReactions.Intercept, Reactions.Stun</summary>
		PegasusSeed,
		/// <summary>Hit by a gale seed.
		/// <para/>Default = SenderReactions.Intercept</summary>
		GaleSeed,
		/// <summary>Hit by a mystery seed.
		/// <para/>Default = Reactions.MysterySeed</summary>
		MysterySeed,
		
		
		//-----------------------------------------------------------------------------
		// Projectile Interactions
		//-----------------------------------------------------------------------------

		/// <summary>Hit by an arrow.
		/// <para/>Default = SenderReactions.Intercept, Reactions.Damage</summary>
		Arrow,
		/// <summary>Hit by a sword beam projectile.
		/// <para/>Default = SenderReactions.Destroy, Reactions.Damage</summary>
		SwordBeam,
		/// <summary>Hit by a projectile from the fire-rod.
		/// <para/>Default = SenderReactions.Intercept</summary>
		RodFire,
		/// <summary>Hit by a boomerang.
		/// <para/>Default = SenderReactions.Intercept, Reactions.Stun</summary>
		Boomerang,
		/// <summary>Hit by the switch hook.
		/// <para/>Default = SenderReactions.Intercept, Reactions.SwitchHook</summary>
		SwitchHook,

		
		//-----------------------------------------------------------------------------
		// Player Interactions
		//-----------------------------------------------------------------------------

		/// <summary>The player presses the 'A' button when facing the monster.
		/// <para/>Default = Reactions.None</summary>
		ButtonAction,
		/// <summary>Collides with the player.
		/// <para/>Default = OnTouchPlayer</summary>
		PlayerContact,
		

		//-----------------------------------------------------------------------------
		// Environment
		//-----------------------------------------------------------------------------
		
		/// <summary>Touches fire.
		/// <para/>Default = Reactions.Burn</summary>
		Fire,
		/// <summary>Touches gale.
		/// <para/>Default = Reactions.Gale</summary>
		Gale,
		/// <summary>Hit by a bomb explosion.
		/// <para/>Default = Reactions.Damage</summary>
		BombExplosion,
		/// <summary>Hit by a thrown object (thrown tiles, not bombs).
		/// <para/>Default = Reactions.Damage</summary>
		ThrownObject,
		/// <summary>Hit by a minecart.</summary>
		/// <para/>Default = Reactions.SoftKill</summary>
		MineCart,
		/// <summary>Hit by a block (either moving or spawned on top of).
		/// <para/>Default = Reactions.Damage</summary>
		Block,


		//-----------------------------------------------------------------------------
		// UNUSED:
		//-----------------------------------------------------------------------------

		SwordHitShield,			// Their sword hits my shield.
		BiggoronSwordHitShield,	// Their biggoron sword hits my shield.
		ShieldHitShield,		// Their shield hits my shield.


		//-----------------------------------------------------------------------------

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

		public delegate void InteractionMemberDelegate(
			Entity sender, EventArgs args);

		public delegate void InteractionStaticDelegate(
			Monster monster, Entity sender, EventArgs args);
		
		private static InteractionStaticDelegate ToStaticInteractionDelegate(
			InteractionMemberDelegate memberDelegate)
		{
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
			
			
			//-------------------------------------------------------------------------
			// Basic Monster Reactions
			//-------------------------------------------------------------------------

			/// <summary>No reaction.</summary>
			public const InteractionStaticDelegate None = null;

			/// <summary>Instantly kill the monster.</summary>
			public static void Kill(Monster monster, Entity sender, EventArgs args) {
				monster.Kill();
			}
			
			/// <summary>Instantly kill the monster, softly, meaning it will respawn upon
			/// re-entering the room.</summary>
			public static void SoftKill(Monster monster, Entity sender, EventArgs args) {
				monster.SoftKill();
			}

			/// <summary>Damage the monster for 1 damage.</summary>
			public static void Damage(Monster monster, Entity sender, EventArgs args) {
				monster.Hurt(1, sender.Center);
			}
			
			/// <summary>Damage the monster for 2 damage.</summary>
			public static void Damage2(Monster monster, Entity sender, EventArgs args) {
				monster.Hurt(2, sender.Center);
			}
			
			/// <summary>Damage the monster for 3 damage.</summary>
			public static void Damage3(Monster monster, Entity sender, EventArgs args) {
				monster.Hurt(3, sender.Center);
			}
			
			/// <summary>Damage the monster for 4 damage.</summary>
			public static void Damage4(Monster monster, Entity sender, EventArgs args) {
				monster.Hurt(4, sender.Center);
			}

			/// <summary>Bump the monster.</summary>
			public static void Bump(Monster monster, Entity sender, EventArgs args) {
				if (!monster.IsBeingKnockedBack) {
					monster.Bump(sender.Center);
				}
			}

			/// <summary>Bump the monster and sender.</summary>
			public static void Parry(Monster monster, Entity sender, EventArgs args) {
				monster.Bump(sender.Center);
				Unit unitSender = sender as Unit;

				if (unitSender != null && !unitSender.IsBeingKnockedBack) {
					unitSender.Bump(monster.Center);
					AudioSystem.PlaySound(GameData.SOUND_EFFECT_CLING);
				}
			}

			/// <summary>Bump the monster and sender.</summary>
			public static void ParryWithClingEffect(Monster monster, Entity sender,
				EventArgs args)
			{
				monster.Bump(sender.Center);
				Unit unitSender = sender as Unit;

				if (unitSender != null && !unitSender.IsBeingKnockedBack &&
					!monster.IsBeingKnockedBack)
				{
					unitSender.Bump(monster.Center);
					AudioSystem.PlaySound(GameData.SOUND_EFFECT_CLING);
					
					Effect effect = new EffectCling();
					Vector2F effectPos = (monster.Center + sender.Center) * 0.5f;
					if (args is InteractionArgs)
						effectPos = (args as InteractionArgs).ContactPoint;
					monster.RoomControl.SpawnEntity(effect, effectPos);
				}
			}
			
			/// <summary>Burn the monster for 1 damage.</summary>
			public static void Burn(Monster monster, Entity sender, EventArgs args) {
				monster.Burn(1);
				if (sender is Fire)
					sender.Destroy();
			}
			
			// <summary>Stun the monster.</summary>
			public static void Stun(Monster monster, Entity sender, EventArgs args) {
				monster.Stun();
			}
			
			/// <summary>Send the monster in a gale.</summary>
			public static void Gale(Monster monster, Entity sender, EventArgs args) {
				monster.EnterGale((EffectGale) sender);
			}
			
			/// <summary>Trigger a random seed effect.</summary>
			public static void MysterySeed(Monster monster, Entity sender,
				EventArgs args)
			{
				SeedType seedType;

				// Random: burn, stun, damage, gale
				int rand = GRandom.NextInt(4);
				if (rand == 0)
					seedType = SeedType.Ember;
				else if (rand == 1)
					seedType = SeedType.Scent;
				else if (rand == 2)
					seedType = SeedType.Pegasus;
				else
					seedType = SeedType.Gale;

				((SeedEntity) sender).SeedType = seedType;

				monster.TriggerInteraction(
					Monster.GetSeedInteractionType(seedType),
					sender, args);
			}
			
			/// <summary>Switch places with the monster (Only for switch-hook
			/// interactions).</summary>
			public static void SwitchHook(Monster monster, Entity sender,
				EventArgs args)
			{
				SwitchHookProjectile hook = sender as SwitchHookProjectile;
				hook.SwitchWithEntity(monster);
				monster.BeginState(new MonsterBusyState(20));
			}
			
			/// <summary>Create a cling effect with sound.</summary>
			public static void ClingEffect(Monster monster, Entity sender,
				EventArgs args)
			{
				Effect effect = new EffectCling();
				
				Vector2F effectPos = (monster.Center + sender.Center) * 0.5f;
				if (args is InteractionArgs)
					effectPos = (args as InteractionArgs).ContactPoint;

				monster.RoomControl.SpawnEntity(effect, effectPos);
				AudioSystem.PlaySound(GameData.SOUND_EFFECT_CLING);
			}

			/// <summary>Electrocute the player.</summary>
			public static void Electrocute(Monster monster, Entity sender,
				EventArgs args)
			{
				if (!(monster.CurrentState is MonsterElectrocuteState)) {

					// Hurt the player
					monster.RoomControl.Player.Hurt(new DamageInfo(4, monster.Center) {
						Flicker = false
					});

					// Begin the electrocute state
					monster.BeginState(new MonsterElectrocuteState(null));
					monster.OnElectrocute();
				}
			}
			

			//-------------------------------------------------------------------------
			// Customizable Monster Reactions
			//-------------------------------------------------------------------------

			/// <summary>Damage the monster for the given amount.</summary>
			public static InteractionStaticDelegate Damage(int amount) {
				return delegate(Monster monster, Entity sender, EventArgs args) {
					monster.Hurt(amount, sender.Center);
				};
			}

			/// <summary>Damage the monster for the given amount based on the used item's
			/// level.</summary>
			public static InteractionStaticDelegate DamageByLevel(
				int amountLevel1, int amountLevel2, int amountLevel3)
			{
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
					monster.RoomControl.SpawnEntity(clonedEffect,
						interactionArgs.ContactPoint);
				};
			}
			

			//-------------------------------------------------------------------------
			// Conditions
			//-------------------------------------------------------------------------
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

			/// <summary>No reaction.</summary>
			public const Delegate None = null;

			
			//-------------------------------------------------------------------------
			// Basic Sender Reactions
			//-------------------------------------------------------------------------

			/// <summary>Destroy the sender.</summary>
			public static void Destroy(Monster monster, Entity sender, EventArgs args) {
				sender.Destroy();
			}

			/// <summary>Intercept the sender if it implements IInterceptable. Typically,
			/// this is used for projectiles to cause them to crash or be destroyed.
			/// </summary>
			public static void Intercept(Monster monster, Entity sender, EventArgs args) {
				if (sender is IInterceptable)
					(sender as IInterceptable).Intercept();
			}
			
			/// <summary>Bump the sender if it is a unit.</summary>
			public static void Bump(Monster monster, Entity sender, EventArgs args) {
				if (sender is Unit && !(sender as Unit).IsBeingKnockedBack) {
					AudioSystem.PlaySound(GameData.SOUND_BOMB_BOUNCE);
					(sender as Unit).Bump(monster.Center);
				}
			}
			
			/// <summary>Damage the sender by 1 if it is a unit.</summary>
			public static void Damage(Monster monster, Entity sender, EventArgs args) {
				if (sender is Unit)
					(sender as Unit).Hurt(1, monster.Center);
			}

			/// <summary>Kill the sender if it is a unit, otherwise destroy it instead.
			/// </summary>
			public static void Kill(Monster monster, Entity sender, EventArgs args) {
				if (sender is Unit)
					(sender as Unit).Kill();
				else
					sender.Destroy();
			}


			//-------------------------------------------------------------------------
			// Customizable Sender Reactions
			//-------------------------------------------------------------------------

		}
	}
}
