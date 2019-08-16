namespace BurningKnight.entity.room.controllable.turret {
	public class RotatingTurret : Turret {
		public override void Init() {
			base.Init();
			Rotates = true;
		}
	}
}