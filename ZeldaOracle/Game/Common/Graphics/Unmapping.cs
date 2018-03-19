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
	
	/// <summary>A structured used to lookup already-unmapped unmapped sprites.</summary>
	public struct UnmappedSpriteLookup {
		/// <summary>The sprite parts this unmapped sprite is made of.</summary>
		public SpritePart SpriteParts { get; }
		/// <summary>The tile palette used for this unmapped sprite.</summary>
		public Palette TilePalette { get; }
		/// <summary>The entity palette used for this unmapped sprite.</summary>
		public Palette EntityPalette { get; }


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs an unmapped sprite lookup from the specified sprite,
		/// settings, and palettes.</summary>
		public UnmappedSpriteLookup(ISprite sprite, SpriteSettings settings, Palette tilePalette, Palette entityPalette) {
			this.SpriteParts	= sprite.GetParts(settings);
			this.TilePalette	= tilePalette;
			this.EntityPalette	= entityPalette;
		}

		/// <summary>Constructs an unmapped sprite lookup from the specified sprite parts
		/// and palettes.</summary>
		public UnmappedSpriteLookup(SpritePart spriteParts, Palette tilePalette, Palette entityPalette) {
			this.SpriteParts    = spriteParts;
			this.TilePalette    = tilePalette;
			this.EntityPalette  = entityPalette;
		}


		//-----------------------------------------------------------------------------
		// General
		//-----------------------------------------------------------------------------

		/// <summary>Gets the hash code for the unmapped sprite lookup.</summary>
		public override int GetHashCode() {
			if (SpriteParts == null)
				return 0;
			return SpriteParts.GetHashCode();
		}

		/// <summary>Returns true if the object is an unmapped sprite lookup and equal.</summary>
		public override bool Equals(object obj) {
			if (obj is UnmappedSpriteLookup)
				return this == ((UnmappedSpriteLookup) obj);
			return false;
		}


		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

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

	/// <summary>A static class for creating unmapped sprites from paletted sprites.</summary>
	public static class Unmapping {

		/// <summary>The collection of unmapped sprites.</summary>
		private static Dictionary<UnmappedSpriteLookup, UnmappedSprite> unmappedSprites;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the database of unmapped sprites.</summary>
		static Unmapping() {
			unmappedSprites = new Dictionary<UnmappedSpriteLookup, UnmappedSprite>();
		}


		//-----------------------------------------------------------------------------
		// Unmapping
		//-----------------------------------------------------------------------------

		/// <summary>Unmaps the specified sprite.</summary>
		public static UnmappedSprite UnmapSprite(Graphics2D g, ISprite sprite) {
			return UnmapSprite(g, sprite, SpriteSettings.Default, GameData.PAL_TILES_DEFAULT, GameData.PAL_ENTITIES_DEFAULT);
		}

		/// <summary>Unmaps the specified sprite.</summary>
		public static UnmappedSprite UnmapSprite(Graphics2D g, ISprite sprite, SpriteSettings settings) {
			return UnmapSprite(g, sprite, settings, GameData.PAL_TILES_DEFAULT, GameData.PAL_ENTITIES_DEFAULT);
		}

		/// <summary>Unmaps the specified sprite.</summary>
		public static UnmappedSprite UnmapSprite(Graphics2D g, ISprite sprite, Palette tilePalette, Palette entityPalette) {
			return UnmapSprite(g, sprite, SpriteSettings.Default, tilePalette, entityPalette);
		}

		/// <summary>Unmaps the specified sprite.</summary>
		public static UnmappedSprite UnmapSprite(Graphics2D g, ISprite sprite, PaletteShader shader) {
			return UnmapSprite(g, sprite, SpriteSettings.Default, shader.TilePalette, shader.EntityPalette);
		}

		/// <summary>Unmaps the specified sprite.</summary>
		public static UnmappedSprite UnmapSprite(Graphics2D g, ISprite sprite, SpriteSettings settings, PaletteShader shader) {
			return UnmapSprite(g, sprite, settings, shader.TilePalette, shader.EntityPalette);
		}

		/// <summary>Unmaps the specified sprite.</summary>
		public static UnmappedSprite UnmapSprite(Graphics2D g, ISprite sprite, SpriteSettings settings, Palette tilePalette, Palette entityPalette) {
			UnmappedSpriteLookup lookup = new UnmappedSpriteLookup(sprite, settings, tilePalette, entityPalette);
			UnmappedSprite unmappedSprite = null;
			if (unmappedSprites.TryGetValue(lookup, out unmappedSprite))
				return unmappedSprite;

			Rectangle2I bounds = sprite.GetBounds(settings);
			bounds.Size = GMath.Max(Point2I.One, bounds.Size);
			RenderTarget renderTarget = new RenderTarget(bounds.Size, SurfaceFormat.Color);
			GameData.PaletteShader.TilePalette = tilePalette;
			GameData.PaletteShader.EntityPalette = entityPalette;
			GameData.PaletteShader.ApplyPalettes();
			g.SetRenderTarget(renderTarget);
			g.Begin(GameSettings.DRAW_MODE_DEFAULT);
			g.Clear(Color.Transparent);
			g.DrawSprite(sprite, settings, -bounds.Point);
			g.End();
			g.SetRenderTarget(null);
			unmappedSprite = new UnmappedSprite(renderTarget, bounds.Point);
			unmappedSprites.Add(lookup, unmappedSprite);
			return unmappedSprite;
		}
	}
}
