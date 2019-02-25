using BurningKnight.entity.item.consumable;
using BurningKnight.util;

namespace BurningKnight.entity.item.weapon.throwing {
	public class ConfettiGrenade : Consumable {
		public override void Use() {
			base.Use();
			var Fx = new CGFx();
			Fx.X = Owner.X + Owner.W / 2;
			Fx.Y = Owner.Y + Owner.H / 2;
			var Aim = Owner.GetAim();
			var A = Owner.GetAngleTo(Aim.X, Aim.Y);
			var F = 80f;
			Fx.Vel.X = (float) Math.Cos(A) * F;
			Fx.Vel.Y = (float) Math.Sin(A) * F;
			Dungeon.Area.Add(Fx);
		}

		public override void Generate() {
			base.Generate();
			SetCount(Random.NewInt(10, 30));
		}
	}
}