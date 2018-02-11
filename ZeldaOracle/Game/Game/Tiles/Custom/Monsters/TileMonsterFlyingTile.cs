using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Monsters;

namespace ZeldaOracle.Game.Tiles.Custom.Monsters {
	public class TileMonsterFlyingTile : TileMonster {
		
		private int order;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileMonsterFlyingTile() {
		}


		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		public void Launch() {
			RoomControl.RemoveTile(this);
			MonsterFlyingTile flyingTile = new MonsterFlyingTile();
			flyingTile.Position = Center + new Vector2F(0, 6);
			RoomControl.SpawnEntity(flyingTile);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			order = Properties.GetInteger("order", 0);
		}

		public override void Update() {
			base.Update();

			if (RoomControl.CurrentRoomTicks == StartTime) {
				Launch();
			}
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			Tile.DrawTileData(g, args);
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int StartTime {
			get {
				return GameSettings.MONSTER_FLYING_TILE_START_OFFSET +
					GameSettings.MONSTER_FLYING_TILE_NEXT_OFFSET * order;
			}
		}
	}
}
