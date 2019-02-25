using BurningKnight.core.assets;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.fx {
	public class SimplePart : Entity {
		public static TextureRegion Defaul = Graphics.GetTexture("particle-bullet_trail");
		public TextureRegion Texture = Defaul;
		public Point Vel;
		public float Speed = 1f;
		public bool Shadow = true;
		private float S = 1f;

		public override Void Init() {
			this.AlwaysActive = true;

			if (this.Vel == null) {
				this.Vel = new Point(Random.NewFloat(-1f, 1f), Random.NewFloat(-1f, 1f));
			} 

			this.Vel.Mul(60);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			this.X += this.Vel.X * Dt;
			this.Y += this.Vel.Y * Dt;
			this.Vel.Mul(0.95f);
			this.S -= Dt;

			if (this.S <= 0f) {
				this.Done = true;
			} 
		}

		public override Void Render() {
			Graphics.Render(Texture, this.X, this.Y, 0, Texture.GetRegionWidth() / 2, Texture.GetRegionHeight() / 2, false, false, S, S);
		}
	}
}
