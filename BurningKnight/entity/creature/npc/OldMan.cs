using System.Linq;
using BurningKnight.assets;
using BurningKnight.assets.input;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.level.biome;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using BurningKnight.util;
using Lens.entity;
using Lens.input;
using Lens.util.file;
using Lens.util.math;
using Lens.util.timer;

namespace BurningKnight.entity.creature.npc {
	public class OldMan : Npc {
		public bool RickRoll;
		
		public override void AddComponents() {
			base.AddComponents();

			Width = 20;
			Height = 18;

			AddComponent(new AnimationComponent("old_man"));
			AddComponent(new RectBodyComponent(-Padding, -Padding, Width + Padding * 2, Height + Padding * 2));

			if (Run.Depth == 0) {
				AddComponent(new CloseDialogComponent("old_man_0"));
				
				// AddComponent(new InteractDialogComponent("old_man_1"));
			} else if (Run.Depth == Run.ContentEndDepth) {
				GetComponent<DialogComponent>().Start("old_man_4");
				// cycle = !BK.Version.Dev;;
			} else if (Run.Depth == -2) {
				set = false;
				AddComponent(new CloseDialogComponent("control_1") {
						Radius = 64 * 64,
						RadiusMax = 72 * 72
				});
			} else {
				Subscribe<ItemTakenEvent>();
				Subscribe<RoomChangedEvent>();
				
				inSecret = true;
			}

			AlwaysActive = true;
			GetComponent<DialogComponent>().Dialog.Voice = 28;
		}

		private string[] song = {
			"Never gonna give you up",
			"Never gonna let you down",
			"Never gonna run around and desert you",
			"Never gonna make you cry",
			"Never gonna say goodbye",
			"Never gonna tell a lie and hurt you"
		};

		private int index;

		private void StartSong() {
			if (index >= song.Length) {
				GetComponent<DialogComponent>().Close();
				return;
			}
			
			GetComponent<DialogComponent>().Start(song[index], null, () => {
				Timer.Add(() => {
					StartSong();
				}, 2);

				if (RickRoll) {
					foreach (var item in GetComponent<RoomComponent>().Room.Tagged[Tags.Item]) {
						item.Done = true;
						AnimationUtil.Poof(item.Center);
					}
				}
			});

			index++;
		}

		public override bool HandleEvent(Event e) {
			if (inSecret) {
				if (e is RoomChangedEvent rce && rce.Who is Player) {
					if (rce.New == GetComponent<RoomComponent>().Room) {
						if (RickRoll) {
							StartSong();
							index = 0;
						} else {
							GetComponent<DialogComponent>().Start("old_man_6");
						}
					} else {
						GetComponent<DialogComponent>().Close();
					}
				} else if (e is ItemTakenEvent ite && ite.Stand.GetComponent<RoomComponent>().Room == GetComponent<RoomComponent>().Room) {
					GetComponent<AnimationComponent>().Animate(() => {
						Done = true;
					});
				}
			}

			return base.HandleEvent(e);
		}

		private bool inSecret;
		private bool set = true;
		private bool cycle;
		private float t;
		
		public override void Update(float dt) {
			base.Update(dt);

			if (cycle) {
				t += dt;

				if (t >= 1f) {
					t = 0;

					var all = BiomeRegistry.Defined.Values.ToArray();
					Run.Level.SetBiome(all[Rnd.Int(all.Length)]);
				}

				return;
			}
			
			if (!set) {
				set = true;
				var dialog = GetComponent<DialogComponent>();
								
				dialog.Dialog.Str.ClearIcons();
				dialog.Dialog.Str.AddIcon(CommonAse.Ui.GetSlice(Controls.FindSlice(Controls.Roll, false)));

				if (GamepadComponent.Current != null && GamepadComponent.Current.Attached) {
					dialog.Dialog.Str.AddIcon(CommonAse.Ui.GetSlice(Controls.FindSlice(Controls.Roll, true)));
				}
			}
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			RickRoll = stream.ReadBoolean();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(RickRoll);
		}
	}
}