using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Common.Util {
	/// <summary>A static helper class for functions involving reflection.</summary>
	public static class ReflectionHelper {

		/// <summary>Creates a callable Func/Action object from a MethodInfo.</summary>
		public static TDelegate GetFunction<TDelegate>(MethodInfo methodInfo) {
			ParameterInfo[] parameterInfos = methodInfo.GetParameters();
			ParameterExpression[] parameters = new ParameterExpression[parameterInfos.Length];
			for (int i = 0; i < parameterInfos.Length; i++)
				parameters[i] = Expression.Parameter(parameterInfos[i].ParameterType, "param" + i);
			if (methodInfo.IsStatic) {
				return Expression.Lambda<TDelegate>(
					Expression.Call(methodInfo, parameters), parameters).Compile();
			}
			else {
				ParameterExpression[] parametersFull = new ParameterExpression[parameterInfos.Length + 1];
				ParameterExpression instance = Expression.Parameter(methodInfo.ReflectedType, "instance");
				parametersFull[0] = instance;
				for (int i = 0; i < parameterInfos.Length; i++)
					parametersFull[i + 1] = parameters[i];
				return Expression.Lambda<TDelegate>(
					Expression.Call(instance, methodInfo, parameters), parametersFull).Compile();
			}
		}

		/// <summary> Searches for the specified method whose parameters match the
		/// specified argument types and modifiers, using the specified binding
		/// constraints.</summary>
		/// <param name="type">The type to get the method from.</param>
		/// <param name="name">The string containing the name of the method to get.</param>
		/// <param name="bindingAttr">A bitmask comprised of one or more
		/// System.Reflection.BindingFlags that specify how the search is conducted.
		/// -or- Zero, to return null.</param>
		/// <param name="types">An array of System.Type objects representing the number,
		/// order, and type of the parameters for the method to get.-or- An empty array
		/// of System.Type objects (as provided by the System.Type.EmptyTypes field)
		/// to get a method that takes no parameters.</param>
		/// <returns>An object representing the method that matches the specified
		/// requirements, if found; otherwise, null.</returns>
		/// <exception cref="System.Reflection.AmbiguousMatchException">
		/// More than one method is found with the specified name and matching the
		/// specified binding constraints.</exception>
		/// <exception cref="System.ArgumentNullException">
		/// name is null.-or- types is null.-or- One of the elements in types is null.</exception>
		/// <exception cref="System.ArgumentException">
		/// types is multidimensional.-or- modifiers is multidimensional.</exception>
		public static MethodInfo GetMethod(this Type type, string name,
			BindingFlags bindingAttr, params Type[] types)
		{
			return type.GetMethod(name, bindingAttr, null, types, null);
		}
		
		/// <summary>Constructs an object from the type's empty constructor.</summary>
		public static T Construct<T>(Type type) where T : class {
			ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
			if (constructor == null)
				return null;
			return (constructor.Invoke(null) as T);
		}

		/// <summary>Constructs an object from the type's empty constructor.</summary>
		public static object Construct(Type type) {
			ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
			if (constructor == null)
				return null;
			return constructor.Invoke(null);
		}
	}
}
