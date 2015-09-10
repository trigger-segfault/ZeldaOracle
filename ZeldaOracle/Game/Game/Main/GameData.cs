using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;

using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

using ZeldaOracle.Common.Graphics.Particles;
using ZeldaOracle.Common.Scripts;
using System.IO;
//using ParticleGame.Project.Particles;

namespace GameFramework.MyGame.Main {
/** <summary>
 * A static class for storing links to all game content.
 * </summary> */
class GameData {

	//=========== LOADING ============
	#region Loading
	//--------------------------------
	#region Initialize

	/** <summary> Initializes and loads the game content. </summary> */
	public static void Initialize() {
		Console.WriteLine("Loading Images");
		LoadImages();

		Console.WriteLine("Loading Sprite Sheets");
		LoadSpriteSheets();

		Console.WriteLine("Loading Fonts");
		LoadFonts();

		Console.WriteLine("Loading Shaders");
		LoadShaders();

		Console.WriteLine("Loading Sound Effects");
		LoadSounds();

		Console.WriteLine("Loading Music");
		LoadMusic();

		Console.WriteLine("Loading Languages");
		LoadLanguages();

	}

	#endregion
	//--------------------------------
	#region Images

	private static int PointIndex(int x, int y) {
		return 234 * y + x;
	}

	/** <summary> Loads the images. </summary> */
	private static void LoadImages() {
		/*
		Image palette = Resources.LoadImage("Images/Palette");

		List<Color> colors = new List<Color>();


		int index = 0;
		Color[] colorData = new Color[palette.Width * palette.Height];
		palette.Texture.GetData<Color>(colorData);

		StreamWriter writer = new StreamWriter("Palette.txt");

		int[,] colorIndexes = new int[32, 12];
		int[,] colorRows = new int[32, 12];

		for (int i = 0; i < 18; i++) {
			for (int j = 0; j < 12; j++) {
				Color color = colorData[PointIndex(i * 13, (11 - j) * 10)];
				writer.Write("Color.FromAarg(" + color.A + ", " + color.R + ", " + color.G +"),");
				index++;
				if ((j + 1) % 4 != 0)
					writer.Write(" ");
				else
					writer.Write(writer.NewLine);
			}
			writer.Write(writer.NewLine);
		}

		Image p = Resources.LoadImage("Images/ColorPal");



		writer.Close();
		*/
	}

	#endregion
	//--------------------------------
	#region Sprite Sheets

	/** <summary> Loads the sprite sheets. </summary> */
	private static void LoadSpriteSheets() {

		SheetDebugMenu			= Resources.LoadSpriteSheet(Resources.SpriteSheetDirectory + "sheet_debug_menu.conscript");

		Resources.LoadSpriteSheets(Resources.SpriteSheetDirectory + "sheet_gamepad.conscript");
		SheetGamePadControls	= Resources.GetSpriteSheet("sheet_gamepad_controls");
		SheetGamePadArrows		= Resources.GetSpriteSheet("sheet_gamepad_arrows");

		Resources.LoadSpriteSheets(Resources.SpriteSheetDirectory + "sheet_particles.conscript");
		Resources.LoadSpriteSheets(Resources.SpriteSheetDirectory + "custom_sheets.conscript");
		ParticleSR.UsingDegrees = true;
		Resources.LoadParticles(Resources.ParticleDirectory + "particle_data.conscript");
		//ParticleSR.UsingDegrees = false;
		//Resources.LoadParticles(Resources.ParticleDirectory + "particle_data_before.conscript");
	}

	#endregion
	//--------------------------------
	#region Fonts

	/** <summary> Loads the fonts. </summary> */
	private static void LoadFonts() {

		FontDebugMenu = Resources.LoadFont("Fonts/font_debug_menu");
		FontDebugMenuBold = Resources.LoadFont("Fonts/font_debug_menu_bold");
	}

	#endregion
	//--------------------------------
	#region Shaders

	/** <summary> Loads the shaders. </summary> */
	private static void LoadShaders() {

	}

	#endregion
	//--------------------------------
	#region Sound Effects

	/** <summary> Loads the sound effects. </summary> */
	private static void LoadSounds() {


		//Resources.LoadSoundGroups(Resources.SoundDirectory + "sounds.conscript");

	}

	#endregion
	//--------------------------------
	#region Music

	/** <summary> Loads the music. </summary> */
	private static void LoadMusic() {

		//Resources.LoadPlaylists(Resources.MusicDirectory + "music.conscript");
	}

	#endregion
	//--------------------------------
	#region Languages

	/** <summary> Loads the languages. </summary> */
	private static void LoadLanguages() {

	}

	#endregion
	//--------------------------------
	#endregion
	//========== GAME DATA ===========
	#region Game Data
	//--------------------------------
	#region Images

	public static Image ImageSMG2Logo;

	#endregion
	//--------------------------------
	#region Sprite Sheets

	public static SpriteSheet SheetDebugMenu;
	public static SpriteSheet SheetGamePadControls;
	public static SpriteSheet SheetGamePadArrows;

	#endregion
	//--------------------------------
	#region Fonts

	public static Font FontDebugMenu;
	public static Font FontDebugMenuBold;

	#endregion
	//--------------------------------
	#region Shaders


	#endregion
	//--------------------------------
	#region Sound Effects


	#endregion
	//--------------------------------
	#region Music


	#endregion
	//--------------------------------
	#region Languages


	#endregion
	//--------------------------------
	#region Render Targets

	public static RenderTarget2D RenderTargetGame;
	public static RenderTarget2D RenderTargetDebug;

	#endregion
	//--------------------------------
	#endregion
}
} // end namespace
