using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaAPI {
	
	// Puzzle colors.
	// NOTE: this currently mirrors the PuzzleColor enum and requires a matching order.
	// TODO: decouple this enum with PuzzleColor
	public enum Color {
		None = -1,
		Red,
		Yellow,
		Blue
	}
}
