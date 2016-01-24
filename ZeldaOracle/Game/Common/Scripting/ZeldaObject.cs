using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Scripting {
	
	// NOTE: This is UNUSED at the moment.
	// It's supposed to serve as a base class for property objects.
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
