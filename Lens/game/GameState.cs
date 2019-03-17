using Lens.entity;

namespace Lens.game {
	public class GameState {
		public static bool RenderDebug = false;

		public Area Area = new Area();
		public Area Ui = new Area();

		private bool paused;
		
		public bool Paused {
			get {
				return paused;
			}
			
			set {
				if (paused != value) {
					paused = value;

					if (paused) {
						OnPause();
					} else {
						OnResume();						
					}
				}
			}
		}

		public virtual void OnActivated() {
			
		}

		public virtual void OnDeactivated() {
			
		}

		protected virtual void OnPause() {
			
		}

		protected virtual void OnResume() {
			
		}

		public virtual void Init() {
			
		}

		public virtual void Destroy() {
			Area?.Destroy();
			Ui.Destroy();
		}

		public virtual void Update(float dt) {
			Area?.Update(dt);
			Ui.Update(dt);
		}

		public virtual void Render() {
			Area?.Render();

			if (RenderDebug) {
				Area?.RenderDebug();
			}
		}

		public virtual void RenderUi() {
			Ui.Render();

			if (RenderDebug) {
				Ui.RenderDebug();
			}

			Engine.Instance.RenderUi();
		}
	}
}