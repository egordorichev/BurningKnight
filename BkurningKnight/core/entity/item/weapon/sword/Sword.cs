using BurningKnight.core.assets;
using BurningKnight.core.entity.creature;

namespace BurningKnight.core.entity.item.weapon.sword {
	public class Sword : SlashSword {
		protected void _Init() {
			{
				MoveXA = 0;
				MoveXB = -8;
				MoveYA = 0;
				MoveYB = 0;
				TimeA = 0f;
				DelayA = 0.0f;
				TimeB = 0.15f;
				DelayB = 0.05f;
				TimeC = 0.1f;
				BackAngle = 0;
				MaxAngle = 200;
				UseTime = TimeA + DelayA + TimeB + DelayB + TimeC;
				string Letter = "a";
				Name = Locale.Get("sword_" + Letter);
				Description = Locale.Get("sword_desc");
				Sprite = "item-sword_" + Letter;
				Damage = 10;
				UseTime = 0.4f;
				Region = Graphics.GetTexture(Sprite);
			}

			{
				Name = "Sword";
				Sprite = "item-sword_b";
			}
		}

		private static class Frame {
			public float Added;
		}

		protected float Oy;
		protected float Ox;
		protected int MaxAngle;
		private float LastFrame;
		private List<Frame> Frames = new List<>();
		protected float Tr = 1f;
		protected float Tg = 1f;
		protected float Tb = 1f;
		protected float LastAngle;
		protected bool Tail;

		public override Void Update(float Dt) {
			base.Update(Dt);
			this.LastFrame += Dt;

			if (this.LastFrame >= 0.005f) {
				this.LastFrame = 0;

				if (this.Added > 0) {
					Frame Frame = new Frame();
					Frame.Added = (float) Math.ToRadians(this.Added);
					this.Frames.Add(Frame);

					if (this.Frames.Size() > 10) {
						this.Frames.Remove(0);
					} 
				} else if (this.Frames.Size() > 0) {
					this.Frames.Remove(0);

					if (this.Frames.Size() > 0) {
						this.Frames.Remove(0);
					} 
				} 
			} 
		}

		public override Void OnHit(Creature Creature) {
			base.OnHit(Creature);
			float A = this.Owner.GetAngleTo(Creature.X + Creature.W / 2, Creature.Y + Creature.H / 2);
			this.Owner.Knockback.X += -Math.Cos(A) * 120f;
			this.Owner.Knockback.Y += -Math.Sin(A) * 120f;
		}

		public Sword() {
			_Init();
		}
	}
}
