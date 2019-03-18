using BurningKnight.entity.component;
using Lens.entity;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.item.util {
	public class MeleeArc : Entity {
		public float LifeTime = 0.1f;
		public int Damage;
		public Entity Owner;
		public float Angle;

		private float t;

		public override void AddComponents() {
			base.AddComponents();

			Width = 32;
			Height = 24;

			AddComponent(new RectBodyComponent(0, -Height / 2f, Width, Height, BodyType.Dynamic, true) {
				Angle = Angle
			});
		}

		public override void Update(float dt) {
			base.Update(dt);

			t += dt;

			if (t >= LifeTime) {
				Done = true;
			}
		}
	}
}