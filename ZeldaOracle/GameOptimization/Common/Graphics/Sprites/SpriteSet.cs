using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Common.Graphics {
	/// <summary>A grid of defined sprites that can come from anywhere.</summary>
	public class SpriteSet : ISpriteSource {
		/// <summary>The grid of defined sprites.</summary>
		private ISprite[,] sprites;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a sprite set with the specified dimensions.</summary>
		public SpriteSet(Point2I dimensions) {
			sprites	= new ISprite[dimensions.X, dimensions.Y];
		}

		/// <summary>Constructs a copy of the specified sprite set.</summary>
		public SpriteSet(SpriteSet copy) {
			sprites = new ISprite[copy.Width, copy.Height];
			for (int x = 0; x < Width; x++) {
				for (int y = 0; y < Height; y++) {
					sprites[x, y] = copy.sprites[x, y];
				}
			}
		}


		//-----------------------------------------------------------------------------
		// ISpriteSheet Overrides
		//-----------------------------------------------------------------------------

		/// <summary>Gets the sprite at the specified index in the set.</summary>
		public ISprite GetSprite(int indexX, int indexY) {
			return sprites[indexX, indexY];
		}

		/// <summary>Gets the sprite at the specified index in the set.</summary>
		public ISprite GetSprite(Point2I index) {
			return sprites[index.X, index.Y];
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Sets the sprite at the specified index in the set.</summary>
		public void SetSprite(int indexX, int indexY, ISprite sprite) {
			sprites[indexX, indexY] = sprite;
		}

		/// <summary>Sets the sprite at the specified index in the set.</summary>
		public void SetSprite(Point2I index, ISprite sprite) {
			sprites[index.X, index.Y] = sprite;
		}

		/// <summary>Removes the sprite at the specified index from the set.</summary>
		public void RemoveSprite(int indexX, int indexY) {
			sprites[indexX, indexY] = null;
		}

		/// <summary>Removes the sprite at the specified index from the set.</summary>
		public void RemoveSprite(Point2I index) {
			sprites[index.X, index.Y] = null;
		}

		/// <summary>Removes all sprites from the set.</summary>
		public void ClearSprites() {
			for (int x = 0; x < Width; x++) {
				for (int y = 0; y < Height; y++) {
					sprites[x, y] = null;
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the dimensions of the sprite set.</summary>
		public Point2I Dimensions {
			get { return new Point2I(sprites.GetLength(0), sprites.GetLength(1)); }
		}

		/// <summary>Gets the width of the sprite set.</summary>
		public int Width {
			get { return sprites.GetLength(0); }
		}

		/// <summary>Gets the height of the sprite set.</summary>
		public int Height {
			get { return sprites.GetLength(1); }
		}
	}
}
