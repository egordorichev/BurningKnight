using Lens.entity;
using Lens.entity.component.logic;

namespace BurningKnight.entity.creature {
	public class CreatureState<T> : EntityState where T: Creature {
		public new T Self;

		public override void Assign(Entity self) {
			base.Assign(self);
			Self = (T) self;
		}
	}
}