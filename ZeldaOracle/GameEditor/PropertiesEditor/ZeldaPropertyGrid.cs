using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using ZeldaEditor.Control;
using ZeldaEditor.Undo;
using ZeldaEditor.Windows;
using ZeldaEditor.WinForms;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Tiles;

namespace ZeldaEditor.PropertiesEditor {
	public class ZeldaPropertyGrid : PropertyGrid {

		private EditorControl editorControl;
		private static IPropertyObject propertyObject;
		//private PropertiesContainer propertiesContainer;
		private WpfFocusMessageFilter messageFilter;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ZeldaPropertyGrid() {
			editorControl		= null;
			propertyObject		= null;
			//propertiesContainer = null;// new PropertiesContainer(this);

			//this.SelectedObject			= propertiesContainer;
			this.PropertyValueChanged	+= OnPropertyChange;
			this.IsMiscCategoryLabelHidden = false;
			this.ShowAdvancedOptions	= false;
			this.AdvancedOptionsMenu	= null;
			FocusManager.SetIsFocusScope(this, true);

			// Ignore mouse buttons to prevent breaking comboboxes.
			this.messageFilter = new WpfFocusMessageFilter(this);
			this.messageFilter.AddFilter(true);
		}

		
		//-----------------------------------------------------------------------------
		// Properties Methods
		//-----------------------------------------------------------------------------

		public void OpenProperties(IPropertyObject propObject) {
			if (propObject != propertyObject) {
				propertyObject = propObject;
				editorControl.EditorWindow.UpdatePropertyPreview(propertyObject);
				SelectedObject = new PropertiesContainer(this, propertyObject);
			}
		}

		public void UpdateProperties() {
			if (SelectedObject != null)
				Update();
		}

		public void RefreshProperties() {
			if (SelectedObject != null) {
				SelectedObject = new PropertiesContainer(this, propertyObject);
			}
		}

		public void CloseProperties() {
			if (propertyObject != null) {
				propertyObject = null;
				SelectedObject = new PropertiesContainer(this, null);
				editorControl.EditorWindow.UpdatePropertyPreview(propertyObject);
			}
		}


		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		private void OnPropertyChange(object sender, PropertyValueChangedEventArgs e) {
			PropertyDescriptor baseDescriptor =
				((PropertyItem) e.OriginalSource).PropertyDescriptor;
			if (baseDescriptor is CustomPropertyDescriptor) {
				CustomPropertyDescriptor propertyDescriptor =
					(CustomPropertyDescriptor) baseDescriptor;
				Property property = propertyDescriptor.Property;

				editorControl.PushPropertyAction(propertyObject,
					property.Name, e.OldValue, e.NewValue, ActionExecution.None);

				/*ActionChangeProperty action = new ActionChangeProperty(
					propertyObject, property, e.OldValue, e.NewValue);

				// Special behavior for updating changes to tile size
				bool isTileSize = (property.Name == "size" &&
									propertyObject is TileDataInstance);
				if (isTileSize) {
					action = new ActionChangeTileSizeProperty(
						(TileDataInstance) propertyObject, property,
						(Point2I) e.OldValue, (Point2I) e.NewValue);
					editorControl.PushAction(action, ActionExecution.Execute);
					return;
				}

				if (editorControl.LastAction is ActionChangeProperty) {
					ActionChangeProperty lastAction =
						(ActionChangeProperty) editorControl.LastAction;
					if (action.PropertyObject == lastAction.PropertyObject &&
						action.PropertyName == lastAction.PropertyName)
					{
						action.OldValue = lastAction.OldValue;
						editorControl.PopAction();
					}
				}

				editorControl.PushAction(action, ActionExecution.None);*/
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public EditorControl EditorControl {
			get { return editorControl; }
			set { editorControl = value; }
		}

		public Properties PropertyList {
			get { return propertyObject.Properties; }
		}

		public IPropertyObject PropertyObject {
			get { return propertyObject; }
		}

		public IEventObject EventObject {
			get { return propertyObject as IEventObject; }
		}
	}
}
