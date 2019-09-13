using System;
using Lens.util.tween;

namespace BurningKnight.ui {
	public static class UiSlider {
		public static UiSliderLabel Make(UiPane pane, float x, float y, string label, int value) {
			var a = pane.Add(new UiLabel {
				LocaleLabel = label,
				RelativeX = x,
				RelativeY = y
			});

			UiSliderLabel c = null;

			var b = pane.Add(new UiButton {
				Label = "-",
				RelativeX = x + a.Width + 10,
				RelativeY = y,
				Click = () => {
					c.Value = Math.Max(0, c.Value - 10);
				},
				Padding = 5,
				ScaleMod = 2
			});

			c = (UiSliderLabel) pane.Add(new UiSliderLabel {
				Label = "100%",
				RelativeX = x + a.Width + b.Width + 20,
				RelativeY = y
			});

			c.Value = value;
			
			var d = pane.Add(new UiButton {
				Label = "+",
				RelativeX = x + a.Width + b.Width + c.Width + 30,
				RelativeY = y,
				Click = () => {
					c.Value = Math.Min(100, c.Value + 10);
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

		public class UiSliderLabel : UiLabel {
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