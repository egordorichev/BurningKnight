namespace BurningKnight.box2dLight {
	public class PointLight : PositionalLight {
		public PointLight(RayHandler RayHandler, int Rays) {
			this(RayHandler, Rays, Light.DefaultColor, 15f, 0f, 0f);
		}

		public PointLight(RayHandler RayHandler, int Rays, Color Color, float Distance, float X, float Y) {
			base(RayHandler, Rays, Color, Distance, X, Y, 0f);
		}

		public override Void Update() {
			UpdateBody();

			if (Dirty) SetEndPoints();


			if (Cull()) return;


			if (StaticLight && !Dirty) return;


			Dirty = false;
			UpdateMesh();
		}

		public override Void SetDistance(float Dist) {
			Dist *= RayHandler.GammaCorrectionParameter;
			this.Distance = Dist < 0.01f ? 0.01f : Dist;
			Dirty = true;
		}

		public Void SetEndPoints() {
			float AngleNum = 360f / (RayNum - 1);

			for (int I = 0; I < RayNum; I++) {
				float Angle = AngleNum * I;
				Sin[I] = MathUtils.SinDeg(Angle);
				Cos[I] = MathUtils.CosDeg(Angle);
				EndX[I] = Distance * Cos[I];
				EndY[I] = Distance * Sin[I];
			}
		}

		public override Void SetDirection(float DirectionDegree) {

		}
	}
}
