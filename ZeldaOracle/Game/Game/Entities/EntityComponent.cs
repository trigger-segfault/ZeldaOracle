
namespace ZeldaOracle.Game.Entities {
	
	public class EntityComponent {

		private Entity entity;
		private bool isEnabled;
		
		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public EntityComponent(Entity entity, bool isEnabled = false) {
			this.entity = entity;
			this.isEnabled = isEnabled;
		}
		
		
		//-----------------------------------------------------------------------------
		// Virtual Methods
		//-----------------------------------------------------------------------------
		
		public virtual void OnEnable() {}

		public virtual void OnDisable() {}


		//-----------------------------------------------------------------------------
		// Enable/Disable
		//-----------------------------------------------------------------------------

		public void Enable() {
			isEnabled = true;
			OnEnable();
		}

		public void Disable() {
			isEnabled = true;
			OnEnable();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>The entity to which this component belongs.</summary>
		public Entity Entity {
			get { return entity; }
			set { entity = value; }
		}

		/// <summary>True if this component is enabled.</summary>
		public bool IsEnabled {
			get { return isEnabled; }
			set { isEnabled = value; }
		}

	}
}
