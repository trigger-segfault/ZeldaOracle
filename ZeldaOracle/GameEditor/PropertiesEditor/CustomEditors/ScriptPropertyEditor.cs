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
using ZeldaEditor.Windows;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Tiles;

namespace ZeldaEditor.PropertiesEditor.CustomEditors {

	// TODO: update this to reference hidden scripts.
	public class ScriptPropertyEditor : WindowPropertyEditor {
		
		protected override bool IsReadOnly { get { return true; } }

		protected override void OpenWindow(PropertyItem propertyItem) {
			Script script = null;
			bool wasGenerated = false;
			string scriptName = (string)propertyItem.Value;

			// Open the script this property is referencing.
			if (scriptName.Length > 0)
				script = EditorControl.Instance.World.GetScript(scriptName);

			// If it is null, create a new hidden script.
			if (script == null) {
				script = EditorControl.Instance.GenerateInternalScript();
				wasGenerated = true;


				CustomPropertyDescriptor descriptor = (CustomPropertyDescriptor)propertyItem.PropertyDescriptor;
				if (descriptor.PropertyObject is IEventObject) {
					ObjectEvent evt = ((IEventObject) descriptor.PropertyObject).Events.GetEvent(descriptor.Name);
					if (evt != null)
						script.Parameters = evt.Parameters;
				}
			}

			bool result = ScriptEditor.Show(Window.GetWindow(Editor), script, EditorControl.Instance, false);
			if (result) {
				SetValue(script.ID);
			}
			else if (wasGenerated) {
				EditorControl.Instance.World.RemoveScript(script);
			}
		}
	}
}
