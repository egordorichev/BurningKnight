namespace BurningKnight.entity.creature.buff {
	public class FrozenBuff : Buff {
		public FrozenBuff() {
			_Init();
		}

		protected void _Init() {
			{
				Id = Buffs.FROZEN;
				Name = "Frozen";
				Duration = 5f;
			}
		}

		public override void OnStart() {
			Owner.Freezed = true;
			Owner.RemoveBuff(Buffs.BURNING);
		}

		public override void OnEnd() {
			Owner.Freezed = false;
		}
	}
}