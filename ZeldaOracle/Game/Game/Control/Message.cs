using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Control {
	public class Message {

		// The text for the message.
		private string text;
		private string[] options;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public Message(string text, params string[] options) {
			this.text = text;
			this.options = options;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Gets the text for the message.
		public string Text {
			get { return text; }
		}
		// Gets the options for the message.
		public string[] Options {
			get { return options; }
		}
		// Gets the number of options for the message.
		public int NumOptions {
			get { return options.Length; }
		}
	}
}
