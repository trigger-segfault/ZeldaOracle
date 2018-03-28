using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Shaders;

namespace ZeldaOracle.Game.Control.VisualEffects {
	public class SubrosiaWarpVisualEffect : ShaderVisualEffect<SineShiftShader> {

		private int timer;

		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the subrosia warp visual effect.</summary>
		public SubrosiaWarpVisualEffect() : base(GameData.SHADER_SINE_SHIFT) {
			timer = 0;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Updates the visual effect's state.</summary>
		public override void Update() {
			Shader.Height = 32.1f;
			Shader.Offset += 16.5f;
			Shader.Offset %= Shader.Height;
			Shader.Background = TileColors.SubrosiaWarpBackground;
			Shader.Magnitude = GMath.Min(11f, timer / 4f);
			timer += 1;
		}
	}
}
