using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.buff.fx;
using BurningKnight.core.entity.creature.mob;
using BurningKnight.core.entity.creature.mob.common;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.game;

namespace BurningKnight.core.entity.creature.buff {
	public class BurningBuff : Buff {
		protected void _Init() {
			{
				Id = Buffs.BURNING;
				Name = "Burning";
			}
		}

		public static TextureRegion Frame = Graphics.GetTexture("ui-debuff_frame");
		public static TextureRegion Flame = Graphics.GetTexture("ui-debuff_fire");
		private float LastFlame = 0;
		private float Progress;
		private float Rate;
		private bool Did;
		private float Al;

		public override Buff SetDuration(float Duration) {
			Rate = 1 / (Duration - 1);

			return base.SetDuration(Duration);
		}

		public override Void OnStart() {
			base.OnStart();
			SetDuration(7.0f);
			this.Owner.RemoveBuff(Buffs.FROZEN);
		}

		protected override Void OnUpdate(float Dt) {
			bool Mob = this.Owner is Mob;

			if (!this.Did) {
				Progress += Mob ? Dt * 2 : Dt * Rate;
			} 

			Dungeon.Level.AddLightInRadius(this.Owner.X + this.Owner.W / 2, this.Owner.Y + this.Owner.H / 2, 0.9f, 3f, false);
			this.LastFlame += Dt;

			if (this.LastFlame >= 0.1f) {
				Dungeon.Area.Add(new FlameFx(this.Owner));
				this.LastFlame = 0;
			} 

			if (Progress >= 1f) {
				if (!(this.Owner is BurningMan)) {
					this.Owner.ModifyHp(this.Owner is Player ? -1 : -2, null, true);
				} 

				if (!Mob) {
					Did = true;

					if (this.Owner is Player && this.Owner.IsDead()) {
						Achievements.Unlock(Achievements.BURN_TO_DEATH);
					} 

					this.Progress = 1f;
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
			float X = Creature.X + Creature.W / 2 - Frame.GetRegionWidth() / 2;
			float Y = Creature.Y + Creature.H + 4;

			if (!(this.Owner is Mob)) {
				Graphics.Batch.SetColor(1, 1, 1, this.Al);
				Graphics.Render(Frame, X, Y);

				if (this.Progress < 1f) {
					Graphics.Batch.SetColor(0.2f, 0.2f, 0.2f, this.Al);
					Graphics.Render(Flame, X + 1, Y + 1, 0, 0, 0, false, false, 1, 5);
				} 

				Graphics.Batch.SetColor(1, 1, 1, this.Al);
				Graphics.Render(Flame, X + 1, Y + 1, 0, 0, 0, false, false, 1, (float) Math.Floor(5 * Progress));
				Graphics.Batch.SetColor(1, 1, 1, 1);
			} 
		}

		public BurningBuff() {
			_Init();
		}
	}
}
