using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics {
	/// <summary>A font containing a sprite font.</summary>
	public class RealFont {
		
		/// <summary>The sprite font of the font.</summary>
		private SpriteFont spriteFont;
		/// <summary>The file path of the font.</summary>
		private string filePath;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a font with the specified sprite font.</summary>
		public RealFont(SpriteFont spriteFont, string filePath = "") {
			this.spriteFont		= spriteFont;
			this.filePath		= filePath;
		}

		/// <summary>Load a font from the specified file path.</summary>
		public RealFont(ContentManager content, string filePath) {
			if (filePath.Length != 0)
				this.spriteFont	= content.Load<SpriteFont>(filePath);
			else
				this.spriteFont	= null;
			this.filePath		= filePath;
		}


		//-----------------------------------------------------------------------------
		// Loading
		//-----------------------------------------------------------------------------

		/// <summary>Loads the image from the file path.</summary>
		public void Load(ContentManager content) {
			if (spriteFont == null && filePath.Length != 0)
				spriteFont = content.Load<SpriteFont>(filePath);
		}

		
		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

		public static implicit operator SpriteFont(RealFont font) {
			return font.spriteFont;
		}


		//-----------------------------------------------------------------------------
		// Measuring
		//-----------------------------------------------------------------------------

		/// <summary>Returns the width and height of a string.</summary>
		public Vector2F MeasureString(string text) {
			return (Vector2F)spriteFont.MeasureString(text);
		}

		/// <summary>Returns the width and height of a string.</summary>
		public Rectangle2F MeasureStringBounds(string text, Align alignment) {
			Rectangle2F stringBounds = new Rectangle2F(Vector2F.Zero, spriteFont.MeasureString(text));
			bool intAlign = (alignment & Align.Int) != 0;
			if (((alignment & Align.Left) != 0) == ((alignment & Align.Right) != 0))
				stringBounds.X -= (intAlign ? (int)(stringBounds.Width / 2.0f) : (stringBounds.Width / 2.0f));
			else if ((alignment & Align.Right) != 0)
				stringBounds.X -= stringBounds.Width;
			if (((alignment & Align.Top) != 0) == ((alignment & Align.Bottom) != 0))
				stringBounds.Y -= (intAlign ? (int)(stringBounds.Height / 2.0f) : (stringBounds.Height / 2.0f));
			else if ((alignment & Align.Bottom) != 0)
				stringBounds.Y -= stringBounds.Height;
			return stringBounds;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Dimensions -----------------------------------------------------------------

		/// <summary>Gets or sets the spacing of the font characters.</summary>
		public float CharacterSpacing {
			get { return spriteFont.Spacing; }
			set { spriteFont.Spacing = value; }
		}

		/// <summary>Gets or sets the vertical distance (in pixels) between the base lines of two consecutive lines of text.</summary>
		public int LineSpacing {
			get { return spriteFont.LineSpacing; }
			set { spriteFont.LineSpacing = value; }
		}

		// Information ----------------------------------------------------------------

		/// <summary>Gets the sprite font of the font.</summary>
		public SpriteFont SpriteFont {
			get { return spriteFont; }
		}

		/// <summary>Gets the file path of the font.</summary>
		public string FilePath {
			get { return filePath; }
		}
	}
}
