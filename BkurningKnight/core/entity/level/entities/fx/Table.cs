using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.file;

namespace BurningKnight.core.entity.level.entities.fx {
	public class Table : SaveableEntity {
		protected void _Init() {
			{
				W = 42;
				H = 28;
			}
		}

		private static Animation Animations = Animation.Make("prop-throne", "-desk");
		private AnimationData Animation;
		private Body Body;

		public override Void Init() {
			base.Init();
			this.Animation = Animations.Get("idle");
			this.Body = World.CreateSimpleBody(this, 0, 10, (int) W, (int) H - 14, BodyDef.BodyType.StaticBody, false);

			if (this.Body != null) {
				World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
			} 
		}

		public override Void Destroy() {
			base.Destroy();
			this.Body = World.RemoveBody(this.Body);
		}

		public override Void Load(FileReader Reader) {
			base.Load(Reader);
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
		}

		public override Void Render() {
			base.Render();
			this.Animation.Render(this.X, this.Y, false);
		}

		public Table() {
			_Init();
		}
	}
}
