using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Projectiles;

namespace ZeldaOracle.Game.Tiles {

	public class TileColorCubeSlot : Tile, ZeldaAPI.ColorCubeSlot {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileColorCubeSlot() {

		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnCoverComplete(Tile tile) {
			if (tile is TileColorCube) {
				TileColorCube colorCube = (TileColorCube) tile;
				Color = ((TileColorCube) tile).TopColor;
			}
		}

		public override void OnUncoverComplete(Tile tile) {
			if (tile is TileColorCube) {
				Color = PuzzleColor.None;
			}
		}

		public override void OnInitialize() {
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public PuzzleColor Color {
			get { return (PuzzleColor) Properties.GetInteger("color", (int) PuzzleColor.None); }
			set {
				PuzzleColor prevColor = Color;
				Properties.Set("color", (int) value);
				if (prevColor != value) {
					GameControl.ExecuteScript(
						Properties.GetString("on_color_change", ""),
						this, (ZeldaAPI.Color) value);
				}
			}
		}
		

		//-----------------------------------------------------------------------------
		// Scripting API
		//-----------------------------------------------------------------------------

		ZeldaAPI.Color ZeldaAPI.ColorCubeSlot.Color {
			get { return (ZeldaAPI.Color) Color; }
			set { Color = (PuzzleColor) value; }
		}
	}
}
