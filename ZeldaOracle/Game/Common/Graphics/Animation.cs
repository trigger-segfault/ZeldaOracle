using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Graphics {
	
	public class Animation {

		private List<AnimationFrame> frames;
		private int duration;
		private int loops;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Animation() {
			frames		= new List<AnimationFrame>();
			duration	= 0;
			loops		= 0;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int Duration {
			get { return duration; }
			set { duration = value; }
		}
		
		public List<AnimationFrame> Frames {
			get { return frames; }
			set { frames = value; }
		}
	}
}
