using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Content;
using ZeldaEditor.Control;
using ZeldaOracle.Game.Worlds;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace ZeldaEditor.PropertiesEditor {

	public class CustomEventDescriptor : PropertyDescriptor {
		private EditorControl           editorControl;
		private EventCollection			events;
		private string					eventName;
		private IEventObject            eventObject;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public CustomEventDescriptor(EditorControl editorControl, IEventObject eventObject, string eventName, EventCollection events, Attribute[] attributes) :
			base(eventName, attributes) {
			this.editorControl      = editorControl;
			this.eventObject        = eventObject;
			this.eventName			= eventName;
			this.events				= events;
		}

		public CustomEventDescriptor(EditorControl editorControl, IEventObject eventObject, string eventName, EventCollection events) :
			this(editorControl, eventObject, eventName, events, null) {
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Get the editor to use to edit this property.
		/*public override object GetEditor(Type editorBaseType) {
			return base.GetEditor(editorBaseType);
		}*/

		// Returns true if the property is modified.
		public override bool ShouldSerializeValue(object component) {
			return true;
		}

		// Is the value allowed to be reset?
		public override bool CanResetValue(object component) {
			return true;
		}

		// Get the displayed value of the property.
		public override object GetValue(object component) {
			Event evnt = Event;
			if (evnt.IsDefined) {
				string existingScript = evnt.GetExistingScript(editorControl.World.ScriptManager.Scripts);
				return existingScript ?? "custom";
			}
			else {
				return "";
			}
		}

		// Reset the value to the default.
		public override void ResetValue(object component) {
			Event.UndefineScript();
		}

		// Set the displayed value of the property.
		// This method will only be called if the new value is different from the old one.
		public override void SetValue(object component, object value) {
			
		}


		//-----------------------------------------------------------------------------
		// Overridden properties
		//-----------------------------------------------------------------------------

		// Is the property read-only?
		public override bool IsReadOnly {
			get { return false; }
		}

		// Should the property be listed in the property grid?
		public override bool IsBrowsable {
			get { return true; }
		}

		public override Type ComponentType {
			get { return eventObject.GetType(); }
		}

		// Get the category this property should be listed under.
		public override string Category {
			get { return Event.Category; }
		}

		// Get the description for the property.
		public override string Description {
			get { return Event.Description; }
		}

		// Get the display name for the property.
		public override string DisplayName {
			get { return Event.FinalReadableName; }
		}

		public override Type PropertyType {
			get { return typeof(string); }
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Event Event {
			get { return events.GetEvent(eventName); }
		}
		
		public IEventObject EventObject {
			get { return eventObject; }
		}

		public EditorControl EditorControl {
			get { return editorControl; }
		}
	}
}
