using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Util;

namespace ZeldaOracle.Game.Entities.Monsters {
	
	public class MonsterPincer : Monster {
		
		private enum PincerState {
			Hide,
			Peek,
			Strike,
			StrikePause,
			Return,
		}
		
		private Vector2F holePosition;
		private Vector2F strikeVelocity;
		private float distance;
		private Angle angle;
		private bool drawBody;
		private int timer;
		private GenericStateMachine<PincerState> stateMachine;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterPincer() {
			// General
			MaxHealth		= 3;
			ContactDamage	= 2;
			Color			= MonsterColor.Red;
			
			// Graphics
			Graphics.DrawOffset			= new Point2I(-8, -8);
			centerOffset				= new Point2I(0, 0);
			syncAnimationWithDirection	= false;

			// Physics
			Physics.HasGravity				= false;
			Physics.CollideWithWorld		= false;
			Physics.DisableSurfaceContact	= true;
			Physics.CollisionBox			= new Rectangle2F(-6, -6, 12, 12);

			// Interactions
			Interactions.InteractionBox = Physics.CollisionBox.Inflated(-2, -2);
			// Reactions
			Interactions.SetReaction(InteractionType.Gale, SenderReactions.Intercept, Reactions.None);
			Interactions.SetReaction(InteractionType.GaleSeed, SenderReactions.Intercept, Reactions.None);
			Interactions.SetReaction(InteractionType.SwitchHook, SenderReactions.Intercept, Reactions.Damage);

			// State Machine
			stateMachine = new GenericStateMachine<PincerState>();
			stateMachine.AddState(PincerState.Hide)
				.OnBegin(OnBeginHideState)
				.OnUpdate(OnUpdateHideState);
			stateMachine.AddState(PincerState.Peek)
				.OnBegin(OnBeginPeekState)
				.SetDuration(GameSettings.MONSTER_PINCER_PEEK_DURATION);
			stateMachine.AddState(PincerState.Strike)
				.OnBegin(OnBeginStrikeState)
				.OnUpdate(OnUpdateStrikeState)
				.OnEnd(OnEndStrikeState);
			stateMachine.AddState(PincerState.StrikePause)
				.SetDuration(GameSettings.MONSTER_PINCER_RETURN_DELAY);
			stateMachine.AddState(PincerState.Return)
				.OnBegin(OnBeginReturnState)
				.OnUpdate(OnUpdateReturnState);
		}


		//-----------------------------------------------------------------------------
		// State Callbacks
		//-----------------------------------------------------------------------------

		private void OnBeginHideState() {
			IsPassable = true;
			position = holePosition;
			physics.Velocity = Vector2F.Zero;
			Graphics.IsVisible = false;
			drawBody = false;
		}

		private void OnUpdateHideState() {
			timer++;

			// Wait for player to be in range to start peeking
			if (timer > GameSettings.MONSTER_PINCER_PEEK_DELAY &&
				Center.DistanceTo(RoomControl.Player.Center) <
					GameSettings.MONSTER_PINCER_PEEK_RANGE)
			{
				stateMachine.BeginState(PincerState.Peek);
			}
		}

		private void OnBeginPeekState() {
			Graphics.IsVisible = true;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_PINCER_EYES);
		}

		private void OnBeginStrikeState() {
			Vector2F vectorToPlayer = RoomControl.Player.Center - Center;
			angle = Angle.FromVector(vectorToPlayer);
			strikeVelocity = angle.ToVector(GameSettings.MONSTER_PINCER_STRIKE_SPEED);
			IsPassable = false;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_PINCER_HEAD);
			drawBody = true;
			distance = 0;
		}

		private void OnUpdateStrikeState() {
			physics.Velocity = strikeVelocity;
			distance += physics.Velocity.Length;

			// End strike after extending a certain distance
			if (distance >= GameSettings.MONSTER_PINCER_STRIKE_DISTANCE)
				stateMachine.BeginState(PincerState.Return);
		}

		private void OnEndStrikeState() {
			physics.Velocity = Vector2F.Zero;
		}

		private void OnBeginReturnState() {
			Vector2F vectorToHole = holePosition - position;
			physics.Velocity = vectorToHole.Normalized *
				GameSettings.MONSTER_PINCER_RETURN_SPEED;
		}

		private void OnUpdateReturnState() {
			Vector2F vectorToHole = holePosition - position;
			physics.Velocity = vectorToHole.Normalized *
				GameSettings.MONSTER_PINCER_RETURN_SPEED;

			// Check if returned to hole
			if (vectorToHole.Length < GameSettings.MONSTER_PINCER_RETURN_SPEED)
				stateMachine.BeginState(PincerState.Hide);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			// Begin hiding
			holePosition = position;
			stateMachine.InitializeOnState(PincerState.Hide);
		}

		public override void UpdateAI() {
			stateMachine.Update();
			Graphics.SubStripIndex = angle;
		}

		public override void Draw(RoomGraphics g) {
			// Draw body segments
			if (drawBody) {
				for (int i = 0; i <
					GameSettings.MONSTER_PINCER_BODY_SEGMENT_COUNT; i++)
				{
					Vector2F segmentPosition = holePosition +
						((position - holePosition) *
						(i / (GameSettings.MONSTER_PINCER_BODY_SEGMENT_COUNT + 1.0f)));
					g.DrawSprite(GameData.SPR_MONSTER_PINCER_BODY_SEGMENT,
						segmentPosition, Graphics.DepthLayer);
				}
			}

			// Draw head/eyes
			base.Draw(g);
		}
	}
}
