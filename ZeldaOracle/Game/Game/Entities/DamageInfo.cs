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
		private int			knockbackDuration;
		private bool		flicker;
		private int			flickerDuration;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public DamageInfo(int amount) {
			this.amount			= amount;
			this.hasSource		= false;
			this.applyKnockBack	= true;
			this.knockbackDuration	= GameSettings.UNIT_KNOCKBACK_DURATION;
			this.flicker			= true;
			this.flickerDuration	= GameSettings.MONSTER_HURT_FLICKER_DURATION;
		}

		public DamageInfo(int amount, Vector2F sourcePosition) {
			this.amount			= amount;
			this.hasSource		= true;
			this.sourcePosition	= sourcePosition;
			this.applyKnockBack	= true;
			this.knockbackDuration	= GameSettings.UNIT_KNOCKBACK_DURATION;
			this.flicker			= true;
			this.flickerDuration	= GameSettings.MONSTER_HURT_FLICKER_DURATION;
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

		public bool Flicker {
			get { return flicker; }
			set { flicker = value; }
		}

		public bool ApplyKnockBack {
			get { return applyKnockBack; }
			set { applyKnockBack = value; }
		}

		public int KnockbackDuration {
			get { return knockbackDuration; }
			set { knockbackDuration = value; }
		}

		public int FlickerDuration {
			get { return flickerDuration; }
			set { flickerDuration = value; }
		}
	}

}
