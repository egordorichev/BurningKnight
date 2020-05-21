using BurningKnight.assets;
using BurningKnight.assets.achievements;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.level.entities;
using BurningKnight.level.entities.chest;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using Lens.assets;
using Lens.entity;
using Lens.util.file;
using Lens.util.math;
using Lens.util.timer;

namespace BurningKnight.entity.creature.npc {
	public class Maanex2 : Npc {
		private const int Cost = 8;
		internal ClawControll clawControll;
		
		public override void AddComponents() {
			base.AddComponents();
			
			Width = 12;
			Height = 15;

			AddComponent(new AnimationComponent("maanex"));
			GetComponent<DropsComponent>().Add(new SingleDrop("bk:maanex_head"));

			AddComponent(new InteractableComponent(Interact));
			AddComponent(new SensorBodyComponent(-Padding, -Padding, Width + Padding * 2, Height + Padding * 2));
			
			Subscribe<RoomChangedEvent>();
			
			Dialogs.RegisterCallback("maanex2_0", (d, c) => {
				if (((ChoiceDialog) d).Choice == 0) {
					if (!c.To.TryGetComponent<ConsumablesComponent>(out var component) || component.Coins < Cost) {
						return Dialogs.Get("maanex_11");
					}

					var room = GetComponent<RoomComponent>().Room;

					if (room == null) {
						return null;
					}

					component.Coins -= Cost;

					clawControll.Payed = true;

					Timer.Add(() => {
						GetComponent<DialogComponent>().StartAndClose(Locale.Get("m2_3"), 3);
					}, 0.2f);

					return null;
				}

				return null;
			});
		}

		public override void PostInit() {
			base.PostInit();
			
			var h = GetComponent<HealthComponent>();
			h.Unhittable = false;
			h.InitMaxHealth = 50;
			h.SetHealth(50, this);
		}

		private bool Interact(Entity e) {
			var d = GetComponent<DialogComponent>();
			
			d.Dialog.Str.SetVariable("cost", Cost);
			d.Start("maanex2_0", e);
			
			return true;
		}

		public override bool HandleEvent(Event e) {
			if (e is RoomChangedEvent rce) {
				if (rce.Who is Player) {
					var r = GetComponent<RoomComponent>().Room;
					
					if (rce.New == r) {
						// Wanna try out your skill?
						GetComponent<DialogComponent>().Start("m2_2");
					} else if (rce.Old == r) {
						GetComponent<DialogComponent>().Close();
					}
				}
			} else if (e is DiedEvent de) {
				Items.Unlock("bk:maanex_head");
				ExplosionMaker.Make(this);

				if (de.From is Player p && p.GetComponent<HatComponent>().Item?.Id == "bk:maanex_head") {
					Achievements.Unlock("bk:maanex");
				}
			} else if (e is HealthModifiedEvent hme && hme.Amount < 0) {
				GetComponent<DialogComponent>().StartAndClose(Maanex.Bruh[Rnd.Int(Maanex.Bruh.Length)], 2);
			}
			
			return base.HandleEvent(e);
		}
	}
}