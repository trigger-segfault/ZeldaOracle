using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.GameStates.Transitions;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerUnderwaterEnvironmentState : PlayerEnvironmentState {

		// Used in OnEnterRoom() to know if we resurfaced into the room
		bool isResurfacing = false;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerUnderwaterEnvironmentState() {
			StateParameters.ProhibitJumping	= true;
			StateParameters.ProhibitPushing	= true;

			PlayerAnimations.Default		= GameData.ANIM_PLAYER_MERMAID_SWIM;
			PlayerAnimations.Aim			= GameData.ANIM_PLAYER_MERMAID_AIM;
			PlayerAnimations.Throw			= GameData.ANIM_PLAYER_MERMAID_THROW;
			PlayerAnimations.Swing			= GameData.ANIM_PLAYER_MERMAID_SWING;
			PlayerAnimations.SwingNoLunge	= GameData.ANIM_PLAYER_MERMAID_SWING;
			PlayerAnimations.Spin			= GameData.ANIM_PLAYER_MERMAID_SPIN;
			PlayerAnimations.Stab			= GameData.ANIM_PLAYER_MERMAID_STAB;

			MotionSettings.MovementSpeed		= 0.5f;
			MotionSettings.IsSlippery			= true;
			MotionSettings.Acceleration			= 0.08f;
			MotionSettings.Deceleration			= 0.05f;
			MotionSettings.MinSpeed				= 0.05f;
			MotionSettings.DirectionSnapCount	= 32;
		}
		

		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		/// <summary>Check if it is possible to resurface from the player's current
		/// location.</summary>
		public bool CanResurface() {
			Level surfaceLevel = player.RoomControl.Level.ConnectedLevelAbove;
			if (surfaceLevel == null)
				return false;
			Point2I roomLocation = player.RoomControl.Room.Location;
			if (!surfaceLevel.ContainsRoom(roomLocation))
				return false;
			return true;
		}

		/// <summary>Resurface to the level above the current level. This will
		/// transition to the room located directly above this room in the same room
		/// location.</summary>
		public void Resurface() {
			Level surfaceLevel = player.RoomControl.Level.ConnectedLevelAbove;
			Point2I roomLocation = player.RoomControl.Room.Location;
			Room connectedRoom = surfaceLevel.GetRoomAt(roomLocation);

			isResurfacing = true;

			player.Movement.StopMotion();
			player.RoomControl.TransitionToRoom(
				connectedRoom, new RoomTransitionFade());
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnEnterRoom() {
			if (isResurfacing) {
				// Snap the player's position to the nearest tile location
				player.Position = GMath.Floor(player.Position /
					new Vector2F(GameSettings.TILE_SIZE)) *
					new Vector2F(GameSettings.TILE_SIZE) +
					new Vector2F(GameSettings.TILE_SIZE / 2) - player.CenterOffset;

				// Change to standing animation and face downwards
				player.InterruptWeapons();
				player.Direction = Direction.Down;
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_DEFAULT);
				End();
			}
		}

		public override void OnBegin(PlayerState previousState) {
			isResurfacing = false;
		}
		
		public override void OnEnd(PlayerState newState) {
			isResurfacing = false;
		}
	}
}
