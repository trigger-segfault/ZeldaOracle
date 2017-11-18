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

		public static readonly BitmapSource FolderBlueOpen = LoadIcon("FolderBlueOpen");
		public static readonly BitmapSource FolderBlueClosed = LoadIcon("FolderBlueClosed");

		public static readonly BitmapSource World		= LoadIcon("World");
		public static readonly BitmapSource Room			= LoadIcon("Room");
		public static readonly BitmapSource Level		= LoadIcon("Level");
		public static readonly BitmapSource LevelGroup	= LoadIcon("LevelGroup");
		public static readonly BitmapSource Dungeon		= LoadIcon("Dungeon");
		public static readonly BitmapSource DungeonGroup	= LoadIcon("DungeonGroup");
		public static readonly BitmapSource Script		= LoadIcon("Script");
		public static readonly BitmapSource ScriptError	= LoadIcon("ScriptError");
		public static readonly BitmapSource ScriptGroup	= LoadIcon("ScriptGroup");

		public static readonly BitmapSource StringCodes = LoadIcon("StringCodes");
		public static readonly BitmapSource ColorCodes = LoadIcon("ColorCodes");

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
