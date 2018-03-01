using System;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Units;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Game.Entities.Monsters.States;
using ZeldaOracle.Game.Entities.Projectiles.Seeds;
using ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles;
using ZeldaOracle.Game.Entities.Projectiles;

namespace ZeldaOracle.Game.Entities.Monsters {
	
	public partial class Monster : Unit {

		public static InteractionType GetSeedInteractionType(SeedType seedType) {
			return (InteractionType) ((int) InteractionType.EmberSeed + (int) seedType);
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
			public static void Kill(Entity subject, Entity sender, EventArgs args) {
				((Monster) subject).Kill();
			}
			
			/// <summary>Instantly kill the monster, softly, meaning it will respawn upon
			/// re-entering the room.</summary>
			public static void SoftKill(Entity subject, Entity sender, EventArgs args) {
				((Monster) subject).SoftKill();
			}

			/// <summary>Damage the monster for 1 damage.</summary>
			public static void Damage(Entity subject, Entity sender, EventArgs args) {
				((Monster) subject).Hurt(1, sender.Center);
			}
			
			/// <summary>Damage the monster for 2 damage.</summary>
			public static void Damage2(Entity subject, Entity sender, EventArgs args) {
				((Monster) subject).Hurt(2, sender.Center);
			}
			
			/// <summary>Damage the monster for 3 damage.</summary>
			public static void Damage3(Entity subject, Entity sender, EventArgs args) {
				((Monster) subject).Hurt(3, sender.Center);
			}
			
			/// <summary>Damage the monster for 4 damage.</summary>
			public static void Damage4(Entity subject, Entity sender, EventArgs args) {
				((Monster) subject).Hurt(4, sender.Center);
			}

			/// <summary>Silently damage the monster for 1 damage.</summary>
			public static void SilentDamage(Entity subject, Entity sender, EventArgs args) {
				((Monster) subject).Hurt(new DamageInfo(1, sender.Center) {
					PlaySound = false
				});
			}

			/// <summary>Silently damage the monster for 2 damage.</summary>
			public static void SilentDamage2(Entity subject, Entity sender, EventArgs args) {
				((Monster) subject).Hurt(new DamageInfo(2, sender.Center) {
					PlaySound = false
				});
			}

			/// <summary>Silently damage the monster for 3 damage.</summary>
			public static void SilentDamage3(Entity subject, Entity sender, EventArgs args) {
				((Monster) subject).Hurt(new DamageInfo(3, sender.Center) {
					PlaySound = false
				});
			}

			/// <summary>Silently damage the monster for 4 damage.</summary>
			public static void SilentDamage4(Entity subject, Entity sender, EventArgs args) {
				((Monster) subject).Hurt(new DamageInfo(4, sender.Center) {
					PlaySound = false
				});
			}

			/// <summary>Bump the monster.</summary>
			public static void Bump(Entity subject, Entity sender, EventArgs args) {
				Monster monster = (Monster) subject;
				if (!monster.IsBeingKnockedBack) {
					monster.Bump(sender.Center);
				}
			}

			/// <summary>Bump the monster and sender.</summary>
			public static void Parry(Entity subject, Entity sender, EventArgs args) {
				Monster monster = (Monster) subject;
				monster.Bump(sender.Center);
				Unit unitSender = sender as Unit;

				if (unitSender != null && !unitSender.IsBeingKnockedBack) {
					unitSender.Bump(monster.Center);
					AudioSystem.PlaySound(GameData.SOUND_EFFECT_CLING);
				}
			}

			/// <summary>Bump the monster and sender.</summary>
			public static void ParryWithClingEffect(Entity subject, Entity sender,
				EventArgs args)
			{
				Monster monster = (Monster) subject;
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
			
			/// <summary>Burn the monster for 2 damage.</summary>
			public static void Burn(Entity subject, Entity sender, EventArgs args) {
				((Monster) subject).Burn(2);
				if (sender is Fire)
					sender.Destroy();
			}
			
			/// <summary>Stun the monster.</summary>
			public static void Stun(Entity subject, Entity sender, EventArgs args) {
				((Monster) subject).Stun();
			}
			
			/// <summary>Send the monster in a gale.</summary>
			public static void Gale(Entity subject, Entity sender, EventArgs args) {
				((Monster) subject).EnterGale((EffectGale) sender);
			}
			
			/// <summary>Trigger a random seed effect.</summary>
			public static void MysterySeed(Entity subject, Entity sender,
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

				((Monster) subject).Interactions.Trigger(
					Monster.GetSeedInteractionType(seedType),
					sender, args);
			}
			
			/// <summary>Switch places with the monster (Only for switch-hook
			/// interactions).</summary>
			public static void SwitchHook(Entity subject, Entity sender,
				EventArgs args)
			{
				Monster monster = (Monster) subject;
				SwitchHookProjectile hook = sender as SwitchHookProjectile;
				hook.SwitchWithEntity(monster);
				monster.BeginState(new MonsterBusyState(20));
			}
			
			/// <summary>Create a cling effect with sound.</summary>
			public static void ClingEffect(Entity subject, Entity sender,
				EventArgs args)
			{
				Monster monster = (Monster) subject;
				Effect effect = new EffectCling();
				
				Vector2F effectPos = (monster.Center + sender.Center) * 0.5f;
				if (args is InteractionArgs)
					effectPos = (args as InteractionArgs).ContactPoint;

				monster.RoomControl.SpawnEntity(effect, effectPos);
				AudioSystem.PlaySound(GameData.SOUND_EFFECT_CLING);
			}

			/// <summary>Electrocute the player.</summary>
			public static void Electrocute(Entity subject, Entity sender,
				EventArgs args)
			{
				Monster monster = (Monster) subject;
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
				return delegate(Entity subject, Entity sender, EventArgs args) {
					((Monster) subject).Hurt(amount, sender.Center);
				};
			}

			/// <summary>Damage the monster for the given amount based on the used item's
			/// level.</summary>
			public static InteractionStaticDelegate DamageByLevel(
				int amountLevel1, int amountLevel2, int amountLevel3 = 0)
			{
				return delegate(Entity subject, Entity sender, EventArgs args) {
					int level = (args as WeaponInteractionEventArgs).Weapon.Level;
					int amount = 0;
					if (level == Item.Level1)
						amount = amountLevel1;
					else if (level == Item.Level2)
						amount = amountLevel2;
					else if (level == Item.Level3)
						amount = amountLevel3;
					((Monster) subject).Hurt(amount, sender.Center);
				};
			}

			/// <summary>Silentyly damage the monster for the given amount based on the used item's
			/// level.</summary>
			public static InteractionStaticDelegate SilentDamageByLevel(
				int amountLevel1, int amountLevel2, int amountLevel3 = 0) {
				return delegate (Entity subject, Entity sender, EventArgs args) {
					int level = (args as WeaponInteractionEventArgs).Weapon.Level;
					int amount = 0;
					if (level == Item.Level1)
						amount = amountLevel1;
					else if (level == Item.Level2)
						amount = amountLevel2;
					else if (level == Item.Level3)
						amount = amountLevel3;
					((Monster) subject).Hurt(new DamageInfo(amount, sender.Center) {
						PlaySound = false
					});
				};
			}

			public static InteractionStaticDelegate ContactEffect(Effect effect) {
				return delegate(Entity subject, Entity sender, EventArgs args) {
					Effect clonedEffect = effect.Clone();
					InteractionArgs interactionArgs = args as InteractionArgs;
					((Monster) subject).RoomControl.SpawnEntity(clonedEffect,
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
			public static void Destroy(Entity subject, Entity sender, EventArgs args) {
				sender.Destroy();
			}

			/// <summary>Intercept the sender if it implements IInterceptable. Typically,
			/// this is used for projectiles to cause them to crash or be destroyed.
			/// </summary>
			public static void Intercept(Entity subject, Entity sender, EventArgs args) {
				if (sender is IInterceptable)
					(sender as IInterceptable).Intercept();
			}

			/// <summary>Deflect the sender if it is a projectile.</summary>
			public static void Deflect(Entity subject, Entity sender, EventArgs args) {
				if (sender is Projectile)
					((Projectile) sender).Intercept();
			}
			
			/// <summary>Bump the sender if it is a unit.</summary>
			public static void Bump(Entity subject, Entity sender, EventArgs args) {
				if (sender is Unit && !(sender as Unit).IsBeingKnockedBack) {
					AudioSystem.PlaySound(GameData.SOUND_BOMB_BOUNCE);
					(sender as Unit).Bump(subject.Center);
				}
			}
			
			/// <summary>Damage the sender by 1 if it is a unit.</summary>
			public static void Damage(Entity subject, Entity sender, EventArgs args) {
				if (sender is Unit)
					(sender as Unit).Hurt(1, subject.Center);
			}

			/// <summary>Kill the sender if it is a unit, otherwise destroy it instead.
			/// </summary>
			public static void Kill(Entity subject, Entity sender, EventArgs args) {
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
