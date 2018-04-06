using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Translation {

	/// <summary>A letter string formatted and wrapped into multiple lines.</summary>
	public class WrappedLetterString : IEnumerable<LetterString> {

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
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public IEnumerator<LetterString> GetEnumerator() {
			foreach (LetterString line in Lines)
				yield return line;
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
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
