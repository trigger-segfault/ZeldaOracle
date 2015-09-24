using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.GameStates {

	// This is just a hack around empty game-state stacks.
	public class StateDummy : GameState {

		public StateDummy() {}
		
		public override void OnBegin() {}
		
		public override void Update() {}
		
		public override void Draw(Graphics2D g) {}
	}
}
