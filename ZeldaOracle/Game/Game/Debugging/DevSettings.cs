using System;
using System.ComponentModel;
using System.IO;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Ini;
using ZeldaOracle.Common.Input;

namespace ZeldaOracle.Game {

	public class DevSettings : IniReflectionSettings {

		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		/// <summary>Called after loading finishes only if the load was unsuccessful.</summary>
		protected override void PostLoadFailed(Exception ex) {
			// TODO: Lo g error here 
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
					"My Games", "Zelda Oracle Engine", "DevSettings.ini");
			}
		}

		/// <summary>The comments to display at the top of the ini file.</summary>
		protected override string HeaderComments {
			get {
				return "Development Settings for the Zelda Oracle Engine";
			}
		}


		//-----------------------------------------------------------------------------
		// Ini Sections
		//-----------------------------------------------------------------------------

		public class StartLocationSection {
			[DefaultValue("../../../../../WorldFiles/temp_world.zwd")]
			public string WorldFile { get; set; }
			[DefaultValue("default")]
			public string Level { get; set; }
			//[DefaultPoint2IValue(0, 0)]
			public Point2I Room { get; set; }
			//[DefaultPoint2IValue(0, 0)]
			public Point2I Location { get; set; }
		}


		//-----------------------------------------------------------------------------
		// Ini Properties
		//-----------------------------------------------------------------------------

		[Section]
		public StartLocationSection StartLocation { get; } = new StartLocationSection();


	}
}