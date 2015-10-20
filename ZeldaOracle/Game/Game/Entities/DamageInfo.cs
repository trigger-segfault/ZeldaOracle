using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Entities {
	
	public class DamageInfo {
		private int			amount;
		private Vector2F	sourcePosition;
		private bool		hasSource;
		private bool		applyKnockBack;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public DamageInfo(int amount) {
			this.amount			= amount;
			this.hasSource		= false;
			this.applyKnockBack	= true;
		}

		public DamageInfo(int amount, Vector2F sourcePosition) {
			this.amount			= amount;
			this.hasSource		= true;
			this.sourcePosition	= sourcePosition;
			this.applyKnockBack	= true;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int Amount {
			get { return amount; }
			set { amount = value; }
		}

		public Vector2F SourcePosition {
			get { return sourcePosition; }
			set { sourcePosition = value; }
		}

		public bool HasSource {
			get { return hasSource; }
			set { hasSource = value; }
		}

		public bool ApplyKnockBack {
			get { return applyKnockBack; }
			set { applyKnockBack = value; }
		}
	}

}
