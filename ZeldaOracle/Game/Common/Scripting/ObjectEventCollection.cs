using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Scripting {
	
	public class ObjectEventCollection {

		private Dictionary<string, ObjectEvent> events;

		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ObjectEventCollection() {
			events = new Dictionary<string, ObjectEvent>();
		}
		
		public ObjectEventCollection(ObjectEventCollection copy) {
			events = new Dictionary<string, ObjectEvent>();
			foreach (KeyValuePair<string, ObjectEvent> pair in copy.events) {
				events[pair.Key] = new ObjectEvent(pair.Value);
			}
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------
		
		public void Clear() {
			events.Clear();
		}

		public ObjectEvent GetEvent(string id) {
			if (events.ContainsKey(id))
				return events[id];
			return null;
		}

		
		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		public void AddEvent(string name, string readableName, string description, params ScriptParameter[] parameters) {
			AddEvent(new ObjectEvent(name, readableName, description, parameters));
		}

		public void AddEvent(ObjectEvent e) {
			events.Add(e.Name, e);
		}

		public Dictionary<string, ObjectEvent> Events {
			get { return events; }
			set { events = value; }
		}
	}
}
