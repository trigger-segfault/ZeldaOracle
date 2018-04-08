using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using ZeldaEditor.Control;
using ZeldaEditor.Util;
using ZeldaEditor.WinForms;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaEditor.Controls {
	/// <summary>A control for previewing a Zelda object.</summary>
	public partial class ObjectPreview : TimersUserControl {

		//-----------------------------------------------------------------------------
		// Dependency Properties
		//-----------------------------------------------------------------------------

		/// <summary>The dependency property for the previewed object.</summary>
		public static DependencyProperty PreviewObjectProperty =
			DependencyProperty.Register("PreviewObject", typeof(object),
				typeof(ObjectPreview),
				new FrameworkPropertyMetadata(OnPreviewObjectChanged));

		/// <summary>Updates the object preview.</summary>
		private static void OnPreviewObjectChanged(object sender,
			DependencyPropertyChangedEventArgs e)
		{
			ObjectPreview preview = sender as ObjectPreview;
			preview?.PreviewObjectChanged();
		}

		/// <summary>Gets or sets the previewed object.</summary>
		public object PreviewObject {
			get { return GetValue(PreviewObjectProperty); }
			set { SetValue(PreviewObjectProperty, value); }
		}


		/// <summary>The dependency property for the background color.</summary>
		public static DependencyProperty PreviewBackgroundProperty =
			DependencyProperty.Register("PreviewBackground", typeof(Color),
				typeof(ObjectPreview),
				new FrameworkPropertyMetadata(OnPreviewBackgroundChanged));

		/// <summary>Updates the tile preview's background.</summary>
		private static void OnPreviewBackgroundChanged(object sender,
			DependencyPropertyChangedEventArgs e) {
			ObjectPreview preview = sender as ObjectPreview;
			if (preview == null || DesignerProperties.GetIsInDesignMode(preview))
				return;

			preview.tilePreview.Background = new ZeldaOracle.Common.Graphics.Color(
				preview.PreviewBackground.R,
				preview.PreviewBackground.G,
				preview.PreviewBackground.B);
		}

		/// <summary>Gets or sets the background color.</summary>
		public Color PreviewBackground {
			get { return (Color) GetValue(PreviewBackgroundProperty); }
			set { SetValue(PreviewBackgroundProperty, value); }
		}


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The WinForms control for drawing tile previews.</summary>
		private TilePreview tilePreview;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the object previewer.</summary>
		public ObjectPreview() {
			InitializeComponent();

			if (!DesignerProperties.GetIsInDesignMode(this)) {
				tilePreview = new TilePreview();
				tilePreview.Name = "tilePreview";
				tilePreview.Dock = System.Windows.Forms.DockStyle.Fill;
				host.Child = tilePreview;
			}
			else {
				host.Visibility = Visibility.Hidden;
			}
		}

		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		/// <summary>Updates the object preview.</summary>
		private void PreviewObjectChanged() {
			if (DesignerProperties.GetIsInDesignMode(this))
				return;
			object obj = PreviewObject;
			
			// The scheduled events below prevent white flickering
			// when transitioning from WinForms to Wpf images.
			if (obj is Room) {
				Room room = obj as Room;
				image.Source = EditorImages.Room;
				previewName.Text = "Room[" + room.Location.X + ", " + room.Location.Y + "]";
				ScheduleHideTilePreview();
			}
			else if (obj is Level) {
				image.Source = EditorImages.Level;
				previewName.Text = (obj as Level).ID;
				ScheduleHideTilePreview();
			}
			else if (obj is Area) {
				image.Source = EditorImages.Area;
				previewName.Text = (obj as Area).ID;
				ScheduleHideTilePreview();
			}
			else if (obj is World) {
				image.Source = EditorImages.World;
				previewName.Text = (obj as World).ID;
				ScheduleHideTilePreview();
			}
			else if (obj is BaseTileDataInstance) {
				host.Visibility = Visibility.Visible;
				BaseTileDataInstance tile = obj as BaseTileDataInstance;
				tilePreview.Tile = tile;
				image.Source = null;
				previewName.Text = tile.BaseData.ResourceName;
			}
			else {
				image.Source = null;
				previewName.Text = "";
				HideTilePreview();
			}
		}


		//-----------------------------------------------------------------------------
		// Hide Tile Preview
		//-----------------------------------------------------------------------------

		/// <summary>Schedules an event to hide the tile preview.</summary>
		private void ScheduleHideTilePreview() {
			ScheduledEvents.Start(0.02, TimerPriority.High, HideTilePreview);
		}

		/// <summary>Immediately hides the tile preview.</summary>
		private void HideTilePreview() {
			host.Visibility = Visibility.Hidden;
			tilePreview.Tile = null;
		}
	}
}
