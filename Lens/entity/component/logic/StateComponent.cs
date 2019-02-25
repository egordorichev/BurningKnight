using System;

namespace Lens.entity.component.logic {
	public class StateComponent<T> : Component where T: Entity {
		private EntityState<T> state;
		private Type newState;
		
		public Type State {
			get { return state.GetType(); }
			set { newState = value; }
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (newState != null) {
				state?.Destroy();
				
				state = (EntityState<T>) Activator.CreateInstance(newState);
				state.Self = (T) Entity;
				state.Init();
				
				newState = null;
			}
			
			state?.Update(dt);
		}
	}
}