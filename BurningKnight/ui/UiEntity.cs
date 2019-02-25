using BurningKnight.entity;
using BurningKnight.game.state;

namespace BurningKnight.ui {
	public class UiEntity : Entity {
		protected bool IsSelectable = true;

		protected bool IsSelected;
		protected bool WasSelected;

		public UiEntity() {
			_Init();
		}

		protected void _Init() {
			{
				AlwaysRender = true;
				AlwaysActive = true;
			}
		}

		public bool IsSelected() {
			return IsSelected;
		}

		public override void Update(float Dt) {
			base.Update(Dt);
		}

		public void Select() {
			IsSelected = true;
		}

		public void Unselect() {
			IsSelected = false;
		}

		public bool IsSelectable() {
			return IsSelectable;
		}

		public override bool IsOnScreen() {
			OrthographicCamera Camera = Camera.Ui;
			float Zoom = Camera.Zoom;

			return this.X + W * 2 >= Camera.Position.X - Display.GAME_WIDTH / 2 * Zoom + State.SettingsX && this.Y + H * 2 >= Camera.Position.Y - Display.GAME_HEIGHT / 2 * Zoom &&
			       this.X <= Camera.Position.X + Display.GAME_WIDTH / 2 * Zoom + State.SettingsX && this.Y <= Camera.Position.Y + H + Display.GAME_HEIGHT / 2 * Zoom;
		}
	}
}