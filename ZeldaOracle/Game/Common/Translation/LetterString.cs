using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Common.Translation {

	/// <summary>A structure that contains either a string, LetterString or WrappedLetterString.</summary>
	public struct DrawableString {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>A null drawable string.</summary>
		public static readonly DrawableString Null = new DrawableString((string) null);


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The object storing either a string, LetterString, or WrappedLetterString.
		/// This value can be null.</summary>
		private object value;
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a string.</summary>
		public DrawableString(string text) {
			value = text;
		}

		/// <summary>Constructs a LetterString.</summary>
		public DrawableString(LetterString letterString) {
			value = letterString;
		}

		/// <summary>Constructs a WrappedLetterString.</summary>
		public DrawableString(WrappedLetterString letterString) {
			value = letterString;
		}

		
		//-----------------------------------------------------------------------------
		// General
		//-----------------------------------------------------------------------------

		/// <summary>Gets the hash code for the text.</summary>
		public override int GetHashCode() {
			return value.GetHashCode();
		}

		/// <summary>Returns true if the object is equal to the string,
		/// LetterString, or WrappedString.</summary>
		public override bool Equals(object obj) {
			if (obj is string)
				return (string) value == (string) obj;
			else if (obj is LetterString)
				return value == obj;
			else if (obj is WrappedLetterString)
				return value == obj;
			else if (obj is DrawableString)
				return this == ((DrawableString) obj);
			else if (obj == null)
				return value == null;
			return false;
		}


		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

		public static bool operator ==(DrawableString a, DrawableString b) {
			if (a.value is string && b.value is string)
				return (string) a.value == (string) b.value;
			else
				return a.value == b.value;
		}

		public static bool operator !=(DrawableString a, DrawableString b) {
			if (a.value is string && b.value is string)
				return (string) a.value != (string) b.value;
			else
				return a.value != b.value;
		}


		//-----------------------------------------------------------------------------
		// Implicit Conversion
		//-----------------------------------------------------------------------------

		public static implicit operator DrawableString(string text) {
			return new DrawableString(text);
		}

		public static implicit operator DrawableString(LetterString letterString) {
			return new DrawableString(letterString);
		}

		public static implicit operator DrawableString(WrappedLetterString letterString) {
			return new DrawableString(letterString);
		}
		

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the value is null.</summary>
		public bool IsNull {
			get { return value == null; }
		}

		/// <summary>Returns true if the value is a string.</summary>
		public bool IsString {
			get { return value is string; }
		}

		/// <summary>Gets the value as a string.</summary>
		public string String {
			get { return value as string; }
		}

		/// <summary>Returns true if the value is a LetterString.</summary>
		public bool IsLetterString {
			get { return value is LetterString; }
		}

		/// <summary>Gets the value as a LetterString.</summary>
		public LetterString LetterString {
			get { return value as LetterString; }
		}

		/// <summary>Returns true if the value is a WrappedLetterString.</summary>
		public bool IsWrappedLetterString {
			get { return value is WrappedLetterString; }
		}

		/// <summary>Gets the value as a WrappedLetterString.</summary>
		public WrappedLetterString WrappedLetterString {
			get { return value as WrappedLetterString; }
		}
	}

	/// <summary>A formatted game letter with a color and character</summary>
	public struct Letter {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The default color of a letter.</summary>
		public static readonly Color DefaultColor = ZeldaOracle.Common.Graphics.Color.White;


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The character of the letter.</summary>
		public char Char { get; private set; }
		/// <summary>The color or paletted color of the letter.</summary>
		public ColorOrPalette Color { get; private set; }


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a letter with the specified character and default color.</summary>
		public Letter(char character) {
			this.Char = character;
			this.Color = DefaultColor;
		}
		
		/// <summary>Constructs a letter with the specified character and color.</summary>
		public Letter(char character, ColorOrPalette color) {
			this.Char = character;
			this.Color = color;
		}


		//-----------------------------------------------------------------------------
		// General
		//-----------------------------------------------------------------------------

		/// <summary>Gets the letter as just a string.</summary>
		public override string ToString() {
			return "" + Char;
		}

		/// <summary>Gets the letter as just a string.</summary>
		public string ToString(IFormatProvider provider) {
			return "" + Char;
		}

		/// <summary>Gets the letter as just a string.</summary>
		public string ToString(string format, IFormatProvider provider) {
			return "" + Char;
		}

		/// <summary>Gets the letter as just a string.</summary>
		public string ToString(string format) {
			return "" + Char;
		}

		/// <summary>Returns true if the specified letter is equal to the letter.</summary>
		public override bool Equals(object obj) {
			if (obj is Letter)
				return (Char == ((Letter) obj).Char && Color == ((Letter) obj).Color);
			return false;
		}

		/// <summary>This keeps the compiler from giving a warning.</summary>
		public override int GetHashCode() {
			return base.GetHashCode();
		}


		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

		public static bool operator ==(Letter l1, Letter l2) {
			return (l1.Char == l2.Char) && (l1.Color == l2.Color);
		}

		public static bool operator !=(Letter l1, Letter l2) {
			return (l1.Char != l2.Char) || (l1.Color != l2.Color);
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Gets the final (mapped) color with the chosen tile and entity palette.</summary>
		public Color GetMappedColor(Palette tilePalette, Palette entityPalette) {
			return Color.GetMappedColor(tilePalette, entityPalette);
		}

		/// <summary>Gets the final (unmapped) color with the chosen tile and entity palette.</summary>
		public Color GetUnmappedColor(Palette tilePalette, Palette entityPalette) {
			return Color.GetUnmappedColor(tilePalette, entityPalette);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the final (mapped) color of the letter based on if it's using a palette or not.</summary>
		public Color MappedColor {
			get { return Color.MappedColor; }
		}

		/// <summary>Gets the final (unmapped) color of the letter based on if it's using a palette or not.</summary>
		public Color UnmappedColor {
			get { return Color.MappedColor; }
		}
	}

	/// <summary>A string of game letters with colors and characters.</summary>
	public class LetterString {
		
		/// <summary>The list of letters in the game string.</summary>
		private List<Letter> letters;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the default letter string.</summary>
		public LetterString() {
			this.letters = new List<Letter>();
		}

		/// <summary>Constructs a copy of the letter string.</summary>
		public LetterString(LetterString letterString) {
			this.letters = new List<Letter>();
			this.letters.AddRange(letterString.letters);
		}

		/// <summary>Constructs a copy of the letter string.</summary>
		public LetterString(Letter letter) {
			this.letters = new List<Letter>();
			this.letters.Add(letter);
		}

		/// <summary>Constructs a letter string from a string.</summary>
		public LetterString(string text) {
			this.letters = new List<Letter>();
			for (int i = 0; i < text.Length; i++) {
				this.letters.Add(new Letter(text[i]));
			}
		}

		/// <summary>Constructs a letter string from a colored string.</summary>
		public LetterString(string text, ColorOrPalette color) {
			this.letters = new List<Letter>();
			for (int i = 0; i < text.Length; i++) {
				this.letters.Add(new Letter(text[i], color));
			}
		}

		/// <summary>Constructs a letter string from a character.</summary>
		public LetterString(char character) {
			this.letters = new List<Letter>();
			this.letters.Add(new Letter(character));
		}


		//-----------------------------------------------------------------------------
		// General
		//-----------------------------------------------------------------------------

		/// <summary>Gets the letter string as just a string.</summary>
		public override string ToString() {
			string text = "";
			for (int i = 0; i < letters.Count; i++) {
				text += letters[i].Char;
			}
			return text;
		}

		/// <summary>Gets the letter string as just a string.</summary>
		public string ToString(IFormatProvider provider) {
			string text = "";
			for (int i = 0; i < letters.Count; i++) {
				text += letters[i].Char;
			}
			return text;
		}

		/// <summary>Gets the letter string as just a string.</summary>
		public string ToString(string format, IFormatProvider provider) {
			string text = "";
			for (int i = 0; i < letters.Count; i++) {
				text += letters[i].Char;
			}
			return text;
		}

		/// <summary>Gets the letter string as just a string.</summary>
		public string ToString(string format) {
			string text = "";
			for (int i = 0; i < letters.Count; i++) {
				text += letters[i].Char;
			}
			return text;
		}

		/// <summary>Returns true if the specified letter string is equal to the
		/// letter string.</summary>
		public override bool Equals(object obj) {
			if (obj is LetterString) {
				LetterString ls = (LetterString) obj;
				if (letters.Count == ls.letters.Count) {
					for (int i = 0; i < letters.Count; i++) {
						if (letters[i] != ls.letters[i])
							return false;
					}
					return true;
				}
			}
			return false;
		}

		/// <summary>This keeps the compiler from giving a warning.</summary>
		public override int GetHashCode() {
			return base.GetHashCode();
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Clears the letter string.</summary>
		public void Clear() {
			letters.Clear();
		}

		/// <summary>Adds a letter to the letter string.</summary>
		public void Add(Letter letter) {
			letters.Add(letter);
		}

		/// <summary>Adds a character to the letter string.</summary>
		public void Add(char character) {
			letters.Add(new Letter(character));
		}

		/// <summary>Adds a letter string to the letter string.</summary>
		public void AddRange(LetterString letterString) {
			letters.AddRange(letterString.letters);
		}

		/// <summary>Adds a string to the letter string.</summary>
		public void AddRange(string text) {
			for (int i = 0; i < text.Length; i++) {
				letters.Add(new Letter(text[i]));
			}
		}

		/// <summary>Adds a colored string to the letter string.</summary>
		public void AddRange(string text, ColorOrPalette color) {
			for (int i = 0; i < text.Length; i++) {
				letters.Add(new Letter(text[i], color));
			}
		}

		/// <summary>Inserts a letter into the letter string.</summary>
		public void Insert(int index, Letter letter) {
			letters.Insert(index, letter);
		}

		/// <summary>Inserts a character into the letter string.</summary>
		public void Insert(int index, char character) {
			letters.Insert(index, new Letter(character));
		}

		/// <summary>Inserts a letter string into the letter string.</summary>
		public void InsertRange(int index, LetterString letterString) {
			letters.InsertRange(index, letterString.letters);
		}

		/// <summary>Inserts a string into the letter string.</summary>
		public void InsertRange(int index, string text) {
			for (int i = index; i < text.Length; i++) {
				letters.Insert(i, new Letter(text[i]));
			}
		}

		/// <summary>Inserts a colored string into the letter string.</summary>
		public void InsertRange(int index, string text, ColorOrPalette color) {
			for (int i = index; i < text.Length; i++) {
				letters.Insert(i, new Letter(text[i], color));
			}
		}

		/// <summary>Removes a letter from the letter string.</summary>
		public void RemoveAt(int index) {
			letters.RemoveAt(index);
		}

		/// <summary>Removes a range of letters from the letter string.</summary>
		public void RemoveRange(int index, int count) {
			letters.RemoveRange(index, count);
		}

		/// <summary>Returns a substring of the letter string.</summary>
		public LetterString Substring(int startIndex) {
			LetterString letterString = new LetterString();
			for (int i = startIndex; i < letters.Count; i++) {
				letterString.letters.Add(letters[i]);
			}
			return letterString;
		}

		/// <summary>Returns a substring of the letter string.</summary>
		public LetterString Substring(int startIndex, int length) {
			LetterString letterString = new LetterString();
			for (int i = 0; i < length; i++) {
				letterString.letters.Add(letters[startIndex + i]);
			}
			return letterString;
		}

		/// <summary>Returns true if the letter string starts with the
		/// specified letter string.</summary>
		public bool StartsWith(LetterString letterString) {
			if (letters.Count >= letterString.letters.Count) {
				for (int i = 0; i < letterString.letters.Count; i++) {
					if (letters[i] != letterString.letters[i])
						return false;
				}
				return true;
			}
			return false;
		}

		/// <summary>Returns true if the letter string starts with the specified string.</summary>
		public bool StartsWith(string text) {
			if (letters.Count >= text.Length) {
				for (int i = 0; i < text.Length; i++) {
					if (letters[i].Char != text[i])
						return false;
				}
				return true;
			}
			return false;
		}

		/// <summary>Returns true if the letter string starts with the letter.</summary>
		public bool StartsWith(Letter letter) {
			if (letters.Count >= 1)
				return letters[0] == letter;
			return false;
		}

		/// <summary>Returns true if the letter string starts with the specified character.</summary>
		public bool StartsWith(char character) {
			if (letters.Count >= 1)
				return letters[0].Char == character;
			return false;
		}

		/// <summary>Returns true if the letter string ends with the specified letter string.</summary>
		public bool EndsWith(LetterString letterString) {
			if (letters.Count >= letterString.letters.Count) {
				for (int i = 0; i < letterString.letters.Count; i++) {
					if (letters[letters.Count - letterString.letters.Count + i] != letterString.letters[i])
						return false;
				}
				return true;
			}
			return false;
		}

		/// <summary>Returns true if the letter string ends with the specified string.</summary>
		public bool EndsWith(string text) {
			if (letters.Count >= text.Length) {
				for (int i = 0; i < text.Length; i++) {
					if (letters[letters.Count - text.Length + i].Char != text[i])
						return false;
				}
				return true;
			}
			return false;
		}

		/// <summary>Returns true if the letter string ends with the letter.</summary>
		public bool EndsWith(Letter letter) {
			if (letters.Count >= 1)
				return letters[letters.Count - 1] == letter;
			return false;
		}

		/// <summary>Returns true if the letter string ends with the specified
		/// character.</summary>
		public bool EndsWith(char character) {
			if (letters.Count >= 1)
				return letters[letters.Count - 1].Char == character;
			return false;
		}

		/// <summary>Returns the last letter of the letter string.</summary>
		/// <exception cref="InvalidOperationException">The letter string is empty.</exception>
		public Letter Last() {
			return letters.Last();
		}

		/// <summary>Returns the last letter of the letter string or the default letter
		/// if the letter string is empty.</summary>
		public Letter LastOrDefault() {
			return letters.LastOrDefault();
		}


		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

		public static bool operator ==(LetterString ls1, LetterString ls2) {
			if (ls1.letters.Count == ls2.letters.Count) {
				for (int i = 0; i < ls1.letters.Count; i++) {
					if (ls1.letters[i] != ls2.letters[i])
						return false;
				}
				return true;
			}
			return false;
		}

		public static bool operator !=(LetterString ls1, LetterString ls2) {
			if (ls1.letters.Count == ls2.letters.Count) {
				for (int i = 0; i < ls1.letters.Count; i++) {
					if (ls1.letters[i] != ls2.letters[i])
						return true;
				}
				return false;
			}
			return true;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the length of the letter string.</summary>
		public int Length {
			get { return letters.Count; }
		}

		/// <summary>Gets or sets the letter at the specified index.</summary>
		public Letter this[int index] {
			get { return letters[index]; }
			set { letters[index] = value; }
		}

		/// <summary>Gets the text of the letter string.</summary>
		public string String {
			get {
				string text = "";
				for (int i = 0; i < letters.Count; i++) {
					text += letters[i].Char;
				}
				return text;
			}
		}

		/// <summary>Gets the colors of the letter string.</summary>
		public ColorOrPalette[] Colors {
			get {
				ColorOrPalette[] colors = new ColorOrPalette[letters.Count];
				for (int i = 0; i < letters.Count; i++) {
					colors[i] = letters[i].Color;
				}
				return colors;
			}
		}

		/// <summary>Returns true if the letter string is empty.</summary>
		public bool IsEmpty {
			get { return (letters.Count == 0); }
		}

	}
}
