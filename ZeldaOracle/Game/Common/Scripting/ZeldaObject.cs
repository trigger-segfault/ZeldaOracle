using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Scripting {
	

	public class ZeldaObject {
		private Properties properties;
		private ObjectEventCollection events;


		public ZeldaObject() {
			properties = new Properties();
			events = new ObjectEventCollection();
		}


		public Properties Properties {
			get { return properties; }
			set { properties = value; }
		}
		
		public ObjectEventCollection Events {
			get { return events; }
			set { events = value; }
		}
	}
}
