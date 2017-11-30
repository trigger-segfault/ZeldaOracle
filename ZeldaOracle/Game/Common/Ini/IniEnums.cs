using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Common.Ini {
	/// <summary>The available property assignment styles for ini documents.</summary>
	public enum IniAssignmentStyle {
		/// <summary>Uses "=" for ini property assignment.</summary>
		Equals,
		/// <summary>Uses ":" for ini property assignment.</summary>
		Colon,
		/// <summary>Uses " = " for ini property assignment.</summary>
		EqualsSpaced,
		/// <summary>Uses " : " for ini property assignment.</summary>
		ColonSpaced
	}

	/// <summary>The available comment styles for ini documents.</summary>
	public enum IniCommentStyle {
		/// <summary>Uses ";" as the ini comment indicator.</summary>
		Semicolon,
		/// <summary>Uses "#" as the ini comment indicator.</summary>
		Pound,
		/// <summary>Uses "!" as the ini comment indicator.</summary>
		Exclamation,
		/// <summary>Uses "; " as the ini comment indicator.</summary>
		SemicolonSpaced,
		/// <summary>Uses "# " as the ini comment indicator.</summary>
		PoundSpaced,
		/// <summary>Uses "! " as the ini comment indicator.</summary>
		ExclamationSpaced
	}

	/// <summary>The formats an enum can be stored in.</summary>
	public enum EnumFormat {
		/// <summary>The enum is stored in string form.</summary>
		String,
		/// <summary>The enum is stored in integer form.</summary>
		Int,
	}

	/// <summary>Extensions for the ini enums.</summary>
	public static class IniEnumExtensions {
		/// <summary>Gets the string representation of the ini assignment style.</summary>
		public static string GetString(this IniAssignmentStyle assignmentStyle) {
			switch (assignmentStyle) {
			case IniAssignmentStyle.Equals:			return "=";
			case IniAssignmentStyle.Colon:			return ":";
			case IniAssignmentStyle.EqualsSpaced:	return " = ";
			case IniAssignmentStyle.ColonSpaced:	return " : ";
			}
			return "";
		}

		/// <summary>Gets the string representation of the ini comment style.</summary>
		public static string GetString(this IniCommentStyle commentStyle) {
			switch (commentStyle) {
			case IniCommentStyle.Semicolon:			return ";";
			case IniCommentStyle.Pound:				return "#";
			case IniCommentStyle.Exclamation:		return "!";
			case IniCommentStyle.SemicolonSpaced:	return "; ";
			case IniCommentStyle.PoundSpaced:		return "# ";
			case IniCommentStyle.ExclamationSpaced:	return "! ";
			}
			return "";
		}
	}
}
