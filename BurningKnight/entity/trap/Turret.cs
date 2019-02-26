using BurningKnight.entity.creature.player;
using BurningKnight.entity.fx;
using BurningKnight.entity.level.entities;
using BurningKnight.entity.level.rooms;
using BurningKnight.util;
using Lens.util.file;

namespace BurningKnight.entity.trap {
	public class Turret : SolidProp {
		public float A;
		protected int Frame;
		public float Last;
		private float LastFlame;
		protected bool On;
		protected Room Room;
		private bool Rotated;
		private bool S;

		protected AnimationData Single;
		protected float Sp = 4f;
		protected float Sx = 1f;
		protected float Sy = 1f;
		protected byte Type;
		private bool Was;

		protected void _Init() {
			{
				AlwaysActive = true;
				Collider = new Rectangle(1, 10, 14, 4);
			}
		}

		public override void Init() {
			base.Init();
			var R = Random.NewFloat();
			ReadRoom();
		}

		public override void Load(FileReader Reader) {
			base.Load(Reader);
			Last = Reader.ReadFloat();
			A = Reader.ReadFloat();
			Type = Reader.ReadByte();
			Frame = Reader.ReadByte();
			ReadRoom();
		}

		private void ReadRoom() {
			Room = Dungeon.Level.FindRoomFor(this.X, this.Y);
		}

		public override void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteFloat(Last);
			Writer.WriteFloat(A);
			Writer.WriteByte(Type);
			Writer.WriteByte((byte) Math.FloorMod(Frame, 8));
		}

		protected Animation GetAnimation() {
			switch (Type) {
				case 1: {
					return Animation.Make("actor-turret", "-poison");
				}

				case 2: {
					return Animation.Make("actor-turret", "-ice");
				}

				case 3: {
					return Animation.Make("actor-turret", "-fire");
				}
			}

			return Animation.Make("actor-turret", "-normal");
		}

		public override void Render() {
			if (Single == null) Single = GetAnimation().Get("turret_1_directions");

			Single.Render(this.X, this.Y, false, false, 8, 0, 0, Sx, Sy);
		}

		protected void SendFlames() {
			var Fx = Random.Chance(50) && T >= 1.5f ? new FireFxPhysic() : new FireFx();
			float D = 5;
			Fx.X = X + Random.NewFloat(-4, 4) + 8 + Math.Cos(A) * D;
			Fx.Y = Y + Random.NewFloat(-4, 4) + 8 + Math.Sin(A) * D;
			var F = T >= 1.5f ? 120f : 40f;
			Fx.Vel.X = Math.Cos(A) * F;
			Fx.Vel.Y = Math.Sin(A) * F;
			Dungeon.Area.Add(Fx);
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (On) {
				if (T >= 0.5f && T < 1.5f) {
					Was = false;

					return;
				}

				if (!Was) {
					Tween();
					Was = true;
					PlaySfx("fire");
				}

				LastFlame += Dt;

				if (LastFlame >= 0.04f) {
					if (Room == Player.Instance.Room && Room.LastNumEnemies > 0) SendFlames();

					if (T >= 4.5f) On = false;

					LastFlame = 0;
				}
			}

			if (Type == 3) {
				if (T >= 9f) {
					if (!Rotated) {
						if (Room == Player.Instance.Room && Room.LastNumEnemies > 0) this.Rotate();

						Rotated = true;
					}
				}
				else {
					Rotated = false;
				}
			}
			else {
				if (T >= Sp / 2) {
					if (!Rotated) {
						if (Room == Player.Instance.Room && Room.LastNumEnemies > 0) this.Rotate();

						Rotated = true;
					}
				}
				else {
					Rotated = false;
				}
			}


			if (!S) {
				S = true;

				for (var X = 0; X < W / 16; X++)
				for (var Y = 0; Y < H / 16 + 1; Y++)
					Dungeon.Level.SetPassable(X + this.X / 16, Y + (this.Y + 8) / 16, false);
			}

			if (Single != null) SetFrame();

			Last += Dt;

			if (Last >= Sp / 2) {
				Last = 0;

				if (Room == Player.Instance.Room && Room.LastNumEnemies > 0) this.Send();
			}
		}

		protected void SetFrame() {
			Single.SetFrame(7 - Math.FloorMod((int) Math.Floor(A / (Math.PI / 4)) - 1, 8));
		}

		protected void Tween() {
			Tween.To(new Tween.Task(0.6f, 0.1f) {

		public override float GetValue() {
			return Sy;
		}

		public override void SetValue(float Value) {
			Sy = Value;
		}

		public override void OnEnd() {
			Tween.To(new Tween.Task(1f, 0.15f) {

		public override float GetValue() {
			return Sy;
		}

		public override void SetValue(float Value) {
			Sy = Value;
		}
	});
}

});
Tween.To(new Tween.Task(1.4f, 0.1f) {
public override float GetValue() {
return Sx;
}
public override void SetValue(float Value) {
Sx = Value;
}
public override void OnEnd() {
Tween.To(new Tween.Task(1f, 0.15f) {
public override float GetValue() {
return Sx;
}
public override void SetValue(float Value) {
Sx = Value;
}
});
}
});
}
public override void RenderShadow() {
Graphics.ShadowSized(this.X, this.Y + 2, this.W, this.H, 6);
}
protected void Send() {
this.Tween();
this.T = 0;
this.PlaySfx(this.Type == 3 ? "fire" : "gun_machinegun");
BulletProjectile Bullet = new NanoBullet();
float X = (float) (this.X + 8 + Math.Cos(this.A) * 8);
float Y = (float) (this.Y + 8 + Math.Sin(this.A) * 8);
if (this.Type == 3) {
this.On = true;
this.T = 0;
} else {
Bullet.X = X;
Bullet.Y = Y;
Bullet.Damage = 2;
Bullet.W = 12;
Bullet.H = 12;
Bullet.Bad = true;
this.Modify(Bullet);
float S = 1.5f * 30f;
Bullet.Velocity = new Point((float) Math.Cos(this.A) * S, (float) Math.Sin(this.A) * S);
Bullet.A = A;
Dungeon.Area.Add(Bullet);
}
}
protected void Modify(BulletProjectile Entity) {
}
protected void Rotate() {
}
public Turret() {
_Init();
}
}
}