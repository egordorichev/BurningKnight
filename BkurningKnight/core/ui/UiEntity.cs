using BurningKnight.core.entity;
using BurningKnight.core.game.state;

namespace BurningKnight.core.ui {
	public class UiEntity : Entity {
		protected void _Init() {
			{
				AlwaysRender = true;
				AlwaysActive = true;
			}
		}

		protected bool IsSelected;
		protected bool IsSelectable = true;
		protected bool WasSelected;

		public bool IsSelected() {
			return IsSelected;
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
		}

		public Void Select() {
			IsSelected = true;
		}

		public Void Unselect() {
			IsSelected = false;
		}

		public bool IsSelectable() {
			return IsSelectable;
		}

		public override bool IsOnScreen() {
			OrthographicCamera Camera = Camera.Ui;
			float Zoom = Camera.Zoom;

			return this.X + this.W * 2 >= Camera.Position.X - Display.GAME_WIDTH / 2 * Zoom + State.SettingsX && this.Y + this.H * 2 >= Camera.Position.Y - Display.GAME_HEIGHT / 2 * Zoom && this.X <= Camera.Position.X + Display.GAME_WIDTH / 2 * Zoom + State.SettingsX && this.Y <= Camera.Position.Y + this.H + Display.GAME_HEIGHT / 2 * Zoom;
		}

		public UiEntity() {
			_Init();
		}
	}
}
