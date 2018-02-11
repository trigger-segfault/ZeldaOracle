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
		private bool		applyKnockback;
		private int			knockbackDuration;
		private bool		flicker;
		private int			flickerDuration;
		private int			invincibleDuration;
		private bool        playSound;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public DamageInfo(int amount) {
			this.amount				= amount;
			this.hasSource			= false;
			this.applyKnockback		= true;
			this.knockbackDuration	= -1;
			this.flicker			= true;
			this.flickerDuration	= GameSettings.MONSTER_HURT_FLICKER_DURATION;
			this.invincibleDuration	= -1;
			this.playSound			= true;
		}

		public DamageInfo(int amount, Vector2F sourcePosition) {
			this.amount				= amount;
			this.hasSource			= true;
			this.sourcePosition		= sourcePosition;
			this.applyKnockback		= true;
			this.knockbackDuration	= -1;
			this.flicker			= true;
			this.flickerDuration	= GameSettings.MONSTER_HURT_FLICKER_DURATION;
			this.invincibleDuration	= -1;
			this.playSound			= true;
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

		public bool ApplyKnockback {
			get { return applyKnockback; }
			set { applyKnockback = value; }
		}

		public int KnockbackDuration {
			get { return knockbackDuration; }
			set { knockbackDuration = value; }
		}

		public int InvincibleDuration {
			get { return invincibleDuration; }
			set { invincibleDuration = value; }
		}

		public int FlickerDuration {
			get { return flickerDuration; }
			set { flickerDuration = value; }
		}

		public bool PlaySound {
			get { return playSound; }
			set { playSound = value; }
		}
	}

}
