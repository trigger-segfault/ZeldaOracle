using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using XnaGame = Microsoft.Xna.Framework.Game;
using Color = Microsoft.Xna.Framework.Color;
using XnaKeys = Microsoft.Xna.Framework.Input.Keys;

using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using GamePad = ZeldaOracle.Common.Input.GamePad;
using Keyboard = ZeldaOracle.Common.Input.Keyboard;
using Mouse = ZeldaOracle.Common.Input.Mouse;
using Keys = ZeldaOracle.Common.Input.Keys;
using Buttons = ZeldaOracle.Common.Input.Buttons;
using MouseButtons = ZeldaOracle.Common.Input.MouseButtons;
using ZeldaOracle.Common.Util;

namespace ZeldaOracle.Game.Main {
	/// <summary>The class that manages the XNA aspects of the game.</summary>
	public class GameBase : XnaGame {

		// Graphics:
		/// <summary>The graphics manager.</summary>
		private GraphicsDeviceManager graphics;
		/// <summary>The sprite batch to draw to.</summary>
		private SpriteBatch spriteBatch;
		/// <summary>True if the game is in fullscreen mode.</summary>
		private bool fullScreen;
		/// <summary>The current size of the non-fullscreen window.</summary>
		private Point2I windowSize;
		/// <summary>True if the window size has been changed.</summary>
		private bool windowSizeChanged;

		// Game:
		/// <summary>The instance of the game manager class.</summary>
		private GameManager game;
		/// <summary>True if a screenshot was requested.</summary>
		private bool screenShotRequested;
		/// <summary>The name of the requested screenshot.</summary>
		private string screenShotName;

		private int slowTimer;

		// Frame Rate:
		/// <summary>The total number of frames passed since the last frame rate check.</summary>
		private int totalFrames;
		/// <summary>The amount of time passed since the last frame rate check.</summary>
		private double elapsedTime;
		/// <summary>The current frame rate of the game.</summary>
		private double fps;

		private bool isContentLoaded;

		/// <summary>The launch parameters for the game.</summary>
		private string[] launchParameters;


		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the game base class.</summary>
		public GameBase(string[] launchParameters) {
			// Graphics
			this.graphics				= new GraphicsDeviceManager(this);
			this.spriteBatch			= null;
			this.Content.RootDirectory	= "Content";
			this.fullScreen				= false;
			this.windowSize             = GameSettings.SCREEN_SIZE * 4;
			this.windowSizeChanged		= false;
			this.isContentLoaded		= true;
			this.launchParameters		= launchParameters;

			// Game
			this.game					= null;
			this.screenShotRequested	= false;
			this.screenShotName			= "";

			// Frame Rate
			this.totalFrames			= 0;
			this.elapsedTime			= 0.0;
			this.fps					= 0.0;

			// Setup
			this.IsMouseVisible					= true;
			this.Window.AllowUserResizing		= false;
			this.graphics.PreferMultiSampling	= true;
			this.graphics.PreferredBackBufferWidth	= windowSize.X;
			this.graphics.PreferredBackBufferHeight	= windowSize.Y;
			
			Form.Icon = new Icon("Game.ico");
			Form.MinimumSize = new Size(32, 32);
			Form.Shown += OnWindowShown;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize() {
			game = new GameManager(launchParameters, this);

			Console.WriteLine("Begin Initialize");

			Console.WriteLine("Initializing Input");
			EventInput.Initialize(Window);
			Keyboard.Initialize();
			Mouse.Initialize();
			GamePad.Initialize();
			
			base.Initialize();

			if (!isContentLoaded) {
				// BAD STUFF.
				Exit();
				return;
			}

			// Create and initialize the game.
			game.Initialize();

			Window.ClientSizeChanged += OnClientSizeChanged;

			Console.WriteLine("End Initialize");
		}


		//-----------------------------------------------------------------------------
		// Content
		//-----------------------------------------------------------------------------

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent() {
			isContentLoaded = false;

			try {
				Console.WriteLine("Begin Load Content");

				// Create a new SpriteBatch, which can be used to draw textures.
				spriteBatch = new SpriteBatch(GraphicsDevice);

				AudioSystem.Initialize();
				Resources.Initialize(spriteBatch, GraphicsDevice, Content);
				
				game.LoadContent(Content);

				base.LoadContent();

				Console.WriteLine("End Load Content");
				isContentLoaded = true;
			}
			catch (LoadContentException e) {
				//Console.WriteLine("LOAD CONTENT EXCEPTION: " + e.Message);
				e.PrintMessage();
			}
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent() {
			Console.WriteLine("Begin Unload Content");

			AudioSystem.Uninitialize();
			//Resources.Uninitialize();

			game.UnloadContent(Content);

			base.UnloadContent();

			Console.WriteLine("End Unload Content");
		}


		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------
		
		/// <summary>Called when the window is shown for the first time.
		/// This event is primarily used to steal focus from the debug console on startup.</summary>
		private void OnWindowShown(object sender, EventArgs e) {
			Form.Activate();
		}

		/// <summary>Called when the window has been manually resized.</summary>
		private void OnClientSizeChanged(object sender, EventArgs e) {
			Console.WriteLine("OnClientSizeChanged");
			windowSizeChanged = true;
		}


		//-----------------------------------------------------------------------------
		// Updating
		//-----------------------------------------------------------------------------

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		protected override void Update(GameTime gameTime) {
			if (!isContentLoaded) {
				Exit();
				return;
			}

			// Update the fullscreen mode.
			UpdateFullScreen();

			if (windowSizeChanged) {
				game.ScreenResized();
				windowSizeChanged = false;
			}

			// Update the listeners.
			if (Form.Focused) {
				Keyboard.Enable();
				GamePad.Enable();
				Mouse.Enable();
				Keyboard.Update(gameTime);
				GamePad.Update(gameTime);
				Mouse.Update(gameTime, (IsFullScreen ? -Window.ClientBounds.Location.ToVector2F() : Vector2F.Zero));
			}
			else {
				Keyboard.Disable(false);
				GamePad.Disable(false);
				Mouse.Disable(false);
			}
			AudioSystem.Update(gameTime);

			// Update the game logic.
			//game.Update((float) gameTime.ElapsedGameTime.TotalSeconds);

			// DEBUG: Hold 1 to speed up the game.
			if (Keyboard.IsKeyDown(Keys.D1)) {
				for (int i = 0; i < 16; i++)
					game.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
			}
			// DEBUG: Hold 2 to slow down the game.
			else if (Keyboard.IsKeyDown(Keys.D2)) {
				slowTimer++;
				if (slowTimer >= 10) {
					slowTimer = 0;
					game.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
				}
			}
			else {
				// Update the game logic.
				game.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
			}

			base.Update(gameTime);

			// Update screenshot requests.
			UpdateScreenShot();

			//windowSizeChanged = false;
		}

		/// <summary>Called every step to update the frame rate.</summary>
		protected void UpdateFrameRate(GameTime gameTime) {

			// FPS Counter from:
			// http://www.david-amador.com/2009/11/how-to-do-a-xna-fps-counter/
			elapsedTime		+= gameTime.ElapsedGameTime.TotalMilliseconds;
			if (elapsedTime >= 1000.0) {
				fps			= (double)totalFrames * 1000.0 / elapsedTime;
				totalFrames	= 0;
				elapsedTime	= 0.0;
			}
		}

		/// <summary>Called every step to update the fullscreen toggle.</summary>
		protected void UpdateFullScreen() {
			
			if (fullScreen != graphics.IsFullScreen) {
				Console.WriteLine("UpdateFullScreen");
				if (graphics.IsFullScreen) {
					Form.FormBorderStyle				= FormBorderStyle.None;

					//graphics.ToggleFullScreen();
					graphics.PreferredBackBufferWidth	= windowSize.X;
					graphics.PreferredBackBufferHeight	= windowSize.Y;
					graphics.IsFullScreen				= false;
					graphics.ApplyChanges();

					Form.FormBorderStyle				= FormBorderStyle.Sizable;
					Application.VisualStyleState		= VisualStyleState.ClientAndNonClientAreasEnabled;
					Form.Icon							= new Icon("Game.ico");
					Window.AllowUserResizing = true;
				}
				else {
					windowSize.X						= GraphicsDevice.Viewport.Width;
					windowSize.Y						= GraphicsDevice.Viewport.Height;
					//graphics.ToggleFullScreen();
					graphics.PreferredBackBufferWidth	= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
					graphics.PreferredBackBufferHeight	= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
					graphics.IsFullScreen				= true;
					graphics.ApplyChanges();

					Application.VisualStyleState		= VisualStyleState.ClientAndNonClientAreasEnabled;
					Form.Icon							= new Icon("Game.ico");
					Window.AllowUserResizing = true;
				}
				Console.WriteLine("End UpdateFullScreen");
			}
		}

		/// <summary>Called every step to update the screenshot requests.</summary>
		protected void UpdateScreenShot() {
			if (screenShotRequested) {
				screenShotRequested = false;
				SaveScreenShot();
			}
		}


		//-----------------------------------------------------------------------------
		// Drawing
		//-----------------------------------------------------------------------------

		/// <summary>This is called when the game should draw itself.</summary>
		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.Black);

			// Update the frame rate
			IncrementTotalFrames(gameTime);

			// Update the frame rate.
			UpdateFrameRate(gameTime);

			// Render the game
			Graphics2D g = new Graphics2D(spriteBatch);
			game.Draw(g);
			base.Draw(gameTime);
		}

		/// <summary>Called every step to update the frame rate during the draw step.</summary>
		protected void IncrementTotalFrames(GameTime gameTime) {
			totalFrames++;
		}


		//-----------------------------------------------------------------------------
		// Management
		//-----------------------------------------------------------------------------

		/// <summary>Requests a screen shot to be taken at the end of the step.</summary>
		public void TakeScreenShot(string fileName = "") {
			screenShotRequested = true;
			screenShotName		= fileName;
		}

		/// <summary>Takes a screenshot of the game and saves it as a png.</summary>
		private void SaveScreenShot() {
			// Screenshot function taken from http://clifton.me/screenshot-xna-csharp/
			
			// Get the screen size
			int width	= GraphicsDevice.PresentationParameters.BackBufferWidth;
			int height	= GraphicsDevice.PresentationParameters.BackBufferHeight;

			// Force a frame to be drawn (otherwise back buffer is empty)
			Draw(new GameTime());

			// Pull the picture from the buffer
			int[] backBuffer = new int[width * height];
			GraphicsDevice.GetBackBufferData(backBuffer);

			// Copy the screen into a texture
			Texture2D texture = new Texture2D(GraphicsDevice, width, height, false,
				GraphicsDevice.PresentationParameters.BackBufferFormat);
			texture.SetData(backBuffer);

			// Get the next available indexed file name
			int index = 1;

			// Create a folder to store the screenshots in
			if (!Directory.Exists("Screenshots")) {
				Directory.CreateDirectory("Screenshots");
			}

			// Get the screenshot file name
			string currentPath = "Screenshots/Screenshot-" + index + ".png";
			if (screenShotName.Length == 0) {
				while (File.Exists(currentPath)) {
					index++;
					currentPath = "Screenshots/Screenshot-" + index + ".png";
				}
			}
			else {
				currentPath = "Screenshots/" + screenShotName + ".png";
			}

			// Save the image to file
			using (Stream stream = File.OpenWrite(currentPath)) {
				texture.SaveAsPng(stream, width, height);
			}
			texture.Dispose();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>Returns the stored sprite batch class.</summary>
		public SpriteBatch SpriteBatch {
			get { return spriteBatch; }
		}

		/// <summary>The current frame rate of the game.</summary>
		public double FPS {
			get { return fps; }
		}

		/// <summary>Gets or sets if the game should be in fullscreen.</summary>
		public bool IsFullScreen {
			get { return fullScreen; }
			set { fullScreen = value; }
		}
		
		/// <summary>Gets the Windows form of the XNA game.</summary>
		public Form Form {
			get { return (Form)Form.FromHandle(Window.Handle); }
		}
	}
}
