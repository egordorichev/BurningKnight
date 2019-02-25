using BurningKnight.core.physics;
using BurningKnight.core.util.file;

namespace BurningKnight.core.entity.level.entities {
	public class SolidProp : Prop {
		protected Body Body;
		protected Rectangle Collider;

		public override Void Init() {
			base.Init();
			CreateCollider();
		}

		protected Void CreateCollider() {
			if (this.Body == null && this.Collider != null) {
				this.Body = World.CreateSimpleBody(this, Collider.X, Collider.Y, Collider.Width, Collider.Height, BodyDef.BodyType.StaticBody, false);

				if (this.Body != null) {
					World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
				} 
			} 
		}

		public override Void Load(FileReader Reader) {
			base.Load(Reader);

			if (this.Body != null) {
				World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
			} 
		}

		public override Void Destroy() {
			this.Body = World.RemoveBody(this.Body);
		}
	}
}
