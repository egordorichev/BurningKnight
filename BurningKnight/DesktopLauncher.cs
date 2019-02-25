using System;
using BurningKnight;
using BurningKnight.state;
using Lens;

namespace burningknight {
	public class DesktopLauncher {
		internal class TestLauncher {
			[STAThread]
			public static void Main() {
				int scale = 2;

				using (var game = new BK(new InGameState(), $"Burning Knight {Engine.Version}: Burn, baby, burn!", Display.Width * scale, Display.Height * scale, false)) {
					game.Run();
				}
			}
		}
	}

	/*private const int SCALE = 3;
	private static List<string> Titles = new List<>(Arrays.AsList("Fireproof", "Might burn", "'Friendly' fire", "Get ready to burn", "Do you need some heat?", "BBQ is ready!", "Hot sales!", "AAAAAA", "It burns burns burns", "Not for children under -1", "Unhandled fire", "Chili music", "Fire trap", "On-fire", "Hot potatoo", "Is this loss?", "Also try Enter the Gungeon - @ArutyunovPaul", "Also try Nuclear Throne", "undefined", "If only you could talk to the enemies...", "It slit. Thank you, good knight - @_kaassouffle", "Why aren't you in fullscreen? - @Brastin", "Burning Knight. It's what's for dinner - @MysticJemLP", "so fire, so good - @somepx", "Please don't feed the goblins - @BigastOon", "Burn, baby, burn - @Hairic_Lilred", "Only you can prevent the absence of forest fire - @Hairic_Lilred", "If at first you don't success, just burn everything - @Hairic_Lilred", "Attention: may cause sick burns - @Hairic_Lilred", "What doesn't make you stronger kills you - @Hairic_Lilred", "Is that a plane? Is that a bird? It is a flying fire! - @Hairic_Lilred", "Of course we still love you", "Just read the instructions", "Your CPU is close to overheating", "This is fine", "Ice cool", "Overheat alarm was triggered", "Burn", "Being a Knight does not guarantee burning - @wombatstuff", "Burning does not guarantee becoming a knight - @wombatstuff", "The thing of fire - @wombatstuff", "Sick burn, knight - @wombatstuff", "Wombats are dangerous", "Stuff happens", "1000% screen shake", "Water, fire, air, and... chocolate!", "What can be cooler than fire?", "???", "!!!", "Ya feel lucky", "Today is the day", "Run, gobbo, run!", "Oszor is watching you", "Always wear dry socks", "The walls are shifting, don't get squeezed! - @Hairic_Lilred", "Procgen? More like PRO-cgen! - @Hairic_Lilred", "Turn up your firewall! - @Hairic_Lilred", "Generating new generation - @Hairic_Lilred", "But can you procedurally generate this?! - @Hairic_Lilred", "But can you procedurally generate titles?", "Now with more Math.random() than ever! - @Hairic_Lilred", "Who needs static level design when you have math? - @Hairic_Lilred", "Too hot to handle - @viza", "Hot as hell - @viza", "Go for the burn! - @viza", "Never gets the cold feet - @viza", "If you play with fire... - @viza", "Now with more pathos than ever!", "Now with less bugs than ever!", "Now with more code than ever!", "There is a throne, however it's made of metal and not uranium... - @Xist3nce_Dev", "No, we're not associated with the flaming bear - @Xist3nce_Dev", "Eeeeeeeenttteeeeerrr the game... wait what did you think I was gonna say? - @Xist3nce_Dev", "There are no shovels in this one, I swear - @Xist3nce_Dev", "Не ожидали?", "Flameberrry", "No relation to Evolver Analog - @Xist3nce_Dev", "You are not allowed to die", "This is a title mimic", "Penguins will take over the world!", "Don't crash, don't crash, don't crash!!!", "Is this a batterfly?", "Lets play", "That's lit! - @JackerDeluxe", "Remains functional up to 4000 degrees Kelvin - @s_nnnikki", "Caution: might contain knights - @Eiyeron", "/!\\ Do not throw oil at the knight! - @TRASEVOL_DOG", "/!\\ /!\\ /!\\", "xD", "All your knights are belong to us - @Telecoda", "It's dangerous to go alone, take this!", "The early bird gets two in the bush - @Hybrid_Games_", "Please keep hands and feet inside the chaos at all times - @bwalter_indie", "You just got fired! - @brephenson", "Note: This game contains flammables - @brephenson", "We didn't start the fire - @brephenson", "I'm a slow burner - @dollarone", "I'm on fire! - @dollarone", "The Knight is lava - @dollarone", "Hello fire, my old friend - @dollarone", "I'm here to talk with you again", "Hot n' spicy! - @dollarone", "GET OUT OF HERE!!! Or buy these DELICIOUS stones for 9.99 each - @DS100", "This it lit", "Вообще огонь", "0b11111100011", "0x7E3", "Take it easy", "Slow up, please", "Thy hath the sickest of burns! - @avivbeer", "Is this a game?", "Roses are red, violets are blue, water boils at 100°C and freezes at 0°C - @DSF100", "Burn CPU, BURN", "No one can defeat BK, why? READ THE LORE - @DSF100", "Please, read the instructions", ":bok: - @DSF100", "BK stands for Bonkey Kong - @DSF100", "BK is a legendary enemy, I hope that you will defeat him in a legendary way! - @DSF100", "discord.gg/xhNbrtx", "twitter.com/egordorichev", "twitter.com/rexcellentgames", "rexcellentgames.com", "Rex", "¯\\_(ツ)_/¯", "☉_☉", "⌐■-■", "Open fire", "In case of fire break the monitor", "In case of fire backup the saves", "You'd better get burning", "Why are you reading this?"));
	private static string[] BirthdayTitles = { "Happy burning!", "It's a good day to die hard", "Someone is not burning today", "Burning party", "Fire hard!", "Today is a special day", "I need a cake", "I hope the presents won't explode" };
	private static string[] Birthdays = { "06-29", "09-25", "02-21" };

	public static Void Main(string Arg) {
		if (!LockInstance(System.GetProperty("user.home") + File.Separator + ".burningknight_lock")) {
			Log.Error("Another instance of Burning Knight is already running, exiting");

			return;
		} 

		Thread.SetDefaultUncaughtExceptionHandler(new Thread.UncaughtExceptionHandler() {
			public override Void UncaughtException(Thread Thread, Throwable Throwable) {
				Throwable.PrintStackTrace();
				Crash.Report(Thread, Throwable);
				Gdx.App.Exit();
			}
		});
		LwjglApplicationConfiguration Config = new LwjglApplicationConfiguration();
		Dungeon.Title = "Burning Knight: " + GenerateTitle();
		Config.Title = Dungeon.Title;
		Config.Width = Display.GAME_WIDTH * SCALE;
		Config.Height = Display.GAME_HEIGHT * SCALE;
		Config.AddIcon("icon.png", Files.FileType.Internal);
		Config.AddIcon("icon32x32.png", Files.FileType.Internal);
		Config.AddIcon("icon128x128.png", Files.FileType.Internal);
		Config.Resizable = true;
		Config.Samples = 2;
		Config.BackgroundFPS = 0;
		Config.InitialBackgroundColor = Color.BLACK;
		Config.VSyncEnabled = true;
		Dungeon.Arg = Arg;
		new LwjglApplication(new Client(), Config) {
			public override Void Exit() {
				if (Assets.FinishedLoading) {
					base.Exit();
				} 
			}
		};
	}

	private static string GenerateTitle() {
		SimpleDateFormat Format = new SimpleDateFormat("MM-dd");

		if (Random.Chance(0.001f)) {
			Titles.Add("This title will never appear, strange?");
		} 

		if (Random.Chance(0.01f)) {
			Titles.Add("You feel lucky");
		} 

		string Extra = Titles.Get(Random.NewInt(Titles.Size()));
		Date Now = new Date();
		Calendar Current = Calendar.GetInstance();

		try {
			foreach (string Birthday in Birthdays) {
				Date B = Format.Parse(Birthday);
				Calendar BirthdayCal = Calendar.GetInstance();
				BirthdayCal.SetTime(B);
				Current.SetTime(Now);
				bool SameDay = BirthdayCal.Get(Calendar.DAY_OF_YEAR) == Current.Get(Calendar.DAY_OF_YEAR);

				if (SameDay) {
					Extra = BirthdayTitles[Random.NewInt(BirthdayTitles.Length)];

					break;
				} 
			}
		} catch (ParseException) {
			E.PrintStackTrace();
			Extra = "Houston, we have a problem!";
		}

		return Extra;
	}

	private static bool LockInstance(string LockFile) {
		try {
			File File = new File(LockFile);
			RandomAccessFile RandomAccessFile = new RandomAccessFile(File, "rw");
			FileLock FileLock = RandomAccessFile.GetChannel().TryLock();

			if (FileLock != null) {
				Runtime.GetRuntime().AddShutdownHook(new Thread() {
					public Void Run() {
						try {
							FileLock.Release();
							RandomAccessFile.Close();
							File.Delete();
						} catch (Exception) {
							E.PrintStackTrace();
							Log.Error("Unable to remove lock file: " + LockFile);
						}
					}
				});

				return true;
			} 
		} catch (Exception) {
			E.PrintStackTrace();
			Log.Error("Unable to create and/or lock file: " + LockFile);
		}

		return false;
	}
}*/
}
