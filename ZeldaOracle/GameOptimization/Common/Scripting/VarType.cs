using System;
using System.Collections.Generic;
using ZeldaOracle.Common.Conscripts.Commands;
using ZeldaOracle.Common.Scripting.Internal;

namespace ZeldaOracle.Common.Scripting {
	/// <summary>The possible raw data types for some sort of variable.</summary>
	[Serializable]
	public enum VarType : short {
		/// <summary>Currently unsupported.</summary>
		Custom = 0,
		String,
		Integer,
		Float,
		Boolean,
		Point,
		Vector,
		RangeI,
		RangeF,
		RectangleI,
		RectangleF,
		Color,
	}

	public class VarTypeInfo {
		public Type InternalType { get; set; }
		public Type Type { get; set; }
		public VarType VarType { get; set; }
		public bool IsClass { get; set; } = false;
		public bool IsReference { get; set; } = false;
		public object DefaultValue { get; set; } = null;

		public VarTypeInfo() {
			Type.IsValueType;
		}
		
		public virtual string ToString(object value) {
			return value.ToString();
		}

		public virtual void Serialize(BinaryWriter writer, object value) {
			throw new NotImplementedException();
		}

		public virtual object Deserialize(BinaryReader reader) {
			throw new NotImplementedException();
		}

		public virtual object ParseConscript(CommandParam parameter) {
			throw new NotImplementedException();
		}
	}
	

	public class RawTypeInfo : VarTypeInfo {

		public RawTypeInfo() {
			IsClass = false;
			IsReference = false;
		}
	}

	/*
	Last uncommitted changes from David Jordan:
	
	public static class VarTypeInfos {
		private static List<VarTypeInfo> types;

		static VarTypeInfos() {

			types.Add(new VarTypeInfo() {
			});

			AddRawType<bool>(VarType.Boolean);
			AddRawType<int>(VarType.Integer);
			AddRawType<float>(VarType.Float);
			AddRawType<Point2I>(VarType.Point);
			AddRawType<Vector2F>(VarType.Vector);
			AddRawType<RangeF>(VarType.RangeF);
			AddRawType<RangeI>(VarType.RangeI);
			AddRawType<Rectangle2F>(VarType.RectangleF);
			AddRawType<Rectangle2I>(VarType.RectangleI);
			AddRawType<Color>(VarType.Color);
			//AddReferenceType<string>();
		}

		private static VarTypeInfo AddReferenceType<T>(VarType type) {
			VarTypeInfo info = new VarTypeInfo() {
				VarType = type,
				Type = typeof(T),
				InternalType = typeof(T),
				DefaultValue = null,
				IsClass = true,
				IsReference = true,
			};
			types.Add(info);
			return info;
		}

		private static VarTypeInfo AddRawType<T>(VarType type) {
			VarTypeInfo info = new VarTypeInfo() {
				VarType = type,
				Type = typeof(T),
				InternalType = typeof(T),
				DefaultValue = default(T),
				IsClass = false,
				IsReference = false,
			};
			types.Add(info);
			return info;
		}
	}
	*/


	/// <summary>The list types for use with var types.</summary>
	[Serializable]
	public enum ListType : short {
		Single = 0,
		Array,
		List,
	}

	/// <summary>Extensions for the var type and list type enums.</summary>
	public static class VarExtensions {

		/// <summary>Convert a VarType to a System.Type.</summary>
		public static Type ToType(this VarType varType) {
			return VarBase.VarTypeToType(varType);
		}

		/// <summary>Convert a VarType to a System.Type.</summary>
		public static Type ToType(this VarType varType, ListType listType) {
			return VarBase.VarTypeToType(varType, listType);
		}

		/// <summary>Gets the default value of a VarType.</summary>
		public static object GetDefaultValue(this VarType varType) {
			Type type = varType.ToType();
			if (type.IsValueType)
				return Activator.CreateInstance(type);
			return null;
		}
	}
}
