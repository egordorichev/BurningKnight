using System;

namespace Lens.entity.component.logic {
	public class StateChangedEvent : Event {
		public Type NewState;
		public EntityState State;
	}
}