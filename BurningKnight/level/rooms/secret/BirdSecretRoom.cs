using BurningKnight.entity.creature.npc;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.secret {
	public class BirdSecretRoom : SecretRoom {
		public override void Paint(Level level) {
			var c = GetTileCenter();
			var npc = new Bird();
			level.Area.Add(npc);
			npc.BottomCenter = c * 16 + new Vector2(8, 8);
		}
	}
}