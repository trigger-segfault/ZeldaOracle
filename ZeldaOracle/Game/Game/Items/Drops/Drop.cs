using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Items.Drops {

	public class Drop : IDropCreator {

		private Reward reward;
		private Type entityType;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public Drop() {
			reward		= null;
			entityType	= null;
		}
		
		public Drop(Reward reward) {
			this.reward		= reward;
			this.entityType	= null;
		}
		
		public Drop(Type entityType) {
			this.reward		= null;
			this.entityType	= entityType;
		}


		//-----------------------------------------------------------------------------
		// Implementations
		//-----------------------------------------------------------------------------
		
		public Entity CreateDropEntity(GameControl gameControl) {
			if (reward != null) {
				return new CollectibleReward(reward, true);
			}
			else if (entityType != null) {
				// Spawn a new entity.
				Entity entity = (Entity) entityType.GetConstructor(Type.EmptyTypes).Invoke(null);
				return entity;
			}
			return null;
		}

		public bool IsAvailable(GameControl gameControl) {
			if (reward != null)
				return reward.IsAvailable;
			return true;
		}
	}
}
