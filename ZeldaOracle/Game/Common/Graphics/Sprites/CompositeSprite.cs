using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Common.Graphics.Sprites {
	/// <summary>A sprite composed of multiple subsprites.</summary>
	public class CompositeSprite : ISprite {
		/// <summary>The parts of the sprite.</summary>
		private List<OffsetSprite> sprites;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public CompositeSprite() {
			this.sprites		= new List<OffsetSprite>(); ;
		}
		
		public CompositeSprite(CompositeSprite copy) :
			this()
		{
			foreach (OffsetSprite part in copy.sprites) {
				this.sprites.Add(new OffsetSprite(part));
			}
		}


		//-----------------------------------------------------------------------------
		// ISprite Overrides
		//-----------------------------------------------------------------------------

		/// <summary>Gets the drawable parts for the sprite.</summary>
		public SpritePart GetParts(SpriteDrawSettings settings) {
			SpritePart firstPart = null;
			SpritePart nextParts = null;
			foreach (OffsetSprite sprite in sprites) {
				nextParts = sprite.GetParts(settings);
				if (nextParts != null) {
					if (firstPart == null) {
						firstPart = nextParts;
					}
					else {
						firstPart.TailPart.NextPart = nextParts;
						firstPart.TailPart = nextParts.TailPart;
					}
				}
			}
			return firstPart;
		}

		/// <summary>Clones the sprite.</summary>
		public ISprite Clone() {
			return new CompositeSprite(this);
		}

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		public Rectangle2I GetBounds(SpriteDrawSettings settings) {
			Rectangle2I bounds = Rectangle2I.Zero;
			foreach (OffsetSprite sprite in sprites) {
				if (bounds.IsEmpty)
					bounds = sprite.GetBounds(settings);
				else
					bounds = Rectangle2I.Union(bounds, sprite.GetBounds(settings));
			}
			return bounds;
		}

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		public Rectangle2I Bounds {
			get {
				Rectangle2I bounds = Rectangle2I.Zero;
				foreach (OffsetSprite sprite in sprites) {
					if (bounds.IsEmpty)
						bounds = sprite.Bounds;
					else
						bounds = Rectangle2I.Union(bounds, sprite.Bounds);
				}
				return bounds;
			}
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		public IEnumerable<OffsetSprite> GetSprites() {
			return sprites;
		}

		public OffsetSprite GetSprite(int index) {
			return sprites[index];
		}

		public OffsetSprite LastSprite() {
			return sprites[sprites.Count - 1];
		}

		public OffsetSprite LastSpriteOrDefault() {
			if (sprites.Count == 0)
				return null;
			return sprites[sprites.Count - 1];
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		public void ClearSprites() {
			sprites.Clear();
		}

		public void AddOffsetSprite(OffsetSprite sprite) {
			sprites.Add(new OffsetSprite(sprite));
		}

		public void AddSprite(ISprite sprite, Rectangle2I? clipping = null, Flip flip = Flip.None, Rotation rotation = Rotation.None) {
			sprites.Add(new OffsetSprite(sprite, clipping, flip, rotation));
		}

		public void AddSprite(ISprite sprite, Point2I drawOffset, Rectangle2I? clipping = null, Flip flip = Flip.None, Rotation rotation = Rotation.None) {
			sprites.Add(new OffsetSprite(sprite, drawOffset, clipping, flip, rotation));
		}

		public void InsertOffsetSprite(int index, OffsetSprite sprite) {
			sprites.Insert(index, new OffsetSprite(sprite));
		}

		public void InsertSprite(int index, ISprite sprite, Rectangle2I? clipping = null, Flip flip = Flip.None, Rotation rotation = Rotation.None) {
			sprites.Insert(index, new OffsetSprite(sprite, clipping, flip, rotation));
		}

		public void InsertSprite(int index, ISprite sprite, Point2I drawOffset, Rectangle2I? clipping = null, Flip flip = Flip.None, Rotation rotation = Rotation.None) {
			sprites.Insert(index, new OffsetSprite(sprite, drawOffset, clipping, flip, rotation));
		}

		public void ReplaceSprite(int index, ISprite sprite) {
			sprites[index].Sprite = sprite;
		}

		public void ReplaceSprite(int index, ISprite sprite, Point2I drawOffset, Rectangle2I? clipping = null, Flip flip = Flip.None, Rotation rotation = Rotation.None) {
			sprites[index].Sprite = sprite;
			sprites[index].DrawOffset = drawOffset;
			sprites[index].Clipping = clipping;
			sprites[index].FlipEffects = flip;
			sprites[index].Rotation = rotation;
		}

		public void RemoveSprite(int index) {
			sprites.RemoveAt(index);
		}

		public void ClipSprite(int index, Rectangle2I clipping) {
			sprites[index].Clip(clipping);
		}

		public void Clip(Rectangle2I clipping) {
			foreach (OffsetSprite sprite in sprites) {
				sprite.Clip(clipping);
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the number of subsprites in the composite sprite.</summary>
		public int SpriteCount {
			get { return sprites.Count; }
		}
	}
}
