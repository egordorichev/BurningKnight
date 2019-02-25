using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item;
using BurningKnight.core.entity.item.weapon.dagger;
using BurningKnight.core.entity.level;
using BurningKnight.core.entity.level.entities;
using BurningKnight.core.entity.trap;
using BurningKnight.core.physics;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.mob.hall {
	public class Thief : Mob {
		protected void _Init() {
			{
				HpMax = 5;
				W = 15;
				Idle = GetAnimation().Get("idle").Randomize();
				Run = GetAnimation().Get("run").Randomize();
				Hurt = GetAnimation().Get("hurt").Randomize();
				Killed = GetAnimation().Get("death").Randomize();
				Animation = this.Idle;
			}
		}

		public class ThiefState : Mob.State<Thief>  {

		}

		public class AttackState : ThiefState {
			public override Void Update(float Dt) {
				base.Update(Dt);
				float F = Math.Min(40f, T * 40f);
				Acceleration.X = Direction.X * F;
				Acceleration.Y = Direction.Y * F;
			}
		}

		public class IdleState : ThiefState {
			public override Void Update(float Dt) {
				this.CheckForPlayer();

				if (Self.Target != null && Self.Target.Room == Self.Room) {
					float Dx = Self.Target.X + Self.Target.W / 2 - Self.X - Self.W / 2;
					float Dy = Self.Target.Y + Self.Target.H / 2 - Self.Y - Self.H / 2;
					float D = 16;

					if (Math.Abs(Dy) < D || Math.Abs(Dx) < D) {
						if (Math.Abs(Dx) > Math.Abs(Dy)) {
							Direction.X = Dx < 0 ? -1 : 1;
							Direction.Y = 0;
						} else {
							Direction.Y = Dy < 0 ? -1 : 1;
							Direction.X = 0;
						}


						Self.Become("attack");
					} 
				} 
			}
		}

		public static Animation Animations = Animation.Make("actor-thief", "-purple");
		protected Item Sword;
		private AnimationData Idle;
		private AnimationData Run;
		private AnimationData Hurt;
		private AnimationData Killed;
		private AnimationData Animation;
		public Vector2 Direction = new Vector2();

		public Animation GetAnimation() {
			return Animations;
		}

		public override Void RenderShadow() {
			Graphics.Shadow(this.X, this.Y, this.W, this.H, 0, this.A);
		}

		public override Void Init() {
			base.Init();
			this.Sword = new Dagger();
			this.Sword.SetOwner(this);
			this.Body = this.CreateSimpleBody(2, 1, 12, 12, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
			Speed = 150;
			MaxSpeed = 150;
		}

		public override Void Render() {
			if (Math.Abs(this.Velocity.X) > 1f) {
				this.Flipped = this.Velocity.X < 0;
			} else if (Target != null) {
				Flipped = Target.X < X;
			} 

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
			this.Sword.Render(this.X, this.Y, this.W, this.H, this.Flipped, false);
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

		public override Void OnCollision(Entity Entity) {
			base.OnCollision(Entity);

			if (State.Equals("attack") && (Entity == null || Entity is Level || Entity is RollingSpike || Entity is SolidProp || Entity is Door)) {
				Become("idle");
			} 
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity == null || Entity is Level || Entity is RollingSpike || Entity is SolidProp || Entity is Door) {
				return true;
			} 

			return base.ShouldCollide(Entity, Contact, Fixture);
		}

		protected override List GetDrops<Item> () {
			List<Item> Items = base.GetDrops();

			if (Random.Chance(5)) {
				Items.Add(new Dagger());
			} 

			return Items;
		}

		protected override Void OnHurt(int A, Entity Creature) {
			base.OnHurt(A, Creature);
			this.PlaySfx("damage_thief");
		}

		public override Void Destroy() {
			base.Destroy();
			this.Sword.Destroy();
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (this.Freezed) {
				return;
			} 

			if (this.Dead) {
				base.Common();

				return;
			} 

			if (this.Animation != null) {
				this.Animation.Update(Dt * SpeedMod);
			} 

			this.Sword.Update(Dt * SpeedMod);
			base.Common();
		}

		protected override Void DeathEffects() {
			base.DeathEffects();
			this.PlaySfx("death_thief");
			DeathEffect(Killed);
		}

		public override Void OnHit(Creature Who) {
			base.OnHit(Who);

			if (Who is Player) {
				Player Player = (Player) Who;

				if (Player.GetMoney() > 0) {
					int Amount = Random.NewInt(Math.Min(Player.GetMoney(), 5), Math.Min(Player.GetMoney(), 10));
					Player.SetMoney(Player.GetMoney() - Amount);

					for (int I = 0; I < Amount; I++) {
						ItemHolder Holder = new ItemHolder();
						Holder.X = this.X + W / 2 + Random.NewFloat(-4, 4);
						Holder.Y = this.Y + H / 2 + Random.NewFloat(-4, 4);
						Holder.SetItem(new Gold());
						Dungeon.Area.Add(Holder.Add());
						Holder.RandomVelocity();
					}
				} 
			} 
		}

		public Thief() {
			_Init();
		}
	}
}
