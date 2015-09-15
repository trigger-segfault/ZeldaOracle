using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Translation {

	// A letter string formatted and wrapped into multiple lines.
	public class WrappedLetterString {

		// The formatted lines of the wrapped string.
		public LetterString[] Lines;
		// The lengths of each line.
		public int[] LineLengths;
		// The bounds of the entire wrapped string.
		public Rectangle2I Bounds;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		// Constructs the default wrapped string.
		public WrappedLetterString() {
			this.Lines = null;
			this.LineLengths = null;
			this.Bounds = Rectangle2I.Zero;
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Gets the number of lines in the wrapped string
		public int NumLines {
			get { return Lines.Length; }
		}
	}
}
