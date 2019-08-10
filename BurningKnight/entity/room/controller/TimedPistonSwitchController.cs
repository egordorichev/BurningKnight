namespace BurningKnight.entity.room.controller {
	public class TimedPistonSwitchController : RoomController {
		public override void Update(float dt) {
			base.Update(dt);

			if (T >= 2f) {
				T = 0;
				
				foreach (var p in Room.Pistons) {
					p.Toggle();	
				}
			}
		}
	}
}