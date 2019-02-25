using BurningKnight.core.entity.creature.player;

namespace BurningKnight.core.entity.level.entities {
	public class Blocker : SolidProp {
		protected void _Init() {
			{
				Collider = new Rectangle(0, 10, 29, 5);
				Sprite = "props-blocker";
				W = 32;
				H = 16;
			}
		}

		public override Void Init() {
			base.Init();
			Player.Ladder = this;
		}

		public Blocker() {
			_Init();
		}
	}
}
