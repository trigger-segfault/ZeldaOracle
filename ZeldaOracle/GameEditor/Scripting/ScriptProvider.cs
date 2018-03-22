using ICSharpCode.CodeCompletion;
using ZeldaOracle.Game.Control.Scripting;

namespace ZeldaEditor.Scripting {
	/// <summary>
	/// This is a simple script provider that adds a few using statements to the C#
	/// scripts (.csx files)
	/// </summary>
	class ScriptProvider : ICSharpScriptProvider {

		public string GetUsing() {
			return ScriptCodeGenerator.CreateUsingsString();
		}
		
		public string GetVars() => null;

		public string GetNamespace() => null;
	}
}