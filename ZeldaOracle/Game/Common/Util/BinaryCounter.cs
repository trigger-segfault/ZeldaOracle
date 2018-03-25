using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Common.Util {
	/// <summary>A helper class for saving the stream position to write a count or
	/// file size and then writing and returning the current position.<para/>
	/// No generic argument will default to int.</summary>
	public class BinaryCounter : BinaryCounter<int> {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a new binary counter without setting the start
		/// position.</summary>
		public BinaryCounter(Stream stream)
			: this(new BinaryWriter(stream), 0)
		{
		}

		/// <summary>Constructs a new binary counter without setting the start
		/// position.</summary>
		public BinaryCounter(BinaryWriter writer)
			: this(writer, 0)
		{
		}

		/// <summary>Constructs a new binary counter without setting the start
		/// position but setting an initial count.</summary>
		public BinaryCounter(Stream stream, int initialCount)
			: this(new BinaryWriter(stream), initialCount)
		{
		}

		/// <summary>Constructs a new binary counter without setting the start
		/// position but setting an initial count.</summary>
		public BinaryCounter(BinaryWriter writer, int initialCount)
			: base(writer, initialCount)
		{
		}


		//-----------------------------------------------------------------------------
		// Static Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Starts a new binary counter.</summary>
		public new static BinaryCounter Start(Stream stream) {
			return Start(new BinaryWriter(stream));
		}

		/// <summary>Starts a new binary counter.</summary>
		public new static BinaryCounter Start(BinaryWriter writer) {
			BinaryCounter countWriter = new BinaryCounter(writer);
			countWriter.SetStart();
			return countWriter;
		}

		/// <summary>Starts a new binary counter with the initial count.</summary>
		public new static BinaryCounter Start(Stream stream, int initialCount) {
			return Start(new BinaryWriter(stream), initialCount);
		}

		/// <summary>Starts a new binary counter with the initial count.</summary>
		public new static BinaryCounter Start(BinaryWriter writer, int initialCount) {
			BinaryCounter countWriter = new BinaryCounter(writer, initialCount);
			countWriter.SetStart();
			return countWriter;
		}
	}

	/// <summary>A helper class for saving the stream position to write a count or
	/// file size and then writing and returning the current position.<para/>
	/// No generic argument will default to int.</summary>
	public class BinaryCounter<T> {

		/// <summary>The value identifying a position as unset.</summary>
		public const long UnsetPosition = -1;


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The binary writer for writing the value.</summary>
		private BinaryWriter writer;
		/// <summary>The start position for navigating and tracking file size.</summary>
		private long startPosition;
		/// <summary>The end position for returning and tracking file size.</summary>
		private long endPosition;
		/// <summary>The count to keep track of.</summary>
		private T count;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a new binary counter without setting the start
		/// position.</summary>
		public BinaryCounter(Stream stream)
			: this(new BinaryWriter(stream), default(T))
		{
		}

		/// <summary>Constructs a new binary counter without setting the start
		/// position.</summary>
		public BinaryCounter(BinaryWriter writer)
			: this(writer, default(T))
		{
		}

		/// <summary>Constructs a new binary counter without setting the start
		/// position but setting an initial count.</summary>
		public BinaryCounter(Stream stream, T initialCount)
			: this(new BinaryWriter(stream), initialCount)
		{
		}

		/// <summary>Constructs a new binary counter without setting the start
		/// position but setting an initial count.</summary>
		public BinaryCounter(BinaryWriter writer, T initialCount) {
			if (!BinaryExtensions.ContainsWriter<T>())
				throw new ArithmeticException("Unsupported type for " +
					"BinaryWriter.WriteGeneric!");
			this.writer = writer;
			startPosition = -1;
			endPosition = -1;
			count = initialCount;
		}


		//-----------------------------------------------------------------------------
		// Static Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Starts a new binary counter.</summary>
		public static BinaryCounter<T> Start(Stream stream) {
			return Start(new BinaryWriter(stream));
		}

		/// <summary>Starts a new binary counter.</summary>
		public static BinaryCounter<T> Start(BinaryWriter writer) {
			BinaryCounter<T> countWriter = new BinaryCounter<T>(writer);
			countWriter.SetStart();
			return countWriter;
		}

		/// <summary>Starts a new binary counter with the initial count.</summary>
		public static BinaryCounter<T> Start(Stream stream, T initialCount) {
			return Start(new BinaryWriter(stream), initialCount);
		}

		/// <summary>Starts a new binary counter with the initial count.</summary>
		public static BinaryCounter<T> Start(BinaryWriter writer, T initialCount) {
			BinaryCounter<T> countWriter = new BinaryCounter<T>(writer, initialCount);
			countWriter.SetStart();
			return countWriter;
		}


		//-----------------------------------------------------------------------------
		// Static Reader Constructors
		//-----------------------------------------------------------------------------
		
		/// <summary>Returns a memory stream with the read size.</summary>
		public static MemoryStream ReadStream(Stream stream) {
			return ReadStream(new BinaryReader(stream));
		}

		/// <summary>Returns a memory stream with the read size.</summary>
		public static MemoryStream ReadStream(BinaryReader reader) {
			if (!typeof(T).IsInteger())
				throw new ArgumentException("Unsupported type for for " +
					"BinaryCounter.ReadStream!");
			
			int size = Convert.ToInt32(reader.ReadGeneric<T>());
			if (size < 0)
				throw new IOException("Size cannot be less than zero!");
			return new MemoryStream(reader.ReadBytes(size));
		}

		/// <summary>Returns a memory stream with the read size.</summary>
		public static byte[] ReadBytes(Stream stream) {
			return ReadBytes(new BinaryReader(stream));
		}

		/// <summary>Returns a memory stream with the read size.</summary>
		public static byte[] ReadBytes(BinaryReader reader) {
			if (!typeof(T).IsInteger())
				throw new ArgumentException("Unsupported type for for " +
					"BinaryCounter.ReadBytes!");
			int size = Convert.ToInt32(reader.ReadGeneric<T>());
			if (size < 0)
				throw new IOException("Size cannot be less than zero!");
			return reader.ReadBytes(size);
		}


		//-----------------------------------------------------------------------------
		// Setup
		//-----------------------------------------------------------------------------

		/// <summary>Sets the start position for the count and then increments the
		/// stream position by the value size.</summary>
		public void SetStart() {
			startPosition = writer.BaseStream.Position;
			writer.BaseStream.Position += ValueSize;
		}

		/// <summary>Sets the end position in the stream that will be returned to.
		/// This is done automatically in WriteAndReturn if the end position is unset.</summary>
		public void SetEnd() {
			endPosition = writer.BaseStream.Position;
		}

		/// <summary>Sets the start position for the count and then increments the
		/// stream position by the value size. Also unsets the end position.</summary>
		public void Restart() {
			SetStart();
			endPosition = UnsetPosition;
			count = default(T);
		}


		//-----------------------------------------------------------------------------
		// Writing
		//-----------------------------------------------------------------------------
		
		/// <summary>Writes the accumulated size and returns to the end position.
		/// Automatically sets the end position if unset.</summary>
		public void WriteSizeAndReturn() {
			if (!IsEndPositionSet)
				SetEnd();
			WriteAndReturn(Size);
		}

		/// <summary>Writes the stored count and returns to the end position.
		/// Automatically sets the end position if unset.</summary>
		public void WriteCountAndReturn() {
			WriteAndReturn(count);
		}

		/// <summary>Writes the value and returns to the end position.
		/// Automatically sets the end position if unset.</summary>
		public void WriteAndReturn(T value) {
			GotoStart();
			writer.WriteGeneric<T>(value);
			GotoEnd();
		}


		//-----------------------------------------------------------------------------
		// Navigation
		//-----------------------------------------------------------------------------

		/// <summary>Navigates to the start position and sets the end position if
		/// it is unset.</summary>
		public void GotoStart(bool setEnd = true) {
			if (!IsStartPositionSet)
				throw new InvalidOperationException("Cannot goto start " +
					"when start position is unset!");
			if (!IsEndPositionSet && setEnd)
				SetEnd();
			writer.BaseStream.Position = startPosition;
		}

		/// <summary>Navigates to the end position.</summary>
		public void GotoEnd() {
			if (!IsEndPositionSet)
				throw new InvalidOperationException("Cannot goto end " +
					"when end position is unset!");
			writer.BaseStream.Position = endPosition;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the value size of the type.</summary>
		public int ValueSize {
			get { return Marshal.SizeOf<T>(); }
		}

		/// <summary>Gets if the start position has been set.</summary>
		public bool IsStartPositionSet {
			get { return startPosition != UnsetPosition; }
		}

		/// <summary>Gets if the end position has been set.</summary>
		public bool IsEndPositionSet {
			get { return endPosition != UnsetPosition; }
		}

		/// <summary>Gets or sets the start position before the value is written.</summary> 
		public long StartPosition {
			get { return startPosition; }
			set { startPosition = value; }
		}

		/// <summary>Gets or sets the end position to return to.</summary> 
		public long EndPosition {
			get { return endPosition; }
			set { endPosition = value; }
		}

		/// <summary>Gets the size as the numeric type T.</summary>
		public T Size {
			get { return (T) Convert.ChangeType(endPosition - ValueSize - startPosition, typeof(T)); }
		}

		/// <summary>Gets or sets the stored count value.</summary>
		public T Count {
			get { return count; }
			set { count = value; }
		}
	}
}
