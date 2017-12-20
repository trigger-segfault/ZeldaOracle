using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConscriptDesigner.Content {
	public class InvalidFileNameException : IOException {
		public InvalidFileNameException(string fileName) :
			base("The file name '" + fileName + "' contains invalid characters!") { }
	}

	public class InvalidFilePathException : IOException {
		public InvalidFilePathException(string filePath) :
			base("The file path '" + filePath + "' contains invalid characters!") { }
	}

	public class DirectoryAlreadyExistsException : IOException {
		public DirectoryAlreadyExistsException(string fileName) :
			base("The directory '" + fileName + "' already exists!") { }
	}

	public class FileAlreadyExistsException : IOException {
		public FileAlreadyExistsException(string fileName) :
			base("The file '" + fileName + "' already exists!") { }
	}

	public class FileDoesNotExistException : FileNotFoundException {
		public FileDoesNotExistException(string fileName) :
			base("No file with the name '" + fileName + "' exists!") { }
	}

	public class DirectoryDoesNotExistException : DirectoryNotFoundException {
		public DirectoryDoesNotExistException(string directoryPath) :
			base("No folder at the path '" + directoryPath + "' exists!") { }
	}

	public class DirectoryIsSubdirectoryException : IOException {
		public DirectoryIsSubdirectoryException(string directory, string subdirectory) :
			base("Cannot move to '" + subdirectory + "' because it is a subdirectory of '" + directory + "'!") { }
	}

	public class DirectoryFileMismatchException : IOException {
		public DirectoryFileMismatchException(string name, string newName) :
			base("Cannot move to '" + name + "' to '" + newName + "' due to directory/file mismatch!") { }
	}

	public class InvalidProjectFileException : IOException {
		public InvalidProjectFileException(string projectName) :
			base("The xml in '" + projectName + "' does not form a valid project file!") { }
	}
}
