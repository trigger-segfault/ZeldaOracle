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
		protected Type type;
		/// <summary>The properties of the resource data.</summary>
		protected Properties properties;
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the base resource data.</summary>
		public BaseResourceData() {
			name		= "";
			type		= OutputType;
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
		// Abstract Methods
		//-----------------------------------------------------------------------------

		/// <summary>Initializes data after a change in the final type.<para/>
		/// This needs to be extended for each non-abstract class in order
		/// to make use of compile-time generic arguments within
		/// ResourceDataInitializing.InitializeData.</summary>
		public abstract void InitializeData(Type previousType);


		//-----------------------------------------------------------------------------
		// Abstract Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the base output type for this resource data.</summary>
		public abstract Type OutputType { get; }


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the resource name of the base resource data.</summary>
		public string ResourceName {
			get { return name; }
			set { name = value; }
		}

		/// <summary>Gets or sets the overridden type for the base resource item.</summary>
		public Type Type {
			get { return type; }
			set {
				if (type != value) {
					// Make sure we extend the correct type and don't go backwards.
					if (type != null) {
						if (value == null || !type.IsAssignableFrom(value))
							throw new ArgumentException("New type does not inherit " +
								"from previous type!");
					}
					Type previousType = type;
					type = value;
					// Initialize the resource's new types
					InitializeData(previousType);
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
