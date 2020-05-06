using Lens.entity;

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
			AddComponent(new LaserGraphicsComponent("projectiles", "rect"));
		}

		public override bool BreaksFrom(Entity entity) {
			return false;
		}

		public void Recalculate() {
			
		}
	}
}