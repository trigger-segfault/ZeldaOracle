using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using ZeldaOracle.Common.Geometry;
using ZeldaWpf.Util;

namespace ZeldaEditor.Control {
	public static class EditorImages {

		public static readonly BitmapSource FolderBlueOpen	= LoadIcon("FolderBlueOpen");
		public static readonly BitmapSource FolderBlueClosed= LoadIcon("FolderBlueClosed");
		public static readonly BitmapSource Rename			= LoadIcon("Rename");
		public static readonly BitmapSource Edit            = LoadIcon("Edit");
		public static readonly BitmapSource Event			= LoadIcon("Event");
		public static readonly BitmapSource EventEdit		= LoadIcon("EventEdit");
		public static readonly BitmapSource EventAdd        = LoadIcon("EventAdd");
		public static readonly BitmapSource EventError		= LoadIcon("EventError");
		public static readonly BitmapSource EventWarning	= LoadIcon("EventWarning");
		public static readonly BitmapSource EventDelete		= LoadIcon("EventDelete");
		public static readonly BitmapSource EventRefactor	= LoadIcon("EventRefactor");
		public static readonly BitmapSource Property		= LoadIcon("Property");
		public static readonly BitmapSource PropertyRefactor= LoadIcon("PropertyRefactor");
		public static readonly BitmapSource GotoOwner		= LoadIcon("GotoOwner");


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
		public static readonly BitmapSource WorldNew		= LoadIcon("WorldNew");
		public static readonly BitmapSource Room			= LoadIcon("Room");
		public static readonly BitmapSource Level			= LoadIcon("Level");
		public static readonly BitmapSource LevelAdd		= LoadIcon("LevelAdd");
		public static readonly BitmapSource LevelDelete		= LoadIcon("LevelDelete");
		public static readonly BitmapSource LevelDuplicate	= LoadIcon("LevelDuplicate");
		public static readonly BitmapSource LevelGroup		= LoadIcon("LevelGroup");
		public static readonly BitmapSource LevelResize		= LoadIcon("LevelResize");
		public static readonly BitmapSource LevelShift		= LoadIcon("LevelShift");
		public static readonly BitmapSource Area			= LoadIcon("Area");
		public static readonly BitmapSource AreaAdd			= LoadIcon("AreaAdd");
		public static readonly BitmapSource AreaDelete		= LoadIcon("AreaDelete");
		public static readonly BitmapSource AreaDuplicate	= LoadIcon("AreaDuplicate");
		public static readonly BitmapSource AreaGroup		= LoadIcon("AreaGroup");
		public static readonly BitmapSource Script			= LoadIcon("Script");
		public static readonly BitmapSource ScriptAdd		= LoadIcon("ScriptAdd");
		public static readonly BitmapSource ScriptDelete	= LoadIcon("ScriptDelete");
		public static readonly BitmapSource ScriptDuplicate	= LoadIcon("ScriptDuplicate");
		public static readonly BitmapSource ScriptEdit		= LoadIcon("ScriptEdit");
		public static readonly BitmapSource ScriptError		= LoadIcon("ScriptError");
		public static readonly BitmapSource ScriptWarning	= LoadIcon("ScriptWarning");
		public static readonly BitmapSource ScriptGroup     = LoadIcon("ScriptGroup");

		public static readonly BitmapSource MoveUp			= LoadIcon("MoveUp");
		public static readonly BitmapSource MoveDown		= LoadIcon("MoveDown");

		public static readonly BitmapSource StringCodes		= LoadIcon("StringCodes");
		public static readonly BitmapSource ColorCodes		= LoadIcon("ColorCodes");

		public static readonly BitmapSource CharacterFormatCodes = LoadResource("FormatCodeCharacters");


		public static readonly Dictionary<string, CroppedBitmap> StringCodeImages = new Dictionary<string, CroppedBitmap>();

		static EditorImages() {
			
			string[] stringCodesOrder = new string[] {
				"a", "b", "x", "y", "", "", "dpad", "rupee",
				"triangle", "square", "circle", "heart", "diamond", "club", "spade", "cursor",
				"up", "down", "right", "left", "up-tri", "down-tri", "right-tri", "left-tri",
				"male", "female", "music", "music-beam", "!!", "pilcrow", "section", "house",
				"1", "2", "3", "invalid"
			};

			Point2I wideCharSize = new Point2I(25, 24);
			Point2I thinCharSize = new Point2I(16, 24);
			Point2I charSize = wideCharSize;
			int i = 0;
			for (i = 0; i < stringCodesOrder.Length; i++) {
				Int32Rect source = new Int32Rect(
					2 + (charSize.X + 2) * (i % 8), 2 + (charSize.Y + 2) * (i / 8),
					charSize.X, charSize.Y
				);
				CroppedBitmap cropped = new CroppedBitmap(EditorImages.CharacterFormatCodes, source);
				StringCodeImages.Add(stringCodesOrder[i], cropped);
				if (i + 1 == 4) {
					// Skip four spaces to align the rows
					i += 2;
					charSize = thinCharSize;
				}
			}
		}

		private static BitmapSource LoadIcon(string name) {
			return BitmapFactory.FromResource("Resources/Icons/" + name + ".png",
				typeof(EditorImages));
		}
		private static BitmapSource LoadResource(string name) {
			return BitmapFactory.FromResource("Resources/" + name + ".png",
				typeof(EditorImages));
		}
	}
}
