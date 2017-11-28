using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Translation {

	/// <summary>A letter string formatted and wrapped into multiple lines.</summary>
	public class WrappedLetterString {

		/// <summary>The formatted lines of the wrapped string.</summary>
		public LetterString[] Lines { get; set; }
		/// <summary>The lengths of each line.</summary>
		public int[] LineLengths { get; set; }
		/// <summary>The bounds of the entire wrapped string.</summary>
		public Rectangle2I Bounds { get; set; }


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the default wrapped string.</summary>
		public WrappedLetterString() {
			this.Lines = null;
			this.LineLengths = null;
			this.Bounds = Rectangle2I.Zero;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the number of lines in the wrapped string.</summary>
		public int LineCount {
			get { return Lines.Length; }
		}
	}
}
