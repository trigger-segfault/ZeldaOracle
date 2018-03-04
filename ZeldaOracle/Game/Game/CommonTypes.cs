using System;

namespace ZeldaOracle.Game {
	
	/// <summary>Magnetic polarity (north or south).</summary>
	public enum Polarity {
		None = -1,
		North = 0,
		South = 1,
	}
	
	/// <summary>The types of seeds.</summary>
	public enum SeedType {
		Ember	= 0,
		Scent	= 1,
		Pegasus	= 2,
		Gale	= 3,
		Mystery	= 4,
	}

	/// <summary>Buttons used to perform player actions (A or B).</summary>
	public enum ActionButtons {
		A = 0,
		B = 1,
		Count = 2,
	}
	
	/// <summary>The different colored tunics the player can wear.</summary>
	public enum PlayerTunics {
		GreenTunic	= 0,
		RedTunic	= 1,
		BlueTunic	= 2
	}
	
	/// <summary>The types of liquids that the player can swim in.</summary>
	[Flags]
	public enum PlayerSwimmingSkills {
		CannotSwim		= 0x0,
		CanSwimInWater	= 0x1,
		CanSwimInOcean	= 0x2,
		CanSwimInLava	= 0x4,
	}
	
	public interface IInterceptable {
		/// <summary>Intercept the object. Returns true if the object was successfully
		/// intercpted.</summary>
		bool Intercept();
	}
}
