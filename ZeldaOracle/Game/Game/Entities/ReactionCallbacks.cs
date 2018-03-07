using System;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Entities.Monsters.States;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles;
using ZeldaOracle.Game.Entities.Projectiles.Seeds;
using ZeldaOracle.Game.Entities.Units;
using ZeldaOracle.Game.Items;

namespace ZeldaOracle.Game.Entities {

	/// <summary>General entity reaction callbacks.</summary>
	public static class EntityReactions {

		public static void OneTime(
			Entity reactionEntity, Entity actionEntity, EventArgs args)
		{
		}

		/// <summary>Count the reaction as taking up a button action, so that the
		/// player knows not to trigger any other button actions such as using weapons.
		/// </summary>
		public static void TriggerButtonReaction(
			Entity a, Entity b, EventArgs args)
		{
			a.RoomControl.Player.TriggeredButtonReaction = true;
		}

		/// <summary>Pickup the entity. This only works if the action entity is the
		/// player.</summary>
		public static void Pickup(
			Entity reactionEntity, Entity actionEntity, EventArgs args)
		{
			if (actionEntity.RootEntity is Player) {
				Player player = (Player) actionEntity.RootEntity;
				player.PickupEntity(reactionEntity);
			}
		}
	}

	
	/// <summary>Reaction callbacks for units.</summary>
	public static class UnitReactions {

		/// <summary>Damage the unit for 1 damage.</summary>
		public static void Damage(Entity subject, Entity sender, EventArgs args) {
			((Unit) subject.RootEntity).Hurt(1, sender.Center);
		}
			
		/// <summary>Damage the unit for 2 damage.</summary>
		public static void Damage2(Entity subject, Entity sender, EventArgs args) {
			((Unit) subject.RootEntity).Hurt(2, sender.Center);
		}
			
		/// <summary>Damage the unit for 3 damage.</summary>
		public static void Damage3(Entity subject, Entity sender, EventArgs args) {
			((Unit) subject.RootEntity).Hurt(3, sender.Center);
		}
			
		/// <summary>Damage the unit for 4 damage.</summary>
		public static void Damage4(Entity subject, Entity sender, EventArgs args) {
			((Unit) subject.RootEntity).Hurt(4, sender.Center);
		}

		/// <summary>Knockback the unit.</summary>
		public static void Bump(Entity subject, Entity sender, EventArgs args) {
			Unit unit = (Unit) subject.RootEntity;
			if (!unit.IsBeingKnockedBack)
				unit.Bump(sender.Center);
		}
	}

	
	/// <summary>Reaction callbacks for monsters.</summary>
	public static class MonsterReactions {
			
		//-----------------------------------------------------------------------------
		// Basic Monster Reactions
		//-----------------------------------------------------------------------------

		/// <summary>Instantly kill the monster.</summary>
		public static void Kill(Entity subject, Entity sender, EventArgs args) {
			((Monster) subject.RootEntity).Kill();
		}
			
		/// <summary>Instantly kill the monster, softly, meaning it will respawn upon
		/// re-entering the room.</summary>
		public static void SoftKill(Entity subject, Entity sender, EventArgs args) {
			((Monster) subject.RootEntity).SoftKill();
		}

		/// <summary>Damage the monster for 1 damage.</summary>
		public static void Damage(Entity subject, Entity sender, EventArgs args) {
			((Unit) subject.RootEntity).Hurt(1, sender.Center);
		}
			
		/// <summary>Damage the monster for 2 damage.</summary>
		public static void Damage2(Entity subject, Entity sender, EventArgs args) {
			((Unit) subject.RootEntity).Hurt(2, sender.Center);
		}
			
		/// <summary>Damage the monster for 3 damage.</summary>
		public static void Damage3(Entity subject, Entity sender, EventArgs args) {
			((Unit) subject.RootEntity).Hurt(3, sender.Center);
		}
			
		/// <summary>Damage the monster for 4 damage.</summary>
		public static void Damage4(Entity subject, Entity sender, EventArgs args) {
			((Unit) subject.RootEntity).Hurt(4, sender.Center);
		}

		/// <summary>Silently damage the monster for 1 damage.</summary>
		public static void SilentDamage(Entity subject, Entity sender, EventArgs args) {
			((Monster) subject.RootEntity).Hurt(new DamageInfo(1, sender.Center) {
				PlaySound = false
			});
		}

		/// <summary>Silently damage the monster for 2 damage.</summary>
		public static void SilentDamage2(Entity subject, Entity sender, EventArgs args) {
			((Monster) subject.RootEntity).Hurt(new DamageInfo(2, sender.Center) {
				PlaySound = false
			});
		}

		/// <summary>Silently damage the monster for 3 damage.</summary>
		public static void SilentDamage3(Entity subject, Entity sender, EventArgs args) {
			((Monster) subject.RootEntity).Hurt(new DamageInfo(3, sender.Center) {
				PlaySound = false
			});
		}

		/// <summary>Silently damage the monster for 4 damage.</summary>
		public static void SilentDamage4(Entity subject, Entity sender, EventArgs args) {
			((Monster) subject.RootEntity).Hurt(new DamageInfo(4, sender.Center) {
				PlaySound = false
			});
		}

		/// <summary>Bump the monster.</summary>
		public static void Bump(Entity subject, Entity sender, EventArgs args) {
			Monster monster = (Monster) subject.RootEntity;
			if (!monster.IsBeingKnockedBack) {
				monster.Bump(sender.Center);
			}
		}

		/// <summary>Bump the monster and sender.</summary>
		public static void Parry(InteractionInstance interaction) {
			Entity actionRoot = interaction.ActionEntity.RootEntity;
			Entity reactionRoot = interaction.ReactionEntity.RootEntity;

			// Bump and/or intercept the action entity
			Unit actionUnit = actionRoot as Unit;
			if (actionUnit != null && actionUnit.IsKnockbackable &&
				!actionUnit.IsBeingKnockedBack)
				actionUnit.Bump(reactionRoot.Center);
			if (interaction.ActionEntity is IInterceptable)
				((IInterceptable) interaction.ActionEntity).Intercept();

			// Bump and/or intercept the reaction unit
			Unit reactionUnit = reactionRoot as Unit;
			if (reactionUnit != null && reactionUnit.IsKnockbackable &&
				!reactionUnit.IsBeingKnockedBack)
				reactionUnit.Bump(actionRoot.Center);
			if (interaction.ReactionEntity is IInterceptable)
				((IInterceptable) interaction.ReactionEntity).Intercept();
		}

		/// <summary>Kncokback and/or intercept both entities in the interaction.</summary>
		public static void ParryWithClingEffect(InteractionInstance interaction) {
			bool cling = false;
			Entity actionRoot = interaction.ActionEntity.RootEntity;
			Entity reactionRoot = interaction.ReactionEntity.RootEntity;

			// Bump and/or intercept the action entity
			Unit actionUnit = actionRoot as Unit;
			if (actionUnit != null && actionUnit.IsKnockbackable &&
				!actionUnit.IsBeingKnockedBack)
			{
				actionUnit.Bump(reactionRoot.Center);
				cling = true;
			}
			if (interaction.ActionEntity is IInterceptable) {
				if (((IInterceptable) interaction.ActionEntity).Intercept())
					cling = true;
			}

			// Bump and/or intercept the reaction unit
			Unit reactionUnit = reactionRoot as Unit;
			if (reactionUnit != null && reactionUnit.IsKnockbackable &&
				!reactionUnit.IsBeingKnockedBack)
			{
				reactionUnit.Bump(actionRoot.Center);
				cling = true;
			}
			if (interaction.ReactionEntity is IInterceptable) {
				if (((IInterceptable) interaction.ReactionEntity).Intercept())
					cling = true;
			}

			// Spawn the cling effect if either unit was intercepted or knocked-back
			if (cling) {
				Effect effect = new EffectCling();
				Vector2F effectPos = (actionRoot.Center + reactionRoot.Center) * 0.5f;
				if (interaction.Arguments is InteractionArgs)
					effectPos = ((InteractionArgs) interaction.Arguments).ContactPoint;
				interaction.RoomControl.SpawnEntity(effect, effectPos);
				AudioSystem.PlaySound(GameData.SOUND_EFFECT_CLING);
			}
		}
			
		/// <summary>Burn the monster for 2 damage.</summary>
		public static void Burn(Entity subject, Entity sender, EventArgs args) {
			((Monster) subject.RootEntity).Burn(2);
			if (sender.RootEntity is Fire)
				sender.RootEntity.Destroy();
		}
			
		/// <summary>Stun the monster.</summary>
		public static void Stun(Entity subject, Entity sender, EventArgs args) {
			((Monster) subject.RootEntity).Stun();
		}
			
		/// <summary>Send the monster in a gale.</summary>
		public static void Gale(Entity subject, Entity sender, EventArgs args) {
			((Monster) subject.RootEntity).EnterGale((EffectGale) sender);
		}
			
		/// <summary>Trigger a random seed effect.</summary>
		public static void MysterySeed(InteractionInstance interaction) {
			// Randomly choose between Burn, Stun, damage, and Gale
			SeedType seedType = GRandom.Choose(
				SeedType.Ember, SeedType.Scent, SeedType.Pegasus, SeedType.Gale);
			InteractionType interactionType =
				InteractionComponent.GetSeedInteractionType(seedType);

			// Let the seed knock which seed type it turned into so it spawns
			// the correct effect
			((SeedEntity) interaction.ActionEntity.RootEntity).SeedType = seedType;

			// Trigger the actual chose seed interaction
			((Monster) interaction.ReactionEntity.RootEntity)
				.Interactions.Trigger(interaction);
		}
			
		/// <summary>Switch places with the monster (Only for switch-hook
		/// interactions).</summary>
		public static void SwitchHook(Entity subject, Entity sender,
			EventArgs args)
		{
			Monster monster = (Monster) subject.RootEntity;
			SwitchHookProjectile hook = sender.RootEntity as SwitchHookProjectile;
			hook.SwitchWithEntity(monster);
			monster.BeginState(new MonsterBusyState(20));
		}
			
		/// <summary>Create a cling effect with sound.</summary>
		public static void ClingEffect(InteractionInstance interaction) {
			Effect effect = new EffectCling();
			Vector2F effectPos = (interaction.ActionEntity.RootEntity.Center +
				interaction.ReactionEntity.RootEntity.Center) * 0.5f;
			if (interaction.Arguments is InteractionArgs)
				effectPos = ((InteractionArgs) interaction.Arguments).ContactPoint;
			interaction.RoomControl.SpawnEntity(effect, effectPos);
			AudioSystem.PlaySound(GameData.SOUND_EFFECT_CLING);
		}

		/// <summary>Electrocute the player.</summary>
		public static void Electrocute(Entity subject, Entity sender,
			EventArgs args)
		{
			Monster monster = (Monster) subject.RootEntity;
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
			

		//-----------------------------------------------------------------------------
		// Customizable Monster Reactions
		//-----------------------------------------------------------------------------

		/// <summary>Damage the monster for the given amount.</summary>
		public static ReactionStaticCallback Damage(int amount) {
			return delegate(Entity subject, Entity sender, EventArgs args) {
				((Monster) subject).Hurt(amount, sender.RootEntity.Center);
			};
		}

		/// <summary>Damage the monster for the given amount based on the used item's
		/// level.</summary>
		public static ReactionStaticCallback DamageByLevel(
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
				((Monster) subject).Hurt(amount, sender.RootEntity.Center);
			};
		}

		/// <summary>Silentyly damage the monster for the given amount based on the used item's
		/// level.</summary>
		public static ReactionStaticCallback SilentDamageByLevel(
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
				((Monster) subject).Hurt(new DamageInfo(amount, sender.RootEntity.Center) {
					PlaySound = false
				});
			};
		}

		public static ReactionStaticCallback ContactEffect(Effect effect) {
			return delegate(Entity subject, Entity sender, EventArgs args) {
				Effect clonedEffect = effect.Clone();
				InteractionArgs interactionArgs = args as InteractionArgs;
				((Monster) subject).RoomControl.SpawnEntity(clonedEffect,
					interactionArgs.ContactPoint);
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
		
	
	/// <summary>Reactions that apply to the entity that triggered the interaction.
	/// </summary>
	public static class SenderReactions {
			
		//-----------------------------------------------------------------------------
		// Basic Sender Reactions
		//-----------------------------------------------------------------------------

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
			Unit unit = sender.RootEntity as Unit;
			if (unit != null && !unit.IsBeingKnockedBack) {
				AudioSystem.PlaySound(GameData.SOUND_BOMB_BOUNCE);
				unit.Bump(subject.Center);
			}
		}
			
		/// <summary>Damage the sender by 1 if it is a unit.</summary>
		public static void Damage(Entity subject, Entity sender, EventArgs args) {
			Unit unit = sender.RootEntity as Unit;
			if (unit != null)
				unit.Hurt(1, subject.Center);
		}
			
		/// <summary>Damage the sender by 2 if it is a unit.</summary>
		public static void Damage2(Entity subject, Entity sender, EventArgs args) {
			Unit unit = sender.RootEntity as Unit;
			if (unit != null)
				unit.Hurt(2, subject.Center);
		}

		/// <summary>Kill the sender if it is a unit, otherwise destroy it instead.
		/// </summary>
		public static void Kill(Entity subject, Entity sender, EventArgs args) {
			Unit unit = sender.RootEntity as Unit;
			if (sender is Unit)
				unit.Kill();
			else
				sender.RootEntity.Destroy();
		}


		//-----------------------------------------------------------------------------
		// Customizable Sender Reactions
		//-----------------------------------------------------------------------------

	}

}
