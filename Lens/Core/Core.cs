using Microsoft.Xna.Framework;

namespace Lens.Core {
	public class Core {
		public GameWindow Window;
		public GraphicsDeviceManager Graphics;
		
		public virtual void Init(int width, int height, bool fullscreen) {
			
		}

		public virtual void SetWindowed(int width, int height) {
			
		}

		public virtual void SetFullscreen() {
			
		}

		public static Core SelectCore(GameWindow Window, GraphicsDeviceManager Graphics) {
			Core core;
			
#if NSWITCH
			core = SwitchCore();
#elif XBOXONE
			core = new XBoxCore();
#elif PS4
			core = new PS4Core();
#else
			core = new DesktopCore();
#endif

			core.Window = Window;
			core.Graphics = Graphics;

			return core;
		}
	}
}