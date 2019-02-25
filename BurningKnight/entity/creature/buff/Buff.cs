namespace BurningKnight.entity.creature.buff {
	public class Buff {
		protected float Duration;
		protected bool Ended;
		protected string Id;
		public bool Infinite = false;
		protected string Name;
		protected Creature Owner;
		private TextureRegion Region;
		protected float Time;

		public Buff(float Duration) {
			this.Duration = Duration;
		}

		public Buff() {
			Duration = Time;
		}

		public void OnStart() {
		}

		public void OnEnd() {
		}

		public Buff SetDuration(float Duration) {
			this.Duration = Duration;

			return this;
		}

		protected void OnUpdate(float Dt) {
		}

		public void Render(Creature Creature) {
		}

		public void Update(float Dt) {
			Time += Dt;
			OnUpdate(Dt);

			if (Time >= Duration) {
				if (Infinite) {
					Time = 0;
				}
				else {
					Ended = true;
					OnEnd();
					Owner.RemoveBuff(GetId());
				}
			}
		}

		public void SetOwner(Creature Owner) {
			this.Owner = Owner;
		}

		public string GetName() {
			return Name;
		}

		public string GetId() {
			return Id;
		}

		public float GetDuration() {
			return Duration;
		}

		public float GetTime() {
			return Time;
		}
	}
}