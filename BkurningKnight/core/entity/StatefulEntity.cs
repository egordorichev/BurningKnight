using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity {
	public class StatefulEntity : Entity {
		public Point Velocity = new Point();
		public float T;
		protected string State = "idle";

		public string GetState() {
			return State;
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			this.T += Dt;
		}

		public Void Become(string State) {
			if (!this.State.Equals(State)) {
				this.State = State;
				this.T = 0;
			} 
		}
	}
}
