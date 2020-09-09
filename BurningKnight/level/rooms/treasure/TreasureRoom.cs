using System;
using System.Collections.Generic;
using BurningKnight.assets.items;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using BurningKnight.level.biome;
using BurningKnight.level.floors;
using BurningKnight.level.rooms.regular;
using BurningKnight.level.rooms.special;
using BurningKnight.level.tile;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.util.geometry;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.treasure {
	public class TreasureRoom : SpecialRoom {
		private List<ItemStand> stands = new List<ItemStand>();
		private List<Dot> standPositions = new List<Dot>();
		protected bool SpawnedBarrier;
		protected bool DisableBarrier;

		private bool scourged;
		
		public override void Paint(Level level) {
			if ((LevelSave.BiomeGenerated is JungleBiome && Rnd.Chance(50))) {
				var clip = Painter.Clip;
				Painter.Clip = null;
				Painter.Rect(level, this, 0, Tile.WallB);
				Painter.Clip = clip;
			}
			
			scourged = Rnd.Chance(Run.Scourge * 2 + 2);
			PaintInside(level);

			if (scourged) {
				for (var i = Left + 1; i <= Right - 1; i++) {
					for (var j = Top + 1; j <= Bottom - 1; j++) {
						if (level.Get(i, j) == Tile.FloorD) {
							level.Set(i, j, Tile.EvilFloor);
						}
					}
				}
			}
		}

		public virtual void PaintInside(Level level) {
			var c = GetCenter() * 16;
			
			PlaceStand(level, c - new Dot(32, 0));
			PlaceStand(level, c);
			PlaceStand(level, c + new Dot(32, 0));
		}

		protected void SetupStands(Level level) {
			if (stands.Count == 0) {
				return;
			}

			Func<ItemData, bool> filter = null;

			if (Rnd.Chance(10)) {
				filter = (i) => i.Type == ItemType.Weapon;
			}

			var id = Rnd.Int(stands.Count);
			var st = stands[id];

			var stnd = stands[id] = Rnd.Chance(30) ? new ShieldChoiceStand() : new HealChoiceStand();
			level.Area.Add(stnd);
			stnd.Center = st.Center;

			st.Done = true;
			
			var pool = Items.GeneratePool(Items.GetPool(ItemPool.Treasure), filter);

			foreach (var s in stands) {
				if (s is HealChoiceStand) {
					continue;
				}
				
				var item = Items.CreateAndAdd(Items.GenerateAndRemove(pool, null, true), level.Area, false);

				if (scourged) {
					item.Scourged = true;
				}
				
				s.SetItem(item, null);

				if (pool.Count == 0) {
					break;
				}
			}
			
			foreach (var s in standPositions) {
				if (level.Get(s.X, s.Y).Matches(TileFlags.Danger) || level.Get(s.X, s.Y, true).Matches(TileFlags.Danger)
				     || level.Get(s.X, s.Y).Matches(TileFlags.Solid) || level.Get(s.X, s.Y, true).Matches(TileFlags.Solid)) {
					
					Painter.Set(level, s, Tile.FloorD);
				}
			}
		}

		protected void PlaceStand(Level level, Dot where) {
			var stand = new SingleChoiceStand();
			level.Area.Add(stand);
			stand.Center = where * 16 + new Vector2(8, 8);
			
			stands.Add(stand);
			standPositions.Add(where);

			if (!DisableBarrier && !SpawnedBarrier && Rnd.Chance(20)) {
				SpawnedBarrier = true;
				var t = Tiles.Pick(Tile.SpikeOnTmp, Tile.Rock);

				Painter.Set(level, where + new Dot(-1, 0), t);
				Painter.Set(level, where + new Dot(1, 0), t);
				Painter.Set(level, where + new Dot(0, -1), t);
				Painter.Set(level, where + new Dot(0, 1), t);

				if (t == Tile.Rock) {
					t = Tile.Rock; // Tiles.Pick(Tile.Chasm, Tile.Rock);
					
					Painter.Set(level, where + new Dot(-1, 1), t);
					Painter.Set(level, where + new Dot(1, 1), t);
					Painter.Set(level, where + new Dot(1, -1), t);
					Painter.Set(level, where + new Dot(-1, -1), t);
				}
			}
		}

		public override void PaintFloor(Level level) {
			FloorRegistry.Paint(level, this, -1, true);
			Painter.Rect(level, this, 1, Tile.FloorD);
		}

		public override void SetupDoors(Level level) {
			var rude = Rnd.Chance(5); // Hehe
			
			foreach (var door in Connected.Values) {
				door.Type = rude ? DoorPlaceholder.Variant.Locked : DoorPlaceholder.Variant.Treasure;
			}
		}

		public override bool CanConnect(RoomDef r) {
			if (Run.Type != RunType.BossRush && LevelSave.BiomeGenerated is JungleBiome && !(r is HiveRoom)) {
				return false;
			}
			
			return base.CanConnect(r);
		}
		
		public override bool CanConnect(RoomDef r, Dot p) {
			if (p.X == Left + 1 || p.X == Right - 1 || p.Y == Top + 1 || p.Y == Bottom - 1) {
				return false;
			}
			
			return base.CanConnect(r, p);
		}

		public override bool ShouldSpawnMobs() {
			return false;// Random.Chance(10);
		}
	}
}