using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Tiles.ActionTiles;

namespace ZeldaOracle.Game.Tiles.Custom.Monsters {
	/// <summary>The base class for all tiles that spawn monsters.</summary>
	public abstract class TileMonster : Tile {
		
		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			base.OnInitialize();
			
			if (!CanSpawn)
				RoomControl.RemoveTile(this);
		}


		//-----------------------------------------------------------------------------
		// Virtual Methods
		//-----------------------------------------------------------------------------

		/// <summary>Creates and spawns the monster associated with this tile.</summary>
		public virtual void SpawnMonster() {
			if (!CanSpawn)
				return;
			RoomControl.RemoveTile(this);
			Monster monster = ConstructMonster();
			monster.Position = SpawnPosition;
			monster.Properties = Properties;
			monster.Events = Events;
			monster.Vars = Variables;
			RoomControl.SpawnEntity(monster);
		}

		/// <summary>Constructs the monster associated with the tile.</summary>
		public virtual Monster ConstructMonster() {
			return ReflectionHelper.ConstructSafe<Monster>(EntityType);
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------
		
		/// <summary>Initializes the properties and events for the tile type.</summary>
		public static void InitializeTileData(TileData data) {
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
			data.ResetCondition = TileResetCondition.Never;
			data.Properties.Hide("reset_condition");
		}


		//-----------------------------------------------------------------------------
		// Virtual Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the type of monster to spawn.</summary>
		//public abstract Type MonsterType { get; }

		/// <summary>Gets the spawn position for the monster.</summary>
		public virtual Vector2F SpawnPosition {
			get { return Center + new Vector2F(0, 6); }
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets if this monster is not counted towards clearing a room.</summary>
		public bool IgnoreMonster {
			get { return Properties.Get("ignore_monster", true); }
			set { Properties.Set("ignore_monster", value); }
		}

		/// <summary>True if this monster needs to be killed in order to clear the room.</summary>
		public bool NeedsClearing {
			get { return !IgnoreMonster; }
		}

		/// <summary>Gets or sets if this monster is dead.</summary>
		public bool IsDead {
			get { return Properties.Get<bool>("dead", false); }
			set { Properties.Set("dead", value); }
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
			get { return Properties.Get("monster_id", -1); }
			set { Properties.Set("monster_id", value); }
		}
	}
}
