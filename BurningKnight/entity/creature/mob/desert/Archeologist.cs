using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.entity.item.weapon.gun;
using BurningKnight.entity.level.entities.chest;
using BurningKnight.physics;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.creature.mob.desert {
	public class Archeologist : Mob {
		public static Animation Animations = Animation.Make("actor-archeologist", "-green");
		private AnimationData Animation;
		public bool Bombs;
		private AnimationData Hurt;
		private AnimationData Idle;
		private AnimationData Killed;
		private Point LastAim = new Point();
		private AnimationData Run;
		private Item Weapon;

		public Archeologist() {
			_Init();
		}

		protected void _Init() {
			{
				HpMax = 6;
				Idle = GetAnimation().Get("idle").Randomize();
				Run = GetAnimation().Get("run").Randomize();
				Hurt = GetAnimation().Get("hurt").Randomize();
				Killed = GetAnimation().Get("death").Randomize();
				Animation = Idle;
				Speed = 100;
				MaxSpeed = 100;
			}

			{
				AlwaysRender = true;
			}
		}

		public Animation GetAnimation() {
			return Animations;
		}

		protected override void OnHurt(int A, Entity Creature) {
			base.OnHurt(A, Creature);
			PlaySfx("damage_clown");
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

		public void CheckForRun() {
			if (((Gun) Weapon).IsReloading()) return;

			if (Target == null) return;

			var D = GetDistanceTo(Target.X + Target.W / 2, Target.Y + Target.H / 2);

			if (D < 64f) Become("runaway");
		}

		public override void Tp(float X, float Y) {
			base.Tp(X, Y);
			LastAim.X = X + 10;
			LastAim.Y = Y;
		}

		public override Point GetAim() {
			return LastAim;
		}

		public override void Init() {
			base.Init();
			Body = CreateSimpleBody(2, 1, 12, 12, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
			Weapon = new SnipperGun();
			Weapon.SetOwner(this);
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (Freezed) return;

			if (Math.Abs(Velocity.X) > 1f) Flipped = Velocity.X < 0;

			if (Dead) {
				Common();

				return;
			}

			if (Animation != null) Animation.Update(Dt * SpeedMod);

			Weapon.Update(Dt * SpeedMod);
			Common();
		}

		public override void Render() {
			float V = Math.Abs(Acceleration.X) + Math.Abs(Acceleration.Y);

			if (Dead)
				Animation = Killed;
			else if (Invt > 0)
				Animation = Hurt;
			else if (V > 1f)
				Animation = Run;
			else
				Animation = Idle;


			this.RenderWithOutline(Animation);
			Weapon.Render(this.X, this.Y, W, H, Flipped, false);
			base.RenderStats();
		}

		public override void Destroy() {
			base.Destroy();

			if (Weapon != null) Weapon.Destroy();
		}

		protected override void DeathEffects() {
			base.DeathEffects();
			PlaySfx("death_clown");
			Done = true;
			DeathEffect(Killed);
			PlaySfx("explosion");
			Done = true;
			Explosion.Make(this.X + 8, this.Y + 8, false);

			for (var I = 0; I < Dungeon.Area.GetEntities().Size(); I++) {
				Entity Entity = Dungeon.Area.GetEntities().Get(I);

				if (Entity is Creature) {
					var Creature = (Creature) Entity;

					if (Creature.GetDistanceTo(this.X + 8, this.Y + 8) < 24f) {
						if (!Creature.ExplosionBlock) {
							if (Creature is Player)
								Creature.ModifyHp(-1000, this, true);
							else
								Creature.ModifyHp(-Math.Round(Random.NewFloatDice(20 / 3 * 2, 20)), this, true);
						}

						var A = (float) Math.Atan2(Creature.Y + Creature.H / 2 - this.Y - 8, Creature.X + Creature.W / 2 - this.X - 8);
						var KnockbackMod = Creature.KnockbackMod;
						Creature.Velocity.X += Math.Cos(A) * 5000f * KnockbackMod;
						Creature.Velocity.Y += Math.Sin(A) * 5000f * KnockbackMod;
					}
				}
				else if (Entity is Chest) {
					if (Entity.GetDistanceTo(this.X + 8, this.Y + 8) < 24f) ((Chest) Entity).Explode();
				}
			}
		}

		protected override List GetDrops<Item>() {
			List<Item> Items = base.GetDrops();

			if (Random.Chance(5)) Items.Add(new Shotgun());

			return Items;
		}

		public class ArcheologistState : State<Archeologist> {
		}

		public class IdleState : ArcheologistState {
			public override void Update(float Dt) {
				base.Update(Dt);
				CheckForPlayer();

				if (Self.Target != null) Self.Become("alerted");
			}
		}

		public class ChaseState : ArcheologistState {
			public override void Update(float Dt) {
				CheckForPlayer();

				if (Self.LastSeen == null) {
					Self.Become("idle");

					return;
				}

				float Att = 180;

				if (MoveTo(Self.LastSeen, 18f, Att) && Self.OnScreen) {
					if (Self.Target != null && Self.GetDistanceTo((int) (Self.Target.X + Self.Target.W / 2), (int) (Self.Target.Y + Self.Target.H / 2)) <= Att) {
						if (Self.CanSee(Self.Target)) Self.Become("preattack");
					}
					else {
						Self.NoticeSignT = 0f;
						Self.HideSignT = 2f;
						Self.Become("idle");
					}
				}


				base.Update(Dt);
			}
		}

		public class RunAwayState : ArcheologistState {
			public override void OnEnter() {
				base.OnEnter();
				Self.LastSeen = new Point(Self.Target.X, Self.Target.Y);
			}

			public override void Update(float Dt) {
				base.Update(Dt);
				CheckForPlayer();

				if (Self.LastSeen == null) {
					Self.Become("idle");

					return;
				}

				MoveFrom(Self.LastSeen, 20f, 6f);
				var D = Self.GetDistanceTo(Self.Target.X, Self.Target.Y);

				if (D >= 128) Self.Become("preattack");
			}
		}

		public class AttackingState : ArcheologistState {
			public override void OnEnter() {
				base.OnEnter();
			}

			public override void Update(float Dt) {
				if (!((Gun) Self.Weapon).IsReloading() && !((Gun) Self.Weapon).ShowRedLine) {
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
			public override void Update(float Dt) {
				base.Update(Dt);
				var Dx = Self.Target.X + Self.Target.W / 2 - LastAim.X;
				var Dy = Self.Target.Y + Self.Target.H / 2 - LastAim.Y;
				var S = 0.08f;
				LastAim.X += Dx * S;
				LastAim.Y += Dy * S;

				if (T > 1f) Self.Become("attack");

				CheckForRun();
			}
		}
	}
}