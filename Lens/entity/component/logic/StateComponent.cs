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

		public Type ForceState {
			set => newState = value;
		}

		public EntityState StateInstance => state;
		
		public void Become<T>(bool force = false) {
			if (force) {
				ForceState = typeof(T);
			} else {
				State = typeof(T);
			}
		}

		public void PushState(EntityState st) {
			state?.Destroy();

			state = st;
			state.Assign(Entity);
			state.Init();

			Send(new StateChangedEvent {
				NewState = state.GetType(),
				State = state
			});
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (newState != null) {
				PushState((EntityState) Activator.CreateInstance(newState));
				newState = null;
			}
			
			state?.Update(dt);
		}
	}
}