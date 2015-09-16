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
	
		private int				direction;
		private int				angle;
		private Item[]			equippedItems; // TODO: move this to somewhere else.

		private PlayerState		state;
		private PlayerNormalState stateNormal;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Player() : base() {
			direction		= Directions.Down;
			angle			= Directions.ToAngle(direction);
			equippedItems	= new Item[2] { null, null };

			// Physics.
			Physics.CollideWithWorld = true;
			Physics.HasGravity = true;

			// DEBUG: equip a bow item.
			equippedItems[0] = new ItemBow();

			state = null;
			stateNormal = new PlayerNormalState();
			

			Graphics.ShadowDrawOffset = new Point2I(0, -2);
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
					if (i == 0 && Controls.A.IsPressed())
						equippedItems[i].OnButtonPress();
					else if (i == 1 && Controls.B.IsPressed())
						equippedItems[i].OnButtonPress();
					//equippedItems[i].Update();
				}
			}
		}

		public override void Update() {

			// Update the current player state.
			state.Update();

			Graphics.SubStripIndex = direction;

			// Update superclass.
			base.Update();
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
		
		public PlayerNormalState NormalState {
			get { return stateNormal; }
		}
	}
}
