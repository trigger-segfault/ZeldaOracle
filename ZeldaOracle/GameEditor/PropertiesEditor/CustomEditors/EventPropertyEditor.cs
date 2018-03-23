using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using ZeldaEditor.Control;
using ZeldaEditor.Controls;
using ZeldaEditor.Scripting;
using ZeldaEditor.Undo;
using ZeldaEditor.Windows;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Tiles;

namespace ZeldaEditor.PropertiesEditor.CustomEditors {

	// TODO: update this to reference hidden scripts.
	public class EventPropertyEditor : WindowPropertyEditor {

		protected override bool IsReadOnly { get { return true; } }

		protected CustomEventDescriptor EventDescriptor {
			get { return PropertyItem.PropertyDescriptor as CustomEventDescriptor; }
		}
		protected EditorControl EditorControl {
			get { return EventDescriptor.EditorControl; }
		}

		protected override void OpenWindow() {
			Event evnt = EventDescriptor.Event;
			bool newScript = false;
			bool deleteScript = false;
			string oldCode = (evnt.IsDefined ? evnt.Script.Code : null);

			if (!evnt.IsDefined) {
				evnt.DefineScript();
				newScript = true;
			}
			Script script = evnt.Script;
			
			bool result = ScriptEditor.ShowCustomEditor(Window.GetWindow(Editor), script, EditorControl, false);
			if (result) {
				if (string.IsNullOrEmpty(script.Code)) {
					deleteScript = true;
					evnt.UndefineScript();
				}
			}
			else if (newScript) {
				deleteScript = true;
				evnt.UndefineScript();
			}

			if (!newScript || !deleteScript) {
				EditorAction action;
				if (newScript)
					action = ActionChangeEvent.CreateDefineEventAction(EventDescriptor.EventObject, evnt, script.Code);
				else if (deleteScript)
					action = ActionChangeEvent.CreateUndefineEventAction(EventDescriptor.EventObject, evnt, oldCode);
				else
					action = ActionChangeEvent.CreateChangeEventAction(EventDescriptor.EventObject, evnt, oldCode, script.Code);
				EditorControl.PushAction(action, ActionExecution.PostExecute);
			}

			// Send a dummy value to update the control
			SetValue("");
			if (evnt.Script != null)
				SetValue(EventDescriptor.GetValue(null) as string);
		}
	}
}
