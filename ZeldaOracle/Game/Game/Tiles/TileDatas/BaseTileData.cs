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
using ZeldaOracle.Game.ResourceData;

namespace ZeldaOracle.Game.Tiles {
	/// <summary>The base data structure detailing tiles and action tiles.</summary>
	public abstract class BaseTileData : BaseResourceData {
		
		/// <summary>The type of entity this tile spawns.</summary>
		protected Type entityType;
		protected Tileset tileset;
		protected Point2I sheetLocation;	// TODO: remove this, maybe?
		protected EventDocumentationCollection events;
		private ISprite previewSprite;

		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public BaseTileData() {
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

		public BaseTileData(BaseTileData copy) : base(copy) {
			entityType		= copy.EntityType;
			tileset			= copy.tileset;
			sheetLocation	= copy.sheetLocation;
			previewSprite	= copy.previewSprite;
			events			= new EventDocumentationCollection(copy.events);
		}

		/// <summary>Clones the specified base tile data.</summary>
		public override void Clone(BaseResourceData baseCopy) {
			base.Clone(baseCopy);

			BaseTileData copy = (BaseTileData) baseCopy;
			entityType		= copy.EntityType;
			tileset			= copy.tileset;
			sheetLocation	= copy.sheetLocation;
			previewSprite	= copy.previewSprite;
			events			= new EventDocumentationCollection(copy.events);
		}


		//-----------------------------------------------------------------------------
		// Abstract Methods
		//-----------------------------------------------------------------------------

		/// <summary>Initializes data after a change in the final entity type.<para/>
		/// This needs to be extended for each non-abstract class in order
		/// to make use of compile-time generic arguments within
		/// ResourceDataInitializing.InitializeData.</summary>
		protected abstract void InitializeEntityData(Type previousType);


		//-----------------------------------------------------------------------------
		// Abstract Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the sprite of the base tile data.</summary>
		public abstract ISprite Sprite { get; set; }

		/// <summary>Gets or sets the size of the base tile data in tiles.</summary>
		public abstract Point2I TileSize { get; set; }

		/// <summary>Gets or sets the size of the base tile data in pixels.
		/// Setter only applies for actions.</summary>
		public abstract Point2I PixelSize { get; set; }


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the type of entity this tile spawns.</summary>
		public Type EntityType {
			get { return entityType; }
			set {
				if (entityType != value) {
					// Make sure we extend the correct type and don't go backwards.
					if (entityType != null) {
						if (value == null || !entityType.IsAssignableFrom(value))
							throw new ArgumentException("New entity type does not " +
								"inherit from previous entity type!");
					}
					else if (!typeof(Entity).IsAssignableFrom(value)) {
						throw new ArgumentException("New entity type does not " +
							"inherit from Entity!");
					}
					Type previousType = entityType;
					entityType = value;
					// Initialize the resource's new types
					InitializeEntityData(previousType);
				}
			}
		}

		public Tileset Tileset {
			get { return tileset; }
			set { tileset = value; }
		}

		public Point2I SheetLocation {
			get { return sheetLocation; }
			set { sheetLocation = value; }
		}

		public EventDocumentationCollection Events {
			get { return events; }
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
