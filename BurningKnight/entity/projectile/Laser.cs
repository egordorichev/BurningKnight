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

			var graphics = new LaserGraphicsComponent("projectiles", "rect");
			AddComponent(graphics);
			
			var w = graphics.Sprite.Source.Width;
			var h = graphics.Sprite.Source.Height;

			Width = w;
			Height = h;

			BodyComponent = new RectBodyComponent(0, 0, w, h, BodyType.Dynamic, false, true);
		}

		public override bool BreaksFrom(Entity entity) {
			return false;
		}

		public void Recalculate() {
			
		}
	}
}