namespace BurningKnight.box2dLight {
	public class BlendFunc {
		public const int Default_sfactor;
		public const int Default_dfactor;
		public int Sfactor;
		public int Dfactor;

		public BlendFunc(int Sfactor, int Dfactor) {
			this.Default_sfactor = Sfactor;
			this.Default_dfactor = Dfactor;
			this.Sfactor = Sfactor;
			this.Dfactor = Dfactor;
		}

		public Void Set(int Sfactor, int Dfactor) {
			this.Sfactor = Sfactor;
			this.Dfactor = Dfactor;
		}

		public Void Reset() {
			Sfactor = Default_sfactor;
			Dfactor = Default_dfactor;
		}

		public Void Apply() {
			Gdx.Gl20.GlBlendFunc(Sfactor, Dfactor);
		}
	}
}
