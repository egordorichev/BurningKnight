using BurningKnight.core.entity.fx;
using BurningKnight.core.entity.item;
using BurningKnight.core.entity.item.weapon.projectile;
using BurningKnight.core.util;
using BurningKnight.core.util.file;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.trap {
	public class FourSideTurret : Turret {
		protected void _Init() {
			{
				Region = Item.Missing;
			}
		}

		private AnimationData Four;
		public bool Str = true;

		public override Void Load(FileReader Reader) {
			base.Load(Reader);
			this.Str = Reader.ReadBoolean();
		}

		public override Void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteBoolean(this.Str);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (Four == null) {
				Four = GetAnimation().Get("turret_4_directions");
			} 

			this.Four.SetFrame(Str ? 0 : 1);
		}

		public override Void Render() {
			if (Four == null) {
				Four = GetAnimation().Get("turret_4_directions");
			} 

			Four.Render(this.X, this.Y, false, false, 8, 0, 0, Sx, Sy);
		}

		protected override Void SendFlames() {
			for (int I = 0; I < 4; I++) {
				FireFx Fx = (Random.Chance(50) && this.T >= 1.5f) ? new FireFxPhysic() : new FireFx();
				float D = 5;
				float A = (float) (this.A + I * (Math.PI / 2));
				Fx.X = X + Random.NewFloat(-4, 4) + 8 + (float) (Math.Cos(A) * D);
				Fx.Y = Y + Random.NewFloat(-4, 4) + 8 + (float) (Math.Sin(A) * D);
				float F = this.T >= 1.5f ? 120f : 40f;
				Fx.Vel.X = (float) (Math.Cos(A) * F);
				Fx.Vel.Y = (float) (Math.Sin(A) * F);
				Dungeon.Area.Add(Fx);
			}
		}

		protected override Void Send() {
			this.Tween();
			this.PlaySfx(this.Type == 3 ? "fire" : "gun_machinegun");

			if (this.Type == 3) {
				this.On = true;
				this.T = 0;

				return;
			} 

			for (int I = 0; I < 4; I++) {
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
				float S = 1.5f * 30f;
				float A = (float) (this.A + I * Math.PI / 2);
				Bullet.Velocity = new Point((float) Math.Cos(A) * S, (float) Math.Sin(A) * S);
				Bullet.A = A;
				Dungeon.Area.Add(Bullet);
			}
		}

		public FourSideTurret() {
			_Init();
		}
	}
}
