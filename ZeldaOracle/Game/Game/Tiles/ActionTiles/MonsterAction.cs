using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Tiles.ActionTiles {

	public class MonsterAction : ActionTile {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterAction() {
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// TODO: Move these generic methods somewhere else.
		public static T ConstructObject<T>(string typeName) where T : class {
			Type type = GameUtil.FindTypeWithBase<Monster>(typeName, false);
			return ConstructObject<T>(type);
		}

		public static T ConstructObject<T>(Type type) where T : class {
			ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
			if (constructor == null)
				return null;
			return (constructor.Invoke(null) as T);
		}

		protected override void Initialize() {
			base.Initialize();

			Monster monster = null;
			
			// Construct the monster object
			if (CanSpawn) {
				monster = ReflectionHelper.ConstructSafe<Monster>(EntityType);
				if (monster == null)
					Logs.Entity.LogError("Error trying to spawn monster of type '" +
						EntityType.Name + "'!");
			}

			// Spawn the monster entity
			if (monster != null) {
				monster.SetPositionByCenter(Center);
				monster.Properties = properties;
				monster.Events = Events;
				monster.Vars = Variables;
				RoomControl.SpawnEntity(monster);
			}
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the action tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, ActionDataDrawArgs args) {
			ColorDefinitions colorDefinitions = new ColorDefinitions();
			MonsterColor color = args.Properties.GetEnum("color", MonsterColor.Red);
			switch (color) {
			case MonsterColor.Red:
				colorDefinitions.SetAll("red");
				break;
			case MonsterColor.Blue:
				colorDefinitions.SetAll("blue");
				break;
			case MonsterColor.Green:
				colorDefinitions.SetAll("green");
				break;
			case MonsterColor.Orange:
				colorDefinitions.SetAll("orange");
				break;
			case MonsterColor.Gold:
				colorDefinitions.SetAll("gold");
				break;
			case MonsterColor.DarkRed:
				colorDefinitions.SetAll("shaded_red");
				break;
			case MonsterColor.DarkBlue:
				colorDefinitions.SetAll("shaded_blue");
				break;
			}
			ActionTile.DrawTileDataColors(g, args, colorDefinitions);
		}

		/// <summary>Initializes the properties and events for the tile type.</summary>
		public static void InitializeTileData(ActionTileData data) {
			data.ResetCondition = TileResetCondition.Never;
			data.IsShared = true;

			data.Properties.Hide("reset_condition");

			data.Properties.SetEnumInt("color", MonsterColor.Red)
				.SetDocumentation("Color", "enum", typeof(MonsterColor), "Monster", "The color of the monster.");
			data.Properties.SetEnumInt("respawn_type", MonsterRespawnType.Normal)
				.SetDocumentation("Respawn Type", "enum", typeof(MonsterRespawnType), "Monster", "How a monster respawns.");
			data.Properties.Set("dead", false)
				.SetDocumentation("Is Dead", "Monster", "True if the monster is permanently dead.");
			data.Properties.Set("ignore_monster", false)
				.SetDocumentation("Ignore Monster", "Monster", "True if the monster is not counted towards clearing the room.");
			data.Properties.Set("monster_id", 0)
				.SetDocumentation("Monster ID", "Monster", "An ID unique to each monster in a room used to manage which monsters are dead. An ID of 0 will use an ID unique to every other monster in the game.");

			data.Events.AddEvent("die", "Die", "Monster", "Occurs when the monster dies.",
				new ScriptParameter(typeof(ZeldaAPI.Monster), "monster"));

			data.EntityType = typeof(Monster);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public override Type TriggerObjectType {
			get { return EntityType; }
		}

		/// <summary>Gets or sets if this monster is dead.</summary>
		public bool IsDead {
			get { return Properties.Get("dead", false); }
			set { Properties.Set("dead", value); }
		}

		/// <summary>Gets if the monster is ignored in room clear counts.</summary>
		public bool IgnoreMonster {
			get { return Properties.Get("ignore_monster", false); }
		}

		/// <summary>Gets the respawn type of the monster.</summary>
		public MonsterRespawnType RespawnType {
			get { return Properties.GetEnum("respawn_type", MonsterRespawnType.Normal); }
		}

		/// <summary>Gets if the monster can spawn.</summary>
		public bool CanSpawn {
			get { return !IsDead && !RoomControl.IsMonsterDead(MonsterID); }
		}

		/// <summary>Gets the ID unique to each monster in the room.</summary>
		public int MonsterID {
			get { return Properties.Get("monster_id", 0); }
			set { Properties.Set("monster_id", value); }
		}
	}
}
