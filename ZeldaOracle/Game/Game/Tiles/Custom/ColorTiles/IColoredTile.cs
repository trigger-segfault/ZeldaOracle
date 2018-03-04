using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ZeldaOracle.Game.API;

namespace ZeldaOracle.Game.Tiles {

	/// <summary>The colors types for use with puzzles.</summary>
	/*public enum PuzzleColor {
		/// <summary>No puzzle color.</summary>
		None = -1,
		/// <summary>Red puzzle color.</summary>
		Red,
		/// <summary>Yellow puzzle color.</summary>
		Yellow,
		/// <summary>Blue puzzle color.</summary>
		Blue
	}*/

	public interface IColoredTile {
		PuzzleColor Color { get; }
	}
}
