using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracleBuilder.Content;
using ZeldaOracleBuilder.Util;

namespace ZeldaOracleBuilder {
	public static class Program {

		//-----------------------------------------------------------------------------
		// Enumerations
		//-----------------------------------------------------------------------------

		/// <summary>The additional paths for bin directories.</summary>
		public enum BinTypes {
			/// <summary>No additional path is used.</summary>
			Current,
			/// <summary>Debug directory is used.</summary>
			Debug,
			/// <summary>Release directory is used.</summary>
			Release
		}

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The game binaries that need copying to the output directory.</summary>
		private static readonly string[] RequiredGameBinaries = {
			"ZeldaOracle.exe",
			"ZeldaOracle.exe.config",
			"ZeldaCommon.dll",
			"ZeldaAPI.dll",
			"ZeldaAPI.xml"
		};

		/// <summary>The editor binaries that need copying to the output directory.</summary>
		private static readonly string[] RequiredEditorBinaries = {
			"ZeldaEditor.exe",
			"ZeldaEditor.exe.config",
			"ICSharpCode.AvalonEdit.dll",
			"ICSharpCode.CodeCompletion.dll",
			"ICSharpCode.NRefactory.Cecil.dll",
			"ICSharpCode.NRefactory.CSharp.dll",
			"ICSharpCode.NRefactory.dll",
			"ICSharpCode.NRefactory.Xml.dll",
			"Mono.Cecil.dll",
			"Xceed.Wpf.Toolkit.dll"
		};


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>The path to the content project file.</summary>
		public static string ContentProjectFile { get; private set; }
		/// <summary>The path to the content project directory.</summary>
		public static string ContentProjectDirectory {
			get { return Path.GetDirectoryName(ContentProjectFile); }
		}

		/// <summary>The path to the game binaries directory.</summary>
		public static string GameBinDirectory { get; private set; }
		/// <summary>The path to the editor binaries directory.</summary>
		public static string EditorBinDirectory { get; private set; }

		/// <summary>The path to the output binaries directory.</summary>
		public static string OutputBinDirectory { get; private set; }
		/// <summary>The path to the output binaries content directory.</summary>
		public static string OutputContentDirectory {
			get { return Path.Combine(OutputBinDirectory, "Content"); }
		}
		/// <summary>The path to the preloaded paletted sprite database file.</summary>
		public static string OutputSpriteDatabaseFile {
			get { return Path.Combine(OutputContentDirectory, "PalettedSprites.dat"); }
		}

		/// <summary>True if game content should not be recompiled.</summary>
		public static bool NoCompile { get; private set; }
		/// <summary>The binary path type to append to the supplied paths.</summary>
		public static BinTypes BinType { get; private set; }


		/// <summary>The content project containing information about all content files.</summary>
		public static ContentRoot Project { get; private set; }


		//-----------------------------------------------------------------------------
		// Main
		//-----------------------------------------------------------------------------

		/// <summary>The entry point for the program.</summary>
		static int Main(string[] args) {
			if (args.Length == 0) {
				WriteHelpMessage();
				return 0;
			}

			try {
				// Parse the command line arguments
				for (int i = 0; i < args.Length; i++) {
					string arg = args[i];
					if (arg == "-content" || arg == "-c") {
						ThrowIf(i + 1 == args.Length, "-content must specify a content project file!");
						ThrowIf(ContentProjectFile != null, "-content already specified!");
						ContentProjectFile = CheckPath(args[++i], "-content path is invalid!");
					}
					else if (arg == "-game" || arg == "-g") {
						ThrowIf(i + 1 == args.Length, "-game must specify a game bin directory!");
						ThrowIf(GameBinDirectory != null, "-game already specified!");
						GameBinDirectory = CheckPath(args[++i], "-game path is invalid!");
					}
					else if (arg == "-editor" || arg == "-e") {
						ThrowIf(i + 1 == args.Length, "-editor must specify an editor bin directory!");
						ThrowIf(EditorBinDirectory != null, "-editor already specified!");
						EditorBinDirectory = CheckPath(args[++i], "-editor path is invalid!");
					}
					else if (arg == "-out" || arg == "-o") {
						ThrowIf(i + 1 == args.Length, "-game must specify an output bin directory!");
						ThrowIf(OutputBinDirectory != null, "-out already specified!");
						OutputBinDirectory = CheckPath(args[++i], "-out path is invalid!");
					}
					else if (arg == "-no-compile" || arg == "-nc") {
						ThrowIf(NoCompile, "-no-compile already specified!");
						NoCompile = true;
					}
					else if (arg == "-debug" || arg == "-d") {
						ThrowIf(BinType != BinTypes.Current, "-debug or -release already specified!");
						BinType = BinTypes.Debug;
					}
					else if (arg == "-release" || arg == "-r") {
						ThrowIf(BinType != BinTypes.Current, "-debug or -release already specified!");
						BinType = BinTypes.Release;
					}
					else {
						throw new ArgumentException("Unknown command '" + arg + "'!");
					}
				}

				// Make sure everything is setup
				ThrowIf(ContentProjectFile	== null, "-content must be specified!");
				ThrowIf(GameBinDirectory	== null, "-game must be specified!");
				ThrowIf(OutputBinDirectory	== null, "-out must be specified!");

				GameBinDirectory	= AppendBin(GameBinDirectory);
				EditorBinDirectory	= AppendBin(EditorBinDirectory);
				OutputBinDirectory	= AppendBin(OutputBinDirectory);


				Console.WriteLine("Beginning Post-build!");
				
				// Update content
				Console.WriteLine("Updating Content...");
				Project = new ContentRoot();
				Project.LoadContentProject(ContentProjectFile);
				if (!NoCompile)
					CompileContent();
				UpdateContentFolder();
				
				// Update game binaries
				Console.WriteLine("Copying Game Binaries...");
				CopyGameBinaries();

				// Update editor binaries
				if (EditorBinDirectory != null) {
					Console.WriteLine("Copying Editor Binaries...");
					CopyEditorBinaries();
				}
				
				Console.WriteLine("Post-build Complete!");
				Console.WriteLine();

				return 0;
			}
			catch (Exception ex) {
				Console.WriteLine("ZeldaOracleBuilder.exe:");
				Console.WriteLine(ex.Message);
				return -1;
			}
		}


		//-----------------------------------------------------------------------------
		// Helper Methods
		//-----------------------------------------------------------------------------
		
		/// <summary>Writes the help message.</summary>
		private static void WriteHelpMessage() {
			Console.WriteLine("ZeldaOracleBuilder.exe:");
			Console.WriteLine("-c  -content <directory>  Specify the content project file (Required).");
			Console.WriteLine("-g  -game <directory>     Specify the game bin directory (Required).");
			Console.WriteLine("-e  -editor <directory>   Specify the editor bin directory.");
			Console.WriteLine("-o  -out <directory>      Specify the output bin directory (Required).");
			Console.WriteLine("-nc -no-compile           Skip recompiling the content files.");
			Console.WriteLine("-d  -debug                Binary directories will append /Debug to the end.");
			Console.WriteLine("-r  -release              Binary directories will append /Release to the end.");
		}

		/// <summary>Throws an error with the specified message if the value is true.</summary>
		private static void ThrowIf(bool value, string message) {
			if (value)
				throw new ArgumentException(message);
		}

		/// <summary>Throws an error with the specified message if the path is invalid
		/// or does not exist. Otherwise returns the path.</summary>
		private static string CheckPath(string path, string message) {
			if (!PathHelper.IsValidPath(path) && PathHelper.Exists(path))
				throw new ArgumentException(message);
			return path;
		}

		/// <summary>Appends the bin path type to the supplied path.</summary>
		private static string AppendBin(string path) {
			if (path == null)
				return null;
			switch (BinType) {
			case BinTypes.Debug: return Path.Combine(path, "Debug");
			case BinTypes.Release: return Path.Combine(path, "Release");
			default: return path;
			}
		}


		//-----------------------------------------------------------------------------
		// Updating Methods
		//-----------------------------------------------------------------------------

		/// <summary>Compiles all game content.</summary>
		private static void CompileContent() {
			foreach (ContentFile file in Project.GetAllFiles()) {
				if (file.ShouldCompile) {
					string outPath = file.OutputFilePath;
					string outDir = Path.GetDirectoryName(outPath);
					if (!Directory.Exists(outDir))
						Directory.CreateDirectory(outDir);
					Console.WriteLine("Building " + file.Path.Replace('/', '\\') + " -> " + outPath.Replace('/', '\\'));
					file.Compile();
				}
			}
		}

		/// <summary>Updates the contents of the output content folder.</summary>
		private static void UpdateContentFolder(ContentFolder folder = null) {
			if (folder == null)
				folder = Project;
			HashSet<string> existingFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (string file in Directory.GetFiles(folder.OutputFilePath)) {
				existingFiles.Add(Path.GetFileName(file));
			}
			foreach (string file in Directory.GetDirectories(folder.OutputFilePath)) {
				existingFiles.Add(Path.GetFileName(file));
			}
			foreach (ContentFile file in folder.GetLocalFiles()) {
				string inPath = file.FilePath;
				string outPath = file.OutputFilePath;
				if (outPath == null)
					continue;
				string outName = Path.GetFileName(outPath);
				if (existingFiles.Contains(outName)) {
					if (file.ShouldCopyToOutput /*&& File.GetLastWriteTimeUtc(inPath) != File.GetLastWriteTimeUtc(outPath)*/) {
						File.Copy(inPath, outPath, true);
					}
					existingFiles.Remove(outName);
				}
				else if (file.ShouldCopyToOutput) {
					File.Copy(inPath, outPath, true);
				}

				if (file.IsFolder) {
					if (!Directory.Exists(outPath))
						Directory.CreateDirectory(outPath);
					UpdateContentFolder((ContentFolder) file);
				}
			}

			// Remove files that are no longer needed
			foreach (string leftoverFile in existingFiles) {
				string filePath = Path.Combine(folder.OutputFilePath, leftoverFile);
				if (Directory.Exists(filePath))
					Directory.Delete(filePath, true);
				// Don't delete the sprite database
				else if (!PathHelper.IsPathTheSame(filePath, OutputSpriteDatabaseFile))
					File.Delete(Path.Combine(folder.OutputFilePath, leftoverFile));
			}
		}

		/// <summary>Copy all binary files required by the game to the output directory.</summary>
		private static void CopyGameBinaries() {
			foreach (string file in RequiredGameBinaries) {
				string source = Path.Combine(GameBinDirectory, file);
				string dest = Path.Combine(OutputBinDirectory, file);
				ThrowIf(!File.Exists(source), "Missing game binary '" + file + "'!");
				File.Copy(source, dest, true);
			}
		}

		/// <summary>Copy all binary files required by the editor to the output directory.</summary>
		private static void CopyEditorBinaries() {
			foreach (string file in RequiredEditorBinaries) {
				string source = Path.Combine(EditorBinDirectory, file);
				string dest = Path.Combine(OutputBinDirectory, file);
				ThrowIf(!File.Exists(source), "Missing editor binary '" + file + "'!");
				File.Copy(source, dest, true);
			}
		}
	}
}
