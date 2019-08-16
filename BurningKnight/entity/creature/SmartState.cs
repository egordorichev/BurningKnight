using Lens.entity;
using Lens.entity.component.logic;

namespace BurningKnight.entity.creature {
	public class SmartState<T> : EntityState where T: Entity {
		public new T Self;

		public override void Assign(Entity self) {
			base.Assign(self);
			Self = (T) self;
		}
	}
}