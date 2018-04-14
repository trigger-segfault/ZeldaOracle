using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Projectiles.MonsterProjectiles;
using ZeldaOracle.Game.Entities.Projectiles.Seeds;

namespace ZeldaOracle.Game.Tiles.Custom.Monsters {

	public class TileMonsterBeamos : TileMonster {

		private int shootTimer;
		private bool shooting;
		private int canShoot;
		private Angle angle;
		private int rotateTimer;
		private Vector2F shootVector;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileMonsterBeamos() {
		}


		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		private void BeginShoot() {
			shooting = true;
			shootTimer = 0;

			// Determine shoot vector
			Vector2F vectorToPlayer = RoomControl.Player.Position - Center;
			int shootAngleCount = GameSettings.MONSTER_BEAMOS_SHOOT_ANGLE_COUNT;
			int shootAngle = Orientations.NearestFromVector(vectorToPlayer, shootAngleCount);
			shootVector = Orientations.ToVector(shootAngle, shootAngleCount);
			shootVector *= GameSettings.MONSTER_BEAMOS_SHOOT_SPEED;
		}

		private void EndShoot() {
			Graphics.Colors.SetAll("blue");
			shooting = false;
			canShoot = 3; // Must rotate 3 times before shooting again
		}

		private void ShootBeam(bool flicker) {
			// Create the projectile
			BeamProjectile projectile = new BeamProjectile();
			projectile.Flickers	= flicker;
			projectile.Angle	= angle;
			ShootProjectile(projectile, shootVector);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void OnInitialize() {
			base.OnInitialize();
			shootTimer	= 0;
			shooting	= false;
			angle		= Angle.Up;
			rotateTimer	= 15;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_BEAMOS);
			Graphics.Colors.SetAll("blue");
			Graphics.SubStripIndex = angle;
		}

		public override void Update() {
			base.Update();

			Vector2F vectorToPlayer = RoomControl.Player.Position - Center;
			Angle angleToPlayer = Angle.FromVector(vectorToPlayer);
			
			// Update shooting
			if (shooting) {
				shootTimer++;

				// 14 frames before first flash
				// shoot happens at last frame of second flash
				// 15 frames after last flash
				if (shootTimer >= 14) {
					int flashIndex = (shootTimer - 14) / 4;

					//if (shootTimer == 21)
					if (shootTimer >= 21) {
						int shootIndex = shootTimer - 21;
						if (shootIndex == 0) {
							AudioSystem.PlaySound(GameData.SOUND_LASER);
						}
						if (shootIndex < 10) {
							// Shoot the beam!
							ShootBeam(((shootTimer - 21) % 2) == 1);
						}
					}

					if (flashIndex < 4) {
						if (flashIndex % 2 == 0)
							Graphics.Colors.SetAll("inverse_red");
						else
							Graphics.Colors.SetAll("blue");
					}
					else
						EndShoot();
					Graphics.SubStripIndex = angle;
				}
			}
			else {
				// Update rotation
				rotateTimer--;
				if (rotateTimer <= 0) {
					angle = angle.Rotate(1,
						GameSettings.MONSTER_BEAMOS_ROTATE_DIRECTION);
					rotateTimer = (angle.IsAxisAligned ? 15 : 25);
					Graphics.SubStripIndex = angle;

					if (canShoot > 0)
						canShoot--;
				}
				
				// Check for shooting
				if (canShoot == 0 && angleToPlayer == angle)
					BeginShoot();
			}
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			int spriteIndex = args.Properties.Get<int>("sprite_index", 0);
			ISprite sprite = args.Tile.GetSpriteIndex(spriteIndex);
			if (sprite is Animation) {
				int substripIndex = ((int)args.Time / 15) % 8;
				if (substripIndex == -1)
					substripIndex = args.Properties.Get<int>("substrip_index", 0);
				sprite = ((Animation) sprite).GetSubstrip(substripIndex);
			}
			if (sprite != null) {
				SpriteSettings settings = new SpriteSettings(args.Zone.StyleDefinitions,
					ColorDefinitions.All("blue"), args.Time);
				g.DrawSprite(
					sprite,
					settings,
					args.Position,
					args.Color);
			}
		}


		//-----------------------------------------------------------------------------
		// Override Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the type of monster to spawn.</summary>
		/*public override Type MonsterType {
			get { return null; }
		}*/
	}
}
