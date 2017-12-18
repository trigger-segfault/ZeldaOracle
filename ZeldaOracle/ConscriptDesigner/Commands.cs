using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ConscriptDesigner {
	public static class Commands {

		//-----------------------------------------------------------------------------
		// General
		//-----------------------------------------------------------------------------

		/// <summary>The command to exit the designer.</summary>
		public static readonly RoutedUICommand Exit = new RoutedUICommand(
			"Exit", "Exit", typeof(Commands),
			new InputGestureCollection() {
				new KeyGesture(Key.W, ModifierKeys.Control) });

		/// <summary>The command to save all files open in the designer.</summary>
		public static readonly RoutedUICommand SaveAll = new RoutedUICommand(
			"SaveAll", "Save All", typeof(Commands),
			new InputGestureCollection() {
				new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift) });

		/// <summary>The secondary keybinding for the redo command.</summary>
		public static readonly RoutedUICommand RedoSecondary = new RoutedUICommand(
			"RedoSecondary", "Redo", typeof(Commands),
			new InputGestureCollection() {
				new KeyGesture(Key.Z, ModifierKeys.Control | ModifierKeys.Shift) });

		
		//-----------------------------------------------------------------------------
		// World
		//-----------------------------------------------------------------------------

		/// <summary>The command to test the current conscripts.</summary>
		public static readonly RoutedUICommand RunConscripts = new RoutedUICommand(
			"RunConscripts", "Run Conscripts", typeof(Commands),
			new InputGestureCollection() {
				new KeyGesture(Key.F5) });

		/// <summary>The command to recompile the image and sound files.</summary>
		public static readonly RoutedUICommand CompileContent = new RoutedUICommand(
			"CompileContent", "Compile Content", typeof(Commands),
			new InputGestureCollection() {
				new KeyGesture(Key.F5, ModifierKeys.Shift) });
	}
}
