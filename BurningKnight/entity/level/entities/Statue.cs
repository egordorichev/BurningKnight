namespace BurningKnight.entity.level.entities {
	public class Statue : SolidProp {
		private bool S;

		public Statue() {
			_Init();
		}

		protected void _Init() {
			{
				Sprite = "props-statue";
				Collider = new Rectangle(4, 10, 7, 10);
			}
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (!S) {
				S = true;
				Dungeon.Level.SetPassable(this.X / 16, (this.Y + 8) / 16, false);
				Dungeon.Level.SetPassable(this.X / 16, (this.Y + 24) / 16, false);
			}
		}
	}
}