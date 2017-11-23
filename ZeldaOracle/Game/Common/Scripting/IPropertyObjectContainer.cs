using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Common.Scripting {
	public interface IPropertyObjectContainer {

		IEnumerable<IPropertyObject> GetPropertyObjects();
	}
}
