using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.GameStates {
	public class TransitionEntityPalette : GameState {
		
		// The entity palette to switch to.
		private Palette palette;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public TransitionEntityPalette(Palette palette) {
			this.palette	= palette;
			if (palette.PaletteType != PaletteTypes.Entity)
				throw new ArgumentException("Cannot create TransitionEntityPalette with non-entity based palette!");
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			GameData.PaletteShader.EntityPalette = palette;
			End();
		}
	}
}
