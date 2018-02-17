using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerGrabState : PlayerState {

		private int pullTimer;
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
		// Internal Methods
		//-----------------------------------------------------------------------------
		
		/// <summary>Get the tile directly in front of the player.</summary>
		public Tile GetGrabTile() {
			Collision collision = Player.Physics
				.GetCenteredPotentialCollisionInDirection(Player.Direction);
			if (collision != null && collision.IsTile && !collision.Tile.IsMoving)
				return collision.Tile;
			return null;
		}

		/// <summary>Attempt to pickup a tile.</summary>
		private bool AttemptPickup(Tile tile) {
			int minLevel = tile.Properties.GetInteger(
				"pickupable_bracelet_level", Item.Level1);
			Item item = player.Inventory.GetItem("item_bracelet");

			if (tile.HasFlag(TileFlags.Pickupable) && item.Level >= minLevel) {
				player.CarryState.SetCarryObject(tile);
				player.BeginWeaponState(player.CarryState);
				tile.SpawnDrop();
				player.RoomControl.RemoveTile(tile);
				return true;
			}

			return false;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_GRAB);
			player.Graphics.PauseAnimation();
			player.Movement.StopMotion();

			pullTimer = 0;
			
			// Attempt to pickup instantly pickupable tiles
			if (tile.HasFlag(TileFlags.Pickupable | TileFlags.InstantPickup))
				AttemptPickup(tile);
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

			if (GetGrabTile() != tile || !grabButton.IsDown()) {
				End();
			}
			else if (pullButton.IsDown()) {
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_PULL);
				pullTimer++;
				if (pullTimer > duration)
					AttemptPickup(tile);
			}
			else {
				pullTimer = 0;
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
