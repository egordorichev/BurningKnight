namespace BurningKnight.core.entity.creature.buff {
	public class FrozenBuff : Buff {
		protected void _Init() {
			{
				Id = Buffs.FROZEN;
				Name = "Frozen";
				Duration = 5f;
			}
		}

		public override Void OnStart() {
			this.Owner.Freezed = true;
			this.Owner.RemoveBuff(Buffs.BURNING);
		}

		public override Void OnEnd() {
			this.Owner.Freezed = false;
		}

		public FrozenBuff() {
			_Init();
		}
	}
}
