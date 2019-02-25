namespace BurningKnight.entity.creature.fx {
	public class Zzz : Entity {
		public float Delay;

		private float Sx;
		private float Sy;
		private float T;

		public Zzz() {
			_Init();
		}

		protected void _Init() {
			{
				AlwaysActive = true;
			}
		}

		public override void Init() {
			base.Init();
			Sx = X;
			Sy = Y;
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (Delay > 0) {
				Delay -= Dt;

				if (Delay > 0) return;
			}

			T += Dt;
			X = Sx + Math.Cos(T * 6.5f) * Math.Max(0, 4 - T * 1.5f) * 1.5f;
			Y = Sy + T * 16;

			if (T >= 3f) Done = true;
		}

		public override void Render() {
			if (Delay > 0) return;

			Graphics.Small.SetColor(1, 1, 1, (3 - T) * 0.33f);
			Graphics.Print("z", Graphics.Small, this.X, this.Y);
			Graphics.Small.SetColor(1, 1, 1, 1);
		}
	}
}