using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Util {
	/// <summary>Extensions for binary reader and writer classes.</summary>
	public static class BinaryExtensions {

		/// <summary>Reads the Point2I from the stream and advances the current position.</summary>
		public static Point2I ReadPoint2I(this BinaryReader reader) {
			return new Point2I(reader.ReadInt32(), reader.ReadInt32());
		}

		/// <summary>Reads the Vector2F from the stream and advances the current position.</summary>
		public static Vector2F ReadVector2F(this BinaryReader reader) {
			return new Vector2F(reader.ReadSingle(), reader.ReadSingle());
		}

		/// <summary>Writes the Point2I to the stream and advances the current position.</summary>
		public static void Write(this BinaryWriter writer, Point2I point) {
			writer.Write(point.X);
			writer.Write(point.Y);
		}

		/// <summary>Writes the Vector2F to the stream and advances the current position.</summary>
		public static void Write(this BinaryWriter writer, Vector2F vector) {
			writer.Write(vector.X);
			writer.Write(vector.Y);
		}
	}
}
