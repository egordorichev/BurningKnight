using System;
using BurningKnight.assets;
using BurningKnight.assets.achievements;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.level.entities.chest;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using BurningKnight.util;
using Lens.util.timer;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.npc {
	public class Builder : Npc {
		private int cost;
		private int paid;
		private bool shouldDissappear;
		
		public override void AddComponents() {
			base.AddComponents();
			
			Width = 7;
			Height = 17;
			
			AddComponent(new SensorBodyComponent(-Npc.Padding, -Npc.Padding, Width + Npc.Padding * 2, Height + Npc.Padding * 2, BodyType.Static));

			cost = Math.Min(99, Run.Depth * 15);
			paid = GlobalSave.GetInt("builder_paid", 0);
			
			AddComponent(new AnimationComponent("builder"));
			// GetComponent<DialogComponent>().Dialog.Voice = 15;

			var dl = GetComponent<DialogComponent>();

			dl.InitCallback = () => {
				dl.Dialog.Str.AddIcon(CommonAse.Ui.GetSlice("coin"));
				dl.Dialog.Str.SetVariable("need", cost - paid);
			};
			
			AddComponent(new InteractableComponent(e => {
				dl.Start("builder_0", e);
				return true;
			}) {
				CanInteract = e => !shouldDissappear
			});

			Dialogs.RegisterCallback("builder_0", (d, c) => {
				if (((ChoiceDialog) d).Choice == 0) {
					if (!c.To.TryGetComponent<ConsumablesComponent>(out var component) || component.Coins == 0) {
						// Bro, you have no money!
						return Dialogs.Get("builder_1");
					}

					var amount = Math.Min(cost - paid, component.Coins);

					paid += amount;
					component.Coins -= amount;
					dl.Dialog.Str.SetVariable("need", cost - paid);

					if (paid == cost) {
						GlobalSave.Put("builder_paid", 0);
						GlobalSave.Put($"shortcut_{Run.Depth}", true);

						return Dialogs.Get("builder_3");
					} else {
						GlobalSave.Put("builder_paid", paid);
					}
					
					return Dialogs.Get("builder_2");
				}

				return Dialogs.Get("builder_4");
			});

			Dialogs.RegisterCallback("builder_3", (d, c) => {
				shouldDissappear = true;

				CheckShortcutUnlocks();
				
				Timer.Add(() => {
					AnimationUtil.Poof(Center);
					Done = true;
				}, 5f);
				
				return null;
			});
		}

		public static void CheckShortcutUnlocks() {
			if (GlobalSave.IsTrue("shortcut_3")) {
				Achievements.Unlock("bk:desert_shortcut");
			}
			
			if (GlobalSave.IsTrue("shortcut_5")) {
				Achievements.Unlock("bk:jungle_shortcut");
			}
			
			if (GlobalSave.IsTrue("shortcut_7")) {
				Achievements.Unlock("bk:ice_shortcut");
			}
			
			if (GlobalSave.IsTrue("shortcut_9")) {
				Achievements.Unlock("bk:library_shortcut");
			}
		}

		public override void PostInit() {
			base.PostInit();

			if (GlobalSave.IsTrue($"shortcut_{Run.Depth}")) {
				Done = true;
				return;
			}
			
			if (Run.Depth > 0) {
				GameSave.Put("seen_builder", true);
			}
		}

		public static bool ShouldAppear() {
			if (Run.Type != RunType.Regular || GameSave.IsTrue("seen_builder")) {
				return false;
			}

			if (Run.Depth == 5 && LevelSave.GenerateMarket && Run.Loop == 0) {
				return false;
			}

			if (Run.Depth == 3 || Run.Depth == 5 || Run.Depth == 7 || Run.Depth == 9) {
				return GlobalSave.IsFalse($"shortcut_{Run.Depth}");
			}

			return false;
		}
	}
}