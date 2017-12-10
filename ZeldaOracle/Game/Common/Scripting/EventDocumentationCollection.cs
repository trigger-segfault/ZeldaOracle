using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Scripting {

	/// <summary>A collection of documentation on definable events.</summary>
	public class EventDocumentationCollection {
		/// <summary>The collection of event documentations.</summary>
		private Dictionary<string, EventDocumentation> events;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs an empty event documentation collection.</summary>
		public EventDocumentationCollection() {
			this.events = new Dictionary<string, EventDocumentation>();
		}

		/// <summary>Constructs a copy of the event documentation collection.</summary>
		public EventDocumentationCollection(EventDocumentationCollection copy) {
			this.events = new Dictionary<string, EventDocumentation>();
			foreach (EventDocumentation documentation in copy.GetEvents()) {
				events[documentation.Name] = documentation;
			}
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets an enumerable list for all event documentations.</summary>
		public IEnumerable<EventDocumentation> GetEvents() {
			foreach (EventDocumentation documentation in events.Values) {
				yield return documentation;
			}
		}

		/// <summary>Gets the documentation with the specified event name.</summary>
		public EventDocumentation GetEvent(string eventName) {
			EventDocumentation documentation;
			events.TryGetValue(eventName, out documentation);
			return documentation;
		}

		/// <summary>Returns true id documentation with the specified event name exists.</summary>
		public bool ContainsEvent(string eventName) {
			return events.ContainsKey(eventName);
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Clears the list of event documentations.</summary>
		public void Clear() {
			events.Clear();
		}

		/// <summary>Adds the event documentation to the collection.</summary>
		public void AddEvent(string name, string readableName, string category,
			string description, params ScriptParameter[] parameters) {
			AddEvent(new EventDocumentation(name, readableName, category, description,
				parameters));
		}

		/// <summary>Adds the event documentation to the collection.</summary>
		public void AddEvent(EventDocumentation documentation) {
			events.Add(documentation.Name, documentation);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets the number of event documentations.</summary>
		public int Count {
			get { return events.Count; }
		}
	}
}
