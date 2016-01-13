﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Tiles {
	public class TileDataInstance : IPropertyObject, IEventObject {

		private Room		room;
		private Point2I		location;
		private int			layer;
		private TileData	tileData;
		private Properties	properties;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public TileDataInstance() {
			this.room		= null;
			this.location	= Point2I.Zero;
			this.layer		= 0;
			this.tileData	= null;
			this.properties = new Properties();
			this.properties.PropertyObject = this;
		}

		public TileDataInstance(TileData tileData, int x, int y, int layer) {
			this.room		= null;
			this.location	= new Point2I(x, y);
			this.layer		= layer;
			this.tileData	= tileData;
			this.properties = new Properties();
			this.properties.PropertyObject = this;
			this.properties.BaseProperties = tileData.Properties;
		}


		//-----------------------------------------------------------------------------
		// Flags
		//-----------------------------------------------------------------------------

		public void SetFlags(TileFlags flagsToSet, bool enabled) {
			TileFlags flags = Flags;
			if (enabled)
				flags |= flagsToSet;
			else
				flags &= ~flagsToSet;
			Flags = flags;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public Room Room {
			get { return room; }
			set { room = value; }
		}

		public Point2I Location {
			get { return location; }
			set { location = value; }
		}
		
		public int Layer {
			get { return layer; }
			set { layer = value; }
		}

		public TileData TileData {
			get { return tileData; }
			set {
				tileData = value;
				if (tileData == null)
					properties.BaseProperties = null;
				else
					properties.BaseProperties = tileData.Properties;
			}
		}
		
		public Properties Properties {
			get { return properties; }
			set {
				properties = value;
				properties.PropertyObject = this;
			}
		}
		
		public Type Type {
			get { return tileData.Type; }
		}
		
		public Point2I Size {
			get { return properties.GetPoint("size", Point2I.One); }
			set { properties.Set("size", value); }
		}

		public SpriteAnimation Sprite {
			get { return tileData.Sprite; }
		}

		public SpriteAnimation CurrentSprite {
			get {
				if (tileData.SpriteList.Length > 0)
					return tileData.SpriteList[properties.GetInteger("sprite_index")];
				return new SpriteAnimation();
			}
		}

		public SpriteAnimation[] SpriteList {
			get { return tileData.SpriteList; }
		}

		public SpriteAnimation SpriteAsObject {
			get { return tileData.SpriteAsObject; }
		}

		public Animation BreakAnimation {
			get { return tileData.BreakAnimation; }
		}

		public TileFlags Flags {
			get { return (TileFlags) properties.GetInteger("flags", 0); }
			set { properties.Set("flags", (int) value); }
		}

		public CollisionModel CollisionModel {
			get { return properties.GetResource<CollisionModel>("collision_model", null); }
			set { properties.SetAsResource<CollisionModel>("collision_model", value); }
		}

		public Point2I SheetLocation {
			get { return tileData.SheetLocation; }
		}
		
		public Tileset Tileset {
			get { return TileData.Tileset; }
		}

		public Properties BaseProperties {
			get { return TileData.Properties; }
		}

		public ObjectEventCollection Events {
			get { return TileData.Events; }
		}

		public string Id {
			get { return properties.GetString("id", ""); }
		}

		public TileSpawnOptions SpawnOptions {
			get {
				return new TileSpawnOptions() {
					PoofEffect = properties.GetBoolean("spawn_poof_effect", false),
					SpawnDelayAfterPoof = properties.GetInteger("spawn_delay_after_poof", 31),
				};
			}
		}
	}
}
