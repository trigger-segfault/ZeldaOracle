using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Audio;

namespace ZeldaOracle.Game.Entities.Players.States {
	public class PlayerSwingState : PlayerState {

		private const int SWING_PEAK_DELAY = 6;

		private PlayerState nextState;
		private Animation weaponAnimation;
		private int equipSlot;
		private bool hasPeaked;

		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerSwingState() {
			this.weaponAnimation	= null;
			this.nextState			= null;
			this.equipSlot			= 0;
			this.hasPeaked			= false;
		}
		
		
		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------

		public virtual void OnSwingPeak() {
			// TODO: Create a sword subclass for this and override this method?
			if (player.IsInAir)
				return;
			Vector2F hitPoint = player.Center + (Directions.ToVector(player.Direction) * 16);
			Point2I location = player.RoomControl.GetTileLocation(hitPoint);
			if (player.RoomControl.IsTileInBounds(location)) {
				Tile tile = player.RoomControl.GetTopTile(location);
				if (tile != null)
					tile.OnSwordHit();
			}
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			player.Direction = player.UseDirection;
			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_SWING);
			player.Movement.MoveCondition = PlayerMoveCondition.OnlyInAir;
			player.toolAnimation.Animation = weaponAnimation;
			player.toolAnimation.SubStripIndex = player.Direction;
			player.toolAnimation.Play();
			
			hasPeaked = false;

			AudioSystem.PlayRandomSound("Items/slash_1", "Items/slash_2", "Items/slash_3");
		}
		
		public override void OnEnd(PlayerState newState) {
			player.Movement.MoveCondition = PlayerMoveCondition.FreeMovement;
			player.toolAnimation.Animation = null;
		}

		public override void Update() {
			base.Update();

			// Check for the swing peak.
			if (player.toolAnimation.PlaybackTime == SWING_PEAK_DELAY)
				OnSwingPeak();

			// Reset the swing
			if (player.GameControl.Inventory.GetSlotButton(equipSlot).IsPressed()) {
				player.Direction = player.UseDirection;
				player.toolAnimation.SubStripIndex = player.Direction;
				player.Graphics.PlayAnimation();
				player.toolAnimation.Play();
				AudioSystem.PlayRandomSound("Items/slash_1", "Items/slash_2", "Items/slash_3");
			}

			// End the swing
			if (player.Graphics.IsAnimationDone)
				player.BeginState(nextState);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public PlayerState NextState {
			get { return nextState; }
			set { nextState = value; }
		}

		public Animation WeaponAnimation {
			get { return weaponAnimation; }
			set { weaponAnimation = value; }
		}

		public int EquipSlot {
			get { return equipSlot; }
			set { equipSlot = value; }
		}
	}
}
