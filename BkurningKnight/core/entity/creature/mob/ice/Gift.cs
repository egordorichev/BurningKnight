using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.physics;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.mob.ice {
	public class Gift : Mob {
		protected void _Init() {
			{
				HpMax = 8;
				Idle = GetAnimation().Get("idle");
				Hurt = GetAnimation().Get("hurt");
				Killed = GetAnimation().Get("dead");
				Animation = Idle;
			}
		}

		public class IdleState : Mob.State<Gift>  {
			public override Void Update(float Dt) {
				base.Update(Dt);

				if (Self.Dd) {
					return;
				} 

				if (Player.Instance.Room == Self.Room && Self.GetDistanceTo(Player.Instance.X + 8, Player.Instance.Y + 8) < 32) {
					Self.Die();
				} 
			}
		}

		public static Animation Animations = Animation.Make("actor-gift", "-normal");
		private AnimationData Idle;
		private AnimationData Killed;
		private AnimationData Hurt;
		private AnimationData Animation;

		public Animation GetAnimation() {
			return Animations;
		}

		public override Void Init() {
			base.Init();
			this.Body = this.CreateSimpleBody(0, 0, 16, 16, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
		}

		public override Void Destroy() {
			base.Destroy();
			this.Body = World.RemoveBody(this.Body);
		}

		public override Void Render() {
			if (this.Dead) {
				this.Animation = Killed;
			} else if (this.Invt > 0) {
				this.Animation = Hurt;
			} else {
				this.Animation = Idle;
			}


			this.RenderWithOutline(this.Animation);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			Animation.Update(Dt);
			base.Common();
		}

		protected override Void DeathEffects() {
			base.DeathEffects();
			this.PlaySfx("death_clown");
			DeathEffect(Killed);

			for (int I = 0; I < Random.NewInt(4, 8); I++) {
				Mob Mob = Random.Chance(25) ? (Random.Chance(50) ? new SnowballFly() : new Snowball()) : (Random.Chance(60) ? new Snowflake() : new Roller());
				Mob.X = this.X + Random.NewFloat(16);
				Mob.Y = this.Y + Random.NewFloat(16);
				Dungeon.Area.Add(Mob.Add());
			}
		}

		protected override Void OnHurt(int A, Entity Creature) {
			base.OnHurt(A, Creature);
			this.PlaySfx("damage_clown");
		}

		protected override Mob.State GetAi(string State) {
			return new IdleState();
		}

		public Gift() {
			_Init();
		}
	}
}
