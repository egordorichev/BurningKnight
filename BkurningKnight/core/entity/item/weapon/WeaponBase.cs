using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.level;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.item.weapon {
	public class WeaponBase : Item {
		static WeaponBase() {
			string VertexShader;
			string FragmentShader;
			VertexShader = Gdx.Files.Internal("shaders/default.vert").ReadString();
			FragmentShader = Gdx.Files.Internal("shaders/blink.frag").ReadString();
			Shader = new ShaderProgram(VertexShader, FragmentShader);

			if (!Shader.IsCompiled()) throw new GdxRuntimeException("Couldn't compile shader: " + Shader.GetLog());

		}

		public static ShaderProgram Shader;
		private static string CritLocale = Locale.Get("crit_chance");
		private static string DamageLocale = Locale.Get("damage");
		private static string PenetratesLocale = Locale.Get("penetrates");
		public int Damage = 2;
		public float TimeA = 0.1f;
		public float TimeDelay = 0f;
		public int InitialDamage;
		public int InitialDamageMin;
		protected int MinDamage = -1;
		protected float TimeB = 0.1f;
		protected float Knockback = 10f;
		protected bool Penetrates;
		private float T;

		protected string GetSfx() {
			return "whoosh";
		}

		public override int GetPrice() {
			return 15;
		}

		public override Void OnPickup() {
			base.OnPickup();
			this.T = Random.NewFloat(10f);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			this.T += Dt;
		}

		public override StringBuilder BuildInfo() {
			StringBuilder Builder = base.BuildInfo();

			if (this.MinDamage == -1) {
				MinDamage = Math.Round(((float) Damage) / 3 * 2);
			} 

			Builder.Append("\n[orange]");
			float Mod = Player.Instance.GetDamageModifier();
			int Min = Math.Max(1, Math.Round((this.MinDamage) * Mod));
			int Dmg = Math.Max(1, Math.Round((this.Damage) * Mod));

			if (Min != Dmg) {
				Builder.Append(Min);
				Builder.Append("-");
			} 

			Builder.Append(Dmg);
			Builder.Append(" ");
			Builder.Append(DamageLocale);
			Builder.Append("[gray]");

			if (this.Penetrates || this.Owner.Penetrates) {
				Builder.Append("\n[green]");
				Builder.Append(PenetratesLocale);
				Builder.Append("[gray]");
			} 

			return Builder;
		}

		public int RollDamage() {
			if (this.Owner == null) {
				this.Owner = Player.Instance;
			} 

			if (this.MinDamage == -1) {
				MinDamage = Math.Round(((float) Damage) / 3 * 2);
			} 

			if (this.Owner.IsTouching(Terrain.ICE)) {
				return this.Damage;
			} 

			return Math.Round(Random.NewFloatDice(this.MinDamage, this.Damage));
		}

		public Void ModifyDamage(int Am) {
			this.InitialDamage = Damage;
			this.InitialDamageMin = MinDamage;
			this.Damage += Am;
			this.MinDamage += Am;
		}

		public Void RestoreDamage() {
			this.Damage = InitialDamage;
			this.MinDamage = InitialDamageMin;
		}

		public Void RenderAt(float X, float Y, float Ox, float Oy, float Scale) {
			RenderAt(X, Y, 0, Ox, Oy, false, false, Scale, Scale);
		}

		public Void RenderAt(float X, float Y, float A, float Ox, float Oy, bool Fx, bool Fy, float Sx, float Sy) {
			RenderAt(X, Y, A, Ox, Oy, Fx, Fy, Sx, Sy, 1, 1);
		}

		public Void RenderAt(float X, float Y, float A, float Ox, float Oy, bool Fx, bool Fy, float Sx, float Sy, float Al, float Gray) {
			Graphics.Batch.SetColor(1, 1, 1, 1);
			Graphics.Batch.End();
			Shader.Begin();
			Shader.SetUniformf("gray", Gray);
			Shader.SetUniformf("a", Al == 1 ? (this.Owner == null ? 1 : this.Owner.A) : Al);
			Shader.SetUniformf("time", this.T);
			Shader.End();
			Graphics.Batch.SetShader(Shader);
			Graphics.Batch.Begin();
			Graphics.Render(GetSprite(), X, Y, A, Ox, Oy, Fx, Fy, Sx, Sy);
			Graphics.Batch.End();
			Graphics.Batch.SetShader(null);
			Graphics.Batch.Begin();
		}

		public Void RenderAt(float X, float Y, float A, float Ox, float Oy, bool Fx, bool Fy) {
			RenderAt(X, Y, A, Ox, Oy, Fx, Fy, 1, 1);
		}

		public override Void Init() {
			base.Init();
			this.T = Random.NewFloat(10f);
		}
	}
}
