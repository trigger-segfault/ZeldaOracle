using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Tiles {

	public abstract class BaseTileData : IPropertyObject {
		
		protected Type type;
		protected Tileset tileset;
		protected Point2I sheetLocation;	// TODO: remove this, maybe?
		protected Properties properties;
		protected EventDocumentationCollection events;
		private ISprite previewSprite;
		private string name;

		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public BaseTileData() {
			name			= "";
			type			= null;
			tileset			= null;
			sheetLocation	= Point2I.Zero;
			properties		= new Properties(this);
			events			= new EventDocumentationCollection();
			previewSprite	= null;

			properties.Set("id", "")
				.SetDocumentation("ID", "General",
				"The id used to refer to this tile.");

			properties.Set("enabled", true)
				.SetDocumentation("Enabled", "General",
				"True if the tile is spawned upon entering the room.");

			properties.Set("sprite_index", 0)
				.SetDocumentation("Sprite Index", "sprite_index", "", "Internal",
				"The current sprite in the sprite list to draw.", true, false);

			properties.Set("substrip_index", 0)
				.SetDocumentation("Animation Substrip Index", "", "", "Internal",
				"The index of the substrip for dynamic animations.", true, false);
		}

		public BaseTileData(BaseTileData copy) {
			type				= copy.type;
			tileset				= copy.tileset;
			sheetLocation		= copy.sheetLocation;
			properties			= new Properties(this);
			properties.SetAll(copy.properties);
			events              = new EventDocumentationCollection(copy.events);
		}

		public virtual void Clone(BaseTileData copy) {
			type		= copy.type;
			properties	= new Properties(copy.properties);
			events		= new EventDocumentationCollection(copy.events);
			//properties.SetAll(copy.properties);
		}
		
		
		//-----------------------------------------------------------------------------
		// Abstract Properties
		//-----------------------------------------------------------------------------

		public abstract ISprite Sprite { get; set; }


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

		public EventDocumentationCollection Events {
			get { return events; }
		}

		public string Name {
			get { return name; }
			set { name = value; }
		}

		public bool HasPreviewSprite {
			get { return previewSprite != null; }
		}

		public ISprite PreviewSprite {
			get { return previewSprite; }
			set { previewSprite = value; }
		}
	}
}
