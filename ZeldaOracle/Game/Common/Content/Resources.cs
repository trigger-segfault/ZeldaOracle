using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Scripts;
using ZeldaOracle.Common.Scripts.CustomReaders;
using ZeldaOracle.Common.Translation;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;
using ZeldaOracle.Game.Worlds;
using Song = ZeldaOracle.Common.Audio.Song;
using XnaSong = Microsoft.Xna.Framework.Media.Song;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Common.Content {

	/// <summary>A fatal exception that's thrown during the games LoadContent function
	/// This is used to end the game before updates start happening.</summary>
	public class LoadContentException : Exception {

		/// <summary>Constructs the load content exception.</summary>
		public LoadContentException(string message) :
			base(message) {
		}

		/// <summary>Prints the exception message.</summary>
		public virtual void PrintMessage() {
			Console.WriteLine(Message);
		}
	}

	/// <summary>A class for storing information about a style group.</summary>
	public class StyleGroupCollection {
		/// <summary>The style group for the contained styles.</summary>
		public string Group { get; }
		/// <summary>The collection of styles for the style group.</summary>
		public HashSet<string> Styles { get; }
		/// <summary>The default style for the styleGroup.</summary>
		public string DefaultStyle { get; set; }
		/// <summary>The preview for the style group.</summary>
		public ISprite Preview { get; set; }

		/// <summary>Gets if the style group has a preview.</summary>
		public bool HasPreview {
			get { return Preview != null; }
		}
		/// <summary>Gets if the style group has any styles.</summary>
		public bool HasStyles {
			get { return Styles.Any(); }
		}

		/// <summary>Constructs the style group collection.</summary>
		public StyleGroupCollection(string group, StyleSprite preview) {
			this.Group			= group;
			this.Styles			= new HashSet<string>();
			this.DefaultStyle	= "";
			this.Preview		= preview;
		}
	}

	/// <summary>
	/// The Resources class serves as a resource manager.
	/// It has methods to load in resources from the game
	/// content and stores them in key/value maps.
	/// </summary>
	public class Resources {

		// CONTAINMENT:
		/// <summary>The game's content manager.</summary>
		private static ContentManager contentManager;
		/// <summary>The game's graphics device.</summary>
		private static GraphicsDevice graphicsDevice;
		/// <summary>A map of the resource dictionaries by resource type.</summary>
		private static Dictionary<Type, object> resourceDictionaries;

		// GRAPHICS:
		/// <summary>The collection of loaded images.</summary>
		private static Dictionary<string, Image> images;
		/// <summary>The collection of loaded real fonts.</summary>
		private static Dictionary<string, RealFont> realFonts;
		/// <summary>The collection of loaded game fonts.</summary>
		private static Dictionary<string, GameFont> gameFonts;
		/// <summary>The collection of loaded sprite sheets.</summary>
		private static Dictionary<string, ISpriteSource> spriteSheets;
		/// <summary>The collection of loaded sprites.</summary>
		private static Dictionary<string, ISprite> sprites;
		/// <summary>The collection of loaded animations.</summary>
		private static Dictionary<string, Animation> animations;
		/// <summary>The collection of loaded palette dictionaries.</summary>
		private static Dictionary<string, PaletteDictionary> paletteDictionaries;
		/// <summary>The collection of loaded palettes.</summary>
		private static Dictionary<string, Palette> palettes;
		/// <summary>The collection of loaded shaders.</summary>
		private static Dictionary<string, Effect> shaders;
		/// <summary>The texture loader for loading images from file.</summary>
		private static TextureLoader textureLoader;

		/// <summary>The database for creating paletted sprites.</summary>
		private static PalettedSpriteDatabase palettedSpriteDatabase;

		/// <summary>The collection of loaded collision models.</summary>
		private static Dictionary<string, CollisionModel> collisionModels;
		private static Dictionary<string, BaseTileData> baseTileData;
		private static Dictionary<string, TileData> tileData;
		private static Dictionary<string, ActionTileData> actionTileData;
		private static Dictionary<string, Tileset> tilesets;
		private static Dictionary<string, Zone> zones;
		private static Dictionary<string, PropertyAction> propertyActions;

		// SOUNDS:
		/// <summary>The collection of loaded sound effects.</summary>
		private static Dictionary<string, Sound> sounds;
		/// <summary>The collection of loaded songs.</summary>
		private static Dictionary<string, Song> songs;

		// LANGUAGES:
		/// <summary>The collection of loaded languages.</summary>
		private static List<Language> languages;

		// SETTINGS:
		/// <summary>True if the resource manager should output load information to the console.</summary>
		private static bool verboseOutput;

		// STYLE PREVIEWS:
		/// <summary>The list of registered style groups along with their previews for each style.</summary>
		private static Dictionary<string, StyleGroupCollection> registeredStyles;


		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		// Graphics
		/// <summary>The directory for storing images.</summary>
		public const string ImageDirectory = "Images/";
		/// <summary>The directory for storing fonts.</summary>
		public const string FontDirectory = "Fonts/";
		/// <summary>The directory for storing sprite sheets.</summary>
		public const string SpriteDirectory = "Sprites/";
		/// <summary>The directory for storing palette dictionaries.</summary>
		public const string PaletteDictionaryDirectory = "Palettes/Dictionaries/";
		/// <summary>The directory for storing palettes.</summary>
		public const string PaletteDirectory = "Palettes/";
		/// <summary>The directory for storing shaders.</summary>
		public const string ShaderDirectory = "Shaders/";

		// Sounds
		/// <summary>The directory for storing sounds.</summary>
		public const string SoundDirectory = "Sounds/";
		/// <summary>The directory for storing songs.</summary>
		public const string MusicDirectory = "Music/";

		// Languages
		/// <summary>The directory for storing languages.</summary>
		public const string LanguageDirectory = "Languages/";


		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the resource manager.</summary>
		public static void Initialize(ContentManager contentManager, GraphicsDevice graphicsDevice) {
			// Containment
			Resources.contentManager	= contentManager;
			Resources.graphicsDevice	= graphicsDevice;

			// Graphics
			images				= new Dictionary<string, Image>();
			realFonts			= new Dictionary<string, RealFont>();
			gameFonts			= new Dictionary<string, GameFont>();
			spriteSheets		= new Dictionary<string, ISpriteSource>();
			sprites				= new Dictionary<string, ISprite>();
			animations			= new Dictionary<string, Animation>();
			shaders				= new Dictionary<string, Effect>();
			paletteDictionaries = new Dictionary<string, PaletteDictionary>();
			palettes            = new Dictionary<string, Palette>();
			palettedSpriteDatabase  = new PalettedSpriteDatabase();
			textureLoader       = new TextureLoader(graphicsDevice);

			// Sounds
			sounds				= new Dictionary<string, Sound>();
			songs				= new Dictionary<string, Song>();

			// Languages
			languages			= new List<Language>();

			collisionModels		= new Dictionary<string, CollisionModel>();
			baseTileData        = new Dictionary<string, BaseTileData>();
			tileData			= new Dictionary<string, TileData>();
			actionTileData		= new Dictionary<string, ActionTileData>();
			tilesets			= new Dictionary<string, Tileset>();
			zones				= new Dictionary<string, Zone>();

			propertyActions		= new Dictionary<string, PropertyAction>();

			// Settings
			verboseOutput		= false;

			// Style previews
			registeredStyles	= new Dictionary<string, StyleGroupCollection>();

			// Setup the resource dictionary lookup map.
			resourceDictionaries = new Dictionary<Type, object>();
			resourceDictionaries[typeof(Image)]				= images;
			resourceDictionaries[typeof(RealFont)]			= realFonts;
			resourceDictionaries[typeof(GameFont)]			= gameFonts;
			resourceDictionaries[typeof(ISpriteSource)]		= spriteSheets;
			resourceDictionaries[typeof(ISprite)]			= sprites;
			resourceDictionaries[typeof(Animation)]			= animations;
			resourceDictionaries[typeof(Effect)]			= shaders;
			resourceDictionaries[typeof(Sound)]				= sounds;
			resourceDictionaries[typeof(Song)]				= songs;
			resourceDictionaries[typeof(CollisionModel)]	= collisionModels;
			resourceDictionaries[typeof(BaseTileData)]		= baseTileData;
			resourceDictionaries[typeof(TileData)]			= tileData;
			resourceDictionaries[typeof(ActionTileData)]		= actionTileData;
			resourceDictionaries[typeof(Tileset)]			= tilesets;
			resourceDictionaries[typeof(Zone)]				= zones;
			resourceDictionaries[typeof(PropertyAction)]	= propertyActions;
			resourceDictionaries[typeof(PaletteDictionary)] = paletteDictionaries;
			resourceDictionaries[typeof(Palette)]           = palettes;
		}

		public static void Uninitialize() {
			// Have we initialized yet?
			if (images == null) return;
			
			contentManager.Unload();

			images = null;
			realFonts = null;
			gameFonts = null;
			spriteSheets = null;
			sprites = null;
			animations = null;
			shaders = null;
			sounds = null;
			songs = null;
			collisionModels = null;
			baseTileData = null;
			tileData = null;
			actionTileData = null;
			tilesets = null;
			zones = null;
			propertyActions = null;
			paletteDictionaries = null;
			foreach (var pair in palettes) {
				pair.Value.Dispose();
			}
			palettes = null;
			resourceDictionaries = null;
			palettedSpriteDatabase.Dispose();
			palettedSpriteDatabase = null;
			textureLoader = null;

			registeredStyles = null;
		}


		//-----------------------------------------------------------------------------
		// Generic Resource Methods
		//-----------------------------------------------------------------------------

		/// <summary>Does the a resource with the given name and type exist?</summary>
		public static bool ContainsResource<T>(T resource) {
			if (!ContainsResourceType<T>())
				return false; // This type of resource doesn't exist!
			Dictionary<string, T> dictionary = (Dictionary<string, T>) resourceDictionaries[typeof(T)];
			return dictionary.ContainsValue(resource);
		}

		/// <summary>Does the a resource with the given name and type exist?</summary>
		public static bool ContainsResource<T>(string name) {
			if (!ContainsResourceType<T>())
				return false; // This type of resource doesn't exist!
			Dictionary<string, T> dictionary = (Dictionary<string, T>) resourceDictionaries[typeof(T)];
			return dictionary.ContainsKey(name);
		}

		/// <summary>Get the resource with the given name and type.</summary>
		public static T GetResource<T>(string name, bool allowEmptyNames = false) {
			if (name.Length == 0 && !allowEmptyNames)
				return default(T);
			if (!ContainsResourceType<T>())
				return default(T); // This type of resource doesn't exist!
			Dictionary<string, T> dictionary = (Dictionary<string, T>) resourceDictionaries[typeof(T)];
			//if (!dictionary.ContainsKey(name))
			//	return default(T); // A resource with the given name doesn't exist!
			T result;
			dictionary.TryGetValue(name, out result);
			return result;
		}

		/// <summary>Get the resource with the given name and type.</summary>
		public static string GetResourceName<T>(T resource) where T : class {
			if (!ContainsResourceType<T>())
				return ""; // This type of resource doesn't exist!
			Dictionary<string, T> dictionary = (Dictionary<string, T>) resourceDictionaries[typeof(T)];
			return dictionary.FirstOrDefault(x => x.Value == resource).Key;
		}

		/// <summary>Get the dictionary used to store the given type of resources.</summary>
		public static Dictionary<string, T> GetResourceDictionary<T>() {
			if (!ContainsResourceType<T>())
				return null; // This type of resource doesn't exist!
			return (Dictionary<string, T>) resourceDictionaries[typeof(T)];
		}

		/// <summary>Gets the list of resource keys for the given type of resource.</summary>
		public static List<string> GetResourceKeyList(Type type) {
			if (type == typeof(Image))
				return GetResourceKeyList<Image>();
			if (type == typeof(RealFont))
				return GetResourceKeyList<RealFont>();
			if (type == typeof(GameFont))
				return GetResourceKeyList<GameFont>();
			if (type == typeof(ISpriteSource))
				return GetResourceKeyList<ISpriteSource>();
			if (type == typeof(ISprite))
				return GetResourceKeyList<ISprite>();
			if (type == typeof(Animation))
				return GetResourceKeyList<Animation>();
			if (type == typeof(Effect))
				return GetResourceKeyList<Effect>();
			if (type == typeof(Sound))
				return GetResourceKeyList<Sound>();
			if (type == typeof(Song))
				return GetResourceKeyList<Song>();

			if (type == typeof(CollisionModel))
				return GetResourceKeyList<CollisionModel>();
			if (type == typeof(BaseTileData))
				return GetResourceKeyList<BaseTileData>();
			if (type == typeof(TileData))
				return GetResourceKeyList<TileData>();
			if (type == typeof(ActionTileData))
				return GetResourceKeyList<ActionTileData>();
			if (type == typeof(Tileset))
				return GetResourceKeyList<Tileset>();
			if (type == typeof(Zone))
				return GetResourceKeyList<Zone>();

			if (type == typeof(PaletteDictionary))
				return GetResourceKeyList<PaletteDictionary>();
			if (type == typeof(Palette))
				return GetResourceKeyList<Palette>();

			return new List<string>();
		}

		/// <summary>Gets the list of resource keys for the given type of resource.</summary>
		public static List<string> GetResourceKeyList<T>() {
			return ((Dictionary<string, T>) resourceDictionaries[typeof(T)]).Keys.ToList();
		}

		/// <summary>Is the given type of resource handled by this class?</summary>
		public static bool ContainsResourceType<T>() {
			return resourceDictionaries.ContainsKey(typeof(T));
		}

		/// <summary>Add the given resource under the given name.</summary>
		public static void AddResource<T>(string assetName, T resource) {
			if (!ContainsResourceType<T>())
				return; // This type of resource doesn't exist!
			Dictionary<string, T> dictionary = (Dictionary<string, T>) resourceDictionaries[typeof(T)];
			dictionary.Add(assetName, resource);
			// TODO: Properly implement a way to separate animations from sprites.
			if (resource is Animation)
				animations.Add(assetName, resource as Animation);
		}

		/// <summary>Add the given resource under the given name.</summary>
		public static void SetResource<T>(string assetName, T resource) {
			if (!ContainsResourceType<T>())
				return; // This type of resource doesn't exist!
			Dictionary<string, T> dictionary = (Dictionary<string, T>) resourceDictionaries[typeof(T)];
			dictionary[assetName] = resource;
			// TODO: Properly implement a way to separate animations from sprites.
			if (resource is Animation)
				animations[assetName] = resource as Animation;
		}


		//-----------------------------------------------------------------------------
		// Resource Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Gets a sprite or animation depending on which one exists.</summary>
		public static ISprite GetSpriteAnimation(string name) {
			if (sprites.ContainsKey(name))
				return sprites[name];
			else
				return animations[name];
		}

		/// <summary>Gets the image with the specified name.</summary>
		public static Image GetImage(string name) {
			return images[name];
		}

		/// <summary>Returns true if an image with the specified name exists.</summary>
		public static bool ContainsImage(string name) {
			return images.ContainsKey(name);
		}

		/// <summary>Gets the real font with the specified name.</summary>
		public static RealFont GetRealFont(string name) {
			return realFonts[name];
		}

		/// <summary>Returns true if a real font with the specified name exists.</summary>
		public static bool ContainsRealFont(string name) {
			return realFonts.ContainsKey(name);
		}

		/// <summary>Gets the game font with the specified name.</summary>
		public static GameFont GetGameFont(string name) {
			return gameFonts[name];
		}

		/// <summary>Returns true if a game font with the specified name exists.</summary>
		public static bool ContainsGameFont(string name) {
			return gameFonts.ContainsKey(name);
		}

		/// <summary>Gets the sprite sheet with the specified name.</summary>
		public static ISpriteSource GetSpriteSheet(string name) {
			return spriteSheets[name];
		}

		/// <summary>Returns true if a sprite sheet with the specified name exists.</summary>
		public static bool ContainsSpriteSheet(string name) {
			return spriteSheets.ContainsKey(name);
		}

		/// <summary>Gets the sprite with the specified name.</summary>
		public static ISprite GetSprite(string name) {
			return sprites[name];
		}

		/// <summary>Returns true if a sprite with the specified name exists.</summary>
		public static bool ContainsSprite(string name) {
			return sprites.ContainsKey(name);
		}

		/// <summary>Gets the animation with the specified name.</summary>
		public static Animation GetAnimation(string name) {
			return animations[name];
		}

		/// <summary>Returns true if an animation with the specified name exists.</summary>
		public static bool ContainsAnimation(string name) {
			return animations.ContainsKey(name);
		}

		/// <summary>Gets the palette dictionary with the specified name.</summary>
		public static PaletteDictionary GetPaletteDictionary(string name) {
			return paletteDictionaries[name];
		}

		/// <summary>Returns true if a palette dictionary with the specified name exists.</summary>
		public static bool ContainsPaletteDictionary(string name) {
			return paletteDictionaries.ContainsKey(name);
		}

		/// <summary>Gets the palette with the specified name.</summary>
		public static Palette GetPalette(string name) {
			return palettes[name];
		}

		/// <summary>Returns true if a palette with the specified name exists.</summary>
		public static bool ContainsPalette(string name) {
			return palettes.ContainsKey(name);
		}

		/// <summary>Gets the shader with the specified name.</summary>
		public static Effect GetShader(string name) {
			return shaders[name];
		}

		/// <summary>Returns true if a shader with the specified name exists.</summary>
		public static bool ContainsShader(string name) {
			return shaders.ContainsKey(name);
		}

		/// <summary>Gets the sound with the specified name.</summary>
		public static Sound GetSound(string name) {
			return sounds[name];
		}

		/// <summary>Gets the song with the specified name.</summary>
		public static Song GetSong(string name) {
			return songs[name];
		}

		/// <summary>Gets the language with the specified name.</summary>
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

		/// <summary>Loads the image with the specified asset name.</summary>
		public static Image LoadImage(string assetName, bool addToRegistry = true) {
			string name = assetName.Substring(assetName.IndexOf('/') + 1);
			Image resource = new Image(contentManager.Load<Texture2D>(assetName), name);
			if (addToRegistry)
				images.Add(name, resource);
			return resource;
		}

		/// <summary>Loads the image with the specified asset name.</summary>
		public static Image LoadImageFromFile(string assetName, bool addToRegistry = true) {
			string name = assetName.Substring(assetName.IndexOf('/') + 1);
			name = name.Substring(0, name.LastIndexOf('.'));
			Image resource = new Image(textureLoader.FromFile(contentManager.RootDirectory + "/" + assetName), name);
			if (addToRegistry)
				images.Add(name, resource);
			return resource;
		}

		/// <summary>Loads the real font with the specified asset name.</summary>
		public static RealFont LoadRealFont(string assetName) {
			string name = assetName.Substring(assetName.IndexOf('/') + 1);
			RealFont resource = new RealFont(contentManager.Load<SpriteFont>(assetName), name);
			realFonts.Add(name, resource);
			return resource;
		}

		/// <summary>Loads/compiles palette dictionaries from a script file.</summary>
		public static void LoadPaletteDictionaries(string assetName) {
			LoadScript(assetName, new PaletteDictionarySR());
		}

		/// <summary>Loads/compiles palettes from a script file.</summary>
		public static void LoadPalettes(string assetName) {
			LoadScript(assetName, new PaletteSR());
		}

		/// <summary>Loads a shader (Effect).</summary>
		public static Effect LoadShader(string assetName) {
			string name = assetName.Substring(assetName.IndexOf('/') + 1);
			Effect resource = contentManager.Load<Effect>(assetName);
			shaders.Add(name, resource);
			return resource;
		}

		/// <summary>Loads/compiles sprite sheets from a script file.</summary>
		public static void LoadSpriteSheets(string assetName) {
			LoadScript(assetName, new ISpriteSR());
		}

		/// <summary>Loads/compiles images from a script file.</summary>
		public static void LoadImagesFromScript(string assetName) {
			LoadScript(assetName, new ImageSR());
		}

		/// <summary>Loads the game font with the specified asset name.</summary>
		public static GameFont LoadGameFont(string assetName) {
			GameFontSR script = new GameFontSR();
			LoadScript(assetName, script);
			return script.Font;
		}

		/// <summary>Loads/compiles game fonts from a script file.</summary>
		public static void LoadGameFonts(string assetName) {
			LoadScript(assetName, new GameFontSR());
		}

		/// <summary>Loads/compiles animations from a script file.</summary>
		public static void LoadAnimations(string assetName) {
			// TODO: Phase out separation of animations and sprites
			LoadScript(assetName, new ISpriteSR());
		}

		/// <summary>Loads/compiles collision models from a script file.</summary>
		public static void LoadCollisionModels(string assetName) {
			LoadScript(assetName, new CollisionModelSR());
		}

		/// <summary>Loads/compiles tilesets from a script file.</summary>
		public static void LoadTilesets(string assetName) {
			LoadScript(assetName, new TilesetSR());
		}

		/// <summary>Loads/compiles tile data from a script file.</summary>
		public static void LoadTiles(string assetName) {
			LoadScript(assetName, new TileDataSR());
		}

		/// <summary>Loads/compiles zones from a script file.</summary>
		/// <param name="postTileData">Set to false when loading zones right after palettes in-case of an error
		/// in order to continue previewing sprites with a specific zone.</param>
		public static void LoadZones(string assetName, bool postTileData) {
			LoadScript(assetName, new ZoneSR(postTileData));
		}

		/// <summary>Loads a sound effect.</summary>
		public static Sound LoadSound(string assetName) {
			string name = assetName.Substring(assetName.IndexOf('/') + 1);
			Sound resource = new Sound(contentManager.Load<SoundEffect>(assetName), assetName, name);
			sounds.Add(name, resource);
			return resource;
		}

		/// <summary>Loads a sound effect.</summary>
		public static Sound LoadSound(string name, string assetPath) {
			Sound resource = new Sound(contentManager.Load<SoundEffect>(assetPath), assetPath, name);
			sounds.Add(name, resource);
			return resource;
		}

		/// <summary>Loads a song.</summary>
		public static Song LoadSong(string assetName) {
			string name = assetName.Substring(assetName.IndexOf('/') + 1);
			Song resource = new Song(contentManager.Load<SoundEffect>(assetName), assetName, name);
			songs.Add(name, resource);
			return resource;
		}

		/// <summary>Loads a sounds file.</summary>
		public static void LoadSounds(string assetName) {
			LoadScript(assetName, new SoundSR());
		}

		/// <summary>Loads a music file.</summary>
		public static void LoadMusic(string assetName) {
			LoadScript(assetName, new MusicSR());
		}

		/// <summary>Loads a language file.</summary>
		public static Language LoadLanguage(string assetName) {
			throw new LoadContentException("Language is not supported yet.");
		}


		//-----------------------------------------------------------------------------
		// Scripts
		//-----------------------------------------------------------------------------

		/// <summary>Loads a script file with the given script reader object.</summary>
		public static void LoadScript(string assetName, ScriptReader sr) {
			LoadScript(assetName, sr, Encoding.Default);
		}

		/// <summary>Loads a script file with the given encoding and script reader object.</summary>
		public static void LoadScript(string assetName, ScriptReader sr, Encoding encoding) {
			try {
				string path = Path.Combine(contentManager.RootDirectory, assetName);
				using (Stream stream = TitleContainer.OpenStream(path)) {
					StreamReader reader = new StreamReader(stream, encoding);
					sr.ReadScript(reader, assetName);
				}
			}
			catch (FileNotFoundException) {
				throw new LoadContentException("Error loading file '" + assetName + "'!");
			}
		}


		//-----------------------------------------------------------------------------
		// Resource Adding.
		//-----------------------------------------------------------------------------

		/// <summary>Adds the specified animation.</summary>
		public static void AddImage(string assetName, Image image) {
			images.Add(assetName, image);
		}

		/// <summary>Adds the specified sprite sheet.</summary>
		public static void AddSpriteSheet(string assetName, SpriteSheet sheet) {
			spriteSheets.Add(assetName, sheet);
		}

		/// <summary>Adds the specified sprite.</summary>
		public static void AddSprite(string assetName, ISprite sprite) {
			sprites.Add(assetName, sprite);
			// TODO: Properly implement a way to separate animations from sprites.
			if (sprite is Animation)
				animations.Add(assetName, sprite as Animation);
		}

		/// <summary>Adds the specified game font.</summary>
		public static void AddGameFont(string assetName, GameFont font) {
			gameFonts.Add(assetName, font);
		}

		/// <summary>Adds the specified animation.</summary>
		public static void AddAnimation(string assetName, Animation animation) {
			animations.Add(assetName, animation);
		}

		/// <summary>Adds the specified palette dictionary.</summary>
		public static void AddPaletteDictionary(string assetName, PaletteDictionary dictionary) {
			paletteDictionaries.Add(assetName, dictionary);
		}

		/// <summary>Adds the specified palette.</summary>
		public static void AddPalette(string assetName, Palette palette) {
			palettes.Add(assetName, palette);
		}

		/// <summary>Adds the specified language.</summary>
		public static void AddLanguage(Language language) {
			languages.Add(language);
		}


		//-----------------------------------------------------------------------------
		// Style Previews
		//-----------------------------------------------------------------------------

		/// <summary>Gets the preview sprite for the specified style group's style.</summary>
		public static ISprite GetStylePreview(string styleGroup) {
			StyleGroupCollection collection;
			if (registeredStyles.TryGetValue(styleGroup, out collection)) {
				return collection.Preview;
			}
			return null;
		}

		/// <summary>Returns true if the specified style group is registered.</summary>
		public static bool ContainsStyleGroup(string styleGroup) {
			StyleGroupCollection collection;
			if (registeredStyles.TryGetValue(styleGroup, out collection)) {
				return true;
			}
			return false;
		}

		/// <summary>Returns true if the specified style group's style is registered.</summary>
		public static bool ContainsStyle(string styleGroup, string style) {
			StyleGroupCollection collection;
			if (registeredStyles.TryGetValue(styleGroup, out collection)) {
				return collection.Styles.Contains(style);
			}
			return false;
		}

		/// <summary>Registers the style group if it does not exist.</summary>
		public static void RegisterStyleGroup(string styleGroup, StyleSprite originalSprite) {
			if (!registeredStyles.ContainsKey(styleGroup)) {
				registeredStyles.Add(styleGroup, new StyleGroupCollection(styleGroup, originalSprite));
			}
		}

		/// <summary>Registers the style group's preview if it does not exist.</summary>
		public static void RegisterStylePreview(string styleGroup, ISprite preview) {
			StyleGroupCollection collection;
			if (registeredStyles.TryGetValue(styleGroup, out collection)) {
				if (!collection.HasPreview)
					collection.Preview = preview;
			}
			else {
				throw new Exception("Unknown style group '" + styleGroup + "'!");
			}
		}

		/// <summary>Registers the style group's style and preview sprite if it does not exist.</summary>
		public static void RegisterStyle(string styleGroup, string style) {
			StyleGroupCollection collection;
			if (registeredStyles.TryGetValue(styleGroup, out collection)) {
				if (string.IsNullOrEmpty(collection.DefaultStyle))
					collection.DefaultStyle = style;
				if (!collection.Styles.Contains(style))
					collection.Styles.Add(style);
			}
			else {
				throw new Exception("Unknown style group '" + styleGroup + "'!");
			}
		}

		/// <summary>Sets the style group's preview sprite for all styles.</summary>
		public static void SetStylePreview(string styleGroup, ISprite preview) {
			StyleGroupCollection collection;
			if (registeredStyles.TryGetValue(styleGroup, out collection)) {
				collection.Preview = preview;
			}
			else {
				throw new Exception("Unknown style group '" + styleGroup + "'!");
			}
		}

		/// <summary>Gets the collection of registered style groups, styles, and previews.</summary>
		public static IEnumerable<StyleGroupCollection> GetRegisteredStyles() {
			foreach (var pair in registeredStyles) {
				yield return pair.Value;
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the content manager.</summary>
		public static ContentManager ContentManager {
			get { return contentManager; }
		}

		/// <summary>Gets or sets if the resource manager should output load information to the console.</summary>
		public static bool VerboseOutput {
			get { return verboseOutput; }
			set { verboseOutput = value; }
		}

		/// <summary>Gets the dictionary of tilesets.</summary>
		public static Dictionary<string, Tileset> Tilesets {
			get { return tilesets; }
		}

		/// <summary>Gets the dictionary of sounds.</summary>
		public static Dictionary<string, Sound> Sounds {
			get { return sounds; }
		}

		/// <summary>Gets the list of langauges.</summary>
		public static List<Language> Languages {
			get { return languages; }
		}

		/// <summary>Gets the list of sprite sheets.</summary>
		public static ISpriteSource[] SpriteSheets {
			get {
				ISpriteSource[] sheets = new ISpriteSource[spriteSheets.Count];
				spriteSheets.Values.CopyTo(sheets, 0);
				return sheets;
			}
		}

		/// <summary>Gets the number of sprite sheets.</summary>
		public static int SpriteSheetCount {
			get { return spriteSheets.Count; }
		}

		public static GraphicsDevice GraphicsDevice {
			get { return graphicsDevice; }
		}

		public static PalettedSpriteDatabase PalettedSpriteDatabase {
			get { return palettedSpriteDatabase; }
		}

		public static bool IsLoaded {
			get { return resourceDictionaries != null; }
		}
	}
} // end namespace
