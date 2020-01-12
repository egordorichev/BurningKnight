using System;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using Lens.entity;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.npc.dungeon {
	public class Gobetta : DungeonShopNpc {
		public override void AddComponents() {
			base.AddComponents();
			
			AlwaysActive = true;
			Width = 15;
			Height = 13;
			
			AddComponent(new AnimationComponent("gobetta"));
		}

		public override string GetId() {
			return ShopNpc.Gobetta;
		}

		protected override bool OwnsStand(ItemStand stand) {
			return stand is GobettaStand;
		}

		public static void Place(Vector2 where, Area area) {
			where.Y -= 16;
			
			var gobetta = new Gobetta();
			area.Add(gobetta);
			gobetta.BottomCenter = where;
			
			var pool = Items.GeneratePool(Items.GetPool(ItemPool.Gobetta));
			var c = 3;
			var s = (int) Math.Floor(c / 2f) * 18;
			
			for (var i = 0; i < c; i++) {
				var stand = new GobettaStand();
				area.Add(stand);
				stand.Center = where + new Vector2((stand.Width + 4) * i - s, 4 + stand.Height + (i == 0 ? stand.Height * 0.5f : 0));

				var id = Items.GenerateAndRemove(pool, null, true);
				stand.SetItem(Items.CreateAndAdd(id, area, false), null);
			}
		}
		
		protected override string GetDealDialog() {
			return $"gobetta_{Rnd.Int(3)}";
		}

		protected override string GetHiDialog() {
			return $"gobetta_{Rnd.Int(3, 6)}";
		}
	}
}