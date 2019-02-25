using BurningKnight.core.assets;
using BurningKnight.core.entity.item.pet.impl;

namespace BurningKnight.core.entity.item.pet.orbital {
	public class NanoOrbital : Pet {
		protected void _Init() {
			{
				Name = Locale.Get("nano_orbital");
				Description = Locale.Get("nano_orbital_desc");
				Sprite = "item-mini_orbital";
			}
		}

		public static class Impl : Orbital {
			protected void _Init() {
				{
					Sprite = "item-mini_orbital";
				}
			}


		}

		public override PetEntity Create() {
			return new Impl();
		}

		public NanoOrbital() {
			_Init();
		}
	}
}
