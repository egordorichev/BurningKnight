using System;
using System.Collections.Generic;
using BurningKnight.assets.achievements;
using BurningKnight.entity.creature.player;
using Lens.entity;
using Lens.entity.component;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class OrbitGiverComponent : Component {
		public List<Entity> Orbiting = new List<Entity>();
		public float RadiusMultiplier = 1;
		public float T;

		private float count;
		private Vector2 center;
		
		public void DestroyAll() {
			foreach (var o in Orbiting) {
				o.Done = true;
			}

			Orbiting.Clear();
		}
		
		public override void Update(float dt) {
			base.Update(dt);

			T += dt;
			count += (Orbiting.Count - count) * dt * 4;

			if (Entity.DistanceTo(center) > 32f) {
				center = Entity.Center;
			} else {
				center += (Entity.Center - center) * (dt * 8);
			}

			for (var i = Orbiting.Count - 1; i >= 0; i--) {
				var e = Orbiting[i];
				var component = e.GetComponent<OrbitalComponent>();
				var d = component.Radius * RadiusMultiplier;
				var a = i / count * Math.PI * 2 - T * 1.5f;

				if (e.Done) {
					RemoveOrbiter(e);
					continue;
				}

				component.CurrentAngle = (float) a;
				var target = center + new Vector2((float) Math.Cos(a) * d, (float) Math.Sin(a) * d);

				if (component.Lerp) {
					e.Center += (target - e.Center) * (dt * 8);
				} else {
					e.Center = target;
				}
			}
		}

		public void AddOrbiter(Entity e) {
			if (!e.HasComponent<OrbitalComponent>()) {
				e.AddComponent(new OrbitalComponent());
			}

			if (Entity is Player && Orbiting.Count == 0) {
				Entity.GetComponent<AudioEmitterComponent>().Emit("item_orbitals", 0.5f, looped: true, tween: true);
			}			
			
			e.GetComponent<OrbitalComponent>().Orbiting = Entity;
			Orbiting.Add(e);

			if (Entity is Player && Orbiting.Count >= 3) {
				Achievements.Unlock("bk:star");
			}
		}

		public void RemoveOrbiter(Entity e) {
			Orbiting.Remove(e);
			e.GetComponent<OrbitalComponent>().Orbiting = null;

			if (Orbiting.Count == 0) {
				RemoveSound();
			}
		}

		public override void Destroy() {
			base.Destroy();
			
			foreach (var o in Orbiting) {
				o.GetComponent<OrbitalComponent>().Orbiting = null;
			}

			RemoveSound();
			Orbiting.Clear();
		}

		private void RemoveSound() {
			if (Entity is Player) {
				var c = Entity.GetComponent<AudioEmitterComponent>();

				foreach (var s in c.Playing) {
					if (s.Key == "item_orbitals") {
						var e = s.Value;
						
						e.Effect.IsLooped = false;
						e.Effect.Pause();

						Tween.To(0, 1f, x => e.BaseVolume = x, 0.3f);
						break;
					}
				}
			}
		}
	}
}