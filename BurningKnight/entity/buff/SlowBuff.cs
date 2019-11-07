using Microsoft.Xna.Framework;

namespace BurningKnight.entity.buff {
	public class SlowBuff : Buff {
		public const string Id = "bk:slow";
		public static Vector4 Color = new Vector4(0.4f - 0.5f, 0.26f - 0.5f, 0.12f - 0.5f, 1f);

		public SlowBuff() : base(Id) {
			Duration = 5;
		}

		public override string GetIcon() {
			return "snail";
		}
	}
}