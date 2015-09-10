using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Graphics {
/** <summary>
 * A palette type used for foreground objects, items, and UI elements.
 * </summary> */
public class ForegroundPalette {

	//========== CONSTANTS ===========
	#region Constants

	/** <summary> The number of colors in a palette. </summary> */
	public const int NumPalettes = 12;

	#endregion
	//=========== MEMBERS ============
	#region Members

	/** <summary> The list of palettes in the set. </summary> */
	Palette[] palettes;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs a foreground palette set with the specified palettes. </summary> */
	public ForegroundPalette() {
		this.palettes = new Palette[NumPalettes];
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties

	/** <summary> Gets or sets the palette of the foreground set. </summary> */
	public Palette this[int index] {
		get { return this.palettes[index]; }
		set { this.palettes[index] = value; }
	}

	#endregion
}
}
