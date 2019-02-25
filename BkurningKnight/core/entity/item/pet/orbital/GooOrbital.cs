using BurningKnight.core.assets;
using BurningKnight.core.entity.item.pet.impl;

namespace BurningKnight.core.entity.item.pet.orbital {
	public class GooOrbital : Pet {
		protected void _Init() {
			{
				Name = Locale.Get("goo");
				Description = Locale.Get("goo_desc");
				Sprite = "item-goo";
			}
		}

		public static class Impl : Orbital {
			protected void _Init() {
				{
					Sprite = "item-goo";
				}
			}


		}

		public override PetEntity Create() {
			return new Impl();
		}

		public GooOrbital() {
			_Init();
		}
	}
}
