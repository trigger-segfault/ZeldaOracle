using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Game.Entities.Objects;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerPullHandleState : PlayerState {
		
		private ItemBracelet bracelet;
		private PullHandle handle;
		private int pullDuration;
		private int puaseDuration;
		private bool isPulling;
		private int timer;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerPullHandleState() {
			bracelet		= null;
			pullDuration	= GameSettings.TILE_PULL_HANDLE_PLAYER_PULL_DURATION;
			puaseDuration	= GameSettings.TILE_PULL_HANDLE_PLAYER_PAUSE_DURATION;
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			StateParameters.ProhibitJumping			= true;
			StateParameters.ProhibitMovementControl	= true;

			timer = 0;
			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_GRAB);
			player.Graphics.PauseAnimation();
			
			player.Movement.StopMotion();
			AudioSystem.PlaySound(GameData.SOUND_PLAYER_PICKUP);
			
			player.SetPositionByCenter(handle.GetPlayerPullPosition());
		}
		
		public override void OnEnd(PlayerState newState) {
			handle.OnReleaseHandle();
		}

		public override void Update() {
			InputControl pullButton =
				Controls.GetArrowControl(player.Direction.Reverse());
			
			timer--;

			if (!bracelet.IsButtonDown()) {
				End();
			}
			else if (isPulling) {
				// Extend the handle.
				if (!handle.IsFullyExtended) {
					handle.Extend(handle.ExtendSpeed);
					if (handle.IsFullyExtended)
						AudioSystem.PlaySound(GameData.SOUND_CHEST_OPEN);
				}

				if (timer <= 0) {
					// Stop pulling to pause for after a short duration.
					isPulling	= false;
					timer		= puaseDuration;
					player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_GRAB);
				}
				else if (!pullButton.IsDown()) {
					// Stop pulling because the pull arrow-key was released.
					isPulling	= false;
					timer		= 0;
					player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_GRAB);
				}
			}
			else if (timer <= 0 && pullButton.IsDown()) {
				// Begin pulling the handle.
				timer		= pullDuration;
				isPulling	= true;
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_PULL);
				if (!handle.IsFullyExtended)
					AudioSystem.PlaySound(GameData.SOUND_BLOCK_PUSH);
			}

			player.SetPositionByCenter(handle.GetPlayerPullPosition());
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public ItemBracelet Bracelet {
			get { return bracelet; }
			set { bracelet = value; }
		}

		public PullHandle PullHandle {
			get { return handle; }
			set { handle = value; }
		}
	}
}