using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Content {
	public class Resource {
		private string resourceName;

		public Resource() {
			resourceName = "";
		}

		public string ResourceName {
			get { return resourceName; }
			set { resourceName = value; }
		}
	}
}
