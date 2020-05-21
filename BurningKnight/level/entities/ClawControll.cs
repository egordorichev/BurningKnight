using System;
using BurningKnight.assets;
using BurningKnight.assets.input;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.fx;
using Lens;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.input;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities {
	public class ClawControll : Prop {
		private Claw claw;
		private Entity interacting;
		
		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new InteractableSliceComponent("props", "claw_controll"));
			AddComponent(new RectBodyComponent(0, 0, 13, 14, BodyType.Static));
			AddComponent(new SensorBodyComponent(-4, -4, 13 + 8, 14 + 8, BodyType.Static));
			AddComponent(new InteractableComponent(Interact));
			AddComponent(new ShadowComponent());
		}

		private InteractFx ia;
		private InteractFx ib;

		private bool Interact(Entity e) {
			e.RemoveComponent<PlayerInputComponent>();
			interacting = e;
			interacting.GetComponent<StateComponent>().Become<Player.SittingState>();
			
			var region = CommonAse.Ui.GetSlice("key_right");
			Engine.Instance.State.Ui.Add(ia = new InteractFx(this, null, region, -5));
			region = CommonAse.Ui.GetSlice("button_x");
			Engine.Instance.State.Ui.Add(ib = new InteractFx(this, null, region, 5));
			
			return false;
		}

		private bool grabbing;

		public override void Update(float dt) {
			base.Update(dt);

			if (interacting == null || grabbing) {
				return;
			}

			var controller = GamepadComponent.Current;

			if (Input.WasPressed(Controls.UiSelect, controller)) {
				grabbing = true;

				ia.Close();
				ib.Close();
				ia = ib = null;

				claw.Grab(() => {
					interacting.AddComponent(new PlayerInputComponent());
					interacting = null;
					grabbing = false;
				});

				return;
			}

			var body = claw.GetComponent<SensorBodyComponent>().Body;
			var speed = 420 * dt;

			/*if (Input.IsDown(Controls.Left, controller)) {
				body.LinearVelocity -= new Vector2(speed, 0);
			}*/

			if (Input.IsDown(Controls.Right, controller)) {
				body.LinearVelocity += new Vector2(speed, 0);
			}

			if (claw.X > claw.start.X + 128) {
				claw.X = claw.start.X + 128;
			}

			/*if (Input.IsDown(Controls.Up, controller)) {
				body.LinearVelocity -= new Vector2(0, speed);
			}

			if (Input.IsDown(Controls.Down, controller)) {
				body.LinearVelocity += new Vector2(0, speed);
			}*/
		}

		public override void PostInit() {
			base.PostInit();

			claw = new Claw {
				Position = new Vector2(((int) Math.Floor(CenterX / 16) - 0.5f) * 16, (int) (Math.Floor(CenterY / 16) - 3) * 16)
			};

			Area.Add(claw);
		}
	}
}