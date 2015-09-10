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

namespace GameFramework.MyGame.Main {
/** <summary>
 * The class that manages the XNA aspects of the game.
 * </summary> */
public class GameBase : XnaGame {

	//========== CONSTANTS ===========
	#region Constants

	#endregion
	//=========== MEMBERS ============
	#region Members

	// Graphics
	/** <summary> The graphics manager. </summary> */
	private GraphicsDeviceManager graphics;
	/** <summary> The sprite batch to draw to. </summary> */
	private SpriteBatch spriteBatch;
	/** <summary> True if the game is in fullscreen mode. </summary> */
	private bool fullScreen;
	/** <summary> The current size of the non-fullscreen window. </summary> */
	private Point2I windowSize;
	/** <summary> True if the window size has been changed. </summary> */
	private bool windowSizeChanged;

	// Game
	/** <summary> The instance of the game manager class. </summary> */
	private GameManager game;
	/** <summary> True if a screenshot was requested. </summary> */
	private bool screenShotRequested;
	/** <summary> The name of the requested screenshot. </summary> */
	private string screenShotName;
	/** <summary> True if the game has finished starting up. </summary> */
	private bool started;

	// Frame Rate
	/** <summary> The total number of frames passed since the last frame rate check. </summary> */
	private int totalFrames;
	/** <summary> The amount of time passed since the last frame rate check. </summary> */
	private double elapsedTime;
	/** <summary> The current frame rate of the game. </summary> */
	private double fps;


	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the game base class. </summary> */
	public GameBase() {
		// Graphics
		this.graphics				= new GraphicsDeviceManager(this);
		this.spriteBatch			= null;
		this.Content.RootDirectory	= "Content";
		this.fullScreen				= false;
		this.windowSize				= new Point2I(1024, 800);
		this.windowSizeChanged		= false;

		// Game
		this.game					= null;
		this.screenShotRequested	= false;
		this.screenShotName			= "";
		this.started				= false;

		// Frame Rate
		this.totalFrames			= 0;
		this.elapsedTime			= 0.0;
		this.fps					= 0.0;

		// Setup
		this.IsMouseVisible					= true;
		this.Window.AllowUserResizing		= true;
		this.graphics.PreferMultiSampling	= true;
		this.graphics.PreferredBackBufferWidth	= windowSize.X;
		this.graphics.PreferredBackBufferHeight	= windowSize.Y;
		Form.Icon = new Icon("Game.ico");
		Form.MinimumSize = new System.Drawing.Size(32, 32);

	}
	/** <summary>
	 * Allows the game to perform any initialization it needs to before starting to run.
	 * This is where it can query for any required services and load any non-graphic
	 * related content.  Calling base.Initialize will enumerate through any components
	 * and initialize them as well.
	 * </summary> */
	protected override void Initialize() {
		Console.WriteLine("Begin Initialize");

		// Create and initialize the game
		this.game = new GameManager();
		this.game.Initialize(this);

		base.Initialize();

		Console.WriteLine("Initializing Input");
		EventInput.Initialize(Window);
		Keyboard.Initialize();
		Mouse.Initialize();
		GamePad.Initialize();


		this.Window.ClientSizeChanged			+= OnClientSizeChanged;

		Console.WriteLine("End Initialize");
	}

	#endregion
	//=========== CONTENT ============
	#region Content

	/** <summary>
	 * LoadContent will be called once per game and is the place to load
	 * all of your content.
	 * </summary> */
	protected override void LoadContent() {
		Console.WriteLine("Begin Load Content");

		// Create a new SpriteBatch, which can be used to draw textures.
		this.spriteBatch	= new SpriteBatch(GraphicsDevice);

		AudioSystem.Initialize();
		Resources.Initialize(Content, GraphicsDevice);

		this.game.LoadContent(Content);

		base.LoadContent();

		Console.WriteLine("End Load Content");
	}
	/** <summary>
	 * UnloadContent will be called once per game and is the place to unload
	 * all content.
	 * </summary> */
	protected override void UnloadContent() {
		System.Console.WriteLine("Begin Unload Content");

		//AudioSystem.Uninitialize();
		//Resources.Uninitialize();

		this.game.UnloadContent(Content);

		base.UnloadContent();

		System.Console.WriteLine("End Unload Content");
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties

	/** <summary> Returns true if the game is running on Windows. </summary> */
	public bool IsWindows {
		get {
			#if WINDOWS
			return true;
			#else
			return false;
			#endif
		}
	}
	/** <summary> Returns true if the game is running on the Xbox 360. </summary> */
	public bool IsXbox {
		get {
			#if XBOX
			return true;
			#else
			return false;
			#endif
		}
	}
	/** <summary> The current frame rate of the game. </summary> */
	public double FPS {
		get { return fps; }
	}
	/** <summary> Gets or sets if the game should be in fullscreen. </summary> */
	public bool IsFullScreen {
		get { return fullScreen; }
		set {
			#if WINDOWS
			fullScreen = value;
			#endif
		}
	}
	#if WINDOWS
	/** <summary> Gets the Windows form of the XNA game. </summary> */
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

	#endregion
	//============ EVENTS ============
	#region Events
	//--------------------------------
	#region Game Window

	/** <summary> Called when the window has been manually resized. </summary> */
	private void OnClientSizeChanged(object sender, EventArgs e) {
		Console.WriteLine("OnClientSizeChanged");
		windowSizeChanged = true;
	}

	#endregion
	//--------------------------------
	#endregion
	//=========== UPDATING ===========
	#region Updating

	/** <summary>
	 * Allows the game to run logic such as updating the world,
	 * checking for collisions, gathering input, and playing audio.
	 * </summary>
	 * <param name="gameTime">Provides a snapshot of timing values.</param> */
	protected override void Update(GameTime gameTime) {

		// Update the fullscreen mode
		UpdateFullScreen();

		if (windowSizeChanged) {
			game.ScreenResized();
			windowSizeChanged = false;
		}

		// Update the frame rate
		UpdateFrameRate(gameTime);

		// Update the listeners
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

		// Update the game logic
		game.Update();

		base.Update(gameTime);

		// Update screenshot requests
		UpdateScreenShot();

		//windowSizeChanged = false;
	}
	/** <summary> Called every step to update the frame rate. </summary> */
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
	/** <summary> Called every step to update the fullscreen toggle. </summary> */
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
	/** <summary> Called every step to update the screenshot requests. </summary> */
	protected void UpdateScreenShot() {

		if (screenShotRequested) {
			screenShotRequested = false;
			SaveScreenShot();
		}
	}

	#endregion
	//=========== DRAWING ============
	#region Drawing

	/** <summary>
	 * This is called when the game should draw itself.
	 * </summary>
	 * <param name="gameTime">Provides a snapshot of timing values.</param> */
	protected override void Draw(GameTime gameTime) {

		GraphicsDevice.Clear(Color.Black);

		// Update the frame rate
		DrawUpdatedFrameRate(gameTime);

		// Render the game
		Graphics2D g = new Graphics2D(spriteBatch);
		game.Draw(g);

		base.Draw(gameTime);
	}
	/** <summary> Called every step to update the frame rate during the draw step. </summary> */
	protected void DrawUpdatedFrameRate(GameTime gameTime) {
		totalFrames++;
	}

	#endregion
	//========== MANAGEMENT ==========
	#region Management

	/** <summary> Requests a screen shot to be taken at the end of the step. </summary> */
	public void TakeScreenShot(string fileName = "") {
		screenShotRequested = true;
		screenShotName		= fileName;
	}
	/** <summary> Takes a screenshot of the game and saves it as a png. </summary> */
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

	#endregion
}
} // End namespace
