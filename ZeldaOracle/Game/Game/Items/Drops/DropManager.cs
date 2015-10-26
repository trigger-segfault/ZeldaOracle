using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Items.Drops {

	public class DropManager {

		private GameControl gameControl;
		private Dictionary<string, DropList> dropLists;
		


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public DropManager(GameControl gameControl) {
			this.gameControl	= gameControl;
			this.dropLists		= new Dictionary<string, DropList>();
		}

		
		//-----------------------------------------------------------------------------
		// Drop Lists
		//-----------------------------------------------------------------------------
		
		public DropList CreateDropList(string id) {
			return CreateDropList(id, 1, 1);
		}
		
		public DropList CreateDropList(string id, int oddsNumerator, int oddsDenominator) {
			DropList dropList = new DropList(oddsNumerator, oddsDenominator);
			dropLists.Add(id, dropList);
			return dropList;
		}

		public DropList AddDropList(string id, DropList dropList) {
			dropLists.Add(id, dropList);
			return dropList;
		}

		public DropList GetDropList(string id) {
			return dropLists[id];
		}

		public bool HasDropList(string id) {
			return dropLists.ContainsKey(id);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public Dictionary<string, DropList> DropListDictionary {
			get { return dropLists; }
		}
	}
}
