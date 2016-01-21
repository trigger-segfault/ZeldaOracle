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
			Player.MarkRespawn();
			Player.OnEnterRoom();
			End();
		}

		protected void DestroyOldRoom() {
			OldRoomControl.DestroyRoom();
		}
		
		protected void SetupNewRoom() {
			OldRoomControl.Entities.Remove(Player);
			NewRoomControl.BeginRoom();
			Player.RoomControl = NewRoomControl;
			if (eventSetupNewRoom != null)
				eventSetupNewRoom.Invoke(roomNew);
			NewRoomControl.ViewControl.CenterOn(Player.Center + Player.ViewFocusOffset);
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
