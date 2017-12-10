using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Common.Scripts.Commands {
	public class CommandPrefix {
		public string Name { get; }
		public int[] Modes { get; }

		public CommandPrefix(string name, int[] modes) {
			this.Name   = name;
			this.Modes  = modes;
		}
	}
}
