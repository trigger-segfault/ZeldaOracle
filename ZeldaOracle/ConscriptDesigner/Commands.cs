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

		/// <summary>The command to close the current project.</summary>
		public static readonly RoutedUICommand Close = new RoutedUICommand(
			"Close", "Close Project", typeof(Commands));

		/// <summary>The command to save all files open in the designer.</summary>
		public static readonly RoutedUICommand SaveAll = new RoutedUICommand(
			"SaveAll", "Save All", typeof(Commands),
			new InputGestureCollection() {
				new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift) });

		/// <summary>The command to exit the designer.</summary>
		public static readonly RoutedUICommand Exit = new RoutedUICommand(
			"Exit", "Exit", typeof(Commands),
			new InputGestureCollection() {
				new KeyGesture(Key.W, ModifierKeys.Control) });


		//-----------------------------------------------------------------------------
		// Edit
		//-----------------------------------------------------------------------------

		/// <summary>The secondary keybinding for the redo command.</summary>
		public static readonly RoutedUICommand RedoSecondary = new RoutedUICommand(
			"RedoSecondary", "Redo", typeof(Commands),
			new InputGestureCollection() {
				new KeyGesture(Key.Z, ModifierKeys.Control | ModifierKeys.Shift) });

		/// <summary>The command to goto a specific line in a text editor.</summary>
		public static readonly RoutedUICommand GotoLine = new RoutedUICommand(
			"GotoLine", "Go To Line", typeof(Commands),
			new InputGestureCollection() {
				new KeyGesture(Key.G, ModifierKeys.Control) });


		//-----------------------------------------------------------------------------
		// View
		//-----------------------------------------------------------------------------

		/// <summary>The command to toggle the project explorer window.</summary>
		public static readonly RoutedUICommand ProjectExplorer = new RoutedUICommand(
			"ProjectExplorer", "Project Explorer", typeof(Commands));

		/// <summary>The command to toggle the output console window.</summary>
		public static readonly RoutedUICommand OutputConsole = new RoutedUICommand(
			"OutputConsole", "Output Console", typeof(Commands));

		/// <summary>The command to toggle the sprite browser window.</summary>
		public static readonly RoutedUICommand SpriteBrowser = new RoutedUICommand(
			"SpriteBrowser", "Sprite Browser", typeof(Commands));

		/// <summary>The command to toggle the sprite sourcebrowser window.</summary>
		public static readonly RoutedUICommand SpriteSourceBrowser = new RoutedUICommand(
			"SpriteSourceBrowser", "Sprite Source Browser", typeof(Commands));

		/// <summary>The command to toggle the tile browser window.</summary>
		public static readonly RoutedUICommand TileBrowser = new RoutedUICommand(
			"TileBrowser", "Tile Browser", typeof(Commands));


		//-----------------------------------------------------------------------------
		// Project
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

		/// <summary>The command to cancel builds.</summary>
		public static readonly RoutedUICommand CancelBuild = new RoutedUICommand(
			"CancelBuild", "Cancel Build", typeof(Commands),
			new InputGestureCollection() {
				new KeyGesture(Key.Cancel, ModifierKeys.Control) });
	}
}
