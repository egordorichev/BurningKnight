using BurningKnight.box2dLight;
using BurningKnight.core.assets;
using BurningKnight.core.entity.level;
using BurningKnight.core.entity.level.levels.ice;
using BurningKnight.core.physics;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.fx {
	public class Firefly : SaveableEntity {
		protected void _Init() {
			{
				Depth = 14;
				W = 12;
				H = 12;
			}
		}

		private PointLight Light;
		private float T;
		private bool Ice;
		private float Rd = 6;
		private static Color Color = Color.ValueOf("#99e65f");
		private static Color Off = Color.ValueOf("#134c4c");
		public static Color ColorIce = Color.ValueOf("#0cf1ff");
		private static Color OffIce = Color.ValueOf("#0069aa");

		public override Void Init() {
			base.Init();
			Ice = Dungeon.Level is IceLevel;
			this.T = Random.NewFloat(1024);
			Light = World.NewLight(32, Ice ? new Color(0, 0.3f, 1, 1f) : new Color(0, 1, 0.3f, 1f), 0, X, Y);
			Light.SetXray(true);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			this.T += Dt * 2;
		}

		public override Void Destroy() {
			base.Destroy();
			World.RemoveLight(Light);
		}

		public override Void Render() {
			int R = 6;
			bool On = this.T % 20 <= 16f;
			Rd += ((On ? 6 : 0) - Rd) * Gdx.Graphics.GetDeltaTime() * 3;
			Rd = MathUtils.Clamp(0, 6, Rd);
			float X = (float) (this.X + Math.Cos(this.T / 8) * Math.Sin(this.T / 9) * 32);
			float Y = (float) (this.Y + Math.Sin(this.T / 7) * Math.Cos(this.T / 10) * 32);
			Light.SetPosition(X + R, Y + R);
			Light.SetDistance(Rd * 10);

			if (Ice) {
				Light.SetColor(0, 0.3f, 1, 1);
			} else {
				Light.SetColor(0, 1, 0.3f, 1);
			}


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

		public Firefly() {
			_Init();
		}
	}
}
