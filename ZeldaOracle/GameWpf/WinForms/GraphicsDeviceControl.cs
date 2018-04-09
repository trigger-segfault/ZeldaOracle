//-----------------------------------------------------------------------------
// GraphicsDeviceControl.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;

namespace ZeldaWpf.WinForms {
	// System.Drawing and the XNA Framework both define Color and Rectangle
	// types. To avoid conflicts, we specify exactly which ones to use.
	using Color = System.Drawing.Color;
	using Rectangle = Microsoft.Xna.Framework.Rectangle;


	/// <summary>
	/// Custom control uses the XNA Framework GraphicsDevice to render onto
	/// a Windows Form. Derived classes can override the Initialize and Draw
	/// methods to add their own drawing code.
	/// </summary>
	public class GraphicsDeviceControl : Panel {

		/// <summary>However many GraphicsDeviceControl instances you have, they all
		/// share the same underlying GraphicsDevice, managed by this helper service.
		/// </summary>
		private GraphicsDeviceService graphicsDeviceService;

		/// <summary>The IServiceProvider containing our IGraphicsDeviceService.
		/// </summary>
		private ServiceContainer services = new ServiceContainer();

		/// <summary>True if the mouse is over the control.</summary>
		private bool isMouseOver;

		/// <summary>The message filter for capturing scroll focus.</summary>
		private WinFormsFocusMessageFilter messageFilter;

		/// <summary>True if an exception occurred while resetting the graphics device.</summary>
		private bool resetError;
		

		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		/// <summary>Called just before resetting the graphics device.</summary>
		public event EventHandler PreviewReset;
		/// <summary>Called just after resetting the graphics device.</summary>
		public event EventHandler PostReset;
		
				
		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------
		
		/// <summary>Initializes the control.</summary>
		protected override void OnCreateControl() {
			// Don't initialize the graphics device if we are running in the designer.
			if (!DesignMode) {
				graphicsDeviceService = GraphicsDeviceService.AddRef(Handle,
																	 ClientSize.Width,
																	 ClientSize.Height);

				// Register the service, so components like ContentManager can find it.
				services.AddService<IGraphicsDeviceService>(graphicsDeviceService);

				// Give derived classes a chance to initialize themselves.
				Initialize();
			}

			messageFilter = new WinFormsFocusMessageFilter(this);
			messageFilter.AddFilter();

			base.OnCreateControl();
		}
		
		/// <summary>Disposes the control.</summary>
		protected override void Dispose(bool disposing) {
			if (graphicsDeviceService != null) {
				graphicsDeviceService.Release(disposing);
				graphicsDeviceService = null;
			}

			base.Dispose(disposing);
		}

		
		//-----------------------------------------------------------------------------
		// Paint
		//-----------------------------------------------------------------------------

		/// <summary>Redraws the control in response to a WinForms paint message.
		/// </summary>
		protected override void OnPaint(PaintEventArgs e) {
			if (ClientSize.Width > 0 && ClientSize.Height > 0) {
				string beginDrawError = BeginDraw();

				if (string.IsNullOrEmpty(beginDrawError)) {
					// Draw the control using the GraphicsDevice.
					if (ClientSize.Width > 0 && ClientSize.Height > 0) {
						Draw();
						EndDraw();
					}
				}
				else {
					// If BeginDraw failed, show an error message using System.Drawing.
					PaintUsingSystemDrawing(e.Graphics, beginDrawError);
				}
			}
		}
		
		/// <summary>
		/// Attempts to begin drawing the control. Returns an error message string
		/// if this was not possible, which can happen if the graphics device is
		/// lost, or if we are running inside the Form designer.
		/// </summary>
		private string BeginDraw() {
			// If we have no graphics device, we must be running in the designer.
			if (graphicsDeviceService == null) {
				return Text + "\n\n" + GetType();
			}

			// Make sure the graphics device is big enough, and is not lost.
			string deviceResetError = HandleDeviceReset();

			if (!string.IsNullOrEmpty(deviceResetError)) {
				return deviceResetError;
			}

			// Many GraphicsDeviceControl instances can be sharing the same
			// GraphicsDevice. The device backbuffer will be resized to fit the
			// largest of these controls. But what if we are currently drawing
			// a smaller control? To avoid unwanted stretching, we set the
			// viewport to only use the top left portion of the full backbuffer.
			Viewport viewport = new Viewport();

			viewport.X = 0;
			viewport.Y = 0;

			viewport.Width = ClientSize.Width;
			viewport.Height = ClientSize.Height;

			viewport.MinDepth = 0;
			viewport.MaxDepth = 1;

			GraphicsDevice.Viewport = viewport;

			return null;
		}
		
		/// <summary>
		/// Ends drawing the control. This is called after derived classes
		/// have finished their Draw method, and is responsible for presenting
		/// the finished image onto the screen, using the appropriate WinForms
		/// control handle to make sure it shows up in the right place.
		/// </summary>
		private void EndDraw() {
			try {
				Rectangle sourceRectangle = new Rectangle(0, 0,
					Math.Max(0, ClientSize.Width), Math.Max(0, ClientSize.Height));

				GraphicsDevice.Present(sourceRectangle, null, this.Handle);
			}
			catch {
				// Present might throw if the device became lost while we were
				// drawing. The lost device will be handled by the next BeginDraw,
				// so we just swallow the exception.
			}
		}
		
		/// <summary>
		/// Helper used by BeginDraw. This checks the graphics device status,
		/// making sure it is big enough for drawing the current control, and
		/// that the device is not lost. Returns an error string if the device
		/// could not be reset.
		/// </summary>
		private string HandleDeviceReset() {
			bool deviceNeedsReset = false;

			switch (GraphicsDevice.GraphicsDeviceStatus) {
			case GraphicsDeviceStatus.Lost:
				// If the graphics device is lost, we cannot use it at all.
				return "Graphics device lost";

			case GraphicsDeviceStatus.NotReset:
				// If device is in the not-reset state, we should try to reset it.
				deviceNeedsReset = true;
				break;

			default:
				// If the device state is ok, check whether it is big enough.
				PresentationParameters pp = GraphicsDevice.PresentationParameters;

				deviceNeedsReset = (ClientSize.Width > pp.BackBufferWidth) ||
									(ClientSize.Height > pp.BackBufferHeight);
				break;
			}

			// Do we need to reset the device?
			if (deviceNeedsReset) {
				try {
					if (PreviewReset != null)
						PreviewReset(this, EventArgs.Empty);
					graphicsDeviceService.ResetDevice(ClientSize.Width,
													  ClientSize.Height);
				}
				catch (Exception e) {
					resetError = true;
					return "Graphics device reset failed\n\n" + e;
				}
				if (PostReset != null)
					PostReset(this, EventArgs.Empty);
			}

			return null;
		}
		
		/// <summary>
		/// If we do not have a valid graphics device (for instance if the device
		/// is lost, or if we are running inside the Form designer), we must use
		/// regular System.Drawing method to display a status message.
		/// </summary>
		protected virtual void PaintUsingSystemDrawing(Graphics graphics, string text) {
			graphics.Clear(Color.CornflowerBlue);

			using (Brush brush = new SolidBrush(Color.Black)) {
				using (StringFormat format = new StringFormat()) {
					format.Alignment = StringAlignment.Center;
					format.LineAlignment = StringAlignment.Center;

					graphics.DrawString(text, Font, brush, ClientRectangle, format);
				}
			}
		}
		
		/// <summary>
		/// Ignores WinForms paint-background messages. The default implementation
		/// would clear the control to the current background color, causing
		/// flickering when our OnPaint implementation then immediately draws some
		/// other color over the top using the XNA Framework GraphicsDevice.
		/// </summary>
		protected override void OnPaintBackground(PaintEventArgs pevent) { }

		
		//-----------------------------------------------------------------------------
		// Is Mouse Over
		//-----------------------------------------------------------------------------
		
		protected override void OnMouseEnter(EventArgs e) {
			Application.AddMessageFilter(messageFilter);
			base.OnMouseEnter(e);
			isMouseOver = true;
		}

		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			isMouseOver = false;
		}
		

		//-----------------------------------------------------------------------------
		// Virtual Methods
		//-----------------------------------------------------------------------------
		
		/// <summary>Derived classes override this to initialize their drawing code.</summary>
		protected virtual void Initialize() { }

		/// <summary>Derived classes override this to draw themselves using the
		/// GraphicsDevice.</summary>
		protected virtual void Draw() { }

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets a GraphicsDevice that can be used to draw onto this control.</summary>
		public GraphicsDevice GraphicsDevice {
			get { return graphicsDeviceService.GraphicsDevice; }
		}

		/// <summary>
		/// Gets an IServiceProvider containing our IGraphicsDeviceService.
		/// This can be used with components such as the ContentManager,
		/// which use this service to look up the GraphicsDevice.
		/// </summary>
		internal ServiceContainer Services {
			get { return services; }
		}

		/// <summary>Returns true if the mouse is over the control.</summary>
		public bool IsMouseOver {
			get { return isMouseOver; }
		}

		/// <summary>True if an exception occurred while resetting the graphics device.</summary>
		public bool ResetError {
			get { return resetError; }
			set { resetError = false; }
		}
	}
}
