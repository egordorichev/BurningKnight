using BurningKnight.entity.creature.player;
using BurningKnight.entity.level;
using BurningKnight.util;

namespace BurningKnight.entity.item.weapon {
	public class WeaponBase : Item {
		public static ShaderProgram Shader;
		private static string CritLocale = Locale.Get("crit_chance");
		private static string DamageLocale = Locale.Get("damage");
		private static string PenetratesLocale = Locale.Get("penetrates");
		public int Damage = 2;
		public int InitialDamage;
		public int InitialDamageMin;
		protected float Knockback = 10f;
		protected int MinDamage = -1;
		protected bool Penetrates;
		private float T;
		public float TimeA = 0.1f;
		protected float TimeB = 0.1f;
		public float TimeDelay = 0f;

		static WeaponBase() {
			string VertexShader;
			string FragmentShader;
			VertexShader = Gdx.Files.Internal("shaders/default.vert").ReadString();
			FragmentShader = Gdx.Files.Internal("shaders/blink.frag").ReadString();
			Shader = new ShaderProgram(VertexShader, FragmentShader);

			if (!Shader.IsCompiled()) throw new GdxRuntimeException("Couldn't compile shader: " + Shader.GetLog());
		}

		protected string GetSfx() {
			return "whoosh";
		}

		public override int GetPrice() {
			return 15;
		}

		public override void OnPickup() {
			base.OnPickup();
			T = Random.NewFloat(10f);
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			T += Dt;
		}

		public override StringBuilder BuildInfo() {
			StringBuilder Builder = base.BuildInfo();

			if (MinDamage == -1) MinDamage = Math.Round((float) Damage / 3 * 2);

			Builder.Append("\n[orange]");
			float Mod = Player.Instance.GetDamageModifier();
			int Min = Math.Max(1, Math.Round(MinDamage * Mod));
			int Dmg = Math.Max(1, Math.Round(Damage * Mod));

			if (Min != Dmg) {
				Builder.Append(Min);
				Builder.Append("-");
			}

			Builder.Append(Dmg);
			Builder.Append(" ");
			Builder.Append(DamageLocale);
			Builder.Append("[gray]");

			if (Penetrates || Owner.Penetrates) {
				Builder.Append("\n[green]");
				Builder.Append(PenetratesLocale);
				Builder.Append("[gray]");
			}

			return Builder;
		}

		public int RollDamage() {
			if (Owner == null) Owner = Player.Instance;

			if (MinDamage == -1) MinDamage = Math.Round((float) Damage / 3 * 2);

			if (Owner.IsTouching(Terrain.ICE)) return Damage;

			return Math.Round(Random.NewFloatDice(MinDamage, Damage));
		}

		public void ModifyDamage(int Am) {
			InitialDamage = Damage;
			InitialDamageMin = MinDamage;
			Damage += Am;
			MinDamage += Am;
		}

		public void RestoreDamage() {
			Damage = InitialDamage;
			MinDamage = InitialDamageMin;
		}

		public void RenderAt(float X, float Y, float Ox, float Oy, float Scale) {
			RenderAt(X, Y, 0, Ox, Oy, false, false, Scale, Scale);
		}

		public void RenderAt(float X, float Y, float A, float Ox, float Oy, bool Fx, bool Fy, float Sx, float Sy) {
			RenderAt(X, Y, A, Ox, Oy, Fx, Fy, Sx, Sy, 1, 1);
		}

		public void RenderAt(float X, float Y, float A, float Ox, float Oy, bool Fx, bool Fy, float Sx, float Sy, float Al, float Gray) {
			Graphics.Batch.SetColor(1, 1, 1, 1);
			Graphics.Batch.End();
			Shader.Begin();
			Shader.SetUniformf("gray", Gray);
			Shader.SetUniformf("a", Al == 1 ? (Owner == null ? 1 : Owner.A) : Al);
			Shader.SetUniformf("time", T);
			Shader.End();
			Graphics.Batch.SetShader(Shader);
			Graphics.Batch.Begin();
			Graphics.Render(GetSprite(), X, Y, A, Ox, Oy, Fx, Fy, Sx, Sy);
			Graphics.Batch.End();
			Graphics.Batch.SetShader(null);
			Graphics.Batch.Begin();
		}

		public void RenderAt(float X, float Y, float A, float Ox, float Oy, bool Fx, bool Fy) {
			RenderAt(X, Y, A, Ox, Oy, Fx, Fy, 1, 1);
		}

		public override void Init() {
			base.Init();
			T = Random.NewFloat(10f);
		}
	}
}