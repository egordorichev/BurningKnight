using System;
using BurningKnight.assets.particle;
using BurningKnight.entity.creature;
using BurningKnight.entity.events;
using BurningKnight.level;
using BurningKnight.level.tile;
using BurningKnight.state;
using Lens;
using Lens.entity.component;
using Lens.util;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.component {
	public class TileInteractionComponent : Component {
		public bool[] LastTouching;
		public bool[] Touching;

		public bool[] LastFlags;
		public bool[] Flags;

		public bool HasNoSupport;
		public bool HadNoSupport;
		public Vector2 LastSupportedPosition;

		public TileInteractionComponent() {
			Touching = new bool[(int) Tile.Total];
			LastTouching = new bool[(int) Tile.Total];
			
			LastFlags = new bool[8];
			Flags = new bool[8];
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Entity is Creature c && c.InAir()) {
				HasNoSupport = false;
				return;
			}
			
			for (int i = 0; i < Touching.Length; i++) {
				LastTouching[i] = Touching[i];
				Touching[i] = false;
			}
			
			for (int i = 0; i < Flags.Length; i++) {
				LastFlags[i] = Flags[i];
				Flags[i] = false;
			}

			HadNoSupport = HasNoSupport;
			HasNoSupport = true;
			
			ApplyForAllTouching(InspectTile);

			for (int i = 0; i < Touching.Length; i++) {
				CheckTile(i);
			}

			for (int i = 0; i < Flags.Length; i++) {
				CheckFlag(i);
			}

			CheckSupport();
		}

		private void InspectTile(int index, int x, int y) {
			var level = Run.Level;
			
			var tile = level.Tiles[index];
			var liquid = level.Liquid[index];

			if (tile > 0) {
				Touching[tile] = true;

				if (HasNoSupport) {
					var t = (Tile) tile;

					if (t != Tile.Chasm) {
						HasNoSupport = false;
						LastSupportedPosition = Entity.Position;
					}
				}
			}
					
			if (liquid > 0) {
				Touching[liquid] = true;
			}

			if (level.CheckFlag(index, Flag.Burning)) {
				Flags[Flag.Burning] = true;
			}
		}

		public void ApplyForAllTouching(Action<int, int, int> action) {
			var startX = (int) Math.Floor((Entity.X + Entity.Height / 2f) / 16f);
			var startY = (int) Math.Floor(Entity.Y / 16f);
			var endX = (int) Math.Floor(Entity.Right / 16f);
			var endY = (int) Math.Floor(Entity.Bottom / 16f);

			var level = Run.Level;
			
			for (int x = startX; x <= endX; x++) {
				for (int y = startY; y <= endY; y++) {
					var index = level.ToIndex(x, y);

					if (!level.IsInside(index)) {
						continue;
					}

					action(index, x, y);
				}
			}
		}

		private void CheckSupport() {
			if (!HadNoSupport && HasNoSupport && !Engine.EditingLevel) {
				Send(new LostSupportEvent {
					Who = Entity
				});

				Entity.Position = LastSupportedPosition;
				
				for (var i = 0; i < 4; i++) {
					var part = new ParticleEntity(Particles.Dust());
					
					part.Position = Entity.Center;
					part.Particle.Scale = Random.Float(0.4f, 0.8f);
					Entity.Area.Add(part);
				}
			}
		}

		private void CheckTile(int tile) {
			if (!LastTouching[tile] && Touching[tile]) {
				Send(new TileCollisionStartEvent {
					Who = Entity,
					Tile = (Tile) tile
				});
			} else if (LastTouching[tile] && !Touching[tile]) {
				Send(new TileCollisionEndEvent {
					Who = Entity,
					Tile = (Tile) tile
				});
			}
		}
		
		private void CheckFlag(int flag) {
			if (!LastFlags[flag] && Flags[flag]) {
				Send(new FlagCollisionStartEvent {
					Who = Entity,
					Flag = flag
				});
			} else if (LastFlags[flag] && !Flags[flag]) {
				Send(new FlagCollisionEndEvent {
					Who = Entity,
					Flag = flag
				});
			}
		}
	}
}