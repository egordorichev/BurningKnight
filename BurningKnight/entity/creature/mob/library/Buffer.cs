using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.desert;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.library {
	public class Buffer : Mob {
		private const float SafeDistance = 128f;
    		
    protected override void SetStats() {
    	base.SetStats();

    	Width = 11;
    	Height = 15;

    	SetMaxHp(16);
    	
    	var body = new RectBodyComponent(3, 14, 6, 1);
    	AddComponent(body);

    	body.Body.LinearDamping = 4f;
    	
    	AddComponent(new SensorBodyComponent(2, 1, 7, 14));
    	AddComponent(new MobAnimationComponent("buffer"));
    	
    	Become<RunState>();
    }

    #region Skeleton States
    public class RunState : SmartState<Buffer> {
    	public override void Update(float dt) {
    		base.Update(dt);

    		if (Self.Target != null) {
    			if (!Self.CanSeeTarget() || Self.MoveTo(Self.Target.Center, 80f, SafeDistance, true)) {
    				Become<SummonState>();
    			}
    		}
        
        Self.PushOthersFromMe(dt);
    	}
    }

    public class SummonState : SmartState<Buffer> {
	    private Mob mob;
	    
    	public override void Init() {
    		base.Init();
    		Self.GetComponent<MobAnimationComponent>().Animation.Tag = "idle";
    	}

      public override void Destroy() {
	      base.Destroy();
	      mob?.GetComponent<BuffsComponent>().Remove<BuffedBuff>();
      }

      public override void Update(float dt) {
    		if (Self.CanSeeTarget() && Self.DistanceTo(Self.Target) < SafeDistance - 16) {
    			Become<RunState>();
    			return;
    		}

        if (mob != null) {
	        if (mob.Done) {
		        mob = null;
	        }

	        T = 0;
        }

    		if (T >= 3f) {
    			T = 0;
          var list = Self.GetComponent<RoomComponent>().Room.Tagged[Tags.Mob];

          if (list.Count == 0) {
	          return;
          }

          do {
	          mob = (Mob) list[Rnd.Int(list.Count)];
          } while (mob == Self);

          mob.GetComponent<BuffsComponent>().Add(new BuffedBuff() {
						Infinite = true
          });
        }
    	}
    }
    #endregion
	}
}