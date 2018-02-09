using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.GameStates.Transitions;

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerSwimEnvironmentState : PlayerJumpEnvironmentState {

		private bool	isSubmerged;
		private int		submergedTimer;
		private int		submergedDuration;
		private bool	isDiving;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerSwimEnvironmentState() {
			submergedDuration	= 128;
			isSubmerged			= false;
			submergedTimer		= 0;
			
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
		// Internal methods
		//-----------------------------------------------------------------------------

		// Check if it is possible to dive from the player's current location.
		private bool CanDive() {
			Level surfaceLevel = player.RoomControl.Level.ConnectedLevelBelow;
			if (surfaceLevel == null)
				return false;
			Point2I roomLocation = player.RoomControl.Room.Location;
			if (!surfaceLevel.ContainsRoom(roomLocation))
				return false;
			return true;
		}

		// Dive to the level below the current level. This will transition
		// to the room located directly below this room in the same room
		// location. The level below is expected to be underwater but this is
		// not required.
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
			}
		}

		public override void OnBegin(PlayerState previousState) {
			StateParameters.ProhibitJumping		= true;
			StateParameters.ProhibitWeaponUse	= true;
			//player.Movement.MoveSpeedScale = 1.0f;
			player.Movement.AutoAccelerate = false;

			player.InterruptWeapons();

			isDiving	= false;
			isSubmerged	= false;

			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_SWIM);

			// Create a splash effect.
			if (player.Physics.IsInLava) {
				Effect splash = new Effect(GameData.ANIM_EFFECT_LAVA_SPLASH, DepthLayer.EffectSplash, true);
				splash.Position = player.Center + new Vector2F(0, 4);
				player.RoomControl.SpawnEntity(splash);
			}
			else {
				Effect splash = new Effect(GameData.ANIM_EFFECT_WATER_SPLASH, DepthLayer.EffectSplash, true);
				splash.Position = player.Center + new Vector2F(0, 4);
				player.RoomControl.SpawnEntity(splash);
			}

			// Check if the player should drown.
			if ((player.Physics.IsInWater && !player.SwimmingSkills.HasFlag(PlayerSwimmingSkills.CanSwimInWater)) ||
				player.Physics.IsInOcean && !player.SwimmingSkills.HasFlag(PlayerSwimmingSkills.CanSwimInOcean))
			{
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_DROWN);
				player.RespawnDeath();
				// TODO: Cancel the hurt animation if the player was knocked in.
				//player.InvincibleTimer = 0;
				//player.Graphics.IsHurting = false;
			}
			else if (player.Physics.IsInLava && !player.SwimmingSkills.HasFlag(PlayerSwimmingSkills.CanSwimInLava)) {
				player.Graphics.IsHurting = true;
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_DROWN);
				player.RespawnDeath();
			}
			
			AudioSystem.PlaySound(GameData.SOUND_PLAYER_WADE);
		}
		
		public override void OnEnd(PlayerState newState) {
			player.Movement.CanJump			= true;
			//player.Movement.MoveSpeedScale	= 1.0f;
			player.Movement.AutoAccelerate	= false;
			player.Graphics.DepthLayer		= DepthLayer.PlayerAndNPCs;
			
			isDiving = false;
			
			if (isSubmerged) {
				isSubmerged = false;
				player.IsPassable = false;
			}
		}

		public override void Update() {

			// TODO: Code duplication with PlayerUnderwaterState
			// TODO: magic numbers
			/*
			// Update the submerge state.
			if (isSubmerged) {
				submergedTimer--;
				player.IsPassable = true;
				if (submergedTimer <= 0 || Controls.B.IsPressed()) {
					isSubmerged = false;
					player.IsPassable = false;
					player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_SWIM);
					player.Graphics.DepthLayer = DepthLayer.PlayerAndNPCs;
				}
			}
			else if (Controls.B.IsPressed()) {
				isSubmerged = true;
				player.IsPassable = true;
				submergedTimer = submergedDuration;
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_SUBMERGED);

				// Create a splash effect.
				Effect splash = new Effect(GameData.ANIM_EFFECT_WATER_SPLASH, DepthLayer.EffectSplash, true);
				splash.Position = player.Center + new Vector2F(0, 4);
				player.RoomControl.SpawnEntity(splash);
				
				AudioSystem.PlaySound(GameData.SOUND_PLAYER_WADE);

				// Change player depth to lowest.
				player.Graphics.DepthLayer = DepthLayer.PlayerSubmerged;

				if (player.Physics.IsInOcean && CanDive())
				{
					Dive();
					return;
				}
			}

			// Slow down movement over time from strokes
			if (player.Movement.MoveSpeedScale > 1.0f)
				player.Movement.MoveSpeedScale -= 0.025f;
			
			// Stroking scales the movement speed.
			if (player.Movement.MoveSpeedScale <= 1.4f && Controls.A.IsPressed()) {
				AudioSystem.PlaySound(GameData.SOUND_PLAYER_SWIM);
				player.Movement.MoveSpeedScale = 2.0f;
			}

			// Auto accelerate during the beginning of a stroke.
			player.Movement.AutoAccelerate = IsStroking;
			*/
			
			base.Update();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		// This is the threshhold of movement speed scale to be considered stroking.
		//public bool IsStroking {
		//	get { return (player.Movement.MoveSpeedScale > 1.3f); }
		//}

	}
}
