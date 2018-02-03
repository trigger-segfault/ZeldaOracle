using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripts.Commands;

namespace ZeldaOracle.Common.Scripts.CustomReaders {
	public class GameFontSR : ScriptReader {

		private enum Modes {
			Root,
			Font
		}

		// The current font being created.
		private GameFont font;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public GameFontSR() {
			
			//=====================================================================================
			AddCommand("FONT", (int) Modes.Root,
				"string sheetPath",
				"string name, string sheetPath",
			delegate(CommandParam parameters) {
				string fontName = parameters.GetString(0);
				string fontPath = fontName;
				if (parameters.ChildCount == 2)
					fontPath = parameters.GetString(1);
				
				Image image = Resources.LoadImage(Resources.ImageDirectory + fontPath);

				SpriteSheet sheet = new SpriteSheet(image, Point2I.One, Point2I.Zero, Point2I.Zero);
				font = new GameFont(sheet, 1, 0, 1);
				AddResource<GameFont>(fontName, font);
				Mode = Modes.Font;
			});
			//=====================================================================================
			AddCommand("GRID", (int) Modes.Font,
				"Point charSize, Point charSpacing, Point offset",
			delegate(CommandParam parameters) {
				font.SpriteSheet.CellSize	= parameters.GetPoint(0);
				font.SpriteSheet.Spacing	= parameters.GetPoint(1);
				font.SpriteSheet.Offset		= parameters.GetPoint(2);
			});
			//=====================================================================================
			AddCommand("SPACING", (int) Modes.Font,
				"int charSpacing, int lineSpacing, int charsPerRow",
			delegate(CommandParam parameters) {
				font.CharacterSpacing	= parameters.GetInt(0);
				font.LineSpacing		= parameters.GetInt(1);
				font.CharactersPerRow	= parameters.GetInt(2);
			});
			//=====================================================================================
			AddCommand("END", (int) Modes.Font,
				"",
			delegate(CommandParam parameters) {
				font = null;
				Mode = Modes.Root;
			});
			//=====================================================================================
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Begins reading the script.</summary>
		protected override void BeginReading() {
			font		= null;
		}

		/// <summary>Ends reading the script.</summary>
		protected override void EndReading() {
			font = null;
		}
		
		/// <summary>Creates a new script reader of the derived type.</summary>
		protected override ScriptReader CreateNew() {
			return new GameFontSR();
		}


		//-----------------------------------------------------------------------------
		// Internal Properties
		//-----------------------------------------------------------------------------

		/// <summary>The mode of the Game Font script reader.</summary>
		private new Modes Mode {
			get { return (Modes) base.Mode; }
			set { base.Mode = (int) value; }
		}
	}
}
