using BurningKnight.entity.fx;
using BurningKnight.entity.item;
using BurningKnight.entity.item.weapon.projectile;
using BurningKnight.util;
using BurningKnight.util.geometry;
using Lens.util.file;

namespace BurningKnight.entity.trap {
	public class FourSideTurret : Turret {
		private AnimationData Four;
		public bool Str = true;

		public FourSideTurret() {
			_Init();
		}

		protected void _Init() {
			{
				Region = Item.Missing;
			}
		}

		public override void Load(FileReader Reader) {
			base.Load(Reader);
			Str = Reader.ReadBoolean();
		}

		public override void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteBoolean(Str);
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (Four == null) Four = GetAnimation().Get("turret_4_directions");

			Four.SetFrame(Str ? 0 : 1);
		}

		public override void Render() {
			if (Four == null) Four = GetAnimation().Get("turret_4_directions");

			Four.Render(this.X, this.Y, false, false, 8, 0, 0, Sx, Sy);
		}

		protected override void SendFlames() {
			for (var I = 0; I < 4; I++) {
				var Fx = Random.Chance(50) && T >= 1.5f ? new FireFxPhysic() : new FireFx();
				float D = 5;
				var A = this.A + I * (Math.PI / 2);
				Fx.X = X + Random.NewFloat(-4, 4) + 8 + Math.Cos(A) * D;
				Fx.Y = Y + Random.NewFloat(-4, 4) + 8 + Math.Sin(A) * D;
				var F = T >= 1.5f ? 120f : 40f;
				Fx.Vel.X = Math.Cos(A) * F;
				Fx.Vel.Y = Math.Sin(A) * F;
				Dungeon.Area.Add(Fx);
			}
		}

		protected override void Send() {
			Tween();
			PlaySfx(Type == 3 ? "fire" : "gun_machinegun");

			if (Type == 3) {
				On = true;
				T = 0;

				return;
			}

			for (var I = 0; I < 4; I++) {
				BulletProjectile Bullet = new NanoBullet();
				float X = this.X + 8;
				float Y = this.Y + 8;
				Bullet.X = X;
				Bullet.Y = Y;
				Bullet.Damage = 2;
				Bullet.W = 12;
				Bullet.H = 12;
				Bullet.Bad = true;
				this.Modify(Bullet);
				var S = 1.5f * 30f;
				var A = this.A + I * Math.PI / 2;
				Bullet.Velocity = new Point((float) Math.Cos(A) * S, (float) Math.Sin(A) * S);
				Bullet.A = A;
				Dungeon.Area.Add(Bullet);
			}
		}
	}
}