using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.API;
using ZeldaOracle.Common.Util;

namespace ZeldaOracle.Game.Worlds {

	public class LocateResourceEventArgs : EventArgs {
		// In
		public Type Type { get; }
		public string OldName { get; }

		// Out
		public string NewName { get; set; }
		public bool SkipRemaining { get; set; }


		public LocateResourceEventArgs(Type type, string name) {
			this.Type			= type;
			this.OldName		= name;

			this.NewName		= null;
			this.SkipRemaining	= false;
		}
	}

	public delegate void LocateResourceEventHandler(LocateResourceEventArgs e);

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
		private const int WORLDFILE_VERSION = 4;

		private const int NULL_TILE = -11;

		private string fileName;
		private int version;
		private bool editorMode; // If set, event scripts will go back to being stored by the object itself

		private List<string>						strings;
		private List<ResourceInfo<Zone>>			zones;
		private List<ResourceInfo<Tileset>>			tilesets;
		private List<ResourceInfo<Type>>			tileTypes;
		private List<ResourceInfo<CollisionModel>>	collisionModels;
		private List<ResourceInfo<ISprite>>			sprites;
		private List<ResourceInfo<Animation>>		animations;
		private List<ResourceInfo<TileData>>		tileData;
		private List<ResourceInfo<ActionTileData>>	actionTileData;
		
		private string errorMessage;
		private Exception exception;
		private bool skipRemainingResources;


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
		// Events
		//-----------------------------------------------------------------------------

		public event LocateResourceEventHandler LocateResource;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public WorldFile() {
			strings			= new List<string>();
			tileTypes		= new List<ResourceInfo<Type>>();
			zones			= new List<ResourceInfo<Zone>>();
			tilesets		= new List<ResourceInfo<Tileset>>();
			collisionModels	= new List<ResourceInfo<CollisionModel>>();
			sprites			= new List<ResourceInfo<ISprite>>();
			animations		= new List<ResourceInfo<Animation>>();
			tileData		= new List<ResourceInfo<TileData>>();
			actionTileData	= new List<ResourceInfo<ActionTileData>>();
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
			actionTileData.Clear();
		}

		//-----------------------------------------------------------------------------
		// Loading
		//-----------------------------------------------------------------------------

		public World Load(string fileName, bool editorMode) {
			this.fileName = fileName;
			this.editorMode = editorMode;
			BinaryReader reader = new BinaryReader(File.OpenRead(fileName));
			World world = Load(reader);
			reader.Close();
			return world;
		}

		private World Load(BinaryReader reader) {
			Clear();
			skipRemainingResources = false;

			World world = null;

			try {
				world = new World();

				// Read the header
				ReadHeader(reader, world);
				// Read the string list
				ReadStrings(reader);
				// Read resource lists
				ReadTileTypes(reader);
				ReadResourceList(reader, zones);
				ReadResourceList(reader, tilesets);
				ReadResourceList(reader, collisionModels);
				ReadResourceList(reader, sprites);
				ReadResourceList(reader, animations);
				ReadResourceList(reader, tileData);
				ReadResourceList(reader, actionTileData);

				// Read the world data.
				ReadWorld(reader, world);
			}
			catch (WorldFileException ex) {
				exception = ex;
				Console.WriteLine("Error loading world: " + ex.Message);
				return null;
			}
			//catch (Exception ex) {
			//	exception = ex;
			//	Console.WriteLine("Error loading world: " + ex.Message);
			//	return null;
			//}

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
			if (version >= 3) {
				// Read the scripts first so that references can be added.
				ReadScripts(reader, world);
			}

			// Read the world's properties.
			ReadProperties(reader, world.Properties);
			//if (version >= 4)
			//	ReadVariables(reader, world.Variables);
			if (version <= 3)
				ReadEvents(reader, world, world);
			else
				ReadTriggers(reader, world, world);

			// Read the areas.
			int areaCount = reader.ReadInt32();
			for (int i = 0; i < areaCount; i++) {
				Area area = ReadArea(reader, world);
				world.AddArea(area);
			}

			// Read the levels.
			int levelCount = reader.ReadInt32();
			for (int i = 0; i < levelCount; i++) {
				Level level = ReadLevel(reader, world);
				world.AddLevel(level);
			}

			if (version == 2) {
				// Read the scripts.
				ReadScripts(reader, world);
				ReadLegacyEvents(world);
			}
		}

		private void ReadLegacyEvents(World world) {
			foreach (IEventObject eventObject in world.GetEventObjects()) {
				foreach (var evnt in eventObject.Events.GetEvents()) {
					Property property = eventObject.Properties.GetProperty(evnt.Name, false);
					if (property == null)
						property = eventObject.Properties.GetProperty("event_" + evnt.Name, false);
					if (property == null)
						property = eventObject.Properties.GetProperty(evnt.Name + "d", false);
					if (property == null)
						property = eventObject.Properties.GetProperty(evnt.Name + "ed", false);

					if (property != null) {
						eventObject.Properties.RemoveProperty(property.Name, false);
						if (!string.IsNullOrWhiteSpace(property.StringValue))
							ReadLegacyEvent(evnt, property.StringValue, world);
					}
				}
			}
		}

		private void ReadLegacyEvent(Event evnt, string scriptID, World world) {
			Script script = world.GetScript(scriptID);
			if (script != null) {
				if (editorMode) {
					if (script.IsHidden) {
						evnt.Script = script;
						world.RemoveScript(scriptID);
					}
					else {
						evnt.DefineScript(scriptID + "();");
					}
				}
				else {
					evnt.InternalScriptID = scriptID;
				}
			}
			else {
				Console.WriteLine("Unknown event: " + scriptID);
			}
		}

		private Area ReadArea(BinaryReader reader, World world) {
			Area area = new Area();
			ReadProperties(reader, area.Properties);
			if (version <= 3)
				ReadEvents(reader, area, world);
			else
				ReadTriggers(reader, area, world);
			return area;
		}

		private Level ReadLevel(BinaryReader reader, World world) {
			// Read the level dimensions.
			Point2I dimensions	= reader.ReadPoint2I();
			Point2I roomSize	= reader.ReadPoint2I();
			int roomLayerCount	= reader.ReadInt32();
			Level level = new Level(dimensions, roomLayerCount, roomSize);

			// Read the level's properties.
			ReadProperties(reader, level.Properties);
			if (version <= 3)
				ReadEvents(reader, level, world);
			else
				ReadTriggers(reader, level, world);

			// Read all the rooms in the level.
			for (int y = 0; y < level.Height; y++) {
				for (int x = 0; x < level.Width; x++) {
					Room room = new Room(level, x, y);
					ReadRoom(reader, room, level, world);
					level.SetRoom(room, x, y);
				}
			}

			return level;
		}
		
		private void ReadRoom(BinaryReader reader, Room room, Level level, World world) {
			// Read the dimensions.
			Point2I size = reader.ReadPoint2I();
			int layerCount = reader.ReadInt32();

			// Read the room's properties.
			ReadProperties(reader, room.Properties);
			if (version <= 3)
				ReadEvents(reader, room, world);
			else
				ReadTriggers(reader, room, world);

			// Read tile data for first layer (stored as a grid of tiles).
			for (int y = 0; y < room.Height; y++) {
				for (int x = 0; x < room.Width; x++) {
					TileDataInstance tile = ReadTileData(reader, world);
					if (tile != null)
						room.PlaceTile(tile, new Point2I(x, y), 0);
				}
			}

			// Read tile data for higher layers (stored as a list of non-null tiles).
			int tileDataCount = reader.ReadInt32();
			for (int i = 0; i < tileDataCount; i++) {
				Point2I location	= reader.ReadPoint2I();
				int layer			= reader.ReadInt32();
				TileDataInstance tile = ReadTileData(reader, world);
				if (tile != null)
					room.PlaceTile(tile, location, layer);
			}

			// Read action tile data.
			int actionTileDataCount = reader.ReadInt32();
			for (int i = 0; i < actionTileDataCount; i++) {
				ActionTileDataInstance actionTileData = ReadActionTileData(reader, world);
				if (actionTileData != null)
					room.AddActionTile(actionTileData);
			}
		}

		private TileDataInstance ReadTileData(BinaryReader reader, World world) {
			int tilesetIndex = reader.ReadInt32();
			if (tilesetIndex == NULL_TILE) // -11 indicates a null tile.
				return null;
			
			TileDataInstance tile = new TileDataInstance();
			Tileset tileset = null;
			if (tilesetIndex >= 0)
				tileset = tilesets[tilesetIndex].Resource;

			if (tileset != null) {
				// Create tile from a tileset.
				Point2I sheetLocation = new Point2I(
					reader.ReadInt32(), reader.ReadInt32());
				try {
					BaseTileData baseTileData = tileset.GetTileData(sheetLocation);
					if (baseTileData is TileData)
						tile.TileData = (TileData) baseTileData;
				}
				catch (Exception) { }
			}
			else {
				// Create tile from a TileData resource.
				tile.TileData = ReadResource(reader, tileData);
			}

			// Read the tile's properties
			ReadProperties(reader, tile.Properties);
			if (version <= 3)
				ReadEvents(reader, tile, world);
			else
				ReadTriggers(reader, tile, world);
			if (tile.TileData != null)
				tile.Properties.BaseProperties = tile.TileData.Properties;
			tile.ModifiedProperties.BaseProperties = tile.Properties;

			return (tile.TileData != null ? tile : null);
		}

		private ActionTileDataInstance ReadActionTileData(BinaryReader reader, World world) {
			ActionTileData tileData = ReadResource(reader, actionTileData);
			if (tileData == null)
				return null;
			Point2I position = new Point2I(
				reader.ReadInt32(), reader.ReadInt32());

			ActionTileDataInstance actionTile = new ActionTileDataInstance(tileData, position);
			ReadProperties(reader, actionTile.Properties);
			if (version <= 3)
				ReadEvents(reader, actionTile, world);
			else
				ReadTriggers(reader, actionTile, world);
			actionTile.Properties.PropertyObject = actionTile;
			if (tileData != null)
				actionTile.Properties.BaseProperties = actionTile.ActionTileData.Properties;
			actionTile.ModifiedProperties.BaseProperties = actionTile.Properties;

			return (actionTile.ActionTileData != null ? actionTile : null);
		}

		private void ReadTriggers(BinaryReader reader, ITriggerObject triggerObject, World world) {
			if (version < 3)
				return;
			int count = reader.ReadInt32();
			for (int i = 0; i < count; i++) {
				string name = ReadString(reader);
				string description = ReadString(reader);
				bool initiallyOn = reader.ReadBoolean();
				bool fireOnce = reader.ReadBoolean();
				string eventName = ReadString(reader);
				Script script = ReadScript(reader);

				Event evnt = triggerObject.Events.GetEvent(eventName);
				if (evnt != null) {
					triggerObject.Triggers.AddTrigger(new Trigger(triggerObject.Triggers) {
						Name = eventName,
						Script = script,
						InitiallyOn = initiallyOn,
						IsEnabled = initiallyOn,
						FireOnce = fireOnce,
						EventType = new TriggerEvent(evnt),
					});
				}
			}
		}

		private void ReadEvents(BinaryReader reader, ITriggerObject triggerObject, World world) {
			if (version < 3)
				return;
			int count = reader.ReadInt32();
			for (int i = 0; i < count; i++) {
				string eventName = ReadString(reader);
				string eventScriptID = ReadString(reader);

				Event evnt = triggerObject.Events.GetEvent(eventName);
				Script script = world.GetScript(eventScriptID);
				
				if (evnt != null) {
					//Console.WriteLine("Reading event " + evnt.Name + " with script " + eventScriptID);
					//if (script != null)
					if (script != null)
						world.ScriptManager.RemoveScript(script);
					triggerObject.Triggers.AddTrigger(new Trigger(triggerObject.Triggers) {
						Name = evnt.Name,
						Script = script,
						InitiallyOn = true,
						IsEnabled = true,
						FireOnce = false,
						EventType = new TriggerEvent(evnt),
					});
				}
			}
		}

		private Properties ReadProperties(BinaryReader reader, Properties properties) {
			int count = reader.ReadInt32();
			for (int i = 0; i < count; i++) {
				Property property = ReadProperty(reader);
				property = properties.Set(property.Name, property);
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
			if (type == PropertyType.Point) {
				return Property.Create(name, reader.ReadPoint2I());
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

		private Variables ReadVariables(BinaryReader reader, Variables variables) {
			int count = reader.ReadInt32();
			for (int i = 0; i < count; i++) {
				Variable variable = ReadVariable(reader);
				variable = variables.SetGeneric(variable.Name, variable.ObjectValue);
			}
			return variables;
		}

		private Variable ReadVariable(BinaryReader reader) {
			VariableType type = (VariableType) reader.ReadInt32();
			string name = ReadString(reader);

			if (type == VariableType.Integer) {
				return Variable.Create(name, reader.ReadInt32());
			}
			else if (type == VariableType.Float) {
				return Variable.Create(name, reader.ReadSingle());
			}
			else if (type == VariableType.Boolean) {
				return Variable.Create(name, reader.ReadBoolean());
			}
			else if (type == VariableType.String) {
				return Variable.Create(name, ReadString(reader));
			}
			if (type == VariableType.Point) {
				return Variable.Create(name, reader.ReadPoint2I());
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
				Type type	= GameUtil.FindTypeWithBase<Tile>(name, false);
				tileTypes.Add(new ResourceInfo<Type>(type, index));
			}
		}
		
		private void ReadResourceList<T>(BinaryReader reader, List<ResourceInfo<T>> list) where T : class {
			int count = reader.ReadInt32();
			list.Capacity = count;

			for (int i = 0; i < count; i++) {
				int index = reader.ReadInt32();
				string name = strings[index];
				T resource = Resources.Get<T>(name);
				if (resource == null && !skipRemainingResources && LocateResource != null) {
					LocateResourceEventArgs e = new LocateResourceEventArgs(typeof(T), name);
					LocateResource(e);
					if (!e.SkipRemaining && e.NewName != null) {
						resource = Resources.Get<T>(e.NewName);
					}
					skipRemainingResources = e.SkipRemaining;
				}
				list.Add(new ResourceInfo<T>(resource, index));
			}
		}
		

		//-----------------------------------------------------------------------------
		// Saving
		//-----------------------------------------------------------------------------
		
		public void Save(string fileName, World world, bool editorMode) {
			this.fileName = fileName;
			this.editorMode = editorMode;
			FileStream fileStream = new FileStream(fileName, FileMode.Create);
			BinaryWriter writer = new BinaryWriter(fileStream);
			Save(writer, world);
			writer.Close();
		}

		private void Save(BinaryWriter writer, World world) {
			Clear();

			version = WORLDFILE_VERSION;

			// Write the level data to memory.
			MemoryStream worldDataStream = new MemoryStream();
			BinaryWriter worldDataWriter = new BinaryWriter(worldDataStream);
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
			WriteResourceList(writer, actionTileData);

			// Write the world data.
			writer.Write(worldData, 0, levelDataSize);
		}

		private void WriteHeader(BinaryWriter writer, World world) {
			writer.Write(MAGIC);	// Magic 4 bytes "Zwd2"
			writer.Write(version);	// WorldFile version number

			writer.Write(world.StartLevelIndex);
			writer.Write(world.StartRoomLocation.X);
			writer.Write(world.StartRoomLocation.Y);
			writer.Write(world.StartTileLocation.X);
			writer.Write(world.StartTileLocation.Y);
		}
		
		private void WriteScripts(BinaryWriter writer, World world) {
			// Write the compiled assembly
			byte[] rawAssembly = world.ScriptManager.RawAssembly;
			if (rawAssembly != null) {
				writer.Write(world.ScriptManager.RawAssembly.Length);
				writer.Write(world.ScriptManager.RawAssembly);
			}
			else
				writer.Write((int) 0);

			// Write the individual scripts
			writer.Write(world.ScriptManager.Scripts.Count);
			foreach (Script script in world.ScriptManager.Scripts.Values)
				WriteScript(writer, script);
		}
		
		private void WriteScript(BinaryWriter writer, Script script, Event evnt = null) {
			// Write the name and source code.
			WriteString(writer, script.ID);
			if (version >= 4)
				WriteString(writer, script.Description);
			WriteString(writer, script.Code);
			if (version >= 4) {
				writer.Write(script.OffsetInCode);
				WriteString(writer, script.MethodName);
			}
			if (version <= 3)
				writer.Write(script.IsHidden);

			// Write the script parameters
			if (evnt != null) {
				writer.Write(0);
			}
			else {
				writer.Write(script.Parameters.Count);
				for (int i = 0; i < script.Parameters.Count; i++) {
					WriteString(writer, script.Parameters[i].Type);
					WriteString(writer, script.Parameters[i].Name);
				}
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
			// Read the raw assembly
			int byteCount = reader.ReadInt32();
			if (byteCount > 0)
				world.ScriptManager.RawAssembly = reader.ReadBytes(byteCount);
			else
				world.ScriptManager.RawAssembly = null;

			// Read the individual scripts
			int count = reader.ReadInt32();
			for (int i = 0; i < count; i++) {
				Script script = ReadScript(reader);
				//if (!script.IsHidden)
					//if (script != null)
					world.AddScript(script);
				//else
				//	Console.WriteLine("ASDADS");
			}
		}
		
		private Script ReadScript(BinaryReader reader) {
			Script script = new Script();

			// Read the name and source code
			script.ID = ReadString(reader);
			if (version >= 4)
				script.Description = ReadString(reader);
			script.Code = ReadString(reader);
			if (version >= 4) {
				script.OffsetInCode = reader.ReadInt32();
				script.MethodName = ReadString(reader);
			}
			if (version <= 3)
				script.IsHidden	= reader.ReadBoolean();
			
			// Read the parameters
			int paramCount = reader.ReadInt32();
			for (int i = 0; i < paramCount; i++) {
				ScriptParameter param = new ScriptParameter();
				param.Type = ReadString(reader);
				param.Name = ReadString(reader);
				script.Parameters.Add(param);
			}
			
			// Read errors and warnings
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
			// Write the scripts
			WriteScripts(writer, world);

			// Write the world's properties
			WriteProperties(writer, world.Properties);
			//WriteVariables(writer, world.Variables);
			if (version <= 3)
				WriteEvents(writer, world.Events);
			else
				WriteTriggers(writer, world.Triggers);

			// Write the area
			writer.Write(world.AreaCount);
			for (int i = 0; i < world.AreaCount; i++)
				WriteArea(writer, world.GetAreaAt(i));

			// Write the level data
			writer.Write(world.LevelCount);
			for (int i = 0; i < world.LevelCount; i++)
				WriteLevel(writer, world.GetLevelAt(i));
		}

		private void WriteArea(BinaryWriter writer, Area area) {
			WriteProperties(writer, area.Properties);
			if (version <= 3)
				WriteEvents(writer, area.Events);
			else
				WriteTriggers(writer, area.Triggers);
		}

		private void WriteLevel(BinaryWriter writer, Level level) {
			//WriteString(writer, level.Name);
			writer.Write(level.Dimensions);
			writer.Write(level.RoomSize);
			writer.Write(level.RoomLayerCount);
			WriteProperties(writer, level.Properties);
			if (version <= 3)
				WriteEvents(writer, level.Events);
			else
				WriteTriggers(writer, level.Triggers);

			// Write rooms.
			for (int y = 0; y < level.Height; y++) {
				for (int x = 0; x < level.Width; x++) {
					Room room = level.GetRoomAt(x, y);
					WriteRoom(writer, room);
				}
			}
		}

		private void WriteRoom(BinaryWriter writer, Room room) {
			writer.Write(room.Size);
			writer.Write(room.LayerCount);
			WriteProperties(writer, room.Properties);
			if (version <= 3)
				WriteEvents(writer, room.Events);
			else
				WriteTriggers(writer, room.Triggers);

			// Write all tiles for the first tile layer.
			for (int y = 0; y < room.Height; y++) {
				for (int x = 0; x < room.Width; x++) {
					TileDataInstance tile = room.GetTile(x, y, 0);
					if (tile != null && tile.IsAtLocation(x, y))
						WriteTileData(writer, tile);
					else
						writer.Write(NULL_TILE); // -11 signifies a null tile.
				}
			}

			// Write non-null tile data for higher layers.
			var counter = BinaryCounter.Start(writer);
			//writer.Write(tileDataCount);
			for (int i = 1; i < room.LayerCount; i++) {
				foreach (TileDataInstance tile in room.GetTileLayer(i)) {
					writer.Write(tile.Location);
					writer.Write(tile.Layer);
					WriteTileData(writer, tile);
					counter.Count++;
				}
			}
			counter.WriteCountAndReturn();

			// Write action tile data
			writer.Write(room.ActionCount);
			for (int i = 0; i < room.ActionCount; i++) {
				ActionTileDataInstance actionTile = room.GetActionTileAt(i);
				WriteActionTileData(writer, actionTile);
			}
		}

		private void WriteTileData(BinaryWriter writer, TileDataInstance tile) {
			writer.Write((int) -1);
			WriteResource(writer, tile.TileData, tileData);
			WriteProperties(writer, tile.Properties);
			if (version <= 3)
				WriteEvents(writer, tile.Events);
			else
				WriteTriggers(writer, tile.Triggers);
		}

		private void WriteActionTileData(BinaryWriter writer,
			ActionTileDataInstance actionTile)
		{
			WriteResource(writer, actionTile.ActionTileData, actionTileData);
			writer.Write(actionTile.Position);
			WriteProperties(writer, actionTile.Properties);
			if (version <= 3)
				WriteEvents(writer, actionTile.Events);
			else
				WriteTriggers(writer, actionTile.Triggers);
		}

		private void WriteEvents(BinaryWriter writer, EventCollection events) {
			int count = events.GetDefinedEvents().Count();
			writer.Write(count);
			foreach (Event evnt in events.GetDefinedEvents()) {
				WriteString(writer, evnt.Name);
				WriteString(writer, evnt.InternalScriptID);
			}
		}

		private void WriteTriggers(BinaryWriter writer, TriggerCollection triggers) {
			writer.Write(triggers.Count);
			foreach (Trigger trigger in triggers) {
				if (trigger.Script != null) {
					WriteString(writer, trigger.Name);
					WriteString(writer, trigger.Description);
					writer.Write(trigger.InitiallyOn);
					writer.Write(trigger.FireOnce);
					WriteString(writer, trigger.EventType.Name);
					WriteScript(writer, trigger.Script);
				}
			}
		}

		private void WriteProperties(BinaryWriter writer, Properties properties) {
			writer.Write((int) properties.Count);
			foreach (Property property in properties.GetProperties()) {
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
			else if (property.Type == PropertyType.Point) {
				writer.Write(property.PointValue);
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


		private void WriteVariables(BinaryWriter writer, Variables variables) {
			writer.Write(variables.CustomCount);
			foreach (Variable variable in variables.GetCustomVariables()) {
				WriteVariable(writer, variable);
			}
		}

		private void WriteVariable(BinaryWriter writer, Variable variable) {
			writer.Write((int) variable.Type);
			WriteString(writer, variable.Name);

			if (variable.Type == VariableType.Integer) {
				writer.Write(variable.IntValue);
			}
			else if (variable.Type == VariableType.Float) {
				writer.Write(variable.FloatValue);
			}
			else if (variable.Type == VariableType.Boolean) {
				writer.Write(variable.BoolValue);
			}
			else if (variable.Type == VariableType.String) {
				WriteString(writer, variable.StringValue);
			}
			else if (variable.Type == VariableType.Point) {
				writer.Write(variable.PointValue);
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
				Dictionary<string, T> resourceList = Resources.GetDictionary<T>();
				string name = resourceList.FirstOrDefault(x => x.Value == resource).Key;
				index = list.Count;
				list.Add(new ResourceInfo<T>(resource, strings.Count));
				strings.Add(name);
			}

			writer.Write(index);
		}

		private int WriteObject<T>(BinaryWriter writer, T obj, List<T> objList) where T : class {
			int index = objList.Count;
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

		public Exception Exception {
			get { return exception; }
		}

		public int Version {
			get { return version; }
		}
	}
}
