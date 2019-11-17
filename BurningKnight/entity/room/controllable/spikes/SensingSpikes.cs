using BurningKnight.entity.creature.player;

namespace BurningKnight.entity.room.controllable.spikes {
	public class SensingSpikes : Spikes {
		private float timer;
		
		
		public override void Update(float dt) {
			base.Update(dt);

			if (timer > 0) {
				timer -= dt;

				if (timer <= 0) {
					TurnOff();
				}
			} else if (!On && Colliding.Count > 0) {
				foreach (var c in Colliding) {
					if (c is Player) {
						timer = 1.5f;
						TurnOnSlowly();
						
						break;
					}
				}
			}
		}
	}
}