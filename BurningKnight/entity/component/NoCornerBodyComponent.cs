using BurningKnight.physics;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Factories;
using VelcroPhysics.Shared;

namespace BurningKnight.entity.component {
	public class NoCornerBodyComponent : BodyComponent {
		public NoCornerBodyComponent(float x, float y, float w, float h, BodyType type = BodyType.Dynamic, bool sensor = false, bool center = false) {
			if (center) {
				x -= w / 2;
				y -= h / 2;
			}

			Body = BodyFactory.CreateBody(Physics.World, Vector2.Zero, 0, type);
			Body.FixedRotation = true;
			Body.UserData = this;
			Body.LinearDamping = 0;

			float mx = w / 3f;
			float my = h / 3f;

			FixtureFactory.AttachPolygon(new Vertices(4) {
				new Vector2(x, y + my), new Vector2(x + mx, y), 
				new Vector2(x + w - mx, y), new Vector2(x + w, y + my),
				new Vector2(x + w, y + h - my), new Vector2(x + w - mx, y + h),
				new Vector2(x + mx, y + h), new Vector2(x, y + h - my)
			}, 1f, Body);
		}
	}
}