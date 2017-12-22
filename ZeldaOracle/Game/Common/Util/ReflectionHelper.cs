using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Common.Util {
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

	}
}
