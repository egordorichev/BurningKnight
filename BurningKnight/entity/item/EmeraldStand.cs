using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.assets.achievements;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item.stand;
using BurningKnight.save;
using BurningKnight.ui.dialog;
using BurningKnight.util;
using Lens.assets;
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

		protected override string GetSprite() {
			return "shop_stand";
		}
		
		protected override bool CanTake(Entity entity) {
			if (!base.CanTake(entity)) {
				return false;
			}

			if (GlobalSave.Emeralds < price) {
				AnimationUtil.ActionFailed();
				var npc = Area.FindClosest(Center, Tags.Npc, n => n is ShopNpc);

				if (npc != null && npc.TryGetComponent<DialogComponent>(out var c)) {
					c.StartAndClose($"shopkeeper_{Rnd.Int(15, 18)}", 3);
				}
				
				return false;
			}

			GlobalSave.Emeralds -= price;
			Items.Unlock(Item.Id);
			AlreadyOnStand.Remove(Item.Id);

			if (!ShowUnlocked) {
				Item.Done = true;
				
				/*var item = PickItem();
				SetItem(item, entity, false);*/

				AnimationUtil.Poof(Center, 1);
			}
			
			Audio.PlaySfx("item_purchase");
			Achievements.Unlock("bk:unlock");
			
			entity.HandleEvent(new ItemBoughtEvent {
				Item = item,
				Who = entity,
				Stand = this
			});

			foreach (var i in Area.Tagged[Tags.Item]) {
				if (i is Item it) {
					it.CheckMasked();
				} else if (i is ItemStand its) {
					its.Item?.CheckMasked();
				}
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
				if (i.Lockable && i.UnlockPrice > 0 && ApproveItem(i) && !AlreadyOnStand.Contains(i.Id) 
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
			base.SetItem(i, entity, remove);
			RecalculatePrice();
		}

		public void RecalculatePrice() {
			if (Item == null) {
				return;
			}
			
			if (GlobalSave.IsTrue(Item.Id)) {
				price = 0;
			} else {
				price = Item.Data.UnlockPrice;
				var player = LocalPlayer.Locate(Area);

				if (player != null && player.GetComponent<HatComponent>().Item?.Id == "bk:dunce_hat") {
					price++;
				}
				
				priceString = $"{price}";
				priceWidth = Font.Small.MeasureString(priceString).Width;
				priceX = (Width - priceWidth - 2 - emerald.Width) / 2f;
			}
		}
	}
}