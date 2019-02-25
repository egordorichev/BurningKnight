using BurningKnight.core.assets;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.item.weapon.projectile.fx {
	public class RectFx : Entity {
		protected void _Init() {
			{
				AlwaysActive = true;
			}
		}

		static RectFx() {
			Shader = new ShaderProgram(Gdx.Files.Internal("shaders/default.vert").ReadString(), Gdx.Files.Internal("shaders/bloom.frag").ReadString());

			if (!Shader.IsCompiled()) throw new GdxRuntimeException("Couldn't compile shader: " + Shader.GetLog());

		}

		public float R = 1;
		public float G = 1;
		public float B = 1;
		public float A = 1;
		public float Scale = 1;
		public float Angle;
		public static TextureRegion Region = Graphics.GetTexture("particle-rect");
		private bool Left;
		public static ShaderProgram Shader;

		public override Void Init() {
			base.Init();
			Left = Random.Chance(50);
			Angle = Random.NewFloat(360);
			this.R = MathUtils.Clamp(0, 1, Random.NewFloat(-0.2f, 0.2f) + R);
			this.G = MathUtils.Clamp(0, 1, Random.NewFloat(-0.2f, 0.2f) + G);
			this.B = MathUtils.Clamp(0, 1, Random.NewFloat(-0.2f, 0.2f) + B);
		}

		public override Void Update(float Dt) {
			this.A -= Dt / 4;
			this.Angle += (this.Left ? Dt : -Dt) * 360;
			this.Scale -= Dt;

			if (this.A <= 0 || this.Scale <= 0) {
				this.Done = true;
			} 
		}

		public override Void Render() {
			Graphics.Batch.SetColor(R, G, B, A);
			Graphics.Render(Region, this.X + this.W / 2, this.Y + this.H / 2, this.Angle, this.W / 2, this.H / 2, false, false, this.Scale, this.Scale);
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}

		public RectFx() {
			_Init();
		}
	}
}
