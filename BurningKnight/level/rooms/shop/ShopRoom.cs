using System;
using System.Collections.Generic;
using BurningKnight.assets.items;
using BurningKnight.entity;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using BurningKnight.level.biome;
using BurningKnight.level.entities;
using BurningKnight.level.entities.machine;
using BurningKnight.level.rooms.shop.sub;
using BurningKnight.level.rooms.special;
using BurningKnight.level.tile;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.util.geometry;
using Lens.entity;
using Lens.util;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.shop {
	public class ShopRoom : LockedRoom {
		public override void Paint(Level level) {
			if (LevelSave.BiomeGenerated is IceBiome) {
				var clip = Painter.Clip;
				Painter.Clip = null;
				Painter.Rect(level, this, 0, Tile.WallB);
				Painter.Clip = clip;
			}
			
			var scourged = Rnd.Chance(Run.Scourge + 1);
		
			if (Rnd.Chance(30)) {
				var t = Tiles.Pick(Tile.FloorC, Tile.FloorD);

				if (Rnd.Chance()) {
					Painter.FillEllipse(level, this, 3, t);
				} else {
					Painter.Fill(level, this, 3, t);
				}
				
				if (Rnd.Chance()) {
					t = Tiles.Pick(Tile.FloorC, Tile.FloorD);

					if (Rnd.Chance()) {
						Painter.FillEllipse(level, this, 4, t);
					} else {
						Painter.Fill(level, this, 4, t);
					}
				}
			}

			if (Rnd.Chance()) {
				if (Rnd.Chance()) {
					PaintTunnel(level, Tiles.RandomFloor(), GetCenterRect(), true);
					PaintTunnel(level, Tiles.RandomNewFloor(), GetCenterRect());
				} else {
					PaintTunnel(level, Tiles.RandomNewFloor(), GetCenterRect(), Rnd.Chance());
				}
			}

			if (GameSave.IsTrue("sk_enraged")) {
				var rp = GetCenter();
				var rsk = new ShopKeeper();

				level.Area.Add(rsk);
				rsk.Center = new Vector2(rp.X * 16 + 8, rp.Y * 16 + 16);
				
				return;
			}
			
			var stands = ValidateStands(level, GenerateStands());

			if (stands.Count < 4) {
				Painter.Fill(level, this, 1, Tile.FloorD);
				Paint(level);
				return;
			}
			
			if (scourged) {
				var f = Tiles.RandomFloor();
				
				for (var m = Left + 1; m <= Right - 1; m++) {
					for (var j = Top + 1; j <= Bottom - 1; j++) {
						var t = level.Get(m, j);
						
						if (t.IsPassable() && t != f) {
							level.Set(m, j, Tile.EvilFloor);
						}
					}
				}
			}
			
			var pool = Items.GeneratePool(Items.GetPool(ItemPool.Shop));
			var consumablePool = Items.GeneratePool(Items.GetPool(ItemPool.ShopConsumable));

			var con = Math.Max(1, Math.Ceiling(stands.Count / 4f));
			var i = 0;
			var g = (Run.Depth == 5 && Run.Loop == 0 && LevelSave.GenerateMarket) ? Rnd.Int(stands.Count) : -1;

			foreach (var s in stands) {
				var stand = new ShopStand();
				level.Area.Add(stand);
				stand.Center = new Vector2(s.X * 16 + 8, s.Y * 16 + 8);

				var id = g == i ? "bk:bucket" : Items.GenerateAndRemove(i < con && consumablePool.Count > 0 ? consumablePool : pool, null, true);
				var item = Items.CreateAndAdd(id, level.Area, false);

				if (scourged) {
					item.Scourged = true;
				}
				
				stand.SetItem(item, null);

				if (pool.Count == 0) {
					break;
				}

				i++;
			}

			var p = stands[Rnd.Int(stands.Count)];
			var sk = new ShopKeeper();

			level.Area.Add(sk);
			sk.Center = new Vector2(p.X * 16 + 8, p.Y * 16 + 16);

			// Painter.DrawLine(level, new Dot(Left + 1, Top + 1), new Dot(Right - 1, Top + 1), Tiles.RandomFloor());
			
			var points = new List<Point>();

			for (var x = Left + 2; x < Right - 1; x++) {
				var found = false;

				foreach (var c in Connected.Values) {
					if (c.X == x && c.Y == Top) {
						found = true;
						break;
					}
				}
				
				if (!found) {
					points.Add(new Point(x, Top + 2));
				}
			}

			var props = new List<Entity> {
				new Gramophone()
			};

			if (Rnd.Chance(40 + Run.Luck * 7)) {
				props.Add(new RerollMachine());
			}

			if (Rnd.Chance(40 + Run.Luck * 7)) {
				props.Add(new VendingMachine());
			}

			foreach (var prop in props) {
				var pl = points[Rnd.Int(points.Count)];
				points.Remove(pl);
				
				level.Area.Add(prop);
				prop.CenterX = pl.X * 16 + 8 + Rnd.Int(-4, 4);
				prop.Bottom = pl.Y * 16;

				if (points.Count == 0) {
					break;
				}
			}

			var tt = Tiles.RandomFloor();
			
			Painter.Call(level, this, 1, (x, y) => {
				if (level.Get(x, y).Matches(Tile.SpikeOffTmp, Tile.SensingSpikeTmp, Tile.Chasm, Tile.Lava)) {
					level.Set(x, y, tt);
				}
			});

			var letters = new[] {'a', 'b', 'c'};
			var spr = $"mat_{letters[Rnd.Int(letters.Length)]}";
			
			foreach (var pair in Connected) {
				if (pair.Key is SubShopRoom) {
					continue;
				}

				var door = pair.Value;
				var mat = new SlicedProp(spr, Layers.Entrance);
				level.Area.Add(mat);

				if (door.X == Left) {
					PlaceSign(level, new Vector2(door.X * 16 - 8, door.Y * 16 - 5));
					mat.Center = new Vector2(door.X * 16 - 8, door.Y * 16 + 8);
				} else if (door.X == Right) {
					PlaceSign(level, new Vector2(door.X * 16 + 24, door.Y * 16 - 5));
					mat.Center = new Vector2(door.X * 16 + 24, door.Y * 16 + 8);
				} else if (door.Y == Top) {
					mat.Center = new Vector2(door.X * 16 + 8, door.Y * 16 - 8);
				} else {
					mat.Center = new Vector2(door.X * 16 + 8, door.Y * 16 + 24);
				}
			}
		}

		private void PlaceSign(Level level, Vector2 where) {
			var sign = new ShadowedProp("shop_sign");
			level.Area.Add(sign);
			sign.BottomCenter = where;
		}

		protected List<Point> ValidateStands(Level level, List<Point> stands) {
			var list = new List<Point>();

			for (var i = 0; i < stands.Count; i++) {
				var found = false;
				var p = stands[i];
				var t = level.Get(p.X, p.Y);
				
				if (t.IsWall() || t == Tile.Chasm) {
					continue;
				}
				
				for (var j = i + 1; j < stands.Count; j++) {
					var s = stands[j];

					// Check if 2 stands are on the same tile
					if (s.X == p.X && s.Y == p.Y) {
						found = true;
						break;
					}

					// Check if 2 stands are too close
					foreach (var d in MathUtils.Directions) {
						if (s.X + (int) d.X == p.X && s.Y + (int) d.Y == p.Y) {
							found = true;
							break;
						}
					}
				}

				if (!found) {
					list.Add(p);
				}
			}
			
			return list;
		}

		protected virtual List<Point> GenerateStands() {
			var list = new List<Point>();

			var sides = new bool[4];
			var count = 0;

			while (count < 2) {
				for (var i = 0; i < 4; i++) {
					if (Rnd.Chance(80)) {
						count++;
						sides[i] = true;
					}
				}
			}

			for (var x = Left + (Rnd.Chance() ? 2 : 3); x < Right - 2; x += 2) {
				if (sides[0]) {
					list.Add(new Point(x, Top + 3));
				}

				if (sides[1]) {
					list.Add(new Point(x, Bottom - 2));
				}
			}
			
			for (var y = Top + (Rnd.Chance() ? 3 : 4); y < Bottom - 2; y += 2) {
				if (sides[2]) {
					list.Add(new Point(Left + 2, y));
				}

				if (sides[3]) {
					list.Add(new Point(Right - 2, y));
				}
			}

			return list;
		}

		public override void SetupDoors(Level level) {
			foreach (var door in Connected) {
				door.Value.Type = door.Key is SubShopRoom || (Run.Depth == 5 && Run.Loop == 0 && LevelSave.GenerateMarket) ? DoorPlaceholder.Variant.Enemy : DoorPlaceholder.Variant.Shop;
			}
		}
		
		public override bool CanConnect(Connection direction) {
			return true;
		}
	}
}