using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using ZeldaOracle.Common.Geometry;
using ZeldaWpf.Resources;
using ZeldaWpf.Util;

namespace ZeldaEditor.Control {
	/// <summary>A static class for storing pre-loaded images.</summary>
	public class EditorImages : WpfImages {
		
		//-----------------------------------------------------------------------------
		// Images
		//-----------------------------------------------------------------------------

		public static readonly BitmapSource FolderBlueOpen;
		public static readonly BitmapSource FolderBlueClosed;
		public static readonly BitmapSource Rename;
		public static readonly BitmapSource Edit;
		public static readonly BitmapSource Event;
		public static readonly BitmapSource EventEdit;
		public static readonly BitmapSource EventAdd;
		public static readonly BitmapSource EventError;
		public static readonly BitmapSource EventWarning;
		public static readonly BitmapSource EventDelete;
		public static readonly BitmapSource EventRefactor;
		public static readonly BitmapSource Property;
		public static readonly BitmapSource PropertyRefactor;
		public static readonly BitmapSource GotoOwner;


		public static readonly BitmapSource Eraser;
		public static readonly BitmapSource ToolPlace;
		public static readonly BitmapSource ToolPlaceErase;
		public static readonly BitmapSource ToolSquare;
		public static readonly BitmapSource ToolSquareErase;
		public static readonly BitmapSource ToolFill;
		public static readonly BitmapSource ToolFillErase;
		public static readonly BitmapSource ToolSelection;
		public static readonly BitmapSource SelectAll;
		public static readonly BitmapSource Deselect;
		public static readonly BitmapSource Copy;
		public static readonly BitmapSource Cut;
		public static readonly BitmapSource Paste;

		public static readonly BitmapSource Open;
		public static readonly BitmapSource World;
		public static readonly BitmapSource WorldNew;
		public static readonly BitmapSource Room;
		public static readonly BitmapSource Level;
		public static readonly BitmapSource LevelAdd;
		public static readonly BitmapSource LevelDelete;
		public static readonly BitmapSource LevelDuplicate;
		public static readonly BitmapSource LevelGroup;
		public static readonly BitmapSource LevelResize;
		public static readonly BitmapSource LevelShift;
		public static readonly BitmapSource Area;
		public static readonly BitmapSource AreaAdd;
		public static readonly BitmapSource AreaDelete;
		public static readonly BitmapSource AreaDuplicate;
		public static readonly BitmapSource AreaGroup;
		public static readonly BitmapSource Script;
		public static readonly BitmapSource ScriptAdd;
		public static readonly BitmapSource ScriptDelete;
		public static readonly BitmapSource ScriptDuplicate;
		public static readonly BitmapSource ScriptEdit;
		public static readonly BitmapSource ScriptError;
		public static readonly BitmapSource ScriptWarning;
		public static readonly BitmapSource ScriptGroup;

		public static readonly BitmapSource MoveUp;
		public static readonly BitmapSource MoveDown;

		public static readonly BitmapSource StringCodes;
		public static readonly BitmapSource ColorCodes;

		[CustomImage(Path = "Resources/")]
		public static readonly BitmapSource CharacterFormatCodes;


		//-----------------------------------------------------------------------------
		// Dictionaries
		//-----------------------------------------------------------------------------
		
		/// <summary>The collection of readonly string code images.</summary>
		public static readonly Dictionary<string, CroppedBitmap> StringCodeImages;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the Editor images.</summary>
		static EditorImages() {
			StringCodeImages = new Dictionary<string, CroppedBitmap>();
			LoadImages(typeof(EditorImages));

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
			for (int i = 0; i < stringCodesOrder.Length; i++) {
				Int32Rect source = new Int32Rect(
					2 + (charSize.X + 2) * (i % 8), 2 + (charSize.Y + 2) * (i / 8),
					charSize.X, charSize.Y
				);
				CroppedBitmap cropped = new CroppedBitmap(CharacterFormatCodes, source);
				StringCodeImages.Add(stringCodesOrder[i], cropped);
				if (i + 1 == 4) {
					// Skip four spaces to align the rows
					i += 2;
					charSize = thinCharSize;
				}
			}
		}
	}
}
