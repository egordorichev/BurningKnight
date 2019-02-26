using BurningKnight;
using BurningKnight.entity.creature.inventory;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.entity.level;
using BurningKnight.entity.level.entities;
using BurningKnight.entity.level.entities.chest;
using BurningKnight.entity.level.rooms;
using BurningKnight.entity.level.save;
using BurningKnight.game.input;
using BurningKnight.physics;
using BurningKnight.ui;
using BurningKnight.util;

namespace BurningKnight.game.state {
	public class LoadState : State {
		public static bool FromSelect;

		private static List<string> Jokes = new List<>(Arrays.AsList("Please stand by", "Press X", "Generating trouble", "Looking cool", "Get em!", "Terraforming mars", "Dodge this!", "I think that knight wanted to say something",
			"Are we there yet?", "Generating secrets", "Hiding secrets", "I'm hungry", "/!\\ /!\\ /!\\", "Saving is important", "Cooling down", "Heating up", "Adding some drama", "Melee weapons can reflect bullets", "You better get digging",
			"QWERTY", "F2", "Generating generators", "Waking up", "Deleting the saves", "Preparing to start", "Looking for an excuse", "asdasdasd", "Automatically synchronizing cardinal grammeters", "Reducing sinusoidal repleneration",
			"Fromaging the bituminous spandrels", "Join our discord!", "Reticulating splines", "Calculating Math.PI", "Inventing the wheel", "Adding some oil", "Recruiting robot hamsters", "Generating buttons", "Installing deinstallers",
			"Thinking", "I like pizza", "That fight tho", "Be careful", "Almost RIP", "@egordorichev", "Dog food", "Always wear dry socks", "Its your lucky day", "Press F to pay respect", "Let's do this", "Let's go", "YOOOOOO", "Go go go",
			"Делаем вид что это что-то значит", "Loading terrain", "Building terrain", "Googling", "Help me", "SOS", "Are we lost?", "Spooooky", "On fire", "It's magic time", "That joke tho", "Settings things on fire", "Preparing to explode",
			"Installing linux", "Erasing data", "Generating a joke"));

		public static bool Generating;
		private float Al;
		private bool Error;
		private string ErrorString;
		private float Ew;
		private string Joke;
		private float Progress;
		private bool Ready = false;
		private bool RunM;
		private bool Second;
		private bool Third;

		public override void Init() {
			Joke = Jokes.Get(new Java.Util.Random().NextInt(Jokes.Size()));
			UiInventory.JustUsed = 0;
			Generating = false;
			Dungeon.DarkR = Dungeon.MAX_R;
			Dungeon.Dark = 1;
			Dungeon.Grayscale = 0;

			if (Ui.Ui != null) Ui.Ui.Healthbars.Clear();

			Dungeon.SetBackground(new Color(0, 0, 0, 1));
		}

		public override void Update(float Dt) {
			if (Ready) {
				Dungeon.Game.SetState(new InGameState());

				return;
			}

			if (Input.Instance.WasPressed("F")) InGameState.Horn();

			if (Input.Instance.WasPressed("pause")) InGameState.TriggerPause = true;

			Progress += Version.Debug ? Dt * 1f : Dt * 0.3f;

			if (RunM) {
				RunM = false;
				RunMain();
			}
		}

		private void ShowError(string Err) {
			Log.Error(Err);
			Error = true;
			ErrorString = Err;
			Graphics.Layout.SetText(Graphics.Medium, ErrorString);
			Ew = Graphics.Layout.Width / 2;
			Dungeon.Ui.Add(new UiButton("start_new_game", Display.UI_WIDTH / 2, Display.UI_HEIGHT / 3) {

		public override void OnClick() {
			Dungeon.NewGame();
		}
	});
}

private void RunThread() {
HandmadeRoom.Init();
Dungeon.Speed = 1f;
Dungeon.Ui.Destroy();
Dungeon.Area.Destroy();
Exit.Instance = null;
Player.Ladder = null;
Level.GENERATED = false;
Shopkeeper.Instance = null;
World.Init();
Player.All.Clear();
Mob.All.Clear();
ItemHolder.All.Clear();
Chest.All.Clear();
Mimic.All.Clear();
PlayerSave.All.Clear();
LevelSave.All.Clear();
if (FromSelect) {
	FromSelect = false;
	Thread Thread = new Thread(new Runnable {

		public override void Run() {
		PlayerSave.Generate();
		Dungeon.Area.Remove(Player.Instance);
		SaveManager.Save(SaveManager.Type.PLAYER,
		false);
		Dungeon.LastDepth = Dungeon.Depth;
		Dungeon.Depth = ItemSelectState.Depth;
		RunM = true;
	}
});
Thread.SetPriority(1);
Thread.Run();
} else {
RunMain();
}
}
private void RunMain() {
Thread Thread = new Thread(new Runnable() {
public override void Run() {
Level Lvl = Level.ForDepth(Dungeon.Depth - 1);
bool Error;
try {
Error = !SaveManager.Load(SaveManager.Type.GAME);
} catch (Exception) {
Log.Report(E);
Error = true;
}
if (Error) {
ShowError("Failed to load game!");
return;
}
try {
Error = !SaveManager.Load(SaveManager.Type.LEVEL);
} catch (Exception) {
Log.Report(E);
Error = true;
}
if (Error) {
ShowError("Failed to load level!");
return;
}
Dungeon.Area.Add(new Camera());
try {
Error = !SaveManager.Load(SaveManager.Type.PLAYER);
} catch (Exception) {
Log.Report(E);
Error = true;
}
if (Error) {
ShowError("Failed to load player!");
return;
}
Dungeon.Level.LoadPassable();
Dungeon.Level.AddPhysics();
InGameState.ToPlay = Dungeon.Level.GetMusic();
InGameState.ResetMusic = Dungeon.Depth == 1 || !Dungeon.Level.Same(Lvl);
if (Player.Instance == null) {
ShowError("Failed to load player!");
Dungeon.NewGame();
return;
}
PathFinder.SetMapSize(Level.GetWidth(), Level.GetHeight());
Log.Info("Loading done!");
UiBanner Banner = new UiBanner();
Banner.Text = Dungeon.Level.FormatDepth();
Dungeon.Ui.Add(Banner);
Dungeon.BuildDiscordBadge();
Third = true;
}
});
Thread.SetPriority(1);
Thread.Run();
}
public override void RenderUi() {
RenderPortal();
if (!Second) {
Al += Gdx.Graphics.GetDeltaTime() * 3;
if (Al >= 1f) {
Al = 1;
this.Second = true;
RunThread();
}
}
if ((!Generating || Progress >= 1.0f) && this.Third) {
Al -= Gdx.Graphics.GetDeltaTime() * 3;
if (Al <= 0) {
Al = 0;
this.Ready = true;
}
}
if (Error) {
Graphics.Print(this.ErrorString, Graphics.Medium, Display.UI_WIDTH / 2 - Ew, (Display.UI_HEIGHT - 16) / 2 - 8);
Dungeon.Ui.Render();
} else if (Generating) {
int I = (int) MathUtils.Clamp(0, 100, Math.Round(Progress * 100));
Graphics.Medium.SetColor(1, 1, 1, Al);
Graphics.Print("Generating... " + I + "%", Graphics.Medium, (Display.UI_HEIGHT - 16) / 2 - 8);
Graphics.Medium.SetColor(1, 1, 1, 1);
Graphics.Small.SetColor(1, 1, 1, Al);
Graphics.Print(Joke, Graphics.Small, (Display.UI_HEIGHT - 16) / 2 - 16);
Graphics.Small.SetColor(1, 1, 1, 1);
}
Ui.Ui.RenderCursor();
}
}
}