using System;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities;

namespace ZeldaOracle.Game.Tiles.ActionTiles {

	/// <summary>Action tile used to spawn an entity.</summary>
	public class EntityActionTile<T> : ActionTile where T : Entity, new() {

		/// <summary>The spawned entity.</summary>
		private T entity;

		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public EntityActionTile() {
		}


		//-----------------------------------------------------------------------------
		// Entity Methods
		//-----------------------------------------------------------------------------

		/// <summary>Spawns the entity into the room.</summary>
		private void SpawnEntity() {
			entity = new T();
			entity.Position = Center;
			entity.Properties = properties;
			entity.Events = Events;
			entity.Vars = Variables;
			entity.Triggers = Triggers;
			RoomControl.SpawnEntity(entity);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		protected override void Initialize() {
			SpawnEntity();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public T Entity {
			get { return entity; }
		}

		public override Type TriggerObjectType {
			get { return typeof(T); }
		}
	}
}
