namespace BurningKnight.core.entity.level {
	public class LightLevel : Entity {
		private Level Level;
		public static bool LIGHT = true;

		public Void SetLevel(Level Level) {
			this.Level = Level;
			this.Depth = 14;
			this.AlwaysRender = true;
		}

		public override Void Render() {
			if (LIGHT) {
				this.Level.RenderLight();
			} 
		}
	}
}
