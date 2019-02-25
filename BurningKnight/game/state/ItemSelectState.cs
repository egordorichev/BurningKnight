using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.entity.item.weapon.axe;
using BurningKnight.entity.item.weapon.dagger;
using BurningKnight.entity.item.weapon.gun;
using BurningKnight.entity.item.weapon.magic;
using BurningKnight.entity.item.weapon.spear;
using BurningKnight.entity.item.weapon.sword;
using BurningKnight.entity.level.save;
using BurningKnight.ui;
using BurningKnight.util;

namespace BurningKnight.game.state {
	public class ItemSelectState : State {
		private const string ALPHA_NUMERIC_STRING = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		private static List<Item> Melee = new List<>();
		private static List<Item> Ranged = new List<>();
		private static List<Item> Mage = new List<>();
		public static int Depth;
		private static UiTextInput Input;
		private static string Seed;
		private static float UiY;
		private static bool Picked;

		public static string RandomAlphaNumeric(int Count) {
			StringBuilder Builder = new StringBuilder();

			while (Count-- != 0) {
				var Character = (int) (Math.Random() * ALPHA_NUMERIC_STRING.Length());
				Builder.Append(ALPHA_NUMERIC_STRING.CharAt(Character));
			}

			return Builder.ToString();
		}

		public static int StringToSeed(string Str) {
			var Seed = 0;

			for (var I = 0; I < Str.Length(); I++) Seed += Str.CharAt(I);

			return Seed;
		}

		public override void Init() {
			base.Init();
			StartingItem.Hovered = null;
			Seed = RandomAlphaNumeric(8);
			SaveManager.Delete();
			Audio.Play("Void");
			Melee.Clear();
			Ranged.Clear();
			Mage.Clear();
			Mage.Add(new MagicMissileWand());
			var Ur = GlobalSave.IsTrue("unlocked_ranged");

			if (Ur) Ranged.Add(new Revolver());

			var Um = GlobalSave.IsTrue("unlocked_melee");

			if (Um) Melee.Add(new Sword());

			if (Player.Instance != null) {
				Dungeon.Area.Remove(Player.Instance);
				PlayerSave.Remove(Player.Instance);
				Player.Instance = null;
			}

			if (Achievements.Unlocked("UNLOCK_DAGGER")) Melee.Add(new Dagger());

			if (Achievements.Unlocked("UNLOCK_SPEAR")) Melee.Add(new Spear());

			if (Achievements.Unlocked("UNLOCK_AXE")) Ranged.Add(new Axe());

			Picked = false;

			for (var I = 0; I < Melee.Size(); I++) {
				var Item = new StartingItem();
				Item.Item = Melee.Get(I);
				Item.Name = Melee.Get(I).GetName();
				Item.Type = Player.Type.WARRIOR;
				Item.Y = Display.UI_HEIGHT / 2 - 48;
				Item.X = (Display.UI_WIDTH - Melee.Size() * 48) / 2 + I * 48 + 24;
				Dungeon.Ui.Add(Item);
			}

			for (var I = 0; I < Ranged.Size(); I++) {
				var Item = new StartingItem();
				Item.Item = Ranged.Get(I);
				Item.Name = Ranged.Get(I).GetName();
				Item.Type = Player.Type.RANGER;
				Item.Y = Display.UI_HEIGHT / 2;
				Item.X = (Display.UI_WIDTH - Ranged.Size() * 48) / 2 + I * 48 + 24;
				Dungeon.Ui.Add(Item);
			}

			for (var I = 0; I < Mage.Size(); I++) {
				var Item = new StartingItem();
				Item.Item = Mage.Get(I);
				Item.Name = Mage.Get(I).GetName();
				Item.Type = Player.Type.WIZARD;
				Item.Y = Display.UI_HEIGHT / 2 + 48;
				Item.X = (Display.UI_WIDTH - Mage.Size() * 48) / 2 + I * 48 + 24;
				Dungeon.Ui.Add(Item);

				if (I == 0) Dungeon.Ui.Select(Item);
			}

			Dungeon.Ui.Add(new UiButton("random", Display.UI_WIDTH / 2, Display.UI_HEIGHT / 2 - 48 * 2) {

		public override void OnClick() {
			base.OnClick();
			var R = Random.NewFloat(1);

			if (R < 0.333f || Melee.Size() == 0 && Ranged.Size() == 0)
				Pick(Mage.Get(Random.NewInt(Mage.Size() - 1)), Player.Type.WIZARD);
			else if (R < 0.666f || Melee.Size() == 0)
				Pick(Ranged.Get(Random.NewInt(Ranged.Size() - 1)), Player.Type.RANGER);
			else
				Pick(Melee.Get(Random.NewInt(Melee.Size() - 1)), Player.Type.WARRIOR);
		}
	});

	Input = (UiTextInput) Dungeon.Ui.Add(new UiTextInput("seed", Display.UI_WIDTH / 2, (int) (Display.UI_HEIGHT / 2 - 48 * 2.5f)) {
	public override int GetMaxLength() {
		return 8;
	}

	public override char Validate(char Ch) {
		if (!Character.IsLetterOrDigit(Ch)) return '\0';

		return Character.ToUpperCase(Ch);
	}
	});
	Ui.SaveAlpha = 0;
	Dungeon.White = 0;
	Dungeon.Dark = 1;
	Dungeon.DarkR = Dungeon.MAX_R;
	UiY = Display.UI_HEIGHT;
	Tween.To(new Tween.Task(0, 0.5f, Tween.Type.BACK_OUT) {
	public override float GetValue() {
		return UiY;
	}

	public override void SetValue(float Value) {
		UiY = Value;
	}
	});
	if (!Ur && !Um && Mage.Size() == 1) {
	internal FastPick(Mage.Get(0), Player.Type.WIZARD);
	}
}

public override void Destroy() {
base.Destroy();
Ui.SaveAlpha = 0;
}
public override void RenderUi() {
RenderPortal();
base.RenderUi();
Camera.Ui.Position.Y -= UiY;
Camera.Ui.Update();
Graphics.Batch.SetProjectionMatrix(Camera.Ui.Combined);
Dungeon.Ui.Render();
Graphics.Print("Pick starting item", Graphics.Medium, Display.UI_HEIGHT / 2 + 48 * 2);
Camera.Ui.Position.Y += UiY;
Camera.Ui.Update();
Graphics.Batch.SetProjectionMatrix(Camera.Ui.Combined);
Ui.Ui.RenderCursor();
}
public override void Update(float Dt) {
base.Update(Dt);
if (Input.Instance.WasPressed("F")) {
InGameState.Horn();
}
if (Input.Instance.WasPressed("pause")) {
InGameState.TriggerPause = true;
}
}
public static void Pick(Item Item, Player.Type Tp) {
if (Picked) {
return;
}
Picked = true;
Tween.To(new Tween.Task(Display.UI_HEIGHT, 0.15f, Tween.Type.QUAD_IN) {
public override float GetValue() {
return UiY;
}
public override void SetValue(float Value) {
UiY = Value;
}
public override void OnEnd() {
base.OnEnd();
FastPick(Item, Tp);
}
});
}
public static void FastPick(Item Item, Player.Type Tp) {
Player.ToSet = Tp;
GlobalSave.Put("last_class", Player.GetTypeId(Player.ToSet));
Player.StartingItem = Item;
LoadState.FromSelect = true;
Dungeon.Game.SetState(new LoadState());
if (Input != null) {
string String = Input.Input;
if (!String.IsEmpty()) {
Seed = String;
}
Input = null;
}
Random.SetSeed(Seed);
}
}
}