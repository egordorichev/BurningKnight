using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.item.stand;
using BurningKnight.save;
using BurningKnight.ui.dialog;
using BurningKnight.util;
using Lens.entity;
using Lens.graphics;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item {
	public class EmeraldStand : ItemStand {
		public static List<string> AlreadyOnStand = new List<string>();

		private TextureRegion emerald;
		
		private int price;
		private float priceWidth;
		private string priceString;
		private float priceX;
		
		public EmeraldStand() {
			dontSaveItem = true;
			emerald = CommonAse.Ui.GetSlice("emerald");
		}

		public override void Init() {
			base.Init();
			var item = PickItem();

			if (item != null) {
				SetItem(item, null);
			}
		}

		public override void Destroy() {
			base.Destroy();

			if (Item != null) {
				AlreadyOnStand.Remove(Item.Id);
			}
		}

		protected override void OnTake(Item item, Entity who) {
			base.OnTake(item, who);
			
			who.HandleEvent(new ItemBoughtEvent {
				Item = item,
				Who = who,
				Stand = this
			});

			foreach (var i in Area.Tagged[Tags.Item]) {
				if (i is Item it) {
					it.CheckMasked();
				} else if (i is ItemStand its) {
					its.Item?.CheckMasked();
				}
			}
		}

		protected override string GetSprite() {
			return "shop_stand";
		}
		
		protected override bool CanTake(Entity entity) {
			if (!base.CanTake(entity)) {
				return false;
			}

			if (GlobalSave.Emeralds < price) {
				AnimationUtil.ActionFailed();

				foreach (var n in GetComponent<RoomComponent>().Room.Tagged[Tags.Npc]) {
					n.GetComponent<DialogComponent>().StartAndClose($"shopkeeper_{Rnd.Int(15, 18)}", 3);
					break;
				}
				
				return false;
			}

			GlobalSave.Emeralds -= price;
			Items.Unlock(Item.Id);
			AlreadyOnStand.Remove(Item.Id);

			if (!ShowUnlocked) {
				Item.Done = true;
				
				var item = PickItem();
				SetItem(item, entity, false);

				AnimationUtil.Poof(Center, 1);
			}
			
			return ShowUnlocked;
		}

		protected virtual bool ApproveItem(ItemData item) {
			return true;
		}

		protected bool ShowUnlocked;

		private Item PickItem() {
			var items = new List<ItemData>();

			foreach (var i in Items.Datas.Values) {
				if (((this is HatStand && i.Id == "bk:no_hat") || (i.Lockable && i.UnlockPrice > 0)) && ApproveItem(i) && !AlreadyOnStand.Contains(i.Id) 
				    && (ShowUnlocked || GlobalSave.IsFalse(i.Id))) {
					
					items.Add(i);
				}
			}

			if (items.Count == 0) {
				return null;
			}
			
			items.Sort((a, b) => a.UnlockPrice.CompareTo(b.UnlockPrice));

			var id = items[0].Id;
			AlreadyOnStand.Add(id);

			return Items.CreateAndAdd(id, Area);
		}

		protected override bool CanInteract(Entity e) {
			return Item != null;
		}

		public override void Render() {
			base.Render();

			if (Item != null && price > 0) {
				Graphics.Print(priceString, Font.Small, Position + new Vector2(priceX, 14));
				Graphics.Render(emerald, Position + new Vector2(priceX + priceWidth + 2, 17));
			}
		}

		public override void SetItem(Item i, Entity entity, bool remove = true) {
			if (i != null) {
				if (GlobalSave.IsTrue(i.Id)) {
					price = 0;
				} else {
					price = i.Data.UnlockPrice;
					priceString = $"{price}";
					priceWidth = Font.Small.MeasureString(priceString).Width;
					priceX = (Width - priceWidth - 2 - emerald.Width) / 2f;
				}
			}
			
			base.SetItem(i, entity, remove);
		}
	}
}