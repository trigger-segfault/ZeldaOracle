using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using RoslynPad.Roslyn;
using RoslynPad.Roslyn.Diagnostics;
using ZeldaOracle.Game;

namespace ZeldaEditor.Scripting {
	public class CustomRoslynHost : RoslynHost {

		internal static readonly ImmutableArray<string> PreprocessorSymbols =
			ImmutableArray.CreateRange(new[] { "__DEMO__", "__DEMO_EXPERIMENTAL__", "TRACE", "DEBUG" });

		public CustomRoslynHost()
			: base(null, GetAssemblies(), GetReferences())
		{
		}

		private static IEnumerable<Assembly> GetAssemblies() {
			return new Assembly[] {
				Assembly.Load("RoslynPad.Roslyn.Windows"),
				Assembly.Load("RoslynPad.Editor.Windows")
			};
		}

		private static RoslynHostReferences GetReferences() {
			RoslynHostReferences refs = RoslynHostReferences.Default;
			return refs.With(assemblyReferences: Assemblies.PureScripting);
		}

		protected override CompilationOptions CreateCompilationOptions(DocumentCreationArgs args, bool addDefaultImports) {
			var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
				usings: Assemblies.ScriptUsings,
				allowUnsafe: true,
				sourceReferenceResolver: new SourceFileResolver(ImmutableArray<string>.Empty, args.WorkingDirectory));
			return compilationOptions;
		}


		protected override ParseOptions CreateDefaultParseOptions() {
			return new CSharpParseOptions(kind: SourceCodeKind.Script,
				preprocessorSymbols: PreprocessorSymbols, languageVersion: LanguageVersion.Latest);
		}

	}
}
