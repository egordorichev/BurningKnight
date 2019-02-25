using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.entity.item.weapon.dagger;
using BurningKnight.entity.level;
using BurningKnight.entity.level.entities;
using BurningKnight.entity.trap;
using BurningKnight.physics;
using BurningKnight.util;

namespace BurningKnight.entity.creature.mob.hall {
	public class Thief : Mob {
		public static Animation Animations = Animation.Make("actor-thief", "-purple");
		private AnimationData Animation;
		public Vector2 Direction = new Vector2();
		private AnimationData Hurt;
		private AnimationData Idle;
		private AnimationData Killed;
		private AnimationData Run;
		protected Item Sword;

		public Thief() {
			_Init();
		}

		protected void _Init() {
			{
				HpMax = 5;
				W = 15;
				Idle = GetAnimation().Get("idle").Randomize();
				Run = GetAnimation().Get("run").Randomize();
				Hurt = GetAnimation().Get("hurt").Randomize();
				Killed = GetAnimation().Get("death").Randomize();
				Animation = Idle;
			}
		}

		public Animation GetAnimation() {
			return Animations;
		}

		public override void RenderShadow() {
			Graphics.Shadow(this.X, this.Y, W, H, 0, A);
		}

		public override void Init() {
			base.Init();
			Sword = new Dagger();
			Sword.SetOwner(this);
			Body = CreateSimpleBody(2, 1, 12, 12, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
			Speed = 150;
			MaxSpeed = 150;
		}

		public override void Render() {
			if (Math.Abs(Velocity.X) > 1f)
				Flipped = Velocity.X < 0;
			else if (Target != null) Flipped = Target.X < X;

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
			Sword.Render(this.X, this.Y, W, H, Flipped, false);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		protected override State GetAi(string State) {
			switch (State) {
				case "idle":
				case "roam":
				case "alerted": {
					return new IdleState();
				}

				case "attack": {
					return new AttackState();
				}
			}

			return base.GetAi(State);
		}

		public override void OnCollision(Entity Entity) {
			base.OnCollision(Entity);

			if (State.Equals("attack") && (Entity == null || Entity is Level || Entity is RollingSpike || Entity is SolidProp || Entity is Door)) Become("idle");
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity == null || Entity is Level || Entity is RollingSpike || Entity is SolidProp || Entity is Door) return true;

			return base.ShouldCollide(Entity, Contact, Fixture);
		}

		protected override List GetDrops<Item>() {
			List<Item> Items = base.GetDrops();

			if (Random.Chance(5)) Items.Add(new Dagger());

			return Items;
		}

		protected override void OnHurt(int A, Entity Creature) {
			base.OnHurt(A, Creature);
			PlaySfx("damage_thief");
		}

		public override void Destroy() {
			base.Destroy();
			Sword.Destroy();
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (Freezed) return;

			if (Dead) {
				Common();

				return;
			}

			if (Animation != null) Animation.Update(Dt * SpeedMod);

			Sword.Update(Dt * SpeedMod);
			Common();
		}

		protected override void DeathEffects() {
			base.DeathEffects();
			PlaySfx("death_thief");
			DeathEffect(Killed);
		}

		public override void OnHit(Creature Who) {
			base.OnHit(Who);

			if (Who is Player) {
				var Player = (Player) Who;

				if (Player.GetMoney() > 0) {
					var Amount = Random.NewInt(Math.Min(Player.GetMoney(), 5), Math.Min(Player.GetMoney(), 10));
					Player.SetMoney(Player.GetMoney() - Amount);

					for (var I = 0; I < Amount; I++) {
						var Holder = new ItemHolder();
						Holder.X = this.X + W / 2 + Random.NewFloat(-4, 4);
						Holder.Y = this.Y + H / 2 + Random.NewFloat(-4, 4);
						Holder.SetItem(new Gold());
						Dungeon.Area.Add(Holder.Add());
						Holder.RandomVelocity();
					}
				}
			}
		}

		public class ThiefState : State<Thief> {
		}

		public class AttackState : ThiefState {
			public override void Update(float Dt) {
				base.Update(Dt);
				float F = Math.Min(40f, T * 40f);
				Acceleration.X = Direction.X * F;
				Acceleration.Y = Direction.Y * F;
			}
		}

		public class IdleState : ThiefState {
			public override void Update(float Dt) {
				CheckForPlayer();

				if (Self.Target != null && Self.Target.Room == Self.Room) {
					var Dx = Self.Target.X + Self.Target.W / 2 - Self.X - Self.W / 2;
					var Dy = Self.Target.Y + Self.Target.H / 2 - Self.Y - Self.H / 2;
					float D = 16;

					if (Math.Abs(Dy) < D || Math.Abs(Dx) < D) {
						if (Math.Abs(Dx) > Math.Abs(Dy)) {
							Direction.X = Dx < 0 ? -1 : 1;
							Direction.Y = 0;
						}
						else {
							Direction.Y = Dy < 0 ? -1 : 1;
							Direction.X = 0;
						}


						Self.Become("attack");
					}
				}
			}
		}
	}
}