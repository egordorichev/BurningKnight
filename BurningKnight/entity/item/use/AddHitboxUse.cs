using BurningKnight.physics;
using Lens.entity;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Factories;
using VelcroPhysics.Shared;

namespace BurningKnight.entity.item.use {
	public class AddHitboxUse : ItemUse {
		private Body body;

		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);
			
			body = BodyFactory.CreateBody(Physics.World, Vector2.Zero, 0, BodyType.Dynamic);

			body.FixedRotation = true;
			body.UserData = this;
			body.LinearDamping = 0;

			var r = Item.Region;
			
			var w = r.Width;
			var h = r.Height;
			var x = -r.Width / 2;
			var y = 0;
			
			FixtureFactory.AttachPolygon(new Vertices(4) {
				new Vector2(x, y), new Vector2(x + w, y), new Vector2(x + w, y + h), new Vector2(x, y + h)
			}, 1f, body).IsSensor = true;

			body.Position = entity.Center;
		}

		public override void Destroy() {
			base.Destroy();

			if (body != null) {
				Physics.RemoveBody(body);
				body = null;
			}
		}
	}
}