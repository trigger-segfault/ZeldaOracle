using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics.Sprites {
	/// <summary>An interface for a grid of sprites.</summary>
	public interface ISpriteSource {
		/// <summary>Gets the sprite at the specified index in the sprite source.</summary>
		ISprite GetSprite(int indexX, int indexY);

		/// <summary>Gets the sprite at the specified index in the sprite source.</summary>
		ISprite GetSprite(Point2I index);


		/// <summary>Calculates the dimensions of the sprite source.</summary>
		Point2I Dimensions { get; }

		/// <summary>Gets the width of the sprite source.</summary>
		int Width { get; }

		/// <summary>Gets the height of the sprite source.</summary>
		int Height { get; }
	}
}
