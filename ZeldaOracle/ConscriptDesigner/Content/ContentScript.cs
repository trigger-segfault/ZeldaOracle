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
using System.IO;

namespace ConscriptDesigner.Content {
	/// <summary>A content file representing .conscript extension.</summary>
	public class ContentScript : ContentFile {

		/// <summary>Cached text used for searching.</summary>
		private string cachedText;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Conscructs the content script.</summary>
		public ContentScript(string name) :
			base(name)
		{
			this.cachedText     = null;

			TreeViewItem.Source = DesignerImages.ConscriptFile;
			XmlInfo.ElementName = "None";
			XmlInfo.CopyToOutputDirectory = "PreserveNewest";
		}


		//-----------------------------------------------------------------------------
		// Script Navigatiuon
		//-----------------------------------------------------------------------------

		/// <summary>Loads the file text into the cache.</summary>
		public bool LoadText() {
			try {
				cachedText = File.ReadAllText(FilePath, Encoding.UTF8);
				return true;
			}
			catch (Exception ex) {
				DesignerControl.ShowExceptionMessage(ex, "search", Name);
				return false;
			}
		}

		/// <summary>Cleras the file text in the cache.</summary>
		public void UnloadText() {
			cachedText = null;
		}

		/// <summary>Navigates to the location in the file.</summary>
		public void GotoLocation(int line, int column) {
			if (!IsOpen) {
				if (!Open(false))
					return;
			}
			Document.IsActive = true;
			Editor.GotoLocation(line, column);
		}

		/// <summary>Selects text in the editor.</summary>
		public void SelectText(int start, int length, bool focus) {
			if (!IsOpen) {
				if (!Open(false))
					return;
			}
			Document.IsActive = true;
			Editor.SelectText(start, length, focus);
		}


		//-----------------------------------------------------------------------------
		// Override Events
		//-----------------------------------------------------------------------------

		/// <summary>Called when the file is opened.</summary>
		protected override void OnOpen() {
			Document = new ConscriptEditor(this);
			cachedText = null;
			Editor.FocusEditor();
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
			if (IsOpen) {
				Editor.Save();
			}
			else if (cachedText != null) {
				File.WriteAllText(FilePath, cachedText, Encoding.UTF8);
				cachedText = null;
			}
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

		/// <summary>Called when the file becomes outdated.</summary>
		protected override void OnFileOutdated() {
			cachedText = null;
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


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the loaded text either from the editor or the cache.</summary>
		public string LoadedText {
			get {
				if (IsOpen)
					return Editor.Text;
				return cachedText;
			}
			set {
				if (IsOpen)
					throw new Exception("Cannot set loaded text when conscript is open!");
				cachedText = value;
			}
		}
		
		/// <summary>Gets the text selected in the editor.</summary>
		public string SelectedText {
			get {
				if (IsOpen)
					return Editor.SelectedText;
				return null;
			}
		}

		/// <summary>Gets the text editor control.</summary>
		public TextEditor TextEditor {
			get {
				if (IsOpen)
					return Editor.TextEditor;
				return null;
			}
		}
	}
}
