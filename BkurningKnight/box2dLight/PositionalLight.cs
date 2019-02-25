namespace BurningKnight.box2dLight {
	public abstract class PositionalLight : Light {
		protected const Vector2 TmpEnd = new Vector2();
		protected const Vector2 Start = new Vector2();
		protected Body Body;
		protected float BodyOffsetX;
		protected float BodyOffsetY;
		protected float BodyAngleOffset;
		protected float[] Sin;
		protected float[] Cos;
		protected float[] EndX;
		protected float[] EndY;

		public PositionalLight(RayHandler RayHandler, int Rays, Color Color, float Distance, float X, float Y, float DirectionDegree) {
			base(RayHandler, Rays, Color, Distance, DirectionDegree);
			Start.X = X;
			Start.Y = Y;
			LightMesh = new Mesh(VertexDataType.VertexArray, false, VertexNum, 0, new VertexAttribute(Usage.Position, 2, "vertex_positions"), new VertexAttribute(Usage.ColorPacked, 4, "quad_colors"), new VertexAttribute(Usage.Generic, 1, "s"));
			SoftShadowMesh = new Mesh(VertexDataType.VertexArray, false, VertexNum * 2, 0, new VertexAttribute(Usage.Position, 2, "vertex_positions"), new VertexAttribute(Usage.ColorPacked, 4, "quad_colors"), new VertexAttribute(Usage.Generic, 1, "s"));
			SetMesh();
		}

		public override Void Update() {
			UpdateBody();

			if (Cull()) return;


			if (StaticLight && !Dirty) return;


			Dirty = false;
			UpdateMesh();
		}

		public override Void Render() {
			if (RayHandler.Culling && Culled) return;


			RayHandler.LightRenderedLastFrame++;
			LightMesh.Render(RayHandler.LightShader, GL20.GL_TRIANGLE_FAN, 0, VertexNum);

			if (Soft && !Xray) {
				SoftShadowMesh.Render(RayHandler.LightShader, GL20.GL_TRIANGLE_STRIP, 0, (VertexNum - 1) * 2);
			} 
		}

		public override Void AttachToBody(Body Body) {
			AttachToBody(Body, 0f, 0f, 0f);
		}

		public Void AttachToBody(Body Body, float OffsetX, float OffsetY) {
			AttachToBody(Body, OffsetX, OffsetY, 0f);
		}

		public Void AttachToBody(Body Body, float OffsetX, float OffSetY, float Degrees) {
			this.Body = Body;
			BodyOffsetX = OffsetX;
			BodyOffsetY = OffSetY;
			BodyAngleOffset = Degrees;

			if (StaticLight) Dirty = true;

		}

		public override Vector2 GetPosition() {
			TmpPosition.X = Start.X;
			TmpPosition.Y = Start.Y;

			return TmpPosition;
		}

		public Body GetBody() {
			return Body;
		}

		public override float GetX() {
			return Start.X;
		}

		public override float GetY() {
			return Start.Y;
		}

		public override Void SetPosition(float X, float Y) {
			Start.X = X;
			Start.Y = Y;

			if (StaticLight) Dirty = true;

		}

		public override Void SetPosition(Vector2 Position) {
			Start.X = Position.X;
			Start.Y = Position.Y;

			if (StaticLight) Dirty = true;

		}

		public override bool Contains(float X, float Y) {
			float X_d = Start.X - X;
			float Y_d = Start.Y - Y;
			float Dst2 = X_d * X_d + Y_d * Y_d;

			if (Distance * Distance <= Dst2) return false;


			bool OddNodes = false;
			float X2 = Mx[RayNum] = Start.X;
			float Y2 = My[RayNum] = Start.Y;
			float X1 = 0;
			float Y1 = 0;

			for (int I = 0; I <= RayNum; ++I) {
				X2 = X1;
				Y2 = Y1;
				X1 = Mx[I];
				Y1 = My[I];

				if (((Y1 < Y) && (Y2 >= Y)) || (Y1 >= Y) && (Y2 < Y)) {
					if ((Y - Y1) / (Y2 - Y1) * (X2 - X1) < (X - X1)) OddNodes = !OddNodes;

				} 
			}

			return OddNodes;
		}

		protected override Void SetRayNum(int Rays) {
			base.SetRayNum(Rays);
			Sin = new float[Rays];
			Cos = new float[Rays];
			EndX = new float[Rays];
			EndY = new float[Rays];
		}

		protected bool Cull() {
			Culled = RayHandler.Culling && !RayHandler.Intersect(Start.X, Start.Y, Distance + SoftShadowLength);

			return Culled;
		}

		protected Void UpdateBody() {
			if (Body == null || StaticLight) return;


			Vector2 Vec = Body.GetPosition();
			float Angle = Body.GetAngle();
			float Cos = MathUtils.Cos(Angle);
			float Sin = MathUtils.Sin(Angle);
			float DX = BodyOffsetX * Cos - BodyOffsetY * Sin;
			float DY = BodyOffsetX * Sin + BodyOffsetY * Cos;
			Start.X = Vec.X + DX;
			Start.Y = Vec.Y + DY;
			SetDirection(BodyAngleOffset + Angle * MathUtils.RadiansToDegrees);
		}

		protected Void UpdateMesh() {
			for (int I = 0; I < RayNum; I++) {
				M_index = I;
				F[I] = 1f;
				TmpEnd.X = EndX[I] + Start.X;
				Mx[I] = TmpEnd.X;
				TmpEnd.Y = EndY[I] + Start.Y;
				My[I] = TmpEnd.Y;

				if (RayHandler.World != null && !Xray) {
					RayHandler.World.RayCast(Ray, Start, TmpEnd);
				} 
			}

			SetMesh();
		}

		protected Void SetMesh() {
			int Size = 0;
			Segments[Size++] = Start.X;
			Segments[Size++] = Start.Y;
			Segments[Size++] = ColorF;
			Segments[Size++] = 1;

			for (int I = 0; I < RayNum; I++) {
				Segments[Size++] = Mx[I];
				Segments[Size++] = My[I];
				Segments[Size++] = ColorF;
				Segments[Size++] = 1 - F[I];
			}

			LightMesh.SetVertices(Segments, 0, Size);

			if (!Soft || Xray) return;


			Size = 0;

			for (int I = 0; I < RayNum; I++) {
				Segments[Size++] = Mx[I];
				Segments[Size++] = My[I];
				Segments[Size++] = ColorF;
				float S = (1 - F[I]);
				Segments[Size++] = S;
				Segments[Size++] = Mx[I] + S * SoftShadowLength * Cos[I];
				Segments[Size++] = My[I] + S * SoftShadowLength * Sin[I];
				Segments[Size++] = ZeroColorBits;
				Segments[Size++] = 0f;
			}

			SoftShadowMesh.SetVertices(Segments, 0, Size);
		}
	}
}
