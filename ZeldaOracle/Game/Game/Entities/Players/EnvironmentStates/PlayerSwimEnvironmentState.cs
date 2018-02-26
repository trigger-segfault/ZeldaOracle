using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.GameStates.Transitions;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerSwimEnvironmentState : PlayerEnvironmentState {

		private bool	isSubmerged;
		private int		submergedTimer;
		private int		submergedDuration;
		private bool	isDiving;
		private bool	silentBeginning;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerSwimEnvironmentState() {
			submergedDuration	= 128;
			isSubmerged			= false;
			submergedTimer		= 0;
			silentBeginning     = false;

			StateParameters.ProhibitJumping		= true;
			StateParameters.ProhibitPushing		= true;
			StateParameters.ProhibitWeaponUse	= true;
			StateParameters.DisableAnimationPauseWhenNotMoving = true;
			
			MotionSettings = new PlayerMotionType() {
				MovementSpeed			= 0.5f,
				IsSlippery				= true,
				Acceleration			= 0.08f,
				Deceleration			= 0.05f,
				MinSpeed				= 0.05f,
				DirectionSnapCount		= 32,
			};
		}


		//-----------------------------------------------------------------------------
		// Submerge/Resurface Actions
		//-----------------------------------------------------------------------------

		/// <summary>Submerge or dive.</summary>
		public void Submerge() {
			PlayerAnimations.Default = GameData.ANIM_PLAYER_SUBMERGED;
			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_SUBMERGED);

			// Create a splash effect
			CreateSplashEffect();

			// Change player depth to lowest
			player.Graphics.DepthLayer = DepthLayer.PlayerSubmerged;

			if (player.Physics.IsInOcean && CanDive()) {
				Dive();
			}
			else {
				isSubmerged = true;
				submergedTimer = submergedDuration;
				StateParameters.DisableInteractionCollisions = true;
			}
		}
		
		public void Resurface() {
			isSubmerged = false;
			player.Graphics.DepthLayer = DepthLayer.PlayerAndNPCs;
			PlayerAnimations.Default = GameData.ANIM_PLAYER_SWIM;
			StateParameters.DisableInteractionCollisions = false;
		}
		

		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Create the water or lava splash effect at the player's position.
		/// </summary>
		private void CreateSplashEffect() {
			Animation effectAnimation = GameData.ANIM_EFFECT_WATER_SPLASH;
			if (player.Physics.IsInLava)
				effectAnimation = GameData.ANIM_EFFECT_LAVA_SPLASH;

			Effect splash = new Effect(effectAnimation, DepthLayer.EffectSplash, true);
			splash.Position = player.Center + new Vector2F(0, 4);
			player.RoomControl.SpawnEntity(splash);
			
			AudioSystem.PlaySound(GameData.SOUND_PLAYER_WADE);
		}

		/// <summary>Returns true if the player has the capability to swim in the
		/// liquid he is currently in.</summary>
		private bool CanSwimInCurrentLiquid() {
			if (player.Physics.IsInLava)
				return player.SwimmingSkills.HasFlag(PlayerSwimmingSkills.CanSwimInLava);
			else if (player.Physics.IsInOcean)
				return player.SwimmingSkills.HasFlag(PlayerSwimmingSkills.CanSwimInOcean);
			else if (player.Physics.IsInWater)
				return player.SwimmingSkills.HasFlag(PlayerSwimmingSkills.CanSwimInWater);
			else
				return false;
		}

		/// <summary>Make the player die by drowning.</summary>
		private void Drown() {
			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_DROWN);
			
			if (player.Physics.IsInLava)
				player.Graphics.IsHurting = true;

			player.RespawnDeath();
		}

		/// <summary>Returns true if it is possible to dive from the player's current
		/// location.</summary>
		private bool CanDive() {
			Level surfaceLevel = player.RoomControl.Level.ConnectedLevelBelow;
			if (surfaceLevel == null)
				return false;
			Point2I roomLocation = player.RoomControl.Room.Location;
			if (!surfaceLevel.ContainsRoom(roomLocation))
				return false;
			return true;
		}

		/// <summary>Dive to the level below the current level. This will transition to
		/// the room located directly below this room in the same room location. The
		/// level below is expected to be underwater but this is not required.
		/// </summary>
		private void Dive() {
			Level surfaceLevel = player.RoomControl.Level.ConnectedLevelBelow;
			Point2I roomLocation = player.RoomControl.Room.Location;
			Room connectedRoom = surfaceLevel.GetRoomAt(roomLocation);

			isDiving = true;

			player.Movement.StopMotion();
			player.RoomControl.TransitionToRoom(
				connectedRoom,
				new RoomTransitionFade());
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnEnterRoom() {
			if (isDiving) {
				// Snap the player's position to the tile he is standing on.
				player.Position = GMath.Floor(player.Position /
					new Vector2F(GameSettings.TILE_SIZE)) *
					new Vector2F(GameSettings.TILE_SIZE) +
					new Vector2F(GameSettings.TILE_SIZE / 2) - player.CenterOffset;

				// Change to standing animation and face downwards.
				player.Direction = Directions.Down;
				player.BeginEnvironmentState(player.UnderwaterState);
				player.IntegrateStateParameters();
				player.Graphics.PlayAnimation(player.Animations.Default);
			}
		}

		public override void OnBegin(PlayerState previousState) {
			PlayerAnimations.Default = GameData.ANIM_PLAYER_SWIM;
			StateParameters.DisableInteractionCollisions = false;

			player.InterruptWeapons();

			isDiving	= false;
			isSubmerged	= false;

			if (!silentBeginning) {
				CreateSplashEffect();

				// Check if the player should drown
				if (!CanSwimInCurrentLiquid()) {
					Drown();
					// TODO: Cancel the hurt animation if the player was knocked in.
					//player.InvincibleTimer = 0;
					//player.Graphics.IsHurting = false;
				}
			}
			else {
				silentBeginning = false;
			}
		}
		
		public override void OnEnd(PlayerState newState) {
			player.Graphics.DepthLayer = DepthLayer.PlayerAndNPCs;
			isDiving = false;
			isSubmerged = false;
		}

		public override void Update() {
			// Update the submerge state
			if (isSubmerged) {
				submergedTimer--;
				if (submergedTimer <= 0)
					Resurface();
			}
			
			// Check if the player is trying to swim in a bad liquid
			if (!CanSwimInCurrentLiquid()) {
				CreateSplashEffect();
				Drown();
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public bool IsSubmerged {
			get { return isSubmerged; }
		}

		public bool SilentBeginning {
			get { return silentBeginning; }
			set { silentBeginning = value; }
		}
	}
}
