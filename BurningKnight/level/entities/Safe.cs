using BurningKnight.assets;
using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.creature.drop;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using Lens.entity;
using Lens.util.file;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities {
	public class Safe : SolidProp {
		public Safe() {
			Sprite = "safe";
		}

		protected override Rectangle GetCollider() {
			return new Rectangle(0, 6, 19, 17);
		}

		public override void AddComponents() {
			base.AddComponents();

			Width = 19;
			Height = 23;
			
			AddComponent(new HealthComponent {
				RenderInvt = true,
				InitMaxHealth = Random.Int(2, 5)
			});
			
			AddComponent(new ExplodableComponent());
			AddComponent(new ShadowComponent());

			var drops = new DropsComponent();
			AddComponent(drops);
			
			drops.Add(new SimpleDrop {
				Items = new [] {
					"bk:coin"
				},
				
				Chance = 0.8f,
				Min = 3,
				Max = 10
			});

			drops.Add(new SimpleDrop {
				Items = new[] {
					"bk:key"
				},

				Chance = 0.5f,
				Min = 1,
				Max = 4
			});
			
			drops.Add(new SimpleDrop {
				Items = new [] {
					"bk:bomb"
				},
				
				Chance = 0.3f,
				Min = 1,
				Max = 2
			});
			
			drops.Add(new SimpleDrop {
				Items = new [] {
					"bk:tnt"
				},
				
				Chance = 0.05f
			});
			
			drops.Add(new PoolDrop(ItemPool.Safe, 1f, 1, 3));
		}

		public override void PostInit() {
			base.PostInit();
			
			if (GetComponent<HealthComponent>().Health == 0) {
				Break(false);
			}
		}

		public void Break(bool spawnLoot = true) {
			GetComponent<SliceComponent>().Sprite = CommonAse.Props.GetSlice("safe_broken");
			GetComponent<HealthComponent>().RenderInvt = false;

			if (spawnLoot) {
				GetComponent<DropsComponent>().SpawnDrops();
			}
		}

		public override bool HandleEvent(Event e) {
			if (e is HealthModifiedEvent ev) {
				if (ev.Type != DamageType.Explosive) {
					ev.Amount = 0;
					return true;
				}
				
				ev.Amount = -1;
			} else if (e is DiedEvent d) {
				Break();
				ExplosionMaker.Make(d.From);
				return true;
			}
			
			return base.HandleEvent(e);
		}
	}
}