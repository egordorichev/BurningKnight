using Microsoft.Xna.Framework;

namespace BurningKnight.entity.buff {
	public class InvisibleBuff : Buff {
		public static Vector4 Color = new Vector4(0.3f, 0.3f, 0.7f, 0.5f);
		
		public const string Id = "bk:invisible";
		public InvisibleBuff() : base(Id) {
			Duration = 10;
		}

		public override string GetIcon() {
			return "invisible";
		}
	}
}