using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using ZeldaOracle.Common;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input.Controls;

namespace ZeldaOracle.Common.Debug {
public class RadioButtonGroup {
	private List<RadioButtonMenuItem> items;

	public RadioButtonGroup() {
		items = new List<RadioButtonMenuItem>();
	}

	public List<RadioButtonMenuItem> Items {
		get { return items; }
	}
}


public class RadioButtonMenuItem : ToggleMenuItem {
	private RadioButtonGroup group;



	// ================== CONSTRUCTORS ================== //

	public RadioButtonMenuItem(string text, HotKey hotkey, RadioButtonGroup group, bool startsEnabled, MenuItemToggleAction toggleAction,
		MenuItemAction enableAction, MenuItemAction disableAction) :
		base(text, hotkey, startsEnabled, toggleAction, enableAction, disableAction) {
		this.group = group;
		group.Items.Add(this);


		Action = delegate() {
			if (!IsEnabled) {
				Enable();

				for (int i = 0; i < group.Items.Count; ++i) {
					if (group.Items[i] != this && group.Items[i].IsEnabled)
						group.Items[i].Disable();
				}
			}
		};
	}



	// ================== PROPERTIES =================== //

	public RadioButtonGroup RadioButtonGroup {
		get { return group; }
	}
}
}
