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
using XnaKeys		= Microsoft.Xna.Framework.Input.Keys;

using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Common.Input.Controls;
using ZeldaOracle.Common.Translation;
using ZeldaOracle.Common.Util;
using GamePad		= ZeldaOracle.Common.Input.GamePad;
using Keyboard		= ZeldaOracle.Common.Input.Keyboard;
using Mouse			= ZeldaOracle.Common.Input.Mouse;
using Keys			= ZeldaOracle.Common.Input.Keys;
using Buttons		= ZeldaOracle.Common.Input.Buttons;
using MouseButtons	= ZeldaOracle.Common.Input.MouseButtons;
using Color			= ZeldaOracle.Common.Graphics.Color;
using Song			= ZeldaOracle.Common.Audio.Song;

using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Control.Menus;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.Debug;


namespace ZeldaOracle.Game.Main {
	/// <summary>The class that manages the framework of the game.</summary>
	public class GameManager {
		/// <summary>The base game running the XNA framework.</summary>
		private GameBase		gameBase;
		/// <summary>The game scale used to alter screen size and mouse properties.</summary>
		public int				gameScale;
		/// <summary>The controller for the game.</summary>
		private GameControl		gameControl;
		/// <summary>The stack for game states.</summary>
		private GameStateStack	gameStateStack;
		/// <summary>The number of ticks since the start of the game.</summary>
		private int				elapsedTicks;
		/// <summary>True if the game is in debug mode.</summary>
		private bool			debugMode;
		/// <summary>The launch parameters for the process.</summary>
		private string[]		launchParameters;
		/// <summary>True if the game is hard-paused (not in a pause menu).</summary>
		private bool			isGamePaused;
		/// <summary>True if a console window as been allocated for this game.</summary>
		private bool            isConsoleOpen;
		/// <summary>The user settings for the game.</summary>
		private UserSettings userSettings;

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The default game window title.</summary>
		public const string GameName = "The Legend of Zelda - Oracle Engine";
		/// <summary>The default debug console window title.</summary>
		public const string ConsoleName = "The Legend of Zelda - Oracle Engine (Debug Console)";


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the game manager.</summary>
		public GameManager(string[] launchParameters, GameBase gameBase) {
			this.gameBase			= gameBase;
			this.gameScale			= 4;
			this.debugMode			= false;
			this.launchParameters	= launchParameters;
			this.isGamePaused       = false;
			this.isConsoleOpen      = false;
			this.userSettings       = new UserSettings();

			this.gameBase.Window.Title = GameName;

			ReadLaunchParameters();
		}

		/// <summary>Initializes the game manager.</summary>
		public void Initialize() {
			elapsedTicks = 0;

			FormatCodes.Initialize();
			ScreenResized();

			// Controls is initialized in here
			LoadUserSettings();

			// Begin the game state stack with a RoomControl.
			gameStateStack	= new GameStateStack(new StateDummy());
			gameStateStack.Begin(this);
			gameControl		= new GameControl(this);
			gameControl.StartGame();

			GameDebug.GameControl = gameControl;
			GameDebug.OnGameStart();
		}

		/// <summary>Uninitializes the game manager.</summary>
		public void Uninitialize() {
			this.gameBase = null;
		}


		//-----------------------------------------------------------------------------
		// Content
		//-----------------------------------------------------------------------------

		/// <summary>Called to load game manager content.</summary>
		public void LoadContent(ContentManager content) {
			GameData.Initialize();

			// Setup the render targets
			GameData.RenderTargetGame = new RenderTarget2D(gameBase.GraphicsDevice, GameSettings.SCREEN_WIDTH, GameSettings.SCREEN_HEIGHT);
			GameData.RenderTargetGameTemp = new RenderTarget2D(gameBase.GraphicsDevice, GameSettings.SCREEN_WIDTH, GameSettings.SCREEN_HEIGHT);
		}

		/// <summary>Called to unload game manager content.</summary>
		public void UnloadContent(ContentManager content) {
		
		}


		//-----------------------------------------------------------------------------
		// Settings
		//-----------------------------------------------------------------------------

		/// <summary>Loads the user settings and updates any settings that
		/// reference them.</summary>
		public bool LoadUserSettings() {
			bool result = userSettings.Load();

			// User settings are still valid even when they fail to load.
			Controls.LoadControls(userSettings);

			return result;
		}


		//-----------------------------------------------------------------------------
		// Gameplay
		//-----------------------------------------------------------------------------

		/// <summary>Exits the game.</summary>
		public void Exit() {
			gameBase.Exit();
		}

		/// <summary>Restarts the game.</summary>
		public void Restart() {
			while (gameStateStack.Count > 1)
				gameStateStack.Pop();
			gameControl.StartGame();
			GameDebug.GameControl = gameControl;
			GameDebug.OnGameStart();
		}


		//-----------------------------------------------------------------------------
		// Game State Management
		//-----------------------------------------------------------------------------

		/// <summary>Push a new game-state onto the stack and begin it.</summary>
		public void PushGameState(GameState state) {
			gameStateStack.Push(state);
		}

		/// <summary>Push a queue of game states.</summary>
		public void QueueGameStates(params GameState[] states) {
			PushGameState(new GameStateQueue(states));
		}

		/// <summary>End the top-most game state in the stack.</summary>
		public void PopGameState() {
			gameStateStack.Pop();
		}

		/// <summary>End the given number of states in the stack from the top down.</summary>
		public void PopGameStates(int amount) {
			gameStateStack.Pop(amount);
		}


		//-----------------------------------------------------------------------------
		// Management
		//-----------------------------------------------------------------------------

		/// <summary>Called when the screen has been resized.</summary>
		public void ScreenResized() {

		}


		//-----------------------------------------------------------------------------
		// Launch Parameters
		//-----------------------------------------------------------------------------

		/// <summary>Reads the launch parameters and makes any necissary changes.</summary>
		private void ReadLaunchParameters() {
			for (int i = 1; i < launchParameters.Length; i++) {
				if (launchParameters[i] == "-console") {
					IsConsoleOpen = true;
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Updating
		//-----------------------------------------------------------------------------

		/// <summary>Called every step to update the game.</summary>
		public void Update(float timeDelta) {
			//prop.Update(1.0 / 60.0, new Point2I(ScreenSize.X - Property<int>.Width, ScreenSize.Y / 2));
			
			//if (Keyboard.IsKeyPressed(Keys.F4))
			//	GameBase.IsFullScreen = !GameBase.IsFullScreen;

			// Update the menu
			Controls.Update();

			// Toggle debug mode
			if (Keyboard.IsKeyPressed(Keys.F2) || (GamePad.IsButtonDown(Buttons.Back) && GamePad.IsButtonPressed(Buttons.RightStickButton))) {
				debugMode = !debugMode;
			}

			// Update the game-state stack.
			if (!isGamePaused)
				gameStateStack.Update();
			
			elapsedTicks++;
			
			// DEBUG: Update debug keys.
			GameDebug.GameControl = gameControl;
			GameDebug.UpdateRoomDebugKeys();
		}

		/// <summary>Updates the game by one frame.</summary>
		public void NextFrame() {
			gameStateStack.Update();
		}


		//-----------------------------------------------------------------------------
		// Drawing
		//-----------------------------------------------------------------------------

		/// <summary>Called every step to draw the game.</summary>
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
			g.DrawImage(GameData.RenderTargetGame, Vector2F.Zero, Vector2F.Zero, new Vector2F(gameScale, gameScale), 0.0);
			g.End();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Containment ----------------------------------------------------------------

		/// <summary>Gets the base game running the XNA framework.</summary>
		public GameBase GameBase {
			get { return gameBase; }
		}

		/// <summary>Gets the game controller.</summary>
		public GameControl GameControl {
			get { return gameControl; }
		}

		/// <summary>Gets the current game state.</summary>
		public GameState CurrentGameState {
			get { return gameStateStack.CurrentGameState; }
		}

		/// <summary>Returns true if the game is running in windows.</summary>
		public bool IsWindows {
			get { return gameBase.IsWindows; }
		}

		/// <summary>Returns true if the game is running on the Xbox 360.</summary>
		public bool IsXbox {
			get { return gameBase.IsXbox; }
		}
		
		// Information ----------------------------------------------------------------

		/// <summary>Gets the current framerate of the game.</summary>
		public double FPS {
			get { return gameBase.FPS; }
		}

		/// <summary>Gets or sets if the game is in fullscreen mode.</summary>
		public bool IsFullScreen {
			get { return gameBase.IsFullScreen; }
			set { gameBase.IsFullScreen = value; }
		}

		/// <summary>Gets the true size of the screen.</summary>
		public Point2I ScreenSize {
			get {
				return new Point2I(gameBase.GraphicsDevice.Viewport.Width,
								   gameBase.GraphicsDevice.Viewport.Height);
			}
		}

		/// <summary>Gets the size of the screen based on the game scale.</summary>
		public Point2I GameScreenSize {
			get {
				return new Point2I(gameBase.GraphicsDevice.Viewport.Width,
								   gameBase.GraphicsDevice.Viewport.Height) / gameScale;
			}
		}

		/// <summary>Gets or sets the draw scale of the game.</summary>
		public int GameScale {
			get { return gameScale; }
			set { gameScale = GMath.Max(1, value); }
		}

		/// <summary>Gets the ellapsed ticks since the start of the game.</summary>
		public int ElapsedTicks {
			get { return elapsedTicks; }
		}

		/// <summary>Gets the launch parameters for the application.</summary>
		public string[] LaunchParameters {
			get { return launchParameters; }
		}

		// Debug ----------------------------------------------------------------------

		/// <summary>Gets if the game is currently in debug mode.</summary>
		public bool DebugMode {
			get { return debugMode; }
			set { debugMode = value; }
		}

		/// <summary>Gets or sets if the game is paused.</summary>
		public bool IsGamePaused {
			get { return isGamePaused; }
			set { isGamePaused = value; }
		}
		
		/// <summary>Opens or closes a console window for the game.</summary>
		public bool IsConsoleOpen {
			get { return isConsoleOpen; }
			set {
				if (value != isConsoleOpen) {
					isConsoleOpen = value;
					if (isConsoleOpen) {
						NativeMethods.AllocConsole();
						// stdout's handle seems to always be equal to 7
						IntPtr defaultStdout = new IntPtr(7);
						IntPtr currentStdout = NativeMethods.GetStdHandle(NativeMethods.StdOutputHandle);

						if (currentStdout != defaultStdout)
							// reset stdout
							NativeMethods.SetStdHandle(NativeMethods.StdOutputHandle, defaultStdout);

						// reopen stdout
						TextWriter writer = new StreamWriter(Console.OpenStandardOutput()) {
							AutoFlush = true
						};
						Console.SetOut(writer);

						NativeMethods.SetConsoleTitle(ConsoleName);
						gameBase.Form.Activate();
					}
					else {
						NativeMethods.FreeConsole();
					}
				}
			}
		}

	}
} // End namespace
