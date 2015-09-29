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

using ZeldaOracle.Game.Control;
using ZeldaOracle.Common.Translation;
using ZeldaOracle.Game.Control.Menus;


namespace ZeldaOracle.Game.Main {

	// The class that manages the framework of the game.
	public class GameManager {
	
		private GameBase		gameBase;			// The base game running the XNA framework.
		public int				gameScale;			// The game scale used to alter screen size and mouse properties.
		private GameControl		gameControl;
		private GameStateStack	gameStateStack;
		private int				elapsedTicks;		// The number of ticks since the start of the game.
		private bool			debugMode;
		private string[]		launchParameters;


		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------
	
		public const string GameName = "The Legend of Zelda - Oracle Engine";
	

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public GameManager(string[] launchParameters) {
			this.gameBase = null;
			this.gameScale = 4;
			this.debugMode = false;
			this.launchParameters = launchParameters;
		}

		// Initializes the game manager.
		public void Initialize(GameBase gameBase) {
			this.gameBase = gameBase;

			elapsedTicks = 0;

			FormatCodes.Initialize();
			Controls.Initialize();
			ScreenResized();

			AudioSystem.MasterVolume = 0.1f;

			// Begin the game state stack with a RoomControl.
			gameStateStack	= new GameStateStack(new StateDummy());
			gameStateStack.Begin(this);
			gameControl		= new GameControl(this);
			gameControl.StartGame();
		}

		// Uninitializes the game manager.
		public void Uninitialize() {

			this.gameBase = null;
		}

	
		//-----------------------------------------------------------------------------
		// Content
		//-----------------------------------------------------------------------------

		// Called to load game manager content.
		public void LoadContent(ContentManager content, GameBase gameBase) {
			this.gameBase = gameBase;
			
			GameData.Initialize();

			// Setup the render targets
			GameData.RenderTargetGame = new RenderTarget2D(gameBase.GraphicsDevice, GameSettings.SCREEN_WIDTH, GameSettings.SCREEN_HEIGHT);
			GameData.RenderTargetGameTemp = new RenderTarget2D(gameBase.GraphicsDevice, GameSettings.SCREEN_WIDTH, GameSettings.SCREEN_HEIGHT);
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

		}


		//-----------------------------------------------------------------------------
		// Updating
		//-----------------------------------------------------------------------------

		// Called every step to update the game.
		public void Update(float timeDelta) {

			if (Keyboard.IsKeyPressed(Keys.R) && Keyboard.IsKeyDown(Keys.LControl)) {
				while (gameStateStack.Count > 1)
					gameStateStack.Pop();
				gameControl.StartGame();
			}

			//prop.Update(1.0 / 60.0, new Point2I(ScreenSize.X - Property<int>.Width, ScreenSize.Y / 2));
			
			if (Keyboard.IsKeyPressed(Keys.F4)) {
				GameBase.IsFullScreen = !GameBase.IsFullScreen;
			}
			// Update the menu
			Controls.Update();

			// Toggle debug mode
			if (Keyboard.IsKeyPressed(Keys.F2) || (GamePad.IsButtonDown(Buttons.Back) && GamePad.IsButtonPressed(Buttons.RightStickButton))) {
				debugMode = !debugMode;
			}

			// Update the game-state stack.
			gameStateStack.Update();
			elapsedTicks++;
		}
	
	
		//-----------------------------------------------------------------------------
		// Drawing
		//-----------------------------------------------------------------------------

		// Called every step to draw the game.
		public void Draw(Graphics2D g) {
			g.UseIntegerPrecision = true;
			
			// Render the game-state stack to a buffer.
			g.SetRenderTarget(GameData.RenderTargetGame);
			g.Begin(GameSettings.DRAW_MODE_DEFAULT);
			g.Clear(Color.Black);
			gameStateStack.Draw(g);
			g.End();

			// Draw the buffer to the screen scaled.
			g.SetRenderTarget(null);
			g.ResetTranslation();
			g.Begin(GameSettings.DRAW_MODE_DEFAULT);
			g.DrawImage(GameData.RenderTargetGame, Vector2F.Zero, Vector2F.Zero, (Vector2F) gameScale, 0.0);
			g.End();
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

		// Gets the current framerate of the game.
		public double FPS {
			get { return gameBase.FPS; }
		}

		// Gets if the game is currently in debug mode.
		public bool DebugMode {
			get { return debugMode; }
			set { debugMode = value; }
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
				return new Point2I(gameBase.GraphicsDevice.Viewport.Width,
								   gameBase.GraphicsDevice.Viewport.Height) / gameScale;
			}
		}
		// Gets or sets the draw scale of the game.
		public int GameScale {
			get { return gameScale; }
			set { gameScale = GMath.Max(1, value); }
		}

		public GameControl GameControl {
			get { return gameControl; }
		}

		public int ElapsedTicks {
			get { return elapsedTicks; }
		}

		public GameState CurrentGameState {
			get { return gameStateStack.CurrentGameState; }
		}

		public string[] LaunchParameters {
			get { return launchParameters; }
		}

	}
} // End namespace
