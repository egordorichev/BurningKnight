using BurningKnight.entity.creature.player;

namespace BurningKnight.entity.twitch.happening {
	public abstract class Happening {
		public abstract void Happen(Player player);

		public virtual void Update(float dt) {
			
		}
		
		public virtual void End(Player player) {
			
		}

		public virtual float GetVoteDelay() {
			return 30;
		}
	}
}