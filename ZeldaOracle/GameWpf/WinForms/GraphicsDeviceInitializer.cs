using System;
using ZeldaOracle.Common.Content;

namespace ZeldaWpf.WinForms {
	/// <summary>A dummy graphics device control used to initialize resources.</summary>
	internal class GraphicsDeviceInitializer : GraphicsDeviceControl {

		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the resources.</summary>
		protected override void Initialize() {
			GraphicsInitialized?.Invoke(this, EventArgs.Empty);
		}


		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		/// <summary>Occurs after the dummy graphics control initializes the resources.</summary>
		public event EventHandler GraphicsInitialized;


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets an IServiceProvider containing our IGraphicsDeviceService.
		/// This can be used with components such as the ContentManager, which use this
		/// service to look up the GraphicsDevice.</summary>
		public new IServiceProvider Services {
			get { return base.Services; }
		}
	}
}
