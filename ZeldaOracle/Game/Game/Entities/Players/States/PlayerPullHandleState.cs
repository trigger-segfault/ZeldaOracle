using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerPullHandleState : PlayerState {
		
		private ItemBracelet bracelet;
		private TilePullHandle tileHandle;
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
			player.Movement.CanJump = false;
			player.Movement.MoveCondition = PlayerMoveCondition.NoControl;

			timer = 0;
			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_GRAB);
			player.Graphics.PauseAnimation();
			
			player.Movement.StopMotion();
			AudioSystem.PlaySound(GameData.SOUND_PLAYER_PICKUP);
			
			player.Position = tileHandle.GetPlayerPullPosition();
		}
		
		public override void OnEnd(PlayerState newState) {
			player.Movement.CanJump = true;
			player.Movement.MoveCondition = PlayerMoveCondition.FreeMovement;
			tileHandle.EndPull();
		}

		public override void Update() {
			base.Update();

			InputControl grabButton = player.Inventory.GetSlotButton(bracelet.CurrentEquipSlot);
			InputControl pullButton = Controls.GetArrowControl(Directions.Reverse(player.Direction));
			
			timer--;

			if (!grabButton.IsDown()) {
				player.BeginNormalState();
			}
			else if (isPulling) {
				tileHandle.Extend(tileHandle.ExtendSpeed);

				if (tileHandle.IsFullyExtended) {
					// The handle has been fully extended.
					isPulling	= false;
					timer		= 0;
					player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_GRAB);
					AudioSystem.PlaySound(GameData.SOUND_CHEST_OPEN);
				}
				else if (timer <= 0) {
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
			else if (timer <= 0 && !tileHandle.IsFullyExtended && pullButton.IsDown()) {
				// Begin pulling the handle.
				timer		= pullDuration;
				isPulling	= true;
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_PULL);
				AudioSystem.PlaySound(GameData.SOUND_BLOCK_PUSH);
			}

			player.Position = tileHandle.GetPlayerPullPosition();
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public ItemBracelet Bracelet {
			get { return bracelet; }
			set { bracelet = value; }
		}

		public TilePullHandle PullHandleTile {
			get { return tileHandle; }
			set { tileHandle = value; }
		}
	}
}