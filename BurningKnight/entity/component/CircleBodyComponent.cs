using BurningKnight.physics;
using Microsoft.Xna.Framework;
using VelcroPhysics.Collision.Shapes;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Factories;

namespace BurningKnight.entity.component {
	public class CircleBodyComponent : BodyComponent {
		public CircleBodyComponent(float x, float y, float r, BodyType type = BodyType.Dynamic, bool sensor = false, bool center = false) {
			if (center) {
				x -= r;
				y -= r;
			}

			Body = BodyFactory.CreateBody(Physics.World, Vector2.Zero, 0, type);
			Body.FixedRotation = true;
			Body.UserData = this;
			Body.LinearDamping = 0;

			Body.CreateFixture(new CircleShape(r, 1) {
				Position = new Vector2(x + r, y + r)
			}).IsSensor = sensor;
		}
	}
}