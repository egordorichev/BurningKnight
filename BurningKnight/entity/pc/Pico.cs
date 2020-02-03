using BurningKnight.assets.input;
using BurningKnight.entity.component;
using BurningKnight.save;
using BurningKnight.ui.editor;
using Lens;
using Lens.graphics;
using Lens.input;
using Lens.util;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.pc {
	public class Pico : SaveableEntity, PlaceableEntity {
		private bool on;
		private Controller controller;
		private const float UpdateTime30 = 1 / 30f;
		private const float UpdateTime60 = 1 / 60f;
		private float deltaUpdate30, deltaUpdate60, deltaDraw;

		public override void PostInit() {
			base.PostInit();

			controller = new Controller();
			Area.Add(controller);

			controller.Y = Bottom + 16;
			controller.X = X + 16;
			controller.Pico = this;

		}

		public override void AddComponents() {
			base.AddComponents();

			Width = 138;
			Height = 155;
			
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Static, false));
			AddComponent(new SliceComponent("props", "pico"));
			AddComponent(new ShadowComponent());
		}

		public void TurnOn() {
			if (on) {
				return;
			}
			
			Log.Info("Turning on");
			
			on = true;
			Input.Blocked++;

			
		}

		public void TurnOff() {
			if (!on) {
				return;
			}
			
			Log.Info("Turning off");

			on = false;
			Input.Blocked--;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (!on) {
				return;
			}

			if (Input.WasPressed(Controls.Pause, GamepadComponent.Current, true)) {
				TurnOff();

				return;
			}
		}
	}
}