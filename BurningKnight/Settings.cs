using BurningKnight.save;
using Lens;

namespace BurningKnight {
	public class Settings {
		public static bool Fullscreen;
		public static bool Vsync = true;
		public static bool Blood = true;
		public static bool Gore = true;
		public static bool Uisfx = true;
		public static bool Speedrun_mode = true;
		public static bool Speedrun_timer = true;
		public static int Quality = 2;
		public static float Screenshake = 0.7f;
		public static float Music = 0.5f;
		public static float Sfx = 0.75f;
		public static string Cursor = "cursor-standart";
		public static bool RotateCursor = true;
		public static int Side_art;
		public static float Freeze_frames = 0.5f;
		public static float Flash_frames = 0.5f;
		public static bool Vegan;
	
		public static void Load() {
			Fullscreen = GlobalSave.IsTrue("settings_fullscreen");
			Blood = GlobalSave.IsTrue("settings_blood");
			Uisfx = GlobalSave.IsTrue("settings_uisfx");
			Gore = GlobalSave.IsTrue("settings_gore");
			Vsync = GlobalSave.IsTrue("settings_vsync");
			Speedrun_mode = GlobalSave.IsTrue("settings_sm");
			Speedrun_timer = GlobalSave.IsTrue("settings_st");
			Side_art = GlobalSave.GetInt("settings_sa");
			Quality = GlobalSave.GetInt("settings_quality");
			Screenshake = GlobalSave.GetFloat("settings_screenshake");
			Sfx = GlobalSave.GetFloat("settings_sfx");
			Freeze_frames = GlobalSave.GetFloat("settings_frf");
			Flash_frames = GlobalSave.GetFloat("settings_ff");
			Music = GlobalSave.GetFloat("settings_music");
			Cursor = GlobalSave.GetString("settings_cursor", "cursor-standart");
			RotateCursor = GlobalSave.IsTrue("settings_rotate_cursor", true);
			Vegan = GlobalSave.IsTrue("settings_v", false);

			if (Fullscreen) {
				Engine.SetFullscreen();
			} else {
				Engine.SetWindowed(Display.Width * 3, Display.Height * 3);
			}

			Engine.Graphics.SynchronizeWithVerticalRetrace = Vsync;
		}

		public static void Save() {
			GlobalSave.Put("settings_fullscreen", Fullscreen);
			GlobalSave.Put("settings_blood", Blood);
			GlobalSave.Put("settings_uisfx", Uisfx);
			GlobalSave.Put("settings_gore", Gore);
			GlobalSave.Put("settings_vsync", Vsync);
			GlobalSave.Put("settings_sm", Speedrun_mode);
			GlobalSave.Put("settings_st", Speedrun_timer);
			GlobalSave.Put("settings_sa", Side_art);
			GlobalSave.Put("settings_frf", Freeze_frames);
			GlobalSave.Put("settings_ff", Flash_frames);
			GlobalSave.Put("settings_quality", Quality);
			GlobalSave.Put("settings_screenshake", Screenshake);
			GlobalSave.Put("settings_sfx", Sfx);
			GlobalSave.Put("settings_music", Music);
			GlobalSave.Put("settings_cursor", Cursor);
			GlobalSave.Put("settings_rotate_cursor", RotateCursor);
			GlobalSave.Put("settings_v", Vegan);
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
		}
	}
}