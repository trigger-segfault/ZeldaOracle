using System;
using System.Collections.Generic;
using System.Text;

namespace ZeldaOracle.Common.Translation {
	/// <summary>The container of translations for a language.</summary>
	public class Language {

		//=========== MEMBERS ============
		#region Members

		/// <summary>The collection of translations.</summary>
		private Dictionary<string, string> dictionary;
		/// <summary>The native language name.</summary>
		private string name;
		/// <summary>Language code (ex: en, de, ru...).</summary>
		private string code;
		/// <summary>Used for unimplemented translations.</summary>
		private string defaultTranslation;

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		/// <summary>Constructs the default language.</summary>
		public Language() {
			this.dictionary			= new Dictionary<string, string>();

			this.name				= "";
			this.code				= "";
			this.defaultTranslation	= "N/A";
		}
		/// <summary>Constructs the specified language.</summary>
		public Language(string name, string code, string defaultTranslation) {
			this.dictionary			= new Dictionary<string, string>();

			this.name				= name;
			this.code				= code;
			this.defaultTranslation	= defaultTranslation;
		}

		#endregion
		//========== PROPERTIES ==========
		#region Properties

		/// <summary>Translate the given text key.</summary>
		public string this[string textKey] {
			get { return Translate(textKey); }
			set { SetTranslation(textKey, value); }
		}
		/// <summary>Gets or sets the native language name.</summary>
		public string Name {
			get { return name; }
			set { name = value; }
		}
		/// <summary>Gets or sets the langauge code.</summary>
		public string Code {
			get { return code; }
			set { code = value; }
		}
		/// <summary>Gets or sets the default translation.</summary>
		public string DefaultTranslation {
			get { return defaultTranslation; }
			set { defaultTranslation = value; }
		}

		#endregion
		//========= TRANSLATION ==========
		#region Translation

		/// <summary>Gets the translation for the given key.</summary>
		public string Translate(string textKey) {
			if (!dictionary.ContainsKey(textKey))
				return defaultTranslation;
			return dictionary[textKey];
		}
		/// <summary>Sets the translation for the given key.</summary>
		public void SetTranslation(string textKey, string translation) {
			dictionary[textKey] = translation;
		}

		#endregion
	}
} // end namespace
