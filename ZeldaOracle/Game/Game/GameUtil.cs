using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game {
	/// <summary>The static class for game helper functions.</summary>
	public static class GameUtil {
		/// <summary>Rounds the specified vector to the nearest integral coordinates with bias.</summary>
		public static Vector2F Bias(Vector2F a) {
			return new Vector2F(
				(float) Math.Round(a.X + GameSettings.BIAS),
				(float) Math.Round(a.Y + GameSettings.BIAS));
		}

		/// <summary>Rounds the specified vector to the nearest integral coordinates with bias.</summary>
		public static Vector2F ReverseBias(Vector2F a) {
			return new Vector2F(
				(float) Math.Round(a.X - GameSettings.BIAS),
				(float) Math.Round(a.Y - GameSettings.BIAS));
		}
	}
}
