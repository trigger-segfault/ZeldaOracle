using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Entities.Players.States {
	public class PlayerSpinSwordState : PlayerState {

		private Animation weaponAnimation;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerSpinSwordState() {
			this.weaponAnimation	= null;
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			base.OnBegin();
			player.Movement.AllowMovementControl = false;
			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_SPIN);
			player.toolAnimation.Animation = weaponAnimation;
			player.toolAnimation.SubStripIndex = player.Direction;
			player.toolAnimation.Play();
		}
		
		public override void OnEnd() {
			player.toolAnimation.Animation = null;
			player.Movement.AllowMovementControl = true;
			base.OnEnd();
		}

		public override void Update() {
			base.Update();
			if (player.Graphics.IsAnimationDone)
				player.BeginNormalState();
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Animation WeaponAnimation {
			get { return weaponAnimation; }
			set { weaponAnimation = value; }
		}
	}
}
