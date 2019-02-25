using BurningKnight.entity.creature.player;
using BurningKnight.physics;
using BurningKnight.util;

namespace BurningKnight.entity.creature.mob.ice {
	public class Gift : Mob {
		public static Animation Animations = Animation.Make("actor-gift", "-normal");
		private AnimationData Animation;
		private AnimationData Hurt;
		private AnimationData Idle;
		private AnimationData Killed;

		public Gift() {
			_Init();
		}

		protected void _Init() {
			{
				HpMax = 8;
				Idle = GetAnimation().Get("idle");
				Hurt = GetAnimation().Get("hurt");
				Killed = GetAnimation().Get("dead");
				Animation = Idle;
			}
		}

		public Animation GetAnimation() {
			return Animations;
		}

		public override void Init() {
			base.Init();
			Body = CreateSimpleBody(0, 0, 16, 16, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
		}

		public override void Destroy() {
			base.Destroy();
			Body = World.RemoveBody(Body);
		}

		public override void Render() {
			if (Dead)
				Animation = Killed;
			else if (Invt > 0)
				Animation = Hurt;
			else
				Animation = Idle;


			this.RenderWithOutline(Animation);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			Animation.Update(Dt);
			Common();
		}

		protected override void DeathEffects() {
			base.DeathEffects();
			PlaySfx("death_clown");
			DeathEffect(Killed);

			for (var I = 0; I < Random.NewInt(4, 8); I++) {
				Mob Mob = Random.Chance(25) ? (Random.Chance(50) ? new SnowballFly() : new Snowball()) : (Random.Chance(60) ? new Snowflake() : new Roller());
				Mob.X = this.X + Random.NewFloat(16);
				Mob.Y = this.Y + Random.NewFloat(16);
				Dungeon.Area.Add(Mob.Add());
			}
		}

		protected override void OnHurt(int A, Entity Creature) {
			base.OnHurt(A, Creature);
			PlaySfx("damage_clown");
		}

		protected override Mob.State GetAi(string State) {
			return new IdleState();
		}

		public class IdleState : State<Gift> {
			public override void Update(float Dt) {
				base.Update(Dt);

				if (Self.Dd) return;

				if (Player.Instance.Room == Self.Room && Self.GetDistanceTo(Player.Instance.X + 8, Player.Instance.Y + 8) < 32) Self.Die();
			}
		}
	}
}