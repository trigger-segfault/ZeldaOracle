using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Xceed.Wpf.Toolkit.PropertyGrid {
	public class DummyInstance : DependencyObject {
		private PropertyDescriptor prop;
		private object instance;

		public DummyInstance(PropertyDescriptor prop, object instance) {
			this.prop = prop;
			this.instance = instance;
			Value = prop.GetValue(instance);
		}

		public object Value {
			get { return prop.GetValue(instance); }
			set { prop.SetValue(instance, value); }
		}
	}
}
