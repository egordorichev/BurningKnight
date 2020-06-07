using System;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.npc;
using BurningKnight.save;
using BurningKnight.ui.dialog;
using Lens.entity;
using Lens.util;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.twitch {
	public class TwitchNpc : Npc {
		static TwitchNpc() {
			Dialogs.RegisterCallback("twitch_0", (d, c) => {
				try {
					var id = GlobalSave.GetString("twitch_username");

					if (id != null) {
						c.Dialog.Str.SetVariable("username", id);
					}

					return Dialogs.Get($"twitch_{(id == null ? 1 : 5)}");
				} catch (Exception e) {
					Log.Error(e);
				}

				return null;
			});
			
			Dialogs.RegisterCallback("twitch_2", (d, c) => {
				var a = ((AnswerDialog) d).Answer;

				Log.Info($"Twitch username is set the seed to {a}");
				GlobalSave.Put("twitch_username", a);
				c.Dialog.Str.SetVariable("username", a);

				return null;
			});
		}
		
		public override void AddComponents() {
			base.AddComponents();
			
			Width = 10;
			Height = 11;
			
			AddComponent(new AnimationComponent("twitch"));
			GetComponent<DialogComponent>().Dialog.Voice = 2;

			AddComponent(new SensorBodyComponent(-Npc.Padding, -Npc.Padding, Width + Npc.Padding * 2, Height + Npc.Padding * 2, BodyType.Static));
			AddComponent(new InteractableComponent(Interact));
		}

		private bool Interact(Entity e) {
			GetComponent<DialogComponent>().Start("twitch_0", e);

			return true;
		}
	}
}