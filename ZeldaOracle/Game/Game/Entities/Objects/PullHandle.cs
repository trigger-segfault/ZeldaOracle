using System;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Items.Weapons;

namespace ZeldaOracle.Game.Entities.Objects {

	public class PullHandle : Entity {

		private float extendDistance;
		private float maxExtendLength;
		private Direction direction;
		private bool isBeingPulled;
		private float retractSpeed;
		private float extendSpeed;
		private Vector2F startPosition;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PullHandle() {
			// Graphics
			Graphics.DepthLayer	= DepthLayer.TileLayer3;
			Graphics.DrawOffset	= new Point2I(-8, -8);
			centerOffset		= new Point2I(0, 0);

			// Physics
			Physics.Enable(
				PhysicsFlags.DisableSurfaceContact |
				PhysicsFlags.Solid);
			physics.CollisionBox = new Rectangle2I(-4, -4, 8, 8);
			Physics.CollisionStyle = CollisionStyle.Circular;

			// Interactions
			Interactions.Enable();
			Interactions.InteractionBox = new Rectangle2I(-4, -4, 8, 8);
			Reactions[InteractionType.Bracelet].Set(GrabReaction);
			
			// Pull Handle
			extendSpeed		= GameSettings.TILE_PULL_HANDLE_EXTEND_SPEED;
			retractSpeed	= GameSettings.TILE_PULL_HANDLE_RETRACT_SPEED;
			maxExtendLength	= GameSettings.TILE_PULL_HANDLE_EXTEND_LENGTH;
		}
		

		//-----------------------------------------------------------------------------
		// Reaction Callbacks
		//-----------------------------------------------------------------------------

		/// <summary>Called when the player grabs the pull-handle with the bracelet.
		/// </summary>
		private void GrabReaction(Entity entity, EventArgs args) {
			Player player = RoomControl.Player;
			WeaponInteractionEventArgs weaponArgs = (WeaponInteractionEventArgs) args;
			
			if (player.Direction.Reverse() == direction) {
				player.PullHandleState.Bracelet =
					(ItemBracelet) weaponArgs.Weapon;
				player.PullHandleState.PullHandle = this;
				player.BeginControlState(player.PullHandleState);
				isBeingPulled = true;
			}
		}

		
		//-----------------------------------------------------------------------------
		// Handle Extend/Retract
		//-----------------------------------------------------------------------------

		/// <summary>Extend the handle outward by the given distance.</summary>
		public void Extend(float amount) {
			bool wasFullyExtended = IsFullyExtended;
			SetLength(extendDistance + amount);
			if (!wasFullyExtended) {
				GameControl.FireEvent(this, "extending");
				if (IsFullyExtended)
					GameControl.FireEvent(this, "fully_extend");
			}
		}

		/// <summary>Retract the handle inward by the given distance.</summary>
		public void Retract(float amount) {
			bool wasFullyRetracted = IsFullyRetracted;
			SetLength(extendDistance - amount);
			if (!wasFullyRetracted) {
				GameControl.FireEvent(this, "retracting");
				if (IsFullyRetracted)
					GameControl.FireEvent(this, "fully_retract");
			}
		}

		/// <summary>Set the current extended length.</summary>
		private void SetLength(float length) {
			extendDistance = GMath.Clamp(length, 0.0f, maxExtendLength);
			position = startPosition + direction.ToVector(extendDistance);
		}

		
		//-----------------------------------------------------------------------------
		// Player Interaction
		//-----------------------------------------------------------------------------

		/// <summary>Called by the player's PullHandleState to notify the pull handle
		/// that the player has stopped grabbing/pulling on the handle.</summary>
		public void OnReleaseHandle() {
			isBeingPulled = false;
		}

		/// <summary>Get the position the player should be at when grabbing this
		/// handle.</summary>
		public Vector2F GetPlayerPullPosition() {
			Vector2F pullPosition = Center + direction.ToVector(10);
			if (direction.IsHorizontal)
				pullPosition.Y -= 2;
			return pullPosition;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			startPosition = position;

			direction		= Properties.Get<int>("direction", Direction.Down);
			extendDistance	= 0.0f;
			isBeingPulled	= false;

			Rectangle2I interactionBox = new Rectangle2I(-4, -4, 8, 8);
			interactionBox.ExtendEdge(direction, 5);
			Interactions.InteractionBox = interactionBox;
			Physics.CollisionBox = interactionBox;
			
			Graphics.PlayAnimation(GameData.ANIM_TILE_PULL_HANDLE);
			Graphics.SubStripIndex = direction.Index;
		}

		public override void Update() {
			// Automatically retract when not being pulled
			if (!isBeingPulled && !IsFullyRetracted)
				Retract(retractSpeed);

			base.Update();
		}

		public override void Draw(RoomGraphics g) {
			// Draw the connecting rod
			if (extendDistance > 0.0f) {
				ISprite spr;
				if (direction.IsHorizontal)
					spr = GameData.SPR_TILE_PULL_HANDLE_BAR_HORIZONTAL;
				else
					spr = GameData.SPR_TILE_PULL_HANDLE_BAR_VERTICAL;

				for (float length = 0.0f; length < extendDistance;
					length += GameSettings.TILE_SIZE)
				{
					Vector2F drawPos = startPosition + direction.ToVector(length);
					g.DrawSprite(spr, drawPos, Graphics.CurrentDepthLayer);
				}
			}
			
			base.Draw(g);
		}
		

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public float ExtendSpeed {
			get { return extendSpeed; }
		}

		public float RetractSpeed {
			get { return retractSpeed; }
		}

		public bool IsFullyExtended {
			get { return (extendDistance >= maxExtendLength); }
		}

		public bool IsFullyRetracted {
			get { return (extendDistance <= 0.0f); }
		}

		public float ExtendDistance {
			get { return extendDistance; }
		}
	}
}
