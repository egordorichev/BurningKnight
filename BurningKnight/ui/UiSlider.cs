using System;
using BurningKnight.assets.input;
using BurningKnight.entity.component;
using Lens.input;
using Lens.util.tween;

namespace BurningKnight.ui {
	public static class UiSlider {
		public static UiSliderLabel Make(UiPane pane, float x, float y, string label, int value, int max = 100, int min = 0) {
			var a = (UiSliderButton) pane.Add(new UiSliderButton {
				LocaleLabel = label,
				RelativeX = x,
				RelativeCenterY = y
			});

			UiSliderLabel c = null;

			var b = pane.Add(new UiButton {
				Label = "-",
				XPadding = 4,
				Selectable = false,
				RelativeX = x + a.Width + 10,
				Type = ButtonType.Slider,
				RelativeCenterY = y,
				Click = bt => {
					c.Value = Math.Max(min, c.Value - 10);
				},
				ScaleMod = 3
			});

			c = (UiSliderLabel) pane.Add(new UiSliderLabel {
				Label = $"{value}%",
				RelativeX = x + a.Width + b.Width + 20,
				RelativeCenterY = y,
				Click = bt => {
					var n = (UiSliderLabel) bt;
					
					if (Input.Mouse.CheckRightButton) {
						n.Value = Math.Max(min, n.Value - 10);
					} else {
						n.Value = Math.Min(max, n.Value + 10);
					}
				}
			});

			c.Value = value;
			
			var d = pane.Add(new UiButton {
				Label = "+",
				XPadding = 4,
				Selectable = false,
				Type = ButtonType.Slider,
				RelativeX = x + a.Width + b.Width + c.Width + 30,
				RelativeCenterY = y,
				Click = bt => {
					c.Value = Math.Min(max, c.Value + 10);
				},
				ScaleMod = 3
			});

			var m = (d.RelativeX + d.Width - a.RelativeX) / 2f;
			
			a.RelativeX -= m;
			b.RelativeX -= m;
			c.RelativeX -= m;
			d.RelativeX -= m;

			a.Minus = (UiButton) b;
			a.Plus = (UiButton) d;

			return c;
		}

		public class UiSliderLabel : UiButton {
			private int value;
			public Action<UiSliderLabel> OnValueChange;

			public override void Init() {
				base.Init();
				Selectable = false;
			}

			public int Value {
				get => value;

				set {
					if (this.value == value) {
						return;
					}
					
					this.value = value;
					Label = $"{value}%";

					scale = 2;
					Tween.To(1f, scale, x => scale = x, 0.2f);
					
					OnValueChange?.Invoke(this);
				}
			}
		}

		public class UiSliderButton : UiButton {
			public UiButton Minus;
			public UiButton Plus;
			
			public override void Update(float dt) {
				base.Update(dt);
				
				if (Selected == Id) {
					if (Input.WasPressed(Controls.UiLeft, GamepadComponent.Current, true)) {
						Minus.OnClick();
					}
					
					if (Input.WasPressed(Controls.UiRight, GamepadComponent.Current, true)) {
						Plus.OnClick();
					}
				}
			}
		}
	}
}