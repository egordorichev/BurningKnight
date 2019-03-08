using System;
using Lens.entity.component.graphics;

namespace Lens.entity.component.logic {
	public class StateComponent : Component {
		private EntityState state;
		private Type newState;
		
		public Type State {
			get => state.GetType();
			
			set {
				if (state == null || state.GetType() != value) {
					newState = value;
				}				
			}
		}

		public EntityState StateInstance => state;
		
		public void Become<T>() {
			State = typeof(T);
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (newState != null) {
				state?.Destroy();
				
				state = (EntityState) Activator.CreateInstance(newState);
				state.Self = Entity;
				state.Init();

				Send(new StateChangedEvent {
					NewState = newState
				});
				
				newState = null;
			}
			
			state?.Update(dt);
		}
	}
}