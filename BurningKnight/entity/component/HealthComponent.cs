using System;
using BurningKnight.entity.creature;
using BurningKnight.util;
using Lens.entity.component;

namespace BurningKnight.entity.component {
	public class HealthComponent : Component {
		private uint health;
		
		public uint Health {
			get => health;
			set {
				health = (uint) MathUtils.Clamp(health, 0, maxHealth);

				if (health == 0) {
					dead = true;
				}
			}
		}

		private uint maxHealth;
		
		public uint MaxHealth {
			get => maxHealth;

			set {
				maxHealth = Math.Max(1, value);
				health = Math.Min(maxHealth, Health);
			}
		}

		private bool dead;

		public bool Dead {
			set {
				if (dead != value) {
					dead = value;
					health = dead ? 0 : MaxHealth;

					if (dead) {
						Entity.Done = true;
					}
				}
			}		
		}

		public HealthComponent() {
			MaxHealth = 1;
			Health = MaxHealth;
		}
	}
}