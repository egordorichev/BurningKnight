using BurningKnight.core.entity.creature.mob;
using BurningKnight.core.entity.level.entities;

namespace BurningKnight.core.entity.level {
	public class SignsLevel : Entity {
		protected void _Init() {
			{
				Depth = 6;
				AlwaysRender = true;
			}
		}

		public override Void Render() {
			foreach (Mob Mob in Mob.All) {
				if (Mob.OnScreen) {
					Mob.RenderSigns();
				} 
			}

			foreach (Door Door in Door.All) {
				if (Door.OnScreen) {
					Door.RenderSigns();
				} 
			}
		}

		public SignsLevel() {
			_Init();
		}
	}
}
