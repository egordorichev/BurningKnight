using BurningKnight;
using BurningKnight.entity;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.level.entities;
using BurningKnight.game.input;
using BurningKnight.game.state;
using BurningKnight.ui;
using BurningKnight.util;

namespace BurningKnight.entity.level.entities.fx {
	public class LadderFx : UiEntity {
		private float A;

		private Entity Ladder;
		private string Text;

		public LadderFx(Entity Ladder, string Text) {
			_Init();
			this.Ladder = Ladder;
			this.Text = Locale.Get(Text);
			GlyphLayout Layout = new GlyphLayout(Graphics.Medium, this.Text);
			this.X = Ladder.X + 8 - Layout.Width / 2;
			this.Y = Ladder.Y + Ladder.H + 4;
			Depth = 16;
			Tween.To(new Tween.Task(1, 0.1f, Tween.Type.QUAD_OUT) {

		protected void _Init() {
			{
				AlwaysActive = true;
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
if (Input.Instance.WasPressed("interact") && Dialog.Active == null && Player.Instance.PickupFx == null) {
	this.Remove();
	this.End();
}
}

public void End() {
Dungeon.DarkR = 0;
Dungeon.DarkR = Dungeon.MAX_R;
Player.Instance.SetUnhittable(true);
Camera.Follow(null);
Player.Instance.PlaySfx("menu/select");
InGameState.StartTween = true;
InGameState.Id = ((Exit) Ladder).
internal GetType();
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