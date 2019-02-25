using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.mob;

namespace BurningKnight.core.entity.fx {
	public class BKSFx : Entity {
		protected void _Init() {
			{
				AlwaysActive = true;
				Depth = -1;
			}
		}

		public TextureRegion Region;
		public float A;
		public float Ox;
		public float Oy;
		public float Sx = 1;
		public float Sy = 1;
		private float S = 1;
		public bool Flipped;
		public float Speed = 1;
		public Vector3 Color;

		public BKSFx() {
			_Init();
			Color = new Vector3(1, 0.3f, 0.3f);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			S -= Dt * Speed;

			if (S <= 0) {
				Done = true;
			} 
		}

		public override Void Render() {
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
