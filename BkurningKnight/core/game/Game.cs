using BurningKnight.core.assets;
using BurningKnight.core.entity;
using BurningKnight.core.game.state;
using BurningKnight.core.util;

namespace BurningKnight.core.game {
	public class Game {
		private State State;
		private State ToSet;

		public Game() {

		}

		public Void SetState(State State) {
			ToSet = State;
		}

		public Void DestroyState() {
			if (this.State != null) {
				State Old = this.State;
				this.State = null;
				Old.Destroy();
			} 
		}

		public State GetState() {
			return this.State;
		}

		public Void Update(float Dt) {
			if (ToSet != null) {
				if (this.State != null) {
					if (!(this.State is LoadState)) {
						Dungeon.Ui.Destroy();
						Dungeon.Area.Destroy();
					} 
				} 

				State Old = this.State;
				Camera.Instance.ResetShake();
				Log.Info("Set state to " + ToSet.GetClass().GetSimpleName());
				this.State = ToSet;
				ToSet = null;
				Dialog.Active = null;

				if (Old != null) {
					Old.Destroy();
				} 

				Dungeon.Blood = 0;
				this.State.Init();
			} 

			if (this.State != null) {
				this.State.Update(Dt);
			} 
		}

		public Void Render() {
			Render(true);
		}

		public Void Render(bool Ui) {
			if (this.State != null) {
				Graphics.Shape.SetProjectionMatrix(Camera.Game.Combined);

				if (!Ui) {
					this.State.Render();
				} else {
					RenderUi();
				}

			} 
		}

		public Void RenderUi() {
			if (this.State != null) {
				Graphics.Batch.SetProjectionMatrix(Camera.Ui.Combined);
				this.State.RenderUi();
			} 
		}

		public Void Destroy() {
			this.DestroyState();
		}
	}
}
