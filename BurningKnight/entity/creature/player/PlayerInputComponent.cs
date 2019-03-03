using BurningKnight.entity.component;
using Lens.entity.component;
using Lens.entity.component.logic;
using Lens.input;
using Lens.util;
using Microsoft.Xna.Framework.Input;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.player {
	public class PlayerInputComponent : Component {
		public const string Up = "up";
		public const string Left = "left";
		public const string Down = "down";
		public const string Right = "right";
		public const string Active = "active";
		public const string Swap = "swap";
		public const string Use = "use";
		public const string Roll = "roll";

		private const float Speed = 25f;
		
		public PlayerInputComponent() {
			Input.Bind(Up, Keys.W);
			Input.Bind(Left, Keys.A);
			Input.Bind(Down, Keys.S);
			Input.Bind(Right, Keys.D);
			
			Input.Bind(Active, Keys.Space);
			Input.Bind(Swap, Keys.LeftShift);
			
			Input.Bind(Use, MouseButtons.Left);
			Input.Bind(Roll, MouseButtons.Right);
		}

		public override void Update(float dt) {
			base.Update(dt);

			var body = GetComponent<NoCornerBodyComponent>();

			body.Acceleration.X = 0;
			body.Acceleration.Y = 0;

			if (Input.IsDown(Up)) {
				body.Acceleration.Y -= Speed;
			}
			
			if (Input.IsDown(Down)) {
				body.Acceleration.Y += Speed;
			}
			
			if (Input.IsDown(Left)) {
				body.Acceleration.X -= Speed;
			}
			
			if (Input.IsDown(Right)) {
				body.Acceleration.X += Speed;
			}

			body.Velocity -= body.Velocity * dt * 10;

			if (body.Acceleration.Length() > 0.1f) {
				Entity.GetComponent<StateComponent>().Become<Player.RunState>();
			} else {
				Entity.GetComponent<StateComponent>().Become<Player.IdleState>();
			}
		}
	}
}