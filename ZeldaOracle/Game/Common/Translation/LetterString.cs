using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Common.Translation {
	// A formatted game letter with a color and character
	public struct Letter {

		// The default color of a letter
		public static readonly Color DefaultColor = Color.White;

		// The character of the letter.
		public char Char;
		// The color of the letter.
		public Color Color;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		// Constructs a letter with the specified character and default color
		public Letter(char character) {
			this.Char = character;
			this.Color = DefaultColor;
		}
		// Constructs a letter with the specified character and color
		public Letter(char character, Color color) {
			this.Char = character;
			this.Color = color;
		}

		//-----------------------------------------------------------------------------
		// General
		//-----------------------------------------------------------------------------

		// Gets the letter as just a string.
		public override string ToString() {
			return "" + Char;
		}
		// Gets the letter as just a string
		public string ToString(IFormatProvider provider) {
			return "" + Char;
		}
		// Gets the letter as just a string.
		public string ToString(string format, IFormatProvider provider) {
			return "" + Char;
		}
		// Gets the letter as just a string.
		public string ToString(string format) {
			return "" + Char;
		}
		// Returns true if the specified letter is equal to the letter.
		public override bool Equals(object obj) {
			if (obj is Letter)
				return (Char == ((Letter)obj).Char && Color == ((Letter)obj).Color);
			return false;
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
	}

	// A string of game letters with colors and characters.
	public class LetterString {

		// The list of letters in the game string.
		private List<Letter> letters;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		// Constructs the default letter string.
		public LetterString() {
			this.letters = new List<Letter>();
		}
		// Constructs a copy of the letter string.
		public LetterString(LetterString letterString) {
			this.letters = new List<Letter>();
			this.letters.AddRange(letterString.letters);
		}
		// Constructs a copy of the letter string.
		public LetterString(Letter letter) {
			this.letters = new List<Letter>();
			this.letters.Add(letter);
		}
		// Constructs a letter string from a string.
		public LetterString(string text) {
			this.letters = new List<Letter>();
			for (int i = 0; i < text.Length; i++) {
				this.letters.Add(new Letter(text[i]));
			}
		}
		// Constructs a letter string from a colored string.
		public LetterString(string text, Color color) {
			this.letters = new List<Letter>();
			for (int i = 0; i < text.Length; i++) {
				this.letters.Add(new Letter(text[i], color));
			}
		}
		// Constructs a letter string from a character.
		public LetterString(char character) {
			this.letters = new List<Letter>();
			this.letters.Add(new Letter(character));
		}

		//-----------------------------------------------------------------------------
		// General
		//-----------------------------------------------------------------------------

		// Gets the letter string as just a string.
		public override string ToString() {
			string text = "";
			for (int i = 0; i < letters.Count; i++) {
				text += letters[i].Char;
			}
			return text;
		}
		// Gets the letter string as just a string.
		public string ToString(IFormatProvider provider) {
			string text = "";
			for (int i = 0; i < letters.Count; i++) {
				text += letters[i].Char;
			}
			return text;
		}
		// Gets the letter string as just a string.
		public string ToString(string format, IFormatProvider provider) {
			string text = "";
			for (int i = 0; i < letters.Count; i++) {
				text += letters[i].Char;
			}
			return text;
		}
		// Gets the letter string as just a string.
		public string ToString(string format) {
			string text = "";
			for (int i = 0; i < letters.Count; i++) {
				text += letters[i].Char;
			}
			return text;
		}
		// Returns true if the specified letter string is equal to the letter string.
		public override bool Equals(object obj) {
			if (obj is LetterString) {
				LetterString ls = (LetterString)obj;
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

		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		// Clears the letter string.
		public void Clear() {
			letters.Clear();
		}

		// Adds a letter to the letter string.
		public void Add(Letter letter) {
			letters.Add(letter);
		}
		// Adds a character to the letter string.
		public void Add(char character) {
			letters.Add(new Letter(character));
		}
		// Adds a letter string to the letter string.
		public void AddRange(LetterString letterString) {
			letters.AddRange(letterString.letters);
		}
		// Adds a string to the letter string.
		public void AddRange(string text) {
			for (int i = 0; i < text.Length; i++) {
				letters.Add(new Letter(text[i]));
			}
		}
		// Adds a colored string to the letter string.
		public void AddRange(string text, Color color) {
			for (int i = 0; i < text.Length; i++) {
				letters.Add(new Letter(text[i], color));
			}
		}

		// Inserts a letter into the letter string.
		public void Insert(int index, Letter letter) {
			letters.Insert(index, letter);
		}
		// Inserts a character into the letter string.
		public void Insert(int index, char character) {
			letters.Insert(index, new Letter(character));
		}
		// Inserts a letter string into the letter string.
		public void InsertRange(int index, LetterString letterString) {
			letters.InsertRange(index, letterString.letters);
		}
		// Inserts a string into the letter string.
		public void InsertRange(int index, string text) {
			for (int i = index; i < text.Length; i++) {
				letters.Insert(i, new Letter(text[i]));
			}
		}
		// Inserts a colored string into the letter string.
		public void InsertRange(int index, string text, Color color) {
			for (int i = index; i < text.Length; i++) {
				letters.Insert(i, new Letter(text[i], color));
			}
		}

		// Removes a letter from the letter string.
		public void RemoveAt(int index) {
			letters.RemoveAt(index);
		}
		// Removes a range of letters from the letter string.
		public void RemoveRange(int index, int count) {
			letters.RemoveRange(index, count);
		}

		// Returns a substring of the letter string.
		public LetterString Substring(int startIndex) {
			LetterString letterString = new LetterString();
			for (int i = startIndex; i < letters.Count; i++) {
				letterString.letters.Add(letters[i]);
			}
			return letterString;
		}
		// Returns a substring of the letter string.
		public LetterString Substring(int startIndex, int length) {
			LetterString letterString = new LetterString();
			for (int i = 0; i < length; i++) {
				letterString.letters.Add(letters[startIndex + i]);
			}
			return letterString;
		}

		// Returns true if the letter string starts with the specified letter string.
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
		// Returns true if the letter string starts with the specified string.
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
		// Returns true if the letter string starts with the letter.
		public bool StartsWith(Letter letter) {
			if (letters.Count >= 1)
				return letters[0] == letter;
			return false;
		}
		// Returns true if the letter string starts with the specified character.
		public bool StartsWith(char character) {
			if (letters.Count >= 1)
				return letters[0].Char == character;
			return false;
		}

		// Returns true if the letter string ends with the specified letter string.
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
		// Returns true if the letter string ends with the specified string.
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
		// Returns true if the letter string ends with the letter.
		public bool EndsWith(Letter letter) {
			if (letters.Count >= 1)
				return letters[letters.Count - 1] == letter;
			return false;
		}
		// Returns true if the letter string ends with the specified character.
		public bool EndsWith(char character) {
			if (letters.Count >= 1)
				return letters[letters.Count - 1].Char == character;
			return false;
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

		// Gets the length of the letter string.
		public int Length {
			get { return letters.Count; }
		}
		// Gets or sets the letter at the specified index.
		public Letter this[int index] {
			get { return letters[index]; }
			set { letters[index] = value; }
		}
		// Gets the string of the letter string.
		public string String {
			get {
				string text = "";
				for (int i = 0; i < letters.Count; i++) {
					text += letters[i].Char;
				}
				return text;
			}
		}
		// Gets the colors of the letter string.
		public Color[] Colors {
			get {
				Color[] colors = new Color[letters.Count];
				for (int i = 0; i < letters.Count; i++) {
					colors[i] = letters[i].Color;
				}
				return colors;
			}
		}
	}
}
