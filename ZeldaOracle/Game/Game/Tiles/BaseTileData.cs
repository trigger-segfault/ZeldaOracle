using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Entities;

namespace ZeldaOracle.Game.Tiles {

	public abstract class BaseTileData : IPropertyObject {

		/// <summary>The overridden type for the tile.</summary>
		protected Type type;
		/// <summary>The type of entity this tile spawns.</summary>
		protected Type entityType;
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
			entityType		= null;
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
			
			properties.SetEnumInt("reset_condition", TileResetCondition.LeaveRoom)
				.SetDocumentation("Reset Condition", "enum", typeof(TileResetCondition), "General",
				"The condition for when the tile resets its properties.");

			properties.Set("shared", false)
				.SetDocumentation("Is Shared", "Parenting", "This tile will appear in all child rooms if its area is unoccupied.");
		}

		public BaseTileData(BaseTileData copy) {
			type			= copy.type;
			entityType		= copy.EntityType;
			tileset			= copy.tileset;
			sheetLocation	= copy.sheetLocation;
			previewSprite	= copy.previewSprite;
			properties		= new Properties(copy.properties, this);
			events			= new EventDocumentationCollection(copy.events);
		}

		public virtual void Clone(BaseTileData copy) {
			type			= copy.type;
			entityType		= copy.entityType;
			tileset			= copy.tileset;
			sheetLocation	= copy.sheetLocation;
			previewSprite	= copy.previewSprite;
			properties		= new Properties(copy.properties, this);
			events			= new EventDocumentationCollection(copy.events);
		}
		
		
		//-----------------------------------------------------------------------------
		// Abstract Properties
		//-----------------------------------------------------------------------------

		public abstract ISprite Sprite { get; set; }

		public abstract Point2I Size { get; set; }


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the overridden type for the tile.</summary>
		public Type Type {
			get { return type; }
			set {
				if ((type == null && value != null) || !type.Equals(value)) {
					// Make sure we extend the correct type and don't go backwards.
					if (type != null) {
						if (value == null || !type.IsAssignableFrom(value))
							throw new ArgumentException("New type does not inherit from previous tile type!");
					}
					Type previousType = type;
					type = value;
					// Initialize the tile's new types.
					TileDataInitializing.InitializeTile(this, previousType);
				}
			}
		}

		/// <summary>Gets or sets the type of entity this tile spawns.</summary>
		public Type EntityType {
			get { return entityType; }
			set {
				if ((entityType == null && value != null) ||
					!entityType.Equals(value))
				{
					// Make sure we extend entity and don't go backwards.
					if (entityType != null) {
						if (value == null || !entityType.IsAssignableFrom(value))
							throw new ArgumentException("New type does not inherit from previous entity type!");
					}
					else if (!typeof(Entity).IsAssignableFrom(value)) {
						throw new ArgumentException("New type does not inherit from Entity!");
					}
					Type previousType = entityType;
					entityType = value;
					// Initialize the tile's new entity types.
					TileDataInitializing.InitializeEntity(this, previousType);
				}
			}
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

		public TileResetCondition ResetCondition {
			get { return properties.GetEnum("reset_condition",
				TileResetCondition.LeaveRoom); }
			set { properties.SetEnum("reset_condition", value); }
		}

		/// <summary>Gets or sets if the tile is shared between child rooms.</summary>
		public bool IsShared {
			get { return properties.Get("shared", false); }
			set { properties.Set("shared", value); }
		}

		/// <summary>Returns true if the tile should be treated as a monster.</summary>
		public bool IsMonster {
			get { return properties.Contains("monster_id"); }
		}
	}
}
