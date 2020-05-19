pico-8 cartridge // http://www.pico-8.com
version 16
__lua__

debug=false

function deep_copy(obj)
 if (type(obj)~="table") return obj
 local cpy={}
 setmetatable(cpy,getmetatable(obj))
 for k,v in pairs(obj) do
  cpy[k]=deep_copy(v)
 end
 return cpy
end
function shallow_copy(obj)
  if (type(obj)~="table") return obj
 local cpy={} 
 for k,v in pairs(obj) do
  cpy[k]=v
 end
 return cpy
end


-- creates a new object by calling obj = object:extend()
object={}
function object:extend(kob)
  kob=kob or {}
  kob.extends=self
  return setmetatable(kob,{
   __index=self,
   __call=function(self,ob)
	   ob=setmetatable(ob or {},{__index=kob})
	   local ko,init_fn=kob
	   while ko do
	    if ko.init and ko.init~=init_fn then
	     init_fn=ko.init
	     init_fn(ob)
	    end
	    ko=ko.extends
	   end
	   return ob
  	end
  })
end

vector={}
vector.__index=vector
 -- operators: +, -, *, /
 function vector:__add(b)
  return v(self.x+b.x,self.y+b.y)
 end
 function vector:__sub(b)
  return v(self.x-b.x,self.y-b.y)
 end
 function vector:__mul(m)
  return v(self.x*m,self.y*m)
 end
 function vector:__div(d)
  return v(self.x/d,self.y/d)
 end
 function vector:__unm()
  return v(-self.x,-self.y)
 end
function vector:__neq(v)
  return not (self.x==v.x and self.y==v.y)
end
function vector:__eq(v)
  return self.x==v.x and self.y==v.y
end
 -- dot product
 function vector:dot(v2)
  return self.x*v2.x+self.y*v2.y
 end
 -- normalization
 function vector:norm()
  return self/sqrt(#self)
 end
 -- length
 function vector:len()
  return sqrt(#self)
 end
 -- the # operator returns
 -- length squared since
 -- that's easier to calculate
 function vector:__len()
  return self.x^2+self.y^2
 end
 -- printable string
 function vector:str()
  return self.x..","..self.y
 end

-- creates a new vector with
-- the x,y coords specified
function v(x,y)
 return setmetatable({
  x=x,y=y
 },vector)
end


-------------------------------
-- entity: base
-------------------------------

entity=object:extend(
  {
    t=0,
    spawns={}
  }
)

 -- common initialization
 -- for all entity types
function entity:init()  
  if self.sprite then
   self.sprite=deep_copy(self.sprite)
   if not self.render then
    self.render=spr_render
   end
  end
end
 -- called to transition to
 -- a new state - has no effect
 -- if the entity was in that
 -- state already
function entity:become(state)
  if state~=self.state then
   self.state,self.t=state,0
  end
end
-- checks if entity has 'tag'
-- on its list of tags
function entity:is_a(tag)
  if (not self.tags) return false
  for i=1,#self.tags do
   if (self.tags[i]==tag) return true
  end
  return false
end
 -- called when declaring an
 -- entity class to make it
 -- spawn whenever a tile
 -- with a given number is
 -- encountered on the level map
function entity:spawns_from(...)
  for tile in all({...}) do
   entity.spawns[tile]=self
  end
end

function entity:draw_dit(ct,ft,flip)
  draw_dithered(
      ct/ft,
      flip,
      box(self.pos.x+self.hitbox.xl,
      self.pos.y+self.hitbox.yt,
      self.pos.x+self.hitbox.xr,
      self.pos.y+self.hitbox.yb)
      )
end


-------------------------------
-- collision boxes
-------------------------------

-- collision boxes are just
-- axis-aligned rectangles
cbox=object:extend()
 -- moves the box by the
 -- vector v and returns
 -- the result
 function cbox:translate(v)
  return cbox({
   xl=self.xl+v.x,
   yt=self.yt+v.y,
   xr=self.xr+v.x,
   yb=self.yb+v.y
  })
 end

 -- checks if two boxes
 -- overlap
 function cbox:overlaps(b)
  return
   self.xr>b.xl and
   b.xr>self.xl and
   self.yb>b.yt and
   b.yb>self.yt
 end

 -- calculates a vector that
 -- neatly separates this box
 -- from another. optionally
 -- takes a table of allowed
 -- directions
function cbox:sepv(b,allowed)
  local candidates={
    v(b.xl-self.xr,0),
    v(b.xr-self.xl,0),
    v(0,b.yt-self.yb),
    v(0,b.yb-self.yt)
  }
  if type(allowed)~="table" then
   allowed={true,true,true,true}
  end
  local ml,mv=32767
  for d,v in pairs(candidates) do
   if allowed[d] and #v<ml then
    ml,mv=#v,v
   end
  end

  return mv
end
 
--  -- printable representation
--  function cbox:str()
--   return self.xl..","..self.yt..":"..self.xr..","..self.yb
--  end

-- makes a new box
function box(xl,yt,xr,yb) 
 return cbox({
  xl=min(xl,xr),xr=max(xl,xr),
  yt=min(yt,yb),yb=max(yt,yb)
 })
end

-------------------------------
-- entity: dynamic
-------------------------------

dynamic=entity:extend({
    maxvel=1,
    acc=0.5,
    fric=0.5,
    vel=v(0,0),
    dir=v(0,0)
  })

function dynamic:set_vel(grav)  
  if (self.vel.x<0 and self.dir.x>0) or
     (self.vel.x>0 and self.dir.x<0) or
     (self.dir.x==0) then     
    self.vel.x=approach(self.vel.x,0,self.fric)
  else
    self.vel.x=approach(self.vel.x,self.dir.x*self.maxvel,self.acc)
  end

  if grav then
    self.vel.y=approach(self.vel.y, self.term_vel, self.grav*(self.vel.y>0 and 2 or 1))
  else
    if (self.vel.y<0 and self.dir.y>0) or
      (self.vel.y>0 and self.dir.y<0) or
      (self.dir.y==0) then
      self.vel.y=approach(self.vel.y,0,self.fric)
    else
      self.vel.y=approach(self.vel.y,self.dir.y*self.maxvel,self.acc)
    end
  end
end

-------------------------------
-- entity: enemy
-------------------------------

enemy=dynamic:extend({
  collides_with={"player"},
  tags={"enemy"},
  hitbox=box(0,0,8,8),
  c_tile=true,  
  inv_t=30,
  ht=0,
  hit=false,
  sprite=26,
  draw_order=4,
  death_time=15,
  health=1,
  give=1,
  take=1,
  ssize=1,
  svel=0.1,
  clifedrop=0.1,
  coindrop=4
})

function enemy:update()
  self.ht+=1

  if self.hit and self.ht > self.inv_t then
    self.hit=false
    self.ht=0
    self.dir=self.olddir
  end
end

function enemy:dead()
  self.dir.x=0 self.dir.y=0
  if  self.t == ceil(self.death_time/2) then
    local midx=(self.hitbox.xl+self.hitbox.xr)/2
    drop_entity(ceil(self.coindrop),self.pos.x+midx,self.pos.y,coin)
    if(rnd(1)<self.clifedrop)drop_entity(1,self.pos.x+midx,self.pos.y,life)
  end
  if self.t > self.death_time then
    rp_ekilled+=1
    self.done=true
  end
end

function enemy:damage(s)
  if not self.hit then
    sfx(37)
    s=s or 1
    self.health-=s/lp.hm
    p_add(ptext({
      pos=v(self.pos.x-10,self.pos.y),
      txt="+"..s,
      }))
    self.hit=true
    self.ht=0
    self.vel=v(0,0)
    self.olddir=self.dir
    self.dir=v(0,0)
  end

  if self.health <=0 then 
    -- shake+=2 
    self:become("dead") 
  end
end

function enemy:collide(e)
  if (self.state=="dead") return
  if(not self.hit) e_damage(self,e)
end

function enemy:render()
  if (self.hit and self.t%3==0) return
  local s=self.sprite
  s+=(self.t*self.svel)%self.ssize
  spr(s,self.pos.x,self.pos.y,1,1,self.dir.x<0 and true or false)

  if self.state=="dead" then
    self:draw_dit(self.t, self.death_time, true)
  end
end

-------------------------------
-- entity: dmg_box
-------------------------------

dmg_box=dynamic:extend({
  vel=v(0,0),
  collides_with={"enemy"},
  hitbox=box(0,0,4,4),
  dmg=1
})

function dmg_box:update()
  self:set_vel()
end

function dmg_box:collide(e)
  if (e.damage) e:damage(self.dmg*dmg_mod)
end

-------------------------------
-- entity: player
-------------------------------

player=dynamic:extend({
  state="walking", vel=v(0,0),
  tags={"player"}, dir=v(1,0),
  hitbox=box(1,0,3,4),
  c_tile=true,
  health=5,
  maxh=5,
  sprite=0,
  draw_order=7,
  fric=0.5,
  inv_t=30,
  ht=0,
  hit=false,
  dmg=1,
  max_attk_vel=5,
  jump_vel=2,
  grav=0.1,
  jump_height=4*8+4,
  jump_time=45,
  term_vel=2,
  inv=false
})

function player:init()
  self.last_dir=v(1,0)
  self.attk_vel=0
  self.attk_acc=0.15
  self.grav=(2*self.jump_height)/(self.jump_time*self.jump_time/4)
  self.jump_vel=sqrt(2*self.grav*self.jump_height)
  self.precharge_t=0
  self.precharge_mt=5
  self.has_released_charge=true
  self.attk_box=nil
  self.item=nil
end

function player:use_item()
  if self.item ~= nil then
    self.item.pos=v(self.pos.x,self.pos.y)
    self.item:use()
    self.item=nil
  end
end

function player:update()
  self.grounded=solid_at(self.pos.x+self.hitbox.xl, self.pos.y+self.hitbox.yt+1,
                         self.hitbox.xr-self.hitbox.xl, self.hitbox.yb-self.hitbox.yt+1)

  self:set_vel(true)

  self.ht+=1
  if self.hit and self.ht<self.inv_t/2 then return end
  if self.ht > self.inv_t then
    self.hit=false
    self.ht=0
  end

  if self.attk_box~=nil then
    self.attk_box.pos.y=self.pos.y
    self.attk_box.pos.x=self.pos.x+(self.flip and -8 or 4)
  end

  self.dir=v(0,0)
  if btn_hold(0) then self.dir.x = -1 end
  if btn_hold(1) then self.dir.x =  1 end
end

function player:walking()
  
  if self.hit and self.ht<self.inv_t/2 then return end

  -- jump
  if self.grounded then
  ycamtarget=self.pos.y
  lastpoint=ycamtarget
  if (btn_press(2)) self.vel.y = -self.jump_vel
  end
  
  -- variable jumping
  if btn_release(2) then
    self.vel.y /= 3
  end

  if btn_press(4) then
    self:use_item()
  end

  -- charge attack
  if btn_hold(5) and self.has_released_charge then
    self.precharge_t+=1
    if self.precharge_t>self.precharge_mt then
      self.precharge_t=0
      self.attk_vel=self.max_attk_vel/3
      sfx(52,3)
      self:become("charging")
    end
  end

  -- relese charge or normal attack
  if btn_release(5) then
    if self.attk_box~=nil then
      e_remove(self.attk_box)
    end
    self.attk_box=dmg_box{pos=v(self.pos.x+(self.flip and -8 or 4), self.pos.y)}
    e_add(self.attk_box)
    self.attk_box.hitbox.xr=8
    self.attk_box.hitbox.yt-=1
    self.attk_box.hitbox.yt+=1
    if self.has_released_charge then
      self.precharge_t=0
      self:become("attacking")
    else
      self.attk_box.dmg=2
      self.inv=true
      self.vel=v(self.last_dir.x*self.attk_vel, self.vel.y)
      self:become("attacking_charge")
      sfx(36)
    end
    self.has_released_charge=true
    self.attk_vel=0
  end

  if (self.dir~=v(0,0)) self.last_dir=v(self.dir.x,self.dir.y)

  self.maxvel = self.basevel
end

function player:charging()
  if btn_hold(5) then
    self.dir=v(0,0)
    self.attk_vel+=self.attk_acc
    self.attk_vel=clamp(0, self.max_attk_vel, self.attk_vel)
    self.has_released_charge=false

    if (self.attk_vel==self.max_attk_vel) self:become("walking")
  else
    if self.attk_box~=nil then
      e_remove(self.attk_box)
    end
    self.attk_box=dmg_box{pos=v(self.pos.x+(self.flip and -8 or 4), self.pos.y)}
    e_add(self.attk_box)
    self.attk_box.hitbox.xr=8
    self.vel=v(self.last_dir.x*self.attk_vel, self.vel.y)
    self.attk_vel=0
    self.has_released_charge=true
    sfx(36)
    self:become("attacking_charge")
    sfx(-1,3)
  end
end

function player:attacking_charge()
  self.inv=true
  self.attk_box.dmg=2
  self.dir=v(0,0)
  p_add(smoke{pos=v(self.pos.x, self.pos.y+4)})
  if self.vel.x == 0 then
    invoke(function() scene_player.inv=false end,10)
    self.attk_box.dmg=1
    self:become("walking")
    if self.attk_box~=nil then
      e_remove(self.attk_box)
    end
  end
end

function player:attacking()
  self.dir=v(0,0)
  if self.t>5 then
    self:become("walking")
    if self.attk_box~=nil then
      e_remove(self.attk_box)
    end
  end
end

function player:dead()
  self.dir.x=0
  self.dir.y=0

  self.fric=0.05
  if (self.t==0)self.vel.y=-2
end

function player:render()
  local yoff=0
  self.flip = self.last_dir.x < 0 and true or false
  if self.state=="walking" then
    if self.vel.y~=0 then
      self.sprite=6
      yoff=0.5
    elseif self.vel.x==0 then
      if (self.t%4==0) self.sprite+=0.5
      self.sprite=self.sprite%2 + 8
    else
      if (self.t%5==0) self.sprite+=0.5
      self.sprite=self.sprite%2 + 6
    end
  elseif self.state=="charging" then
    self.sprite=6
    pal(12,9)
  elseif self.state=="attacking" or self.state=="attacking_charge" then
    self.sprite=6
    yoff=0.5

    local wx,wy=sprite_to_pixel_coordinates(7, yoff)
    local px=self.pos.x + (self.flip and -8 or 4)
    sspr(wx,wy,8,4,px,self.pos.y,8,4,self.flip)
  elseif self.state=="dead" then
    self.sprite=8
    yoff=0.5
  end

  if(self.attk_vel==self.max_attk_vel)pal(12,9)
  local sx,sy=sprite_to_pixel_coordinates(self.sprite, yoff)
  sspr(sx,sy,4,4,self.pos.x,self.pos.y,4,4,self.flip)
  pal()
end

function player:damage()
  if not self.hit and not self.inv then
    p_add(ptext({
      pos=v(self.pos.x-10,self.pos.y),
      txt="-1"
    }))
    self.ht=0
    self.hit=true
    sfx(37)
    self.health-=1
    if self.health<=0 and self.state~="dead" then
      invoke(restart_game, 60)
      sfx(53)
      music(-1)
      self:become("dead")
    end
    return true
  end  
end

-------------------------------
-- entity: door
-------------------------------
door=entity:extend({
  tags={"door"},
  collides_with={"player"},
  hitbox=box(0,0,8,8),
  sprite=40
})

function door:collide()
  if btn_press(5) then 
    next_level()
  end
end

function door:update()
  for i=1,2 do
    p_add(smoke{pos=v(self.pos.x+rnd(8),self.pos.y+5),c=choose({1,12}),v=0.2})
  end
end

-- function door:render()
--   spr(40,self.pos.x,self.pos.y)
-- end

-------------------------------
-- entity: coin
-------------------------------
coin=dynamic:extend({    
  c_tile=true,
  collides_with={"player"},
  tags={"coin"},
  hitbox=box(0,0,4,4),
  dir=v(0,0),
  grav=0.2,
  fric=0.1,
  term_vel=2,
  sprite=0,
  stop=false
})

function coin:init()
  self.yoff=0
end

function coin:update()
  self:set_vel(self.grav)
  if(self.t==0)sfx(51)
end

function coin:collide(e)
  if (rp_coin_collected) rp_coin_collected+=1*lp.hm
  self.done=true
  sfx(51)
end



function coin:render()
  if (self.t%4==0) self.sprite+=0.5
  if (self.t%8==0) self.yoff+=0.5
  self.sprite=self.sprite%1 + 1
  self.yoff=self.yoff%1

  local sx,sy=sprite_to_pixel_coordinates(self.sprite, self.yoff)
  sspr(sx,sy,4,4,self.pos.x,self.pos.y,4,4,self.flip)
end

-------------------------------
-- entity: life
-------------------------------
life=coin:extend({
  tags={"life"},
})

function life:collide(e)
  if e:is_a("player") and e.health<e.maxh then
    e.health+=1
    self.done=true
  end
end

function life:render()
  sspr(56,16,4,4,self.pos.x,self.pos.y)
end

-------------------------------
-- entity: explosion
-------------------------------

explosion=entity:extend({    
  collides_with={"enemy"},
  hitbox=box(-2,-7,6,6),
  lifetime=5
})

function explosion:init()
  sfx(36)
  -- shake+=2
  self.dmg_box=dmg_box{pos=v(self.pos.x, self.pos.y),hitbox=self.hitbox,dmg=3}
  e_add(self.dmg_box)

  for i=0,10 do
    p_add(smoke{pos=v(self.pos.x+(self.hitbox.xr+self.hitbox.xl)/2+rnd(1)-0.5, 
                      self.pos.y+(self.hitbox.yb+self.hitbox.yt)/2+rnd(1)-0.5),
                c=choose({7,8,9}),r=rnd(2)+1,v=rnd(0.2)+0.2})
  end
  for i in all(tile_flag_at(c_get_entity(self).b,2)) do
    mset(flr(i.x/8),flr(i.y/8),0)
  end
end

function explosion:update()
  if self.t>self.lifetime then 
    self.done=true
    self.dmg_box.done=true
  end
end

-------------------------------
-- entity: bat
-------------------------------

bat=enemy:extend({
  hitbox=box(1,0,8,5),
  collides_with={"player"},
  draw_order=5,
  state="idle",
  attack_dist=30,
  maxvel=0.3,
  fric=2,
  acc=2,
  health=1,
  c_tile=false
})

bat:spawns_from(22)

function bat:init()
  self.orig = v(self.pos.x, self.pos.y)
  self.dir=v(0,0)
  self.vel=v(0,0)
  self.coindrop=3
  self.maxvel*=lp.hm
end

function bat:idle()
  if not scene_player then return end
  if near_player(self.pos,self.attack_dist) then 
    self:become("attacking")
    self.sprite=20
    self.ssize=2
    self.svel=0.05
  end
end

function bat:attacking()
  if not scene_player then return end

  self.dir.x=sign(scene_player.pos.x-self.pos.x)
  self.dir.y=sign(scene_player.pos.y-self.pos.y)

  self:set_vel()

  self.pos += v(0, 0.5*sin(self.t/40+0.5))
end

-------------------------------
-- entity: laser dude
-------------------------------

laserdude=enemy:extend(
  {
    state="wondering",
    vel=v(0,0),
    hitbox=box(2,2,6,5),
    health=3,
    fric=0.07,
  }
)

laserdude:spawns_from(2)

function laserdude:init()
  self.coindrop=6
end

function laserdude:shooting()
  self.vel=v(0,0)

  local mid=v(self.pos.x+(self.hitbox.xl+self.hitbox.xr)/2,self.pos.y+(self.hitbox.yb+self.hitbox.yt)/2)
  if self.t==10 then
    e_add(bullet{dir=v(0,-1),pos=v(mid.x,mid.y-4)})
    e_add(bullet{dir=v(0,1),pos=v(mid.x,mid.y+4)})
    e_add(bullet{dir=v(1,0),pos=v(mid.x+4,mid.y)})
    e_add(bullet{dir=v(-1,0),pos=v(mid.x-4,mid.y)})
  end
  if self.t > 30 then
    self:become("wondering")
  end
end

function laserdude:wondering()
  local wonder_time=60
  if self.t > wonder_time and not self.hit then
    self:become("shooting")
  end

  if self.t == 1 then
    self.dir=v(rnd(2)-1,rnd(2)-1)*0.5
  end

  self:set_vel()
end

-------------------------------
-- entity: slime
-------------------------------

slime=enemy:extend(
  {
    state="wonder",
    vel=v(0,0),
    health=2,
    fric=1,
    maxvel=0.3
  }
)

slime:spawns_from(16)

function slime:init()
  self.coindrop=5
  self.dir=v(choose{-1,1},0)
end

function slime:wonder()
  local wall=solid_at(self.pos.x+self.hitbox.xl+(self.dir.x<0 and -1 or 0), self.pos.y+self.hitbox.yt,
                         self.hitbox.xr-self.hitbox.xl+1, self.hitbox.yb-self.hitbox.yt)
  local grnd=solid_at(self.pos.x+(self.dir.x==1 and self.hitbox.xr or -1), self.pos.y+self.hitbox.yb,
                         1, 1)
  if not grnd or wall then 
    self.dir.x=-self.dir.x
  end

  self:set_vel()
end

-------------------------------
-- entity: spike
-------------------------------

spike=enemy:extend(
  {
    state="waiting",
    vel=v(0,0),
    hitbox=box(0,3,8,8),
    health=100,
    fric=1,
    maxvel=0.3
  }
)

spike:spawns_from(25)

function spike:waiting()
  self.sprite=24
  if self.t>60 then
    self:become("attack")
    -- if(near_player(self.pos,32)) shake+=2
  end
end

function spike:attack()
  self.sprite=25
  if (self.t>30)self:become("waiting")
end

function spike:collide(e)
  if (self.state=="waiting") return
  e_damage(self, e)
end

-------------------------------
-- entity: bullet
-------------------------------

bullet=dynamic:extend({
    collides_with={"player"},
    tags={"bullet"},
    hitbox=box(-1,-1,1,1),
    maxvel=2,
    acc=1,
    c_tile=true,
    lifetime=15,
    r=1.5
})

function bullet:init()
  self.vel = v(0,0)
end

function bullet:update()
  self.set_vel(self)
  if (self.t > self.lifetime) self.done=true
end

function bullet:render()
  circfill(self.pos.x,self.pos.y,self.r,8)
end

function bullet:collide(e)
  p_add_n(2,smoke({
      pos=v(self.pos.x,self.pos.y),
      c=rnd(1)<0.5 and 7 or 9
    }),-1,1,-1,1)
  if e:is_a("player") and e.damage then
    if e:damage() then
      e.vel=self.dir*3
    end
  end
  self.done=true
end

function bullet:tcollide()
  p_add_n(2,smoke(
    {
      pos=v(self.pos.x,self.pos.y),
      c=rnd(1)<0.5 and 7 or 9
    }
  ),-1,1,-1,1)
  self.done=true
end

-------------------------------
-- entity: chest
-------------------------------

chest=entity:extend({
  collides_with={"player"},
  tags={"loot"},
  hitbox=box(0,0,8,8),
  draw_order=1
})

chest:spawns_from(11)

function chest:init()
  local eclass=choose({item,life,coin})
  self.obj=eclass{pos=v(self.pos.x+2,self.pos.y),vel=v(0,-2)}
  self.col=false
  self.sprite=11
end

function chest:open()
  for i=1,5 do
    p_add(smoke({
        pos=v(self.pos.x+rnd(8),self.pos.y+rnd(8)),c=6,r=2
        }))
  end
  if self.obj:is_a("coin") then
    drop_entity(20,self.obj.pos.x,self.obj.pos.y,coin)
  else 
    e_add(self.obj)
  end

  sfx(36)
  -- shake+=2
  self:become("nil")
end

function chest:update()
  local ec,oc=c_get_entity({pos=self.pos,hitbox=box(-4,-4,12,12)}),c_get_entity(scene_player)
  self.col=ec.b:overlaps(oc.b)
  if btn_press(5) and ec.b:overlaps(oc.b) and self.state~="nil" then
    self.sprite=12
    self:become("open");
  end
end

function chest:render()
  spr(self.sprite,self.pos.x,self.pos.y)
  if self.col and self.state~="open" and self.state~="nil" then
    rectfill(self.pos.x+1,self.pos.y-5,self.pos.x+4,self.pos.y-1,7)
    print("\151",self.pos.x,self.pos.y-5,0)
  end
end

-------------------------------------------------------------------
-- items
--    common class for all
--    items
-------------------------------------------------------------------

item=dynamic:extend({    
  c_tile=true,
  collides_with={"player"},
  tags={"item"},
  hitbox=box(0,0,4,4),
  dir=v(0,0),
  grav=0.2,
  fric=0.1,
  term_vel=2,
  sprsize=v(4,4),
  sprcoord=v(56,24)
})

function item:use()
  sfx(37)
  e_add(explosion{pos=v(self.pos.x+4, self.pos.y)})
  e_add(explosion{pos=v(self.pos.x-4, self.pos.y)})
end

function item:update()
  self:set_vel(self.grav)
  p_add_n(1,smoke({
        pos=v(self.pos.x+2,self.pos.y),c=9,r=1
        }),0,0,0,0)
end

function item:collide(e)
  e.item=self
  self.done=true
end

function item:render()
  sspr(self.sprcoord.x,self.sprcoord.y,4,4,self.pos.x,self.pos.y)
end

-------------------------------
-- item: dmg_potion
-------------------------------

dmg_potion=item:extend({
  sprcoord=v(84,8),
  hitbox=box(0,0,8,8),
})

dmg_potion:spawns_from(43)

function dmg_potion:update() end

function dmg_potion:init()
  self.perk=lp.hm*rnd(10)+10
end

function dmg_potion:use()
  sfx(37)
  p_add(ptext({
      pos=v(self.pos.x-10,self.pos.y),
      txt=tostr(flr(self.perk)) .. "% dmg",
      }))
  dmg_mod+=dmg_mod*self.perk/100
  dmg_cost=flr(50 + frac(dmg_mod)*250)
end

function dmg_potion:collide(e)
  if btn_press(5) and rp_coin_collected >= dmg_cost then
    rp_coin_collected-=dmg_cost
    e.item=self
    self.done=true
  end
end

function dmg_potion:render()
  spr(43,self.pos.x,self.pos.y)
  sspr(8,0,4,4,self.pos.x-4,self.pos.y-5)
  print(dmg_cost,self.pos.x+1,self.pos.y-6,9)
end

-------------------------------
-- item: health_potion
-------------------------------

mhealth=12
health_potion=dmg_potion:extend({
  sprcoord=v(80,8),
  hitbox=box(0,0,8,8),
})

health_potion:spawns_from(42)

function health_potion:use()
  sfx(37)
  p_add(ptext({
      pos=v(self.pos.x-10,self.pos.y),
      txt= "+1 health",
      }))
  if (scene_player.maxh<mhealth) scene_player.maxh+=1
  scene_player.health=scene_player.maxh
  health_cost=flr(50 + (scene_player.maxh-4)*50)
end

function health_potion:collide(e)
  if btn_press(5) and rp_coin_collected >= health_cost then
    rp_coin_collected-=health_cost
    e.item=self
    self.done=true
  end
end

function health_potion:render()
  spr(42,self.pos.x,self.pos.y)
  sspr(8,0,4,4,self.pos.x-4,self.pos.y-5)
  print(health_cost,self.pos.x+1,self.pos.y-6,9)
end

-------------------------------------------------------------------
-- particles
--    common class for all
--    particles
-------------------------------------------------------------------

particle=object:extend(
  {
    t=0,vel=v(0,0),
    lifetime=30
  }
)

-------------------------------
-- smoke particle
-------------------------------

smoke=particle:extend(
  {
    vel=v(0,0),
    c=9,
    v=0.1
  }
)

function smoke:init()
  self.vel=v(rnd(0.5)-0.25,-(rnd(1)+0.5))
  if (not self.r) self.r=rnd(1)+0.5
end

function smoke:update()
  self.r-=self.v
  if (self.r<=0) self.done=true
end

function smoke:render()
  if (not self.pos) return  
  circfill(self.pos.x, self.pos.y, self.r, self.c)
end

-------------------------------
-- text particle
-------------------------------

ptext=particle:extend(
  {
    lifetime=45,
    txt="-1",
  }
)

function ptext:init()
  local vx=0
  if (self.vh)vx=rnd(0.5)-0.5
  self.vel=v(vx,-(rnd(1)+0.5))
end

function ptext:update()
  if self.t > self.lifetime/3 then 
    self.vel=v(0,0) 
  end
end

function ptext:render()
  if (not self.pos) return

  draw_text(self.txt,self.pos.x,self.pos.y,self.c or 7)

  if (self.t > 2*self.lifetime/3) draw_dithered((self.lifetime-self.t)/(2*self.lifetime/3),false,box(self.pos.x,self.pos.y,self.pos.x+4*#self.txt+2,self.pos.y+4))
end

function load_level(sx,sy)
  e_add(level({
    base=v(level_index.x*16,level_index.y*16),
    pos=v(level_index.x*128,level_index.y*128),
    size=v(sx,sy)
  }))
end

level=entity:extend({
 draw_order=0,
 tags={"level"}
})
 function level:init()
  local enemy_pool,loot_pool={},{}
  local b,s=
   self.base,self.size
  for x=0,s.x-1 do
   for y=0,s.y-1 do
    -- get tile number
    local blk=mget(b.x+x,b.y+y)    
    -- does this tile spawn
    -- an entity?
    local eclass=entity.spawns[blk]
    if eclass then
     -- yes, it spawns one
     -- let's do it!
     local e=eclass({
      pos=v(b.x+x,b.y+y)*8,
      vel=v(0,0),
      sprite=blk,
      map_pos=v(b.x+x,b.y+y)
     })

    if e:is_a("enemy") then
      add(enemy_pool,e)
    elseif e:is_a("loot") then
      add(loot_pool,e)
    else
      e_add(e)
    end
     -- replace the tile
     -- with empty space
     -- in the map
     --mset(b.x+x,b.y+y,0)
     blk=0
    end
   end
  end

  local en=clamp(0,80,ceil(#enemy_pool*lp.pe))
  local ln=ceil(#loot_pool*lp.pl)

  for i=1,en do
    local e=choose(enemy_pool)
    del(enemy_pool,e)
    e_add(e)
  end

  for i=1,ln do
    local e=choose(loot_pool)
    del(loot_pool,e)
    e_add(e)
  end

  printh(#entities)
  
 end
 -- renders the level
 function level:render()
  map(self.base.x,self.base.y,
      self.pos.x,self.pos.y,
      self.size.x,self.size.y,0x1)
 end

-------------------------------
-- collision system
-------------------------------

function do_movement()
  for e in all(entities) do
      if e.vel and near_player(e.pos,128) then
        for i=1,2 do
          e.pos.x+=e.vel.x/2
          collide_tile(e)
          
          e.pos.y+=e.vel.y/2
          collide_tile(e)
        end
      end
    end
--   end
end

-------------
-- buckets
-------------

c_bucket = {}

function bkt_pos(e)
  local x,y=e.pos.x,e.pos.y
  return flr(shr(x,4)),flr(shr(y,4))
end

-- add entity to all the indexes
-- it belongs in the bucket
function bkt_insert(e)
  local x,y=bkt_pos(e)
  for t in all(e.tags) do
    add(bkt_get(t,x,y),e)
  end

  e.bkt=v(x,y)
end

function bkt_remove(e)
  local x,y=e.bkt.x,e.bkt.y
  for t in all(e.tags) do
    del(bkt_get(t,x,y),e)
  end
end

function bkt_get(t,x,y)
  local ind=t..":"..x..","..y
  if (not c_bucket[ind])c_bucket[ind]={}
  return c_bucket[ind]
end

function bkt_update()  
  for e in all(entities) do
    bkt_update_entity(e)
  end
end

function bkt_update_entity(e)
  if not e.pos or not e.tags then return end
  local bx,by=bkt_pos(e)
  if not e.bkt or e.bkt.x~=bx or e.bkt.x~=by then
    if not e.bkt then
      bkt_insert(e)
    else
      bkt_remove(e)
      bkt_insert(e)
    end
  end
end

-- iterator that goes over
-- all entities with tag "tag"
-- that can potentially collide
-- with "e" - uses the bucket
-- structure described earlier.
function c_potentials(e,tag)
 local cx,cy=bkt_pos(e)
 local bx,by=cx-2,cy-1
 local bkt,nbkt,bi={},0,1
 return function()
  -- ran out of current bucket,
  -- find next non-empty one
  while bi>nbkt do
   bx+=1
   if (bx>cx+1) bx,by=cx-1,by+1
   if (by>cy+1) return nil
   bkt=bkt_get(tag,bx,by)
   nbkt,bi=#bkt,1
  end
  -- return next entity in
  -- current bucket and
  -- increment index
  local e=bkt[bi]
  bi+=1
  return e
 end 
end

function do_collisions()    
  	for e in all(entities) do
      collide(e)
    end
end

function collide(e)
  if not e.collides_with then return end
  if not e.hitbox then return end

  local ec=c_get_entity(e)

  ---------------------
  -- entity collision
  ---------------------
  for tag in all(e.collides_with) do
    --local bc=bkt_get(tag,e.bkt.x,e.bkt.y)
    for o in  c_potentials(e,tag) do  --all(entities[tag]) do
      -- create an object that holds the entity
      -- and the hitbox in the right position
      local oc=c_get_entity(o)
      -- call collide function on the entity
      -- that e collided with
      if o~=e and ec.b:overlaps(oc.b) then
        if ec.e.collide then 
          local func,arg=ec.e:collide(oc.e)
          if (func)func(ec,oc,arg)
        end
      end

    end
  end
end


--------------------
-- tile collision
--------------------

function collide_tile(e)  
  -- do not collide if it's not set to
  if (not e.c_tile) return

  local ec=c_get_entity(e)

  local pos=tile_flag_at(ec.b, 1)

  for p in all(pos) do
    local oc={}
    oc.b=box(p.x,p.y,p.x+8,p.y+8)

    -- only allow pushing to empty spaces
    local dirs={v(-1,0),v(1,0),v(0,-1),v(0,1)}
    local allowed={}
    for i=1,4 do
      local np=v(p.x/8,p.y/8)+dirs[i]
      allowed[i]= not is_solid(np.x,np.y) and not (np.x < 0 or np.x > 127 or np.y < 0 or np.y > 63)
    end

    c_push_out(oc, ec, allowed)
    if (ec.e.tcollide) ec.e:tcollide()    
  end
end

-- get entity with the right position
-- for cboxes
function c_get_entity(e)
  local ec={e=e,b=e.hitbox}
  if (ec.b) ec.b=ec.b:translate(e.pos)
  return ec
end

function tile_at(cel_x, cel_y)
	return mget(cel_x, cel_y)
end

function is_solid(cel_x,cel_y)
  return fget(mget(cel_x, cel_y),1)
end

function solid_at(x, y, w, h)
	return #tile_flag_at(box(x,y,x+w,y+h), 1)>0
end

function tile_flag_at(b, flag)
  local pos={}

	for i=flr(b.xl/8), ((ceil(b.xr)-1)/8) do
		for j=flr(b.yt/8), ((ceil(b.yb)-1)/8) do
			if(fget(tile_at(i, j), flag))add(pos,{x=i*8,y=j*8})
		end
	end

  return pos
end

-- reaction function, used by
-- returning it from :collide().
-- cause the other object to
-- be pushed out so it no
-- longer collides.
function c_push_out(oc,ec,allowed_dirs)
 local sepv=ec.b:sepv(oc.b,allowed_dirs)
 if not sepv then return end
 ec.e.pos+=sepv
 if ec.e.vel then
  local vdot=ec.e.vel:dot(sepv)
  if vdot<0 then   
   if sepv.x~=0 then ec.e.vel.x=0 end
   if sepv.y~=0 then ec.e.vel.y=0 end
  end
 end
 ec.b=ec.b:translate(sepv)
 return sepv
 end
 
--------------------
-- entity handling
--------------------

entities = {}
particles = {}
-- old_ent = {}

function p_add(p)  
  add(particles, p)
end

function p_remove(p)
  del(particles, p)
end

function p_update()
  for p in all(particles) do
    if p.pos and p.vel then
      p.pos+=p.vel
    end
    if (p.update) p:update()

    if (p.t > p.lifetime or p.done)p_remove(p)
    p.t+=1
  end
end

-- adds entity to all entries
-- of the table indexed by it's tags
function e_add(e)
  add(entities, e)

  local dr=e.draw_order or 3
  if (not r_entities[dr]) r_entities[dr]={}
  add(r_entities[dr],e)
end

function e_remove(e)
  del(entities, e)
  for tag in all(e.tags) do        
    if e.bkt then
      del(bkt_get(tag, e.bkt.x, e.bkt.y), e)
    end
  end

  del(r_entities[e.draw_order or 3],e)

  if e.destroy then e:destroy() end
end

-- loop through all entities and
-- update them based on their state
function e_update_all()  
  for e in all(entities) do
    if near_player(e.pos,128) then
      if (e[e.state])e[e.state](e)
      if (e.update)e:update()
      e.t+=1
    end

    if e.done then
      e_remove(e)
    end
  end
end

r_entities = {}

function e_draw_all()
  for i=0,7 do
    for e in all(r_entities[i]) do
      if (e.render and (near_player(e.pos,128) or e:is_a("level")))e:render()
    end
  end
end

function p_draw_all()
  for p in all(particles) do
    if(_update==update_title or near_player(p.pos,128))p:render()
  end
end

function spr_render(e)
  spr(e.sprite, e.pos.x, e.pos.y)
end

-------------------------------
-- helper functions
-------------------------------

function near_player(pos,dist)
  return scene_player and abs(pos.x-scene_player.pos.x) < dist and  abs(pos.y-scene_player.pos.y) < dist
end

function e_damage(self,e)
  if e:is_a("player") and e.damage and e:damage() then
    local d=v(e.pos.x-self.pos.x,e.pos.y-self.pos.y)
    if #d>0.001 then d=d:norm() end
    e.vel=d*3
  end
end

function p_add_n(n,e,sx,ex,sy,ey)
  for i=1,n do
    p_add(e)
    e.pos.x+=rnd(ex-sx)+sx
    e.pos.y+=rnd(ey-sy)+sy
  end
end

function sign(val)
  return val<0 and -1 or (val > 0 and 1 or 0)
end

function frac(val)
  return val-flr(val)
end

function ceil(val)
  if (frac(val)>0) return flr(val+sign(val))
  return val
end

function approach(val,target,step)
  step=abs(step)
  return val < target and min(val+step,target) or max(val-step,target)
end

function sprite_to_pixel_coordinates(spr, yoff)
  spr *= 8
  local y = flr(spr / 256)
  local x = spr - y * 256
  if (yoff~=nil) y += yoff*8
  return x,y
end

function draw_dithered(t,flip,box,c)
  local low,mid,hi=0b0000000000000000.1,
                   0b1010010110100101.1,
                   0b1111111111111111.1                
  if flip then low,hi=hi,low end

  if t <= 0.3 then
    fillp(low)
  elseif t <= 0.6 then
    fillp(mid)
  elseif t <= 1 then
    fillp(hi)
  end

  if box then
    rectfill(box.xl,box.yt,box.xr,box.yb,c or 0)
    fillp()
  end
end

function e_is_any(e, op)
  for i in all(e.tags) do
    for o in all(op) do
      if e:is_a(o) then return true end
    end
  end

  return false
end

function drop_entity(n,x,y,e)
  for i=0,n-1 do
    local c=e{pos=v(x,y),vel=v(choose({1,-1})*(rnd(1)+0.5),-rnd(1)-1)}
    invoke(e_add,i*4,c)
  end
end

invoke_func = {}
function invoke(func,t,p)
  add(invoke_func,{func,0,t,p})
end

function update_invoke()
  for i=#invoke_func,1,-1 do
    invoke_func[i][2]+=1
    if invoke_func[i][2]>=invoke_func[i][3] then
      invoke_func[i][1](invoke_func[i][4])
      del(invoke_func,invoke_func[i])
    end
  end
end

camx = 0
camy = 0
cam_vel = 5
ycamtarget = 0
lastpoint=0
shake = 0

screen_res_x = 8
screen_res_y = 8

-- room types
r_not_solution = 0
r_left_right = 1
r_left_right_down = 2
r_left_right_top = 3

-- directions
--dir_down = 0
dir_left = 1
dir_right = 2
dir_up = 3
dir_any = 4

room_layouts_coord = {[r_not_solution] = {x=96, y=0, n=6},
                      [r_left_right] = {x=112, y=0, n=5},
                      [r_left_right_down] = {x=104, y=0, n=5},
                      [r_left_right_top] = {x=120, y=0, n=5}}

current_level=1
level_params = {[1]={sx=3,sy=3,pe=0.3,pl=0.2,hm=1},
                [2]={sx=4,sy=3,pe=0.4,pl=0.2,hm=1},
                [3]={sx=4,sy=4,pe=0.4,pl=0.3,hm=1.1},
                [4]={sx=5,sy=4,pe=0.5,pl=0.3,hm=1.2},
                [5]={sx=5,sy=4,pe=0.5,pl=0.4,hm=1.5},
                [6]={sx=5,sy=5,pe=0.5,pl=0.4,hm=1.6},
                [7]={sx=6,sy=5,pe=0.5,pl=0.5,hm=1.8},
                [8]={sx=7,sy=5,pe=0.5,pl=0.6,hm=2},
                [9]={sx=8,sy=6,pe=0.5,pl=0.6,hm=2.1},
                [10]={sx=9,sy=7,pe=0.2,pl=0.7,hm=2.2}}

dmg_mod=1

function generate_map(room_res_x, room_res_y, cellx, celly)

    local gen_map = {}
    for i=0,room_res_y-1 do
        for j=0,room_res_x-1 do
            gen_map[i * room_res_x + j] = r_not_solution
        end
    end

    -- choose path
    local ended = false
    local x,y = flr(rnd(room_res_x)), room_res_y-1
    local last_dir = dir_any

    local pos_init = v(x * screen_res_x, y * screen_res_y)
    local pos_end
    
    while(not ended) do
        local next_dir = dir_any
        local new_room_type = r_not_solution
        local pd = {}

        -- direction choosing
        if x > 0 and last_dir ~= dir_right then
            add(pd, dir_left)
            add(pd, dir_left)
        end
        if x < room_res_x-1 and last_dir ~= dir_left then
            add(pd, dir_right)
            add(pd, dir_right)
        end
        if last_dir ~= dir_up then
            add(pd, dir_up)
        end

        next_dir = choose(pd)

        -- room choosing
        if last_dir == dir_up then
            new_room_type = r_left_right_down
        elseif next_dir == dir_up and y > 0 then
            new_room_type = r_left_right_top
        else
            new_room_type = r_left_right
        end

        gen_map[y * room_res_x + x] = new_room_type
        last_dir = next_dir

        if next_dir == dir_up then y -= 1
        elseif next_dir == dir_left then x -= 1
        elseif next_dir == dir_right then x += 1
        end        

        if y >= room_res_y or y < 0 then
            pos_end=v(x*screen_res_x,0)
            ended = true
        end
    end

    -- draw the map
    for i=0,room_res_y-1 do
        for j=0,room_res_x-1 do
            local coord = room_layouts_coord[gen_map[i * room_res_x + j]]
            local rnd_layout = flr(rnd(coord.n))
            for my=0,screen_res_y-1 do
                for mx=0,screen_res_x-1 do
                    mset(j * screen_res_x + mx + cellx, i * screen_res_y + my + celly, mget(coord.x + mx, coord.y + my + screen_res_y * rnd_layout))
                end
            end
        end
    end

    for i=0,room_res_y*screen_res_y do
      mset(0,i,choose({56,57,58,59}))
      mset(room_res_x*screen_res_x+1,i,choose({56,57,58,59}))
    end

    for i=0,room_res_x*screen_res_x do
      mset(i,0,choose({56,57,58,59}))
      mset(i,room_res_y*screen_res_y,choose({56,57,58,59}))
    end

    return pos_init,pos_end

end

function clamp(low, hi, val)
	return (val < low) and low or (val > hi and hi or val)
end

function choose(arg)
    return arg[flr(rnd(#arg)) + 1]
end

last_btn = {false,false,false,false,false,false}
current_btn = {false,false,false,false,false,false}

function update_controller()
  for b=0,5 do
    last_btn[b+1]=current_btn[b+1]
    current_btn[b+1]=btn(b)
  end
end

function btn_press(v)
  return last_btn[v+1]==false and current_btn[v+1]==true
end

function btn_hold(v)
  return current_btn[v+1]
end

function btn_release(v)
  return last_btn[v+1]==true and current_btn[v+1]==false
end

function next_level()
  current_level=clamp(1,#level_params,current_level+1)
  delete_all({"player"})
  init_level()
  _update=update_level
  _draw=draw_level
end

function restart_game()
  _update = update_endgame
  _draw = draw_endgame

  invoke_func={}

  for i=0,95 do
    for j=0,63 do
      mset(i,j,0)
    end
  end

  delete_all({})
  scene_player=nil
end

function delete_all(but)
  for i in all(entities) do
    if(not e_is_any(i,but or {})) i.done = true
  end
  for i in all(particles) do
    i.done=true
  end

  p_update()
  e_update_all()
end

function draw_ui()
  rectfill(0,120,127,127,0)

  print("z",1,121,7)
  rect(7,120,16,127,7)
  
  local item=scene_player.item
  if(item)sspr(item.sprcoord.x,item.sprcoord.y,item.sprsize.x,item.sprsize.y,10,121,4,4)

  print("x",18,121,7)
  rect(24,120,33,127,7)
  local wx,wy=sprite_to_pixel_coordinates(7, 0.5)
  sspr(wx,wy,7,3,26,122,7,3)
  
  local hh=flr(mhealth/2)
  for i=0,scene_player.health-1 do
    print("\135",35+(i%hh*6),122,8)
  end
  for i=scene_player.health,scene_player.maxh-1 do
    print("\135",35+(i%hh*6),122,2)
  end

  sspr(8,0,4,4,108,124)
  print(flr(rp_coin_collected),112,122,7)
end

function update_endgame()
  update_controller()
  if btn_press(5) then
    dmg_mod=1
    init_title()
    _update = update_title
    _draw = draw_title
  end
end

function draw_endgame()
  cls()
  local coins = "coins lost: " .. tostr(flr(rp_coin_collected))
  local totalsec = flr(rp_timer / 30)
  local time = "time: " .. tostr(flr(totalsec / 60)) .. "m " .. tostr(totalsec%60) .. "s"
  local enemies = "kills: " .. tostr(rp_ekilled)
  
  camera()
  print(time, 64 - 4*#time/2,54)
  print(coins, 64 - 4*#coins/2,62)
  print(enemies, 64 - 4*#enemies/2,70)

  local res = "x to restart"
  print(res, 64 - 4*#res/2, 40)
end

function init_level()
  lp=level_params[current_level]
  local pos_init,pos_end = generate_map(lp.sx, lp.sy, 1, 1)

  level_index=v(0, 0)
  load_level(lp.sx*9,lp.sy*9)

  e_add(door{pos=pos_end * 8 + v(32,32)})
  mset(pos_end.x+4,pos_end.y+4,0)
  mset(pos_end.x+4,pos_end.y+5,35)

  local player_pos=pos_init * 8 + v(32, 32)
  if not scene_player then
    scene_player = player{pos=player_pos}
    e_add(scene_player)
  end

  scene_player.pos,scene_player.inv=player_pos,true
  invoke(function() scene_player.inv=false end,60)

  if(is_solid(pos_init.x+4,pos_init.y+4))mset(pos_init.x+4,pos_init.y+4,0)
  mset(pos_init.x+4,pos_init.y+5,35)

  camx,camy,ycamtarget = scene_player.pos.x - 64, scene_player.pos.y - 64,scene_player.pos.y

  p_add(ptext({
      pos=v(scene_player.pos.x-10,scene_player.pos.y),
      txt="level "..tostr(current_level),
      }))
end

function update_level()
  rp_timer+=1
  update_invoke()
  update_controller()
  e_update_all()
  bkt_update()
  do_movement()
  do_collisions()
  p_update()
end

function draw_level()
  cls()

  palt(0, false)
  palt(1, true)
  e_draw_all()
  p_draw_all()

  camera()
  draw_ui()

  -- local s_amount=0
  -- if shake>0 then 
  --   shake-=1
  --   s_amount=4
  -- end
  
  camx = clamp(0,lp.sx*64-111,camx+(scene_player.pos.x-64-camx)/cam_vel)
  if (scene_player.pos.y>lastpoint) ycamtarget=scene_player.pos.y
  camy = clamp(0, lp.sy*64-111, camy+(ycamtarget-70-camy)/(1.5*cam_vel))
  -- camera(camx+rnd(s_amount/2)-s_amount/2, camy+rnd(s_amount/2)-s_amount/2)
  camera(camx,camy)
end

cmusic=8
function init_title()
  cmusic=8
  music(cmusic)
  current_level=1
  rp_timer = 0
  rp_ekilled = 0
  rp_coin_collected = 0
  health_cost=50
  dmg_cost=50
end

function update_title()
  update_controller()
  if btn_press(5) then
    delete_all({})
    cmusic=choose({0,14})
    music(cmusic)
    init_level()
    _update = update_level
    _draw = draw_level
  end

  for i=0,1 do
  	p_add(smoke{pos=v(rnd(128),128),c=choose({6,5}),v=0.01})
  end
  p_update()
end

function draw_text(t,x,y,c)
  local dirs={v(1,1),v(-1,-1),v(1,-1),v(-1,1),v(0,1),v(0,-1),v(1,0),v(-1,0)}

  for d in all(dirs) do
    print(t, x+d.x,y+d.y,0)
  end
  print(t, x,y,c)
end

function draw_title()
  cls()
  palt(0, false)
  palt(1, true)
  camera()
  map(112,48,0,0,16,16)
  p_draw_all()

  local msg1="upcrawl"
  local msg2="press x to start"

  draw_text(msg1, 64 - (#msg1/2)*4,16,13)
  draw_text(msg2, 64 - (#msg2/2)*4,38,13)

  palt()
end

menuitem(1, "music by: " .. (cmusic==14 and "scowsh molosh" or "simon hutchinson"))
menuitem(2, "change music", function() cmusic=(cmusic==14) and 0 or 14 music(cmusic) menuitem(1, "music by: " .. (cmusic==14 and "scowsh molosh" or "simon hutchinson")) end)

function _init()
  -- poke(0x5f2c, 3)
  init_title()
  _update = update_title
  _draw = draw_title
end

__gfx__
00000000000000001100001100000000000000000000000000000d7000000d700d70111100000d70000000000000000007777770000000000000000000000000
000000000aa00090100880010007700000077000006666000d700cc70d707cd00cc000000d700cc0000666600777777070000007000000000000000000000000
000000000a700a90008008000077770000777700007777600c70ccdd07d0dccc07d00d700cd007d0006777707000000770000007000000000000000000000000
0000000000000000080990800777777000077770000066700cd0000d0cd0000c0cd007d007d00cd0007776607007700700000000000000000000000000000000
0000000000000000008008000777077000007770000000700d7000000c0000010000000000000000007666607770077777700777000000000000000000000000
0000000009a009a0100880010070070000777700007777700cc70000cc5666017cdd000000000000007777707007700770077007000000000000000000000000
0000000009000aa0110000110000000000077000006666707cd000000c000001dc7c000000000000076666007000000770000007000000000000000000000000
000000000000000011111111000000000000000000007000c0d00000111111110000000000000000000007007777777777777777000000000000000000000000
10111011000000000000000000700070110111011111111100080800606666061111111111111111077007700000000000000000000000000000000000000000
08010801007000700000000000777770108000801110001100086800000770001111111111111111008000b00000000000000000000000000000000000000000
0880880100770770000000000777777708006008100060000088088000677600111111111000100078877bb70000000000000000000000000000000000000000
08888801007777700070007007006007088606880086068000006000006776001111111110801080077007700000000000000000000000000000000000000000
80000080070000070077777006060606080060080880608800000000006776001111111100800080000000000000000000000000000000000000000000000000
80000808707000070777777706606066100000000080008000000000006776001111111102880288000000000000000000000000000000000000000000000000
20000002600000060766066700666660111111111000000000000000006776000000000002820282000000000000000000000000000000000000000000000000
02222220066666600066666000600060111111111111111100000000000770000222022202220222000000000000000000000000000000000000000000000000
07707077077077700770077007777770666660006666600066660000080802020000000000000000000000000000000000000000000000000000000000000000
77606607776000777660007766666667000000000000000000000000088802220000000000000000000000000000000000000000000000000000000000000000
76000000760076076600706766666667666666006666660066660000008000200000000000000000000000000000000000000000000000000000000000000000
00007606000766060007600666666667666666006666660066660000000000000000000000000000000000000000000000000000000000000000000000000000
60076600600660000076600066666667777766007777660077770000000000000000000000000000000770000007700000000000000000000000000000000000
66006006670000667006600660666667666666606666660066660000000000000000000000000000000080000000b00000000000000000000000000000000000
6660006666700666660000666606666777776660777700007777000000000000055555500000000000788700007bb70000000000000000000000000000000000
06660666066606606660066006666660777777707777000077770000000000006666666600000000000770000007700000000000000000000000000000000000
50505566000560000005600077777777777777707777000077770000009000000777777007707077077077700770077044444444000000000000000000000000
05055660000560000005600006666660777766607777000077770000090000006666666777606607776000777660007740000004000000000000000000000000
00506600000560000005600000077000666666606666660066660000ddd000006666666776000000760076076600706740440404000000000000000000000000
000560000005600000056000007007007777660077776600777700000d0000006666666700007606000766060007600640440404000000000000000000000000
00056000000560000005600000700700666666006666660066660000000000006666666760076600600660000076600040404404000000000000000000000000
00056000000560000050660000077000666666006666660066660000000000006066666766006006670000667006600640404404000000000000000000000000
00056000000560000505566006666660000000000000000000000000000000006606666766600066667006666600006640000004000000000000000000000000
00056000000560005050556677777777666660006666600066660000000000000666666006660666066606606660066044444444000000000000000000000000
60000000000000000000000605505505505555055055550555505055500550050005500505005055005005055050050000000550050000005005500505505005
60000000000000000000000650555556555556656556566566666655650650650050550055055050055055055055050000005050065000006506506506606605
60000000000000000000000600505655565665666667776676665660660770660000500055050055055050050055000000000050066000006607706606605600
60000000000000000000000605555555556566656657777767666655665777770005550550550056055505056065550000000565666500006657777767666655
60000000000000000000000655055655565665666666777676666665666677760055055605565605565656566566650000005556566600006666777676666665
60000000000000000000000600505556556566656666777767665650666677770000505556556566565577576556500000000065666500006666777767665650
60000000000000000000000605555655555665666666777676666655666677760005555655565665667776766666550000000556566600006666777676666655
66666666666666666666666600055556555566656667777767665665666777770000055556556566677777676656650000000055666600006667777767665665
00000000000000000000000000500505565665666667777666666655000000000005505505505555555505555050550000000000000000000000000000000000
60606060606060600000000050555556556566656667777766666655000000000050555556555556565665666666550000000000000000000000000000000000
60606060606060600000000000505655555665666667777676665660000000000000505555565565677766766656600000000000000000000000000000000000
66666666666666600000000005555555556566656667777767666655000000000005555556555566577777676666550000000000000000000000000000000000
60000000000000600000000055055555565665666666777676666665000000000055055555555665667776766666650000000000000000000000000000000000
60000000000000600000000000505556556566656666777767665650000000000000505555555566667777676656500000000000000000000000000000000000
60000000000000600000000005555655565665666566777676666655000000000005555655565665667776766666550000000000000000000000000000000000
66666666666666600000000000050556555566656667777767665665000000000000055556556566677777676656650000000000000000000000000000000000
00000000000000000000000000500505565665666667777666666655000000000000500505565665677776666666550056600500505565666667777666666655
00000000000000000000000050555556555566656667777766666655000000000050555555555566677777666666550066550555556556566667777766666655
00000000000000000000000000505655565665666667777676665660000000000000505655565665677776766656600056600505655565666667777676665660
00000000000000000000000005555555556566656667777767666650000000000005555556555566677777676666550066505555556556566667777767666650
00000000000000000000000055055555555665666666767676666600000000000055055655565665667776766666650056655055655565666666767676666600
00000000000000000000000000505555556566656666676756665600000000000000505555555566667777676656500066500505556556566666676756665600
00000000000000000000000005555655565665666565656565665650000000000005555555555665667776766666550056605555655565666565656565665650
00000000000000000000000000050555555555555555555555555555000000000000050556555566677777676656650055500050555555555555555555555555
00000000000000000000000000005005055656656777766666665500000000000000500505565665677776666666550000000000000000000000000000000000
00000000000000000000000000505555555555666777776666665500000000000050555556555566677777666666550000000000000000000000000000000000
00000000000000000000000000005056555656656777767666566000000000000000505655555665677776766656600000000000000000000000000000000000
00000000000000000000000000055555560005666777776766665500000000000005555556556566677777676666500000000000000000000000000000000000
00000000000000000000000000550556555006656677767666666500000000000055055555565665667676766666000000000000000000000000000000000000
00000000000000000000000000005055550055666677776766565000000000000000505556555566666767566656000000000000000000000000000000000000
00000000000000000000000000055555555006656677767666665500000000000005555655565665656565656656500000000000000000000000000000000000
00000000000000000000000000000505565555666777776766566500000000000000050555555555555555555555550000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000222222222222222212121212121212120000002000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000a200000000b200000061000061000000000000000000000001000000000100
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000001212120000121212000000000000000000000000010000000002020000020200
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000091009100910091000012121212000000010012120001000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000022002200220012000100000000010000121212121212000000020202020000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000009100910091009100002222000022120000000012120000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000002200220022002200000000000000000000919191919191000000002000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000220012000022002212121212121212120202020202020202
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000c3c332c332c33232000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000032000000000000c3000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000c300000000000032000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000003200b0b0b0b000c3000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000003200323232320032000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000c3000000000000c3000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000003200000000000032000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000c3c33232c33232c3000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000008494a4b4000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000037475767000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000037475767000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000037475767000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000037475767000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000037475767000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000037475767000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000037475767000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000034445464000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000036465666000000000000
__gff__
0000000000000000000000000000000000000000000000000000000000000000030303030000000000000000000000000101010300000000030303030700000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
__map__
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000002020212220212021202022212022202020000022212220202220210000202221
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000002000000000000020220000000000002000000010000000222000001900160000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000020002a00002b0022000023000023232100003323330000200000233323000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000230000230000000030000016000000023000300000000000300030000200
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001919000000000031000000000000003100232322000010320031000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000190b23230b1900332331000000000021003100310022202220210031002020
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000002223232023202320230032000000002222203219320020200000000032000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000002021222222202022222122000021222021222021222022212100002120222020
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000002020000000002020212100000021212120212220202121202020000000222021
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000002000000010000020210000000000162121160000100000000000000200000020
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000003333330000000000230000222120002233230000000019190023332320
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000003000300000000000000023230000000030000000000023220000300000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000003100310000000033000000000000000031000000000000000000320000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000022223100310000000030000023232120212131000021202100003333330021
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000002222320032000021203200003c0b190000003210190b192000003100310020
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000002121202020202121212121000020212122202221202220202200212121212222
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000002220222322222022212221232221232121232120232120202000000000000020
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000002323000000000000000021000000000000000010000000000000190000190000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000003c00000000000000190b230b1900000000230020210000000000220019230000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000003c0b002300000000232123212300190000001000000000000000000023200000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000002323000000000000000000000000230000002023002100000000001000000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000003c30000000230022000000000000000000191919190000000000232123220000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000003c32191919191922000000000000000000222123210000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000002022232220232222212100000000212121232122212321222020202020202020
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000002121000021210021222222222222222222222222222222220000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000210000000000003c001600000000160000001600001600000000002020000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000003c00000000000021000000100000000000000000000000000019000000001900
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000002100020b0b020021000023222300000000000000000000000020190000192000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000210000232300003c001900190019000000212102022121000020200000202000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000003c0000000000003c002000200020000021212100002121210021200000202100
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000003c19001900190021000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000003c21002100210021222200000000222222222222222222222020202020202020
__sfx__
010e01000c0530c053000000c0530f617000000c053000000c053000000f6170c0530d023000000c053000000c053000000f6170c0530c000000000c0530d0230c053000000f617000001b653000000f61700000
010e00200c0530c053000000c0530f617000000c053000000c053000000f6170c0530d023000000c053000000c053000000f6170c0530c000000000c0530d0230c053000000f617000000c053000000f61700000
010e00000c05300000000000000000000000000c053000000c05300000000000c0530d023000000c053000000c05300000000000c0530c000000000c0530d0230c0530000000000000000c053000000f61700000
010e00200c0530c053000000c0531b653000000c053000000c05300000000000c0531b653000000c053000000c053000000f6170c0531b653000000c0530d0230c053000000f617000001b653000000f61700000
010e000002050020500205502051020500205502050020550405104050040500405504050040550405004050040500705505050050550405004055000000000009050090550b0500b0550b0500b0550000000000
010e0000007000070000700007003472434725347243472234724347253272432722307223072230722307252f7242d7242b7242b725007000070000700117002b7242b7202d7202d7252d7202d7250070000700
010e0000007000070000700007003072030720307203072230722307252b7242b7202b7202b7222b7222b7252d7242d7202d7222d7202d7222d7222d7150070000700007002b7000070000700007000070000700
010e0000007000070000700007002b7202b7202b7202b7222b7222b7252d7242d7202d7202d7222d7222d725287222872228722287222872228722287222872500700007002b7000070000700007000070000700
010e0000007000070000700007002b7202b7202b7202b7222b7222b7252d7242d7202d7202d7222d7222d7252e7222e7222e7222e7222e7222e7222e7222e72500700007002b7000070000700007000070000700
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010e00000206002060020650206102060020650206002065040610406004060040650406004065040600406002060020650206002065050600506505020050200902009020090600906509060090650902009020
010e000002060020600206502061020600206502060020650406104060040600406504060040650406004060040600706505060050650406004065000000000009060090650b0600b0650b0600b0650000000000
011c000005060050600506005065090610906009060090650b0610b0600b0600b0650706107060070600706505060050600506005065090610906009060090650406104060040600406507061070600706007065
011c000005060050600506005065090610906009060090650b0610b0600b0600b0650706107060070600706505060050600906009065040610406005060050650206102060020600206002060020650200000000
010e00000506405064050640506500000000000506405062050620506200000000000506205062050620506505064050640506405065000000000005062050620506205065000000000005062050620506200000
010e000007064070640706407065000000000007064070620706207062000000000007062070620706207065070640706407064070650000000000070620706207062070650000000000070620a0620a0620a065
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010e0000007000070000700007003473434735347343473234734347353273432732307323073230732307352f7342d7342b7342b73500700007002f7202f7202d7312d7302d7302d73500700007000070000700
010e0000007000070000700007003473434735347343473234734347353273432732307323073230732307352f7342d7342b7342b735007000070000700117002b7342b7302d7302d7352d7302d7350070000700
011c0000000002d0302b0312b03529030290352803028035260302b0312b0322903528031280322803228035260302b0312b03229030280302603024031240352403224035260302603226032260350000000000
011c0000000002d0302b0312b03529030290352803028035260302b0312b032290312803228032280322d0352f0302b0312b03228035290312603528031240352303123032210312103221032210350000000000
011c000000500305202f5202f5252d5202d5252b5202b525295202f5202f5222d5212b5222b5222b5222b525295202f5202f5222d5202b5202952128521285252852028525295202952229522295250050000500
011c000000500305202f5202f5252d5202d5252b5202b525295202f5202f5222d5202b5222b5222b52230525325202f5202f5222b5252d521295252b521285252652126522245212452224522245250050000500
011c0000007000070000700007002d734347313473434735007000070000700007002b734267312673426735007000070000700007002d734347313473434735007000070000700007002b7352b7312b7342b735
010e00000050000500005000050037524375253752437522375243752537524375223b5223b5223b5223b5252f5002f5002b5002b500005000050032520325203052130520305203052500500005000050000500
010e00000050000500005000050037524375253752437522375243752537524375223b5223b5223b5223b5253c5243b5243952437524395243752435524345243752435524345243252535515345153251530515
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000200001567015670146701466014650156501564015640156401564015630156301563015620156201562015610156101561015600156001460014600146000000000000000000000000000000000000000000
000100000f6700f6700f6700f6500f6400f6300f6200e6100e6100e6100e6100e6100e6100e6100e6100e61007600000000000000000000000000000000000000000000000000000000000000000000000000000
011100000d05300000000000d02300000000000d013000000d05300000000000d0231f653000000d053000001d6530d023000031d6230d013000001d613000001f65300000000001f6530d053000001f6530d023
011100002855428535285153000000000000002b5542b5352b515000002d5542d5352f5542f5352f515005002d5542d5352d51500000000000000039004390053900521200000000000000000000000000000000
011100232a7722a715267502671523750237151c7501c7152a7722a715267502671523750237151c7501c7152a7722a7152575025715217502171520750207152a7722a715257502571521750217152075020715
011100200c0410c0200c0200c0200c0200c0200c0200c0200c0200c0220c0220c0220c0220c0220c0220c0450e0210e0200e0200e0200e0200e0200e0200e0200e0200e0220e0220e0220e0220e0220e0220e025
011100000d05300000086150d02300000000000d0133e600266200000000000136100000000000056150b0230d053000000a0130d02300000000000d01300000286200000000000156102862000000086101a615
0111000030054300353001500000302003020034054340353401500000360543603537054370353701531200360543603536015000003620000000360443603536015000003b7403b7153a7403a7153474434715
011100003005430035300150000000000000003405434035340150000036054360353705437035370153720536054360353601500000000000000000000000000000000000000000000000000000000000000000
011100001002110020100201002010020100201002010020100201002010020100201002010020100201002010022100221002210022100221002210022100221002210022100221002210020100201002010025
011100001402114020140201402014020140201402014020140201402014020140201402014020140201402014022140221402214022140221402214022140221402214022140221402500000000001f6541f670
011100002a7722a7152775027715237502371520750207152a7722a7152775027715237502371520750207152a7722a7152775027715237502371520750207152a7722a715277502771523750237152075020715
011100002b7722b7152675026715247502471520750207152b7722b7152675026715247502471520750207152b7722b7152675026715247502471520750207152b7722b715267502671500000000000000000000
011100003807438075000000000020204380443804500000000002020438034380350000000000000003802438025000000000000000380143801500000000000000000000000000000000000000000000000000
0111000037074370750000000000000003704437045300003000030000370343703530000300003000037024370253000030000300003701437015300003000030000000000000000000103730c2730c2730c273
0001000020370283702f37036370383002a40033400257001d400264003c4001a0001f000250002f0003200000000000000000000000000000000000000000000000000000000000000000000000000000000000
000300000221002210032100321003210032100321003220042200422004220042200423005230052300523006240062400724008240092400a2400b2400c2500e250102501225014250172501a2501d2501f250
010600003407434073300003000034064340633000030000340543405330000300003404434043300003000034034340333000030000340243402330000300003401434013300003000034014340130000030000
__music__
01 28696a44
00 28692a6b
00 2842292b
00 28292a2c
00 292a272b
00 292a272c
00 2d2f2631
02 2e302632
01 292b6726
02 292c7226
00 41424344
00 434a5444
00 434b5444
00 414c5544
01 410c5519
00 010d5619
00 010c1557
00 010d1659
00 030c1517
00 030d1618
00 020a1344
00 020b1444
00 020a131a
00 020b141b
00 010e4344
00 020e4344
00 030c1517
00 030d1618
00 030c1505
00 030d1605
00 010e4344
02 020e4344
01 010e4444
00 010f4344
00 010e0644
00 010f0744
00 010e0644
02 010f0844

