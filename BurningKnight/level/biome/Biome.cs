using System.Collections.Generic;
using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.state;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.biome {
	public class Biome {
		public const string Castle = "castle";
		public const string Hub = "hub";
		public const string Library = "library";
		public const string Desert = "desert";
		public const string Ice = "ice";
		public const string Jungle = "jungle";
		public const string Tech = "tech";
		
		public readonly string Music;
		public readonly string Id;
		public readonly string Tileset;
		public readonly Color Bg;

		public Biome(string music, string id, string tileset, Color bg) {
			Music = music;
			Id = id;
			Tileset = tileset;
			Bg = bg;
		}

		public bool IsPresent(string[] biomes) {
			return IsPresent(Id, biomes);
		}

		public static bool IsPresent(string id, string[] biomes) {
			if (biomes == null || biomes.Length == 0) {
				return true;
			}

			foreach (var b in biomes) {
				if (b == id) {
					return true;
				}
			}

			return false;
		}

		public virtual void Apply() {
			
		}

		public virtual void ModifyPainter(Painter painter) {
			
		}

		public virtual void ModifyRooms(List<RoomDef> rooms) {
			
		}

		public virtual Tile GetFilling() {
			return Tile.WallA;
		}

		public virtual bool HasCobwebs() {
			return true;
		}

		public virtual bool HasPaintings() {
			return true;
		}

		public virtual bool HasTorches() {
			return true;
		}

		public virtual bool HasSpikes() {
			return true;
		}

		public virtual bool HasBrekables() {
			return true;
		}

		public virtual bool HasTnt() {
			return true;
		}

		public virtual bool HasPlants() {
			return false;
		}
		
		public virtual int GetNumRegularRooms() {
			return Run.Depth + 2;
		}

		public virtual int GetNumTrapRooms() {
			return Rnd.Int(0, 2);
		}

		public virtual int GetNumSpecialRooms() {
			return 1;
		}

		public virtual int GetNumSecretRooms() {
			return Run.Depth <= 0 ? 0 : 1;
		}
	}
}