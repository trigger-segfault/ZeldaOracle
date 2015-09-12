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
using ZeldaOracle.Common.Input;
using ZeldaOracle.Common.Input.Controls;
using ZeldaOracle.Common.Scripts;
using ZeldaOracle.Game.GameStates;
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
using ZeldaOracle.Game.Control;


namespace ZeldaOracle.Game.Main {

// The class that manages the framework of the game.
public class GameManager {
	
	// The base game running the XNA framework.
	private GameBase gameBase;

	// The game scale used to alter screen size and mouse properties.
	public double gameScale;

	private RoomControl roomControl; // TODO: replace this with a game-state stack
	
	private GameStateStack gameStateStack;
	

	//-----------------------------------------------------------------------------
	// Constants
	//-----------------------------------------------------------------------------
	
	public const string GameName = "ZeldaOracle";
	

	//-----------------------------------------------------------------------------
	// Constructors
	//-----------------------------------------------------------------------------

	public GameManager() {
		gameBase		= null;
		gameScale		= 4;
	}

	// Initializes the game manager.
	public void Initialize(GameBase gameBase) {
		this.gameBase = gameBase;

		Controls.Initialize();
		ScreenResized();
	}

	// Uninitializes the game manager.
	public void Uninitialize() {

		this.gameBase = null;
	}

	
	//-----------------------------------------------------------------------------
	// Content
	//-----------------------------------------------------------------------------

	// Called to load game manager content.
	public void LoadContent(ContentManager content) {
		GameData.Initialize();

		// Begin the game state stack with a RoomControl.
		roomControl		= new RoomControl();
		gameStateStack	= new GameStateStack(roomControl);
		gameStateStack.Begin(this);
	}

	// Called to unload game manager content.
	public void UnloadContent(ContentManager content) {
		
	}

	
	//-----------------------------------------------------------------------------
	// Gameplay
	//-----------------------------------------------------------------------------

	public void Exit() {
		gameBase.Exit();
	}

	
	//-----------------------------------------------------------------------------
	// Game state management
	//-----------------------------------------------------------------------------

	// Push a new game-state onto the stack and begin it.
	public void PushGameState(GameState state) {
		gameStateStack.Push(state);
	}
	
	// Push a queue of game states.
	public void QueueGameStates(params GameState[] states) {
		PushGameState(new GameStateQueue(states));
	}
	
	// End the top-most game state in the stack.
	public void PopGameState() {
		gameStateStack.Pop();
	}
	
	// End the given number of states in the stack from the top down.
	public void PopGameStates(int amount) {
		gameStateStack.Pop(amount);
	}


	//-----------------------------------------------------------------------------
	// Management
	//-----------------------------------------------------------------------------

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


	//-----------------------------------------------------------------------------
	// Updating
	//-----------------------------------------------------------------------------

	// Called every step to update the game.
	public void Update(float timeDelta) {

		/*if (Keyboard.IsKeyPressed(Keys.F11) && IsWindows) {
			IsFullScreen = !IsFullScreen;
		}*/

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
		
		// DEBUG: Fade the screen out and in.
		if (Keyboard.IsKeyPressed(Keys.F)) {
			QueueGameStates(
				new StateScreenFade(Color.Black, 0.5f, FadeType.FadeOut),
				new StateScreenFade(Color.Black, 0.5f, FadeType.FadeIn)
			);
		}

		/*
		// Toggle debug mode
		if (Keyboard.IsKeyPressed(Keys.Insert) || (GamePad.IsButtonDown(Buttons.Back) && GamePad.IsButtonPressed(Buttons.RightStickButton))) {
			DebugMode = !DebugMode;
		}*/

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

		// Update the game-state stack.
		gameStateStack.Update(timeDelta);
	}
	
	
	//-----------------------------------------------------------------------------
	// Drawing
	//-----------------------------------------------------------------------------

	// Called every step to draw the game.
	public void Draw(Graphics2D g) {
		
		DrawMode drawMode = new DrawMode();
		drawMode.SortMode = SpriteSortMode.Deferred;
		drawMode.BlendState = BlendState.NonPremultiplied;
		drawMode.SamplerState = SamplerState.PointClamp;

		// Render the game-state stack to a buffer.
		g.SetRenderTarget(GameData.RenderTargetGame);
		g.Begin(drawMode);
		g.Clear(Color.Black);
		gameStateStack.Draw(g);
		g.End();

		// Draw the buffer to the screen scaled.
		g.SetRenderTarget(null);
		g.ResetTranslation();
		g.Begin(drawMode);
		g.DrawImage(GameData.RenderTargetGame, Vector2F.Zero, Vector2F.Zero, (Vector2F) gameScale, 0.0);
		g.DrawImage(GameData.RenderTargetDebug, Vector2F.Zero);
		g.End();
	}

	// Called every step to draw the debug information if debug mode is enabled.
	private void DrawDebugInfo(Graphics2D g) {

	}

	
	//-----------------------------------------------------------------------------
	// Properties
	//-----------------------------------------------------------------------------
	
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

}
} // End namespace
