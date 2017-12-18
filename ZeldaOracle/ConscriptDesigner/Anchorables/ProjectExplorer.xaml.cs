using System;
using System.Collections.Generic;
using System.IO;
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
using ConscriptDesigner.Content;
using ConscriptDesigner.Controls;
using ConscriptDesigner.Windows;
using Path = System.IO.Path;

namespace ConscriptDesigner.Anchorables {
	/// <summary>
	/// Interaction logic for ProjectTreeView.xaml
	/// </summary>
	public partial class ProjectExplorer : UserControl {
		Point lastMouseDown;
		ImageTreeViewItem draggedItem;
		ImageTreeViewItem dropTarget;
		ContentRoot project;
		bool dragging;
		bool dragStarted;
		DragDropEffects finalDropEffect;
		bool isFileDrop;

		private static ProjectExplorer instance;

		public ProjectExplorer() {
			InitializeComponent();
			instance = this;
		}

		private void OnSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
			
		}

		public void Initialize(ContentRoot project) {
			treeView.Items.Add(project.TreeViewItem);
			this.project = project;
			project.TreeView = treeView;
		}

		private void OnTreeViewPreviewMouseDown(object sender, MouseButtonEventArgs e) {
			if (e.ChangedButton == MouseButton.Left) {
				lastMouseDown = e.GetPosition(treeView);
				dragging = false;
				draggedItem = null;
				dropTarget = null;
				isFileDrop = false;
			}
			else if (e.LeftButton == MouseButtonState.Released && e.ChangedButton == MouseButton.Right) {
				ImageTreeViewItem item = VisualUpwardSearch(e.OriginalSource as DependencyObject);

				// Force focus on right click
				if (item != null) {
					item.Focus();
					e.Handled = true;
				}
			}
		}

		private void OnTreeViewRootMouseLeave(object sender, MouseEventArgs e) {
			dragStarted = false;
		}

		private static ImageTreeViewItem VisualUpwardSearch(DependencyObject source) {
			while (source != null && !(source is ImageTreeViewItem))
				source = VisualTreeHelper.GetParent(source);

			return source as ImageTreeViewItem;
		}
		
		private void OnTreeViewMouseMove(object sender, MouseEventArgs e) {
			try {
				if (!dragStarted && !dragging && e.LeftButton == MouseButtonState.Pressed) {
					Point currentPosition = e.GetPosition(treeView);

					if ((Math.Abs(currentPosition.X - lastMouseDown.X) > SystemParameters.MinimumHorizontalDragDistance) ||
						(Math.Abs(currentPosition.Y - lastMouseDown.Y) > SystemParameters.MinimumVerticalDragDistance)) {
						draggedItem = treeView.SelectedItem as ImageTreeViewItem;
						if (draggedItem != null && !draggedItem.File().IsRoot) {
							dragging = true;
							dragStarted = true;
							finalDropEffect = DragDrop.DoDragDrop(treeView, draggedItem,
								DragDropEffects.Move);
						}
						else {
							draggedItem = null;
						}
					}
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
			catch { }
		}
		private void OnTreeViewDragOver(object sender, DragEventArgs e) {
			try {
				e.Handled = true;
				Point currentPosition = e.GetPosition(treeView);

				ImageTreeViewItem item = GetNearestContainer(e.OriginalSource as UIElement);

				if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
					if (item != null) {
						e.Effects = DragDropEffects.Copy;
					}
					else {
						e.Effects = DragDropEffects.None;
					}
				}
				else if ((Math.Abs(currentPosition.X - lastMouseDown.X) > SystemParameters.MinimumHorizontalDragDistance) ||
					(Math.Abs(currentPosition.Y - lastMouseDown.Y) > SystemParameters.MinimumVerticalDragDistance)) {
					// Verify that this is a valid drop and then store the drop target
					if (IsValidDropTarget(draggedItem, item)) {
						e.Effects = DragDropEffects.Move;
					}
					else {
						e.Effects = DragDropEffects.None;
					}
				}
			}
			catch { }
		}
		private void OnTreeViewDragLeave(object sender, DragEventArgs e) {
			ImageTreeViewItem item = GetNearestContainer(e.OriginalSource as UIElement);
			item.IsHighlighted = false;
		}

		private void OnTreeViewDragEnter(object sender, DragEventArgs e) {
			ImageTreeViewItem item = GetNearestContainer(e.OriginalSource as UIElement);

			item.IsHighlighted = true;
			try {
				if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
					e.Effects = DragDropEffects.Copy;
					isFileDrop = true;
					dragging = true;
				}
				else if (dragging) {
					e.Effects = DragDropEffects.Move;
					isFileDrop = false;
				}
				else {
					e.Effects = DragDropEffects.None;
					isFileDrop = false;
				}
				e.Handled = true;
			}
			catch { }
		}

		private void OnTreeViewDrop(object sender, DragEventArgs e) {
			string offendingFile = "";
			try {
				//dragging = false;
				//e.Effects = DragDropEffects.None;
				e.Handled = true;

				// Verify that this is a valid drop and then store the drop target
				dropTarget = GetNearestContainer(e.OriginalSource as UIElement);

				dropTarget.IsHighlighted = false;

				if (!dropTarget.File().IsFolder) {
					ImageTreeViewItem child = dropTarget;
					dropTarget = child.Parent as ImageTreeViewItem;
				}

				if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
					string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);
					project.RequestDrop(files, dropTarget.File().Path);
				}
				else {
					//ImageTreeViewItem target = ((ImageTreeViewItem)e.OriginalSource).Parent as ImageTreeViewItem;
					if (dropTarget != null && draggedItem != null && IsValidDropTarget(draggedItem, dropTarget)) {
						//e.Effects = DragDropEffects.Move;
						// Checking target is not null and item is dragging(moving)
						//if ((finalDropEffect == DragDropEffects.Move)) {
						// A Move drop was accepted
						
						if (!dropTarget.Items.Contains(draggedItem)) {
							project.Move(draggedItem.File().Path, dropTarget.File().Path, true);
						}
						//}
					}
					draggedItem.IsSelected = true;
				}
				dropTarget = null;
				draggedItem = null;
				dragging = false;
			}
			catch (DirectoryAlreadyExistsException) {
				TriggerMessageBox.Show(Window.GetWindow(this), MessageIcon.Warning, "Cannot include file '" +
					Path.GetFileName(offendingFile) + "' because a directory with that name already exists!", "Include Failed");
			}
			catch (FileAlreadyExistsException) {
				TriggerMessageBox.Show(Window.GetWindow(this), MessageIcon.Warning, "Cannot include directory '" +
					Path.GetFileName(offendingFile) + "' because a file with that name already exists!", "Include Failed");
			}
			catch { }
		}
		private bool IsValidDropTarget(ImageTreeViewItem source, ImageTreeViewItem target) {
			return source.Tag != target.Tag && !(source.File().IsFolder && InsideItself(source, target));
		}
		private bool InsideItself(ImageTreeViewItem item, ImageTreeViewItem target) {
			ImageTreeViewItem parent = target as ImageTreeViewItem;
			while (parent != null) {
				if (parent == item)
					return true;
				parent = parent.Parent as ImageTreeViewItem;
			}
			return false;
		}

		private ImageTreeViewItem GetNearestContainer(UIElement element) {
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
