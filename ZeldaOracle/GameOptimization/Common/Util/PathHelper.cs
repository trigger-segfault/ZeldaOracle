using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ZeldaOracle.Common.Util {
	/// <summary>A helper with extra methods for paths, files, and directories.</summary>
	public static class PathHelper {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The stored executable path for the entry assembly.</summary>
		public static readonly string ExePath =
			Assembly.GetEntryAssembly().Location;

		/// <summary>The directory of the entry executable.</summary>
		public static readonly string ExeDirectory =
			Path.GetDirectoryName(ExePath);

		/// <summary>Gets the file name of the entry executable.</summary>
		public static readonly string ExeFile =
			Path.GetFileName(ExePath);

		/// <summary>Gets the file name of the entry executable without its extension.</summary>
		public static readonly string ExeName =
			Path.GetFileNameWithoutExtension(ExePath);
		
		/// <summary>Provides a platform-specific character used to separate directory
		/// levels in a path string that reflects a hierarchical file system
		/// organization.</summary>
		public static readonly char[] DirectorySeparators = new char[] {
			Path.DirectorySeparatorChar,
			Path.AltDirectorySeparatorChar,
		};


		//-----------------------------------------------------------------------------
		// Methods
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the fileName has valid characters.</summary>
		public static bool IsValidName(string name) {
			return name.IndexOfAny(Path.GetInvalidFileNameChars()) == -1;
		}

		/// <summary>Returns true if the filePath has valid characters.</summary>
		public static bool IsValidPath(string name) {
			return name.IndexOfAny(Path.GetInvalidPathChars()) == -1;
		}

		/// <summary>Copies a file or directory.</summary>
		public static void CopyFileOrDirectory(string sourcePath, string destPath, bool overwrite) {
			if (Directory.Exists(sourcePath))
				CopyDirectory(sourcePath, destPath, overwrite);
			else
				File.Copy(sourcePath, destPath, overwrite);
		}

		/// <summary>Moves a file or directory.</summary>
		public static void MoveFileOrDirectory(string sourcePath, string destPath) {
			if (Directory.Exists(sourcePath))
				Directory.Move(sourcePath, destPath);
			else
				File.Move(sourcePath, destPath);
		}

		/// <summary>Deletes a file or directory.</summary>
		public static void DeleteFileOrDirectory(string path) {
			if (Directory.Exists(path))
				Directory.Delete(path, true);
			else
				File.Delete(path);
		}

		/// <summary>Returns true if a file or directory exists at the path.</summary>
		public static bool Exists(string path) {
			return (File.Exists(path) || Directory.Exists(path));
		}

		/// <summary>Copies the directory and all subfolders and files.</summary>
		public static void CopyDirectory(string sourceDir, string destDir, bool merge) {
			// Get the subdirectories for the specified directory.
			DirectoryInfo dir = new DirectoryInfo(sourceDir);

			if (!dir.Exists) {
				throw new DirectoryNotFoundException(
					"Source directory does not exist or could not be found: "
					+ sourceDir);
			}

			DirectoryInfo[] dirs = dir.GetDirectories();
			// If the destination directory doesn't exist, create it.
			if (!Directory.Exists(destDir)) {
				Directory.CreateDirectory(destDir);
			}
			else if (!merge) {
				throw new IOException("Directory already exists at location!");
			}

			// Get the files in the directory and copy them to the new location.
			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo file in files) {
				string temppath = Path.Combine(destDir, file.Name);
				file.CopyTo(temppath, true);
			}

			// Copy subdirectories and their contents to new location.
			foreach (DirectoryInfo subdir in dirs) {
				string temppath = Path.Combine(destDir, subdir.Name);
				CopyDirectory(subdir.FullName, temppath, true);
			}
		}

		/// <summary>Returns a path that can be compared with another normalized path.</summary>
		public static string NormalizePath(string path) {
			return Path.GetFullPath(path)
					   .TrimEnd(DirectorySeparators)
					   .ToUpperInvariant();
		}

		/// <summary>Returns true if the two paths lead to the same location.</summary>
		public static bool IsPathTheSame(string path1, string path2) {
			return string.Compare(NormalizePath(path1), NormalizePath(path2), true) == 0;
		}

		/// <summary>Removes the ending directory separator from the path.</summary>
		public static string TrimEnd(string path) {
			return path.TrimEnd(DirectorySeparators);
		}

		/// <summary>Returns a file path with copy appended to the filename.</summary>
		public static string GetCopyName(string path) {
			string newPath = Path.GetFileNameWithoutExtension(path) + " - Copy";
			string ext = Path.GetExtension(path);

			int index = 1;
			string finalPath = newPath + ext;
			while (Exists(finalPath)) {
				index++;
				finalPath = newPath + " (" + index + ")" + ext;
			}
			return finalPath;
		}

		/// <summary>Combines the specified paths with the executable directory.</summary>
		public static string CombineExecutable(string path1) {
			return Path.Combine(ExeDirectory, path1);
		}

		/// <summary>Combines the specified paths with the executable directory.</summary>
		public static string CombineExecutable(string path1, string path2) {
			return Path.Combine(ExeDirectory, path1, path2);
		}

		/// <summary>Combines the specified paths with the executable directory.</summary>
		public static string CombineExecutable(string path1, string path2,
			string path3)
		{
			return Path.Combine(ExeDirectory, path1, path2, path3);
		}

		/// <summary>Combines the specified paths with the executable directory.</summary>
		public static string CombineExecutable(params string[] paths) {
			return Path.Combine(ExeDirectory, Path.Combine(paths));
		}

		/// <summary>Returns a collection of all files and subfiles in the directory.</summary>
		public static List<string> GetAllFiles(string directory) {
			List<string> files = new List<string>();
			AddAllFiles(files, directory);
			return files;
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Adds all of the files and subfiles in the directory to the list.</summary>
		private static void AddAllFiles(List<string> files, string directory) {
			foreach (string file in Directory.GetFiles(directory)) {
				files.Add(file);
			}
			foreach (string dir in Directory.GetDirectories(directory)) {
				AddAllFiles(files, dir);
			}
		}
	}
}
