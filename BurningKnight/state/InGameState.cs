using BurningKnight.assets;
using BurningKnight.entity;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.ui;
using BurningKnight.physics;
using BurningKnight.save;
using BurningKnight.util;
using Lens;
using Lens.entity;
using Lens.game;
using Lens.graphics;
using Lens.input;
using Lens.util.camera;
using Lens.util.tween;
using Console = BurningKnight.debug.Console;

namespace BurningKnight.state {
	public class InGameState : GameState {
		private bool pausedByMouseOut;
		private bool pausedByLostFocus;
		private float blur;
		
		public InGameState(Area area) {
			Area = area;
		}
		
		public override void Init() {
			base.Init();
			SetupUi();
		}

		public override void Destroy() {
			Physics.Destroy();
			// Fixme: enable
			// SaveManager.SaveAll(Area);
			Area = null;

			// Clears the area, but we don't want that, cause we are still saving
			base.Destroy();
		}

		protected override void OnPause() {
			base.OnPause();
			Tween.To(this, new {blur = 1}, 0.25f);
		}

		protected override void OnResume() {
			base.OnResume();
			pausedByMouseOut = false;

			Tween.To(this, new {blur = 0}, 0.25f);
		}

		public override void OnActivated() {
			base.OnActivated();

			if (Paused && pausedByLostFocus) {
				Paused = false;
			}
		}

		public override void OnDeactivated() {
			base.OnDeactivated();

			Paused = true;
			pausedByLostFocus = true;
			pausedByMouseOut = false;
		}

		public override void Update(float dt) {
			var inside = Engine.GraphicsDevice.Viewport.Bounds.Contains(Input.Mouse.CurrentState.Position);
			Shaders.Screen.Parameters["blur"].SetValue(blur);
			
			if (!Paused && !inside) {
				Paused = true;
				pausedByMouseOut = true;
			} else if (Paused && pausedByMouseOut && inside) {
				Paused = false;
			}
			
			if (!Paused) {
				Physics.Update(dt);
				base.Update(dt);

				if (Input.WasPressed(Controls.Pause)) {
					Paused = true;
				}
			}
		}

		public override void Render() {
			base.Render();
			Physics.Render();
		}

		public override void RenderUi() {
			base.RenderUi();
			
			Graphics.Print($"{Engine.Instance.Counter.AverageFramesPerSecond}", Font.Small, 1, 1);

			if (Paused) {
				Graphics.Print("Paused", Font.Medium, 1, 16);
			}
		}

		private void SetupUi() {
			Ui.Add(new Camera(new FollowingDriver()));
			
			var cursor = new Cursor();
			Ui.Add(cursor);

			var player = LocalPlayer.Locate(Area);
			
			Camera.Instance.Follow(player, 1f);
			Camera.Instance.Follow(cursor, 1f);
			Camera.Instance.Jump();

			if (Engine.Version.Debug) {
				Ui.Add(new Console(Area));
			}
			
			Ui.Add(new UiInventory(player));
		}
	}
}