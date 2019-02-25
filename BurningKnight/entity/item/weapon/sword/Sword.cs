using BurningKnight.entity.creature;

namespace BurningKnight.entity.item.weapon.sword {
	public class Sword : SlashSword {
		private List<Frame> Frames = new List<>();
		protected float LastAngle;
		private float LastFrame;
		protected int MaxAngle;
		protected float Ox;

		protected float Oy;
		protected bool Tail;
		protected float Tb = 1f;
		protected float Tg = 1f;
		protected float Tr = 1f;

		public Sword() {
			_Init();
		}

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
				var Letter = "a";
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

		public override void Update(float Dt) {
			base.Update(Dt);
			LastFrame += Dt;

			if (LastFrame >= 0.005f) {
				LastFrame = 0;

				if (Added > 0) {
					var Frame = new Frame();
					Frame.Added = (float) Math.ToRadians(Added);
					Frames.Add(Frame);

					if (Frames.Size() > 10) Frames.Remove(0);
				}
				else if (Frames.Size() > 0) {
					Frames.Remove(0);

					if (Frames.Size() > 0) Frames.Remove(0);
				}
			}
		}

		public override void OnHit(Creature Creature) {
			base.OnHit(Creature);
			var A = Owner.GetAngleTo(Creature.X + Creature.W / 2, Creature.Y + Creature.H / 2);
			Owner.Knockback.X += -Math.Cos(A) * 120f;
			Owner.Knockback.Y += -Math.Sin(A) * 120f;
		}

		private static class Frame {
			public float Added;
		}
	}
}