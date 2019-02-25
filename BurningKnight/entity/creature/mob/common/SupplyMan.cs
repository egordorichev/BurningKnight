using BurningKnight.entity.creature.player;
using BurningKnight.entity.item.key;
using BurningKnight.physics;
using BurningKnight.util;

namespace BurningKnight.entity.creature.mob.common {
	public class SupplyMan : Mob {
		public static Animation Animations = Animation.Make("actor-supply", "-key");
		private AnimationData Animation;
		private AnimationData Hurt;
		private AnimationData Idle;
		private AnimationData Killed;
		private AnimationData Run;

		public SupplyMan() {
			_Init();
		}

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

		public Animation GetAnimation() {
			return Animations;
		}

		public override void Init() {
			base.Init();
			Body = CreateSimpleBody(0, 0, 10, 14, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
		}

		public override void Destroy() {
			base.Destroy();
			Body = World.RemoveBody(Body);
		}

		public override void Render() {
			if (Math.Abs(Velocity.X) > 1f) Flipped = Velocity.X < 0;

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
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		public override void RenderShadow() {
			Graphics.Shadow(X, Y + 4, W, H, 0);
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (!Freezed) Animation.Update(Dt);

			Common();
		}

		protected override void DeathEffects() {
			base.DeathEffects();
			PlaySfx("death_clown");
			DeathEffect(Killed);
		}

		protected override void OnHurt(int A, Entity Creature) {
			base.OnHurt(A, Creature);
			PlaySfx("damage_clown");
		}

		protected override List GetDrops<Item>() {
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

		public class SupplyState : State<SupplyMan> {
		}

		public class IdleState : SupplyState {
			public override void Update(float Dt) {
				base.Update(Dt);

				if (Self.Target != null && Self.OnScreen && Self.CanSee(Self.Target)) {
					Self.Become("run");
					Self.NoticeSignT = 2f;
					Self.PlaySfx("enemy_alert");
				}
			}
		}

		public class RunState : SupplyState {
			public override void Update(float Dt) {
				base.Update(Dt);
				MoveFrom(Player.Instance, 20f, 512f);

				if (!Self.OnScreen || Self.GetDistanceTo(Self.Target.X + 8, Self.Target.Y + 8) > 256f) {
					Self.Poof();
					Player.Instance.PlaySfx("head_explode");
					Self.Done = true;
				}
			}
		}
	}
}