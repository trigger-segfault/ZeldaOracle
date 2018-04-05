using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Content;

namespace ZeldaOracle.Common.Scripting {
	/// <summary>Extensions for properties that cannot be defined in the optimizations
	/// class.</summary>
	public static class PropertiesExtensions {

		/// <summary>Get a resource value with a default value fallback.</summary>
		public static R GetResource<R>(this Properties properties, string name)
			where R : class
		{
			return Resources.Get<R>(properties.Get<string>(name));
		}
		
		/// <summary>Get a resource property value with a default value fallback.</summary>
		public static R GetResource<R>(this Properties properties, string name,
			R defaultValue) where R : class
		{
			R result = null;
			string resourceName = properties.Get<string>(name, null);
			if (resourceName != null)
				result = Resources.Get<R>(resourceName);
			return result ?? defaultValue;
		}
		
		/// <summary>Sets the property's value as an resource name.</summary>
		public static Property SetResource<T>(this Properties properties, string name,
			T resource) where T : class
		{
			string resourceName = "";
			if (resource != null)
				resourceName = Resources.GetName<T>(resource);
			return properties.SetProperty(name, resourceName);
		}
	}
}
