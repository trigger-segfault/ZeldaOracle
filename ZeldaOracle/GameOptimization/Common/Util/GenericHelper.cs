using System;
using System.Collections.Generic;

namespace ZeldaOracle.Common.Util {
	/// <summary>A static helper class for generic functions.</summary>
	public static class GenericHelper {

		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The collection of signed integer types.</summary>
		private static HashSet<Type> signedTypes;
		/// <summary>The collection of unsined integer types.</summary>
		private static HashSet<Type> unsignedTypes;
		/// <summary>The collection of floating-point types.</summary>
		private static HashSet<Type> floatingTypes;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Initializes and registers each type association.</summary>
		static GenericHelper() {
			signedTypes = new HashSet<Type>();
			AddSigned<sbyte>();
			AddSigned<short>();
			AddSigned<int>();
			AddSigned<long>();

			unsignedTypes = new HashSet<Type>();
			AddUnsigned<byte>();
			AddUnsigned<ushort>();
			AddUnsigned<uint>();
			AddUnsigned<ulong>();
			AddUnsigned<char>();

			floatingTypes = new HashSet<Type>();
			AddFloating<float>();
			AddFloating<double>();
			AddFloating<decimal>();
		}

		/// <summary>Registers a signed integer type.</summary>
		public static void AddSigned<T>() {
			AddSigned(typeof(T));
		}

		/// <summary>Registers a signed integer type.</summary>
		public static void AddSigned(Type type) {
			signedTypes.Add(type);
		}

		/// <summary>Registers an unsigned integer type.</summary>
		public static void AddUnsigned<T>() {
			AddUnsigned(typeof(T));
		}

		/// <summary>Registers an unsigned integer type.</summary>
		public static void AddUnsigned(Type type) {
			unsignedTypes.Add(type);
		}

		/// <summary>Registers a floating-point type.</summary>
		public static void AddFloating<T>() {
			AddFloating(typeof(T));
		}

		/// <summary>Registers a floating-point type.</summary>
		public static void AddFloating(Type type) {
			floatingTypes.Add(type);
		}

		/// <summary>Gets if the type is a numeric type.</summary>
		public static bool IsNumeric<T>() {
			return IsNumeric(typeof(T));
		}

		/// <summary>Gets if the type is a numeric type.</summary>
		public static bool IsNumeric(this Type type) {
			return (signedTypes.Contains(type) ||
					unsignedTypes.Contains(type) ||
					floatingTypes.Contains(type));
		}

		/// <summary>Gets if the type is an integer type.</summary>
		public static bool IsInteger<T>() {
			return IsInteger(typeof(T));
		}

		/// <summary>Gets if the type is an integer type.</summary>
		public static bool IsInteger(this Type type) {
			return (signedTypes.Contains(type) || unsignedTypes.Contains(type));
		}

		/// <summary>Gets if the type is a signed integer type.</summary>
		public static bool IsSigned<T>() {
			return IsSigned(typeof(T));
		}

		/// <summary>Gets if the type is a signed integer type.</summary>
		public static bool IsSigned(this Type type) {
			return signedTypes.Contains(type);
		}

		/// <summary>Gets if the type is an unsigned integer type.</summary>
		public static bool IsUnsigned<T>() {
			return IsUnsigned(typeof(T));
		}

		/// <summary>Gets if the type is an unsigned integer type.</summary>
		public static bool IsUnsigned(this Type type) {
			return unsignedTypes.Contains(type);
		}

		/// <summary>Gets if the type is a floating type.</summary>
		public static bool IsFloating<T>() {
			return IsFloating(typeof(T));
		}

		/// <summary>Gets if the type is a floating type.</summary>
		public static bool IsFloating(this Type type) {
			return floatingTypes.Contains(type);
		}
	}
}
