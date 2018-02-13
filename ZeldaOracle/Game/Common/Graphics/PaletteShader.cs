using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace ZeldaOracle.Common.Graphics {
	/// <summary>A wrapper class for the shader used for palettizing while drawing.</summary>
	public class PaletteShader {
		/// <summary>The palette shader.</summary>
		private Effect shader;

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

		/// <summary>Constructs the palette shader wrapper.</summary>
		public PaletteShader(Effect shader) {
			this.shader = shader;
		}


		//-----------------------------------------------------------------------------
		// Management
		//-----------------------------------------------------------------------------

		/// <summary>Applies the palettes to the graphics device.</summary>
		public void ApplyPalettes() {
			shader.GraphicsDevice.Textures[1] = tilePalette.PaletteTexture;
			shader.GraphicsDevice.Textures[2] = entityPalette.PaletteTexture;
			if (tilePaletteLerp != null)
				shader.GraphicsDevice.Textures[3] = tilePaletteLerp.PaletteTexture;
			else
				shader.GraphicsDevice.Textures[3] = tilePalette.PaletteTexture;
			if (entityPaletteLerp != null)
				shader.GraphicsDevice.Textures[4] = entityPaletteLerp.PaletteTexture;
			else
				shader.GraphicsDevice.Textures[4] = entityPalette.PaletteTexture;
		}

		/// <summary>Resets the lerping to zero.</summary>
		public void ResetLerp() {
			tilePaletteLerp = null;
			entityPaletteLerp = null;
			TileRatio = 0f;
			EntityRatio = 0f;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the underlying shader effect.</summary>
		public Effect Effect {
			get { return shader; }
		}

		/// <summary>Gets or sets the current tile palette.</summary>
		public Palette TilePalette {
			get { return tilePalette; }
			set {
				if (value.PaletteType != PaletteTypes.Tile)
					throw new ArgumentException("Cannot set tile palette using a non-tile palette!");
				tilePalette = value;
			}
		}

		/// <summary>Gets or sets the current entity palette.</summary>
		public Palette EntityPalette {
			get { return entityPalette; }
			set {
				if (value.PaletteType != PaletteTypes.Entity)
					throw new ArgumentException("Cannot set entity palette using a non-entity palette!");
				entityPalette = value;
			}
		}

		/// <summary>Gets or sets the upcoming tile palette being lerped to.</summary>
		public Palette LerpTilePalette {
			get { return tilePaletteLerp; }
			set {
				if (value != null && value.PaletteType != PaletteTypes.Tile)
					throw new ArgumentException("Cannot set tile palette using a non-tile palette!");
				tilePaletteLerp = value;
			}
		}

		/// <summary>Gets or sets the upcoming entity palette being lerped to.</summary>
		public Palette LerpEntityPalette {
			get { return entityPaletteLerp; }
			set {
				if (value != null && value.PaletteType != PaletteTypes.Entity)
					throw new ArgumentException("Cannot set entity palette using a non-entity palette!");
				entityPaletteLerp = value;
			}
		}

		/// <summary>Gets or sets the ratio for lerping from the current to upcoming
		/// tile palette. This cannot be set while the sprite batch is drawing!</summary>
		public float TileRatio {
			get { return shader.Parameters["TileRatio"].GetValueSingle(); }
			set { shader.Parameters["TileRatio"].SetValue(value); }
		}

		/// <summary>Gets or sets the ratio for lerping from the current to upcoming
		/// entity palette. This cannot be set while the sprite batch is drawing!</summary>
		public float EntityRatio {
			get { return shader.Parameters["EntityRatio"].GetValueSingle(); }
			set { shader.Parameters["EntityRatio"].SetValue(value); }
		}
	}
}
