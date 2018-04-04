using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Worlds {
	public class Zone : IPropertyObject, IIDObject {
		
		private TileData	defaultTileData;
		private Properties  properties;

		private StyleDefinitions styles;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public Zone() {
			this.styles = new StyleDefinitions();

			properties = new Properties(this);

			properties.Set("id", "");
			properties.Set("name", "");
			properties.Set("palette", "tiles_default");
			properties.Set("side_scrolling", false);
			properties.Set("underwater", false);
		}

		public Zone(string id) :
			this()
		{
			properties.Set("id", id);
		}

		public Zone(string id, string name) :
			this()
		{
			properties.Set("id", id);
			properties.Set("name", name);
		}

		public Zone(string id, string name, TileData defaultTileData) :
			this()
		{
			this.defaultTileData	= defaultTileData;

			properties.Set("id", id);
			properties.Set("name", name);
		}

		public Zone(Zone copy) :
			this()
		{
			this.defaultTileData    = copy.defaultTileData;
			this.styles             = new StyleDefinitions(copy.styles);
			this.properties			= new Properties(copy.properties, this);
		}

		
		//-----------------------------------------------------------------------------
		// Stuff
		//-----------------------------------------------------------------------------

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public string ID {
			get { return properties.Get<string>("id"); }
			set { properties.Set("id", value); }
		}
		
		public string Name {
			get { return properties.Get<string>("name"); }
			set { properties.Set("name", value); }
		}
		
		public TileData DefaultTileData {
			get { return defaultTileData; }
			set { defaultTileData = value; }
		}
		
		public bool IsSideScrolling {
			get { return properties.Get<bool>("side_scrolling"); }
			set { properties.Set("side_scrolling", value); }
		}
		
		public bool IsUnderwater {
			get { return properties.Get<bool>("underwater"); }
			set { properties.Set("underwater", value); }
		}

		public Properties Properties {
			get { return properties; }
		}

		public StyleDefinitions StyleDefinitions {
			get { return styles; }
			set { styles = value; }
		}

		public string PaletteID {
			get { return properties.Get<string>("palette"); }
			set { properties.Set("palette", value); }
		}

		public Palette Palette {
			get { return properties.GetResource<Palette>("palette"); }
		}
	}
}
