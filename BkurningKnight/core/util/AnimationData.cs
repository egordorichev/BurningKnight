using BurningKnight.core.assets;

namespace BurningKnight.core.util {
	public class AnimationData {
		public static class Listener {
			public Void OnFrame(int Frame) {

			}

			public Void OnEnd() {

			}
		}

		private const List<Animation.Frame> Frames;
		private float T;
		private int Index;
		private bool Pause;
		private bool Back;
		private Animation.Frame Current;
		private bool Auto;
		private Listener Listener;

		public Void SetSpeedModifier(float Mod) {
			foreach (Animation.Frame Frame in Frames) {
				Frame.Delay = Frame.Initial * Mod;
			}
		}

		public bool IsGoingBack() {
			return Back;
		}

		public Void SetListener(Listener Listener) {
			this.Listener = Listener;
		}

		public List GetFrames<Animation.Frame> () {
			return this.Frames;
		}

		public Void SetAutoPause(bool Auto) {
			this.Auto = Auto;
		}

		public AnimationData(List Frames) {
			this.Frames = Frames;
			this.Current = this.Frames.Get(0);
		}

		public AnimationData Randomize() {
			this.SetFrame(Random.NewInt(this.Frames.Size()));
			this.T = Random.NewFloat(0f, 100f);

			return this;
		}

		public Animation.Frame GetCurrent() {
			return this.Current;
		}

		public bool IsPaused() {
			return this.Pause;
		}

		public bool Update(float Dt) {
			bool Val = false;

			if (!this.Pause) {
				this.T += Dt;

				if (this.T >= this.Current.Delay) {
					this.Index += (this.Back ? -1 : 1);
					this.T = 0;

					if ((!this.Back && this.Index >= this.Frames.Size()) || (this.Back && this.Index < 0)) {
						if (this.Listener != null) {
							this.Listener.OnEnd();
						} 

						this.Index = (this.Back ? this.Frames.Size() - 1 : 0);
						Val = true;

						if (this.Auto) {
							this.Index = (this.Back ? 0 : this.Frames.Size() - 1);
							this.Pause = true;
						} 
					} 

					if (this.Listener != null) {
						this.Listener.OnFrame(this.Index);
					} 

					this.Current = this.Frames.Get(this.Index);
				} 
			} 

			return Val;
		}

		public Void SetPaused(bool Pause) {
			this.Pause = Pause;
		}

		public Void SetBack(bool Back) {
			this.Back = Back;
		}

		public Void SetFrame(int I) {
			this.Current = this.Frames.Get(I);
		}

		public int GetFrame() {
			return this.Index;
		}

		public Void Render(float X, float Y, bool Flip) {
			Graphics.Render(this.Current.Frame, X, Y, 0, 0, 0, Flip, false);
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}

		public Void Render(float X, float Y, bool Flip, bool FlipY, float Ox, float Oy, float A) {
			Graphics.Render(this.Current.Frame, X + Ox, Y + Oy, A, Ox, Oy, Flip, FlipY);
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}

		public Void Render(float X, float Y, bool Flip, bool FlipY, float Ox, float Oy, float A, float Sx, float Sy) {
			Graphics.Render(this.Current.Frame, X + Ox, Y + Oy, A, Ox, Oy, Flip, FlipY, Sx, Sy);
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}

		public Void Render(float X, float Y, bool Flip, bool FlipY, int F) {
			Graphics.Render(this.Frames.Get(F).Frame, X, Y, 0, 0, 0, Flip, FlipY);
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}
	}
}
