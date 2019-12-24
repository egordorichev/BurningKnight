using System;
using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.level.entities.chest;
using Lens.entity;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities.statue {
	public class ChestStatue : Statue {
		public override void AddComponents() {
			base.AddComponents();

			Sprite = "chest_statue";
			Width = 20;
			Height = 26;
		}

		protected override Rectangle GetCollider() {
			return new Rectangle(0, 11, 20, 15);
		}

		protected override bool Interact(Entity e) {
			if (e.GetComponent<HealthComponent>().ModifyHealth(-4, this, DamageType.Custom)) {
				try {
					var chest = (Chest) Activator.CreateInstance(ChestRegistry.Instance.Generate());
					Area.Add(chest);
					chest.TopCenter = BottomCenter + new Vector2(0, 4);
				} catch (Exception ex) {
					Log.Error(ex);
				}
			}
			
			return true;
		}
	}
}