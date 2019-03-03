using Lens;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.level.levels {
	public class CastleLevel : RegularLevel {
		public CastleLevel() : base("castle_biome") {
			Engine.Instance.StateRenderer.Bg = new Color();
		}
	}
}