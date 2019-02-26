using BurningKnight.physics;
using BurningKnight.util;
using Lens.util.file;

namespace BurningKnight.entity.level.entities.fx {
	public class Table : SaveableEntity {
		private static Animation Animations = Animation.Make("prop-throne", "-desk");
		private AnimationData Animation;
		private Body Body;

		public Table() {
			_Init();
		}

		protected void _Init() {
			{
				W = 42;
				H = 28;
			}
		}

		public override void Init() {
			base.Init();
			Animation = Animations.Get("idle");
			Body = World.CreateSimpleBody(this, 0, 10, (int) W, (int) H - 14, BodyDef.BodyType.StaticBody, false);

			if (Body != null) World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
		}

		public override void Destroy() {
			base.Destroy();
			Body = World.RemoveBody(Body);
		}

		public override void Load(FileReader Reader) {
			base.Load(Reader);
			World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
		}

		public override void Render() {
			base.Render();
			Animation.Render(this.X, this.Y, false);
		}
	}
}