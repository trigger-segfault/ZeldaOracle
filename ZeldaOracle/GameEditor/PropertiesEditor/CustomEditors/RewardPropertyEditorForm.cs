using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaEditor.Control;

namespace ZeldaEditor.PropertiesEditor.CustomEditors {
	public partial class RewardPropertyEditorForm : Form {
		private RewardManager rewardManager;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public RewardPropertyEditorForm(RewardManager rewardManager, string startValue) {
			this.rewardManager = rewardManager;

			InitializeComponent();
			
			buttonOkay.DialogResult		= DialogResult.OK;
			buttonCancel.DialogResult	= DialogResult.Cancel;
			
			// Add rewards to the list.
			listBox1.Items.Add("");
			textBox1.AutoCompleteCustomSource.Add("");
			textBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;
			foreach (KeyValuePair<string, Reward> entry in rewardManager.RewardDictionary) {
				listBox1.Items.Add(entry.Key);
				textBox1.AutoCompleteCustomSource.Add(entry.Key);
			}

			textBox1.Text = startValue;
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e) {
			if (ContainsFocus && listBox1.SelectedIndex >= 0)
				textBox1.Text = (string) listBox1.SelectedItem;
		}

		private void textBox1_TextChanged(object sender, EventArgs e) {
			if (listBox1.Items.Contains(textBox1.Text))
				listBox1.SelectedItem = textBox1.Text;
			else
				listBox1.ClearSelected();
		}

		private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e) {
			DialogResult = DialogResult.OK;
			Close();
		}

		private void textBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
			if (e.KeyCode == Keys.Up) {
				if (listBox1.SelectedIndex > 0) {
					textBox1.AutoCompleteMode = AutoCompleteMode.None;
					listBox1.SelectedIndex--;
					textBox1.Text = (string) listBox1.SelectedItem;
					textBox1.AutoCompleteMode = AutoCompleteMode.Append;
				}
			}
			else if (e.KeyCode == Keys.Down) {
				if (listBox1.SelectedIndex < listBox1.Items.Count - 1) {
					textBox1.AutoCompleteMode = AutoCompleteMode.None;
					listBox1.SelectedIndex++;
					textBox1.Text = (string) listBox1.SelectedItem;
					textBox1.AutoCompleteMode = AutoCompleteMode.Append;
				}
			}
		}

		private void listBox1_MeasureItem(object sender, MeasureItemEventArgs e) {
			e.ItemHeight = 20;
		}

		private void listBox1_DrawItem(object sender, DrawItemEventArgs e) {
			e.DrawBackground();
			
			string name = (string) listBox1.Items[e.Index];

			if (rewardManager.HasReward(name)) {
				Reward reward = rewardManager.GetReward(name);
				ZeldaOracle.Common.Graphics.Sprite sprite = null;

				Point2I position = (Point2I) e.Bounds.Location + new Point2I(2, 2);

				if (reward != null && reward.Animation != null)
					sprite = reward.Animation.Frames[0].Sprite;
				if (sprite != null) {
					EditorGraphics.DrawSprite(e.Graphics, sprite, position);
				}
			}

			int x = e.Bounds.Left + 20;
			int y = e.Bounds.Top;
			int width = e.Bounds.Right - x;
			int height = e.Bounds.Bottom - y;
			Rectangle textRect = new Rectangle(x, y, width, height);

			if (name == "")
				name = "(none)";

			Point2I textPosition = (Point2I) e.Bounds.Location + new Point2I(24, 0);
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Near;
			stringFormat.LineAlignment = StringAlignment.Center;
			e.Graphics.DrawString(name, textBox1.Font, new SolidBrush(System.Drawing.Color.Black), textRect, stringFormat);
			
			// Draw the focus rectangle if appropriate.
			e.DrawFocusRectangle();
		}
		

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public string RewardName {
			get { return textBox1.Text; }
			set { textBox1.Text = value; }
		}
	}

	public abstract class FormPropertyEditor : UITypeEditor {

		public abstract Form CreateForm(object value);

		public abstract object OnResultOkay(Form form, object value);

		public virtual object OnResultCancel(Form form, object value) {
			return value;
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}

		public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value) {
			IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			if (svc != null) {
				using (Form form = CreateForm(value)) {
					if (svc.ShowDialog(form) == DialogResult.OK)
						return OnResultOkay(form, value);
					else
						return OnResultCancel(form, value);
				}
			}
			return value;
		}
	}

	/*
	public class RewardPropertyEditor : UITypeEditor {
		private RewardManager rewardManager;

		public RewardPropertyEditor(RewardManager rewardManager) {
			this.rewardManager = rewardManager;
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}

		public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value) {
			IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			if (svc != null) {
				using (RewardPropertyEditorForm form = new RewardPropertyEditorForm(rewardManager)) {
					form.RewardName = (string) value;
					if (svc.ShowDialog(form) == DialogResult.OK)
						value = form.RewardName;
				}
			}
			return value;
		}
	}*/
	
	
	public class RewardPropertyEditor : FormPropertyEditor {
		private RewardManager rewardManager;

		public RewardPropertyEditor(RewardManager rewardManager) {
			this.rewardManager = rewardManager;
		}

		public override Form CreateForm(object value) {
			return new RewardPropertyEditorForm(rewardManager, (string) value);
		}

		public override object OnResultOkay(Form form, object value) {
			return ((RewardPropertyEditorForm) form).RewardName;
		}
		
		public override object OnResultCancel(Form form, object value) {
			return value;
		}
	}

	/*
	public class RewardPropertyEditor : UITypeEditor {
        private IWindowsFormsEditorService svc;
		private RewardManager rewardManager;
		private ListBox listBox;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public RewardPropertyEditor(RewardManager rewardManager) {
			this.rewardManager	= rewardManager;
			this.svc			= null;
			this.listBox		= new ListBox();
			
			// Add resources to the list.
			listBox.Items.Add("(none)");
			foreach (KeyValuePair<string, Reward> entry in rewardManager.RewardDictionary) {
				listBox.Items.Add(entry.Key);
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}

		public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value) {
			svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			
			if (svc != null)
			{
				listBox.SelectedValueChanged += new EventHandler(this.ValueChanged);
				svc.DropDownControl(listBox);

				if (listBox.SelectedIndex >= 0) {
					if (listBox.SelectedIndex == 0)
						value = "";
					else 
						value = (string) listBox.Items[listBox.SelectedIndex];
				}
			}
			return value;
		}

        private void ValueChanged(object sender, EventArgs e) {
            if (svc != null)
                svc.CloseDropDown();
        }
	}
		*/
}
