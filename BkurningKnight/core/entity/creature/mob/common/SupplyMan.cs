using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item.key;
using BurningKnight.core.physics;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.mob.common {
	public class SupplyMan : Mob {
		protected void _Init() {
			{
				HpMax = 2;
				W = 10;
				Run = GetAnimation().Get("run");
				Idle = GetAnimation().Get("idle");
				Killed = GetAnimation().Get("dead");
				Hurt = GetAnimation().Get("hurt");
				Animation = Run;
			}
		}

		public class SupplyState : Mob.State<SupplyMan>  {

		}

		public class IdleState : SupplyState {
			public override Void Update(float Dt) {
				base.Update(Dt);

				if (Self.Target != null && Self.OnScreen && Self.CanSee(Self.Target)) {
					Self.Become("run");
					Self.NoticeSignT = 2f;
					Self.PlaySfx("enemy_alert");
				} 
			}
		}

		public class RunState : SupplyState {
			public override Void Update(float Dt) {
				base.Update(Dt);
				MoveFrom(Player.Instance, 20f, 512f);

				if (!Self.OnScreen || Self.GetDistanceTo(Self.Target.X + 8, Self.Target.Y + 8) > 256f) {
					Self.Poof();
					Player.Instance.PlaySfx("head_explode");
					Self.Done = true;
				} 
			}
		}

		public static Animation Animations = Animation.Make("actor-supply", "-key");
		private AnimationData Run;
		private AnimationData Idle;
		private AnimationData Hurt;
		private AnimationData Killed;
		private AnimationData Animation;

		public Animation GetAnimation() {
			return Animations;
		}

		public override Void Init() {
			base.Init();
			this.Body = this.CreateSimpleBody(0, 0, 10, 14, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
		}

		public override Void Destroy() {
			base.Destroy();
			this.Body = World.RemoveBody(this.Body);
		}

		public override Void Render() {
			if (Math.Abs(this.Velocity.X) > 1f) {
				this.Flipped = this.Velocity.X < 0;
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
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		public override Void RenderShadow() {
			Graphics.Shadow(X, Y + 4, W, H, 0);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (!Freezed) {
				Animation.Update(Dt);
			} 

			base.Common();
		}

		protected override Void DeathEffects() {
			base.DeathEffects();
			this.PlaySfx("death_clown");
			DeathEffect(Killed);
		}

		protected override Void OnHurt(int A, Entity Creature) {
			base.OnHurt(A, Creature);
			this.PlaySfx("damage_clown");
		}

		protected override List GetDrops<Item> () {
			List<Item> Items = new List<>();
			Items.Add(new KeyC());

			return Items;
		}

		protected override State GetAi(string State) {
			switch (State) {
				case "idle": 
				case "roam": 
				case "alerted": {
					return new IdleState();
				}

				case "run": {
					return new RunState();
				}
			}

			return base.GetAi(State);
		}

		public SupplyMan() {
			_Init();
		}
	}
}
