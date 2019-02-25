namespace BurningKnight.entity.level {
	public class SolidLevel : Entity {
		private Level Level;

		public void SetLevel(Level Level) {
			this.Level = Level;
			Depth = 5;
			AlwaysRender = true;
		}

		public override void Render() {
			Level.RenderSolid();
		}
	}
}