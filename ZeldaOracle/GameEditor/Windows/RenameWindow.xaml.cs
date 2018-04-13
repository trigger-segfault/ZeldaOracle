using System.Windows;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Worlds;
using ZeldaWpf.Windows;

namespace ZeldaEditor.Windows {
	/// <summary>
	/// Interaction logic for RenameWindow.xaml
	/// </summary>
	public partial class RenameWindow : Window {

		World world;
		IIDObject target;

		public RenameWindow(World world, IIDObject target) {
			InitializeComponent();

			this.world = world;
			this.target = target;

			textBox.Text = target.ID;
			textBox.Focus();
			textBox.SelectAll();
		}

		private void OnOK(object sender = null, RoutedEventArgs e = null) {
			string newID = textBox.Text;
			IIDObject containedID = null;
			if (string.IsNullOrWhiteSpace(newID)) {
				TriggerMessageBox.Show(this, MessageIcon.Warning,
					"ID cannot be empty or whitespace!", "Invalid ID");
				return;
			}
			else if (target.ID != newID) {
				if (target is Level) {
					containedID = world.GetLevel(newID);
				}
				else if (target is Area) {
					containedID = world.GetArea(newID);
				}
				else if (target is Script) {
					if (!ScriptManager.IsValidScriptName(newID)) {
						TriggerMessageBox.Show(this, MessageIcon.Warning,
							"Script ID must start with a letter or underscore and can only contain letters, digits, and underscores!", "Invalid Script ID");
						return;
					}
					containedID = world.GetScript(newID);
				}
				else if (target is Zone) {
					//containedID = world.GetScript(target.ID);
				}
			}
			if (containedID != null) {
				TriggerMessageBox.Show(this, MessageIcon.Warning,
					"A " + target.GetType().Name.ToLower() + " with that ID already exists!", "ID Taken");
			}
			else {
				DialogResult = true;
				Close();
			}
		}

		public static string Show(Window owner, World world, IIDObject target) {
			RenameWindow window = new RenameWindow(world, target);
			window.Owner = owner;
			var result = window.ShowDialog();
			if (result.HasValue && result.Value) {
				return window.textBox.Text;
			}
			return null;
		}
	}
}
