namespace Pico8Emulator.backend {
	public abstract class InputBackend {
		public Emulator Emulator;
		public abstract bool IsButtonDown(int i, int p);
	}
}