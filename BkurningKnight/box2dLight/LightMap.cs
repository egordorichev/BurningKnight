using BurningKnight.shaders;

namespace BurningKnight.box2dLight {
	public class LightMap {
		private ShaderProgram ShadowShader;
		public FrameBuffer FrameBuffer;
		private Mesh LightMapMesh;
		private FrameBuffer PingPongBuffer;
		private RayHandler RayHandler;
		private ShaderProgram WithoutShadowShader;
		private ShaderProgram BlurShader;
		private ShaderProgram DiffuseShader;
		public bool LightMapDrawingDisabled;
		public const int VERT_SIZE = 16;
		public const int X1 = 0;
		public const int Y1 = 1;
		public const int U1 = 2;
		public const int V1 = 3;
		public const int X2 = 4;
		public const int Y2 = 5;
		public const int U2 = 6;
		public const int V2 = 7;
		public const int X3 = 8;
		public const int Y3 = 9;
		public const int U3 = 10;
		public const int V3 = 11;
		public const int X4 = 12;
		public const int Y4 = 13;
		public const int U4 = 14;
		public const int V4 = 15;

		public Void Render() {
			bool Needed = RayHandler.LightRenderedLastFrame > 0;

			if (LightMapDrawingDisabled) return;


			FrameBuffer.GetColorBufferTexture().Bind(0);

			if (RayHandler.Shadows) {
				Color C = RayHandler.AmbientLight;
				ShaderProgram Shader = ShadowShader;

				if (RayHandler.IsDiffuse) {
					Shader = DiffuseShader;
					Shader.Begin();
					RayHandler.DiffuseBlendFunc.Apply();
					Shader.SetUniformf("ambient", C.R, C.G, C.B, C.A);
				} else {
					Shader.Begin();
					RayHandler.ShadowBlendFunc.Apply();
					Shader.SetUniformf("ambient", C.R * C.A, C.G * C.A, C.B * C.A, 1f - C.A);
				}


				LightMapMesh.Render(Shader, GL20.GL_TRIANGLE_FAN);
				Shader.End();
			} else if (Needed) {
				RayHandler.SimpleBlendFunc.Apply();
				WithoutShadowShader.Begin();
				LightMapMesh.Render(WithoutShadowShader, GL20.GL_TRIANGLE_FAN);
				WithoutShadowShader.End();
			} 

			Gdx.Gl20.GlDisable(GL20.GL_BLEND);
		}

		public Void GaussianBlur() {
			Gdx.Gl20.GlDisable(GL20.GL_BLEND);

			for (int I = 0; I < RayHandler.BlurNum; I++) {
				FrameBuffer.GetColorBufferTexture().Bind(0);
				PingPongBuffer.Begin();

				{
					BlurShader.Begin();
					BlurShader.SetUniformf("dir", 1f, 0f);
					LightMapMesh.Render(BlurShader, GL20.GL_TRIANGLE_FAN, 0, 4);
					BlurShader.End();
				}

				PingPongBuffer.End();
				PingPongBuffer.GetColorBufferTexture().Bind(0);
				FrameBuffer.Begin();

				{
					BlurShader.Begin();
					BlurShader.SetUniformf("dir", 0f, 1f);
					LightMapMesh.Render(BlurShader, GL20.GL_TRIANGLE_FAN, 0, 4);
					BlurShader.End();
				}

				if (RayHandler.CustomViewport) {
					FrameBuffer.End(RayHandler.ViewportX, RayHandler.ViewportY, RayHandler.ViewportWidth, RayHandler.ViewportHeight);
				} else {
					FrameBuffer.End();
				}

			}

			Gdx.Gl20.GlEnable(GL20.GL_BLEND);
		}

		public LightMap(RayHandler RayHandler, int FboWidth, int FboHeight) {
			this.RayHandler = RayHandler;

			if (FboWidth <= 0) FboWidth = 1;


			if (FboHeight <= 0) FboHeight = 1;


			FrameBuffer = new FrameBuffer(Format.RGBA8888, FboWidth, FboHeight, false);
			PingPongBuffer = new FrameBuffer(Format.RGBA8888, FboWidth, FboHeight, false);
			LightMapMesh = CreateLightMapMesh();
			ShadowShader = ShadowShader.CreateShadowShader();
			DiffuseShader = DiffuseShader.CreateShadowShader();
			WithoutShadowShader = WithoutShadowShader.CreateShadowShader();
			BlurShader = Gaussian.CreateBlurShader(FboWidth, FboHeight);
		}

		public Void Dispose() {
			ShadowShader.Dispose();
			BlurShader.Dispose();
			LightMapMesh.Dispose();
			FrameBuffer.Dispose();
			PingPongBuffer.Dispose();
		}

		private Mesh CreateLightMapMesh() {
			float[] Verts = new float[VERT_SIZE];
			Verts[X1] = -1;
			Verts[Y1] = -1;
			Verts[X2] = 1;
			Verts[Y2] = -1;
			Verts[X3] = 1;
			Verts[Y3] = 1;
			Verts[X4] = -1;
			Verts[Y4] = 1;
			Verts[U1] = 0f;
			Verts[V1] = 0f;
			Verts[U2] = 1f;
			Verts[V2] = 0f;
			Verts[U3] = 1f;
			Verts[V3] = 1f;
			Verts[U4] = 0f;
			Verts[V4] = 1f;
			Mesh TmpMesh = new Mesh(true, 4, 0, new VertexAttribute(Usage.Position, 2, "a_position"), new VertexAttribute(Usage.TextureCoordinates, 2, "a_texCoord"));
			TmpMesh.SetVertices(Verts);

			return TmpMesh;
		}
	}
}
