using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.GameStates.RoomStates;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Tiles.Custom {
	public class TileChest : Tile {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileChest() { }


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		// Called when the player presses A on this tile, when facing the given direction.
		public override bool OnAction(int direction) {
			if (!IsLooted) {

				if (direction == Directions.Up) {
					string rewardName = Properties.GetString("reward", "rupees_1");
					Reward reward = RoomControl.GameControl.RewardManager.GetReward(rewardName);
					RoomControl.GameControl.PushRoomState(
						new RoomStateReward(reward, (Point2I)Position));
					
					IsLooted = true;
					IsEnabled = true; // Opened chest will always spawn

					AudioSystem.PlaySound(GameData.SOUND_CHEST_OPEN);
					Graphics.PlayAnimation(SpriteList[1]);
				}
				else {
					RoomControl.GameControl.DisplayMessage(
						"It won't open from this side!");
				}
				return true;
			}
			return false;
		}

		public override void OnInitialize() {
			base.OnInitialize();
			
			Graphics.PlayAnimation(SpriteList[IsLooted ? 1 : 0]);
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			bool looted = args.Properties.GetBoolean("looted", false);
			Tile.DrawTileDataIndex(g, args, looted ? 1 : 0);
			if (args.Extras) {
				Reward reward = args.RewardManager.GetReward(args.Properties.GetString("reward"));
				if (reward != null) {
					g.DrawSprite(
						reward.Sprite,
						args.SpriteDrawSettings,
						args.Position,
						args.Color);
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsLooted {
			get { return Properties.GetBoolean("looted", false); }
			set { Properties.Set("looted", value); }
		}
	}
}
