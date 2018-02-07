using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.GameStates.Transitions;
using ZeldaOracle.Game.Entities.Effects;

namespace ZeldaOracle.Game.Entities.Players.States {
	public class PlayerUnderwaterState : PlayerState {

		// Used in OnEnterRoom() to know if we resurfaced into the room
		bool isResurfacing = false;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerUnderwaterState() {
			IsNaturalState = true;
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
				player.BeginState(player.NormalState);
			}
		}

		// Always allow state changes, because this is the "Normal" state for
		// underwater rooms. Every other state takes precidence over the player's
		// "Normal" state
		public override bool RequestStateChange(PlayerState newState) {
			return true;
		}

		public override void OnBegin(PlayerState previousState) {
			player.Movement.CanJump			= false;
			player.Movement.MoveSpeedScale	= 1.0f;
			player.Movement.AutoAccelerate	= false;
			player.MoveAnimation			= GameData.ANIM_PLAYER_MERMAID_SWIM;
			player.Graphics.PlayAnimation(player.MoveAnimation);

			isResurfacing = false;
		}
		
		public override void OnEnd(PlayerState newState) {
			player.Movement.CanJump			= true;
			player.Movement.MoveSpeedScale	= 1.0f;
			player.Movement.AutoAccelerate	= false;
			player.Graphics.DepthLayer		= DepthLayer.PlayerAndNPCs;
			player.MoveAnimation			= GameData.ANIM_PLAYER_DEFAULT;

			isResurfacing = false;
		}

		public override void Update() {

			// TODO: Code duplication with PlayerSwimState
			// TODO: magic numbers

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

			base.Update();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		// This is the threshhold of movement speed scale to be considered stroking.
		public bool IsStroking {
			get { return (player.Movement.MoveSpeedScale > 1.3f); }
		}

	}
}
