using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Projectiles.Seeds;

namespace ZeldaOracle.Game.Tiles {

	public class TileSeedBouncer : Tile, ZeldaAPI.SeedBouncer {

		// Seed bouncers can be in 8 possible angles, but really only 4
		// of them are unique (because each angle has an equal opposite angle).
		private Angle angle;
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileSeedBouncer() {
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			angle = Properties.Get<int>("angle", 0);

			Graphics.PlayAnimation(GameData.ANIM_TILE_SEED_BOUNCER);
			Graphics.SubStripIndex = angle;

			fallsInHoles = false;
		}


		//-----------------------------------------------------------------------------
		// Zelda API methods
		//-----------------------------------------------------------------------------

		public void RotateClockwise(int amount = 1) {
			Angle = Angle.Rotate(amount, WindingOrder.Clockwise);
		}
		
		public void RotateCounterClockwise(int amount = 1) {
			Angle = Angle.Rotate(amount, WindingOrder.CounterClockwise);
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			int angle = args.Properties.Get<int>("angle", 0);
			g.DrawSprite(
				GameData.ANIM_TILE_SEED_BOUNCER.GetSubstrip(angle),
				args.SpriteSettings,
				args.Position,
				args.Color);
		}

		/// <summary>Initializes the properties and events for the tile type.</summary>
		public static void InitializeTileData(TileData data) {
			data.Flags |= TileFlags.NotSurface | TileFlags.NoClingOnStab;
			
			data.Properties.Set("angle", Angle.Right)
				.SetDocumentation("Angle", "angle", "", "Seed Bouncer", "The angle the seed bouncer is facing.");
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Angle Angle {
			get { return angle; }
			set {
				angle = value;
				Graphics.SubStripIndex = angle;
			}
		}
	}
}
