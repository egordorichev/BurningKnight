using System;
using BurningKnight.assets.particle;
using BurningKnight.entity.fx;
using BurningKnight.level.tile;
using Lens;
using Lens.entity;
using Lens.util.camera;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.level {
	public class DestroyableLevel : Entity {
		public Level Level;
		
		public override void AddComponents() {
			base.AddComponents();
			AddComponent(new DestroyableBodyComponent());
		}

		public void Break(float x, float y) {
			Set((int) Math.Floor(x / 16), (int) Math.Floor(y / 16));
			
			Set((int) Math.Floor(x / 16 - 0.5f), (int) Math.Floor(y / 16));
			Set((int) Math.Floor(x / 16 + 0.5f), (int) Math.Floor(y / 16));
			Set((int) Math.Floor(x / 16), (int) Math.Floor(y / 16 - 0.5f));
			Set((int) Math.Floor(x / 16), (int) Math.Floor(y / 16 + 0.5f));
		}

		private void Set(int tx, int ty) {
			if (!Level.IsInside(tx, ty)) {
				return;
			}

			var index = Level.ToIndex(tx, ty);
			var tile = Level.Tiles[index];

			if ((Tile) tile != Tile.Planks) {
				return;
			}

			Level.Tiles[index] = (byte) Tile.FloorA;
			Level.UpdateTile(tx, ty);
			
			GetComponent<DestroyableBodyComponent>().CreateBody();

			Animate(Area, tx, ty);
		}

		public static void Animate(Area area, int x, int y) {
			area.Add(new TileFx {
				X = x * 16,
				Y = y * 16 - 8
			});
			
			for (var i = 0; i < 4; i++) {
				var part = new ParticleEntity(Particles.Dust());
						
				part.Position = new Vector2(x * 16 + 8, y * 16 + 8);
				area.Add(part);
			}
			
			for (var i = 0; i < 8; i++) {
				var part = new ParticleEntity(Particles.Plank());
						
				part.Position = new Vector2(x * 16 + 8, y * 16);
				part.Particle.Scale = Random.Float(0.4f, 0.8f);
				
				area.Add(part);
			}

			Engine.Instance.Freeze = 0.1f;
			Camera.Instance.Shake(2);
		}
	}
}