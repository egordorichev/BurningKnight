namespace BurningKnight.core.entity.creature.buff {
	public class Buff {
		protected string Name;
		protected string Id;
		protected float Duration;
		protected float Time;
		public bool Infinite = false;
		protected Creature Owner;
		protected bool Ended;
		private TextureRegion Region;

		public Buff(float Duration) {
			this.Duration = Duration;
		}

		public Buff() {
			this.Duration = this.Time;
		}

		public Void OnStart() {

		}

		public Void OnEnd() {

		}

		public Buff SetDuration(float Duration) {
			this.Duration = Duration;

			return this;
		}

		protected Void OnUpdate(float Dt) {

		}

		public Void Render(Creature Creature) {

		}

		public Void Update(float Dt) {
			this.Time += Dt;
			this.OnUpdate(Dt);

			if (this.Time >= this.Duration) {
				if (this.Infinite) {
					this.Time = 0;
				} else {
					this.Ended = true;
					this.OnEnd();
					this.Owner.RemoveBuff(this.GetId());
				}

			} 
		}

		public Void SetOwner(Creature Owner) {
			this.Owner = Owner;
		}

		public string GetName() {
			return this.Name;
		}

		public string GetId() {
			return this.Id;
		}

		public float GetDuration() {
			return this.Duration;
		}

		public float GetTime() {
			return this.Time;
		}
	}
}
