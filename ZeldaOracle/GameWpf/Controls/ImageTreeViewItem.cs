using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ZeldaWpf.Resources;

namespace ZeldaWpf.Controls {
	/// <summary>Wrapper arguments for MouseButtonEventArgs because it doesn't
	/// support a proper constructor.</summary>
	public class ItemMouseButtonEventArgs : RoutedEventArgs {
		/// <summary>The actual MouseButtonEventArgs.</summary>
		public MouseButtonEventArgs Args { get; set; }

		/// <summary>Constructs the ItemMouseButtonEventArgs.</summary>
		public ItemMouseButtonEventArgs(RoutedEvent routedEvent, MouseButtonEventArgs e) :
			base(routedEvent) {
			Args = e;
		}
	}

	/// <summary>Wrapper arguments for MouseButtonEventArgs handler because it
	/// doesn't support a proper constructor.</summary>
	public delegate void ItemMouseButtonEventHandler(object sender, ItemMouseButtonEventArgs e);

	/// <summary>A tree view item that also displays an image.</summary>
	public class ImageTreeViewItem : TreeViewItem {

		//-----------------------------------------------------------------------------
		// Static Members
		//-----------------------------------------------------------------------------

		/// <summary>The default empty context menu.</summary>
		private static ContextMenu NullContextMenu;


		//-----------------------------------------------------------------------------
		// Routed Events
		//-----------------------------------------------------------------------------

		/// <summary>The routed event for when the mouse is down on the item only.</summary>
		public static readonly RoutedEvent ItemMouseDownEvent =
			EventManager.RegisterRoutedEvent("ItemMouseDown", RoutingStrategy.Bubble,
				typeof(ItemMouseButtonEventHandler), typeof(ImageTreeViewItem));

		/// <summary>Occurs when the mouse is down on the item. This is needed
		/// because the tree view lines and expanders shouldn't be triggering
		/// mouse down.</summary>
		public event ItemMouseButtonEventHandler ItemMouseDown {
			add { AddHandler(ItemMouseDownEvent, value); }
			remove { RemoveHandler(ItemMouseDownEvent, value); }
		}


		//-----------------------------------------------------------------------------
		// Dependency Properties
		//-----------------------------------------------------------------------------

		/// <summary>The dependency property for the tree view item's image.</summary>
		public static readonly DependencyProperty SourceProperty =
			DependencyProperty.RegisterAttached("Source", typeof(ImageSource),
				typeof(ImageTreeViewItem));

		/// <summary>Gets or sets the source of the tree view item's image.</summary>
		[Category("Common")]
		public ImageSource Source {
			get { return (ImageSource) GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

		/// <summary>The dependency property for the tree view item's expanded image.</summary>
		public static readonly DependencyProperty SourceExpandedProperty =
			DependencyProperty.RegisterAttached("SourceExpanded", typeof(ImageSource),
				typeof(ImageTreeViewItem));

		/// <summary>Gets or sets the source of the tree view item's expanded image.</summary>
		[Category("Common")]
		public ImageSource SourceExpanded {
			get { return (ImageSource) GetValue(SourceExpandedProperty); }
			set { SetValue(SourceExpandedProperty, value); }
		}

		/// <summary>The dependency property for the tree view item's collapsed image.</summary>
		public static readonly DependencyProperty SourceCollapsedProperty =
			DependencyProperty.RegisterAttached("SourceCollapsed", typeof(ImageSource),
				typeof(ImageTreeViewItem));

		/// <summary>Gets or sets the source of the tree view item's collapsed image.</summary>
		[Category("Common")]
		public ImageSource SourceCollapsed {
			get { return (ImageSource) GetValue(SourceCollapsedProperty); }
			set { SetValue(SourceCollapsedProperty, value); }
		}

		/// <summary>The dependency property for the opacity of tree view item's image.</summary>
		public static readonly DependencyProperty ImageOpacityProperty =
			DependencyProperty.RegisterAttached("ImageOpacity", typeof(double),
				typeof(ImageTreeViewItem), new FrameworkPropertyMetadata(1.0));

		/// <summary>Gets or sets the opacity of tree view item's image.</summary>
		public double ImageOpacity {
			get { return (double) GetValue(ImageOpacityProperty); }
			set { SetValue(ImageOpacityProperty, value); }
		}

		/// <summary>The dependency property for if  the tree view item's is
		/// highlighted.</summary>
		public static readonly DependencyProperty IsHighlightedProperty =
			DependencyProperty.RegisterAttached("IsHighlighted", typeof(bool),
				typeof(ImageTreeViewItem), new FrameworkPropertyMetadata(false));

		/// <summary>Gets or sets if the tree view item is highlighted.</summary>
		public bool IsHighlighted {
			get { return (bool) GetValue(IsHighlightedProperty); }
			set { SetValue(IsHighlightedProperty, value); }
		}


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the image tree view item default style.</summary>
		static ImageTreeViewItem() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageTreeViewItem),
					   new FrameworkPropertyMetadata(typeof(ImageTreeViewItem)));

			NullContextMenu = new ContextMenu();
			NullContextMenu.Visibility = Visibility.Hidden;
		}

		/// <summary>Constructs an empty tree view item.</summary>
		public ImageTreeViewItem() {
			ContextMenu = NullContextMenu;
		}

		/// <summary>Constructs an tree view item with an image and header.</summary>
		public ImageTreeViewItem(ImageSource source, object header, bool expanded)
			: this()
		{
			Source = source;
			Header = header;
			IsExpanded = expanded;
		}

		/// <summary>Constructs an tree view item with an images and header.</summary>
		public ImageTreeViewItem(ImageSource sourceExpanded,
			ImageSource sourceCollapsed, object header, bool expanded) : this()
		{
			SourceExpanded = sourceExpanded;
			SourceCollapsed = sourceCollapsed;
			Header = header;
			IsExpanded = expanded;
			UpdateExpandedSource();
		}

		/// <summary>Constructs a copy of the tree view item's images, header, and
		/// expanded state.</summary>
		public ImageTreeViewItem(ImageTreeViewItem copy) : this() {
			Source = copy.Source;
			SourceExpanded = copy.SourceExpanded;
			SourceCollapsed = copy.SourceCollapsed;
			Header = copy.Header;
			IsExpanded = copy.IsExpanded;
			UpdateExpandedSource();
		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		/// <summary>Applies the mouse down event for just the tree view item.</summary>
		public override void OnApplyTemplate() {
			((FrameworkElement) GetTemplateChild("Bd")).PreviewMouseDown +=
				OnItemPreviewMouseDown;
		}

		/// <summary>Applies the expended source if one exists.</summary>
		protected override void OnExpanded(RoutedEventArgs e) {
			base.OnExpanded(e);
			UpdateExpandedSource();
		}

		/// <summary>Applies the collapsed source if one exists.</summary>
		protected override void OnCollapsed(RoutedEventArgs e) {
			base.OnCollapsed(e);
			UpdateExpandedSource();
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		/// <summary>Triggers the item mouse down event.</summary>
		private void OnItemPreviewMouseDown(object sender, MouseButtonEventArgs e) {
			RaiseEvent(new ItemMouseButtonEventArgs(ItemMouseDownEvent, e));
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Updates the source to refer to the expanded or collapsed source.</summary>
		protected void UpdateExpandedSource() {
			if (IsExpanded && SourceExpanded != null)
				Source = SourceExpanded;
			else if (!IsExpanded && SourceCollapsed != null)
				Source = SourceCollapsed;
		}
	}
}
