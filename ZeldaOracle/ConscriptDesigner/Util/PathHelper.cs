using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConscriptDesigner.Util {
	public static class PathHelper {
		public static bool IsValidName(string name) {
			return name.IndexOfAny(Path.GetInvalidFileNameChars()) == -1;
		}

		public static bool IsValidPath(string name) {
			return name.IndexOfAny(Path.GetInvalidPathChars()) == -1;
		}

		public static void CopyDirectory(string sourceDir, string destDir) {
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

			// Get the files in the directory and copy them to the new location.
			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo file in files) {
				string temppath = Path.Combine(destDir, file.Name);
				file.CopyTo(temppath, true);
			}

			// Copy subdirectories and their contents to new location.
			foreach (DirectoryInfo subdir in dirs) {
				string temppath = Path.Combine(destDir, subdir.Name);
				CopyDirectory(subdir.FullName, temppath);
			}
		}
	}
}
