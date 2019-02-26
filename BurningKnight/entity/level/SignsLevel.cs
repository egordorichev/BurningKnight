using BurningKnight.entity.creature.mob;
using BurningKnight.entity.level.entities;

namespace BurningKnight.entity.level {
	public class SignsLevel : Entity {
		public SignsLevel() {
			_Init();
		}

		protected void _Init() {
			{
				Depth = 6;
				AlwaysRender = true;
			}
		}

		public override void Render() {
			foreach (Mob Mob in Mob.All)
				if (Mob.OnScreen)
					Mob.RenderSigns();

			foreach (Door Door in Door.All)
				if (Door.OnScreen)
					Door.RenderSigns();
		}
	}
}