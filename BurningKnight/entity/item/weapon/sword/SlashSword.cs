using BurningKnight.entity.item.weapon.gun;
using BurningKnight.physics;
using BurningKnight.util;

namespace BurningKnight.entity.item.weapon.sword {
	public class SlashSword : Weapon {
		protected float Added;
		protected float BackAngle = -60;
		protected float DelayA;
		protected float DelayB;
		protected float LastAngle;
		protected float MaxAngle = 150;
		protected float Move;
		protected float MoveXA;
		protected float MoveXB;
		protected float MoveY;
		protected float MoveYA;
		protected float MoveYB;
		protected float Ox;
		protected float Oy;
		protected float TimeC;

		protected void _Init() {
			{
				MoveXA = 5 * 2;
				MoveXB = -16 * 2;
				MoveYA = 8 * 2;
				MoveYB = 0;
				TimeA = 0.3f;
				DelayA = 0.15f;
				TimeB = 0.2f;
				DelayB = 0.1f;
				Penetrates = true;
				TimeC = 0.3f;
				UseTime = TimeA + DelayA + TimeB + DelayB + TimeC;
			}
		}

		public override void Render(float X, float Y, float W, float H, bool Flipped, bool Back) {
			var Angle = Added;

			if (Owner != null) {
				var An = Owner.GetWeaponAngle() - Math.PI;
				An = Gun.AngleLerp(LastAngle, An, 0.15f, Owner != null && Owner.Freezed);
				LastAngle = An;
				var A = (float) Math.ToDegrees(LastAngle);
				Angle += Flipped ? A : -A;
				Angle = Flipped ? Angle : 180 - Angle;
			}

			TextureRegion Sprite = GetSprite();
			var Xx = X + W / 2 + (Flipped ? 0 : W / 4) + Move;
			var Yy = Y + H / 4 + MoveY;
			RenderAt(Xx - (Flipped ? Sprite.GetRegionWidth() / 2 : 0), Yy, Back ? (Flipped ? -45 : 45) : Angle, Sprite.GetRegionWidth() / 2 + Ox, Oy, false, false, Flipped ? -1 : 1, 1);

			if (Body != null) {
				var A = (float) Math.ToRadians(Angle);
				World.CheckLocked(Body).SetTransform(Xx + (Flipped ? -W / 4 : 0), Yy, A);
			}
		}

		public void Use() {
			if (Delay > 0) return;

			Delay = UseTime;
			var An = Owner.GetWeaponAngle();
			var Ya = MoveYA;
			var Yb = MoveYB;
			var Xa = -MoveXA;
			var Xb = -MoveXB;

			if (Owner.IsFlipped()) {
				Ya *= -1;
				Yb *= -1;
			}

			double C = Math.Cos(An);
			double S = Math.Sin(An);
			var Mxb = (float) (Xb * C - Yb * S);
			var Myb = (float) (Xb * S + Yb * C);
			var Mxa = (float) (Xa * C - Ya * S);
			var Mya = (float) (Xa * S + Ya * C);
			Tween.To(new Tween.Task(Mxa, TimeA, Tween.Type.QUAD_OUT) {

		public override float GetValue() {
			return Move;
		}

		public override void SetValue(float Value) {
			Move = Value;
		}
	});

	Tween.To(new Tween.Task(Mya, this.TimeA, Tween.Type.QUAD_OUT) {
	public override float GetValue() {
		return MoveY;
	}

	public override void SetValue(float Value) {
		MoveY = Value;
	}
	});
	float FinalMyb = Myb;
	float FinalMxb = Mxb;
	Tween.To(new Tween.Task(this.BackAngle, this.TimeA, Tween.Type.QUAD_OUT) {
	public override float GetValue() {
		return Added;
	}

	public override void SetValue(float Value) {
		Added = Value;
	}

	public override void OnEnd() {
		Tween.To(new Tween.Task(MaxAngle, TimeB) {

	public override void OnStart() {
		base.OnStart();

		if (Body != null) Body = World.RemoveBody(Body);

		CreateHitbox();
		Owner.PlaySfx(GetSfx());
		Tween.To(new Tween.Task(FinalMxb, TimeB) {

	public override float GetValue() {
		return Move;
	}

	public override void SetValue(float Value) {
		Move = Value;
	}
	});
	Tween.To(new Tween.Task(FinalMyb, TimeB) {
	public override float GetValue() {
		return MoveY;
	}

	public override void SetValue(float Value) {
		MoveY = Value;
	}
	});
}

public override float GetValue() {
return Added;
}
public override void SetValue(float Value) {
Added = Value;
}
public override void OnEnd() {
Tween.To(new Tween.Task(0, TimeC) {
public override float GetValue() {
return Added;
}
public override void SetValue(float Value) {
Added = Value;
}
public override void OnEnd() {
EndUse();
}
public override void OnStart() {
Tween.To(new Tween.Task(0, TimeC) {
public override float GetValue() {
return Move;
}
public override void SetValue(float Value) {
Move = Value;
}
});
Tween.To(new Tween.Task(0, TimeC) {
public override float GetValue() {
return MoveY;
}
public override void SetValue(float Value) {
MoveY = Value;
}
});
}
}).Delay(DelayB);
}
}).Delay(DelayA);
}
});
}
public SlashSword() {
_Init();
}
}
}