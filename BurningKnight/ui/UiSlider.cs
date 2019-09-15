using System;
using Lens.input;
using Lens.util.tween;

namespace BurningKnight.ui {
	public static class UiSlider {
		public static UiSliderLabel Make(UiPane pane, float x, float y, string label, int value, int max = 100) {
			var a = pane.Add(new UiLabel {
				LocaleLabel = label,
				RelativeX = x,
				RelativeCenterY = y
			});

			UiSliderLabel c = null;

			var b = pane.Add(new UiButton {
				Label = "-",
				RelativeX = x + a.Width + 10,
				RelativeCenterY = y,
				Click = bt => {
					c.Value = Math.Max(0, c.Value - 10);
				},
				Padding = 5,
				ScaleMod = 2
			});

			c = (UiSliderLabel) pane.Add(new UiSliderLabel {
				Label = $"{max}%",
				RelativeX = x + a.Width + b.Width + 20,
				RelativeCenterY = y,
				Click = bt => {
					var n = (UiSliderLabel) bt;
					
					if (Input.Mouse.CheckRightButton) {
						n.Value = Math.Max(0, n.Value - 10);
					} else {
						n.Value = Math.Min(max, n.Value + 10);
					}
				}
			});

			c.Value = value;
			
			var d = pane.Add(new UiButton {
				Label = "+",
				RelativeX = x + a.Width + b.Width + c.Width + 30,
				RelativeCenterY = y,
				Click = bt => {
					c.Value = Math.Min(max, c.Value + 10);
				},
				Padding = 5,
				ScaleMod = 2
			});

			var m = (d.RelativeX + d.Width - a.RelativeX) / 2f;
			
			a.RelativeX -= m;
			b.RelativeX -= m;
			c.RelativeX -= m;
			d.RelativeX -= m;

			return c;
		}

		public class UiSliderLabel : UiButton {
			private int value;
			public Action<UiSliderLabel> OnValueChange;

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
	}
}