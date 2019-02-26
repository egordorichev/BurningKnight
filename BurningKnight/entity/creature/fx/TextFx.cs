using BurningKnight.util;

namespace BurningKnight.entity.creature.fx {
	public class TextFx : Entity {
		private float A = 1f;
		private Color Color = Color.WHITE;
		private string Text;

		public TextFx(string Text, float X, float Yv) {
			this.Text = Text;
			Depth = 15;
			Graphics.Layout.SetText(Graphics.Medium, this.Text);
			W = Graphics.Layout.Width;
			this.X = X - Graphics.Layout.Width / 2;
			this.Y = Yv;
			Tween.To(new Tween.Task(this.Y + 8, 0.3f) {

		public override float GetValue() {
			return Y;
		}

		public override void SetValue(float Value) {
			Y = Value;
		}

		public override void OnEnd() {
			Tween.To(new Tween.Task(0, 0.5f) {

		public override float GetValue() {
			return A;
		}

		public override void SetValue(float Value) {
			A = Value;
		}

		public override void OnEnd() {
			Done = true;
		}
	});
}

});
}
public TextFx(string Text, Entity Entity) {
this(Text, Entity.X + Entity.W / 2, Entity.Y + Entity.H - 4);
}
public TextFx SetColor(Color Color) {
this.Color = Color;
return this;
}
public override void Render() {
Graphics.Medium.SetColor(this.Color.R, this.Color.G, this.Color.B, this.A);
Graphics.Write(this.Text, Graphics.Medium, this.X, this.Y);
Graphics.Medium.SetColor(1, 1, 1, 1);
}
}
}