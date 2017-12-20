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
	/// <summary>A content file representing .conscript extension.</summary>
	public class ContentScript : ContentFile {
		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Conscructs the content script.</summary>
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

		/// <summary>Called when the file is opened.</summary>
		protected override void OnOpen() {
			Document = new ConscriptEditor(this);
		}

		/// <summary>Called when the file is closed.</summary>
		protected override void OnClose() {

		}

		/// <summary>Called when the file is reloaded.</summary>
		protected override void OnReload() {
			Editor.Reload();
		}

		/// <summary>Called when the file is saved.</summary>
		protected override void OnSave() {
			Editor.Save();
		}

		/// <summary>Called when the file is renamed.</summary>
		protected override void OnRename() {
			if (IsOpen) {
				Editor.UpdateTitle();
			}
		}

		/// <summary>Called during undo.</summary>
		protected override void OnUndo() {
			Editor.Undo();
		}

		/// <summary>Called during redo.</summary>
		protected override void OnRedo() {
			Editor.Redo();
		}

		/// <summary>Called when the modified override is changed.</summary>
		protected override void OnModifiedChanged() {
			if (IsOpen) {
				Editor.UpdateTitle();
			}
		}


		//-----------------------------------------------------------------------------
		// Override Context Menu
		//-----------------------------------------------------------------------------

		/// <summary>Creates the context menu for the tree view item.</summary>
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

		/// <summary>Gets the type of the content file.</summary>
		public override ContentTypes ContentType {
			get { return ContentTypes.Conscript; }
		}
		
		/// <summary>Gets if the file is modified.</summary>
		protected override bool IsModifiedInternal {
			get {
				if (IsOpen)
					return Editor.IsModified;
				return false;
			}
		}
		
		/// <summary>Gets if the content file can undo any actions.</summary>
		public override bool CanUndo {
			get {
				if (IsOpen)
					return Editor.CanUndo;
				return false;
			}
		}

		/// <summary>Gets if the content file can redo any actions.</summary>
		public override bool CanRedo {
			get {
				if (IsOpen)
					return Editor.CanRedo;
				return false;
			}
		}


		//-----------------------------------------------------------------------------
		// Internal Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the document as a conscript editor.</summary>
		private ConscriptEditor Editor {
			get { return Document as ConscriptEditor; }
		}
	}
}
