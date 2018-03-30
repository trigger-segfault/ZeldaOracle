using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Control.VisualEffects {
	/// <summary>A visual effect that makes use of a shader. This class can be used
	/// as is or overridden.</summary>
	public class ShaderVisualEffect : ShaderVisualEffect<Shader> {
		
		/// <summary>Constructs the shader visual effect with the specified shader.</summary>
		public ShaderVisualEffect(Shader shader) : base(shader) { }
	}

	/// <summary>A visual effect that makes use of a shader. This class can be used
	/// as is or overridden.</summary>
	public class ShaderVisualEffect<TShader> : RoomVisualEffect
		where TShader : Shader
	{
		/// <summary>The shader used for the visual effect.</summary>
		private TShader shader;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the shader visual effect with the specified shader.</summary>
		public ShaderVisualEffect(TShader shader) {
			if (shader == null)
				throw new ArgumentNullException("Shader in ShaderVisualEffect " +
					"cannot be null!");
			this.shader = shader;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Begins tile drawing for the visual effect. This is called once for
		/// drawing regular tiles and another time for drawing tiles' above sprites.</summary>
		public override void Begin(Graphics2D g, Vector2F position) {
			g.End();
			g.PushTranslation(-position);
			g.SetRenderTarget(GameData.RenderTargetGameTemp);
			g.Clear(Color.Transparent);
			g.Begin(GameSettings.DRAW_MODE_PALLETE);
		}

		/// <summary>Ends tile drawing for the visual effect. This is called once for
		/// drawing regular tiles and another time for drawing tiles' above sprites.</summary>
		public override void End(Graphics2D g, Vector2F position) {
			g.End();
			g.PopTranslation(); // -position
			g.SetRenderTarget(GameData.RenderTargetGame);
			Shader.ApplyParameters();
			g.Begin(GameSettings.DRAW_MODE_BASIC, shader);
			g.PushTranslation(-ViewPosition);
			Render(g, GameData.RenderTargetGameTemp);
			g.PopTranslation(); // -ViewPosition

			// Go back to the normal draw mode
			g.End();
			g.Begin(GameSettings.DRAW_MODE_PALLETE);
		}

		/// <summary>Renders the visual effect after all drawing is completed.
		/// By default this is called in RoomVisualEffect.End.</summary>
		public override void Render(Graphics2D g, Texture2D roomImage) {
			g.DrawImage(roomImage, Vector2F.Zero);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the shader used for the visual effect.</summary>
		public TShader Shader {
			get { return shader; }
		}
	}
}
