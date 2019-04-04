using BurningKnight.assets;
using BurningKnight.entity.item;
using BurningKnight.entity.item.use;

namespace TestMod {
	public class Program : Mod {
		public void Init() {
			ItemRegistry.Register(this, new ItemInfo("test", () => new Item(
				new ModifyHpUse(2),
				new TestUse()
			)));
		}

		public void Destroy() {
			
		}

		public void Update(float dt) {
			
		}

		public void Render() {
			
		}

		public string GetPrefix() {
			return "tm";
		}
	}
}