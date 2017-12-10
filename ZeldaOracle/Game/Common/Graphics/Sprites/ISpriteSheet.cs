using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics.Sprites {
	public interface ISpriteSheet {

		/// <summary>Gets the sprite at the specified index in the sheet.</summary>
		ISprite GetSprite(int indexX, int indexY);

		/// <summary>Gets the sprite at the specified index in the sheet.</summary>
		ISprite GetSprite(Point2I index);
	}
}
