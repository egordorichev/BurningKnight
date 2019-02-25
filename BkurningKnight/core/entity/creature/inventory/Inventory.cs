using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item;
using BurningKnight.core.entity.item.accessory.equippable;
using BurningKnight.core.entity.item.autouse;
using BurningKnight.core.entity.item.weapon;
using BurningKnight.core.util;
using BurningKnight.core.util.file;

namespace BurningKnight.core.entity.creature.inventory {
	public class Inventory {
		private Creature Creature;
		private Item[] Slots;
		private List<Item> Spaces = new List<>();
		public int Active;

		public Inventory(Creature Creature) {
			this.Creature = Creature;
			this.Slots = new Item[3];
		}

		public Void Clear() {
			this.Slots = new Item[3];
			Spaces.Clear();
		}

		public Void Load(FileReader Reader) {
			this.Active = Reader.ReadByte();

			for (int I = 0; I < 3; I++) {
				if (Reader.ReadBoolean()) {
					string Type = Reader.ReadString();

					try {
						Class Clazz = Class.ForName(Type);
						Constructor Constructor = Clazz.GetConstructor();
						Object Object = Constructor.NewInstance();
						Item Item = (Item) Object;
						Item.Load(Reader);
						Item.SetOwner(Player.Instance);
						this.SetSlot(I, Item);
					} catch (Exception) {
						Dungeon.ReportException(E);
					}
				} 
			}

			short Spaces = Reader.ReadInt16();

			for (int I = 0; I < Spaces; I++) {
				string Type = Reader.ReadString();

				try {
					Class Clazz = Class.ForName(Type);
					Constructor Constructor = Clazz.GetConstructor();
					Object Object = Constructor.NewInstance();
					Item Item = (Item) Object;
					Item.Load(Reader);
					Item.SetOwner(Player.Instance);

					if (Item is Equippable) {
						((Equippable) Item).OnEquip(true);
					} 

					this.Spaces.Add(Item);
				} catch (Exception) {
					Dungeon.ReportException(E);
				}
			}
		}

		public Void Save(FileWriter Writer) {
			Writer.WriteByte((byte) this.Active);

			for (int I = 0; I < Slots.Length; I++) {
				Item Slot = this.GetSlot(I);

				if (Slot == null) {
					Writer.WriteBoolean(false);
				} else {
					Writer.WriteBoolean(true);
					Writer.WriteString(Slot.GetClass().GetName());
					Slot.Save(Writer);
				}

			}

			Writer.WriteInt16((short) this.Spaces.Size());

			for (int I = 0; I < Spaces.Size(); I++) {
				Item Slot = Spaces.Get(I);
				Writer.WriteString(Slot.GetClass().GetName());
				Slot.Save(Writer);
			}
		}

		public bool Add(ItemHolder Holder) {
			Item Item = Holder.GetItem();

			if (Item == null) {
				return false;
			} 

			if (Item is WeaponBase) {
				for (int I = 0; I < 3; I++) {
					if (this.IsEmpty(I) && (I != 2)) {
						this.SetSlot(I, Item);
						Item.SetOwner(Player.Instance);
						Item.OnPickup();
						Holder.Done = true;

						if (Item is Equippable) {
							((Equippable) Item).OnEquip(false);
						} 

						this.OnAdd(Holder, I);

						return true;
					} 
				}
			} 

			if (Item is Autouse) {
				int C = Item.GetCount();

				for (int J = 0; J < C; J++) {
					Item.Use();
				}
			} else if (Item is Equippable) {
				((Equippable) Item).OnEquip(false);
			} 

			this.OnAdd(Holder, 0);
			this.Spaces.Add(Holder.GetItem());

			return true;
		}

		private Void OnAdd(ItemHolder Holder, int Slot) {
			Item Item = Holder.GetItem();
			Item.SetOwner(Player.Instance);
			Item.A = 0;
			Tween.To(new Tween.Task(1, 0.3f) {
				public override float GetValue() {
					return Item.A;
				}

				public override Void SetValue(float Value) {
					Item.A = Value;
				}
			});
			PickupFx Fx = new PickupFx();
			Fx.X = Holder.X + Holder.W / 2;
			Fx.Y = Holder.Y + Holder.H / 2;
			Fx.Region = Item.GetSprite();
			Fx.Target = new Point(Camera.Game.Position.X - Display.GAME_WIDTH / 2 * Camera.Game.Zoom, Camera.Game.Position.Y - Display.GAME_HEIGHT / 2 * Camera.Game.Zoom);
			Dungeon.Area.Add(Fx);
		}

		public bool Find(Class Clazz) {
			for (int I = 0; I < 3; I++) {
				if (!this.IsEmpty(I)) {
					if (Clazz.IsInstance(this.GetSlot(I))) {
						return true;
					} 
				} 
			}

			return FindEquipped(Clazz);
		}

		public bool FindEquipped(Class Clazz) {
			foreach (Item Item in Spaces) {
				if (Clazz.IsInstance(Item)) {
					return true;
				} 
			}

			return false;
		}

		public Item FindItem(Class Clazz) {
			for (int I = 0; I < 3; I++) {
				if (!this.IsEmpty(I)) {
					if (Clazz.IsInstance(this.GetSlot(I))) {
						return this.GetSlot(I);
					} 
				} 
			}

			foreach (Item Item in Spaces) {
				if (Clazz.IsInstance(Item)) {
					return Item;
				} 
			}

			return null;
		}

		public Item Remove(Class Clazz) {
			for (int I = 0; I < 3; I++) {
				if (!this.IsEmpty(I)) {
					Item Item = this.GetSlot(I);

					if (Clazz.IsInstance(Item)) {
						Item.SetCount(Item.GetCount() - 1);

						if (Item.GetCount() == 0) {
							this.SetSlot(I, null);
						} 

						return Item;
					} 
				} 
			}

			for (int I = Spaces.Size() - 1; I >= 0; I--) {
				Item Item = Spaces.Get(I);

				if (Clazz.IsInstance(Item)) {
					Item.SetCount(Item.GetCount() - 1);

					if (Item.GetCount() == 0) {
						Spaces.Remove(Item);
					} 

					return Item;
				} 
			}

			return null;
		}

		public bool IsEmpty(int I) {
			return this.GetSlot(I) == null || this.GetSlot(I).GetCount() == 0;
		}

		public Item GetSlot(int I) {
			return this.Slots[I];
		}

		public int GetSpace() {
			return Spaces.Size();
		}

		public Item GetSpace(int I) {
			return this.Spaces.Get(I);
		}

		public Void SetSlot(int I, Item Item) {
			this.Slots[I] = Item;
		}

		public int GetSize() {
			return 3;
		}

		public Creature GetCreature() {
			return this.Creature;
		}
	}
}
