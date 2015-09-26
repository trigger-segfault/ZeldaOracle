using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.GameStates.RoomStates;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Tiles.Custom {
	public class TileReward : Tile {

		private Reward reward;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileReward() {
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		// Called when the player presses A on this tile, when facing the given direction.
		public override bool OnAction(int direction) {
			RoomControl.GameControl.PushRoomState(new RoomStateReward(reward));
			RoomControl.RemoveTile(this);
			return true;
		}


		public override void Initialize() {
			base.Initialize();
			
			string rewardName = properties.GetString("reward", "rupees_1");
			reward = RoomControl.GameControl.RewardManager.GetReward(rewardName);
			AnimationPlayer.Animation = reward.Animation;
		}

		public override void Draw(Graphics2D g) {

			if (AnimationPlayer.SubStrip != null) {
				// Draw as an animation.
				g.DrawAnimation(AnimationPlayer.SubStrip, RoomControl.GameControl.RoomTicks, Position + GameSettings.TILE_SIZE / 2);
			}
			else {
				// Draw as a sprite.
				Sprite spr = Sprite;
				if (IsMoving && SpriteAsObject != null)
					spr = SpriteAsObject;
				g.DrawSprite(spr, Position);
			}
		}

	}
}
