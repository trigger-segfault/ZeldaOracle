using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.EventTiles;

namespace ZeldaOracle.Game.Worlds {

	public class WorldFileException : Exception {
		public WorldFileException(string message) :
			base("")
		{
		}
	}

	public class ResourceInfo<T> {
		private T resource;
		private int nameStringIndex;

		public ResourceInfo(T resource, int stringIndex) {
			this.resource = resource;
			this.nameStringIndex = stringIndex;
		}

		public T Resource {
			get { return resource; }
			set { resource = value; }
		}

		public int StringIndex {
			get { return nameStringIndex; }
			set { nameStringIndex = value; }
		}
	}

	// Used to save and load world files.
	public class WorldFile {
		
		private static char[] MAGIC = { 'Z', 'w', 'd', '2' };
		private const int WORLDFILE_VERSION = 2;

		private string fileName;
		private int version;

		private List<string>						strings;
		private List<ResourceInfo<Zone>>			zones;
		private List<ResourceInfo<Tileset>>			tilesets;
		private List<ResourceInfo<Type>>			tileTypes;
		private List<ResourceInfo<CollisionModel>>	collisionModels;
		private List<ResourceInfo<Sprite>>			sprites;
		private List<ResourceInfo<Animation>>		animations;
		private List<ResourceInfo<TileData>>		tileData;
		private List<ResourceInfo<EventTileData>>	eventTileData;
		
		private string errorMessage;


		//private int[]			zones;
		
		private static bool VerifyMagic(char[] magic) {
			if (magic.Length != MAGIC.Length)
				return false;
			for (int i = 0; i < MAGIC.Length; i++) {
				if (magic[i] != MAGIC[i])
					return false;
			}
			return true;
		}


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public WorldFile() {
			strings			= new List<string>();
			tileTypes		= new List<ResourceInfo<Type>>();
			zones			= new List<ResourceInfo<Zone>>();
			tilesets		= new List<ResourceInfo<Tileset>>();
			collisionModels	= new List<ResourceInfo<CollisionModel>>();
			sprites			= new List<ResourceInfo<Sprite>>();
			animations		= new List<ResourceInfo<Animation>>();
			tileData		= new List<ResourceInfo<TileData>>();
			eventTileData	= new List<ResourceInfo<EventTileData>>();
		}


		//-----------------------------------------------------------------------------
		// Reset
		//-----------------------------------------------------------------------------

		private void Clear() {
			strings.Clear();
			tileTypes.Clear();
			zones.Clear();
			tilesets.Clear();
			collisionModels.Clear();
			sprites.Clear();
			animations.Clear();
			tileData.Clear();
			eventTileData.Clear();
		}

		//-----------------------------------------------------------------------------
		// Loading
		//-----------------------------------------------------------------------------

		public World Load(string fileName) {
			this.fileName = fileName;
			BinaryReader reader = new BinaryReader(File.OpenRead(fileName));
			World world = Load(reader);
			reader.Close();
			return world;
		}

		private World Load(BinaryReader reader) {
			Clear();
			
			World world = null;

			try {
				world = new World();

				// Read the header.
				ReadHeader(reader, world);
				// Read the string list.
				ReadStrings(reader);
				// Read resource lists.
				ReadTileTypes(reader);
				ReadResourceList(reader, zones);
				ReadResourceList(reader, tilesets);
				ReadResourceList(reader, collisionModels);
				ReadResourceList(reader, sprites);
				ReadResourceList(reader, animations);
				ReadResourceList(reader, tileData);
				ReadResourceList(reader, eventTileData);
				if (version == 2)
					ReadScripts(reader, world);

				// Read the world data.
				ReadWorld(reader, world);

				// TEMP: Trigger property actions on tiles.
				for (int i = 0; i < world.LevelCount; i++) {
					Level level = world.Levels[i];
					for (int x = 0; x < level.Width; x++) {
						for (int y = 0; y < level.Height; y++) {
							Room room = level.GetRoomAt(x, y);
							room.IterateTiles(delegate(TileDataInstance tile) {
								tile.Properties.RunActionForAll();
							});
							room.IterateEventTiles(delegate(EventTileDataInstance tile) {
								tile.Properties.RunActionForAll();
							});
						}
					}
				}
			}
			catch (WorldFileException e) {
				Console.WriteLine("Error loading world: " + e.Message);
				return null;
			}

			return world;
		}
		
		private void ReadHeader(BinaryReader reader, World world) {
			// 4 bytes: Read and verify the magic number.
			char[] magic = reader.ReadChars(4);
			if (!VerifyMagic(magic))
				ThrowWorldFileException("Invalid file type");

			// 4 bytes: Read the version number.
			version = reader.ReadInt32();
			
			// Read the player starting position.
			world.StartLevelIndex = reader.ReadInt32();
			world.StartRoomLocation = new Point2I(
					reader.ReadInt32(),
					reader.ReadInt32());
			world.StartTileLocation = new Point2I(
					reader.ReadInt32(),
					reader.ReadInt32());
		}

		private void ReadWorld(BinaryReader reader, World world) {
			// Read the world's properties.
			world.Properties.Merge(ReadProperties(reader), true);
			
			// Read the world's levels.
			int levelCount = reader.ReadInt32();
			for (int i = 0; i < levelCount; i++) {
				Level level = ReadLevel(reader);
				world.Levels.Add(level);
			}
		}

		private Level ReadLevel(BinaryReader reader) {
			// Read the level dimensions.
			int width			= reader.ReadInt32();
			int height			= reader.ReadInt32();
			int roomWidth		= reader.ReadInt32();
			int roomHeight		= reader.ReadInt32();
			int roomLayerCount	= reader.ReadInt32();
			Level level = new Level(width, height, new Point2I(roomWidth, roomHeight));
			level.RoomLayerCount = roomLayerCount;

			// Read the level's properties.
			level.Properties.Merge(ReadProperties(reader), true);
			
			// Read all the rooms in the level.
			for (int y = 0; y < level.Height; y++) {
				for (int x = 0; x < level.Width; x++) {
					Room room = new Room(level, x, y);
					ReadRoom(reader, room);
					level.Rooms[x, y] = room;
				}
			}

			return level;
		}
		
		private void ReadRoom(BinaryReader reader, Room room) {
			// Read the dimensions.
			int width  = reader.ReadInt32();
			int height = reader.ReadInt32();
			int layerCount = reader.ReadInt32();

			// Read the zone.
			room.Zone = ReadResource(reader, zones);

			// Read the room's properties.
			room.Properties.Merge(ReadProperties(reader), true);
			
			// Read tile data for first layer (stored as a grid of tiles).
			for (int y = 0; y < room.Height; y++) {
				for (int x = 0; x < room.Width; x++) {
					room.SetTile(ReadTileData(reader), x, y, 0);
				}
			}

			// Read tile data for higher layers (stored as a list of non-null tiles).
			int tileDataCount = reader.ReadInt32();
			for (int i = 0; i < tileDataCount; i++) {
				int x		= reader.ReadInt32();
				int y		= reader.ReadInt32();
				int layer	= reader.ReadInt32();
				room.SetTile(ReadTileData(reader), x, y, layer);
			}

			// Read event tile data.
			int eventTileDataCount = reader.ReadInt32();
			for (int i = 0; i < eventTileDataCount; i++) {
				room.AddEventTile(ReadEventTileData(reader));
			}
		}

		private TileDataInstance ReadTileData(BinaryReader reader) {
			int tilesetIndex = reader.ReadInt32();
			if (tilesetIndex == -11) // -11 indicates a null tile.
				return null;
			
			TileDataInstance tile = new TileDataInstance();
			Tileset tileset = null;
			if (tilesetIndex >= 0)
				tileset = tilesets[tilesetIndex].Resource;

			if (tileset != null) {
				// Create tile from a tileset.
				Point2I sheetLocation = new Point2I(
					reader.ReadInt32(),
					reader.ReadInt32());
				tile.TileData = tileset.TileData[sheetLocation.X, sheetLocation.Y];
				tile.Properties = ReadProperties(reader);
				tile.Properties.BaseProperties = tile.TileData.Properties;
			}
			else {
				// Create tile from a TileData resource.
				tile.TileData = ReadResource(reader, tileData);
				tile.Properties = ReadProperties(reader);
				tile.Properties.BaseProperties = tile.TileData.Properties;
			}

			return tile;
		}

		private EventTileDataInstance ReadEventTileData(BinaryReader reader) {
			EventTileData tileData = ReadResource(reader, eventTileData);
			Point2I position = new Point2I(
				reader.ReadInt32(),
				reader.ReadInt32());

			EventTileDataInstance eventTile = new EventTileDataInstance(tileData, position);
			eventTile.Properties = ReadProperties(reader);
			eventTile.Properties.BaseProperties = eventTile.EventTileData.Properties;

			return eventTile;
		}

		private Properties ReadProperties(BinaryReader reader) {
			int count = reader.ReadInt32();
			Properties properties = new Properties();
			for (int i = 0; i < count; i++) {
				Property property = ReadProperty(reader);
				properties.Add(property);
			}
			return properties;
		}

		private Property ReadProperty(BinaryReader reader) {
			PropertyType type = (PropertyType) reader.ReadInt32();
			string name = ReadString(reader);

			if (type == PropertyType.Integer) {
				return Property.CreateInt(name, reader.ReadInt32());
			}
			else if (type == PropertyType.Float) {
				return Property.CreateFloat(name, reader.ReadSingle());
			}
			else if (type == PropertyType.Boolean) {
				return Property.CreateBool(name, reader.ReadBoolean());
			}
			else if (type == PropertyType.String) {
				return Property.CreateString(name, ReadString(reader));
			}
			else if (type == PropertyType.List) {
				int count = reader.ReadInt32();
				Property list = Property.CreateList(name);
				Property child = null;

				for (int i = 0; i < count; i++) {
					Property newChild = ReadProperty(reader);

					if (child != null)
						child.Next = newChild;
					else
						list.FirstChild = newChild;
					child = newChild;
				}

				list.Count = count;
				return list;
			}
			return null;
		}

		private string ReadString(BinaryReader reader) {
			int index = reader.ReadInt32();
			return strings[index];
		}

		private T ReadResource<T>(BinaryReader reader, List<ResourceInfo<T>> list) where T : class {
			int index = reader.ReadInt32();
			if (index < 0)
				return null;
			return list[index].Resource;
		}

		private Type ReadTileType(BinaryReader reader) {
			int index = reader.ReadInt32();
			if (index < 0)
				return null;
			return tileTypes[index].Resource;
		}

		private int[] ReadTable(BinaryReader reader)  {
			int count = reader.ReadInt32();
			int[] table = new int[count];
			for (int i = 0; i < count; i++)
				table[i] = reader.ReadInt32();
			return table;
		}

		private void ReadStrings(BinaryReader reader)  {
			int stringCount = reader.ReadInt32();
			strings.Capacity = stringCount;
			for (int i = 0; i < stringCount; i++)
				strings.Add(reader.ReadString());
		}

		private void ReadTileTypes(BinaryReader reader)  {
			int typeCount = reader.ReadInt32();
			tileTypes.Capacity = typeCount;

			for (int i = 0; i < typeCount; i++) {
				int index	= reader.ReadInt32();
				string name	= strings[index];
				Type type	= Tile.GetType(name, false);
				tileTypes.Add(new ResourceInfo<Type>(type, index));
			}
		}
		
		private void ReadResourceList<T>(BinaryReader reader, List<ResourceInfo<T>> list) where T : class {
			int count = reader.ReadInt32();
			list.Capacity = count;

			for (int i = 0; i < count; i++) {
				int index = reader.ReadInt32();
				string name = strings[index];
				T resource = Resources.GetResource<T>(name);
				list.Add(new ResourceInfo<T>(resource, index));
			}
		}
		

		//-----------------------------------------------------------------------------
		// Saving
		//-----------------------------------------------------------------------------
		
		public void Save(string fileName, World world) {
			this.fileName = fileName;
			FileStream fileStream = new FileStream(fileName, FileMode.Create);
			BinaryWriter writer = new BinaryWriter(fileStream);
			Save(writer, world);
			writer.Close();
		}

		private void Save(BinaryWriter writer, World world) {
			Clear();

			// Write the level data to memory.
			MemoryStream worldDataStream = new MemoryStream();
			BinaryWriter worldDataWriter = new BinaryWriter(worldDataStream);
			WriteScripts(worldDataWriter, world);
			WriteWorld(worldDataWriter, world);
			byte[] worldData = worldDataStream.GetBuffer();
			int levelDataSize = (int) worldDataStream.Length;
			worldDataWriter.Close();
			
			// Write the header.
			WriteHeader(writer, world);
			// Write the strings list.
			WriteStrings(writer);
			// Write the tileset list.
			WriteResourceList(writer, tileTypes);
			WriteResourceList(writer, zones);
			WriteResourceList(writer, tilesets);
			WriteResourceList(writer, collisionModels);
			WriteResourceList(writer, sprites);
			WriteResourceList(writer, animations);
			WriteResourceList(writer, tileData);
			WriteResourceList(writer, eventTileData);

			// Write the world data.
			writer.Write(worldData, 0, levelDataSize);
		}

		private void WriteHeader(BinaryWriter writer, World world) {
			writer.Write(MAGIC);	// Magic 4 bytes "Zwd2"
			writer.Write(WORLDFILE_VERSION);	// WorldFile version number.

			writer.Write(world.StartLevelIndex);
			writer.Write(world.StartRoomLocation.X);
			writer.Write(world.StartRoomLocation.Y);
			writer.Write(world.StartTileLocation.X);
			writer.Write(world.StartTileLocation.Y);
		}
		
		private void WriteScripts(BinaryWriter writer, World world) {
			// Write the compiled assembly.
			byte[] rawAssembly = world.ScriptManager.RawAssembly;
			if (rawAssembly != null) {
				writer.Write(world.ScriptManager.RawAssembly.Length);
				writer.Write(world.ScriptManager.RawAssembly);
			}
			else
				writer.Write((int) 0);

			// Write the individual scripts.
			writer.Write(world.Scripts.Count);
			foreach (KeyValuePair<string, Script> script in world.Scripts)
				WriteScript(writer, script.Value);
		}
		
		private void WriteScript(BinaryWriter writer, Script script) {
			// Write the name and source code.
			WriteString(writer, script.Name);
			WriteString(writer, script.Code);
			writer.Write(script.IsHidden);
			
			// Write the parameters.
			writer.Write(script.Parameters.Count);
			for (int i = 0; i < script.Parameters.Count; i++) {
				WriteString(writer, script.Parameters[i].Type);
				WriteString(writer, script.Parameters[i].Name);
			}
			
			// Write the errors and warnings.
			writer.Write(script.Errors.Count);
			for (int i = 0; i < script.Errors.Count; i++) {
				writer.Write(script.Errors[i].Line);
				writer.Write(script.Errors[i].Column);
				WriteString(writer, script.Errors[i].ErrorNumber);
				WriteString(writer, script.Errors[i].ErrorText);
			}
			writer.Write(script.Warnings.Count);
			for (int i = 0; i < script.Warnings.Count; i++) {
				writer.Write(script.Warnings[i].Line);
				writer.Write(script.Warnings[i].Column);
				WriteString(writer, script.Warnings[i].ErrorNumber);
				WriteString(writer, script.Warnings[i].ErrorText);
			}
		}
				
		private void ReadScripts(BinaryReader reader, World world) {
			// Read the raw assembly.
			int byteCount = reader.ReadInt32();
			if (byteCount > 0)
				world.ScriptManager.RawAssembly = reader.ReadBytes(byteCount);
			else
				world.ScriptManager.RawAssembly = null;

			// Read the individual scripts.
			int count = reader.ReadInt32();
			for (int i = 0; i < count; i++)
				world.AddScript(ReadScript(reader));
		}
		
		private Script ReadScript(BinaryReader reader) {
			Script script = new Script();

			// Read the name and source code.
			script.Name		= ReadString(reader);
			script.Code		= ReadString(reader);
			script.IsHidden	= reader.ReadBoolean();
			
			// Read the parameters.
			int paramCount = reader.ReadInt32();
			for (int i = 0; i < paramCount; i++) {
				ScriptParameter param = new ScriptParameter();
				param.Type = ReadString(reader);
				param.Name = ReadString(reader);
				script.Parameters.Add(param);
			}
			
			// Read errors and warnings.
			int errorCount = reader.ReadInt32();
			for (int i = 0; i < errorCount; i++) {
				ScriptCompileError error = new ScriptCompileError();
				error.Line			= reader.ReadInt32();
				error.Column		= reader.ReadInt32();
				error.ErrorNumber	= ReadString(reader);
				error.ErrorText		= ReadString(reader);
				error.IsWarning		= true;
				script.Errors.Add(error);
			}
			int warningCount = reader.ReadInt32();
			for (int i = 0; i < warningCount; i++) {
				ScriptCompileError warning = new ScriptCompileError();
				warning.Line		= reader.ReadInt32();
				warning.Column		= reader.ReadInt32();
				warning.ErrorNumber	= ReadString(reader);
				warning.ErrorText	= ReadString(reader);
				warning.IsWarning	= true;
				script.Warnings.Add(warning);
			}

			return script;
		}

		private void WriteWorld(BinaryWriter writer, World world) {
			// Write the world's properties.
			WriteProperties(writer, world.Properties);
			
			// Write the level data.
			writer.Write(world.Levels.Count);
			for (int i = 0; i < world.Levels.Count; i++)
				WriteLevel(writer, world.Levels[i]);
		}

		private void WriteLevel(BinaryWriter writer, Level level) {
			//WriteString(writer, level.Name);
			writer.Write(level.Width);
			writer.Write(level.Height);
			writer.Write(level.RoomWidth);
			writer.Write(level.RoomHeight);
			writer.Write(level.RoomLayerCount);
			WriteProperties(writer, level.Properties);

			// Write rooms.
			for (int y = 0; y < level.Height; y++) {
				for (int x = 0; x < level.Width; x++) {
					Room room = level.GetRoomAt(x, y);
					WriteRoom(writer, room);
				}
			}
		}

		private void WriteRoom(BinaryWriter writer, Room room) {
			writer.Write(room.Width);
			writer.Write(room.Height);
			writer.Write(room.LayerCount);
			WriteResource(writer, room.Zone, zones);
			WriteProperties(writer, room.Properties);

			// Write all tiles for the first tile layer.
			for (int y = 0; y < room.Height; y++) {
				for (int x = 0; x < room.Width; x++) {
					TileDataInstance tile = room.GetTile(x, y, 0);
					if (tile != null)
						WriteTileData(writer, tile);
					else
						writer.Write(-11); // -11 signifies a null tile.
				}
			}
			
			// Count non-null tile data for higher layers.
			int tileDataCount = 0;
			for (int i = 1; i < room.LayerCount; i++) {
				for (int y = 0; y < room.Height; y++) {
					for (int x = 0; x < room.Width; x++) {
						TileDataInstance tile = room.GetTile(x, y, i);
						if (tile != null)
							tileDataCount++;
					}
				}
			}

			// Write non-null tile data for higher layers.
			writer.Write(tileDataCount);
			for (int i = 1; i < room.LayerCount; i++) {
				for (int y = 0; y < room.Height; y++) {
					for (int x = 0; x < room.Width; x++) {
						TileDataInstance tile = room.GetTile(x, y, i);
						if (tile != null) {
							writer.Write(x);
							writer.Write(y);
							writer.Write(i);
							WriteTileData(writer, tile);
						}
					}
				}
			}

			// Write event tile data.
			writer.Write(room.EventData.Count);
			for (int i = 0; i < room.EventData.Count; i++) {
				EventTileDataInstance eventTile = room.EventData[i];
				WriteEventTileData(writer, eventTile);
			}
		}

		private void WriteTileData(BinaryWriter writer, TileDataInstance tile) {
			if (tile.Tileset != null) {
				//WriteTileset(writer, tileData.Tileset);
				WriteResource(writer, tile.Tileset, tilesets);
				writer.Write((int) tile.SheetLocation.X);
				writer.Write((int) tile.SheetLocation.Y);
				WriteProperties(writer, tile.Properties);
			}
			else {
				writer.Write((int) -1);
				WriteResource(writer, tile.TileData, tileData);
				WriteProperties(writer, tile.Properties);
				//writer.Write((long) tileData.Flags);
				//WriteTileType(writer, tileData.Type);
				//WriteResource(writer, tileData.CollisionModel, collisionModels);
				//WriteResource(writer, tileData.Sprite, sprites);
				//WriteResource(writer, tileData.SpriteAsObject, sprites);
				//WriteResource(writer, tileData.Animation, animations);
				//WriteResource(writer, tileData.BreakAnimation, animations);
				//WriteProperties(writer, tile.ModifiedProperties);
			}
		}

		private void WriteEventTileData(BinaryWriter writer, EventTileDataInstance eventTile) {
			WriteResource(writer, eventTile.EventTileData, eventTileData);
			writer.Write(eventTile.Position.X);
			writer.Write(eventTile.Position.Y);
			WriteProperties(writer, eventTile.Properties);
		}

		private void WriteProperties(BinaryWriter writer, Properties properties) {
			writer.Write((int) properties.PropertyMap.Count);
			foreach (Property property in properties.PropertyMap.Values) {
				WriteProperty(writer, property);
			}
		}

		private void WriteProperty(BinaryWriter writer, Property property) {
			writer.Write((int) property.Type);
			WriteString(writer, property.Name);

			if (property.Type == PropertyType.Integer) {
				writer.Write(property.IntValue);
			}
			else if (property.Type == PropertyType.Float) {
				writer.Write(property.FloatValue);
			}
			else if (property.Type == PropertyType.Boolean) {
				writer.Write(property.BoolValue);
			}
			else if (property.Type == PropertyType.String) {
				WriteString(writer, property.StringValue);
			}
			else if (property.Type == PropertyType.List) {
				writer.Write((int) property.Count);
				property = property.FirstChild;
				while (property != null) {
					WriteProperty(writer, property);
					property = property.Next;
				}
			}
		}
		
		private void WriteStrings(BinaryWriter writer) {
			writer.Write((int) strings.Count);
			for (int i = 0; i < strings.Count; i++)
				writer.Write(strings[i]);
		}
		
		private void WriteResourceList<T>(BinaryWriter writer, List<ResourceInfo<T>> list) {
			writer.Write((int) list.Count);
			for (int i = 0; i < list.Count; i++)
				writer.Write((int) list[i].StringIndex);
		}

		private int WriteString(BinaryWriter writer, string str) {
			return WriteObject(writer, str, strings);
		}

		private void WriteResource<T>(BinaryWriter writer, T resource, List<ResourceInfo<T>> list) where T : class {
			// Write an index of -1 for null resources.
			if (resource == null) {
				writer.Write((int) -1);
				return;
			}

			// Find the resource in the resource list.
			int index = -1;
			for (int i = 0; i < list.Count; i++) {
				if (list[i].Resource == resource) {
					index = i;
					break;
				}
			}

			// Create a new entry in the resource list.
			if (index < 0) {
				Dictionary<string, T> resourceList = Resources.GetResourceDictionary<T>();
				string name = resourceList.FirstOrDefault(x => x.Value == resource).Key;
				index = list.Count;
				list.Add(new ResourceInfo<T>(resource, strings.Count));
				strings.Add(name);
			}

			writer.Write(index);
		}

		private int WriteObject<T>(BinaryWriter writer, T obj, List<T> objList) where T : class {
			int index = 0;
			for (int i = 0; i < objList.Count; i++) {
				if (objList[i] == obj) {
					index = i;
					break;
				}
			}
			index = objList.Count;
			objList.Add(obj);
			writer.Write(index);
			return index;
		}

		private int WriteTileType(BinaryWriter writer, Type tileType) {
			if (tileType == null) {
				writer.Write((int) -1);
				return -1;
			}
			int index = -1;
			for (int i = 0; i < tileTypes.Count; i++) {
				if (tileTypes[i].Resource == tileType) {
					index = i;
					break;
				}
			}
			if (index < 0) {
				index = tileTypes.Count;
				tileTypes.Add(new ResourceInfo<Type>(tileType, strings.Count));
				strings.Add(tileType.Name);
			}
			writer.Write(index);
			return index;
		}

		
		//-----------------------------------------------------------------------------
		// Errors
		//-----------------------------------------------------------------------------

		public void ThrowWorldFileException(string message) {
			this.errorMessage = message;
			throw new WorldFileException(message);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public string ErrorMessage {
			get { return errorMessage; }
		}
	}
}
