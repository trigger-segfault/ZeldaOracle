using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.GameStates.RoomStates;

namespace ZeldaOracle.Game.GameStates.RoomStates {

	public class RoomStateStack : RoomState {
		
        private List<RoomState> states;
        

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

        public RoomStateStack(params RoomState[] states) {
            this.states = new List<RoomState>();
            this.states.AddRange(states);
        }


		//-----------------------------------------------------------------------------
		// Internal
		//-----------------------------------------------------------------------------

		private void DeleteInactiveStates() {
			int count = states.Count;

			// Chop off any hanging states.
            for (int i = 0; i < states.Count; i++) {
                if (!states[i].IsActive && i < count) {
                    count = i;
					break;
				}
            }

            for (int i = count; i < states.Count; i++) {
                states[i].End();
				states.RemoveAt(i--);
			}

			if (count <= 0)
				End();
		}

		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

        public void Push(RoomState state) {
			if (IsActive) {
				DeleteInactiveStates();
				if (!IsActive)
					return;
			}

            states.Add(state);
			
			if (IsActive) {
				state.Begin(gameControl);
				DeleteInactiveStates();
			}
        }
		
        public void Pop() {
			Pop(1);
        }
		
        public void Pop(int amount) {
			if (IsActive) {
				DeleteInactiveStates();
				if (!IsActive)
					return;
			}

			for (int i = 0; i < amount && states.Count > 0; i++) {
				if (IsActive)
					states.Last().End();
				states.RemoveAt(states.Count - 1);
				if (IsActive) {
					DeleteInactiveStates();
					if (!IsActive)
						return;
				}
			}
        }


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

        public override void OnBegin() {

			// Begin all states from the bottom up.
            for (int i = 0; i < states.Count; i++) {
				states[i].Begin(gameControl);
                if (!states[i].IsActive)
                    break;
            }

			DeleteInactiveStates();
        }

        public override void Update() {
			DeleteInactiveStates();
			if (!IsActive)
				return;

			// Only update the top state.
			states.Last().Update();

			DeleteInactiveStates();
        }

        public override void Draw(Graphics2D g) {
			// Draw all states from the bottom up.
            for (int i = 0; i < states.Count; ++i) {
				states[i].Draw(g);
            }
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public override RoomState CurrentRoomState {
			get { return states.Last().CurrentRoomState; }
		}
	}
}
