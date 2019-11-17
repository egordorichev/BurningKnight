using System;
using BurningKnight.assets.lighting;
using BurningKnight.assets.particle;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.state;
using Lens.assets;
using Lens.entity;
using Lens.util.camera;
using Lens.util.math;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.util {
	public static class AnimationUtil {
		public static void ActionFailed() {
			Camera.Instance.Shake(10);
			Audio.PlaySfx("item_nocash");
		}

		public static void Poof(Vector2 where, int depth = 0) {
			for (var i = 0; i < 4; i++) {
				var part = new ParticleEntity(Particles.Dust());
						
				part.Position = where;
				part.Particle.Scale = Rnd.Float(0.4f, 0.8f);
				Run.Level.Area.Add(part);
				part.Depth = depth;
			}
		}

		public static void Explosion(Vector2 where) {
			var explosion = new ParticleEntity(Particles.Animated("explosion", "explosion"));
			explosion.Position = where;
			Run.Level.Area.Add(explosion);
			explosion.Depth = 32;
			explosion.Particle.Velocity = Vector2.Zero;
			explosion.AddShadow();

			Lights.Flash = 1f;
		}

		public static void TeleportAway(Entity entity, Action callback) {
			var scale = entity is Player
				? entity.GetComponent<PlayerGraphicsComponent>().Scale
				: entity.GetAnyComponent<MobAnimationComponent>().Scale;

			var z = entity.GetComponent<ZComponent>();
			
			entity.GetComponent<HealthComponent>().Unhittable = true;

			Tween.To(0, entity.GetComponent<PlayerGraphicsComponent>().Scale.X, x => entity.GetComponent<PlayerGraphicsComponent>().Scale.X = x, 0.3f, Ease.QuadIn);
			Tween.To(4, entity.GetComponent<PlayerGraphicsComponent>().Scale.Y, x => entity.GetComponent<PlayerGraphicsComponent>().Scale.Y = x, 0.3f, Ease.QuadIn);
			
			Tween.To(128, z.Z, x => z.Z = x, 0.5f, Ease.QuadIn).OnEnd = callback;
		}

		public static void TeleportIn(Entity entity) {
			var scale = new Func<Vector2>(() => entity is Player
				? entity.GetComponent<PlayerGraphicsComponent>().Scale
				: entity.GetAnyComponent<MobAnimationComponent>().Scale);

			var z = entity.GetComponent<ZComponent>();
			
			entity.GetComponent<HealthComponent>().Unhittable = true;

			Tween.To(1.5f, scale().X, x => entity.GetComponent<PlayerGraphicsComponent>().Scale.X = x, 0.3f);
			Tween.To(0.1f, scale().Y, x => entity.GetComponent<PlayerGraphicsComponent>().Scale.Y = x, 0.3f).OnEnd = () => {
				Tween.To(1, scale().X, x => entity.GetComponent<PlayerGraphicsComponent>().Scale.X = x, 0.3f);
				Tween.To(1f, scale().Y, x => entity.GetComponent<PlayerGraphicsComponent>().Scale.Y = x, 0.3f);
			};
			
			Tween.To(0, z.Z, x => z.Z = x, 0.3f).OnEnd = () => {
				entity.GetComponent<HealthComponent>().Unhittable = false;
			};
		}
	}
}