using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.GameStates.RoomStates;

namespace ZeldaOracle.Game.GameStates.RoomStates {
	
	public class RoomStateQueue : RoomState {
	
        private List<RoomState> states;
		private int stateIndex;
        

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public RoomStateQueue(params RoomState[] states) {
            this.states = new List<RoomState>();
            this.states.AddRange(states);
        }


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		public void Add(RoomState state) {
            states.Add(state);
        }

		public void AddRange(params RoomState[] states) {
			this.states.AddRange(states);
		}

		public void NextState() {
            stateIndex++;

            if (CurrentState != null) {
                CurrentState.Begin(gameControl);
                if (!CurrentState.IsActive)
                    NextState();
            }
            else
                End();
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

        public override void OnBegin() {
            stateIndex = 0;

            if (CurrentState != null)
            {
                CurrentState.Begin(gameControl);
                if (!CurrentState.IsActive)
                    NextState();
            }
            else
                End();
        }

        public override void Update() {
            if (CurrentState != null)
            {
                CurrentState.Update();
                if (!CurrentState.IsActive)
                    NextState();
            }
            else
                End();
        }

        public override void Draw(Graphics2D g) {
			if (CurrentState != null)
				CurrentState.Draw(g);
        }


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		private RoomState CurrentState {
            get {
                if (stateIndex >= states.Count)
                    return null;
                return states[stateIndex];
            }
        }

		public override RoomState CurrentRoomState {
			get { return CurrentState.CurrentRoomState; }
		}
	}
}
