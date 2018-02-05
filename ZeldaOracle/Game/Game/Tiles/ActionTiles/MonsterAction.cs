using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
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
			Type type = Assembly.GetExecutingAssembly().GetTypes().First(x => x.Name == typeName);
			if (type == null)
				return null;
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

			bool isDead = properties.GetBoolean("dead", false);

			Monster monster = null;

			// Construct the monster object.
			if (!isDead) {
				string monsterTypeStr = Properties.GetString("monster_type", "");
				monster = ConstructObject<Monster>(monsterTypeStr);
				if (monster == null)
					Console.WriteLine("Error trying to spawn monster of type " + monsterTypeStr);
			}

			// Spawn the monster entity.
			if (monster != null) {
				Vector2F center = position + (size * GameSettings.TILE_SIZE / 2);
				monster.SetPositionByCenter(center);
				monster.Properties = properties;
				RoomControl.SpawnEntity(monster);
			}
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the action tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, ActionTileDataDrawArgs args) {
			ColorDefinitions colorDefinitions = new ColorDefinitions();
			MonsterColor color = (MonsterColor) args.Properties.GetInteger("color", 0);
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


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Type MonsterType {
			get { return typeof(MonsterOctorok); }
		}
	}
}
