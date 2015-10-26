using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Items.Drops {

	public class DropList : IDropCreator {

		private List<DropChance> drops;
		private int oddsNumerator;
		private int oddsDenominator;

		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public DropList() :
			this(1, 1)
		{
		}

		public DropList(int numerator, int denominator) {
			this.drops				= new List<DropChance>();
			this.oddsNumerator		= numerator;
			this.oddsDenominator	= denominator;
		}


		//-----------------------------------------------------------------------------
		// Drop Methods
		//-----------------------------------------------------------------------------

		public void SetOdds(int numerator, int denominator) {
			this.oddsNumerator		= numerator;
			this.oddsDenominator	= denominator;
		}

		public void AddDrop(int odds, Reward reward) {
			AddDrop(odds, new Drop(reward));
		}

		public void AddDrop(int odds, Type entityType) {
			AddDrop(odds, new Drop(entityType));
		}
		
		public void AddDrop(int odds, IDropCreator drop) {
			drops.Add(new DropChance(odds, drop));
		}


		//-----------------------------------------------------------------------------
		// Implementations
		//-----------------------------------------------------------------------------

		public Entity CreateDropEntity(GameControl gameControl) {
			if (drops.Count == 0)
				return null;
			int roll;

			// Roll to see if we drop anything at all.
			if (oddsNumerator < oddsDenominator) {
				roll = GRandom.NextInt(oddsDenominator);
				if (roll >= oddsNumerator)
					return null;
			}
			
			// Sum up the odds of all the available drops in the list.
			int dropsOddsSum = 0;
			for (int i = 0; i < drops.Count; i++) {
				if (drops[i].Drop.IsAvailable(gameControl))
					dropsOddsSum += drops[i].Odds;
			}
			
			// Roll to see which drop to create.
			roll = GRandom.NextInt(dropsOddsSum);

			// Determine which drop should be created.
			int x = 0;
			for (int i = 0; i < drops.Count; i++) {
				if (drops[i].Drop.IsAvailable(gameControl)) {
					x += drops[i].Odds;
					if (roll < x)
						return drops[i].Drop.CreateDropEntity(gameControl);
				}
			}

			return null;
		}

		public bool IsAvailable(GameControl gameControl) {
			return true;
		}
	}
}
