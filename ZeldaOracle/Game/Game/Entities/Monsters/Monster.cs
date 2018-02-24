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
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Monsters {

	public enum MonsterRespawnType {
		Never	= 0,
		Always	= 1,
		Normal	= 2,
	}

	public enum MonsterColor {
		Red			= 0,
		Blue		= 1,
		Green		= 2,
		Orange		= 3,
		Gold		= 4,
		DarkRed		= 5,
		DarkBlue	= 6,
		InverseBlue	= 7,

		Count
	}

	// MonsterState:
	// - MonsterBurnState
	// - MonsterGaleState
	// - MonsterStunState
	// - MonsterFallInHoleState

	public partial class Monster : Unit, ZeldaAPI.Monster {
		
		private int contactDamage;
		private InteractionHandler[] interactionHandlers;
		private MonsterColor color;

		// The current monster state.
		private MonsterState state;
		// The previous monster state.
		private MonsterState previousState;

		protected bool softKill;
		protected bool isBurnable;
		protected bool isGaleable;
		protected bool isStunnable;
		protected bool isFlying;
		/// <summary>True if the player will always check collisions
		/// as if the monsters z position was at zero.</summary>
		protected bool ignoreZPosition;

		// States.
		/*private MonsterBurnState		stateBurn;
		private MonsterStunState		stateStun;
		private MonsterFallInHoleState	stateFallInHole;
		private MonsterGaleState		stateGale;*/



		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Monster() {
			// With player:
			// - Top: 4 overlap
			// - Bottom: 3 overlap?
			// - Sides: 3 overlap	

			color = MonsterColor.Red;
			Properties = new Properties();

			// Physics
			Physics.CollisionBox        = new Rectangle2I(-5, -9, 10, 10);
			Physics.SoftCollisionBox    = new Rectangle2I(-6, -11, 12, 11);
			Physics.CollideWithWorld    = true;
			Physics.CollideWithRoomEdge = true;
			Physics.AutoDodges          = true;
			Physics.HasGravity          = true;
			Physics.IsDestroyedInHoles  = true;

			// Graphics
			Graphics.DepthLayer         = DepthLayer.Monsters;
			Graphics.DepthLayerInAir    = DepthLayer.InAirMonsters;
			Graphics.DrawOffset         = new Point2I(-8, -14);
			centerOffset                = new Point2I(0, -6);

			// Monster & unit settings
			knockbackSpeed          = GameSettings.MONSTER_KNOCKBACK_SPEED;
			hurtKnockbackDuration   = GameSettings.MONSTER_HURT_KNOCKBACK_DURATION;
			bumpKnockbackDuration   = GameSettings.MONSTER_BUMP_KNOCKBACK_DURATION;
			hurtInvincibleDuration  = GameSettings.MONSTER_HURT_INVINCIBLE_DURATION;
			hurtFlickerDuration     = GameSettings.MONSTER_HURT_FLICKER_DURATION;
			contactDamage           = 1;
			isGaleable              = true;
			isBurnable              = true;
			isStunnable             = true;
			isKnockbackable         = true;
			isFlying                = false;
			softKill                = false;
			ignoreZPosition			= false;

			// Setup default interactions.
			interactionHandlers = new InteractionHandler[(int) InteractionType.Count];
			for (int i = 0; i < (int) InteractionType.Count; i++)
				interactionHandlers[i] = new InteractionHandler();
			SetDefaultReactions();
		}

		protected void SetDefaultReactions() {
			// Weapon interations
			SetReaction(InteractionType.Sword,			SenderReactions.Intercept, Reactions.DamageByLevel(1, 2, 3));
			SetReaction(InteractionType.SwordSpin,		Reactions.Damage2);
			SetReaction(InteractionType.BiggoronSword,	Reactions.Damage3);
			SetReaction(InteractionType.Shield,			SenderReactions.Bump,		Reactions.Bump);
			SetReaction(InteractionType.Shovel,			Reactions.Bump);
			SetReaction(InteractionType.Parry,			Reactions.Parry);
			SetReaction(InteractionType.Pickup,			Reactions.None);
			// Seed interations
			SetReaction(InteractionType.EmberSeed,		SenderReactions.Intercept);
			SetReaction(InteractionType.ScentSeed,		SenderReactions.Intercept,	Reactions.SilentDamage);
			SetReaction(InteractionType.PegasusSeed,	SenderReactions.Intercept,	Reactions.Stun);
			SetReaction(InteractionType.GaleSeed,		SenderReactions.Intercept);
			SetReaction(InteractionType.MysterySeed,	Reactions.MysterySeed);
			// Projectile interations
			SetReaction(InteractionType.Arrow,			SenderReactions.Destroy,	Reactions.Damage);
			SetReaction(InteractionType.SwordBeam,		SenderReactions.Destroy,	Reactions.Damage);
			SetReaction(InteractionType.RodFire,		SenderReactions.Intercept);
			SetReaction(InteractionType.Boomerang,		SenderReactions.Intercept,	Reactions.Stun);
			SetReaction(InteractionType.SwitchHook,		Reactions.SwitchHook);
			// Environment interations
			SetReaction(InteractionType.Fire,			Reactions.Burn);
			SetReaction(InteractionType.Gale,			Reactions.Gale);
			SetReaction(InteractionType.BombExplosion,	Reactions.Damage);
			SetReaction(InteractionType.ThrownObject,	Reactions.Damage);
			SetReaction(InteractionType.MineCart,		Reactions.SoftKill);
			SetReaction(InteractionType.Block,			Reactions.Damage);
			// Player interations
			SetReaction(InteractionType.ButtonAction,	Reactions.None);
			SetReaction(InteractionType.PlayerContact,	OnTouchPlayer);
		}


		//-----------------------------------------------------------------------------
		// Monster States and Behavior
		//-----------------------------------------------------------------------------

		// Begin the given monster state.
		public void BeginState(MonsterState newState) {
			if (state != newState) {
				if (state != null) {
					state.End(newState);
				}
				previousState = state;
				state = newState;
				newState.Begin(this, previousState);
			}
		}

		public void BeginNormalState() {
			BeginState(new MonsterNormalState());
		}

		public virtual void UpdateAI() {

		}

		public void BeginSpawnState(int duration = GameSettings.MONSTER_SPAWN_STATE_DURATION) {
			BeginState(new MonsterSpawnState(duration));
		}


		//-----------------------------------------------------------------------------
		// Monster Reactions
		//-----------------------------------------------------------------------------

		public virtual void OnStun() { }

		public virtual void OnBurn() { }

		public virtual void OnBurnComplete() { }

		public virtual void OnElectrocute() { }

		public virtual void OnElectrocuteComplete() { }

		public void SoftKill() {
			softKill = true;
			Kill();
		}

		public bool Stun(int damage = 0) {
			if (isStunnable && ((state is MonsterNormalState) || (state is MonsterStunState))) {
				OnStun();
				AudioSystem.PlaySound(GameData.SOUND_MONSTER_HURT);
				BeginState(new MonsterStunState(GameSettings.MONSTER_STUN_DURATION));
				if (damage > 0) {
					DamageInfo damageInfo = new DamageInfo(damage);
					damageInfo.ApplyKnockback = false;
					Hurt(damageInfo);
				}
				return true;
			}
			return false;
		}

		public bool EnterGale(EffectGale gale) {
			if (isGaleable && ((state is MonsterNormalState) || (state is MonsterStunState))) {
				BeginState(new MonsterGaleState(gale));
				return true;
			}
			return false;
		}

		public virtual bool Burn(int damage) {
			if (!IsInvincible && isBurnable &&
				((state is MonsterNormalState) || (state is MonsterStunState))) {
				OnBurn();
				BeginState(new MonsterBurnState(damage));
				return true;
			}
			return false;
		}

		public virtual void OnTouchPlayer(Entity sender, EventArgs args) {
			Player player = (Player) sender;
			player.Hurt(contactDamage, Center);
		}

		public virtual void OnSeedHit(SeedEntity seed) {
			seed.TriggerMonsterReaction(this);
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

		protected void SetReaction(InteractionType type,
			InteractionStaticDelegate staticReaction,
			params InteractionMemberDelegate[] memberReactions) {
			InteractionHandler handler = GetInteraction(type);
			handler.Clear();
			handler.Add(staticReaction);
			for (int i = 0; i < memberReactions.Length; i++)
				handler.Add(memberReactions[i]);
		}

		protected void SetReaction(InteractionType type,
			InteractionStaticDelegate staticReaction1,
			InteractionStaticDelegate staticReaction2,
			params InteractionMemberDelegate[] memberReactions) {
			InteractionHandler handler = GetInteraction(type);
			handler.Clear();
			handler.Add(staticReaction1);
			handler.Add(staticReaction2);
			for (int i = 0; i < memberReactions.Length; i++)
				handler.Add(memberReactions[i]);
		}

		protected void SetReaction(InteractionType type,
			InteractionStaticDelegate staticReaction1,
			InteractionStaticDelegate staticReaction2,
			InteractionStaticDelegate staticReaction3,
			params InteractionMemberDelegate[] memberReactions) {
			InteractionHandler handler = GetInteraction(type);
			handler.Clear();
			handler.Add(staticReaction1);
			handler.Add(staticReaction2);
			handler.Add(staticReaction3);
			for (int i = 0; i < memberReactions.Length; i++)
				handler.Add(memberReactions[i]);
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		protected virtual void FacePlayer() {
			Vector2F lookVector = RoomControl.Player.Center - Center;
			direction = Direction.FromVector(lookVector);
		}

		public virtual bool CanSpawnAtLocation(Point2I location) {
			Vector2F spawnPosition =
				(location * GameSettings.TILE_SIZE) +
				new Vector2F(8, 8) - centerOffset;

			// Check for solid tiles
			Tile tile = RoomControl.GetTopTile(location);
			if (tile != null && (tile.IsSolid || tile.IsHoleWaterOrLava))
				return false;

			// Check for touching the player
			if (Physics.IsPlaceMeetingEntity(spawnPosition,
				RoomControl.Player, CollisionBoxType.Soft))
				return false;

			return true;
		}

		public virtual Vector2F GetRandomSpawnLocation() {

			List<Vector2F> locations = new List<Vector2F>();

			for (int x = 0; x < RoomControl.Room.Width; x++) {
				for (int y = 0; y < RoomControl.Room.Height; y++) {
					Vector2F spawnPosition =
						(new Point2I(x, y) * GameSettings.TILE_SIZE) +
						new Vector2F(8, 8) - centerOffset;
					if (CanSpawnAtLocation(new Point2I(x, y)))
						locations.Add(spawnPosition);
				}
			}

			if (locations.Count == 0)
				return position;
			return GRandom.Choose(locations);
		}

		public virtual void CreateDeathEffect() {
			Effect explosion = new Effect(
				GameData.ANIM_EFFECT_MONSTER_EXPLOSION,
				DepthLayer.EffectMonsterExplosion);
			AudioSystem.PlaySound(GameData.SOUND_MONSTER_DIE);
			RoomControl.SpawnEntity(explosion, Center);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			color = (MonsterColor) Properties.GetInteger("color", (int) color);

			health = healthMax;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_OCTOROK);

			// Begin the default monster state
			BeginNormalState();
			previousState = state;
		}

		public override void Die() {
			CreateDeathEffect();
			if (!softKill)
				Properties.Set("dead", true);
			base.Die();
		}

		public override void OnHurt(DamageInfo damage) {
			if (damage.PlaySound)
				AudioSystem.PlaySound(GameData.SOUND_MONSTER_HURT);
		}

		public override void OnFallInHole() {
			// Begin the fall-in-hole state (slipping into a hole)
			if (!isFlying && state is MonsterNormalState) {
				BeginState(new MonsterFallInHoleState());
			}
		}

		public override void OnFallInWater() {
			if (!isFlying)
				base.OnFallInWater();
		}

		public override void OnFallInLava() {
			if (!isFlying)
				base.OnFallInLava();
		}

		private void CollideMonsterAndPlayer() {
			Player player = RoomControl.Player;

			float tempZPosition = ZPosition;
			if (ignoreZPosition)
				ZPosition = 0;

			// Check for minecart interactions
			if (!isPassable && player.IsInMinecart &&
				physics.IsCollidingWith(player, CollisionBoxType.Soft)) {
				TriggerInteraction(InteractionType.MineCart, player);
				if (IsDestroyed)
					return;
			}

			if (isPassable || player.IsPassable || GMath.Abs(player.ZPosition - zPosition) > 10) // TODO: magic number
				return;

			IEnumerable<UnitTool> monsterTools = EquippedTools.Where(t => t.IsPhysicsEnabled && t.IsSwordOrShield);
			IEnumerable<UnitTool> playerTools = player.EquippedTools.Where(t => t.IsPhysicsEnabled && t.IsSwordOrShield);

			// 1. (M-M) MonsterTools to PlayerTools
			// 2. (M-1) MonsterTools to Player
			// 4. (M-1) PlayerTools to Monster
			// 2. (1-1) Monster to Player

			// Collide my tools with the player's tools
			if (!IsStunned) {
				foreach (UnitTool monsterTool in monsterTools) {
					foreach (UnitTool playerTool in playerTools) {
						if (monsterTool.PositionedCollisionBox.Intersects(playerTool.PositionedCollisionBox)) {
							Vector2F contactPoint = playerTool.PositionedCollisionBox.Center;

							TriggerInteraction(InteractionType.Parry, player, new ParryInteractionArgs() {
								ContactPoint    = contactPoint,
								MonsterTool     = monsterTool,
								SenderTool      = playerTool
							});

							playerTool.OnParry(this, contactPoint);

							RoomControl.SpawnEntity(new EffectCling(), contactPoint);

							return;
						}
					}
				}
			}

			// Collide my tools with the player
			bool parry = false;
			if (!player.IsInvincible && player.IsDamageable && !IsStunned) {
				foreach (UnitTool tool in monsterTools) {
					if (player.Physics.PositionedSoftCollisionBox.Intersects(tool.PositionedCollisionBox)) {
						player.Hurt(contactDamage, Center);
						parry = true;
						break;
					}
				}
			}

			// Collide with the player's tools
			foreach (UnitTool tool in playerTools) {
				if (Physics.PositionedSoftCollisionBox.Intersects(tool.PositionedCollisionBox)) {
					tool.OnHitMonster(this);
					parry = true;
					break;
				}
			}

			// Check collisions with player
			if (!parry && !IsStunned &&
				physics.IsCollidingWith(player, CollisionBoxType.Soft)) {
				TriggerInteraction(InteractionType.PlayerContact, player);
			}

			if (ignoreZPosition)
				ZPosition = tempZPosition;
		}

		public override void Update() {
			// Update the current monster state
			state.Update();

			// Collide player and monster and their tools
			CollideMonsterAndPlayer();

			base.Update();
		}

		public override void Draw(RoomGraphics g) {
			state.DrawUnder(g);
			base.Draw(g);
			state.DrawOver(g);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int ContactDamage {
			get { return contactDamage; }
			set { contactDamage = value; }
		}

		public bool IsStunned {
			get { return (state is MonsterStunState); }
		}

		public MonsterState CurrentState {
			get { return state; }
		}

		public MonsterColor Color {
			get { return color; }
			set {
				color = value;
				Graphics.ColorDefinitions.SetAll(
					GameData.MONSTER_COLOR_DEFINITION_MAP[(int) color]);
			}
		}

		/// <summary>True if this monster is not counted towards clearing a room.</summary>
		public bool IgnoreMonster {
			get { return Properties.Get("ignore_monster", false); }
			set { Properties.Set("ignore_monster", value); }
		}

		/// <summary>True if this monster needs to be killed in order to clear the room.</summary>
		public bool NeedsClearing {
			get { return !IgnoreMonster && IsAlive; }
		}
	}
}
