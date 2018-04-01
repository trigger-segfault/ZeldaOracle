using System.Collections.Generic;
using System.Linq;

namespace ZeldaOracle.Common.Scripting {

	public class TriggerCollection {

		private List<Trigger> triggers;
		private ITriggerObject triggerObject;
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public TriggerCollection(ITriggerObject triggerObject) {
			this.triggerObject = triggerObject;
			triggers = new List<Trigger>();
		}

		/// <summary>Constructs a copy of the trigger collection.</summary>
		public TriggerCollection(TriggerCollection copy, ITriggerObject triggerObject) {
			this.triggerObject = triggerObject;
			triggers = new List<Trigger>();
			foreach (Trigger trigger in copy.triggers)
				triggers.Add(new Trigger(trigger, this));
		}


		//-----------------------------------------------------------------------------
		// Acccessors
		//-----------------------------------------------------------------------------

		/// <summary>Find a trigger by name.</summary>
		public Trigger GetTrigger(string name) {
			return triggers.FirstOrDefault(t => t.Name == name);
		}

		public Trigger CreateNewTrigger(string name) {
			Trigger trigger = new Trigger(this, name);
			triggers.Add(trigger);
			return trigger;
		}

		public Trigger AddTrigger(Trigger trigger) {
			triggers.Add(trigger);
			trigger.Collection = this;
			return trigger;
		}

		public void RemoveTrigger(Trigger trigger) {
			triggers.Remove(trigger);
		}

		/// <summary>Return an enumerable list of all triggers which would be triggered
		/// by the given event.</summary>
		public IEnumerable<Trigger> GetTriggersByEvent(Event e) {
			return triggers.Where(t => t.IsEnabled && t.IsTriggeredByEvent(e));
		}
		

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public ITriggerObject TriggerObject {
			get { return triggerObject; }
			set { triggerObject = value; }
		}

		public IEnumerator<Trigger> GetEnumerator() {
			for (int i = 0; i < triggers.Count; i++)
				yield return triggers[i];
		}

		public int Count {
			get { return triggers.Count; }
		}
	}
}
