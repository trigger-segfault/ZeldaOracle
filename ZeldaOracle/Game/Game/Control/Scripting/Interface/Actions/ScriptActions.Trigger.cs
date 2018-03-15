using System;
using ZeldaOracle.Common.Scripting;

namespace ZeldaOracle.Game.Control.Scripting.Interface.Actions {

	public class ScriptActionsTrigger :
		ScriptInterfaceSection, ZeldaAPI.ScriptActionsTrigger
	{
		public void Run(ZeldaAPI.Trigger trigger) {
			throw new NotImplementedException();
		}

		public void TurnOff(ZeldaAPI.Trigger trigger) {
			((Trigger) trigger).IsEnabled = false;
		}

		public void TurnOn(ZeldaAPI.Trigger trigger) {
			((Trigger) trigger).IsEnabled = true;
		}
	}
}
