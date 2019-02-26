using BurningKnight.entity.item.pet.impl;

namespace BurningKnight.entity.item.pet.orbital {
	public class NanoOrbital : Pet {
		public NanoOrbital() {
			_Init();
		}

		protected void _Init() {
			{
				Name = Locale.Get("nano_orbital");
				Description = Locale.Get("nano_orbital_desc");
				Sprite = "item-mini_orbital";
			}
		}

		public override PetEntity Create() {
			return new Impl();
		}

		public static class Impl : Orbital {
			protected void _Init() {
				{
					Sprite = "item-mini_orbital";
				}
			}
		}
	}
}