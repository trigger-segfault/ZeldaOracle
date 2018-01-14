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
		public IEnumerable<SpritePart> GetParts(SpriteDrawSettings settings) {
			Rectangle2I bounds = Bounds;
			foreach (OffsetSprite sprite in sprites) {
				foreach (SpritePart part in sprite.GetParts(settings)) {
					yield return part;
				}
			}
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
					bounds = sprite.Sprite.GetBounds(settings) + sprite.DrawOffset;
				else
					bounds = Rectangle2I.Union(bounds, sprite.Sprite.GetBounds(settings) + sprite.DrawOffset);
			}
			return bounds;
		}

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		public Rectangle2I Bounds {
			get {
				Rectangle2I bounds = Rectangle2I.Zero;
				foreach (OffsetSprite sprite in sprites) {
					if (bounds.IsEmpty)
						bounds = sprite.Sprite.Bounds + sprite.DrawOffset;
					else
						bounds = Rectangle2I.Union(bounds, sprite.Sprite.Bounds + sprite.DrawOffset);
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

		public void AddSprite(ISprite sprite, Flip flip = Flip.None, Rotation rotation = Rotation.None) {
			sprites.Add(new OffsetSprite(sprite, flip, rotation));
		}

		public void AddSprite(ISprite sprite, Point2I drawOffset, Flip flip = Flip.None, Rotation rotation = Rotation.None) {
			sprites.Add(new OffsetSprite(sprite, drawOffset, flip, rotation));
		}

		public void InsertSprite(int index, ISprite sprite, Flip flip = Flip.None, Rotation rotation = Rotation.None) {
			sprites.Insert(index, new OffsetSprite(sprite, flip, rotation));
		}

		public void InsertSprite(int index, ISprite sprite, Point2I drawOffset, Flip flip = Flip.None, Rotation rotation = Rotation.None) {
			sprites.Insert(index, new OffsetSprite(sprite, drawOffset, flip, rotation));
		}

		public void ReplaceSprite(int index, ISprite sprite) {
			sprites[index].Sprite = sprite;
		}

		public void ReplaceSprite(int index, ISprite sprite, Point2I drawOffset, Flip flip = Flip.None, Rotation rotation = Rotation.None) {
			sprites[index].Sprite = sprite;
			sprites[index].DrawOffset = drawOffset;
			sprites[index].FlipEffects = flip;
			sprites[index].Rotation = rotation;
		}

		public void RemoveSprite(int index) {
			sprites.RemoveAt(index);
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
