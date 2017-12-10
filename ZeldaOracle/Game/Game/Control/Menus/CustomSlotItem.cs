using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Control.Menus {
	public class CustomSlotItem : ISlotItem {

		private string name;
		private string description;
		private ISprite sprite;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public CustomSlotItem(string name, string description, ISprite sprite) {
			this.name			= name;
			this.description	= description;
			this.sprite			= sprite;
		}

		//-----------------------------------------------------------------------------
		// Drawing
		//-----------------------------------------------------------------------------

		public void DrawSlot(Graphics2D g, Point2I position, int lightOrDark) {
			if (sprite != null)
				g.DrawSprite(sprite, position);
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public string Name {
			get { return name; }
			set { name = value; }
		}

		public string Description {
			get { return description; }
			set { description = value; }
		}
	}
}
