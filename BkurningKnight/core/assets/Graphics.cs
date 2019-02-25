using BurningKnight.core.entity;
using BurningKnight.core.entity.item;
using BurningKnight.core.game;
using BurningKnight.core.util;

namespace BurningKnight.core.assets {
	public class Graphics {
		public static SpriteBatch Batch;
		public static ShapeRenderer Shape;
		public static GlyphLayout Layout;
		public static TextureAtlas Atlas;
		public static BitmapFont Small;
		public static BitmapFont Medium;
		public static BitmapFont SmallSimple;
		public static BitmapFont MediumSimple;
		public static FrameBuffer Shadows;
		public static FrameBuffer Surface;
		public static FrameBuffer Text;
		public static FrameBuffer Map;
		public static FrameBuffer Blood;
		public static float DelayTime;

		public static Void Delay() {
			Delay(20);
		}

		public static Void Delay(float Ms) {
			Ms *= 0.001f;
			DelayTime = Math.Max(DelayTime, Ms * Settings.Freeze_frames * 2f);
		}

		public static Void StartShadows() {
			Graphics.Batch.End();
			Graphics.Surface.End(Camera.Viewport.GetScreenX(), Camera.Viewport.GetScreenY(), Camera.Viewport.GetScreenWidth(), Camera.Viewport.GetScreenHeight());
			Graphics.Shadows.Begin();
			Graphics.Batch.SetProjectionMatrix(Camera.Game.Combined);
			Graphics.Batch.Begin();
		}

		public static Void EndShadows() {
			Graphics.Batch.End();
			Graphics.Shadows.End(Camera.Viewport.GetScreenX(), Camera.Viewport.GetScreenY(), Camera.Viewport.GetScreenWidth(), Camera.Viewport.GetScreenHeight());
			Graphics.Surface.Begin();
			Graphics.Batch.Begin();
		}

		public static Void TargetAssets() {
			Log.Info("Init assets...");
			Batch = new SpriteBatch();
			Shape = new ShapeRenderer();
			Layout = new GlyphLayout();
			Shadows = new FrameBuffer(Pixmap.Format.RGBA8888, Gdx.Graphics.GetWidth(), Gdx.Graphics.GetHeight(), false);
			Surface = new FrameBuffer(Pixmap.Format.RGBA8888, Display.GAME_WIDTH, Display.GAME_HEIGHT, false);
			Text = new FrameBuffer(Pixmap.Format.RGBA8888, Display.GAME_WIDTH, Display.GAME_HEIGHT, false);
			Map = new FrameBuffer(Pixmap.Format.RGBA8888, Display.UI_WIDTH, Display.UI_HEIGHT, false);
			Blood = new FrameBuffer(Pixmap.Format.RGBA8888, Display.GAME_WIDTH, Display.GAME_HEIGHT, false);
			Assets.Manager.Load("atlas/atlas.atlas", TextureAtlas.GetType());
			FileHandleResolver Resolver = new InternalFileHandleResolver();
			Assets.Manager.SetLoader(FreeTypeFontGenerator.GetType(), new FreeTypeFontGeneratorLoader(Resolver));
			Assets.Manager.SetLoader(BitmapFont.GetType(), ".ttf", new FreetypeFontLoader(Resolver));
			Small = new BitmapFont(Gdx.Files.Internal("fonts/small.fnt"), Gdx.Files.Internal("fonts/small.png"), false);
			Medium = new BitmapFont(Gdx.Files.Internal("fonts/large.fnt"), Gdx.Files.Internal("fonts/large.png"), false);
			SmallSimple = new BitmapFont(Gdx.Files.Internal("fonts/small_simple.fnt"), Gdx.Files.Internal("fonts/small_simple.png"), false);
			MediumSimple = new BitmapFont(Gdx.Files.Internal("fonts/large_simple.fnt"), Gdx.Files.Internal("fonts/large_simple.png"), false);
		}

		public static Void LoadAssets() {
			Atlas = Assets.Manager.Get("atlas/atlas.atlas");
			Small.GetData().MarkupEnabled = true;
			Small.GetData().SetLineHeight(10);
			Medium.GetData().MarkupEnabled = true;
			SmallSimple.GetData().MarkupEnabled = true;
			SmallSimple.GetData().SetLineHeight(10);
			MediumSimple.GetData().MarkupEnabled = true;
			new Ui();
		}

		public static Void Resize(int W, int H) {
			W = Math.Max(W, Display.GAME_WIDTH);
			H = Math.Max(H, Display.GAME_HEIGHT);
			Shadows.Dispose();
			Shadows = new FrameBuffer(Pixmap.Format.RGBA8888, W, H, false);
		}

		public static TextureRegion GetTexture(string Name) {
			TextureRegion Region = Atlas.FindRegion(Name);

			if (Region == null) {
				Log.Error("Texture '" + Name + "' is not found!");

				return Item.Missing;
			} 

			return Region;
		}

		public static Void Write(string S, BitmapFont Font, float X, float Y) {
			Write(S, Font, X, Y, 0, 1, 1);
		}

		public static Void Write(string S, BitmapFont Font, float X, float Y, float A, float Sx, float Sy) {
			Graphics.Batch.End();
			Graphics.Surface.End();
			Graphics.Text.Begin();
			Graphics.Batch.Begin();
			Graphics.Layout.SetText(Font, S);
			Graphics.Batch.SetProjectionMatrix(Camera.Nil.Combined);
			Gdx.Gl.GlClearColor(0, 0, 0, 0);
			Gdx.Gl.GlClear(GL20.GL_COLOR_BUFFER_BIT | GL20.GL_DEPTH_BUFFER_BIT | (Gdx.Graphics.GetBufferFormat().CoverageSampling ? GL20.GL_COVERAGE_BUFFER_BIT_NV : 0));
			Font.Draw(Graphics.Batch, S, 2, 16);
			Graphics.Batch.End();
			Graphics.Text.End();
			Graphics.Surface.Begin();
			Graphics.Batch.Begin();
			Graphics.Batch.SetProjectionMatrix(Camera.Game.Combined);
			Texture Texture = Graphics.Text.GetColorBufferTexture();
			Texture.SetFilter(Texture.TextureFilter.Nearest, Texture.TextureFilter.Nearest);
			Graphics.Batch.Draw(Texture, X + 2, Y, Graphics.Layout.Width / 2 + 4, Graphics.Layout.Height, Graphics.Layout.Width, Graphics.Layout.Height * 2, Sx, Sy, A, 0, 0, (int) Graphics.Layout.Width + 4, (int) Graphics.Layout.Height * 2, false, true);
		}

		public static Void Print(string S, BitmapFont Font, float X, float Y) {
			Font.Draw(Batch, S, X, Y + (Font == Medium || Font == MediumSimple ? 16 : 7));
		}

		public static Void PrintCenter(string S, BitmapFont Font, float X, float Y) {
			Layout.SetText(Font, S);
			Print(S, Font, (Display.UI_WIDTH - Layout.Width) / 2 + X, Y);
		}

		public static Void Print(string S, BitmapFont Font, float Y) {
			Layout.SetText(Font, S);
			Print(S, Font, (Display.UI_WIDTH - Layout.Width) / 2, Y);
		}

		public static Void Render(TextureRegion Texture, float X, float Y) {
			float Ox = Texture.GetRegionWidth() / 2;
			float Oy = Texture.GetRegionHeight() / 2;
			Render(Texture, X + Ox, Y + Oy, 0, Ox, Oy, false, false);
		}

		public static Void Render(TextureRegion Texture, float X, float Y, float A, float Ox, float Oy, bool Fx, bool Fy) {
			Graphics.Batch.Draw(Texture, X - Ox + (Fx ? Texture.GetRegionWidth() : 0), Y - Oy + (Fy ? Texture.GetRegionHeight() : 0), Ox, Oy, Texture.GetRegionWidth(), Texture.GetRegionHeight(), Fx ? -1 : 1, Fy ? -1 : 1, A);
		}

		public static Void Render(TextureRegion Texture, float X, float Y, float A, float Ox, float Oy, bool Fx, bool Fy, float Sx, float Sy) {
			Graphics.Batch.Draw(Texture, X - Ox + (Fx ? Texture.GetRegionWidth() : 0), Y - Oy + (Fy ? Texture.GetRegionHeight() : 0), Ox, Oy, Texture.GetRegionWidth(), Texture.GetRegionHeight(), Sx, Sy, A);
		}

		public static Void Shadow(float X, float Y, float W, float H) {
			Shadow(X, Y, W, H, 0);
		}

		public static Void Shadow(float X, float Y, float W, float H, float Z) {
			W -= Z;
			H -= Z;
			X += Z / 2;
			Y -= Z / 2;
			Graphics.Shape.Ellipse(X - 1, Y - H / 4, W + 2, H / 2);
		}

		public static Void ShadowSized(float X, float Y, float W, float H, float S) {
			W -= S;
			H -= S;
			X += S / 2;
			Graphics.Shape.Ellipse(X - 1, Y - H / 4, W + 2, H / 2);
		}

		public static Void Shadow(float X, float Y, float W, float H, float Z, float A) {
			W -= Z;
			H -= Z;
			X += Z / 2;
			Y -= Z / 2;
			Graphics.Shape.SetColor(1, 1, 1, A);
			Graphics.Shape.Ellipse(X - 1, Y - H / 4, W + 2, H / 2);
			Graphics.Shape.SetColor(1, 1, 1, 1);
		}

		public static Void StartShape() {
			Graphics.Batch.End();
			Graphics.Shape.Begin(ShapeRenderer.ShapeType.Filled);
		}

		public static Void EndShape() {
			Graphics.Shape.End();
			Graphics.Batch.Begin();
		}

		public static Void StartAlphaShape() {
			Graphics.Batch.End();
			Gdx.Gl.GlEnable(GL20.GL_BLEND);
			Gdx.Gl.GlBlendFunc(GL20.GL_SRC_ALPHA, GL20.GL_ONE_MINUS_SRC_ALPHA);
			Graphics.Shape.Begin(ShapeRenderer.ShapeType.Filled);
		}

		public static Void EndAlphaShape() {
			Graphics.Shape.End();
			Gdx.Gl.GlDisable(GL20.GL_BLEND);
			Graphics.Batch.Begin();
		}

		public static Void Destroy() {
			if (Atlas == null) {
				return;
			} 

			Map.Dispose();
			Atlas.Dispose();
			Batch.Dispose();
			Shape.Dispose();
			Shadows.Dispose();
			Surface.Dispose();
			Blood.Dispose();
		}
	}
}
