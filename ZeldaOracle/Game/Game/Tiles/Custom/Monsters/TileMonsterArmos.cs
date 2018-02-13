using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Monsters;

namespace ZeldaOracle.Game.Tiles.Custom.Monsters {
	public class TileMonsterArmos : TileMonster {
		
		private int spawnTimer;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileMonsterArmos() {
			spawnTimer = -1;
		}


		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		public void BreathLife() {
			if (spawnTimer == -1) {
				spawnTimer = GameSettings.MONSTER_ARMOS_BREATH_LIFE_DURATION;
			}
		}

		private void CheckForPlayer() {
			Rectangle2F box;
			if (!IsBlue)
				box = new Rectangle2F(-1, -1, 2, 2);
			else
				box = new Rectangle2F(-12, -11, 24, 23);
			box.Point += Position;
			box.Size += GameSettings.TILE_SIZE;
			if (RoomControl.Player.Physics.PositionedCollisionBox.Intersects(box)) {
				BreathLife();
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			spawnTimer	= -1;
		}

		public override void Update() {
			base.Update();

			if (spawnTimer == 0) {
				SpawnMonster();
				return;
			}

			if (spawnTimer != -1) {
				spawnTimer--;
			}
			else {
				CheckForPlayer();
			}
		}

		public override void Draw(RoomGraphics g) {
			base.Draw(g);

			if (spawnTimer != -1 && spawnTimer % 2 == 0) {
				var color = ColorDefinitions.All(IsBlue ? "shaded_blue" : "shaded_red");
				g.DrawSprite(
					GameData.ANIM_MONSTER_ARMOS,
					new SpriteSettings(color),
					Position,
					DepthLayer.Monsters);
			}
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			Tile.DrawTileData(g, args);
			if (args.Extras) {
				bool blue = args.Properties.GetBoolean("blue", false);
				var color = ColorDefinitions.All(blue ? "shaded_blue" : "shaded_red");
				g.DrawSprite(
					GameData.ANIM_MONSTER_ARMOS,
					new SpriteSettings(color),
					args.Position,
					args.Color);
			}
		}


		//-----------------------------------------------------------------------------
		// Override Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the type of monster to spawn.</summary>
		public override Type MonsterType {
			get { return (IsBlue ? typeof(MonsterArmosBlue) : typeof(MonsterArmosRed)); }
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsBlue {
			get { return Properties.GetBoolean("blue", false); }
		}
	}
}
