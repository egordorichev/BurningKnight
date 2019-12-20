using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.entity.item;
using BurningKnight.state;
using Lens.entity;

namespace BurningKnight.level.entities.chest {
	public class ScourgedChest : Chest, DropModifier {
		public ScourgedChest() {
			Sprite = "scourged_chest";
		}

		protected override void DefineDrops() {
			GetComponent<DropsComponent>().Add("bk:scourged_chest");
		}

		protected override bool TryOpen(Entity entity) {
			Run.AddScourge(true);
			return true;
		}

		public void ModifyDrops(List<Item> drops) {
			foreach (var d in drops) {
				d.Scourged = true;
			}
		}
	}
}