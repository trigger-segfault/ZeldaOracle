using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using XnaKeyboard	= Microsoft.Xna.Framework.Input.Keyboard;
using XnaMouse		= Microsoft.Xna.Framework.Input.Mouse;
using XnaGamePad	= Microsoft.Xna.Framework.Input.GamePad;
using XnaButtons	= Microsoft.Xna.Framework.Input.Buttons;
using XnaPlayer		= Microsoft.Xna.Framework.PlayerIndex;
using XnaColor		= Microsoft.Xna.Framework.Color;
using XnaKeys		= Microsoft.Xna.Framework.Input.Keys;

using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Common.Input.Controls;
using Color			= ZeldaOracle.Common.Graphics.Color;
using Keyboard		= ZeldaOracle.Common.Input.Keyboard;
using Mouse			= ZeldaOracle.Common.Input.Mouse;
using GamePad		= ZeldaOracle.Common.Input.GamePad;
using Buttons		= ZeldaOracle.Common.Input.Buttons;
using Keys			= ZeldaOracle.Common.Input.Keys;

namespace ZeldaOracle.Common.Debug {
	/**<summary>The different modes of controlling the debug menu.</summary>*/
	internal enum MenuControlMode {
		Keyboard,
		Mouse,
		GamePad
	}

	/**<summary>The container for all debug menus and menu items.</summary>*/
	public class DebugMenu {

		//========== CONSTANTS ===========
		#region Constants

		private static Color colorBackground			= new Color(40, 40, 40);
		private static Color colorBackgroundHighlight	= new Color(60, 60, 60);
		private static Color colorOutline				= new Color(70, 70, 70);
		private static Color colorText					= Color.White;
		private static Color colorTextHighlight			= Color.White;
		private static Color colorHotkey				= new Color(128, 128, 128);
		private static Color colorArrow					= new Color(160, 160, 160);

		#endregion
		//=========== MEMBERS ============
		#region Members

		/**<summary>The root debug menu.</summary>*/
		private DebugMenuItem menu;
		/**<summary>The path to the current menu item.</summary>*/
		private List<int> currentPath;
		/**<summary>The currently selected menu item.</summary>*/
		private DebugMenuItem currentItem;
		/**<summary>The current menu item being hovered over by the mouse.</summary>*/
		private DebugMenuItem mouseHoverItem;
		/**<summary>True if the mouse is hovering over an item.</summary>*/
		private bool mouseHover;
		/**<summary>The current menu control mode.</summary>*/
		private MenuControlMode controlMode;

		/**<summary>True if the menu is open.</summary>*/
		private bool open;

		/**<summary>The font used for the menu text.</summary>*/
		internal RealFont debugMenuFont;
		/**<summary>The bold font used for the menu text.</summary>*/
		internal RealFont debugMenuFontBold;
		/**<summary>The sprite sheet used for the menu items.</summary>*/
		//internal SpriteAtlas debugMenuSprites;

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		/**<summary>Constructs the main debug menu.</summary>*/
		public DebugMenu() {
			this.menu				= new DebugMenuItem("root");

			this.currentPath		= new List<int>();
			this.currentItem		= null;
			this.mouseHoverItem		= null;
			this.mouseHover			= false;
			this.controlMode		= MenuControlMode.Keyboard;

			this.open				= false;

			this.debugMenuFont		= null;
			this.debugMenuFontBold	= null;
			this.debugMenuSprites	= null;
		}

		public void Open() {
			currentPath.Clear();
			currentPath.Add(0);
			currentItem = menu.Items[0];
			mouseHoverItem = null;
			open = true;
		}
		public void Close() {
			open = false;
		}

		#endregion
		//========== PROPERTIES ==========
		#region Properties

		/**<summary>Gets the root debug menu.</summary>*/
		public DebugMenuItem Root {
			get { return menu; }
		}
		/**<summary>Gets the current menu.</summary>*/
		public DebugMenuItem CurrentMenu {
			get { return currentItem.Root; }
		}
		/**<summary>Gets the current menu item.</summary>*/
		public DebugMenuItem CurrentItem {
			get { return currentItem; }
		}
		/**<summary>Returns true if the menu is open.</summary>*/
		public bool IsOpen {
			get { return open; }
		}

		#endregion
		//=========== UPDATING ===========
		#region Updating

		/**<summary>Updates the debug menu while it's open.</summary>*/
		public void Update() {

			if (Mouse.IsMouseMoved()) {
				if (Mouse.GetDistance().Length > 2.0 && mouseHoverItem != null)
					controlMode = MenuControlMode.Mouse;
			}

			if (Keyboard.IsKeyPressed(Keys.Left) ||
				Keyboard.IsKeyPressed(Keys.Right) ||
				Keyboard.IsKeyPressed(Keys.Up) ||
				Keyboard.IsKeyPressed(Keys.Down) ||
				Keyboard.IsKeyPressed(Keys.X) ||
				Keyboard.IsKeyPressed(Keys.Z) ||
				Keyboard.IsKeyPressed(Keys.Space) ||
				Keyboard.IsKeyPressed(Keys.Enter) ||
				Keyboard.IsKeyPressed(Keys.Escape)) {
				controlMode = MenuControlMode.Keyboard;
			}
			else if (GamePad.IsButtonPressed(Buttons.DPadLeft) ||
					GamePad.IsButtonPressed(Buttons.DPadRight) ||
					GamePad.IsButtonPressed(Buttons.DPadUp) ||
					GamePad.IsButtonPressed(Buttons.DPadDown) ||
					GamePad.IsButtonPressed(Buttons.LeftStickLeft) ||
					GamePad.IsButtonPressed(Buttons.LeftStickRight) ||
					GamePad.IsButtonPressed(Buttons.LeftStickUp) ||
					GamePad.IsButtonPressed(Buttons.LeftStickDown) ||
					GamePad.IsButtonPressed(Buttons.A) ||
					GamePad.IsButtonPressed(Buttons.B) ||
					GamePad.IsButtonPressed(Buttons.Start) ||
					GamePad.IsButtonPressed(Buttons.Back) ||
					GamePad.IsButtonPressed(Buttons.Y)) {
				controlMode = MenuControlMode.GamePad;
			}

			if (controlMode == MenuControlMode.Mouse) {
				UpdateMouseControls();
			}
			else if (controlMode == MenuControlMode.Keyboard) {
				UpdateKeyboardControls();
			}
			else {
				UpdateGamePadControls();
			}
		}
		/**<summary>Updates the debug menu with mouse controls.</summary>*/
		private void UpdateMouseControls() {
			// Open submenus when highlighted.
			if (mouseHoverItem != CurrentMenu && mouseHoverItem != null) {
				currentItem = mouseHoverItem;
				if (mouseHoverItem.Items.Count > 0)
					StepIntoSubMenu();

			}

			if (currentItem != null) {
				// Create path from highlighted item.
				currentPath.Clear();
				ReconstructPath(currentItem);

				// [Left] Click on the item and then close the menu.
				if (Mouse.IsButtonPressed(MouseButtons.Left) && mouseHoverItem != null) {
					if (currentItem.Items.Count == 0) {
						currentItem.Press();
						Close();
					}
				}

				// [Right] Click on the item without closing the menu.
				else if (Mouse.IsButtonPressed(MouseButtons.Right) && mouseHoverItem != null) {
					if (currentItem.Items.Count == 0)
						currentItem.Press();
				}
			}
		}
		/**<summary>Updates the debug menu with keyboard controls.</summary>*/
		private void UpdateKeyboardControls() {
			Keys keyNextItem = Keys.Down;
			Keys keyPrevItem = Keys.Up;
			Keys keyStepInto = Keys.Right;
			Keys keyStepOut  = Keys.Left;

			if (currentItem.Root.Root == null) {
				keyNextItem = Keys.Right;
				keyPrevItem = Keys.Left;
				keyStepInto = Keys.Down;
				keyStepOut  = Keys.Up;
			}

			// Select next root menu item.
			if (Keyboard.IsKeyPressed(Keys.Right) && currentItem.Items.Count == 0 && CurrentMenu != menu) {
				int index = currentPath[0];
				currentItem = menu.Items[index];
				currentPath.Clear();
				currentPath.Add(index);
				SelectNextItem();
				StepIntoSubMenu();
			}

			// Step into a sub-menu.
			else if (Keyboard.IsKeyPressed(keyStepInto))
				StepIntoSubMenu();

			// Select previous root menu item.
			if (Keyboard.IsKeyPressed(Keys.Left) && CurrentMenu.Root == menu) {
				currentItem = CurrentMenu;
				currentPath.RemoveAt(currentPath.Count - 1);
				SelectPreviousItem();
				StepIntoSubMenu();
			}
			// Step out of a sub-menu.
			else if (Keyboard.IsKeyPressed(keyStepOut))
				StepOutOfSubMenu();

			// Select next item.
			if (Keyboard.IsKeyPressed(keyNextItem))
				SelectNextItem();

			// Select previous item.
			if (Keyboard.IsKeyPressed(keyPrevItem))
				SelectPreviousItem();

			// Step out of a sub-menu.
			if (Keyboard.IsKeyPressed(Keys.X) || Keyboard.IsKeyPressed(Keys.Escape))
				StepOutOfSubMenu();

			// Press item.
			if (Keyboard.IsKeyPressed(Keys.Enter) || Keyboard.IsKeyPressed(Keys.Z) || Keyboard.IsKeyPressed(Keys.Space)) {
				if (currentItem.Items.Count == 0) {
					currentItem.Press();
					if (!Keyboard.IsKeyPressed(Keys.Space))
						Close();
				}
				else
					StepIntoSubMenu();
			}
		}
		/**<summary>Updates the debug menu with gamepad controls.</summary>*/
		private void UpdateGamePadControls() {
			/*Keys keyNextItem = Keys.Down;
			Keys keyPrevItem = Keys.Up;
			Keys keyStepInto = Keys.Right;
			Keys keyStepOut  = Keys.Left;*/

			Buttons buttonNextItem	= Buttons.DPadDown;
			Buttons buttonNextItem2	= Buttons.LeftStickDown;
			Buttons buttonPrevItem	= Buttons.DPadUp;
			Buttons buttonPrevItem2	= Buttons.LeftStickUp;
			Buttons buttonStepInto	= Buttons.DPadRight;
			Buttons buttonStepInto2	= Buttons.LeftStickRight;
			Buttons buttonStepOut	= Buttons.DPadLeft;
			Buttons buttonStepOut2	= Buttons.LeftStickLeft;

			if (currentItem.Root.Root == null) {
				/*keyNextItem = Keys.Right;
				keyPrevItem = Keys.Left;
				keyStepInto = Keys.Down;
				keyStepOut  = Keys.Up;*/
				buttonNextItem	= Buttons.DPadRight;
				buttonNextItem2	= Buttons.LeftStickRight;
				buttonPrevItem	= Buttons.DPadLeft;
				buttonPrevItem2	= Buttons.LeftStickLeft;
				buttonStepInto	= Buttons.DPadDown;
				buttonStepInto2	= Buttons.LeftStickDown;
				buttonStepOut	= Buttons.DPadUp;
				buttonStepOut2	= Buttons.LeftStickUp;
			}

			// Select next root menu item.
			if ((GamePad.IsButtonPressed(Buttons.DPadRight) || GamePad.IsButtonPressed(Buttons.LeftStickRight)) && currentItem.Items.Count == 0 && CurrentMenu != menu) {
				int index = currentPath[0];
				currentItem = menu.Items[index];
				currentPath.Clear();
				currentPath.Add(index);
				SelectNextItem();
				StepIntoSubMenu();
			}

			// Step into a sub-menu.
			else if (GamePad.IsButtonPressed(buttonStepInto) || GamePad.IsButtonPressed(buttonStepInto2))
				StepIntoSubMenu();

			// Select previous root menu item.
			if ((GamePad.IsButtonPressed(Buttons.DPadLeft) || GamePad.IsButtonPressed(Buttons.LeftStickLeft)) && CurrentMenu.Root == menu) {
				currentItem = CurrentMenu;
				currentPath.RemoveAt(currentPath.Count - 1);
				SelectPreviousItem();
				StepIntoSubMenu();
			}
			// Step out of a sub-menu.
			else if (GamePad.IsButtonPressed(buttonStepOut) || GamePad.IsButtonPressed(buttonStepOut2))
				StepOutOfSubMenu();

			// Select next item.
			if (GamePad.IsButtonPressed(buttonNextItem) || GamePad.IsButtonPressed(buttonNextItem2))
				SelectNextItem();

			// Select previous item.
			if (GamePad.IsButtonPressed(buttonPrevItem) || GamePad.IsButtonPressed(buttonPrevItem2))
				SelectPreviousItem();

			// Step out of a sub-menu.
			if (GamePad.IsButtonPressed(Buttons.B) || GamePad.IsButtonPressed(Buttons.Back))
				StepOutOfSubMenu();

			// Press item.
			if (GamePad.IsButtonPressed(Buttons.A) || GamePad.IsButtonPressed(Buttons.Start) || GamePad.IsButtonPressed(Buttons.Y)) {
				if (currentItem.Items.Count == 0) {
					currentItem.Press();
					if (!GamePad.IsButtonPressed(Buttons.Y))
						Close();
				}
				else {
					StepIntoSubMenu();
				}
			}
		}

		#endregion
		//============ MENUS =============
		#region Menus
	
		/**<summary>Checks for pressed hotkeys.</summary>*/
		public void CheckHotkeys() {
			CheckHotkeys(menu);
		}
		/**<summary>Checks for pressed hotkeys.</summary>*/
		public void CheckHotkeys(DebugMenuItem item) {
			for (int i = 0; i < item.Items.Count; ++i) {
				if (item.Items[i].HotKey.Pressed())
					item.Items[i].Press();
				CheckHotkeys(item.Items[i]);
			}
		}
		/**<summary>Selects the next menu item.</summary>*/
		private void SelectNextItem() {
			int n = currentItem.Root.Items.Count;

			currentPath[currentPath.Count - 1] = (currentPath[currentPath.Count - 1] + 1) % n;
			currentItem = CurrentMenu.Items[currentPath[currentPath.Count - 1]];
		}
		/**<summary>Selects the previous menu item.</summary>*/
		private void SelectPreviousItem() {
			int n = currentItem.Root.Items.Count;
			currentPath[currentPath.Count - 1] = (currentPath[currentPath.Count - 1] + n - 1) % n;
			currentItem = CurrentMenu.Items[currentPath[currentPath.Count - 1]];
		}
		/**<summary>Steps into the sub menu of the menu item.</summary>*/
		private void StepIntoSubMenu() {
			if (currentItem.Items.Count > 0) {
				currentItem = currentItem.Items[0];
				currentPath.Add(0);
			}
		}
		/**<summary>Steps out of the sub menu of the menu item.</summary>*/
		private void StepOutOfSubMenu() {
			if (CurrentMenu.Root != null) {
				currentItem = CurrentMenu;
				currentPath.RemoveAt(currentPath.Count - 1);
			}
		}
		/**<summary>Reconstructs the menu item path.</summary>*/
		private void ReconstructPath(DebugMenuItem item) {
			DebugMenuItem menu = item.Root;
			if (menu == null)
				return;

			for (int i = 0; i < menu.Items.Count; ++i) {
				DebugMenuItem subItem = menu.Items[i];

				if (subItem == item) {
					currentPath.Insert(0, i);
					ReconstructPath(menu);
				}
			}
		}

		#endregion
		//=========== DRAWING ============
		#region Drawing

		/**<summary>Draws the debug menus.</summary>*/
		public void Draw(Graphics2D g) {
			mouseHover = false;

			DrawMenu(g, menu, 0, Point2I.Zero);

			if (!mouseHover) {
				mouseHoverItem = null;
			}
		}
		/**<summary>Draws the debug menu.</summary>*/
		private void DrawMenu(Graphics2D g, DebugMenuItem item, int pathIndex, Point2I position) {

			if (pathIndex >= currentPath.Count)
				return;

			int itemWidth  = 32;
			int itemHeight = 28;
			int offset = 32;
			int textOffset   = 0;
			int hotkeyOffset = 0;
			int hotkeyColumnWidth = 0;
			int textColumnWidth = 0;
			int rightPading = 24;
			//int padding = 20;
			int hotkeyColumnPadding = 20;
			if (pathIndex == 0) {
				offset = 6;
				rightPading = 8;
				hotkeyColumnPadding = 0;
			}

			// Measure the width to draw the menu at.
			for (int i = 0; i < item.Items.Count; ++i) {
				DebugMenuItem subItem = item.Items[i];

				Rectangle2I r1 = (Rectangle2I)debugMenuFont.MeasureStringBounds(subItem.Text, Align.Left);
				Rectangle2I r2 = (Rectangle2I)debugMenuFont.MeasureStringBounds(subItem.HotKey.Name, Align.Left);
				hotkeyOffset = GMath.Max(hotkeyOffset, r1.Width + offset + 10);
				itemWidth = GMath.Max(itemWidth, r1.Width + r2.Width + offset + rightPading);
				textColumnWidth   = GMath.Max(textColumnWidth, r1.Width);
				hotkeyColumnWidth = GMath.Max(hotkeyColumnWidth, r2.Width);
			}
			hotkeyOffset = offset + textColumnWidth + hotkeyColumnPadding;
			textOffset = offset;
			itemWidth = offset + textColumnWidth + hotkeyColumnPadding + hotkeyColumnWidth + rightPading;


			// Draw outline.
			Rectangle2I menuRect = new Rectangle2I(position.X, position.Y, itemWidth, itemHeight * item.Items.Count);
			if (pathIndex == 0) {
				menuRect.Width = itemWidth * item.Items.Count;
				menuRect.Height = itemHeight;
			}
			g.DrawRectangle(menuRect, 1.0f, colorOutline);

			// Draw background.
			menuRect.Inflate(-1, -1);
			g.FillRectangle(menuRect, colorBackground);

			// Draw item list.
			for (int i = 0; i < item.Items.Count; ++i) {
				Rectangle2I r = new Rectangle2I(position.X, position.Y, itemWidth, itemHeight);

				if (pathIndex == 0) {
					r.Inflate(0, -1);
					if (i == 0) {
						r.X += 1;
						r.Width -= 1;
					}
					if (i == item.Items.Count - 1)
						r.Width -= 1;
				}
				else {
					r.Inflate(-1, 0);
					if (i == 0) {
						r.Y += 1;
						r.Height -= 1;
					}
					if (i == item.Items.Count - 1)
						r.Height -= 1;
				}

				DebugMenuItem subItem = item.Items[i];

				// Draw highlight.
				if (currentPath[pathIndex] == i) {
					Rectangle2F sr = (Rectangle2I)r;
					sr.Inflate(-2, -2);
					if (controlMode == MenuControlMode.Keyboard || controlMode == MenuControlMode.GamePad || pathIndex < currentPath.Count - 1)
						g.FillRectangle(sr, colorBackgroundHighlight);
				}
				Point2I ms = (Point2I)Mouse.GetPosition();
				if (r.Contains(ms)) {
					mouseHover = true;
					mouseHoverItem = subItem;
					Rectangle2F sr = (Rectangle2I)r;
					sr.Inflate(-2, -2);
					if (controlMode == MenuControlMode.Mouse)
						g.FillRectangle(sr, colorBackgroundHighlight);
				}

				// Draw text label.
				string text   = subItem.Text;
				string hotkey = subItem.HotKey.Name;
				g.DrawRealString(debugMenuFont, text, new Point2I(r.Min.X + textOffset, (int)r.Center.Y), Align.Left | Align.Int, colorText);
				g.DrawRealString(debugMenuFont, hotkey, new Point2I(r.Min.X + hotkeyOffset, (int)r.Center.Y), Align.Left | Align.Int, colorHotkey);

				// Draw toggle check.
				if (subItem is ToggleMenuItem) {
					bool enabled = ((ToggleMenuItem)subItem).IsEnabled;
					SpriteEx spr = debugMenuSprites["checkbox_disabled"];
					if (enabled)
						spr = debugMenuSprites["checkbox_enabled"];
					if (subItem is RadioButtonMenuItem) {
						spr = debugMenuSprites["radiobutton_disabled"];
						if (enabled)
							spr = debugMenuSprites["radiobutton_enabled"];
					}
					g.DrawSpriteEx(spr, new Vector2F(r.Min.X + 6, r.Min.Y + 6), colorText);
				}

				// Draw submenu arrow.
				if (item != menu && subItem.Items.Count > 0) {
					g.DrawSpriteEx(debugMenuSprites["submenu_arrow"], new Vector2F(r.Max.X - 18, r.Min.Y + 6), colorArrow);
				}

				// Draw nested menu.
				if (currentPath[pathIndex] == i) {
					Point2I p = position;
					if (pathIndex == 0)
						p.Y += itemHeight - 1;
					else
						p.X += itemWidth - 1;

					DrawMenu(g, subItem, pathIndex + 1, p);
				}

				// Move current position.
				if (pathIndex == 0)
					position.X += itemWidth;
				else
					position.Y += itemHeight;
			}
		}

		#endregion
	}
} // end namespace
