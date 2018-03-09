using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;
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
			//monster.IgnoreMonster = IgnoreMonster;
			monster.Position = SpawnPosition;
			monster.Properties = Properties;
			RoomControl.SpawnEntity(monster);
		}

		/// <summary>Constructs the monster associated with the tile.</summary>
		public virtual Monster ConstructMonster() {
			return MonsterAction.ConstructObject<Monster>(MonsterType);
		}


		//-----------------------------------------------------------------------------
		// Virtual Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the type of monster to spawn.</summary>
		public abstract Type MonsterType { get; }

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
			get { return Properties.GetBoolean("dead", false); }
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
