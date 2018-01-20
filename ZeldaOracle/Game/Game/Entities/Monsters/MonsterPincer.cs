using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Entities.Monsters {
	
	public class MonsterPincer : Monster {
		
		private enum PincerState {
			Hide,
			Peek,
			Strike,
			Return,
		}
		
		private PincerState pincerState;
		private int timer;
		private Vector2F holePosition;
		private Vector2F strikeVelocity;
		private float distance;

		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterPincer() {
			// General
			MaxHealth		= 3;
			ContactDamage	= 2;
			color			= MonsterColor.Red;
			
			// Graphics
			Graphics.DrawOffset			= new Point2I(-8, -8);
			centerOffset				= new Point2I(0, 0);
			syncAnimationWithDirection	= true;

			// Physics
			Physics.HasGravity			= false;
			Physics.CollideWithWorld	= false;
			Physics.IsDestroyedInHoles	= false;
			Physics.CollisionBox		= new Rectangle2F(-6, -6, 12, 12);
			Physics.SoftCollisionBox	= Physics.CollisionBox.Inflated(-2, -2);

			// Reactions
			SetReaction(InteractionType.Gale, SenderReactions.Intercept, Reactions.None);
			SetReaction(InteractionType.GaleSeed, SenderReactions.Intercept, Reactions.None);
			SetReaction(InteractionType.SwitchHook, SenderReactions.Intercept, Reactions.Damage);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnFallInHole() {
			// Do not fall in holes
		}

		public override void Initialize() {
			base.Initialize();

			// Begin hiding
			isPassable = true;
			holePosition = position;
			Graphics.IsVisible = false;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_PINCER_EYES);
			pincerState = PincerState.Hide;
		}

		public override void UpdateAI() {
			Player player = RoomControl.Player;
			
			if (pincerState == PincerState.Return) {
				Vector2F vectorToHole = holePosition - position;
				timer++;

				// Begin returning after a short delay
				if (timer > GameSettings.MONSTER_PINCER_RETURN_DELAY)
					physics.Velocity = vectorToHole.Normalized *
						GameSettings.MONSTER_PINCER_RETURN_SPEED;

				// Check if returned to hole
				if (vectorToHole.Length < GameSettings.MONSTER_PINCER_RETURN_SPEED) {
					timer = 0;
					isPassable = true;
					position = holePosition;
					physics.Velocity = Vector2F.Zero;
					Graphics.IsVisible = false;
					pincerState = PincerState.Hide;
				}
			}
			else if (pincerState == PincerState.Strike) {
				physics.Velocity = strikeVelocity;
				distance += physics.Velocity.Length;

				// End strike after extending a certain distance
				if (distance >= GameSettings.MONSTER_PINCER_STRIKE_DISTANCE) {
					timer = 0;
					physics.Velocity = Vector2F.Zero;
					pincerState = PincerState.Return;
				}
			}
			else if (pincerState == PincerState.Peek) {
				timer++;

				// Peek for a short duration before striking
				if (timer > GameSettings.MONSTER_PINCER_PEEK_DURATION) {
					distance = 0;
					Vector2F vectorToPlayer = player.Center - Center;
					Angle = Angles.NearestFromVector(vectorToPlayer);
					strikeVelocity = Angles.ToVector(Angle) *
						GameSettings.MONSTER_PINCER_STRIKE_SPEED;
					isPassable = false;
					Graphics.PlayAnimation(GameData.ANIM_MONSTER_PINCER_HEAD);
					pincerState = PincerState.Strike;
				}
			}
			else {
				timer++;

				// Wait for player to be in range to start peeking
				if (timer > GameSettings.MONSTER_PINCER_PEEK_DELAY &&
					Center.DistanceTo(player.Center) <
					GameSettings.MONSTER_PINCER_PEEK_RANGE)
				{
					timer = 0;
					Graphics.IsVisible = true;
					Graphics.PlayAnimation(GameData.ANIM_MONSTER_PINCER_EYES);
					pincerState = PincerState.Peek;
				}
			}
		}

		public override void Update() {
			base.Update();
		}

		public override void Draw(RoomGraphics g) {
			// Draw body segments
			if (pincerState == PincerState.Strike || 
				pincerState == PincerState.Return)
			{
				for (int i = 0; i <
					GameSettings.MONSTER_PINCER_BODY_SEGMENT_COUNT; i++)
				{
					Vector2F segmentPosition = holePosition +
						((position - holePosition) *
						(i / (GameSettings.MONSTER_PINCER_BODY_SEGMENT_COUNT + 1.0f)));
					g.DrawISprite(GameData.SPR_MONSTER_PINCER_BODY_SEGMENT,
						segmentPosition, Graphics.DepthLayer);
				}
			}

			// Draw head/eyes
			base.Draw(g);
		}
	}
}
