using BurningKnight.core.util;

namespace BurningKnight.core.entity.item {
	public class ItemPrice : Entity {
		protected void _Init() {
			{
				Depth = -1;
			}
		}

		public int Price;
		private string Text;
		public bool Sale;
		private bool Did;
		private float A;

		public Void Remove() {
			if (Did) {
				return;
			} 

			Did = true;
			Tween.To(new Tween.Task(0, 0.2f) {
				public override float GetValue() {
					return A;
				}

				public override Void SetValue(float Value) {
					A = Value;
				}
			});
		}

		public override Void Init() {
			base.Init();
			this.Text = string.ValueOf(this.Price);
			Graphics.Layout.SetText(Graphics.Medium, Text);
			this.W = Graphics.Layout.Width;
			this.H = Graphics.Layout.Height * 2;
			this.X -= this.W / 2;
			this.Y -= 16;
			Tween.To(new Tween.Task(1, 0.2f) {
				public override float GetValue() {
					return A;
				}

				public override Void SetValue(float Value) {
					A = Value;
				}
			});
		}

		public override Void Render() {
			base.Render();

			if (Sale) {
				Graphics.Medium.SetColor(1, 0.6f, 0.2f, this.A);
			} else {
				Graphics.Medium.SetColor(1, 1, 1, this.A);
			}


			Graphics.Medium.Draw(Graphics.Batch, Text, this.X, this.Y + 16);
			Graphics.Medium.SetColor(1, 1, 1, 1);
		}

		public ItemPrice() {
			_Init();
		}
	}
}
