using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Core.Input;
using Xceed.Wpf.Toolkit.Primitives;
using ZeldaOracle.Common.Geometry;

namespace ZeldaWpf.Controls {
	/// <summary>A numeric up down for Point2Is.</summary>
	[TemplatePart(Name = PART_TextBox, Type = typeof(TextBox))]
	[TemplatePart(Name = PART_SpinnerX, Type = typeof(Spinner))]
	[TemplatePart(Name = PART_SpinnerY, Type = typeof(Spinner))]
	public class PointUpDown : InputBase, IValidateInput {
		#region Members

		/// <summary>
		/// Name constant for Text template part.
		/// </summary>
		internal const string PART_TextBox = "PART_TextBox";

		/// <summary>
		/// Name constant for Spinner template part.
		/// </summary>
		internal const string PART_SpinnerX = "PART_SpinnerX";
		/// <summary>
		/// Name constant for Spinner template part.
		/// </summary>
		internal const string PART_SpinnerY = "PART_SpinnerY";

		internal bool _isTextChangedFromUI;

		/// <summary>
		/// Flags if the Text and Value properties are in the process of being sync'd
		/// </summary>
		private bool _isSyncingTextAndValueProperties;
		private bool _internalValueSet;

		#endregion //Members

		#region Constructors

		static PointUpDown() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PointUpDown),
				new FrameworkPropertyMetadata(typeof(PointUpDown)));
		}

		protected static void UpdateMetadata(Type type, Point2I? increment, Point2I? minValue, Point2I? maxValue) {
			UpdateMetadataCommon(type, increment, minValue, maxValue);
		}

		private static void UpdateMetadataCommon(Type type, Point2I? increment, Point2I? minValue, Point2I? maxValue) {
			IncrementProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(increment));
			MaximumProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(maxValue));
			MinimumProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(minValue));
		}

		public PointUpDown() {
			this.AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent,
				new RoutedEventHandler(this.HandleClickOutsideOfControlWithMouseCapture), true);
			this.IsKeyboardFocusWithinChanged += this.UpDownBase_IsKeyboardFocusWithinChanged;

		}

		#endregion

		#region Properties

		protected Spinner SpinnerX {
			get;
			private set;
		}

		protected Spinner SpinnerY {
			get;
			private set;
		}

		protected TextBox TextBox {
			get;
			private set;
		}

		#region AllowSpin

		public static readonly DependencyProperty AllowSpinProperty =
			DependencyProperty.Register("AllowSpin", typeof(bool),
				typeof(PointUpDown), new UIPropertyMetadata( true ));
		public bool AllowSpin {
			get {
				return (bool) GetValue(AllowSpinProperty);
			}
			set {
				SetValue(AllowSpinProperty, value);
			}
		}

		#endregion //AllowSpin

		#region ButtonSpinnerLocation

		public static readonly DependencyProperty ButtonSpinnerLocationProperty =
			DependencyProperty.Register("ButtonSpinnerLocation", typeof(Location),
				typeof(PointUpDown), new UIPropertyMetadata(Location.Right));

		public Location ButtonSpinnerLocation {
			get {
				return (Location) GetValue(ButtonSpinnerLocationProperty);
			}
			set {
				SetValue(ButtonSpinnerLocationProperty, value);
			}
		}

		#endregion //ButtonSpinnerLocation

		#region ClipValueToMinMax

		public static readonly DependencyProperty ClipValueToMinMaxProperty =
			DependencyProperty.Register("ClipValueToMinMax", typeof(bool),
				typeof(PointUpDown), new UIPropertyMetadata(false));
		public bool ClipValueToMinMax {
			get {
				return (bool) GetValue(ClipValueToMinMaxProperty);
			}
			set {
				SetValue(ClipValueToMinMaxProperty, value);
			}
		}

		#endregion //ClipValueToMinMax

		#region DisplayDefaultValueOnEmptyText

		public static readonly DependencyProperty DisplayDefaultValueOnEmptyTextProperty =
			DependencyProperty.Register("DisplayDefaultValueOnEmptyText", typeof(bool), typeof(PointUpDown),
				new UIPropertyMetadata(false, OnDisplayDefaultValueOnEmptyTextChanged));
		public bool DisplayDefaultValueOnEmptyText {
			get {
				return (bool) GetValue(DisplayDefaultValueOnEmptyTextProperty);
			}
			set {
				SetValue(DisplayDefaultValueOnEmptyTextProperty, value);
			}
		}

		private static void OnDisplayDefaultValueOnEmptyTextChanged(DependencyObject source, DependencyPropertyChangedEventArgs args) {
			((PointUpDown) source).OnDisplayDefaultValueOnEmptyTextChanged((bool) args.OldValue, (bool) args.NewValue);
		}

		private void OnDisplayDefaultValueOnEmptyTextChanged(bool oldValue, bool newValue) {
			if (this.IsInitialized && string.IsNullOrEmpty(Text)) {
				this.SyncTextAndValueProperties(false, Text);
			}
		}

		#endregion //DisplayDefaultValueOnEmptyText

		#region DefaultValue

		public static readonly DependencyProperty DefaultValueProperty =
			DependencyProperty.Register( "DefaultValue", typeof(Point2I?),
				typeof(PointUpDown), new UIPropertyMetadata(default(Point2I?), OnDefaultValueChanged));
		public Point2I? DefaultValue {
			get {
				return (Point2I?) GetValue(DefaultValueProperty);
			}
			set {
				SetValue(DefaultValueProperty, value);
			}
		}

		private static void OnDefaultValueChanged(DependencyObject source, DependencyPropertyChangedEventArgs args) {
			((PointUpDown) source).OnDefaultValueChanged((Point2I?) args.OldValue, (Point2I?) args.NewValue);
		}

		private void OnDefaultValueChanged(Point2I? oldValue, Point2I? newValue) {
			if (this.IsInitialized && string.IsNullOrEmpty(Text)) {
				this.SyncTextAndValueProperties(true, Text);
			}
		}

		#endregion //DefaultValue

		#region Maximum

		public static readonly DependencyProperty MaximumProperty =
			DependencyProperty.Register("Maximum", typeof(Point2I?),
				typeof(PointUpDown), new UIPropertyMetadata(new Point2I(int.MaxValue), OnMaximumChanged, OnCoerceMaximum));
		public Point2I? Maximum {
			get {
				return (Point2I?) GetValue(MaximumProperty);
			}
			set {
				SetValue(MaximumProperty, value);
			}
		}

		private static void OnMaximumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			PointUpDown upDown = o as PointUpDown;
			if (upDown != null)
				upDown.OnMaximumChanged((Point2I?) e.OldValue, (Point2I?) e.NewValue);
		}

		protected virtual void OnMaximumChanged(Point2I? oldValue, Point2I? newValue) {
			if (this.IsInitialized) {
				SetValidSpinDirection();
			}
		}

		private static object OnCoerceMaximum(DependencyObject d, object baseValue) {
			PointUpDown upDown = d as PointUpDown;
			if (upDown != null)
				return upDown.OnCoerceMaximum((Point2I?) baseValue);

			return baseValue;
		}

		protected virtual Point2I? OnCoerceMaximum(Point2I? baseValue) {
			return baseValue;
		}

		#endregion //Maximum

		#region Minimum

		public static readonly DependencyProperty MinimumProperty =
			DependencyProperty.Register("Minimum", typeof(Point2I?),
				typeof(PointUpDown), new UIPropertyMetadata(new Point2I(int.MinValue), OnMinimumChanged, OnCoerceMinimum));
		public Point2I? Minimum {
			get {
				return (Point2I?) GetValue(MinimumProperty);
			}
			set {
				SetValue(MinimumProperty, value);
			}
		}

		private static void OnMinimumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			PointUpDown upDown = o as PointUpDown;
			if (upDown != null)
				upDown.OnMinimumChanged((Point2I?) e.OldValue, (Point2I?) e.NewValue);
		}

		protected virtual void OnMinimumChanged(Point2I? oldValue, Point2I? newValue) {
			if (this.IsInitialized) {
				SetValidSpinDirection();
			}
		}

		private static object OnCoerceMinimum(DependencyObject d, object baseValue) {
			PointUpDown upDown = d as PointUpDown;
			if (upDown != null)
				return upDown.OnCoerceMinimum((Point2I?) baseValue);

			return baseValue;
		}

		protected virtual Point2I? OnCoerceMinimum(Point2I? baseValue) {
			return baseValue;
		}

		#endregion //Minimum

		#region MouseWheelActiveTrigger

		/// <summary>
		/// Identifies the MouseWheelActiveTrigger dependency property
		/// </summary>
		public static readonly DependencyProperty MouseWheelActiveTriggerProperty =
			DependencyProperty.Register("MouseWheelActiveTrigger", typeof(MouseWheelActiveTrigger),
				typeof(PointUpDown), new UIPropertyMetadata(MouseWheelActiveTrigger.FocusedMouseOver));

		/// <summary>
		/// Get or set when the mouse wheel event should affect the value.
		/// </summary>
		public MouseWheelActiveTrigger MouseWheelActiveTrigger {
			get {
				return (MouseWheelActiveTrigger) GetValue(MouseWheelActiveTriggerProperty);
			}
			set {
				SetValue(MouseWheelActiveTriggerProperty, value);
			}
		}

		#endregion //MouseWheelActiveTrigger

		#region MouseWheelActiveOnFocus

		[Obsolete( "Use MouseWheelActiveTrigger property instead" )]
		public static readonly DependencyProperty MouseWheelActiveOnFocusProperty =
			DependencyProperty.Register("MouseWheelActiveOnFocus", typeof(bool),
				typeof(PointUpDown), new UIPropertyMetadata(true, OnMouseWheelActiveOnFocusChanged));

		[Obsolete("Use MouseWheelActiveTrigger property instead")]
		public bool MouseWheelActiveOnFocus {
			get {
#pragma warning disable 618
				return (bool) GetValue(MouseWheelActiveOnFocusProperty);
#pragma warning restore 618
			}
			set {
#pragma warning disable 618
				SetValue(MouseWheelActiveOnFocusProperty, value);
#pragma warning restore 618
			}
		}

		private static void OnMouseWheelActiveOnFocusChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			PointUpDown upDownBase = o as PointUpDown;
			if (upDownBase != null)
				upDownBase.MouseWheelActiveTrigger = ((bool) e.NewValue)
				  ? MouseWheelActiveTrigger.FocusedMouseOver
				  : MouseWheelActiveTrigger.MouseOver;
		}

		#endregion //MouseWheelActiveOnFocus

		#region ShowButtonSpinner

		public static readonly DependencyProperty ShowButtonSpinnerProperty =
			DependencyProperty.Register("ShowButtonSpinner", typeof(bool),
				typeof(PointUpDown), new UIPropertyMetadata(true));
		public bool ShowButtonSpinner {
			get {
				return (bool) GetValue(ShowButtonSpinnerProperty);
			}
			set {
				SetValue(ShowButtonSpinnerProperty, value);
			}
		}

		#endregion //ShowButtonSpinner

		#region UpdateValueOnEnterKey

		public static readonly DependencyProperty UpdateValueOnEnterKeyProperty =
			DependencyProperty.Register("UpdateValueOnEnterKey", typeof(bool),
				typeof(PointUpDown), new FrameworkPropertyMetadata(false, OnUpdateValueOnEnterKeyChanged));
		public bool UpdateValueOnEnterKey {
			get {
				return (bool) GetValue(UpdateValueOnEnterKeyProperty);
			}
			set {
				SetValue(UpdateValueOnEnterKeyProperty, value);
			}
		}

		private static void OnUpdateValueOnEnterKeyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			var upDownBase = o as PointUpDown;
			if (upDownBase != null)
				upDownBase.OnUpdateValueOnEnterKeyChanged((bool) e.OldValue, (bool) e.NewValue);
		}

		protected virtual void OnUpdateValueOnEnterKeyChanged(bool oldValue, bool newValue) {
		}

		#endregion //UpdateValueOnEnterKey

		#region Value

		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(Point2I?),
				typeof(PointUpDown), new FrameworkPropertyMetadata(default(PointUpDown),
					FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
					OnValueChanged, OnCoerceValue, false, UpdateSourceTrigger.PropertyChanged));
		public Point2I? Value {
			get {
				return (Point2I?) GetValue(ValueProperty);
			}
			set {
				SetValue(ValueProperty, value);
			}
		}

		private void SetValueInternal(Point2I? value) {
			_internalValueSet = true;
			try {
				this.Value = value;
			}
			finally {
				_internalValueSet = false;
			}
		}

		private static object OnCoerceValue(DependencyObject o, object basevalue) {
			return ((PointUpDown) o).OnCoerceValue(basevalue);
		}

		protected virtual object OnCoerceValue(object newValue) {
			return newValue;
		}

		private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			PointUpDown upDownBase = o as PointUpDown;
			if (upDownBase != null)
				upDownBase.OnValueChanged((Point2I?) e.OldValue, (Point2I?) e.NewValue);
		}

		protected virtual void OnValueChanged(Point2I? oldValue, Point2I? newValue) {
			if (!_internalValueSet && this.IsInitialized) {
				SyncTextAndValueProperties(false, null, true);
			}

			SetValidSpinDirection();

			this.RaiseValueChangedEvent(oldValue, newValue);
		}

		#endregion //Value

		#region AutoMoveFocus

		public bool AutoMoveFocus {
			get {
				return (bool) GetValue(AutoMoveFocusProperty);
			}
			set {
				SetValue(AutoMoveFocusProperty, value);
			}
		}

		public static readonly DependencyProperty AutoMoveFocusProperty =
			DependencyProperty.Register("AutoMoveFocus", typeof(bool), typeof(PointUpDown), new UIPropertyMetadata(false));

		#endregion AutoMoveFocus

		#region AutoSelectBehavior

		public AutoSelectBehavior AutoSelectBehavior {
			get {
				return (AutoSelectBehavior) GetValue(AutoSelectBehaviorProperty);
			}
			set {
				SetValue(AutoSelectBehaviorProperty, value);
			}
		}

		public static readonly DependencyProperty AutoSelectBehaviorProperty =
			DependencyProperty.Register("AutoSelectBehavior", typeof(AutoSelectBehavior), typeof(PointUpDown),
				new UIPropertyMetadata(AutoSelectBehavior.OnFocus));

		#endregion AutoSelectBehavior PROPERTY

		#region FormatString

		public static readonly DependencyProperty FormatStringProperty =
			DependencyProperty.Register("FormatString", typeof(string), typeof(PointUpDown),
				new UIPropertyMetadata(String.Empty, OnFormatStringChanged));
		public string FormatString {
			get {
				return (string) GetValue(FormatStringProperty);
			}
			set {
				SetValue(FormatStringProperty, value);
			}
		}

		private static void OnFormatStringChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			PointUpDown numericUpDown = o as PointUpDown;
			if (numericUpDown != null)
				numericUpDown.OnFormatStringChanged((string) e.OldValue, (string) e.NewValue);
		}

		protected virtual void OnFormatStringChanged(string oldValue, string newValue) {
			if (IsInitialized) {
				this.SyncTextAndValueProperties(false, null);
			}
		}

		#endregion //FormatString

		#region Increment

		public static readonly DependencyProperty IncrementProperty =
			DependencyProperty.Register("Increment", typeof(Point2I?), typeof(PointUpDown),
				new PropertyMetadata(Point2I.One, OnIncrementChanged, OnCoerceIncrement));
		public Point2I? Increment {
			get {
				return (Point2I?) GetValue(IncrementProperty);
			}
			set {
				SetValue(IncrementProperty, value);
			}
		}

		private static void OnIncrementChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			PointUpDown numericUpDown = o as PointUpDown;
			if (numericUpDown != null)
				numericUpDown.OnIncrementChanged((Point2I?) e.OldValue, (Point2I?) e.NewValue);
		}

		protected virtual void OnIncrementChanged(Point2I? oldValue, Point2I? newValue) {
			if (this.IsInitialized) {
				SetValidSpinDirection();
			}
		}

		private static object OnCoerceIncrement(DependencyObject d, object baseValue) {
			PointUpDown numericUpDown = d as PointUpDown;
			if (numericUpDown != null)
				return numericUpDown.OnCoerceIncrement((Point2I?) baseValue);

			return baseValue;
		}

		protected virtual Point2I? OnCoerceIncrement(Point2I? baseValue) {
			return baseValue;
		}

		#endregion

		#endregion //Properties

		#region Base Class Overrides

		protected override void OnAccessKey(AccessKeyEventArgs e) {
			if (TextBox != null)
				TextBox.Focus();

			base.OnAccessKey(e);
		}

		public override void OnApplyTemplate() {
			base.OnApplyTemplate();

			if (TextBox != null) {
				TextBox.TextChanged -= new TextChangedEventHandler(TextBox_TextChanged);
				TextBox.RemoveHandler(Mouse.PreviewMouseDownEvent, new MouseButtonEventHandler(this.TextBox_PreviewMouseDown));
			}

			TextBox = GetTemplateChild(PART_TextBox) as TextBox;

			if (TextBox != null) {
				TextBox.Text = Text;
				TextBox.TextChanged += new TextChangedEventHandler(TextBox_TextChanged);
				TextBox.AddHandler(Mouse.PreviewMouseDownEvent, new MouseButtonEventHandler(this.TextBox_PreviewMouseDown), true);
			}

			if (SpinnerX != null)
				SpinnerX.Spin -= OnSpinnerXSpin;

			SpinnerX = GetTemplateChild(PART_SpinnerX) as Spinner;

			if (SpinnerX != null)
				SpinnerX.Spin += OnSpinnerXSpin;


			if (SpinnerY != null)
				SpinnerY.Spin -= OnSpinnerXSpin;

			SpinnerY = GetTemplateChild(PART_SpinnerY) as Spinner;

			if (SpinnerY != null)
				SpinnerY.Spin += OnSpinnerYSpin;

			SetValidSpinDirection();
		}

		protected override void OnKeyDown(KeyEventArgs e) {
			switch (e.Key) {
			case Key.Enter: {
					// Commit Text on "Enter" to raise Error event 
					bool commitSuccess = CommitInput();
					//Only handle if an exception is detected (Commit fails)
					e.Handled = !commitSuccess;
					break;
				}
			}
		}

		protected override void OnTextChanged(string oldValue, string newValue) {
			if (this.IsInitialized) {
				// When text is typed, if UpdateValueOnEnterKey is true, 
				// Sync Value on Text only when Enter Key is pressed.
				if (this.UpdateValueOnEnterKey) {
					if (!_isTextChangedFromUI) {
						this.SyncTextAndValueProperties(true, Text);
					}
				}
				else {
					this.SyncTextAndValueProperties(true, Text);
				}
			}
		}

		protected override void OnCultureInfoChanged(CultureInfo oldValue, CultureInfo newValue) {
			if (IsInitialized) {
				SyncTextAndValueProperties(false, null);
			}
		}

		protected override void OnReadOnlyChanged(bool oldValue, bool newValue) {
			SetValidSpinDirection();
		}

		#endregion //Base Class Overrides

		#region Event Handlers

		private void TextBox_PreviewMouseDown(object sender, RoutedEventArgs e) {
			if (this.MouseWheelActiveTrigger == MouseWheelActiveTrigger.Focused) {
				//Capture the spinner when user clicks on the control.
				/*if (Mouse.Captured != this.Spinner) {
					//Delay the capture to let the DateTimeUpDown select a new DateTime part.
					this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => {
						Mouse.Capture(this.Spinner);
					}
					));
				}*/
			}
		}

		private void HandleClickOutsideOfControlWithMouseCapture(object sender, RoutedEventArgs e) {
			if (Mouse.Captured is Spinner) {
				//Release the captured spinner when user clicks away from spinner.
				//this.Spinner.ReleaseMouseCapture();
			}
		}

		private void OnSpinnerXSpin(object sender, SpinEventArgs e) {
			if (AllowSpin && !IsReadOnly) {
				var activeTrigger = this.MouseWheelActiveTrigger;
				bool spin = !e.UsingMouseWheel;
				spin |= (activeTrigger == MouseWheelActiveTrigger.MouseOver);
				spin |= ((TextBox  != null) && TextBox.IsFocused && (activeTrigger == MouseWheelActiveTrigger.FocusedMouseOver));
				spin |= ((TextBox != null) && TextBox.IsFocused && (activeTrigger == MouseWheelActiveTrigger.Focused) && (Mouse.Captured is Spinner));

				if (spin) {
					e.Handled = true;
					OnXSpin(e);
				}
			}
		}

		private void OnSpinnerYSpin(object sender, SpinEventArgs e) {
			if (AllowSpin && !IsReadOnly) {
				var activeTrigger = this.MouseWheelActiveTrigger;
				bool spin = !e.UsingMouseWheel;
				spin |= (activeTrigger == MouseWheelActiveTrigger.MouseOver);
				spin |= ((TextBox  != null) && TextBox.IsFocused && (activeTrigger == MouseWheelActiveTrigger.FocusedMouseOver));
				spin |= ((TextBox != null) && TextBox.IsFocused && (activeTrigger == MouseWheelActiveTrigger.Focused) && (Mouse.Captured is Spinner));

				if (spin) {
					e.Handled = true;
					OnYSpin(e);
				}
			}
		}

		#endregion //Event Handlers

		#region Events

		public event InputValidationErrorEventHandler InputValidationError;

		public event EventHandler<SpinEventArgs> Spinned;

		#region ValueChanged Event

		//Due to a bug in Visual Studio, you cannot create event handlers for generic T args in XAML, so I have to use object instead.
		public static readonly RoutedEvent ValueChangedEvent =
			EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble,
				typeof(RoutedPropertyChangedEventHandler<object> ), typeof(PointUpDown));
		public event RoutedPropertyChangedEventHandler<object> ValueChanged {
			add { AddHandler( ValueChangedEvent, value ); }
			remove { RemoveHandler( ValueChangedEvent, value ); }
		}

		#endregion

		#endregion //Events

		#region Methods

		protected virtual void OnXSpin(SpinEventArgs e) {
			if (e == null)
				throw new ArgumentNullException("e");

			// Raise the Spinned event to user
			EventHandler<SpinEventArgs> handler = this.Spinned;
			if (handler != null) {
				handler(this, e);
			}

			if (e.Direction == SpinDirection.Increase)
				DoIncrement(false);
			else
				DoDecrement(false);
		}

		protected virtual void OnYSpin(SpinEventArgs e) {
			if (e == null)
				throw new ArgumentNullException("e");

			// Raise the Spinned event to user
			EventHandler<SpinEventArgs> handler = this.Spinned;
			if (handler != null) {
				handler(this, e);
			}

			if (e.Direction == SpinDirection.Increase)
				DoIncrement(true);
			else
				DoDecrement(true);
		}

		protected virtual void RaiseValueChangedEvent(Point2I? oldValue, Point2I? newValue) {
			RoutedPropertyChangedEventArgs<object> args = new RoutedPropertyChangedEventArgs<object>( oldValue, newValue );
			args.RoutedEvent = ValueChangedEvent;
			RaiseEvent(args);
		}

		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			// When both Value and Text are initialized, Value has priority.
			// To be sure that the value is not initialized, it should
			// have no local value, no binding, and equal to the default value.
			bool updateValueFromText =
				(this.ReadLocalValue( ValueProperty ) == DependencyProperty.UnsetValue)
				&& (BindingOperations.GetBinding( this, ValueProperty ) == null)
				&& (object.Equals( this.Value, ValueProperty.DefaultMetadata.DefaultValue ));

			this.SyncTextAndValueProperties(updateValueFromText, Text, !updateValueFromText);
		}

		/// <summary>
		/// Performs an increment if conditions allow it.
		/// </summary>
		internal void DoDecrement(bool yaxis) {
			if (!yaxis) {
				if (SpinnerX == null || (SpinnerX.ValidSpinDirection & ValidSpinDirections.Decrease) == ValidSpinDirections.Decrease)
					OnDecrement(false);
			}
			else {
				if (SpinnerY == null || (SpinnerY.ValidSpinDirection & ValidSpinDirections.Decrease) == ValidSpinDirections.Decrease)
					OnDecrement(true);
			}
		}

		/// <summary>
		/// Performs a decrement if conditions allow it.
		/// </summary>
		internal void DoIncrement(bool yaxis) {
			if (!yaxis) {
				if (SpinnerX == null || (SpinnerX.ValidSpinDirection & ValidSpinDirections.Increase) == ValidSpinDirections.Increase)
					OnIncrement(false);
			}
			else {
				if (SpinnerY == null || (SpinnerY.ValidSpinDirection & ValidSpinDirections.Increase) == ValidSpinDirections.Increase)
					OnIncrement(true);
			}
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
			if (!this.IsKeyboardFocusWithin)
				return;

			try {
				_isTextChangedFromUI = true;
				Text = ((TextBox) sender).Text;
			}
			finally {
				_isTextChangedFromUI = false;
			}
		}

		private void UpDownBase_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if (!(bool) e.NewValue) {
				this.CommitInput();
			}
		}

		private void RaiseInputValidationError(Exception e) {
			if (InputValidationError != null) {
				InputValidationErrorEventArgs args = new InputValidationErrorEventArgs( e );
				InputValidationError(this, args);
				if (args.ThrowException) {
					throw args.Exception;
				}
			}
		}

		public virtual bool CommitInput() {
			return this.SyncTextAndValueProperties(true, Text);
		}

		protected bool SyncTextAndValueProperties(bool updateValueFromText, string text) {
			return this.SyncTextAndValueProperties(updateValueFromText, text, false);
		}

		private bool SyncTextAndValueProperties(bool updateValueFromText, string text, bool forceTextUpdate) {
			if (_isSyncingTextAndValueProperties)
				return true;

			_isSyncingTextAndValueProperties = true;
			bool parsedTextIsValid = true;
			try {
				if (updateValueFromText) {
					if (string.IsNullOrEmpty(text)) {
						// An empty input sets the value to the default value.
						this.SetValueInternal(this.DefaultValue);
					}
					else {
						try {
							Point2I? newValue = this.ConvertTextToValue( text );
							if (!object.Equals(newValue, this.Value)) {
								this.SetValueInternal(newValue);
							}
						}
						catch (Exception e) {
							parsedTextIsValid = false;

							// From the UI, just allow any input.
							if (!_isTextChangedFromUI) {
								// This call may throw an exception. 
								// See RaiseInputValidationError() implementation.
								this.RaiseInputValidationError(e);
							}
						}
					}
				}

				// Do not touch the ongoing text input from user.
				if (!_isTextChangedFromUI) {
					// Don't replace the empty Text with the non-empty representation of DefaultValue.
					bool shouldKeepEmpty = !forceTextUpdate && string.IsNullOrEmpty( Text ) && object.Equals( Value, DefaultValue ) && !this.DisplayDefaultValueOnEmptyText;
					if (!shouldKeepEmpty) {
						string newText = ConvertValueToText();
						if (!object.Equals(this.Text, newText)) {
							Text = newText;
						}
					}

					// Sync Text and textBox
					if (TextBox != null)
						TextBox.Text = Text;
				}

				if (_isTextChangedFromUI && !parsedTextIsValid) {
					// Text input was made from the user and the text
					// repesents an invalid value. Disable the spinner
					// in this case.
					if (SpinnerX != null) {
						SpinnerX.ValidSpinDirection = ValidSpinDirections.None;
					}
					if (SpinnerY != null) {
						SpinnerY.ValidSpinDirection = ValidSpinDirections.None;
					}
				}
				else {
					this.SetValidSpinDirection();
				}
			}
			finally {
				_isSyncingTextAndValueProperties = false;
			}
			return parsedTextIsValid;
		}

		#region Abstract
		
		protected Point2I? ConvertTextToValue(string text) {
			Point2I? result = null;

			if (String.IsNullOrEmpty(text))
				return result;

			// Since the conversion from Value to text using a FormartString may not be parsable,
			// we verify that the already existing text is not the exact same value.
			string currentValueText = ConvertValueToText();
			if (object.Equals(currentValueText, text))
				return this.Value;

			result = this.ConvertTextToValueCore(currentValueText, text);

			if (this.ClipValueToMinMax) {
				return this.GetClippedMinMaxValue(result);
			}

			ValidateDefaultMinMax(result);

			return result;
		}

		protected string ConvertValueToText() {
			if (Value == null)
				return string.Empty;

			//Manage FormatString of type "{}{0:N2} °" (in xaml) or "{0:N2} °" in code-behind.
			//if (FormatString.Contains("{0"))
			//	return string.Format(CultureInfo, FormatString, Value.Value);

			return Value.Value.X + "," + Value.Value.Y;
		}

		protected void OnIncrement(bool yaxis) {
			if (!HandleNullSpin()) {
				// if UpdateValueOnEnterKey is true, 
				// Sync Value on Text only when Enter Key is pressed.
				if (this.UpdateValueOnEnterKey) {
					var currentValue = this.ConvertTextToValue( this.TextBox.Text );
					var result = currentValue.Value + Point2I.FromBoolean(yaxis, Increment.Value[yaxis]);
					var newValue = this.CoerceValueMinMax( result );
					this.TextBox.Text = newValue.Value.ToString(this.FormatString, this.CultureInfo);
				}
				else {
					var result = Value.Value + Point2I.FromBoolean(yaxis, Increment.Value[yaxis]);
					this.Value = this.CoerceValueMinMax(result);
				}
			}
		}

		protected void OnDecrement(bool yaxis) {
			if (!HandleNullSpin()) {
				// if UpdateValueOnEnterKey is true, 
				// Sync Value on Text only when Enter Key is pressed.
				if (this.UpdateValueOnEnterKey) {
					var currentValue = this.ConvertTextToValue( this.TextBox.Text );
					var result = currentValue.Value - Point2I.FromBoolean(yaxis, Increment.Value[yaxis]);
					var newValue = this.CoerceValueMinMax( result );
					this.TextBox.Text = newValue.Value.ToString(this.FormatString, this.CultureInfo);
				}
				else {
					var result = Value.Value - Point2I.FromBoolean(yaxis, Increment.Value[yaxis]);
					this.Value = this.CoerceValueMinMax(result);
				}
			}
		}

		#endregion //Abstract

		#endregion //Methods
		
		#region Implementation
		private bool IsLowerThan(Point2I? value1, Point2I? value2) {
			if (value1 == null || value2 == null)
				return false;

			return (value1.Value < value2.Value);
		}

		private bool IsGreaterThan(Point2I? value1, Point2I? value2) {
			if (value1 == null || value2 == null)
				return false;

			return (value1.Value > value2.Value);
		}

		private bool IsLowerThan(Point2I? value1, Point2I? value2, bool yaxis) {
			if (value1 == null || value2 == null)
				return false;

			return (value1.Value[yaxis] < value2.Value[yaxis]);
		}

		private bool IsGreaterThan(Point2I? value1, Point2I? value2, bool yaxis) {
			if (value1 == null || value2 == null)
				return false;

			return (value1.Value[yaxis] > value2.Value[yaxis]);
		}

		private bool HandleNullSpin() {
			if (!Value.HasValue) {
				Point2I forcedValue = ( DefaultValue.HasValue )
				  ? DefaultValue.Value
				  : default(Point2I);

				Value = CoerceValueMinMax(forcedValue);

				return true;
			}
			else if (!Increment.HasValue) {
				return true;
			}

			return false;
		}

		internal bool IsValid(Point2I? value) {
			return !IsLowerThan(value, Minimum) && !IsGreaterThan(value, Maximum);
		}

		private Point2I? CoerceValueMinMax(Point2I value) {
			if (Minimum != null) {
				if (Maximum != null) {
					return GMath.Clamp(value, Minimum.Value, Maximum.Value);
				}
				return GMath.Max(value, Minimum.Value);
			}
			else if (Maximum != null) {
				return GMath.Min(value, Maximum.Value);
			}
			return value;
		}

		protected void SetValidSpinDirection() {
			ValidSpinDirections validDirections = ValidSpinDirections.None;

			// Null increment always prevents spin.
			if ((this.Increment != null) && !IsReadOnly) {
				if (IsLowerThan(Value, Maximum, false) || !Value.HasValue || !Maximum.HasValue)
					validDirections = validDirections | ValidSpinDirections.Increase;

				if (IsGreaterThan(Value, Minimum, false) || !Value.HasValue || !Minimum.HasValue)
					validDirections = validDirections | ValidSpinDirections.Decrease;
			}

			if (SpinnerX != null)
				SpinnerX.ValidSpinDirection = validDirections;

			validDirections = ValidSpinDirections.None;

			if ((this.Increment != null) && !IsReadOnly) {
				if (IsLowerThan(Value, Maximum, true) || !Value.HasValue || !Maximum.HasValue)
					validDirections = validDirections | ValidSpinDirections.Increase;

				if (IsGreaterThan(Value, Minimum, true) || !Value.HasValue || !Minimum.HasValue)
					validDirections = validDirections | ValidSpinDirections.Decrease;
			}

			if (SpinnerY != null)
				SpinnerY.ValidSpinDirection = validDirections;
		}



		private Point2I? ConvertTextToValueCore(string currentValueText, string text) {
			Point2I? result;
			
			Point2I outputValue = new Point2I();
			// Problem while converting new text
			if (!Point2I.TryParse(text, out outputValue)) {
				bool shouldThrow = true;
				
				if (shouldThrow)
					throw new InvalidDataException("Input string was not in a correct format.");
			}
			result = outputValue;
			return result;
		}

		private Point2I? GetClippedMinMaxValue(Point2I? result) {
			if (result == null)
				return null;

			if (Minimum != null) {
				if (Maximum != null) {
					return GMath.Clamp(result.Value, Minimum.Value, Maximum.Value);
				}
				return GMath.Max(result.Value, Minimum.Value);
			}
			else if (Maximum != null) {
				return GMath.Min(result.Value, Maximum.Value);
			}
			return result.Value;
		}

		private void ValidateDefaultMinMax(Point2I? value) {
			// DefaultValue is always accepted.
			if (object.Equals(value, DefaultValue))
				return;

			if (IsLowerThan(value, Minimum))
				throw new ArgumentOutOfRangeException("Minimum", String.Format("Value must be greater than MinValue of {0}", Minimum));
			else if (IsGreaterThan(value, Maximum))
				throw new ArgumentOutOfRangeException("Maximum", String.Format("Value must be less than MaxValue of {0}", Maximum));
		}
		#endregion
	}
}
