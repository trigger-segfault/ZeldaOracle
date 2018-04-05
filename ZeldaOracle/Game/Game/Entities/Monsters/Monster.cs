using System;
using System.Collections.Generic;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Units;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Game.Entities.Monsters.States;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Monsters {
	
	public enum MonsterColor {
		Red			= 0,
		Blue		= 1,
		Green		= 2,
		Orange		= 3,
		Gold		= 4,
		DarkRed		= 5,
		DarkBlue	= 6,
		InverseBlue	= 7,

		Count,
	}

	public partial class Monster : Unit, ZeldaAPI.Monster {

		// Settings -------------------------------------------------------------------

		private MonsterColor color;
		private int contactDamage;
		protected bool softKill;
		protected bool isBurnable;
		protected bool isGaleable;
		protected bool isStunnable;
		
		// States ---------------------------------------------------------------------

		/// <summary>The current monster state.</summary>
		private MonsterState state;
		/// <summary>The previous monster state.</summary>
		private MonsterState previousState;

		//private MonsterBurnState			stateBurn;
		//private MonsterStunState			stateStun;
		//private MonsterFallInHoleState	stateFallInHole;
		//private MonsterGaleState			stateGale;

		// Property settings that should not modify the properties --------------------

		/// <summary>The ID of the monster used for respawn identification.</summary>
		private int monsterID;
		/// <summary>True if the monster does not need to be killed in order to
		/// clear the room.</summary>
		private bool ignoreMonster;
		/// <summary>True if the monster is ignored for room respawn but still counts
		/// towards the room's cleared event.</summary>
		private bool ignoreRespawn;
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Monster() {
			// Entity
			Properties = new Properties();

			// Graphics
			Graphics.DepthLayer         = DepthLayer.Monsters;
			Graphics.DepthLayerInAir    = DepthLayer.InAirMonsters;
			Graphics.DrawOffset         = new Point2I(-8, -14);
			centerOffset                = new Point2I(0, -6);

			// Physics
			Physics.CollisionBox        = new Rectangle2I(-5, -9, 10, 10);
			Physics.CollideWithWorld    = true;
			Physics.CollideWithRoomEdge = true;
			Physics.AutoDodges          = true;
			Physics.HasGravity          = true;
			Physics.IsDestroyedInHoles  = true;

			// Interactions
			Interactions.Enable();
			Interactions.InteractionBox = new Rectangle2I(-6, -11, 12, 11);
			SetDefaultReactions();

			// Unit settings
			knockbackSpeed          = GameSettings.MONSTER_KNOCKBACK_SPEED;
			hurtKnockbackDuration   = GameSettings.MONSTER_HURT_KNOCKBACK_DURATION;
			bumpKnockbackDuration   = GameSettings.MONSTER_BUMP_KNOCKBACK_DURATION;
			hurtInvincibleDuration  = GameSettings.MONSTER_HURT_INVINCIBLE_DURATION;
			hurtFlickerDuration     = GameSettings.MONSTER_HURT_FLICKER_DURATION;
			isKnockbackable         = true;
			
			// Monster
			contactDamage	= 1;
			color			= MonsterColor.Red;
			isGaleable		= true;
			isBurnable		= true;
			isStunnable		= true;
			softKill		= false;
		}

		protected void SetDefaultReactions() {
			Interactions.ClearReactions();

			// Weapon Reactions
			Reactions[InteractionType.Sword]
				.Add(MonsterReactions.DamageByLevel(1, 2, 3))
				.Add(SenderReactions.Intercept);
			Reactions[InteractionType.SwordSpin].Add(MonsterReactions.Damage2);
			Reactions[InteractionType.BiggoronSword].Add(MonsterReactions.Damage3);
			Reactions[InteractionType.SwordStrafe]
				.Add(MonsterReactions.DamageByLevel(1, 2, 3))
				.Add(SenderReactions.Intercept);
			Reactions[InteractionType.Shield]
				.Add(SenderReactions.Bump).Add(MonsterReactions.Bump);
			Reactions[InteractionType.Shovel].Add(MonsterReactions.Bump);

			// Seed Reactions
			Reactions[InteractionType.EmberSeed].Add(SenderReactions.Intercept);
			Reactions[InteractionType.ScentSeed]
				.Add(SenderReactions.Intercept).Add(MonsterReactions.SilentDamage);
			Reactions[InteractionType.PegasusSeed]
				.Add(SenderReactions.Intercept).Add(MonsterReactions.Stun);
			Reactions[InteractionType.GaleSeed].Add(SenderReactions.Intercept);
			Reactions[InteractionType.MysterySeed].Add(MonsterReactions.MysterySeed);

			// Projectile Reactions
			Reactions[InteractionType.Arrow]
				.Add(SenderReactions.Destroy).Add(MonsterReactions.Damage);
			Reactions[InteractionType.SwordBeam]
				.Add(SenderReactions.Destroy).Add(MonsterReactions.Damage);
			Reactions[InteractionType.RodFire].Add(SenderReactions.Intercept);
			Reactions[InteractionType.Boomerang]
				.Add(SenderReactions.Intercept).Add(MonsterReactions.Stun);
			Reactions[InteractionType.SwitchHook].Add(MonsterReactions.SwitchHook);

			// Environment Reactions
			Reactions[InteractionType.Fire].Add(MonsterReactions.Burn);
			Reactions[InteractionType.Gale].Add(MonsterReactions.Gale);
			Reactions[InteractionType.BombExplosion].Add(MonsterReactions.Damage2);
			Reactions[InteractionType.ThrownObject].Add(MonsterReactions.Damage);
			Reactions[InteractionType.MineCart].Add(MonsterReactions.SoftKill);
			Reactions[InteractionType.MagnetBall].Add(MonsterReactions.Kill); // TODO: Confirm  this
			Reactions[InteractionType.Block].Add(MonsterReactions.Damage2);

			// Player interations
			Reactions[InteractionType.PlayerContact].Set(
				delegate(Entity sender, EventArgs args)
			{
				if (!IsStunned)
					OnTouchPlayer(sender, args);
			});
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

		public virtual void UpdateAI() {}

		public void BeginSpawnState(int duration = GameSettings.MONSTER_SPAWN_STATE_DURATION) {
			BeginState(new MonsterSpawnState(duration));
		}


		//-----------------------------------------------------------------------------
		// Monster Reactions
		//-----------------------------------------------------------------------------

		public virtual void OnStun() {}

		public virtual void OnBurn() {}

		public virtual void OnBurnComplete() {}

		public virtual void OnElectrocute() {}

		public virtual void OnElectrocuteComplete() {}

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
			if (Rectangle2F.Translate(Interactions.InteractionBox, spawnPosition)
				.Intersects(RoomControl.Player.Interactions.InteractionBox))
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

			color = Properties.GetEnum<MonsterColor>("color", color);

			health = healthMax;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_OCTOROK);

			// Begin the default monster state
			BeginNormalState();
			previousState = state;

			monsterID		= Properties.Get("monster_id", 0);
			ignoreMonster	= Properties.Get("ignore_monster", false);
			ignoreRespawn	= ignoreMonster;
		}

		public override void Die() {
			CreateDeathEffect();
			// Dead is now only used for permenant deaths. Everything else is
			// tracked by the area's respawn manager.
			if (!softKill) {
				if (RespawnMode == MonsterRespawnType.Never)
					Properties.Set("dead", true);
				else if (RespawnMode == MonsterRespawnType.Normal)
					RoomControl.ClearMonster(MonsterID);
			}
			else {
				// Prevent the room from being "respawn cleared"
				RoomControl.AddSoftKill(this);
			}
			base.Die();
		}

		public override void OnHurt(DamageInfo damage) {
			if (damage.PlaySound)
				AudioSystem.PlaySound(GameData.SOUND_MONSTER_HURT);
		}

		public override void OnFallInHole() {
			// Begin the fall-in-hole state (slipping into a hole)
			if (state is MonsterNormalState) {
				BeginState(new MonsterFallInHoleState());
			}
		}

		public override void Update() {
			// Update the current monster state
			state.Update();

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
			//get { return Properties.Get("ignore_monster", false); }
			//set { Properties.Set("ignore_monster", value); }
			get { return ignoreMonster; }
			set { ignoreMonster = value; }
		}

		/// <summary>True if the monster if ignored for room respawn but still counts
		/// towards the room's cleared event.</summary>
		public bool IgnoreRespawn {
			get { return ignoreRespawn; }
			set { ignoreRespawn = value; }
		}

		/// <summary>Gets the respawn type of the monster.</summary>
		public MonsterRespawnType RespawnType {
			get { return Properties.GetEnum("respawn_type", MonsterRespawnType.Normal); }
		}

		/// <summary>Gets if the monster monster should be marked as dead in properties.</summary>
		/*public bool IsPermenantDeath {
			get {
				return (RespawnType == MonsterRespawnType.Never ||
					(AreaControl.RespawnMode == RoomRespawnMode.Never &&
					RespawnType == MonsterRespawnType.Normal));
			}
		}*/

		/// <summary>Gets if the monster monster should be marked as dead in properties.</summary>
		/*public bool AlwaysRespawns {
			get {
				return (RespawnType == MonsterRespawnType.Always ||
					AreaControl.RespawnMode == RoomRespawnMode.Always) &&
					!IsPermenantDeath;
			}
		}*/

		/// <summary>Gets the monsters respawn type combined with the
		/// area's respawn mode.</summary>
		public MonsterRespawnType RespawnMode {
			get {
				if (RespawnType == MonsterRespawnType.Normal) {
					switch (AreaControl.RespawnMode) {
					case RoomRespawnMode.Never: return MonsterRespawnType.Never;
					case RoomRespawnMode.Always: return MonsterRespawnType.Always;
					default: return MonsterRespawnType.Normal;
					}
				}
				return RespawnType;
			}
		}

		/// <summary>Gets the ID unique to each monster in the room.
		/// An ID of -1 means the monster is never registered for death.</summary>
		public int MonsterID {
			//get { return Properties.Get("monster_id", -1); }
			//set { Properties.Set("monster_id", value); }
			get { return monsterID; }
			set { monsterID = value; }
		}

		/// <summary>True if this monster needs to be killed in order to clear the room.</summary>
		public bool NeedsClearing {
			get { return !IgnoreMonster /*&& IsAlive*/; }
		}
	}
}
