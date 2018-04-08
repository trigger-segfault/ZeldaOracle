using System.Windows;
using ZeldaEditor.Control;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Scripting;
using ZeldaEditor.WinForms;
using ZeldaOracle.Game.Worlds;
using System.Windows.Input;

namespace ZeldaEditor.Windows {
	/// <summary>
	/// Interaction logic for ObjectEditor.xaml
	/// </summary>
	public partial class ObjectEditor : Window {

		// TriggerEditor triggerEditor
		private EditorControl editorControl;
		/// <summary>The object to edit properties for.</summary>
		private object obj;
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		private ObjectEditor(EditorControl editorControl, object obj) {
			InitializeComponent();
			
			this.editorControl = editorControl;
			this.obj = obj;
			// Set this before loading
			objectPreview.PreviewObject = obj;
		}
		
		public static ObjectEditor Show(Window owner, EditorControl editorControl,
			object obj = null)
		{
			ObjectEditor window = new ObjectEditor(editorControl, obj);
			window.Owner = owner;
			window.Show();
			return window;
		}
		
	
		//-----------------------------------------------------------------------------
		// Object Management
		//-----------------------------------------------------------------------------

		/// <summary>Set the object to show properties for.</summary>
		public void SetObject(object obj) {
			this.obj = obj;
			triggerEditor.SetObject(obj as ITriggerObject);

			if (obj is IVariableObject)
				objectVariableEditor.Variables = ((IVariableObject) obj).Variables;
			else
				objectVariableEditor.Variables = null;

			objectPreview.PreviewObject = obj;
		}


		//-----------------------------------------------------------------------------
		// UI Callbacks
		//-----------------------------------------------------------------------------

		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		private void OnLoaded(object sender, RoutedEventArgs e) {
			SetObject(obj);
		}

		private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e) {
			editorControl.NeedsRecompiling = true;
		}
	}
}
