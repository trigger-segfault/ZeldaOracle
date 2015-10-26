using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Control;

namespace ZeldaOracle.Game.Items.Drops {

	public interface IDropCreator {

		bool IsAvailable(GameControl gameControl);
		Entity CreateDropEntity(GameControl gameControl);
	}
}
