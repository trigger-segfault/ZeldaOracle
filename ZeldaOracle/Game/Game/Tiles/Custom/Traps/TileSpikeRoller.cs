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
using ZeldaOracle.Game.Tiles.Custom.Monsters;

namespace ZeldaOracle.Game.Tiles {
	public class TileSpikeRoller : TileMonster {
		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileSpikeRoller() {
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			SpawnMonster();
		}

		/// <summary>Constructs the monster associated with the tile.</summary>
		public override Monster ConstructMonster() {
			return new MonsterSpikeRoller(this);
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			bool vertical = args.Properties.GetBoolean("vertical", false);
			int length = GMath.Max(1, args.Properties.GetPoint("size", Point2I.One)[!vertical]);
			for (int i = 0; i < length; i++) {
				int spriteIndex = 0;
				if (i == 0) {
					if (i + 1 < length)
						spriteIndex = 1;
				}
				else if (i + 1 < length)
					spriteIndex = 2;
				else
					spriteIndex = 3;
				ISprite sprite = args.Tile.GetSpriteIndex(spriteIndex);
				if (sprite != null) {
					g.DrawSprite(
						sprite,
						args.SpriteDrawSettings,
						args.Position + Point2I.FromBoolean(!vertical, i * GameSettings.TILE_SIZE),
						args.Color);
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Override Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the type of monster to spawn.</summary>
		public override Type MonsterType {
			get { return typeof(MonsterSpikeRoller); }
		}

		/// <summary>Gets the spawn position for the monster.</summary>
		public override Vector2F SpawnPosition {
			get { return Position; }
		}
	}
}
