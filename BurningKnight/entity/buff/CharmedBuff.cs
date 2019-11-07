using Microsoft.Xna.Framework;

namespace BurningKnight.entity.buff {
	public class CharmedBuff : Buff {
		public const string Id = "bk:charmed";
		public static Vector4 Color = new Vector4(0.5f, -0.2f, 0.5f, 1f);
		
		public CharmedBuff() : base(Id) {
			Duration = 10;
		}
	}
}