using System.Collections.Generic;

namespace ZeldaOracle.Game.Entities {
	public class EntityTracker<T> where T : Entity {

		private Entity[] entities;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public EntityTracker(int count) {
			entities = new Entity[count];
		}


		//-----------------------------------------------------------------------------
		// Tracking
		//-----------------------------------------------------------------------------

		// Get the first entity (that's of the desired tracking type).
		public T GetEntity() {
			for (int i = 0; i < entities.Length; i++) {
				RefreshEntityAtIndex(i);
				if (entities[i] != null && (entities[i] is T))
					return (T) entities[i];
			}
			return null;
		}

		// Add an entity to be tracked, returning true if there was an available tracking slot.
		public bool TrackEntity(T entity) {
			for (int i = 0; i < entities.Length; i++) {
				RefreshEntityAtIndex(i);
				if (entities[i] == null) {
					entities[i] = entity;
					return true;
				}
			}
			return false;
		}

		public void ClearEntities() {
			for (int i = 0; i < entities.Length; i++) {
				entities[i] = null;
			}
		}

		// Return true if a non-null entity is considered dead/destroyed.
		private bool IsEntityDead(Entity entity) {
			return (entity.IsDestroyed || !entity.IsInRoom);
		}

		private void RefreshEntityAtIndex(int index) {
			Entity entity = entities[index];
			if (entity != null && IsEntityDead(entity)) {
				if (entity.TransformedEntity != null) {
					entities[index] = entity.TransformedEntity;
					RefreshEntityAtIndex(index);
				}
				else
					entities[index] = null;
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>Iterate the currently tracked, alive entities.</summary>
		public IEnumerable<T> Entities {
			get {
				for (int i = 0; i < entities.Length; i++) {
					RefreshEntityAtIndex(i);
					if (entities[i] != null && (entities[i] is T))
						yield return (T) entities[i];
				}
			}
		}

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
					RefreshEntityAtIndex(i);
					if (entities[i] != null)
						aliveCount++;
				}
				return aliveCount;
			}
		}
	}
}
