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
				Offset = new Vector2(r, r);
			}

			Body = BodyFactory.CreateBody(Physics.World, Vector2.Zero, 0, type);
			Body.FixedRotation = true;
			Body.UserData = this;
			Body.LinearDamping = 0;

			Body.CreateFixture(new CircleShape(r, 1) {
				Position = new Vector2(x + r, y + r),
			}).IsSensor = sensor;
		}

		public override void Resize(float x, float y, float w, float h, bool center = false) {
			var fixture = Body.FixtureList[0];
			var sensor = fixture.IsSensor;
			
			Body.DestroyFixture(fixture);

			var r = w / 2f;
			
			if (center) {
				x -= r;
				y -= r;
				Offset = new Vector2(r, r);
			}
			
			Body.CreateFixture(new CircleShape(r, 1) {
				Position = new Vector2(x + r, y + r),
			}).IsSensor = sensor;
		}
	}
}