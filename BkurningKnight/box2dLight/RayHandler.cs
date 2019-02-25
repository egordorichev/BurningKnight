namespace BurningKnight.box2dLight {
	public class RayHandler : Disposable {
		public const float GAMMA_COR = 0.625f;
		public static bool GammaCorrection = false;
		public static float GammaCorrectionParameter = 1f;
		public static bool IsDiffuse = false;
		public const BlendFunc DiffuseBlendFunc = new BlendFunc(GL20.GL_DST_COLOR, GL20.GL_ZERO);
		public const BlendFunc ShadowBlendFunc = new BlendFunc(GL20.GL_ONE, GL20.GL_ONE_MINUS_SRC_ALPHA);
		public const BlendFunc SimpleBlendFunc = new BlendFunc(GL20.GL_SRC_ALPHA, GL20.GL_ONE);
		public const Matrix4 Combined = new Matrix4();
		public const Color AmbientLight = new Color();
		public const Array<Light> LightList = new Array<Light>(false, 16);
		public const Array<Light> DisabledLights = new Array<Light>(false, 16);
		public LightMap LightMap;
		public const ShaderProgram LightShader;
		public ShaderProgram CustomLightShader = null;
		public bool Culling = true;
		public bool Shadows = true;
		public bool Blur = true;
		public int BlurNum = 1;
		public bool CustomViewport = false;
		public int ViewportX = 0;
		public int ViewportY = 0;
		public int ViewportWidth = Gdx.Graphics.GetWidth();
		public int ViewportHeight = Gdx.Graphics.GetHeight();
		public int LightRenderedLastFrame = 0;
		public float X1;
		public float X2;
		public float Y1;
		public float Y2;
		public World World;

		public RayHandler(World World) {
			this(World, Gdx.Graphics.GetWidth() / 4, Gdx.Graphics.GetHeight() / 4);
		}

		public RayHandler(World World, int FboWidth, int FboHeight) {
			this.World = World;
			ResizeFBO(FboWidth, FboHeight);
			LightShader = LightShader.CreateLightShader();
		}

		public Void ResizeFBO(int FboWidth, int FboHeight) {
			if (LightMap != null) {
				LightMap.Dispose();
			} 

			LightMap = new LightMap(this, FboWidth, FboHeight);
		}

		public Void SetCombinedMatrix(OrthographicCamera Camera) {
			this.SetCombinedMatrix(Camera.Combined, Camera.Position.X, Camera.Position.Y, Camera.ViewportWidth * Camera.Zoom, Camera.ViewportHeight * Camera.Zoom);
		}

		public Void SetCombinedMatrix(Matrix4 Combined) {
			System.Arraycopy(Combined.Val, 0, this.Combined.Val, 0, 16);
			float InvWidth = Combined.Val[Matrix4.M00];
			float HalfViewPortWidth = 1f / InvWidth;
			float X = -HalfViewPortWidth * Combined.Val[Matrix4.M03];
			X1 = X - HalfViewPortWidth;
			X2 = X + HalfViewPortWidth;
			float InvHeight = Combined.Val[Matrix4.M11];
			float HalfViewPortHeight = 1f / InvHeight;
			float Y = -HalfViewPortHeight * Combined.Val[Matrix4.M13];
			Y1 = Y - HalfViewPortHeight;
			Y2 = Y + HalfViewPortHeight;
		}

		public Void SetCombinedMatrix(Matrix4 Combined, float X, float Y, float ViewPortWidth, float ViewPortHeight) {
			System.Arraycopy(Combined.Val, 0, this.Combined.Val, 0, 16);
			float HalfViewPortWidth = ViewPortWidth * 0.5f;
			X1 = X - HalfViewPortWidth;
			X2 = X + HalfViewPortWidth;
			float HalfViewPortHeight = ViewPortHeight * 0.5f;
			Y1 = Y - HalfViewPortHeight;
			Y2 = Y + HalfViewPortHeight;
		}

		public bool Intersect(float X, float Y, float Radius) {
			return (X1 < (X + Radius) && X2 > (X - Radius) && Y1 < (Y + Radius) && Y2 > (Y - Radius));
		}

		public Void UpdateAndRender() {
			Update();
			Render();
		}

		public Void Update() {
			foreach (Light Light in LightList) {
				Light.Update();
			}
		}

		public Void PrepareRender() {
			LightRenderedLastFrame = 0;
			Gdx.Gl.GlDepthMask(false);
			Gdx.Gl.GlEnable(GL20.GL_BLEND);
			SimpleBlendFunc.Apply();
			bool UseLightMap = (Shadows || Blur);

			if (UseLightMap) {
				LightMap.FrameBuffer.Begin();
				Gdx.Gl.GlClearColor(AmbientLight.R, AmbientLight.G, AmbientLight.B, AmbientLight.A);
				Gdx.Gl.GlClear(GL20.GL_COLOR_BUFFER_BIT);
			} 

			ShaderProgram Shader = CustomLightShader != null ? CustomLightShader : LightShader;
			Shader.Begin();

			{
				Shader.SetUniformMatrix("u_projTrans", Combined);

				if (CustomLightShader != null) UpdateLightShader();


				foreach (Light Light in LightList) {
					if (CustomLightShader != null) UpdateLightShaderPerLight(Light);


					Light.Render();
				}
			}

			Shader.End();

			if (UseLightMap) {
				if (CustomViewport) {
					LightMap.FrameBuffer.End(ViewportX, ViewportY, ViewportWidth, ViewportHeight);
				} else {
					LightMap.FrameBuffer.End();
				}


				bool Needed = LightRenderedLastFrame > 0;

				if (Needed && Blur) LightMap.GaussianBlur();

			} 
		}

		public Void Render() {
			PrepareRender();
			LightMap.Render();
		}

		public Void RenderOnly() {
			LightMap.Render();
		}

		protected Void UpdateLightShader() {

		}

		protected Void UpdateLightShaderPerLight(Light Light) {

		}

		public bool PointAtLight(float X, float Y) {
			foreach (Light Light in LightList) {
				if (Light.Contains(X, Y)) return true;

			}

			return false;
		}

		public bool PointAtShadow(float X, float Y) {
			foreach (Light Light in LightList) {
				if (Light.Contains(X, Y)) return false;

			}

			return true;
		}

		public Void Dispose() {
			RemoveAll();

			if (LightMap != null) LightMap.Dispose();


			if (LightShader != null) LightShader.Dispose();

		}

		public Void RemoveAll() {
			foreach (Light Light in LightList) {
				Light.Dispose();
			}

			LightList.Clear();

			foreach (Light Light in DisabledLights) {
				Light.Dispose();
			}

			DisabledLights.Clear();
		}

		public Void SetLightShader(ShaderProgram CustomLightShader) {
			this.CustomLightShader = CustomLightShader;
		}

		public Void SetCulling(bool Culling) {
			this.Culling = Culling;
		}

		public Void SetBlur(bool Blur) {
			this.Blur = Blur;
		}

		public Void SetBlurNum(int BlurNum) {
			this.BlurNum = BlurNum;
		}

		public Void SetShadows(bool Shadows) {
			this.Shadows = Shadows;
		}

		public Void SetAmbientLight(float AmbientLight) {
			this.AmbientLight.A = MathUtils.Clamp(AmbientLight, 0f, 1f);
		}

		public Void SetAmbientLight(float R, float G, float B, float A) {
			this.AmbientLight.Set(R, G, B, A);
		}

		public Void SetAmbientLight(Color AmbientLightColor) {
			this.AmbientLight.Set(AmbientLightColor);
		}

		public Void SetWorld(World World) {
			this.World = World;
		}

		public static bool GetGammaCorrection() {
			return GammaCorrection;
		}

		public static Void SetGammaCorrection(bool GammaCorrectionWanted) {
			GammaCorrection = GammaCorrectionWanted;
			GammaCorrectionParameter = GammaCorrection ? GAMMA_COR : 1f;
		}

		public static Void UseDiffuseLight(bool UseDiffuse) {
			IsDiffuse = UseDiffuse;
		}

		public Void UseCustomViewport(int X, int Y, int Width, int Height) {
			CustomViewport = true;
			ViewportX = X;
			ViewportY = Y;
			ViewportWidth = Width;
			ViewportHeight = Height;
		}

		public Void UseDefaultViewport() {
			CustomViewport = false;
		}

		public Void SetLightMapRendering(bool IsAutomatic) {
			LightMap.LightMapDrawingDisabled = !IsAutomatic;
		}

		public Texture GetLightMapTexture() {
			return LightMap.FrameBuffer.GetColorBufferTexture();
		}

		public FrameBuffer GetLightMapBuffer() {
			return LightMap.FrameBuffer;
		}
	}
}
