using Microsoft.Xna.Framework.Audio;

namespace ZeldaOracle.Common.Audio {
	/// <summary>An interface for sound and music objects.</summary>
	public interface ISound {

		//-----------------------------------------------------------------------------
		// Playback
		//-----------------------------------------------------------------------------

		/// <summary>Plays the sound.</summary>
		SoundInstance Play(bool looped = false);
		
		/// <summary>Plays the sound.</summary>
		SoundInstance Play(bool looped, float volume, float pitch = 0.0f, float pan = 0.0f, bool muted = false);
		
		/// <summary>Stops the sound.</summary>
		void Stop();

		/// <summary>Resumes the sound.</summary>
		void Resume();

		/// <summary>Pauses the sound.</summary>
		void Pause();


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Containment ----------------------------------------------------------------

		/// <summary>Gets the sound effect contained by this sound.</summary>
		SoundEffect SoundEffect { get; }

		/// <summary>Gets the name of the sound.</summary>
		string Name { get; }

		/// <summary>Gets the file path of the sound.</summary>
		string FilePath { get; }

		// Settings -------------------------------------------------------------------

		/// <summary>Gets or sets the default volume of the sound between 0 and 1.</summary>
		float Volume { get; set; }

		/// <summary>Gets or sets the default pitch of the sound between -1 and 1.</summary>
		float Pitch { get; set; }

		/// <summary>Gets or sets the default pan of the sound between -1 and 1.</summary>
		float Pan { get; set; }

		/// <summary>Gets or sets if the sound is muted.</summary>
		bool IsMuted { get; set; }

		// Playback -------------------------------------------------------------------

		/// <summary>Returns true if the sound is currently playing.</summary>
		bool IsPlaying { get; }

		/// <summary>Returns true if the sound is currently paused.</summary>
		bool IsPaused { get; }

		/// <summary>Returns true if the sound is currently stopped.</summary>
		bool IsStopped { get; }
	}
}
