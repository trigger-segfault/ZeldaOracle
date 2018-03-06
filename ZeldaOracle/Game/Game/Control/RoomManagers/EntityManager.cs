using System.Collections.Generic;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities;

namespace ZeldaOracle.Game.Control {
	
	/// <summary>Manages the list of entities in a room.</summary>
	public class EntityManager : RoomManager {
		
		/// <summary>The current list of entities being managed.</summary>
		private List<Entity> entities;
		/// <summary>The order in which entities where last updated.</summary>
		private List<Entity> entityUpdateOrder;
		/// <summary>Used to give a unique index to each alive entity.</summary>
		private int entityIndexCounter;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public EntityManager(RoomControl roomControl) :
			base (roomControl)
		{
			entities = new List<Entity>();
			entityUpdateOrder = new List<Entity>();
		}
		

		//-----------------------------------------------------------------------------
		// Initialize / Terminate
		//-----------------------------------------------------------------------------

		/// <summary>Setup the entity manager with list of entities to spawn in the
		/// room.</summary>
		public void BeginRoom(List<Entity> persistentEntities) {
			entities.Clear();
			entityIndexCounter = 0;

			// Spawn the persistant entities
			SpawnEntity(Player);
			foreach (Entity entity in persistentEntities)
				SpawnEntity(entity);
		}

		
		/// <summary>Mark all entities as having left the room.</summary>
		public void LeaveRoom() {
			for (int i = 0; i < entities.Count; i++) {
				if (entities[i] != Player)
					entities[i].IsInRoom = false;
			}
		}


		//-----------------------------------------------------------------------------
		// Entity Processing
		//-----------------------------------------------------------------------------

		/// <summary>Update all alive entities.</summary>
		public void UpdateEntities() {
			// Determine an order to update entities in, which has the following
			// characteristics:
			//   1. The player updates first
			//   2. All child entities update after their parent entity.
			entityUpdateOrder.Clear();
			for (int i = 0; i < entities.Count; i++) {
				if (entities[i].Parent == null) {
					entityUpdateOrder.Add(entities[i]);
					for (int j = 0; j < entities[i].Children.Count; j++)
						entityUpdateOrder.Add(entities[i].Children[j]);
				}
			}

			// Now udpate all entities in that order
			for (int i = 0; i < entityUpdateOrder.Count; i++) {
				if (entityUpdateOrder[i].IsAlive)
					entityUpdateOrder[i].Update();
			}

			// If any new entities where spawned, update those too
			for (int i = entityUpdateOrder.Count; i < entities.Count; i++) {
				if (entities[i].IsAlive)
					entities[i].Update();
			}

			// Remove any entities that were destroyed during update
			RemoveDestroyedEntities();
		}
		
		/// <summary>Update the graphics for all entities.</summary>
		public void UpdateEntityGraphics() {
			for (int i = 0; i < entityUpdateOrder.Count; i++) {
				if (entityUpdateOrder[i].IsAlive)
					entityUpdateOrder[i].UpdateGraphics();
			}
			for (int i = entityUpdateOrder.Count; i < entities.Count; i++) {
				if (entities[i].IsAlive)
					entities[i].UpdateGraphics();
			}
		}

		/// <summary>Remove destroyed entities from the entity list.</summary>
		private void RemoveDestroyedEntities() {
			for (int i = 0; i < entities.Count; i++) {
				if (!entities[i].IsAlive)
					entities.RemoveAt(i--);
			}
		}
		

		//-----------------------------------------------------------------------------
		// Entity Spawning
		//-----------------------------------------------------------------------------
		
		/// <summary>Initialize and spawn an entity, and have it be managed by the
		/// RoomControl.</summary>
		public void SpawnEntity(Entity entity) {
			entity.RoomControl = RoomControl;
			entity.IsAlive = true;
			entity.IsInRoom = true;

			// Add the entity to the entity list and give it an index that's among
			// between all active entities
			if (!entities.Contains(entity)) {
				entity.EntityIndex = entityIndexCounter++;
				entities.Add(entity);
			}

			entity.Initialize(RoomControl);
		}
		
		/// <summary>Initialize and spawn an entity at the given position, and have it
		/// be managed by the RoomControl.</summary>
		public void SpawnEntity(Entity entity, Vector2F position) {
			entity.Position = position;
			SpawnEntity(entity);
		}
		
		/// <summary>Initialize and spawn an entity at the given position, and have it
		/// be managed by the RoomControl.</summary>
		public void SpawnEntity(Entity entity, Vector2F position, float zPosition) {
			entity.Position = position;
			entity.ZPosition = zPosition;
			SpawnEntity(entity);
		}
		

		//-----------------------------------------------------------------------------
		// Entity Queries
		//-----------------------------------------------------------------------------
		
		/// <summary>Iterate entities of the given type.</summary>
		public IEnumerable<T> GetEntitiesOfType<T>() where T : Entity {
			foreach (Entity entity in entities) {
				if ((entity is T) && entity.IsAlive)
					yield return ((T) entity);
			}
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>The list of all entities. This can include destroyed entities.
		/// </summary>
		public List<Entity> Entities {
			get { return entities; }
		}
		
		/// <summary>The list of all entities which currently alive.</summary>
		public IEnumerable<Entity> AliveEntities {
			get {
				int entityCount = entities.Count;
				for (int i = 0; i < entityCount; i++) {
					if (entities[i].IsAlive && entities[i].IsInRoom)
						yield return entities[i];
				}
			}
		}

		/// <summary>The order in which entities where last updated. For debug
		/// purposes only.</summary>
		public List<Entity> EntityUpdateOrder {
			get { return entityUpdateOrder; }
		}
	}
}
