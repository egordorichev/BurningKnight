using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item;
using BurningKnight.core.entity.item.weapon.gun;
using BurningKnight.core.entity.level.entities.chest;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.creature.mob.desert {
	public class Archeologist : Mob {
		protected void _Init() {
			{
				HpMax = 6;
				Idle = GetAnimation().Get("idle").Randomize();
				Run = GetAnimation().Get("run").Randomize();
				Hurt = GetAnimation().Get("hurt").Randomize();
				Killed = GetAnimation().Get("death").Randomize();
				Animation = this.Idle;
				Speed = 100;
				MaxSpeed = 100;
			}

			{
				AlwaysRender = true;
			}
		}

		public class ArcheologistState : Mob.State<Archeologist>  {

		}

		public class IdleState : ArcheologistState {
			public override Void Update(float Dt) {
				base.Update(Dt);
				this.CheckForPlayer();

				if (Self.Target != null) {
					Self.Become("alerted");
				} 
			}
		}

		public class ChaseState : ArcheologistState {
			public override Void Update(float Dt) {
				this.CheckForPlayer();

				if (Self.LastSeen == null) {
					Self.Become("idle");

					return;
				} else {
					float Att = 180;

					if (this.MoveTo(Self.LastSeen, 18f, Att) && Self.OnScreen) {
						if (Self.Target != null && Self.GetDistanceTo((int) (Self.Target.X + Self.Target.W / 2), (int) (Self.Target.Y + Self.Target.H / 2)) <= Att) {
							if (Self.CanSee(Self.Target)) {
								Self.Become("preattack");
							} 
						} else {
							Self.NoticeSignT = 0f;
							Self.HideSignT = 2f;
							Self.Become("idle");
						}

					} 
				}


				base.Update(Dt);
			}
		}

		public class RunAwayState : ArcheologistState {
			public override Void OnEnter() {
				base.OnEnter();
				Self.LastSeen = new Point(Self.Target.X, Self.Target.Y);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);
				this.CheckForPlayer();

				if (Self.LastSeen == null) {
					Self.Become("idle");

					return;
				} 

				this.MoveFrom(Self.LastSeen, 20f, 6f);
				float D = Self.GetDistanceTo(Self.Target.X, Self.Target.Y);

				if (D >= 128) {
					Self.Become("preattack");
				} 
			}
		}

		public class AttackingState : ArcheologistState {
			public override Void OnEnter() {
				base.OnEnter();
			}

			public override Void Update(float Dt) {
				if (!((Gun) Self.Weapon).IsReloading() && !(((Gun) Self.Weapon).ShowRedLine)) {
					if (!CanSee(Self.Target) || Self.GetDistanceTo(Self.Target.X + Self.Target.W / 2, Self.Target.Y + Self.Target.H / 2) > 220f) {
						Self.Become("chase");

						return;
					} 

					Self.Weapon.Use();
					Self.Become("preattack");
					CheckForRun();
				} 
			}
		}

		public class PreAttackState : ArcheologistState {
			public override Void Update(float Dt) {
				base.Update(Dt);
				float Dx = Self.Target.X + Self.Target.W / 2 - LastAim.X;
				float Dy = Self.Target.Y + Self.Target.H / 2 - LastAim.Y;
				float S = 0.08f;
				LastAim.X += Dx * S;
				LastAim.Y += Dy * S;

				if (this.T > 1f) {
					Self.Become("attack");
				} 

				CheckForRun();
			}
		}

		public static Animation Animations = Animation.Make("actor-archeologist", "-green");
		private AnimationData Idle;
		private AnimationData Run;
		private AnimationData Hurt;
		private AnimationData Killed;
		private AnimationData Animation;
		private Item Weapon;
		private Point LastAim = new Point();
		public bool Bombs;

		public Animation GetAnimation() {
			return Animations;
		}

		protected override Void OnHurt(int A, Entity Creature) {
			base.OnHurt(A, Creature);
			this.PlaySfx("damage_clown");
		}

		protected override State GetAi(string State) {
			switch (State) {
				case "preattack": {
					return new PreAttackState();
				}

				case "attack": {
					return new AttackingState();
				}

				case "runaway": 
				case "alerted": {
					return new RunAwayState();
				}

				case "roam": 
				case "idle": {
					return new IdleState();
				}

				case "chase": {
					return new ChaseState();
				}
			}

			return base.GetAi(State);
		}

		public override bool RollBlock() {
			return false;
		}

		public Void CheckForRun() {
			if (((Gun) this.Weapon).IsReloading()) {
				return;
			} 

			if (this.Target == null) {
				return;
			} 

			float D = this.GetDistanceTo(this.Target.X + this.Target.W / 2, this.Target.Y + this.Target.H / 2);

			if (D < 64f) {
				this.Become("runaway");
			} 
		}

		public override Void Tp(float X, float Y) {
			base.Tp(X, Y);
			LastAim.X = X + 10;
			LastAim.Y = Y;
		}

		public override Point GetAim() {
			return LastAim;
		}

		public override Void Init() {
			base.Init();
			this.Body = this.CreateSimpleBody(2, 1, 12, 12, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
			this.Weapon = new SnipperGun();
			this.Weapon.SetOwner(this);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (this.Freezed) {
				return;
			} 

			if (Math.Abs(this.Velocity.X) > 1f) {
				this.Flipped = this.Velocity.X < 0;
			} 

			if (this.Dead) {
				base.Common();

				return;
			} 

			if (this.Animation != null) {
				this.Animation.Update(Dt * SpeedMod);
			} 

			this.Weapon.Update(Dt * SpeedMod);
			base.Common();
		}

		public override Void Render() {
			float V = Math.Abs(this.Acceleration.X) + Math.Abs(this.Acceleration.Y);

			if (this.Dead) {
				this.Animation = Killed;
			} else if (this.Invt > 0) {
				this.Animation = Hurt;
			} else if (V > 1f) {
				this.Animation = Run;
			} else {
				this.Animation = Idle;
			}


			this.RenderWithOutline(this.Animation);
			this.Weapon.Render(this.X, this.Y, this.W, this.H, this.Flipped, false);
			base.RenderStats();
		}

		public override Void Destroy() {
			base.Destroy();

			if (this.Weapon != null) {
				this.Weapon.Destroy();
			} 
		}

		protected override Void DeathEffects() {
			base.DeathEffects();
			this.PlaySfx("death_clown");
			this.Done = true;
			DeathEffect(Killed);
			this.PlaySfx("explosion");
			this.Done = true;
			Explosion.Make(this.X + 8, this.Y + 8, false);

			for (int I = 0; I < Dungeon.Area.GetEntities().Size(); I++) {
				Entity Entity = Dungeon.Area.GetEntities().Get(I);

				if (Entity is Creature) {
					Creature Creature = (Creature) Entity;

					if (Creature.GetDistanceTo(this.X + 8, this.Y + 8) < 24f) {
						if (!Creature.ExplosionBlock) {
							if (Creature is Player) {
								Creature.ModifyHp(-1000, this, true);
							} else {
								Creature.ModifyHp(-Math.Round(Random.NewFloatDice(20 / 3 * 2, 20)), this, true);
							}

						} 

						float A = (float) Math.Atan2(Creature.Y + Creature.H / 2 - this.Y - 8, Creature.X + Creature.W / 2 - this.X - 8);
						float KnockbackMod = Creature.KnockbackMod;
						Creature.Velocity.X += Math.Cos(A) * 5000f * KnockbackMod;
						Creature.Velocity.Y += Math.Sin(A) * 5000f * KnockbackMod;
					} 
				} else if (Entity is Chest) {
					if (Entity.GetDistanceTo(this.X + 8, this.Y + 8) < 24f) {
						((Chest) Entity).Explode();
					} 
				} 
			}
		}

		protected override List GetDrops<Item> () {
			List<Item> Items = base.GetDrops();

			if (Random.Chance(5)) {
				Items.Add(new Shotgun());
			} 

			return Items;
		}

		public Archeologist() {
			_Init();
		}
	}
}
