using Lens.entity.component.logic;

namespace BurningKnight.entity.buff {
	public class FrozenBuff : Buff {
		public const string Id = "bk:frozen";
		
		public FrozenBuff() : base(Id) {
			Duration = 3;
		}

		public override void Init() {
			base.Init();

			if (Entity.TryGetComponent<StateComponent>(out var s)) {
				s.Pause++;
			}
		}

		public override void Destroy() {
			base.Destroy();

			if (Entity.TryGetComponent<StateComponent>(out var s)) {
				s.Pause--;
			}
		}
	}
}