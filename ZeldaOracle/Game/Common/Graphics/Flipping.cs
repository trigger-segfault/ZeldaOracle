using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects;

namespace ZeldaOracle.Common.Graphics {
	/// <summary>Defines sprite mirroring options.</summary>
	[Flags]
	public enum Flip {
		/// <summary>No rotations specified.</summary>
		None = 0,
		/// <summary>Rotate 180 degrees about the Y axis before rendering.</summary>
		Horizontal = 1,
		/// <summary>Rotate 180 degrees about the X axis before rendering.</summary>
		Vertical = 2
	}

	/// <summary>Extensions for the Flip enumeration.</summary>
	public static class Flipping {

		/// <summary>Adds the flipping together by xor-ing them.</summary>
		public static Flip Add(Flip flip1, Flip flip2) {
			return flip1 ^ flip2;
		}
	}
}
