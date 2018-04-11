using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace ZeldaWpf.Controls {
	// https://blogs.msdn.microsoft.com/mikehillberg/2008/09/30/a-comparable-datatrigger/

	/// <summary>ComparisonBinding is a Binding that should be used in a
	/// <see cref="DataTrigger.Binding"/> It supports a comparison operator and a
	/// comparand, so that you can use it as a. conditional DataTrigger. The trick
	/// is to set {x:Null} as the <see cref="DataTrigger.Value"/>.<para/>
	/// The operator can be 'EQ', 'NEQ', 'GT', 'GTE', 'LT', or 'LTE'.</summary>
	/// <example>&lt;DataTrigger Value={x:Null}
	/// Binding={h:ComparisonBinding Width, EQ, 100}" Value="True" &gt;</example>
	public class ComparisonBinding : Binding {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the comparison binding.</summary>
		public ComparisonBinding() : this(null, ComparisonOperators.EQ, null) {
		}
		
		/// <summary>Constructs the comparison binding with the specified operator
		/// and comparand.</summary>
		public ComparisonBinding(string path, ComparisonOperators op) : base(path)
		{
			RelativeSource = RelativeSource.Self;
			Comparand = null;
			Operator = op;
			//Converter = new ComparisonConverter(this);
		}

		/// <summary>Constructs the comparison binding with the specified operator
		/// and comparand.</summary>
		public ComparisonBinding(string path, ComparisonOperators op,
			object comparand) : base(path)
		{
			RelativeSource = RelativeSource.Self;
			Comparand = comparand;
			Operator = op;
			//Converter = new ComparisonConverter(this);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the operator used for comparison.</summary>
		public ComparisonOperators Operator { get; set; }

		/// <summary>Gets the value being compared to.</summary>
		public object Comparand { get; set; }
	}
	
	/// <summary>Supported types of comparisons.</summary>
	public enum ComparisonOperators {
		/// <summary>Returns true if the value is null.</summary>
		NULL = 0,
		/// <summary>Returns true if the values are equal.</summary>
		EQ,
		/// <summary>Returns true if the values are not equal.</summary>
		NEQ,
		/// <summary>Returns true if the value is greater.</summary>
		GT,
		/// <summary>Returns true if the value is greater or equal.</summary>
		GTE,
		/// <summary>Returns true if the value is less.</summary>
		LT,
		/// <summary>Returns true if the value is less or equal.</summary>
		LTE
	}
	
	/// <summary>Thie IValueConverter is used by the StyleBinding to implement the
	/// logical comparisson. ConvertBack isn't supported. Convert returns true if
	/// the condition is met and false otherwise.</summary>
	internal class ComparisonConverter : IValueConverter {

		/// <summary>Return this if the condition isn't met.</summary>
		private static object notNull = new object();
		/// <summary>Keep a back reference to the StyleBinding.</summary>
		private ComparisonBinding bind;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>In construction, get a reference to the StyleBinding.</summary>
		public ComparisonConverter(ComparisonBinding bind) {
			this.bind = bind;
		}


		//-----------------------------------------------------------------------------
		// IValueConverter Overrides
		//-----------------------------------------------------------------------------

		/// <summary>Return true if the condition is met, false if not.</summary>
		public object Convert(object value, Type targetType, object parameter,
			CultureInfo culture)
		{
			// Simple check for null
			if (value == null || (bind.Comparand == null ||
				bind.Operator == ComparisonOperators.NULL))
			{
				return ReturnHelper(value == bind.Comparand);
			}
			
			// Convert the comparand so that it matches the value
			object convertedComparand = bind.Comparand;

			try {
				// Only support simple conversions in here. 
				convertedComparand = System.Convert.ChangeType(
					bind.Comparand, value.GetType());
			}
			catch (InvalidCastException) {
				// If Convert.ChangeType didn't work, try a type converter
				TypeConverter typeConverter = TypeDescriptor.GetConverter(value);
				if (typeConverter != null) {
					if (typeConverter.CanConvertFrom(
						bind.Comparand.GetType()))
					{
						convertedComparand =
							typeConverter.ConvertFrom(bind.Comparand);
					}
				}
			}

			// Simple check for the equality case

			// Actually, equality is a little more interesting, so put it in
			// a helper routine
			if (bind.Operator == ComparisonOperators.EQ) {
				return ReturnHelper(
							CheckEquals(value.GetType(), value, convertedComparand));
			}
			else if (bind.Operator == ComparisonOperators.NEQ) {
				return ReturnHelper(
							!CheckEquals(value.GetType(), value, convertedComparand));
			}
			
			// For anything other than Equals, we need IComparable
			if (!(value is IComparable) || !(convertedComparand is IComparable)) {
				Trace(value, "One of the values was not an IComparable");
				return ReturnHelper(false);
			}
			
			// Compare the values
			int comparison = (value as IComparable).CompareTo(convertedComparand);
			
			// And return the comparisson result
			switch (bind.Operator) {
			case ComparisonOperators.GT:
				return ReturnHelper(comparison > 0);
			case ComparisonOperators.GTE:
				return ReturnHelper(comparison >= 0);
			case ComparisonOperators.LT:
				return ReturnHelper(comparison < 0);
			case ComparisonOperators.LTE:
				return ReturnHelper(comparison <= 0);
			}
			
			return notNull;
		}
		
		/// <summary>IValueConverter.ConvertBack isn't supported.</summary>
		public object ConvertBack(object value, Type targetType, object parameter,
			CultureInfo culture)
		{
			throw new NotSupportedException();
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>This helper produces the return value; null if the values match,
		/// non-null otherwise.</summary>
		private object ReturnHelper(bool result) {
			//return result;
			return result ? null : notNull;
		}
		
		/// <summary>Trace output to the debugger.</summary>
		private void Trace(object value, string message) {
			if (Debugger.IsAttached) {
				Debug.WriteLine("StyleBinding couldn't convert '"
								 + value.GetType()
								 + "' to '"
								 + bind.Comparand.GetType()
								 + "'");
				Debug.WriteLine("(" + message + ")");
			}
		}
		
		/// <summary>Check for equality of two values.</summary>
		private bool CheckEquals(Type type, object value1, object value2) {

			if (type.IsValueType || type == typeof(string))
				return object.Equals(value1, value2);
			else
				return object.ReferenceEquals(value1, value2);
		}
	}
}
