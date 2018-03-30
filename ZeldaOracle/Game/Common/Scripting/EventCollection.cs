using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Scripting {
	/// <summary>The collection of definable events.</summary>
	[Serializable]
	public class EventCollection {
		/// <summary>The original collection of event documentation.</summary>
		[NonSerialized]
		private EventDocumentationCollection documentations;
		/// <summary>The object that holds these events.</summary>
		[NonSerialized]
		private IEventObject eventObject;
		/// <summary>The collection of events.</summary>
		private Dictionary<string, Event> events;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		/// <summary>Constructs an empty event collection.</summary>
		public EventCollection(IEventObject eventObject) {
			this.documentations	= new EventDocumentationCollection();
			this.eventObject	= eventObject;
			this.events			= new Dictionary<string, Event>();
		}

		/// <summary>Constructs an event collection from the list of documented events.</summary>
		public EventCollection(EventDocumentationCollection documentations, IEventObject eventObject) {
			this.documentations	= documentations;
			this.eventObject	= eventObject;
			this.events			= new Dictionary<string, Event>();
			foreach (EventDocumentation documentation in documentations.GetEvents()) {
				AddEvent(new Event(documentation));
			}
		}

		/// <summary>Constructs a copy of the event collection.</summary>
		public EventCollection(EventCollection copy, IEventObject eventObject) {
			this.documentations	= copy.documentations;
			this.eventObject	= eventObject;
			this.events			= new Dictionary<string, Event>();
			foreach (Event evnt in copy.events.Values) {
				AddEvent(new Event(evnt));
			}
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Gets an enumerable list for all events.</summary>
		public IEnumerable<Event> GetEvents() {
			foreach (Event evnt in events.Values) {
				yield return evnt;
			}
		}

		/// <summary>Gets an enumerable list for all defined events.</summary>
		public IEnumerable<Event> GetDefinedEvents() {
			foreach (Event evnt in events.Values) {
				if (evnt.IsDefined)
					yield return evnt;
			}
		}

		/// <summary>Gets the event with the specified name.</summary>
		public Event GetEvent(string eventName) {
			Event evnt;
			events.TryGetValue(eventName, out evnt);
			return evnt;
		}

		/// <summary>Returns true if an event with the specified name exists.</summary>
		public bool ContainsEvent(string eventName) {
			return events.ContainsKey(eventName);
		}

		/// <summary>Returns true if the event with the specified name can be renamed.</summary>
		public bool CanRenameEvent(string oldName, string newName) {
			return events.ContainsKey(oldName) &&
				(!documentations.ContainsEvent(oldName) ||
				documentations.ContainsEvent(newName));
		}

		/// <summary>Returns true if the event with the specified name exists,
		/// but has no documentation.</summary>
		public bool ContainsWithNoDocumentation(string eventName) {
			return events.ContainsKey(eventName) &&
				!documentations.ContainsEvent(eventName);
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Clears the list of events.</summary>
		public void Clear() {
			events.Clear();
		}

		/// <summary>Adds an event to the collection based on the documentation.</summary>
		public void AddEvent(string name, string readableName, string category,
			string description, params ScriptParameter[] parameters)
		{
			EventDocumentation documentation = new EventDocumentation(name,
				readableName, category, description, parameters);
			AddEvent(new Event(documentation));
		}

		/// <summary>Adds an event to the collection based on the documentation.</summary>
		public void AddEvent(EventDocumentation documentation) {
			documentations.AddEvent(documentation);
			AddEvent(documentation);
		}

		/// <summary>Adds an event to the collection.</summary>
		public void AddEvent(Event e) {
			events.Add(e.Name, e);
			e.Events = this;
		}

		/// <summary>Sets all defined events in the collection.</summary>
		public void SetAll(EventCollection events) {
			foreach (Event evnt in events.GetDefinedEvents()) {
				Event newEvent = new Event(evnt);
				newEvent.Events = this;
				this.events[evnt.Name] = newEvent;
			}
		}

		/// <summary>Removes the event from the collection only if it doesn't have
		/// supporting documentation.</summary>
		public bool RemoveEvent(string eventName, bool onlyIfNoDocumentation) {
			bool hasDocumentation = documentations.ContainsEvent(eventName);
			Event evnt = GetEvent(eventName);
			if (evnt != null && (!onlyIfNoDocumentation || !hasDocumentation)) {
				if (hasDocumentation) {
					evnt.UndefineScript();
					evnt.InternalScriptID = "";
				}
				else {
					events.Remove(eventName);
				}
				return true;
			}
			return false;
		}

		/// <summary>Renames the event with the specified name and locates its
		/// new documentation.</summary>
		public bool RenameEvent(string oldName, string newName, bool onlyIfNoDocumentation) {
			bool hasOldDocumentation = documentations.ContainsEvent(oldName);
			EventDocumentation newDocumentation = documentations.GetEvent(newName);
			Event oldEvent = GetEvent(oldName);
			if (oldEvent != null && (!onlyIfNoDocumentation || !hasOldDocumentation) &&
				(!hasOldDocumentation || newDocumentation != null))
			{
				if (hasOldDocumentation) {
					Event newEvent = GetEvent(newName);
					newEvent.Script = oldEvent.Script;
					newEvent.InternalScriptID = oldEvent.InternalScriptID;
					oldEvent.UndefineScript();
					oldEvent.InternalScriptID = "";
				}
				else {
					if (newDocumentation != null) {
						oldEvent.Documentation = newDocumentation;
					}
					else {
						oldEvent.Documentation.Name = newName;
					}
					events.Remove(oldName);
					events[newName] = oldEvent;
				}
				return true;
			}
			return false;
		}

		/// <summary>Used to restore events that were aquired from the clipboard.</summary>
		public void RestoreFromClipboard(EventDocumentationCollection documentations,
			IEventObject eventObject)
		{
			this.eventObject = eventObject;
			this.documentations = documentations;
			foreach (EventDocumentation doc in documentations.GetEvents()) {
				if (ContainsEvent(doc.Name)) {
					GetEvent(doc.Name).Documentation = doc;
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets the object that holds these events.</summary>
		public IEventObject EventObject {
			get { return eventObject; }
			set { eventObject = value; }
		}

		/// <summary>Gets or sets the documentations for the events.</summary>
		public EventDocumentationCollection Documentation {
			get { return documentations; }
		}

		/// <summary>Gets the number of events.</summary>
		public int Count {
			get { return events.Count; }
		}

		/// <summary>Gets the number of defined events.</summary>
		public int DefinedCount {
			get {
				int count = 0;
				foreach (Event evnt in events.Values) {
					if (evnt.IsDefined)
						count++;
				}
				return count;
			}
		}

		/// <summary>Returns true if any events in this collection are defined.</summary>
		public bool HasDefinedEvents {
			get {
				foreach (Event evnt in events.Values) {
					if (evnt.IsDefined)
						return true;
				}
				return false;
			}
		}
	}
}
