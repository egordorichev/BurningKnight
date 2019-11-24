using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.level.entities;
using Lens.entity;
using Lens.graphics;
using Lens.util.math;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.room.controllable {
	public class FireTrap : RoomControllable {
		private static TextureRegion tile;
		private float timer;
		private bool flaming;
		private float lastParticle;
		
		protected List<Entity> Colliding = new List<Entity>();
		
		public override void AddComponents() {
			base.AddComponents();

			AlwaysActive = true;
			Depth = Layers.Entrance;

			if (tile == null) {
				tile = CommonAse.Props.GetSlice("firetrap");
			}
			
			AddComponent(new RectBodyComponent(1, 1, 11, 14, BodyType.Static, true));
		}

		public override void PostInit() {
			base.PostInit();
			ResetTimer();
		}

		private void ResetTimer() {
			timer = X / 256f * 5f % 5f;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (!On) {
				ResetTimer();
				return;
			} else {
				var room = GetComponent<RoomComponent>().Room;

				if (room.Tagged[Tags.Player].Count == 0 || room.Tagged[Tags.MustBeKilled].Count == 0) {
					ResetTimer();
					return;
				}
			}

			timer += dt;

			if (flaming) {
				if (timer <= 0.3f) {
					lastParticle -= dt;
					
					if (lastParticle <= 0) {
						lastParticle = 0.1f;
						
						Area.Add(new FireParticle {
							Position = new Vector2(CenterX, CenterY),
							XChange = 0.1f,
							Scale = 0.3f,
							Vy = 8,
							Mod = 2
						});
					}
				} else if (timer >= 1.5f) {
					lastParticle -= dt;
					Hurt();

					if (lastParticle <= 0) {
						lastParticle = 0.15f;
						
						Area.Add(new FireParticle {
							Position = new Vector2(CenterX, CenterY),
							XChange = 0.1f,
							Scale = 0.3f,
							Vy = 8,
							Mod = 4
						});
					}
					
					if (timer >= 5f) {
						timer = 0;
						flaming = false;
					}
				}	
			} else {
				if (timer >= 5f) {
					timer = 0;
					flaming = true;
					lastParticle = 0;
				}	
			}
		}

		public override void Render() {
			Graphics.Render(tile, Position);
		}
		
		protected virtual bool ShouldHurt(Entity e) {
			return e is Player;
		}
		
		protected void Hurt() {
			foreach (var c in Colliding) {
				c.GetComponent<HealthComponent>().ModifyHealth(-1, this);
			}
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent cse) {
				if (ShouldHurt(cse.Entity)) {
					Colliding.Add(cse.Entity);
				}
			} else if (e is CollisionEndedEvent cee) {
				Colliding.Remove(cee.Entity);
			}
			
			return base.HandleEvent(e);
		}
	}
}