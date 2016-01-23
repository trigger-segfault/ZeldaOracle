using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;

namespace ZeldaOracle.Game.Tiles {

	public abstract class BaseTileData {
		
		protected Type type;
		protected Tileset tileset;
		protected Point2I sheetLocation;	// TODO: remove this, maybe?
		protected Properties properties;
		protected ObjectEventCollection events;
		private string name;

		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public BaseTileData() {
			name			= "";
			type			= null;
			tileset			= null;
			sheetLocation	= Point2I.Zero;
			properties		= new Properties();
			events			= new ObjectEventCollection();

			properties.Set("id", "");
			properties.SetDocumentation("id", "ID", "", "", "General",
				"The id used to refer to this tile.");

			properties.Set("enabled", true);
			properties.SetDocumentation("enabled", "Enabled", "", "", "General",
				"True if the tile is spawned upon entering the room.");

			properties.Set("sprite_index", 0);
			properties.SetDocumentation("sprite_index", "Sprite Index", "sprite_index", "", "Internal",
				"The current sprite in the sprite list to draw.", true, true);

			properties.Set("substrip_index", 0);
			properties.SetDocumentation("substrip_index", "Animation Substrip Index", "", "", "Internal",
				"The index of the substrip for dynamic animations.", true, true);
		}

		public BaseTileData(BaseTileData copy) {
			type				= copy.type;
			tileset				= copy.tileset;
			sheetLocation		= copy.sheetLocation;
			properties			= new Properties();
			properties.SetAll(copy.properties);
		}

		public virtual void Clone(BaseTileData copy) {
			type		= copy.type;
			properties	= new Properties(copy.properties);
			//properties.SetAll(copy.properties);
		}
		
		
		//-----------------------------------------------------------------------------
		// Abstract Properties
		//-----------------------------------------------------------------------------

		public abstract SpriteAnimation Sprite { get; set; }


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Type Type {
			get { return type; }
			set { type = value; }
		}

		/*
		public SpriteAnimation Sprite {
			get {
				if (spriteList.Length > 0)
					return spriteList[0];
				return new SpriteAnimation();
			}
			set {
				if (value == null)
					spriteList = new SpriteAnimation[0];
				else
					spriteAsObject.Set(value);
				spriteList = new SpriteAnimation[] { value };
			}
		}*/
		
		public Tileset Tileset {
			get { return tileset; }
			set { tileset = value; }
		}

		public Point2I SheetLocation {
			get { return sheetLocation; }
			set { sheetLocation = value; }
		}

		public Properties Properties {
			get { return properties; }
			set { properties = value; }
		}

		public ObjectEventCollection Events {
			get { return events; }
		}

		public string Name {
			get { return name; }
			set { name = value; }
		}
	}
}
