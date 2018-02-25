using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles.MonsterProjectiles;
using ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles;
using ZeldaOracle.Game.Tiles;

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
			Physics.SoftCollisionBox	= new Rectangle2I(-4, -11, 8, 6);
			Physics.BraceletCollisionBox	= new Rectangle2I(-6, -12, 12, 12);

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
				SetReaction(InteractionType.Sword,			SenderReactions.Intercept,	Reactions.DamageByLevel(1, 2, 3), BreakCover);
				SetReaction(InteractionType.SwordSpin,		Reactions.Damage2,			BreakCover);
				SetReaction(InteractionType.BiggoronSword,	Reactions.Damage3,			BreakCover);
				SetReaction(InteractionType.SwordBeam,		SenderReactions.Destroy,	Reactions.Damage, BreakCover);
				SetReaction(InteractionType.ThrownObject,	Reactions.Damage);
			}
			else {
				SetReaction(InteractionType.Sword,			Reactions.ClingEffect);
				SetReaction(InteractionType.SwordSpin,		Reactions.ClingEffect);
				SetReaction(InteractionType.BiggoronSword,	Reactions.ClingEffect);
				SetReaction(InteractionType.SwordBeam,		SenderReactions.Intercept);
				SetReaction(InteractionType.ThrownObject,	Reactions.None);
			}

			int boomerangLevel = -1;
			if (GameControl.Inventory.ItemExists("item_boomerang"))
				boomerangLevel = GameControl.Inventory.GetItem("item_boomerang").Level;
			if (CoverFlags.HasFlag(TileFlags.Boomerangable) && boomerangLevel >= Items.Item.Level2) {
				SetReaction(InteractionType.Boomerang,		SenderReactions.Intercept,	Reactions.Stun, BreakCover);
			}
			else {
				SetReaction(InteractionType.Boomerang,		SenderReactions.Intercept);
			}

			if (CoverFlags.HasFlag(TileFlags.Burnable)) {
				SetReaction(InteractionType.Fire,			Reactions.Burn);
			}
			else {
				// TODO: Flames need to go out quickly
				SetReaction(InteractionType.Fire,			SenderReactions.Intercept);
			}

			if (CoverFlags.HasFlag(TileFlags.Bombable)) {
				SetReaction(InteractionType.BombExplosion,	Reactions.Damage, BreakCover);
			}
			else {
				SetReaction(InteractionType.BombExplosion,	Reactions.None);
			}

			SetReaction(InteractionType.Shield,			SenderReactions.Bump,		Reactions.Bump);
			SetReaction(InteractionType.Shovel,			Reactions.Bump);
			SetReaction(InteractionType.Parry,			Reactions.Parry);
			SetReaction(InteractionType.Bracelet,			PickupInteraction);
			// Seed interations
			SetReaction(InteractionType.EmberSeed,		SenderReactions.Intercept);
			SetReaction(InteractionType.ScentSeed,		SenderReactions.Intercept);
			SetReaction(InteractionType.PegasusSeed,	SenderReactions.Intercept);
			SetReaction(InteractionType.GaleSeed,		SenderReactions.Intercept);
			SetReaction(InteractionType.MysterySeed,	Reactions.MysterySeed);
			// Projectile interations
			SetReaction(InteractionType.Arrow,			SenderReactions.Intercept);
			SetReaction(InteractionType.RodFire,		SenderReactions.Intercept);
			SetReaction(InteractionType.SwitchHook,		SwitchHookInteraction);
			// Environment interations
			SetReaction(InteractionType.Gale,			SenderReactions.Intercept);
			SetReaction(InteractionType.MineCart,		Reactions.SoftKill);
			SetReaction(InteractionType.Block,			Reactions.None);
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

		private void SwitchHookInteraction(Monster monster, Entity sender, EventArgs args) {
			SwitchHookProjectile hook = sender as SwitchHookProjectile;
			carriedTile.Position = Center + new Vector2F(0, 4);
			if (revealed)
				carriedTile.ZPosition = 4;
			// Prevent the spawned carried tile from hurting the monster.
			carriedTile.DisablePhysics();
			// Spawn the entity so it draws during the pause before switching.
			RoomControl.SpawnEntity(carriedTile);
			hook.SwitchWithEntity(carriedTile);
			Uncover(false);
		}

		private void PickupInteraction(Monster monster, Entity sender, EventArgs args) {
			RoomControl.Player.CarryState.SetCarryObject(coverTile);
			RoomControl.Player.BeginWeaponState(RoomControl.Player.CarryState);
			Uncover(false);
		}

		private void BreakCover(Monster monster, Entity sender, EventArgs args) {
			Uncover(true);
		}

		private void BurnCover(Monster monster, Entity sender, EventArgs args) {
			// TODO: Detach and burn cover tile
			// (Monster keeps charging (most likely untill burned up))
			BreakCover(monster, sender, args);
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
				if (direction == Directions.Left || direction == Directions.Up) {
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
				Position + Physics.Velocity, CollisionBoxType.Hard))
			{
				if (tile.IsHoleWaterOrLava) {
					if (direction == Directions.Left || direction == Directions.Up) {
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
			if (Physics.IsMeetingEntity(RoomControl.Player, CollisionBoxType.Soft)) {
				TriggerInteraction(InteractionType.PlayerContact, RoomControl.Player);
				return;
			}
			Vector2F nextVelocity = Angles.ToVector(moveAngle) * moveSpeed;

			// Avoid moving into a hazardous or solid tiles
			foreach (Tile tile in Physics.GetTilesMeeting(
				Position + nextVelocity, CollisionBoxType.Hard))
			{
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
			foreach (Tile tile in Physics.GetTilesMeeting(
				Position + velocity, CollisionBoxType.Hard))
			{
				if (tile.IsHoleWaterOrLava) {
					int direction = Directions.NearestFromVector(tile.Center - Physics.PositionedCollisionBox.Center);
					int axis = Directions.ToAxis(direction);
					velocity[axis] = 0f;
					if (Physics.IsPlaceMeetingTile(Position + velocity, tile)) {
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
			if (direction == Directions.Up) {
				Rectangle2F box = new Rectangle2F(-8, -8, 16, 8) + Position;
				if (RoomControl.Player.Physics.
					PositionedSoftCollisionBox.Intersects(box))
					StartCharging(direction);
			}
			else if (direction == Directions.Down) {
				RangeF range = new RangeF(Position.X - 8, Position.X + 8);
				if (RoomControl.Player.Physics.PositionedSoftCollisionBox.
					LeftRight.Intersects(range))
					StartCharging(direction);
			}
			else {
				RangeF range = new RangeF(Position.Y - 12, Position.Y + 1);
				if (RoomControl.Player.Physics.PositionedSoftCollisionBox.
					TopBottom.Intersects(range))
					StartCharging(direction);
			}
		}

		public void Uncover(bool shouldBreak) {
			StopCharging();
			covered = false;
			SetDefaultReactions();
			Physics.SoftCollisionBox    = new Rectangle2I(-6, -11, 12, 11);
			if (shouldBreak) {
				if (coverTile.BreakAnimation != null) {
					Effect breakEffect = new Effect(coverTile.BreakAnimation, coverTile.BreakLayer, true);
					RoomControl.SpawnEntity(breakEffect, Center);
				}
				if (coverTile.BreakSound != null)
					AudioSystem.PlaySound(coverTile.BreakSound);
			}
			uncoverPause = true;
			moveTimer = 64;
			Physics.Velocity = Vector2F.Zero;
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
