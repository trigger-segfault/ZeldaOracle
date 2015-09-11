using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using XnaSong		= Microsoft.Xna.Framework.Media.Song;
using XnaPlaylist	= Microsoft.Xna.Framework.Media.Playlist;
using XnaKeys		= Microsoft.Xna.Framework.Input.Keys;

using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Debug;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
//using ZeldaOracle.Common.Graphics.Particles;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Common.Input.Controls;
using ZeldaOracle.Common.Scripts;
using GamePad		= ZeldaOracle.Common.Input.GamePad;
using Keyboard		= ZeldaOracle.Common.Input.Keyboard;
using Mouse			= ZeldaOracle.Common.Input.Mouse;
using Keys			= ZeldaOracle.Common.Input.Keys;
using Buttons		= ZeldaOracle.Common.Input.Buttons;
using MouseButtons	= ZeldaOracle.Common.Input.MouseButtons;
using Color			= ZeldaOracle.Common.Graphics.Color;
using Song			= ZeldaOracle.Common.Audio.Song;
using Playlist		= ZeldaOracle.Common.Audio.Playlist;

using GameFramework.MyGame.Debug;
//using GameFramework.MyGame.Editor;
//using GameFramework.MyGame.Editor.Properties;

namespace GameFramework.MyGame.Main {

// The class that manages the framework of the game.
public class GameManager {
	
	//========== CONSTANTS ===========

	/** <summary> The name of the game. </summary> */
	public const string GameName	= "ZeldaOracle";

	//=========== MEMBERS ============

	// Containment
	/** <summary> The base game running the XNA framework. </summary> */
	private GameBase gameBase;

	// Game
	/** <summary> The game scale used to alter screen size and mouse properties. </summary> */

	public double gameScale;

	// Other:
	// The index of the current language.
	private int language;

	// Debug:
	// True if the game is in debug mode.
	private bool debugMode;
	// The debug controller for the game.
	private DebugController debugController;

	//-----------------------------------------------------------------------------
	// Constructors
	//-----------------------------------------------------------------------------

	// Constructs the default game manager.
	public GameManager() {
		this.gameBase		= null;

		this.language		= 0;
		this.debugMode		= false;
		this.gameScale		= 1;
	}

	// Initializes the game manager.
	public void Initialize(GameBase gameBase) {
		this.gameBase		= gameBase;

		this.debugController	= new DebugController(this);

		ScreenResized();
	}

	// Uninitializes the game manager.
	public void Uninitialize() {

		this.gameBase		= null;
	}

	//=========== CONTENT ============

	// Called to load game manager content.
	public void LoadContent(ContentManager content) {

		GameData.Initialize();

		this.debugController.DebugMenuKey = new Key(Keys.F2);
		this.debugController.DebugMenuMouseButton = new MouseButton(MouseButtons.Middle);
		this.debugController.DebugMenuButton = new ComboControl(new Control[] {new Button(Buttons.Back), new Button(Buttons.Start)});

		this.debugController.DebugMenuFont = GameData.FontDebugMenu;
		this.debugController.DebugMenuFontBold = GameData.FontDebugMenuBold;
		this.debugController.DebugMenuSprites = GameData.SheetDebugMenu;
	}

	// Called to unload game manager content.
	public void UnloadContent(ContentManager content) {

	}

	//========== PROPERTIES ==========
	
	// Containment

	// Gets the base game running the XNA framework.
	public GameBase GameBase {
		get { return gameBase; }
	}

	// Returns true if the game is running in windows.
	public bool IsWindows {
		get { return gameBase.IsWindows; }
	}

	// Returns true if the game is running on the Xbox 360.
	public bool IsXbox {
		get { return gameBase.IsXbox; }
	}

	// Information

	//  Gets the current framerate of the game.
	public double FPS {
		get { return gameBase.FPS; }
	}
	// Gets or sets if the game is in fullscreen mode.
	public bool IsFullScreen {
		get { return gameBase.IsFullScreen; }
		set { gameBase.IsFullScreen = value; }
	}

	// Gets the true size of the screen.
	public Point2I ScreenSize {
		get {
			return new Point2I(gameBase.GraphicsDevice.Viewport.Width,
							   gameBase.GraphicsDevice.Viewport.Height);
		}
	}
	//  Gets the size of the screen based on the game scale.
	public Point2I GameScreenSize {
		get {
			return (Point2I)GMath.Ceiling(new Vector2F(gameBase.GraphicsDevice.Viewport.Width,
													   gameBase.GraphicsDevice.Viewport.Height) / (float)gameScale);
		}
	}
	// Gets or sets the draw scale of the game.
	public double GameScale {
		get { return gameScale; }
		set { gameScale = GMath.Max(0.1, value); }
	}

	// Debug

	// Gets or sets if the game is in debug mode.
	public bool DebugMode {
		get { return debugMode; }
		set { debugMode = value; }
	}

	// Gets the debug controller of the game.
	public DebugController DebugController {
		get { return debugController; }
	}


	//=========== UPDATING ===========

	// Called every step to update the game.
	public void Update() {

		/*if (Keyboard.IsKeyPressed(Keys.F11) && IsWindows) {
			IsFullScreen = !IsFullScreen;
		}*/

		// Update the room
		if (!debugController.IsGamePaused || debugController.nextStep) {
			debugController.nextStep = false;
		}

		debugController.Update();

		//prop.Update(1.0 / 60.0, new Point2I(ScreenSize.X - Property<int>.Width, ScreenSize.Y / 2));

		// Update the menu

		// Check for screenshot requests
		if (Keyboard.IsKeyPressed(Keys.F12)) {
			GameBase.TakeScreenShot();
		}
		else if ((GamePad.IsButtonDown(Buttons.LeftShoulder) && GamePad.IsButtonPressed(Buttons.RightShoulder)) ||
			(GamePad.IsButtonDown(Buttons.RightShoulder) && GamePad.IsButtonPressed(Buttons.LeftShoulder))) {
			GameBase.TakeScreenShot();
		}

		// Toggle debug mode
		if (Keyboard.IsKeyPressed(Keys.Insert) || (GamePad.IsButtonDown(Buttons.Back) && GamePad.IsButtonPressed(Buttons.RightStickButton))) {
			DebugMode = !DebugMode;
		}

		// Debug quit buttons
		/*if (GamePad.IsButtonDown(Buttons.Start) || Keyboard.IsKeyPressed(Keys.Escape)) {
			if (IsInLevel) {
				if (IsMenuOpen)
					CloseMenu();
				else
					OpenMenu(new PauseMenu());
			}
			else {
				GameBase.Exit();
			}
		}*/

	}

	//=========== GAMEPLAY ===========

	public void Exit() {
		gameBase.Exit();
	}

	//=========== DRAWING ============

	// Called every step to draw the game.
	public void Draw(Graphics2D g) {

		g.SetRenderTarget(GameData.RenderTargetGame);
		g.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
		g.Clear(Color.Black);

		// Draw the room
		//g.DrawImage(GameData.ImageSMG2Logo, Vector2D.Zero);
		//room.Draw(g);


		g.End();
		//particleSystem.Draw(g);

		// Draw the debug info
		g.SetRenderTarget(GameData.RenderTargetDebug);
		g.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
		g.Clear(Color.Transparent);
		debugController.Draw(g);
		//prop.Draw(g, new Point2I(ScreenSize.X - Property<int>.Width, ScreenSize.Y / 2));
		g.End();

		g.SetRenderTarget(null);
		g.ResetTranslation();
		g.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
		g.DrawImage(GameData.RenderTargetGame, Vector2F.Zero, Vector2F.Zero, (Vector2F)gameScale, 0.0);
		g.DrawImage(GameData.RenderTargetDebug, Vector2F.Zero);
		g.End();
	}

	// Called every step to draw the debug information if debug mode is enabled.
	private void DrawDebugInfo(Graphics2D g) {
		if (debugMode) {
			g.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			Vector2F pos = new Vector2F(12, 50);
			int spacing = 22;
			//Vector2D viewPos = GMath.Floor(-level.Player.Position + ScreenSize / 2 - new Vector2D(8, 12));
			//Vector2D viewPos2 = viewPos + level.Player.Position;
			Color color = Color.White;
			/*SpriteFont font = FontLoader.Get("BitTransition");
			g.DrawString(font, "FPS: " + FPS, pos, Align.TopLeft, color); pos.Y += spacing;
			g.DrawString(font, "Objects: " + room.NumObjects, pos, Align.TopLeft, color); pos.Y += spacing;
			g.DrawString(font, "Entities: " + room.NumEntities, pos, Align.TopLeft, color); pos.Y += spacing;
			g.DrawString(font, "Particles: " + room.NumParticles, pos, Align.TopLeft, color); pos.Y += spacing;
			g.End();*/
		}
	}

	//========== MANAGEMENT ==========

	// Called when the screen has been resized.
	public void ScreenResized() {
		/*if (ScreenSize.Y < 740)
			gameScale = 1.0;
		else if (ScreenSize.Y < 920)
			gameScale = 1.5;
		else
			gameScale = 2.0;*/
		//Point2I targetSize = (Point2I)GMath.Ceiling((Vector2D)ScreenSize / gameScale);
		Point2I targetSize = ScreenSize;
		if (GameData.RenderTargetGame != null)
			GameData.RenderTargetGame.Dispose();
		GameData.RenderTargetGame	= new RenderTarget2D(gameBase.GraphicsDevice, ScreenSize.X, ScreenSize.Y);

		if (GameData.RenderTargetDebug != null)
			GameData.RenderTargetDebug.Dispose();
		GameData.RenderTargetDebug	= new RenderTarget2D(gameBase.GraphicsDevice, ScreenSize.X, ScreenSize.Y);
	}

}
} // End namespace
