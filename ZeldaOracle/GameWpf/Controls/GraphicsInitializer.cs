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

		//-----------------------------------------------------------------------------
		// Routed Events
		//-----------------------------------------------------------------------------

		/// <summary>The routed event for when the graphics have been initialized.</summary>
		public static readonly RoutedEvent GraphicsInitializedEvent =
			EventManager.RegisterRoutedEvent("GraphicsInitialized", RoutingStrategy.Bubble,
				typeof(GraphicsInitializerEventHandler), typeof(GraphicsInitializer));
		
		/// <summary>Occurs when the graphics have been initialized.</summary>
		public event GraphicsInitializerEventHandler GraphicsInitialized {
			add { AddHandler(GraphicsInitializedEvent, value); }
			remove { RemoveHandler(GraphicsInitializedEvent, value); }
		}


		//-----------------------------------------------------------------------------
		// Dependency Properties
		//-----------------------------------------------------------------------------

		/// <summary>The dependency property for setting up the graphics control on
		/// initialization.</summary>
		public static readonly DependencyProperty ReadyToInitializeProperty =
			DependencyProperty.Register("ReadyToInitialize", typeof(bool),
				typeof(GraphicsInitializer), new FrameworkPropertyMetadata(true,
					OnReadyToInitializeChanged));

		/// <summary>Initializes the graphics control.</summary>
		private static void OnReadyToInitializeChanged(object sender,
			DependencyPropertyChangedEventArgs e)
		{
			GraphicsInitializer element = sender as GraphicsInitializer;
			if (element?.ReadyToInitialize ?? false) {
				element.Initialize();
			}
		}

		/// <summary>Gets or sets if the graphics can be initialized yet.</summary>
		public bool ReadyToInitialize {
			get { return (bool) GetValue(ReadyToInitializeProperty); }
			set { SetValue(ReadyToInitializeProperty, value); }
		}


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The Xna Graphics Device Initializer.</summary>
		private GraphicsDeviceInitializer initializer;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the graphics initializer.</summary>
		public GraphicsInitializer() {
			Width = 0;
			Height = 0;
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		/// <summary>Raises the GraphicsInitialized routed event.</summary>
		private void OnGraphicsInitialized(object sender, EventArgs e) {
			RaiseEvent(new GraphicsInitializerEventArgs(this,
				initializer.GraphicsDevice, initializer.Services));
		}
		
		/// <summary>Initializes the graphics control if ready.</summary>
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			if (ReadyToInitialize)
				Initialize();
		}


		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------

		/// <summary>Starts initialization by setting up the graphics control.</summary>
		public void Initialize() {
			if (Child != null)
				return;
			if (!DesignerProperties.GetIsInDesignMode(this)) {
				initializer = new GraphicsDeviceInitializer();
				initializer.GraphicsInitialized += OnGraphicsInitialized;
				Child = initializer;
			}
		}
	}

	/// <summary>A windows forms host that handles auto-initializing Graphics Devices.</summary>
	public class AutoGraphicsInitializer : GraphicsInitializer {

		/// <summary>Constructs the auto graphics initializer.</summary>
		public AutoGraphicsInitializer() {
			Width = 0;
			Height = 0;
			Initialize();
		}
	}
}
