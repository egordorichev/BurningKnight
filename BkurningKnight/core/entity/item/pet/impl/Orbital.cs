using BurningKnight.core.assets;
using BurningKnight.core.entity.item.weapon.projectile;
using BurningKnight.core.physics;

namespace BurningKnight.core.entity.item.pet.impl {
	public class Orbital : PetEntity {
		protected void _Init() {
			{
				NoTp = false;
			}
		}

		private static List<Orbital> All = new List<>();
		public static float Speed = 1f;
		public static float Count;
		protected float Sx = 1;
		protected float Sy = 1;
		private Body Body;
		private float A;
		public static float OrbitalTime;
		private int Id;

		public static Void UpdateTime(float Dt) {
			OrbitalTime += Dt * Speed;
		}

		private Void SetPos() {
			this.A = (float) (((float) Id) / (Count) * Math.PI * 2 + OrbitalTime);
			float D = 28f;
			this.X = this.Owner.OrbitalRing.X + (float) Math.Cos(A) * D;
			this.Y = this.Owner.OrbitalRing.Y + (float) Math.Sin(A) * D;
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, A);
		}

		public override Void Init() {
			base.Init();
			this.W = this.Region.GetRegionWidth();
			this.H = this.Region.GetRegionHeight();
			Body = World.CreateCircleCentredBody(this, 0f, 0f, Math.Min(Region.GetRegionWidth(), Region.GetRegionHeight()) / 2f, BodyDef.BodyType.DynamicBody, true);
			Body.SetSleepingAllowed(false);
			All.Add(this);
			ReadIndex();
			SetPos();
		}

		public override Void OnCollision(Entity Entity) {
			base.OnCollision(Entity);
			this.OnHit(Entity);
		}

		public override Void RenderShadow() {
			Graphics.Shadow(this.X - this.W / 2, this.Y - this.H / 2, this.W, this.H, 5);
		}

		protected Void OnHit(Entity Entity) {
			if (Entity is Projectile && ((Projectile) Entity).Bad) {
				((Projectile) Entity).Remove();
			} 
		}

		protected Void ReadIndex() {
			this.Id = All.IndexOf(this);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			SetPos();
			Count += (All.Size() - Count) * Dt;
		}

		public override Void Destroy() {
			base.Destroy();
			Body = World.RemoveBody(this.Body);
			All.Remove(this);

			foreach (Orbital O in All) {
				O.ReadIndex();
			}
		}

		public override Void Render() {
			Graphics.Render(Region, this.X, this.Y, 0, this.W / 2, this.H / 2, false, false, this.Sx, this.Sy);
		}

		public Orbital() {
			_Init();
		}
	}
}
