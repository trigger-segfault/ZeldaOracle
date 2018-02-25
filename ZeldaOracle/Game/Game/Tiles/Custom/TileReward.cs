using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.GameStates.RoomStates;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Tiles.Custom {
	public class TileReward : Tile, ZeldaAPI.Reward {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileReward() { }
		

		//-----------------------------------------------------------------------------
		// Reward Methods
		//-----------------------------------------------------------------------------

		public void SpawnReward() {
			if (!IsLooted) {
				string rewardName = Properties.GetString("reward", "rupees_1");
				Reward reward = RoomControl.GameControl.RewardManager.GetReward(rewardName);
				
				CollectibleReward collectible = new CollectibleReward(reward);
				collectible.Collected += delegate() {
					Properties.Set("looted", true);
					RoomControl.RemoveTile(this);
					IsEnabled = false;
				};

				// Spawn the reward collectible.
				RoomControl.SpawnEntity(collectible);
				collectible.SetPositionByCenter(Center);
				
				if (Properties.GetBoolean("spawn_from_ceiling", false)) {
					collectible.ZPosition						= Center.Y + 8;
					collectible.Physics.HasGravity				= true;
					collectible.Physics.CollideWithWorld		= RoomControl.IsSideScrolling;
				}
				else {
					collectible.Physics.CollideWithWorld		= false;
					collectible.Physics.HasGravity				= false;
					collectible.Physics.IsDestroyedInHoles		= false;
					collectible.Physics.IsDestroyedOutsideRoom	= false;
				}
			}
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			base.OnInitialize();

			SpawnReward();
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			Reward reward = args.RewardManager.GetReward(args.Properties.GetString("reward"));
			if (reward != null) {
				g.DrawSprite(
					reward.Sprite,
					args.SpriteDrawSettings,
					args.Position,
					args.Color);
			}
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
			get { return Properties.GetBoolean("looted", false); }
		}
	}
}
