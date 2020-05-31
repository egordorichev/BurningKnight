using System;
using System.Collections.Generic;
using BurningKnight.assets.items;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using BurningKnight.entity.item.stand;
using BurningKnight.entity.room;
using Lens.assets;
using Lens.entity;

namespace BurningKnight.entity.item {
	public static class Reroller {
		private static bool CanReroll(Item i) {
			return !(i.Id == "bk:idol" || i.Type == ItemType.Scourge || i.Type == ItemType.Bomb || i.Type == ItemType.Key || i.Type == ItemType.Heart || i.Type == ItemType.Coin || i.Type == ItemType.Battery || i.Type == ItemType.Mana || i is RoundItem);
		}

		public static void Reroll(Area area, Room room, bool rerollStands, bool spawnNewItems, bool ignore, ItemType[] types, Action<Item> processItem = null, bool d2 = false) {
			var items = room.Tagged[Tags.Item].ToArray();
			var pool = Items.GeneratePool(Items.GetPool(room.GetPool() ?? ItemPool.Shop));
			
			foreach (var e in items) {
				Item item = null;
				
				if (e is ItemStand s) {
					if (rerollStands) {
						if (s.Item == null) {
							if (spawnNewItems) {
								var id = s is ShopStand std ? Items.Generate(std.GetPool()) : Items.GenerateAndRemove(pool);
								s.SetItem(Items.CreateAndAdd(id, area), null);

								TextParticle.Add(s, Locale.Get("rerolled")).Stacks = false;
							}

							continue;
						}

						var i = s.Item;
						
						if (!CanReroll(i)) {
							continue;
						}

						item = i;
					}
				} else if (e is Item i) {
					if (!CanReroll(i)) {
						continue;
					}
					
					if (types != null) {
						var found = false;
						
						foreach (var t in types) {
							if (t == i.Type) {
								found = true;
							}
						}

						if (found == ignore) {
							continue;
						}
					}
					
					item = i;
				}

				if (item == null || (item.TryGetComponent<OwnerComponent>(out var o) && !(o.Owner is ItemStand))) {
					continue;
				}

				if (Reroll(item, pool, null, d2)) {
					processItem?.Invoke(item);
				}

				if (e is ShopStand st) {
					st.Recalculate();
				}
			}
			
			Audio.PlaySfx("item_reroll");
		}

		public static bool Reroll(Item item, ItemPool pool, Func<ItemData, bool> filter = null) {
			var id = Items.Generate(pool, i => i.Id != item.Id && Items.ShouldAppear(i) && (filter == null || filter(i)));

			if (id != null) {
				item.LastId = item.Id;
				item.ConvertTo(id);
				item.AutoPickup = false;
				
				TextParticle.Add(item, Locale.Get("rerolled")).Stacks = false;

				return true;
			}

			return false;
		}

		public static bool Reroll(Item item, List<ItemData> pool, Func<ItemData, bool> filter = null, bool d2 = false) {
			var id = d2 ? item.LastId : null;

			if (id == null) {
				id = Items.GenerateAndRemove(pool, i => i.Id != item.Id && (filter == null || filter(i)));
			}
			
			if (id != null) {
				item.LastId = item.Id;
				item.ConvertTo(id);
				item.AutoPickup = false;

				TextParticle.Add(item, Locale.Get("rerolled")).Stacks = false;

				return true;
			}

			return false;
		}
	}
}