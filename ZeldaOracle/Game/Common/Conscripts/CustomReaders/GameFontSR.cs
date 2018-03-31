using ZeldaOracle.Common.Conscripts.Commands;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Common.Conscripts.CustomReaders {
	public class GameFontSR : ConscriptRunner {

		private enum Modes {
			Root,
			Font,
		}

		/// <summary>The current font being created.</summary>
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
		// Override Methods
		//-----------------------------------------------------------------------------

		/// <summary>Begins reading the script.</summary>
		protected override void OnBeginReading() {
			font = null;
		}

		/// <summary>Ends reading the script.</summary>
		protected override void OnEndReading() {
			OnBeginReading();
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
