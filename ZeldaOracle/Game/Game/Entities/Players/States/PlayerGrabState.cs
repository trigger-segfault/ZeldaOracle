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
	public class PlayerGrabState : PlayerState {
		private int timer;
		private int duration;
		private int equipSlot;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerGrabState() {
			equipSlot = 0;
		}
		

		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		private bool AttemptPickup() {
			Tile grabTile = player.Physics.GetMeetingSolidTile(player.Position, player.Direction);
			
			if (grabTile != null) {
				int minLevel = grabTile.Properties.GetInteger("pickupable_bracelet_level", Item.Level1);
				Item item = player.Inventory.GetItem("item_bracelet");

				if (grabTile.HasFlag(TileFlags.Pickupable) && item.Level >= minLevel) {
					player.CarryState.SetCarryObject(grabTile);
					player.BeginState(player.CarryState);
					grabTile.SpawnDrop();
					player.RoomControl.RemoveTile(grabTile);
					return true;
				}
			}

			return false;
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
		}
		
		public override void OnEnd(PlayerState newState) {
			player.Movement.CanJump = true;
			player.Movement.MoveCondition = PlayerMoveCondition.FreeMovement;
		}

		public override void Update() {
			base.Update();

			InputControl grabButton = player.Inventory.GetSlotButton(equipSlot);
			InputControl pullButton = Controls.Arrows[(player.Direction + 2) % 4];

			if (!grabButton.IsDown()) {
				player.BeginNormalState();
			}
			else if (pullButton.IsDown()) {
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_PULL);
				timer++;
				if (timer > 10) {
					AttemptPickup();
				}
			}
			else {
				timer = 0;
				player.Graphics.StopAnimation();
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_GRAB);
			}
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int BraceletEquipSlot {
			get { return equipSlot; }
			set { equipSlot = value; }
		}
	}
}
