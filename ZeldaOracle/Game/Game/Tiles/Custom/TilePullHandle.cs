using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Tiles {

	public class TilePullHandle : Tile, ZeldaAPI.PullHandle {

		private float extendDistance;
		private float maxExtendLength;
		private int direction;
		private bool isBeingPulled;
		private float retractSpeed;
		private float extendSpeed;
		
		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TilePullHandle() {
			extendSpeed		= GameSettings.TILE_PULL_HANDLE_EXTEND_SPEED;
			retractSpeed	= GameSettings.TILE_PULL_HANDLE_RETRACT_SPEED;
			maxExtendLength	= GameSettings.TILE_PULL_HANDLE_EXTEND_LENGTH;
		}

		
		//-----------------------------------------------------------------------------
		// Handle Extend/Retract
		//-----------------------------------------------------------------------------

		public void Extend(float amount) {
			bool wasFullyExtended = IsFullyExtended;
			SetLength(extendDistance + amount);
			if (!wasFullyExtended) {
				GameControl.FireEvent(this, "extending", this);
				if (IsFullyExtended)
					GameControl.FireEvent(this, "fully_extend", this);
			}
		}

		public void Retract(float amount) {
			bool wasFullyRetracted = IsFullyRetracted;
			SetLength(extendDistance - amount);
			if (!wasFullyRetracted) {
				GameControl.FireEvent(this, "retracting", this);
				if (IsFullyRetracted)
					GameControl.FireEvent(this, "fully_retract", this);
			}
		}

		private void SetLength(float length) {
			extendDistance = GMath.Clamp(length, 0.0f, maxExtendLength);
			Offset = Directions.ToVector(direction) *
				(extendDistance + GameSettings.TILE_PULL_HANDLE_WALL_OFFSET);
		}

		
		//-----------------------------------------------------------------------------
		// Player Interaction
		//-----------------------------------------------------------------------------

		public void EndPull() {
			isBeingPulled = false;
		}

		public Vector2F GetPlayerPullPosition() {
			Vector2F pos = Position + CollisionModel.Boxes[0].Center + (Directions.ToVector(direction) * 10);
			if (Directions.IsHorizontal(direction))
				pos.Y -= 2;
			return pos;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			base.OnInitialize();

			direction		= Properties.GetInteger("direction", Directions.Down);
			extendDistance	= 0.0f;
			isBeingPulled	= false;

			SetLength(0.0f);

			CollisionStyle		= CollisionStyle.Circular;
			IsSolid				= true;
			ClingWhenStabbed	= false;
			SolidType			= TileSolidType.HalfSolid;
			Rectangle2I collisionBox = new Rectangle2I(4, 4, 8, 8);
			collisionBox.ExtendEdge(direction, 5);
			CollisionModel = new CollisionModel(collisionBox);

			if (direction == Directions.Right)
				Graphics.PlayAnimation(GameData.SPR_TILE_PULL_HANDLE_RIGHT);
			else if (direction == Directions.Up)
				Graphics.PlayAnimation(GameData.SPR_TILE_PULL_HANDLE_UP);
			else if (direction == Directions.Left)
				Graphics.PlayAnimation(GameData.SPR_TILE_PULL_HANDLE_LEFT);
			else if (direction == Directions.Down)
				Graphics.PlayAnimation(GameData.SPR_TILE_PULL_HANDLE_DOWN);
		}

		public override void OnGrab(int direction, ItemBracelet bracelet) {
			if (direction == Directions.Reverse(this.direction)) {
				Player player = RoomControl.Player;
				player.PullHandleState.Bracelet			= bracelet;
				player.PullHandleState.PullHandleTile	= this;
				player.BeginControlState(player.PullHandleState);
				isBeingPulled = true;
			}
		}

		public override void Update() {

			// Automatically retract when not being pulled.
			if (!isBeingPulled && !IsFullyRetracted) {
				Retract(retractSpeed);
			}

			base.Update();
		}

		public override void Draw(RoomGraphics g) {
			// Draw the extension bar.
			if (extendDistance > 0.0f) {
				ISprite spr;
				if (Directions.IsHorizontal(direction))
					spr = GameData.SPR_TILE_PULL_HANDLE_BAR_HORIZONTAL;
				else
					spr = GameData.SPR_TILE_PULL_HANDLE_BAR_VERTICAL;

				for (float length = 0.0f; length < extendDistance; length += GameSettings.TILE_SIZE) {
					Vector2F drawPos = Position - Offset + (Directions.ToVector(direction) * (length + 8.0f));
					g.DrawSprite(spr, drawPos, DepthLayer.TileLayer1);
				}
			}
			
			base.Draw(g);
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			int direction = args.Properties.GetInteger("direction", Directions.Down);
			ISprite sprite = null;
			if (direction == Directions.Right)
				sprite = GameData.SPR_TILE_PULL_HANDLE_RIGHT;
			else if (direction == Directions.Up)
				sprite = GameData.SPR_TILE_PULL_HANDLE_UP;
			else if (direction == Directions.Left)
				sprite = GameData.SPR_TILE_PULL_HANDLE_LEFT;
			else if (direction == Directions.Down)
				sprite = GameData.SPR_TILE_PULL_HANDLE_DOWN;
			g.DrawSprite(
				sprite,
				args.SpriteDrawSettings,
				args.Position,
				args.Color);
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
