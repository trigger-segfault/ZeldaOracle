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

		/// <summary>Constructs an empty composite sprite.</summary>
		public CompositeSprite() {
			this.sprites		= new List<OffsetSprite>(); ;
		}

		/// <summary>Constructs a copy of the specified composite sprite.</summary>
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
		public SpritePart GetParts(SpriteSettings settings) {
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

		/// <summary>Gets all sprites contained by this sprite including this one.</summary>
		public IEnumerable<ISprite> GetAllSprites() {
			yield return this;
			foreach (ISprite sprite in sprites) {
				foreach (ISprite subsprite in sprite.GetAllSprites()) {
					yield return subsprite;
				}
			}
		}

		/// <summary>Returns true if this sprite contains the specified sprite.</summary>
		public bool ContainsSubSprite(ISprite sprite) {
			return GetAllSprites().Contains(sprite);
		}

		/// <summary>Clones the sprite.</summary>
		public ISprite Clone() {
			return new CompositeSprite(this);
		}

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		public Rectangle2I GetBounds(SpriteSettings settings) {
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

		/// <summary>Gets the collection of sprites in the list.</summary>
		public IEnumerable<OffsetSprite> GetSprites() {
			return sprites;
		}

		/// <summary>Gets the sprite at the specified index in the list.</summary>
		public OffsetSprite GetSprite(int index) {
			return sprites[index];
		}

		/// <summary>Gets the last sprite in the list.</summary>
		public OffsetSprite LastSprite() {
			return sprites[sprites.Count - 1];
		}

		/// <summary>Gets the last sprite in the list or null if the list is empty.</summary>
		public OffsetSprite LastSpriteOrDefault() {
			if (sprites.Count == 0)
				return null;
			return sprites[sprites.Count - 1];
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Clears the list of sprites.</summary>
		public void ClearSprites() {
			sprites.Clear();
		}

		/// <summary>Adds the offset sprite to the list after making a copy of it.</summary>
		public void AddOffsetSprite(OffsetSprite sprite) {
			if (sprite != null && sprite.ContainsSubSprite(this))
				throw new SpriteContainmentException(this, sprite.Sprite);
			sprites.Add(new OffsetSprite(sprite));
		}

		/// <summary>Adds a new sprite to the list.</summary>
		public void AddSprite(ISprite sprite, Rectangle2I? clipping = null, Flip flip = Flip.None, Rotation rotation = Rotation.None) {
			AddOffsetSprite(new OffsetSprite(sprite, clipping, flip, rotation));
		}

		/// <summary>Adds a new sprite to the list.</summary>
		public void AddSprite(ISprite sprite, Point2I drawOffset, Rectangle2I? clipping = null, Flip flip = Flip.None, Rotation rotation = Rotation.None) {
			AddOffsetSprite(new OffsetSprite(sprite, drawOffset, clipping, flip, rotation));
		}

		/// <summary>Inserts the offset sprite into the list after making a copy of it.</summary>
		public void InsertOffsetSprite(int index, OffsetSprite sprite) {
			if (sprite != null && sprite.ContainsSubSprite(this))
				throw new SpriteContainmentException(this, sprite.Sprite);
			sprites.Insert(index, new OffsetSprite(sprite));
		}

		/// <summary>Inserts a new sprite into the list.</summary>
		public void InsertSprite(int index, ISprite sprite, Rectangle2I? clipping = null, Flip flip = Flip.None, Rotation rotation = Rotation.None) {
			InsertOffsetSprite(index, new OffsetSprite(sprite, clipping, flip, rotation));
		}

		/// <summary>Inserts a new sprite into the list.</summary>
		public void InsertSprite(int index, ISprite sprite, Point2I drawOffset, Rectangle2I? clipping = null, Flip flip = Flip.None, Rotation rotation = Rotation.None) {
			InsertOffsetSprite(index, new OffsetSprite(sprite, drawOffset, clipping, flip, rotation));
		}

		/// <summary>Replaces the sprite at the specified index in the list and keeps
		/// its offset settings.</summary>
		public void ReplaceSprite(int index, ISprite sprite) {
			if (sprite != null && sprite.ContainsSubSprite(this))
				throw new SpriteContainmentException(this, sprite);
			sprites[index].Sprite = sprite;
		}

		/// <summary>Replaces the sprite at the specified index in the list and changes
		/// its offset settings.</summary>
		public void ReplaceSprite(int index, ISprite sprite, Point2I drawOffset, Rectangle2I? clipping = null, Flip flip = Flip.None, Rotation rotation = Rotation.None) {
			if (sprite != null && sprite.ContainsSubSprite(this))
				throw new SpriteContainmentException(this, sprite);
			sprites[index].Sprite = sprite;
			sprites[index].DrawOffset = drawOffset;
			sprites[index].Clipping = clipping;
			sprites[index].FlipEffects = flip;
			sprites[index].Rotation = rotation;
		}

		/// <summary>Removes the sprite at the specified index from the list.</summary>
		public void RemoveSprite(int index) {
			sprites.RemoveAt(index);
		}

		/// <summary>Applies clipping to the specified sprite.</summary>
		public void ClipSprite(int index, Rectangle2I clipping) {
			sprites[index].Clip(clipping);
		}

		/// <summary>Clips all sprites in the list.</summary>
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
