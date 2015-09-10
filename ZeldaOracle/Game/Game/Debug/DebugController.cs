using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using XnaColor		= Microsoft.Xna.Framework.Color;
using XnaKeyboard	= Microsoft.Xna.Framework.Input.Keyboard;
using XnaKeys		= Microsoft.Xna.Framework.Input.Keys;

using ZeldaOracle.Common.Debug;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Common.Input.Controls;
using Color			= ZeldaOracle.Common.Graphics.Color;
using Keyboard		= ZeldaOracle.Common.Input.Keyboard;
using Keys			= ZeldaOracle.Common.Input.Keys;

using GameFramework.MyGame.Main;
//using ZeldaOracle.Common.Graphics.Particles;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Scripts;
using System.IO;
//using ParticleGame.Project.Particles;

namespace GameFramework.MyGame.Debug {
/** <summary>
 * The controller for game debugging.
 * </summary> */
public class DebugController : DebugControllerBase {

	//=========== MEMBERS ============
	#region Members

	/** <summary> The game manager. </summary> */
	private GameManager game;
	/** <summary> True if the frame rate is displayed. </summary> */
	private bool showFPS;
	/** <summary> True if particle counts should be shown. </summary> */
	private bool showParticleCount;
	private bool showControls;
	public double timeScale;

	public bool nextStep;

	public bool showParticlePos;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the game debugger. </summary> */
	public DebugController(GameManager game) : base() {

		this.game = game;

		this.timeScale = 1.0;
		this.showParticlePos = true;
		this.showControls = true;
		this.nextStep		= false;

		
		/*RadioButtonGroup drawModeGroup = new RadioButtonGroup();
		RadioButtonGroup groupGameSpeed = new RadioButtonGroup();

		// GAME.
		DebugMenuItem menuGame = menu.Root.AddItem("Game");
		DebugMenuItem menuGameSpeed = menuGame.AddItem("Time control");
		menuGameSpeed.AddItem(new ToggleMenuItem("Paused", new HotKey(Keys.F9), false, null,
			delegate() { gamePaused = true; },
			delegate() { gamePaused = false; }));
		menuGameSpeed.AddItem(new DebugMenuItem("Next Step", new HotKey(Keys.F8),
			delegate() { nextStep = true; }));
		menuGameSpeed.AddItem(new RadioButtonMenuItem("25%", null, groupGameSpeed, false, null,
			delegate() { timeScale = 0.25; }, null));
		menuGameSpeed.AddItem(new RadioButtonMenuItem("50%", null, groupGameSpeed, false, null,
			delegate() { timeScale = 0.50; }, null));
		menuGameSpeed.AddItem(new RadioButtonMenuItem("100%", null, groupGameSpeed, true, null,
			delegate() { timeScale = 1.0; }, null));
		menuGameSpeed.AddItem(new RadioButtonMenuItem("200%", null, groupGameSpeed, false, null,
			delegate() { timeScale = 2.0; }, null));
		menuGameSpeed.AddItem(new RadioButtonMenuItem("400%", null, groupGameSpeed, false, null,
			delegate() { timeScale = 4.0; }, null));
		menuGame.AddItem("Save Particles", new HotKey(Keys.S, true),
			delegate() {
				Resources.SaveScript("Particles/particle_data.conscript", new ParticleSW(), Encoding.Default);
			});
		menuGame.AddItem("Save As Particles", new HotKey(Keys.S, true, true),
			delegate() {
				int index = 1;
				while (File.Exists("Particles/particle_data_" + index.ToString() + ".conscript")) {
					index++;
				}
				Resources.SaveScript("Particles/particle_data_" + index.ToString() + ".conscript", new ParticleSW(), Encoding.Default);
			});
		menuGame.AddItem("Quit", new HotKey(Keys.W, true), delegate() { game.Exit(); });*/

		// PLAYER.
		/*DebugMenuItem menuPlayer = menu.Root.AddItem("Player");
		menuPlayer.AddItem(new ToggleMenuItem("God mode", new HotKey(Keys.G), false, null,
			null, null));
		menuPlayer.AddItem("Toggle shield", new HotKey(Keys.Y), null);
		menuPlayer.AddItem("Cycle power", new HotKey(Keys.U), null);
		menuPlayer.AddItem("Add projectile option", new HotKey(Keys.I), null);
		menuPlayer.AddItem("Clear projectile options", new HotKey(Keys.I, true), null);

		// ENTITY.
		DebugMenuItem menuEntity = menu.Root.AddItem("Entity");
		DebugMenuItem menuSpawn1 = menuEntity.AddItem("Spawn tier 1 enemy");
		menuSpawn1.AddItem("Basic Shooter", new HotKey(Keys.D1), null);
		menuSpawn1.AddItem("Side Stepper", new HotKey(Keys.D2), null);
		menuSpawn1.AddItem("Powerup", new HotKey(Keys.D3), null);
		menuSpawn1.AddItem("Jack", new HotKey(Keys.D4), null);
		menuSpawn1.AddItem("Spinner", new HotKey(Keys.D5), null);
		menuSpawn1.AddItem("Seeker", new HotKey(Keys.D6), null);
		menuSpawn1.AddItem("Laser", new HotKey(Keys.D7), null);
		menuSpawn1.AddItem("Miniboss 1", new HotKey(Keys.D9), null);
		menuSpawn1.AddItem("Boss 1", new HotKey(Keys.D0), null);

		DebugMenuItem menuSpawn2 = menuEntity.AddItem("Spawn tier 2 enemy");
		menuSpawn2.AddItem("Burst Shot", new HotKey(Keys.D1, false, true), null);
		menuSpawn2.AddItem("Popper", new HotKey(Keys.D2, false, true), null);
		menuSpawn2.AddItem("Stinger", new HotKey(Keys.D3, false, true), null);
		menuSpawn2.AddItem("Arc Shooter", new HotKey(Keys.D4, false, true), null);
		menuSpawn2.AddItem("Splitter", new HotKey(Keys.D5, false, true), null);
		menuSpawn2.AddItem("Pather", new HotKey(Keys.D6, false, true), null);
		menuSpawn2.AddItem("Miniboss 2", new HotKey(Keys.D8, false, true), null);
		menuSpawn2.AddItem("Miniboss 3", new HotKey(Keys.D9, false, true), null);
		menuSpawn2.AddItem("Boss 2", new HotKey(Keys.D0, false, true), null);

		DebugMenuItem menuSpawn3 = menuEntity.AddItem("Spawn tier 3 enemy");
		menuSpawn3.AddItem("Drone", new HotKey(Keys.D1, true), null);
		menuSpawn3.AddItem("Energy Beamer", new HotKey(Keys.D2, true), null);
		menuSpawn3.AddItem("Acid Spitter", new HotKey(Keys.D3, true), null);
		menuSpawn3.AddItem("Flamer", new HotKey(Keys.D4, true), null);
		menuSpawn3.AddItem("Wave Beamer", new HotKey(Keys.D5, true), null);
		menuSpawn3.AddItem("Laser Trap", new HotKey(Keys.D6, true), null);
		menuSpawn3.AddItem("Miniboss 4", new HotKey(Keys.D8, true), null);
		menuSpawn3.AddItem("Boss 3", new HotKey(Keys.D9, true), null);
		menuSpawn3.AddItem("Boss 4", new HotKey(Keys.D0, true), null);

		menuEntity.AddItem("Kill all enemies", new HotKey(Keys.Back), null);
		menuEntity.AddItem("Delete all entities", new HotKey(Keys.Back, true), null);
		menuEntity.AddItem("Create level text effect", null, null);
		menuEntity.AddItem("Create boss warning text effect", null, null);

		// LEVEL.
		DebugMenuItem menuLevel = menu.Root.AddItem("Level");
		menuLevel.AddItem("Restart level sequence", new HotKey(Keys.Home), null);
		menuLevel.AddItem("End level sequence", new HotKey(Keys.End), null);
		menuLevel.AddItem("Next level", new HotKey(Keys.PageUp), null);
		menuLevel.AddItem("Previous level", new HotKey(Keys.PageDown), null);

		// SHADERS.
		DebugMenuItem menuShaders = menu.Root.AddItem("Shaders");
		menuShaders.AddItem(new ToggleMenuItem("Screen fade", new HotKey(Keys.NumPad9), true, null,
			null, null));
		menuShaders.AddItem(new ToggleMenuItem("Bloom", new HotKey(Keys.NumPad6), true, null,
			null, null));
		menuShaders.AddItem(new ToggleMenuItem("Refraction Tester", new HotKey(Keys.Insert), false, null,
			null, null));*/

		// VIEW.
		DebugMenuItem menuView = menu.Root.AddItem("View");
		menuView.AddItem(new ToggleMenuItem("Fullscreen", new HotKey(Keys.F11), false, null,
			delegate() { game.IsFullScreen = true; },
			delegate() { game.IsFullScreen = false; }));
		menuView.AddItem(new ToggleMenuItem("Show FPS", new HotKey(Keys.F3), false, null,
			delegate() { showFPS = true; },
			delegate() { showFPS = false; }));
		menuView.AddItem(new ToggleMenuItem("Show Particle Count", null, false, null,
			delegate() { showParticleCount = true; },
			delegate() { showParticleCount = false; }));
		menuView.AddItem(new ToggleMenuItem("Show Particle Cursor", null, true, null,
			delegate() { showParticlePos = true; },
			delegate() { showParticlePos = false; }));
		menuView.AddItem(new ToggleMenuItem("Show Controls", null, true, null,
			delegate() { showControls = true; },
			delegate() { showControls = false; }));
		/*menuView.AddItem(new ToggleMenuItem("Show debug info", new HotKey(Keys.F1), false, null,
			null, null));
		menuView.AddItem(new ToggleMenuItem("Background", new HotKey(Keys.NumPad4), true, null,
			null, null));
		menuView.AddItem(new ToggleMenuItem("Particles", new HotKey(Keys.NumPad5), true, null,
			null, null));
		DebugMenuItem menuDrawMode = menuView.AddItem("Color Mode");
		menuDrawMode.AddItem(new RadioButtonMenuItem("None", new HotKey(Keys.NumPad0), drawModeGroup, false, null,
			null, null));
		menuDrawMode.AddItem(new RadioButtonMenuItem("Diffuse", new HotKey(Keys.NumPad1), drawModeGroup, true, null,
			null, null));
		menuDrawMode.AddItem(new RadioButtonMenuItem("Masks", new HotKey(Keys.NumPad2), drawModeGroup, false, null,
			null, null));
		menuDrawMode.AddItem(new RadioButtonMenuItem("Brightness", new HotKey(Keys.NumPad3), drawModeGroup, false, null,
			null, null));*/
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties

	/** <summary> Gets or sets if the frame rate is displayed. </summary> */
	public bool ShowFPS {
		get { return showFPS; }
		set { showFPS = value; }
	}

	#endregion
	//=========== UPDATING ===========
	#region Updating

	/** <summary> Called every step to update the debug controller. </summary> */
	public override void Update() {
		base.Update();
	}

	#endregion
	//=========== DRAWING ============
	#region Drawing

	/** <summary> Called every step to draw debug information. </summary> */
	public override void Draw(Graphics2D g) {
		DrawSideInfo(g);

		base.Draw(g);
	}

	private void DrawSideInfo(Graphics2D g) {

		List<string> items = new List<string>();

		/*if (gamePaused)
			items.Add("Paused");
		items.Add("Current Effect: " + game.effectType.Name);
		items.Add(game.grid.typeName + " Type name: " + game.grid.name);
		items.Add("Effect Position: " + game.effectPos.ToString());
		if (showFPS)
			items.Add("FPS: " + game.FPS.ToString("#.0"));
		if (showParticleCount) {
			items.Add("Particles: " + game.particleSystem.ParticleCount);
			items.Add("Effects: " + game.particleSystem.EffectCount);
		}
		if (showControls) {
			items.Add("");
			items.Add("[Controls]");
			items.Add("PageUp - Previous");
			items.Add("PageDown - Next");
			items.Add("End - Reset Effect");
			items.Add("RMB - Set Effect Location");
			items.Add("RMB2 - Edit Selected Type");
			items.Add("F2/MMB - Open Debug Menu");
		}*/

		/*if (items.Count > 0) {
			int itemWidth  = 460;
			int itemHeight = 20;
			int menuPadding = 4;
			int textOffset = 8;
			//Point2I position = new Point2I(game.ScreenSize.X - itemWidth, 0);
			Point2I position = new Point2I(0, 0);

			Color colorBackground			= new Color(40, 40, 40);
			Color colorBackgroundHighlight	= new Color(60, 60, 60);
			Color colorOutline				= new Color(70, 70, 70);
			Color colorText					= Color.White;

			Rectangle2I menuRect = new Rectangle2I(position.X, position.Y, itemWidth, itemHeight * items.Count + menuPadding * 2);
			//g.FillRectangle(menuRect, colorOutline);
			//g.DrawRectangle(menuRect, 1.0, colorOutline);

			//menuRect.Inflate(-1, -1);
			//g.FillRectangle(menuRect, colorBackground);

			for (int i = 0; i < items.Count; i++) {
				Rectangle2I r = new Rectangle2I(position.X, position.Y + i * itemHeight + menuPadding, itemWidth, itemHeight);
				if (items[i] == "Paused")
					g.DrawString(DebugMenuFontBold, items[i], new Point2I(r.Min.X + textOffset, (int)r.Center.Y), Align.Left | Align.Int, new Color(240, 20, 20));
					//g.DrawString(DebugMenuFontBold, items[i], new Point2I((int)r.Center.X, (int)r.Center.Y), Align.Center | Align.Int, new Color(240, 20, 20));
				else
					g.DrawString(DebugMenuFont, items[i], new Point2I(r.Min.X + textOffset, (int)r.Center.Y), Align.Left | Align.Int, colorText);
			}
		}*/
	}

	#endregion
}
} // end namespace
