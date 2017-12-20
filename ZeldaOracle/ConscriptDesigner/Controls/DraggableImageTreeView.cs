using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ConscriptDesigner.Controls {

	public class TreeViewCanDragEventArgs : RoutedEventArgs {
		public ImageTreeViewItem Item { get; }
		public bool CanDrag { get; set; }

		public TreeViewCanDragEventArgs(RoutedEvent routedEvent, ImageTreeViewItem item) :
			base(routedEvent)
		{
			this.Item		= item;
			this.CanDrag	= false;
		}
	}
	public class TreeViewCanDropEventArgs : RoutedEventArgs {
		public bool IsFileDrop { get; }
		public ImageTreeViewItem Item { get; }
		public ImageTreeViewItem Target { get; }
		public bool CanDrop { get; set; }

		public TreeViewCanDropEventArgs(RoutedEvent routedEvent, ImageTreeViewItem item, ImageTreeViewItem target) :
			base(routedEvent)
		{
			this.IsFileDrop = false;
			this.Item		= item;
			this.Target		= target;
			this.CanDrop	= false;
		}

		public TreeViewCanDropEventArgs(RoutedEvent routedEvent, ImageTreeViewItem target) :
			base(routedEvent)
		{
			this.IsFileDrop = true;
			this.Item		= null;
			this.Target		= target;
			this.CanDrop	= false;
		}
	}

	public class TreeViewDropEventArgs : RoutedEventArgs {
		public bool IsFileDrop { get; }
		public ImageTreeViewItem Item { get; }
		public ImageTreeViewItem Target { get; }

		public TreeViewDropEventArgs(RoutedEvent routedEvent, ImageTreeViewItem item, ImageTreeViewItem target) :
			base(routedEvent)
		{
			this.IsFileDrop = false;
			this.Item		= item;
			this.Target		= target;
		}

		public TreeViewDropEventArgs(RoutedEvent routedEvent, ImageTreeViewItem target) :
			base(routedEvent)
		{
			this.IsFileDrop = true;
			this.Item		= null;
			this.Target		= target;
		}
	}

	public delegate void TreeViewCanDragEventHandler(object sender, TreeViewCanDragEventArgs e);
	public delegate void TreeViewCanDropEventHandler(object sender, TreeViewCanDropEventArgs e);
	public delegate void TreeViewDropEventHandler(object sender, TreeViewDropEventArgs e);

	public class DraggableImageTreeView : TreeView {

		//-----------------------------------------------------------------------------
		// Routed Events
		//-----------------------------------------------------------------------------

		public static readonly RoutedEvent CanDragItemEvent = EventManager.RegisterRoutedEvent(
			"CanDragItem", RoutingStrategy.Bubble, typeof(TreeViewCanDragEventHandler),
			typeof(DraggableImageTreeView));

		public static readonly RoutedEvent CanDropItemEvent = EventManager.RegisterRoutedEvent(
			"CanDropItem", RoutingStrategy.Bubble, typeof(TreeViewCanDropEventHandler),
			typeof(DraggableImageTreeView));

		public static readonly RoutedEvent DropItemEvent = EventManager.RegisterRoutedEvent(
			"DropItem", RoutingStrategy.Bubble, typeof(TreeViewDropEventHandler),
			typeof(DraggableImageTreeView));

		public event TreeViewCanDragEventHandler CanDragItem {
			add { AddHandler(CanDragItemEvent, value); }
			remove { RemoveHandler(CanDragItemEvent, value); }
		}

		public event TreeViewCanDropEventHandler CanDropItem {
			add { AddHandler(CanDropItemEvent, value); }
			remove { RemoveHandler(CanDropItemEvent, value); }
		}

		public event TreeViewDropEventHandler DropItem {
			add { AddHandler(DropItemEvent, value); }
			remove { RemoveHandler(DropItemEvent, value); }
		}


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		private Point lastMouseDown;
		private ImageTreeViewItem draggedItem;
		private bool dragStarted;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public DraggableImageTreeView() {
			this.draggedItem = null;
			this.dragStarted = false;

			AllowDrop = false;
			MouseLeave += OnTreeViewRootMouseLeave;

			Style style = new Style(typeof(TreeViewItem));
			style.Setters.Add(new Setter(TreeViewItem.AllowDropProperty, true));

			// Set events for all tree view items in this tree view
			style.Setters.Add(new EventSetter(ImageTreeViewItem.ItemMouseDownEvent,
				new ItemMouseButtonEventHandler(OnTreeViewItemMouseDown)));
			//style.Setters.Add(new EventSetter(TreeViewItem.PreviewMouseDownEvent,
			//	new MouseButtonEventHandler(OnTreeViewPreviewMouseDown)));
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

		private void OnTreeViewItemMouseDown(object sender, ItemMouseButtonEventArgs e) {
			if (e.Args.ChangedButton == MouseButton.Left) {
				lastMouseDown = e.Args.GetPosition(this);
				dragStarted = true;
				draggedItem = null;
			}
			else if (e.Args.LeftButton == MouseButtonState.Released && e.Args.ChangedButton == MouseButton.Right) {
				ImageTreeViewItem item = VisualUpwardSearch(e.OriginalSource as DependencyObject);

				// Force focus on right click
				if (item != null) {
					item.Focus();
					e.Handled = true;
				}
			}
		}

		/*private void OnTreeViewPreviewMouseDown(object sender, MouseButtonEventArgs e) {
			if (e.ChangedButton == MouseButton.Left) {
				lastMouseDown = e.GetPosition(this);
				dragStarted = true;
				draggedItem = null;
			}
			else if (e.LeftButton == MouseButtonState.Released && e.ChangedButton == MouseButton.Right) {
				ImageTreeViewItem item = VisualUpwardSearch(e.OriginalSource as DependencyObject);

				// Force focus on right click
				if (item != null) {
					item.Focus();
					e.Handled = true;
				}
			}
		}*/
		
		private void OnTreeViewMouseMove(object sender, MouseEventArgs e) {
			if (dragStarted && e.LeftButton == MouseButtonState.Pressed && CanMouseDrag(e)) {
				draggedItem = SelectedItem as ImageTreeViewItem;
				if (draggedItem != null) {
					var canDrag = new TreeViewCanDragEventArgs(CanDragItemEvent, draggedItem);

					// Ask if we can drag this item
					RaiseEvent(canDrag);

					if (canDrag.CanDrag) {
						dragStarted = false;
						DragDrop.DoDragDrop(this, draggedItem, DragDropEffects.Move);
						return;
					}
				}
				draggedItem = null;
			}
			else if (e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Pressed) {
				ImageTreeViewItem item = VisualUpwardSearch(e.OriginalSource as DependencyObject);

				// Force focus on right click
				if (item != null) {
					item.Focus();
					e.Handled = true;
				}
			}
		}

		private void OnTreeViewDragEnter(object sender, DragEventArgs e) {
			// Highlight the item being dragged over
			ImageTreeViewItem dropTarget = GetNearestContainer(e.OriginalSource as UIElement);
			dropTarget.IsHighlighted = true;

			e.Handled = true;

			bool fileDrop = e.Data.GetDataPresent(DataFormats.FileDrop);
			if (fileDrop || (draggedItem != null && IsValidDropTarget(draggedItem, dropTarget))) {
				TreeViewCanDropEventArgs canDrop;
				if (fileDrop)
					canDrop = new TreeViewCanDropEventArgs(CanDropItemEvent, dropTarget);
				else
					canDrop = new TreeViewCanDropEventArgs(CanDropItemEvent, draggedItem, dropTarget);

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
			ImageTreeViewItem dropTarget = GetNearestContainer(e.OriginalSource as UIElement);
			dropTarget.IsHighlighted = false;
		}

		private void OnTreeViewDragOver(object sender, DragEventArgs e) {
			// Treated as same event
			OnTreeViewDragEnter(sender, e);
		}

		private void OnTreeViewDrop(object sender, DragEventArgs e) {
			// Highlight the item being dragged over
			ImageTreeViewItem dropTarget = GetNearestContainer(e.OriginalSource as UIElement);
			dropTarget.IsHighlighted = false;
			
			e.Handled = true;

			bool fileDrop = e.Data.GetDataPresent(DataFormats.FileDrop);

			if (fileDrop || (draggedItem != null && IsValidDropTarget(draggedItem, dropTarget))) {
				TreeViewDropEventArgs drop;
				if (fileDrop)
					drop = new TreeViewDropEventArgs(DropItemEvent, dropTarget);
				else
					drop = new TreeViewDropEventArgs(DropItemEvent, draggedItem, dropTarget);

				// Drop the item
				RaiseEvent(drop);

				draggedItem = null;
				dragStarted = false;
			}
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		private bool CanMouseDrag(MouseEventArgs e) {
			return CanMouseDrag(e.GetPosition(this));
		}

		private bool CanMouseDrag(DragEventArgs e) {
			return CanMouseDrag(e.GetPosition(this));
		}

		private bool CanMouseDrag(Point mouse) {
			return ((Math.Abs(mouse.X - lastMouseDown.X) > SystemParameters.MinimumHorizontalDragDistance) ||
					(Math.Abs(mouse.Y - lastMouseDown.Y) > SystemParameters.MinimumVerticalDragDistance));
		}

		private static bool IsValidDropTarget(ImageTreeViewItem source, ImageTreeViewItem target) {
			return (source.Tag != target.Tag && !InsideItself(source, target));
		}

		private static bool InsideItself(ImageTreeViewItem item, ImageTreeViewItem target) {
			ImageTreeViewItem parent = target as ImageTreeViewItem;
			while (parent != null) {
				if (parent == item)
					return true;
				parent = parent.Parent as ImageTreeViewItem;
			}
			return false;
		}

		private static ImageTreeViewItem VisualUpwardSearch(DependencyObject source) {
			while (source != null && !(source is ImageTreeViewItem))
				source = VisualTreeHelper.GetParent(source);

			return source as ImageTreeViewItem;
		}

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
