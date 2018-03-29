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
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Closes and the chest and allows looting again.</summary>
		public void Close() {
			IsLooted = false;
			AudioSystem.PlaySound(GameData.SOUND_CHEST_OPEN);
			Graphics.PlayAnimation(SpriteList[0]);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Called when the player presses A on this tile, when facing the given direction.
		public override bool OnAction(Direction direction) {
			if (!IsLooted) {

				if (direction == Direction.Up) {
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
						args.SpriteSettings,
						args.Position,
						args.Color);
				}
			}
		}

		/// <summary>Initializes the properties and events for the tile type.</summary>
		public static void InitializeTileData(TileData data) {
			data.ResetCondition = TileResetCondition.Never;
			data.IsShared = true;

			data.Properties.Set("spawn_delay_after_poof", 16);
			data.Properties.Set("reward", "rupees_1")
				.SetDocumentation("Reward", "reward", "", "Chest", "The reward contained inside the chest.");
			data.Properties.Set("looted", false)
				.SetDocumentation("Looted", "Chest", "True if the item has been taken from the chest.");
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
