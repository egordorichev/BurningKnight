using BurningKnight.entity.creature.player;

namespace BurningKnight.entity.level.entities {
	public class Blocker : SolidProp {
		public Blocker() {
			_Init();
		}

		protected void _Init() {
			{
				Collider = new Rectangle(0, 10, 29, 5);
				Sprite = "props-blocker";
				W = 32;
				H = 16;
			}
		}

		public override void Init() {
			base.Init();
			Player.Ladder = this;
		}
	}
}