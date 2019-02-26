using BurningKnight.entity.level;
using BurningKnight.entity.level.levels.ice;
using BurningKnight.physics;
using BurningKnight.util;

namespace BurningKnight.entity.creature.fx {
	public class Firefly : SaveableEntity {
		private static Color Color = Color.ValueOf("#99e65f");
		private static Color Off = Color.ValueOf("#134c4c");
		public static Color ColorIce = Color.ValueOf("#0cf1ff");
		private static Color OffIce = Color.ValueOf("#0069aa");
		private bool Ice;

		private PointLight Light;
		private float Rd = 6;
		private float T;

		public Firefly() {
			_Init();
		}

		protected void _Init() {
			{
				Depth = 14;
				W = 12;
				H = 12;
			}
		}

		public override void Init() {
			base.Init();
			Ice = Dungeon.Level is IceLevel;
			T = Random.NewFloat(1024);
			Light = World.NewLight(32, Ice ? new Color(0, 0.3f, 1, 1f) : new Color(0, 1, 0.3f, 1f), 0, X, Y);
			Light.SetXray(true);
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			T += Dt * 2;
		}

		public override void Destroy() {
			base.Destroy();
			World.RemoveLight(Light);
		}

		public override void Render() {
			var R = 6;
			var On = T % 20 <= 16f;
			Rd += ((On ? 6 : 0) - Rd) * Gdx.Graphics.GetDeltaTime() * 3;
			Rd = MathUtils.Clamp(0, 6, Rd);
			var X = this.X + Math.Cos(T / 8) * Math.Sin(T / 9) * 32;
			var Y = (float) (this.Y + Math.Sin(T / 7) * Math.Cos(T / 10) * 32);
			Light.SetPosition(X + R, Y + R);
			Light.SetDistance(Rd * 10);

			if (Ice)
				Light.SetColor(0, 0.3f, 1, 1);
			else
				Light.SetColor(0, 1, 0.3f, 1);


			Graphics.StartAlphaShape();
			Color Color = Ice ? ColorIce : Firefly.Color;
			Color Off = Ice ? OffIce : Firefly.Off;

			if (Rd > 2) {
				Graphics.Shape.SetColor(Color.R, Color.G, Color.B, 0.3f);
				Graphics.Shape.Circle(X + R, Y + R, Rd);
			}

			Graphics.Shape.SetColor(On ? Color.R : Off.R, On ? Color.G : Off.G, On ? Color.B : Off.B, 1);
			Graphics.Shape.Circle(X + R, Y + R, 2);
			Graphics.EndAlphaShape();
		}
	}
}