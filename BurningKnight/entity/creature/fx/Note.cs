using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.level.entities.fx;
using BurningKnight.physics;
using BurningKnight.util;

namespace BurningKnight.entity.creature.fx {
	public class Note : Entity {
		public static Animation Animations = Animation.Make("note");
		private static Color[] Colors = {Color.ValueOf("#3d3d3d"), Color.ValueOf("#1f6f50"), Color.ValueOf("#c42430"), Color.ValueOf("#0069aa")};
		public float A;
		public bool Bad = true;
		private Body Body;
		private bool Brk;
		private bool Flip;
		private PointLight Light;
		public Creature Owner;
		private TextureRegion Region;
		private float Scale = 1f;
		private float T;
		private Vector2 Vel;

		public Note() {
			_Init();
		}

		protected void _Init() {
			{
				AlwaysActive = true;
				AlwaysRender = true;
			}
		}

		public override void Init() {
			PlaySfx("ukulele_" + Random.NewInt(1, 5));
			base.Init();
			Flip = Random.Chance(50);
			Vel = new Vector2();
			this.Y -= 4;
			Vel.X = (float) (Math.Cos(A) * 60);
			Vel.Y = (float) (Math.Sin(A) * 60);
			Body = World.CreateSimpleCentredBody(this, 0, 0, 10, 10, BodyDef.BodyType.DynamicBody, true);

			if (Body != null) {
				Body.SetBullet(true);
				World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
				Body.SetLinearVelocity(Vel);
			}

			List<Animation.Frame> Frames = Animations.GetFrames("idle");
			var I = Random.NewInt(Frames.Size());
			Region = Frames.Get(I).Frame;
			Color Color = Colors[I];
			Light = World.NewLight(32, new Color(Color.R * 2, Color.G * 2, Color.B * 2, 1f), 32, X, Y);
		}

		private void Parts() {
			for (var I = 0; I < 3; I++) {
				var Part = new PoofFx();
				Part.X = this.X;
				Part.Y = this.Y;
				Dungeon.Area.Add(Part);
			}

			Done = true;
		}

		public override void OnCollision(Entity Entity) {
			if (Brk || Body == null) return;

			if (Entity is Mob && !Bad && !((Mob) Entity).IsDead()) {
				((Mob) Entity).ModifyHp(Math.Round(Random.NewFloatDice(-1, -2)), Owner, true);
				Brk = true;
				Vel.X = 0;
				Vel.Y = 0;
				Body.SetLinearVelocity(Vel);
				Parts();
			}
			else if (Entity is Player && Bad) {
				if (!((Player) Entity).IsRolling()) {
					((Player) Entity).ModifyHp(Math.Round(Random.NewFloatDice(-1, -2)), Owner, true);
					Brk = true;
					Vel.X = 0;
					Vel.Y = 0;
					Body.SetLinearVelocity(Vel);
					Parts();
				}
			}
			else if (Entity == null) {
				Brk = true;
				Vel.X = 0;
				Vel.Y = 0;
				Body.SetLinearVelocity(Vel);
				Parts();
			}
		}

		public override void Destroy() {
			base.Destroy();
			World.RemoveLight(Light);
			Body = World.RemoveBody(Body);
		}

		public override void Update(float Dt) {
			T += Dt;
			base.Update(Dt);
			this.X = Body.GetPosition().X;
			this.Y = Body.GetPosition().Y;
			Light.SetPosition(X, Y);

			if (T >= 5f || Scale <= 0) Done = true;
		}

		public override void Render() {
			Graphics.Render(Region, this.X, this.Y, 0, Region.GetRegionWidth() / 2, Region.GetRegionHeight() / 2, false, false, Flip ? -Scale : Scale, Scale);
		}

		public override void RenderShadow() {
			Graphics.Shadow(this.X - W / 2 + (Flip ? 0 : 4), this.Y - H / 2 + 3, W - 4, H, 8);
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			return (!(Entity is Player) || ((Player) Entity).IsRolling()) && base.ShouldCollide(Entity, Contact, Fixture);
		}
	}
}