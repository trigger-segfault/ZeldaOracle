using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles.MonsterProjectiles;
using ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;

namespace ZeldaOracle.Game.Entities.Monsters {
	public class MonsterSpinyBeetle : Monster {

		//-------------------------------------------------------------------------
		// Constants
		//-------------------------------------------------------------------------

		private static readonly Rectangle2F DEFAULT_BOX			= new Rectangle2I(-5, -9, 10, 10);
		//private static readonly Rectangle2F COVERED_SOFT_BOX	= new Rectangle2I(-7, -13, 14, 14);
		//private static readonly Rectangle2F UNCOVERED_SOFT_BOX	= new Rectangle2I(-6, -11, 12, 11);
		//private static readonly Rectangle2F PLAYER_SOFT_BOX		= new Rectangle2I(-4, -11, 8, 6);

		private static readonly Rectangle2F[] CHARGE_BOXES = {
			new Rectangle2I(-5, -9, 13, 10),
			new Rectangle2I(-5, -14, 10, 15),
			new Rectangle2I(-8, -9, 13, 10),
			new Rectangle2I(-5, -9, 10, 11)
		};


		//-------------------------------------------------------------------------
		// Members
		//-------------------------------------------------------------------------

		private Tile coverTile;
		private CarriedTile carriedTile;
		
		private bool covered;
		private bool revealed;
		private bool uncoverPause;

		private float moveSpeed;
		private int moveAngle;
		private int moveDuration;
		private int moveTimer;

		private int chargeDuration;
		private int chargeCooldown;


		//-------------------------------------------------------------------------
		// Constructor
		//-------------------------------------------------------------------------

		public MonsterSpinyBeetle() {
			MaxHealth		= 1;
			ContactDamage	= 2;
			Color           = MonsterColor.Red;
			
			Physics.CollisionBox		= DEFAULT_BOX;
			Interactions.InteractionBox	= new Rectangle2I(-4, -11, 8, 6);
			Interactions.ReactionManager[InteractionType.Bracelet].CollisionBox =
				new Rectangle2I(-6, -12, 12, 12);
			
			// TODO: Bigger collision box for cover
			
			syncAnimationWithDirection = false;
			
			covered				= true;
			revealed			= false;
			uncoverPause		= false;

			moveSpeed			= 0.875f;
			moveDuration		= 40;
			moveTimer           = 0;
			
			chargeDuration		= (int) GMath.Ceiling(GameSettings.TILE_SIZE * 3 / moveSpeed);
			chargeCooldown      = 30;
		}

		private void SetCoveredInteractions() {
			// Weapon interations

			int swordLevel = -1;
			if (GameControl.Inventory.ItemExists("item_sword"))
				swordLevel = GameControl.Inventory.GetItem("item_sword").Level;
			int cuttableLevel = CoverProperties.GetInteger("cuttable_sword_level");
			if (CoverFlags.HasFlag(TileFlags.Cuttable) && swordLevel >= cuttableLevel) {
				Interactions.SetReaction(InteractionType.Sword,			SenderReactions.Intercept,	MonsterReactions.DamageByLevel(1, 2, 3), BreakCover);
				Interactions.SetReaction(InteractionType.SwordSpin,		MonsterReactions.Damage2,			BreakCover);
				Interactions.SetReaction(InteractionType.BiggoronSword,	MonsterReactions.Damage3,			BreakCover);
				Interactions.SetReaction(InteractionType.SwordBeam,		SenderReactions.Destroy,	MonsterReactions.Damage, BreakCover);
				Interactions.SetReaction(InteractionType.ThrownObject,	MonsterReactions.Damage);
			}
			else {
				Reactions[InteractionType.Sword]
					.Set(MonsterReactions.ParryWithClingEffect);
				Reactions[InteractionType.SwordSpin]
					.Set(MonsterReactions.ParryWithClingEffect);
				Reactions[InteractionType.BiggoronSword]
					.Set(MonsterReactions.ParryWithClingEffect);
				Interactions.SetReaction(InteractionType.SwordBeam,		SenderReactions.Intercept);
				Reactions[InteractionType.ThrownObject].Clear();
			}

			int boomerangLevel = -1;
			if (GameControl.Inventory.ItemExists("item_boomerang"))
				boomerangLevel = GameControl.Inventory.GetItem("item_boomerang").Level;
			if (CoverFlags.HasFlag(TileFlags.Boomerangable) && boomerangLevel >= Items.Item.Level2) {
				Interactions.SetReaction(InteractionType.Boomerang,		SenderReactions.Intercept,	MonsterReactions.Stun, BreakCover);
			}
			else {
				Interactions.SetReaction(InteractionType.Boomerang,		SenderReactions.Intercept);
			}

			if (CoverFlags.HasFlag(TileFlags.Burnable)) {
				Interactions.SetReaction(InteractionType.Fire,			MonsterReactions.Burn);
			}
			else {
				// TODO: Flames need to go out quickly
				Interactions.SetReaction(InteractionType.Fire,			SenderReactions.Intercept);
			}

			if (CoverFlags.HasFlag(TileFlags.Bombable)) {
				Reactions[InteractionType.BombExplosion]
					.Set(MonsterReactions.Damage).Add(BreakCover);
			}
			else {
				Reactions[InteractionType.BombExplosion].Clear();
			}

			Interactions.SetReaction(InteractionType.Shield,		SenderReactions.Bump, MonsterReactions.Bump);
			Interactions.SetReaction(InteractionType.Shovel,		MonsterReactions.Bump);
			Interactions.SetReaction(InteractionType.Bracelet,		PickupInteraction);
			// Seed interations
			Interactions.SetReaction(InteractionType.EmberSeed,		SenderReactions.Intercept);
			Interactions.SetReaction(InteractionType.ScentSeed,		SenderReactions.Intercept);
			Interactions.SetReaction(InteractionType.PegasusSeed,	SenderReactions.Intercept);
			Interactions.SetReaction(InteractionType.GaleSeed,		SenderReactions.Intercept);
			Reactions[InteractionType.MysterySeed].Set(MonsterReactions.MysterySeed);
			// Projectile interations
			Interactions.SetReaction(InteractionType.Arrow,			SenderReactions.Intercept);
			Interactions.SetReaction(InteractionType.RodFire,		SenderReactions.Intercept);
			Interactions.SetReaction(InteractionType.SwitchHook,	SwitchHookInteraction);
			// Environment interations
			Interactions.SetReaction(InteractionType.Gale,			SenderReactions.Intercept);
			Interactions.SetReaction(InteractionType.MineCart,		MonsterReactions.SoftKill);
			Reactions[InteractionType.Block].Clear();
		}


		//-------------------------------------------------------------------------
		// Override Events
		//-------------------------------------------------------------------------
		
		public override void Initialize() {
			base.Initialize();

			TileData coverTileData = Properties.GetResource<TileData>("cover_tile");
			coverTile = Tile.CreateTile(coverTileData);
			coverTile.Initialize(RoomControl);
			carriedTile = new CarriedTile(coverTile);
			carriedTile.Initialize(RoomControl);

			SetCoveredInteractions();
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_SPINY_BEETLE);
		}

		public override void OnHurt(DamageInfo damage) {
			if (covered)
				Uncover(true);
			base.OnHurt(damage);
		}

		public override void OnTouchPlayer(Entity sender, EventArgs args) {
			if (revealed || !covered)
				base.OnTouchPlayer(sender, args);
		}

		public override void Draw(RoomGraphics g) {
			if (revealed || !covered) {
				base.Draw(g);
			}
			if (covered) {
				carriedTile.Position = Position + Graphics.DrawOffset -
					carriedTile.Graphics.DrawOffset;
				carriedTile.ZPosition = ZPosition;
				if (revealed)
					carriedTile.ZPosition += 4;
				carriedTile.Draw(g, DepthLayer.PlayerAndNPCs);
			}
		}


		//-------------------------------------------------------------------------
		// Reactions
		//-------------------------------------------------------------------------

		private void SwitchHookInteraction(Entity sender, EventArgs args) {
			SwitchHookProjectile hook = sender as SwitchHookProjectile;
			carriedTile.Position = Center + new Vector2F(0, 4);
			if (revealed)
				carriedTile.ZPosition = 4;
			// Prevent the spawned carried tile from hurting the monster.
			carriedTile.Physics.Disable();
			// Spawn the entity so it draws during the pause before switching.
			RoomControl.SpawnEntity(carriedTile);
			hook.SwitchWithEntity(carriedTile);
			Uncover(false);
		}

		private void PickupInteraction(Entity sender, EventArgs args) {
			RoomControl.Player.PickupTile(coverTile);
			Uncover(false);
		}

		private void BreakCover(Entity sender, EventArgs args) {
			Uncover(true);
		}

		private void BurnCover(Entity sender, EventArgs args) {
			// TODO: Detach and burn cover tile
			// (Monster keeps charging (most likely untill burned up))
			BreakCover(sender, args);
		}


		//-------------------------------------------------------------------------
		// AI
		//-------------------------------------------------------------------------

		public override void UpdateAI() {
			if (covered) {
				carriedTile.UpdateGraphics();
				if (!revealed)
					UpdateTargetState();
				else
					UpdateChargeState();
			}
			else if (uncoverPause) {
				// Pause after being uncovered
				moveTimer--;
				if (moveTimer == 0)
					uncoverPause = false;
			}
			else {
				UpdateMoveState();
			}
		}
		
		private void UpdateChargeState() {
			if (moveTimer == 0) {
				StopCharging();
				return;
			}

			Vector2F velocity = Angles.ToVector(moveAngle) * moveSpeed;
			int direction = moveAngle / 2;
			int axis = Directions.ToAxis(direction);

			if (moveTimer == 1) {
				if (direction == Direction.Left || direction == Direction.Up) {
					velocity = Vector2F.FromIndex(axis, GMath.Floor(
						Position[axis]) - position[axis]);
				}
				else {
					velocity = Vector2F.FromIndex(axis, GMath.Ceiling(
						Position[axis]) - position[axis]);
				}
			}

			Physics.Velocity = velocity;

			if (Physics.IsColliding) {
				StopCharging();
				return;
			}
			
			// Avoid moving into a hazardous or solid tiles
			foreach (Tile tile in Physics.GetTilesMeeting(
				Position + Physics.Velocity))
			{
				if (tile.IsHoleWaterOrLava) {
					if (direction == Direction.Left || direction == Direction.Up) {
						position[axis] = tile.Bounds.BottomRight[axis] -
							Graphics.DrawOffset[axis];
					}
					else {
						position[axis] = tile.Position[axis] -
							GameSettings.TILE_SIZE - Graphics.DrawOffset[axis];
					}
					StopCharging();
					return;
				}
			}

			moveTimer--;
		}

		private void StartCharging(int direction) {
			revealed = true;
			moveTimer = chargeDuration;
			moveAngle = Directions.ToAngle(direction);
			Physics.CollisionBox = CHARGE_BOXES[direction];
			
			// Don't check for future collisions and force a popup if player is hit.
			// Otherwise, if future collision is detected, don't popup.
			// NOTE: SoftCollisionBox is deprecated in favor of Interactions.InteractionBox
			//if (Physics.IsMeetingEntity(RoomControl.Player, CollisionBoxType.Soft)) {
				//Interactions.Trigger(InteractionType.PlayerContact, RoomControl.Player);
				//return;
			//}
			Vector2F nextVelocity = Angles.ToVector(moveAngle) * moveSpeed;

			// Avoid moving into a hazardous or solid tiles
			foreach (Tile tile in Physics.GetTilesMeeting(Position + nextVelocity)) {
				if (tile.IsHoleWaterOrLava || tile.IsSolid) {
					StopCharging();
					return;
				}
			}
			if (Physics.IsPlaceMeetingRoomEdge(Position + nextVelocity)) {
				StopCharging();
				return;
			}
		}

		private void StopCharging() {
			revealed = false;
			moveTimer = chargeCooldown;
			Physics.Velocity = Vector2F.Zero;
			Physics.CollisionBox = DEFAULT_BOX;
		}

		private void UpdateMoveState() {
			// Change direction timer
			if (moveTimer == 0) {
				moveTimer = moveDuration;
				moveAngle = GRandom.NextInt(Angles.AngleCount);
			}


			Vector2F velocity = Angles.ToVector(moveAngle) * moveSpeed;
			
			// Avoid moving into a hazardous tile
			foreach (Tile tile in Physics.GetTilesMeeting(Position + velocity)) {
				if (tile.IsHoleWaterOrLava) {
					int direction = Directions.NearestFromVector(tile.Center - Physics.PositionedCollisionBox.Center);
					int axis = Directions.ToAxis(direction);
					velocity[axis] = 0f;
					if (Physics.IsPlaceMeetingSolidTile(Position + velocity, tile)) {
						velocity = Vector2F.Zero;
						break;
					}
				}
			}

			Physics.Velocity = velocity;

			moveTimer--;
		}

		private void UpdateTargetState() {
			// Charge cooldown
			if (moveTimer > 0) {
				moveTimer--;
				return;
			}
			Physics.Velocity = Vector2F.Zero;
			Vector2F vectorToPlayer = RoomControl.Player.Center - Center;
			int direction = Directions.NearestFromVector(vectorToPlayer);
			if (direction == Direction.Up) {
				Rectangle2F box = new Rectangle2F(-8, -8, 16, 8) + Position;
				if (RoomControl.Player.Interactions.
					PositionedInteractionBox.Intersects(box))
					StartCharging(direction);
			}
			else if (direction == Direction.Down) {
				RangeF range = new RangeF(Position.X - 8, Position.X + 8);
				if (RoomControl.Player.Interactions.PositionedInteractionBox.
					LeftRight.Intersects(range))
					StartCharging(direction);
			}
			else {
				RangeF range = new RangeF(Position.Y - 12, Position.Y + 1);
				if (RoomControl.Player.Interactions.PositionedInteractionBox.
					TopBottom.Intersects(range))
					StartCharging(direction);
			}
		}

		public void Uncover(bool shouldBreak) {
			StopCharging();
			covered = false;
			SetDefaultReactions();
			Interactions.InteractionBox = new Rectangle2I(-6, -11, 12, 11);
			if (shouldBreak) {
				if (coverTile.BreakAnimation != null) {
					Effect breakEffect = new Effect(
						coverTile.BreakAnimation, coverTile.BreakLayer, true);
					RoomControl.SpawnEntity(breakEffect, Center);
				}
				if (coverTile.BreakSound != null)
					AudioSystem.PlaySound(coverTile.BreakSound);
			}
			uncoverPause = true;
			moveTimer = 64;
			Physics.Velocity = Vector2F.Zero;
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the action tile data to display in the editor.</summary>
		public static void DrawTileData(Graphics2D g, ActionDataDrawArgs args) {
			MonsterAction.DrawTileData(g, args);
			TileData cover = args.Properties.GetResource<TileData>("cover_tile");
			if (cover != null) {
				TileDataDrawing.DrawTileObject(g, cover,
					args.Position - new Point2I(0, 4), args.Zone);
			}
		}

		/// <summary>Initializes the properties and events for the action type.</summary>
		public static void InitializeTileData(ActionTileData data) {
			data.Properties.Set("cover_tile", "bush")
				.SetDocumentation("Cover Tile", "tile_data", "", "Monster", "The tile that covers the spiny beetle.");
		}

		
		//-------------------------------------------------------------------------
		// Internal Properties
		//-------------------------------------------------------------------------

		private TileFlags CoverFlags {
			get { return coverTile.Flags; }
		}

		private Properties CoverProperties {
			get { return coverTile.Properties; }
		}
	}
}
