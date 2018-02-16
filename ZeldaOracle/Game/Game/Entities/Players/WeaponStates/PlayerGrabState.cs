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
		private ItemBracelet bracelet;
		private Tile tile;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerGrabState() {
			bracelet	= null;
			duration	= 10;
			tile		= null;
			
			StateParameters.ProhibitMovementControl	= true;
			StateParameters.ProhibitJumping			= true;
		}
		

		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		private bool AttemptPickup() {
			Tile grabTile = player.Physics.GetFacingSolidTile(player.Direction);
			
			if (grabTile != null) {
				int minLevel = grabTile.Properties.GetInteger("pickupable_bracelet_level", Item.Level1);
				Item item = player.Inventory.GetItem("item_bracelet");

				if (grabTile.HasFlag(TileFlags.Pickupable) && item.Level >= minLevel) {
					player.CarryState.SetCarryObject(grabTile);
					player.BeginWeaponState(player.CarryState);
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
			timer = 0;
			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_GRAB);
			player.Graphics.PauseAnimation();
			player.Movement.StopMotion();
			
			// Check if grabbing an instantly pickupable tile
			if (tile.HasFlag(TileFlags.Pickupable | TileFlags.InstantPickup)) {
				if (AttemptPickup())
					return;
			}
		}
		
		public override void OnEnd(PlayerState newState) {
		}
		
		public override void OnHurt(DamageInfo damage) {
			base.OnHurt(damage);
			End();
		}

		public override void Update() {
			InputControl grabButton = player.Inventory
				.GetSlotButton(bracelet.CurrentEquipSlot);
			InputControl pullButton = Controls.Arrows[
				Directions.Reverse(player.Direction)];

			if (!grabButton.IsDown() || tile.IsMoving ||
				!player.Physics.IsFacingSolidTile(tile, player.Direction))
			{
				End();
			}
			else if (pullButton.IsDown()) {
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_PULL);
				timer++;
				if (timer > duration)
					AttemptPickup();
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

		public ItemBracelet Bracelet {
			get { return bracelet; }
			set { bracelet = value; }
		}

		public Tile Tile {
			get { return tile; }
			set { tile = value; }
		}
	}
}
