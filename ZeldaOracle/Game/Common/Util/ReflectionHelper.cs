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

		/// <summary>Searches for the specified method whose parameters match the
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

		/// <summary>Searches for the specified constructor whose parameters match the
		/// specified argument types and modifiers, using the specified binding
		/// constraints.</summary>
		/// <param name="type">The type to get the method from.</param>
		/// <param name="bindingAttr">A bitmask comprised of one or more
		/// System.Reflection.BindingFlags that specify how the search is conducted.
		/// -or- Zero, to return null.</param>
		/// <param name="types">An array of System.Type objects representing the
		/// number, order, and type of the parameters for the constructor to get.
		/// -or- An empty array of the type System.Type (that is, Type[] types =
		/// new Type[0]) to get a constructor that takes no parameters.-or-
		/// System.Type.EmptyTypes.</param>
		/// <returns>A System.Reflection.ConstructorInfo object representing the
		/// constructor that matches the specified requirements, if found;
		/// otherwise, null.</returns>
		/// <exception cref="System.Reflection.AmbiguousMatchException">
		/// More than one method is found with the specified name and matching the
		/// specified binding constraints.</exception>
		/// <exception cref="System.ArgumentNullException">
		/// types is null.-or- One of the elements in types is null.</exception>
		/// <exception cref="System.ArgumentException">
		/// types is multidimensional.-or- modifiers is multidimensional.-or- types
		/// and modifiers do not have the same length.</exception>
		public static ConstructorInfo GetConstructor(this Type type,
			BindingFlags bindingAttr, params Type[] types)
		{
			return type.GetConstructor(bindingAttr, null, types, null);
		}

		/// <summary>Constructs an object from the type's empty constructor.</summary>
		public static object Construct(Type type) {
			ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
			if (constructor == null)
				return null;
			return constructor.Invoke(null);
		}

		/// <summary>Constructs an object from the type's empty constructor.</summary>
		public static T Construct<T>(Type type) where T : class {
			return Construct(type) as T;
		}

		/// <summary>Constructs an object from the type's constructor that uses the
		/// passed parameters.</summary>
		/*public static object Construct(Type type, params object[] args) {
			Type[] types = new Type[args.Length];
			for (int i = 0; i < args.Length; i++)
				types[i] = args[i].GetType();
			ConstructorInfo constructor = type.GetConstructor(types);
			if (constructor == null)
				return null;
			return constructor.Invoke(args);
		}*/

		/// <summary>Constructs an object from the type's constructor that uses the
		/// passed parameters.</summary>
		/*public static T Construct<T>(Type type, params object[] args)
			where T : class
		{
			return Construct(type, args) as T;
		}*/

		/// <summary>Constructs an object from the type's constructor that uses the
		/// passed parameters.</summary>
		public static object Construct<P1>(Type type, P1 param1) {
			Type[] types = new Type[1];
			types[0] = typeof(P1);
			object[] args = new object[1];
			args[0] = param1;
			ConstructorInfo constructor = type.GetConstructor(types);
			if (constructor == null)
				return null;
			return constructor.Invoke(args);
		}

		/// <summary>Constructs an object from the type's constructor that uses the
		/// passed parameters.</summary>
		public static T Construct<T, P1>(Type type, P1 param1)
			where T : class
		{
			return Construct(type, param1) as T;
		}

		/// <summary>Constructs an object from the type's constructor that uses the
		/// passed parameters.</summary>
		public static object Construct<P1, P2>(Type type, P1 param1, P2 param2) {
			Type[] types = new Type[1];
			types[0] = typeof(P1);
			types[1] = typeof(P2);
			object[] args = new object[1];
			args[0] = param1;
			args[1] = param2;
			ConstructorInfo constructor = type.GetConstructor(types);
			if (constructor == null)
				return null;
			return constructor.Invoke(args);
		}

		/// <summary>Constructs an object from the type's constructor that uses the
		/// passed parameters.</summary>
		public static T Construct<T, P1, P2>(Type type, P1 param1, P2 param2)
			where T : class
		{
			return Construct(type, param1, param2) as T;
		}

		/// <summary>Constructs an object from the type's constructor that uses the
		/// passed parameters.</summary>
		public static object Construct<P1, P2, P3>(Type type, P1 param1, P2 param2,
			P3 param3)
		{
			Type[] types = new Type[1];
			types[0] = typeof(P1);
			types[1] = typeof(P2);
			types[2] = typeof(P3);
			object[] args = new object[1];
			args[0] = param1;
			args[1] = param2;
			args[2] = param3;
			ConstructorInfo constructor = type.GetConstructor(types);
			if (constructor == null)
				return null;
			return constructor.Invoke(args);
		}

		/// <summary>Constructs an object from the type's constructor that uses the
		/// passed parameters.</summary>
		public static T Construct<T, P1, P2, P3>(Type type, P1 param1, P2 param2,
			P3 param3) where T : class
		{
			return Construct(type, param1, param2, param3) as T;
		}
	}
}
