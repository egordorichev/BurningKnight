namespace BurningKnight.core.entity.trap {
	public class RotatingTurret : Turret {
		protected override Void Rotate() {
			this.A += Math.PI / 4;
			this.Frame -= 1;
			this.Tween();
		}

		protected override Void SetFrame() {
			this.Single.SetFrame(Math.FloorMod(Frame, 8));
		}
	}
}
