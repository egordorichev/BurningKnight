using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.game;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity {
	public class Entity : Point {
		protected Area Area;
		public int Depth = 0;
		public float W = 16;
		public float H = 16;
		public bool OnScreen = true;
		public bool AlwaysActive = false;
		public bool AlwaysRender = false;
		public bool Done = false;
		public int Id;
		protected bool Active = true;
		private const float DISTANCE = 256f;

		public Void SetActive(bool Active) {
			this.Active = Active;
		}

		public bool IsActive() {
			return Active;
		}

		public Void SetId(int Id) {
			this.Id = Id;
		}

		public float GetDistanceTo(float X, float Y) {
			float Dx = X - this.X - this.W / 2;
			float Dy = Y - this.Y - this.H / 2;

			return (float) Math.Sqrt(Dx * Dx + Dy * Dy);
		}

		public float GetAngleTo(float X, float Y) {
			float Dx = X - this.X - this.W / 2;
			float Dy = Y - this.Y - this.H / 2;

			return (float) Math.Atan2(Dy, Dx);
		}

		public Void SetDone(bool Done) {
			this.Done = Done;
		}

		public int GetId() {
			return this.Id;
		}

		public Void Init() {

		}

		public Void Destroy() {

		}

		public Void Update(float Dt) {

		}

		public Void Render() {

		}

		public Void RenderShadow() {

		}

		public Void SetArea(Area Area) {
			this.Area = Area;
		}

		public int GetDepth() {
			return this.Depth;
		}

		public Area GetArea() {
			return this.Area;
		}

		public Void OnCollision(Entity Entity) {

		}

		public Void OnCollisionEnd(Entity Entity) {

		}

		public bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			return true;
		}

		public bool IsOnScreen() {
			OrthographicCamera Camera = Camera.Game;
			float Zoom = Camera.Zoom;

			return this.X + this.W * 2f >= Camera.Position.X - Display.GAME_WIDTH / 2 * Zoom && this.Y + this.H * 2f >= Camera.Position.Y - Display.GAME_HEIGHT / 2 * Zoom && this.X <= Camera.Position.X + Display.GAME_WIDTH / 2 * Zoom && this.Y <= Camera.Position.Y + this.H + Display.GAME_HEIGHT / 2 * Zoom;
		}

		public Long PlaySfx(string Sound) {
			return PlaySfx(Sound, 1f);
		}

		public Long PlaySfx(string Sound, float Pitch) {
			if (this is Player) {
				return Audio.PlaySfx(Sound);
			} 

			if (!this.OnScreen) {
				return -1;
			} 

			if (Player.Instance == null) {
				return -1;
			} 

			float D = this.GetDistanceTo(Player.Instance.X + 8, Player.Instance.Y + 8);

			if (D >= DISTANCE) {
				return -1;
			} 

			return Audio.PlaySfx(Sound, (DISTANCE - D) / DISTANCE, Sound.StartsWith("menu") ? 1f : Math.Min(1.5f, Pitch + Random.NewFloat(0.3f)));
		}
	}
}
