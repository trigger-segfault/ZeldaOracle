using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using XnaKeyboard	= Microsoft.Xna.Framework.Input.Keyboard;
using XnaKeys		= Microsoft.Xna.Framework.Input.Keys;

using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Common.Input.Controls;
using Keyboard		= ZeldaOracle.Common.Input.Keyboard;
using Keys			= ZeldaOracle.Common.Input.Keys;

namespace ZeldaOracle.Common.Debug {
	/// <summary>The menu item class used for the debug menu.</summary>
	public class DebugMenuItem {

		//========== CONSTANTS ===========
		#region Constants

		#endregion
		//========== DELEGATES ===========
		#region Delegates

		/// <summary>The delegate called when the menu item is clicked.</summary>
		public delegate void MenuItemAction();

		#endregion
		//=========== MEMBERS ============
		#region Members

		/// <summary>The root debug menu.</summary>
		private DebugMenuItem root;

		/// <summary>The text of the menu item.</summary>
		private string text;
		/// <summary>The index of the menu item.</summary>
		private int index;
		/// <summary>The hotkey used by the menu item.</summary>
		private HotKey hotkey;
		/// <summary>The action called when the menu item is clicked.</summary>
		private MenuItemAction action;
		/// <summary>The list of menu items in this menu item.</summary>
		private List<DebugMenuItem> items;

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		/// <summary>Constructs a menu item with the specified text.</summary>
		public DebugMenuItem(string text) {
			this.root			= null;

			this.text			= text;
			this.index			= 0;
			this.hotkey			= new HotKey();
			this.action			= null;
			this.items			= new List<DebugMenuItem>();
		}

		/// <summary>Constructs a menu item with the specified text, action, and hotkey.</summary>
		public DebugMenuItem(string text, HotKey hotkey, MenuItemAction action) {
			this.root			= null;

			this.text			= text;
			this.index			= 0;
			this.hotkey			= hotkey ?? new HotKey();
			this.action			= action;
			this.items			= new List<DebugMenuItem>();
		}

		#endregion
		//============ ITEMS =============
		#region Items

		/// <summary>Adds the specified menu item to the list of menu items.</summary>
		public DebugMenuItem AddItem(string text) {
			return AddItem(new DebugMenuItem(text, null, null));
		}
		/// <summary>Adds the specified menu item to the list of menu items.</summary>
		public DebugMenuItem AddItem(string text, HotKey hotkey, MenuItemAction action) {
			return AddItem(new DebugMenuItem(text, hotkey, action));
		}

		/// <summary>Adds the specified menu item to the list of menu items.</summary>
		public DebugMenuItem AddItem(DebugMenuItem item) {
			item.Root  = this;
			item.Index = items.Count;
			items.Add(item);
			return item;
		}

		#endregion
		//=========== ACTIONS ============
		#region Actions

		/// <summary>Called when the menu item is pressed.</summary>
		public void Press() {
			if (action != null)
				action();
		}

		#endregion
		//========== PROPERTIES ==========
		#region Properties

		/// <summary>The root debug menu.</summary>
		public DebugMenuItem Root {
			get { return root; }
			set { root = value; }
		}
		/// <summary>The text of the menu item.</summary>
		public string Text {
			get { return text; }
			set { text = value; }
		}
		/// <summary>The index of the menu item.</summary>
		public int Index {
			get { return index; }
			set { index = value; }
		}
		/// <summary>The hotkey used by the menu item.</summary>
		public HotKey HotKey {
			get { return hotkey; }
			set {
				HotKey = value ?? new HotKey();
			}
		}
		/// <summary>The action called when the menu item is clicked.</summary>
		public MenuItemAction Action {
			get { return action; }
			set { action = value; }
		}
		/// <summary>The list of menu items in this menu item.</summary>
		public List<DebugMenuItem> Items {
			get { return items; }
		}

		#endregion
	}
}
