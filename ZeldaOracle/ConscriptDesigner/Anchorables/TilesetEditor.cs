using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;
using ConscriptDesigner.Content;
using ConscriptDesigner.Control;

namespace ConscriptDesigner.Anchorables {
	public class TilesetEditor : RequestCloseAnchorable, ICommandAnchorable {

		private TilesetEditorControl control;
		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the tileset editor.</summary>
		public TilesetEditor() {
			Border border = CreateBorder();
			this.control = new TilesetEditorControl();
			this.control.ModifiedChanged += OnModifiedChanged;
			border.Child = this.control;

			Closed += OnAnchorableClosed;
			IsActiveChanged += OnIsActiveChanged;
			Title = "Tileset Editor";
			Content = border;
		}


		//-----------------------------------------------------------------------------
		// Control
		//-----------------------------------------------------------------------------

		public void Reload() {
			control.Reload();
		}

		public void Unload() {
			control.Unload();
		}

		public bool Save(bool silentFail) {
			return control.Save(silentFail);
		}

		public bool RequestSave(bool silentFail) {
			return control.RequestSave(silentFail);
		}

		/// <summary>Focuses on the anchorable's content.</summary>
		public override void Focus() {
			control.Focus();
		}


		//-----------------------------------------------------------------------------
		// ICommandAnchorable Override Methods
		//-----------------------------------------------------------------------------

		public void Cut() {
			control.CurrentTool.Cut();
		}

		public void Copy() {
			control.CurrentTool.Copy();
		}

		public void Paste() {
			control.Paste();
		}

		public void Delete() {
			control.CurrentTool.Delete();
		}

		public void SelectAll() {
			control.SelectAll();
		}

		public void Deselect() {
			control.CurrentTool.Deselect();
		}

		public void Undo() { }

		public void Redo() { }


		//-----------------------------------------------------------------------------
		// ICommandAnchorable Override Properties
		//-----------------------------------------------------------------------------

		public bool CanCut {
			get { return control.CurrentTool.CanCopyCut; }
		}

		public bool CanCopy {
			get { return control.CurrentTool.CanCopyCut; }
		}

		public bool CanPaste {
			get { return control.CanPaste; }
		}

		public bool CanDelete {
			get { return control.CurrentTool.CanDeleteDeselect; }
		}

		public bool CanSelectAll {
			get { return true; }
		}

		public bool CanDeselect {
			get { return control.CurrentTool.CanDeleteDeselect; }
		}

		public bool CanUndo {
			get { return false; }
		}

		public bool CanRedo {
			get { return false; }
		}


		//-----------------------------------------------------------------------------
		// XML Serialization
		//-----------------------------------------------------------------------------

		public override void ReadXml(XmlReader reader) {
			base.ReadXml(reader);
			//DesignerControl.MainWindow.TilesetEditor = this;
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		/// <summary>Called to force cleanup during close.</summary>
		private void OnAnchorableClosed(object sender, EventArgs e) {
			control.Dispose();
		}

		private void OnModifiedChanged(object sender, EventArgs e) {
			Title = "Tileset Editor" + (IsModified ? "*" : "");
		}

		private void OnIsActiveChanged(object sender, EventArgs e) {
			if (IsActive) {
				control.Focus();
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsModified {
			get { return control.IsModified; }
		}
	}
}
