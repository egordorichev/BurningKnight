using BurningKnight.entity.creature.player;
using BurningKnight.entity.level;
using BurningKnight.entity.level.entities;
using BurningKnight.game;
using BurningKnight.physics;
using BurningKnight.util;
using Lens.util.file;

namespace BurningKnight.entity.trap {
	public class RollingSpike : SaveableEntity {
		private static AnimationData Animations = Animation.Make("actor-rolling-spike").Get("idle");
		private float A;
		private Body Body;
		private List<Player> Colliding = new List<>();
		private TextureRegion Region;

		public RollingSpike() {
			_Init();
		}

		protected void _Init() {
			{
				AlwaysActive = true;
			}
		}

		public override void Init() {
			base.Init();
			Region = Animations.GetFrames().Get(Random.NewInt(Animations.GetFrames().Size())).Frame;
			Body = World.CreateCircleBody(this, 1, 1, 7, BodyDef.BodyType.DynamicBody, false);
			Body.SetSleepingAllowed(false);
			MassData Data = new MassData();
			Data.Mass = 1000000000f;

			if (Body != null) {
				Body.SetMassData(Data);
				World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
			}
		}

		public override void Load(FileReader Reader) {
			base.Load(Reader);
			Velocity.X = Reader.ReadFloat();
			Velocity.Y = Reader.ReadFloat();
			World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
		}

		public override void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteFloat(Velocity.X);
			Writer.WriteFloat(Velocity.Y);
		}

		public override void Render() {
			Graphics.Render(Region, this.X + 8, this.Y + 8, A, 8, 8, false, false);
		}

		public override void RenderShadow() {
			Graphics.ShadowSized(this.X, this.Y + 2, W, H, 6);
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity is Player && ((Player) Entity).IsRolling()) return false;

			return base.ShouldCollide(Entity, Contact, Fixture);
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			A += Dt * -(Velocity.X == 0 ? Velocity.Y : Velocity.X) * 10;
			this.X += Velocity.X * Dt;
			this.Y += Velocity.Y * Dt;
			Body.SetTransform(X, Y, 0);

			foreach (Player Player in Colliding)
				if (Player.GetInvt() == 0) {
					Player.ModifyHp(-2, this, true);
					Player.KnockBackFrom(this, 2f);
					Achievements.Unlock(Achievements.UNLOCK_MEATBOY_AXE);
				}
		}

		public override void OnCollision(Entity Entity) {
			base.OnCollision(Entity);

			if (Entity == null || Entity is Door || Entity is SolidProp) {
				Velocity.X *= -1f;
				Velocity.Y *= -1f;
			}
			else if (Entity is Player && !((Player) Entity).IsRolling()) {
				Colliding.Add((Player) Entity);
			}
		}

		public override void OnCollisionEnd(Entity Entity) {
			base.OnCollisionEnd(Entity);

			if (Entity is Player) Colliding.Remove(Entity);
		}

		public override void Destroy() {
			base.Destroy();
			Body = World.RemoveBody(Body);
		}
	}
}