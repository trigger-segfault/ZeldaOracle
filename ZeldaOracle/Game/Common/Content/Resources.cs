using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XnaSong = Microsoft.Xna.Framework.Media.Song;
using XnaPlaylist = Microsoft.Xna.Framework.Media.Playlist;

using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Graphics;
//using ZeldaOracle.Common.Graphics.Particles;
using ZeldaOracle.Common.Scripts;
using ZeldaOracle.Common.Translation;
using Song = ZeldaOracle.Common.Audio.Song;
using Playlist = ZeldaOracle.Common.Audio.Playlist;

namespace ZeldaOracle.Common.Content {
/** <summary>
 * The Resources class serves as a resource manager.
 * It has methods to load in resources from the game
 * content and stores them in key/value maps.
 * </summary> */
public class Resources {

	//========== CONSTANTS ===========
	#region Constants

	// Graphics
	/** <summary> The directory for storing images. </summary> */
	public const string ImageDirectory = "Images/";
	/** <summary> The directory for storing fonts. </summary> */
	public const string FontDirectory = "Fonts/";
	/** <summary> The directory for storing sprite sheets. </summary> */
	public const string SpriteSheetDirectory = "SpriteSheets/";
	/** <summary> The directory for storing shaders. </summary> */
	public const string ShaderDirectory = "Shaders/";

	// Particles
	/** <summary> The directory for storing particles. </summary> */
	public const string ParticleDirectory = "Particles/";

	// Sounds
	/** <summary> The directory for storing sounds. </summary> */
	public const string SoundDirectory = "Sounds/";
	/** <summary> The directory for storing songs. </summary> */
	public const string MusicDirectory = "Music/";

	// Languages
	/** <summary> The directory for storing languages. </summary> */
	public const string LanguageDirectory = "Languages/";

	#endregion
	//========== VARIABLES ===========
	#region Variables

	// Containment
	/** <summary> The game's content manager. </summary> */
	private static ContentManager contentManager;
	/** <summary> The game's graphics device. </summary> */
	private static GraphicsDevice graphicsDevice;

	// Graphics
	/** <summary> The collection of loaded images. </summary> */
	private static Dictionary<string, Image> images;
	/** <summary> The collection of loaded fonts. </summary> */
	private static Dictionary<string, Font> fonts;
	/** <summary> The collection of loaded sprite sheets. </summary> */
	private static Dictionary<string, SpriteSheet> spriteSheets;
	/** <summary> The collection of loaded effects. </summary> */
	private static Dictionary<string, Effect> shaders;
	/** <summary> The texture loader for loading images from file. </summary> */
	private static TextureLoader textureLoader;

	// Particles
	/** <summary> The collection of loaded particle types. </summary> */
	//private static Dictionary<string, ParticleType> particleTypes;
	/** <summary> The collection of loaded particle emitters. </summary> */
	//private static Dictionary<string, ParticleEmitter> particleEmitters;
	/** <summary> The collection of loaded particle effect types. </summary> */
	//public static Dictionary<string, ParticleEffectType> particleEffects;

	// Sounds
	/** <summary> The collection of loaded sound effects. </summary> */
	private static Dictionary<string, Sound> sounds;
	/** <summary> The collection of loaded songs. </summary> */
	private static Dictionary<string, Song> songs;
	/** <summary> The collection of loaded playlists. </summary> */
	private static Dictionary<string, Playlist> playlists;
	/** <summary> The root sound group. </summary> */
	private static SoundGroup soundGroup;

	// Languages
	/** <summary> The collection of loaded languages. </summary> */
	private static List<Language> languages;

	// Settings
	/** <summary> True if the resource manager should output load information to the console. </summary> */
	private static bool verboseOutput;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Initializes the resource manager. </summary> */
	public static void Initialize(ContentManager contentManager, GraphicsDevice graphicsDevice) {
		// Containment
		Resources.contentManager	= contentManager;
		Resources.graphicsDevice	= graphicsDevice;

		// Graphics
		images				= new Dictionary<string, Image>();
		fonts				= new Dictionary<string, Font>();
		spriteSheets		= new Dictionary<string, SpriteSheet>();
		shaders				= new Dictionary<string, Effect>();
		textureLoader		= new TextureLoader(graphicsDevice);

		// Particles
		//particleTypes		= new Dictionary<string, ParticleType>();
		//particleEmitters	= new Dictionary<string, ParticleEmitter>();
		//particleEffects		= new Dictionary<string, ParticleEffectType>();

		// Sounds
		sounds				= new Dictionary<string, Sound>();
		songs				= new Dictionary<string, Song>();
		playlists			= new Dictionary<string, Playlist>();
		soundGroup			= new SoundGroup("root");

		// Languages
		languages			= new List<Language>();

		// Settings
		verboseOutput		= false;
	}

	#endregion
	//========== RESOURCES ===========
	#region Resources
	//--------------------------------
	#region Graphics

	/** <summary> Gets the image with the specified name. </summary> */
	public static Image GetImage(string name) {
		return images[name];
	}
	/** <summary> Gets the font with the specified name. </summary> */
	public static Font GetFont(string name) {
		return fonts[name];
	}
	/** <summary> Gets the sprite sheet with the specified name. </summary> */
	public static SpriteSheet GetSpriteSheet(string name) {
		return spriteSheets[name];
	}
	/** <summary> Returns true if a sprite sheet with the specified name exists. </summary> */
	public static bool SpriteSheetExists(string name) {
		return spriteSheets.ContainsKey(name);
	}
	/** <summary> Gets the shader with the specified name. </summary> */
	public static Effect GetShader(string name) {
		return shaders[name];
	}

	#endregion
	//--------------------------------
	#region Particles

	/** <summary> Gets the particle type with the specified name. </summary> */
	/*public static ParticleType GetParticleType(string name) {
		return particleTypes[name];
	}*/
	/** <summary> Returns true if a particle type with the specified name exists. </summary> */
	/*public static bool ParticleTypeExists(string name) {
		return particleTypes.ContainsKey(name);
	}*/
	/** <summary> Gets the particle emitter with the specified name. </summary> */
	/*public static ParticleEmitter GetParticleEmitter(string name) {
		return particleEmitters[name];
	}*/
	/** <summary> Returns true if a particle emitter with the specified name exists. </summary> */
	/*public static bool ParticleEmitterExists(string name) {
		return particleEmitters.ContainsKey(name);
	}*/
	/** <summary> Gets the particle effect type with the specified name. </summary> */
	/*public static ParticleEffectType GetParticleEffect(string name) {
		return particleEffects[name];
	}*/
	/** <summary> Returns true if a particle effect type with the specified name exists. </summary> */
	/*public static bool ParticleEffectExists(string name) {
		return particleEffects.ContainsKey(name);
	}*/

	#endregion
	//--------------------------------
	#region Sounds

	/** <summary> Gets the sound with the specified name. </summary> */
	public static Sound GetSound(string name) {
		return sounds[name];
	}
	/** <summary> Gets the song with the specified name. </summary> */
	public static Song GetSong(string name) {
		return songs[name];
	}
	/** <summary> Gets the playlist with the specified name. </summary> */
	public static Playlist GetPlaylist(string name) {
		return playlists[name];
	}
	/** <summary> Gets the sound group with the specified name. </summary> */
	public static SoundGroup GetSoundGroup(string name) {
		return soundGroup.GetGroup(name);
	}

	#endregion
	//--------------------------------
	#region Languages

	/** <summary> Gets the language with the specified name. </summary> */
	public static Language GetLanguage(string name) {
		foreach (Language language in languages) {
			if (language.Name == name)
				return language;
		}
		return null;
	}

	#endregion
	//--------------------------------
	#endregion
	//=========== LOADING ============
	#region Loading
	//--------------------------------
	#region Graphics

	/** <summary> Loads the image with the specified asset name. </summary> */
	public static Image LoadImage(string assetName) {
		string name = assetName.Substring(assetName.IndexOf('/') + 1);
		Image resource = new Image(contentManager.Load<Texture2D>(assetName), name);
		images.Add(name, resource);
		return resource;
	}
	/** <summary> Loads the image with the specified asset name. </summary> */
	public static Image LoadImageFromFile(string assetName) {
		string name = assetName.Substring(assetName.IndexOf('/') + 1);
		name = name.Substring(0, name.LastIndexOf('.'));
		//FileStream stream = new FileStream(contentManager.RootDirectory + "/" + assetName, FileMode.Open);
		//Image resource = new Image(Texture2D.FromStream(graphicsDevice, stream), name);
		Image resource = new Image(textureLoader.FromFile(contentManager.RootDirectory + "/" + assetName), name);
		//stream.Close();
		images.Add(name, resource);
		return resource;
	}
	/** <summary> Loads the font with the specified asset name. </summary> */
	public static Font LoadFont(string assetName) {
		string name = assetName.Substring(assetName.IndexOf('/') + 1);
		Font resource = new Font(contentManager.Load<SpriteFont>(assetName), name);
		fonts.Add(name, resource);
		return resource;
	}
	/** <summary> Loads a shader (Effect). </summary> */
	public static Effect LoadShader(string assetName) {
		string name = assetName.Substring(assetName.IndexOf('/') + 1);
		Effect resource = contentManager.Load<Effect>(assetName);
		shaders.Add(name, resource);
		return resource;
	}
	/** <summary> Loads a single sprite sheet from a script file. </summary> */
	public static SpriteSheet LoadSpriteSheet(string assetName) {
		SpriteSheetSR script = new SpriteSheetSR();
		LoadScript(assetName, script);
		return script.Sheet;
	}
	/** <summary> Loads/compiles sprite sheets from a script file. </summary> */
	public static void LoadSpriteSheets(string assetName) {
		LoadScript(assetName, new SpriteSheetSR());
	}

	#endregion
	//--------------------------------
	#region Particles

	/** <summary> Loads/compiles particles, emitter, and effects from a script file. </summary> */
	/*public static void LoadParticles(string assetName) {
		LoadScript(assetName, new ParticleSR());
	}*/

	#endregion
	//--------------------------------
	#region Sounds

	/** <summary> Loads a sound effect. </summary> */
	public static Sound LoadSound(string assetName) {
		string name = assetName.Substring(assetName.IndexOf('/') + 1);
		Sound resource = new Sound(contentManager.Load<SoundEffect>(assetName), assetName, name);
		sounds.Add(name, resource);
		return resource;
	}
	/** <summary> Loads a song. </summary> */
	public static Song LoadSong(string assetName) {
		string name = assetName.Substring(assetName.IndexOf('/') + 1);
		Song resource = new Song(contentManager.Load<XnaSong>(assetName), assetName, name);
		songs.Add(name, resource);
		return resource;
	}
	/** <summary> Loads a sound groups file. </summary> */
	public static void LoadSoundGroups(string assetName) {
		LoadScript(assetName, new SoundGroupSR());
	}
	/** <summary> Loads a playlists file. </summary> */
	public static void LoadPlaylists(string assetName) {
		LoadScript(assetName, new PlaylistSR());
	}

	#endregion
	//--------------------------------
	#region Languages

	/** <summary> Loads a language file. </summary> */
	public static Language LoadLanguage(string assetName) {
		assetName = assetName.Substring(assetName.IndexOf('/') + 1);
		LanguageSR script = new LanguageSR();
		LoadScript(assetName, script, Encoding.UTF8);
		return script.Language;
	}

	#endregion
	//--------------------------------
	#endregion
	//=========== SCRIPTS ============
	#region Scripts

	/** <summary> Loads a script file with the given script reader object. </summary> */
	public static void LoadScript(string assetName, ScriptReader sr) {
		LoadScript(assetName, sr, Encoding.Default);
	}
	/** <summary> Loads a script file with the given encoding and script reader object. </summary> */
	public static void LoadScript(string assetName, ScriptReader sr, Encoding encoding) {
		try {
			Stream stream = TitleContainer.OpenStream(contentManager.RootDirectory + "/" + assetName);
			StreamReader reader = new StreamReader(stream, encoding);
			sr.ReadScript(reader);
			stream.Close();
		}
		catch (FileNotFoundException) {
			Console.WriteLine("Error loading file \"" + assetName + "\"");
		}
	}
	/** <summary> Loads a script file with the given encoding and script reader object. </summary> */
	/*public static void SaveScript(string assetName, ParticleSW sw, Encoding encoding) {
		try {
			StreamWriter writer = new StreamWriter(contentManager.RootDirectory + "/" + assetName);
			sw.WriteScript(writer);
			writer.Close();
		}
		catch (FileNotFoundException) {
			Console.WriteLine("Error saving file \"" + assetName + "\"");
		}
	}*/

	#endregion
	//============ ADDING ============
	#region Adding
	//--------------------------------
	#region Graphics

	/** <summary> Adds the specified sprite sheet. </summary> */
	public static void AddSpriteSheet(SpriteSheet sheet) {
		spriteSheets.Add(sheet.Name, sheet);
	}

	#endregion
	//--------------------------------
	#region Particles

	/** <summary> Adds the specified particle type. </summary> */
	/*public static void AddParticleType(ParticleType particleType) {
		particleTypes.Add(particleType.Name, particleType);
	}*/
	/** <summary> Adds the specified particle emitter. </summary> */
	/*public static void AddParticleEmitter(ParticleEmitter particleEmitter) {
		particleEmitters.Add(particleEmitter.Name, particleEmitter);
	}*/
	/** <summary> Adds the specified particle effect type. </summary> */
	/*public static void AddParticleEffect(ParticleEffectType particleEffect) {
		particleEffects.Add(particleEffect.Name, particleEffect);
	}*/
	/** <summary> Removes the specified particle type. </summary> */
	/*public static void RemoveParticleType(ParticleType particleType) {
		particleTypes.Remove(particleType.Name);
	}*/
	/** <summary> Removes the specified particle emitter. </summary> */
	/*public static void RemoveParticleEmitter(ParticleEmitter particleEmitter) {
		particleEmitters.Remove(particleEmitter.Name);
	}*/
	/** <summary> Removes the specified particle effect type. </summary> */
	/*public static void RemoveParticleEffect(ParticleEffectType particleEffect) {
		particleEffects.Remove(particleEffect.Name);
	}*/
	/** <summary> Renames the specified particle type. </summary> */
	/*public static void RenameParticleType(string oldName, string newName) {
		//particleTypes;
		particleTypes[oldName].Name = newName;
		ParticleType[] types = ParticleTypes;

		particleTypes.Clear();

		for (int i = 0; i < types.Length; i++) {
			particleTypes.Add(types[i].Name, types[i]);
		}
	}*/
	/** <summary> Renames the specified particle emitter. </summary> */
	/*public static void RenameParticleEmitter(string oldName, string newName) {
		//particleTypes;
		particleEmitters[oldName].Name = newName;
		ParticleEmitter[] types = ParticleEmitters;

		particleEmitters.Clear();

		for (int i = 0; i < types.Length; i++) {
			particleEmitters.Add(types[i].Name, types[i]);
		}
	}*/
	/** <summary> Renames the specified particle effect. </summary> */
	/*public static void RenameParticleEffect(string oldName, string newName) {
		//particleTypes;
		particleEffects[oldName].Name = newName;
		ParticleEffectType[] types = ParticleEffects;

		particleEffects.Clear();

		for (int i = 0; i < types.Length; i++) {
			particleEffects.Add(types[i].Name, types[i]);
		}
	}*/

	#endregion
	//--------------------------------
	#region Sounds

	/** <summary> Adds the specified playlist. </summary> */
	public static void AddPlaylist(Playlist playlist) {
		playlists.Add(playlist.Name, playlist);
	}

	#endregion
	//--------------------------------
	#region Languages

	/** <summary> Adds the specified language. </summary> */
	public static void AddLanguage(Language language) {
		languages.Add(language);
	}

	#endregion
	//--------------------------------
	#endregion
	//========== PROPERTIES ==========
	#region Properties

	/** <summary> Gets or sets if the resource manager should output load information to the console. </summary> */
	public static bool VerboseOutput {
		get { return verboseOutput; }
		set { verboseOutput = value; }
	}
	/** <summary> Gets the root sound group. </summary> */
	public static SoundGroup RootSoundGroup {
		get { return soundGroup; }
	}
	/** <summary> Gets the list of langauges. </summary> */
	public static List<Language> Languages {
		get { return languages; }
	}
	/** <summary> Gets the list of particle types. </summary> */
	/*public static ParticleType[] ParticleTypes {
		get {
			ParticleType[] types = new ParticleType[particleTypes.Count];
			particleTypes.Values.CopyTo(types, 0);
			return types;
		}
	}*/
	/** <summary> Gets the list of particle emitters. </summary> */
	/*public static ParticleEmitter[] ParticleEmitters {
		get {
			ParticleEmitter[] emitters = new ParticleEmitter[particleEmitters.Count];
			particleEmitters.Values.CopyTo(emitters, 0);
			return emitters;
		}
	}*/
	/** <summary> Gets the list of particle effect types. </summary> */
	/*public static ParticleEffectType[] ParticleEffects {
		get {
			ParticleEffectType[] effects = new ParticleEffectType[particleEffects.Count];
			particleEffects.Values.CopyTo(effects, 0);
			return effects;
		}
	}*/
	/** <summary> Gets the list of sprite sheets. </summary> */
	public static SpriteSheet[] SpriteSheets {
		get {
			SpriteSheet[] sheets = new SpriteSheet[spriteSheets.Count];
			spriteSheets.Values.CopyTo(sheets, 0);
			return sheets;
		}
	}
	/** <summary> Gets the number of particle types. </summary> */
	/*public static int ParticleTypeCount {
		get { return particleTypes.Count; }
	}*/
	/** <summary> Gets the number of particle emitters. </summary> */
	/*public static int ParticleEmitterCount {
		get { return particleEmitters.Count; }
	}*/
	/** <summary> Gets the number of particle effect types. </summary> */
	/*public static int ParticleEffectCount {
		get { return particleEffects.Count; }
	}*/
	/** <summary> Gets the number of sprite sheets. </summary> */
	public static int SpriteSheetCount {
		get { return spriteSheets.Count; }
	}

	#endregion
}
} // end namespace
