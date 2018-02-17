using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics.Sprites {
	/// <summary>An interface for a drawable sprite.</summary>
	public interface ISprite {
		/// <summary>Gets the linked list of drawable parts of the sprite.</summary>
		SpritePart GetParts(SpriteSettings settings);

		/// <summary>Clones the sprite.</summary>
		ISprite Clone();

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		Rectangle2I GetBounds(SpriteSettings settings);

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		Rectangle2I Bounds { get; }
	}
}
