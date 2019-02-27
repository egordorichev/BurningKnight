using System;

namespace Lens.entity.component.logic {
	public class StateComponent : Component {
		private EntityState state;
		private Type newState;
		
		public Type State {
			get => state.GetType();
			set => newState = value;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (newState != null) {
				if (state != null && newState == state.GetType()) {
					newState = null;
					return;
				}
				
				state?.Destroy();
				
				state = (EntityState) Activator.CreateInstance(newState);
				state.Self = Entity;
				state.Init();
				
				newState = null;
			}
			
			state?.Update(dt);
		}
	}
}