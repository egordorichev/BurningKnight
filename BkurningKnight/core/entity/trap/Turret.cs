using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.fx;
using BurningKnight.core.entity.level.entities;
using BurningKnight.core.entity.level.rooms;
using BurningKnight.core.util;
using BurningKnight.core.util.file;

namespace BurningKnight.core.entity.trap {
	public class Turret : SolidProp {
		protected void _Init() {
			{
				AlwaysActive = true;
				Collider = new Rectangle(1, 10, 14, 4);
			}
		}

		protected AnimationData Single;
		public float A;
		public float Last;
		protected float Sp = 4f;
		private bool S;
		protected float Sx = 1f;
		protected float Sy = 1f;
		protected Room Room;
		protected byte Type;
		private float LastFlame;
		private bool Was;
		protected int Frame;
		protected bool On;
		private bool Rotated;

		public override Void Init() {
			base.Init();
			float R = Random.NewFloat();
			ReadRoom();
		}

		public override Void Load(FileReader Reader) {
			base.Load(Reader);
			this.Last = Reader.ReadFloat();
			this.A = Reader.ReadFloat();
			this.Type = Reader.ReadByte();
			this.Frame = Reader.ReadByte();
			ReadRoom();
		}

		private Void ReadRoom() {
			this.Room = Dungeon.Level.FindRoomFor(this.X, this.Y);
		}

		public override Void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteFloat(this.Last);
			Writer.WriteFloat(this.A);
			Writer.WriteByte(this.Type);
			Writer.WriteByte((byte) Math.FloorMod(this.Frame, 8));
		}

		protected Animation GetAnimation() {
			switch (this.Type) {
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

		public override Void Render() {
			if (Single == null) {
				Single = GetAnimation().Get("turret_1_directions");
			} 

			Single.Render(this.X, this.Y, false, false, 8, 0, 0, Sx, Sy);
		}

		protected Void SendFlames() {
			FireFx Fx = (Random.Chance(50) && this.T >= 1.5f) ? new FireFxPhysic() : new FireFx();
			float D = 5;
			Fx.X = X + Random.NewFloat(-4, 4) + 8 + (float) (Math.Cos(this.A) * D);
			Fx.Y = Y + Random.NewFloat(-4, 4) + 8 + (float) (Math.Sin(this.A) * D);
			float F = this.T >= 1.5f ? 120f : 40f;
			Fx.Vel.X = (float) (Math.Cos(this.A) * F);
			Fx.Vel.Y = (float) (Math.Sin(this.A) * F);
			Dungeon.Area.Add(Fx);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (this.On) {
				if (this.T >= 0.5f && this.T < 1.5f) {
					Was = false;

					return;
				} 

				if (!Was) {
					Tween();
					Was = true;
					this.PlaySfx("fire");
				} 

				this.LastFlame += Dt;

				if (this.LastFlame >= 0.04f) {
					if (this.Room == Player.Instance.Room && this.Room.LastNumEnemies > 0) {
						this.SendFlames();
					} 

					if (this.T >= 4.5f) {
						this.On = false;
					} 

					this.LastFlame = 0;
				} 
			} 

			if (this.Type == 3) {
				if (this.T >= 9f) {
					if (!this.Rotated) {
						if (this.Room == Player.Instance.Room && this.Room.LastNumEnemies > 0) {
							this.Rotate();
						} 

						this.Rotated = true;
					} 
				} else {
					this.Rotated = false;
				}

			} else {
				if (this.T >= this.Sp / 2) {
					if (!this.Rotated) {
						if (this.Room == Player.Instance.Room && this.Room.LastNumEnemies > 0) {
							this.Rotate();
						} 

						this.Rotated = true;
					} 
				} else {
					this.Rotated = false;
				}

			}


			if (!S) {
				S = true;

				for (int X = 0; X < this.W / 16; X++) {
					for (int Y = 0; Y < this.H / 16 + 1; Y++) {
						Dungeon.Level.SetPassable((int) (X + this.X / 16), (int) (Y + (this.Y + 8) / 16), false);
					}
				}
			} 

			if (this.Single != null) {
				SetFrame();
			} 

			this.Last += Dt;

			if (this.Last >= Sp / 2) {
				this.Last = 0;

				if (this.Room == Player.Instance.Room && this.Room.LastNumEnemies > 0) {
					this.Send();
				} 
			} 
		}

		protected Void SetFrame() {
			this.Single.SetFrame(7 - Math.FloorMod((int) (Math.Floor(this.A / (Math.PI / 4))) - 1, 8));
		}

		protected Void Tween() {
			Tween.To(new Tween.Task(0.6f, 0.1f) {
				public override float GetValue() {
					return Sy;
				}

				public override Void SetValue(float Value) {
					Sy = Value;
				}

				public override Void OnEnd() {
					Tween.To(new Tween.Task(1f, 0.15f) {
						public override float GetValue() {
							return Sy;
						}

						public override Void SetValue(float Value) {
							Sy = Value;
						}
					});
				}
			});
			Tween.To(new Tween.Task(1.4f, 0.1f) {
				public override float GetValue() {
					return Sx;
				}

				public override Void SetValue(float Value) {
					Sx = Value;
				}

				public override Void OnEnd() {
					Tween.To(new Tween.Task(1f, 0.15f) {
						public override float GetValue() {
							return Sx;
						}

						public override Void SetValue(float Value) {
							Sx = Value;
						}
					});
				}
			});
		}

		public override Void RenderShadow() {
			Graphics.ShadowSized(this.X, this.Y + 2, this.W, this.H, 6);
		}

		protected Void Send() {
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

		protected Void Modify(BulletProjectile Entity) {

		}

		protected Void Rotate() {

		}

		public Turret() {
			_Init();
		}
	}
}
