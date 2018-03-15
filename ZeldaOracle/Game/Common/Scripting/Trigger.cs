
using ZeldaOracle.Game.Control.Scripting;

namespace ZeldaOracle.Common.Scripting {

	public struct TriggerEvent {
		public Event Event { get; set; }
		
		public static readonly TriggerEvent None = new TriggerEvent(null);

		//public TriggerEvent() {
		//	Event = null;
		//}

		public TriggerEvent(Event e) {
			Event = e;
		}

		public override string ToString() {
			if (Event != null)
				return Event.ReadableName;
			return "(None)";
		}
	}

	public class Trigger {
		
		private TriggerCollection collection;
		private string name;
		private string description;
		private bool initiallyOn;
		private bool enabled;
		private bool fireOnce;
		private TriggerEvent eventType; // TODO: multiple events
		private Script script;

		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Trigger() : this(null) {
		}

		public Trigger(TriggerCollection collection) {
			this.collection = collection;
			initiallyOn	= true;
			fireOnce	= false;
			name		= "";
			description	= "";
			eventType	= new TriggerEvent(null);
			script		= null;
			enabled		= true;
		}

		public Trigger(Trigger copy, TriggerCollection collection) {
			this.collection = collection;
			initiallyOn	= copy.initiallyOn;
			fireOnce	= copy.fireOnce;
			name		= copy.name;
			description	= copy.description;
			eventType	= copy.eventType;
			script		= new Script(copy.script);
		}
		

		//-----------------------------------------------------------------------------
		// Trigger Action Management
		//-----------------------------------------------------------------------------

		public bool IsTriggeredByEvent(Event e) {
			return (eventType.Event == e);
		}

		public Script CreateScript() {
			script = new Script() {
				ID = name,
				IsHidden = true,
			};
			
			return script;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override string ToString() {
			return Name;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public TriggerCollection Collection {
			get { return collection; }
			set { collection = value; }
		}

		public bool InitiallyOn {
			get { return initiallyOn; }
			set { initiallyOn = value; }
		}

		public bool FireOnce {
			get { return fireOnce; }
			set { fireOnce = value; }
		}

		public bool IsEnabled {
			get { return enabled; }
			set { enabled = value; }
		}

		public string Name {
			get { return name; }
			set { name = value; }
		}

		public TriggerEvent EventType {
			get { return eventType; }
			set { eventType = value; }
		}

		public Script Script {
			get { return script; }
			set { script = value; }
		}
	}
}
