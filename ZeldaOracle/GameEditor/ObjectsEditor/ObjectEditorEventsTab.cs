using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaEditor.Control;
using ZeldaEditor.Scripting;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.EventTiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaEditor.ObjectsEditor {

	public partial class ObjectEditorEventsTab : UserControl {

		private ObjectEditor objectEditor;
		private List<InternalObjectEvent> objectEvents;
		private InternalObjectEvent objectEvent;
		


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ObjectEditorEventsTab() {
			InitializeComponent();
			
			radioButtonEventCustomCode.Location = new Point(groupBoxCustomCode.Location.X + 13, groupBoxCustomCode.Location.Y - 1);
			radioButtonEventScript.Location		= new Point(groupBoxScript.Location.X + 13, groupBoxScript.Location.Y - 1);
			radioButtonEventNone.Location		= new Point(radioButtonEventCustomCode.Location.X, radioButtonEventNone.Location.Y);

			radioButtonEventNone.Checked	= true;
			groupBoxScript.Enabled			= false;
			groupBoxCustomCode.Enabled		= false;

			objectEvents = new List<InternalObjectEvent>();
		}

		public void Initialize(ObjectEditor objectEditor) {
			this.objectEditor = objectEditor;
		}

		public void SetupObject(IPropertyObject propertyObject) {
			// Create a list of all event properties.
			List<Property> eventProperties = new List<Property>();
			foreach (Property property in propertyObject.Properties.GetAllProperties()) {
				PropertyDocumentation doc = property.GetDocumentation();
				if (doc != null && doc.EditorType == "script")
					eventProperties.Add(property);
			}
			
			objectEvents.Clear();
			listBoxEvents.Items.Clear();

			IEventObject eventObject = objectEditor.PropertyObject as IEventObject;

			if (eventObject != null) {
				foreach (KeyValuePair<string, ObjectEvent> entry in eventObject.Events.Events) {
					listBoxEvents.Items.Add(entry.Value.ReadableName);
				
					Property property = objectEditor.PropertyObject.Properties.GetProperty(entry.Value.Name, true);

					InternalObjectEvent objEvent = new InternalObjectEvent();
					objEvent.Property			= property;
					objEvent.Id					= entry.Value.Name;
					objEvent.Name				= entry.Value.ReadableName;
					objEvent.Description		= entry.Value.Description;
					objEvent.CustomScriptExists	= false;
					objEvent.Event				= entry.Value;
					
					if (property != null) {
						string scriptName = property.StringValue;
						Script script = null;
						if (scriptName.Length > 0)
							script = EditorControl.World.GetScript(scriptName);
			
						if (script == null) {
							objEvent.RadioButtonIndex	= RedioButtonIndex.None;
							objEvent.CustomScript		= null;
							objEvent.ReferencedScript	= null;
						}
						else if (script.IsHidden) {
							objEvent.RadioButtonIndex	= RedioButtonIndex.CustomCode;
							objEvent.CustomScript		= script;
							objEvent.ReferencedScript	= null;
							objEvent.CustomScriptExists	= true;
						}
						else {
							objEvent.RadioButtonIndex	= RedioButtonIndex.Script;
							objEvent.CustomScript		= null;
							objEvent.ReferencedScript	= script;
						}
					}
					else {
							objEvent.RadioButtonIndex	= RedioButtonIndex.None;
							objEvent.CustomScript		= null;
							objEvent.ReferencedScript	= null;
					}
				
					objectEvents.Add(objEvent);
				}
			}
			else {
				for (int i = 0; i < eventProperties.Count; i++) {
					Property property = eventProperties[i];
					PropertyDocumentation doc = property.GetRootDocumentation();
					string name = (doc != null ? doc.ReadableName : property.Name);
					listBoxEvents.Items.Add(name);

					InternalObjectEvent objEvent = new InternalObjectEvent();
					objEvent.Property			= property;
					objEvent.Id				= property.Name;
					objEvent.Name				= (doc != null ? doc.ReadableName : property.Name);
					objEvent.Description		= (doc != null ? doc.Description : "");
					objEvent.CustomScriptExists	= false;
					objEvent.Event				= null;
					if (objectEditor.PropertyObject is IEventObject)
						objEvent.Event = ((IEventObject) objectEditor.PropertyObject).Events.GetEvent(property.Name);

					string scriptName = property.StringValue;
					Script script = null;
					if (scriptName.Length > 0)
						script = EditorControl.World.GetScript(scriptName);
			
					if (script == null) {
						objEvent.RadioButtonIndex	= RedioButtonIndex.None;
						objEvent.CustomScript		= null;
						objEvent.ReferencedScript	= null;
					}
					else if (script.IsHidden) {
						objEvent.RadioButtonIndex	= RedioButtonIndex.CustomCode;
						objEvent.CustomScript		= script;
						objEvent.ReferencedScript	= null;
						objEvent.CustomScriptExists	= true;
					}
					else {
						objEvent.RadioButtonIndex	= RedioButtonIndex.Script;
						objEvent.CustomScript		= null;
						objEvent.ReferencedScript	= script;
					}
				
					objectEvents.Add(objEvent);
				}
			}

			if (listBoxEvents.Items.Count > 0) {
				listBoxEvents.SelectedIndex = 0;
				objectEvent = objectEvents[0];
			}
			else {
				listBoxEvents.SelectedIndex = -1;
				objectEvent = null;
			}
		}
		
		public void ApplyChanges() {
			EditorControl.NeedsRecompiling = true;

			for (int i = 0; i < objectEvents.Count; i++) {
				InternalObjectEvent objEvent = objectEvents[i];
				
				// Check if we need to generate the custom script.
				if (objEvent.RadioButtonIndex == RedioButtonIndex.CustomCode && !objEvent.CustomScriptExists) {
					EditorControl.GenerateInternalScript(objEvent.CustomScript);
					Console.WriteLine("Generated internal script with name '" + objEvent.CustomScript.Name + "'");
				}

				// Check if we need to remove the hidden custom script.
				if (objEvent.RadioButtonIndex != RedioButtonIndex.CustomCode && objEvent.CustomScript != null && objEvent.CustomScriptExists) {
					EditorControl.World.RemoveScript(objEvent.CustomScript);
					Console.WriteLine("Removed internal script with name '" + objEvent.CustomScript.Name + "'");
				}
				
				// Determine the value for the script property.
				string propValue = "";
				if (objEvent.RadioButtonIndex == RedioButtonIndex.CustomCode)
					propValue = objEvent.CustomScript.Name;
				else if (objEvent.RadioButtonIndex == RedioButtonIndex.Script)
					propValue = objEvent.ReferencedScript.Name;

				// Set the property.
				Console.WriteLine("Setting property '" + objEvent.Id + "' to the value '" + propValue + "'");
				objectEditor.PropertyObject.Properties.Set(objEvent.Id, propValue);
			}
		}

		
		//-----------------------------------------------------------------------------
		// user Control Events.
		//-----------------------------------------------------------------------------

		// None radio button.
		private void radioButtonEventNone_CheckedChanged(object sender, EventArgs e) {
			if (objectEvent != null)
				objectEvent.RadioButtonIndex = RedioButtonIndex.None;
		}

		// Custom Code radio button.
		private void radioButtonEventCustomCode_CheckedChanged(object sender, EventArgs e) {
			groupBoxCustomCode.Enabled = radioButtonEventCustomCode.Checked;
			if (objectEvent != null)
				objectEvent.RadioButtonIndex = RedioButtonIndex.CustomCode;
		}

		// Script radio button.
		private void radioButtonEventScript_CheckedChanged(object sender, EventArgs e) {
			groupBoxScript.Enabled = radioButtonEventScript.Checked;
			if (objectEvent != null)
				objectEvent.RadioButtonIndex = RedioButtonIndex.Script;
		}

		// Selected event changes.
		private void listBoxEvents_SelectedIndexChanged(object sender, EventArgs e) {
			if (listBoxEvents.SelectedIndex >= 0) {
				// An event was selected.
				objectEvent = objectEvents[listBoxEvents.SelectedIndex];
				panelEventProperties.Visible	= true;
				
				// Name and description.
				labelEventName.Text			= "Event: " + objectEvent.Name;
				labelEventDescription.Text	= objectEvent.Description;

				// Referenced script combo box.
				if (objectEvent.ReferencedScript != null)
					comboBoxScript.Text = objectEvent.ReferencedScript.Name;
				else
					comboBoxScript.Text = "";

				// Radio buttons.
				if (objectEvent.RadioButtonIndex == RedioButtonIndex.None)
					radioButtonEventNone.Checked = true;
				else if (objectEvent.RadioButtonIndex == RedioButtonIndex.CustomCode)
					radioButtonEventCustomCode.Checked = true;
				else if (objectEvent.RadioButtonIndex == RedioButtonIndex.Script)
					radioButtonEventScript.Checked = true;
			}
			else {
				// No event is selected.
				panelEventProperties.Visible = false;
			}
		}

		// Button: Edit Code.
		private void buttonEditCode_Click(object sender, EventArgs e) {
			// Create a custom script if it is null.
			if (objectEvent.CustomScript == null) {
				objectEvent.CustomScript = new Script();
				if (objectEvent.Event != null) {
					objectEvent.CustomScript.Parameters = objectEvent.Event.Parameters;
					objectEvent.CustomScript.Code = "// Parameters = (";
					for (int i = 0; i < objectEvent.CustomScript.Parameters.Count; i++) {
						if (i > 0)
							objectEvent.CustomScript.Code += ", ";
						objectEvent.CustomScript.Code +=
								objectEvent.CustomScript.Parameters[i].Type + " " +
								objectEvent.CustomScript.Parameters[i].Name;
					}
					objectEvent.CustomScript.Code += ")\n";
				}
			}

			// Open the script editor.
			using (ScriptEditor form = new ScriptEditor(objectEvent.CustomScript, EditorControl)) {
				if (form.ShowDialog(EditorControl.EditorForm) == DialogResult.OK) {

				}
			}
		}

		// Button: Edit Script.
		private void buttonEditScript_Click(object sender, EventArgs e) {
			Script script = objectEvent.ReferencedScript;
			
			if (script != null) {
				using (ScriptEditor form = new ScriptEditor(script, EditorControl)) {
					if (form.ShowDialog(EditorControl.EditorForm) == DialogResult.OK) {

					}
					else {

					}
				}
			}
			else {
				MessageBox.Show("The script '" + comboBoxScript.Text +
					"' doesn't exist.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		// Script Combo-box.
		private void comboBoxScript_TextChanged(object sender, EventArgs e) {
			if (objectEvent != null) {
				string scriptName = comboBoxScript.Text;
				objectEvent.ReferencedScript = EditorControl.World.GetScript(scriptName);
			}
		}
		
		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public EditorControl EditorControl {
			get { return objectEditor.EditorControl; }
		}
		
		internal enum RedioButtonIndex {
			None,
			CustomCode,
			Script,
		}

		internal class InternalObjectEvent {
			public Property		Property { get; set; }
			public string		Id { get; set; }
			public string		Name { get; set; }
			public RedioButtonIndex RadioButtonIndex { get; set; }
			public string		Description { get; set; }
			public Script		CustomScript { get; set; }
			public Script		ReferencedScript { get; set; }
			public bool			CustomScriptExists { get; set; }
			public ObjectEvent	Event { get; set; }
		}
	}
}
