using BurningKnight.entity.creature.npc;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using Lens.util.file;

namespace BurningKnight.entity.level.entities {
	public class HatSelector : ItemHolder {
		public static List<HatSelector> All = new List<>();
		public static bool NullGot;
		private string Key;
		private ItemRegistry.Pair Pair;
		public bool Set;

		public override void Init() {
			base.Init();
			All.Add(this);
		}

		public override void Destroy() {
			if (GetItem() is NullItem) NullGot = false;

			if (Pair != null) Pair.Shown = false;

			All.Remove(this);
			base.Destroy();
		}

		public override void Render() {
			base.Render();

			if (!Set && GetItem() == null || Upgrade.UpdateEvent) {
				foreach (Map.Entry<string, ItemRegistry.Pair> Pair in ItemRegistry.Items.EntrySet())
					if (!Pair.GetValue().Shown && Pair.GetValue().Pool == Upgrade.Type.DECOR && !Pair.GetKey().Equals(Key) && Pair.GetValue().Unlocked(Pair.GetKey())) {
						Pair.GetValue().Shown = true;
						this.Pair = Pair.GetValue();
						Key = Pair.GetKey();

						try {
							SetItem((Item) ItemRegistry.Items.Get(Pair.GetKey()).Type.NewInstance());
						}
						catch (InstantiationException |

						IllegalAccessException) {
							E.PrintStackTrace();
						}

						Set = true;

						break;
					}

				if (!Set && !NullGot && Player.HatId != null && Player.HatId.Equals("gobbo_head")) {
					Set = true;
					Key = "null";
					SetItem(new NullItem());
					NullGot = true;
				}
			}
		}

		public override void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteString(Key);
		}

		public override void Load(FileReader Reader) {
			base.Load(Reader);
			Key = Reader.ReadString();
		}

		public override void RenderShadow() {
			if (Set) base.RenderShadow();
		}
	}
}