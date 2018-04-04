using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.GameStates.RoomStates;
using ZeldaOracle.Game.Items.Drops;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Tiles.Custom {
	public class TileDigableReward : Tile {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileDigableReward() { }
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			base.OnInitialize();

			if (IsLooted) {
				RoomControl.RemoveTile(this);
			}
			else {
				string rewardName = Properties.Get<string>("reward", "rupees_1");
				Reward reward = RoomControl.GameControl.RewardManager.GetReward(rewardName);
				DropList = new DropList();
				if (reward != null)
					DropList.AddDrop(1, reward);
			}
		}

		public override bool OnDig(Direction direction) {
			if (IsMoving)
				return false;

			// Remove/dig the tile.
			RoomControl.RemoveTile(this);

			// Spawn the a "dug" tile in this tile's place.
			TileData data = Resources.Get<TileData>("dug");
			Tile dugTile = Tile.CreateTile(data);
			RoomControl.PlaceTile(dugTile, Location, Layer);

			// Spawn drops.
			Entity dropEntity = SpawnDrop();
			if (dropEntity != null) {
				if (dropEntity is CollectibleReward) {
					var collectibleReward = dropEntity as CollectibleReward;
					collectibleReward.HasDuration = false;
					collectibleReward.Collected += delegate () {
						Properties.Set("looted", true);
						IsEnabled = false;
					};
					collectibleReward.CollectibleDelay = GameSettings.COLLECTIBLE_DIG_PICKUPABLE_DELAY;
				}
				dropEntity.Physics.Velocity = Directions.ToVector(direction) * GameSettings.DROP_ENTITY_DIG_VELOCITY;
			}

			return true;
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			Reward reward = args.RewardManager.GetReward(args.Properties.Get<string>("reward"));
			if (reward != null) {
				g.DrawSprite(
					reward.Sprite,
					args.SpriteSettings,
					args.Position,
					args.Color);
				g.DrawSprite(
					GameData.SPR_TILE_DIGABLE_REWARD_DIRT,
					args.SpriteSettings,
					args.Position,
					args.Color);
			}
		}

		/// <summary>Initializes the properties and events for the tile type.</summary>
		public static void InitializeTileData(TileData data) {
			data.Flags |= TileFlags.Digable;
			data.ResetCondition = TileResetCondition.Never;
			data.IsShared = true;

			data.Properties.Set("reward", "heart_piece")
				.SetDocumentation("Reward", "reward", "", "Reward", "The reward to spawn.");
			data.Properties.Set("looted", false)
				.SetDocumentation("Looted", "Reward", "True if the item has been taken.");

			data.EntityType = typeof(CollectibleReward);
		}


		//-----------------------------------------------------------------------------
		// Override Properties
		//-----------------------------------------------------------------------------

		public override bool IsStatic {
			get { return false; }
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsLooted {
			get { return Properties.Get<bool>("looted", false); }
		}
	}
}
