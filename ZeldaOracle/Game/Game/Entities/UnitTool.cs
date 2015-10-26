using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;


namespace ZeldaOracle.Game.Entities {
	public class UnitTool {
		
		private Unit			unit;
		private AnimationPlayer	animationPlayer;
		private bool			drawAboveUnit;
		private Rectangle2F		collisionBox;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public UnitTool() {
			unit			= null;
			drawAboveUnit	= false;
			animationPlayer	= new AnimationPlayer();
			collisionBox	= new Rectangle2F(-1, -1, 2, 2);
		}


		
		

		//-----------------------------------------------------------------------------
		// Animation
		//-----------------------------------------------------------------------------

		public void PlayAnimation() {
			animationPlayer.Play();
		}

		public void PlayAnimation(Animation animation) {
			animationPlayer.Play(animation);
		}
		
		public void StopAnimation() {
			animationPlayer.Stop();
		}
		
		public void RemoveAnimation() {
			animationPlayer.Animation = null;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public AnimationPlayer AnimationPlayer {
			get { return animationPlayer; }
		}
		
		public bool DrawAboveUnit {
			get { return drawAboveUnit; }
			set { drawAboveUnit = value; }
		}

		public Rectangle2F CollisionBox {
			get { return collisionBox; }
			set { collisionBox = value; }
		}

		public Rectangle2F PositionedCollisionBox {
			get { return Rectangle2F.Translate(collisionBox, unit.Position); }
		}
	}
}
