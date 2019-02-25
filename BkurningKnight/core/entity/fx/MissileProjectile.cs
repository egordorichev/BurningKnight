using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item;
using BurningKnight.core.entity.item.weapon.projectile;
using BurningKnight.core.entity.item.weapon.projectile.fx;
using BurningKnight.core.entity.level.entities.fx;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.fx {
	public class MissileProjectile : BulletProjectile {
		protected void _Init() {
			{
				Depth = 32;
				AlwaysRender = true;
			}
		}

		public Point Target;
		public Player To;
		public bool Up = true;
		private float StartY;

		public override Void Init() {
			this.Angle = (float) Math.PI;
			this.Owner.PlaySfx("missile");
			this.StartY = this.Y;
			base.Init();
			this.Velocity.Y = 300;
		}

		protected override Void Setup() {
			base.Setup();
			Sprite = Graphics.GetTexture("bullet-missile_burning");
		}

		public override Void Logic(float Dt) {
			bool Dd = this.Done;
			this.Done = this.Remove;

			if (Broke) {
				if (!DidDie) {
					DidDie = true;
					this.Death();
				} 

				return;
			} 

			if (this.Done && !Dd) {
				this.OnDeath();

				for (int I = 0; I < 20; I++) {
					PoofFx Part = new PoofFx();
					Part.X = this.X;
					Part.Y = this.Y;
					Dungeon.Area.Add(Part);
				}
			} 

			if (this.Up) {
				if (this.Y >= this.StartY + Display.GAME_HEIGHT + 16) {
					this.Up = false;
					this.Velocity.Y = -160;
					Target = new Point(Player.Instance.X + 8, Player.Instance.Y);
					this.X = Target.X;
					this.Y = Target.Y + Display.GAME_HEIGHT + 16;
				} 
			} else {
				this.Velocity.Y -= Dt * 148;
			}


			this.Y += this.Velocity.Y * Dt;
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y + Sprite.GetRegionHeight() / 2, 0);
			this.Body.SetLinearVelocity(this.Velocity);
			Light.SetPosition(X, Y);

			if (!this.Up && this.Y <= Target.Y) {
				this.Y = Target.Y;
				Broke = true;
			} 
		}

		protected override Void OnDeath() {
			base.OnDeath();

			for (int I = 0; I < Random.NewInt(2, 5); I++) {
				Explosion Explosion = new Explosion(X + Random.NewFloat(-16, 16), Y + Random.NewFloat(-16, 16));
				Explosion.Delay = Random.NewFloat(0, 0.25f);
				Dungeon.Area.Add(Explosion);
			}

			PlaySfx("explosion");

			for (int I = 0; I < 10; I++) {
				PoofFx Fx = new PoofFx();
				Fx.X = this.X;
				Fx.Y = this.Y;
				Dungeon.Area.Add(Fx);
			}
		}

		public override Void Destroy() {
			base.Destroy();
			Camera.Shake(3f);
		}

		public override Void OnCollision(Entity Entity) {
			if (Target != null && Entity is Player && this.Y <= Target.Y + 16 && !this.Up) {
				base.OnCollision(Entity);
			} 
		}

		public override Void Update(float Dt) {
			this.Logic(Dt);
		}

		public override Void Render() {
			TextureRegion Reg = Sprite;
			Texture Texture = Reg.GetTexture();
			Graphics.Batch.End();
			RectFx.Shader.Begin();
			RectFx.Shader.SetUniformf("white", (this.DissappearWithTime && this.T >= 4f && (this.T - 4f) % 0.3f > 0.15f) ? 1 : 0);
			RectFx.Shader.SetUniformf("r", 1f);
			RectFx.Shader.SetUniformf("g", 1f);
			RectFx.Shader.SetUniformf("b", 1f);
			RectFx.Shader.SetUniformf("a", 1);
			RectFx.Shader.SetUniformf("remove", 0f);
			RectFx.Shader.SetUniformf("pos", new Vector2(((float) Reg.GetRegionX()) / Texture.GetWidth(), ((float) Reg.GetRegionY()) / Texture.GetHeight()));
			RectFx.Shader.SetUniformf("size", new Vector2(((float) Reg.GetRegionWidth()) / Texture.GetWidth(), ((float) Reg.GetRegionHeight()) / Texture.GetHeight()));
			RectFx.Shader.End();
			Graphics.Batch.SetShader(RectFx.Shader);
			Graphics.Batch.Begin();
			Graphics.Render(Sprite, X, Y + Sprite.GetRegionHeight() / 2, 0, Sprite.GetRegionWidth() / 2, Sprite.GetRegionHeight() / 2, false, false, 1, Up ? 1 : -1);
			Graphics.Batch.End();
			Graphics.Batch.SetShader(null);
			Graphics.Batch.Begin();
		}

		public override Void RenderShadow() {

		}

		public MissileProjectile() {
			_Init();
		}
	}
}
