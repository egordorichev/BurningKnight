using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.entity.item.weapon.projectile;
using BurningKnight.entity.item.weapon.projectile.fx;
using BurningKnight.entity.level.entities.fx;
using BurningKnight.physics;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.fx {
	public class MissileProjectile : BulletProjectile {
		private float StartY;

		public Point Target;
		public Player To;
		public bool Up = true;

		public MissileProjectile() {
			_Init();
		}

		protected void _Init() {
			{
				Depth = 32;
				AlwaysRender = true;
			}
		}

		public override void Init() {
			Angle = (float) Math.PI;
			Owner.PlaySfx("missile");
			StartY = this.Y;
			base.Init();
			Velocity.Y = 300;
		}

		protected override void Setup() {
			base.Setup();
			Sprite = Graphics.GetTexture("bullet-missile_burning");
		}

		public override void Logic(float Dt) {
			var Dd = Done;
			Done = Remove;

			if (Broke) {
				if (!DidDie) {
					DidDie = true;
					Death();
				}

				return;
			}

			if (Done && !Dd) {
				OnDeath();

				for (var I = 0; I < 20; I++) {
					var Part = new PoofFx();
					Part.X = this.X;
					Part.Y = this.Y;
					Dungeon.Area.Add(Part);
				}
			}

			if (Up) {
				if (this.Y >= StartY + Display.GAME_HEIGHT + 16) {
					Up = false;
					Velocity.Y = -160;
					Target = new Point(Player.Instance.X + 8, Player.Instance.Y);
					this.X = Target.X;
					this.Y = Target.Y + Display.GAME_HEIGHT + 16;
				}
			}
			else {
				Velocity.Y -= Dt * 148;
			}


			this.Y += Velocity.Y * Dt;
			World.CheckLocked(Body).SetTransform(this.X, this.Y + Sprite.GetRegionHeight() / 2, 0);
			Body.SetLinearVelocity(Velocity);
			Light.SetPosition(X, Y);

			if (!Up && this.Y <= Target.Y) {
				this.Y = Target.Y;
				Broke = true;
			}
		}

		protected override void OnDeath() {
			base.OnDeath();

			for (var I = 0; I < Random.NewInt(2, 5); I++) {
				var Explosion = new Explosion(X + Random.NewFloat(-16, 16), Y + Random.NewFloat(-16, 16));
				Explosion.Delay = Random.NewFloat(0, 0.25f);
				Dungeon.Area.Add(Explosion);
			}

			PlaySfx("explosion");

			for (var I = 0; I < 10; I++) {
				var Fx = new PoofFx();
				Fx.X = this.X;
				Fx.Y = this.Y;
				Dungeon.Area.Add(Fx);
			}
		}

		public override void Destroy() {
			base.Destroy();
			Camera.Shake(3f);
		}

		public override void OnCollision(Entity Entity) {
			if (Target != null && Entity is Player && this.Y <= Target.Y + 16 && !Up) base.OnCollision(Entity);
		}

		public override void Update(float Dt) {
			Logic(Dt);
		}

		public override void Render() {
			TextureRegion Reg = Sprite;
			Texture Texture = Reg.GetTexture();
			Graphics.Batch.End();
			RectFx.Shader.Begin();
			RectFx.Shader.SetUniformf("white", DissappearWithTime && T >= 4f && (T - 4f) % 0.3f > 0.15f ? 1 : 0);
			RectFx.Shader.SetUniformf("r", 1f);
			RectFx.Shader.SetUniformf("g", 1f);
			RectFx.Shader.SetUniformf("b", 1f);
			RectFx.Shader.SetUniformf("a", 1);
			RectFx.Shader.SetUniformf("remove", 0f);
			RectFx.Shader.SetUniformf("pos", new Vector2((float) Reg.GetRegionX() / Texture.GetWidth(), (float) Reg.GetRegionY() / Texture.GetHeight()));
			RectFx.Shader.SetUniformf("size", new Vector2((float) Reg.GetRegionWidth() / Texture.GetWidth(), (float) Reg.GetRegionHeight() / Texture.GetHeight()));
			RectFx.Shader.End();
			Graphics.Batch.SetShader(RectFx.Shader);
			Graphics.Batch.Begin();
			Graphics.Render(Sprite, X, Y + Sprite.GetRegionHeight() / 2, 0, Sprite.GetRegionWidth() / 2, Sprite.GetRegionHeight() / 2, false, false, 1, Up ? 1 : -1);
			Graphics.Batch.End();
			Graphics.Batch.SetShader(null);
			Graphics.Batch.Begin();
		}

		public override void RenderShadow() {
		}
	}
}