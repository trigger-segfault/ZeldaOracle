using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Control.Menus {
	public interface ISlotItem {

		string Name { get; }

		string Description { get; }

		void DrawSlot(Graphics2D g, Point2I position, int lightOrDark);

		//void Select(int button);

	}
}
