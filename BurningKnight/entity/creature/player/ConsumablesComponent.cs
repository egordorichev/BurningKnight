using BurningKnight.assets;
using BurningKnight.assets.achievements;
using BurningKnight.assets.input;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.bomb;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using BurningKnight.util;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.input;
using Lens.util;
using Lens.util.file;
using Lens.util.math;

namespace BurningKnight.entity.creature.player {
	public class ConsumablesComponent : ItemComponent {
		private byte bombs;
		private byte keys;
		private byte coins;

		public byte MaxCoins = 99;

		public int Bombs {
			set {
				var n = (byte) MathUtils.Clamp(0, 99, value);

				if (n != bombs && AcceptChange(n - bombs, n, ItemType.Bomb)) {
					bombs = (byte) MathUtils.Clamp(0, 99, ((int) bombs + lastAmount));
				}
			}
			
			get => bombs;
		}

		public int Keys {
			set {
				var n = (byte) MathUtils.Clamp(0, 99, value);

				if (n != keys && AcceptChange(n - keys, n, ItemType.Key)) {
					keys = (byte) MathUtils.Clamp(0, 99, (int) keys + lastAmount);
				}
			}
			
			get => keys;
		}
		
		public int Coins {
			set {
				var n = (byte) MathUtils.Clamp(0, MaxCoins, value);

				if (n != coins && AcceptChange(n - coins, n, ItemType.Coin)) {
					coins = (byte) MathUtils.Clamp(0, MaxCoins, (int) coins + lastAmount);

					if (coins >= 99) {
						Achievements.Unlock("bk:rich");
					}
				}
			}
			
			get => coins;
		}

		private int lastAmount;
		
		private bool AcceptChange(int amount, int totalNow, ItemType type) {
			var e = amount > 0
				? (Event) new ConsumableAddedEvent {
					Amount = amount,
					TotalNow = totalNow,
					Type = type
				}
				: new ConsumableRemovedEvent {
					Amount = amount,
					TotalNow = totalNow,
					Type = type
				};

			lastAmount = amount;

			if (!Send(e)) {
				if (e is ConsumableAddedEvent c) {
					lastAmount = c.Amount;
				} else {
					lastAmount = ((ConsumableRemovedEvent) e).Amount;
				}
	
				return true;
			}
			
			return false;
		}

		public override bool HandleEvent(Event e) {
			if (e is ItemCheckEvent ev) {
				var type = ev.Item.Type;
				
				if (type == ItemType.Bomb || type == ItemType.Key || type == ItemType.Coin || type == ItemType.Battery || type == ItemType.Pouch) {
					// var a = Entity.GetComponent<AudioEmitterComponent>();
					
					switch (type) {
						case ItemType.Bomb: {
						
							if (Run.Depth > 0 && GlobalSave.IsFalse("control_bomb")) {
								var dialog = GetComponent<DialogComponent>();

								dialog.Dialog.Str.ClearIcons();
								dialog.Dialog.Str.AddIcon(CommonAse.Ui.GetSlice(Controls.FindSlice(Controls.Bomb, false)));

								if (GamepadComponent.Current != null && GamepadComponent.Current.Attached) {
									dialog.Dialog.Str.AddIcon(CommonAse.Ui.GetSlice(Controls.FindSlice(Controls.Bomb, true)));
								}

								dialog.StartAndClose("control_0", 5);
							}

							Audio.PlaySfx("item_bomb");
							break;
						}
						
						case ItemType.Key: {
							Audio.PlaySfx("item_key_pickup");
							break;
						}
						
						case ItemType.Coin: {
							switch (ev.Item.Id) {
								case "bk:emerald": {
									Audio.PlaySfx("item_emerald", 1f - Audio.Db3);
									break;
								}

								case "bk:platinum_coin": {
									Audio.PlaySfx("item_platinum_coin", 0.2f);
									break;
								}

								case "bk:gold_coin": {
									Audio.PlaySfx("item_gold_coin", 0.2f);
									break;
								}

								case "bk:iron_coin": {
									Audio.PlaySfx("item_silver_coin", 0.2f);
									break;
								}

								default: {
									Audio.PlaySfx("item_coin", 0.2f);
									break;
								}
							}

							break;
						}
						
						case ItemType.Battery: {
							Audio.PlaySfx("item_battery");
							break;
						}

						case ItemType.Pouch: {
							Audio.PlaySfx("item_bag");
							break;
						}
					}
					
					Send(new ItemAddedEvent {
						Item = ev.Item,
						Who = Entity,
						Component = this
					});

					var p = (Player) Entity;
					
					ev.Item.RemoveDroppedComponents();
					
					for (var i = 0; i < 4; i++) {
						Entity.Area.Add(new ParticleEntity(Particles.Dust()) {
							Position = ev.Item.Center, 
							Particle = {
								Scale = Rnd.Float(0.4f, 0.8f)
							}
						});
					}

					Engine.Instance.State.Ui.Add(new ConsumableParticle(ev.Item.Animation != null
						? ev.Item.GetComponent<AnimatedItemGraphicsComponent>().Animation.GetFirstCurrent()
						: ev.Item.Region, p, false, () => {
							ev.Item.Use(p);
							ev.Item.Done = true;
						}, ev.Item.Id == "bk:emerald"));
					
					return true;
				}
			}
			
			return base.HandleEvent(e);
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Input.WasPressed(Controls.Bomb, GetComponent<GamepadComponent>().Controller)) {
				if (GetComponent<PlayerInputComponent>().InDialog) {
					return;
				}
			
				if (Run.Depth > 0 && GlobalSave.IsFalse("control_bomb")) {
					Entity.GetComponent<DialogComponent>().Close();
					GlobalSave.Put("control_bomb", true);
				}
				
				if (GetComponent<StateComponent>().StateInstance is Player.SleepingState) {
					GetComponent<StateComponent>().Become<Player.IdleState>();
				}

				var spawn = false;

				if (bombs > 0) {
					Bombs--;

					spawn = true;
				} else {
					var h = GetComponent<HeartsComponent>();

					if (h.Bombs > 0) {
						h.ModifyBombs(-1, Entity, true);
						spawn = true;
					}
				}

				if (spawn) {
					var bomb = new Bomb(Entity);
					Entity.Area.Add(bomb);
					bomb.Center = Entity.Center;
					bomb.MoveToMouse();
				} else {
					AnimationUtil.ActionFailed();
				}
			}
		}

		public override void Set(Item item, bool animate = true) {
			item.Done = true;
		}

		protected override bool ShouldReplace(Item item) {
			return item.Type == ItemType.Bomb || item.Type == ItemType.Key || item.Type == ItemType.Coin || item.Type == ItemType.Pouch;
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteByte(bombs);
			stream.WriteByte(keys);
			stream.WriteByte(coins);
		}

		public override void Load(FileReader reader) {
			base.Load(reader);

			bombs = reader.ReadByte();
			keys = reader.ReadByte();
			coins = reader.ReadByte();
		}
	}
}
