using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Util;

namespace ZeldaOracle.Common.Graphics {
	/// <summary>A wrapper class for XNA effects.</summary>
	public class Shader : IDisposable {

		/// <summary>The effect of the shader.</summary>
		protected Effect effect;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs an unassigned shader.</summary>
		protected Shader() {
			effect = null;
		}

		/// <summary>Constructs the shader with the specified effect.</summary>
		public Shader(Effect effect) {
			this.effect = effect;
		}


		//-----------------------------------------------------------------------------
		// Disposing
		//-----------------------------------------------------------------------------

		/// <summary>Immediately releases the unmanaged resources used by the effect.</summary>
		public void Dispose() {
			if (effect != null && !effect.IsDisposed)
				effect.Dispose();
		}


		//-----------------------------------------------------------------------------
		// Parameters
		//-----------------------------------------------------------------------------

		/// <summary>Applies the parameters to the shader and graphics device.</summary>
		public virtual void ApplyParameters() { }

		/// <summary>Resets the shader parameters.</summary>
		public virtual void ResetParameters() { }


		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

		/// <summary>Used to auto-convert Shaders into XNA Effects.</summary>
		public static implicit operator Effect(Shader shader) {
			return shader.effect;
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Loads the shader from content.</summary>
		public static Shader FromContent(string assetName) {
			return new Shader(Resources.ContentManager.Load<Effect>(assetName));
		}

		/// <summary>Loads the shader of the specified type from content.</summary>
		public static Shader FromContent(Type type, string assetName) {
			Effect effect = Resources.ContentManager.Load<Effect>(assetName);
			return ReflectionHelper.Construct<Shader, Effect>(type, effect);
		}

		/// <summary>Loads the shader of the specified type from content.</summary>
		public static TShader FromContent<TShader>(string assetName)
			where TShader : Shader
		{
			Effect effect = Resources.ContentManager.Load<Effect>(assetName);
			return ReflectionHelper.Construct<TShader, Effect>(effect);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Information ----------------------------------------------------------------

		/// <summary>Gets the effect of the shader.</summary>
		public Effect Effect {
			get { return effect; }
		}

		/// <summary>Gets the graphics device associated with the shader.</summary>
		public GraphicsDevice GraphicsDevice {
			get { return effect?.GraphicsDevice; }
		}

		/// <summary>Returns true if the shader has been disposed.</summary>
		public bool IsDisposed {
			get { return effect?.IsDisposed ?? true; }
		}

		/// <summary>Gets a collection of parameters used for this shader.</summary>
		public EffectParameterCollection Parameters {
			get { return effect?.Parameters; }
		}

		/// <summary>Returns the collection of textures that have been assigned to
		/// the texture stages of the graphics device.</summary>
		public TextureCollection Textures {
			get { return effect?.GraphicsDevice.Textures; }
		}
	}
}
