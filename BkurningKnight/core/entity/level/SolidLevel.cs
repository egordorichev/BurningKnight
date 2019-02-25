namespace BurningKnight.core.entity.level {
	public class SolidLevel : Entity {
		private Level Level;

		public Void SetLevel(Level Level) {
			this.Level = Level;
			this.Depth = 5;
			this.AlwaysRender = true;
		}

		public override Void Render() {
			this.Level.RenderSolid();
		}
	}
}
