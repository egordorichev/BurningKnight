using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.util.camera;
using Lens.util.timer;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.assets.particle.custom {
	public class ConsumableParticle : Entity {
		private Vector2 target = new Vector2(Random.Float(-4, 4), Random.Float(-4, 4));
		private TextureRegion region;
		private float angle;
		private float angleV;
		private float speed;
		private bool tweened;
		private Player player;
		private Vector2 scale = new Vector2(0, Display.UiScale * 2);
		private Action callback;
		private Vector2 offset;
		
		public ConsumableParticle(TextureRegion r, Player p, bool item = false, Action call = null, bool emerald = false) {
			AlwaysActive = true;
			AlwaysVisible = true;

			region = r;
			Width = region.Width;
			Height = region.Height;

			player = p;

			offset = new Vector2(Random.Int(-4, 4), Height + Random.Int(-4, 4));
			speed = Random.Float(1, 2f) * (Random.Chance() ? -1 : 1);

			var s = item ? Display.UiScale : Random.Float(1f, 2f);

			Tween.To(s, scale.X, x => scale.X = x, 0.3f);
			Tween.To(s, scale.Y, x => scale.Y = x, 0.3f);

			if (item) {
				target += new Vector2(Display.UiWidth, Display.UiHeight);
			} else if (emerald) {
				target += new Vector2(Display.UiWidth, 0);
			}

			callback = call;
		}

		public override void PostInit() {
			base.PostInit();
			Center = Camera.Instance.CameraToUi(player.TopCenter) - offset;

			Timer.Add(() => {
				tweened = true;
				callback?.Invoke();
				
				Tween.To(target.X, X, x => X = x, 1f, Ease.QuadInOut);
				Tween.To(target.Y, Y, x => Y = x, 1f, Ease.QuadInOut).OnEnd = () => {
					Done = true;
				};
			}, 1f);
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (tweened) {
				angleV += dt * speed * 2;
				angle += angleV * dt * 20;
				
				scale.X -= dt * 2.2f;
				scale.Y -= dt * 2.2f;

				if (scale.X <= 0) {
					Done = true;
				}
			} else {
				var pos = player.TopCenter;

				if (player.TryGetComponent<ZComponent>(out var z)) {
					pos -= new Vector2(0, z.Z);
				}
				
				Center = Camera.Instance.CameraToUi(pos) - offset;
			}
		}

		public override void Render() {
			var origin = region.Center;
			Graphics.Render(region, Position + origin, angle, origin, scale);
		}
	}
}