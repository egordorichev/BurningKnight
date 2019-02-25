using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.mob;
using BurningKnight.core.entity.creature.player;

namespace BurningKnight.core.entity.creature.buff {
	public class PoisonedBuff : Buff {
		protected void _Init() {
			{
				Id = Buffs.POISONED;
				Name = "Poisoned";
			}
		}

		public static TextureRegion Poison = Graphics.GetTexture("ui-debuff_poison");
		private float Rate;
		private float Progress;
		private bool Did;
		private float Al;

		public override Buff SetDuration(float Duration) {
			Rate = 1 / (Duration - 1f);

			return base.SetDuration(Duration);
		}

		public override Void OnStart() {
			base.OnStart();
			SetDuration(11);
			this.Owner.Poisoned = true;
		}

		public override Void OnEnd() {
			base.OnEnd();
			this.Owner.Poisoned = false;
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			bool Mob = this.Owner is Mob;

			if (!Did) {
				Progress += Mob ? Dt * 2 : Dt * Rate;
			} 

			if (this.Progress >= 1) {
				this.Owner.ModifyHp(this.Owner is Player ? -1 : -2, null, true);

				if (this.Owner is Player) {
					this.Progress = 1f;
					this.Did = true;
				} else {
					this.Progress = 0;
				}

			} 

			this.Al += ((Did ? 0 : 1) - Al) * Dt * 5;

			if (this.Did && this.Al <= 0.05f) {
				this.SetDuration(0);
			} 
		}

		public override Void Render(Creature Creature) {
			float X = Creature.X + Creature.W / 2 - BurningBuff.Frame.GetRegionWidth() / 2;
			float Y = Creature.Y + Creature.H + 4;

			if (!(this.Owner is Mob)) {
				Graphics.Batch.SetColor(1, 1, 1, this.Al);
				Graphics.Render(BurningBuff.Frame, X, Y);

				if (this.Progress < 1f) {
					Graphics.Batch.SetColor(0.2f, 0.2f, 0.2f, this.Al);
					Graphics.Render(Poison, X + 1, Y + 1, 0, 0, 0, false, false, 1, 5);
				} 

				Graphics.Batch.SetColor(1, 1, 1, this.Al);
				Graphics.Render(Poison, X + 1, Y + 1, 0, 0, 0, false, false, 1, (float) Math.Floor(5 * Progress));
				Graphics.Batch.SetColor(1, 1, 1, 1);
			} 
		}

		public PoisonedBuff() {
			_Init();
		}
	}
}
