using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ZeldaEditor {
	/**<summary>A collection of commands for use in the editor window.</summary>*/
	public static class EditorCommands {
		//=========== GENERAL ============
		#region Gemeral

		/**<summary>The command to exit the editor.</summary>*/
		public static readonly RoutedUICommand Exit = new RoutedUICommand(
			"Exit", "Exit", typeof(EditorCommands),
			new InputGestureCollection() {
				new KeyGesture(Key.W, ModifierKeys.Control) });

		/**<summary>The command to save the world as with a key binding.</summary>*/
		public static readonly RoutedUICommand SaveAs = new RoutedUICommand(
			"SaveAs", "Save As", typeof(EditorCommands),
			new InputGestureCollection() {
				new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift) });

		/**<summary>The command to close the current world.</summary>*/
		public static readonly RoutedUICommand Close = new RoutedUICommand(
			"Close", "Close", typeof(EditorCommands));

		#endregion
		//============ WORLD =============
		#region World

		/**<summary>The command to test the current world.</summary>*/
		public static readonly RoutedUICommand TestWorld = new RoutedUICommand(
			"TestWorld", "Test World", typeof(EditorCommands),
			new InputGestureCollection() {
				new KeyGesture(Key.F5) });

		/**<summary>The command to test the current world at the specified location.</summary>*/
		public static readonly RoutedUICommand TestWorldFromLocation = new RoutedUICommand(
			"TestWorldFromLocation", "Test World From Location", typeof(EditorCommands),
			new InputGestureCollection() {
				new KeyGesture(Key.F5, ModifierKeys.Shift) });

		/**<summary>The command to add a new level.</summary>*/
		public static readonly RoutedUICommand AddNewLevel = new RoutedUICommand(
			"AddNewLevel", "Add New Level", typeof(EditorCommands),
			new InputGestureCollection() {
				new KeyGesture(Key.L, ModifierKeys.Control | ModifierKeys.Shift) });

		/**<summary>The command to add a new dungeon.</summary>*/
		public static readonly RoutedUICommand AddNewDungeon = new RoutedUICommand(
			"AddNewDungeon", "Add New Dungeon", typeof(EditorCommands),
			new InputGestureCollection() {
				new KeyGesture(Key.D, ModifierKeys.Control | ModifierKeys.Shift) });

		/**<summary>The command to add a new script.</summary>*/
		public static readonly RoutedUICommand AddNewScript = new RoutedUICommand(
			"AddNewScript", "Add New Script", typeof(EditorCommands),
			new InputGestureCollection() {
				new KeyGesture(Key.X, ModifierKeys.Control | ModifierKeys.Shift) });

		/**<summary>The command to show the tile properties.</summary>*/
		public static readonly RoutedUICommand TileProperties = new RoutedUICommand(
			"TileProperties", "Tile Properties", typeof(EditorCommands),
			new InputGestureCollection() {
				new KeyGesture(Key.F4) });

		/**<summary>The command to show the room properties.</summary>*/
		public static readonly RoutedUICommand RoomProperties = new RoutedUICommand(
			"RoomProperties", "Room Properties", typeof(EditorCommands),
			new InputGestureCollection() {
				new KeyGesture(Key.F4, ModifierKeys.Shift) });

		/**<summary>The command to resize the current level.</summary>*/
		public static readonly RoutedUICommand ResizeLevel = new RoutedUICommand(
			"ResizeLevel", "Resize Level", typeof(EditorCommands));

		/**<summary>The command to shift the current level.</summary>*/
		public static readonly RoutedUICommand ShiftLevel = new RoutedUICommand(
			"ShiftLevel", "Shift Level", typeof(EditorCommands));

		#endregion
		//============ TOOLS =============
		#region Tools

		/**<summary>The command to switch to the pointer tool.</summary>*/
		public static readonly RoutedUICommand PointerTool = new RoutedUICommand(
			"PointerTool", "Pointer Tool", typeof(EditorCommands));

		/**<summary>The command to switch to the place tool.</summary>*/
		public static readonly RoutedUICommand PlaceTool = new RoutedUICommand(
			"PlaceTool", "Place Tool", typeof(EditorCommands));

		/**<summary>The command to switch to the square tool.</summary>*/
		public static readonly RoutedUICommand SquareTool = new RoutedUICommand(
			"SquareTool", "Square Tool", typeof(EditorCommands));

		/**<summary>The command to switch to the fill tool.</summary>*/
		public static readonly RoutedUICommand FillTool = new RoutedUICommand(
			"FillTool", "Fill Tool", typeof(EditorCommands));

		/**<summary>The command to switch to the select tool.</summary>*/
		public static readonly RoutedUICommand SelectTool = new RoutedUICommand(
			"SelectTool", "Select Tool", typeof(EditorCommands));

		/**<summary>The command to switch to the eyedropper tool.</summary>*/
		public static readonly RoutedUICommand EyedropperTool = new RoutedUICommand(
			"EyedropperTool", "Eyedropper Tool", typeof(EditorCommands));

		/**<summary>The command to deselect the current selection.</summary>*/
		public static readonly RoutedUICommand Deselect = new RoutedUICommand(
			"Deselect", "Deselect", typeof(EditorCommands),
			new InputGestureCollection() {
				new KeyGesture(Key.D, ModifierKeys.Control) });

		#endregion
		//============= VIEW =============
		#region View

		/**<summary>The command to toggle the grid visibility.</summary>*/
		public static readonly RoutedUICommand ShowGrid = new RoutedUICommand(
			"ShowGrid", "Show Grid", typeof(EditorCommands),
			new InputGestureCollection() {
				new KeyGesture(Key.G, ModifierKeys.Control) });

		/**<summary>The command to toggle animations.</summary>*/
		public static readonly RoutedUICommand PlayAnimations = new RoutedUICommand(
			"PlayAnimations", "Play Animations", typeof(EditorCommands));

		/**<summary>The command to cycle a layer up in the level.</summary>*/
		public static readonly RoutedUICommand CycleLayerUp = new RoutedUICommand(
			"CycleLayerUp", "Cycle Layer Up", typeof(EditorCommands),
			new InputGestureCollection() {
				new KeyGesture(Key.PageUp) });

		/**<summary>The command to cycle a layer down in the level.</summary>*/
		public static readonly RoutedUICommand CycleLayerDown = new RoutedUICommand(
			"CycleLayerDown", "Cycle Layer Down", typeof(EditorCommands),
			new InputGestureCollection() {
				new KeyGesture(Key.PageDown) });

		#endregion
	}
}
