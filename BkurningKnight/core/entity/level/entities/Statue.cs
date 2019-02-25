namespace BurningKnight.core.entity.level.entities {
	public class Statue : SolidProp {
		protected void _Init() {
			{
				Sprite = "props-statue";
				Collider = new Rectangle(4, 10, 7, 10);
			}
		}

		private bool S;

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (!S) {
				S = true;
				Dungeon.Level.SetPassable((int) (this.X / 16), (int) ((this.Y + 8) / 16), false);
				Dungeon.Level.SetPassable((int) (this.X / 16), (int) ((this.Y + 24) / 16), false);
			} 
		}

		public Statue() {
			_Init();
		}
	}
}
