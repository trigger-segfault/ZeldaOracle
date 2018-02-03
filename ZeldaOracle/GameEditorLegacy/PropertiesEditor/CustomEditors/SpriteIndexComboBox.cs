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
using ZeldaOracle.Game.Tiles;

namespace ZeldaEditor.PropertiesEditor.CustomEditors {
	public class SpriteIndexComboBox : DropDownPropertyEditor {

		public SpriteIndexComboBox() {
		}

		public override void CreateList(ListBox listBox, object value) {
			listBox.DrawItem += OnDrawItem;
			listBox.DrawMode = DrawMode.OwnerDrawFixed;
			listBox.ItemHeight = 20;

			if (PropertyGrid.PropertyObject is TileDataInstance) {
				TileDataInstance tile = (TileDataInstance) PropertyGrid.PropertyObject;
				for (int i = 0; i < tile.SpriteList.Length; i++) {
					listBox.Items.Add(i);
				}
			}
		}

		public override object OnItemSelected(ListBox listBox, int index, object value) {
			return index;
		}

		private void OnDrawItem(object sender, DrawItemEventArgs e) {
			ListBox listBox = sender as ListBox;
			Brush brush = new SolidBrush(e.ForeColor);
			Rectangle bounds = new Rectangle(e.Bounds.X + 20, e.Bounds.Y + 3, e.Bounds.Width, e.Bounds.Height);

			e.DrawBackground();

			// Draw the label.
			e.Graphics.DrawString(listBox.Items[e.Index].ToString(), e.Font, brush, bounds, StringFormat.GenericDefault);

			// Draw the sprite.
			if (PropertyGrid.PropertyObject is TileDataInstance) {
				TileDataInstance tile = (TileDataInstance) PropertyGrid.PropertyObject;
				EditorGraphics.DrawSprite(e.Graphics, tile.SpriteList[e.Index],
					(Point2I) (e.Bounds.Location) +
					new Point2I(2, 2), new Point2I(16, 16));
			}

			brush.Dispose();
		}
	}
}
