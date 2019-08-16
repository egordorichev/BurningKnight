namespace BurningKnight.entity.room.controllable.turret {
	public class QuadRotatingTurret : QuadTurret {
		public override void Init() {
			base.Init();
			Rotates = true;
		}
	}
}