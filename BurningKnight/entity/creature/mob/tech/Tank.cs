using BurningKnight.entity.creature.player;
using BurningKnight.entity.level;
using BurningKnight.entity.level.entities;
using BurningKnight.entity.trap;
using BurningKnight.physics;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.creature.mob.tech {
	public class Tank : Bot {
		public static Animation Animations = Animation.Make("actor-tank", "-normal");
		private AnimationData Animation;
		private AnimationData HurtHor;
		private AnimationData HurtVert;
		private AnimationData Killed;
		private AnimationData MoveHor;
		private AnimationData MoveVert;
		private bool Vert;

		public Tank() {
			_Init();
		}

		protected void _Init() {
			{
				HpMax = 16;
				MoveVert = GetAnimation().Get("vert");
				HurtVert = GetAnimation().Get("hurt_vert");
				MoveHor = GetAnimation().Get("horiz");
				HurtHor = GetAnimation().Get("hurt_horiz");
				Killed = GetAnimation().Get("dead");
				Animation = MoveHor;
				W = 20;
				H = 24;
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

		public override void OnCollision(Entity Entity) {
			if (Entity is Level || Entity == null || Entity is Door || Entity is SolidProp || Entity is RollingSpike)
				if (Ai != null)
					((IdleState) Ai).SelectDir();

			base.OnCollision(Entity);
		}

		public override float GetOy() {
			return 11;
		}

		public override void Render() {
			Vert = Acceleration.Y != 0;

			if (Vert) {
				Flipped = false;
				FlippedVert = Acceleration.Y < 0;
			}
			else {
				FlippedVert = false;
				Flipped = Acceleration.X > 0;
			}


			if (Dead)
				Animation = Killed;
			else if (Invt > 0)
				Animation = Vert ? HurtVert : HurtHor;
			else
				Animation = Vert ? MoveVert : MoveHor;


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
		}

		protected override void OnHurt(int A, Entity Creature) {
			base.OnHurt(A, Creature);
			PlaySfx("damage_clown");
		}

		protected override State GetAi(string State) {
			switch (State) {
				case "idle":
				case "roam":
				case "alerted": {
					return new IdleState();
				}
			}

			return base.GetAi(State);
		}

		public override void RenderShadow() {
			if (Vert)
				Graphics.Shadow(X + 3, Y + (FlippedVert ? 7 : 6), 14, H, 0);
			else
				Graphics.Shadow(X + (Flipped ? 4 : 0), Y + 3, 16, H, 0);
		}

		public class IdleState : State<Tank> {
			private float Delay;
			private Point Dir;
			private float Last;

			public override void OnEnter() {
				base.OnEnter();
				SelectDir();
			}

			public void SelectDir() {
				T = 0;
				Delay = Random.NewFloat(5, 10);

				if (Dir == null) {
					var Vert = Random.Chance(50);
					Dir = new Point(Vert ? 0 : Random.Chance(50) ? -1 : 1, !Vert ? 0 : Random.Chance(50) ? -1 : 1);

					return;
				}

				Self.Velocity.X -= Dir.X * 60;
				Self.Velocity.Y -= Dir.Y * 60;

				if (Dir.X != 0) {
					Dir.X = 0;
					Dir.Y = Random.Chance(50) ? -1 : 1;
				}
				else {
					Dir.Y = 0;
					Dir.X = Random.Chance(50) ? -1 : 1;
				}


				if (Last <= 0 && Player.Instance.Room == Self.Room) {
					var Mine = new Mine();
					Mine.X = Self.X;
					Mine.Y = Self.Y;
					Dungeon.Area.Add(Mine.Add());
					Last = 1f;
				}
			}

			public override void Update(float Dt) {
				base.Update(Dt);
				Last -= Dt;

				if (T >= Delay) SelectDir();

				float S = 10;
				Self.Acceleration.X = Dir.X * S;
				Self.Acceleration.Y = Dir.Y * S;
			}
		}
	}
}