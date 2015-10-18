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
		protected RoomControl	roomOld;
		protected RoomControl	roomNew;
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public RoomTransition() {
			roomOld = null;
			roomNew = null;
		}


		//-----------------------------------------------------------------------------
		// Room control
		//-----------------------------------------------------------------------------

		protected void EndTransition() {
			gameManager.PopGameState();
			gameManager.PushGameState(roomNew);
			Player.MarkRespawn();
			Player.OnEnterRoom();
		}

		protected void DestroyOldRoom() {
			OldRoomControl.DestroyRoom();
		}
		
		protected void SetupNewRoom() {
			OldRoomControl.Entities.Remove(Player);
			NewRoomControl.BeginRoom();
			Player.RoomControl = NewRoomControl;
			NewRoomControl.ViewControl.CenterOn(Player.Center);
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
	}
}
