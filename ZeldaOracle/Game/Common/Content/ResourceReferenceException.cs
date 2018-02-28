using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Common.Content {
	public class ResourceReferenceException : Exception {

		public ResourceReferenceException(Type type, string name)
			: base("Could not find resource of type '" + type.Name +
				  "' with the name '" + name + "'!") { }
	}
}
