using System;
using System.Collections.Generic;
using System.Reflection;

namespace ZeldaOracle.Game.ResourceData {
	/// <summary>Information about a static delegate dictionary.</summary>
	public class DelegateLookupInfo {
		/// <summary>Gets the collection of delegates for each type.</summary>
		public Dictionary<Type, Delegate> Delegates { get; private set; }
		/// <summary>Gets the full name of the function to search for.</summary>
		public string FunctionName { get; private set; }
		/// <summary>Gets the type of delegate used for delegate creation.</summary>
		public Type DelegateType { get; private set; }
		/// <summary>Gets the types of the parameters for the delegate.</summary>
		public Type[] Parameters { get; private set; }


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the delegate lookup information.</summary>
		public DelegateLookupInfo(string functionName, Type delegateType) {
			Delegates = new Dictionary<Type, Delegate>();
			FunctionName = functionName;
			DelegateType = delegateType;
			MethodInfo method = delegateType.GetMethod("Invoke");
			ParameterInfo[] parameters = method.GetParameters();
			Parameters = new Type[parameters.Length];
			for (int i = 0; i < Parameters.Length; i++) {
				Parameters[i] = parameters[i].ParameterType;
			}
		}
	}
}
