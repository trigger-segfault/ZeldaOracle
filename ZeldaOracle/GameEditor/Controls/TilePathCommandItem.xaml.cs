using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;

namespace ZeldaEditor.Controls {

	public enum TilePathCommandTypes {
		None = -1,
		Right,
		Up,
		Left,
		Down,
		Speed,
		Pause
	}

	/// <summary>
	/// Interaction logic for TilePathCommandItem.xaml
	/// </summary>
	public partial class TilePathCommandItem : UserControl {

		//-----------------------------------------------------------------------------
		// Routed Events
		//-----------------------------------------------------------------------------

		public static readonly RoutedEvent RemoveEvent = EventManager.RegisterRoutedEvent(
			"Remove", RoutingStrategy.Bubble, typeof(RoutedEventHandler),
			typeof(TilePathCommandItem));
		public static readonly RoutedEvent InsertEvent = EventManager.RegisterRoutedEvent(
			"Insert", RoutingStrategy.Bubble, typeof(RoutedEventHandler),
			typeof(TilePathCommandItem));
		public static readonly RoutedEvent ModifiedEvent = EventManager.RegisterRoutedEvent(
			"Modified", RoutingStrategy.Bubble, typeof(RoutedEventHandler),
			typeof(TilePathCommandItem));

		public static readonly RoutedEvent DragStartedEvent     = Thumb.DragStartedEvent.AddOwner(typeof(TilePathCommandItem));
		public static readonly RoutedEvent DragCompletedEvent   = Thumb.DragCompletedEvent.AddOwner(typeof(TilePathCommandItem));
		public static readonly RoutedEvent DragDeltaEvent       = Thumb.DragDeltaEvent.AddOwner(typeof(TilePathCommandItem));

		public event RoutedEventHandler Remove {
			add { AddHandler(RemoveEvent, value); }
			remove { RemoveHandler(RemoveEvent, value); }
		}

		public event RoutedEventHandler Insert {
			add { AddHandler(InsertEvent, value); }
			remove { RemoveHandler(InsertEvent, value); }
		}

		public event RoutedEventHandler Modified {
			add { AddHandler(ModifiedEvent, value); }
			remove { RemoveHandler(ModifiedEvent, value); }
		}
		
		public event DragStartedEventHandler DragStarted {
			add { AddHandler(DragStartedEvent, value); }
			remove { RemoveHandler(DragStartedEvent, value); }
		}

		public event DragCompletedEventHandler DragCompleted {
			add { AddHandler(DragCompletedEvent, value); }
			remove { RemoveHandler(DragCompletedEvent, value); }
		}

		public event DragDeltaEventHandler DragDelta {
			add { AddHandler(DragDeltaEvent, value); }
			remove { RemoveHandler(DragDeltaEvent, value); }
		}

		//-----------------------------------------------------------------------------
		// Dependancy Properties
		//-----------------------------------------------------------------------------

		public static readonly DependencyProperty IsDraggingProperty =
			DependencyProperty.Register("IsDragging", typeof(bool), typeof(TilePathCommandItem));

		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		private bool supressEvents;
		private TilePathCommandTypes commandType;
		private int intParam;
		private float floatParam;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TilePathCommandItem() {
			supressEvents = true;
			commandType = TilePathCommandTypes.None;
			InitializeComponent();

			supressEvents = false;

			UpdateCommandTypeControls();
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		private void OnRemove(object sender, RoutedEventArgs e) {
			RaiseEvent(new RoutedEventArgs(RemoveEvent));
		}

		private void OnInsert(object sender, RoutedEventArgs e) {
			RaiseEvent(new RoutedEventArgs(InsertEvent));
		}

		private void OnIntParamChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
			if (supressEvents) return;
			if (spinnerFloatParam.Value.HasValue) {
				intParam = spinnerIntParam.Value.Value;
				RaiseEvent(new RoutedEventArgs(ModifiedEvent));
			}
		}

		private void OnFloatParamChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
			if (supressEvents) return;
			if (spinnerFloatParam.Value.HasValue) {
				floatParam = spinnerFloatParam.Value.Value;
				RaiseEvent(new RoutedEventArgs(ModifiedEvent));
			}
		}

		private void OnCommandChanged(object sender, SelectionChangedEventArgs e) {
			if (supressEvents) return;
			UpdateCommandTypeControls();
			RaiseEvent(new RoutedEventArgs(ModifiedEvent));

		}

		private void UpdateCommandTypeControls() {
			bool oldSupressEvents = supressEvents;
			supressEvents = true;
			bool previouslyMove = IsCommandTypeMove;
			commandType = (TilePathCommandTypes) comboBoxCommand.SelectedIndex;
			switch (commandType) {
			case TilePathCommandTypes.Right:
			case TilePathCommandTypes.Up:
			case TilePathCommandTypes.Left:
			case TilePathCommandTypes.Down:
				if (!previouslyMove) {
					spinnerIntParam.Visibility = Visibility.Visible;
					spinnerFloatParam.Visibility = Visibility.Collapsed;
					spinnerIntParam.Maximum = TilePath.MaxDistance;
					IntParam = 1;
				}
				break;
			case TilePathCommandTypes.Pause:
				spinnerIntParam.Visibility = Visibility.Visible;
				spinnerFloatParam.Visibility = Visibility.Collapsed;
				spinnerIntParam.Maximum = TilePath.MaxPause;
				IntParam = 30;
				break;
			case TilePathCommandTypes.Speed:
				spinnerIntParam.Visibility = Visibility.Collapsed;
				spinnerFloatParam.Visibility = Visibility.Visible;
				spinnerFloatParam.Maximum = TilePath.MaxSpeed;
				FloatParam = 0.5f;
				break;
			}
			supressEvents = oldSupressEvents;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public bool IsCommandTypeMove {
			get {
				return (commandType >= TilePathCommandTypes.Right &&
						commandType <= TilePathCommandTypes.Down);
			}
		}
		
		public Direction CommandDirection {
			get {
				if (IsCommandTypeMove)
					return (Direction) (int) commandType;
				return Direction.Invalid;
			}
		}

		public TilePathCommandTypes CommandType {
			get { return commandType; }
			set {
				commandType = value;
				supressEvents = true;
				comboBoxCommand.SelectedIndex = (int) commandType;
				UpdateCommandTypeControls();
				supressEvents = false;
			}
		}

		public int IntParam {
			get { return intParam; }
			set {
				intParam = GMath.Clamp(value, spinnerIntParam.Minimum.Value,
					spinnerIntParam.Maximum.Value);
				supressEvents = true;
				spinnerIntParam.Value = intParam;
				supressEvents = false;
			}
		}

		public float FloatParam {
			get { return floatParam; }
			set {
				floatParam = GMath.Clamp(value, spinnerFloatParam.Minimum.Value,
					spinnerFloatParam.Maximum.Value);
				supressEvents = true;
				spinnerFloatParam.Value = floatParam;
				supressEvents = false;
			}
		}
	}
}
