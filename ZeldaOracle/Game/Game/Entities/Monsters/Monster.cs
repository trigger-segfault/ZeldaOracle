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

namespace ZeldaOracle.Game.Entities.Monsters {

	public enum MonsterRespawnType {
		Never	= 0,
		Always	= 1,
		Normal	= 2,
	}

	public enum MonsterColor {
		Red		= 0,
		Blue	= 1,
		Green	= 2,
		Orange	= 3,
	}

	// MonsterState:
	// - MonsterBurnState
	// - MonsterGaleState
	// - MonsterStunState
	// - MonsterFallInHoleState

	public partial class Monster : Unit {
		
		private Properties properties;
		private bool isKnockbackable; // Can the monster be knocked back?
		private int contactDamage;
		private InteractionHandler[] interactionHandlers;

		// Burn State.
		private bool isBurning;
		private AnimationPlayer effectAnimation;
		protected MonsterColor color;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Monster() {
			// With player:
			// - Top: 4 overlap
			// - Bottom: 3 overlap?
			// - Sides: 3 overlap	

			color = MonsterColor.Red;

			// Physics.
			Physics.CollisionBox		= new Rectangle2I(-5, -9, 10, 10);
			Physics.SoftCollisionBox	= new Rectangle2I(-6, -11, 12, 11);
			Physics.CollideWithWorld	= true;
			Physics.CollideWithRoomEdge	= true;
			Physics.AutoDodges			= true;
			Physics.HasGravity			= true;
			Physics.IsDestroyedInHoles	= true;

			// Graphics.
			Graphics.DepthLayer			= DepthLayer.Monsters;
			Graphics.DepthLayerInAir	= DepthLayer.InAirMonsters;
			Graphics.DrawOffset			= new Point2I(-8, -14);
			centerOffset				= new Point2I(0, -6);

			// Monster & unit settings.
			knockbackSpeed			= GameSettings.MONSTER_KNOCKBACK_SPEED;
			hurtKnockbackDuration	= GameSettings.MONSTER_HURT_KNOCKBACK_DURATION;
			bumpKnockbackDuration	= GameSettings.MONSTER_BUMP_KNOCKBACK_DURATION;
			hurtInvincibleDuration	= GameSettings.MONSTER_HURT_INVINCIBLE_DURATION;
			hurtFlickerDuration		= GameSettings.MONSTER_HURT_FLICKER_DURATION;
			contactDamage			= 1;
			isKnockbackable			= true;
			isBurning				= false;
			effectAnimation			= new AnimationPlayer();
			
			interactionHandlers = new InteractionHandler[(int) InteractionType.Count];
			for (int i = 0; i < (int) InteractionType.Count; i++)
				interactionHandlers[i] = new InteractionHandler();

			// Setup default interactions.
			// Seeds
			SetReaction(InteractionType.EmberSeed,		SenderReactions.Intercept);
			SetReaction(InteractionType.ScentSeed,		SenderReactions.Intercept,	Reactions.Damage);
			SetReaction(InteractionType.PegasusSeed,	SenderReactions.Intercept,	Reactions.Stun);
			SetReaction(InteractionType.GaleSeed,		SenderReactions.Intercept,	Reactions.Gale);
			SetReaction(InteractionType.MysterySeed,	Reactions.MysterySeed);
			// Projectiles
			SetReaction(InteractionType.Arrow,			SenderReactions.Destroy,	Reactions.Damage);
			SetReaction(InteractionType.SwordBeam,		SenderReactions.Destroy,	Reactions.Damage);
			SetReaction(InteractionType.RodFire,		SenderReactions.Intercept);
			SetReaction(InteractionType.Boomerang,		SenderReactions.Intercept,	Reactions.Stun);
			SetReaction(InteractionType.SwitchHook,		Reactions.SwitchHook);
			// Environment
			SetReaction(InteractionType.Fire,			SenderReactions.Destroy,	Reactions.Burn);
			SetReaction(InteractionType.BombExplosion,	Reactions.Damage);
			SetReaction(InteractionType.ThrownObject,	Reactions.Damage);
			SetReaction(InteractionType.MineCart,		Reactions.SoftKill);
			SetReaction(InteractionType.Block,			Reactions.Damage);
			// Tools
			SetReaction(InteractionType.Sword,			Reactions.DamageByLevel(1, 2, 3));
			SetReaction(InteractionType.BiggoronSword,	Reactions.Damage3);
			SetReaction(InteractionType.Shield,			SenderReactions.Bump, Reactions.Bump);
			SetReaction(InteractionType.Shovel,			Reactions.Bump);
			// Player
			SetReaction(InteractionType.Pickup,			Reactions.None);
			SetReaction(InteractionType.ButtonAction,	Reactions.None);
			SetReaction(InteractionType.PlayerContact,	OnTouchPlayer);
			
			SetReaction(InteractionType.Parry,			SenderReactions.Bump, Reactions.Bump,
				Reactions.ContactEffect(new Effect(GameData.ANIM_EFFECT_CLING, DepthLayer.EffectCling)));

			// TODO: Parry with Biggoron Sword: don't bump player.
			// TODO: Spin sword reaction.

			//SetReaction(InteractionType.SwordHitShield,			new COMBO(new EFFECT_CLING(), new BUMP(true)));
			//SetReaction(InteractionType.BiggoronSwordHitShield,	new COMBO(new EFFECT_CLING(), new BUMP()));
			//SetReaction(InteractionType.ShieldHitShield,			new COMBO(new EFFECT_CLING(), new BUMP(true)));
		}

		
		//-----------------------------------------------------------------------------
		// Monster Reactions
		//-----------------------------------------------------------------------------

		public void Burn(int damage) {
			if (IsInvincible || isBurning)
				return;

			// TODO: Enter burn state.

			// Apply damage.
			DamageInfo damageInfo = new DamageInfo(damage);
			damageInfo.ApplyKnockBack		= true;
			damageInfo.HasSource			= false;
			damageInfo.KnockbackDuration	= GameSettings.MONSTER_BURN_DURATION;
			damageInfo.Flicker				= false;

			Hurt(damageInfo);

			isBurning = true;

			// Create the burn effect.
			effectAnimation.Play(GameData.ANIM_EFFECT_BURN);
		}

		public void OnTouchPlayer(Entity sender, EventArgs args) {
			Player player = (Player) sender;
			player.Hurt(contactDamage, Center);
		}

		
		//-----------------------------------------------------------------------------
		// Interactions & Reactions
		//-----------------------------------------------------------------------------
		
		// Trigger an interaction.
		public void TriggerInteraction(InteractionType type, Entity sender) {
			TriggerInteraction(type, sender, EventArgs.Empty);
		}

		// Trigger an interaction with the given arguments.
		public void TriggerInteraction(InteractionType type, Entity sender, EventArgs args) {
			InteractionHandler handler = GetInteraction(type);
			handler.Trigger(this, sender, args);
		}

		// Get the interaction handler for the given interaction type.
		protected InteractionHandler GetInteraction(InteractionType type) {
			return interactionHandlers[(int) type];
		}
		
		// Set the reactions to the given interaction type.
		// The reaction functions are called in the order they are specified.
		protected void SetReaction(InteractionType type, params InteractionStaticDelegate[] reactions) {
			InteractionHandler handler = GetInteraction(type);
			handler.Clear();
			for (int i = 0; i < reactions.Length; i++)
				handler.Add(reactions[i]);
		}
		
		// Set the reactions to the given interaction type.
		// The reaction functions are called in the order they are specified.
		protected void SetReaction(InteractionType type, params InteractionMemberDelegate[] reactions) {
			InteractionHandler handler = GetInteraction(type);
			handler.Clear();
			for (int i = 0; i < reactions.Length; i++)
				handler.Add(reactions[i]);
		}

		
		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------
		
		public void ChooseImageVariant() {
			switch (color) {
				case MonsterColor.Red:		Graphics.ImageVariant = GameData.VARIANT_RED;		break;
				case MonsterColor.Blue:		Graphics.ImageVariant = GameData.VARIANT_BLUE;		break;
				case MonsterColor.Green:	Graphics.ImageVariant = GameData.VARIANT_GREEN;		break;
				case MonsterColor.Orange:	Graphics.ImageVariant = GameData.VARIANT_ORANGE;	break;
			}
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			health = healthMax;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_OCTOROK);
			ChooseImageVariant();
		}

		public override void Die() {
			Effect explosion = new Effect(GameData.ANIM_EFFECT_MONSTER_EXPLOSION, DepthLayer.EffectMonsterExplosion);
			RoomControl.SpawnEntity(explosion, Center);
			base.Die();
		}

		public override void OnKnockbackEnd() {
			base.OnKnockbackEnd();
			if (isBurning) {
				isBurning = false;
				effectAnimation.Animation = null;
			}
		}

		public override void UpdateGraphics() {
			ChooseImageVariant();
			base.UpdateGraphics();
			effectAnimation.Update();
		}

		public override void Update() {
			
			// 1. (M-M) MonsterTools to PlayerTools
			// 2. (M-1) MonsterTools to Player
			// 4. (M-1) PlayerTools to Monster
			// 2. (1-1) Monster to Player

			bool parry = false;
			Player player = RoomControl.Player;

			// Collide my tools with player's tools.
			foreach (UnitTool monsterTool in EquippedTools) {
				if (parry)
					break;

				if (monsterTool.IsPhysicsEnabled) {
					foreach (UnitTool playerTool in player.EquippedTools) {
						if (playerTool.IsPhysicsEnabled &&
							monsterTool.PositionedCollisionBox.Intersects(
							playerTool.PositionedCollisionBox))
						{
							if ((monsterTool.ToolType == UnitToolType.Sword || monsterTool.ToolType == UnitToolType.Shield) &&
								(playerTool.ToolType == UnitToolType.Sword || playerTool.ToolType == UnitToolType.Shield) &&
								!IsBeingKnockedBack && !player.IsBeingKnockedBack)
							{
								Vector2F contactPoint = playerTool.PositionedCollisionBox.Center;

								TriggerInteraction(InteractionType.Parry, player, new ParryInteractionArgs() {
									ContactPoint	= contactPoint,
									MonsterTool		= monsterTool,
									SenderTool		= playerTool
								});

								parry = true;
							}
						}
					}
				}
			}

			// Collide my tools with the player.
			foreach (UnitTool tool in EquippedTools) {
				if (parry)
					break;

				if (tool.IsPhysicsEnabled && tool.ToolType == UnitToolType.Sword) {
					if (player.Physics.PositionedSoftCollisionBox.Intersects(tool.PositionedCollisionBox)) {
						player.Hurt(contactDamage, Center);
					}
				}
			}

			// Collide with the player's tools.
			foreach (UnitTool tool in player.EquippedTools) {
				if (parry || IsInvincible || IsBeingKnockedBack)
					break;

				if (tool.IsPhysicsEnabled && (tool.ToolType == UnitToolType.Sword || tool.ToolType == UnitToolType.Shield)) {
					if (Physics.PositionedSoftCollisionBox.Intersects(tool.PositionedCollisionBox)) {
						if (tool.ToolType == UnitToolType.Sword) {
							TriggerInteraction(InteractionType.Sword, player, new WeaponInteractionEventArgs() {
								Weapon = player.Inventory.GetItem("item_sword") as ItemWeapon
							});
							parry = true;
						}
						else if (tool.ToolType == UnitToolType.Shield) {
							Vector2F contactPoint = (player.Center + Center) * 0.5f;
							TriggerInteraction(InteractionType.Shield, player, new WeaponInteractionEventArgs() {
								Weapon = player.Inventory.GetItem("item_shield") as ItemWeapon
							});
							parry = true;
						}
					}
				}
			}

			// Check collisions with player.
			if (!parry && physics.IsCollidingWith(player, CollisionBoxType.Soft)) {
				TriggerInteraction(InteractionType.PlayerContact, player);
			}

			base.Update();
		}

		public override void Draw(RoomGraphics g) {
			base.Draw(g);

			// Draw burn effect.
			if (effectAnimation.Animation != null) {
				g.DrawAnimation(effectAnimation, Center - new Vector2F(0, zPosition), DepthLayer.EffectMonsterBurnFlame);
			}
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Properties Properties {
			get { return properties; }
			set { properties = value; }
		}

		public int ContactDamage {
			get { return contactDamage; }
			set { contactDamage = value; }
		}

		public bool IsKnockbackable {
			get { return isKnockbackable; }
			set { isKnockbackable = value; }
		}
	}
}
