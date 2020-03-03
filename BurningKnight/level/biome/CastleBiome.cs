using BurningKnight.level.rooms.trap;
using BurningKnight.level.rooms.treasure;
using BurningKnight.level.tile;
using BurningKnight.state;
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
			Webbed, // todo: spider enemies
			Gold
		}

		private static float[] variantChances = {
			1f,
			0.2f,
			0.1f,
			0.1f,
			0.01f
		};

		private Variant variant;

		public CastleBiome() : base("Born to do rogueries", Biome.Castle, "castle_biome", new Color(14, 7, 27)) {
			if (Run.Depth > 0) {
				variant = Variant.Gold; // (Variant) Rnd.Chances(variantChances);
			}
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			variant = (Variant) stream.ReadByte();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteByte((byte) variant);
		}

		public override string GetMusic() {
			if (Run.AlternateMusic) {
				return "chip";
			}

			return base.GetMusic();
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
				painter.Water = 0.6f;
			} else if (variant == Variant.Webbed) {
				painter.Cobweb = 0.6f;
			} else if (variant == Variant.Gold) {
				Painter.AllGold = true;
			}
		}

		public override string GetStepSound(Tile tile) {
			if (tile == Tile.FloorB) {
				return $"player_step_wood_{Rnd.Int(1, 4)}";
			}
			
			return base.GetStepSound(tile);
		}
	}
}