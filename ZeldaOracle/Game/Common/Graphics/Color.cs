using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using XnaColor		= Microsoft.Xna.Framework.Color;

using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics {
/** <summary>
 * A structure for storing color data.
 * </summary> */
public struct Color {

	//========== CONSTANTS ===========
	#region Constants

	/** <summary> R:255 G:255 B:255 A:255. </summary> */
	public static Color White {
		get { return new Color(255, 255, 255, 255); }
	}

	/** <summary> R:0 G:0 B:0 A:255. </summary> */
	public static Color Black {
		get { return new Color(0, 0, 0, 255); }
	}

	/** <summary> R:255 G:255 B:255 A:0. </summary> */
	public static Color TransparentWhite {
		get { return new Color(255, 255, 255, 0); }
	}

	/** <summary> R:0 G:0 B:0 A:0. </summary> */
	public static Color Transparent {
		get { return new Color(0, 0, 0, 0); }
	}

	// R:255 G:0 B:0 A:255.
	public static Color Red {
		get { return new Color(255, 0, 0); }
	}

	// R:0 G:255 B:0 A:255.
	public static Color Green {
		get { return new Color(0, 255, 0); }
	}

	// R:0 G:0 B:255 A:255.
	public static Color Blue {
		get { return new Color(0, 0, 255); }
	}

	#endregion
	//=========== MEMBERS ============
	#region Members

	/** <summary> The alpha value of the color. </summary> */
	private byte a;
	/** <summary> The red value of the color. </summary> */
	private byte r;
	/** <summary> The green value of the color. </summary> */
	private byte g;
	/** <summary> The blue value of the color. </summary> */
	private byte b;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs a color with the specified values. </summary> */
	public Color(int red, int green, int blue, bool asHsv = false) {
		if (asHsv) {
			this.r = 0;
			this.g = 0;
			this.b = 0;
			this.a = 255;
			this.SetHSV(red, green, blue);
		}
		else {
			this.r	= (byte)GMath.Clamp(red, 0, 255);
			this.g	= (byte)GMath.Clamp(green, 0, 255);
			this.b	= (byte)GMath.Clamp(blue, 0, 255);
			this.a	= 255;
		}
	}
	/** <summary> Constructs a color with the specified values. </summary> */
	public Color(int red, int green, int blue, int alpha, bool asHsv = false) {
		if (asHsv) {
			this.r = 0;
			this.g = 0;
			this.b = 0;
			this.a = (byte)GMath.Clamp(alpha, 0, 255);
			this.SetHSV(red, green, blue);
		}
		else {
			this.r	= (byte)GMath.Clamp(red, 0, 255);
			this.g	= (byte)GMath.Clamp(green, 0, 255);
			this.b	= (byte)GMath.Clamp(blue, 0, 255);
			this.a	= (byte)GMath.Clamp(alpha, 0, 255);
		}
	}

	/** <summary> Constructs a color with the specified values. </summary> */
	public Color(float red, float green, float blue, bool asHsv = false) {
		if (asHsv) {
			this.r = 0;
			this.g = 0;
			this.b = 0;
			this.a = 255;
			this.SetHSV(red, green, blue);
		}
		else {
			this.r	= (byte)(GMath.Clamp(red, 0f, 1f) * 255f);
			this.g	= (byte)(GMath.Clamp(green, 0f, 1f) * 255f);
			this.b	= (byte)(GMath.Clamp(blue, 0f, 1f) * 255f);
			this.a	= 255;
		}
	}
	/** <summary> Constructs a color with the specified values. </summary> */
	public Color(float red, float green, float blue, float alpha, bool asHsv = false) {
		if (asHsv) {
			this.r	= 0;
			this.g	= 0;
			this.b	= 0;
			this.a	= (byte)(GMath.Clamp(alpha, 0f, 1f) * 255f);
			this.SetHSV(red, green, blue);
		}
		else {
			this.r	= (byte)(GMath.Clamp(red, 0f, 1f) * 255f);
			this.g	= (byte)(GMath.Clamp(green, 0f, 1f) * 255f);
			this.b	= (byte)(GMath.Clamp(blue, 0f, 1f) * 255f);
			this.a	= (byte)(GMath.Clamp(alpha, 0f, 1f) * 255f);
		}
	}

	/** <summary> Constructs a color with the specified values. </summary> */
	public Color(double red, double green, double blue, bool asHsv = false) {
		if (asHsv) {
			this.r = 0;
			this.g = 0;
			this.b = 0;
			this.a = 255;
			this.SetHSV(red, green, blue);
		}
		else {
			this.r	= (byte)(GMath.Clamp(red, 0.0, 1.0) * 255.0);
			this.g	= (byte)(GMath.Clamp(green, 0.0, 1.0) * 255.0);
			this.b	= (byte)(GMath.Clamp(blue, 0.0, 1.0) * 255.0);
			this.a	= 255;
		}
	}
	/** <summary> Constructs a color with the specified values. </summary> */
	public Color(double red, double green, double blue, double alpha, bool asHsv = false) {
		if (asHsv) {
			this.r	= 0;
			this.g	= 0;
			this.b	= 0;
			this.a	= (byte)(GMath.Clamp(alpha, 0.0, 1.0) * 255.0);
			this.SetHSV(red, green, blue);
		}
		else {
			this.r	= (byte)(GMath.Clamp(red, 0.0, 1.0) * 255.0);
			this.g	= (byte)(GMath.Clamp(green, 0.0, 1.0) * 255.0);
			this.b	= (byte)(GMath.Clamp(blue, 0.0, 1.0) * 255.0);
			this.a	= (byte)(GMath.Clamp(alpha, 0.0, 1.0) * 255.0);
		}
	}
	/** <summary> Constructs a copy of the specified color. </summary> */
	public Color(Color color) {
		this.r	= color.r;
		this.g	= color.g;
		this.b	= color.b;
		this.a	= color.a;
	}

	#endregion
	//=========== GENERAL ============
	#region General

	/** <summary> Outputs a string representing this color. </summary> */
	public override string ToString() {
		return "(" + r + ", " + g + ", " + b + ", " + a + ")";
	}
	/** <summary> Outputs a string representing this color. </summary> */
	public string ToString(IFormatProvider provider) {
		// TODO: Write formatting for Color.ToString(format).

		return "(" + r + ", " + g + ", " + b + ", " + a + ")";
	}
	/** <summary> Outputs a string representing this color. </summary> */
	public string ToString(string format, IFormatProvider provider) {
		return "(" + r + ", " + g + ", " + b + ", " + a + ")";
	}
	/** <summary> Outputs a string representing this color. </summary> */
	public string ToString(string format) {
		return "(" + r + ", " + g + ", " + b + ", " + a + ")";
	}
	/** <summary> Returns true if the specified color has the same rgba values. </summary> */
	public override bool Equals(object obj) {
		if (obj is Color)
			return (r == ((Color)obj).r && g == ((Color)obj).g && b == ((Color)obj).b && a == ((Color)obj).a);
		return false;
	}
	/** <summary> Returns the hash code for this color. </summary> */
	public override int GetHashCode() {
		return base.GetHashCode();
	}
	/** <summary> Parses the color. </summary> */
	public static Color Parse(string text) {
		Color value = Color.White;

		if (text.Length > 0) {
			if (text[0] == '(')
				text = text.Substring(1);
			if (text[text.Length - 1] == ')')
				text = text.Substring(0, text.Length - 1);

			int lastCommaPos	= -1;
			int commaPos		= -1;

			for (int i = 0; i < 4; i++) {
				lastCommaPos	= commaPos;
				commaPos		= text.IndexOf(',', lastCommaPos + 1);
				//if (commaPos == -1)
				//	commaPos	= text.IndexOf(' ', lastCommaPos + 1);
				if (commaPos == -1 && i >= 2) {
					commaPos = text.Length;
				}

				if (commaPos != -1) {
					string str = text.Substring(lastCommaPos + 1, commaPos - (lastCommaPos + 1));

					try {
						byte v = Byte.Parse(str);
						switch (i) {
						case 0: value.r = v; break;
						case 1: value.g = v; break;
						case 2: value.b = v; break;
						case 3: value.a = v; break;
						}
					}
					catch (FormatException e) {
						throw e;
					}
					catch (ArgumentNullException e) {
						throw e;
					}
					catch (OverflowException e) {
						throw e;
					}
				}
				else if (i < 2) {
					throw new FormatException();
				}

				if (commaPos == text.Length) {
					return value;
				}
			}
		}
		else {
			throw new ArgumentNullException();
		}

		return value;
	}

	#endregion
	//========== OPERATORS ===========
	#region Operators

	public static explicit operator Color(XnaColor c) {
		return new Color(c.R, c.G, c.B, c.A);
	}
	public static implicit operator XnaColor(Color c) {
		return new XnaColor(c.r, c.g, c.b, c.a);
	}
	public static bool operator ==(Color a, Color b) {
		return (a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a);
	}
	public static bool operator !=(Color a, Color b) {
		return (a.r != b.r || a.g != b.g || a.b != b.b || a.a != b.a);
	}
	public static Color operator *(Color a, float scalar) {
		return new Color(
			(byte) (GMath.Clamp((a.r / 255.0f) * scalar, 0.0f, 1.0f) * 255),
			(byte) (GMath.Clamp((a.g / 255.0f) * scalar, 0.0f, 1.0f) * 255),
			(byte) (GMath.Clamp((a.b / 255.0f) * scalar, 0.0f, 1.0f) * 255),
			(byte) (GMath.Clamp((a.a / 255.0f) * scalar, 0.0f, 1.0f) * 255));
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region RGBA

	/** <summary> Gets or sets the red value of the color. </summary> */
	[ContentSerializer(Optional = true)]
	public byte R {
		get { return r; }
		set { r = value; }
	}
	/** <summary> Gets or sets the green value of the color. </summary> */
	[ContentSerializer(Optional = true)]
	public byte G {
		get { return g; }
		set { g = value; }
	}
	/** <summary> Gets or sets the blue value of the color. </summary> */
	[ContentSerializer(Optional = true)]
	public byte B {
		get { return b; }
		set { b = value; }
	}
	/** <summary> Gets or sets the alpha value of the color. </summary> */
	[ContentSerializer(Optional = true)]
	public byte A {
		get { return a; }
		set { a = value; }
	}
	/** <summary> Gets the packed color with all the values inside an unsigned int. </summary> */
	public uint PackedColor {
		get { return (((uint)a) << 12) + (((uint)r) << 8) + (((uint)g) << 8) + ((uint)b); }
	}

	#endregion
	//--------------------------------
	#region HSV

	/** <summary> Gets or sets hue of the color. </summary> */
	[ContentSerializer(Optional = true)]
	public int Hue {
		get {
			byte max = GMath.Max(r, GMath.Max(g, b));

			if (max == r)
				return   0 + ((int)(g - b) * 60 / 255);
			else if (max == g)
				return 120 + ((int)(b - r) * 60 / 255);
			else
				return 240 + ((int)(r - g) * 60 / 255);
		}
		set { SetHSV(value, Sat, Val); }
	}
	/** <summary> Gets or sets the saturation of the color. </summary> */
	[ContentSerializer(Optional = true)]
	public int Sat {
		get {
			byte max = (byte)((int)GMath.Max(r, GMath.Max(g, b)) * 100 / 255);
			byte min = (byte)((int)GMath.Min(r, GMath.Min(g, b)) * 100 / 255);
			byte c = (byte)(max - min);
			return (c != 0 ? c * 100 / max : 0);
		}
		set { SetHSV(Hue, value, Val); }
	}
	/** <summary> Gets or sets the brightness of the color. </summary> */
	[ContentSerializer(Optional = true)]
	public int Val {
		get { return ((int)GMath.Max(r, GMath.Max(g, b)) * 100 / 255); }
		set { SetHSV(Hue, Sat, value); }
	}

	#endregion
	//--------------------------------
	#endregion
	//========== MANAGEMENT ==========
	#region Management
	//--------------------------------
	#region RGB

	/** <summary> Sets the rgb values of the color. </summary> */
	public void SetRGB(int red, int green, int blue) {
		r = (byte)GMath.Clamp(red, 0, 255);
		g = (byte)GMath.Clamp(green, 0, 255);
		b = (byte)GMath.Clamp(blue, 0, 255);
	}
	/** <summary> Sets the rgb values of the color. </summary> */
	public void SetRGB(float red, float green, float blue) {
		r = (byte)(GMath.Clamp(red, 0f, 1f) * 255f);
		g = (byte)(GMath.Clamp(green, 0f, 1f) * 255f);
		b = (byte)(GMath.Clamp(blue, 0f, 1f) * 255f);
	}
	/** <summary> Sets the rgb values of the color. </summary> */
	public void SetRGB(double red, double green, double blue) {
		r = (byte)(GMath.Clamp(red, 0.0, 1.0) * 255.0);
		g = (byte)(GMath.Clamp(green, 0.0, 1.0) * 255.0);
		b = (byte)(GMath.Clamp(blue, 0.0, 1.0) * 255.0);
	}

	#endregion
	//--------------------------------
	#region HSV

	/** <summary> Sets the hsv values of the color. </summary> */
	public void SetHSV(int hue, int sat, int val) {
		SetHSV((float)hue / 360f, (float)sat / 100f, (float)val / 100f);
	}
	/** <summary> Sets the hsv values of the color. </summary> */
	public void SetHSV(float hue, float sat, float val) {
		hue = GMath.Clamp(hue, 0f, 1f);
		sat = GMath.Clamp(sat, 0f, 1f);
		val = GMath.Clamp(val, 0f, 1f);
		float hue2 = hue * 6f;
		float hueI = (float)GMath.Floor(hue2);

		byte b0 = (byte)(255f * val);
		byte b1 = (byte)(255f * val * (1f - sat));
		byte b2 = (byte)(255f * val * (1f - sat * (hue2 - hueI)));
		byte b3 = (byte)(255f * val * (1f - sat * (1f - (hue2 - hueI))));

		switch ((int)hueI) {
		case 0: r = b0; g = b3; b = b1; break;
		case 1: r = b2; g = b0; b = b1; break;
		case 2: r = b1; g = b0; b = b3; break;
		case 3: r = b1; g = b2; b = b0; break;
		case 4: r = b3; g = b1; b = b0; break;
		case 5: r = b0; g = b1; b = b2; break;
		}
	}
	/** <summary> Sets the hsv values of the color. </summary> */
	public void SetHSV(double hue, double sat, double val) {
		SetHSV((float)hue, (float)sat, (float)val);
	}

	#endregion
	//--------------------------------
	#endregion
}
} // End namespace
