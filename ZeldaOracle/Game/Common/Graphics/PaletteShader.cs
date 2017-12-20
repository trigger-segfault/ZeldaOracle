using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace ZeldaOracle.Common.Graphics {
	public class PaletteShader {
		private Effect shader;

		private Palette tilePalette;
		private Palette entityPalette;
		private Palette tilePaletteLerp;
		private Palette entityPaletteLerp;

		public PaletteShader(Effect shader) {
			this.shader = shader;
		}

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

		public void Apply() {
			shader.CurrentTechnique.Passes[0].Apply();
		}

		public Effect Effect {
			get { return shader; }
		}

		public Palette TilePalette {
			get { return tilePalette; }
			set {
				if (value.PaletteType != PaletteTypes.Tile)
					throw new ArgumentException("Cannot set tile palette using a non-tile palette!");
				tilePalette = value;
			}
		}
		public Palette EntityPalette {
			get { return entityPalette; }
			set {
				if (value.PaletteType != PaletteTypes.Entity)
					throw new ArgumentException("Cannot set entity palette using a non-entity palette!");
				entityPalette = value;
			}
		}
		public Palette LerpTilePalette {
			get { return tilePaletteLerp; }
			set {
				if (value.PaletteType != PaletteTypes.Tile)
					throw new ArgumentException("Cannot set tile palette using a non-tile palette!");
				tilePaletteLerp = value;
			}
		}
		public Palette LerpEntityPalette {
			get { return entityPaletteLerp; }
			set {
				if (value.PaletteType != PaletteTypes.Entity)
					throw new ArgumentException("Cannot set entity palette using a non-entity palette!");
				entityPaletteLerp = value;
			}
		}

		public float TileRatio {
			get { return shader.Parameters["TileRatio"].GetValueSingle(); }
			set { shader.Parameters["TileRatio"].SetValue(value); }
		}
		public float EntityRatio {
			get { return shader.Parameters["EntityRatio"].GetValueSingle(); }
			set { shader.Parameters["EntityRatio"].SetValue(value); }
		}
	}
}
