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

		public override void Update(float dt) {
			base.Update(dt);

			if (newState != null) {
				state?.Destroy();
				
				state = (EntityState) Activator.CreateInstance(newState);
				state.Self = Entity;
				state.Init();

				if (Entity.TryGetCompoenent(out AnimationComponent anim)) {
					anim.Animation.Tag = state.GetType().Name.ToLower().Replace("state", "");
				}
				
				newState = null;
			}
			
			state?.Update(dt);
		}
	}
}