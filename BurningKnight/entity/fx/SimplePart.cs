using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.fx {
	public class SimplePart : Entity {
		public static TextureRegion Defaul = Graphics.GetTexture("particle-bullet_trail");
		private float S = 1f;
		public bool Shadow = true;
		public float Speed = 1f;
		public TextureRegion Texture = Defaul;
		public Point Vel;

		public override void Init() {
			AlwaysActive = true;

			if (Vel == null) Vel = new Point(Random.NewFloat(-1f, 1f), Random.NewFloat(-1f, 1f));

			Vel.Mul(60);
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			this.X += Vel.X * Dt;
			this.Y += Vel.Y * Dt;
			Vel.Mul(0.95f);
			S -= Dt;

			if (S <= 0f) Done = true;
		}

		public override void Render() {
			Graphics.Render(Texture, this.X, this.Y, 0, Texture.GetRegionWidth() / 2, Texture.GetRegionHeight() / 2, false, false, S, S);
		}
	}
}