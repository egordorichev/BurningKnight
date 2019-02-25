namespace BurningKnight.box2dLight {
	public class DirectionalLight : Light {
		protected const Vector2[] Start;
		protected const Vector2[] End;
		protected float Sin;
		protected float Cos;
		protected Body Body;

		public DirectionalLight(RayHandler RayHandler, int Rays, Color Color, float DirectionDegree) {
			base(RayHandler, Rays, Color, Float.POSITIVE_INFINITY, DirectionDegree);
			VertexNum = (VertexNum - 1) * 2;
			Start = new Vector2[RayNum];
			End = new Vector2[RayNum];

			for (int I = 0; I < RayNum; I++) {
				Start[I] = new Vector2();
				End[I] = new Vector2();
			}

			LightMesh = new Mesh(VertexDataType.VertexArray, StaticLight, VertexNum, 0, new VertexAttribute(Usage.Position, 2, "vertex_positions"), new VertexAttribute(Usage.ColorPacked, 4, "quad_colors"), new VertexAttribute(Usage.Generic, 1, "s"));
			SoftShadowMesh = new Mesh(VertexDataType.VertexArray, StaticLight, VertexNum, 0, new VertexAttribute(Usage.Position, 2, "vertex_positions"), new VertexAttribute(Usage.ColorPacked, 4, "quad_colors"), new VertexAttribute(Usage.Generic, 1, "s"));
			Update();
		}

		public override Void SetDirection(float Direction) {
			this.Direction = Direction;
			Sin = MathUtils.SinDeg(Direction);
			Cos = MathUtils.CosDeg(Direction);

			if (StaticLight) Dirty = true;

		}

		public override Void Update() {
			if (StaticLight && !Dirty) return;


			Dirty = false;
			float Width = (RayHandler.X2 - RayHandler.X1);
			float Height = (RayHandler.Y2 - RayHandler.Y1);
			float SizeOfScreen = Width > Height ? Width : Height;
			float XAxelOffSet = SizeOfScreen * Cos;
			float YAxelOffSet = SizeOfScreen * Sin;

			if ((XAxelOffSet * XAxelOffSet < 0.1f) && (YAxelOffSet * YAxelOffSet < 0.1f)) {
				XAxelOffSet = 1;
				YAxelOffSet = 1;
			} 

			float WidthOffSet = SizeOfScreen * -Sin;
			float HeightOffSet = SizeOfScreen * Cos;
			float X = (RayHandler.X1 + RayHandler.X2) * 0.5f - WidthOffSet;
			float Y = (RayHandler.Y1 + RayHandler.Y2) * 0.5f - HeightOffSet;
			float PortionX = 2f * WidthOffSet / (RayNum - 1);
			X = (MathUtils.Floor(X / (PortionX * 2))) * PortionX * 2;
			float PortionY = 2f * HeightOffSet / (RayNum - 1);
			Y = (MathUtils.Ceil(Y / (PortionY * 2))) * PortionY * 2;

			for (int I = 0; I < RayNum; I++) {
				float SteppedX = I * PortionX + X;
				float SteppedY = I * PortionY + Y;
				M_index = I;
				Start[I].X = SteppedX - XAxelOffSet;
				Start[I].Y = SteppedY - YAxelOffSet;
				Mx[I] = End[I].X = SteppedX + XAxelOffSet;
				My[I] = End[I].Y = SteppedY + YAxelOffSet;

				if (RayHandler.World != null && !Xray) {
					RayHandler.World.RayCast(Ray, Start[I], End[I]);
				} 
			}

			int Size = 0;
			int ArraySize = RayNum;

			for (int I = 0; I < ArraySize; I++) {
				Segments[Size++] = Start[I].X;
				Segments[Size++] = Start[I].Y;
				Segments[Size++] = ColorF;
				Segments[Size++] = 1f;
				Segments[Size++] = Mx[I];
				Segments[Size++] = My[I];
				Segments[Size++] = ColorF;
				Segments[Size++] = 1f;
			}

			LightMesh.SetVertices(Segments, 0, Size);

			if (!Soft || Xray) return;


			Size = 0;

			for (int I = 0; I < ArraySize; I++) {
				Segments[Size++] = Mx[I];
				Segments[Size++] = My[I];
				Segments[Size++] = ColorF;
				Segments[Size++] = 1f;
				Segments[Size++] = Mx[I] + SoftShadowLength * Cos;
				Segments[Size++] = My[I] + SoftShadowLength * Sin;
				Segments[Size++] = ZeroColorBits;
				Segments[Size++] = 1f;
			}

			SoftShadowMesh.SetVertices(Segments, 0, Size);
		}

		public override Void Render() {
			RayHandler.LightRenderedLastFrame++;
			LightMesh.Render(RayHandler.LightShader, GL20.GL_TRIANGLE_STRIP, 0, VertexNum);

			if (Soft && !Xray) {
				SoftShadowMesh.Render(RayHandler.LightShader, GL20.GL_TRIANGLE_STRIP, 0, VertexNum);
			} 
		}

		public override bool Contains(float X, float Y) {
			bool OddNodes = false;
			float X2 = Mx[RayNum] = Start[0].X;
			float Y2 = My[RayNum] = Start[0].Y;
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

			for (int I = 0; I < RayNum; ++I) {
				X2 = X1;
				Y2 = Y1;
				X1 = Start[I].X;
				Y1 = Start[I].Y;

				if (((Y1 < Y) && (Y2 >= Y)) || (Y1 >= Y) && (Y2 < Y)) {
					if ((Y - Y1) / (Y2 - Y1) * (X2 - X1) < (X - X1)) OddNodes = !OddNodes;

				} 
			}

			return OddNodes;
		}

		public override Void AttachToBody(Body Body) {

		}

		public override Void SetPosition(float X, float Y) {

		}

		public override Body GetBody() {
			return Body;
		}

		public override float GetX() {
			return 0;
		}

		public override float GetY() {
			return 0;
		}

		public override Void SetPosition(Vector2 Position) {

		}

		public override Void SetDistance(float Dist) {

		}

		public override Void SetIgnoreAttachedBody(bool Flag) {

		}

		public override bool GetIgnoreAttachedBody() {
			return false;
		}

		public Void SetIgnoreBody(Body Body) {
			this.Body = Body;
			IgnoreBody = (Body != null);
		}
	}
}
