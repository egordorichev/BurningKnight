using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.level;
using BurningKnight.core.entity.level.entities;
using BurningKnight.core.entity.trap;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.creature.mob.tech {
	public class Tank : Bot {
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

		public class IdleState : Mob.State<Tank>  {
			private float Delay;
			private Point Dir;
			private float Last;

			public override Void OnEnter() {
				base.OnEnter();
				SelectDir();
			}

			public Void SelectDir() {
				T = 0;
				Delay = Random.NewFloat(5, 10);

				if (Dir == null) {
					bool Vert = Random.Chance(50);
					Dir = new Point(Vert ? 0 : Random.Chance(50) ? -1 : 1, !Vert ? 0 : Random.Chance(50) ? -1 : 1);

					return;
				} 

				Self.Velocity.X -= Dir.X * 60;
				Self.Velocity.Y -= Dir.Y * 60;

				if (Dir.X != 0) {
					Dir.X = 0;
					Dir.Y = Random.Chance(50) ? -1 : 1;
				} else {
					Dir.Y = 0;
					Dir.X = Random.Chance(50) ? -1 : 1;
				}


				if (Last <= 0 && Player.Instance.Room == Self.Room) {
					Mine Mine = new Mine();
					Mine.X = Self.X;
					Mine.Y = Self.Y;
					Dungeon.Area.Add(Mine.Add());
					Last = 1f;
				} 
			}

			public override Void Update(float Dt) {
				base.Update(Dt);
				Last -= Dt;

				if (T >= Delay) {
					SelectDir();
				} 

				float S = 10;
				Self.Acceleration.X = Dir.X * S;
				Self.Acceleration.Y = Dir.Y * S;
			}
		}

		public static Animation Animations = Animation.Make("actor-tank", "-normal");
		private AnimationData MoveHor;
		private AnimationData MoveVert;
		private AnimationData Killed;
		private AnimationData HurtHor;
		private AnimationData HurtVert;
		private AnimationData Animation;
		private bool Vert;

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

		public override Void OnCollision(Entity Entity) {
			if (Entity is Level || Entity == null || Entity is Door || Entity is SolidProp || Entity is RollingSpike) {
				if (this.Ai != null) {
					((IdleState) this.Ai).SelectDir();
				} 
			} 

			base.OnCollision(Entity);
		}

		public override float GetOy() {
			return 11;
		}

		public override Void Render() {
			Vert = this.Acceleration.Y != 0;

			if (Vert) {
				Flipped = false;
				this.FlippedVert = this.Acceleration.Y < 0;
			} else {
				FlippedVert = false;
				this.Flipped = this.Acceleration.X > 0;
			}


			if (this.Dead) {
				this.Animation = Killed;
			} else if (this.Invt > 0) {
				this.Animation = Vert ? HurtVert : HurtHor;
			} else {
				this.Animation = Vert ? MoveVert : MoveHor;
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
		}

		protected override Void OnHurt(int A, Entity Creature) {
			base.OnHurt(A, Creature);
			this.PlaySfx("damage_clown");
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

		public override Void RenderShadow() {
			if (Vert) {
				Graphics.Shadow(X + 3, Y + (FlippedVert ? 7 : 6), 14, H, 0);
			} else {
				Graphics.Shadow(X + (Flipped ? 4 : 0), Y + 3, 16, H, 0);
			}

		}

		public Tank() {
			_Init();
		}
	}
}
