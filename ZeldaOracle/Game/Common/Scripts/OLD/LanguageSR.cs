using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Translation;

namespace ZeldaOracle.Common.Scripts {
/** <summary>
 * Script reader for language data files.
 *
 * FORMAT:
 *
 * lang_name    = Native Language Name
 * lang_code    = Lanuage code
 * lang_default = Default translation
 *
 * text_key_1 = Translation 1
 * text_key_2 = Translation 2
 * ...
 * </summary> */
public class LanguageSR : ScriptReader {

	//=========== MEMBERS ============
	#region Members

	/** <summary> The current language being created. </summary> */
	private Language language;
	/** <summary> The last loaded languaget. </summary> */
	private Language finalLanguage;

	#endregion
	//========== PROPERTIES ==========
	#region Properties

	/** <summary> Gets the last loaded languaget. </summary> */
	public Language Language {
		get { return finalLanguage; }
	}

	#endregion
	//=========== OVERRIDE ===========
	#region Override

	/** <summary> Begins reading the script. </summary> */
	protected override void BeginReading() {
		language = new Language();
		finalLanguage = language;
	}
	/** <summary> Ends reading the script. </summary> */
	protected override void EndReading() {
		Resources.AddLanguage(language);
	}
	/** <summary> Read a single line of the script. </summary> */
	protected override void ReadLine(string line) {
		// Ignore comments.
		if (line.StartsWith("#"))
			return;

		// Parse translation entry.
		if (line.Contains("=")) {
			int index = 0;

			// Parse text key.
			while (index < line.Length && line[index] != ' ' && line[index] != '\t')
				index++;
			string textKey = line.Substring(0, index);

			// Skip over whitespace and equals sign.
			while (index < line.Length && line[index] != '=')
				index++;
			index++;
			while (index < line.Length && (line[index] == ' ' || line[index] == '\t'))
				index++;

			// Parse translation.
			string translation = line.Substring(index);

			if (textKey == "lang_name")
				language.Name = translation;
			else if (textKey == "lang_code")
				language.Code = translation;
			else if (textKey == "lang_default")
				language.DefaultTranslation = translation;
			else
				language[textKey] = translation;
		}
	}
	/** <summary> Reads a line in the script as a command. </summary> */
	protected override bool ReadCommand(string command, List<string> args) {
		return false;
	}

	#endregion
}
} // end namespace
