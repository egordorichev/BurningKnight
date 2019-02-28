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

			var vertices = new Vertices(4);
			
			vertices.Add(new Vector2(x, y));
			vertices.Add(new Vector2(x + w, y));
			vertices.Add(new Vector2(x + w, y + h));
			vertices.Add(new Vector2(x, y + h));
			
			FixtureFactory.AttachPolygon(vertices, 1f, Body);
		}

		public override void Init() {
			base.Init();
			Body.UserData = Entity;
		}
	}
}