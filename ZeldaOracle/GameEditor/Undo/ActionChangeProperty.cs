using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Game.Worlds;
using ZeldaEditor.Control;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Scripting;

namespace ZeldaEditor.Undo {
	public class ActionChangeProperty : EditorAction {

		private IPropertyObject propertyObject;
		private string propertyName;
		private object oldValue;
		private object newValue;

		public ActionChangeProperty(IPropertyObject propertyObject, Property property, object oldValue, object newValue) {
			this.propertyObject = propertyObject;
			this.propertyName = property.Name;
			string realName = property.Name;
			if (property.HasDocumentation && !string.IsNullOrWhiteSpace(property.Documentation.ReadableName))
				realName = property.Documentation.ReadableName;
			ActionName = "Change '" + realName + "' " + "Property";
			ActionIcon = EditorImages.Property;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}

		public override void Undo(EditorControl editorControl) {
			propertyObject.Properties.SetGeneric(propertyName, oldValue);
			if (editorControl.PropertyGrid.PropertyObject == propertyObject) {
				editorControl.PropertyGrid.Update();
			}
		}

		public override void Redo(EditorControl editorControl) {
			propertyObject.Properties.SetGeneric(propertyName, newValue);
			if (editorControl.PropertyGrid.PropertyObject == propertyObject) {
				editorControl.PropertyGrid.Update();
			}
		}

		public IPropertyObject PropertyObject {
			get { return propertyObject; }
		}

		public string PropertyName {
			get { return propertyName; }
		}

		public object OldValue {
			get { return oldValue; }
			set { oldValue = value; }
		}

		public object NewValue {
			get { return newValue; }
			set { newValue = value; }
		}

		public override bool IgnoreAction { get { return oldValue.Equals(newValue); } }
	}
}
