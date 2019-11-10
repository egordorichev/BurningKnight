using BurningKnight.entity.component;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.buff {
	public class SlowBuff : Buff {
		public const string Id = "bk:slow";
		public static Vector4 Color = new Vector4(0.4f - 0.5f, 0.26f - 0.5f, 0.12f - 0.5f, 1f);

		public SlowBuff() : base(Id) {
			Duration = 5;
		}

		public override void Init() {
			base.Init();

			if (Entity.TryGetComponent<RectBodyComponent>(out var b)) {
				b.Slow = true;
			}

			if (Entity.TryGetComponent<SensorBodyComponent>(out var sb)) {
				sb.Slow = true;
			}

			if (Entity.TryGetComponent<CircleBodyComponent>(out var cb)) {
				cb.Slow = true;
			}
		}

		public override void Destroy() {
			base.Destroy();
			
			if (Entity.TryGetComponent<RectBodyComponent>(out var b)) {
				b.Slow = false;
			}

			if (Entity.TryGetComponent<SensorBodyComponent>(out var sb)) {
				sb.Slow = false;
			}

			if (Entity.TryGetComponent<CircleBodyComponent>(out var cb)) {
				cb.Slow = false;
			}
		}

		public override string GetIcon() {
			return "snail";
		}
	}
}