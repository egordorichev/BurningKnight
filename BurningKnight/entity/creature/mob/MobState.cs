using Lens.entity;
using Lens.entity.component.logic;

namespace BurningKnight.entity.creature.mob {
	public class MobState<T> : EntityState where T: Mob {
		public new T Self;

		public override void Assign(Entity self) {
			base.Assign(self);
			Self = (T) self;
		}
	}
}