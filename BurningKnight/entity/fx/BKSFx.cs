using BurningKnight.entity.creature.mob;

namespace BurningKnight.entity.fx {
	public class BKSFx : Entity {
		public float A;
		public Vector3 Color;
		public bool Flipped;
		public float Ox;
		public float Oy;

		public TextureRegion Region;
		private float S = 1;
		public float Speed = 1;
		public float Sx = 1;
		public float Sy = 1;

		public BKSFx() {
			_Init();
			Color = new Vector3(1, 0.3f, 0.3f);
		}

		protected void _Init() {
			{
				AlwaysActive = true;
				Depth = -1;
			}
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			S -= Dt * Speed;

			if (S <= 0) Done = true;
		}

		public override void Render() {
			Graphics.Batch.End();
			Mob.Shader.Begin();
			Mob.Shader.SetUniformf("u_color", Color);
			Mob.Shader.SetUniformf("u_a", 0.8f);
			Mob.Shader.End();
			Graphics.Batch.SetShader(Mob.Shader);
			Graphics.Batch.Begin();
			Graphics.Render(Region, X, Y, A, Ox, Oy, false, false, (Flipped ? -S : S) * Sx, S * Sy);
			Graphics.Batch.End();
			Graphics.Batch.SetShader(null);
			Graphics.Batch.Begin();
		}
	}
}