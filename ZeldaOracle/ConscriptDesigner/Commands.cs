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

		/// <summary>The command to escape-close a window.</summary>
		public static readonly RoutedUICommand EscapeClose = new RoutedUICommand(
			"EscapeClose", "Close Window", typeof(Commands),
			new InputGestureCollection() {
				new KeyGesture(Key.Escape) });


		//-----------------------------------------------------------------------------
		// Launching
		//-----------------------------------------------------------------------------

		/// <summary>The command to launch the game.</summary>
		public static readonly RoutedUICommand LaunchGame = new RoutedUICommand(
			"LaunchGame", "Launch Game", typeof(Commands));

		/// <summary>The command to launch the editor.</summary>
		public static readonly RoutedUICommand LaunchEditor = new RoutedUICommand(
			"LaunchEditor", "Launch Editor", typeof(Commands));


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

		/// <summary>The command to find the next match with the find and replace window.</summary>
		public static readonly RoutedUICommand FindNext = new RoutedUICommand(
			"FindNext", "Find Next", typeof(Commands),
			new InputGestureCollection() {
				new KeyGesture(Key.F3) });

		/// <summary>The command to find the next match or replace the current match with the find and replace window.</summary>
		public static readonly RoutedUICommand ReplaceNext = new RoutedUICommand(
			"ReplaceNext", "Replace Next", typeof(Commands),
			new InputGestureCollection() {
				new KeyGesture(Key.R, ModifierKeys.Alt) });

		/// <summary>The command to replace all matches with the find and replace window.</summary>
		public static readonly RoutedUICommand ReplaceAll = new RoutedUICommand(
			"ReplaceAll", "Replace All", typeof(Commands),
			new InputGestureCollection() {
				new KeyGesture(Key.A, ModifierKeys.Alt) });


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

		/// <summary>The command to toggle the sprite source browser window.</summary>
		public static readonly RoutedUICommand SpriteSourceBrowser = new RoutedUICommand(
			"SpriteSourceBrowser", "Sprite Source Browser", typeof(Commands));

		/// <summary>The command to toggle the style browser window.</summary>
		public static readonly RoutedUICommand StyleBrowser = new RoutedUICommand(
			"StyleBrowser", "Style Browser", typeof(Commands));

		/// <summary>The command to toggle the tile browser window.</summary>
		public static readonly RoutedUICommand TileDataBrowser = new RoutedUICommand(
			"TileDataBrowser", "Tile Data Browser", typeof(Commands));

		/// <summary>The command to toggle the tileset browser window.</summary>
		public static readonly RoutedUICommand TilesetBrowser = new RoutedUICommand(
			"TilesetBrowser", "Tileset Browser", typeof(Commands));


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

		/// <summary>The command to view the location of the last error.</summary>
		public static readonly RoutedUICommand GotoError = new RoutedUICommand(
			"GotoError", "Go To Error", typeof(Commands),
			new InputGestureCollection() {
				new KeyGesture(Key.F8) });
	}
}
