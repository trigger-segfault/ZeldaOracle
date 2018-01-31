using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game;

namespace ZeldaOracle.Common.Graphics {
	
	public struct UnmappedSpriteLookup {
		public SpritePart SpriteParts { get; }
		public Palette TilePalette { get; }
		public Palette EntityPalette { get; }

		public UnmappedSpriteLookup(ISprite sprite, SpriteDrawSettings settings, Palette tilePalette, Palette entityPalette) {
			this.SpriteParts	= sprite.GetParts(settings);
			this.TilePalette	= tilePalette;
			this.EntityPalette	= entityPalette;
		}

		public UnmappedSpriteLookup(SpritePart spriteParts, SpriteDrawSettings settings, Palette tilePalette, Palette entityPalette) {
			this.SpriteParts    = spriteParts;
			this.TilePalette    = tilePalette;
			this.EntityPalette  = entityPalette;
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public override bool Equals(object obj) {
			if (obj is UnmappedSpriteLookup)
				return this == ((UnmappedSpriteLookup) obj);
			return false;
		}

		public static bool operator ==(UnmappedSpriteLookup a, UnmappedSpriteLookup b) {
			if (a.TilePalette == b.TilePalette && a.EntityPalette == b.EntityPalette) {
				SpritePart nextPartA = a.SpriteParts;
				SpritePart nextPartB = b.SpriteParts;
				while (nextPartA != null && nextPartB != null) {
					if (!nextPartA.Equals(nextPartB))
						return false;
					nextPartA = nextPartA.NextPart;
					nextPartB = nextPartB.NextPart;
				}
				return (nextPartA == null && nextPartB == null);
			}
			return false;
		}

		public static bool operator !=(UnmappedSpriteLookup a, UnmappedSpriteLookup b) {
			return !(a == b);
		}
	}

	public static class Unmapping {

		private static Dictionary<UnmappedSpriteLookup, UnmappedSprite> unmappedSprites;

		static Unmapping() {
			unmappedSprites = new Dictionary<UnmappedSpriteLookup, UnmappedSprite>();
		}

		public static UnmappedSprite UnmapSprite(Graphics2D g, ISprite sprite) {
			return UnmapSprite(g, sprite, SpriteDrawSettings.Default, GameData.PAL_TILES_DEFAULT, GameData.PAL_ENTITIES_DEFAULT);
		}

		public static UnmappedSprite UnmapSprite(Graphics2D g, ISprite sprite, SpriteDrawSettings settings) {
			return UnmapSprite(g, sprite, settings, GameData.PAL_TILES_DEFAULT, GameData.PAL_ENTITIES_DEFAULT);
		}

		public static UnmappedSprite UnmapSprite(Graphics2D g, ISprite sprite, Palette tilePalette, Palette entityPalette) {
			return UnmapSprite(g, sprite, SpriteDrawSettings.Default, tilePalette, entityPalette);
		}

		public static UnmappedSprite UnmapSprite(Graphics2D g, ISprite sprite, PaletteShader shader) {
			return UnmapSprite(g, sprite, SpriteDrawSettings.Default, shader.TilePalette, shader.EntityPalette);
		}

		public static UnmappedSprite UnmapSprite(Graphics2D g, ISprite sprite, SpriteDrawSettings settings, PaletteShader shader) {
			return UnmapSprite(g, sprite, settings, shader.TilePalette, shader.EntityPalette);
		}

		public static UnmappedSprite UnmapSprite(Graphics2D g, ISprite sprite, SpriteDrawSettings settings, Palette tilePalette, Palette entityPalette) {
			UnmappedSpriteLookup lookup = new UnmappedSpriteLookup(sprite, settings, tilePalette, entityPalette);
			UnmappedSprite unmappedSprite = null;
			if (unmappedSprites.TryGetValue(lookup, out unmappedSprite))
				return unmappedSprite;

			Rectangle2I bounds = sprite.GetBounds(settings);
			RenderTarget2D renderTarget = new RenderTarget2D(Resources.GraphicsDevice, bounds.Width, bounds.Height, false, SurfaceFormat.Color, DepthFormat.None);
			GameData.PaletteShader.TilePalette = tilePalette;
			GameData.PaletteShader.EntityPalette = entityPalette;
			GameData.PaletteShader.Apply();
			g.SetRenderTarget(renderTarget);
			g.Begin(GameSettings.DRAW_MODE_DEFAULT);
			g.Clear(Color.Transparent);
			g.DrawSprite(sprite, settings, -bounds.Point);
			g.End();
			g.SetRenderTarget(null);
			unmappedSprite = new UnmappedSprite(new Image(renderTarget), bounds.Point);
			unmappedSprites.Add(lookup, unmappedSprite);
			return unmappedSprite;
		}

	}
}
