using BurningKnight.entity.level.save;
using Lens;

namespace BurningKnight {
	public class Settings {
		public static bool Fullscreen;
		public static bool Vsync = true;
		public static bool Blood = true;
		public static bool Gore = true;
		public static bool Uisfx = true;
		public static bool Borderless;
		public static bool Speedrun_mode = true;
		public static bool Speedrun_timer = true;
		public static int Quality = 2;
		public static float Screenshake = 0.7f;
		public static float Music = 0.5f;
		public static float Sfx = 0.75f;
		public static string Cursor = "cursor-standart";
		public static bool RotateCursor = true;
		public static int CursorId;
		public static int Side_art;
		public static float Freeze_frames = 0.5f;
		public static float Flash_frames = 0.5f;

		public static bool Vegan;
		// public const string[] Cursors = { "cursor-standart", "cursor-small", "cursor-rect", "cursor-corner", "cursor-sniper", "cursor-round-sniper", "cursor-cross", "cursor-nt", "native" };

		public static int GetCursorId(string Name) {
			for (var I = 0; I < Cursors.Length; I++)
				if (Cursors[I].Equals(Name))
					return I;

			return 0;
		}

		public static void Load() {
			Fullscreen = GlobalSave.IsTrue("settings_fullscreen");
			Blood = GlobalSave.IsTrue("settings_blood");
			Uisfx = GlobalSave.IsTrue("settings_uisfx");
			Gore = GlobalSave.IsTrue("settings_gore");
			Vsync = GlobalSave.IsTrue("settings_vsync");
			Speedrun_mode = GlobalSave.IsTrue("settings_sm");
			Speedrun_timer = GlobalSave.IsTrue("settings_st");
			Borderless = GlobalSave.IsTrue("settings_bl");
			Side_art = GlobalSave.GetInt("settings_sa");
			Quality = GlobalSave.GetInt("settings_quality");
			Screenshake = GlobalSave.GetFloat("settings_screenshake");
			Sfx = GlobalSave.GetFloat("settings_sfx");
			Dungeon.ColorBlind = GlobalSave.GetFloat("settings_cb");
			Freeze_frames = GlobalSave.GetFloat("settings_frf");
			Flash_frames = GlobalSave.GetFloat("settings_ff");
			Music = GlobalSave.GetFloat("settings_music");
			Cursor = GlobalSave.GetString("settings_cursor", "cursor-standart");
			RotateCursor = GlobalSave.IsTrue("settings_rotate_cursor", true);
			Vegan = GlobalSave.IsTrue("settings_v", false);
			Dungeon.FpsY = GlobalSave.IsTrue("settings_sf", false) ? 18 : 0;
			CursorId = GetCursorId(Cursor);

			if (Fullscreen)
				Gdx.Graphics.SetFullscreenMode(Gdx.Graphics.GetDisplayMode());
			else
				Gdx.Graphics.SetWindowedMode(Display.GAME_WIDTH * 3, Display.GAME_HEIGHT * 3);


			if (Borderless) Gdx.Graphics.SetUndecorated(Borderless);

			if (CursorId == Cursors.Length - 1) Gdx.Graphics.SetSystemCursor(Cursor.SystemCursor.Arrow);

			Gdx.Graphics.SetVSync(Vsync);
			Dungeon.TweenTimer(Speedrun_timer);
		}

		public static void Save() {
			GlobalSave.Put("settings_fullscreen", Fullscreen);
			GlobalSave.Put("settings_blood", Blood);
			GlobalSave.Put("settings_uisfx", Uisfx);
			GlobalSave.Put("settings_gore", Gore);
			GlobalSave.Put("settings_vsync", Vsync);
			GlobalSave.Put("settings_sm", Speedrun_mode);
			GlobalSave.Put("settings_st", Speedrun_timer);
			GlobalSave.Put("settings_bl", Borderless);
			GlobalSave.Put("settings_sa", Side_art);
			GlobalSave.Put("settings_frf", Freeze_frames);
			GlobalSave.Put("settings_ff", Flash_frames);
			GlobalSave.Put("settings_quality", Quality);
			GlobalSave.Put("settings_screenshake", Screenshake);
			GlobalSave.Put("settings_sfx", Sfx);
			GlobalSave.Put("settings_music", Music);
			GlobalSave.Put("settings_cursor", Cursor);
			GlobalSave.Put("settings_rotate_cursor", RotateCursor);
			GlobalSave.Put("settings_cb", Dungeon.ColorBlind);
			GlobalSave.Put("settings_v", Vegan);
			GlobalSave.Put("settings_sf", Dungeon.FpsY == 18);
		}

		public static void Generate() {
			GlobalSave.Put("settings_fullscreen", false);
			GlobalSave.Put("settings_blood", true);
			GlobalSave.Put("settings_uisfx", true);
			GlobalSave.Put("settings_gore", true);
			GlobalSave.Put("settings_vsync", true);
			GlobalSave.Put("settings_sm", false);
			GlobalSave.Put("settings_st", false);
			GlobalSave.Put("settings_bl", false);
			GlobalSave.Put("settings_sa", 0);
			GlobalSave.Put("settings_frf", 0.5f);
			GlobalSave.Put("settings_ff", 0.5f);
			GlobalSave.Put("settings_quality", 2);
			GlobalSave.Put("settings_screenshake", 0.3f);
			GlobalSave.Put("settings_music", 0.5f);
			GlobalSave.Put("settings_sfx", 0.75f);
			GlobalSave.Put("settings_cursor", "cursor-standart");
			GlobalSave.Put("settings_rotate_cursor", true);
			GlobalSave.Put("settings_cb", 0);
			GlobalSave.Put("settings_v", false);
			CursorId = GetCursorId(Cursor);
			Dungeon.TweenTimer(false);
		}
	}
}