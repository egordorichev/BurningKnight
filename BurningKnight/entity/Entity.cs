using BurningKnight.entity.creature.player;
using BurningKnight.game;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity {
	public class Entity : Point {
		private const float DISTANCE = 256f;
		protected bool Active = true;
		public bool AlwaysActive = false;
		public bool AlwaysRender = false;
		protected Area Area;
		public int Depth = 0;
		public bool Done;
		public float H = 16;
		public int Id;
		public bool OnScreen = true;
		public float W = 16;

		public void SetActive(bool Active) {
			this.Active = Active;
		}

		public bool IsActive() {
			return Active;
		}

		public void SetId(int Id) {
			this.Id = Id;
		}

		public float GetDistanceTo(float X, float Y) {
			var Dx = X - this.X - W / 2;
			var Dy = Y - this.Y - H / 2;

			return (float) Math.Sqrt(Dx * Dx + Dy * Dy);
		}

		public float GetAngleTo(float X, float Y) {
			var Dx = X - this.X - W / 2;
			var Dy = Y - this.Y - H / 2;

			return (float) Math.Atan2(Dy, Dx);
		}

		public void SetDone(bool Done) {
			this.Done = Done;
		}

		public int GetId() {
			return Id;
		}

		public void Init() {
		}

		public void Destroy() {
		}

		public void Update(float Dt) {
		}

		public void Render() {
		}

		public void RenderShadow() {
		}

		public void SetArea(Area Area) {
			this.Area = Area;
		}

		public int GetDepth() {
			return Depth;
		}

		public Area GetArea() {
			return Area;
		}

		public void OnCollision(Entity Entity) {
		}

		public void OnCollisionEnd(Entity Entity) {
		}

		public bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			return true;
		}

		public bool IsOnScreen() {
			OrthographicCamera Camera = Camera.Game;
			float Zoom = Camera.Zoom;

			return this.X + W * 2f >= Camera.Position.X - Display.GAME_WIDTH / 2 * Zoom && this.Y + H * 2f >= Camera.Position.Y - Display.GAME_HEIGHT / 2 * Zoom && this.X <= Camera.Position.X + Display.GAME_WIDTH / 2 * Zoom &&
			       this.Y <= Camera.Position.Y + H + Display.GAME_HEIGHT / 2 * Zoom;
		}

		public long PlaySfx(string Sound) {
			return PlaySfx(Sound, 1f);
		}

		public long PlaySfx(string Sound, float Pitch) {
			if (this is Player) return Audio.PlaySfx(Sound);

			if (!OnScreen) return -1;

			if (Player.Instance == null) return -1;

			var D = GetDistanceTo(Player.Instance.X + 8, Player.Instance.Y + 8);

			if (D >= DISTANCE) return -1;

			return Audio.PlaySfx(Sound, (DISTANCE - D) / DISTANCE, Sound.StartsWith("menu") ? 1f : Math.Min(1.5f, Pitch + Random.NewFloat(0.3f)));
		}
	}
}