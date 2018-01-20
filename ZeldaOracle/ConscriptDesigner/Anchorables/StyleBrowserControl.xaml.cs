using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ConscriptDesigner.Control;
using ConscriptDesigner.Util;
using ConscriptDesigner.WinForms;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaResources = ZeldaOracle.Common.Content.Resources;

namespace ConscriptDesigner.Anchorables {
	/// <summary>
	/// Interaction logic for SpriteBrowserControl.xaml
	/// </summary>
	public partial class StyleBrowserControl : UserControl {

		private StylePreview preview;
		
		private string styleGroup;
		private Point2I spriteSize;
		private Dictionary<string, List<SpriteInfo>> styleGroups;
		private List<string> orderedStyleGroups;
		private List<SpriteInfo> filteredSprites;

		private bool suppressEvents;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the style browser control.</summary>
		public StyleBrowserControl() {
			this.suppressEvents = true;
			InitializeComponent();

			this.preview = new StylePreview();
			this.preview.HoverChanged += OnHoverChanged;
			this.host.Child = this.preview;
			this.styleGroup = "";
			this.spriteSize = Point2I.One;
			this.styleGroups = new Dictionary<string, List<SpriteInfo>>();
			this.orderedStyleGroups = new List<string>();
			this.filteredSprites = new List<SpriteInfo>();

			DesignerControl.ResourcesLoaded += OnResourcesLoaded;
			DesignerControl.ResourcesUnloaded += OnResourcesUnloaded;
			DesignerControl.PreviewInvalidated += OnPreviewInvalidated;
			DesignerControl.PreviewScaleChanged += OnPreviewScaleChanged;
			
			this.suppressEvents = false;
		}


		//-----------------------------------------------------------------------------
		// Loading
		//-----------------------------------------------------------------------------

		public void Dispose() {
			DesignerControl.ResourcesLoaded -= OnResourcesLoaded;
			DesignerControl.ResourcesUnloaded -= OnResourcesUnloaded;
			DesignerControl.PreviewInvalidated -= OnPreviewInvalidated;
			DesignerControl.PreviewScaleChanged -= OnPreviewScaleChanged;
			preview.Dispose();
		}

		public void Reload() {
			suppressEvents = true;

			styleGroups.Clear();
			orderedStyleGroups.Clear();
			foreach (StyleGroupCollection collection in ZeldaResources.GetRegisteredStyles()) {
				AddStyleGroup(collection.Group);
				foreach (string style in collection.Styles) {
					AddStyle(collection.Group, style, collection.Preview);
				}
			}
			orderedStyleGroups.Sort((a, b) => AlphanumComparator.Compare(a, b, true));
			if (!styleGroups.ContainsKey(styleGroup) && orderedStyleGroups.Any()) {
				styleGroup = orderedStyleGroups[0];
			}

			comboBoxStyleGroups.Items.Clear();
			foreach (string styleGroup in orderedStyleGroups) {
				ComboBoxItem item = new ComboBoxItem();
				item.Content = styleGroup;
				item.Tag = styleGroup;
				comboBoxStyleGroups.Items.Add(item);
			}
			comboBoxStyleGroups.SelectedIndex = orderedStyleGroups.IndexOf(styleGroup);

			UpdateStyleGroup();
			OnHoverChanged();

			suppressEvents = false;
		}

		public void Unload() {
			preview.Unload();
		}


		//-----------------------------------------------------------------------------
		// Sprites Setup
		//-----------------------------------------------------------------------------

		private void UpdateStyleGroup() {
			spriteSize = Point2I.One;
			List<SpriteInfo> styles;
			if (styleGroups.TryGetValue(styleGroup, out styles)) {
				foreach (SpriteInfo style in styles) {
					spriteSize = GMath.Max(spriteSize, style.Bounds.Size);
				}
			}
			UpdateFilter();
		}

		private void UpdateFilter() {
			string filter = textBoxSearch.Text;
			filteredSprites = new List<SpriteInfo>();
			if (!string.IsNullOrEmpty(filter)) {
				foreach (var spr in GetStyleList()) {
					if (spr.Name.Contains(filter)) {
						filteredSprites.Add(spr);
					}
				}
			}
			else {
				filteredSprites = GetStyleList();
			}
			preview.UpdateList(filteredSprites, spriteSize, styleGroup);
		}

		private void AddStyleGroup(string styleGroup) {
			if (!styleGroups.ContainsKey(styleGroup)) {
				styleGroups.Add(styleGroup, new List<SpriteInfo>());
				orderedStyleGroups.Add(styleGroup);
			}
		}

		private void AddStyle(string styleGroup, string style, ISprite sprite) {
			SpriteInfo spr = new SpriteInfo(style, sprite);
			styleGroups[styleGroup].Add(spr);
		}

		private List<SpriteInfo> GetStyleList() {
			if (styleGroups.ContainsKey(styleGroup))
				return styleGroups[styleGroup];
			return new List<SpriteInfo>();
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		private void OnResourcesLoaded(object sender = null, EventArgs e = null) {
			Reload();
		}

		private void OnResourcesUnloaded(object sender = null, EventArgs e = null) {
			Unload();
		}

		private void OnPreviewInvalidated(object sender = null, EventArgs e = null) {
			preview.Invalidate();
		}

		private void OnPreviewScaleChanged(object sender = null, EventArgs e = null) {
			preview.UpdateScale();
		}

		private void OnHoverChanged(object sender = null, EventArgs e = null) {
			SpriteInfo hoverSprite = preview.HoverSprite;
			if (hoverSprite == null) {
				statusSpriteStyle.Content = "";
				statusSpriteInfo.Content = "";
			}
			else {
				string info = "Type: " + hoverSprite.Sprite.GetType().Name.Replace("Sprite", "");
				if (hoverSprite.HasSubstrips)
					info += ", Substrip: " + hoverSprite.SubstripIndex;
				statusSpriteInfo.Content = info;
				statusSpriteStyle.Content = hoverSprite.Name;
			}
		}

		private void OnSearchTextChanged(object sender, TextChangedEventArgs e) {
			if (suppressEvents) return;
			UpdateFilter();
		}

		private void OnStyleGroupChanged(object sender, SelectionChangedEventArgs e) {
			if (suppressEvents) return;
			if (comboBoxStyleGroups.SelectedIndex != -1) {
				styleGroup = (string) ((ComboBoxItem) comboBoxStyleGroups.SelectedItem).Tag;
				UpdateStyleGroup();
				UpdateFilter();
			}
		}
	}
}
