using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using ZeldaEditor.Util;
using ZeldaOracle.Common.Geometry;

namespace ZeldaEditor.Control {
	public static class EditorImages {

		public static readonly BitmapSource FolderBlueOpen	= LoadIcon("FolderBlueOpen");
		public static readonly BitmapSource FolderBlueClosed= LoadIcon("FolderBlueClosed");
		public static readonly BitmapSource Rename			= LoadIcon("Rename");
		public static readonly BitmapSource Edit            = LoadIcon("Edit");
		public static readonly BitmapSource Event			= LoadIcon("Event");
		public static readonly BitmapSource Property		= LoadIcon("Property");


		public static readonly BitmapSource Eraser			= LoadIcon("Eraser");
		public static readonly BitmapSource ToolPlace		= LoadIcon("ToolPlace");
		public static readonly BitmapSource ToolPlaceErase	= LoadIcon("ToolPlaceErase");
		public static readonly BitmapSource ToolSquare		= LoadIcon("ToolSquare");
		public static readonly BitmapSource ToolSquareErase	= LoadIcon("ToolSquareErase");
		public static readonly BitmapSource ToolFill		= LoadIcon("ToolFill");
		public static readonly BitmapSource ToolFillErase	= LoadIcon("ToolFillErase");
		public static readonly BitmapSource ToolSelection	= LoadIcon("ToolSelection");
		public static readonly BitmapSource SelectAll		= LoadIcon("SelectAll");
		public static readonly BitmapSource Deselect		= LoadIcon("Deselect");
		public static readonly BitmapSource Copy			= LoadIcon("Copy");
		public static readonly BitmapSource Cut				= LoadIcon("Cut");
		public static readonly BitmapSource Paste			= LoadIcon("Paste");

		public static readonly BitmapSource Open			= LoadIcon("Open");
		public static readonly BitmapSource World			= LoadIcon("World");
		public static readonly BitmapSource Room			= LoadIcon("Room");
		public static readonly BitmapSource Level			= LoadIcon("Level");
		public static readonly BitmapSource LevelAdd		= LoadIcon("LevelAdd");
		public static readonly BitmapSource LevelDelete		= LoadIcon("LevelDelete");
		public static readonly BitmapSource LevelDuplicate	= LoadIcon("LevelDuplicate");
		public static readonly BitmapSource LevelGroup		= LoadIcon("LevelGroup");
		public static readonly BitmapSource LevelResize		= LoadIcon("LevelResize");
		public static readonly BitmapSource LevelShift		= LoadIcon("LevelShift");
		public static readonly BitmapSource Dungeon			= LoadIcon("Dungeon");
		public static readonly BitmapSource DungeonAdd		= LoadIcon("DungeonAdd");
		public static readonly BitmapSource DungeonDelete	= LoadIcon("DungeonDelete");
		public static readonly BitmapSource DungeonDuplicate= LoadIcon("DungeonDuplicate");
		public static readonly BitmapSource DungeonGroup	= LoadIcon("DungeonGroup");
		public static readonly BitmapSource Script			= LoadIcon("Script");
		public static readonly BitmapSource ScriptAdd		= LoadIcon("ScriptAdd");
		public static readonly BitmapSource ScriptDelete	= LoadIcon("ScriptDelete");
		public static readonly BitmapSource ScriptDuplicate	= LoadIcon("ScriptDuplicate");
		public static readonly BitmapSource ScriptError		= LoadIcon("ScriptError");
		public static readonly BitmapSource ScriptGroup     = LoadIcon("ScriptGroup");

		public static readonly BitmapSource MoveUp			= LoadIcon("MoveUp");
		public static readonly BitmapSource MoveDown		= LoadIcon("MoveDown");

		public static readonly BitmapSource StringCodes		= LoadIcon("StringCodes");
		public static readonly BitmapSource ColorCodes		= LoadIcon("ColorCodes");

		public static readonly BitmapSource CharacterFormatCodes = LoadResource("FormatCodeCharacters");


		public static readonly Dictionary<string, CroppedBitmap> StringCodeImages = new Dictionary<string, CroppedBitmap>();

		static EditorImages() {
			
			string[] stringCodesOrder = new string[] {
				"triangle", "square", "circle", "heart", "diamond", "club", "spade", "rupee",
				"up", "down", "right", "left", "up-tri", "down-tri", "right-tri", "left-tri",
				"male", "female", "music", "music-beam", "!!", "pilcrow", "section", "house",
				"1", "2", "3", "cursor", "invalid"
			};

			Point2I charSize = new Point2I(16, 24);
			int i = 0;
			for (i = 0; i < stringCodesOrder.Length; i++) {
				Int32Rect source = new Int32Rect(
					2 + (charSize.X + 2) * (i % 8), 2 + (charSize.Y + 2) * (i / 8),
					charSize.X, charSize.Y
				);
				CroppedBitmap cropped = new CroppedBitmap(EditorImages.CharacterFormatCodes, source);
				StringCodeImages.Add(stringCodesOrder[i], cropped);
			}
		}

		private static BitmapSource LoadIcon(string name) {
			return BitmapFactory.LoadSourceFromResource("Resources/Icons/" + name + ".png");
		}
		private static BitmapSource LoadResource(string name) {
			return BitmapFactory.LoadSourceFromResource("Resources/" + name + ".png");
		}
	}
}
