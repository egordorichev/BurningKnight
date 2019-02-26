using BurningKnight.entity.item.pet.impl;

namespace BurningKnight.entity.item.pet.orbital {
	public class GooOrbital : Pet {
		public GooOrbital() {
			_Init();
		}

		protected void _Init() {
			{
				Name = Locale.Get("goo");
				Description = Locale.Get("goo_desc");
				Sprite = "item-goo";
			}
		}

		public override PetEntity Create() {
			return new Impl();
		}

		public static class Impl : Orbital {
			protected void _Init() {
				{
					Sprite = "item-goo";
				}
			}
		}
	}
}