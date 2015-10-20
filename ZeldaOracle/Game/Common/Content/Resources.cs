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
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripts;
using ZeldaOracle.Common.Translation;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.EventTiles;
using ZeldaOracle.Game.Worlds;
using Song = ZeldaOracle.Common.Audio.Song;
using Playlist = ZeldaOracle.Common.Audio.Playlist;
using ZeldaOracle.Common.Scripting;

namespace ZeldaOracle.Common.Content {

	// An fatal exception that's thrown during the games LoadContent function
	// This is used to end the game before updates start happening.
	public class LoadContentException : Exception {
		public LoadContentException(string message) :
			base(message) {
		}

		public virtual void PrintMessage() {
			Console.WriteLine(Message);
		}
	}

	/** <summary>
	 * The Resources class serves as a resource manager.
	 * It has methods to load in resources from the game
	 * content and stores them in key/value maps.
	 * </summary> 
	 */
	public class Resources {
	
		// CONTAINMENT:
		// The game's content manager.
		private static ContentManager contentManager;
		// The game's graphics device.
		private static GraphicsDevice graphicsDevice;
		// A map of the resource dictionaries by resource type.
		private static Dictionary<Type, object> resourceDictionaries;

		// GRAPHICS:
		// The collection of loaded images.
		private static Dictionary<string, Image> images;
		// The collection of loaded real fonts.
		private static Dictionary<string, RealFont> realFonts;
		// The collection of loaded game fonts.
		private static Dictionary<string, GameFont> gameFonts;
		// The collection of loaded sprite sheets.
		private static Dictionary<string, SpriteSheet> spriteSheets;
		// The collection of loaded sprites.
		private static Dictionary<string, Sprite> sprites;
		// The collection of loaded animations.
		private static Dictionary<string, Animation> animations;
		// The collection of loaded shaders.
		private static Dictionary<string, Effect> shaders;
		// The texture loader for loading images from file.
		private static TextureLoader textureLoader;

		// The collection of loaded collision models.
		private static Dictionary<string, CollisionModel> collisionModels;
		private static Dictionary<string, Tileset> tilesets;
		private static Dictionary<string, TileData> tileData;
		private static Dictionary<string, EventTileData> eventTileData;
		private static Dictionary<string, Zone> zones;
		private static Dictionary<string, PropertyAction> propertyActions;

		// SOUNDS:
		// The collection of loaded sound effects.
		private static Dictionary<string, Sound> sounds;
		// The collection of loaded songs.
		private static Dictionary<string, Song> songs;
		// The collection of loaded playlists.
		private static Dictionary<string, Playlist> playlists;

		// LANGUAGES:
		// The collection of loaded languages.
		private static List<Language> languages;

		// SETTINGS:
		// True if the resource manager should output load information to the console.
		private static bool verboseOutput;
		
	
		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		// Graphics
		// The directory for storing images.
		public const string ImageDirectory = "Images/";
		// The directory for storing fonts.
		public const string FontDirectory = "Fonts/";
		// The directory for storing sprite sheets.
		public const string SpriteSheetDirectory = "SpriteSheets/";
		// The directory for storing animations.
		public const string AnimationDirectory = "Animations/";
		// The directory for storing shaders.
		public const string ShaderDirectory = "Shaders/";

		// Sounds
		// The directory for storing sounds.
		public const string SoundDirectory = "Sounds/";
		// The directory for storing songs.
		public const string MusicDirectory = "Music/";

		// Languages
		// The directory for storing languages.
		public const string LanguageDirectory = "Languages/";
	
	
		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------

		// Initializes the resource manager.
		public static void Initialize(ContentManager contentManager, GraphicsDevice graphicsDevice) {
			// Containment
			Resources.contentManager	= contentManager;
			Resources.graphicsDevice	= graphicsDevice;

			// Graphics
			images				= new Dictionary<string, Image>();
			realFonts			= new Dictionary<string, RealFont>();
			gameFonts			= new Dictionary<string, GameFont>();
			spriteSheets		= new Dictionary<string, SpriteSheet>();
			sprites				= new Dictionary<string, Sprite>();
			animations			= new Dictionary<string, Animation>();
			shaders				= new Dictionary<string, Effect>();
			textureLoader		= new TextureLoader(graphicsDevice);

			// Sounds
			sounds				= new Dictionary<string, Sound>();
			songs				= new Dictionary<string, Song>();
			playlists			= new Dictionary<string, Playlist>();

			// Languages
			languages			= new List<Language>();

			collisionModels		= new Dictionary<string, CollisionModel>();
			tilesets			= new Dictionary<string, Tileset>();
			tileData			= new Dictionary<string, TileData>();
			eventTileData		= new Dictionary<string, EventTileData>();
			zones				= new Dictionary<string, Zone>();

			propertyActions		= new Dictionary<string, PropertyAction>();

			// Settings
			verboseOutput		= false;

			// Setup the resource dictionary lookup map.
			resourceDictionaries = new Dictionary<Type, object>();
			resourceDictionaries[typeof(Image)]				= images;
			resourceDictionaries[typeof(RealFont)]			= realFonts;
			resourceDictionaries[typeof(GameFont)]			= gameFonts;
			resourceDictionaries[typeof(SpriteSheet)]		= spriteSheets;
			resourceDictionaries[typeof(Sprite)]			= sprites;
			resourceDictionaries[typeof(Animation)]			= animations;
			resourceDictionaries[typeof(Effect)]			= shaders;
			resourceDictionaries[typeof(Sound)]				= sounds;
			resourceDictionaries[typeof(Song)]				= songs;
			resourceDictionaries[typeof(Playlist)]			= playlists;
			resourceDictionaries[typeof(CollisionModel)]	= collisionModels;
			resourceDictionaries[typeof(Tileset)]			= tilesets;
			resourceDictionaries[typeof(TileData)]			= tileData;
			resourceDictionaries[typeof(EventTileData)]		= eventTileData;
			resourceDictionaries[typeof(Zone)]				= zones;
			resourceDictionaries[typeof(PropertyAction)]	= propertyActions;
		}

	
		//-----------------------------------------------------------------------------
		// Generic Resource Methods
		//-----------------------------------------------------------------------------
	
		// Does the a resource with the given name and type exist?
		public static bool ExistsResource<T>(T resource) {
			if (!ExistsResourceType<T>())
				return false; // This type of resource doesn't exist!
			Dictionary<string, T> dictionary = (Dictionary<string, T>) resourceDictionaries[typeof(T)];
			return dictionary.ContainsValue(resource);
		}

		// Does the a resource with the given name and type exist?
		public static bool ExistsResource<T>(string name) {
			if (!ExistsResourceType<T>())
				return false; // This type of resource doesn't exist!
			Dictionary<string, T> dictionary = (Dictionary<string, T>) resourceDictionaries[typeof(T)];
			return dictionary.ContainsKey(name);
		}

		// Get the resource with the given name and type.
		public static T GetResource<T>(string name) {
			if (name.Length == 0)
				return default(T);
			if (!ExistsResourceType<T>())
				return default(T); // This type of resource doesn't exist!
			Dictionary<string, T> dictionary = (Dictionary<string, T>) resourceDictionaries[typeof(T)];
			//if (!dictionary.ContainsKey(name))
			//	return default(T); // A resource with the given name doesn't exist!
			return dictionary[name];
		}

		// Get the resource with the given name and type.
		public static string GetResourceName<T>(T resource) where T : class {
			if (!ExistsResourceType<T>())
				return ""; // This type of resource doesn't exist!
			Dictionary<string, T> dictionary = (Dictionary<string, T>) resourceDictionaries[typeof(T)];
			return dictionary.FirstOrDefault(x => x.Value == resource).Key;
		}

		// Get the dictionary used to store the given type of resources.
		public static Dictionary<string, T> GetResourceDictionary<T>() {
			if (!ExistsResourceType<T>())
				return null; // This type of resource doesn't exist!
			return (Dictionary<string, T>) resourceDictionaries[typeof(T)];
		}
	
		// Is the given type of resource handled by this class?
		public static bool ExistsResourceType<T>() {
			return resourceDictionaries.ContainsKey(typeof(T));
		}

		// Add the given resource under the given name.
		public static void AddResource<T>(string assetName, T resource) {
			if (!ExistsResourceType<T>())
				return; // This type of resource doesn't exist!
			Dictionary<string, T> dictionary = (Dictionary<string, T>) resourceDictionaries[typeof(T)];
			dictionary.Add(assetName, resource);
		}


		//-----------------------------------------------------------------------------
		// Resource Accessors
		//-----------------------------------------------------------------------------

		// Gets a sprite or animation depending on which one exists.
		public static SpriteAnimation GetSpriteAnimation(string name) {
			if (sprites.ContainsKey(name))
				return sprites[name];
			else
				return animations[name];
		}

		// Gets the image with the specified name.
		public static Image GetImage(string name) {
			return images[name];
		}

		// Returns true if an image with the specified name exists.
		public static bool ImageExists(string name) {
			return images.ContainsKey(name);
		}

		// Gets the real font with the specified name.
		public static RealFont GetRealFont(string name) {
			return realFonts[name];
		}

		// Returns true if a real font with the specified name exists.
		public static bool RealFontExists(string name) {
			return realFonts.ContainsKey(name);
		}

		// Gets the game font with the specified name.
		public static GameFont GetGameFont(string name) {
			return gameFonts[name];
		}

		// Returns true if a game font with the specified name exists.
		public static bool GameFontExists(string name) {
			return gameFonts.ContainsKey(name);
		}

		// Gets the sprite sheet with the specified name.
		public static SpriteSheet GetSpriteSheet(string name) {
			return spriteSheets[name];
		}

		// Returns true if a sprite sheet with the specified name exists.
		public static bool SpriteSheetExists(string name) {
			return spriteSheets.ContainsKey(name);
		}

		// Gets the sprite with the specified name.
		public static Sprite GetSprite(string name) {
			return sprites[name];
		}

		// Returns true if a sprite with the specified name exists.
		public static bool SpriteExists(string name) {
			return sprites.ContainsKey(name);
		}

		// Gets the animation with the specified name.
		public static Animation GetAnimation(string name) {
			return animations[name];
		}

		// Returns true if an animation with the specified name exists.
		public static bool AnimationExists(string name) {
			return animations.ContainsKey(name);
		}

		// Gets the shader with the specified name.
		public static Effect GetShader(string name) {
			return shaders[name];
		}

		// Returns true if a shader with the specified name exists.
		public static bool ShaderExists(string name) {
			return shaders.ContainsKey(name);
		}

		// Gets the sound with the specified name.
		public static Sound GetSound(string name) {
			return sounds[name];
		}

		// Gets the song with the specified name.
		public static Song GetSong(string name) {
			return songs[name];
		}

		// Gets the playlist with the specified name.
		public static Playlist GetPlaylist(string name) {
			return playlists[name];
		}

		// Gets the language with the specified name.
		public static Language GetLanguage(string name) {
			foreach (Language language in languages) {
				if (language.Name == name)
					return language;
			}
			return null;
		}

	
		//-----------------------------------------------------------------------------
		// Resource Loading
		//-----------------------------------------------------------------------------

		// Loads the image with the specified asset name.
		public static Image LoadImage(string assetName, bool addToRegistry = true) {
			string name = assetName.Substring(assetName.IndexOf('/') + 1);
			Image resource = new Image(contentManager.Load<Texture2D>(assetName), name);
			if (addToRegistry)
				images.Add(name, resource);
			return resource;
		}

		// Loads the image with the specified asset name.
		public static Image LoadImageFromFile(string assetName, bool addToRegistry = true) {
			string name = assetName.Substring(assetName.IndexOf('/') + 1);
			name = name.Substring(0, name.LastIndexOf('.'));
			Image resource = new Image(textureLoader.FromFile(contentManager.RootDirectory + "/" + assetName), name);
			if (addToRegistry)
				images.Add(name, resource);
			return resource;
		}

		// Loads the real font with the specified asset name.
		public static RealFont LoadRealFont(string assetName) {
			string name = assetName.Substring(assetName.IndexOf('/') + 1);
			RealFont resource = new RealFont(contentManager.Load<SpriteFont>(assetName), name);
			realFonts.Add(name, resource);
			return resource;
		}

		// Loads a shader (Effect).
		public static Effect LoadShader(string assetName) {
			string name = assetName.Substring(assetName.IndexOf('/') + 1);
			Effect resource = contentManager.Load<Effect>(assetName);
			shaders.Add(name, resource);
			return resource;
		}

		// Loads a single sprite sheet from a script file.
		public static SpriteSheet LoadSpriteSheet(string assetName) {
			SpriteSheetSROld script = new SpriteSheetSROld();
			LoadScript(assetName, script);
			return script.Sheet;
		}

		// Loads/compiles sprite sheets from a script file.
		public static void LoadSpriteSheets(string assetName) {
			LoadScript(assetName, new SpritesSR());
		}

		// Loads/compiles images from a script file.
		public static void LoadImagesFromScript(string assetName) {
			LoadScript(assetName, new ImagesSR());
		}

		// Loads the game font with the specified asset name.
		public static GameFont LoadGameFont(string assetName) {
			GameFontSR script = new GameFontSR();
			LoadScript(assetName, script);
			return script.Font;
		}

		// Loads/compiles game fonts from a script file.
		public static void LoadGameFonts(string assetName) {
			LoadScript(assetName, new GameFontSR());
		}

		// Loads/compiles animations from a script file.
		public static void LoadAnimations(string assetName) {
			LoadScript(assetName, new AnimationSR());
		}

		// Loads/compiles collision models from a script file.
		public static void LoadCollisionModels(string assetName) {
			LoadScript(assetName, new CollisionModelSR());
		}

		// Loads/compiles tilesets from a script file.
		public static void LoadTilesets(string assetName) {
			LoadScript(assetName, new TilesetSR());
		}

		// Loads a sound effect.
		public static Sound LoadSound(string assetName) {
			string name = assetName.Substring(assetName.IndexOf('/') + 1);
			Sound resource = new Sound(contentManager.Load<SoundEffect>(assetName), assetName, name);
			sounds.Add(name, resource);
			return resource;
		}

		// Loads a song.
		public static Song LoadSong(string assetName) {
			string name = assetName.Substring(assetName.IndexOf('/') + 1);
			Song resource = new Song(contentManager.Load<SoundEffect>(assetName), assetName, name);
			songs.Add(name, resource);
			return resource;
		}

		// Loads a sounds file.
		public static void LoadSounds(string assetName) {
			LoadScript(assetName, new SoundsSR());
		}

		// Loads a music file.
		public static void LoadMusic(string assetName) {
			LoadScript(assetName, new MusicSR());
		}

		// Loads a language file.
		public static Language LoadLanguage(string assetName) {
			assetName = assetName.Substring(assetName.IndexOf('/') + 1);
			LanguageSR script = new LanguageSR();
			LoadScript(assetName, script, Encoding.UTF8);
			return script.Language;
		}


		//-----------------------------------------------------------------------------
		// Scripts
		//-----------------------------------------------------------------------------

		// Loads a script file with the given script reader object.
		public static void LoadScript(string assetName, ScriptReader sr) {
			LoadScript(assetName, sr, Encoding.Default);
		}

		// Loads a script file with the given encoding and script reader object.
		public static void LoadScript(string assetName, ScriptReader sr, Encoding encoding) {
			try {
				Stream stream = TitleContainer.OpenStream(contentManager.RootDirectory + "/" + assetName);
				StreamReader reader = new StreamReader(stream, encoding);
				sr.ReadScript(reader, assetName);
				stream.Close();
			}
			catch (FileNotFoundException) {
				Console.WriteLine("Error loading file \"" + assetName + "\"");
			}
		}


		//-----------------------------------------------------------------------------
		// Resource Adding.
		//-----------------------------------------------------------------------------
		
		// Adds the specified animation.
		public static void AddImage(string assetName, Image image) {
			images.Add(assetName, image);
		}

		// Adds the specified sprite sheet.
		public static void AddSpriteSheet(string assetName, SpriteSheet sheet) {
			spriteSheets.Add(assetName, sheet);
		}

		// Adds the specified sprite.
		public static void AddSprite(string assetName, Sprite sprite) {
			sprites.Add(assetName, sprite);
		}

		// Adds the specified game font.
		public static void AddGameFont(string assetName, GameFont font) {
			gameFonts.Add(assetName, font);
		}

		// Adds the specified animation.
		public static void AddAnimation(string assetName, Animation animation) {
			animations.Add(assetName, animation);
		}

		// Adds the specified language.
		public static void AddLanguage(Language language) {
			languages.Add(language);
		}

	
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Gets the content manager.
		public static ContentManager ContentManager {
			get { return contentManager; }
		}

		// Gets or sets if the resource manager should output load information to the console.
		public static bool VerboseOutput {
			get { return verboseOutput; }
			set { verboseOutput = value; }
		}

		// Gets the dictionary of tilesets.
		public static Dictionary<string, Tileset> Tilesets {
			get { return tilesets; }
		}

		// Gets the dictionary of sounds.
		public static Dictionary<string, Sound> Sounds {
			get { return sounds; }
		}

		// Gets the list of langauges.
		public static List<Language> Languages {
			get { return languages; }
		}

		// Gets the list of sprite sheets.
		public static SpriteSheet[] SpriteSheets {
			get {
				SpriteSheet[] sheets = new SpriteSheet[spriteSheets.Count];
				spriteSheets.Values.CopyTo(sheets, 0);
				return sheets;
			}
		}

		// Gets the number of sprite sheets.
		public static int SpriteSheetCount {
			get { return spriteSheets.Count; }
		}
	}
} // end namespace
