namespace Pico8Emulator.backend {
	public abstract class AudioBackend {
		public Emulator Emulator;

		public abstract void Update();

		public virtual void Destroy() {
			
		}
	}
}