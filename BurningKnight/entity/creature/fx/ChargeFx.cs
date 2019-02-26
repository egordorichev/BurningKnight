using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.util;

namespace BurningKnight.entity.creature.fx {
	public class ChargeFx : Entity {
		private static Animation Animations = Animation.Make("fx_charge");
		public Entity Owner;
		private TextureRegion Region;
		private float Val;

		public ChargeFx(float X, float Y, float Val) {
			_Init();
			this.X = X;
			this.Y = Y;
			this.Val = Val;
			Region = Animations.GetFrames("idle").Get(Random.NewInt(Animations.GetFrames("idle").Size())).Frame;
		}

		protected void _Init() {
			{
				Depth = 15;
				AlwaysActive = true;
			}
		}

		public override void Update(float Dt) {
			var Dx = this.X - (Owner.W / 2 + Owner.X - 2);
			var Dy = this.Y - (Owner.H / 2 + Owner.Y - 2);
			var D = (float) Math.Sqrt(Dx * Dx + Dy * Dy);
			this.X -= Dx / D * 3;
			this.Y -= Dy / D * 3;

			if (D < 3) {
				Done = true;
				Lamp Lamp = (Lamp) Player.Instance.GetInventory().FindItem(Lamp.GetType());

				if (Lamp != null) {
					var V = Lamp.Val;
					Lamp.Val = Math.Min(100f, Lamp.Val + Val);

					if (V < 100f && Lamp.Val == 100f) Dungeon.Area.Add(new TextFx("Lamp Charged", Owner).SetColor(Dungeon.YELLOW));
				}
			}
		}

		public override void Render() {
			Graphics.Render(Region, this.X - 2, this.Y - 2);
		}
	}
}