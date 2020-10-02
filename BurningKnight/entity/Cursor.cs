using System;
using BurningKnight.assets;
using BurningKnight.assets.input;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.state;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.graphics;
using Lens.input;
using Lens.util;
using Lens.util.camera;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity {
	public class Cursor : Entity, CustomCameraJumper {
		private static TextureRegion[] regions;

		private Vector2 scale = new Vector2(1);
		private Vector2 stickOffset;
		private bool needsAdjusting = true;
		private bool readTint = true;
		private Color tint;

		public Player Player;
		public Vector2 GamePosition;

		public override void Init() {
			base.Init();

			AlwaysActive = true;
			AlwaysVisible = true;
			Depth = Layers.Cursor;
			Width = 0;
			Height = 0;
			
			AddTag(Tags.Cursor);

			if (regions == null) {
				regions = new[] {
					CommonAse.Ui.GetSlice("cursor_a"),
					CommonAse.Ui.GetSlice("cursor_b"),
					CommonAse.Ui.GetSlice("cursor_c"),
					CommonAse.Ui.GetSlice("cursor_d"),
					CommonAse.Ui.GetSlice("cursor_e"),
					CommonAse.Ui.GetSlice("cursor_f"),
					CommonAse.Ui.GetSlice("cursor_g"),
					CommonAse.Ui.GetSlice("cursor_j"),
					CommonAse.Ui.GetSlice("cursor_k")
				};
			}
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Player.Dead) {
				var found = false;
				
				foreach (var e in Area.Entities.Entities) {
					if (e is Cursor && e != this) {
						found = true;
						break;
					}
				}

				if (found) {
					Done = true;
					return;
				}
			}
			
			if (readTint) {
				readTint = false;
				tint = Player.Tint;
			}
			
			var input = Player.GetComponent<InputComponent>();
			
			if (input.KeyboardEnabled && (Input.Mouse.WasMoved || !input.GamepadEnabled || input.GamepadData == null || input.GamepadData.Attached)) {
				Position = Camera.Instance.CameraToUi(GamePosition = Camera.Instance.ScreenToCamera(Input.Mouse.ScreenPosition));
			}

			var controller = input.GamepadEnabled ? input.GamepadData : null;
			
			if (controller != null && Engine.Instance.State is InGameState st && !st.Paused && !st.Died && !Run.Won) {
      	if (needsAdjusting) {
	        needsAdjusting = false;
	        Position = Camera.Instance.CameraToUi(GamePosition = Player.Center);
        }
				
				var stick = controller.GetRightStick();

      	var dx = stick.X;
      	var dy = stick.Y;
      	var d = (float) Math.Sqrt(dx * dx + dy * dy);

      	if (d > 1) {
      		stick /= d;
      	} else {
      		stick *= d;
      	}

        var l = stick.Length();

        if (l > 0.25f) {
      		var target = MathUtils.CreateVector(Math.Atan2(dy, dx), 1f);
      		dx = target.X - stickOffset.X;
      		dy = target.Y - stickOffset.Y;

      		d = (float) Math.Sqrt(dx * dx + dy * dy);

      		if (d > 1) {
      			dx /= d;
      			dy /= d;
      		} else {
      			dx *= d;
      			dy *= d;
      		}

      		stickOffset += l * new Vector2(dx, dy) * dt * 10f * Settings.Sensivity;
	        Position = Camera.Instance.CameraToUi(GamePosition = (Player.Center + stickOffset * (48 * Settings.CursorRadius)));

      		double a = 0;
      		var pressed = false;

      		if (controller.DPadLeftCheck) {
      			a = Math.PI;
      			pressed = true;
      		} else if (controller.DPadDownCheck) {
      			a = Math.PI / 2f;
      			pressed = true;
      		} else if (controller.DPadUpCheck) {
      			a = Math.PI * 1.5f;
      			pressed = true;
      		} else if (controller.DPadRightCheck) {
      			pressed = true;
      		}

      		if (pressed) {
      			Position = Camera.Instance.CameraToUi(GamePosition = (Player.Center + MathUtils.CreateVector(a, 48)));
      		}
        }
      }

			if (Input.WasPressed(Controls.Use, input)) {
				Tween.To(1.3f, scale.X, x => { scale.X = scale.Y = x; }, 0.05f).OnEnd = () =>
					Tween.To(1f, scale.X, x => { scale.X = scale.Y = x; }, 0.15f);
			}
		}

		public override void Render() {
			if (Settings.HideCursor) {
				return;
			}

			var r = regions[Settings.Cursor];

			if (InGameState.Multiplayer) {
				Graphics.Color = tint;
			}

			Graphics.Render(r, Position, 0, r.Center, scale);
			Graphics.Color = ColorUtils.WhiteColor;
		}

		public Vector2 Jump(Camera.Target target) {
			return new Vector2();
			/*Position = Camera.Instance.CameraToUi(GamePosition = Camera.Instance.ScreenToCamera(Input.Mouse.ScreenPosition));
			return new Vector2((CenterX - Display.UiWidth * 0.5f) * target.Priority, (CenterY - Display.UiHeight * 0.5f) * target.Priority * Display.Viewport * 1.6f)*/;	
		}
	}
}