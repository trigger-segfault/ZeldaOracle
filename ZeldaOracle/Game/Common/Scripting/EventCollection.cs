using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Scripting {
	/**<summary>The collection of definable events.</summary>*/
	public class EventCollection {
		/**<summary>The object that holds these events.</summary>*/
		private IEventObject eventObject;
		/**<summary>The collection of events.</summary>*/
		private Dictionary<string, Event> events;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		/**<summary>Constructs an empty event collection.</summary>*/
		public EventCollection(IEventObject eventObject) {
			this.eventObject	= eventObject;
			this.events			= new Dictionary<string, Event>();
		}

		/**<summary>Constructs an event collection from the list of documented events.</summary>*/
		public EventCollection(EventDocumentationCollection documentations, IEventObject eventObject) {
			this.eventObject	= eventObject;
			this.events			= new Dictionary<string, Event>();
			foreach (EventDocumentation documentation in documentations.GetEvents()) {
				AddEvent(new Event(documentation));
			}
		}

		/**<summary>Constructs a copy of the event collection.</summary>*/
		public EventCollection(EventCollection copy, IEventObject eventObject) {
			this.eventObject	= eventObject;
			this.events			= new Dictionary<string, Event>();
			foreach (Event evnt in copy.events.Values) {
				AddEvent(new Event(evnt));
			}
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/**<summary>Gets an enumerable list for all events.</summary>*/
		public IEnumerable<Event> GetEvents() {
			foreach (Event evnt in events.Values) {
				yield return evnt;
			}
		}

		/**<summary>Gets an enumerable list for all defined events.</summary>*/
		public IEnumerable<Event> GetDefinedEvents() {
			foreach (Event evnt in events.Values) {
				if (evnt.IsDefined)
					yield return evnt;
			}
		}

		/**<summary>Gets the documentation with the specified event name.</summary>*/
		public Event GetEvent(string eventName) {
			if (events.ContainsKey(eventName))
				return events[eventName];
			return null;
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/**<summary>Clears the list of events.</summary>*/
		public void Clear() {
			events.Clear();
		}

		/**<summary>Adds an event to the collection based on the documentation.</summary>*/
		public void AddEvent(string name, string readableName, string category, string description, params ScriptParameter[] parameters) {
			AddEvent(new EventDocumentation(name, readableName, category, description, parameters));
		}

		/**<summary>Adds an event to the collection based on the documentation.</summary>*/
		public void AddEvent(EventDocumentation documentation) {
			AddEvent(new Event(documentation));
		}

		/**<summary>Adds an event to the collection.</summary>*/
		public void AddEvent(Event e) {
			events.Add(e.Name, e);
			e.Events = this;
		}

		/**<summary>Sets all defined events in the collection.</summary>*/
		public void SetAll(EventCollection events) {
			foreach (Event evnt in events.GetDefinedEvents()) {
				Event newEvent = new Event(evnt);
				newEvent.Events = this;
				this.events[evnt.Name] = newEvent;
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/**<summary>Gets the collection of events.</summary>*/
		public Dictionary<string, Event> Events {
			get { return events; }
			set { events = value; }
		}

		/**<summary>Returns true if any events in this collection are defined.</summary>*/
		public bool HasDefinedEvents {
			get {
				foreach (Event evnt in events.Values) {
					if (evnt.IsDefined)
						return true;
				}
				return false;
			}
		}

		/**<summary>Gets the object that holds these events.</summary>*/
		public IEventObject EventObject {
			get { return eventObject; }
			set { eventObject = value; }
		}
	}
}
