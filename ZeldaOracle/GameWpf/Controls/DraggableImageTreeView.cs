using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ZeldaWpf.Controls {
	/// <summary>Arguments used to determine if a tree view item can be dragged.</summary>
	public class TreeViewCanDragEventArgs : RoutedEventArgs {
		/// <summary>The item that wants to be dragged.</summary>
		public ImageTreeViewItem Item { get; }
		/// <summary>Set this value to determine if the item can be dragged.</summary>
		public bool CanDrag { get; set; }

		/// <summary>Constructs the event args.</summary>
		public TreeViewCanDragEventArgs(RoutedEvent routedEvent,
			ImageTreeViewItem item) : base(routedEvent)
		{
			Item = item;
			CanDrag = false;
		}
	}

	/// <summary>Arguments used to determine if a tree view item can be dropped while
	/// dragging.</summary>
	public class TreeViewCanDropEventArgs : RoutedEventArgs {
		/// <summary>True if the dragged item is file drop.</summary>
		public bool IsFileDrop { get; }
		/// <summary>The item being dragged.</summary>
		public ImageTreeViewItem Item { get; }
		/// <summary>The target item being dropped on.</summary>
		public ImageTreeViewItem Target { get; }
		/// <summary>Set this value to determine if the items can be dropped.</summary>
		public bool CanDrop { get; set; }

		/// <summary>Constructs the item-based event args.</summary>
		public TreeViewCanDropEventArgs(RoutedEvent routedEvent,
			ImageTreeViewItem item, ImageTreeViewItem target)
			: base(routedEvent)
		{
			IsFileDrop = false;
			Item = item;
			Target = target;
			CanDrop	= false;
		}

		/// <summary>Constructs the filedrop-based event args.</summary>
		public TreeViewCanDropEventArgs(RoutedEvent routedEvent,
			ImageTreeViewItem target) : base(routedEvent)
		{
			IsFileDrop = true;
			Item = null;
			Target = target;
			CanDrop = false;
		}
	}

	/// <summary>The arguments for the event that fires when a tree view item is
	/// dropped.</summary>
	public class TreeViewDropEventArgs : RoutedEventArgs {
		public bool IsFileDrop { get; }
		public ImageTreeViewItem Item { get; }
		public ImageTreeViewItem Target { get; }
		public string[] Files { get; }

		/// <summary>Constructs the item-based event args.</summary>
		public TreeViewDropEventArgs(RoutedEvent routedEvent, ImageTreeViewItem item,
			ImageTreeViewItem target) : base(routedEvent)
		{
			IsFileDrop = false;
			Item = item;
			Target = target;
			Files = null;
		}

		/// <summary>Constructs the filedrop-based event args.</summary>
		public TreeViewDropEventArgs(RoutedEvent routedEvent, ImageTreeViewItem target,
			string[] files) : base(routedEvent)
		{
			IsFileDrop = true;
			Item = null;
			Target = target;
			Files = files;
		}
	}

	/// <summary>The delegate to determine if a tree view item can be dragged.</summary>
	public delegate void TreeViewCanDragEventHandler(object sender, TreeViewCanDragEventArgs e);
	/// <summary>The delegate to determine if a tree view item can be dropped while
	/// dragging.</summary>
	public delegate void TreeViewCanDropEventHandler(object sender, TreeViewCanDropEventArgs e);
	/// <summary>The delegate for when a drop opperation commences.</summary>
	public delegate void TreeViewDropEventHandler(object sender, TreeViewDropEventArgs e);

	/// <summary>A tree view for draggable image tree view items.</summary>
	public class DraggableImageTreeView : TreeView {

		//-----------------------------------------------------------------------------
		// Routed Events
		//-----------------------------------------------------------------------------

		/// <summary>The routed event for if an item can be dragged.</summary>
		public static readonly RoutedEvent CanDragItemEvent =
			EventManager.RegisterRoutedEvent("CanDragItem", RoutingStrategy.Bubble,
				typeof(TreeViewCanDragEventHandler), typeof(DraggableImageTreeView));

		/// <summary>The routed event for if an item can be dropped.</summary>
		public static readonly RoutedEvent CanDropItemEvent =
			EventManager.RegisterRoutedEvent("CanDropItem", RoutingStrategy.Bubble,
				typeof(TreeViewCanDropEventHandler), typeof(DraggableImageTreeView));

		/// <summary>The routed event for when a drop commences.</summary>
		public static readonly RoutedEvent DropItemEvent =
			EventManager.RegisterRoutedEvent("DropItem", RoutingStrategy.Bubble,
				typeof(TreeViewDropEventHandler), typeof(DraggableImageTreeView));

		/// <summary>Occurs to ask the owner if the item can be dragged.</summary>
		public event TreeViewCanDragEventHandler CanDragItem {
			add { AddHandler(CanDragItemEvent, value); }
			remove { RemoveHandler(CanDragItemEvent, value); }
		}

		/// <summary>Occurs to ask the owner if the item can be dropped.</summary>
		public event TreeViewCanDropEventHandler CanDropItem {
			add { AddHandler(CanDropItemEvent, value); }
			remove { RemoveHandler(CanDropItemEvent, value); }
		}

		/// <summary>Occurs to ask the owner how to drop the items.</summary>
		public event TreeViewDropEventHandler DropItem {
			add { AddHandler(DropItemEvent, value); }
			remove { RemoveHandler(DropItemEvent, value); }
		}


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The initial position of the mouse when the drag started.</summary>
		private Point lastMouseDown;
		/// <summary>The tree view item being dragged.</summary>
		private ImageTreeViewItem draggedItem;
		/// <summary>True while attempting to start a drag. Set to false afterwords.</summary>
		private bool dragStarted;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs and sets up the draggable tree view.</summary>
		public DraggableImageTreeView() {
			draggedItem = null;
			dragStarted = false;

			AllowDrop = false;
			MouseLeave += OnTreeViewRootMouseLeave;

			Style style = new Style(typeof(TreeViewItem));
			style.Setters.Add(new Setter(TreeViewItem.AllowDropProperty, true));

			// Set events for all tree view items in this tree view
			style.Setters.Add(new EventSetter(ImageTreeViewItem.ItemMouseDownEvent,
				new ItemMouseButtonEventHandler(OnTreeViewItemMouseDown)));
			style.Setters.Add(new EventSetter(TreeViewItem.MouseMoveEvent,
				new MouseEventHandler(OnTreeViewMouseMove)));
			style.Setters.Add(new EventSetter(TreeViewItem.DragEnterEvent,
				new DragEventHandler(OnTreeViewDragEnter)));
			style.Setters.Add(new EventSetter(TreeViewItem.DragLeaveEvent,
				new DragEventHandler(OnTreeViewDragLeave)));
			style.Setters.Add(new EventSetter(TreeViewItem.DragOverEvent,
				new DragEventHandler(OnTreeViewDragOver)));
			style.Setters.Add(new EventSetter(TreeViewItem.DropEvent,
				new DragEventHandler(OnTreeViewDrop)));
			ItemContainerStyle = style;
		}


		//-----------------------------------------------------------------------------
		// TreeView Event Handlers
		//-----------------------------------------------------------------------------

		private void OnTreeViewRootMouseLeave(object sender, MouseEventArgs e) {
			// Make sure a manual drag can't be initiated until clicking inside again
			dragStarted = false;
		}


		//-----------------------------------------------------------------------------
		// TreeViewItem Event Handlers
		//-----------------------------------------------------------------------------

		private void OnTreeViewItemMouseDown(object sender,
			ItemMouseButtonEventArgs e)
		{
			if (e.Args.ChangedButton == MouseButton.Left) {
				lastMouseDown = e.Args.GetPosition(this);
				dragStarted = true;
				draggedItem = null;
			}
			else if (e.Args.LeftButton == MouseButtonState.Released &&
				e.Args.ChangedButton == MouseButton.Right)
			{
				ImageTreeViewItem item =
					VisualUpwardSearch(e.OriginalSource as DependencyObject);

				// Force focus on right click
				if (item != null) {
					item.Focus();
					e.Handled = true;
				}
			}
		}
		
		private void OnTreeViewMouseMove(object sender, MouseEventArgs e) {
			if (dragStarted && e.LeftButton == MouseButtonState.Pressed &&
				CanMouseDrag(e))
			{
				draggedItem = SelectedItem as ImageTreeViewItem;
				if (draggedItem != null) {
					TreeViewCanDragEventArgs canDragArgs;
					canDragArgs = new TreeViewCanDragEventArgs(
						CanDragItemEvent, draggedItem);

					// Ask if we can drag this item
					RaiseEvent(canDragArgs);

					if (canDragArgs.CanDrag) {
						dragStarted = false;
						DragDrop.DoDragDrop(this, draggedItem, DragDropEffects.Move);
						return;
					}
				}
				draggedItem = null;
			}
			else if (e.LeftButton == MouseButtonState.Released &&
				e.RightButton == MouseButtonState.Pressed)
			{
				ImageTreeViewItem item =
					VisualUpwardSearch(e.OriginalSource as DependencyObject);

				// Force focus on right click
				if (item != null) {
					item.Focus();
					e.Handled = true;
				}
			}
		}

		private void OnTreeViewDragEnter(object sender, DragEventArgs e) {
			// Highlight the item being dragged over
			ImageTreeViewItem dropTarget =
				GetNearestContainer(e.OriginalSource as UIElement);
			dropTarget.IsHighlighted = true;

			e.Handled = true;

			bool fileDrop = e.Data.GetDataPresent(DataFormats.FileDrop);
			if (fileDrop || (draggedItem != null &&
				IsValidDropTarget(draggedItem, dropTarget)))
			{
				TreeViewCanDropEventArgs canDrop;
				if (fileDrop)
					canDrop = new TreeViewCanDropEventArgs(
						CanDropItemEvent, dropTarget);
				else
					canDrop = new TreeViewCanDropEventArgs(
						CanDropItemEvent, draggedItem, dropTarget);

				// Ask if we can drop this item
				RaiseEvent(canDrop);

				if (canDrop.CanDrop) {
					if (fileDrop)
						e.Effects = DragDropEffects.Copy;
					else
						e.Effects = DragDropEffects.Move;
					return;
				}
			}
			e.Effects = DragDropEffects.None;
		}

		private void OnTreeViewDragLeave(object sender, DragEventArgs e) {
			// Highlight the item being dragged over
			ImageTreeViewItem dropTarget =
				GetNearestContainer(e.OriginalSource as UIElement);
			dropTarget.IsHighlighted = false;
		}

		private void OnTreeViewDragOver(object sender, DragEventArgs e) {
			// Treated as same event
			OnTreeViewDragEnter(sender, e);
		}

		private void OnTreeViewDrop(object sender, DragEventArgs e) {
			// Highlight the item being dragged over
			ImageTreeViewItem dropTarget =
				GetNearestContainer(e.OriginalSource as UIElement);
			dropTarget.IsHighlighted = false;
			
			e.Handled = true;

			bool fileDrop = e.Data.GetDataPresent(DataFormats.FileDrop);

			if (fileDrop || (draggedItem != null &&
				IsValidDropTarget(draggedItem, dropTarget)))
			{
				TreeViewDropEventArgs drop;
				if (fileDrop) {
					string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);
					drop = new TreeViewDropEventArgs(
						DropItemEvent, dropTarget, files);
				}
				else {
					drop = new TreeViewDropEventArgs(
						DropItemEvent, draggedItem, dropTarget);
				}

				// Drop the item
				RaiseEvent(drop);

				draggedItem = null;
				dragStarted = false;
			}
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Determines if the mouse has moved enough to trigger dragging.</summary>
		private bool CanMouseDrag(MouseEventArgs e) {
			return CanMouseDrag(e.GetPosition(this));
		}

		/// <summary>Determines if the mouse has moved enough to trigger dragging.</summary>
		private bool CanMouseDrag(DragEventArgs e) {
			return CanMouseDrag(e.GetPosition(this));
		}

		/// <summary>Determines if the mouse has moved enough to trigger dragging.</summary>
		private bool CanMouseDrag(Point mouse) {
			return ((Math.Abs(mouse.X - lastMouseDown.X) >
						SystemParameters.MinimumHorizontalDragDistance) ||
					(Math.Abs(mouse.Y - lastMouseDown.Y) >
						SystemParameters.MinimumVerticalDragDistance));
		}

		/// <summary>Confirms if the drop target is not contained by itself.</summary>
		private static bool IsValidDropTarget(ImageTreeViewItem source,
			ImageTreeViewItem target)
		{
			return (source.Tag != target.Tag && !InsideItself(source, target));
		}

		/// <summary>Determines if the item contains the target.</summary>
		private static bool InsideItself(ImageTreeViewItem item,
			ImageTreeViewItem target)
		{
			ImageTreeViewItem parent = target as ImageTreeViewItem;
			while (parent != null) {
				if (parent == item)
					return true;
				parent = parent.Parent as ImageTreeViewItem;
			}
			return false;
		}

		/// <summary>Returns the source's containing image tree view item.
		/// This is called during mouse events.</summary>
		private static ImageTreeViewItem VisualUpwardSearch(DependencyObject source) {
			while (source != null && !(source is ImageTreeViewItem))
				source = VisualTreeHelper.GetParent(source);

			return source as ImageTreeViewItem;
		}

		/// <summary>Gets the first tree view item parent of this tree view item.
		/// This is called during drag and drop events.</summary>
		private static ImageTreeViewItem GetNearestContainer(UIElement element) {
			// Walk up the element tree to the nearest tree view item.
			ImageTreeViewItem container = element as ImageTreeViewItem;
			while ((container == null) && (element != null)) {
				element = VisualTreeHelper.GetParent(element) as UIElement;
				container = element as ImageTreeViewItem;
			}
			return container;
		}
	}
}
