using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.entity.item;
using BurningKnight.state;
using Lens.entity;

namespace BurningKnight.level.entities.chest {
	public class CursedChest : Chest, DropModifier {
		public CursedChest() {
			Sprite = "cursed_chest";
		}

		protected override void DefineDrops() {
			GetComponent<DropsComponent>().Add("bk:cursed_chest");
		}

		protected override bool TryOpen(Entity entity) {
			Run.AddCurse(true);
			return true;
		}

		public void ModifyDrops(List<Item> drops) {
			foreach (var d in drops) {
				if (d.Type == ItemType.Weapon) {
					d.Cursed = true;
				}
			}
		}
	}
}