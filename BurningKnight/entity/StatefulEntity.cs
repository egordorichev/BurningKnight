using BurningKnight.util.geometry;

namespace BurningKnight.entity {
	public class StatefulEntity : Entity {
		protected string State = "idle";
		public float T;
		public Point Velocity = new Point();

		public string GetState() {
			return State;
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			T += Dt;
		}

		public void Become(string State) {
			if (!this.State.Equals(State)) {
				this.State = State;
				T = 0;
			}
		}
	}
}