using BurningKnight.assets.input;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.input;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities {
	public class ClawControll : Prop {
		private Claw claw;
		private Entity interacting;
		
		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new InteractableSliceComponent("props", "claw_controll"));
			AddComponent(new SensorBodyComponent(-4, -4, 13 + 8, 14 + 8));
			AddComponent(new InteractableComponent(Interact));
			AddComponent(new ShadowComponent());
		}

		private bool Interact(Entity e) {
			e.RemoveComponent<PlayerInputComponent>();
			interacting = e;
			interacting.GetComponent<StateComponent>().Become<Player.SittingState>();

			return true;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (interacting == null) {
				return;
			}

			var controller = GamepadComponent.Current;

			if (Input.WasPressed(Controls.UiAccept, controller) || Input.WasPressed(Controls.UiSelect, controller)) {
				interacting.AddComponent(new PlayerInputComponent());
				interacting = null;

				return;
			}

			var body = claw.GetComponent<SensorBodyComponent>().Body;
			var speed = 360 * dt;

			if (Input.IsDown(Controls.Left, controller)) {
				body.LinearVelocity -= new Vector2(speed, 0);
			}

			if (Input.IsDown(Controls.Right, controller)) {
				body.LinearVelocity += new Vector2(speed, 0);
			}

			if (Input.IsDown(Controls.Up, controller)) {
				body.LinearVelocity -= new Vector2(0, speed);
			}

			if (Input.IsDown(Controls.Down, controller)) {
				body.LinearVelocity += new Vector2(0, speed);
			}
		}

		public override void PostInit() {
			base.PostInit();

			claw = new Claw {
				Position = Center - new Vector2(0, 64)
			};

			Area.Add(claw);
		}
	}
}