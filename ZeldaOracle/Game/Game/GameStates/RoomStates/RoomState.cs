using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.GameStates.RoomStates {
	public abstract class RoomState {

		protected GameControl gameControl;
		protected bool updateRoom;
		protected bool animateRoom;
		private bool isActive;
		private bool isVisible;

		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public RoomState() {
			updateRoom		= true;
			animateRoom		= true;
			isActive		= false;
			isVisible		= true;
		}

		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------

		public virtual void OnBegin() {}

		public virtual void OnEnd() {}

		public virtual void Update() {}

		public virtual void Draw(Graphics2D g) {}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		public void Begin(GameControl gameControl) {
			if (!isActive) {
				this.gameControl = gameControl;
				isActive = true;
				gameControl.UpdateRoom = updateRoom;
				gameControl.AnimateRoom = animateRoom;
				OnBegin();
			}
		}

		public void End() {
			if (isActive) {
				isActive = false;
				OnEnd();
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsActive {
			get { return isActive; }
			set { isActive = value; }
		}

		public bool IsVisible {
			get { return isVisible; }
			set { isVisible = value; }
		}

		public bool UpdateRoom {
			get { return updateRoom; }
			set { updateRoom = value; }
		}

		public bool AnimateRoom {
			get { return animateRoom; }
			set { animateRoom = value; }
		}

		public GameControl GameControl {
			get { return gameControl; }
			set { gameControl = value; }
		}

		public GameManager GameManager {
			get { return gameControl.GameManager; }
		}

		public RoomControl RoomControl {
			get { return gameControl.RoomControl; }
		}

		public virtual RoomState CurrentRoomState {
			get { return this; }
		}
	}
}
