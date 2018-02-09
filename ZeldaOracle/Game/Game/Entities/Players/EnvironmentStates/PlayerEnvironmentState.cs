using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Players.States {
	
	public class PlayerEnvironmentState : PlayerState {

		private PlayerMotionType motionSettings;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		public PlayerEnvironmentState() {
			motionSettings = new PlayerMotionType();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public PlayerMotionType MotionSettings {
			get { return motionSettings; }
			set { motionSettings = value; }
		}
	}
}
