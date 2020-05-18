using BurningKnight.entity.component;
using Lens.entity;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.pc {
	public class Controller : Entity {
		public Pico Pico;
		
		public override void AddComponents() {
			base.AddComponents();

			Height = 9;

			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Static, false));
			AddComponent(new SensorBodyComponent(-10, -10, Width + 20, Height + 20, BodyType.Static, true));
			AddComponent(new InteractableComponent(Interact));
			AddComponent(new InteractableSliceComponent("props", "controller"));
			AddComponent(new ShadowComponent());
		}

		private bool Interact(Entity e) {
			Pico.TurnOn();
			return true;
		}
	}
}