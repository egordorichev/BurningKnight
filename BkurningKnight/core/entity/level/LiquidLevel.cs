namespace BurningKnight.core.entity.level {
	public class LiquidLevel : Entity {
		protected void _Init() {
			{
				Depth = -8;
				AlwaysRender = true;
			}
		}

		private Level Level;

		public Void SetLevel(Level Level) {
			this.Level = Level;
		}

		public override Void Render() {
			Level.RenderLiquids();
		}

		public LiquidLevel() {
			_Init();
		}
	}
}
