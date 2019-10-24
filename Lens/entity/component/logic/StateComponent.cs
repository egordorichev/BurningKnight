using System;
using ImGuiNET;
using Lens.entity.component.graphics;

namespace Lens.entity.component.logic {
	public class StateComponent : Component {
		private EntityState state;
		private Type newState;

		public bool PauseOnChange;
		
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
		public int Pause;
		
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
			
			if (PauseOnChange) {
				PauseOnChange = false;
				Pause = 1;
			}
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (newState != null) {
				PushState((EntityState) Activator.CreateInstance(newState));
				newState = null;
			}

			if (!Engine.EditingLevel && Pause < 1) {
				state?.Update(dt);
			}
		}

		public override bool HandleEvent(Event e) {
			state?.HandleEvent(e);
			return base.HandleEvent(e);
		}

		public override void RenderDebug() {
			ImGui.Text($"State: {(state == null ? "null" : state.GetType().Name)}");
			var paused = Pause > 0;

			if (ImGui.Checkbox("Paused", ref paused)) {
				Pause = paused ? 1 : 0;
			}
		}
	}
}