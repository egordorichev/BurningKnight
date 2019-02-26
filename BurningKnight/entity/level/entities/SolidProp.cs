using BurningKnight.physics;
using Lens.util.file;

namespace BurningKnight.entity.level.entities {
	public class SolidProp : Prop {
		protected Body Body;
		protected Rectangle Collider;

		public override void Init() {
			base.Init();
			CreateCollider();
		}

		protected void CreateCollider() {
			if (Body == null && Collider != null) {
				Body = World.CreateSimpleBody(this, Collider.X, Collider.Y, Collider.Width, Collider.Height, BodyDef.BodyType.StaticBody, false);

				if (Body != null) World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
			}
		}

		public override void Load(FileReader Reader) {
			base.Load(Reader);

			if (Body != null) World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
		}

		public override void Destroy() {
			Body = World.RemoveBody(Body);
		}
	}
}