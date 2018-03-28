using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Control.VisualEffects {
	/// <summary>The abstract class for all visual effects in the room such as being
	/// underwater or using the Harp of Ages to warp into a forbidden area.</summary>
	public abstract class RoomVisualEffect {
		
		/// <summary>The current room control.</summary>
		private RoomControl roomControl;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the room visual effect.</summary>
		protected RoomVisualEffect() {
			roomControl		= null;
		}
		

		//-----------------------------------------------------------------------------
		// Virtual Methods
		//-----------------------------------------------------------------------------

		/// <summary>Updates the visual effect's state.</summary>
		public virtual void Update() { }

		/// <summary>Begins tile drawing for the visual effect. This is called once for
		/// drawing regular tiles and another time for drawing tiles' above sprites.</summary>
		public virtual void Begin(Graphics2D g, Vector2F position) {
			g.End();
			g.PushTranslation(-position);
			g.SetRenderTarget(GameData.RenderTargetGameTemp);
			g.Clear(Color.Transparent);
			g.Begin(GameSettings.DRAW_MODE_PALLETE);
		}

		/// <summary>Ends tile drawing for the visual effect. This is called once for
		/// drawing regular tiles and another time for drawing tiles' above sprites.</summary>
		public virtual void End(Graphics2D g, Vector2F position) {
			g.End();
			g.PopTranslation(); // -position
			g.SetRenderTarget(GameData.RenderTargetGame);
			g.Begin(GameSettings.DRAW_MODE_PALLETE);
			g.PushTranslation(-ViewPosition);
			Render(g, GameData.RenderTargetGameTemp);
			g.PopTranslation(); // -ViewPosition
		}

		/// <summary>Renders the visual effect after all drawing is completed.
		/// By default this is called in RoomVisualEffect.End.</summary>
		public virtual void Render(Graphics2D g, Texture2D roomImage) { }


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the room control for the visual effect.</summary>
		public RoomControl RoomControl {
			get { return roomControl; }
			set { roomControl = value; }
		}

		/// <summary>Gets the room's view control.</summary>
		public ViewControl ViewControl {
			get { return roomControl.ViewControl; }
		}

		/// <summary>Gets the view position of the room's view control.</summary>
		public Vector2F ViewPosition {
			get { return -GameUtil.Bias(roomControl.ViewControl.Camera.TopLeft); }
		}
	}
}
