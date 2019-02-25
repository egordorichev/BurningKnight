using BurningKnight.core.entity.item.consumable;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.item.weapon.throwing {
	public class ConfettiGrenade : Consumable {
		public override Void Use() {
			base.Use();
			CGFx Fx = new CGFx();
			Fx.X = this.Owner.X + this.Owner.W / 2;
			Fx.Y = this.Owner.Y + this.Owner.H / 2;
			Point Aim = this.Owner.GetAim();
			float A = this.Owner.GetAngleTo(Aim.X, Aim.Y);
			float F = 80f;
			Fx.Vel.X = (float) Math.Cos(A) * F;
			Fx.Vel.Y = (float) Math.Sin(A) * F;
			Dungeon.Area.Add(Fx);
		}

		public override Void Generate() {
			base.Generate();
			this.SetCount(Random.NewInt(10, 30));
		}
	}
}
