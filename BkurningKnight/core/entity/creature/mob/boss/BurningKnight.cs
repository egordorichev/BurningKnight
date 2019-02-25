using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.level.rooms;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.creature.mob.boss {
	public class BurningKnight : Boss {
		protected void _Init() {
			{
				Texture = "ui-bkbar-skull";
				HpMax = 120;
				Damage = 10;
				W = 23;
				H = 30;
				Depth = 16;
				AlwaysActive = true;
				AlwaysRender = true;
				Speed = 2;
				MaxSpeed = 100;
				KnockbackMod = 0;
				SetFlying(true);
				Idle = Animations.Get("idle");
				Anim = Idle;
				Hurt = Animations.Get("hurt");
				Killed = Animations.Get("dead");
				Defeated = Animations.Get("defeat");
				Unhittable = false;
			}
		}

		static BurningKnight() {
			string VertexShader;
			string FragmentShader;
			VertexShader = Gdx.Files.Internal("shaders/default.vert").ReadString();
			FragmentShader = Gdx.Files.Internal("shaders/bk.frag").ReadString();
			Shader = new ShaderProgram(VertexShader, FragmentShader);

			if (!Shader.IsCompiled()) throw new GdxRuntimeException("Couldn't compile shader: " + Shader.GetLog());

		}

		private static class Frame : Point {
			public bool Flipped;
			public float S = 1f;
			public int Frame;
		}

		public class BKState : State<BurningKnight>  {
			public override Void Update(float Dt) {
				if (Self.Target != null) {
					Self.LastSeen = new Point(Self.Target.X, Self.Target.Y);
				} 

				base.Update(Dt);
			}
		}

		public class WaitState : BKState {
			public override Void Update(float Dt) {
				base.Update(Dt);
				Self.CheckForTarget();
			}
		}

		public class IdleState : BKState {
			public float Delay;

			public override Void OnEnter() {
				base.OnEnter();
				this.Delay = Random.NewFloat(1f, 3f);
			}

			public override Void Update(float Dt) {
				if (this.T >= this.Delay) {
					Self.Become("roam");

					return;
				} 

				Self.CheckForTarget();
				base.Update(Dt);
			}
		}

		public class RoamState : BKState {
			public float Delay;
			public Point RoomToVisit;

			public override Void OnEnter() {
				base.OnEnter();
				this.Delay = Random.NewFloat(30f, 60f);
			}

			public override Void Update(float Dt) {
				if (this.T >= this.Delay) {
					Self.Become("fadeOut");

					return;
				} 

				Self.CheckForTarget();

				if (this.RoomToVisit == null) {
					Room Room;
					float D;
					int Attempts = 0;

					do {
						Room = Dungeon.Level.GetRandomRoom();
						Point Point = Room.GetCenter();
						float Dx = Point.X * 16 - Self.X;
						float Dy = Point.Y * 16 - Self.Y;
						D = (float) Math.Sqrt(Dx * Dx + Dy * Dy);
						Attempts++;

						if (Attempts > 40) {
							Log.Info("Too many");

							break;
						} 
					} while (D > 400f && (Self.Last == null || Self.Last != Room));

					this.RoomToVisit = Room.GetCenter();
					this.RoomToVisit.Mul(16);
					Self.Last = Room;
				} 

				if (this.RoomToVisit != null) {
					if (this.FlyTo(this.RoomToVisit, Self.Speed * 5, 32f)) {
						if (Random.Chance(25)) {
							Self.Become("idle");

							return;
						} else {
							this.RoomToVisit = null;
						}

					} 
				} else {
					Log.Error("No room");
				}


				base.Update(Dt);
			}
		}

		public class AlertedState : BKState {
			public const float DELAY = 1f;

			public override Void OnEnter() {
				base.OnEnter();
			}

			public override Void Update(float Dt) {
				if (this.T >= DELAY) {
					Self.Become("preattack");

					return;
				} 

				base.Update(Dt);
			}
		}

		public class LTiredState : BKState {
			private float Delay;

			public override Void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(1f, 3f);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (T >= Delay) {
					Self.Become("lchase");
				} 
			}
		}

		public class LChaseState : BKState {
			public float Delay;

			public override Void OnEnter() {
				base.OnEnter();
				this.Delay = Random.NewFloat(5f, 7f);
			}

			public override Void Update(float Dt) {
				if (this.FlyTo(Player.Instance, 20, 24f)) {

				} 

				if (T >= Delay) {
					Self.Become("ltired");

					return;
				} 

				base.Update(Dt);
			}
		}

		public class ChaseState : BKState {
			public float Delay;

			public override Void OnEnter() {
				base.OnEnter();
				this.Delay = Random.NewFloat(5f, 7f);
			}

			public override Void Update(float Dt) {
				if (this.FlyTo(Player.Instance, 30, 16f)) {
					Self.Become("explode");

					return;
				} 

				if (T >= Delay) {
					Self.Become("preattack");

					return;
				} 

				base.Update(Dt);
			}
		}

		public class DashState : BKState {
			public float Delay;

			public override Void OnEnter() {
				base.OnEnter();
				this.Delay = Random.NewFloat(1f, 3f);
			}

			public override Void Update(float Dt) {
				if (Self.T >= this.Delay) {
					Self.Become("preattack");

					return;
				} 

				base.Update(Dt);
			}
		}

		public class PreattackState : BKState {
			public override Void Update(float Dt) {
				if (Self.Dl > 0) {
					Self.Dl -= Dt;

					return;
				} 

				int I = LastAttack % (Pat.GetNumAttacks());

				if (I == 0) {
					Pattern = Random.NewInt(3);
				} 

				Self.Become(Pat.GetState(Pattern, I));
				LastAttack++;
			}
		}

		public class AttackState : BKState {
			public bool Attacked;

			public override Void Update(float Dt) {
				if (Self.Target == null) {
					Self.Become("idle");

					return;
				} 

				if (!this.Attacked) {
					Sword.Use();
					this.Attacked = true;
				} else if (Sword.GetDelay() == 0) {
					if (Random.Chance(30)) {
						float A = Random.NewFloat(0, (float) (Math.PI * 2));
						float D = 128;
						Tween.To(new Tween.Task(0, 0.5f) {
							public override float GetValue() {
								return Self.A;
							}

							public override Void SetValue(float Value) {
								Self.A = Value;
							}

							public override Void OnEnd() {
								Self.Tp((float) Math.Cos(A) * D + Player.Instance.X - Player.Instance.W / 2 + Self.W / 2, (float) Math.Sin(A) * D + Player.Instance.Y - Player.Instance.H / 2 + Self.H / 2);
								Tween.To(new Tween.Task(1, 0.5f) {
									public override float GetValue() {
										return Self.A;
									}

									public override Void SetValue(float Value) {
										Self.A = Value;
									}
								});
							}
						});
					} else {
						Self.Become("preattack");
					}

				} 

				base.Update(Dt);
			}
		}

		public class AppearState : BKState {
			public override Void OnEnter() {
				Self.AttackTp = true;
				Self.FindStartPoint();
				Self.SetUnhittable(true);
				Input.Instance.Blocked = true;
				Self.Rage = true;
				Self.Knockback.X = 0;
				Self.Knockback.Y = 0;
				Self.Velocity.X = 0;
				Self.Velocity.Y = 0;
				Self.Hp = 1;
				Self.IgnoreRooms = true;
				Lamp.Play();
				Camera.Shake(6);

				for (int I = 0; I < 30; I++) {
					CurseFx Fx = new CurseFx();
					Fx.X = X + W / 2 + Random.NewFloat(-W, W);
					Fx.Y = Y + H / 2 + Random.NewFloat(-H, H);
					Dungeon.Area.Add(Fx);
				}

				Camera.Follow(Self, false);
				PlaySfx("explosion");
				Self.A = 0;
				Tween.To(new Tween.Task(0.6f, 2f) {
					public override float GetValue() {
						return Self.A;
					}

					public override Void SetValue(float Value) {
						Self.A = Value;
					}
				});
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (this.T < 1.6f) {
					FadeFx Ft = new FadeFx();
					float An = Random.NewFloat((float) (Math.PI * 2));
					Ft.X = (float) (X + W / 2 + Math.Cos(An) * 48);
					Ft.Y = (float) (Y + H / 2 + Math.Sin(An) * 48);
					Ft.To = true;
					float Fr = 150;
					Ft.Vel = new Point((float) Math.Cos(An - Math.PI) * Fr, (float) Math.Sin(An - Math.PI) * Fr);
					Dungeon.Area.Add(Ft);
				} else if (Self.A == 0.6f) {
					Input.Instance.Blocked = false;
					Camera.Follow(Player.Instance, false);
					Self.Become("preattack");
				} 
			}
		}

		public class FadeInState : BKState {
			public override Void OnEnter() {
				Self.A = 0;
				Camera.Follow(Self, false);
				Tween.To(new Tween.Task(Self.Rage ? 0.6f : 1f, 2f) {
					public override float GetValue() {
						return Self.A;
					}

					public override Void SetValue(float Value) {
						Self.A = Value;
					}

					public override Void OnEnd() {
						Self.Become("lchase");
						Self.AttackTp = false;
						Input.Instance.Blocked = false;
						Camera.Follow(Player.Instance, false);
					}
				});
				Camera.Shake(6);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (T < 1.6f) {
					FadeFx Ft = new FadeFx();
					float An = Random.NewFloat((float) (Math.PI * 2));
					Ft.X = (float) (X + W / 2 + Math.Cos(An) * 48);
					Ft.Y = (float) (Y + H / 2 + Math.Sin(An) * 48);
					Ft.To = true;
					float Fr = 150;
					Ft.Vel = new Point((float) Math.Cos(An - Math.PI) * Fr, (float) Math.Sin(An - Math.PI) * Fr);
					Dungeon.Area.Add(Ft);
				} 
			}
		}

		public class FadeOutState : BKState {
			public override Void OnEnter() {
				base.OnEnter();
				Tween.To(new Tween.Task(0, 2f) {
					public override float GetValue() {
						return Self.A;
					}

					public override Void SetValue(float Value) {
						Self.A = Value;
					}

					public override Void OnEnd() {
						Self.FindStartPoint();
						Self.Become("fadeIn");
					}
				});
				Camera.Shake(8);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (T < 1.6f) {
					FadeFx Ft = new FadeFx();
					Ft.X = X + W / 2;
					Ft.Y = Y + H / 2;
					float Fr = 70;
					float An = Random.NewFloat((float) (Math.PI * 2));
					Ft.Vel = new Point((float) Math.Cos(An) * Fr, (float) Math.Sin(An) * Fr);
					Dungeon.Area.Add(Ft);
				} 
			}
		}

		public class DialogState : BKState {
			public override Void OnEnter() {
				base.OnEnter();
				Dialog.Active = Self.Dialog;
				Dialog.Active.Start();
				Camera.Follow(Self, false);
				Dialog.Active.OnEnd(new Runnable() {
					public override Void Run() {
						Camera.Follow(Player.Instance, false);
					}
				});
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (Dialog.Active == null) {
					Self.Become("preattack");
				} 
			}

			public override Void OnExit() {
				base.OnExit();
				Talked = true;
			}
		}

		public class UnactiveState : BKState {
			public override Void OnEnter() {
				base.OnEnter();
				Self.A = 0;
				Self.SetUnhittable(true);
				Self.Tp(-100, -100);
				Mob.Every.Remove(Self);
			}

			public override Void OnExit() {
				base.OnExit();
				Self.SetUnhittable(false);
				Mob.Every.Add(Self);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (Self.T >= 0.1f && Dungeon.Depth > -1 && (!(Dungeon.Level is BossLevel) || Player.Instance.Room is BossRoom)) {
					Log.Info("BK is out");
					float A = Random.NewAngle();
					float D = 64;

					if (Player.Instance.Room is BossRoom) {
						Self.Tp((Player.Instance.Room.Left + Player.Instance.Room.GetWidth() / 2) * 16, (Player.Instance.Room.Top + Player.Instance.Room.GetHeight() / 2) * 16);
						Camera.Follow(Self, false);
					} else {
						Self.Tp((float) (Player.Instance.X + 8 + Math.Cos(A) * D), (float) (Player.Instance.Y + 8 + Math.Sin(A) * D));
					}


					Self.SetUnhittable(false);
					Self.Become("fadeIn");
					Self.A = 0;
					Self.Dl = 1f;
					Camera.Shake(8);
					Lamp.Play();
				} 
			}
		}

		public class SpawnAttack : BKState {
			private int Cn;
			private Point Center;
			private int Num;
			private bool Reached;

			public override Void OnEnter() {
				base.OnEnter();
				Center = Self.Room.GetCenter();
				Center.X *= 16;
				Center.Y *= 16;
				Cn = Random.NewInt(1, 5);
				MobPool.Instance.InitForFloor();
				MobPool.Instance.InitForRoom();
				int Count = 0;

				foreach (Mob Mob in Mob.All) {
					if (Mob.Room == Self.Room) {
						Count++;
					} 
				}

				if (Count >= 8f) {
					Self.Become("preattack");
				} 
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (!Reached) {
					if (this.FlyTo(Center, 15f, 16f)) {
						Reached = true;
						this.T = 0;
					} 

					return;
				} 

				if (Num < Cn && this.T > Num * 0.5f + 1f) {
					Point Point = Self.Room.GetRandomFreeCell();

					if (Point == null) {
						return;
					} 

					Mob Mob = null;

					try {
						Mob = (Mob) MobPool.Instance.Generate().Types.Get(0).NewInstance();
					} catch (InstantiationException | IllegalAccessException) {
						E.PrintStackTrace();
					}

					if (Mob == null) {
						MobPool.Instance.InitForFloor();

						try {
							Mob = (Mob) MobPool.Instance.Generate().Types.Get(0).NewInstance();
						} catch (InstantiationException | IllegalAccessException) {
							E.PrintStackTrace();
						}
					} 

					Mob.X = Point.X * 16 + Random.NewFloat(-8, 8);
					Mob.Y = Point.Y * 16 + Random.NewFloat(-8, 8);
					Mob.NoDrop = true;
					Mob.Poof();
					Dungeon.Area.Add(Mob);
					Num++;

					return;
				} 

				if (Num >= Cn) {
					Self.Become("preattack");
				} 
			}
		}

		public class DefeatedState : BKState {
			public override Void OnEnter() {
				Self.Dead = false;
				Self.DeathDepth = Dungeon.Depth;
				Self.Done = false;
				Self.Hp = 1;
				Self.Rage = true;
				Self.Unhittable = true;
				Self.IgnoreRooms = true;
				Self.Tp(-1000, -1000);

				if (!Player.Instance.DidGetHit()) {
					Achievements.Unlock(Achievements.DONT_GET_HIT_IN_BOSS_FIGHT);
				} 
			}

			public override Void OnExit() {
				Lamp.Play();
				Tween.To(new Tween.Task(0.6f, 0.3f) {
					public override float GetValue() {
						return A;
					}

					public override Void SetValue(float Value) {
						A = Value;
					}
				});
			}
		}

		public class AwaitState : BKState {
			public override Void Update(float Dt) {
				base.Update(Dt);

				if (Player.Instance == null || Player.Instance.IsDead() || !(Player.Instance.Room is ShopRoom)) {
					Self.Become("idle");
				} 
			}
		}

		public class MissileAttackState : BKState {
			private int Num;

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (Num < 7) {
					if (this.T >= 1.5f) {
						Num++;
						this.T = 0;
						MissileProjectile Missile = new MissileProjectile();
						Missile.To = Player.Instance;
						Missile.Bad = true;
						Missile.Owner = Self;
						Missile.X = Self.X + Self.GetOx();
						Missile.Y = Self.Y + Self.H - 16;
						Dungeon.Area.Add(Missile);
						MissileAppear Appear = new MissileAppear();
						Appear.Missile = Missile;
						Dungeon.Area.Add(Appear);
					} 
				} else {
					Self.Become("preattack");
				}

			}
		}

		public class NewAttackState : BKState {
			private int Num;

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (T >= 3f) {
					if (Num == 5) {
						Self.Become("preattack");

						return;
					} 

					T = 0;
					Num++;
					BulletProjectile Ball = new SkullBullet() {
						protected override Void OnDeath() {
							base.OnDeath();

							if (!BrokeWeapon) {
								for (int I = 0; I < 8; I++) {
									BulletProjectile Ball = new NanoBullet();
									bool Fast = I % 2 == 0;
									float Vel = Fast ? 70 : 40;
									float A = (float) (I * Math.PI / 4);
									Ball.Velocity = new Point((float) Math.Cos(A) / 2f, (float) Math.Sin(A) / 2f).Mul(Vel * Mob.ShotSpeedMod);
									Ball.X = (this.X);
									Ball.Y = (this.Y);
									Ball.Damage = 2;
									Ball.Bad = true;
									Dungeon.Area.Add(Ball);
								}
							} 
						}
					};
					Ball.Depth = 17;
					float A = Self.GetAngleTo(Self.Target.X + 8, Self.Target.Y + 8) + Random.NewFloat(-0.3f, 0.3f);
					Ball.Velocity = new Point((float) Math.Cos(A) / 2f, (float) Math.Sin(A) / 2f).Mul(60f * Mob.ShotSpeedMod);
					Ball.X = (Self.X + Self.W / 2);
					Ball.Y = (Self.Y + Self.H - 8);
					Ball.Damage = 2;
					Ball.Bad = true;
					Ball.Auto = true;
					Ball.Delay = 0.5f;
					Ball.Alp = 0;
					Tween.To(new Tween.Task(1, 0.5f) {
						public override float GetValue() {
							return Ball.Alp;
						}

						public override Void SetValue(float Value) {
							Ball.Alp = Value;
						}
					});
					Dungeon.Area.Add(Ball);
				} 
			}
		}

		public class LaserAttackState : BKState {
			private Laser Laser;
			private float Last;
			private int Num;

			public override Void Update(float Dt) {
				base.Update(Dt);
				float X = Self.X + GetOx();
				float Y = Self.Y + Self.H + Self.Z - 12;

				if (Laser != null) {
					Laser.X = X;
					Laser.Y = Y;
				} 

				if (Last <= 0) {
					Last = 2f;
					T = 0;
					Laser = new Laser();
					double An = Self.GetAngleTo(Self.GetAim().X, Self.GetAim().Y);
					Laser.Fake = true;
					Laser.X = X;
					Laser.Y = Y;
					Laser.A = (float) Math.ToDegrees(An - Math.PI / 2);
					Laser.Huge = false;
					Laser.W = 32f;
					Laser.Bad = true;
					Laser.Damage = 2;
					Laser.Depth = 17;
					Laser.Owner = Self;
					Dungeon.Area.Add(Laser);
				} else if (T >= 2f && !Laser.Removing) {
					Laser.Remove();
					Last = 0;
					Num++;
				} else if (Laser.Al == 1) {
					Laser.Huge = true;
					Laser.Fake = false;
				} 

				if (T >= 2f && Num >= 3) {
					Self.Become("preattack");
				} 

				Last -= Dt;
			}

			public override Void OnExit() {
				base.OnExit();

				if (!Laser.Dead) {
					Laser.Remove();
				} 
			}
		}

		public class LaserAimAttackState : BKState {
			private Laser Laser;
			private float AVel;
			private bool Removed;

			public override Void OnEnter() {
				Laser = new Laser();
				float X = Self.X + GetOx();
				float Y = Self.Y + Self.H + Self.Z - 12;
				double An = Self.GetAngleTo(Self.GetAim().X, Self.GetAim().Y);
				Laser.Fake = true;
				Laser.X = X;
				Laser.Y = Y;
				Laser.A = (float) Math.ToDegrees(An - Math.PI / 2) + (Random.Chance(50) ? 10 : -10);
				Laser.Huge = false;
				Laser.W = 32f;
				Laser.Bad = true;
				Laser.Damage = 2;
				Laser.Owner = Self;
				Dungeon.Area.Add(Laser);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);
				float X = Self.X + GetOx();
				float Y = Self.Y + Self.H + Self.Z - 12;
				Laser.X = X;
				Laser.Y = Y;

				if (this.T > 2 || this.Laser.Al == 1) {
					Laser.Huge = true;
					Laser.Fake = false;
					Laser.Depth = 17;

					if (T <= 9f) {
						double An = Math.ToDegrees(Self.GetAngleTo(Self.GetAim().X, Self.GetAim().Y) - Math.PI / 2);
						float V = (float) (An - Laser.A);
						float F = 64 * (Math.Abs(V) > Math.PI ? 3 : 1);

						if (Gun.ShortAngleDist((float) Math.ToRadians(Laser.A), (float) Math.ToRadians(An)) > 0) {
							AVel += Dt * F;
						} else {
							AVel -= Dt * F;
						}

					} 

					Laser.A += AVel * Dt;
					AVel -= AVel * Dt;
					Laser.Recalc();

					if (this.T >= 10) {
						if (!this.Removed) {
							Removed = true;
							Laser.Remove();
							Self.Become("preattack");
						} 
					} 
				} 
			}

			public override Void OnExit() {
				base.OnExit();

				if (!Laser.Dead) {
					Laser.Remove();
				} 
			}
		}

		public class RangedAttackState : BKState {
			private int Count;
			private List<FireballProjectile> Balls = new List<>();
			private bool Fast;
			private float Speed = 1;
			private float Tt;
			private bool Did;
			private int I;
			private bool Left;
			private float Wait;
			private bool Sec;

			public override Void OnEnter() {
				base.OnEnter();
				Fast = Random.Chance(50);
				Count = Random.NewInt(2, 8);
			}

			public override Void OnExit() {
				foreach (FireballProjectile Ball in Balls) {
					Ball.Done = true;
				}

				Balls.Clear();
			}

			public override Void Update(float Dt) {
				base.Update(Dt);
				Speed = Math.Min(Speed + Dt * 10, 10);
				Tt += Speed * Dt;
				float R = 24;

				for (int I = 0; I < Balls.Size(); I++) {
					FireballProjectile Ball = Balls.Get(I);
					double A = ((float) I) / Count * Math.PI * 2 + Tt;
					Ball.SetPos(Self.X + Self.W / 2 + (float) Math.Cos(A) * R, Self.Y + Self.H / 2 + (float) Math.Sin(A) * R);
				}

				if (Sec) {
					if (this.Wait > 0) {
						this.Wait -= Dt;

						return;
					} 

					if (Fast) {
						bool AllDir = Random.Chance(50);
						double A = (Self.GetAngleTo(Self.Target.X + Self.Target.W / 2, Self.Target.Y + Self.Target.H / 2));
						float S = 200;

						for (int I = 0; I < Balls.Size(); I++) {
							FireballProjectile Ball = Balls.Get(I);

							if (AllDir) {
								A = ((float) I) / Count * Math.PI * 2 + Tt;
							} 

							if (Ball.Done) {
								Ball.Destroy();
							} else {
								Ball.Tar = new Point();
								Ball.Tar.X = (float) (S * Math.Cos(A));
								Ball.Tar.Y = (float) (S * Math.Sin(A));
							}

						}

						this.Balls.Clear();
						Self.Become("preattack");
					} else {
						if (this.Balls.Size() == 0) {
							Self.Become("preattack");

							return;
						} 

						float T = (Self.GetAngleTo(Self.Target.X + Self.Target.W / 2, Self.Target.Y + Self.Target.H / 2));
						float An = (float) ((((float) (this.Balls.Size() - 1)) / Count * Math.PI * 2 + Dungeon.Time * 10) % (Math.PI * 2));

						if (Math.Abs(T - An) % (Math.PI * 2) < Math.PI / 16) {
							FireballProjectile Ball = this.Balls.Get(this.Balls.Size() - 1);
							this.Balls.Remove(this.Balls.Size() - 1);

							if (Ball.Done) {
								Ball.Destroy();
							} else {
								float A = (float) (T + Math.ToRadians(Random.NewFloat(-5, 5)));
								float S = 200;
								Ball.Tar = new Point();
								Ball.Tar.X = (float) (S * Math.Cos(A));
								Ball.Tar.Y = (float) (S * Math.Sin(A));
							}

						} 
					}

				} else if (this.T >= (Self.Rage ? Balls.Size() * 0.25f + 0.25f : Balls.Size() * 0.5f + 1f)) {
					FireballProjectile Ball = new FireballProjectile();
					Ball.Owner = Self;
					Dungeon.Area.Add(Ball);
					Balls.Add(Ball);

					if (Balls.Size() == Count) {
						this.T = 0;
						this.Wait = Self.Rage ? 0.5f : 1.5f;
						this.Sec = true;
					} 
				} 
			}
		}

		public class ExplodeState : BKState {
			public override Void Update(float Dt) {
				base.Update(Dt);
				Explosion.Make(Self.X + Self.W / 2, Self.Y + Self.H / 2, true);

				for (int I = 0; I < Dungeon.Area.GetEntities().Size(); I++) {
					Entity Entity = Dungeon.Area.GetEntities().Get(I);

					if (Entity is Creature && Entity != Self) {
						Creature Creature = (Creature) Entity;

						if (Creature.GetDistanceTo(Self.X + W / 2, Self.Y + H / 2) < 24f) {
							if (!Creature.ExplosionBlock) {
								if (Creature is Player) {
									Creature.ModifyHp(-1000, Self, true);
								} else {
									Creature.ModifyHp(-Math.Round(Random.NewFloatDice(20 / 3 * 2, 20)), Self, true);
								}

							} 

							float A = (float) Math.Atan2(Creature.Y + Creature.H / 2 - Self.Y - 8, Creature.X + Creature.W / 2 - Self.X - 8);
							Creature.Knockback.X += Math.Cos(A) * 10f * Creature.KnockbackMod;
							Creature.Knockback.Y += Math.Sin(A) * 10f * Creature.KnockbackMod;
						} 
					} else if (Entity is BombEntity) {
						BombEntity B = (BombEntity) Entity;
						float A = (float) Math.Atan2(B.Y - Self.Y, B.X - Self.X) + Random.NewFloat(-0.5f, 0.5f);
						B.Vel.X += Math.Cos(A) * 200f;
						B.Vel.Y += Math.Sin(A) * 200f;
					} 
				}

				Self.TpFromPlayer();
				Self.Become("preattack");
			}
		}

		public class TpntackState : BKState {
			private int Cn;
			private int Nm;
			private float Last;

			public override Void OnEnter() {
				base.OnEnter();
				Cn = Random.NewInt(3, 6);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (Nm >= Cn && T >= (Cn + 1) * 1f) {
					Self.Become("preattack");
				} 

				if (Nm >= Cn) {
					return;
				} 

				Last -= Dt;

				if (Last <= 0) {
					Last = 1f;
					Nm++;
					Tween.To(new Tween.Task(0, 0.2f) {
						public override float GetValue() {
							return Self.A;
						}

						public override Void SetValue(float Value) {
							Self.A = Value;
						}

						public override Void OnEnd() {
							Self.TpFromPlayer();
							Tween.To(new Tween.Task(1, 0.2f) {
								public override float GetValue() {
									return Self.A;
								}

								public override Void SetValue(float Value) {
									Self.A = Value;
								}

								public override Void OnEnd() {
									RectBulletPattern Pattern = new RectBulletPattern();

									for (int I = 0; I < 4; I++) {
										Pattern.AddBullet(NewProjectile());
									}

									BulletPattern.Fire(Pattern, Self.X + GetOx(), Self.Y + H / 2, Self.GetAngleTo(Self.Target.X + 8, Self.Target.Y + 8), 70f);
								}
							});
						}
					});
				} 
			}
		}

		public class SpinState : BKState {
			private float Last;
			private bool Good;
			private float Ls;
			private float Dir;

			public override Void OnEnter() {
				base.OnEnter();
				Dir = Random.Chance(50) ? -1 : 1;
				Good = Random.Chance(50);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);
				Last -= Dt;
				Ls += Dt;

				if (T <= 5f && Ls >= 2f) {
					Ls = 0;
					BulletProjectile Ball = new SkullBullet() {
						protected override Void OnDeath() {
							base.OnDeath();

							if (!BrokeWeapon) {
								for (int I = 0; I < 8; I++) {
									BulletProjectile Ball = new NanoBullet();
									bool Fast = I % 2 == 0;
									float Vel = Fast ? 70 : 40;
									float A = (float) (I * Math.PI / 4);
									Ball.Velocity = new Point((float) Math.Cos(A) / 2f, (float) Math.Sin(A) / 2f).Mul(Vel * Mob.ShotSpeedMod);
									Ball.X = (this.X);
									Ball.Y = (this.Y);
									Ball.Damage = 2;
									Ball.Bad = true;
									Dungeon.Area.Add(Ball);
								}
							} 
						}
					};
					Ball.Depth = 17;
					float A = Self.GetAngleTo(Self.Target.X + 8, Self.Target.Y + 8) + Random.NewFloat(-0.3f, 0.3f);
					Ball.Velocity = new Point((float) Math.Cos(A) / 2f, (float) Math.Sin(A) / 2f).Mul(40f * Mob.ShotSpeedMod);
					Ball.X = (Self.X + Self.W / 2);
					Ball.Y = (Self.Y + Self.H - 8);
					Ball.Damage = 2;
					Ball.Bad = true;
					Ball.Auto = true;
					Ball.Delay = 0.5f;
					Ball.Alp = 0;
					Tween.To(new Tween.Task(1, 0.5f) {
						public override float GetValue() {
							return Ball.Alp;
						}

						public override Void SetValue(float Value) {
							Ball.Alp = Value;
						}
					});
					Dungeon.Area.Add(Ball);
				} 

				if (T >= 12f) {
					Self.Become("preattack");

					return;
				} 

				if (T <= 5f && Last <= 0) {
					Last = Good ? 0.2f : 0.1f;
					float S = 30 * Mob.ShotSpeedMod;

					for (int I = 0; I < 6; I++) {
						if (Random.Chance(10)) {
							continue;
						} 

						float A = ((float) I) / 6 * Dir;

						if (Good) {
							A *= Math.PI * 2;
							A += T * 2f;
						} else {
							if (I % 2 == 0) {
								A += Math.PI;
							} 

							A += T * 0.5f;
						}


						BulletProjectile Bullet = new NanoBullet();
						Bullet.NoLight = true;
						Bullet.Damage = 1;
						Bullet.Owner = Self;
						Bullet.Bad = true;
						Bullet.X = X + GetOx();
						Bullet.Y = Y + H / 2;
						Bullet.Velocity.X = (float) (Math.Cos(A)) * S;
						Bullet.Velocity.Y = (float) (Math.Sin(A)) * S;
						Dungeon.Area.Add(Bullet);
					}
				} 
			}
		}

		public class SkullState : BKState {
			private int Num;

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (T >= 3f) {
					if (Num == 5) {
						Self.Become("preattack");

						return;
					} 

					T = 0;
					Num++;
					float A = Self.GetAngleTo(Self.Target.X + 8, Self.Target.Y + 8);
					Shoot(A - 0.5f);
					Shoot(A + 0.5f);
				} 
			}

			private Void Shoot(float A) {
				BulletProjectile Ball = new BadBullet() {
					protected override Void OnDeath() {
						base.OnDeath();

						if (!BrokeWeapon) {
							for (int I = 0; I < 16; I++) {
								BulletProjectile Ball = I > 7 ? new BulletRect() : new NanoBullet();
								bool Fast = I % 2 == 0;
								float Vel = I > 7 ? 80 : Fast ? 70 : 40;
								float A = (float) (I * Math.PI / 4);

								if (I > 7) {
									A += Math.PI / 8;
								} 

								Ball.Velocity = new Point((float) Math.Cos(A) / 2f, (float) Math.Sin(A) / 2f).Mul(Vel * Mob.ShotSpeedMod);
								Ball.X = (this.X);
								Ball.Y = (this.Y);
								Ball.Damage = 2;
								Ball.Bad = true;
								Dungeon.Area.Add(Ball);
							}
						} 
					}
				};
				Ball.Depth = 17;
				Ball.Velocity = new Point((float) Math.Cos(A) / 2f, (float) Math.Sin(A) / 2f).Mul(40f * Mob.ShotSpeedMod);
				Ball.X = (Self.X + Self.W / 2);
				Ball.Y = (Self.Y + Self.H - 8);
				Ball.Damage = 2;
				Ball.Bad = true;
				Ball.DissappearWithTime = true;
				Ball.Ds = 2f;
				Dungeon.Area.Add(Ball);
			}
		}

		public class NanoState : BKState {
			private NanoPattern Pattern;

			public override Void OnEnter() {
				base.OnEnter();
				Pattern = new NanoPattern();

				for (int I = 0; I < 32; I++) {
					BulletProjectile B = NewProjectile();
					B.NoLight = true;
					Pattern.AddBullet(B);
				}

				BulletPattern.Fire(Pattern, Self.X + GetOx(), Self.Y + H / 2, 0, 0f);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (Pattern.Done && T >= 12f) {
					Self.Become("preattack");
				} 
			}
		}

		public class TearState : BKState {
			private float Last;

			public override Void Update(float Dt) {
				base.Update(Dt);
				Last -= Dt;

				if (Last <= 0 && T <= 5f) {
					Last = 0.25f;
					BulletProjectile Bullet = new GreenBullet();
					Bullet.Damage = 1;
					Bullet.Owner = Self;
					Bullet.Bad = true;
					Bullet.Bounce = 3;
					Bullet.Ds = 2f;
					Bullet.NoLight = true;
					float A = GetAngleTo(Self.Target.X + 8, Self.Target.Y + 8) + Random.NewFloat(-0.1f, 0.1f);
					float S = (30 + Random.NewFloat(-10f, 10f)) * Mob.ShotSpeedMod;
					Bullet.Ra = A;
					Bullet.A = (float) Math.ToDegrees(A);
					Bullet.NoLight = true;
					Bullet.X = X + GetOx();
					Bullet.Y = Y + H / 2;
					Bullet.Velocity.X = (float) (Math.Cos(A)) * S;
					Bullet.Velocity.Y = (float) (Math.Sin(A)) * S;
					Dungeon.Area.Add(Bullet);
				} 

				if (T >= 12f) {
					Self.Become("preattack");
				} 
			}
		}

		public class CircState : BKState {
			private float Last;
			private int M;

			public override Void Update(float Dt) {
				base.Update(Dt);
				Last -= Dt;

				if (T >= 12f) {
					Self.Become("preattack");

					return;
				} 

				if (T <= 5f * 1.5f && Last <= 0) {
					M++;
					Last = 1.5f;
					float S = 20 * Mob.ShotSpeedMod;
					int Cn = 32;

					for (int I = 0; I < Cn; I++) {
						if ((I + (M % 2 * 4)) % 8 <= 1) {
							continue;
						} 

						float A = (float) (((float) I) / Cn * Math.PI * 2);
						BulletProjectile Bullet = new BulletRect();
						Bullet.NoLight = true;
						Bullet.Damage = 1;
						Bullet.Owner = Self;
						Bullet.Bad = true;
						Bullet.X = X + GetOx();
						Bullet.Y = Y + H / 2;
						Bullet.Velocity.X = (float) (Math.Cos(A)) * S;
						Bullet.Velocity.Y = (float) (Math.Sin(A)) * S;
						Dungeon.Area.Add(Bullet);
					}
				} 
			}
		}

		public class FourState : BKState {
			private Laser[] Lasers = new Laser[5];
			private float Dir;
			private float A;
			private float AVel;
			private bool Removed;

			public override Void OnEnter() {
				float X = Self.X + GetOx();
				float Y = Self.Y + Self.H + Self.Z - 12;
				double An = Self.GetAngleTo(Self.GetAim().X, Self.GetAim().Y);
				float Max = (float) (Math.ToDegrees(An - Math.PI / 2) + (360 / Lasers.Length / 2));
				A = Max;

				for (int I = 0; I < Lasers.Length; I++) {
					Laser Laser = new Laser();
					Laser.Fake = true;
					Laser.X = X;
					Laser.Y = Y;
					Laser.A = (Max + ((360 / Lasers.Length) * I));
					Laser.Huge = false;
					Laser.W = 32f;
					Laser.Bad = true;
					Laser.Damage = 2;
					Laser.Owner = Self;
					Lasers[I] = Laser;
					Dungeon.Area.Add(Laser);
				}

				Dir = 1;
			}

			public override Void Update(float Dt) {
				base.Update(Dt);
				float X = Self.X + GetOx();
				float Y = Self.Y + Self.H + Self.Z - 12;

				if (T < 9.5f) {
					AVel += Dt * Dir * 200;
				} 

				A += AVel * Dt;
				AVel -= AVel * Dt * 4;

				for (int I = 0; I < Lasers.Length; I++) {
					Laser Laser = Lasers[I];
					Laser.X = X;
					Laser.Y = Y;

					if (this.T > 2 || Laser.Al == 1) {
						Laser.Huge = true;
						Laser.Fake = false;
						Laser.Depth = 17;
						Laser.A = A + I * (360 / Lasers.Length);
						Laser.Recalc();

						if (this.T >= 10) {
							if (!this.Removed) {
								Removed = true;
								Laser.Remove();
								Self.Become("preattack");
							} 
						} 
					} 
				}
			}

			public override Void OnExit() {
				base.OnExit();

				for (int I = 0; I < Lasers.Length; I++) {
					Laser Laser = Lasers[I];

					if (!Laser.Dead) {
						Laser.Remove();
					} 
				}
			}
		}

		public class BookState : BKState {
			private float Last;
			private int M;
			private float An;

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (T >= 11f) {
					Self.Become("preattack");

					return;
				} 

				Last -= Dt;

				if (M < 32 && Last <= 0) {
					if (M % 8 == 0) {
						An += Math.PI / 4;
					} 

					M++;
					Last = 0.15f;
					float S = 70 * Mob.ShotSpeedMod;

					for (int I = 0; I < 4; I++) {
						float A = (float) (((float) I) / 4 * Math.PI * 2) + An;
						BulletProjectile Bullet = new NanoBullet();
						Bullet.Sprite = Graphics.GetTexture("bullet-nano");
						Bullet.NoLight = true;
						Bullet.Damage = 1;
						Bullet.Owner = Self;
						Bullet.Bad = true;
						Bullet.X = X + GetOx();
						Bullet.Y = Y + H / 2;
						Bullet.Velocity.X = (float) (Math.Cos(A)) * S;
						Bullet.Velocity.Y = (float) (Math.Sin(A)) * S;
						Dungeon.Area.Add(Bullet);
					}
				} 
			}
		}

		public class LineState : BKState {
			private float Last;
			private int M;

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (T >= 12f) {
					Self.Become("preattack");

					return;
				} 

				Last -= Dt;

				if (M < 32 && Last <= 0) {
					M++;
					Last = 0.1f;
					float S = 80 * Mob.ShotSpeedMod;
					float An = Self.GetAngleTo(Self.Target.X + 8, Self.Target.Y + 8);
					float D = 24;

					for (int I = 0; I < 2; I++) {
						float A = (float) (((float) I) / 2 * Math.PI * 2 + Math.PI / 2);
						BulletProjectile Bullet = new NanoBullet();
						Bullet.Sprite = Graphics.GetTexture("bullet-nano");
						Bullet.NoLight = true;
						Bullet.Damage = 1;
						Bullet.Owner = Self;
						Bullet.Bad = true;
						Bullet.X = X + GetOx() + (float) (Math.Cos(A)) * D;
						Bullet.Y = Y + H / 2 + (float) (Math.Sin(A)) * D;
						Bullet.Velocity.X = (float) (Math.Cos(An)) * S;
						Bullet.Velocity.Y = (float) (Math.Sin(An)) * S;
						Dungeon.Area.Add(Bullet);
					}

					if (M % 16 == 0) {
						BulletProjectile Bullet = new NanoBullet();
						Bullet.NoLight = true;
						Bullet.Damage = 1;
						Bullet.Owner = Self;
						Bullet.Bad = true;
						Bullet.X = X + GetOx();
						Bullet.Y = Y + H / 2;
						Bullet.Velocity.X = (float) (Math.Cos(An)) * S;
						Bullet.Velocity.Y = (float) (Math.Sin(An)) * S;
						Dungeon.Area.Add(Bullet);
					} 
				} 
			}
		}

		public class WeirdState : BKState {
			private BulletPattern Pattern;

			public override Void OnEnter() {
				base.OnEnter();
				Pattern = new WeirdPattern();

				for (int I = 0; I < 20; I++) {
					BulletProjectile B = NewProjectile();
					B.NoLight = true;
					B.DissappearWithTime = true;
					B.Ds = 5.5f;
					Pattern.AddBullet(B);
				}

				BulletPattern.Fire(Pattern, Self.X + GetOx(), Self.Y + H / 2, 0, 0f);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (Pattern.Done && T >= 12f) {
					Self.Become("preattack");
				} 
			}
		}

		public class CShootState : BKState {
			private CircleBulletPattern Pattern;

			public override Void OnEnter() {
				base.OnEnter();
				Pattern = new CircleBulletPattern();
				Pattern.Radius = 32;
				Pattern.Grow = true;

				for (int I = 0; I < 24; I++) {
					BulletProjectile Bullet = new NanoBullet() {
						protected override Void OnDeath() {
							base.OnDeath();

							if (!BrokeWeapon) {
								BulletProjectile Ball = new NanoBullet();
								float Vel = 180;
								float A = this.GetAngleTo(Self.Target.X + 8, Self.Target.Y + 8);
								Ball.Velocity = new Point((float) Math.Cos(A) / 2f, (float) Math.Sin(A) / 2f).Mul(Vel * Mob.ShotSpeedMod);
								Ball.X = (this.X);
								Ball.Y = (this.Y);
								Ball.Damage = 2;
								Ball.Bad = true;
								Dungeon.Area.Add(Ball);
							} 
						}
					};
					Bullet.NoLight = true;
					Bullet.Damage = 1;
					Bullet.Owner = Self;
					Bullet.Bad = true;
					Bullet.X = X + GetOx();
					Bullet.Y = Y + H / 2;
					Bullet.NoLight = true;
					Bullet.DissappearWithTime = true;
					Bullet.Ds = I * 0.2f + 1f;
					Pattern.AddBullet(Bullet);
				}

				BulletPattern.Fire(Pattern, Self.X + GetOx(), Self.Y + H / 2, 0, 0f);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (Pattern.Done && T >= 12f) {
					Self.Become("preattack");
				} 
			}
		}

		public static BurningKnight Instance;
		public static float LIGHT_SIZE = 12f;
		public static ShaderProgram Shader;
		public static Dialog Dialogs = Dialog.Make("burning-knight");
		public static DialogData ItsYouAgain = Dialogs.Get("its_you_again");
		public static DialogData JustDie = Dialogs.Get("just_die");
		public static DialogData NoPoint = Dialogs.Get("it_is_pointless");
		private static Animation Animations = Animation.Make("actor-burning-knight");
		private static Sound Sfx;
		public bool Dest;
		public bool AttackTp = false;
		public DialogData Dialog;
		private Room Last;
		private AnimationData Idle;
		private AnimationData Hurt;
		private AnimationData Killed;
		private AnimationData Defeated;
		private AnimationData Animation;
		private Long Sid;
		private BKSword Sword;
		private PointLight Light;
		private float Dtx;
		private float Dty;
		private int DeathDepth;
		private float LastExpl;
		private AnimationData Anim;
		private float Time;
		private float LastFrame;
		private List<Frame> Frames = new List<>();
		private float ActivityTimer;
		private int LastAttack;
		private int Pattern = -1;
		private BossPattern Pat;
		private float Dl;

		public Void FindStartPoint() {
			if (this.AttackTp) {
				float A = Random.NewFloat(0, (float) (Math.PI * 2));
				float D = IsActiveState() ? 64 : 96;
				this.Tp((float) Math.Cos(A) * D + Player.Instance.X - Player.Instance.W / 2 + this.W / 2, (float) Math.Sin(A) * D + Player.Instance.Y - Player.Instance.H / 2 + this.H / 2);

				return;
			} 

			Room Room;
			Point Center;
			int Attempts = 0;

			do {
				Room = Dungeon.Level.GetRandomRoom();
				Center = Room.GetCenter();

				if (Attempts++ > 40) {
					Log.Info("Too many");

					break;
				} 
			} while (Room is EntranceRoom);

			this.Tp(Center.X * 16 - 16, Center.Y * 16 - 16);

			if (!this.State.Equals("unactive")) {
				this.Become("idle");
			} 
		}

		public bool IsActiveState() {
			return this.ActivityTimer % 50 <= 30;
		}

		public override Void Load(FileReader Reader) {
			base.Load(Reader);

			if (Dungeon.Depth != -3) {
				this.Tp(0, 0);
			} 

			this.Rage = Reader.ReadBoolean();
			bool Def = Reader.ReadBoolean();
			DeathDepth = Reader.ReadInt16();

			if (DeathDepth != Dungeon.Depth) {
				Restore();
				DeathDepth = Dungeon.Depth;
			} else if (Def) {
				this.Rage = true;
				this.Become("defeated");
			} 
		}

		public Void Restore() {
			Log.Error("Restore bk");
			this.HpMax = 120;
			this.Hp = this.HpMax;
			this.Rage = false;
		}

		public override Void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteBoolean(this.Rage);
			Writer.WriteBoolean(this.State.Equals("defeated"));
			Writer.WriteInt16((short) DeathDepth);
		}

		public override Void Init() {
			Talked = true;
			Saw = true;
			IgnoreRooms = true;
			Sfx = Audio.GetSound("bk");
			this.Sid = Sfx.Loop(Audio.PlaySfx("bk", 0f));
			Sfx.SetVolume(this.Sid, 0);
			Instance = this;
			base.Init();
			this.T = 0;
			this.Body = this.CreateSimpleBody(8, 3, 23 - 8, 20, BodyDef.BodyType.DynamicBody, true);
			this.Become("unactive");
			Sword = new BKSword();
			Sword.SetOwner(this);
			Sword.ModifyDamage(-10);
			this.Tp(0, 0);
			Light = World.NewLight(256, new Color(1, 0f, 0f, 2f), 64, X, Y);
			SetPattern();
		}

		private Void SetPattern() {
			if (Dungeon.Level is HallLevel) {
				Pat = new BossPattern();
			} else if (Dungeon.Level is DesertLevel) {
				Pat = new DesertPattern();
			} else if (Dungeon.Level is ForestLevel) {
				Pat = new ForestBossPattern();
			} else if (Dungeon.Level is LibraryLevel) {
				Pat = new LibraryPattern();
			} else {
				Pat = new BossPattern();
			}

		}

		public override Void Destroy() {
			base.Destroy();
			Sfx.Stop(this.Sid);
			Sword.Destroy();
			World.RemoveLight(Light);
		}

		protected override Void Die(bool Force) {
			base.Die(Force);
			Instance = null;
			this.Done = true;
			GameSave.DefeatedBK = true;
			Camera.Shake(8);
			DeathEffect(Killed);
			PlayerSave.Remove(this);
			Achievements.Unlock(Achievements.REALLY_KILL_BK);
		}

		protected override List GetDrops<Item> () {
			List<Item> Items = base.GetDrops();
			Items.Add(new BKSword());

			return Items;
		}

		protected override bool CanHaveBuff(Buff Buff) {
			return !(Buff is BurningBuff || Buff is FrozenBuff);
		}

		public override float GetOx() {
			return 18.5f;
		}

		public override bool IsLow() {
			return false;
		}

		public override Void Update(float Dt) {
			Knockback.X = 0;
			Knockback.Y = 0;
			Target = Player.Instance;

			if (Target != null && Target.IsDead()) {
				Become("idle");
			} 

			this.Flipped = this.Target.X + this.Target.W / 2 < this.X + this.W / 2;

			if (this.Animation != null) {
				this.Animation.Update(Dt * SpeedMod);
			} 

			if (this.Dest) {
				this.Invt -= Dt;
				LastExpl += Dt;

				if (LastExpl >= 0.2f) {
					LastExpl = 0;
					Dungeon.Area.Add(new Explosion(this.X + this.W / 2 + Random.NewFloat(this.W * 2) - this.W, this.Y + this.H / 2 + Random.NewFloat(this.H * 2) - this.H));
					this.PlaySfx("explosion");
				} 

				FadeFx Ft = new FadeFx();
				Ft.X = X + W / 2;
				Ft.Y = Y + H / 2;
				float Fr = 120;
				float An = Random.NewFloat((float) (Math.PI * 2));
				Ft.Vel = new Point((float) Math.Cos(An) * Fr, (float) Math.Sin(An) * Fr);
				Dungeon.Area.Add(Ft);

				if (this.Invt <= 0) {
					List<Item> Items = new List<>();
					Items.Add(Chest.Generate(ItemRegistry.Quality.IRON_PLUS, Random.Chance(50)));

					if (Player.Instance != null && !Player.Instance.IsDead()) {
						for (int I = 0; I < Random.NewInt(2, 6); I++) {
							HeartFx Fx = new HeartFx();
							Fx.X = X + W / 2 + Random.NewFloat(-4, 4);
							Fx.Y = Y + H / 2 + Random.NewFloat(-4, 4);
							Dungeon.Area.Add(Fx);
							LevelSave.Add(Fx);
							Fx.RandomVelocity();
						}
					} 

					foreach (Item Item in Items) {
						ItemHolder Holder = new ItemHolder(Item);
						Holder.X = this.X;
						Holder.Y = this.Y;
						Holder.GetItem().Generate();
						this.Area.Add(Holder.Add());
						Camera.Follow(Holder, false);
						Holder.RandomVelocity();
					}

					Tween.To(new Tween.Task(0, 2f) {
						public override Void OnEnd() {
							Camera.Follow(Player.Instance, false);
						}
					});
					this.Invt = 0;
					Achievements.Unlock(Achievements.KILL_BK);
					this.Become("defeated");
					this.Dest = false;
					Projectile.AllDie = false;

					if (false) {
						Point Point = Room.GetCenter();
						Point.X *= 16;
						Point.Y *= 16;
						Point.Y -= 64;

						if (Player.Instance.GetDistanceTo(Point.X, Point.Y) < 64) {
							Point.Y += 128;
						} 

						Portal Exit = new Portal();
						Exit.X = Point.X;
						Exit.Y = Point.Y;
						Painter.Fill(Dungeon.Level, (int) Point.X / 16 - 1, (int) Point.Y / 16 - 1, 3, 3, Terrain.FLOOR_D);
						Painter.Fill(Dungeon.Level, (int) Point.X / 16 - 1, (int) Point.Y / 16 - 1, 3, 3, Terrain.DIRT);
						Dungeon.Level.Set((int) Point.X / 16, (int) Point.Y / 16, Terrain.PORTAL);

						for (int Yy = -1; Yy <= 1; Yy++) {
							for (int Xx = -1; Xx <= 1; Xx++) {
								Dungeon.Level.TileRegion((int) Point.X / 16 + Xx, (int) Point.Y / 16 + Yy);
							}
						}

						LevelSave.Add(Exit);
						Dungeon.Area.Add(Exit);
					} 

					Player.Instance.SetUnhittable(false);
					Input.Instance.Blocked = false;
					Camera.Shake(4);
					Audio.HighPriority("Reckless");
				} 

				return;
			} 

			this.ActivityTimer += Dt;
			this.Time += Dt;

			if (!this.State.Equals("defeated")) {
				base.Update(Dt);
			} 

			this.Light.SetPosition(X + this.W / 2, Y + this.H / 2);
			this.LastFrame += Dt;

			if (this.LastFrame > 0.2f) {
				this.LastFrame = 0;

				if (Frames.Size() > 5) {
					Frames.Remove(0);
				} 

				Frame P = new Frame();
				P.X = this.X;
				P.Y = this.Y;
				P.Frame = this.Animation == null ? 0 : this.Animation.GetFrame();
				P.Flipped = this.Flipped;
				Frames.Add(P);
			} 

			if (this.Freezed) {
				return;
			} 

			this.Sword.Update(Dt * SpeedMod);

			if (this.OnScreen && !this.State.Equals("defeated")) {
				float Dx = this.X + 8 - Player.Instance.X;
				float Dy = this.Y + 8 - Player.Instance.Y;
				float D = (float) Math.Sqrt(Dx * Dx + Dy * Dy);
				Sfx.SetVolume(Sid, this.State.Equals("unactive") ? 0 : Settings.Sfx * MathUtils.Clamp(0, 1, (200 - D) / 200f));
			} else {
				Sfx.SetVolume(Sid, 0);
			}


			this.T += Dt * SpeedMod;

			if (this.Dead) {
				base.Common();

				return;
			} 

			if (this.Invt > 0) {
				this.Common();

				return;
			} 

			base.Common();
		}

		public override Void Become(string State) {
			if (this.State.Equals("defeated") && !State.Equals("appear")) {
				return;
			} 

			base.Become(State);
		}

		protected override State GetAi(string State) {
			switch (State) {
				case "idle": {
					return new IdleState();
				}

				case "appear": {
					return new AppearState();
				}

				case "roam": {
					return new RoamState();
				}

				case "alerted": {
					return new AlertedState();
				}

				case "chase": 
				case "fleeing": {
					return new ChaseState();
				}

				case "dash": {
					return new DashState();
				}

				case "preattack": {
					return new PreattackState();
				}

				case "attack": {
					return new AttackState();
				}

				case "fadeIn": {
					return new FadeInState();
				}

				case "fadeOut": {
					return new FadeOutState();
				}

				case "dialog": {
					return new DialogState();
				}

				case "wait": {
					return new WaitState();
				}

				case "unactive": {
					return new UnactiveState();
				}

				case "missileAttack": {
					return new MissileAttackState();
				}

				case "autoAttack": {
					return new NewAttackState();
				}

				case "laserAttack": {
					return new LaserAttackState();
				}

				case "laserAimAttack": {
					return new LaserAimAttackState();
				}

				case "await": {
					return new AwaitState();
				}

				case "defeated": {
					return new DefeatedState();
				}

				case "spawnAttack": {
					return new SpawnAttack();
				}

				case "rangedAttack": {
					return new RangedAttackState();
				}

				case "explode": {
					return new ExplodeState();
				}

				case "tpntack": {
					return new TpntackState();
				}

				case "spin": {
					return new SpinState();
				}

				case "skull": {
					return new SkullState();
				}

				case "nano": {
					return new NanoState();
				}

				case "tear": {
					return new TearState();
				}

				case "circ": {
					return new CircState();
				}

				case "four": {
					return new FourState();
				}

				case "book": {
					return new BookState();
				}

				case "line": {
					return new LineState();
				}

				case "weird": {
					return new WeirdState();
				}

				case "cshoot": {
					return new CShootState();
				}

				case "lchase": {
					return new LChaseState();
				}

				case "ltired": {
					return new LTiredState();
				}
			}

			return base.GetAi(State);
		}

		protected override Void OnHurt(int Am, Entity From) {
			base.OnHurt(Am, From);
			this.PlaySfx("BK_hurt_" + Random.NewInt(1, 6));
		}

		public override Void Render() {
			if (this.State.Equals("unactive") || this.State.Equals("defeated")) {
				return;
			} 

			if (this.Invt > 0) {
				this.Animation = Hurt;
			} else {
				this.Animation = Anim;
			}


			float Ox = GetOx();
			Graphics.Batch.End();
			Mob.Shader.Begin();
			Mob.Shader.SetUniformf("u_color", new Vector3(1, 0.3f, 0.3f));
			Mob.Shader.SetUniformf("u_a", 0.5f);
			Mob.Shader.End();
			Graphics.Batch.SetShader(Mob.Shader);
			Graphics.Batch.Begin();
			TextureRegion Region = this.Animation.GetCurrent().Frame;
			float Dt = Gdx.Graphics.GetDeltaTime();

			foreach (Frame Point in this.Frames) {
				float S = Point.S;

				if (!Dungeon.Game.GetState().IsPaused()) {
					Point.S = Math.Max(0, Point.S - Dt * 0.8f);
				} 

				TextureRegion R = this.Idle.GetFrames().Get(Math.Min(1, Point.Frame)).Frame;
				Graphics.Render(R, Point.X + Ox, Point.Y + this.H / 2, 0, Ox, R.GetRegionHeight() / 2, false, false, Point.Flipped ? -S : S, S);
			}

			Graphics.Batch.End();
			Shader.Begin();
			Texture Texture = Region.GetTexture();
			Shader.SetUniformf("time", this.Time);
			Shader.SetUniformf("a", this.A);
			Shader.SetUniformf("white", this.Invt > 0.2f ? 1f : 0f);
			Shader.SetUniformf("pos", new Vector2(((float) Region.GetRegionX()) / Texture.GetWidth(), ((float) Region.GetRegionY()) / Texture.GetHeight()));
			Shader.SetUniformf("size", new Vector2(((float) Region.GetRegionWidth()) / Texture.GetWidth(), ((float) Region.GetRegionHeight()) / Texture.GetHeight()));
			Shader.End();
			Graphics.Batch.SetShader(Shader);
			Graphics.Batch.Begin();
			TextureRegion Reg = this.Animation.GetCurrent().Frame;
			Graphics.Render(Reg, this.X + Ox, this.Y + this.H / 2, 0, Ox, Reg.GetRegionHeight() / 2, false, false, this.Flipped ? -1 : 1, 1);
			Graphics.Batch.End();
			Graphics.Batch.SetShader(null);
			Graphics.Batch.Begin();
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		public override Void RenderShadow() {

		}

		public override Void Die() {
			this.Dead = false;
			this.DeathDepth = Dungeon.Depth;
			this.Done = false;
			this.Hp = 1;
			this.Rage = true;
			this.Unhittable = true;
			this.IgnoreRooms = true;

			foreach (Mob Mob in Mob.All) {
				if (Mob.Room == Player.Instance.Room) {
					Mob.Die();
				} 
			}

			Projectile.AllDie = true;
			Dtx = X;
			Dty = Y;
			Player.Instance.SetUnhittable(true);
			Input.Instance.Blocked = true;
			Camera.Follow(this, false);

			if (Dest) {
				return;
			} 

			Camera.Shake(6);
			this.Invt = 3f;
			this.Dest = true;
			Audio.Stop();
			Tween.To(new Tween.Task(1, 1f) {
				public override float GetValue() {
					return Dungeon.White;
				}

				public override Void SetValue(float Value) {
					Dungeon.White = Value;
				}

				public override Void OnEnd() {
					Tween.To(new Tween.Task(0, 0.1f) {
						public override float GetValue() {
							return Dungeon.White;
						}

						public override Void SetValue(float Value) {
							Dungeon.White = Value;
						}

						public override Void OnEnd() {
							Vector3 Vec = Camera.Game.Project(new Vector3(Dtx + W / 2, Dty + H / 2, 0));
							Vec = Camera.Ui.Unproject(Vec);
							Vec.Y = Display.UI_HEIGHT - Vec.Y;
							Dungeon.ShockTime = 0;
							Dungeon.ShockPos.X = (Vec.X) / Display.UI_WIDTH;
							Dungeon.ShockPos.Y = (Vec.Y) / Display.UI_HEIGHT;

							for (int I = 0; I < 30; I++) {
								CurseFx Fx = new CurseFx();
								Fx.X = Dtx + W / 2 + Random.NewFloat(-W, W);
								Fx.Y = Dty + H / 2 + Random.NewFloat(-H, H);
								Dungeon.Area.Add(Fx);
							}

							PlaySfx("explosion");
						}
					});
				}
			}).Delay(2f);
		}

		public override Void KnockBackFrom(Entity From, float Force) {

		}

		private Void CheckForTarget() {
			foreach (Creature Player in (this.Stupid ? Mob.All : Player.All)) {
				if (Player.Invisible || Player == this) {
					continue;
				} 

				if (Player is Player && this.State.Equals("wait") && ((Player) Player).Room != this.Room) {
					continue;
				} 

				float Dx = Player.X - this.X - 8;
				float Dy = Player.Y - this.Y - 8;
				float D = (float) Math.Sqrt(Dx * Dx + Dy * Dy);

				if (D < (LIGHT_SIZE + 3) * 16) {
					this.Target = Player;
					this.Become("alerted");
					this.NoticeSignT = 2f;
					this.HideSignT = 0f;

					return;
				} 
			}
		}

		private Void TpFromPlayer() {
			for (int I = 0; I < 50; I++) {
				Point Point = Room.GetRandomFreeCell();

				if (Point != null) {
					Point.X *= 16;
					Point.Y *= 16;

					if (Player.Instance == null || Player.Instance.GetDistanceTo(Point.X, Point.Y) > 32) {
						Tp(Point.X, Point.Y);

						return;
					} 
				} 
			}

			Log.Error("Too many attempts");
		}

		public BulletProjectile NewProjectile() {
			BulletProjectile Bullet = new NanoBullet() {
				protected override Void OnDeath() {
					base.OnDeath();

					if (!BrokeWeapon) {
						BulletProjectile Ball = new NanoBullet();
						bool Fast = I % 2 == 0;
						float A = GetAngleTo(Player.Instance.X + 8, Player.Instance.Y + 8) + Random.NewFloat(-0.1f, 0.1f);
						Ball.Velocity = new Point((float) Math.Cos(A), (float) Math.Sin(A)).Mul((Fast ? 2 : 1) * 40 * Mob.ShotSpeedMod);
						Ball.X = (this.X);
						Ball.Y = (this.Y);
						Ball.Damage = 2;
						Ball.Bad = true;
						Ball.NoLight = true;
						Dungeon.Area.Add(Ball);
					} 
				}
			};
			Bullet.Damage = 1;
			Bullet.Owner = this;
			Bullet.Bad = true;
			Bullet.DissappearWithTime = true;
			Bullet.Ds = 2f;
			Bullet.NoLight = true;
			float A = 0;
			Bullet.X = X;
			Bullet.Y = Y;
			Bullet.Velocity.X = (float) (Math.Cos(A));
			Bullet.Velocity.Y = (float) (Math.Sin(A));

			return Bullet;
		}

		public BurningKnight() {
			_Init();
		}
	}
}
