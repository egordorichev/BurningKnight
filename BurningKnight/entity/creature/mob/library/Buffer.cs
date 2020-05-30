using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.library {
	public class Buffer : Mob {
		private const float SafeDistance = 128f;
    		
    protected override void SetStats() {
    	base.SetStats();

    	Width = 11;
    	Height = 15;
      TouchDamage = 0;

    	SetMaxHp(32);
    	
    	var body = new RectBodyComponent(3, 14, 6, 1);
    	AddComponent(body);

    	body.Body.LinearDamping = 4f;
    	
    	AddComponent(new SensorBodyComponent(2, 1, 7, 14));
    	AddComponent(new MobAnimationComponent("buffer"));
    	
    	Become<RunState>();
    }

    public override void Destroy() {
	    base.Destroy();
	    mob?.GetComponent<BuffsComponent>().Remove<BuffedBuff>();
    }

    #region Buffer States
    public class RunState : SmartState<Buffer> {
	    public override void Update(float dt) {
    		base.Update(dt);

    		if (Self.Target != null) {
	        if (!Self.CanSeeTarget() || Self.MoveTo(Self.Target.Center, 60f, SafeDistance, true)) {
    				Become<SummonState>();
    			}
    		}
        
        Self.PushOthersFromMe(dt);
    	}
    }

    private Mob mob;

    public class SummonState : SmartState<Buffer> {
	    public override void Init() {
		    base.Init();
		    Self.GetComponent<MobAnimationComponent>().Animation.Tag = "idle";
	    }

	    public override void Update(float dt) {
		    base.Update(dt);
	      
		    if (Self.CanSeeTarget() && Self.DistanceTo(Self.Target) < SafeDistance - 16) {
			    Become<RunState>();
			    return;
		    }

		    if (T >= 3f) {
			    var list = Self.GetComponent<RoomComponent>().Room.Tagged[Tags.Mob];

			    if (list.Count == 0) {
				    return;
			    }

			    do {
				    Self.mob = (Mob) list[Rnd.Int(list.Count)];
			    } while (Self.mob == Self);

			    Self.mob.GetComponent<BuffsComponent>().Add(new BuffedBuff() {
				    Infinite = true
			    });
			    
			    Become<BuffState>();
		    }
	    }
    }

    public class BuffState : SmartState<Buffer> {
      public override void Destroy() {
	      base.Destroy();
	      Self.mob?.GetComponent<BuffsComponent>().Remove<BuffedBuff>();
      }

      public override void Update(float dt) {
	      base.Update(dt);
	      
    		if (Self.CanSeeTarget() && Self.DistanceTo(Self.Target) < SafeDistance - 16) {
    			Become<RunState>();
    			return;
    		}

        if (Self.mob != null && Self.mob.Done) {
	        Self.mob = null;
	        Become<SummonState>();
        }
      }
    }
    #endregion
		
    protected override string GetHurtSfx() {
	    return "mob_wizard_hurt";
    }

    protected override string GetDeadSfx() {
	    return "mob_wizard_death";
    }
	}
}