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

using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Common.Input.Controls;
using Color			= ZeldaOracle.Common.Graphics.Color;
using Keyboard		= ZeldaOracle.Common.Input.Keyboard;
using Keys			= ZeldaOracle.Common.Input.Keys;

using ZeldaOracle.Game.Main;
//using ZeldaOracle.Common.Graphics.Particles;
using ZeldaOracle.Common.Content;
using System.IO;
//using ParticleGame.Project.Particles;

namespace GameFramework.MyGame.Debug {
	/**<summary>The controller for game debugging.</summary>*/
	public class DebugController : DebugControllerBase {

		//=========== MEMBERS ============
		#region Members

		/**<summary>The game manager.</summary>*/
		private GameManager game;
		/**<summary>True if the frame rate is displayed.</summary>*/
		private bool showFPS;

		public bool nextStep;


		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		/**<summary>Constructs the game debugger.</summary>*/
		public DebugController(GameManager game) {

			this.game = game;

			this.nextStep		= false;

		
		}

		#endregion
		//========== PROPERTIES ==========
		#region Properties

		/**<summary>Gets or sets if the frame rate is displayed.</summary>*/
		public bool ShowFPS {
			get { return showFPS; }
			set { showFPS = value; }
		}

		#endregion
		//=========== UPDATING ===========
		#region Updating

		/**<summary>Called every step to update the debug controller.</summary>*/
		public override void Update() {
			base.Update();
		}

		#endregion
		//=========== DRAWING ============
		#region Drawing

		/**<summary>Called every step to draw debug information.</summary>*/
		public override void Draw(Graphics2D g) {
			DrawSideInfo(g);

			base.Draw(g);
		}

		private void DrawSideInfo(Graphics2D g) {

		}

		#endregion
	}
} // end namespace
