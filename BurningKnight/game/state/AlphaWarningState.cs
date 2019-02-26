using BurningKnight.game.input;
using BurningKnight.util;

namespace BurningKnight.game.state {
	public class AlphaWarningState : State {
		private float Alpha;
		private bool Did;

		public override void Init() {
			base.Init();
			Dungeon.SetBackground(new Color(0, 0, 0, 1));
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			Alpha += ((Did ? 0 : 1) - Alpha) * Dt * 5;

			if (!Did && Input.Instance.WasPressed("X")) {
				Did = true;
				Color Color = Color.WHITE;
				var T = 0.1f;
				Tween.To(new Tween.Task(Color.R, T) {

		public override float GetValue() {
			return 0;
		}

		public override void SetValue(float Value) {
			Dungeon.GetBackground2().R = Value;
		}

		public override void OnEnd() {
			Color Color = Color.ValueOf("#1a1932");
			var T = 0.2f;
			Tween.To(new Tween.Task(Color.R, T) {

		public override float GetValue() {
			return Dungeon.GetBackground2().R;
		}

		public override void SetValue(float Value) {
			Dungeon.GetBackground2().R = Value;
		}

		public override void OnEnd() {
			Dungeon.Game.SetState(new MainMenuState());
		}
	});

	Tween.To(new Tween.Task(Color.G, T) {
	public override float GetValue() {
		return Dungeon.GetBackground2().G;
	}

	public override void SetValue(float Value) {
		Dungeon.GetBackground2().G = Value;
	}
	});
	Tween.To(new Tween.Task(Color.B, T) {
	public override float GetValue() {
		return Dungeon.GetBackground2().B;
	}

	public override void SetValue(float Value) {
		Dungeon.GetBackground2().B = Value;
	}
	});
}

}).Delay(0.5f);
Tween.To(new Tween.Task(Color.G, T) {
public override float GetValue() {
return 0;
}
public override void SetValue(float Value) {
Dungeon.GetBackground2().G = Value;
}
}).Delay(0.5f);
Tween.To(new Tween.Task(Color.B, T) {
public override float GetValue() {
return 0;
}
public override void SetValue(float Value) {
Dungeon.GetBackground2().B = Value;
}
}).Delay(0.5f);
}
}
public override void RenderUi() {
base.RenderUi();
float V = Did ? 0.7f : 1f;
Graphics.Small.SetColor(V, V, V, this.Alpha);
int Y = Display.UI_HEIGHT / 3 * 2;
Graphics.Print("Warning", Graphics.Small, Y);
Graphics.Print("This game is still in alpha", Graphics.Small, Y - 32);
Graphics.Print("This means, that it still might have bugs in it", Graphics.Small, Y - 48);
Graphics.Print("If you meet any, please report them", Graphics.Small, Y - 64);
Graphics.Print("Press X to continue", Graphics.Small, Y - 96);
Graphics.Small.SetColor(1, 1, 1, 1);
}
}
}