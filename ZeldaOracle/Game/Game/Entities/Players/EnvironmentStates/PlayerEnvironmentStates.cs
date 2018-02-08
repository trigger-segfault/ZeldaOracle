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
		
		public PlayerEnvironmentState() {
			motionSettings = new PlayerMotionType();
		}

		public PlayerMotionType MotionSettings {
			get { return motionSettings; }
			set { motionSettings = value; }
		}
	}

	public class PlayerEnvironmentStateGrass : PlayerEnvironmentState {
		public PlayerEnvironmentStateGrass() {
			MotionSettings.MovementSpeed = 0.75f;
			StateParameters.ProhibitJumping = true;
		}
	}

	public class PlayerEnvironmentStateStairs : PlayerEnvironmentState {
		public PlayerEnvironmentStateStairs() {
			MotionSettings.MovementSpeed = 0.5f;
		}
	}

	public class PlayerEnvironmentStateJump : PlayerEnvironmentState {
		public PlayerEnvironmentStateJump() {
			MotionSettings.MovementSpeed		= 1.0f;
			MotionSettings.IsSlippery			= true;
			MotionSettings.Acceleration			= 0.10f;
			MotionSettings.Deceleration			= 0.00f;
			MotionSettings.MinSpeed				= 0.05f;
			MotionSettings.DirectionSnapCount	= 8;
		}

		public override void OnBegin(PlayerState previousState) {
			StateParameters.ProhibitLedgeJumping	= true;
			StateParameters.ProhibitRoomTransitions	= true;
			StateParameters.EnableStrafing			= true;
		}
	}

	public class PlayerEnvironmentStateIce : PlayerEnvironmentState {
		public PlayerEnvironmentStateIce() {
			MotionSettings.MovementSpeed		= 1.0f;
			MotionSettings.IsSlippery			= true;
			MotionSettings.Acceleration			= 0.02f;
			MotionSettings.Deceleration			= 0.05f;
			MotionSettings.MinSpeed				= 0.05f;
			MotionSettings.DirectionSnapCount	= 32;
		}
	}
}
