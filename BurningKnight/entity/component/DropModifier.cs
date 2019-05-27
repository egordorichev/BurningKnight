using System.Collections.Generic;
using BurningKnight.entity.item;

namespace BurningKnight.entity.component {
	public interface DropModifier {
		void ModifyDrops(List<Item> drops);
	}
}