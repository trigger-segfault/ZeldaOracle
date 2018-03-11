using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ZeldaOracle.Game.API;

namespace ZeldaOracle.Game.Tiles {
	
	public interface IColoredTile {
		PuzzleColor Color { get; }
	}
}
