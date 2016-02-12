using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Common.Audio;

namespace ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles {
	public class SwitchHookProjectile : Projectile {

		private float	speed;
		private int		length;
		private int		level;
		private bool	isReturning;
		private bool	isLifting;
		private bool	isHooked;
		private float	distance;
		private int		timer;
		private object	hookedObject;
		private Collectible collectible;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public SwitchHookProjectile(int level) {
			this.level = level;
			
			Physics.SoftCollisionBox = new Rectangle2F(-2, -2, 4, 4);

			speed	= GameSettings.PROJECTILE_SWITCH_HOOK_SPEEDS[level];
			length	= GameSettings.PROJECTILE_SWITCH_HOOK_LENGTHS[level];

			// General.
			syncAnimationWithDirection = true;

			// Physics.
			EnablePhysics(
				PhysicsFlags.CollideWorld |
				PhysicsFlags.CollideRoomEdge |
				PhysicsFlags.LedgePassable |
				PhysicsFlags.HalfSolidPassable);

			// Graphics.
			Graphics.DepthLayer			= DepthLayer.ProjectileSwitchHook;
			Graphics.IsShadowVisible	= false;
		}


		//-----------------------------------------------------------------------------
		// Hook Methods
		//-----------------------------------------------------------------------------
		
		public void OnSwitchPositions() {
		}

		public void SwitchWithEntity(Entity entity) {
			if (!isReturning && !isHooked && !isLifting) {
				hookedObject	= entity;
				isHooked		= true;
				timer			= 0;
				graphics.PlayAnimation();
				Physics.Velocity = Vector2F.Zero;
			}
		}

		public void BeginSwitching() {
			isLifting = true;

			Graphics.Animation = null;
			RoomControl.Player.SwitchHookState.BeginSwitch(hookedObject);
		}

		public void BeginReturn(bool grabbing) {
			if (!isReturning) {
				hookedObject				= null;
				isHooked					= false;
				isReturning					= true;
				physics.CollideWithWorld	= false;
				physics.CollideWithRoomEdge	= false;
				
				Graphics.StopAnimation();
				if (grabbing)
					Graphics.AnimationPlayer.PlaybackTime = 4; // TODO: magic number
			}
		}

		public bool IsHookedObjectAlive() {
			if (hookedObject == null)
				return false;
			if (hookedObject is Entity)
				return (hookedObject as Entity).IsAlive;
			if (hookedObject is Tile)
				return (hookedObject as Tile).IsAlive;
			return true;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			if (direction == Directions.Right)
				Physics.CollisionBox = new Rectangle2F(0, 0, 2, 1);
			else if (direction == Directions.Left)
				Physics.CollisionBox = new Rectangle2F(-2, 0, 2, 1);
			else if (direction == Directions.Up)
				Physics.CollisionBox = new Rectangle2F(0, -2, 1, 2);
			else if (direction == Directions.Down)
				Physics.CollisionBox = new Rectangle2F(0, 0, 1, 4);

			collectible			= null;
			hookedObject		= null;
			isReturning			= false;
			isHooked			= false;
			isLifting			= false;
			distance			= 0;
			position			= HookStartPosition;
			physics.Velocity	= Directions.ToVector(direction) * speed;

			Graphics.Animation = GameData.ANIM_PROJECTILE_SWITCH_HOOK;
		}

		public override void Intercept() {
			BeginReturn(false);
		}

		public override void OnCollideTile(Tile tile, bool isInitialCollision) {
			if (!isReturning && !isHooked && !isLifting) {
				if (tile.IsSwitchable) {
					// TODO: Switch with tile.

					hookedObject	= tile;
					isHooked		= true;
					timer			= 0;
					graphics.PlayAnimation();
				}
				else {
					BeginReturn(false);
				}
			
				// Create cling effect.
				Effect effect = new EffectCling();
				RoomControl.SpawnEntity(effect, position + Directions.ToVector(direction) * 5.0f, zPosition);
				AudioSystem.PlaySound(GameData.SOUND_EFFECT_CLING);
			}
		}

		public override void OnCollideMonster(Monster monster) {
			if (!isReturning && !isHooked && !isLifting)
				monster.TriggerInteraction(InteractionType.SwitchHook, this);
		}

		public override void Update() {
			if (isLifting) {

			}
			else if (isReturning) {
				AudioSystem.LoopSoundWhileActive(GameData.SOUND_SWITCH_HOOK_LOOP);

				// Return to player.
				Vector2F trajectory = (RoomControl.Player.Center + new Vector2F(0, 3)) - Center;
				if (trajectory.Length <= speed) {
					if (collectible != null)
						collectible.Collect();
					Destroy();
				}
				else {
					physics.Velocity = trajectory.Normalized * speed;
				}
			}
			else if (isHooked) {
				// This is the state when latched on to on object, before lifting up.
				timer++;
				if (timer >= GameSettings.SWITCH_HOOK_LATCH_DURATION) {
					// Start lifting if object is still alive.
					if (IsHookedObjectAlive()) {
						BeginSwitching();
					}
					else {
						BeginReturn(true);
					}
				}
			}
			else {
				AudioSystem.LoopSoundWhileActive(GameData.SOUND_SWITCH_HOOK_LOOP);

				// Check for collectibles to pick up.
				foreach (Collectible c in Physics.GetEntitiesMeeting<Collectible>(CollisionBoxType.Soft)) {
					if (c.IsPickupable && c.IsCollectibleWithItems) {
						collectible = c;
						c.Destroy();
						BeginReturn(true);
					}
				}

				// Return after extending to the maximum distance.
				distance += speed;
				if (distance >= length)
					BeginReturn(false);
			}

			base.Update();

			// This should handle room edge collisions.
			if (!isReturning && !isHooked && !isLifting && physics.IsColliding) {
				BeginReturn(false);
			}
		}

		public override void Draw(RoomGraphics g) {
			base.Draw(g);

			// Draw 3 links between hook and player (alternating which one is visible).
			Vector2F hookStartPos = HookStartPosition;
			int linkIndex = (GameControl.RoomTicks % 3) + 1;
			float percent = (linkIndex / 4.0f);
			Vector2F linkPos = Vector2F.Lerp(hookStartPos, position, percent);
			g.DrawSprite(GameData.SPR_SWITCH_HOOK_LINK,
				linkPos - new Vector2F(0, zPosition), Graphics.DepthLayer);

			// Draw collectible over hook.
			if (collectible != null) {
				Vector2F pos = Center + Directions.ToVector(direction) * 4;
				collectible.SetPositionByCenter(pos);
				collectible.ZPosition = zPosition;
				collectible.Graphics.Draw(g, Graphics.CurrentDepthLayer);
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Vector2F HookStartPosition {
			get {
				if (Directions.IsHorizontal(direction))
					return RoomControl.Player.Center + new Vector2F(0, 3);
				else
					return RoomControl.Player.Center;
			}
		}

		public float Speed {
			get { return speed; }
		}
	}
}
