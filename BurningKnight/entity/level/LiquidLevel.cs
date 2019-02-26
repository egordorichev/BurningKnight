namespace BurningKnight.entity.level {
	public class LiquidLevel : Entity {
		private Level Level;

		public LiquidLevel() {
			_Init();
		}

		protected void _Init() {
			{
				Depth = -8;
				AlwaysRender = true;
			}
		}

		public void SetLevel(Level Level) {
			this.Level = Level;
		}

		public override void Render() {
			Level.RenderLiquids();
		}
	}
}