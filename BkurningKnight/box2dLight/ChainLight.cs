namespace BurningKnight.box2dLight {
	public class ChainLight : Light {
		public static float DefaultRayStartOffset = 0.001f;
		public float RayStartOffset;
		public const FloatArray Chain;
		protected int RayDirection;
		protected float BodyAngle;
		protected float BodyAngleOffset;
		protected Body Body;
		protected const FloatArray SegmentAngles = new FloatArray();
		protected const FloatArray SegmentLengths = new FloatArray();
		protected const float[] StartX;
		protected const float[] StartY;
		protected const float[] EndX;
		protected const float[] EndY;
		protected const Vector2 BodyPosition = new Vector2();
		protected const Vector2 TmpEnd = new Vector2();
		protected const Vector2 TmpStart = new Vector2();
		protected const Vector2 TmpPerp = new Vector2();
		protected const Vector2 TmpVec = new Vector2();
		protected const Matrix3 ZeroPosition = new Matrix3();
		protected const Matrix3 RotateAroundZero = new Matrix3();
		protected const Matrix3 RestorePosition = new Matrix3();
		protected const Rectangle ChainLightBounds = new Rectangle();
		protected const Rectangle RayHandlerBounds = new Rectangle();

		public ChainLight(RayHandler RayHandler, int Rays, Color Color, float Distance, int RayDirection) {
			this(RayHandler, Rays, Color, Distance, RayDirection, null);
		}

		public ChainLight(RayHandler RayHandler, int Rays, Color Color, float Distance, int RayDirection, float Chain) {
			base(RayHandler, Rays, Color, Distance, 0f);
			RayStartOffset = ChainLight.DefaultRayStartOffset;
			this.RayDirection = RayDirection;
			VertexNum = (VertexNum - 1) * 2;
			EndX = new float[Rays];
			EndY = new float[Rays];
			StartX = new float[Rays];
			StartY = new float[Rays];
			this.Chain = (Chain != null) ? new FloatArray(Chain) : new FloatArray();
			LightMesh = new Mesh(VertexDataType.VertexArray, false, VertexNum, 0, new VertexAttribute(Usage.Position, 2, "vertex_positions"), new VertexAttribute(Usage.ColorPacked, 4, "quad_colors"), new VertexAttribute(Usage.Generic, 1, "s"));
			SoftShadowMesh = new Mesh(VertexDataType.VertexArray, false, VertexNum * 2, 0, new VertexAttribute(Usage.Position, 2, "vertex_positions"), new VertexAttribute(Usage.ColorPacked, 4, "quad_colors"), new VertexAttribute(Usage.Generic, 1, "s"));
			SetMesh();
		}

		public override Void Update() {
			if (Dirty) {
				UpdateChain();
				ApplyAttachment();
			} else {
				UpdateBody();
			}


			if (Cull()) return;


			if (StaticLight && !Dirty) return;


			Dirty = false;
			UpdateMesh();
		}

		public override Void Render() {
			if (RayHandler.Culling && Culled) return;


			RayHandler.LightRenderedLastFrame++;
			LightMesh.Render(RayHandler.LightShader, GL20.GL_TRIANGLE_STRIP, 0, VertexNum);

			if (Soft && !Xray) {
				SoftShadowMesh.Render(RayHandler.LightShader, GL20.GL_TRIANGLE_STRIP, 0, VertexNum);
			} 
		}

		public Void DebugRender(ShapeRenderer ShapeRenderer) {
			ShapeRenderer.SetColor(Color.YELLOW);
			FloatArray Vertices = Pools.Obtain(FloatArray.GetType());
			Vertices.Clear();

			for (int I = 0; I < RayNum; I++) {
				Vertices.AddAll(Mx[I], My[I]);
			}

			for (int I = RayNum - 1; I > -1; I--) {
				Vertices.AddAll(StartX[I], StartY[I]);
			}

			ShapeRenderer.Polygon(Vertices.Shrink());
			Pools.Free(Vertices);
		}

		public override Void AttachToBody(Body Body) {
			AttachToBody(Body, 0f);
		}

		public Void AttachToBody(Body Body, float Degrees) {
			this.Body = Body;
			this.BodyPosition.Set(Body.GetPosition());
			BodyAngleOffset = MathUtils.DegreesToRadians * Degrees;
			BodyAngle = Body.GetAngle();
			ApplyAttachment();

			if (StaticLight) Dirty = true;

		}

		public override Body GetBody() {
			return Body;
		}

		public override float GetX() {
			return TmpPosition.X;
		}

		public override float GetY() {
			return TmpPosition.Y;
		}

		public override Void SetPosition(float X, float Y) {
			TmpPosition.X = X;
			TmpPosition.Y = Y;

			if (StaticLight) Dirty = true;

		}

		public override Void SetPosition(Vector2 Position) {
			TmpPosition.X = Position.X;
			TmpPosition.Y = Position.Y;

			if (StaticLight) Dirty = true;

		}

		public override bool Contains(float X, float Y) {
			if (!this.ChainLightBounds.Contains(X, Y)) return false;


			FloatArray Vertices = Pools.Obtain(FloatArray.GetType());
			Vertices.Clear();

			for (int I = 0; I < RayNum; I++) {
				Vertices.AddAll(Mx[I], My[I]);
			}

			for (int I = RayNum - 1; I > -1; I--) {
				Vertices.AddAll(StartX[I], StartY[I]);
			}

			int Intersects = 0;

			for (int I = 0; I < Vertices.Size; I += 2) {
				float X1 = Vertices.Items[I];
				float Y1 = Vertices.Items[I + 1];
				float X2 = Vertices.Items[(I + 2) % Vertices.Size];
				float Y2 = Vertices.Items[(I + 3) % Vertices.Size];

				if (((Y1 <= Y && Y < Y2) || (Y2 <= Y && Y < Y1)) && X < ((X2 - X1) / (Y2 - Y1) * (Y - Y1) + X1)) Intersects++;

			}

			bool Result = (Intersects & 1) == 1;
			Pools.Free(Vertices);

			return Result;
		}

		public override Void SetDistance(float Dist) {
			Dist *= RayHandler.GammaCorrectionParameter;
			this.Distance = Dist < 0.01f ? 0.01f : Dist;
			Dirty = true;
		}

		public override Void SetDirection(float DirectionDegree) {

		}

		public Void UpdateChain() {
			Vector2 V1 = Pools.Obtain(Vector2.GetType());
			Vector2 V2 = Pools.Obtain(Vector2.GetType());
			Vector2 VSegmentStart = Pools.Obtain(Vector2.GetType());
			Vector2 VDirection = Pools.Obtain(Vector2.GetType());
			Vector2 VRayOffset = Pools.Obtain(Vector2.GetType());
			Spinor TmpAngle = Pools.Obtain(Spinor.GetType());
			Spinor PreviousAngle = Pools.Obtain(Spinor.GetType());
			Spinor CurrentAngle = Pools.Obtain(Spinor.GetType());
			Spinor NextAngle = Pools.Obtain(Spinor.GetType());
			Spinor StartAngle = Pools.Obtain(Spinor.GetType());
			Spinor EndAngle = Pools.Obtain(Spinor.GetType());
			Spinor RayAngle = Pools.Obtain(Spinor.GetType());
			int SegmentCount = Chain.Size / 2 - 1;
			SegmentAngles.Clear();
			SegmentLengths.Clear();
			float RemainingLength = 0;

			{
				int J = 0;

				for (int I = 0; I < Chain.Size - 2; I += 2) {
					V1.Set(Chain.Items[I + 2], Chain.Items[I + 3]).Sub(Chain.Items[I], Chain.Items[I + 1]);
					SegmentLengths.Add(V1.Len());
					SegmentAngles.Add(V1.Rotate90(RayDirection).Angle() * MathUtils.DegreesToRadians);
					RemainingLength += SegmentLengths.Items[J];
					J++;
				}
			}

			int RayNumber = 0;
			int RemainingRays = RayNum;

			for (int I = 0; I < SegmentCount; I++) {
				PreviousAngle.Set((I == 0) ? SegmentAngles.Items[I] : SegmentAngles.Items[I - 1]);
				CurrentAngle.Set(SegmentAngles.Items[I]);
				NextAngle.Set((I == SegmentAngles.Size - 1) ? SegmentAngles.Items[I] : SegmentAngles.Items[I + 1]);
				StartAngle.Set(PreviousAngle).Slerp(CurrentAngle, 0.5f);
				EndAngle.Set(CurrentAngle).Slerp(NextAngle, 0.5f);
				int SegmentVertex = I * 2;
				VSegmentStart.Set(Chain.Items[SegmentVertex], Chain.Items[SegmentVertex + 1]);
				VDirection.Set(Chain.Items[SegmentVertex + 2], Chain.Items[SegmentVertex + 3]).Sub(VSegmentStart).Nor();
				float RaySpacing = RemainingLength / RemainingRays;
				int SegmentRays = (I == SegmentCount - 1) ? RemainingRays : (int) ((SegmentLengths.Items[I] / RemainingLength) * RemainingRays);

				for (int J = 0; J < SegmentRays; J++) {
					float Position = J * RaySpacing;
					RayAngle.Set(StartAngle).Slerp(EndAngle, Position / SegmentLengths.Items[I]);
					float Angle = RayAngle.Angle();
					VRayOffset.Set(this.RayStartOffset, 0).RotateRad(Angle);
					V1.Set(VDirection).Scl(Position).Add(VSegmentStart).Add(VRayOffset);
					this.StartX[RayNumber] = V1.X;
					this.StartY[RayNumber] = V1.Y;
					V2.Set(Distance, 0).RotateRad(Angle).Add(V1);
					this.EndX[RayNumber] = V2.X;
					this.EndY[RayNumber] = V2.Y;
					RayNumber++;
				}

				RemainingRays -= SegmentRays;
				RemainingLength -= SegmentLengths.Items[I];
			}

			Pools.Free(V1);
			Pools.Free(V2);
			Pools.Free(VSegmentStart);
			Pools.Free(VDirection);
			Pools.Free(VRayOffset);
			Pools.Free(PreviousAngle);
			Pools.Free(CurrentAngle);
			Pools.Free(NextAngle);
			Pools.Free(StartAngle);
			Pools.Free(EndAngle);
			Pools.Free(RayAngle);
			Pools.Free(TmpAngle);
		}

		public Void ApplyAttachment() {
			if (Body == null || StaticLight) return;


			RestorePosition.SetToTranslation(BodyPosition);
			RotateAroundZero.SetToRotationRad(BodyAngle + BodyAngleOffset);

			for (int I = 0; I < RayNum; I++) {
				TmpVec.Set(StartX[I], StartY[I]).Mul(RotateAroundZero).Mul(RestorePosition);
				StartX[I] = TmpVec.X;
				StartY[I] = TmpVec.Y;
				TmpVec.Set(EndX[I], EndY[I]).Mul(RotateAroundZero).Mul(RestorePosition);
				EndX[I] = TmpVec.X;
				EndY[I] = TmpVec.Y;
			}
		}

		protected bool Cull() {
			if (!RayHandler.Culling) {
				Culled = false;
			} else {
				UpdateBoundingRects();
				Culled = ChainLightBounds.Width > 0 && ChainLightBounds.Height > 0 && !ChainLightBounds.Overlaps(RayHandlerBounds);
			}


			return Culled;
		}

		public Void UpdateBody() {
			if (Body == null || StaticLight) return;


			Vector2 Vec = Body.GetPosition();
			TmpVec.Set(0, 0).Sub(BodyPosition);
			BodyPosition.Set(Vec);
			ZeroPosition.SetToTranslation(TmpVec);
			RestorePosition.SetToTranslation(BodyPosition);
			RotateAroundZero.SetToRotationRad(BodyAngle).Inv().RotateRad(Body.GetAngle());
			BodyAngle = Body.GetAngle();

			for (int I = 0; I < RayNum; I++) {
				TmpVec.Set(StartX[I], StartY[I]).Mul(ZeroPosition).Mul(RotateAroundZero).Mul(RestorePosition);
				StartX[I] = TmpVec.X;
				StartY[I] = TmpVec.Y;
				TmpVec.Set(EndX[I], EndY[I]).Mul(ZeroPosition).Mul(RotateAroundZero).Mul(RestorePosition);
				EndX[I] = TmpVec.X;
				EndY[I] = TmpVec.Y;
			}
		}

		protected Void UpdateMesh() {
			for (int I = 0; I < RayNum; I++) {
				M_index = I;
				F[I] = 1f;
				TmpEnd.X = EndX[I];
				Mx[I] = TmpEnd.X;
				TmpEnd.Y = EndY[I];
				My[I] = TmpEnd.Y;
				TmpStart.X = StartX[I];
				TmpStart.Y = StartY[I];

				if (RayHandler.World != null && !Xray) {
					RayHandler.World.RayCast(Ray, TmpStart, TmpEnd);
				} 
			}

			SetMesh();
		}

		protected Void SetMesh() {
			int Size = 0;

			for (int I = 0; I < RayNum; I++) {
				Segments[Size++] = StartX[I];
				Segments[Size++] = StartY[I];
				Segments[Size++] = ColorF;
				Segments[Size++] = 1;
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
				TmpPerp.Set(Mx[I], My[I]).Sub(StartX[I], StartY[I]).Nor().Scl(SoftShadowLength * S).Add(Mx[I], My[I]);
				Segments[Size++] = TmpPerp.X;
				Segments[Size++] = TmpPerp.Y;
				Segments[Size++] = ZeroColorBits;
				Segments[Size++] = 0f;
			}

			SoftShadowMesh.SetVertices(Segments, 0, Size);
		}

		protected Void UpdateBoundingRects() {
			float MaxX = StartX[0];
			float MinX = StartX[0];
			float MaxY = StartY[0];
			float MinY = StartY[0];

			for (int I = 0; I < RayNum; I++) {
				MaxX = MaxX > StartX[I] ? MaxX : StartX[I];
				MaxX = MaxX > Mx[I] ? MaxX : Mx[I];
				MinX = MinX < StartX[I] ? MinX : StartX[I];
				MinX = MinX < Mx[I] ? MinX : Mx[I];
				MaxY = MaxY > StartY[I] ? MaxY : StartY[I];
				MaxY = MaxY > My[I] ? MaxY : My[I];
				MinY = MinY < StartY[I] ? MinY : StartY[I];
				MinY = MinY < My[I] ? MinY : My[I];
			}

			ChainLightBounds.Set(MinX, MinY, MaxX - MinX, MaxY - MinY);
			RayHandlerBounds.Set(RayHandler.X1, RayHandler.Y1, RayHandler.X2 - RayHandler.X1, RayHandler.Y2 - RayHandler.Y1);
		}
	}
}
