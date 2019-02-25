namespace BurningKnight.core.entity.level {
	public class WallLevel : Entity {
		protected void _Init() {
			{
				Depth = -7;
				AlwaysRender = true;
			}
		}

		private Level Level;

		public Void SetLevel(Level Level) {
			this.Level = Level;
		}

		public override Void Render() {
			this.Level.RenderSides();
		}

		public WallLevel() {
			_Init();
		}
	}
}
