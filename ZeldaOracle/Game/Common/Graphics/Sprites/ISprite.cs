using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics.Sprites {
	/// <summary>An interface for a drawable sprite.</summary>
	public interface ISprite {
		/// <summary>Gets the drawable parts of the sprite.</summary>
		IEnumerable<SpritePart> GetParts(SpriteDrawSettings settings);

		/// <summary>Clones the sprite.</summary>
		ISprite Clone();

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		Rectangle2I GetBounds(SpriteDrawSettings settings);

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		Rectangle2I Bounds { get; }
	}
}
