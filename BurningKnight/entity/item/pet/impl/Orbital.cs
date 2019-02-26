using BurningKnight.entity.item.weapon.projectile;
using BurningKnight.physics;

namespace BurningKnight.entity.item.pet.impl {
	public class Orbital : PetEntity {
		private static List<Orbital> All = new List<>();
		public static float Speed = 1f;
		public static float Count;
		public static float OrbitalTime;
		private float A;
		private Body Body;
		private int Id;
		protected float Sx = 1;
		protected float Sy = 1;

		public Orbital() {
			_Init();
		}

		protected void _Init() {
			{
				NoTp = false;
			}
		}

		public static void UpdateTime(float Dt) {
			OrbitalTime += Dt * Speed;
		}

		private void SetPos() {
			A = Id / Count * Math.PI * 2 + OrbitalTime;
			var D = 28f;
			this.X = Owner.OrbitalRing.X + (float) Math.Cos(A) * D;
			this.Y = Owner.OrbitalRing.Y + (float) Math.Sin(A) * D;
			World.CheckLocked(Body).SetTransform(this.X, this.Y, A);
		}

		public override void Init() {
			base.Init();
			W = Region.GetRegionWidth();
			H = Region.GetRegionHeight();
			Body = World.CreateCircleCentredBody(this, 0f, 0f, Math.Min(Region.GetRegionWidth(), Region.GetRegionHeight()) / 2f, BodyDef.BodyType.DynamicBody, true);
			Body.SetSleepingAllowed(false);
			All.Add(this);
			ReadIndex();
			SetPos();
		}

		public override void OnCollision(Entity Entity) {
			base.OnCollision(Entity);
			OnHit(Entity);
		}

		public override void RenderShadow() {
			Graphics.Shadow(this.X - W / 2, this.Y - H / 2, W, H, 5);
		}

		protected void OnHit(Entity Entity) {
			if (Entity is Projectile && ((Projectile) Entity).Bad) ((Projectile) Entity).Remove();
		}

		protected void ReadIndex() {
			Id = All.IndexOf(this);
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			SetPos();
			Count += (All.Size() - Count) * Dt;
		}

		public override void Destroy() {
			base.Destroy();
			Body = World.RemoveBody(Body);
			All.Remove(this);

			foreach (Orbital O in All) O.ReadIndex();
		}

		public override void Render() {
			Graphics.Render(Region, this.X, this.Y, 0, W / 2, H / 2, false, false, Sx, Sy);
		}
	}
}