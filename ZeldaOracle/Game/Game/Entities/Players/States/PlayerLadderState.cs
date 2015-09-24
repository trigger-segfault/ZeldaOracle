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
	public class PlayerLadderState : PlayerState {


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerLadderState() {
			isNaturalState = true;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			base.OnBegin();
			player.CanJump = false;
			Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_DEFAULT);
		}
		
		public override void OnEnd() {
			player.CanJump = true;
			Player.Graphics.StopAnimation();
			base.OnEnd();
		}

		public override void Update() {
			base.Update();
			
			// Always face up when on a ladder.
			player.Direction = Directions.Up;

			// Update animations
			//if (player.Movement.IsMoving && !Player.Graphics.IsAnimationPlaying)
			//	Player.Graphics.PlayAnimation();
			//if (!player.Movement.IsMoving && Player.Graphics.IsAnimationPlaying)
			//	Player.Graphics.StopAnimation();
			
			// Update items.
			//Player.UpdateEquippedItems();
			// TODO: Handle holding sheild on ladder
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

	}
}
