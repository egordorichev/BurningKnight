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
		public static void Reroll(Area area, Room room, bool rerollStands, bool spawnNewItems, bool ignore, ItemType[] types, Action<Item> processItem = null) {
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

						item = s.Item;
					}
				} else if (e is Item i) {
					if (i.Id == "bk:idol") {
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

				if (Reroll(item, pool)) {
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
				item.ConvertTo(id);
				item.AutoPickup = false;
				
				TextParticle.Add(item, Locale.Get("rerolled")).Stacks = false;

				return true;
			}

			return false;
		}

		public static bool Reroll(Item item, List<ItemData> pool, Func<ItemData, bool> filter = null) {
			var id = Items.GenerateAndRemove(pool, i => i.Id != item.Id && (filter == null || filter(i)));

			if (id != null) {
				item.ConvertTo(id);
				item.AutoPickup = false;

				TextParticle.Add(item, Locale.Get("rerolled")).Stacks = false;

				return true;
			}

			return false;
		}
	}
}