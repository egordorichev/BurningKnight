using Lens.entity;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.prefix {
	public abstract class Prefix {
		public Mob Mob;

		public virtual void Init() {
			
		}

		public virtual void Update(float dt) {
			
		}

		public virtual bool HandleEvent(Event e) {
			return false;
		}

		public abstract Color GetColor();
	}
}