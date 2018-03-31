using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Conscripts;
using ZeldaOracle.Common.Conscripts.CustomReaders;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Translation;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Common.Content {
	/// <summary>
	/// The Resources class serves as a resource manager.
	/// It has methods to load in resources from the game
	/// content and stores them in key/value maps.
	/// </summary>
	public static class Resources {

		// CONTAINMENT:
		/// <summary>True if the resource manager has been fully initialized.</summary>
		private static bool isInitialized;
		/// <summary>The game's content manager.</summary>
		private static ContentManager contentManager;
		/// <summary>The game's graphics device.</summary>
		private static GraphicsDevice graphicsDevice;
		/// <summary>The game's sprite batch for drawing.</summary>
		private static SpriteBatch spriteBatch;
		/// <summary>True if the content manager was created by an IServiceProvider
		/// and should be disposed of when calling Uninitialize().</summary>
		private static bool disposeContent;
		/// <summary>A collection of content that need to manually be disposed of
		/// during unload.<para/>
		/// This is a hashset to prevent resources from being added continuously.</summary>
		private static HashSet<IDisposable> independentResources;
		/// <summary>A map of the resource dictionaries by resource type.</summary>
		private static Dictionary<Type, IDictionary> resourceDictionaries;

		// GRAPHICS:
		/// <summary>The collection of loaded images.</summary>
		//private static Dictionary<string, Image> images;
		/// <summary>The collection of loaded real fonts.</summary>
		//private static Dictionary<string, RealFont> realFonts;
		/// <summary>The collection of loaded game fonts.</summary>
		//private static Dictionary<string, GameFont> gameFonts;
		/// <summary>The collection of loaded sprite sheets.</summary>
		//private static Dictionary<string, ISpriteSource> spriteSheets;
		/// <summary>The collection of loaded sprites.</summary>
		//private static Dictionary<string, ISprite> sprites;
		/// <summary>The collection of loaded animations.</summary>
		//private static Dictionary<string, Animation> animations;
		/// <summary>The collection of loaded palette dictionaries.</summary>
		//private static Dictionary<string, PaletteDictionary> paletteDictionaries;
		/// <summary>The collection of loaded palettes.</summary>
		//private static Dictionary<string, Palette> palettes;
		/// <summary>The collection of loaded shaders.</summary>
		//private static Dictionary<string, Shader> shaders;

		/// <summary>The database for creating paletted sprites.</summary>
		private static PalettedSpriteDatabase spriteDatabase;

		/// <summary>The collection of loaded collision models.</summary>
		/*private static Dictionary<string, CollisionModel> collisionModels;
		private static Dictionary<string, BaseTileData> baseTileData;
		private static Dictionary<string, TileData> tileData;
		private static Dictionary<string, ActionTileData> actionTileData;
		private static Dictionary<string, Tileset> tilesets;
		private static Dictionary<string, Zone> zones;
		private static Dictionary<string, ItemData> items;
		private static Dictionary<string, AmmoData> ammos;
		private static Dictionary<string, RewardData> rewards;*/

		// SOUNDS:
		/// <summary>The collection of loaded sound effects.</summary>
		//private static Dictionary<string, Sound> sounds;
		/// <summary>The collection of loaded music.</summary>
		//private static Dictionary<string, Music> music;

		// LANGUAGES:
		/// <summary>The collection of loaded languages.</summary>
		//private static List<Language> languages;

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
		/// <summary>The directory for storing music.</summary>
		public const string MusicDirectory = "Music/";

		// Languages
		/// <summary>The directory for storing languages.</summary>
		public const string LanguageDirectory = "Languages/";

		// Items
		/// <summary>The directory for storing items, ammos, and rewards.</summary>
		public const string ItemDirectory = "Items/";


		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the resource manager class.</summary>
		static Resources() {
			isInitialized = false;
			independentResources = new HashSet<IDisposable>();
		}

		/// <summary>Sets up the resource manager with the specified
		/// GraphicDevice and IServiceProvider to use for the ContentManager.</summary>
		public static void Initialize(GraphicsDevice graphicsDevice,
			IServiceProvider serviceProvider)
		{
			// Make sure nothing is null
			if (graphicsDevice == null)
				throw new ArgumentNullException("GraphicsDevice cannot be null!");
			if (serviceProvider == null)
				throw new ArgumentNullException("IServiceProvider cannot be null!");

			Initialize(graphicsDevice, new ContentManager(serviceProvider, "Content"));
			// We made this ContentManager, so we need to dispose of it
			disposeContent = true;
		}

		/// <summary>Sets up the resource manager with the specified
		/// GraphicDevice and ContentManager.</summary>
		public static void Initialize(GraphicsDevice graphicsDevice,
			ContentManager contentManager)
		{
			// Uninitialize if we're already initialized
			if (IsInitialized)
				Uninitialize();

			// Make sure nothing is null
			if (graphicsDevice == null)
				throw new ArgumentNullException("GraphicsDevice cannot be null!");
			if (contentManager == null)
				throw new ArgumentNullException("ContentManager cannot be null!");

			// Containment
			disposeContent  = false;
			Resources.graphicsDevice	= graphicsDevice;
			Resources.spriteBatch		= new SpriteBatch(graphicsDevice);
			Resources.contentManager	= contentManager;

			// Graphics
			spriteDatabase		= new PalettedSpriteDatabase();
			/*images				= new Dictionary<string, Image>();
			realFonts			= new Dictionary<string, RealFont>();
			gameFonts			= new Dictionary<string, GameFont>();
			spriteSheets		= new Dictionary<string, ISpriteSource>();
			sprites				= new Dictionary<string, ISprite>();
			animations			= new Dictionary<string, Animation>();
			shaders				= new Dictionary<string, Shader>();
			paletteDictionaries	= new Dictionary<string, PaletteDictionary>();
			palettes			= new Dictionary<string, Palette>();
			spriteDatabase		= new PalettedSpriteDatabase();

			// Sounds
			music				= new Dictionary<string, Music>();

			// Languages
			languages			= new List<Language>();

			collisionModels		= new Dictionary<string, CollisionModel>();
			baseTileData        = new Dictionary<string, BaseTileData>();
			tileData			= new Dictionary<string, TileData>();
			actionTileData		= new Dictionary<string, ActionTileData>();
			tilesets			= new Dictionary<string, Tileset>();
			zones				= new Dictionary<string, Zone>();

			items				= new Dictionary<string, ItemData>();
			ammos				= new Dictionary<string, AmmoData>();
			rewards				= new Dictionary<string, RewardData>();

			propertyActions		= new Dictionary<string, PropertyAction>();*/
			//sounds              = new Dictionary<string, Sound>();

			// Style previews
			registeredStyles	= new Dictionary<string, StyleGroupCollection>();

			// Setup the resource dictionary lookup map.
			resourceDictionaries = new Dictionary<Type, IDictionary>();
			//resourceDictionaries.Add(typeof(Sound), sounds);

			isInitialized = true;
		}

		/// <summary>Uninitializes the resource manager and also unloads all content.</summary>
		public static void Uninitialize() {
			if (!IsInitialized)
				return;

			// Unload and dispose of all resources beforehand
			Unload();

			/*images = null;
			realFonts = null;
			gameFonts = null;
			spriteSheets = null;
			sprites = null;
			animations = null;
			shaders = null;
			sounds = null;
			music = null;
			collisionModels = null;
			baseTileData = null;
			tileData = null;
			actionTileData = null;
			tilesets = null;
			zones = null;
			items = null;
			ammos = null;
			rewards = null;
			paletteDictionaries = null;
			foreach (var pair in palettes) {
				pair.Value.Dispose();
			}
			palettes = null;*/
			resourceDictionaries = null;
			//spriteDatabase.Dispose();
			spriteDatabase = null;

			registeredStyles = null;

			if (!SpriteBatch.IsDisposed)
				spriteBatch.Dispose();
			if (disposeContent)
				contentManager.Dispose();
			graphicsDevice	= null;
			spriteBatch		= null;
			contentManager	= null;

			isInitialized = false;
		}


		//-----------------------------------------------------------------------------
		// Content Management
		//-----------------------------------------------------------------------------
		
		/// <summary>Unloads and disposes of all resources.</summary>
		public static void Unload() {
			if (!IsInitialized)
				return;

			// Clear all resource dictionaries
			/*foreach (var pair in resourceDictionaries) {
				pair.Value.Clear();
			}*/
			resourceDictionaries.Clear();

			// Clear the paletted sprite database
			spriteDatabase.Clear();

			// Clear the registered styles
			registeredStyles.Clear();

			// Unload resources loaded through the content manager
			ContentManager.Unload();

			// Dispose of all resources not loaded through the content manager
			foreach (IDisposable resource in independentResources) {
				resource.Dispose();
			}
			independentResources.Clear();
		}

		/// <summary>Adds an indenpendent resource that will be disposed
		/// of during Unload().</summary>
		public static void AddDisposable(IDisposable disposable) {
			if (!IsInitialized)
				throw new InvalidOperationException("Cannot add disposable when " +
					"Resources has not been initialized!");
			independentResources.Add(disposable);
		}


		//-----------------------------------------------------------------------------
		// Resource Type Accessors
		//-----------------------------------------------------------------------------

		// Generic --------------------------------------------------------------------

		/// <summary>Returns true if the resources contains the given type and name.</summary>
		public static bool Contains<T>(string name) {
			Dictionary<string, T> dictionary = GetDictionary<T>();
			return dictionary.ContainsKey(name);
		}

		/// <summary>Returns true if the resources contains the given type and object.</summary>
		public static bool Contains<T>(T resource) {
			Dictionary<string, T> dictionary = GetDictionary<T>();
			return dictionary.ContainsValue(resource);
		}

		/// <summary>Gets the resource with given type and name.</summary>
		public static T Get<T>(string name, bool allowEmptyNames = false) {
			if (name.Length == 0 && !allowEmptyNames)
				return default(T);
			Dictionary<string, T> dictionary = GetDictionary<T>();
			T result = default(T);
			dictionary.TryGetValue(name, out result);
			return result;
		}

		/// <summary>Gets the name of the given resource.</summary>
		public static string GetName<T>(T resource) where T : class {
			Dictionary<string, T> dictionary = GetDictionary<T>();
			if (dictionary == null)
				return "";
			return dictionary.FirstOrDefault(x => x.Value == resource).Key ?? "";
		}

		/// <summary>Returns true if resources contains a dictionary for the given
		/// type.</summary>
		public static bool ContainsType<T>() {
			return resourceDictionaries.ContainsKey(typeof(T));
		}

		/// <summary>Get the dictionary used to store the given type of resource.</summary>
		public static Dictionary<string, T> GetDictionary<T>() {
			IDictionary dictionary;
			resourceDictionaries.TryGetValue(typeof(T), out dictionary);
			if (dictionary == null) {
				dictionary = new Dictionary<string, T>();
				resourceDictionaries.Add(typeof(T), dictionary);
			}
			return (Dictionary<string, T>) dictionary;
		}

		/// <summary>Gets the list of resource keys for the given type of resource.</summary>
		public static IEnumerable<string> GetDictionaryKeys<T>() {
			Dictionary<string, T> dictionary = GetDictionary<T>();
			return dictionary.Keys;
		}

		// Type -----------------------------------------------------------------------

		/// <summary>Returns true if the resources contains the given type and name.</summary>
		public static bool Contains(Type type, string name) {
			IDictionary dictionary = GetDictionary(type);
			return dictionary.Contains(name);
		}

		/// <summary>Returns true if the resources contains the given type and object.</summary>
		public static bool Contains(Type type, object resource) {
			IDictionary dictionary = GetDictionary(type);
			if (dictionary == null)
				return false;
			foreach (DictionaryEntry entry in dictionary.Values) {
				if (entry.Value == resource)
					return true;
			}
			return false;
		}

		/// <summary>Gets the resource with given type and name.</summary>
		public static object Get(Type type, string name, bool allowEmptyNames = false) {
			if (name.Length == 0 && !allowEmptyNames)
				return TypeHelper.GetDefaultValue(type);
			IDictionary dictionary = GetDictionary(type);
			if (dictionary.Contains(name))
				return dictionary[name];
			return TypeHelper.GetDefaultValue(type);
		}

		/// <summary>Gets the name of the given resource.</summary>
		public static string GetName(Type type, object resource) {
			IDictionary dictionary = GetDictionary(type);
			if (dictionary == null)
				return "";
			foreach (DictionaryEntry entry in dictionary.Values) {
				if (entry.Value == resource)
					return (string) entry.Key;
			}
			return "";
		}

		/// <summary>Returns true if resources contains a dictionary for the given
		/// type.</summary>
		public static bool ContainsType(Type type) {
			return resourceDictionaries.ContainsKey(type);
		}

		/// <summary>Get the dictionary used to store the given type of resources.</summary>
		public static IDictionary GetDictionary(Type type) {
			IDictionary dictionary;
			resourceDictionaries.TryGetValue(type, out dictionary);
			if (dictionary == null) {
				Type newType = typeof(Dictionary<,>)
					.MakeGenericType(typeof(string), type);
				dictionary = ReflectionHelper.Construct<IDictionary>(newType);
				resourceDictionaries.Add(type, dictionary);
			}
			return dictionary;
		}

		/// <summary>Gets the list of resource keys for the given type of resource.</summary>
		public static IEnumerable<string> GetDictionaryKeys(Type type) {
			IDictionary dictionary;
			if (resourceDictionaries.TryGetValue(type, out dictionary)) {
				foreach (object key in dictionary.Keys) {
					yield return (string) key;
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Resource Type Mutators
		//-----------------------------------------------------------------------------

		// Generic --------------------------------------------------------------------

		/// <summary>Add the given resource under the given name.</summary>
		public static void Add<T>(string assetName, T resource) {
			Dictionary<string, T> dictionary = GetDictionary<T>();
			dictionary.Add(assetName, resource);
		}

		/// <summary>Add the given resource under the given name.</summary>
		public static void Set<T>(string assetName, T resource) {
			Dictionary<string, T> dictionary = GetDictionary<T>();
			dictionary[assetName] = resource;
		}

		/// <summary>Remove the resource under the given name from the database.</summary>
		public static void Remove<T>(string assetName) {
			Dictionary<string, T> dictionary = GetDictionary<T>();
			dictionary.Remove(assetName);
		}

		/// <summary>Clears the resource dictionary for the given resource type.</summary>
		public static void ClearDictionary<T>() {
			IDictionary dictionary = GetDictionary<T>();
			dictionary.Clear();
		}

		// Type -----------------------------------------------------------------------

		/// <summary>Add the given resource under the given name.</summary>
		public static void Add(Type type, string assetName, object resource) {
			IDictionary dictionary = GetDictionary(type);
			dictionary.Add(assetName, resource);
		}

		/// <summary>Add the given resource under the given name.</summary>
		public static void Set(Type type, string assetName, object resource) {
			IDictionary dictionary = GetDictionary(type);
			dictionary[assetName] = resource;
		}

		/// <summary>Remove the resource under the given name from the database.</summary>
		public static void Remove(Type type, string assetName) {
			IDictionary dictionary = GetDictionary(type);
			dictionary.Remove(assetName);
		}

		/// <summary>Clears the resource dictionary for the specified resource type.</summary>
		public static void ClearDictionary(Type type) {
			IDictionary dictionary = GetDictionary(type);
			dictionary.Clear();
		}


		//-----------------------------------------------------------------------------
		// Resource Loading
		//-----------------------------------------------------------------------------

		/// <summary>Loads the image with the specified asset name.</summary>
		public static Image LoadImage(string assetName) {
			string name = assetName.Substring(assetName.IndexOf('/') + 1);
			Image resource = Image.FromContent(assetName);
			Add<Image>(name, resource);
			return resource;
		}

		/// <summary>Loads the image with the specified asset name from file.</summary>
		public static Image LoadImageFromFile(string assetPath) {
			string name = assetPath.Substring(assetPath.IndexOf('/') + 1);
			name = name.Substring(0, name.LastIndexOf('.'));
			Image resource = Image.FromFile(assetPath);
			Add<Image>(name, resource);
			return resource;
		}

		/// <summary>Loads the real font with the specified asset name.</summary>
		public static RealFont LoadRealFont(string assetName) {
			string name = assetName.Substring(assetName.IndexOf('/') + 1);
			RealFont resource = new RealFont(
				contentManager.Load<SpriteFont>(assetName), name);
			Add<RealFont>(name, resource);
			return resource;
		}

		/// <summary>Loads a shader (effect).</summary>
		public static Shader LoadShader(string assetName) {
			string name = assetName.Substring(assetName.IndexOf('/') + 1);
			Shader resource = Shader.FromContent(assetName);
			Add<Shader>(name, resource);
			return resource;
		}

		/// <summary>Loads a shader (effect) with the specified type.</summary>
		public static TShader LoadShader<TShader>(string assetName)
			where TShader : Shader
		{
			string name = assetName.Substring(assetName.IndexOf('/') + 1);
			TShader resource = Shader.FromContent<TShader>(assetName);
			Add<Shader>(name, resource);
			return resource;
		}

		/// <summary>Loads a sound effect.</summary>
		public static Sound LoadSound(string assetName) {
			string name = assetName.Substring(assetName.IndexOf('/') + 1);
			Sound resource = new Sound(contentManager.Load<SoundEffect>(assetName),
				assetName, name);
			Add<Sound>(name, resource);
			return resource;
		}

		/// <summary>Loads a sound effect.</summary>
		public static Sound LoadSound(string name, string assetPath) {
			Sound resource = new Sound(contentManager.Load<SoundEffect>(assetPath),
				assetPath, name);
			Add<Sound>(name, resource);
			return resource;
		}

		/// <summary>Loads a song.</summary>
		public static Music LoadSong(string assetName) {
			string name = assetName.Substring(assetName.IndexOf('/') + 1);
			Music resource = new Music(contentManager.Load<SoundEffect>(assetName),
				assetName, name);
			Add<Music>(name, resource);
			return resource;
		}

		/// <summary>Loads a language file.</summary>
		public static Language LoadLanguage(string assetName) {
			throw new LoadContentException("Language is not supported yet.");
		}

		//-----------------------------------------------------------------------------
		// Type Conscripts
		//-----------------------------------------------------------------------------
		
		/// <summary>Loads/compiles palette dictionaries from a script file.</summary>
		public static void LoadPaletteDictionaries(string assetName) {
			LoadScript(assetName, new PaletteDictionarySR());
		}

		/// <summary>Loads/compiles palettes from a script file.</summary>
		public static void LoadPalettes(string assetName) {
			LoadScript(assetName, new PaletteSR());
		}

		/// <summary>Loads/compiles images from a script file.</summary>
		public static void LoadImages(string assetName) {
			LoadScript(assetName, new ImageSR());
		}

		/// <summary>Loads/compiles sprites from a script file.</summary>
		public static void LoadSprites(string assetName) {
			LoadScript(assetName, new SpriteSR());
		}

		/// <summary>Loads/compiles game fonts from a script file.</summary>
		public static void LoadGameFonts(string assetName) {
			LoadScript(assetName, new GameFontSR());
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

		/// <summary>Loads/compiles items and ammos from a script file.</summary>
		public static void LoadItems(string assetName) {
			LoadScript(assetName, new ItemSR());
		}

		/// <summary>Loads/compiles items and rewards from a script file.</summary>
		public static void LoadRewards(string assetName) {
			LoadScript(assetName, new RewardSR());
		}

		/// <summary>Loads/compiles zones from a script file.</summary>
		/// <param name="postTileData">Set to false when loading zones right after
		/// palettes in-case of an error in order to continue previewing sprites
		/// with a specific zone.</param>
		public static void LoadZones(string assetName, bool postTileData) {
			LoadScript(assetName, new ZoneSR(postTileData));
		}

		/// <summary>Loads a sounds file.</summary>
		public static void LoadSounds(string assetName) {
			LoadScript(assetName, new SoundSR());
		}

		/// <summary>Loads a music file.</summary>
		public static void LoadMusic(string assetName) {
			LoadScript(assetName, new MusicSR());
		}


		//-----------------------------------------------------------------------------
		// Scripts
		//-----------------------------------------------------------------------------

		/// <summary>Loads a script file with the given script reader object.</summary>
		public static void LoadScript(string assetName, ConscriptRunner runner) {
			try {
				string path = Path.Combine(contentManager.RootDirectory, assetName);
				using (Stream stream = TitleContainer.OpenStream(path)) {
					ScriptReader scriptReader = new ScriptReader();
					scriptReader.ReadScript(runner, stream, assetName);
				}
			}
			catch (FileNotFoundException) {
				throw new LoadContentException("Error loading file '" + assetName +
					"'!");
			}
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
		public static void RegisterStyleGroup(string styleGroup,
			StyleSprite originalSprite)
		{
			if (!registeredStyles.ContainsKey(styleGroup)) {
				registeredStyles.Add(styleGroup,
					new StyleGroupCollection(styleGroup, originalSprite));
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

		/// <summary>Registers the style group's style and preview sprite if it does
		/// not exist.</summary>
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

		/// <summary>Gets the collection of registered style groups, styles, and
		/// previews.</summary>
		public static IEnumerable<StyleGroupCollection> GetRegisteredStyles() {
			foreach (var pair in registeredStyles) {
				yield return pair.Value;
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets the graphics device for the game.</summary>
		public static GraphicsDevice GraphicsDevice {
			get { return graphicsDevice; }
		}

		/// <summary>Gets the sprite batch for the game.</summary>
		public static SpriteBatch SpriteBatch {
			get { return spriteBatch; }
		}

		/// <summary>Gets the content manager for the game.</summary>
		public static ContentManager ContentManager {
			get { return contentManager; }
		}

		/// <summary>Gets if the resource manager has been fully been initialized.</summary>
		public static bool IsInitialized {
			get { return isInitialized; }
		}

		/// <summary>Gets or sets the root directory associated with the
		/// ContentManager.</summary>
		public static string RootDirectory {
			get { return ContentManager.RootDirectory; }
			set { ContentManager.RootDirectory = value; }
		}

		/// <summary>Gets the sprite database for palettizing images.</summary>
		public static PalettedSpriteDatabase SpriteDatabase {
			get { return spriteDatabase; }
		}
	}
}
