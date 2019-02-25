namespace BurningKnight.box2dLight {
	public abstract class Light : Disposable {
		public const Color DefaultColor = new Color(0.75f, 0.75f, 0.5f, 0.75f);
		public const float ZeroColorBits = Color.ToFloatBits(0f, 0f, 0f, 0f);
		public const int MIN_RAYS = 3;
		protected const Color Color = new Color();
		protected const Vector2 TmpPosition = new Vector2();
		protected RayHandler RayHandler;
		protected bool Active = true;
		protected bool Soft = true;
		protected bool Xray = false;
		protected bool StaticLight = false;
		protected bool Culled = false;
		protected bool Dirty = true;
		protected bool IgnoreBody = false;
		protected int RayNum;
		protected int VertexNum;
		protected float Distance;
		protected float Direction;
		protected float ColorF;
		protected float SoftShadowLength = 2.5f;
		protected Mesh LightMesh;
		protected Mesh SoftShadowMesh;
		protected float[] Segments;
		protected float[] Mx;
		protected float[] My;
		protected float[] F;
		protected int M_index = 0;
		private static Filter GlobalFilterA = null;
		private Filter FilterA = null;
		public const RayCastCallback Ray = new RayCastCallback() {
			public const override float ReportRayFixture(Fixture Fixture, Vector2 Point, Vector2 Normal, float Fraction) {
				if ((GlobalFilterA != null) && !GlobalContactFilter(Fixture)) return -1;


				if ((FilterA != null) && !ContactFilter(Fixture)) return -1;


				if (IgnoreBody && Fixture.GetBody() == GetBody()) return -1;


				Mx[M_index] = Point.X;
				My[M_index] = Point.Y;
				F[M_index] = Fraction;

				return Fraction;
			}
		};

		public Light(RayHandler RayHandler, int Rays, Color Color, float Distance, float DirectionDegree) {
			RayHandler.LightList.Add(this);
			this.RayHandler = RayHandler;
			SetRayNum(Rays);
			SetColor(Color);
			SetDistance(Distance);
			SetSoftnessLength(Distance * 0.1f);
			SetDirection(DirectionDegree);
		}

		public abstract Void Update();

		public abstract Void Render();

		public abstract Void SetDistance(float Dist);

		public abstract Void SetDirection(float DirectionDegree);

		public abstract Void AttachToBody(Body Body);

		public abstract Body GetBody();

		public abstract Void SetPosition(float X, float Y);

		public abstract Void SetPosition(Vector2 Position);

		public abstract float GetX();

		public abstract float GetY();

		public Vector2 GetPosition() {
			return TmpPosition;
		}

		public Void SetColor(Color NewColor) {
			if (NewColor != null) {
				Color.Set(NewColor);
			} else {
				Color.Set(DefaultColor);
			}


			ColorF = Color.ToFloatBits();

			if (StaticLight) Dirty = true;

		}

		public Void SetColor(float R, float G, float B, float A) {
			Color.Set(R, G, B, A);
			ColorF = Color.ToFloatBits();

			if (StaticLight) Dirty = true;

		}

		public Void Add(RayHandler RayHandler) {
			this.RayHandler = RayHandler;

			if (Active) {
				RayHandler.LightList.Add(this);
			} else {
				RayHandler.DisabledLights.Add(this);
			}

		}

		public Void Remove() {
			Remove(true);
		}

		public Void Remove(bool DoDispose) {
			if (Active) {
				RayHandler.LightList.RemoveValue(this, false);
			} else {
				RayHandler.DisabledLights.RemoveValue(this, false);
			}


			RayHandler = null;

			if (DoDispose) Dispose();

		}

		public Void Dispose() {
			LightMesh.Dispose();
			SoftShadowMesh.Dispose();
		}

		public bool IsActive() {
			return Active;
		}

		public Void SetActive(bool Active) {
			if (Active == this.Active) return;


			this.Active = Active;

			if (RayHandler == null) return;


			if (Active) {
				RayHandler.LightList.Add(this);
				RayHandler.DisabledLights.RemoveValue(this, true);
			} else {
				RayHandler.DisabledLights.Add(this);
				RayHandler.LightList.RemoveValue(this, true);
			}

		}

		public bool IsXray() {
			return Xray;
		}

		public Void SetXray(bool Xray) {
			this.Xray = Xray;

			if (StaticLight) Dirty = true;

		}

		public bool IsStaticLight() {
			return StaticLight;
		}

		public Void SetStaticLight(bool StaticLight) {
			this.StaticLight = StaticLight;

			if (StaticLight) Dirty = true;

		}

		public bool IsSoft() {
			return Soft;
		}

		public Void SetSoft(bool Soft) {
			this.Soft = Soft;

			if (StaticLight) Dirty = true;

		}

		public float GetSoftShadowLength() {
			return SoftShadowLength;
		}

		public Void SetSoftnessLength(float SoftShadowLength) {
			this.SoftShadowLength = SoftShadowLength;

			if (StaticLight) Dirty = true;

		}

		public Color GetColor() {
			return Color;
		}

		public float GetDistance() {
			return Distance / RayHandler.GammaCorrectionParameter;
		}

		public float GetDirection() {
			return Direction;
		}

		public bool Contains(float X, float Y) {
			return false;
		}

		public Void SetIgnoreAttachedBody(bool Flag) {
			IgnoreBody = Flag;
		}

		public bool GetIgnoreAttachedBody() {
			return IgnoreBody;
		}

		public Void SetRayNum(int Rays) {
			if (Rays < MIN_RAYS) Rays = MIN_RAYS;


			RayNum = Rays;
			VertexNum = Rays + 1;
			Segments = new float[VertexNum * 8];
			Mx = new float[VertexNum];
			My = new float[VertexNum];
			F = new float[VertexNum];
		}

		public int GetRayNum() {
			return RayNum;
		}

		public bool ContactFilter(Fixture FixtureB) {
			Filter FilterB = FixtureB.GetFilterData();

			if (FilterA.GroupIndex != 0 && FilterA.GroupIndex == FilterB.GroupIndex) return FilterA.GroupIndex > 0;


			return (FilterA.MaskBits & FilterB.CategoryBits) != 0 && (FilterA.CategoryBits & FilterB.MaskBits) != 0;
		}

		public Void SetContactFilter(Filter Filter) {
			FilterA = Filter;
		}

		public Void SetContactFilter(short CategoryBits, short GroupIndex, short MaskBits) {
			FilterA = new Filter();
			FilterA.CategoryBits = CategoryBits;
			FilterA.GroupIndex = GroupIndex;
			FilterA.MaskBits = MaskBits;
		}

		public bool GlobalContactFilter(Fixture FixtureB) {
			Filter FilterB = FixtureB.GetFilterData();

			if (GlobalFilterA.GroupIndex != 0 && GlobalFilterA.GroupIndex == FilterB.GroupIndex) return GlobalFilterA.GroupIndex > 0;


			return (GlobalFilterA.MaskBits & FilterB.CategoryBits) != 0 && (GlobalFilterA.CategoryBits & FilterB.MaskBits) != 0;
		}

		public static Void SetGlobalContactFilter(Filter Filter) {
			GlobalFilterA = Filter;
		}

		public static Void SetGlobalContactFilter(short CategoryBits, short GroupIndex, short MaskBits) {
			GlobalFilterA = new Filter();
			GlobalFilterA.CategoryBits = CategoryBits;
			GlobalFilterA.GroupIndex = GroupIndex;
			GlobalFilterA.MaskBits = MaskBits;
		}
	}
}
