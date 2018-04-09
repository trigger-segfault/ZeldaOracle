using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms.Integration;
using ZeldaWpf.WinForms;

namespace ZeldaWpf.Controls {
	/// <summary>A windows forms host that handles initializing Graphics Devices.</summary>
	public class GraphicsInitializer : WindowsFormsHost {

		/// <summary>The routed event for when the graphics have been initialized.</summary>
		public static RoutedEvent GraphicsInitializedEvent =
			EventManager.RegisterRoutedEvent("GraphicsInitialized", RoutingStrategy.Bubble,
				typeof(RoutedEventHandler), typeof(GraphicsInitializer));
		
		/// <summary>Occurs when the graphics have been initialized.</summary>
		public event RoutedEventHandler GraphicsInitialized {
			add { AddHandler(GraphicsInitializedEvent, value); }
			remove { RemoveHandler(GraphicsInitializedEvent, value); }
		}

		/// <summary>Constructs the graphics initializer.</summary>
		public GraphicsInitializer() {
			if (!DesignerProperties.GetIsInDesignMode(this)) {
				GraphicsDeviceInitializer initializer =
					new GraphicsDeviceInitializer();
				initializer.GraphicsInitialized += OnGraphicsInitialized;
				Child = initializer;
			}
		}

		/// <summary>Raises the GraphicsInitialized routed event.</summary>
		private void OnGraphicsInitialized(object sender, EventArgs e) {
			RaiseEvent(new RoutedEventArgs(GraphicsInitializedEvent, this));
		}
	}
}
