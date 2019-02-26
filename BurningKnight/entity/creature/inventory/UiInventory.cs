using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.entity.item.entity;
using BurningKnight.entity.item.key;
using BurningKnight.entity.level.save;
using BurningKnight.game.input;
using BurningKnight.ui;
using BurningKnight.util;

namespace BurningKnight.entity.creature.inventory {
	public class UiInventory : UiEntity {
		public static int JustUsed;
		public static UiInventory Instance;
		private static TextureRegion Heart = Graphics.GetTexture("ui-heart");
		private static TextureRegion Half = Graphics.GetTexture("ui-half_heart");
		private static TextureRegion HeartIron = Graphics.GetTexture("ui-heart_iron");
		private static TextureRegion HalfIron = Graphics.GetTexture("ui-half_heart_iron");
		private static TextureRegion HeartGolden = Graphics.GetTexture("ui-heart_golden");
		private static TextureRegion HalfGolden = Graphics.GetTexture("ui-half_heart_golden");
		private static TextureRegion Heart_bg = Graphics.GetTexture("ui-heart_bg");
		private static TextureRegion Hurt = Graphics.GetTexture("ui-hit_heart");
		private static TextureRegion Star = Graphics.GetTexture("ui-mana_star");
		private static TextureRegion Star_bg = Graphics.GetTexture("ui-star_bg");
		private static TextureRegion Star_change = Graphics.GetTexture("ui-mana_change");
		private static TextureRegion HalfStar = Graphics.GetTexture("ui-half_star");
		private static TextureRegion Defense = Graphics.GetTexture("ui-defense");
		private static TextureRegion Ammo_texture = Graphics.GetTexture("ui-ammo");
		private static TextureRegion Ammo_bg = Graphics.GetTexture("ui-ammo_bg");
		private static TextureRegion Ammo_change = Graphics.GetTexture("ui-ammo_hurt_bg");
		private static TextureRegion Key = Graphics.GetTexture("ui-key");
		private static TextureRegion Bomb = Graphics.GetTexture("ui-bomb");
		private static TextureRegion Gold = Graphics.GetTexture("ui-gold");
		private int Active;
		private float As = 1;
		private Item CurrentSlot;
		public bool Handled;
		private float Hp;
		private float Inva;
		private Inventory Inventory;
		private float Invm;
		private int LastAmmo;
		private int LastMana;
		private bool LastNotUsed;
		private float Mana;
		private TextureRegion Slot = Graphics.GetTexture("ui-inventory_slot");

		public UiInventory(Inventory Inventory) {
			_Init();
			this.Inventory = Inventory;
			Player.Instance.SetUi(this);
		}

		protected void _Init() {
			{
				IsSelectable = false;
			}
		}

		public override void Init() {
			Dungeon.Area.Add(new Camera());

			if (Instance != null) Instance.Done = true;

			Instance = this;
			Hp = Player.Instance.GetHp();
			Mana = Player.Instance.GetMana();
		}

		public override void Destroy() {
			base.Destroy();
			Done = true;
		}

		public Inventory GetInventory() {
			return Inventory;
		}

		public int GetActive() {
			return Active;
		}

		private void Validate(int A) {
			Active = Math.Abs(Active % 2);
		}

		public override void Update(float Dt) {
			if (Dungeon.Depth < 0 && Dungeon.Depth != -3) return;

			Depth = 1;

			if (Dungeon.Game.GetState().IsPaused() || Dialog.Active != null) return;

			for (var I = 0; I < Inventory.GetSize(); I++) {
				Item Item = Inventory.GetSlot(I);

				if (Item != null) Item.Update(Dt);
			}

			Handled = false;
			Active = Inventory.Active;

			if (!UiMap.Large) {
				if (Input.Instance.WasPressed("switch")) {
					Active = Active == 1 ? 0 : 1;
					Validate(Input.Instance.GetAmount());
					Player.Instance.PlaySfx("menu/moving");
				}
				else if (Input.Instance.WasPressed("scroll")) {
					Active = Active + Input.Instance.GetAmount();
					Player.Instance.PlaySfx("menu/moving");
					Validate(Input.Instance.GetAmount());
				}

				if (Input.Instance.WasPressed("bomb")) {
					var Count = Player.Instance.GetBombs();

					if (Count == 0) {
						Player.Instance.PlaySfx("item_nocash");
					}
					else {
						Player.Instance.SetBombs(Count - 1);
						var E = new BombEntity(Player.Instance.X + (Player.Instance.W - 16) / 2, Player.Instance.Y + (Player.Instance.H - 16) / 2).ToMouseVel();
						E.Owner = Player.Instance;
						var Player = Player.Instance;
						E.LeaveSmall = Player.LeaveSmall;
						Dungeon.Area.Add(E);
					}
				}

				if (Input.Instance.WasPressed("active")) {
					Item Item = Player.Instance.GetInventory().GetSlot(2);

					if (Item != null) {
						if (Item.CanBeUsed()) {
							Item.Use();

							if (Item.GetCount() == 0) Player.Instance.GetInventory().SetSlot(2, null);
						}
						else {
							Player.Instance.PlaySfx("item_nocash");
						}
					}
				}
			}

			CheckUse();

			if (Player.Instance != null) {
				var Hp = Player.Instance.GetHp();

				if (this.Hp > Hp)
					this.Hp = Hp;
				else if (this.Hp < Hp) this.Hp += Dt * 10;

				int Mana = Player.Instance.GetMana();

				if (this.Mana > Mana)
					this.Mana = Mana;
				else if (this.Mana < Mana) this.Mana += Dt * 20;
			}

			Inventory.Active = Active;
		}

		private void CheckUse() {
			if (Player.Instance != null && !Player.Instance.Freezed && !Handled && !Player.Instance.IsDead()) {
				if (CurrentSlot != null && Input.Instance.WasPressed("use")) {
					var Slot = CurrentSlot;
					Drop(Slot);
					CurrentSlot = null;
				}
				else {
					Item Slot = Inventory.GetSlot(Active);

					if (Slot != null) {
						if (Input.Instance.WasPressed("use")) {
							if (Slot.IsUseable() && Slot.CanBeUsed() && Slot.GetDelay() == 0 && !Player.Instance.IsRolling()) {
								Slot.SetOwner(Player.Instance);
								Slot.Use();
								JustUsed = 2;
							}
						}
						else if (Input.Instance.IsDown("use") && Slot.IsAuto() && Slot.GetDelay() == 0 && !Player.Instance.IsRolling()) {
							Slot.SetOwner(Player.Instance);
							Slot.Use();
							JustUsed = 2;
						}
					}
				}
			}
		}

		private void Drop(Item Slot) {
			Slot.DisableAutoPickup();

			if (Slot is BurningKey) Player.Instance.HasBkKey = false;

			Tween.To(new Tween.Task(0, 0.1f) {

		public override float GetValue() {
			return Slot.A;
		}

		public override void SetValue(float Value) {
			Slot.A = Value;
		}

		public override void OnEnd() {
			var Holder = new ItemHolder(Slot);
			Holder.X = (float) Math.Floor(Player.Instance.X) + (16 - Slot.GetSprite().GetRegionWidth()) / 2;
			Holder.Y = (float) Math.Floor(Player.Instance.Y) + (16 - Slot.GetSprite().GetRegionHeight()) / 2;
			Holder.VelocityToMouse();

			for (var I = 0; I < Inventory.GetSize(); I++) {
				Item It = Inventory.GetSlot(I);

				if (It == Slot) {
					Inventory.SetSlot(I, null);

					break;
				}
			}

			Dungeon.Area.Add(Holder);
			LevelSave.Add(Holder);
		}
	});
}

public override void Render() {
if ((Dungeon.Depth != -3 && Dungeon.Depth < 0) || Ui.HideUi || Player.Instance == null || Player.Instance.IsDead()) {
return;
}
Graphics.Batch.SetProjectionMatrix(Camera.Ui.Combined);
Graphics.Shape.SetProjectionMatrix(Camera.Ui.Combined);
float Y = -3;
float X = 32;
if (LastMana > Mana) {
Invm = 1.0f;
}
if (Invm > 0) {
Invm -= Gdx.Graphics.GetDeltaTime();
}
LastMana = (int) Mana;
int Hp = (int) Math.Floor(this.Hp);
int Iron = Player.Instance.GetIronHearts();
int Golden = Player.Instance.GetGoldenHearts();
float Invt = Player.Instance.GetInvt();
int Max = Player.Instance.GetHpMax();
int I;
for (I = 0;
; I < Math.Ceil(((float) Max) / 2); I++) {
float Yy = (float) ((Hp <= 2 && Hp - 2 >= I * 2 - 1) ? Math.Cos(((float) I) % 2 / 2 + Dungeon.Time * 15) * 2.5f : 0) + Y;
float S = 1f;
if (Iron == 0 && Golden == 0 && (Hp - 2 == I * 2 || Hp - 2 == I * 2 - 1)) {
S = (float) (1f + Math.Abs(Math.Cos(Dungeon.Time * 3) / 2.5f));
}
Graphics.Render((Invt > 0.7f || (Invt > 0.5f && Invt % 0.2f > 0.1f)) ? Hurt : Heart_bg, X + (I % 8) * 11 + 1 + Heart.GetRegionWidth() / 2, (float) (Yy + 9 + Heart.GetRegionHeight() / 2 + Math.Floor(I / 8) * 11), 0, Heart_bg.GetRegionWidth()
/ 2, Heart_bg.GetRegionHeight() / 2, false, false, S, S);
if (Hp - 2 >= I * 2) {
Graphics.Render(Heart, X + (I % 8) * 11 + 1 + Heart.GetRegionWidth() / 2, (float) (Yy + 9 + Math.Floor(I / 8) * 11 + Heart.GetRegionHeight() / 2), 0, Heart.GetRegionWidth() / 2, Heart.GetRegionHeight() / 2, false, false, S, S);
} else if (Hp - 2 >= I * 2 - 1) {
Graphics.Render(Half, X + (I % 8) * 11 + 1 + Heart.GetRegionWidth() / 2, (float) (Yy + 9 + Heart.GetRegionHeight() / 2 + Math.Floor(I / 8) * 11), 0, Heart.GetRegionWidth() / 2, Heart.GetRegionHeight() / 2, false, false, S, S);
}
}
int Nn = (int) (Math.Ceil(((float) Max) / 2) + Math.Ceil(((float) Iron) / 2));
for (; I < Nn; I++) {
float S = 1f;
if (Golden == 0 && Nn == I + 1) {
S = (float) (1f + Math.Abs(Math.Cos(Dungeon.Time * 3) / 2.5f));
}
Graphics.Render((Invt > 0.7f || (Invt > 0.5f && Invt % 0.2f > 0.1f)) ? Hurt : Heart_bg, X + (I % 8) * 11 + 1 + Heart.GetRegionWidth() / 2, (float) (Y + 9 + Heart.GetRegionHeight() / 2 + Math.Floor(I / 8) * 11), 0, Heart_bg.GetRegionWidth()
/ 2, Heart_bg.GetRegionHeight() / 2, false, false, S, S);
if (Max + Iron - 2 >= I * 2) {
Graphics.Render(HeartIron, X + (I % 8) * 11 + 1 + Heart.GetRegionWidth() / 2, (float) (Y + 9 + Math.Floor(I / 8) * 11 + Heart.GetRegionHeight() / 2), 0, Heart.GetRegionWidth() / 2, Heart.GetRegionHeight() / 2, false, false, S, S);
} else if (Max + Iron - 2 >= I * 2 - 1) {
Graphics.Render(HalfIron, X + (I % 8) * 11 + 1 + Heart.GetRegionWidth() / 2, (float) (Y + 9 + Heart.GetRegionHeight() / 2 + Math.Floor(I / 8) * 11), 0, Heart.GetRegionWidth() / 2, Heart.GetRegionHeight() / 2, false, false, S, S);
}
}
int Mm = (int) (Math.Ceil(((float) Max) / 2) + Math.Ceil(((float) Iron) / 2) + Math.Ceil(((float) Golden) / 2));
for (; I < Mm; I++) {
float S = 1f;
if (Mm == I + 1) {
S = (float) (1f + Math.Abs(Math.Cos(Dungeon.Time * 3) / 2.5f));
}
Graphics.Render((Invt > 0.7f || (Invt > 0.5f && Invt % 0.2f > 0.1f)) ? Hurt : Heart_bg, X + (I % 8) * 11 + 1 + Heart.GetRegionWidth() / 2, (float) (Y + 9 + Heart.GetRegionHeight() / 2 + Math.Floor(I / 8) * 11), 0, Heart_bg.GetRegionWidth()
/ 2, Heart_bg.GetRegionHeight() / 2, false, false, S, S);
if (Max + Iron + Golden - 2 >= I * 2) {
Graphics.Render(HeartGolden, X + (I % 8) * 11 + 1 + Heart.GetRegionWidth() / 2, (float) (Y + 9 + Math.Floor(I / 8) * 11 + Heart.GetRegionHeight() / 2), 0, Heart.GetRegionWidth() / 2, Heart.GetRegionHeight() / 2, false, false, S, S);
} else if (Max + Iron + Golden - 2 >= I * 2 - 1) {
Graphics.Render(HalfGolden, X + (I % 8) * 11 + 1 + Heart.GetRegionWidth() / 2, (float) (Y + 9 + Math.Floor(I / 8) * 11 + Heart.GetRegionHeight() / 2), 0, Heart.GetRegionWidth() / 2, Heart.GetRegionHeight() / 2, false, false, S, S);
}
}
Mm--;
Item Item = Player.Instance.GetInventory().GetSlot(Player.Instance.GetInventory().Active);
if (Item is Wand) {
int Mana = (int) this.Mana;
for (I = 0;
; I < Player.Instance.GetManaMax() / 2; I++) {
float S = 1f;
float Yy = (float) (Y + 10 + Math.Floor(Mm / 8) * 11);
bool Change = (Invm > 0.7f || (Invm > 0.5f && Invm % 0.2f > 0.1f));
Graphics.Render(Change ? Star_change : Star_bg, X + I * 11 + Star.GetRegionWidth() / 2 + (Change ? -1 : 0), Yy + 8 + Star.GetRegionHeight() / 2 + (Change ? -1 : 0), 0, Star.GetRegionWidth() / 2, Star.GetRegionHeight() / 2, false, false, S,
S);
if (Mana - 2 >= I * 2) {
Graphics.Render(Star, X + I * 11 + Star.GetRegionWidth() / 2, Yy + 8 + Star.GetRegionHeight() / 2, 0, Star.GetRegionWidth() / 2, Star.GetRegionHeight() / 2, false, false, S, S);
} else if (Mana - 2 >= I * 2 - 1) {
Graphics.Render(HalfStar, X + I * 11 + Star.GetRegionWidth() / 2, Yy + 8 + Star.GetRegionHeight() / 2, 0, Star.GetRegionWidth() / 2, Star.GetRegionHeight() / 2, false, false, S, S);
}
}
} else if (Item is Gun) {
int Ammo = ((Gun) Item).GetAmmoLeft();
Max = ((Gun) Item).AmmoMax;
if (LastAmmo < Ammo) {
Inva = 1.0f;
}
LastAmmo = Ammo;
if (Inva > 0) {
Inva -= Gdx.Graphics.GetDeltaTime();
}
for (I = 0;
; I < Max; I++) {
float S = 1f;
float Yy = (float) (Y + 10 + Math.Floor(Mm / 8) * 11);
bool Change = false;
Graphics.Render(Change ? Ammo_change : Ammo_bg, X + I * 3 + Ammo_texture.GetRegionWidth() / 2, Yy + 8 + Ammo_texture.GetRegionHeight() / 2, 0, Ammo_texture.GetRegionWidth() / 2, Ammo_texture.GetRegionHeight() / 2, false, false, S, S);
if (I < Ammo) {
Graphics.Render(Ammo_texture, X + I * 3 + Ammo_texture.GetRegionWidth() / 2 + 1, Yy + 9 + Ammo_texture.GetRegionHeight() / 2, 0, Ammo_texture.GetRegionWidth() / 2, Ammo_texture.GetRegionHeight() / 2, false, false, S, S);
}
}
}
float By = 24 + 8;
Graphics.Render(Bomb, 4, By);
Graphics.Render(Key, 4, By + 12);
Graphics.Render(Gold, 4, By + 24);
Graphics.Print(Player.Instance.GetBombs() + "", Graphics.Small, 16, By);
Graphics.Print(Player.Instance.GetKeys() + "", Graphics.Small, 16, By + 12);
Graphics.Print(Player.Instance.GetMoney() + "", Graphics.Small, 16, By + 24);
Item = Player.Instance.GetInventory().GetSlot(2);
Graphics.Render(Slot, 4, 4);
if (Item != null) {
TextureRegion Region = Item.GetSprite();
int W = Region.GetRegionWidth();
int H = Region.GetRegionHeight();
if (Item.GetDelay() > 0) {
if (LastNotUsed) {
LastNotUsed = false;
Tween.To(new Tween.Task(1.5f, 0.1f) {
public override float GetValue() {
return As;
}
public override void SetValue(float Value) {
As = Value;
}
public override void OnEnd() {
Tween.To(new Tween.Task(1f, 1f, Tween.Type.ELASTIC_OUT) {
public override float GetValue() {
return As;
}
public override void SetValue(float Value) {
As = Value;
}
});
}
});
}
Graphics.Batch.SetColor(0.1f, 0.1f, 0.1f, 1);
Graphics.Render(Region, 4 + (24 - W) / 2 + W / 2, 4 + (24 - Region.GetRegionHeight()) / 2 + H / 2, 0, W / 2, H / 2, false, false, 2 - As, As);
Graphics.Batch.SetColor(1, 1, 1, 1);
Region.SetRegionWidth((int) (W * (1f - Item.GetDelay() / Item.GetUseTime())));
Graphics.Render(Region, 4 + (24 - W) / 2 + W / 2, 4 + (24 - Region.GetRegionHeight()) / 2 + H / 2, 0, W / 2, H / 2, false, false, 2 - As, As);
Region.SetRegionWidth(W);
} else {
LastNotUsed = true;
Graphics.Render(Region, 4 + (24 - W) / 2 + W / 2, 4 + (24 - Region.GetRegionHeight()) / 2 + H / 2, 0, W / 2, H / 2, false, false, 2 - As, As);
}
}
this.RenderCurrentSlot();
}
public bool HasEquipped(Class Type) {
return GetEquipped(Type) != null;
}
public Equippable GetEquipped(Class Type) {
for (int I = 6; I < this.Inventory.GetSize(); I++) {
Item It = this.Inventory.GetSlot(I);
if (Type.IsInstance(It)) {
return (Equippable) It;
}
}
return null;
}
public void RenderCurrentSlot() {
Graphics.Batch.SetProjectionMatrix(Camera.Ui.Combined);
if (this.CurrentSlot != null) {
float X = Input.Instance.UiMouse.X;
float Y = Input.Instance.UiMouse.Y;
int Count = this.CurrentSlot.GetCount();
if (Count > 0) {
TextureRegion Sprite = this.CurrentSlot.GetSprite();
Graphics.Render(Sprite, X + 12 - Sprite.GetRegionWidth() / 2, Y - 8);
if (Count > 1) {
Graphics.Small.Draw(Graphics.Batch, string.ValueOf(Count), X + 16, Y - 4);
}
}
}
}
public void RenderOnPlayer(Player Player, float Of) {
Item Slot = this.Inventory.GetSlot(this.Active);
if (Slot != null) {
Slot.Render(Player.X, Player.Y + Of, Player.W, Player.H, Player.IsFlipped(), false);
}
}
public void RenderBeforePlayer(Player Player, float Of) {
Item Slot = this.Inventory.GetSlot(this.Active == 0 ? 1 : 0);
if (Slot != null) {
Slot.Render(Player.X, Player.Y + Of, Player.W, Player.H, Player.IsFlipped(), true);
}
}
}
}