using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Scripting;

namespace ZeldaOracle.Game.ResourceData {
	/// <summary>The base class for data objects stored in resources.</summary>
	public abstract class BaseResourceData : IPropertyObject {

		/// <summary>The name of the resource data.</summary>
		private string name;
		/// <summary>The type of the resource data.</summary>
		private Type type;
		/// <summary>The properties of the resource data.</summary>
		private Properties properties;
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the base resource data.</summary>
		public BaseResourceData() {
			name		= "";
			properties	= new Properties(this);
		}

		/// <summary>Constructs a copy of the base resource data.</summary>
		public BaseResourceData(BaseResourceData copy) {
			name		= "";
			type		= copy.type;
			properties	= new Properties(copy.properties, this);
		}

		/// <summary>Clones the specified base resource data.</summary>
		public virtual void Clone(BaseResourceData copy) {
			type		= copy.type;
			properties	= new Properties(copy.properties, this);
		}


		//-----------------------------------------------------------------------------
		// Abstract Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the base output type for this resource data.</summary>
		public abstract Type OutputType { get; }


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the name of the base resource data.</summary>
		public string Name {
			get { return name; }
			set { name = value; }
		}

		/// <summary>Gets or sets the overridden type for the base resource item.</summary>
		public Type Type {
			get { return type; }
			set {
				if ((type == null && value != null) || !type.Equals(value)) {
					// Make sure we extend the correct type and don't go backwards.
					if (type != null) {
						if (value == null || !type.IsAssignableFrom(value))
							throw new ArgumentException("New type does not inherit " +
								"from previous type!");
					}
					Type previousType = type;
					type = value;
					// Initialize the item's new types.
					ResourceDataInitializing.InitializeData(
						OutputType, this, type, previousType);
					//ItemDataInitializing.InitializeItem(this, previousType);
				}
			}
		}

		/// <summary>Gets or sets the properties of the base base resource data.</summary>
		public Properties Properties {
			get { return properties; }
			set { properties = value; }
		}
	}
}
