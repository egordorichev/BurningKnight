using BurningKnight.core.assets;

namespace BurningKnight.core.entity.creature.fx {
	public class Zzz : Entity {
		protected void _Init() {
			{
				AlwaysActive = true;
			}
		}

		private float Sx;
		private float Sy;
		private float T;
		public float Delay;

		public override Void Init() {
			base.Init();
			Sx = X;
			Sy = Y;
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (Delay > 0) {
				Delay -= Dt;

				if (Delay > 0) {
					return;
				} 
			} 

			T += Dt;
			X = (float) (Sx + Math.Cos(T * 6.5f) * Math.Max(0, 4 - T * 1.5f) * 1.5f);
			Y = Sy + T * 16;

			if (T >= 3f) {
				Done = true;
			} 
		}

		public override Void Render() {
			if (Delay > 0) {
				return;
			} 

			Graphics.Small.SetColor(1, 1, 1, (3 - T) * 0.33f);
			Graphics.Print("z", Graphics.Small, this.X, this.Y);
			Graphics.Small.SetColor(1, 1, 1, 1);
		}

		public Zzz() {
			_Init();
		}
	}
}
