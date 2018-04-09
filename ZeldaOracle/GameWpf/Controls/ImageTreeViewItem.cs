using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ZeldaWpf.Controls {
	/// <summary>A tree view item that also displays an image.</summary>
	public class ImageTreeViewItem2 : TreeViewItem {

		/// <summary>The default empty context menu.</summary>
		private static ContextMenu NullContextMenu;

		/// <summary>The dependency property for the tree view item's image.</summary>
		public static readonly DependencyProperty SourceProperty =
			DependencyProperty.RegisterAttached(
			"Source", typeof(ImageSource), typeof(ImageTreeViewItem2));

		/// <summary>Gets or sets the source of the tree view item's image.</summary>
		[Category("Common")]
		public ImageSource Source {
			get { return (ImageSource)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

		/// <summary>Initializes the image tree view item default style.</summary>
		static ImageTreeViewItem2() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageTreeViewItem2),
					   new FrameworkPropertyMetadata(typeof(ImageTreeViewItem2)));

			NullContextMenu = new ContextMenu();
			NullContextMenu.Visibility = Visibility.Hidden;
		}

		/// <summary>Constructs an empty tree view item.</summary>
		public ImageTreeViewItem2() : this(null, null, false) { }

		/// <summary>Constructs an tree view item with an image and name.</summary>
		public ImageTreeViewItem2(ImageSource source, string name, bool expanded) {
			Source = source;
			Header = name;
			IsExpanded = expanded;
			ContextMenu = NullContextMenu;
		}
	}

	public class ItemMouseButtonEventArgs : RoutedEventArgs {
		public MouseButtonEventArgs Args { get; set; }

		public ItemMouseButtonEventArgs(RoutedEvent routedEvent, MouseButtonEventArgs e) :
			base(routedEvent) {
			this.Args = e;
		}
	}

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
		public ImageTreeViewItem() : this(null, null, false) { }

		/// <summary>Constructs an tree view item with an image and name.</summary>
		public ImageTreeViewItem(ImageSource source, string name, bool expanded) {
			Source = source;
			Header = name;
			IsExpanded = expanded;
			ContextMenu = NullContextMenu;
		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		/// <summary>Applies the mouse down event for just the tree view item.</summary>
		public override void OnApplyTemplate() {
			((FrameworkElement) GetTemplateChild("Bd")).PreviewMouseDown +=
				OnItemPreviewMouseDown;
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		/// <summary>Triggers the item mouse down event.</summary>
		private void OnItemPreviewMouseDown(object sender, MouseButtonEventArgs e) {
			RaiseEvent(new ItemMouseButtonEventArgs(ItemMouseDownEvent, e));
		}
	}
}
