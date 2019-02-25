using BurningKnight.core.assets;
using BurningKnight.core.entity.item;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.player.fx {
	public class ItemPickupFx : Entity {
		protected void _Init() {
			{
				Depth = 15;
				AlwaysActive = true;
			}
		}

		private string Text;
		public ItemHolder Item;
		private Player Player;
		private float A;
		private bool Go;

		public ItemPickupFx(ItemHolder Item, Player Player) {
			_Init();
			this.Text = Item.GetItem().GetName();

			if (this.Text == null) {
				this.Text = "Missing item name";
			} 

			if (this.Text.Equals("null_item")) {
				this.Text = "";
			} 

			this.Item = Item;
			this.Player = Player;
			Graphics.Layout.SetText(Graphics.Medium, this.Text);
			this.X = Item.X + Item.W / 2 - Graphics.Layout.Width / 2;
			this.Y = Item.Y + Item.H + 4;
			Tween.To(new Tween.Task(1, 0.2f, Tween.Type.QUAD_OUT) {
				public override float GetValue() {
					return A;
				}

				public override Void SetValue(float Value) {
					A = Value;
				}
			});
		}

		public static Item SetSkin(string Skin) {
			if (Skin == null || Skin.Equals("gobbo_head")) {
				return new NullItem();
			} else if (Skin.Equals("knight")) {
				return new KnightHat();
			} else if (Skin.Equals("stone")) {
				return new MoaiHat();
			} else if (Skin.Equals("viking")) {
				return new VikingHat();
			} else if (Skin.Equals("dunce")) {
				return new DunceHat();
			} else if (Skin.Equals("ravi")) {
				return new RaveHat();
			} else if (Skin.Equals("ushanka")) {
				return new UshankaHat();
			} else if (Skin.Equals("ruby")) {
				return new RubyHat();
			} else if (Skin.Equals("gold")) {
				return new GoldHat();
			} else if (Skin.Equals("wings")) {
				return new ValkyreHat();
			} else if (Skin.Equals("skull")) {
				return new SkullHat();
			} else if (Skin.Equals("cowboy")) {
				return new CoboiHat();
			} else if (Skin.Equals("red_mushroom")) {
				return new ShroomHat();
			} else if (Skin.Equals("brown_mushroom")) {
				return new FungiHat();
			} 

			return null;
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (this.Go) {
				return;
			} 

			if (Input.Instance.WasPressed("interact") && Dialog.Active == null) {
				if (this.Item.GetItem() is NullItem) {
					string Skin = Player.HatId;
					Player.Instance.SetHat("gobbo_head");
					Player.Instance.GetInventory().SetSlot(6, null);
					HatSelector.NullGot = false;
					this.Item.SetItem(SetSkin(Skin));
					Player.Instance.PlaySfx("menu/select");
				} else if (this.Item.GetItem() is Hat && Dungeon.Depth == -2) {
					string Skin = Player.HatId;
					Player.Instance.SetHat(((Hat) this.Item.GetItem()).Skin);
					Player.Instance.GetInventory().SetSlot(6, this.Item.GetItem());
					this.Item.GetItem().SetOwner(Player.Instance);
					this.Item.SetItem(SetSkin(Skin));
					Player.Instance.PlaySfx("menu/select");
				} else {
					if (this.Item.GetItem().Shop) {
						int G = this.Player.GetMoney();

						if (G < this.Item.GetItem().Price) {
							this.Remove();
							Player.Instance.PlaySfx("item_nocash");
							Camera.Shake(6);

							return;
						} else {
							this.Player.SetMoney(G - this.Item.GetItem().Price);
							this.Item.GetItem().Shop = false;
							this.Item.Price.Remove();
							this.Erase();
							Player.Instance.PlaySfx("item_purchase");

							if (Shopkeeper.Instance != null && !Shopkeeper.Instance.Enranged) {
								Shopkeeper.Instance.Become("thanks");
							} 

							for (int I = 0; I < 15; I++) {
								Confetti C = new Confetti();
								C.X = this.Item.X + Random.NewFloat(this.Item.W);
								C.Y = this.Item.Y + Random.NewFloat(this.Item.H);
								C.Vel.X = Random.NewFloat(-30f, 30f);
								C.Vel.Y = Random.NewFloat(30f, 40f);
								Dungeon.Area.Add(C);
							}
						}

					} 

					if (this.Player.TryToPickup(this.Item)) {
						this.Erase();
						Item.Done = true;
						LevelSave.Remove(Item);

						if (Item.GetItem() is BurningKey) {
							Player.Instance.HasBkKey = true;
						} 
					} 
				}

			} else if (this.Item.Done) {
				LevelSave.Remove(Item);
			} 
		}

		public Void Remove() {
			if (Go) {
				return;
			} 

			this.Go = true;
			Tween.To(new Tween.Task(0, 0.3f) {
				public override float GetValue() {
					return A;
				}

				public override Void SetValue(float Value) {
					A = Value;
				}

				public override Void OnEnd() {
					SetDone(true);
				}
			});
		}

		public Void Erase() {
			for (int I = 0; I < 3; I++) {
				PoofFx Fx = new PoofFx();
				Fx.X = this.Item.X + this.Item.W / 2;
				Fx.Y = this.Item.Y + this.Item.H / 2;
				Dungeon.Area.Add(Fx);
			}

			this.Go = true;

			if (!Text.StartsWith("+")) {
				this.Text = "+" + this.Text;
				Graphics.Layout.SetText(Graphics.Medium, this.Text);
				this.X = Item.X + Item.W / 2 - Graphics.Layout.Width / 2;
				this.Y = Item.Y + Item.H + 4;
			} 

			Tween.To(new Tween.Task(this.Y + 10, 2f, Tween.Type.QUAD_OUT) {
				public override float GetValue() {
					return Y;
				}

				public override Void SetValue(float Value) {
					Y = Value;
				}
			});
			Tween.To(new Tween.Task(0, 2f) {
				public override float GetValue() {
					return A;
				}

				public override Void SetValue(float Value) {
					A = Value;
				}

				public override Void OnEnd() {
					SetDone(true);
				}
			});
		}

		public override Void Render() {
			float C = (float) (0.8f + Math.Cos(Dungeon.Time * 10) / 5f);
			Graphics.Medium.SetColor(C, C, C, this.A);
			Graphics.Print(this.Text, Graphics.Medium, this.X, this.Y);
			Graphics.Medium.SetColor(1, 1, 1, 1);
		}
	}
}
