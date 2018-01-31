using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.GameStates.Transitions {
	
	public class RoomTransition : GameState {
		private RoomControl roomOld;
		private RoomControl roomNew;
		private event Action<RoomControl> eventSetupNewRoom; // Called right after the room is set up.


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public RoomTransition() {
			roomOld = null;
			roomNew = null;
			eventSetupNewRoom = null;
		}


		//-----------------------------------------------------------------------------
		// Room control
		//-----------------------------------------------------------------------------

		protected void EndTransition() {
			//gameManager.PopGameState(); // Pop this state
			//gameManager.PushGameState(roomNew);
			End();

			// Call this now incase it hasn't been called yet. (Like in push transition)
			SetupNewRoomPalette();
		}

		protected void DestroyOldRoom() {
			OldRoomControl.DestroyRoom();
		}

		protected void SetupNewRoom(bool setupPalette) {
			// Move the player from the old room to the new room.
			// RoomControl.BeginRoom() will update the player's RoomControl
			// reference.
			OldRoomControl.Entities.Remove(Player);
			NewRoomControl.BeginRoom();

			if (eventSetupNewRoom != null)
				eventSetupNewRoom.Invoke(roomNew);

			NewRoomControl.ViewControl.CenterOn(
				Player.Center + Player.ViewFocusOffset);

			// Mark the player's respawn point in the new room.
			Player.MarkRespawn();

			Player.OnEnterRoom();

			if (setupPalette) {
				SetupNewRoomPalette();
			}
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		private void SetupNewRoomPalette() {
			//GameData.PaletteShader.TilePalette = NewRoomControl.Zone.Palette;
			//GameData.PaletteShader.TileRatio = 0f;
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {}

		public override void Update() {}

		public override void Draw(Graphics2D g) {}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public Player Player {
			get { return GameControl.Player; }
		}

		public RoomControl NewRoomControl {
			get { return roomNew; }
			set { roomNew = value; }
		}
		
		public RoomControl OldRoomControl {
			get { return roomOld; }
			set { roomOld = value; }
		}
		
		public event Action<RoomControl> NewRoomSetup {
			add { eventSetupNewRoom += value; }
			remove { eventSetupNewRoom -= value; }
		}
	}
}
