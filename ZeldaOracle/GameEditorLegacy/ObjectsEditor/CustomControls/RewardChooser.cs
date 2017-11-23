using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Common.Geometry;
using ZeldaEditor.Control;

namespace ZeldaEditor.ObjectsEditor.CustomControls {
	public partial class RewardChooser : UserControl {

		private RewardManager rewardManager;

		public RewardChooser() {
			InitializeComponent();
			rewardManager = null;
		}

		public RewardChooser(RewardManager rewardManager) {
			InitializeComponent();
			Setup(rewardManager);
		}

		public void Setup(RewardManager rewardManager) {
			this.rewardManager = rewardManager;

			listBox1.Items.Clear();
			textBox1.AutoCompleteCustomSource.Clear();
			
			// Add rewards to the list.
			listBox1.Items.Add("");
			textBox1.AutoCompleteCustomSource.Add("");
			textBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;
			foreach (KeyValuePair<string, Reward> entry in rewardManager.RewardDictionary) {
				listBox1.Items.Add(entry.Key);
				textBox1.AutoCompleteCustomSource.Add(entry.Key);
			}

			textBox1.Text = "";
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
		/*
		private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e) {
			DialogResult = DialogResult.OK;
			Close();
		}*/

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
}
