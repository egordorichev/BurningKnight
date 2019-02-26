using BurningKnight.util;

namespace BurningKnight.entity.item.weapon.projectile.fx {
	public class RectFx : Entity {
		public static TextureRegion Region = Graphics.GetTexture("particle-rect");
		public static ShaderProgram Shader;
		public float A = 1;
		public float Angle;
		public float B = 1;
		public float G = 1;
		private bool Left;

		public float R = 1;
		public float Scale = 1;

		static RectFx() {
			Shader = new ShaderProgram(Gdx.Files.Internal("shaders/default.vert").ReadString(), Gdx.Files.Internal("shaders/bloom.frag").ReadString());

			if (!Shader.IsCompiled()) throw new GdxRuntimeException("Couldn't compile shader: " + Shader.GetLog());
		}

		public RectFx() {
			_Init();
		}

		protected void _Init() {
			{
				AlwaysActive = true;
			}
		}

		public override void Init() {
			base.Init();
			Left = Random.Chance(50);
			Angle = Random.NewFloat(360);
			R = MathUtils.Clamp(0, 1, Random.NewFloat(-0.2f, 0.2f) + R);
			G = MathUtils.Clamp(0, 1, Random.NewFloat(-0.2f, 0.2f) + G);
			B = MathUtils.Clamp(0, 1, Random.NewFloat(-0.2f, 0.2f) + B);
		}

		public override void Update(float Dt) {
			A -= Dt / 4;
			Angle += (Left ? Dt : -Dt) * 360;
			Scale -= Dt;

			if (A <= 0 || Scale <= 0) Done = true;
		}

		public override void Render() {
			Graphics.Batch.SetColor(R, G, B, A);
			Graphics.Render(Region, this.X + W / 2, this.Y + H / 2, Angle, W / 2, H / 2, false, false, Scale, Scale);
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}
	}
}