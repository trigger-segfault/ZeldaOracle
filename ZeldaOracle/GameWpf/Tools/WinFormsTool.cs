using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using Cursor = System.Windows.Forms.Cursor;
using System.Windows.Input;
using Key = System.Windows.Input.Key;
using MouseButton = System.Windows.Input.MouseButton;
using ModifierKeys = System.Windows.Input.ModifierKeys;
using FormsControl = System.Windows.Forms.Control;
using ZeldaOracle.Common.Geometry;
using ZeldaWpf.Util;
using ZeldaWpf.WinForms;

namespace ZeldaWpf.Tools {
	/// <summary>The base class for WinForms tools.</summary>
	public abstract class WinFormsTool<TOwner, TArgs>
		where TOwner : GraphicsDeviceControl where TArgs : WinFormsToolEventArgs
	{
		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The constant for when no button is pressed.</summary>
		public const MouseButton NoButton = (MouseButton) byte.MaxValue;
		

		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		private TOwner control;
		private string name;
		private bool isDragging;
		private MouseButton dragButton;
		private Cursor mouseCursor;
		private bool isDrawing;
		private Key hotKey;
		private HashSet<string> options;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		protected WinFormsTool(string name, Key hotKey = Key.None) {
			this.name		= name;
			this.hotKey		= hotKey;
			mouseCursor		= MultiCursors.Arrow;
			options			= new HashSet<string>();
		}


		//-----------------------------------------------------------------------------
		// Protected Methods
		//-----------------------------------------------------------------------------

		protected void AddOption(string name) {
			options.Add(name);
		}

		protected void StopDragging() {
			isDragging = false;
		}

		protected void UpdateCommands() {
			System.Windows.Input.CommandManager.InvalidateRequerySuggested();
		}


		//-----------------------------------------------------------------------------
		// State Methods
		//-----------------------------------------------------------------------------

		public virtual void Initialize(TOwner owner) {
			control = owner;
			OnInitialize();
		}

		public virtual void Begin() {
			control.Cursor = MouseCursor;
			TArgs args = MakeArgs();
			OnBegin(args);
		}

		public virtual void End() {
			Cancel();
			TArgs args = MakeArgs();
			OnEnd(args);
		}

		public virtual void Finish() {
			TArgs args = MakeArgs();
			OnFinish(args);
			isDrawing = false;
			isDragging = false;
		}

		public virtual void Cancel() {
			TArgs args = MakeArgs();
			OnCancel(args);
			isDrawing = false;
			isDragging = false;
		}

		public virtual void Update() {
			TArgs args = MakeArgs();
			OnUpdate(args);
		}


		//-----------------------------------------------------------------------------
		// Mouse Methods
		//-----------------------------------------------------------------------------

		public virtual void MouseDown(MouseEventArgs e) {
			TArgs args = MakeArgs(e);
			OnMouseDown(args);
			if (!isDragging) {
				isDragging = true;
				dragButton = args.Button;
				OnMouseDragBegin(args);
			}
		}

		public virtual void MouseUp(MouseEventArgs e) {
			TArgs args = MakeArgs(e);
			OnMouseUp(args);
			if (isDragging && args.Button == dragButton) {
				isDragging = false;
				OnMouseDragEnd(args);
				dragButton = NoButton;
			}
		}

		public virtual void MouseMove(MouseEventArgs e) {
			TArgs args = MakeArgs(e);
			OnMouseMove(args);
			if (isDragging && args.Button == dragButton) {
				OnMouseDragMove(args);
			}
		}

		public virtual void MouseDoubleClick(MouseEventArgs e) {
			TArgs args = MakeArgs(e);
			OnMouseDoubleClick(args);
		}


		//-----------------------------------------------------------------------------
		// Abstract Methods
		//-----------------------------------------------------------------------------

		protected abstract TArgs MakeArgs();
		protected abstract TArgs MakeArgs(MouseEventArgs e);
		protected abstract TArgs MakeArgs(KeyEventArgs e);

		//-----------------------------------------------------------------------------
		// Virtual Clipboard Methods
		//-----------------------------------------------------------------------------

		public virtual void Cut() { }

		public virtual void Copy() { }

		public virtual void Paste() { }

		public virtual void Delete() { }

		public virtual void SelectAll() { }

		public virtual void Deselect() { }


		//-----------------------------------------------------------------------------
		// Virtual State Methods
		//-----------------------------------------------------------------------------

		protected virtual void OnInitialize() { }

		protected virtual void OnBegin(TArgs e) { }

		protected virtual void OnEnd(TArgs e) { }

		protected virtual void OnCancel(TArgs e) { }

		protected virtual void OnFinish(TArgs e) { }

		protected virtual void OnUpdate(TArgs e) { }


		//-----------------------------------------------------------------------------
		// Virtual Mouse Methods
		//-----------------------------------------------------------------------------

		protected virtual void OnMouseDown(TArgs e) { }

		protected virtual void OnMouseUp(TArgs e) { }

		protected virtual void OnMouseMove(TArgs e) { }

		protected virtual void OnMouseDoubleClick(TArgs e) { }

		protected virtual void OnMouseDragBegin(TArgs e) { }

		protected virtual void OnMouseDragEnd(TArgs e) { }

		protected virtual void OnMouseDragMove(TArgs e) { }


		//-----------------------------------------------------------------------------
		// Abstract Properties
		//-----------------------------------------------------------------------------

		public abstract bool IsCurrentTool { get; }


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public TOwner Owner {
			get { return control; }
			set { control = value; }
		}

		public string Name {
			get { return name; }
		}

		public Key HotKey {
			get { return hotKey; }
		}

		public Cursor MouseCursor {
			get { return mouseCursor; }
			set {
				mouseCursor = value;
				if (IsCurrentTool)
					control.Cursor = MouseCursor;
			}
		}

		public MouseButton DragButton {
			get { return dragButton; }
		}

		public bool IsDragging {
			get { return isDragging; }
		}

		public bool IsDrawing {
			get { return isDrawing; }
			set { isDrawing = value; }
		}

		public HashSet<string> Options {
			get { return options; }
		}

		public ModifierKeys Modifiers {
			get { FormsControl.ModifierKeys.ToWpfModifierKeys(); }
		}

		public Point2I ScrollPosition {
			get { return control.ScrollPosition; }
			set { control.ScrollPosition = value; }
		}
	}
}
