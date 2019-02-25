using BurningKnight.core.assets;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.level {
	public class Terrain {
		static Terrain() {
			Flags[CHASM] = HOLE;
			Flags[DIRT] = PASSABLE | LIQUID_LAYER;
			Flags[GRASS] = PASSABLE | BURNS | LIQUID_LAYER;
			Flags[FLOOR_A] = PASSABLE;
			Flags[WALL] = SOLID | HIGH | BREAKS_LOS;
			Flags[CRACK] = SOLID | HIGH | BREAKS_LOS;
			Flags[WATER] = PASSABLE | LIQUID_LAYER;
			Flags[VENOM] = LIQUID_LAYER;
			Flags[WALL_SIDE] = 0;
			Flags[FLOOR_B] = PASSABLE | BURNS;
			Flags[FLOOR_C] = PASSABLE;
			Flags[FLOOR_D] = PASSABLE;
			Flags[LAVA] = LIQUID_LAYER;
			Flags[TABLE] = SOLID | HIGH;
			Flags[ICE] = PASSABLE | LIQUID_LAYER;
			Flags[DRY_GRASS] = PASSABLE | BURNS | LIQUID_LAYER;
			Flags[HIGH_GRASS] = PASSABLE | BURNS | LIQUID_LAYER;
			Flags[HIGH_DRY_GRASS] = PASSABLE | BURNS | LIQUID_LAYER;
			Flags[OBSIDIAN] = PASSABLE | LIQUID_LAYER;
			Flags[EMBER] = PASSABLE | LIQUID_LAYER;
			Flags[COBWEB] = PASSABLE | LIQUID_LAYER | BURNS;
			Flags[EXIT] = PASSABLE | LIQUID_LAYER;
			Flags[PORTAL] = PASSABLE | LIQUID_LAYER;
			Flags[DISCO] = PASSABLE;
			Colors[CHASM] = Color.ValueOf("#000000");
			Colors[DIRT] = Color.ValueOf("#8a4836");
			Colors[GRASS] = Color.ValueOf("#33984b");
			Colors[HIGH_GRASS] = Color.ValueOf("#33984b");
			Colors[DRY_GRASS] = Color.ValueOf("#e69c69");
			Colors[HIGH_DRY_GRASS] = Color.ValueOf("#e69c69");
			Colors[OBSIDIAN] = Color.ValueOf("#2a2f4e");
			Colors[WATER] = Color.ValueOf("#0098dc");
			Colors[LAVA] = Color.ValueOf("#ff5000");
			Colors[EXIT] = Color.ValueOf("#424c6e");
			Colors[PORTAL] = Color.ValueOf("#424c6e");
			Colors[TABLE] = Color.ValueOf("#f6ca9f");
			Colors[DISCO] = Color.ValueOf("#ff0000");
			Colors[VENOM] = Color.ValueOf("#93388f");
			Floors[0][0] = Color.ValueOf("#657392");
			Floors[0][1] = Color.ValueOf("#bf6f4a");
			Floors[0][2] = Color.ValueOf("#92a1b9");
			Floors[0][3] = Color.ValueOf("#ffa214");
			Floors[1][0] = Color.ValueOf("#bf6f4a");
			Floors[1][1] = Color.ValueOf("#f5555d");
			Floors[1][2] = Color.ValueOf("#5d2c28");
			Floors[1][3] = Color.ValueOf("#f6ca9f");
			Floors[2][0] = Color.ValueOf("#8a4836");
			Floors[2][1] = Color.ValueOf("#891e2b");
			Floors[2][2] = Color.ValueOf("#5d2c28");
			Floors[2][3] = Color.ValueOf("#657392");
			Floors[4][0] = Color.ValueOf("#8a4836");
			Floors[4][1] = Color.ValueOf("#891e2b");
			Floors[4][2] = Color.ValueOf("#5d2c28");
			Floors[4][3] = Color.ValueOf("#657392");
			Floors[5][0] = Color.ValueOf("#1e6f50");
			Floors[5][1] = Color.ValueOf("#33984b");
			Floors[5][2] = Color.ValueOf("#5d2c28");
			Floors[5][3] = Color.ValueOf("#424c6e");
			Floors[4][0] = Color.ValueOf("#1e6f50");
			Floors[4][1] = Color.ValueOf("#33984b");
			Floors[4][2] = Color.ValueOf("#5d2c28");
			Floors[4][3] = Color.ValueOf("#424c6e");
			Floors[6][0] = Color.ValueOf("#1e6f50");
			Floors[6][1] = Color.ValueOf("#33984b");
			Floors[6][2] = Color.ValueOf("#5d2c28");
			Floors[6][3] = Color.ValueOf("#424c6e");
			Floors[0][0] = Color.ValueOf("#657392");
			Floors[0][1] = Color.ValueOf("#bf6f4a");
			Floors[0][2] = Color.ValueOf("#92a1b9");
			Floors[0][3] = Color.ValueOf("#ffa214");
		}

		public static byte WALL = 0;
		public static byte DIRT = 1;
		public static byte FLOOR_A = 2;
		public static byte WATER = 3;
		public static byte WALL_SIDE = 4;
		public static byte CHASM = 5;
		public static byte FLOOR_B = 6;
		public static byte LAVA = 7;
		public static byte GRASS = 8;
		public static byte TABLE = 9;
		public static byte EXIT = 10;
		public static byte FLOOR_C = 11;
		public static byte FLOOR_D = 12;
		public static byte CRACK = 13;
		public static byte ICE = 14;
		public static byte DRY_GRASS = 15;
		public static byte HIGH_GRASS = 16;
		public static byte HIGH_DRY_GRASS = 17;
		public static byte OBSIDIAN = 18;
		public static byte EMBER = 19;
		public static byte COBWEB = 20;
		public static byte VENOM = 21;
		public static byte DISCO = 22;
		public static byte PORTAL = 23;
		public static byte SIZE = 24;
		public static int[] Flags = new int[SIZE];
		public static Color[] Colors = new Color[SIZE];
		public static Color[][] Floors = new Color[4][9];
		public static int PASSABLE = 0x1;
		public static int SOLID = 0x2;
		public static int HOLE = 0x4;
		public static int HIGH = 0x8;
		public static int BURNS = 0x10;
		public static int BREAKS_LOS = 0x20;
		public static int LIQUID_LAYER = 0x40;
		public static byte Last;
		public static TextureRegion[] Dither = new TextureRegion[10];
		public static TextureRegion DirtPattern;
		public static TextureRegion GrassPattern;
		public static TextureRegion DryGrassPattern;
		public static TextureRegion WaterPattern;
		public static TextureRegion VenomPattern;
		public static TextureRegion LavaPattern;
		public static TextureRegion WallPattern;
		public static TextureRegion CrackPattern;
		public static TextureRegion ChasmPattern;
		public static TextureRegion CobwebPattern;
		public static TextureRegion DiscoPattern;
		public static TextureRegion EmberPattern;
		public static TextureRegion ObsidianPattern;
		public static TextureRegion IcePattern;
		public static TextureRegion[] Patterns = new TextureRegion[SIZE];
		public static TextureRegion[][] Edges = new TextureRegion[16][SIZE];
		public static TextureRegion[] Spread = new TextureRegion[16];
		public static TextureRegion[] Pooledge = new TextureRegion[16];
		public static TextureRegion[] Lavaedge = new TextureRegion[16];
		public static TextureRegion[] Dirtedge = new TextureRegion[16];
		public static TextureRegion[] Drygrassedge = new TextureRegion[16];
		public static TextureRegion[] Grassedge = new TextureRegion[16];
		public static TextureRegion[] Obedge = new TextureRegion[16];
		public static TextureRegion[] Webedge = new TextureRegion[16];
		public static TextureRegion[] WoodVariants = new TextureRegion[16];
		public static TextureRegion[] BadVariants = new TextureRegion[16];
		public static TextureRegion[] GoldVariants = new TextureRegion[16];
		public static TextureRegion[] FloorVariants = new TextureRegion[16];
		public static TextureRegion[] TableVariants = new TextureRegion[16];
		public static TextureRegion[] TopVariants = new TextureRegion[12];
		public static TextureRegion[][] WallTop = new TextureRegion[12][3];
		public static TextureRegion[] Sides = new TextureRegion[3];
		public static TextureRegion[][] ChasmSides = new TextureRegion[3][4];
		public static TextureRegion[][] Variants = new TextureRegion[16][SIZE];
		public static TextureRegion[] Decor;
		public static TextureRegion Exit;
		public static TextureRegion Entrance;
		public static TextureRegion DryGrassHigh;
		public static TextureRegion GrassHigh;
		private static int Lastt = -1;
		public static char[] Letters = { 'A', 'B', 'C', 'D' };
		public static char[] Coords = { 'n', 'e', 's', 'w' };
		public static int[] WallMap = { -1, -1, -1, 9, -1, -1, 0, 5, -1, 11, -1, 10, 2, 6, 1, -1 };
		public static int[] WallMapExtra = { -1, 7, 3, -1, 4, -1, -1, -1, 8, -1, -1, -1, -1, -1, -1, -1 };

		public static Color GetColor(byte T) {
			if (T < 0) {
				return Color.WHITE;
			} 

			if (T == Terrain.WALL || T == Terrain.CRACK) {
				return Level.Colors[Dungeon.Level.Uid];
			} 

			if (T == Terrain.FLOOR_A) {
				return Floors[Lastt][0];
			} else if (T == Terrain.FLOOR_B) {
				return Floors[Lastt][1];
			} else if (T == Terrain.FLOOR_C) {
				return Floors[Lastt][2];
			} else if (T == Terrain.FLOOR_D) {
				return Floors[Lastt][3];
			} 

			Color Color = Colors[T];

			if (Color != null) {
				return Color;
			} 

			return Color.WHITE;
		}

		public static byte RandomFloorNotLast() {
			byte L = Last;

			do {
				RandomFloor();
			} while (Last == L);

			return Last;
		}

		public static byte RandomFloor() {
			switch (Random.NewInt(3)) {
				case 0: 
				default:{
					Last = FLOOR_A;

					break;
				}

				case 1: {
					Last = FLOOR_B;

					break;
				}

				case 2: {
					Last = FLOOR_C;

					break;
				}
			}

			return Last;
		}

		public static Void LoadTextures(int Set) {
			if (Lastt == Set) {
				return;
			} 

			Lastt = Set;
			string Bm = "biome-" + Set;
			Log.Info("Loading biome " + Set);
			DirtPattern = Graphics.GetTexture("biome-gen-dirt pattern");
			GrassPattern = Graphics.GetTexture("biome-gen-grass pattern");
			DryGrassPattern = Graphics.GetTexture("biome-gen-dry pattern");
			WaterPattern = Graphics.GetTexture("biome-gen-water pattern");
			VenomPattern = Graphics.GetTexture("biome-gen-polluted pattern");
			LavaPattern = Graphics.GetTexture("biome-gen-lava pattern");
			WallPattern = Graphics.GetTexture(Bm + "-wall pattern");
			CrackPattern = Graphics.GetTexture(Bm + "-crack");
			EmberPattern = Graphics.GetTexture("biome-gen-coal pattern");
			CobwebPattern = Graphics.GetTexture("biome-gen-web pattern");
			ObsidianPattern = Graphics.GetTexture("biome-gen-ob pattern");
			IcePattern = Graphics.GetTexture("biome-gen-ice pattern");
			DiscoPattern = Graphics.GetTexture("biome-gen-disco pattern");
			Entrance = Graphics.GetTexture("props-entance");
			Exit = Graphics.GetTexture("props-exit");
			DryGrassHigh = Graphics.GetTexture("biome-gen-dry_grass_high");
			GrassHigh = Graphics.GetTexture("biome-gen-grass_high");
			Patterns[DIRT] = DirtPattern;
			Patterns[GRASS] = GrassPattern;
			Patterns[WALL] = WallPattern;
			Patterns[CRACK] = CrackPattern;
			Decor = { Graphics.GetTexture("props-decor_a"), Graphics.GetTexture("props-decor_b"), Graphics.GetTexture("props-decor_c"), Graphics.GetTexture("props-decor_d"), Graphics.GetTexture("props-decor_e"), Graphics.GetTexture("props-decor_f"), Graphics.GetTexture("props-decor_g") };

			for (int J = 0; J < 3; J++) {
				for (int I = 0; I < 12; I++) {
					WallTop[J][I] = Graphics.GetTexture(Bm + "-" + J + " top " + I);
				}
			}

			for (int I = 0; I < 3; I++) {
				Sides[I] = Graphics.GetTexture(Bm + "-side " + I);
			}

			for (int I = 0; I < 10; I++) {
				Dither[I] = Graphics.GetTexture("fx-dither-idle-" + string.Format("%02d", I));
			}

			for (int I = 0; I < 16; I++) {
				Spread[I] = Graphics.GetTexture("biome-gen-l" + Level.COMPASS[I]);
			}

			for (int I = 0; I < 16; I++) {
				Pooledge[I] = Graphics.GetTexture("biome-gen-pooledge" + Level.COMPASS[I]);
			}

			for (int I = 0; I < 16; I++) {
				Lavaedge[I] = Graphics.GetTexture("biome-gen-lavaedge" + Level.COMPASS[I]);
			}

			for (int I = 0; I < 16; I++) {
				Obedge[I] = Graphics.GetTexture("biome-gen-ob" + Level.COMPASS[I]);
			}

			for (int I = 0; I < 16; I++) {
				Webedge[I] = Graphics.GetTexture("biome-gen-web" + Level.COMPASS[I]);
			}

			for (int I = 0; I < 16; I++) {
				Grassedge[I] = Graphics.GetTexture("biome-gen-grassedge" + Level.COMPASS[I]);
			}

			for (int I = 0; I < 16; I++) {
				Drygrassedge[I] = Graphics.GetTexture("biome-gen-dry" + Level.COMPASS[I]);
			}

			for (int I = 0; I < 16; I++) {
				Dirtedge[I] = Graphics.GetTexture("biome-gen-dirtedge" + Level.COMPASS[I]);
			}

			for (int I = 0; I < 16; I++) {
				TableVariants[I] = Graphics.GetTexture(Bm + "-platform A" + Level.COMPASS[I]);
			}

			for (int I = 0; I < 16; I++) {
				WoodVariants[I] = Graphics.GetTexture(Bm + "-floor B " + string.Format("%02d", I + 1));
			}

			for (int I = 0; I < 16; I++) {
				FloorVariants[I] = Graphics.GetTexture(Bm + "-floor A " + string.Format("%02d", I + 1));
			}

			for (int I = 0; I < 16; I++) {
				GoldVariants[I] = Graphics.GetTexture(Bm + "-floor D " + string.Format("%02d", I + 1));
			}

			for (int I = 0; I < 16; I++) {
				BadVariants[I] = Graphics.GetTexture(Bm + "-floor C " + string.Format("%02d", I + 1));
			}

			for (int I = 0; I < 12; I++) {
				TopVariants[I] = Graphics.GetTexture(Bm + "-wall " + Letters[I / 4] + " " + string.Format("%02d", I % 4 + 1));
			}

			for (int I = 0; I < 4; I++) {
				ChasmSides[I] = new TextureRegion[3];

				for (int J = 0; J < 3; J++) {
					ChasmSides[I][J] = Graphics.GetTexture("biome-gen-edge_" + Coords[I] + "_" + (J + 1));
				}
			}

			ChasmPattern = Graphics.GetTexture("biome-gen-chasm_bg");
			Variants[FLOOR_B] = WoodVariants;
			Variants[FLOOR_A] = FloorVariants;
			Variants[TABLE] = TableVariants;
			Variants[FLOOR_C] = BadVariants;
			Variants[FLOOR_D] = GoldVariants;
			Edges[WATER] = Pooledge;
			Edges[VENOM] = Pooledge;
			Edges[ICE] = Pooledge;
			Edges[DIRT] = Dirtedge;
			Edges[LAVA] = Lavaedge;
			Edges[OBSIDIAN] = Dirtedge;
			Edges[COBWEB] = Webedge;
		}
	}
}
