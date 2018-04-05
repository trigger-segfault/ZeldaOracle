using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace ZeldaOracle.Common.Graphics.Shaders {
	/// <summary>A wrapper class for the shader used for palettizing while drawing.</summary>
	public class PaletteShader : Shader {

		/// <summary>The current tile palette.</summary>
		private Palette tilePalette;
		/// <summary>The current entity palette.</summary>
		private Palette entityPalette;
		/// <summary>The upcoming tile palette being lerped to.</summary>
		private Palette tilePaletteLerp;
		/// <summary>The upcoming entity palette being lerped to.</summary>
		private Palette entityPaletteLerp;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		/// <summary>Constructs the palette shader.</summary>
		public PaletteShader(Effect effect) : base(effect) { }


		//-----------------------------------------------------------------------------
		// Management
		//-----------------------------------------------------------------------------

		/// <summary>Applies the parameters to the shader and graphics device.</summary>
		public override void ApplyParameters() {
			Textures[1] = tilePalette.PaletteImage;
			Textures[2] = entityPalette.PaletteImage;
			if (tilePaletteLerp != null)
				Textures[3] = tilePaletteLerp?.PaletteImage;
			else
				Textures[3] = tilePalette.PaletteImage;
			if (entityPaletteLerp != null)
				Textures[4] = entityPaletteLerp.PaletteImage;
			else
				Textures[4] = entityPalette.PaletteImage;
		}

		/// <summary>Resets the shader parameters.</summary>
		public override void ResetParameters() {
			tilePaletteLerp = null;
			entityPaletteLerp = null;
			TileRatio = 0f;
			EntityRatio = 0f;
		}


		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

		/// <summary>Used to auto-convert Shaders into XNA Effects.</summary>
		public static implicit operator Effect(PaletteShader shader) {
			return shader.effect;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the current tile palette.</summary>
		public Palette TilePalette {
			get { return tilePalette; }
			set {
				if (value.PaletteType != PaletteTypes.Tile)
					throw new ArgumentException("Cannot set tile palette using a " +
						"non-tile palette!");
				tilePalette = value;
			}
		}

		/// <summary>Gets or sets the current entity palette.</summary>
		public Palette EntityPalette {
			get { return entityPalette; }
			set {
				if (value.PaletteType != PaletteTypes.Entity)
					throw new ArgumentException("Cannot set entity palette using a " +
						"non-entity palette!");
				entityPalette = value;
			}
		}

		/// <summary>Gets or sets the upcoming tile palette being lerped to.</summary>
		public Palette LerpTilePalette {
			get { return tilePaletteLerp; }
			set {
				if (value != null && value.PaletteType != PaletteTypes.Tile)
					throw new ArgumentException("Cannot set tile palette using a " +
						"non-tile palette!");
				tilePaletteLerp = value;
			}
		}

		/// <summary>Gets or sets the upcoming entity palette being lerped to.</summary>
		public Palette LerpEntityPalette {
			get { return entityPaletteLerp; }
			set {
				if (value != null && value.PaletteType != PaletteTypes.Entity)
					throw new ArgumentException("Cannot set entity palette using a " +
						"non-entity palette!");
				entityPaletteLerp = value;
			}
		}

		/// <summary>Gets or sets the ratio for lerping from the current to upcoming
		/// tile palette. This cannot be set while the sprite batch is drawing!</summary>
		public float TileRatio {
			get { return Parameters["TileRatio"].GetValueSingle(); }
			set { Parameters["TileRatio"].SetValue(value); }
		}

		/// <summary>Gets or sets the ratio for lerping from the current to upcoming
		/// entity palette. This cannot be set while the sprite batch is drawing!</summary>
		public float EntityRatio {
			get { return Parameters["EntityRatio"].GetValueSingle(); }
			set { Parameters["EntityRatio"].SetValue(value); }
		}
	}
}
