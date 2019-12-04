using BurningKnight.assets;
using BurningKnight.assets.items;
using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.fx;
using BurningKnight.util;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.graphics;
using Lens.util.file;
using Lens.util.math;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities {
	public class Well : Prop {
		public enum Type {
			Transmutation,
			Healing,
			Coin,
			Death
		}
		
		private TextureRegion water;
		private bool used;
		private Type type;

		public override void Init() {
			base.Init();

			type = (Type) Rnd.Int(4);
		}

		public override void PostInit() {
			base.PostInit();

			if (type != Type.Coin) {
				water = CommonAse.Props.GetSlice($"water_{(int) type}");
			}
		}

		public override void AddComponents() {
			base.AddComponents();

			Width = 30;
			Height = 21;
			
			AddComponent(new RectBodyComponent(0, 6, 30, 16, BodyType.Static));
			AddComponent(new SensorBodyComponent(-Npc.Padding, -Npc.Padding, Width + Npc.Padding * 2, Height + Npc.Padding * 2, BodyType.Static));
			AddComponent(new ShadowComponent(RenderShadow));
			AddComponent(new InteractableSliceComponent("props", "well"));
			
			AddComponent(new InteractableComponent(Interact) {
				CanInteract = (e) => !used,
				
				OnStart = (e) => {
					if (e is LocalPlayer) {
						Engine.Instance.State.Ui.Add(new InteractFx(this, Locale.Get(type == Type.Coin ? "throw_coin" : "sip")));
					}
				}
			});
		}

		private bool Interact(Entity e) {
			Audio.PlaySfx("level_well");
		
			switch (type) {
				case Type.Transmutation: {
					var active = e.GetComponent<ActiveWeaponComponent>();

					if (active.Item == null) {
						AnimationUtil.ActionFailed();
						return false;
					}
					
					var t = Items.Generate(active.Item.Type, (i) => i.Id != active.Item.Id);

					if (t != null) {
						active.Item.ConvertTo(t);
					}
					
					break;
				}

				case Type.Healing: {
					var hp = e.GetComponent<HealthComponent>();
					hp.ModifyHealth(hp.MaxHealth - hp.Health, this);
					
					break;
				}

				case Type.Coin: {
					var c = e.GetComponent<ConsumablesComponent>();

					if (c.Coins == 0) {
						AnimationUtil.ActionFailed();
						return false;
					}

					c.Coins -= 1;
					var r = Rnd.Int(2);

					if (r == 0) {
						for (var i = 0; i < 8; i++) {
							var bomb = new Bomb(this, Rnd.Float(3, 5));
							
							Area.Add(bomb);
							bomb.CenterX = CenterX;
							bomb.CenterY = Bottom;
						}
					} else if (r == 1) {
						for (var i = 0; i < Rnd.Int(2, 5); i++) {
							var coin = Items.CreateAndAdd("bk:coin", Area);

							coin.CenterX = CenterX;
							coin.CenterY = Bottom;
						}
					}

					break;
				}

				case Type.Death: {
					var hp = e.GetComponent<HealthComponent>();
					hp.ModifyHealth(-hp.Health + 1, this);
					hp.MaxHealth += 2;
					
					break;
				}
			}
			
			used = true;
			return true;
		}

		private void RealRender(bool shadow = false) {
			GraphicsComponent.Render(shadow);

			if (!shadow && !used && water != null) {
				Graphics.Render(water, Position + new Vector2(5, 4));
			}
		}

		public override void Render() {
			RealRender();
		}

		private void RenderShadow() {
			RealRender(true);
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteBoolean(used);
			stream.WriteByte((byte) type);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);

			used = stream.ReadBoolean();
			type = (Type) stream.ReadByte();
		}
	}
}