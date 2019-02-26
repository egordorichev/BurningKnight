namespace BurningKnight.entity.trap {
	public class FourSideRotatingTurret : FourSideTurret {
		protected override void Rotate() {
			A += Math.PI / 4;
			Str = !Str;
			Tween();
			T = 0;
		}
	}
}