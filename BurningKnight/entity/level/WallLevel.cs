namespace BurningKnight.entity.level {
	public class WallLevel : Entity {
		private Level Level;

		public WallLevel() {
			_Init();
		}

		protected void _Init() {
			{
				Depth = -7;
				AlwaysRender = true;
			}
		}

		public void SetLevel(Level Level) {
			this.Level = Level;
		}

		public override void Render() {
			Level.RenderSides();
		}
	}
}