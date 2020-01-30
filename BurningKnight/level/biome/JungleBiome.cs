using System.Collections.Generic;
using BurningKnight.assets.lighting;
using BurningKnight.level.builders;
using BurningKnight.level.rooms;
using BurningKnight.level.rooms.regular;
using BurningKnight.level.rooms.trap;
using BurningKnight.level.tile;
using BurningKnight.state;
using Lens.graphics;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.biome {
	public class JungleBiome : Biome {
		public JungleBiome() : base("Botanical Expedition", Biome.Jungle, "jungle_biome", new Color(30, 111, 80)) {}

		public override void Apply() {
			base.Apply();

			var v = 0.8f;
			Lights.ClearColor = new Color(v, v, v, 1f);
		}

		public override void ModifyRooms(List<RoomDef> rooms) {
			base.ModifyRooms(rooms);
			
			if (Run.Depth % 2 == 0) {
				rooms.Add(new HiveRoom());
			} else {
				rooms.Add(new JungleRoom());
				rooms.Add(new JungleRoom());
			}
		}

		public override void ModifyPainter(Painter painter) {
			base.ModifyPainter(painter);

			painter.Water = 0.4f;
			painter.Grass = 0.4f;
			painter.Dirt = 0f;
			
			painter.Modifiers.Add((l, rm, x, y) => {
				if (rm is TrapRoom) {
					return;
				}
				
				var f = Tiles.RandomFloor();
				
				if (l.Get(x, y, true) == Tile.Lava || l.Get(x, y).Matches(Tile.Chasm, Tile.SpikeOffTmp, Tile.SpikeOnTmp, Tile.SensingSpikeTmp, Tile.FireTrapTmp)) {
					var i = l.ToIndex(x, y);
					
					l.Liquid[i] = 0;
					l.Tiles[i] = (byte) f;
				}
			});
		}

		public override Builder GetBuilder() {
			return new LineBuilder();
		}

		public override int GetNumRegularRooms() {
			return 0;
		}
		
		public override int GetNumTrapRooms() {
			return 0;
		}

		public override bool HasTorches() {
			return false;
		}

		public override bool HasSpikes() {
			return true;
		}

		public override bool SpawnAllMobs() {
			return true;
		}

		public override bool HasPaintings() {
			return false;
		}

		public override bool HasTnt() {
			return false;
		}

		public override bool HasCobwebs() {
			return false;
		}

		public override bool HasBrekables() {
			return false;
		}

		public override bool HasPlants() {
			return true;
		}
	}
}