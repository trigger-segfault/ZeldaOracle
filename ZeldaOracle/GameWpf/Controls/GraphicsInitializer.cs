using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms.Integration;
using Microsoft.Xna.Framework.Graphics;
using ZeldaWpf.WinForms;

namespace ZeldaWpf.Controls {
	/// <summary>Event arguments for Xna graphics initializing.</summary>
	public class GraphicsInitializerEventArgs : RoutedEventArgs {
		/// <summary>Gets the graphics device used to initialize the resources.</summary>
		public GraphicsDevice GraphicsDevice { get; }
		/// <summary>Gets the service provider used to initialize the resources.</summary>
		public IServiceProvider Services { get; }

		/// <summary>Constructs the event args.</summary>
		public GraphicsInitializerEventArgs(GraphicsInitializer self,
			GraphicsDevice graphicsDevice, IServiceProvider services)
			: base(GraphicsInitializer.GraphicsInitializedEvent, self)
		{
			GraphicsDevice = graphicsDevice;
			Services = services;
		}
	}

	/// <summary>The event for initializing Xna graphics devices.</summary>
	public delegate void GraphicsInitializerEventHandler(object sender, GraphicsInitializerEventArgs e);
	
	/// <summary>A windows forms host that handles initializing Graphics Devices.</summary>
	public class GraphicsInitializer : WindowsFormsHost {

		/// <summary>The routed event for when the graphics have been initialized.</summary>
		public static readonly RoutedEvent GraphicsInitializedEvent =
			EventManager.RegisterRoutedEvent("GraphicsInitialized", RoutingStrategy.Bubble,
				typeof(GraphicsInitializerEventHandler), typeof(GraphicsInitializer));
		
		/// <summary>Occurs when the graphics have been initialized.</summary>
		public event GraphicsInitializerEventHandler GraphicsInitialized {
			add { AddHandler(GraphicsInitializedEvent, value); }
			remove { RemoveHandler(GraphicsInitializedEvent, value); }
		}

		/// <summary>The Xna Graphics Device Initializer.</summary>
		private GraphicsDeviceInitializer initializer;

		/// <summary>Constructs the graphics initializer.</summary>
		public GraphicsInitializer() {
			Width = 0;
			Height = 0;
			if (!DesignerProperties.GetIsInDesignMode(this)) {
				initializer = new GraphicsDeviceInitializer();
				initializer.GraphicsInitialized += OnGraphicsInitialized;
				Child = initializer;
			}
		}

		/// <summary>Raises the GraphicsInitialized routed event.</summary>
		private void OnGraphicsInitialized(object sender, EventArgs e) {
			RaiseEvent(new GraphicsInitializerEventArgs(this,
				initializer.GraphicsDevice, initializer.Services));
		}
	}
}
