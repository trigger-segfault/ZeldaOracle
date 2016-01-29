using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;

namespace ZeldaOracle.Game.Tiles {

	public class TilePullHandle : Tile {

		private float extendLength;
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
		// Pull Handle Methods
		//-----------------------------------------------------------------------------

		public void Extend(float amount) {
			SetLength(extendLength + amount);
		}

		public void Retract(float amount) {
			SetLength(extendLength - amount);
		}

		public void SetLength(float length) {
			extendLength = GMath.Clamp(length, 0.0f, maxExtendLength);
			Offset = Directions.ToVector(direction) *
				(extendLength + GameSettings.TILE_PULL_HANDLE_WALL_OFFSET);
		}

		public void EndPull() {
			isBeingPulled = false;
		}

		public Vector2F GetPlayerPullPosition() {
			return Center + (Directions.ToVector(direction) * 17);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			base.OnInitialize();

			direction		= Properties.GetInteger("direction", Directions.Down);
			extendLength	= 0.0f;
			isBeingPulled	= false;

			SetLength(0.0f);

			IsSolid				= true;
			ClingWhenStabbed	= false;

			Rectangle2I collisionBox = new Rectangle2I(0, 0, 16, 16);
			collisionBox.ExtendEdge(direction, 1);
			CollisionModel = new CollisionModel(collisionBox);

			if (direction == Directions.Right)
				CustomSprite = GameData.SPR_TILE_PULL_HANDLE_RIGHT;
			else if (direction == Directions.Up)
				CustomSprite = GameData.SPR_TILE_PULL_HANDLE_UP;
			else if (direction == Directions.Left)
				CustomSprite = GameData.SPR_TILE_PULL_HANDLE_LEFT;
			else if (direction == Directions.Down)
				CustomSprite = GameData.SPR_TILE_PULL_HANDLE_DOWN;
		}

		public override void OnGrab(int direction, ItemBracelet bracelet) {
			if (direction == Directions.Reverse(this.direction)) {
				Player player = RoomControl.Player;
				player.PullHandleState.Bracelet			= bracelet;
				player.PullHandleState.PullHandleTile	= this;
				player.BeginState(player.PullHandleState);
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

		public override void Draw(Graphics2D g) {
			// Draw the extension bar.
			if (extendLength > 0.0f) {
				Sprite spr;
				if (Directions.IsHorizontal(direction))
					spr = GameData.SPR_TILE_PULL_HANDLE_BAR_HORIZONTAL;
				else
					spr = GameData.SPR_TILE_PULL_HANDLE_BAR_VERTICAL;

				for (float length = 0.0f; length < extendLength; length += GameSettings.TILE_SIZE) {
					Vector2F drawPos = Position - Offset + (Directions.ToVector(direction) * (length + 8.0f));
					g.DrawSprite(spr, drawPos);
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
			get { return (extendLength >= maxExtendLength); }
		}

		public bool IsFullyRetracted {
			get { return (extendLength <= 0.0f); }
		}
	}
}
