using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.GameStates.Transitions;

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

		// Check if it is possible to resurface from the player's current location.
		private bool CanResurface() {
			Level surfaceLevel = player.RoomControl.Level.ConnectedLevelAbove;
			if (surfaceLevel == null)
				return false;
			Point2I roomLocation = player.RoomControl.Room.Location;
			if (!surfaceLevel.ContainsRoom(roomLocation))
				return false;
			return true;
		}

		// Resurface to the level above the current level. This will transition
		// to the room located directly above this room in the same room
		// location.
		private void Resurface() {
			Level surfaceLevel = player.RoomControl.Level.ConnectedLevelAbove;
			Point2I roomLocation = player.RoomControl.Room.Location;
			Room connectedRoom = surfaceLevel.GetRoomAt(roomLocation);

			isResurfacing = true;

			player.Movement.StopMotion();
			player.RoomControl.TransitionToRoom(
				connectedRoom,
				new RoomTransitionFade());
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnEnterRoom() {
			if (isResurfacing) {
				// Snap the player's position to the nearest tile location.
				player.Position = GMath.Floor(player.Position /
					new Vector2F(GameSettings.TILE_SIZE)) *
					new Vector2F(GameSettings.TILE_SIZE) +
					new Vector2F(GameSettings.TILE_SIZE / 2) - player.CenterOffset;

				// Change to standing animation and face downwards.
				//player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_DEFAULT);
				player.Direction = Directions.Down;
				End();
			}
		}

		public override void OnBegin(PlayerState previousState) {
			//player.Movement.MoveSpeedScale	= 1.0f;
			player.Movement.AutoAccelerate	= false;
			player.MoveAnimation			= GameData.ANIM_PLAYER_MERMAID_SWIM;
			player.Graphics.PlayAnimation(player.MoveAnimation);

			isResurfacing = false;
		}
		
		public override void OnEnd(PlayerState newState) {
			//player.Movement.MoveSpeedScale	= 1.0f;
			player.Movement.AutoAccelerate	= false;
			player.Graphics.DepthLayer		= DepthLayer.PlayerAndNPCs;
			player.MoveAnimation			= GameData.ANIM_PLAYER_DEFAULT;

			isResurfacing = false;
		}

		public override void Update() {

			// TODO: Code duplication with PlayerSwimState
			// TODO: magic numbers
			/*
			// Press B to attempt to resurface.
			if (Controls.B.IsPressed() && CanResurface()) {
				Resurface();
				return;
			}

			// Slow down movement over time from strokes
			if (player.Movement.MoveSpeedScale > 1.0f)
				player.Movement.MoveSpeedScale -= 0.025f;

			// Stroking scales the movement speed.
			// Press A to stroke, but this will not work if an item is usable
			// in slot A.
			if (player.Movement.MoveSpeedScale <= 1.4f &&
				Controls.A.IsPressed() && 
				(player.EquippedUsableItems[Inventory.SLOT_A] == null || 
				!player.EquippedUsableItems[Inventory.SLOT_A].IsUsable()))
			{
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
			//get { return (player.Movement.MoveSpeedScale > 1.3f); }
		//}

	}
}
