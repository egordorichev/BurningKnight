using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.level;
using BurningKnight.core.entity.level.entities;
using BurningKnight.core.game;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.file;

namespace BurningKnight.core.entity.trap {
	public class RollingSpike : SaveableEntity {
		protected void _Init() {
			{
				AlwaysActive = true;
			}
		}

		private static AnimationData Animations = Animation.Make("actor-rolling-spike").Get("idle");
		private Body Body;
		private TextureRegion Region;
		private float A;
		private List<Player> Colliding = new List<>();

		public override Void Init() {
			base.Init();
			Region = Animations.GetFrames().Get(Random.NewInt(Animations.GetFrames().Size())).Frame;
			Body = World.CreateCircleBody(this, 1, 1, 7, BodyDef.BodyType.DynamicBody, false);
			Body.SetSleepingAllowed(false);
			MassData Data = new MassData();
			Data.Mass = 1000000000f;

			if (this.Body != null) {
				this.Body.SetMassData(Data);
				World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
			} 
		}

		public override Void Load(FileReader Reader) {
			base.Load(Reader);
			this.Velocity.X = Reader.ReadFloat();
			this.Velocity.Y = Reader.ReadFloat();
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
		}

		public override Void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteFloat(this.Velocity.X);
			Writer.WriteFloat(this.Velocity.Y);
		}

		public override Void Render() {
			Graphics.Render(Region, this.X + 8, this.Y + 8, this.A, 8, 8, false, false);
		}

		public override Void RenderShadow() {
			Graphics.ShadowSized(this.X, this.Y + 2, this.W, this.H, 6);
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity is Player && ((Player) Entity).IsRolling()) {
				return false;
			} 

			return base.ShouldCollide(Entity, Contact, Fixture);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			this.A += Dt * -(this.Velocity.X == 0 ? this.Velocity.Y : this.Velocity.X) * 10;
			this.X += this.Velocity.X * Dt;
			this.Y += this.Velocity.Y * Dt;
			this.Body.SetTransform(X, Y, 0);

			foreach (Player Player in Colliding) {
				if (Player.GetInvt() == 0) {
					Player.ModifyHp(-2, this, true);
					Player.KnockBackFrom(this, 2f);
					Achievements.Unlock(Achievements.UNLOCK_MEATBOY_AXE);
				} 
			}
		}

		public override Void OnCollision(Entity Entity) {
			base.OnCollision(Entity);

			if (Entity == null || Entity is Door || Entity is SolidProp) {
				this.Velocity.X *= -1f;
				this.Velocity.Y *= -1f;
			} else if (Entity is Player && !((Player) Entity).IsRolling()) {
				this.Colliding.Add((Player) Entity);
			} 
		}

		public override Void OnCollisionEnd(Entity Entity) {
			base.OnCollisionEnd(Entity);

			if (Entity is Player) {
				this.Colliding.Remove(Entity);
			} 
		}

		public override Void Destroy() {
			base.Destroy();
			this.Body = World.RemoveBody(this.Body);
		}

		public RollingSpike() {
			_Init();
		}
	}
}
