using BurningKnight.entity.item.weapon.gun;
using BurningKnight.entity.item.weapon.sword;
using BurningKnight.game;
using BurningKnight.game.input;
using BurningKnight.physics;
using BurningKnight.util;

namespace BurningKnight.entity.item.weapon.dagger {
	public class Dagger : Sword {
		protected void _Init() {
			{
				Knockback = 30f;
				TimeA = 0.1f;
				TimeB = 0.2f;
				DelayA = 0;
				DelayB = 0;
				var Letter = "a";
				Description = Locale.Get("dagger_desc");
				Name = Locale.Get("dagger_" + Letter);
				Sprite = "item-dagger_" + Letter;
				Damage = 4;
				Region = Graphics.GetTexture(Sprite);
			}
		}

		public override void Render(float X, float Y, float W, float H, bool Flipped, bool Back) {
			float Angle = 0;

			if (Owner != null) {
				var Aim = Owner.GetAim();
				var An = Owner.GetAngleTo(Aim.X, Aim.Y) - Math.PI / 2;
				An = Gun.AngleLerp(LastAngle, An, 0.15f, Owner != null && Owner.Freezed);
				LastAngle = An;
				var A = (float) Math.ToDegrees(LastAngle);
				Angle += A;
			}

			TextureRegion Sprite = GetSprite();
			var A = (float) Math.ToRadians(Angle);
			var An = Added / 16f;
			var Xx = X + W / 2 + (Flipped ? -W / 4 : W / 4) + Math.Cos(A + Math.PI / 2) * An;
			var Yy = Y + (Ox == 0 ? H / 4 : H / 2) + Math.Sin(A + Math.PI / 2) * An;
			RenderAt(Xx - (Flipped ? Sprite.GetRegionWidth() : 0), Yy, Back ? (Flipped ? -45 : 45) : Angle, Sprite.GetRegionWidth() / 2 + (Flipped ? Ox : -Ox), Oy, Flipped, false);

			if (Body != null) World.CheckLocked(Body).SetTransform(Xx, Yy, A);
		}

		public override void OnPickup() {
			base.OnPickup();
			Achievements.Unlock("UNLOCK_DAGGER");
		}

		public override void Use() {
			if (Delay > 0) return;

			if (Body != null) Body = World.RemoveBody(Body);

			CreateHitbox();
			Owner.PlaySfx(GetSfx());
			Delay = UseTime;
			var A = Owner.GetAngleTo(Input.Instance.WorldMouse.X, Input.Instance.WorldMouse.Y);
			Owner.Knockback.X += -Math.Cos(A) * 30f;
			Owner.Knockback.Y += -Math.Sin(A) * 30f;
			Tween.To(new Tween.Task(MaxAngle, TimeA) {

		public override float GetValue() {
			return Added;
		}

		public override void SetValue(float Value) {
			Added = Value;
		}

		public override void OnEnd() {
			if (TimeB == 0)
				Added = 0;
			else
				Tween.To(new Tween.Task(0, TimeB) {

		public override float GetValue() {
			return Added;
		}

		public override void SetValue(float Value) {
			Added = Value;
		}

		public override void OnEnd() {
			EndUse();
		}
	}).Delay(TimeDelay);
}

}
});
}
public Dagger() {
_Init();
}
}
}