using System;
using System.Collections.Generic;
using System.IO;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Common.Util {
	/// <summary>A function for reading a generic value with BinaryReader.</summary>
	public delegate object GenericBinaryReader(BinaryReader reader);
	/// <summary>A function for writing a generic value with BinaryWriter.</summary>
	public delegate void GenericBinaryWriter(BinaryWriter writer, object value);

	/// <summary>Extensions for binary reader and writer classes.</summary>
	public static class BinaryExtensions {
		
		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The collection of generic type readers.</summary>
		private static Dictionary<Type, GenericBinaryReader> readers;
		/// <summary>The collection of generic type writers.</summary>
		private static Dictionary<Type, GenericBinaryWriter> writers;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the built-in generic type readers and writers.</summary>
		static BinaryExtensions() {
			// Initialize the built-in generic readers
			readers = new Dictionary<Type, GenericBinaryReader>();
			AddReader<bool>(	(reader) => reader.ReadBoolean());
			AddReader<byte>(	(reader) => reader.ReadByte());
			AddReader<sbyte>(	(reader) => reader.ReadSByte());
			AddReader<short>(	(reader) => reader.ReadInt16());
			AddReader<ushort>(	(reader) => reader.ReadUInt16());
			AddReader<int>(		(reader) => reader.ReadInt32());
			AddReader<uint>(	(reader) => reader.ReadUInt32());
			AddReader<long>(	(reader) => reader.ReadInt64());
			AddReader<ulong>(	(reader) => reader.ReadUInt64());
			AddReader<float>(	(reader) => reader.ReadSingle());
			AddReader<double>(	(reader) => reader.ReadDouble());
			AddReader<decimal>(	(reader) => reader.ReadDecimal());
			AddReader<char>(	(reader) => reader.ReadChar());
			AddReader<string>(	(reader) => reader.ReadString());

			AddReader<Point2I>(		(reader) => reader.ReadPoint2I());
			AddReader<Vector2F>(	(reader) => reader.ReadVector2F());
			AddReader<RangeI>(		(reader) => reader.ReadRangeI());
			AddReader<RangeF>(		(reader) => reader.ReadRangeF());
			AddReader<Rectangle2I>(	(reader) => reader.ReadRectangle2I());
			AddReader<Rectangle2F>(	(reader) => reader.ReadRectangle2F());
			AddReader<Color>(		(reader) => reader.ReadColor());

			// Initialize the built-in generic writers
			writers = new Dictionary<Type, GenericBinaryWriter>();
			AddWriter<bool>(	(writer, x) => writer.Write((bool) x));
			AddWriter<byte>(	(writer, x) => writer.Write((byte) x));
			AddWriter<sbyte>(	(writer, x) => writer.Write((sbyte) x));
			AddWriter<short>(	(writer, x) => writer.Write((short) x));
			AddWriter<ushort>(	(writer, x) => writer.Write((ushort) x));
			AddWriter<int>(		(writer, x) => writer.Write((int) x));
			AddWriter<uint>(	(writer, x) => writer.Write((uint) x));
			AddWriter<long>(	(writer, x) => writer.Write((long) x));
			AddWriter<ulong>(	(writer, x) => writer.Write((ulong) x));
			AddWriter<float>(	(writer, x) => writer.Write((float) x));
			AddWriter<double>(	(writer, x) => writer.Write((double) x));
			AddWriter<decimal>(	(writer, x) => writer.Write((decimal) x));
			AddWriter<char>(	(writer, x) => writer.Write((char) x));
			AddWriter<string>(	(writer, x) => writer.Write((string) x));

			AddWriter<Point2I>(		(writer, x) => writer.Write((Point2I) x));
			AddWriter<Vector2F>(	(writer, x) => writer.Write((Vector2F) x));
			AddWriter<RangeI>(		(writer, x) => writer.Write((RangeI) x));
			AddWriter<RangeF>(		(writer, x) => writer.Write((RangeF) x));
			AddWriter<Rectangle2I>(	(writer, x) => writer.Write((Rectangle2I) x));
			AddWriter<Rectangle2F>(	(writer, x) => writer.Write((Rectangle2F) x));
			AddWriter<Color>(		(writer, x) => writer.Write((Color) x));
		}


		//-----------------------------------------------------------------------------
		// Generic Reader Setup
		//-----------------------------------------------------------------------------

		/// <summary>Adds the generic reader for the specified type.</summary>
		public static void AddReader<T>(GenericBinaryReader reader) {
			AddReader(typeof(T), reader);
		}

		/// <summary>Adds the generic reader for the specified type.</summary>
		public static void AddReader(Type type, GenericBinaryReader reader) {
			readers.Add(type, reader);
		}

		/// <summary>Retrns true if the type has a generic reader assigned to it.</summary>
		public static bool ContainsReader<T>() {
			return ContainsReader(typeof(T));
		}

		/// <summary>Retrns true if the type has a generic reader assigned to it.</summary>
		public static bool ContainsReader(Type type) {
			return readers.ContainsKey(type);
		}


		//-----------------------------------------------------------------------------
		// Generic Writer Setup
		//-----------------------------------------------------------------------------

		/// <summary>Adds the generic writer for the specified type.</summary>
		public static void AddWriter<T>(GenericBinaryWriter writer) {
			AddWriter(typeof(T), writer);
		}

		/// <summary>Adds the generic writer for the specified type.</summary>
		public static void AddWriter(Type type, GenericBinaryWriter writer) {
			writers.Add(type, writer);
		}

		/// <summary>Retrns true if the type has a generic writer assigned to it.</summary>
		public static bool ContainsWriter<T>() {
			return ContainsWriter(typeof(T));
		}

		/// <summary>Retrns true if the type has a generic writer assigned to it.</summary>
		public static bool ContainsWriter(Type type) {
			return writers.ContainsKey(type);
		}


		//-----------------------------------------------------------------------------
		// Readers
		//-----------------------------------------------------------------------------

		/// <summary>Reads the generic type value.</summary>
		public static T ReadGeneric<T>(this BinaryReader reader) {
			GenericBinaryReader readerFunc;
			if (readers.TryGetValue(typeof(T), out readerFunc))
				return (T) readerFunc(reader);
			throw new ArgumentException("Unsupported type for " +
				"BinaryReader.ReadGeneric!");
		}

		/// <summary>Reads the generic type value.</summary>
		public static object ReadGeneric(this BinaryReader reader, Type type) {
			GenericBinaryReader readerFunc;
			if (readers.TryGetValue(type, out readerFunc))
				return readerFunc(reader);
			throw new ArgumentException("Unsupported type for " +
				"BinaryReader.ReadGeneric!");
		}

		/// <summary>Reads the Point2I from the stream and advances the current
		/// position.</summary>
		public static Point2I ReadPoint2I(this BinaryReader reader) {
			return new Point2I(reader.ReadInt32(), reader.ReadInt32());
		}

		/// <summary>Reads the Rectangle2I from the stream and advances the current
		/// position.</summary>
		public static Rectangle2I ReadRectangle2I(this BinaryReader reader) {
			return new Rectangle2I(	reader.ReadInt32(), reader.ReadInt32(),
									reader.ReadInt32(), reader.ReadInt32());
		}

		/// <summary>Reads the RangeI from the stream and advances the current
		/// position.</summary>
		public static RangeI ReadRangeI(this BinaryReader reader) {
			return new RangeI(reader.ReadInt32(), reader.ReadInt32());
		}

		/// <summary>Reads the Vector2F from the stream and advances the current
		/// position.</summary>
		public static Vector2F ReadVector2F(this BinaryReader reader) {
			return new Vector2F(reader.ReadSingle(), reader.ReadSingle());
		}

		/// <summary>Reads the Rectangle2F from the stream and advances the current
		/// position.</summary>
		public static Rectangle2F ReadRectangle2F(this BinaryReader reader) {
			return new Rectangle2F(	reader.ReadSingle(), reader.ReadSingle(),
									reader.ReadSingle(), reader.ReadSingle());
		}

		/// <summary>Reads the RangeF from the stream and advances the current
		/// position.</summary>
		public static RangeF ReadRangeF(this BinaryReader reader) {
			return new RangeF(reader.ReadSingle(), reader.ReadSingle());
		}

		/// <summary>Reads the Color from the stream and advances the current
		/// position.</summary>
		public static Color ReadColor(this BinaryReader reader) {
			return new Color(reader.ReadByte(), reader.ReadByte(),
							 reader.ReadByte(), reader.ReadByte());
		}


		//-----------------------------------------------------------------------------
		// Writers
		//-----------------------------------------------------------------------------

		/// <summary>Writes the generic integral type.</summary>
		public static void WriteGeneric<T>(this BinaryWriter writer, T value) {
			GenericBinaryWriter writerFunc;
			if (writers.TryGetValue(typeof(T), out writerFunc))
				writerFunc(writer, value);
			else
				throw new ArgumentException("Unsupported type for " +
					"BinaryWriter.WriteGeneric!");
		}

		/// <summary>Writes the generic integral type.</summary>
		public static void WriteGeneric(this BinaryWriter writer, Type type, object value) {
			GenericBinaryWriter writerFunc;
			if (writers.TryGetValue(type, out writerFunc))
				writerFunc(writer, value);
			else
				throw new ArgumentException("Unsupported type for " +
					"BinaryWriter.WriteGeneric!");
		}

		/// <summary>Writes the Point2I to the stream and advances the current
		/// position.</summary>
		public static void Write(this BinaryWriter writer, Point2I point) {
			writer.Write(point.X);
			writer.Write(point.Y);
		}

		/// <summary>Writes the Rectangle2I to the stream and advances the current
		/// position.</summary>
		public static void Write(this BinaryWriter writer, Rectangle2I rect) {
			writer.Write(rect.X);
			writer.Write(rect.Y);
			writer.Write(rect.Width);
			writer.Write(rect.Height);
		}

		/// <summary>Writes the RangeI to the stream and advances the current
		/// position.</summary>
		public static void Write(this BinaryWriter writer, RangeI range) {
			writer.Write(range.Min);
			writer.Write(range.Max);
		}

		/// <summary>Writes the Vector2F to the stream and advances the current
		/// position.</summary>
		public static void Write(this BinaryWriter writer, Vector2F vector) {
			writer.Write(vector.X);
			writer.Write(vector.Y);
		}

		/// <summary>Writes the Rectangle2F to the stream and advances the current
		/// position.</summary>
		public static void Write(this BinaryWriter writer, Rectangle2F rect) {
			writer.Write(rect.X);
			writer.Write(rect.Y);
			writer.Write(rect.Width);
			writer.Write(rect.Height);
		}

		/// <summary>Writes the RangeF to the stream and advances the current
		/// position.</summary>
		public static void Write(this BinaryWriter writer, RangeF range) {
			writer.Write(range.Min);
			writer.Write(range.Max);
		}

		/// <summary>Writes the Color to the stream and advances the current
		/// position.</summary>
		public static void Write(this BinaryWriter writer, Color color) {
			writer.Write(color.R);
			writer.Write(color.G);
			writer.Write(color.B);
			writer.Write(color.A);
		}
	}
}
