using BurningKnight.assets;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using Lens.entity;
using Lens.graphics;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.room.controllable {
	public class FireTrap : RoomControllable {
		private static TextureRegion tile;
		private float timer;
		private bool flaming;
		private float lastParticle;
		
		public override void AddComponents() {
			base.AddComponents();

			AlwaysActive = true;
			Depth = Layers.Entrance;

			if (tile == null) {
				tile = CommonAse.Props.GetSlice("firetrap");
			}
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
					
					if (lastParticle <= 0) {
						lastParticle = 0.15f;
						
						Area.Add(new FireParticle {
							Position = new Vector2(CenterX, CenterY),
							XChange = 0.1f,
							Scale = 0.3f,
							Vy = 8,
							Hurts = Rnd.Chance(10),
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
	}
}