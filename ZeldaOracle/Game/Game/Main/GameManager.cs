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
using ZeldaOracle.Common.Graphics.Particles;
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
using GameFramework.MyGame.Editor;
using GameFramework.MyGame.Editor.Properties;

namespace GameFramework.MyGame.Main {
/** <summary>
 * The class that manages the framework of the game.
 * </summary> */
public class GameManager {
	
	//========== CONSTANTS ===========
	#region Constants

	/** <summary> The name of the game. </summary> */
	public const string GameName	= "ReEntry";

	#endregion
	//=========== MEMBERS ============
	#region Members

	// Containment
	/** <summary> The base game running the XNA framework. </summary> */
	private GameBase gameBase;

	// Game
	/** <summary> The current room the game is in. </summary> */
	private Room room;
	/** <summary> The next room the game will switch to. </summary> */
	private Room roomNext;
	/** <summary> The list of rooms in the game. </summary> */
	private List<Room> rooms;
	/** <summary> The menu currently displaying in the game. </summary> */
	protected Menu menu;
	/** <summary> The next menu to be opened in the game. </summary> */
	protected Menu menuNext;
	/** <summary> The game scale used to alter screen size and mouse properties. </summary> */
	public double gameScale;

	// Other
	/** <summary> The index of the current language. </summary> */
	private int language;

	// Debug
	/** <summary> True if the game is in debug mode. </summary> */
	private bool debugMode;
	/** <summary> The debug controller for the game. </summary> */
	private DebugController debugController;


	public ParticleSystem particleSystem;

	public ParticleEffectType effectType;
	public int effectIndex;
	public ParticlePropertyGridBase grid;

	public Vector2F effectPos;


	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default game manager. </summary> */
	public GameManager() {
		this.gameBase		= null;

		this.room			= null;
		this.roomNext		= null;
		this.rooms			= new List<Room>();
		this.menu			= null;
		this.menuNext		= null;

		this.language		= 0;
		this.debugMode		= false;
		this.gameScale		= 1;
	}
	/** <summary> Initializes the game manager. </summary> */
	public void Initialize(GameBase gameBase) {
		this.gameBase		= gameBase;

		this.room			= null;

		this.debugController	= new DebugController(this);

		ScreenResized();
	}
	/** <summary> Uninitializes the game manager. </summary> */
	public void Uninitialize() {
		ClearRooms();

		this.room			= null;
		this.roomNext		= null;
		this.rooms			= null;
		this.gameBase		= null;
	}

	#endregion
	//=========== CONTENT ============
	#region Content

	/** <summary> Called to load game manager content. </summary> */
	public void LoadContent(ContentManager content) {

		this.room = new Room();
		AddRoom(this.room);
		this.room.EnterRoom();

		GameData.Initialize();

		this.debugController.DebugMenuKey = new Key(Keys.F2);
		this.debugController.DebugMenuMouseButton = new MouseButton(MouseButtons.Middle);
		this.debugController.DebugMenuButton = new ComboControl(new Control[] {new Button(Buttons.Back), new Button(Buttons.Start)});

		this.debugController.DebugMenuFont = GameData.FontDebugMenu;
		this.debugController.DebugMenuFontBold = GameData.FontDebugMenuBold;
		this.debugController.DebugMenuSprites = GameData.SheetDebugMenu;

		this.particleSystem	= new ParticleSystem();
		this.particleSystem.DrawMode.BlendState = BlendState.AlphaBlend;

		//AudioSystem.StartPlaylist(Resources.GetPlaylist("playlist1"));

		effectIndex = 0;
		ParticleEffectType[] effects = new ParticleEffectType[Resources.particleEffects.Count];
		Resources.particleEffects.Values.CopyTo(effects, 0);
		effectType = effects[effectIndex];

		grid = new ParticlePropertyGrid(this, Resources.ParticleTypes[0], Resources.ParticleEmitters[0], Resources.ParticleEffects[0]);
		effectPos = new Point2I((1024 - grid.GridWidth) / 2, 800 / 2);
		/*grid = new PropertyGrid(this);
		PropertyGroup group;

		group = new PropertyGroup("General");
		group.AddProperty(new Property("Name", "particle_1", "particle_1"));
		group.AddProperty(new PropertyRangeD("Life Span", new RangeD(1), new RangeD(1)));
		group.AddProperty(new PropertyDouble("Fade Delay", -1.0, -1.0, new RangeD(-1, Double.PositiveInfinity)));
		grid.AddProperty(group);


		group = new PropertyGroup("Scale");
		group.AddProperty(new PropertyRangeD("Initial Scale", new RangeD(1), new RangeD(1)));
		group.AddProperty(new PropertyRangeD("Scale Increase", RangeD.Zero, RangeD.Zero));
		group.AddProperty(new PropertyDouble("Scale Jitter", 0.0, 0.0, RangeD.Full));
		group.AddProperty(new PropertyRangeD("Scale Clamp", new RangeD(-10000, 10000), new RangeD(-10000, 10000)));
		grid.AddProperty(group);


		group = new PropertyGroup("Speed");
		group.AddProperty(new PropertyDouble("Position Jitter", 0.0, 0.0, RangeD.Full));
		group.AddProperty(new PropertyRangeD("Initial Speed", RangeD.Zero, RangeD.Zero));
		group.AddProperty(new PropertyRangeD("Speed Increase", RangeD.Zero, RangeD.Zero));
		group.AddProperty(new PropertyRangeD("Speed Friction", RangeD.Zero, RangeD.Zero));
		grid.AddProperty(group);


		group = new PropertyGroup("Rotation");
		group.AddProperty(new PropertyRangeD("Initial Rotation", RangeD.Zero, RangeD.Zero));
		group.AddProperty(new PropertyRangeD("Rotation Increase", RangeD.Zero, RangeD.Zero));
		group.AddProperty(new PropertyDouble("Rotation Friction", 0.0, 0.0, RangeD.Full));
		group.AddProperty(new PropertyDouble("Rotation Jitter", 0.0, 0.0, RangeD.Full));
		grid.AddProperty(group);


		group = new PropertyGroup("Sprites");
		group.AddProperty(new PropertyDouble("Animation Speed", 1.0, 1.0, new RangeD(0.0, Double.PositiveInfinity)));
		PropertyCombo combo = (PropertyCombo)group.AddProperty(new PropertyCombo("Sprite Sheet", "sheet_particles", "sheet_particles"));
		combo.AddItem("sheet_particles");
		combo.AddItem("sheet_background");
		combo.AddItem("sheet_entities");
		combo.Selection = 0;

		PropertyList sprites = (PropertyList)group.AddProperty(new PropertyList("Sprites", "Sprite", "Sprites", delegate() {
			return new Property("", "sprite_1", "");
		}));

		PropertyList colors = (PropertyList)group.AddProperty(new PropertyList("Colors", "Color", "Colors", delegate() {
			return new PropertyColor("", true, Color.White, Color.White);
		}));
		colors.AddProperty(new PropertyColor("", true, Color.White, Color.White));

		PropertyComboStruct comboStruct = (PropertyComboStruct)group.AddProperty(new PropertyComboStruct("Structs", null, null));
		comboStruct.AddItem(new PropertyPoint2I("Point", false, Point2I.Zero, Point2I.Zero));
		comboStruct.AddItem(new PropertyVector2D("Vector", false, Vector2D.Zero, Vector2D.Zero));
		comboStruct.AddItem(new PropertyColor("Color", true, Color.White, Color.White));
		comboStruct.AddItem(new PropertyRangeD("Range", RangeD.Zero, RangeD.Zero));


		comboStruct = (PropertyComboStruct)group.AddProperty(new PropertyComboStruct("Emitter Area", null, null));
		comboStruct.AddItem(new PropertyPointArea("Point", new PointArea(0, 0), new PointArea(0, 0)));
		comboStruct.AddItem(new PropertyLineArea("Line", new LineArea(0, 0, 0, 0), new LineArea(0, 0, 0, 0)));
		comboStruct.AddItem(new PropertyRectArea("Rect", new RectArea(0, 0, 0, 0), new RectArea(0, 0, 0, 0)));
		comboStruct.AddItem(new PropertyCircleArea("Circle", new CircleArea(0, 0, 0), new CircleArea(0, 0, 0)));

		PropertyList list = (PropertyList)group.AddProperty(new PropertyList("List", "Emitter", "Emitters", delegate() {
			PropertyComboStruct comboStruct2 = new PropertyComboStruct("Emitter Area", null, null);
			comboStruct2.AddItem(new PropertyPointArea("Point", new PointArea(0, 0), new PointArea(0, 0)));
			comboStruct2.AddItem(new PropertyLineArea("Line", new LineArea(0, 0, 0, 0), new LineArea(0, 0, 0, 0)));
			comboStruct2.AddItem(new PropertyRectArea("Rect", new RectArea(0, 0, 0, 0), new RectArea(0, 0, 0, 0)));
			comboStruct2.AddItem(new PropertyCircleArea("Circle", new CircleArea(0, 0, 0), new CircleArea(0, 0, 0)));
			comboStruct2.Selection = 0;
			return comboStruct2;
		}));

		list = (PropertyList)group.AddProperty(new PropertyList("Actions", "Emitter", "Emitters", delegate() {
			PropertyComboStruct comboStruct2 = new PropertyComboStruct("Emitter Area", null, null);
			comboStruct2.AddItem(new PropertyEffectAction("Burst", false));
			comboStruct2.AddItem(new PropertyEffectAction("Stream", true));
			comboStruct2.Selection = 0;
			return comboStruct2;
		}));
		


		grid.AddProperty(group);*/

	}
	/** <summary> Called to unload game manager content. </summary> */
	public void UnloadContent(ContentManager content) {

	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Containment

	/** <summary> Gets the base game running the XNA framework. </summary> */
	public GameBase GameBase {
		get { return gameBase; }
	}
	/** <summary> Returns true if the game is running in windows. </summary> */
	public bool IsWindows {
		get { return gameBase.IsWindows; }
	}
	/** <summary> Returns true if the game is running on the Xbox 360. </summary> */
	public bool IsXbox {
		get { return gameBase.IsXbox; }
	}

	#endregion
	//--------------------------------
	#region Information

	/** <summary> Gets the current framerate of the game. </summary> */
	public double FPS {
		get { return gameBase.FPS; }
	}
	/** <summary> Gets or sets if the game is in fullscreen mode. </summary> */
	public bool IsFullScreen {
		get { return gameBase.IsFullScreen; }
		set { gameBase.IsFullScreen = value; }
	}
	/** <summary> Gets the true size of the screen. </summary> */
	public Point2I ScreenSize {
		get {
			return new Point2I(gameBase.GraphicsDevice.Viewport.Width,
							   gameBase.GraphicsDevice.Viewport.Height);
		}
	}
	/** <summary> Gets the size of the screen based on the game scale. </summary> */
	public Point2I GameScreenSize {
		get {
			return (Point2I)GMath.Ceiling(new Vector2F(gameBase.GraphicsDevice.Viewport.Width,
													   gameBase.GraphicsDevice.Viewport.Height) / gameScale);
		}
	}
	/** <summary> Gets or sets the draw scale of the game. </summary> */
	public double GameScale {
		get { return gameScale; }
		set { gameScale = GMath.Max(0.1, value); }
	}

	#endregion
	//--------------------------------
	#region Game

	/** <summary> Gets the current room the game is in. </summary> */
	public Room Room {
		get { return room; }
	}
	/** <summary> Gets the list of rooms in the game. </summary> */
	public List<Room> Rooms {
		get { return rooms; }
	}
	/** <summary> Gets or sets the current language of the game. </summary> */
	public int Language {
		get { return language; }
		set { language = GMath.Max(0, value); }
	}

	#endregion
	//--------------------------------
	#region Debug

	/** <summary> Gets or sets if the game is in debug mode. </summary> */
	public bool DebugMode {
		get { return debugMode; }
		set { debugMode = value; }
	}
	/** <summary> Gets the debug controller of the game. </summary> */
	public DebugController DebugController {
		get { return debugController; }
	}

	#endregion
	//--------------------------------
	#endregion
	//=========== UPDATING ===========
	#region Updating

	/** <summary> Called every step to update the game. </summary> */
	public void Update() {

		// Update room changes
		UpdateRoomChange();

		/*if (Keyboard.IsKeyPressed(Keys.F11) && IsWindows) {
			IsFullScreen = !IsFullScreen;
		}*/

		// Update the room
		if (!debugController.IsGamePaused || debugController.nextStep) {
			room.Update();
			particleSystem.Update(1.0 / 60.0 * debugController.timeScale);
			debugController.nextStep = false;
		}

		debugController.Update();

		//prop.Update(1.0 / 60.0, new Point2I(ScreenSize.X - Property<int>.Width, ScreenSize.Y / 2));

		int nameWidth = grid.nameWidth;
		int valueWidth = grid.valueWidth;
		if (grid.particleChanged) {
			grid = new ParticlePropertyGrid(this,
				Resources.GetParticleType(grid.propParticleSelection.SelectionName),
				Resources.GetParticleEmitter(grid.propEmitterSelection.SelectionName),
				Resources.GetParticleEffect(grid.propEffectSelection.SelectionName));
		}
		else if (grid.emitterChanged) {
			grid = new EmitterPropertyGrid(this,
				Resources.GetParticleType(grid.propParticleSelection.SelectionName),
				Resources.GetParticleEmitter(grid.propEmitterSelection.SelectionName),
				Resources.GetParticleEffect(grid.propEffectSelection.SelectionName));
		}
		else if (grid.effectChanged) {
			grid = new EffectPropertyGrid(this,
				Resources.GetParticleType(grid.propParticleSelection.SelectionName),
				Resources.GetParticleEmitter(grid.propEmitterSelection.SelectionName),
				Resources.GetParticleEffect(grid.propEffectSelection.SelectionName));
		}

		if (grid.particleState == 1) {
			ParticleType p = new ParticleType();
			int nextNameIndex = 1;

			while (Resources.ParticleTypeExists("type_" + nextNameIndex.ToString())) {
				nextNameIndex++;
			}
			p.Name = "type_" + nextNameIndex.ToString();

			Resources.AddParticleType(p);
			int selection = Resources.ParticleTypeCount - 1;

			PropertyParticleList.UpdateList();

			grid = new ParticlePropertyGrid(this,
				p,
				Resources.GetParticleEmitter(grid.propEmitterSelection.SelectionName),
				Resources.GetParticleEffect(grid.propEffectSelection.SelectionName));
		}
		else if (grid.particleState == 2) {
			ParticleType p = new ParticleType(Resources.GetParticleType(grid.propParticleSelection.SelectionName));
			int nextNameIndex = 1;

			while (Resources.ParticleTypeExists(grid.propParticleSelection.SelectionName + "_copy_" + nextNameIndex.ToString())) {
				nextNameIndex++;
			}
			p.Name = grid.propParticleSelection.SelectionName + "_copy_" + nextNameIndex.ToString();

			Resources.AddParticleType(p);
			int selection = Resources.ParticleTypeCount - 1;

			PropertyParticleList.UpdateList();

			grid = new ParticlePropertyGrid(this,
				p,
				Resources.GetParticleEmitter(grid.propEmitterSelection.SelectionName),
				Resources.GetParticleEffect(grid.propEffectSelection.SelectionName));
		}
		else if (grid.particleState == 3) {
			Resources.RemoveParticleType(grid.particle);
			int selection = GMath.Max(0, grid.propParticleSelection.Selection - 1);

			PropertyParticleList.UpdateList();

			if (grid is ParticlePropertyGrid) {
				grid = new ParticlePropertyGrid(this,
					Resources.ParticleTypes[selection],
					Resources.GetParticleEmitter(grid.propEmitterSelection.SelectionName),
					Resources.GetParticleEffect(grid.propEffectSelection.SelectionName));
			}
			else if (grid is EmitterPropertyGrid) {
				grid = new EmitterPropertyGrid(this,
					Resources.ParticleTypes[selection],
					Resources.GetParticleEmitter(grid.propEmitterSelection.SelectionName),
					Resources.GetParticleEffect(grid.propEffectSelection.SelectionName));
			}
			else if (grid is EffectPropertyGrid) {
				grid = new EffectPropertyGrid(this,
					Resources.ParticleTypes[selection],
					Resources.GetParticleEmitter(grid.propEmitterSelection.SelectionName),
					Resources.GetParticleEffect(grid.propEffectSelection.SelectionName));
			}
		}
		else if (grid.emitterState == 1) {
			ParticleEmitter e = new ParticleEmitter();
			int nextNameIndex = 1;

			while (Resources.ParticleEmitterExists("emitter_" + nextNameIndex.ToString())) {
				nextNameIndex++;
			}
			e.Name = "emitter_" + nextNameIndex.ToString();

			Resources.AddParticleEmitter(e);
			int selection = Resources.ParticleEmitterCount - 1;

			PropertyEmitterList.UpdateList();

			grid = new EmitterPropertyGrid(this,
				Resources.GetParticleType(grid.propParticleSelection.SelectionName),
				e,
				Resources.GetParticleEffect(grid.propEffectSelection.SelectionName));
		}
		else if (grid.emitterState == 2) {
			ParticleEmitter e = new ParticleEmitter(Resources.GetParticleEmitter(grid.propEmitterSelection.SelectionName));
			int nextNameIndex = 1;

			while (Resources.ParticleEmitterExists(grid.propEmitterSelection.SelectionName + "_copy_" + nextNameIndex.ToString())) {
				nextNameIndex++;
			}
			e.Name = grid.propEmitterSelection.SelectionName + "_copy_" + nextNameIndex.ToString();

			Resources.AddParticleEmitter(e);
			int selection = Resources.ParticleEmitterCount - 1;

			PropertyEmitterList.UpdateList();

			grid = new EmitterPropertyGrid(this,
				Resources.GetParticleType(grid.propParticleSelection.SelectionName),
				e,
				Resources.GetParticleEffect(grid.propEffectSelection.SelectionName));
		}
		else if (grid.emitterState == 3) {
			Resources.RemoveParticleEmitter(grid.emitter);
			int selection = GMath.Max(0, grid.propEmitterSelection.Selection - 1);

			PropertyEmitterList.UpdateList();

			if (grid is ParticlePropertyGrid) {
				grid = new ParticlePropertyGrid(this,
					Resources.GetParticleType(grid.propParticleSelection.SelectionName),
					Resources.ParticleEmitters[selection],
					Resources.GetParticleEffect(grid.propEffectSelection.SelectionName));
			}
			else if (grid is EmitterPropertyGrid) {
				grid = new EmitterPropertyGrid(this,
					Resources.GetParticleType(grid.propParticleSelection.SelectionName),
					Resources.ParticleEmitters[selection],
					Resources.GetParticleEffect(grid.propEffectSelection.SelectionName));
			}
			else if (grid is EffectPropertyGrid) {
				grid = new EffectPropertyGrid(this,
					Resources.GetParticleType(grid.propParticleSelection.SelectionName),
					Resources.ParticleEmitters[selection],
					Resources.GetParticleEffect(grid.propEffectSelection.SelectionName));
			}
		}
		else if (grid.effectState == 1) {
			ParticleEffectType e = new ParticleEffectType();
			int nextNameIndex = 1;

			while (Resources.ParticleEffectExists("effect_" + nextNameIndex.ToString())) {
				nextNameIndex++;
			}
			e.Name = "effect_" + nextNameIndex.ToString();

			Resources.AddParticleEffect(e);
			int selection = Resources.ParticleEffectCount - 1;

			PropertyEffectList.UpdateList();

			grid = new EffectPropertyGrid(this,
				Resources.GetParticleType(grid.propParticleSelection.SelectionName),
				Resources.GetParticleEmitter(grid.propEmitterSelection.SelectionName),
				e);
		}
		else if (grid.effectState == 2) {
			ParticleEffectType e = new ParticleEffectType(Resources.GetParticleEffect(grid.propEffectSelection.SelectionName));
			int nextNameIndex = 1;

			while (Resources.ParticleEffectExists(grid.propEffectSelection.SelectionName + "_copy_" + nextNameIndex.ToString())) {
				nextNameIndex++;
			}
			e.Name = grid.propEffectSelection.SelectionName + "_copy_" + nextNameIndex.ToString();

			Resources.AddParticleEffect(e);
			int selection = Resources.ParticleEffectCount - 1;

			PropertyEffectList.UpdateList();

			grid = new EffectPropertyGrid(this,
				Resources.GetParticleType(grid.propParticleSelection.SelectionName),
				Resources.GetParticleEmitter(grid.propEmitterSelection.SelectionName),
				e);
		}
		else if (grid.effectState == 3) {
			Resources.RemoveParticleEffect(grid.effect);
			int selection = GMath.Max(0, grid.propEffectSelection.Selection - 1);

			PropertyEffectList.UpdateList();

			if (grid is ParticlePropertyGrid) {
				grid = new ParticlePropertyGrid(this,
					Resources.GetParticleType(grid.propParticleSelection.SelectionName),
					Resources.GetParticleEmitter(grid.propEmitterSelection.SelectionName),
					Resources.ParticleEffects[selection]);
			}
			else if (grid is EmitterPropertyGrid) {
				grid = new EmitterPropertyGrid(this,
					Resources.GetParticleType(grid.propParticleSelection.SelectionName),
					Resources.GetParticleEmitter(grid.propEmitterSelection.SelectionName),
					Resources.ParticleEffects[selection]);
			}
			else if (grid is EffectPropertyGrid) {
				grid = new EffectPropertyGrid(this,
					Resources.GetParticleType(grid.propParticleSelection.SelectionName),
					Resources.GetParticleEmitter(grid.propEmitterSelection.SelectionName),
					Resources.ParticleEffects[selection]);
			}
		}
		grid.nameWidth = nameWidth;
		grid.valueWidth = valueWidth;

		grid.Update(1.0 / 60.0);

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

		/*if (Keyboard.IsKeyPressed(Keys.M)) {
			AudioSystem.NextSong();
		}
		if (Keyboard.IsKeyPressed(Keys.N)) {
			AudioSystem.PlaySound("player.laser");
		}
		if (Keyboard.IsKeyPressed(Keys.B)) {
			AudioSystem.PlaySound("player.respawn");
		}

		if (Keyboard.IsKeyPressed(Keys.C))
			particleSystem.Clear();
		if (Keyboard.IsKeyPressed(Keys.P)) {
			Console.WriteLine("Explosion");
			particleSystem.CreateEffect(
					Resources.GetParticleEffect("boss_explosion"),
					ScreenSize / 2);
		}*/

		if (grid.displayChanged || effectType != grid.propDisplaySelection.SelectionValue) {
			effectIndex = grid.propDisplaySelection.Selection;
			particleSystem.Clear();
			ParticleEffectType[] effects = new ParticleEffectType[Resources.particleEffects.Count];
			Resources.particleEffects.Values.CopyTo(effects, 0);
			effectType = effects[effectIndex];
			particleSystem.CreateEffect(effectType, (Vector2F)effectPos / gameScale);

			grid.displayChanged = false;
		}

		if (Keyboard.IsKeyPressed(Keys.PageUp)) {
			effectIndex = grid.propDisplaySelection.Selection;
			effectIndex--;
			if (effectIndex < 0)
				effectIndex = Resources.particleEffects.Count - 1;
			particleSystem.Clear();
			ParticleEffectType[] effects = new ParticleEffectType[Resources.particleEffects.Count];
			Resources.particleEffects.Values.CopyTo(effects, 0);
			effectType = effects[effectIndex];
			particleSystem.CreateEffect(effectType, (Vector2F)effectPos / gameScale);
			grid.propDisplaySelection.Selection = effectIndex;
		}
		if (Keyboard.IsKeyPressed(Keys.PageDown)) {
			effectIndex = grid.propDisplaySelection.Selection;
			effectIndex++;
			if (effectIndex >= Resources.particleEffects.Count)
				effectIndex = 0;
			particleSystem.Clear();
			ParticleEffectType[] effects = new ParticleEffectType[Resources.particleEffects.Count];
			Resources.particleEffects.Values.CopyTo(effects, 0);
			effectType = effects[effectIndex];
			particleSystem.CreateEffect(effectType, (Vector2F)effectPos / gameScale);
			grid.propDisplaySelection.Selection = effectIndex;
		}
		if ((particleSystem.EffectCount == 0 && particleSystem.ParticleCount == 0) || Keyboard.IsKeyPressed(Keys.End)) {
			particleSystem.Clear();
			particleSystem.CreateEffect(effectType, (Vector2F)effectPos / gameScale);
		}

		/*if (Keyboard.IsKeyPressed(Keys.S) && Keyboard.IsKeyDown(Keys.LCtrl)) {
			
		}*/

		if (Mouse.IsButtonDown(MouseButtons.Right) && Mouse.GetPosition().X < ScreenSize.X - grid.GridWidth - 6) {

			Vector2F oldPos = effectPos;
			effectPos = Mouse.GetPosition();
			Vector2F offset = effectPos - oldPos;

			/*for (int i = 0; i < particleSystem.ParticleCount; i++) {
				particleSystem.Particles[i].Position += offset;
			}*/
			for (int i = 0; i < particleSystem.EffectCount; i++) {
				particleSystem.Effects[i].Position += offset;
			}
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
	/** <summary> Called every step to update the next room if a new one has been selected. </summary> */
	private void UpdateRoomChange() {
		if (roomNext != null) {
			room.LeaveRoom();
			if (!RoomExists(room)) {
				room.Uninitialize();
			}
			room = roomNext;
			room.EnterRoom();
			roomNext = null;
		}
	}

	#endregion
	//=========== GAMEPLAY ===========
	#region Gameplay

	public void Exit() {
		gameBase.Exit();
	}

	#endregion
	//=========== DRAWING ============
	#region Drawing

	/** <summary> Called every step to draw the game. </summary> */
	public void Draw(Graphics2D g) {

		g.SetRenderTarget(GameData.RenderTargetGame);
		g.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
		g.Clear(Color.Black);

		// Draw the room
		//g.DrawImage(GameData.ImageSMG2Logo, Vector2D.Zero);
		room.Draw(g);


		g.End();
		particleSystem.Draw(g);

		// Draw the debug info
		g.SetRenderTarget(GameData.RenderTargetDebug);
		g.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
		g.Clear(Color.Transparent);
		if (Mouse.IsButtonDown(MouseButtons.Right) && debugController.showParticlePos) {
			int stretch = 8;
			Vector2F mousePos = Mouse.GetPosition();
			g.DrawLine(new Line2F(mousePos - new Vector2F(stretch, 0), mousePos + new Vector2F(stretch, 0)), 1, Color.White);
			g.DrawLine(new Line2F(mousePos - new Vector2F(0, stretch), mousePos + new Vector2F(0, stretch)), 1, Color.White);
		}
		grid.Draw(g);
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
	/** <summary> Called every step to draw the debug information if debug mode is enabled. </summary> */
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

	#endregion
	//========== MANAGEMENT ==========
	#region Management

	/** <summary> Called when the screen has been resized. </summary> */
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
	/** <summary> Changes the room. </summary> */
	public void ChangeRoom(Room room) {
		if (this.room != room) {
			if (!RoomExists(room)) {
				room.Initialize(this);
			}
			roomNext = room;
		}
	}
	/** <summary> Changes the room. </summary> */
	public void ChangeRoom(string id) {
		if (RoomExists(id)) {
			roomNext = GetRoom(id);
		}
	}
	/** <summary> Closes the currently opened menu. </summary> */
	public void CloseMenu() {
		menuNext = null;
	}

	#endregion
	//============ ROOMS =============
	#region Rooms

	/** <summary> Clears all the rooms from the game. </summary> */
	public void ClearRooms() {
		while (rooms.Count > 0) {
			rooms[0].Uninitialize();
			rooms.RemoveAt(0);
		}
	}
	/** <summary> Adds the room to the game. </summary> */
	public void AddRoom(Room room) {
		rooms.Add(room);
		room.Initialize(this);
	}
	/** <summary> Removes the room from the game. </summary> */
	public void RemoveRoom(Room room) {
		for (int i = 0; i < rooms.Count; i++) {
			if (rooms[i] == room) {
				rooms[i].Uninitialize();
				rooms.RemoveAt(i);
				return;
			}
		}
	}
	/** <summary> Removes the rooms with the given ID from the game. </summary> */
	public void RemoveRoom(string id) {
		for (int i = 0; i < rooms.Count; i++) {
			if (rooms[i].ID == id) {
				rooms[i].Uninitialize();
				rooms.RemoveAt(i);
			}
		}
	}
	/** <summary> Gets the room with the given ID. </summary> */
	public Room GetRoom(string id) {
		for (int i = 0; i < rooms.Count; i++) {
			if (rooms[i].ID == id)
				return rooms[i];
		}
		return null;
	}
	/** <summary> Gets the list of entities with the given ID. </summary> */
	public Room[] GetRooms(string id) {
		List<Room> roomList = new List<Room>();
		for (int i = 0; i < rooms.Count; i++) {
			if (rooms[i].ID == id)
				roomList.Add(rooms[i]);
		}
		return roomList.ToArray();
	}
	/** <summary> Returns true if the specified room exists in the game. </summary> */
	public bool RoomExists(Room room) {
		for (int i = 0; i < rooms.Count; i++) {
			if (rooms[i] == room)
				return true;
		}
		return false;
	}
	/** <summary> Returns true if an room with the given ID exists. </summary> */
	public bool RoomExists(string id) {
		for (int i = 0; i < rooms.Count; i++) {
			if (rooms[i].ID == id)
				return true;
		}
		return false;
	}

	#endregion
}
} // End namespace
