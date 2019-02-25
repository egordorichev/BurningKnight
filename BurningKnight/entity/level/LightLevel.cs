namespace BurningKnight.entity.level {
	public class LightLevel : Entity {
		public static bool LIGHT = true;
		private Level Level;

		public void SetLevel(Level Level) {
			this.Level = Level;
			Depth = 14;
			AlwaysRender = true;
		}

		public override void Render() {
			if (LIGHT) Level.RenderLight();
		}
	}
}