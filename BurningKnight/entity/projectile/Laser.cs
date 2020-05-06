using BurningKnight.entity.component;
using Lens.entity;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.projectile {
	public class Laser : Projectile {
		public Laser() {
			BreaksFromWalls = false;
			Spectral = true;
			CanBeBroken = false;
			CanBeReflected = false;
		}

		public override void AddComponents() {
			base.AddComponents();

			var graphics = new LaserGraphicsComponent("projectiles", "laser");
			AddComponent(graphics);

			Width = 32;
			Height = 9;
			
			BodyComponent = new RectBodyComponent(0, 0, Width, Height, BodyType.Dynamic, false, true);
			AddComponent(BodyComponent);
		}

		public override bool BreaksFrom(Entity entity) {
			return false;
		}

		public void Recalculate() {
			
		}
	}
}