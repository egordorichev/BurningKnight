namespace BurningKnight.core.entity.trap {
	public class FourSideRotatingTurret : FourSideTurret {
		protected override Void Rotate() {
			this.A += Math.PI / 4;
			this.Str = !this.Str;
			this.Tween();
			this.T = 0;
		}
	}
}
