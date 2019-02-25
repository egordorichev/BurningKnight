using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.creature.fx {
	public class BloodFx : Entity {
		private float Angle;
		private float Av;
		private float B;
		private float G;
		private float R;
		private bool Second;
		private float Size;

		private Point Vel;

		public BloodFx() {
			_Init();
		}

		protected void _Init() {
			{
				AlwaysActive = true;
			}
		}

		public static void Add(Entity Entity, int Count) {
			Add(Entity.X, Entity.Y, Entity.W, Entity.H, Count);
		}

		public static void Add(float X, float Y, float W, float H, int Count) {
			if (!Settings.Blood) return;

			for (var I = 0; I < Count; I++) {
				var Fx = new BloodFx();
				Fx.X = Random.NewFloat(W - 3.5f) + X + 3.5f;
				Fx.Y = Random.NewFloat(H - 3.5f) + Y + 3.5f;
				Dungeon.Area.Add(Fx);
			}
		}

		public override void Init() {
			Vel = new Point(Random.NewFloat(-1f, 1f), Random.NewFloat(-1f));
			R = Random.NewFloat(0.7f, 1f);
			G = Random.NewFloat(0f, 0.15f);
			B = Random.NewFloat(0f, 0.15f);
			Av = Random.NewFloat(0.5f, 1f) * (Random.Chance(50) ? -1 : 1);
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			this.X += Vel.X * Dt * 60;
			this.Y += Vel.Y * Dt * 60;
			Vel.X -= Vel.X * Math.Min(1, Dt * 3);
			Vel.Y -= Dt;
			Angle += Av * Dt * 360;

			if (Second) {
				Size -= Dt * 10;

				if (Size <= 0) Done = true;
			}
			else {
				Size += Dt * 48;

				if (Size >= 5f) {
					Size = 5f;
					Second = true;
				}
			}
		}

		public override void Render() {
			Graphics.StartShape();
			Graphics.Shape.SetColor(R, G, B, 1);
			var S = Size / 2;
			Graphics.Shape.Rect(this.X, this.Y, S, S, Size, Size, 1, 1, Angle);
			Graphics.EndShape();
		}
	}
}