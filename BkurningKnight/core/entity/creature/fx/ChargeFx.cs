using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.fx {
	public class ChargeFx : Entity {
		protected void _Init() {
			{
				Depth = 15;
				AlwaysActive = true;
			}
		}

		private static Animation Animations = Animation.Make("fx_charge");
		private TextureRegion Region;
		private float Val;
		public Entity Owner;

		public ChargeFx(float X, float Y, float Val) {
			_Init();
			this.X = X;
			this.Y = Y;
			this.Val = Val;
			this.Region = Animations.GetFrames("idle").Get(Random.NewInt(Animations.GetFrames("idle").Size())).Frame;
		}

		public override Void Update(float Dt) {
			float Dx = this.X - (this.Owner.W / 2 + this.Owner.X - 2);
			float Dy = this.Y - (this.Owner.H / 2 + this.Owner.Y - 2);
			float D = (float) Math.Sqrt(Dx * Dx + Dy * Dy);
			this.X -= Dx / D * 3;
			this.Y -= Dy / D * 3;

			if (D < 3) {
				this.Done = true;
				Lamp Lamp = (Lamp) Player.Instance.GetInventory().FindItem(Lamp.GetType());

				if (Lamp != null) {
					float V = Lamp.Val;
					Lamp.Val = Math.Min(100f, Lamp.Val + this.Val);

					if (V < 100f && Lamp.Val == 100f) {
						Dungeon.Area.Add(new TextFx("Lamp Charged", this.Owner).SetColor(Dungeon.YELLOW));
					} 
				} 
			} 
		}

		public override Void Render() {
			Graphics.Render(this.Region, this.X - 2, this.Y - 2);
		}
	}
}
