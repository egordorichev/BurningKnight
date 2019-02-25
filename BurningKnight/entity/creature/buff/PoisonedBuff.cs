using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.player;

namespace BurningKnight.entity.creature.buff {
	public class PoisonedBuff : Buff {
		public static TextureRegion Poison = Graphics.GetTexture("ui-debuff_poison");
		private float Al;
		private bool Did;
		private float Progress;
		private float Rate;

		public PoisonedBuff() {
			_Init();
		}

		protected void _Init() {
			{
				Id = Buffs.POISONED;
				Name = "Poisoned";
			}
		}

		public override Buff SetDuration(float Duration) {
			Rate = 1 / (Duration - 1f);

			return base.SetDuration(Duration);
		}

		public override void OnStart() {
			base.OnStart();
			SetDuration(11);
			Owner.Poisoned = true;
		}

		public override void OnEnd() {
			base.OnEnd();
			Owner.Poisoned = false;
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			var Mob = Owner is Mob;

			if (!Did) Progress += Mob ? Dt * 2 : Dt * Rate;

			if (Progress >= 1) {
				Owner.ModifyHp(Owner is Player ? -1 : -2, null, true);

				if (Owner is Player) {
					Progress = 1f;
					Did = true;
				}
				else {
					Progress = 0;
				}
			}

			Al += ((Did ? 0 : 1) - Al) * Dt * 5;

			if (Did && Al <= 0.05f) SetDuration(0);
		}

		public override void Render(Creature Creature) {
			var X = Creature.X + Creature.W / 2 - BurningBuff.Frame.GetRegionWidth() / 2;
			var Y = Creature.Y + Creature.H + 4;

			if (!(Owner is Mob)) {
				Graphics.Batch.SetColor(1, 1, 1, Al);
				Graphics.Render(BurningBuff.Frame, X, Y);

				if (Progress < 1f) {
					Graphics.Batch.SetColor(0.2f, 0.2f, 0.2f, Al);
					Graphics.Render(Poison, X + 1, Y + 1, 0, 0, 0, false, false, 1, 5);
				}

				Graphics.Batch.SetColor(1, 1, 1, Al);
				Graphics.Render(Poison, X + 1, Y + 1, 0, 0, 0, false, false, 1, (float) Math.Floor(5 * Progress));
				Graphics.Batch.SetColor(1, 1, 1, 1);
			}
		}
	}
}