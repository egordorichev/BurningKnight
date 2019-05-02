using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.level.rooms;
using Lens.assets;
using Lens.entity;

namespace BurningKnight.state {
	public class InGameAudio : Entity {
		public override void Init() {
			base.Init();
			
			Subscribe<RoomChangedEvent>();
			Subscribe<SecretRoomFoundEvent>();
			
			Audio.PlayMusic(Run.Level.Biome.Music);
		}

		public override bool HandleEvent(Event e) {
			if (e is RoomChangedEvent re && re.Who is LocalPlayer) {
				switch (re.New.Type) {
					case RoomType.Secret: {
						Audio.PlayMusic("Serendipity");
						break;
					}

					case RoomType.Shop: {
						Audio.PlayMusic("Shopkeeper");
						break;
					}

					default: {
						Audio.PlayMusic(Run.Level.Biome.Music);
						break;
					}
				}
			} else if (e is SecretRoomFoundEvent) {
				Audio.Stop();
				
				Audio.PlaySfx("secret");
				Audio.PlaySfx("secret_room");
			}
			
			return false;
		}
	}
}