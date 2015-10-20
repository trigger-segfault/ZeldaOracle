using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Entities {
	public class EntityTracker<T> where T : Entity {

		private T[] entities;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public EntityTracker(int count) {
			entities = new T[count];
		}


		//-----------------------------------------------------------------------------
		// Tracking
		//-----------------------------------------------------------------------------

		public T GetEntity() {
			for (int i = 0; i < entities.Length; i++) {
				if (entities[i] != null && !IsEntityDead(entities[i]))
					return entities[i];
			}
			return null;
		}

		// Add an entity to be tracked, returning true if there was an available tracking slot.
		public bool TrackEntity(T entity) {
			for (int i = 0; i < entities.Length; i++) {
				if (entities[i] == null || IsEntityDead(entities[i])) {
					entities[i] = entity;
					return true;
				}
			}
			return false;
		}

		// Return true if a non-null entity is considered dead/destroyed.
		private bool IsEntityDead(T entity) {
			return (entity.IsDestroyed || !entity.IsInRoom);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public int MaxEntityCount {
			get { return entities.Length; }
		}

		public bool IsMaxedOut {
			get { return (AliveCount == entities.Length); }
		}
		
		public bool IsAvailable {
			get { return (AliveCount < entities.Length); }
		}

		public bool IsEmpty {
			get { return (AliveCount == 0); }
		}

		public int AliveCount {
			get {
				int aliveCount = 0;
				for (int i = 0; i < entities.Length; i++) {
					if (entities[i] != null) {
						if (IsEntityDead(entities[i]))
							entities[i] = null;
						else
							aliveCount++;
					}
				}
				return aliveCount;
			}
		}
	}
}
