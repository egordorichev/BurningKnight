using BurningKnight.assets.particle.custom;
using BurningKnight.level.rooms.trap;
using BurningKnight.level.rooms.treasure;
using BurningKnight.level.tile;
using BurningKnight.state;
using Lens.entity;
using Lens.util;
using Lens.util.file;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.biome {
	public class CastleBiome : Biome {
		private enum Variant {
			Regular,
			Sand,
			Flooded, // todo: enable rain
			Webbed, // todo: spider enemies, spider cocons
			Snow,
			Gold
		}

		private static float[] variantChances = {
			5f,
			0.2f,
			0.1f,
			0.1f,
			0.1f,
			0.01f
		};

		private Variant variant;
		private bool raining;

		public CastleBiome() : base("Born to do rogueries", Biome.Castle, "castle_biome", new Color(14, 7, 27)) {
			if (Run.Depth > 0) {
				variant = (Variant) Rnd.Chances(variantChances);
				raining = Rnd.Chance(10);
			}
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			
			variant = (Variant) stream.ReadByte();
			raining = stream.ReadBoolean();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteByte((byte) variant);
			stream.WriteBoolean(raining);
		}

		public override string GetMusic() {
			if (Run.AlternateMusic) {
				return "chip";
			}

			return base.GetMusic();
		}

		public override Tile GetFilling() {
			return variant == Variant.Gold ? Tile.WallB : Tile.WallA;
		}

		public override void ModifyPainter(Painter painter) {
			base.ModifyPainter(painter);
			
			painter.Modifiers.Add((l, rm, x, y) => {
				if (rm is TrapRoom) {
					return;
				}
				
				var f = Run.Depth == 1;
				
				var r = (byte) (f ? Tiles.RandomFloor() : Tile.Chasm);
				
				if (l.Get(x, y, true) == Tile.Lava || (!(rm is TreasureRoom) && f && l.Get(x, y) == Tile.Chasm)) {
					var i = l.ToIndex(x, y);
					
					l.Liquid[i] = 0;
					l.Tiles[i] = r;
				}
			});
			
			if (variant == Variant.Sand) {
				painter.Dirt = 0.5f;
				painter.DirtTile = Tile.Sand;
				
				painter.Modifiers.Add((l, rm, x, y) => {
					if (l.Get(x, y, true) == Tile.Dirt) {
						l.Set(x, y, Tile.Sand);
					}
				});
			} else if (variant == Variant.Flooded) {
				painter.Water = 0.8f;
			} else if (variant == Variant.Webbed) {
				painter.Cobweb = 0.6f;
			} else if (variant == Variant.Gold) {
				Painter.AllGold = true;
			} else if (variant == Variant.Snow) {
				painter.Dirt = 0.5f;
				painter.DirtTile = Tile.Snow;

				painter.Modifiers.Add((l, rm, x, y) => {
					if (l.Get(x, y, true) == Tile.Dirt) {
						l.Set(x, y, Tile.Snow);
					}
				});
			}
		}

		public override string GetStepSound(Tile tile) {
			if (tile == Tile.FloorB) {
				return $"player_step_wood_{Rnd.Int(1, 4)}";
			}
			
			return base.GetStepSound(tile);
		}

		public override void Prepare() {
			base.Prepare();
			
			if (variant == Variant.Flooded || raining) {
				for (var i = 0; i < 40; i++) {
					Run.Level.Area.Add(new RainParticle());
				}
			}
		}
	}
}