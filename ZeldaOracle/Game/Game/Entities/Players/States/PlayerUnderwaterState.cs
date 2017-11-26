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
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Audio;

namespace ZeldaOracle.Game.Entities.Players.States {
	public class PlayerUnderwaterState : PlayerState {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerUnderwaterState() {
			isNaturalState = true;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Always allow state changes, because this is the "Normal" state for
		// underwater rooms.
		public override bool RequestStateChange(PlayerState newState) {
			return true;
		}

		public override void OnBegin(PlayerState previousState) {
			player.Movement.CanJump			= false;
			player.Movement.MoveSpeedScale	= 1.0f;
			player.Movement.AutoAccelerate	= false;
			player.MoveAnimation			= GameData.ANIM_PLAYER_MERMAID_SWIM;
			player.Graphics.PlayAnimation(player.MoveAnimation);
		}
		
		public override void OnEnd(PlayerState newState) {
			player.Movement.CanJump			= true;
			player.Movement.MoveSpeedScale	= 1.0f;
			player.Movement.AutoAccelerate	= false;
			player.Graphics.DepthLayer		= DepthLayer.PlayerAndNPCs;
			player.MoveAnimation			= GameData.ANIM_PLAYER_DEFAULT;
		}

		public override void Update() {

			// Slow down movement over time from strokes
			if (player.Movement.MoveSpeedScale > 1.0f)
				player.Movement.MoveSpeedScale -= 0.025f;
			
			// Stroking scales the movement speed.
			if (player.Movement.MoveSpeedScale <= 1.4f && Controls.A.IsPressed()) {
				AudioSystem.PlaySound(GameData.SOUND_PLAYER_SWIM);
				player.Movement.MoveSpeedScale = 2.0f;
			}

			// Auto accelerate during the beginning of a stroke.
			player.Movement.AutoAccelerate = IsStroking;

			// Press B to attempt to resurface.
			if (Controls.B.IsPressed()) {

			}

			base.Update();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		// This is the threshhold of movement speed scale to be considered stroking.
		public bool IsStroking {
			get { return (player.Movement.MoveSpeedScale > 1.3f); }
		}

	}
}
