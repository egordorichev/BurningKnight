using BurningKnight.entity.level;

namespace BurningKnight.debug {
	public class RoomDebugCommand : ConsoleCommand {
		public RoomDebugCommand() {
			_Init();
		}

		protected void _Init() {
			{
				Name = "/room";
				ShortName = "/r";
			}
		}

		public override void Run(Console Console, string[] Args) {
			Level.RENDER_ROOM_DEBUG = !Level.RENDER_ROOM_DEBUG;
		}
	}
}