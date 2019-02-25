using BurningKnight.core.assets;
using BurningKnight.core.entity.item;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.player.fx {
	public class ItemPickedFx : Entity {
		protected void _Init() {
			{
				Depth = 15;
				AlwaysActive = true;
			}
		}

		private string Text;
		private float A;

		public ItemPickedFx(ItemHolder Item) {
			_Init();
			Item I = Item.GetItem();
			this.Text = "+" + I.GetName();
			this.A = 1f;
			Graphics.Layout.SetText(Graphics.Medium, this.Text);
			this.X = Item.X + Item.W / 2 - Graphics.Layout.Width / 2;
			this.Y = Item.Y + Item.H + 4;
			this.Tween();
		}

		private Void Tween() {
			Tween.To(new Tween.Task(this.Y + 10, 2f, Tween.Type.QUAD_OUT) {
				public override float GetValue() {
					return Y;
				}

				public override Void SetValue(float Value) {
					Y = Value;
				}
			});
			Tween.To(new Tween.Task(0, 2f) {
				public override float GetValue() {
					return A;
				}

				public override Void SetValue(float Value) {
					A = Value;
				}

				public override Void OnEnd() {
					SetDone(true);
				}
			});
		}

		public override Void Render() {
			Graphics.Print(this.Text, Graphics.Medium, this.X, this.Y);
		}
	}
}
