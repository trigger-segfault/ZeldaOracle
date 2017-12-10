using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Ini;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game {
	/// <summary>The user settings for Zelda Oracle Engine gameplay.</summary>
	public class UserSettings : IniReflectionSettings {

		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------
		
		/// <summary>Called after loading finishes only if the load was unsuccessful.</summary>
		protected override void PostLoadFailed(Exception ex) {
			// TODO: Log error here
		}

		/// <summary>Called after saving finishes only if the save was unsuccessful.</summary>
		protected override void PostSaveFailed(Exception ex) {
			// TODO: Log error here
		}


		//-----------------------------------------------------------------------------
		// Override Properties
		//-----------------------------------------------------------------------------

		/// <summary>The path to the settings file.</summary>
		protected override string SettingsPath {
			get {
				return Path.Combine(
					Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
					"My Games", "Zelda Oracle Engine", "GameSettings.ini");
			}
		}

		/// <summary>The comments to display at the top of the ini file.</summary>
		protected override string HeaderComments {
			get {
				return	@"             _________    THE LEGEND OF          " + "\n" +
						@"            / _____  /_________   _____   __     " + "\n" +
						@"            |/    /// \  _ |\ /   \  _ \  \ \    " + "\n" +
						@"                 ///  | |_\|| |   | | \ \ /  \   " + "\n" +
						@"                /O/   |  _| | |   | | | |/ /\ \  " + "\n" +
						@"               ///    | |_/|| |_/|| |_/ / /__\ \ " + "\n" +
						@"              ///____/|____|/____|/____/_/    \_\" + "\n" +
						@"             /________/                          " + "\n" +
						@"                          ORACLE ENGINE          " + "\n" +
						@"===================================================================" + "\n" +
						@"The user settings for Zelda Oracle Engine gameplay." + "\n" +
						@"Deleting this file will reset the settings to their default value.";
			}
		}


		//-----------------------------------------------------------------------------
		// Ini Sections
		//-----------------------------------------------------------------------------

		/// <summary>The customized keyboard controls.</summary>
		public class KeyboardSection {
			/// <summary>True if the keyboard is enabled.</summary>
			[DefaultValue(true)]
			public bool Enabled { get; set; }

			/// <summary>The keyboard Up movement button.</summary>
			[DefaultValue(Keys.Up)]
			[Comments("Movement:")]
			public Keys Up { get; set; }

			/// <summary>The keyboard Down movement button.</summary>
			[DefaultValue(Keys.Down)]
			public Keys Down { get; set; }

			/// <summary>The keyboard Left movement button.</summary>
			[DefaultValue(Keys.Left)]
			public Keys Left { get; set; }

			/// <summary>The keyboard Right movement button.</summary>
			[DefaultValue(Keys.Right)]
			public Keys Right { get; set; }

			/// <summary>The keyboard Start button.</summary>
			[DefaultValue(Keys.Enter)]
			[Comments("Menu:")]
			public Keys Start { get; set; }

			/// <summary>The keyboard Select button.</summary>
			[DefaultValue(Keys.RShift)]
			public Keys Select { get; set; }

			/// <summary>The keyboard A button.</summary>
			[DefaultValue(Keys.Z)]
			[Comments("Buttons:")]
			public Keys A { get; set; }

			/// <summary>The keyboard B button.</summary>
			[DefaultValue(Keys.X)]
			public Keys B { get; set; }

			/// <summary>The keyboard X button.</summary>
			[DefaultValue(Keys.S)]
			[Comments("Currently Unused:")]
			public Keys X { get; set; }

			/// <summary>The keyboard Y button.</summary>
			[DefaultValue(Keys.A)]
			public Keys Y { get; set; }
		}

		/// <summary>The customized gamepad controls.</summary>
		public class GamePadSection {
			/// <summary>True if the gamepad is enabled.</summary>
			[DefaultValue(true)]
			public bool Enabled { get; set; }

			/// <summary>The gamepad Up movement button.</summary>
			[DefaultValue(Buttons.LeftStickUp)]
			public Buttons Up { get; set; }

			/// <summary>The gamepad Down movement button.</summary>
			[DefaultValue(Buttons.LeftStickDown)]
			public Buttons Down { get; set; }

			/// <summary>The gamepad Left movement button.</summary>
			[DefaultValue(Buttons.LeftStickLeft)]
			public Buttons Left { get; set; }

			/// <summary>The gamepad Right movement button.</summary>
			[DefaultValue(Buttons.LeftStickRight)]
			public Buttons Right { get; set; }

			/// <summary>The gamepad analog movement stick.</summary>
			[DefaultValue(AnalogSticks.LeftStick)]
			[Comments("Movement:\n" + @"See https://git.io/vbIZ2 for a list of analog sticks")]
			public AnalogSticks AnalogStick { get; set; }

			/// <summary>The gamepad Start button.</summary>
			[DefaultValue(Buttons.Start)]
			[Comments("Menus:")]
			public Buttons Start { get; set; }

			/// <summary>The gamepad Select button.</summary>
			[DefaultValue(Buttons.Back)]
			public Buttons Select { get; set; }

			/// <summary>The gamepad A button.</summary>
			[DefaultValue(Buttons.A)]
			[Comments("Buttons:")]
			public Buttons A { get; set; }

			/// <summary>The gamepad B button.</summary>
			[DefaultValue(Buttons.B)]
			public Buttons B { get; set; }

			/// <summary>The gamepad X button.</summary>
			[DefaultValue(Buttons.X)]
			[Comments("Currently Unused:")]
			public Buttons X { get; set; }

			/// <summary>The gamepad Y button.</summary>
			[DefaultValue(Buttons.Y)]
			public Buttons Y { get; set; }
		}

		// Audio ----------------------------------------------------------------------

		/// <summary>The volume settings.</summary>
		public class AudioSection {
			/// <summary>The master volume setting between 0.0 and 1.0.</summary>
			[DefaultValue(0.5f)]
			public float MasterVolume {
				get { return AudioSystem.MasterVolume; }
				set { AudioSystem.MasterVolume = value; }
			}

			/// <summary>The sound volume setting between 0.0 and 1.0.</summary>
			[DefaultValue(1.0f)]
			public float SoundVolume {
				get { return AudioSystem.SoundVolume; }
				set { AudioSystem.SoundVolume = value; }
			}

			/// <summary>The music volume setting between 0.0 and 1.0.</summary>
			[DefaultValue(1.0f)]
			public float MusicVolume {
				get { return AudioSystem.MusicVolume; }
				set { AudioSystem.MusicVolume = value; }
			}
		}


		//-----------------------------------------------------------------------------
		// Ini Properties
		//-----------------------------------------------------------------------------

		/// <summary>The customized keyboard controls.</summary>
		[Section]
		[Comments(@"See https://git.io/vbIZ8 for a list of keys")]
		public KeyboardSection Keyboard { get; } = new KeyboardSection();

		/// <summary>The customized gamepad controls.</summary>
		[Section]
		[Comments(@"See https://git.io/vbIZE for a list of gamepad buttons")]
		public GamePadSection GamePad { get; } = new GamePadSection();

		/// <summary>The volume settings.</summary>
		[Section]
		[Comments(@"Volume must be a value between 0.0 and 1.0")]
		public AudioSection Audio { get; } = new AudioSection();


	}
}
