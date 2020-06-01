using BurningKnight.entity.component;

namespace BurningKnight.entity.buff {
	public class ConfusedBuff : Buff {
		public const string Id = "bk:confused";
		
		public ConfusedBuff() : base(Id) {
			Duration = 128;
		}
		
		public override void Init() {
			base.Init();

			if (Entity.TryGetComponent<RectBodyComponent>(out var b)) {
				b.Confused = true;
			}

			if (Entity.TryGetComponent<SensorBodyComponent>(out var sb)) {
				sb.Confused = true;
			}

			if (Entity.TryGetComponent<CircleBodyComponent>(out var cb)) {
				cb.Confused = true;
			}
		}

		public override void Destroy() {
			base.Destroy();
			
			if (Entity.TryGetComponent<RectBodyComponent>(out var b)) {
				b.Confused = false;
			}

			if (Entity.TryGetComponent<SensorBodyComponent>(out var sb)) {
				sb.Confused = false;
			}

			if (Entity.TryGetComponent<CircleBodyComponent>(out var cb)) {
				cb.Confused = false;
			}
		}

		public override string GetIcon() {
			return "confused";
		}
	}
}