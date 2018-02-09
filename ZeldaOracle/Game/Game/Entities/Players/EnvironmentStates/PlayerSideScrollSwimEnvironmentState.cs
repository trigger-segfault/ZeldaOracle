﻿using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Effects;

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerSideScrollSwimEnvironmentState : PlayerJumpEnvironmentState {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerSideScrollSwimEnvironmentState() {
			StateParameters.DisableGravity			= true;
			StateParameters.EnableGroundOverride	= true;
			StateParameters.AlwaysFaceLeftOrRight	= true;
			StateParameters.ProhibitJumping			= true;

			MotionSettings = new PlayerMotionType() {
				MovementSpeed			= 0.5f,
				IsSlippery				= true,
				Acceleration			= 0.08f,
				Deceleration			= 0.05f,
				MinSpeed				= 0.05f,
				DirectionSnapCount		= 32,
			};
		}


		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		private void CreateSplashEffect() {
			Effect splash = new Effect(GameData.ANIM_EFFECT_WATER_SPLASH,
				DepthLayer.EffectSplash, true);
			splash.Position = player.Center + new Vector2F(0, 4);
			player.RoomControl.SpawnEntity(splash);
			AudioSystem.PlaySound(GameData.SOUND_PLAYER_WADE);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			//player.Movement.MoveSpeedScale	= 1.0f;
			player.Movement.AutoAccelerate	= false;
			player.MoveAnimation			= GameData.ANIM_PLAYER_MERMAID_SWIM;
			player.Graphics.PlayAnimation(player.MoveAnimation);

			CreateSplashEffect();
		}
		
		public override void OnEnd(PlayerState newState) {
			//player.Movement.MoveSpeedScale	= 1.0f;
			player.Movement.AutoAccelerate	= false;
			player.Graphics.DepthLayer		= DepthLayer.PlayerAndNPCs;
			player.MoveAnimation			= GameData.ANIM_PLAYER_DEFAULT;
			
			// Jump out of the water, and create a splash effect
			if (newState != player.SideScrollLadderState) {
				player.Physics.ZVelocity = 1.5f;
				CreateSplashEffect();
			}
		}

		public override void Update() {

			// TODO: Code duplication with PlayerSwimState
			
			// Slow down movement over time from strokes
			//if (player.Movement.MoveSpeedScale > 1.0f)
				//player.Movement.MoveSpeedScale -= 0.025f;

			// Stroking scales the movement speed.
			// Press A to stroke, but this will not work if an item is usable
			// in slot A.
			//if (player.Movement.MoveSpeedScale <= 1.4f &&
			//	Controls.A.IsPressed() && 
			//	(player.EquippedUsableItems[Inventory.SLOT_A] == null || 
			//	!player.EquippedUsableItems[Inventory.SLOT_A].IsUsable()))
			//{
			//	AudioSystem.PlaySound(GameData.SOUND_PLAYER_SWIM);
			//	player.Movement.MoveSpeedScale = 2.0f;
			//}

			// Auto accelerate during the beginning of a stroke.
			//player.Movement.AutoAccelerate = IsStroking;

			base.Update();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		// This is the threshhold of movement speed scale to be considered stroking.
		//public bool IsStroking {
			//get { return (player.Movement.MoveSpeedScale > 1.3f); }
		//}

	}
}