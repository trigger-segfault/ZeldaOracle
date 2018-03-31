using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoslynPad.Roslyn;
using ZeldaOracle.Game;

namespace ZeldaEditor.Scripting {
	/// <summary>The customized Roslyn Host for the script editor.</summary>
	public class ScriptRoslynHost : RoslynHost {

		/// <summary>The default supplied preprocessor symbols.</summary>
		//internal static readonly ImmutableArray<string> PreprocessorSymbols =
		//	ImmutableArray.CreateRange(new[] { "__DEMO__", "__DEMO_EXPERIMENTAL__", "TRACE", "DEBUG" });

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the Script Roslyn Host.</summary>
		public ScriptRoslynHost() : base(null, GetAssemblies(), GetReferences()) {
		}

		
		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		/// <summary>Overrides the compilation options to change the using imports.</summary>
		protected override CompilationOptions CreateCompilationOptions(DocumentCreationArgs args, bool addDefaultImports) {
			var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
				usings: Assemblies.ScriptUsings,
				allowUnsafe: true,
				sourceReferenceResolver: new SourceFileResolver(ImmutableArray<string>.Empty, args.WorkingDirectory));
			return compilationOptions;
		}
		
		/// <summary>Overrides the parse options.</summary>
		/*protected override ParseOptions CreateDefaultParseOptions() {
			return new CSharpParseOptions(kind: SourceCodeKind.Script,
				preprocessorSymbols: PreprocessorSymbols, languageVersion: LanguageVersion.Latest);
		}*/


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Gets the assemblies needed for the RoslynHost.</summary>
		private static IEnumerable<Assembly> GetAssemblies() {
			return new Assembly[] {
				Assembly.Load("RoslynPad.Roslyn.Windows"),
				Assembly.Load("RoslynPad.Editor.Windows"),
			};
		}

		/// <summary>Gets the assemblies to reference.</summary>
		private static RoslynHostReferences GetReferences() {
			RoslynHostReferences refs = RoslynHostReferences.Default;
			return refs.With(assemblyReferences: Assemblies.PureScripting);
		}
	}
}
