using BurningKnight.core.assets;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.creature.fx {
	public class BloodFx : Entity {
		protected void _Init() {
			{
				AlwaysActive = true;
			}
		}

		private Point Vel;
		private bool Second;
		private float Av;
		private float Size;
		private float R;
		private float G;
		private float B;
		private float Angle;

		public static Void Add(Entity Entity, int Count) {
			Add(Entity.X, Entity.Y, Entity.W, Entity.H, Count);
		}

		public static Void Add(float X, float Y, float W, float H, int Count) {
			if (!Settings.Blood) {
				return;
			} 

			for (int I = 0; I < Count; I++) {
				BloodFx Fx = new BloodFx();
				Fx.X = Random.NewFloat(W - 3.5f) + X + 3.5f;
				Fx.Y = Random.NewFloat(H - 3.5f) + Y + 3.5f;
				Dungeon.Area.Add(Fx);
			}
		}

		public override Void Init() {
			this.Vel = new Point(Random.NewFloat(-1f, 1f), Random.NewFloat(-1f));
			this.R = Random.NewFloat(0.7f, 1f);
			this.G = Random.NewFloat(0f, 0.15f);
			this.B = Random.NewFloat(0f, 0.15f);
			this.Av = Random.NewFloat(0.5f, 1f) * (Random.Chance(50) ? -1 : 1);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			this.X += this.Vel.X * Dt * 60;
			this.Y += this.Vel.Y * Dt * 60;
			this.Vel.X -= this.Vel.X * Math.Min(1, Dt * 3);
			this.Vel.Y -= Dt;
			this.Angle += this.Av * Dt * 360;

			if (this.Second) {
				this.Size -= Dt * 10;

				if (this.Size <= 0) {
					this.Done = true;
				} 
			} else {
				this.Size += Dt * 48;

				if (this.Size >= 5f) {
					this.Size = 5f;
					this.Second = true;
				} 
			}

		}

		public override Void Render() {
			Graphics.StartShape();
			Graphics.Shape.SetColor(R, G, B, 1);
			float S = this.Size / 2;
			Graphics.Shape.Rect(this.X, this.Y, S, S, this.Size, this.Size, 1, 1, this.Angle);
			Graphics.EndShape();
		}

		public BloodFx() {
			_Init();
		}
	}
}
