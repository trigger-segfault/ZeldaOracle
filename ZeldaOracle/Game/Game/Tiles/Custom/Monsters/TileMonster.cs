using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Tiles.ActionTiles;

namespace ZeldaOracle.Game.Tiles.Custom.Monsters {
	public abstract class TileMonster : Tile {

		//-----------------------------------------------------------------------------
		// Virtual Methods
		//-----------------------------------------------------------------------------

		/// <summary>Creates and spawns the monster associated with this tile.</summary>
		public virtual void SpawnMonster() {
			RoomControl.RemoveTile(this);
			Monster monster = ConstructMonster();
			monster.IgnoreMonster = IgnoreMonster;
			monster.Position = SpawnPosition;
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

		/// <summary>True if this monster is not counted towards clearing a room.</summary>
		public bool IgnoreMonster {
			get { return Properties.Get("ignore_monster", true); }
			set { Properties.Set("ignore_monster", value); }
		}

		/// <summary>True if this monster needs to be killed in order to clear the room.</summary>
		public bool NeedsClearing {
			get { return !IgnoreMonster; }
		}
	}
}
