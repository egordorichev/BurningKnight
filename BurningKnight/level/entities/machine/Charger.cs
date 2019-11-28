using BurningKnight.assets;
using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.creature.drop;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using BurningKnight.util;
using Lens.entity;
using Lens.util.file;
using Lens.util.math;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities.machine {
	public class Charger : SolidProp {
		private int timesUsed;
		private int noMoneyAttempt;
		private bool broken;

		public Charger() {
			Sprite = "charger";
		}

		protected override Rectangle GetCollider() {
			return new Rectangle(0, 6, 19, 17);
		}

		public override void AddComponents() {
			base.AddComponents();

			Width = 19;
			Height = 23;
			
			AddComponent(new DialogComponent());
			AddComponent(new ExplodableComponent());
			AddComponent(new ShadowComponent());
			AddComponent(new SensorBodyComponent(-Npc.Padding, -Npc.Padding, Width + Npc.Padding * 2, Height + Npc.Padding * 2));
			
			AddComponent(new InteractableComponent(Interact) {
				CanInteract = e => !broken
			});

			var drops = new DropsComponent();
			AddComponent(drops);
			
			drops.Add(new SimpleDrop {
				Items = new [] {
					"bk:battery"
				},
				
				Chance = 0.5f,
				Min = 1,
				Max = 2
			});
			
			drops.Add(new SimpleDrop {
				Items = new [] {
					"bk:coin"
				},
				
				Chance = 0.5f,
				Min = 1,
				Max = 3
			});
			
			drops.Add(new PoolDrop(ItemPool.Charger, 0.5f, 1, 2));
			GetComponent<DialogComponent>().Dialog.Voice = 10;
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(broken);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			broken = stream.ReadBoolean();
		}

		public override void PostInit() {
			base.PostInit();

			if (broken) {
				Break();
			}
		}

		public void Break(bool spawnLoot = true) {
			broken = true;
			
			GetComponent<InteractableSliceComponent>().Sprite = CommonAse.Props.GetSlice("charger_broken");

			if (spawnLoot) {
				GetComponent<DropsComponent>().SpawnDrops();
			}
		}
		
		private void Animate() {
			GetComponent<InteractableSliceComponent>().Scale.Y = 0.4f;
			Tween.To(1, 0.4f, x => GetComponent<InteractableSliceComponent>().Scale.Y = x, 0.2f);
			
			GetComponent<InteractableSliceComponent>().Scale.X = 1.3f;
			Tween.To(1, 1.3f, x => GetComponent<InteractableSliceComponent>().Scale.X = x, 0.2f);
		}

		private bool Interact(Entity e) {
			Animate();
			
			var p = e ?? LocalPlayer.Locate(Area);
			var active = p.GetComponent<ActiveItemComponent>();
			
			if (active.Item == null) {
				GetComponent<DialogComponent>().StartAndClose("charger_0", 3);
				AnimationUtil.ActionFailed();
				
				return true;
			}
			
			if (active.Item.Delay <= 0.02f) {
				GetComponent<DialogComponent>().StartAndClose("charger_1", 3);
				AnimationUtil.ActionFailed();
				
				return true;
			}

			if (e != null) {
				var component = p.GetComponent<ConsumablesComponent>();

				if (component.Coins == 0) {
					if (noMoneyAttempt == 0) {
						GetComponent<DialogComponent>().StartAndClose("charger_2", 3);
					} else if (noMoneyAttempt == 1) {
						GetComponent<DialogComponent>().StartAndClose("charger_3", 3);
					} else {
						var hp = p.GetComponent<HealthComponent>();
						hp.ModifyHealth(-1, this);
						GetComponent<DialogComponent>().StartAndClose($"charger_{(hp.HasNoHealth ? 5 : 4)}", 3);
					}

					noMoneyAttempt++;
					AnimationUtil.ActionFailed();
					
					return true;
				}

				noMoneyAttempt = 0;
				component.Coins -= 1;
			}

			timesUsed += e == null ? 2 : 1;

			active.Charge(Rnd.Int(1, 3));
			
			if (Rnd.Float(100) < timesUsed * 2 - Run.Luck * 0.5f) {
				Break();
				ExplosionMaker.Make(p);
				return true;
			}
			
			return false;
		}

		public override bool HandleEvent(Event e) {
			if (e is ExplodedEvent && !broken) {
				Interact(null);
			}
			
			return base.HandleEvent(e);
		}
	}
}