namespace Pico8Emulator.backend {
	public abstract class GraphicsBackend {
		public Emulator Emulator;

		public abstract void CreateSurface();
		public abstract void Flip();
	}
}