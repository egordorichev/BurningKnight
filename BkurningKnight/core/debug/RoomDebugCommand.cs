using BurningKnight.core.entity.level;

namespace BurningKnight.core.debug {
	public class RoomDebugCommand : ConsoleCommand {
		protected void _Init() {
			{
				Name = "/room";
				ShortName = "/r";
			}
		}

		public override Void Run(Console Console, string Args) {
			Level.RENDER_ROOM_DEBUG = !Level.RENDER_ROOM_DEBUG;
		}

		public RoomDebugCommand() {
			_Init();
		}
	}
}
