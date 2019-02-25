using BurningKnight.box2dLight;
using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.mob;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.level.entities.fx;
using BurningKnight.core.physics;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.fx {
	public class Note : Entity {
		protected void _Init() {
			{
				AlwaysActive = true;
				AlwaysRender = true;
			}
		}

		public static Animation Animations = Animation.Make("note");
		private TextureRegion Region;
		public float A;
		private Vector2 Vel;
		private Body Body;
		public bool Bad = true;
		private float T;
		private float Scale = 1f;
		public Creature Owner;
		private bool Flip;
		private PointLight Light;
		private static Color[] Colors = { Color.ValueOf("#3d3d3d"), Color.ValueOf("#1f6f50"), Color.ValueOf("#c42430"), Color.ValueOf("#0069aa") };
		private bool Brk;

		public override Void Init() {
			this.PlaySfx("ukulele_" + Random.NewInt(1, 5));
			base.Init();
			this.Flip = Random.Chance(50);
			this.Vel = new Vector2();
			this.Y -= 4;
			Vel.X = (float) (Math.Cos(this.A) * 60);
			Vel.Y = (float) (Math.Sin(this.A) * 60);
			this.Body = World.CreateSimpleCentredBody(this, 0, 0, 10, 10, BodyDef.BodyType.DynamicBody, true);

			if (this.Body != null) {
				this.Body.SetBullet(true);
				World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
				this.Body.SetLinearVelocity(this.Vel);
			} 

			List<Animation.Frame> Frames = Animations.GetFrames("idle");
			int I = Random.NewInt(Frames.Size());
			Region = Frames.Get(I).Frame;
			Color Color = Colors[I];
			Light = World.NewLight(32, new Color(Color.R * 2, Color.G * 2, Color.B * 2, 1f), 32, X, Y);
		}

		private Void Parts() {
			for (int I = 0; I < 3; I++) {
				PoofFx Part = new PoofFx();
				Part.X = this.X;
				Part.Y = this.Y;
				Dungeon.Area.Add(Part);
			}

			this.Done = true;
		}

		public override Void OnCollision(Entity Entity) {
			if (this.Brk || this.Body == null) {
				return;
			} 

			if (Entity is Mob && !this.Bad && !((Mob) Entity).IsDead()) {
				((Mob) Entity).ModifyHp(Math.Round(Random.NewFloatDice(-1, -2)), this.Owner, true);
				this.Brk = true;
				this.Vel.X = 0;
				this.Vel.Y = 0;
				this.Body.SetLinearVelocity(this.Vel);
				this.Parts();
			} else if (Entity is Player && this.Bad) {
				if (!((Player) Entity).IsRolling()) {
					((Player) Entity).ModifyHp(Math.Round(Random.NewFloatDice(-1, -2)), this.Owner, true);
					this.Brk = true;
					this.Vel.X = 0;
					this.Vel.Y = 0;
					this.Body.SetLinearVelocity(this.Vel);
					this.Parts();
				} 
			} else if (Entity == null) {
				this.Brk = true;
				this.Vel.X = 0;
				this.Vel.Y = 0;
				this.Body.SetLinearVelocity(this.Vel);
				this.Parts();
			} 
		}

		public override Void Destroy() {
			base.Destroy();
			World.RemoveLight(Light);
			this.Body = World.RemoveBody(this.Body);
		}

		public override Void Update(float Dt) {
			this.T += Dt;
			base.Update(Dt);
			this.X = this.Body.GetPosition().X;
			this.Y = this.Body.GetPosition().Y;
			Light.SetPosition(X, Y);

			if (this.T >= 5f || Scale <= 0) {
				this.Done = true;
			} 
		}

		public override Void Render() {
			Graphics.Render(Region, this.X, this.Y, 0, Region.GetRegionWidth() / 2, Region.GetRegionHeight() / 2, false, false, Flip ? -Scale : Scale, Scale);
		}

		public override Void RenderShadow() {
			Graphics.Shadow(this.X - W / 2 + (Flip ? 0 : 4), this.Y - this.H / 2 + 3, W - 4, this.H, 8);
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			return (!(Entity is Player) || ((Player) Entity).IsRolling()) && base.ShouldCollide(Entity, Contact, Fixture);
		}

		public Note() {
			_Init();
		}
	}
}
