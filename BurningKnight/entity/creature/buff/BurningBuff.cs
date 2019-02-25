using BurningKnight.entity.creature.buff.fx;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.mob.common;
using BurningKnight.entity.creature.player;
using BurningKnight.game;

namespace BurningKnight.entity.creature.buff {
	public class BurningBuff : Buff {
		public static TextureRegion Frame = Graphics.GetTexture("ui-debuff_frame");
		public static TextureRegion Flame = Graphics.GetTexture("ui-debuff_fire");
		private float Al;
		private bool Did;
		private float LastFlame;
		private float Progress;
		private float Rate;

		public BurningBuff() {
			_Init();
		}

		protected void _Init() {
			{
				Id = Buffs.BURNING;
				Name = "Burning";
			}
		}

		public override Buff SetDuration(float Duration) {
			Rate = 1 / (Duration - 1);

			return base.SetDuration(Duration);
		}

		public override void OnStart() {
			base.OnStart();
			SetDuration(7.0f);
			Owner.RemoveBuff(Buffs.FROZEN);
		}

		protected override void OnUpdate(float Dt) {
			var Mob = Owner is Mob;

			if (!Did) Progress += Mob ? Dt * 2 : Dt * Rate;

			Dungeon.Level.AddLightInRadius(Owner.X + Owner.W / 2, Owner.Y + Owner.H / 2, 0.9f, 3f, false);
			LastFlame += Dt;

			if (LastFlame >= 0.1f) {
				Dungeon.Area.Add(new FlameFx(Owner));
				LastFlame = 0;
			}

			if (Progress >= 1f) {
				if (!(Owner is BurningMan)) Owner.ModifyHp(Owner is Player ? -1 : -2, null, true);

				if (!Mob) {
					Did = true;

					if (Owner is Player && Owner.IsDead()) Achievements.Unlock(Achievements.BURN_TO_DEATH);

					Progress = 1f;
				}
				else {
					Progress = 0;
				}
			}

			Al += ((Did ? 0 : 1) - Al) * Dt * 5;

			if (Did && Al <= 0.05f) SetDuration(0);
		}

		public override void Render(Creature Creature) {
			var X = Creature.X + Creature.W / 2 - Frame.GetRegionWidth() / 2;
			var Y = Creature.Y + Creature.H + 4;

			if (!(Owner is Mob)) {
				Graphics.Batch.SetColor(1, 1, 1, Al);
				Graphics.Render(Frame, X, Y);

				if (Progress < 1f) {
					Graphics.Batch.SetColor(0.2f, 0.2f, 0.2f, Al);
					Graphics.Render(Flame, X + 1, Y + 1, 0, 0, 0, false, false, 1, 5);
				}

				Graphics.Batch.SetColor(1, 1, 1, Al);
				Graphics.Render(Flame, X + 1, Y + 1, 0, 0, 0, false, false, 1, (float) Math.Floor(5 * Progress));
				Graphics.Batch.SetColor(1, 1, 1, 1);
			}
		}
	}
}