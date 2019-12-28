using BurningKnight.entity.room.controllable.spikes;
using Lens.entity.component.logic;

namespace BurningKnight.entity.room.controllable {
	public class AlwaysOnSpikes : Spikes {
		protected override void InitState() {
			On = true;
			GetComponent<StateComponent>().Become<IdleState>();
		}
	}
}