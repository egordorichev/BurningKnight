namespace BurningKnight.entity.trap {
	public class RotatingTurret : Turret {
		protected override void Rotate() {
			A += Math.PI / 4;
			Frame -= 1;
			Tween();
		}

		protected override void SetFrame() {
			Single.SetFrame(Math.FloorMod(Frame, 8));
		}
	}
}