using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;

namespace ZeldaOracle.Game.Entities.Players {
	
	public class Player : Unit {
	
		private Keys[]			moveKeys;
		private bool[]			moveAxes;
		private bool			isMoving;
		private int				direction;
		private int				angle;
		private int				pushTimer;
		private float			moveSpeedScale;
		private float			moveSpeed;
		private Item[]			equippedItems; // TODO: move this to somewhere else.
		private bool			isBusy;

		private PlayerState		state;
		private PlayerNormalState stateNormal;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Player() {
			moveKeys = new Keys[4];
			moveAxes		= new bool[] { false, false };
			direction		= Directions.Down;
			angle			= Directions.ToAngle(direction);
			pushTimer		= 0;
			isMoving		= false;
			moveSpeed		= GameSettings.PLAYER_MOVE_SPEED;
			moveSpeedScale	= 1.0f;
			equippedItems	= new Item[2] { null, null };
			isBusy			= false;

			// Physics.
			Physics.CollideWithWorld = true;
			Physics.HasGravity = true;

			// Controls.
			moveKeys[Directions.Up]		= Keys.Up;
			moveKeys[Directions.Down]	= Keys.Down;
			moveKeys[Directions.Left]	= Keys.Left;
			moveKeys[Directions.Right]	= Keys.Right;

			// DEBUG: equip a bow item.
			equippedItems[0] = new ItemBow();

			state = null;
			stateNormal = new PlayerNormalState();
		}


		//-----------------------------------------------------------------------------
		// Player states
		//-----------------------------------------------------------------------------

		public void BeginState(PlayerState state) {
			if (this.state != null)
				this.state.End();
			state.Begin(this);
			this.state = state;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void Initialize() {
			base.Initialize();

			// Play the default player animation.
			Graphics.PlayAnimation(GameData.ANIM_PLAYER_DEFAULT);

			BeginState(stateNormal);
		}

		public void UpdateEquippedItems() {
			for (int i = 0; i < equippedItems.Length; i++) {
				if (equippedItems[i] != null) {
					equippedItems[i].Player = this;
					equippedItems[i].Update();
				}
			}
		}

		public override void Update(float ticks) {

			// Update the current player state.
			state.Update();

			Graphics.SubStripIndex = direction;

			// Update superclass.
			base.Update(ticks);
		}

		public override void Draw(Graphics2D g) {
			base.Draw(g);
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int Angle {
			get { return angle; }
			set { angle = value; }
		}
		
		public int Direction {
			get { return direction; }
			set { direction = value; }
		}
		
		public bool IsBusy {
			get { return isBusy; }
			set { isBusy = value; }
		}
		
		public PlayerNormalState NormalState {
			get { return stateNormal; }
		}
	}
}
