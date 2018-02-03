using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Tiles {

	public class TilePlatform : Tile {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TilePlatform() {
			fallsInHoles = false;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void OnInitialize() {
			/*
			path = new TilePath();
			path.Repeats = true;

			path.AddMove(Directions.Down,	1, 0.5f, 0);
			path.AddMove(Directions.Up,		4, 0.5f, 0);
			path.AddMove(Directions.Down,	3, 0.5f, 0);
			*/
			
		}

		public override void Update() {
			base.Update();
		}

		public override void Draw(RoomGraphics g) {
			//base.Draw(g);

			for (int y = 0; y < Height; y++) {
				for (int x = 0; x < Width; x++) {
					g.DrawSprite(Graphics.AnimationPlayer.SpriteOrSubStrip,
						Position + new Point2I(x, y) * GameSettings.TILE_SIZE, DepthLayer.TileLayer1);
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			Point2I size = args.Properties.GetPoint("size", Point2I.One);
			for (int x = 0; x < size.X; x++) {
				for (int y = 0; y < size.Y; y++) {
					Tile.DrawTileDataWithOffset(g, args, new Point2I(x, y) * GameSettings.TILE_SIZE);
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden Propreties
		//-----------------------------------------------------------------------------

		public override bool IsSurface {
			get { return false; }
		}
	}
}
