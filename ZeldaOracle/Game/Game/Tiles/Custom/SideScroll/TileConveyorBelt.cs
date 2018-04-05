using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Entities;

namespace ZeldaOracle.Game.Tiles.Custom.SideScroll {
	public class TileConveyorBelt : Tile {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileConveyorBelt() {

		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			Size = new Point2I(Width, 1);
			CollisionModel = new CollisionModel(
				new Rectangle2I(
					0, 0,
					Width * GameSettings.TILE_SIZE,
					GameSettings.TILE_SIZE));
		}

		public override void Update() {
			base.Update();
		}

		public override void Draw(RoomGraphics g) {
			for (int x = 0; x < Width; x++) {
				ISprite sprite = SpriteList[0];
				if (x == 0) {
					if (x + 1 < Width)
						sprite = SpriteList[1];
				}
				else if (x + 1 < Width)
					sprite = SpriteList[2];
				else
					sprite = SpriteList[3];
				g.DrawSprite(sprite, new SpriteSettings(RoomControl.CurrentRoomTicks),
					Position + new Point2I(x, 0) * GameSettings.TILE_SIZE, DepthLayer.TileLayer1 + Layer);
			}
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			int width = GMath.Max(1, args.Properties.Get<Point2I>("size", Point2I.One).X);
			for (int x = 0; x < width; x++) {
				int spriteIndex = 0;
				if (x == 0) {
					if (x + 1 < width)
						spriteIndex = 1;
				}
				else if (x + 1 < width)
					spriteIndex = 2;
				else
					spriteIndex = 3;
				ISprite sprite = args.Tile.GetSpriteIndex(spriteIndex);
				if (sprite != null) {
					g.DrawSprite(
						sprite,
						args.SpriteSettings,
						args.Position + new Point2I(x * GameSettings.TILE_SIZE, 0),
						args.Color);
				}
			}
		}

		/// <summary>Initializes the properties and events for the tile type.</summary>
		public static void InitializeTileData(TileData data) {
			data.Flags |= TileFlags.NoClingOnStab;
			data.Properties.Set("vertical", false)
				.SetDocumentation("Vertical", "Roller", "The roller rolls vertically.").Hide();
			data.Properties
				.SetDocumentation("size", "Length", "single_axis", "false:1", "Conveyor Belt", "The length of the conveyor belt in tiles.");
		}
	}
}
