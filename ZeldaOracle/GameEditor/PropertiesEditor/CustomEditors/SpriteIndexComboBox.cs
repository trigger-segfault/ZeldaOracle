using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ZeldaEditor.Control;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;

namespace ZeldaEditor.PropertiesEditor.CustomEditors {
	public class SpriteIndexComboBox : DropDownPropertyEditor {

		public SpriteIndexComboBox() {
		}

		public override void CreateList(ListBox listBox, object value) {
			listBox.DrawItem += OnDrawItem;
			listBox.DrawMode = DrawMode.OwnerDrawFixed;
			listBox.ItemHeight = 20;

			for (int i = 0; i < propertyGridControl.TileData.SpriteList.Length; i++) {
				listBox.Items.Add(i);
			}
		}

		public override object OnItemSelected(ListBox listBox, int index, object value) {
			return index;
		}

		private void OnDrawItem(object sender, DrawItemEventArgs e) {
			e.DrawBackground();
			ListBox listBox = sender as ListBox;
			Brush brush = new SolidBrush(e.ForeColor);
			Rectangle bounds = new Rectangle(e.Bounds.X + 20, e.Bounds.Y + 3, e.Bounds.Width, e.Bounds.Height);
			e.Graphics.DrawString(listBox.Items[e.Index].ToString(), e.Font, brush, bounds, StringFormat.GenericDefault);
			EditorGraphics.DrawSprite(e.Graphics, propertyGridControl.TileData.SpriteList[e.Index], propertyGridControl.TileData.Room.Zone.ImageVariantID, (Point2I)(e.Bounds.Location) + new Point2I(2, 2), new Point2I(16, 16));
			brush.Dispose();
		}
	}
}
