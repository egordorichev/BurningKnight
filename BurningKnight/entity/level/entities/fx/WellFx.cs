using BurningKnight;
using BurningKnight.game.input;
using BurningKnight.util;

namespace BurningKnight.entity.level.entities.fx {
	public class WellFx : Entity {
		private float A;
		private string Text;

		private UsableProp Well;

		public WellFx(UsableProp Well, string Text) {
			_Init();
			this.Well = Well;
			this.Text = Locale.Get(Text);
			GlyphLayout Layout = new GlyphLayout(Graphics.Medium, this.Text);
			this.X = Well.X + 16 - Layout.Width / 2;
			this.Y = Well.Y + Well.H;
			Depth = 15;
			Tween.To(new Tween.Task(1, 0.1f, Tween.Type.QUAD_OUT) {

		protected void _Init() {
			{
				Depth = 30;
			}
		}

		public override float GetValue() {
			return A;
		}

		public override void SetValue(float Value) {
			A = Value;
		}
	});
}

public override void Render() {
internal float C = 0.8f + Math.Cos(Dungeon.Time * 10) / 5f;
Graphics.Medium.SetColor(C, C, C, this.A);
Graphics.Print(this.Text, Graphics.Medium, this.X, this.Y);
Graphics.Medium.SetColor(1, 1, 1, 1);
if (Input.Instance.WasPressed("interact") && Dialog.Active == null) {
	if (this.Well.Use()) this.Remove();
}
}

public void Remove() {
Tween.To(new Tween.Task(0, 0.2f, Tween.Type.QUAD_IN) {

public override float GetValue() {
	return A;
}

public override void SetValue(float Value) {
	A = Value;
}

public override void OnEnd() {
	base.OnEnd();
	SetDone(true);
}
});
}
}
}