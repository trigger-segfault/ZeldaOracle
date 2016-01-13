using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.EventTiles;

namespace ZeldaOracle.Game.Worlds {
	public class Dungeon : IPropertyObject {

		//private string id;
		private Properties properties;
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Dungeon() {
			properties = new Properties();
			properties.BaseProperties = new Properties();
			properties.PropertyObject = this;

			properties.BaseProperties.Set("id",			"");
			properties.BaseProperties.Set("name",		"");
			properties.BaseProperties.Set("small_keys",	0);
			properties.BaseProperties.Set("boss_key",	false);
			properties.BaseProperties.Set("map",		false);
			properties.BaseProperties.Set("compass",	false);
		}

		public Dungeon(string id, string name) : 
			this()
		{
			properties.Set("id", id);
			properties.Set("name", name);
		}

		public Dungeon(Dungeon copy) : 
			this()
		{
			properties.SetAll(copy.properties);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int NumSmallKeys {
			get { return properties.GetInteger("small_keys", 0); }
			set { properties.Set("small_keys", value); }
		}
		
		public bool HasBossKey {
			get { return properties.GetBoolean("boss_key", false); }
			set { properties.Set("boss_key", value); }
		}
		
		public bool HasMap {
			get { return properties.GetBoolean("map", false); }
			set { properties.Set("map", value); }
		}
		
		public bool HasCompass {
			get { return properties.GetBoolean("compass", false); }
			set { properties.Set("compass", value); }
		}
		
		public string ID {
			get { return properties.GetString("id", ""); }
			set { properties.Set("id", value); }
		}
		
		public string Name {
			get { return properties.GetString("name", ""); }
			set { properties.Set("name", value); }
		}

		public Properties Properties {
			get { return properties; }
			set {
				properties = value;
				properties.PropertyObject = this;
			}
		}
	}
}
