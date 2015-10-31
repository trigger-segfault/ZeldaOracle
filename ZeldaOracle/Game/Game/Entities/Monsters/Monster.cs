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
		
		//-----------------------------------------------------------------------------
		// Interaction Handler Delegates
		//-----------------------------------------------------------------------------

		// Player & items
		public delegate void InteractionHandler_ButtonAction();
		public delegate void InteractionHandler_Sword(ItemSword itemSword);
		public delegate void InteractionHandler_BigSword(ItemBigSword itemBigSword);
		public delegate void InteractionHandler_Shield(ItemShield itemShield);
		public delegate void InteractionHandler_Shovel(ItemShovel itemShovel);
		public delegate void InteractionHandler_Bracelet(ItemBracelet itemBracelet);

		// Projectiles & Thrown objects
		public delegate void InteractionHandler_Seed(SeedEntity seed);
		public delegate void InteractionHandler_Arrow(Arrow arrow);
		public delegate void InteractionHandler_SwordBeam(SwordBeam swordBeam);
		public delegate void InteractionHandler_Boomerang(Boomerang boomerang);
		public delegate void InteractionHandler_RodFire(MagicRodFire rodFire);
		public delegate void InteractionHandler_SwitchHook(SwitchHookProjectile hook);
		public delegate void InteractionHandler_ThrownObject(CarriedTile thrownObject);

		// Effects
		public delegate void InteractionHandler_Fire(Fire fire);
		public delegate void InteractionHandler_BombExplosion(Effect explosion);
		

		//-----------------------------------------------------------------------------
		// Member Variables
		//-----------------------------------------------------------------------------

		private Properties properties;

		// Settings.
		private bool isKnockbackable; // Can the monster be knocked back?
		private int contactDamage;

		// Burn State.
		private bool isBurning;
		private AnimationPlayer effectAnimation;

		// Interaction handlers.
		// Player & items
		private InteractionHandler_ButtonAction		handlerButtonAction;
		private InteractionHandler_Sword			handlerSword;
		private InteractionHandler_BigSword			handlerBigSword;
		private InteractionHandler_Shield			handlerShield;
		private InteractionHandler_Shovel			handlerShovel;
		private InteractionHandler_Bracelet			handlerBracelet;
		// Projectiles & Thrown objects
		private InteractionHandler_Seed[]			handlerSeeds;
		private InteractionHandler_Arrow			handlerArrow;
		private InteractionHandler_Boomerang		handlerBoomerang;
		private InteractionHandler_SwordBeam		handlerSwordBeam;
		private InteractionHandler_RodFire			handlerRodFire;
		private InteractionHandler_SwitchHook		handlerSwitchHook;
		private InteractionHandler_ThrownObject		handlerThrownObject;
		// Effects
		private InteractionHandler_Fire				handlerFire;
		private InteractionHandler_BombExplosion	handlerBombExplosion;

		private InteractionHandler[] interactionHandlers;

		private static InteractionStaticDelegate ToStaticInteractionDelegate(InteractionMemberDelegate memberDelegate) {
			return delegate(Monster monster, Entity sender, EventArgs args) {
				memberDelegate.Invoke(sender, args);
			};
		}

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

		public InteractionHandler GetInteraction(InteractionType type) {
			return interactionHandlers[(int) type];
		}
		/*
		public void SetReaction(InteractionType type, InteractionStaticDelegate reaction) {
			GetInteraction(type).Set(reaction);
		}

		public void SetReaction(InteractionType type, InteractionMemberDelegate reaction) {
			GetInteraction(type).Set(reaction);
		}
		*/
		public void SetReaction(InteractionType type, params InteractionStaticDelegate[] reactions) {
			InteractionHandler handler = GetInteraction(type);
			handler.Clear();
			for (int i = 0; i < reactions.Length; i++)
				handler.Add(reactions[i]);
		}

		public void SetReaction(InteractionType type, params InteractionMemberDelegate[] reactions) {
			InteractionHandler handler = GetInteraction(type);
			handler.Clear();
			for (int i = 0; i < reactions.Length; i++)
				handler.Add(reactions[i]);
		}

		public void TriggerInteraction(InteractionType type, Entity sender) {
			TriggerInteraction(type, sender, EventArgs.Empty);
		}

		public void TriggerInteraction(InteractionType type, Entity sender, EventArgs args) {
			InteractionHandler handler = GetInteraction(type);
			handler.Trigger(this, sender, args);
		}


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Monster() {
			// Physics.
			Physics.CollisionBox		= new Rectangle2I(-5, -9, 10, 10);
			Physics.SoftCollisionBox	= new Rectangle2I(-6, -11, 12, 11);
			Physics.CollideWithWorld	= true;
			Physics.CollideWithRoomEdge	= true;
			Physics.AutoDodges			= true;
			Physics.HasGravity			= true;
			Physics.IsDestroyedInHoles	= true;

			// With player
			// Top: 4 overlap
			// Bottom: 3 overlap?
			// Sides: 3 overlap	

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
			
			isKnockbackable	= true;
			isBurning		= false;
			effectAnimation	= new AnimationPlayer();

			interactionHandlers = new InteractionHandler[(int) InteractionType.Count];
			for (int i = 0; i < (int) InteractionType.Count; i++)
				interactionHandlers[i] = new InteractionHandler();

			// Interaction Handlers:
			// Player & items
			handlerButtonAction		= OnButtonAction;
			handlerSword			= OnSwordHit;
			handlerBigSword			= OnBigSwordHit;
			handlerShield			= OnShieldHit;
			handlerShovel			= null;
			handlerBracelet			= null;
			// Projectiles & Thrown objects
			handlerArrow			= OnArrowHit;
			handlerBoomerang		= OnBoomerangHit;
			handlerSwordBeam		= OnSwordBeamHit;
			handlerRodFire			= OnMagicRodFireHit;
			handlerSwitchHook		= OnSwitchHook;
			handlerThrownObject		= OnThrownObjectHit;
			// Effects
			handlerFire				= OnFireHit;
			handlerBombExplosion	= OnBombExplosionHit;
			// Seeds
			handlerSeeds = new InteractionHandler_Seed[5];
			handlerSeeds[(int) SeedType.Ember]		= OnEmberSeedHit;
			handlerSeeds[(int) SeedType.Scent]		= OnScentSeedHit;
			handlerSeeds[(int) SeedType.Gale]		= OnGaleSeedHit;
			handlerSeeds[(int) SeedType.Pegasus]	= OnPegasusSeedHit;
			handlerSeeds[(int) SeedType.Mystery]	= OnMysterySeedHit;
			
			// Seeds
			SetReaction(InteractionType.EmberSeed,		SenderReactions.Intercept);
			SetReaction(InteractionType.ScentSeed,		SenderReactions.Intercept);
			SetReaction(InteractionType.PegasusSeed,	SenderReactions.Intercept);
			SetReaction(InteractionType.GaleSeed,		SenderReactions.Intercept);
			SetReaction(InteractionType.MysterySeed,	SenderReactions.Intercept);
			// Projectiles
			SetReaction(InteractionType.Arrow,			SenderReactions.Intercept, Reactions.Damage);
			SetReaction(InteractionType.SwordBeam,		SenderReactions.Intercept, Reactions.Damage);
			SetReaction(InteractionType.Boomerang,		SenderReactions.Intercept, Reactions.Stun);
			SetReaction(InteractionType.SwitchHook,		SenderReactions.Intercept, Reactions.SwitchHook);
			SetReaction(InteractionType.RodFire,		SenderReactions.Intercept);
			// Environment
			SetReaction(InteractionType.Fire,			SenderReactions.Intercept, Reactions.Burn);
			SetReaction(InteractionType.BombExplosion,	Reactions.Damage);
			SetReaction(InteractionType.ThrownObject,	Reactions.Damage);
			SetReaction(InteractionType.MineCart,		Reactions.SoftKill);
			SetReaction(InteractionType.Block,			Reactions.Damage);
			// Tools
			SetReaction(InteractionType.Sword,			Reactions.DamageByLevel(1, 2, 3));
			SetReaction(InteractionType.BiggoronSword,	Reactions.Damage3);
			SetReaction(InteractionType.Shield,			Reactions.Bump); // BUMP(true)
			SetReaction(InteractionType.Shovel,			Reactions.Bump); // BUMP()
			// Player
			SetReaction(InteractionType.Pickup,			Reactions.None);
			SetReaction(InteractionType.ButtonAction,	Reactions.None);

			//SetReaction(InteractionType.SwordHitShield,			new COMBO(new EFFECT_CLING(), new BUMP(true)));
			//SetReaction(InteractionType.BiggoronSwordHitShield,	new COMBO(new EFFECT_CLING(), new BUMP()));
			//SetReaction(InteractionType.ShieldHitShield,			new COMBO(new EFFECT_CLING(), new BUMP(true)));
		}

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

		public override void OnKnockbackEnd() {
			base.OnKnockbackEnd();
			if (isBurning) {
				isBurning = false;
				effectAnimation.Animation = null;
			}
		}
		
		//-----------------------------------------------------------------------------
		// Interactions
		//-----------------------------------------------------------------------------
		/*
		public void TriggerInteraction(Delegate action, params object[] args) {
			if (action != null)
				action.DynamicInvoke(args);
		}*/
		
		protected virtual void OnButtonAction() {
			
		}
		
		protected virtual void OnSwitchHook(SwitchHookProjectile hook) {
			Hurt(1, hook.Position);
			hook.BeginReturn(false);
		}
		
		protected virtual void OnSwordHit(ItemSword itemSword) {
			Hurt(1, RoomControl.Player.Center);
		}
		
		protected virtual void OnBigSwordHit(ItemBigSword itemBigSword) {
			Hurt(2, RoomControl.Player.Center);
		}
		
		protected virtual void OnShieldHit(ItemShield itemShield) {
			Hurt(0, RoomControl.Player.Center); // Knockback
		}
		
		protected virtual void OnEmberSeedHit(SeedEntity seed) {
			//seed.DestroyWithEffect(SeedType.Ember, seed.Center);
			// Burn is handled by OnFireHit()
		}
		
		protected virtual void OnScentSeedHit(SeedEntity seed) {
			Hurt(1, seed.Center);
			seed.DestroyWithVisualEffect(SeedType.Scent, seed.Center);
		}
		
		protected virtual void OnGaleSeedHit(SeedEntity seed) {
			if (seed is SeedProjectile) {
				seed.DestroyWithVisualEffect(SeedType.Gale, Center);
			}
		}
		
		protected virtual void OnPegasusSeedHit(SeedEntity seed) {
			// Stun
			seed.DestroyWithVisualEffect(SeedType.Pegasus, seed.Center);
		}
		
		protected virtual void OnMysterySeedHit(SeedEntity seed) {
			/*
			// Random: burn, stun, damage, gale
			int rand = GRandom.NextInt(4);
			if (rand == 0)
				TriggerInteraction(handlerSeeds[(int) SeedType.Ember], seed);
			else if (rand == 1)
				TriggerInteraction(handlerSeeds[(int) SeedType.Scent], seed);
			else if (rand == 2)
				TriggerInteraction(handlerSeeds[(int) SeedType.Gale], seed);
			else
				TriggerInteraction(handlerSeeds[(int) SeedType.Pegasus], seed);
			*/
		}
		
		protected virtual void OnArrowHit(Arrow arrow) {
			Hurt(1, arrow.Center);
			arrow.Destroy();
		}
		
		protected virtual void OnSwordBeamHit(SwordBeam swordBeam) {
			Hurt(1, swordBeam.Center);
			swordBeam.Destroy();
		}
		
		protected virtual void OnBoomerangHit(Boomerang boomerang) {
			// TODO: Stun
			Hurt(1, boomerang.Center);
			boomerang.BeginReturn();
		}
		
		protected virtual void OnThrownObjectHit(CarriedTile thrownObject) {
			// Damage
			Hurt(1, thrownObject.Center);
		}
		
		protected virtual void OnFireHit(Fire fire) {
			// Burn
			// Burning is like stunning
			if (!IsInvincible && !isBurning) {
				Burn(1);
				fire.Destroy();
			}
		}
		
		protected virtual void OnMagicRodFireHit(MagicRodFire fire) {
			if (!IsInvincible && !isBurning) {
				Burn(1);
				fire.Destroy();
			}
		}
		
		protected virtual void OnBombExplosionHit(Effect explosion) {
			Hurt(1, explosion.Center);
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			health = healthMax;

			Graphics.PlayAnimation(GameData.ANIM_MONSTER_OCTOROK);
		}

		public override void Die() {
			Effect explosion = new Effect(GameData.ANIM_EFFECT_MONSTER_EXPLOSION, DepthLayer.EffectMonsterExplosion);
			RoomControl.SpawnEntity(explosion, Center);
			base.Die();
		}

		public override void UpdateGraphics() {
			base.UpdateGraphics();
			effectAnimation.Update();
		}

		public override void Update() {
			
			// 1. MonsterTools to PlayerTools
			// 2. MonsterTools to Player
			// 4. PlayerTools to Monster
			// 2. Monster to Player

			bool parry = false;
			Player player = RoomControl.Player;

			// Collide my tools with player's tools.
			foreach (UnitTool monsterTool in Tools) {
				if (parry)
					break;

				if (monsterTool.IsPhysicsEnabled) {
					foreach (UnitTool playerTool in player.Tools) {
						if (playerTool.IsPhysicsEnabled &&
							monsterTool.PositionedCollisionBox.Intersects(
							playerTool.PositionedCollisionBox))
						{
							if ((monsterTool.ToolType == UnitToolType.Sword || monsterTool.ToolType == UnitToolType.Shield) &&
								(playerTool.ToolType == UnitToolType.Sword || playerTool.ToolType == UnitToolType.Shield) &&
								!IsBeingKnockedBack && !player.IsBeingKnockedBack)
							{
								Vector2F contactPoint = playerTool.PositionedCollisionBox.Center;
								Bump(contactPoint);
								player.Bump(contactPoint);
								Effect effectCling = new Effect(GameData.ANIM_EFFECT_CLING, DepthLayer.EffectCling);
								RoomControl.SpawnEntity(effectCling, contactPoint);
								parry = true;
							}
						}
					}
				}
			}

			// Collide my tools with player.
			foreach (UnitTool tool in Tools) {
				if (parry)
					break;

				if (tool.IsPhysicsEnabled && tool.ToolType == UnitToolType.Sword) {
					if (player.Physics.PositionedSoftCollisionBox.Intersects(tool.PositionedCollisionBox))
						player.Hurt(contactDamage, Center);
				}
			}

			// Collide with player's tools.
			foreach (UnitTool tool in player.Tools) {
				if (parry || IsInvincible || IsBeingKnockedBack)
					break;

				if (tool.IsPhysicsEnabled && (tool.ToolType == UnitToolType.Sword || tool.ToolType == UnitToolType.Shield)) {
					if (Physics.PositionedSoftCollisionBox.Intersects(tool.PositionedCollisionBox)) {
						if (tool.ToolType == UnitToolType.Sword) {
							Hurt(1, player.Center);
						}
						else if (tool.ToolType == UnitToolType.Shield) {
							Vector2F contactPoint = (player.Center + Center) * 0.5f;
							if (!IsBeingKnockedBack)
								Bump(contactPoint);
							if (!player.IsBeingKnockedBack)
								player.Bump(contactPoint);
							parry = true;
						}
						//TriggerInteraction(HandlerSword, Weapon as ItemSword);
					}
				}
			}

			// Check collisions with player.
			if (!parry && physics.IsCollidingWith(player, CollisionBoxType.Soft)) {
				player.Hurt(contactDamage, Center);
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
		
		// Player & items
		public InteractionHandler_ButtonAction HandlerButtonAction {
			get { return handlerButtonAction; }
			set { handlerButtonAction = value; }
		}
		public InteractionHandler_Sword HandlerSword {
			get { return handlerSword; }
			set { handlerSword = value; }
		}
		public InteractionHandler_BigSword HandlerBigSword {
			get { return handlerBigSword; }
			set { handlerBigSword = value; }
		}
		public InteractionHandler_Shield HandlerShield {
			get { return handlerShield; }
			set { handlerShield = value; }
		}
		public InteractionHandler_Shovel HandlerShovel {
			get { return handlerShovel; }
			set { handlerShovel = value; }
		}
		public InteractionHandler_Bracelet HandlerBracelet {
			get { return handlerBracelet; }
			set { handlerBracelet = value; }
		}
		// Projectiles & Thrown objects
		public InteractionHandler_Seed[] HandlerSeeds {
			get { return handlerSeeds; }
			set { handlerSeeds = value; }
		}
		public InteractionHandler_Arrow HandlerArrow {
			get { return handlerArrow; }
			set { handlerArrow = value; }
		}
		public InteractionHandler_Boomerang HandlerBoomerang {
			get { return handlerBoomerang; }
			set { handlerBoomerang = value; }
		}
		public InteractionHandler_SwordBeam HandlerSwordBeam {
			get { return handlerSwordBeam; }
			set { handlerSwordBeam = value; }
		}
		public InteractionHandler_RodFire HandlerRodFire {
			get { return handlerRodFire; }
			set { handlerRodFire = value; }
		}
		public InteractionHandler_SwitchHook HandlerSwitchHook {
			get { return handlerSwitchHook; }
			set { handlerSwitchHook = value; }
		}
		public InteractionHandler_ThrownObject HandlerThrownObject {
			get { return handlerThrownObject; }
			set { handlerThrownObject = value; }
		}
		// Effects
		public InteractionHandler_Fire HandlerFire {
			get { return handlerFire; }
			set { handlerFire = value; }
		}
		public InteractionHandler_BombExplosion HandlerBombExplosion {
			get { return handlerBombExplosion; }
			set { handlerBombExplosion = value; }
		}
	}
}
