using BurningKnight.entity.item;
using BurningKnight.util;

namespace BurningKnight.entity.creature.player.fx {
	public class ItemPickedFx : Entity {
		private float A;

		private string Text;

		public ItemPickedFx(ItemHolder Item) {
			_Init();
			var I = Item.GetItem();
			Text = "+" + I.GetName();
			A = 1f;
			Graphics.Layout.SetText(Graphics.Medium, Text);
			this.X = Item.X + Item.W / 2 - Graphics.Layout.Width / 2;
			this.Y = Item.Y + Item.H + 4;
			Tween();
		}

		protected void _Init() {
			{
				Depth = 15;
				AlwaysActive = true;
			}
		}

		private void Tween() {
			Tween.To(new Tween.Task(this.Y + 10, 2f, Tween.Type.QUAD_OUT) {

		public override float GetValue() {
			return Y;
		}

		public override void SetValue(float Value) {
			Y = Value;
		}
	});

	Tween.To(new Tween.Task(0, 2f) {
	public override float GetValue() {
		return A;
	}

	public override void SetValue(float Value) {
		A = Value;
	}

	public override void OnEnd() {
		SetDone(true);
	}
	});
}

public override void Render() {
Graphics.Print(this.Text, Graphics.Medium, this.X, this.Y);
}
}
}