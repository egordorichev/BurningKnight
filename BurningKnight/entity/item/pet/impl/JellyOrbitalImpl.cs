using BurningKnight.entity.item.weapon.projectile;
using BurningKnight.util;

namespace BurningKnight.entity.item.pet.impl {
	public class JellyOrbitalImpl : Orbital {
		private Tween.Task Xlast;
		private Tween.Task Ylast;

		protected void _Init() {
			{
				Sprite = "item-jelly";
			}
		}

		protected override void OnHit(Entity Entity) {
			if (Entity is Projectile && ((Projectile) Entity).Bad) {
				((Projectile) Entity).Velocity.X *= -1;
				((Projectile) Entity).Velocity.Y *= -1;
				((Projectile) Entity).Bad = false;
			}

			if (Xlast != null) {
				Tween.Remove(Xlast);
				Tween.Remove(Ylast);
				Xlast = null;
				Ylast = null;
			}

			Xlast = Tween.To(new Tween.Task(1.3f, 0.1f) {

		public override float GetValue() {
			return Sx;
		}

		public override void SetValue(float Value) {
			Sx = Value;
		}

		public override void OnEnd() {
			Xlast = Tween.To(new Tween.Task(1f, 1f, Tween.Type.ELASTIC_OUT) {

		public override float GetValue() {
			return Sx;
		}

		public override void SetValue(float Value) {
			Sx = Value;
		}

		public override void OnEnd() {
			Xlast = null;
		}
	});
}

});
this.Ylast = Tween.To(new Tween.Task(0.7f, 0.1f) {
public override float GetValue() {
return Sy;
}
public override void SetValue(float Value) {
Sy = Value;
}
public override void OnEnd() {
Ylast = Tween.To(new Tween.Task(1f, 1f, Tween.Type.ELASTIC_OUT) {
public override float GetValue() {
return Sy;
}
public override void SetValue(float Value) {
Sy = Value;
}
public override void OnEnd() {
Ylast = null;
}
});
}
});
}
public JellyOrbitalImpl() {
_Init();
}
}
}