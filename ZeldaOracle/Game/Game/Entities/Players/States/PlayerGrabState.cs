using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Players {
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

			if (grabTile != null && grabTile.Flags.HasFlag(TileFlags.Pickupable)) {
				player.BeginState(new PlayerCarryState(grabTile));
				player.RoomControl.RemoveTile(grabTile);
				return true;
			}

			return false;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			base.OnBegin();
			player.Movement.AllowMovementControl = false;

			timer = 0;
			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_GRAB);
			player.Graphics.AnimationPlayer.Pause();
		}
		
		public override void OnEnd() {
			player.Movement.AllowMovementControl = true;
			base.OnEnd();
		}

		public override void Update() {
			base.Update();

			InputControl grabButton = (equipSlot == 0 ? Controls.A : Controls.B);
			InputControl pullButton = Controls.Arrows[(player.Direction + 2) % 4];

			if (!grabButton.IsDown()) {
				player.BeginNormalState();
			}
			else if (pullButton.IsDown()) {
				player.Graphics.AnimationPlayer.PlaybackTime = GameData.ANIM_PLAYER_PULL.Duration;
				//player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_PULL);
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_PULL);
				timer++;
				if (timer > 10) {
					AttemptPickup();
				}
			}
			else {
				timer = 0;
				player.Graphics.AnimationPlayer.PlaybackTime = 0;
				player.Graphics.AnimationPlayer.Stop();
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
