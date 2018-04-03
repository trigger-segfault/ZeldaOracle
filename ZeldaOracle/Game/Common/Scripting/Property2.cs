using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Common.Scripting {
	/// <summary>A proprety represents a piece of data that can be represented
	/// multiple types including lists of other properties.</summary>
	[Serializable]
	public class Property2 : VarBase {

		/// <summary>The documentation for this property, if it is a base-property.</summary>
		[NonSerialized]
		private PropertyDocumentation documentation;
		/// <summary>The properties containing this property.</summary>
		[NonSerialized]
		private Properties2 properties;
		/// <summary>The base property that this property modifies.</summary>
		[NonSerialized]
		private Property2 baseProperty;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a property of the specified type.</summary>
		public Property2(string name, VarType varType, ListType listType,
			int length = 0) : base(name, varType, listType, length)
		{
		}

		/// <summary>Constructs a property of the specified type.</summary>
		public Property2(string name, Type type, int length = 0)
			: base(name, type, length)
		{
		}

		/// <summary>Constructs a property with the specified value.</summary>
		public Property2(string name, object value) : base(name, value) {
		}

		/// <summary>Constructs a copy of the property.</summary>
		public Property2(Property2 copy) : base(copy) {
			properties		= copy.properties;
			baseProperty	= copy.baseProperty;
			documentation	= copy.documentation;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>The properties container this property is contained in.</summary>
		public Properties2 Properties {
			get { return properties; }
			set { properties = value; }
		}

		/// <summary>The base property for this property.</summary>
		public Property2 BaseProperty {
			get { return baseProperty; }
			set { baseProperty = value; }
		}

		/// <summary>Gets if the property has a base.</summary>
		public bool HasBase {
			get { return baseProperty != null; }
		}
	}
}
