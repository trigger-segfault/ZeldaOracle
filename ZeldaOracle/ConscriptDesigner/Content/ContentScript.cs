using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit;
using ConscriptDesigner.Control;
using Xceed.Wpf.AvalonDock.Layout;
using ConscriptDesigner.Anchorables;
using ConscriptDesigner.Windows;

namespace ConscriptDesigner.Content {
	public class ContentScript : ContentFile {
		
		private ConscriptEditor editor;

		public ContentScript(string name) :
			base(name)
		{
			TreeViewItem.Source = DesignerImages.ConscriptFile;
			XmlInfo.ElementName = "None";
			XmlInfo.CopyToOutputDirectory = "PreserveNewest";
		}

		//-----------------------------------------------------------------------------
		// Override Events
		//-----------------------------------------------------------------------------
		
		protected override void OnRename() {
			if (IsOpen) {
				editor.OnRename();
			}
		}

		protected override void OnOpen() {
			Anchorable = DesignerControl.CreateDocumentAnchorable();
			Anchorable.Title = Name;
			editor = new ConscriptEditor(this, Anchorable);
			Anchorable.Content = editor;
			Anchorable.IconSource = DesignerImages.ImageFile;
		}

		protected override void OnClose() {
			editor = null;
		}


		//-----------------------------------------------------------------------------
		// Override Context Menu
		//-----------------------------------------------------------------------------

		protected override void CreateContextMenu(ContextMenu menu) {
			AddOpenContextMenuItem(menu);
			AddSeparatorContextMenuItem(menu);
			AddExcludeContextMenuItem(menu);
			AddSeparatorContextMenuItem(menu);
			AddClipboardContextMenuItems(menu, false);
			AddRenameContextMenuItem(menu);
		}


		//-----------------------------------------------------------------------------
		// Override Properties
		//-----------------------------------------------------------------------------

		public override ContentTypes ContentType {
			get { return ContentTypes.Conscript; }
		}
	}
}
