using BurningKnight.entity;
using BurningKnight.game.state;
using BurningKnight.util;

namespace BurningKnight.game {
	public class Game {
		private State State;
		private State ToSet;

		public void SetState(State State) {
			ToSet = State;
		}

		public void DestroyState() {
			if (State != null) {
				var Old = State;
				State = null;
				Old.Destroy();
			}
		}

		public State GetState() {
			return State;
		}

		public void Update(float Dt) {
			if (ToSet != null) {
				if (State != null)
					if (!(State is LoadState)) {
						Dungeon.Ui.Destroy();
						Dungeon.Area.Destroy();
					}

				var Old = State;
				Camera.Instance.ResetShake();
				Log.Info("Set state to " + ToSet.GetClass().GetSimpleName());
				State = ToSet;
				ToSet = null;
				Dialog.Active = null;

				if (Old != null) Old.Destroy();

				Dungeon.Blood = 0;
				State.Init();
			}

			if (State != null) State.Update(Dt);
		}

		public void Render() {
			Render(true);
		}

		public void Render(bool Ui) {
			if (State != null) {
				Graphics.Shape.SetProjectionMatrix(Camera.Game.Combined);

				if (!Ui)
					State.Render();
				else
					RenderUi();
			}
		}

		public void RenderUi() {
			if (State != null) {
				Graphics.Batch.SetProjectionMatrix(Camera.Ui.Combined);
				State.RenderUi();
			}
		}

		public void Destroy() {
			DestroyState();
		}
	}
}