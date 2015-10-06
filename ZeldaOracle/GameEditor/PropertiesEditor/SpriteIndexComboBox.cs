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

namespace ZeldaEditor.PropertiesEditor {
	public class SpriteIndexComboBox : UITypeEditor {
        private IWindowsFormsEditorService svc = null;
		private PropertyGridControl propertyGridControl;

		public SpriteIndexComboBox(PropertyGridControl propertyGridControl) {
			this.propertyGridControl = propertyGridControl;
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}

		public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value) {
			svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			if (svc != null && propertyGridControl.TileData.SpriteList.Length > 0)
			{
				ListBox comboBox = new ListBox();
				comboBox.BorderStyle = BorderStyle.None;
				comboBox.DrawItem += OnDrawItem;
				comboBox.DrawMode = DrawMode.OwnerDrawFixed;
				comboBox.ItemHeight = 20;
				for (int i = 0; i < propertyGridControl.TileData.SpriteList.Length; i++) {
					comboBox.Items.Add(i);
				}

				comboBox.SelectedValueChanged += new EventHandler(this.ValueChanged);
				svc.DropDownControl(comboBox);
				value = GMath.Clamp(comboBox.SelectedIndex, 0, GMath.Max(propertyGridControl.TileData.SpriteList.Length - 1, 0));

			}
			value = GMath.Clamp((int)value, 0, GMath.Max(propertyGridControl.TileData.SpriteList.Length - 1, 0));
			return value; // can also replace the wrapper object here
		}

        private void ValueChanged(object sender, EventArgs e) {
			if (svc != null) {
                svc.CloseDropDown();
            }
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
