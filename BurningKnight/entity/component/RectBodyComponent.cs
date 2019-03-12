using BurningKnight.physics;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Factories;
using VelcroPhysics.Shared;

namespace BurningKnight.entity.component {
	public class RectBodyComponent : BodyComponent {
		public RectBodyComponent(float x, float y, float w, float h, BodyType type = BodyType.Dynamic, bool sensor = false, bool center = false) {
			if (center) {
				x -= w / 2;
				y -= h / 2;
			}

			Body = BodyFactory.CreateBody(Physics.World, Vector2.Zero, 0, type);
			Body.FixedRotation = true;
			Body.UserData = this;
			Body.LinearDamping = 0;

			FixtureFactory.AttachPolygon(new Vertices(4) {
				new Vector2(x, y), new Vector2(x + w, y), new Vector2(x + w, y + h), new Vector2(x, y + h)
			}, 1f, Body).IsSensor = sensor;
		}
	}
}