using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Graphics {
/** <summary>
 * A 4-color palette for an object or tile.
 * </summary> */
public struct Palette {

	//========== CONSTANTS ===========
	#region Constants

	/** <summary> The number of colors in a palette. </summary> */
	public const int NumColors = 4;

	#endregion
	//=========== MEMBERS ============
	#region Members

	/** <summary> The 4 colors of the palette. </summary> */
	private Color[] colors;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs a palette with the specified colors. </summary> */
	public Palette(Color c1, Color c2, Color c3, Color c4) {
		this.colors = new Color[NumColors];
		this.colors[0] = c1;
		this.colors[1] = c2;
		this.colors[2] = c3;
		this.colors[3] = c4;
	}

	#endregion
	//=========== GENERAL ============
	#region General

	/** <summary> Outputs a string representing this color. </summary> */
	public override string ToString() {
		string text = "[";
		for (int i = 0; i < NumColors; i++) {
			text += this.colors[i].ToString();
			if (i + 1 < NumColors)
				text += ", ";
		}
		return text + "]";
	}
	/** <summary> Outputs a string representing this color. </summary> */
	public string ToString(IFormatProvider provider) {
		// TODO: Write formatting for Palette.ToString(format).
		string text = "[";
		for (int i = 0; i < NumColors; i++) {
			text += this.colors[i].ToString(provider);
			if (i + 1 < NumColors)
				text += ", ";
		}
		return text + "]";
	}
	/** <summary> Outputs a string representing this color. </summary> */
	public string ToString(string format, IFormatProvider provider) {
		string text = "[";
		for (int i = 0; i < NumColors; i++) {
			text += this.colors[i].ToString(format, provider);
			if (i + 1 < NumColors)
				text += ", ";
		}
		return text + "]";
	}
	/** <summary> Outputs a string representing this color. </summary> */
	public string ToString(string format) {
		string text = "[";
		for (int i = 0; i < NumColors; i++) {
			text += this.colors[i].ToString(format);
			if (i + 1 < NumColors)
				text += ", ";
		}
		return text + "]";
	}
	/** <summary> Returns true if the specified color has the same rgba values. </summary> */
	public override bool Equals(object obj) {
		if (obj is Palette) {
			for (int i = 0; i < NumColors; i++) {
				if (this.colors[i] != ((Palette)obj).colors[i])
					return false;
			}
			return true;
		}
		return false;
	}

	#endregion
	//========== OPERATORS ===========
	#region Operators

	public static bool operator ==(Palette a, Palette b) {
		for (int i = 0; i < NumColors; i++) {
			if (a.colors[i] != b.colors[i])
				return false;
		}
		return true;
	}
	public static bool operator !=(Palette a, Palette b) {
		for (int i = 0; i < NumColors; i++) {
			if (a.colors[i] != b.colors[i])
				return true;
		}
		return false;
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties

	/** <summary> Gets or sets the color of the palette. </summary> */
	public Color this[int index] {
		get { return this.colors[index]; }
		set { this.colors[index] = value; }
	}

	#endregion
}
}
