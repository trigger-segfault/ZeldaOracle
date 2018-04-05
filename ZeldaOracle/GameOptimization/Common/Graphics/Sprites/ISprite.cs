using System;
using System.Collections.Generic;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics.Sprites {
	/// <summary>An interface for a drawable sprite.</summary>
	public interface ISprite {
		/// <summary>Gets the linked list of drawable parts of the sprite.</summary>
		SpritePart GetParts(SpriteSettings settings);

		/// <summary>Gets all sprites contained by this sprite including this one.</summary>
		IEnumerable<ISprite> GetAllSprites();

		/// <summary>Returns true if this sprite contains the specified sprite.</summary>
		bool ContainsSubSprite(ISprite sprite);

		/// <summary>Clones the sprite.</summary>
		ISprite Clone();

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		Rectangle2I GetBounds(SpriteSettings settings);

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		Rectangle2I Bounds { get; }
	}

	/// <summary>An exception thrown when trying to add a subsprite to a sprite
	/// that is contained by the subsprite.</summary>
	public class SpriteContainmentException : Exception {
		/// <summary>Constructs the exception with the specified offending sprites.</summary>
		public SpriteContainmentException(ISprite sprite, ISprite subsprite)
			: this(sprite.GetType().Name, subsprite.GetType().Name) { }

		/// <summary>Constructs the exception with the specified offending sprites.</summary>
		public SpriteContainmentException(ISprite sprite, string subSpriteName)
			: this(sprite.GetType().Name, subSpriteName) { }

		/// <summary>Constructs the exception with the specified offending sprites.</summary>
		public SpriteContainmentException(string spriteName, ISprite subsprite)
			: this(spriteName, subsprite.GetType().Name) { }

		/// <summary>Constructs the exception with the specified offending sprites.</summary>
		public SpriteContainmentException(string spriteName, string subSpriteName)
			: base(subSpriteName + " cannot be added to " +
				  spriteName + " because it would contain itself!") { }
	}
}
