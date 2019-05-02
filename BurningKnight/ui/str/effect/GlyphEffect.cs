namespace BurningKnight.ui.str.effect {
	public class GlyphEffect {
		public int Start;
		public int End;
		public float Time;
		public bool Closed;
		
		public virtual void Apply(Glyph glyph, int i) {
			
		}

		public virtual void Update(float dt) {
			Time += dt;
		}

		public virtual bool Ended() {
			return false;
		}
	}
}