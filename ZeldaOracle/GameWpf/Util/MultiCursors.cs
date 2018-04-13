using WinFormsCursors = System.Windows.Forms.Cursors;
using WpfCursors = System.Windows.Input.Cursors;

namespace ZeldaWpf.Util {
	/// <summary>Defines a set of default cursors for use with WinForms and Wpf.</summary>
	public static class MultiCursors {

		/// <summary>Gets the Cursor that appears when an application is starting.</summary>
		public static MultiCursor AppStarting { get; } = new MultiCursor(
			WinFormsCursors.AppStarting, WpfCursors.AppStarting);

		/// <summary>Gets the Arrow Cursor.</summary>
		public static MultiCursor Arrow { get; } = new MultiCursor(
			WinFormsCursors.Arrow, WpfCursors.Arrow);

		/// <summary>Gets the crosshair Cursor.</summary>
		public static MultiCursor Cross { get; } = new MultiCursor(
			WinFormsCursors.Cross, WpfCursors.Cross);

		/// <summary>Gets a hand Cursor.</summary>
		public static MultiCursor Hand { get; } = new MultiCursor(
			WinFormsCursors.Hand, WpfCursors.Hand);

		/// <summary>Gets a help Cursor which is a combination of an arrow and a
		/// question mark.</summary>
		public static MultiCursor Help { get; } = new MultiCursor(
			WinFormsCursors.Help, WpfCursors.Help);

		/// <summary>Gets an I-beam Cursor, which is used to show where the text
		/// cursor appears when the mouse is clicked.</summary>
		public static MultiCursor IBeam { get; } = new MultiCursor(
			WinFormsCursors.IBeam, WpfCursors.IBeam);

		/// <summary>Gets a Cursor with which indicates that a particular region is
		/// invalid for a given operation.</summary>
		public static MultiCursor No { get; } = new MultiCursor(
			WinFormsCursors.No, WpfCursors.No);

		/// <summary>Gets a four-headed sizing Cursor, which consists of four joined
		/// arrows that point north, south, east, and west.</summary>
		public static MultiCursor SizeAll { get; } = new MultiCursor(
			WinFormsCursors.SizeAll, WpfCursors.SizeAll);

		/// <summary>Gets a two-headed northeast/southwest sizing Cursor.</summary>
		public static MultiCursor SizeNESW { get; } = new MultiCursor(
			WinFormsCursors.SizeNESW, WpfCursors.SizeNESW);

		/// <summary>Gets a two-headed north/south sizing Cursor.</summary>
		public static MultiCursor SizeNS { get; } = new MultiCursor(
			WinFormsCursors.SizeNS, WpfCursors.SizeNS);

		/// <summary>Gets a two-headed northwest/southeast sizing Cursor.</summary>
		public static MultiCursor SizeNWSE { get; } = new MultiCursor(
			WinFormsCursors.SizeNWSE, WpfCursors.SizeNWSE);

		/// <summary>Gets a two-headed west/east sizing Cursor.</summary>
		public static MultiCursor SizeWE { get; } = new MultiCursor(
			WinFormsCursors.SizeWE, WpfCursors.SizeWE);

		/// <summary>Gets an up arrow Cursor, which is typically used to identify an
		/// insertion point.</summary>
		public static MultiCursor UpArrow { get; } = new MultiCursor(
			WinFormsCursors.UpArrow, WpfCursors.UpArrow);

		/// <summary>Specifies a wait (or hourglass) Cursor.</summary>
		public static MultiCursor Wait { get; } = new MultiCursor(
			WinFormsCursors.WaitCursor, WpfCursors.Wait);
	}
}
