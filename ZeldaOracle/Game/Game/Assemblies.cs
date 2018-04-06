using System;
using System.Linq;
using System.Reflection;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game {
	/// <summary>A collection of all assemblies needed for the game.</summary>
	public static class Assemblies {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The assembly for the base Zelda classes that are used within
		/// the game and scripting.</summary>
		public static readonly Assembly ZeldaCommon = typeof(Point2I).Assembly;
		/// <summary>The assembly for the base Zelda scripting classes.</summary>
		public static readonly Assembly ZeldaAPI = typeof(ZeldaAPI.Tile).Assembly;
		/// <summary>The assembly for all Zelda game classes.</summary>
		public static readonly Assembly ZeldaOracle = typeof(GameData).Assembly;

		/// <summary>The assembly for basic C# implementation.</summary>
		public static readonly Assembly Mscorlib = typeof(object).Assembly;
		/// <summary>The assembly for System.</summary>
		public static readonly Assembly System = typeof(Uri).Assembly;
		/// <summary>The assembly for System.Core.</summary>
		public static readonly Assembly SystemCore = typeof(Enumerable).Assembly;


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the assemblies used just for the Zelda game.</summary>
		public static Assembly[] Game {
			get {
				return new Assembly[] {
					ZeldaCommon,
					ZeldaOracle
				};
			}
		}

		/// <summary>Gets the assemblies used for compiling scripts.</summary>
		public static Assembly[] Scripting {
			get {
				return new Assembly[] {
					Mscorlib,
					System,
					SystemCore,
					ZeldaCommon,
					ZeldaAPI
				};
			}
		}

		/// <summary>Gets the (Zelda-specific) assemblies used for compiling scripts.</summary>
		public static Assembly[] PureScripting {
			get {
				return new Assembly[] {
					ZeldaCommon,
					ZeldaAPI
				};
			}
		}

		/// <summary>All of the using imports for scripting.</summary>
		public static string[] ScriptUsings {
			get {
				return new string[] {
					"System.Collections",
					"System.Collections.Generic",
					"ZeldaAPI",
					"ZeldaOracle.Game",
					"ZeldaOracle.Common.Geometry",
				};
			}
		}
	}
}
