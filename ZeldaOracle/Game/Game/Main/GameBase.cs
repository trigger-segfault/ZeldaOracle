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
using XnaGame		= Microsoft.Xna.Framework.Game;
using Color			= Microsoft.Xna.Framework.Color;
using XnaKeys		= Microsoft.Xna.Framework.Input.Keys;

using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using GamePad		= ZeldaOracle.Common.Input.GamePad;
using Keyboard		= ZeldaOracle.Common.Input.Keyboard;
using Mouse			= ZeldaOracle.Common.Input.Mouse;
using Keys			= ZeldaOracle.Common.Input.Keys;
using Buttons		= ZeldaOracle.Common.Input.Buttons;
using MouseButtons	= ZeldaOracle.Common.Input.MouseButtons;

namespace ZeldaOracle.Game.Main {

// The class that manages the XNA aspects of the game.
public class GameBase : XnaGame {

	// Graphics:
	// The graphics manager.
	private GraphicsDeviceManager graphics;
	// The sprite batch to draw to.
	private SpriteBatch spriteBatch;
	// True if the game is in fullscreen mode.
	private bool fullScreen;
	// The current size of the non-fullscreen window.
	private Point2I windowSize;
	// True if the window size has been changed.
	private bool windowSizeChanged;

	// Game:
	// The instance of the game manager class.
	private GameManager game;
	// True if a screenshot was requested.
	private bool screenShotRequested;
	// The name of the requested screenshot.
	private string screenShotName;

	// Frame Rate:
	// The total number of frames passed since the last frame rate check.
	private int totalFrames;
	// The amount of time passed since the last frame rate check.
	private double elapsedTime;
	// The current frame rate of the game.
	private double fps;

	
	//-----------------------------------------------------------------------------
	// Initialization
	//-----------------------------------------------------------------------------

	// Constructs the game base class.
	public GameBase() {
		// Graphics
		this.graphics				= new GraphicsDeviceManager(this);
		this.spriteBatch			= null;
		this.Content.RootDirectory	= "Content";
		this.fullScreen				= false;
		this.windowSize				= new Point2I(160 * 4, 144 * 4);
		this.windowSizeChanged		= false;
		
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
		this.Window.AllowUserResizing		= true;
		this.graphics.PreferMultiSampling	= true;
		this.graphics.PreferredBackBufferWidth	= windowSize.X;
		this.graphics.PreferredBackBufferHeight	= windowSize.Y;
		//this.graphics.DeviceReset				+= OnGraphicsDeviceReset;
		//this.graphics.DeviceResetting			+= delegate(object sender, EventArgs e) {
		//	Console.WriteLine("Resetting");
		//};
		Form.Icon = new Icon("Game.ico");
		Form.MinimumSize = new System.Drawing.Size(32, 32);
	}

	// Allows the game to perform any initialization it needs to before starting to run.
	// This is where it can query for any required services and load any non-graphic
	// related content.  Calling base.Initialize will enumerate through any components
	// and initialize them as well.
	protected override void Initialize() {
		Console.WriteLine("Begin Initialize");

		Console.WriteLine("Initializing Input");
		//EventInput.Initialize(Window);
		Keyboard.Initialize();
		Mouse.Initialize();
		GamePad.Initialize();
	
		game = new GameManager();

		base.Initialize();

		// Create and initialize the game.
		game.Initialize(this);

		Window.ClientSizeChanged += OnClientSizeChanged;

		Console.WriteLine("End Initialize");
	}
	

	//-----------------------------------------------------------------------------
	// Content
	//-----------------------------------------------------------------------------

	// LoadContent will be called once per game and is the place to load
	// all of your content.
	protected override void LoadContent() {
		Console.WriteLine("Begin Load Content");

		// Create a new SpriteBatch, which can be used to draw textures.
		spriteBatch = new SpriteBatch(GraphicsDevice);

		AudioSystem.Initialize();
		Resources.Initialize(Content, GraphicsDevice);

		game.LoadContent(Content);

		base.LoadContent();

		Console.WriteLine("End Load Content");
	}

	// UnloadContent will be called once per game and is the place to unload
	// all content.
	protected override void UnloadContent() {
		System.Console.WriteLine("Begin Unload Content");

		//AudioSystem.Uninitialize();
		//Resources.Uninitialize();

		this.game.UnloadContent(Content);

		base.UnloadContent();

		System.Console.WriteLine("End Unload Content");
	}
	

	//-----------------------------------------------------------------------------
	// Events
	//-----------------------------------------------------------------------------

	// Called when the window has been manually resized.
	private void OnClientSizeChanged(object sender, EventArgs e) {
		Console.WriteLine("OnClientSizeChanged");
		windowSizeChanged = true;
		/*
		if (Window.ClientBounds.Width > 0 && Window.ClientBounds.Height > 0)
		{
			graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
			graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;

			//XCameraManager.UpdateViewports(myGraphics.GraphicsDevice.Viewport);
		}
		*/
		game.OnGraphicsDeviceReset(sender, e);
	}
	
    public void OnGraphicsDeviceReset(object sender, EventArgs e) {
		game.OnGraphicsDeviceReset(sender, e);
    }


	//-----------------------------------------------------------------------------
	// Updating
	//-----------------------------------------------------------------------------

	// Allows the game to run logic such as updating the world,
	// checking for collisions, gathering input, and playing audio.
	protected override void Update(GameTime gameTime) {

		// Update the fullscreen mode.
		UpdateFullScreen();

		if (windowSizeChanged) {
			game.ScreenResized();
			windowSizeChanged = false;
		}

		// Update the frame rate.
		UpdateFrameRate(gameTime);

		// Update the listeners.
		if (Form.Focused) {
			Keyboard.Enable();
			GamePad.Enable();
			Mouse.Enable();
			Keyboard.Update(gameTime);
			GamePad.Update(gameTime);
			Mouse.Update(gameTime, (IsFullScreen ? -new Vector2F(Window.ClientBounds.Location) : Vector2F.Zero));
		}
		else {
			Keyboard.Disable(false);
			GamePad.Disable(false);
			Mouse.Disable(false);
		}
		AudioSystem.Update(gameTime);

		// Update the game logic.
		game.Update((float) gameTime.ElapsedGameTime.TotalSeconds);

		base.Update(gameTime);

		// Update screenshot requests.
		UpdateScreenShot();

		//windowSizeChanged = false;
	}

	// Called every step to update the frame rate.
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

	// Called every step to update the fullscreen toggle.
	protected void UpdateFullScreen() {

		#if WINDOWS
		if (IsWindows && fullScreen != graphics.IsFullScreen) {
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
		#endif
	}

	// Called every step to update the screenshot requests.
	protected void UpdateScreenShot() {
		if (screenShotRequested) {
			screenShotRequested = false;
			SaveScreenShot();
		}
	}


	//-----------------------------------------------------------------------------
	// Drawing
	//-----------------------------------------------------------------------------

	// This is called when the game should draw itself.
	protected override void Draw(GameTime gameTime) {
		GraphicsDevice.Clear(Color.Black);

		// Update the frame rate
		DrawUpdatedFrameRate(gameTime);

		// Render the game
		Graphics2D g = new Graphics2D(spriteBatch);
		game.Draw(g);

		base.Draw(gameTime);
	}

	// Called every step to update the frame rate during the draw step.
	protected void DrawUpdatedFrameRate(GameTime gameTime) {
		totalFrames++;
	}


	//-----------------------------------------------------------------------------
	// Management
	//-----------------------------------------------------------------------------

	// Requests a screen shot to be taken at the end of the step.
	public void TakeScreenShot(string fileName = "") {
		screenShotRequested = true;
		screenShotName		= fileName;
	}

	// Takes a screenshot of the game and saves it as a png.
	private void SaveScreenShot() {
		// Screenshot function taken from http://clifton.me/screenshot-xna-csharp/
		#if WINDOWS

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
		if (!File.Exists("Screenshots")) {
			System.IO.Directory.CreateDirectory("Screenshots");
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
		Stream stream = File.OpenWrite(currentPath);
		texture.SaveAsPng(stream, width, height);
		stream.Close();
		texture.Dispose();

		#endif
	}

	
	//-----------------------------------------------------------------------------
	// Properties
	//-----------------------------------------------------------------------------

	// Returns true if the game is running on Windows.
	public bool IsWindows {
		get {
			#if WINDOWS
			return true;
			#else
			return false;
			#endif
		}
	}

	// Returns true if the game is running on the Xbox 360.
	public bool IsXbox {
		get {
			#if XBOX
			return true;
			#else
			return false;
			#endif
		}
	}

	// The current frame rate of the game.
	public double FPS {
		get { return fps; }
	}

	// Gets or sets if the game should be in fullscreen.
	public bool IsFullScreen {
		get { return fullScreen; }
		set {
			#if WINDOWS
			fullScreen = value;
			#endif
		}
	}

	#if WINDOWS
	// Gets the Windows form of the XNA game.
	public Form Form {
		get {
			#if WINDOWS
			return (Form)Form.FromHandle(Window.Handle);
			#else
			return null;
			#endif
		}
	}
	#endif

}
} // End namespace
