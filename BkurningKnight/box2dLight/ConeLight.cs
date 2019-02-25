namespace BurningKnight.box2dLight {
	public class ConeLight : PositionalLight {
		public float ConeDegree;

		public ConeLight(RayHandler RayHandler, int Rays, Color Color, float Distance, float X, float Y, float DirectionDegree, float ConeDegree) {
			base(RayHandler, Rays, Color, Distance, X, Y, DirectionDegree);
			SetConeDegree(ConeDegree);
		}

		public override Void Update() {
			UpdateBody();

			if (Dirty) SetEndPoints();


			if (Cull()) return;


			if (StaticLight && !Dirty) return;


			Dirty = false;
			UpdateMesh();
		}

		public Void SetDirection(float Direction) {
			this.Direction = Direction;
			Dirty = true;
		}

		public float GetConeDegree() {
			return ConeDegree;
		}

		public Void SetConeDegree(float ConeDegree) {
			this.ConeDegree = MathUtils.Clamp(ConeDegree, 0f, 180f);
			Dirty = true;
		}

		public Void SetDistance(float Dist) {
			Dist *= RayHandler.GammaCorrectionParameter;
			this.Distance = Dist < 0.01f ? 0.01f : Dist;
			Dirty = true;
		}

		protected Void SetEndPoints() {
			for (int I = 0; I < RayNum; I++) {
				float Angle = Direction + ConeDegree - 2f * ConeDegree * I / (RayNum - 1f);
				float S = Sin[I] = MathUtils.SinDeg(Angle);
				float C = Cos[I] = MathUtils.CosDeg(Angle);
				EndX[I] = Distance * C;
				EndY[I] = Distance * S;
			}
		}
	}
}
