using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.GameStates {

	public class GameStateStack : GameState {
		
        private List<GameState> states;
        

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

        public GameStateStack(params GameState[] states) {
            this.states = new List<GameState>();
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

		/// <summary>Pushes a single game state to the stack.</summary>
		public void Push(GameState state) {
			if (IsActive) {
				DeleteInactiveStates();
				if (!IsActive)
					return;
			}

            states.Add(state);
			
			if (IsActive) {
				state.Begin(gameManager);
				DeleteInactiveStates();
			}
        }

		/// <summary>Pops a single game state from the stack.</summary>
		public void Pop() {
			Pop(1);
        }

		/// <summary>Pops the specified amount of game states from the stack.</summary>
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

		/// <summary>Clears all game states from the stack.</summary>
		public void Clear() {
			while (states.Any())
				Pop(1);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

        public override void OnBegin() {

			// Begin all states from the bottom up.
            for (int i = 0; i < states.Count; i++) {
                states[i].Begin(gameManager);
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

		public override void AssignPalettes() {
			// PreDraw all states from the bottom up.
			for (int i = 0; i < states.Count; ++i) {
				states[i].AssignPalettes();
			}
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

		public override GameState CurrentGameState {
			get { return states.Last().CurrentGameState; }
		}
		
		/// <summary>Gets the number of game states in the stack.</summary>
		public int Count {
			get { return states.Count; }
		}
	}
}
