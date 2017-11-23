using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.CodeCompletion;

namespace ZeldaEditor.Scripting {
	/// <summary>
	/// This is a simple script provider that adds a few using statements to the C# scripts (.csx files)
	/// </summary>
	class ScriptProvider : ICSharpScriptProvider {
		public string GetUsing() {
			return "" +
				"using System; " +
				"using System.Collections.Generic; " +
				"using System.Linq; " +
				"using System.Text; " +
				"using ZeldaAPI; ";
		}


		public string GetVars() => null;

		public string GetNamespace() => null;
	}
}