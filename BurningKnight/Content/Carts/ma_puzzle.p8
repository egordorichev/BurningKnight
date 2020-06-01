pico-8 cartridge // http://www.pico-8.com
version 19
__lua__
-- ma puzzle
-- by @egordorichev

local osfx=sfx
function sfx(id)
 if (not g_mute_sfx) osfx(id)
end

function play_music(id)
 if (not g_mute_music) music(id)
end

function _init()

 g_time,state,g_index=
 0,ingame,
 
 0
  	 
 g_moves=0 
 shk=0
 restart_level()
 -- ∧⌂웃⬇️♥★█⬅️⬇️♥⌂★█⬅️
 m()
 music(0)
end

function _update60()
 state.update()
 tween_update(1/60)
 g_time+=1
end

function _draw() 
 state.draw()
end

function restart_level()
 g_cgot=false
 reload(0x2000,0x2000,0x1000)
 reload(0x1000,0x1000,0x1000)
 
 entity_reset()
 collision_reset()
 
 g_level=e_add(level({
  base=v(g_index%8*16,flr(g_index/8)*16),
  size=v(16,16)
 }))
end

function lget(x,y)
 return mget(x+g_level.base.x,y+g_level.base.y)
end
-->8
-- oop

function deep_copy(obj)
 if (type(obj)~="table") return obj
 local cpy={}
 setmetatable(cpy,getmetatable(obj))
 for k,v in pairs(obj) do
  cpy[k]=deep_copy(v)
 end
 return cpy
end

function index_add(idx,prop,elem)
 if (not idx[prop]) idx[prop]={}
 add(idx[prop],elem)
end

function event(e,evt,p1,p2)
 local fn=e[evt]
 if fn then
  return fn(e,p1,p2)
 end
end

function state_dependent(e,prop)
 local p=e[prop]
 if (not p) return nil
 if type(p)=="table" and p[e.state] then
  p=p[e.state]
 end
 if type(p)=="table" and p[1] then
  p=p[1]
 end
 return p
end

function round(x)
 return flr(x+0.5)
end

-------------------------------
-- objects
-------------------------------

object={}
 function object:extend(kob)
  -- printh(type(kob))
  if (kob and type(kob)=="string") kob=parse(kob)
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
 
-------------------------------
-- vectors
-------------------------------

vector={}
vector.__index=vector
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
 function vector:dot(v2)
  return self.x*v2.x+self.y*v2.y
 end
 function vector:norm()
  return self/sqrt(#self)
 end
 function vector:len()
  return sqrt(#self)
 end
 function vector:__len()
  return self.x^2+self.y^2
 end
 function vector:str()
  return self.x..","..self.y
 end

function v(x,y)
 return setmetatable({
  x=x,y=y
 },vector)
end

-------------------------------
-- collision boxes
-------------------------------

cbox=object:extend()

 function cbox:translate(v)
  return cbox({
   xl=self.xl+v.x,
   yt=self.yt+v.y,
   xr=self.xr+v.x,
   yb=self.yb+v.y
  })
 end

 function cbox:overlaps(b)
  return
   self.xr>b.xl and
   b.xr>self.xl and
   self.yb>b.yt and
   b.yb>self.yt
 end

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
  local ml,mv=32000
  for d,v in pairs(candidates) do
   if allowed[d] and #v<ml then
    ml,mv=#v,v
   end
  end
  return mv
 end
 
 function cbox:str()
  return self.xl..","..self.yt..":"..self.xr..","..self.yb
 end

function box(xl,yt,xr,yb) 
 return cbox({
  xl=min(xl,xr),xr=max(xl,xr),
  yt=min(yt,yb),yb=max(yt,yb)
 })
end

function vbox(v1,v2)
 return box(v1.x,v1.y,v2.x,v2.y)
end

-------------------------------
-- entities
-------------------------------

entity=object:extend({
 state="idle",t=0,
 last_state="idle",
 dynamic=true,
 dtile=2,
 draw_order=3,
 spawns={}
})

 function entity:init()
  if self.sprite then
   self.sprite=deep_copy(self.sprite)
   if not self.render then
    self.render=spr_render
   end
  end
 end
 
 function entity:become(state)
  if state~=self.state then
   self.last_state=self.state
   self.state,self.t=state,0
  end
 end
 
 function entity:is_a(tag)
  if (not self.tags) return false
  for i=1,#self.tags do
   if (self.tags[i]==tag) return true
  end
  return false
 end
 
 function entity:spawns_from(...)
  for tile in all({...}) do
   entity.spawns[tile]=self
  end
 end

static=entity:extend({
 dynamic=false
})

function spr_render(e,ps,x,y)
 local s,p=e.sprite,e.pos
 
 if x then
  p=v(p.x+x,p.y+y)
 end

 function s_get(prop,dflt)
  local st=s[e.state]
  if (st~=nil and st[prop]~=nil) return st[prop]
  if (s[prop]~=nil) return s[prop]
  return dflt
 end

 local sp=p+s_get("offset",v(0,0))

 local w,h=
  s.width or 1,s.height or 1

 local flip_x=false
 local frames=s[e.state] or s.idle
 local delay=frames.delay or 1
 
 if s.turns and type(frames[1])~="number" then
  if e.facing=="up" then
   frames=frames.u
  elseif e.facing=="down" then
   frames=frames.d
  else
   frames=frames.r
  end
  flip_x=(e.facing=="left")
 end
 if s_get("flips") then
  flip_x=e.flipped
 end

 if (type(frames)~="table") frames={frames}
 local frm_index=flr(e.t/delay) % #frames + 1
 local frm=frames[frm_index]
 
 if e.scl then
	 local sx=e.scl.x
	 local sy=e.scl.y
	 sspr(
	  frm%16*8,flr(frm/16)*8,
	  8,8,
	  sp.x+e.org.x*(1-sx),sp.y+e.org.y*(1-sy),
	  8*sx,8*sy)

  return frm_index
 end
 
 local f=e.bold and ospr or spr
 f(e.exr_sprite or frm,
 (sp.x),(sp.y),w,h,flip_x,e.vert)

 return frm_index
end

function ospr(s,x,y,...)
 for i=0,15 do pal(i,0) end
 spr(s,x-1,y,...)
 spr(s,x+1,y,...)
 spr(s,x,y-1,...)
 spr(s,x,y+1,...)
 r_reset()
 spr(s,x,y,...)
end

-------------------------------
-- entity registry
-------------------------------

function entity_reset()
 entities,entities_with,
  entities_tagged={},{},{}
end

function e_add(e)
 add(entities,e)
 for p in all(indexed_properties) do
  if (e[p]) index_add(entities_with,p,e)
 end
 if e.tags then
  for t in all(e.tags) do
   index_add(entities_tagged,t,e)
  end
  c_update_bucket(e)
 end
 return e
end

function e_remove(e)
 del(entities,e)
 for p in all(indexed_properties) do
  if (e[p]) del(entities_with[p],e)
 end
 if e.tags then
  for t in all(e.tags) do
   del(entities_tagged[t],e)
   if e.bkt then
    del(c_bucket(t,e.bkt.x,e.bkt.y),e)
   end
  end
 end
 e.bkt=nil
end

indexed_properties={
 "dynamic",
 "render","render_hud",
 "vel",
 "collides_with",
 "feetbox"
}

-- systems

-------------------------------
-- update system
-------------------------------

function e_update_all()
 for ent in all(entities_with.dynamic) do
  local state=ent.state
  if ent[state] then
   ent[state](ent,ent.t)
  end
  if ent.done then
   e_remove(ent)
  elseif state~=ent.state then
   ent.t=0
  else
   ent.t+=1
  end  
 end
end

function schedule(fn)
 scheduled=fn
end

-------------------------------
-- render system
-------------------------------

function r_render_all(prop)
 local drawables={}
 for ent in all(entities_with[prop]) do
  local order=ent.draw_order or 0
  if not drawables[order] then
   drawables[order]={}
  end
  add(drawables[order],ent)  
 end
 for o=0,15 do 
  if o==1 and prop=="render" then
   local a=entities_tagged["part"]
   if a then
    for e in all(a) do
     e:render_top()
    end
   end
  end
   
  for ent in all(drawables[o]) do
   r_reset(prop)
   if not ent.done then ent[prop](ent,ent.pos) end
  end
 end
end

function r_reset(prop)
 pal()
 palt(0,false)
 palt(14,true)
end

-------------------------------
-- movement system
-------------------------------

function do_movement()
 for ent in all(entities_with.vel) do
  local ev=ent.vel
  ent.pos+=ev
  if ev.x~=0 then
   ent.flipped=ev.x<0
  end
  if ev.x~=0 and abs(ev.x)>abs(ev.y) then
  
   ent.facing=
    ev.x>0 and "right" or "left"
  elseif ev.y~=0 then
   ent.facing=
    ev.y>0 and "down" or "up"
  end
  if (ent.weight) then
   local w=state_dependent(ent,"weight")
   ent.vel+=v(g_grav_x*w,g_grav_y*w)
  end
 end
end
g_grav_x=0
g_grav_y=1

-------------------------------
-- collision
-------------------------------

function c_bkt_coords(e)
 local p=e.pos
 return flr(shr(p.x,4)),flr(shr(p.y,4))
end

function c_bucket(t,x,y)
 local key=t..":"..x..","..y
 if not c_buckets[key] then
  c_buckets[key]={}
 end
 return c_buckets[key]
end

function c_update_buckets()
 for e in all(entities_with.dynamic) do
  c_update_bucket(e)
 end
end

function c_update_bucket(e)
 if (not e.pos or not e.tags) return 
 local bx,by=c_bkt_coords(e)
 if not e.bkt or e.bkt.x~=bx or e.bkt.y~=by then
  if e.bkt then
   for t in all(e.tags) do
    local old=c_bucket(t,e.bkt.x,e.bkt.y)
    del(old,e)
   end
  end
  e.bkt=v(bx,by)  
  for t in all(e.tags) do
   add(c_bucket(t,bx,by),e) 
  end
 end
end

function c_potentials(e,tag)
 local cx,cy=c_bkt_coords(e)
 local bx,by=cx-2,cy-1
 local bkt,nbkt,bi={},0,1
 return function()
  while bi>nbkt do
   bx+=1
   if (bx>cx+1) bx,by=cx-1,by+1
   if (by>cy+1) return nil
   bkt=c_bucket(tag,bx,by)
   nbkt,bi=#bkt,1
  end
  local e=bkt[bi]
  bi+=1
  return e
 end 
end

function collision_reset()
 c_buckets={}
end

function do_collisions()
 c_update_buckets()
 for e in all(entities_with.collides_with) do
  for tag in all(e.collides_with) do
   if entities_tagged[tag] then
   local nothers=
    #entities_tagged[tag]  
   if nothers>4 then
    for o in c_potentials(e,tag) do
     if o~=e and not e.nocol and not o.nocol  then
      local ec,oc=
       c_collider(e),c_collider(o)
      if ec and oc then
       c_one_collision(ec,oc,e,o)
      end
     end
    end
   else
    for oi=1,nothers do
     local o=entities_tagged[tag][oi]
     local dx,dy=
      abs(e.pos.x-o.pos.x),
      abs(e.pos.y-o.pos.y)
     if dx<=20 and dy<=20 then
      local ec,oc=
       c_collider(e),c_collider(o)
      if ec and oc then
       c_one_collision(ec,oc,e,o)
      end
     end
    end
   end     
   end
  end 
 end
end

function c_check(box,tags,sm)
 local fake_e={pos=v(box.xl,box.yt)} 
 for tag in all(tags) do
  for o in c_potentials(fake_e,tag) do
   if o~=sm then
	   local oc=c_collider(o)
	   if oc and not o.nocol and box:overlaps(oc.b) then
	    return oc.e
	   end
   end
  end
 end
 return nil
end

function c_one_collision(ec,oc,e,o)
 if ec.b:overlaps(oc.b) then
  c_reaction(ec,oc,e,o)
  c_reaction(oc,ec,e,o)
 end
end

function c_reaction(ec,oc,e,o)
 local reaction,param=
  event(ec.e,"collide",oc.e)
  
 if type(reaction)=="function" then
  reaction(ec,oc,param,e,o)
 end
end

function c_collider(ent)
 if ent.collider then 
  if ent.coll_ts==g_time or not ent.dynamic then
   return ent.collider
  end
 end
 local hb=state_dependent(ent,"hitbox")
 if (not hb) return nil
 local coll={
  b=hb:translate(ent.pos),
  e=ent
 }
 ent.collider,ent.coll_ts=
  coll,g_time
 return coll
end

function c_push_out(oc,ec,
 allowed_dirs,e,o)
 local sepv=ec.b:sepv(oc.b,allowed_dirs)
 if (sepv==nil) return
 -- cls() print(ec.b) print(oc.b)
 ec.e.pos+=sepv
 
 if ec.e.vel then
  local vdot=ec.e.vel:dot(sepv)
  if vdot<0 then
   if (sepv.y~=0) ec.e.vel.y=0
   if (sepv.x~=0) ec.e.vel.x=0
  end
 end
 ec.b=ec.b:translate(sepv)
end

function c_move_out(oc,ec,allowed)
 return c_push_out(ec,oc,allowed)
end

-------------------------------
-- support
-------------------------------


function do_supports()
 local vl=v(g_grav_x,g_grav_y)
 for e in all(entities_with.feetbox) do  
  local fb=e.feetbox
  if fb then
   fb=fb:translate(e.pos+vl)
   local support=c_check(fb,{"support"},e)
-- ) support=nil
   e.supported_by=support
   if support and support.vel then
    --e.pos+=support.vel
   end
  end
 end
end
local a="0000000000000000000000000000000000000000000000000000000000000000000000000000000000055555555550000000000000000000005555555555550000000000000000000555155555555500000000000000000005551555555555000000000000000000055555555555550000000000000000000555555555555500000000000000000005555555555555000000008888888800055555550000000000000008888888880555555500000000000000008880008888555555555500000000000080000000558888000000000000005000000000005555558000000000000050000000005555555500000000000000550000005555555555555000000000005550000555555555550050000000000055550055555555555500500000000000555555555555555555000000000000005555555555555555550000000000000005555555555555555500000000000000055555555555555550000000000000000055555555555555500000000000000000055555555555550000000000000000000055555555555000000000000000000000055555555500000000000000000000000055550555000000000000000000000000555000550000000000000000000000005500000500000000000000000000000050000005000000000000000000000000555000055500000000000000000000000000000000000000000000"

function b(y,xx,yy) 
 for c=0,31 do
  for d=1,31 do 
   local e=d+c*32 
   local m=tonum(sub(a,e,e))
   if (m~=0) pset(d+48+xx,c-48+y+yy,m)
  end 
 end
end 

function oprint(s,x,y,...)
 s=smallcaps(s)
 prnt(s,x,y,...)
end

function coprint(s,y,c,m,n)
 s=smallcaps(s)
 prnt(s,64-#s*2+(m or 0),y,c,nil,n)
end

function prnt(s,x,y,c,o,n)
 if(not o) o=13--o=sget(97,c)
 
 for xx=x-1,x+1 do
  for yy=y-1,y+2 do
   print(s,xx,yy,n or 0)
  end
 end
 
 print(s,x,y+1,o)
 print(s,x,y,c)
end
local gt=0
function f(e)
 cls()
 gt=min(0.5,gt+0.03) 
 local y=96+cos(gt)*5
 for xx=-1,1 do
  for yy=-1,1 do
   if abs(xx)+abs(yy)==1 then
    coprint("^rexellent ^games",
     y+yy,0,xx,7)
   end
  end
 end

 coprint("^rexellent ^games",
  y,7)

 for i=0,15 do pal(i,l[e][6]) end
 for xx=-1,1 do
 for yy=-1,1 do
  if(abs(xx)+abs(yy)==1)b(y,xx,yy)
 end
 end
 pl(e)
 b(y,0,0)
 flip()
end 
 
function g(h,i,j) 
 for e=h,i,j do 
  for m=1,5 do
   pl(e)
   f(e) 
  end
 end 
end 

function pl(e)
 local k=l[e]
 pal()
 pal(8,k[1]) 
 pal(1,k[2]) 
 pal(5,k[3]) 
 pal(7,k[4])
 pal(13,k[5]) 
end

l={
 {0,0,0,0,0,1},
 {2,0,1,1,1,5},
 {4,0,1,5,13,5},
 {4,1,5,6,13,6},
 {8,1,5,7,13,7}
} 
 
function m() 
 g(1,#l,1) 
 g(#l,1,-1) 
 pal() 
 fade()
end

function smallcaps(s)
 s=s..""
  local d=""
  local c
  for i=1,#s do
    local a=sub(s,i,i)
    if a!="^" then
      if not c then
        for j=1,26 do
          if a==sub("abcdefghijklmnopqrstuvwxyz",j,j) then
            a=sub("\65\66\67\68\69\70\71\72\73\74\75\76\77\78\79\80\81\82\83\84\85\86\87\88\89\90\91\92",j,j)
          end
        end
      end
      d=d..a
      c=true
    end
    c=not c
  end
  return d
end

function noprint(s,x,y,c)
 s=smallcaps(s)
 prnt(s,x+4-#s*2,y,c)
end

pltt={7,6,5,1,0}
function fade()
 shk=3
 for i=1,#pltt do
  cls(pltt[i])
  flip()-- flip()
 end
end
-->8
-- level

function block_type(blk)
 if (fget(blk,0)) return solid
 if (fget(blk,1)) return support
end

level=entity:extend({
 draw_order=2
})

function level:init()
 local b,s=self.base,self.size
 for x=0,s.x-1 do
  for y=0,s.y-1 do
   local blk=mget(b.x+x,b.y+y)
   local cl=entity.spawns[blk]
   if cl then
    local e=cl({
     pos=v(x,y)*8,
     map_pos=b+v(x,y),
     tile=blk
    })
    local xx,yy=b.x+x,b.y+y
    if(not e.norem)mset(xx,yy,e.trr or 0)
    e_add(e)
    blk=0
   end
   local bt=block_type(blk)
   if bt then
    bl=bt({
     pos=v(x,y)*8,
     map_pos=b+v(x,y),
     tile=blk,
     typ=bt
    })
    if (bl.needed) e_add(bl)
   end
  end
 end
end

function level:render()
 for i=0,15 do pal(i,0) end
 
 for xx=-1,1 do
  for yy=-1,1 do
   if(abs(xx)+abs(yy)==1)map(self.base.x,self.base.y,xx,yy)
  end
 end
 
 r_reset()
 map(self.base.x,self.base.y)
end
 
solid=static:extend({
 tags={"walls","support"},
 hitbox=box(0,0,8,8),
 draw_order=2
})
local dirs={v(-1,0),v(1,0),v(0,-1),v(0,1)}

function solid:init()
 local allowed={}
 local needed=false
 for i=1,4 do
  local np=self.map_pos+dirs[i]
  allowed[i]=
   np.x>0 and np.y>0 and
   block_type(mget(np.x,np.y))
    ~=solid
  needed=needed or allowed[i]
 end
 
 self.allowed=allowed
 self.needed=needed
end

function solid:collide(e)
 return c_push_out,self.allowed
end

support=solid:extend({
 hitbox=box(0,0,8,1)
})
 
function support:collide(e)
 if (not e.vel) return
 local dy,vy=e.pos.y-self.pos.y,e.vel.y
 if vy>0 and dy<=vy+1 then
  return c_push_out,{false,false,true,false}   
 end
end
-->8
-- entities

door=entity:extend({
 sprite={idle={16}},
 hitbox=box(0,0,8,8),
 collides_with={"guy","box"}
})

function door:collide(o)
 if(g_won) return
 if(o:is_a("box")) return c_push_out
 sfx(22)
 
 if g_cgot then
  g_cgot=false
  g_coin+=1
  g_coins[g_index]=true
 end
 
 if g_index==15 then
  g_won=true
  fade()
  return
 end
 
 g_index+=1
 restart_level()
 fade()
end

door:spawns_from(16)

bx=entity:extend({
 sprite={idle={32}},
 hitbox=box(0,0,8,8),
 feetbox=box(0,0,8,8),
 weight=0.2,
 vel=v(0,0),
 collides_with={"walls","guy","box"},
 tags={"solid","support","box"}
})

bx:spawns_from(32)

function bx:init()
 self.scl=v(1,1)
 self.org=v(4,8)
end

function bx:idle()
 if not self.was and self.supported_by then
  if g_grav_y~=0 then
	  self.scl.y=0.3
	  self.scl.x=1.5
	  self.org=v(4,g_grav_y>0 and 8 or 0)
  else
	  self.scl.x=0.3
	  self.scl.y=1.5
	  self.org=v(g_grav_x>0 and 8 or 0,4)
  end
  
  sfx(21)
  shk=2
  tween(self.scl,{x=1,y=1},0.2)
 end
 
 self.vel.x=mid(-4,4,self.vel.x)
 self.vel.y=mid(-4,4,self.vel.y)
 self.was=self.supported_by

 if self.pos.x>124 then
  self.pos.x=-4
 elseif self.pos.x<-4 then
  self.pos.x=124
 end
 
 if self.pos.y>124 then
  self.pos.y=-4
 elseif self.pos.y<-4 then
  self.pos.y=124
 end
end

function bx:collide(o)
 if(o==self) return
 if (o:is_a("box")) return o.tile==self.tile and c_push_out or c_move_out
 if o:is_a("guy") then
  return 
   o.supported_by 
   and o.supported_by~=self 
    
    and c_move_out or c_push_out
 end
end

but=entity:extend({
 sprite={idle={48}},
 draw_order=2,
 hitbox=box(2,2,6,6),
 collides_with={"box","guy"}
})

but:spawns_from(48)

function but:init()
 self.on=false
 self.won=false
end

function but:collide(o)
 self.on=true
end

function but:idle()
 if self.won~=self.on then
  local t=entities_tagged["piston"]
  
  for p in all(t) do
   p:become(self.on and 
    (p.don and "idle" or "on") or 
    (p.don and "on" or "idle"))
  end
  sfx(self.on and 25 or 26)
 end

 self.won=self.on
 self.on=false
end

piston=entity:extend({
 sprite={
  idle={49},
  on={33}
 },
 hitbox=box(0,0,8,8),
 collides_with={"guy","box"},
 tags={"piston","support"}
})

piston:spawns_from(49,33)

function piston:init()
 if(self.tile==33) self.don=true self:become("on")
end

function piston:collide()
 if(self.state=="on") return c_push_out
end

function piston:on() end

lbx=bx:extend({
sprite={idle={34}}
})lbx:spawns_from(34)
function lbx:idle()lbx.extends.idle(self)
self.vel.y=0
self.vel.x=max(0,g_grav_x==0 and 0 or self.vel.x)
end

rbx=bx:extend({
sprite={idle={36}}
})rbx:spawns_from(36)
function rbx:idle()rbx.extends.idle(self)
self.vel.y=0
self.vel.x=min(0,g_grav_x==0 and 0 or self.vel.x)
end

bbx=bx:extend({
sprite={idle={19}}
})bbx:spawns_from(19)
function bbx:idle()bbx.extends.idle(self)
self.vel.x=0
self.vel.y=max(0,g_grav_y==0 and 0 or self.vel.y)
end

ubx=bx:extend({
sprite={idle={51}}
})ubx:spawns_from(51)
function ubx:idle()ubx.extends.idle(self)
self.vel.x=0
self.vel.y=min(0,g_grav_y==0 and 0 or self.vel.y)
end


hbx=bx:extend({
sprite={idle={37}}
})hbx:spawns_from(37)
function hbx:idle()hbx.extends.idle(self)
self.vel.y=0
if(g_grav_x==0) self.vel.x=0
end

vbx=bx:extend({
sprite={idle={38}}
})vbx:spawns_from(38)
function vbx:idle()vbx.extends.idle(self)
self.vel.x=0
if(g_grav_y==0) self.vel.y=0
end

abut=but:extend({
 sprite={idle={18}},
 tags={"abut"}
})

abut:spawns_from(18)

function abut:init()
 self.won=false
 self.con=false
end

function abut:idle()
 if self.con~=self.won then
  sfx(self.con and 25 or 26)
 end
 self.won=self.con
 self.con=self.on
end

coin=entity:extend({
sprite={idle={
6,6,7,8,9,10,10,9,8,7,
delay=10
}},
hitbox=box(0,0,8,8),
collides_with={"guy"}
})

coin:spawns_from(6)
g_coins={}
g_coin=0

function coin:init()
 if(g_coins[g_index]) self.done=true
end

function coin:collide()
 self.done=true
 sfx(24)
 g_cgot=true
end
-->8
--guy

guy=entity:extend({
 sprite={
  idle={
   64,80,96,80,delay=5
  },right={
   65,81,97,81,delay=5
  },left={
   66,82,98,82,delay=5
  },up={
   67,83,99,83,delay=5
  },
 },
 trr=50,
 vel=v(0,0),
 weight=0.1,
 hitbox=box(0,0,8,8),
 feetbox=box(0,0,8,8),
 collides_with={"walls","solid","support"},
 scl=v(1,1),
 org=v(4,8),
 tags={"guy"}
})

guy:spawns_from(64)

function guy:init()
 g_guy=self
 self.was=true
end

function guy:idle()
 self:bound()
 g_grav_x=0
 g_grav_y=1
 self.vel.y=min(4,self.vel.y)
end

function guy:right()
 self:bound()
 g_grav_x=1
 g_grav_y=0
 self.vel.x=min(4,self.vel.x)
end

function guy:up()
 self:bound()
 g_grav_x=0
 g_grav_y=-1
 self.vel.y=max(-4,self.vel.y)
end

function guy:left()
 self:bound()
 g_grav_x=-1
 g_grav_y=0
 self.vel.x=max(-4,self.vel.x)
end

function guy:bound()
 if(g_won) return
 if self.supported_by then
  if(btnp(⬅️) and self.state~="left") self:become("left") g_moves+=1 sfx(20)
  if(btnp(➡️) and self.state~="right") self:become("right") g_moves+=1 sfx(20)
  if(btnp(⬇️) and self.state~="idle") self:become("idle") g_moves+=1 sfx(20)
  if(btnp(⬆️) and self.state~="up") self:become("up") g_moves+=1 sfx(20)
 end
 
 if not self.was and self.supported_by then
  if g_grav_y~=0 then
	  self.scl.y=0
	  self.scl.x=2
	  self.org=v(4,g_grav_y>0 and 8 or 0)
  else
	  self.scl.x=0
	  self.scl.y=2
	  self.org=v(g_grav_x>0 and 8 or 0,4)
  end
  
  shk=2
  sfx(21)
  tween(self.scl,{x=1,y=1},0.2)
 end
 
 self.was=self.supported_by

 if self.pos.x>124 then
  self.pos.x=-4
 elseif self.pos.x<-4 then
  self.pos.x=124
 end
 
 if self.pos.y>124 then
  self.pos.y=-4
 elseif self.pos.y<-4 then
  self.pos.y=124
 end
end

guy.move=guy.idle
-->8
-- fx

spark=entity:extend({
 draw_order=10,
 sprite={idle={delay=4,13,14,15,29,30,31,45}}
})

function spark:idle()
 if(self.t>=28) self.done=true return
end

part=entity:extend({
 draw_order=0,
 tags={"part"}
})

function part:idle()
 self.r-=(self.spd~=nil and self.spd(self.t) or 0.1)
 self.vel*=(self.mul or 0.9)
 if self.r<0 then
  self.done=true
 end
end

function part:render()
 circfill(self.pos.x,self.pos.y,self.r,0)
end

function part:render_top()
 circfill(self.pos.x,self.pos.y,self.r-1,self.c)
end

function parts(x,y,a,r,c)
 for i=1,a do
  e_add(part({
   pos=v(x,y),
   vel=v(rnd(2)-1,rnd()),
   r=r,
   c=rnd()>0.7 and c or sget(97,c)
  }))
 end
end

-->8
-- tween engine
-- by @egordorichev

local back=1.70158

-- feel free to remove unused functions to save up some space
functions={
["linear"]=function(t) return t end,
["quad_out"]=function(t) return -t*(t-2) end,
["quad_in"]=function(t) return t*t end,
["quad_in_out"]=function(t) t=t*2 if(t<1) return 0.5*t*t
    return -0.5*((t-1)*(t-3)-1) end,
["back_in"]=function(t) local s=back  return t*t*((s+1)*t-s) end,
["back_out"]=function(t) local s=back t-=1 return t*t*((s+1)*t+s)+1 end,
["back_in_out"]=function(t) local s=back t*=2 if (t<1) s*=1.525 return 0.5*(t*t*((s+1)*t-s))
 t-=2 s*=1.525  return 0.5*(t*t*((s+1)*t+s)+2) end
}

local tasks={}

function tween(o,vl,t,fn)
 local task={
  vl={},
  rate=1/(t or 1),
  o=o,
  progress=0,
  delay=0,
  fn=functions[fn or "quad_out"]
 }

 for k,v in pairs(vl) do
  local x=o[k]
  task.vl[k]={start=x,diff=v-x}
 end

 add(tasks,task)
 return task
end

function tween_update(dt)
 for t in all(tasks) do
  if t.delay>0 then
   t.delay-=dt
  else
   t.progress+=dt*t.rate
   local p=t.progress
   local x=t.fn(p>=1 and 1 or p)
   for k,v in pairs(t.vl) do
    t.o[k]=v.start+v.diff*x
   end

   if p>=1 then
    del(tasks,t)
    if (t.onend) t.onend()
   end 
  end
 end
end
-->8
-- states

ingame={}

function ingame.update()
 e_update_all()
 
 local ab=entities_tagged["abut"]
   
 if ab then
  local found=false
  for pr in all(ab) do
   if not pr.on then
    found=true
   end
   pr.on=false
  end
 
  local t=entities_tagged["piston"]
  
  if not found then
	  for p in all(t) do
	   p:become((p.don and "idle" or "on") )
	  end   
  end
 end

 do_movement()
 do_collisions()
 do_supports()
 if(btnp(❎)) sfx(23)restart_level()
end

function ingame.draw()
 cls(12)
 camera()
 rectfill(0,96,127,109,1)
 rectfill(0,110,127,127,0)

 for i=0,15 do
  spr(75,i*8,--+g_guy.pos.x/16%8,
   64---g_guy.pos.y/16%32
   ,1,4)
 end
 for i=0,15,3 do
  spr(76,i*8,--+g_guy.pos.x/8%24,
   64+24---g_guy.pos.y/8%8
   ,3,1)
 end
 
 for i=0,15 do
  spr(90,i*8,64+46)
 end
 
 if shk>0.1 then
  shk-=0.5
  camera(rnd(shk)-shk/2,rnd(shk)-shk/2)
 else
  camera()
 end
 
 
 if g_won then
	 coprint("^you won!",32,7)
	 coprint("^thanks for playing!",48,7)
	 coprint("^more games at",58,7)
	 coprint("egordorichev.itch.io",70,6)
	 
	 coprint(g_moves.." moves",100,7)
	 coprint(g_coin.." coins",108,7)
 else
  r_render_all("render")
 end
 
 if g_index==0 then
  coprint("^ma ^puzzle",20,7)
  coprint("⬅️➡️⬇️⬆️    ",100,7)
  coprint("❎ to restart ",110,7)
 end
 
 if(btn(🅾️))oprint(stat(1),2,2,7,6)
end

menu={}

function menu.update()

end

function menu.draw()

end

__gfx__
00000000d66666673b3bbbb73bb3bbbae1e1e1eeee1e1e1eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee0000000000000000000000000000000000000000
000000005dddddd633333b3333333b33e3eee3eeee3e3eeeee0000eeeee00eeeeee0eeeeeee00eeeee0000ee0000000000000000000000000000000000000000
007007005dd555d63d3553d35dd33336e3eeeeeeeeee3eeee0999a0eee09a0eeee090eeeee0a90eee0a9990e0000000000000000000000000000000000000000
000770005d6dd5d63d6dd5d65d6335d6e3eeeeeeeeee3eeee09a790eee0990eeee090eeeee0990eee097a90e0000000000000000000000000000000000000000
000770005d6dd5d65d6dd5d65d6dd5d6eeeeeeeeeeee3eeee09aa90eee0990eeee090eeeee0990eee09aa90e0000000000000000000000000000000000000000
007007005d666dd65d666dd65d666dd6eeeeeeeeeeeeeeeee049990eee0490eeee040eeeee0940eee099940e0000000000000000000000000000000000000000
000000005dddddd65dddddd65dddddd6eeeeeeeeeeeeeeeeee0000eeeee00eeeeee0eeeeeee00eeeee0000ee0000000000000000000000000000000000000000
000000001555555d1555555d1555555deeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee0000000000000000000000000000000000000000
ee0000ee5dddddd6eeeeeeee10000001eeeeeeeeeeeeeeee00000000000000000000000000000000000000000000000000000000000000000000000000000000
e09a9a0e1555555deeeeeeee02499420eeeeeeeeeeeeeeee00000000000000000000000000000000000000000000000000000000000000000000000000000000
0a4949a01551115dee1001ee04499440eeeeeeeeeeeeeeee00000000000000000000000000000000000000000000000000000000000000000000000000000000
0949494015d5515dee0b70ee04499440eeeeeeeeeeeebeee00000000000000000000000000000000000000000000000000000000000000000000000000000000
0949494015d5515dee03b0ee09499490ebeeeeeeeeee3ebe00000000000000000000000000000000000000000000000000000000000000000000000000000000
0909494015ddd55dee1001ee04999940e3eeeebeeebe3e3e00000000000000000000000000000000000000000000000000000000000000000000000000000000
092949401555555deeeeeeee02499420e3ebee3eee3e3e3e00000000000000000000000000000000000000000000000000000000000000000000000000000000
0949494001111115eeeeeeee10000001e1e1ee1eee1e1e1e00000000000000000000000000000000000000000000000000000000000000000000000000000000
10000001100000011000000100000000100000011000000110044001000000000000000000000000000000000000000000000000000000000000000000000000
049999a009aa9a700244942001111110024944200294492002999920000000000000000000000000000000000000000000000000000000000000000000000000
02444490049a49a00444494001000010049444400944449009499490000000000000000000000000000000000000000000000000000000000000000000000000
02442490024924900999999001000010099999904999999404499440000000000000000000000000000000000000000000000000000000000000000000000000
0249449009aa9aa00999999001000010099999904999999404499440000000000000000000000000000000000000000000000000000000000000000000000000
02444490049a49a00444494001000010049444400944449009499490000000000000000000000000000000000000000000000000000000000000000000000000
01222240024924900244942001111110024944200294492002999920000000000000000000000000000000000000000000000000000000000000000000000000
10000001100000011000000100000000100000011000000110044001000000000000000000000000000000000000000000000000000000000000000000000000
eeeeeeeeeeeeeeeeeeeeeeee10000001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
eeeeeeeeeae99eaeee2222ee024994200dddddd00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
ee1001eeeeeeeeeee222222e049999400d0000d00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
ee0870eee9eeee9ee222222e094994900d0000d00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
ee0280eee9eeee9ee222222e044994400d0000d00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
ee1001eeeeeeeeeee222222e044994400d0000d00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
eeeeeeeeeae99eaee222222e024994200dddddd00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
eeeeeeeeeeeeeeeee111111e10000001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
eee00eeeeeee0f0ee0f0eeeee020020e000000000000000000000000000000000000000000000000111111116c6c6c6c7777777773bbbb377777777700000000
ee0f70eeeee0100000010eee0010050000000000000000000000000000000000000000000000000011111111c6c6c6c6777777733bbbbbb33777777700000000
ee09f0eee006dd5225dd600ef0555d0f000000000000000000000000000000000000000000000000111111116c6c6c6c73bb373bbb5335bbb373bb3700000000
e0d1160e07f1d500005d1f70015ddd1000000000000000000000000000000000000000000000000011111111c6c6c6c63bbbb3bbb533335bbbbbbbbb00000000
015ddd100f91d500005d19f0e0d1160e0000000000000000000000000000000000000000000000001111111166666666bbbbbbbb53333335bbb5335b00000000
f0555d0fe00d55122155d00eee09f0ee0000000000000000000000000000000000000000000000001111111166666666b5335bb5333333333333333300000000
00100500eee0100000010eeeee0f70ee000000000000000000000000000000000000000000000000111111116666666633333333333113333333333300000000
e020020eeeee0f0ee0f0eeeeeee00eee000000000000000000000000000000000000000000000000111111116666666633113333331111333333113300000000
eee00eeeeee0ff0ee0ff0eeee020020e00000000000000000000000000000000000000000000000001010101f6f6f6f600000000000000000000000000000000
ee0f70eeeee0100000010eee00100500000000000000000000000000000000000000000000000000101010106f6f6f6f00000000000000000000000000000000
ee09f0eee0006d5225d6000ef0555d0f00000000000000000000000000000000000000000000000001010101f6f6f6f600000000000000000000000000000000
0009f00007ff15000051ff70f1d1161f000000000000000000000000000000000000000000000000101010106f6f6f6f00000000000000000000000000000000
f1d1161f0f991500005199f00009f00000000000000000000000000000000000000000000000000000000000ffffffff00000000000000000000000000000000
f0555d0fe000d512215d000eee09f0ee00000000000000000000000000000000000000000000000000000000ffffffff00000000000000000000000000000000
00100500eee0100000010eeeee0f70ee00000000000000000000000000000000000000000000000000000000ffffffff00000000000000000000000000000000
e020020eeee0ff0ee0ff0eeeeee00eee00000000000000000000000000000000000000000000000000000000ffffffff00000000000000000000000000000000
eeeeeeeeeee0f0eeee0f0eeee020020e000000000000000000000000000000000000000000000000000000007f7f7f7f00000000000000000000000000000000
eee00eeeeee0100000010eeee010050e00000000000000000000000000000000000000000000000000000000f7f7f7f700000000000000000000000000000000
ee0f70eeee006d5225d600ee00555d00000000000000000000000000000000000000000000000000000000007f7f7f7f00000000000000000000000000000000
0009f000e07f15000051f70ef1d1161f00000000000000000000000000000000000000000000000000000000f7f7f7f700000000000000000000000000000000
f1d1161fe0f9150000519f0e0009f000000000000000000000000000000000000000000000000000000000007777777700000000000000000000000000000000
00555d00ee00d512215d00eeee0f70ee000000000000000000000000000000000000000000000000000000007777777700000000000000000000000000000000
e010050eeee0100000010eeeeee00eee000000000000000000000000000000000000000000000000000000007777777700000000000000000000000000000000
e020020eeee0f0eeee0f0eeeeeeeeeee000000000000000000000000000000000000000000000000000000007777777700000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000007777777700000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000007777777700000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000007777777700000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000007777777700000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000007777777700000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000007777777700000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000007777777700000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000007777777700000000000000000000000000000000
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
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000101010100000000000101000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000100000000000000000001000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000100100100031210000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000101010100000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000002100000000000042000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000220000000000210000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010121010000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000330010000010000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000021000010000410000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010101010000000
__label__
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
ccccccccccccccccccccccccccccccccccccccccccccc00000ccccccc00000cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
ccccccccccccccccccccccccccccccccccccccccccccc077700000ccc0777000000000000000c00000cccccccccccccccccccccccccccccccccccccccccccccc
ccccccccccccccccccccccccccccccccccccccccccccc077707770ccc07d7070707770777070c07770cccccccccccccccccccccccccccccccccccccccccccccc
ccccccccccccccccccccccccccccccccccccccccccccc07d707d70ccc077707070dd70dd7070c077d0cccccccccccccccccccccccccccccccccccccccccccccc
ccccccccccccccccccccccccccccccccccccccccccccc070707770ccc07dd0707070d070d070007d00cccccccccccccccccccccccccccccccccccccccccccccc
ccccccccccccccccccccccccccccccccccccccccccccc070707d70ccc07000d7707770777077707770cccccccccccccccccccccccccccccccccccccccccccccc
ccccccccccccccccccccccccccccccccccccccccccccc0d0d0d0d0ccc0d0c00dd0ddd0ddd0ddd0ddd0cccccccccccccccccccccccccccccccccccccccccccccc
ccccccccccccccccccccccccccccccccccccccccccccc000000000ccc000cc00000000000000000000cccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
ccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc0cccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc0b0ccc0ccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc0300c0b0cccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc030b0030cccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc0101001000000000cccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccc0000cccccccccccccccccccccccccccccccccccccccccccc0000c03bb3bbba3b3bbbb70ccccccccccccccc
ccccccccccccccccccccccccccccccccccccccccc00f700cccccccccccccccccccccccccccccccccccccccccc09a9a0033333b3333333b330ccccccccccccccc
cccccccccccccccccccccccccccccccccccc0ccc0209f020cccccccccccccccccccccccccccccccccccccccc0a4949a05dd333363d3553d30ccccccccccccccc
ccccccccccccccccccccccccccccccccccc0b00c00d11600cccccccccccccccccccccccccccccccccccccccc094949405d6335d63d6dd5d60ccccccccccccccc
cccccccccccccccccccccccccccccccccc0030b0015ddd10cccccccccccccccccccccccccccccccccccccccc094949405d6dd5d65d6dd5d60ccccccccccccccc
ccccccccccccccccccccccccccccccccc0b03030f0555d0fcccccccccccccccccccccccccccccccccccccccc090949405d666dd65d666dd60ccccccccccccccc
ccccccccccccccccccccccccccccccccc030303000100500cccccccccccccccccccccccccccccccccccccccc092949405dddddd65dddddd60ccccccccccccccc
cccccccccccccccccccccccccccccccc001010100020020000000000cccccccccccccccccccccccc00000000094949401555555d1555555d0ccccccccccccccc
ccccccccccccccccccccccccccccccc03bb3bbba3b3bbbb73b3bbbb70cccccccccccccccccccccc03bb3bbba3b3bbbb75dddddd6d66666670ccccccccccccccc
ccccccccccccccccccccccccccccccc033333b3333333b3333333b330cccccccccccccccccccccc033333b3333333b331555555d5dddddd60ccccccccccccccc
ccccccccccccccccccccccccccccccc05dd333363d3553d33d3553d30cccccccccccccccccccccc05dd333363d3553d31551115d5dd555d60ccccccccccccccc
ccccccccccccccccccccccccccccccc05d6335d63d6dd5d63d6dd5d60cccccccccccccccccccccc05d6335d63d6dd5d615d5515d5d6dd5d60ccccccccccccccc
ccccccccccccccccccccccccccccccc05d6dd5d65d6dd5d65d6dd5d60cccccccccccccccccccccc05d6dd5d65d6dd5d615d5515d5d6dd5d60ccccccccccccccc
ccccccccccccccccccccccccccccccc05d666dd65d666dd65d666dd60cccccccccccccccccccccc05d666dd65d666dd615ddd55d5d666dd60ccccccccccccccc
ccccccccccccccccccccccccccccccc05dddddd65dddddd65dddddd60cccccccccccccccccccccc05dddddd65dddddd61555555d5dddddd60ccccccccccccccc
ccccccccccccccccccccccccccccccc01555555d1555555d1555555d0cccccccccccccccccccccc01555555d1555555d011111151555555d0ccccccccccccccc
6c6c6c6c6c6c6c6c6c6c6c6c6c6c6c60d66666675dddddd6d66666670c6c6c6c6c6c6c6c6c6c6c60d66666675dddddd65dddddd6d66666670c6c6c6c6c6c6c6c
c6c6c6c6c6c6c6c6c6c6c6c6c6c6c6c05dddddd61555555d5dddddd606c6c6c6c6c6c6c6c6c6c6c05dddddd61555555d1555555d5dddddd606c6c6c6c6c6c6c6
6c6c6c6c6c6c6c6c6c6c6c6c6c6c6c605dd555d61551115d5dd555d60c6c6c6c6c6c6c6c6c6c6c605dd555d61551115d1551115d5dd555d60c6c6c6c6c6c6c6c
c6c6c6c6c6c6c6c6c6c6c6c6c6c6c6c05d6dd5d615d5515d5d6dd5d600c6c6c6c6c6c6c6c6c6c6c05d6dd5d615d5515d15d5515d5d6dd5d606c6c6c6c6c6c6c6
666666666666666666666666666666605d6dd5d615d5515d5d6dd5d60b06660666666666666666605d6dd5d615d5515d15d5515d5d6dd5d60666666666666666
666666666666666666666666666666605d666dd615ddd55d5d666dd6030060b066666666666666605d666dd615ddd55d15ddd55d5d666dd60666666666666666
666666666666666666666666666666605dddddd61555555d5dddddd6030b003066666666666666605dddddd61555555d1555555d5dddddd60666666666666666
666666666666666666666666666666601555555d011111151555555d0101001066666666666666601555555d01111115011111151555555d0666666666666666
f6f6f6f6f6f6f6f6f6f6f6f6f6f6f6f0d66666675dddddd65dddddd63bb3bbba06f6f6f6f6f6f6f0d66666675dddddd65dddddd6d666666706f6f6f6f6f6f6f6
6f6f6f6f6f6f6f6f6f6f6f6f6f6f6f605dddddd61555555d1555555d33333b330f6f6f6f6f6f6f605dddddd61555555d1555555d5dddddd60f6f6f6f6f6f6f6f
f6f6f6f6f6f6f6f6f6f6f6f6f6f6f6f05dd555d61551115d1551115d5dd3333606f6f6f6f6f6f6f05dd555d61551115d1551115d5dd555d606f6f6f6f6f6f6f6
6f6f6f6f6f6f6f6f6f6f6f6f6f6f6f605d6dd5d615d5515d15d5515d5d6335d60f6f6f6f6f6f6f605d6dd5d615d5515d15d5515d5d6dd5d60f6f6f6f6f6f6f6f
fffffffffffffffffffffffffffffff05d6dd5d615d5515d15d5515d5d6dd5d60ffffffffffffff05d6dd5d615d5515d15d5515d5d6dd5d60fffffffffffffff
fffffffffffffffffffffffffffffff05d666dd615ddd55d15ddd55d5d666dd60ffffffffffffff05d666dd615ddd55d15ddd55d5d666dd60fffffffffffffff
fffffffffffffffffffffffffffffff05dddddd61555555d1555555d5dddddd60ffffffffffffff05dddddd61555555d1555555d5dddddd60fffffffffffffff
fffffffffffffffffffffffffffffff01555555d01111115011111151555555d0ffffffffffffff01555555d01111115011111151555555d0fffffffffffffff
7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f00101010d6666667d6666667d66666670f7f7f7f7f7f7f7f01010100d6666667d6666667d66666670f7f7f7f7f7f7f7f
f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f03030005dddddd65dddddd65dddddd607f7f7f7f7f7f7f7030003005dddddd65dddddd65dddddd607f7f7f7f7f7f7f7
7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f0030705dd555d65dd555d65dd555d60f7f7f7f7f7f7f7f030f70705dd555d65dd555d65dd555d60f7f7f7f7f7f7f7f
f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f030f05d6dd5d65d6dd5d65d6dd5d607f7f7f7f7f7f7f70307f7f05d6dd5d65d6dd5d65d6dd5d607f7f7f7f7f7f7f7
77777777777777777777777777777777777030705d6dd5d65d6dd5d65d6dd5d60777777777777777707777705d6dd5d65d6dd5d65d6dd5d60777777777777777
77777777777777777777777777777777777707705d666dd65d666dd65d666dd60777777777777777777777705d666dd65d666dd65d666dd60777777777777777
77777777777777777777777777777777777777705dddddd65dddddd65dddddd60777777777777777777777705dddddd65dddddd65dddddd60777777777777777
77777777777777777777777777777777777777701555555d1555555d1555555d0777777777777777777777701555555d1555555d1555555d0777777777777777
7777777773bbbb37777777777777777773bbbb37000000000000000001010100777777777777777773bbbb30d6666667d6666667010101007777777777777777
777777733bbbbbb337777777777777733bbbbbb337777777777777730300030337777777777777733bb00bb05dddddd65dddddd6030003033777777777777773
73bb373bbb5335bbb373bb3773bb373bbb5335bbb310013773bb373b030330bbb373bb3773bb373bbb0a90b05dd555d65dd555d6030330bbb373bb3773bb373b
3bbbb3bbb533335bbbbbbbbb3bbbb3bbb533335bbb0b70bb3bbbb3bb0303335bbbbbbbbb3bbbb3bbb50990505d6dd5d65d6dd5d60303335bbbbbbbbb3bbbb3bb
bbbbbbbb53333335bbb5335bbbbbbbbb53333335bb03b05bbbbbbbbb50333335bbb5335bbbbbbbbb530990305d6dd5d65d6dd5d600333335bbb5335bbbbbbbbb
b5335bb53333333333333333b5335bb53333333333100133b5335bb53333333333333333b5335bb5330940305d666dd65d666dd60333333333333333b5335bb5
33333333333113333333333333333333333113333333333333333333333113333333333333333333333003305dddddd65dddddd6033113333333333333333333
33113333331111333333113333113333331111333333113333113333331111333333113333113333331111301555555d1555555d031111333333113333113333
1111111111111111111111111111111111111111111111111111111111111111111111111111111111111110d6666667d6666667011111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111105dddddd65dddddd6011111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111105dd555d65dd555d6011111111111111111111111
11111111111111111111111111111111111111111111111100000001000000010000000100000001111111105d6dd5d65d6dd5d6011111111111111111111111
11111111111111111111111111111111111111111111111007777700077777000777770007777700111111105d6dd5d65d6dd5d6011111111111111111111111
111111111111111111111111111111111111111111111110777dd77077dd777077ddd770777d7770111111105d666dd65d666dd6011111111111111111111111
11111111111111111111111111111111111111111111111077d007707700d7707700077077d0d770111111105dddddd65dddddd6011111111111111111111111
11111111111111111111111111111111111111111111111077700770770077707770777077000770111111101555555d1555555d011111111111111111111111
111111111111111111111111111111111111111111111110d77777d0d77777d0d77777d0d77777d0111111110000000000101010111111111111111111111111
1111111111111111111111111111111111111111111111100ddddd000ddddd000ddddd000ddddd00111111111111111110303001111111111111111111111111
11111111111111111111111111111111111111111111111100000001000000010000000100000001111111111111111111003011111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111103011111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111103011111111111111111111111111
11111111111111111111111111111111111111000000011111111111111111111111111111111111111111111111111111110111111111111111111111111111
01010101010101010101010101010101010100077777000100000000000100000000000000000000000000000001010101010101010101010101010101010101
1010101010101010101010101010101010101077d7d7701010777007701010777077700770777077707770777010101010101010101010101010101010101010
01010101010101010101010101010101010100777d77700100d7d07d7001007d7077d07dd0d7d07d707d70d7d001010101010101010101010101010101010101
1010101010101010101010101010101010101077d7d770101007007070101077d07d00d0700700777077d0070010101010101010101010101010101010101010
00000000000000000000000000000000000000d77777d00000070077d000007d70777077d007007d707d70070000000000000000000000000000000000000000
000000000000000000000000000000000000000ddddd0000000d00dd000000d0d0ddd0dd000d00d0d0d0d00d0000000000000000000000000000000000000000
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

__gff__
0001010100000000000000000000000000010000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
__map__
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000015000000000015000000000000000202000000000302020302000000000014000000000000000000140000000000000000001400000000000000
0000000000000000000000000000000000001500000000000000000000000000000000000000000000000000000000000000000000000000001500000000000000000202020302020202020203020000000000001101020202021111111101000000020203020000000000001402020000000000000000020300000000000000
0000000000000000000000000000000000000203000000001500000000000000000000001400000000000202020000000000000000150002020202020200000000000111010101401500010111010000000002020100050001202100110101000003111111110202000000000211010000000000140000010100000000000000
0000000000000000000000000000000000000101000000020302000000000000000000020202000006000111010000000000000000020211010101110100000000000101040000020200000001010000000201000400020001020202010101000001010101010101030000000111010000000003030200050000000000000000
0000000000000000000000001400000000000101001003010101000000000000000000011101000202000001010000000000140000011101000005010100000000000101000000000500000001010000000101000302010001110101100101000001100031002100000015000111010000000301010100000000000000000000
0000000015400000000000100302000000000101030211110101000000000000000000000104001404000000040000000002020202110101000000010100000000000101000000000000000001010000000104400120001401010100000501000001020302000202000003021101000000000101010110000000000000000000
0000000003020200000003021101000000000401110101010100000000000000000000000100000202000002000000000001111101010004000000010100000000000401000000000000000001050000000100020102020301010102020001000000011101000130400001111101000000000401010102000000000000000000
0000000001110114000001111101000000000601010005000000000000000000000000100100000101000001001500000005010100010000400000010100000000000001000000000000000001000000000100010005000015000001010001000000010101020103020201111101000000000000050000000000000000000000
0000000001111103000001111101000000000001000000000000000000000000000000020100150104004001020303000000000100000000200015010100000000000001000000000000000001150000000100010002030202030000140001000000000000000000000001111101060000000000000000000000144000000000
0000000005010101000004010101000000000000000000000000000000000000000000011102020100020200040001000000000102020202100202010100000000000301000000000000000001030000000100010000013001010002020001000000020200000000000000010101000000000000000000000002030202000000
0000000000000004000006010104000000000014000000150000000000400000000000010101000400000100000001000000000401111111021111110100000000000101000000000000200001010000000100010200010001011400000001000000010102210215000000000000000000030014000000000001010101000000
0000000000000000000000010100000000000002000002020000000003020000000000000004000000000100201501000000000001111111011101010000000000000111021003030202030211010000000100000500012001110202001401000000011101200103000000000000000000010202000000000004010101000000
0000000000000000000000000500000000000001030001010300000001010000000000000000000000000103030301000000000001110101000100010000000000000101010201000401010101010000000102020302110211000011020201000000000111011101000000000000000000060101000000000000000004000000
0000000000000000000000000000000000000001010000050000000004000000000000000000000000000000050000000000000000010001000500000000000000000000000500000000000000000000000101010101010100000000010101000000000101010100000000000000000000000004000000000000000000000000
0000000000000000000000000000000000000000050000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000006000000000000000000000000000000000000000000000000140000000000000000000000
0000000000000000140203001400000000000000000000000000000000000000000014000000150000000000000000000000000000000000000000000000000000000000000000000000000000000000000202000000000000020202020000000000000000000000000000000000000000000203020200000000000000000000
0000000000000000021111020300000000000000000000000000000000000000000202000203020202000000000000000000000000001400000000000000000000000000000000000000000000000000000101020200000000010101010200000000000000000202000000000000000000000111110100000014000000000000
0000000000000000010101011103000000000000000000000000000000000000000101000111110101020300140000000000000000020203030000000000000000000000000000000014000000000000000101000000000000010000010100000000000000000101030000000302000000000501010400001002000000000000
0000001400000000012200300101000000000000140000000014000000000000000100000001012000010020030000000000000003010111010000140002030000030315000000000303000000001400000000000000000000010040010100000000000000000000040000000101000000000000040000000301000000000000
0000000203000000010101010100000000001503020000000002030000000000000000000004010300000001011500000000000004012001010002030201010000050103000000000101000000030300000000000000000000010002010000000000000014000000000000000001020000000000000000150101000000000000
0000000101030000000400000400000000000212010000000211110200150000000000140000000000000000010206000014001000212200040001111101040000000101000025000400300003010100000000002600000000000000000000000000020302000000000000000000000000000000000000031101000000000000
0000020000100200000000000000150000030101010000000401011102030000000321030000400000140012010100000002030200030002000201010101000000000106000000000000000005100100000000002100000000000000000000000002010101000000000000000000150000000000000000011101000000000000
0000010002020100000000030202030000011000310000000004000112010000000100010000020000030002110100000001110100010005000000001201000000000015000000000000000000010100000200000000002500260000000202000001000001000300000000000000020000000000000000010105000000002200
0000010031000000000000000111010000010202020000000000000111010000000110010000010312010001010100000000010100010000001203400201000000000303000000000026000000000400000100000000000000020000000101000001100001000140000000030002000000000015001400000000001500400300
0000010303020000400000000101000000040101050000000302210101010000000102010000000102010012010000000000010000010202020201010101020000030101030000000000000000000000000000000000000000000000000001000001020001000102030000060000150000020303020200020203020202030100
0000050101000000020302031101000000000005002200003000000000050000000001010000000001010201010000000000040000010101000025000401010000010005010000000000000000000000000200003100000000000000300201000000140004001500000000000000030000000111110100011111111111010000
0000000004000000000101111101000000000000000000000203400200000000000000050000000000010100050000000000000000000005000000000000040000014000000000001400003100000000000102023002000221020002020100000002030000030200000000000000010000000101010100011111110101000000
0000000000000000000400010105000000000000000000000401020100000000000000000000000000000000000000000000000000000000000000000000000000010303000000000303030000000000000001010201000110010000010100000001010000000400000000000015010000000005010000000101010005000000
0000000000000000000000000500000000000000000000000000000500000000000000000000000000000000000000000000000000000000000000000000000000000005000000000001010000000000000000010100000102010200000000000000010000000000000000000203010000000000000000000400010000000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000050000000000000000000000000000000000000000000000000000000000000000000000000000000000000033000000000000000000
__sfx__
011000200c07510052130520e052130550c002100520c075070750b0520e05213052170551f0021305217055090750b0520e052150520e05500002150520e0550b07510052130520e05213055000020e05213055
011000200c0750c0650c0550c0450c0350c0250c0250c015070750706507055070450703507025070250701509075090650905509045090350902509025090150b0750b0650b0550b0450b0350b0250b0250b015
011000200c05310003130530e0032b6150c003100530c0530c0530c0030e003130032f6151f003130532f6150c0530c0030e05315003266150c003150530e0530c05310003130030e0532b6150c003266142b615
011000200c0720c0620c05507075070320c022070750c01207072070620705509075090320702209075070120907209062090550b0750b032090220b075090120b0720b0620b0550c0750c0320b0220c0750b012
011000200c07510052130520e052130550c002100520c075070750b0520e05213052170551f0021305217055090750b0520e052150520e05500002150520e0550b07510052130520e05213055000020e05213055
011000200c07210062130550e075130320c0220c0750c012070720b0620e0551307517032070220707507012090720b0620e055150750e0320902209075090120b07210062130550e075130320b0220b0750b012
002000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
011000200c05310003130030e0032b6150c003100030c0030c0530c0030e003130032f6151f003130032f6150c0530c0030e00315003266150c003150030e0030c053100030c0430e0030c0530c0631807318073
011000200c07510052130520e052130550c002100520c075070750b0520e05213052170551f0021305217055090750b0520e052150520e05500002150520e055090750b0520e052100520e0550e0021505210055
011000200c07510052130520e052130550c002100520c0750b07510052130520e05213055000020e052130550c07210052130520e002090720b0520e0520b0050b07210052130520e05513055000020e05213055
011000200c07510052130520e052130550c002100520c075090750b0520e052150520e05500002150520e055070750b0520e05213052170551f0021305217055070750b0020e05213002170551f0021305217005
011000200c31710307133170e3171331713307103170c317073160b3160e3161331617316173071331717317093170b3070e317153160e3170e307153170e3170b31710307133170e30713317133070e31713316
011000200c145101050c14510105131450e1451314513105101450c145071450b1450e1451314517145171051314517145091450b1050e145151450e1450e105151450e1450b14510105131450e1051314513105
011000200c33510335133350e335133350c305103350c3350b33510335133350e33513335003050e335133350c33510335133350e305093350b3350e3350b3050b33510335133350e33513335003050e33513335
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0104000018134131340f1340c1340c1040c1040f10400104001040010400104001040010400104001040010400104001040010400104001040010400104001040010400104001040010400104001040010400104
010c00000c07312003000030000300003000030000300003000030000300003000030000300003000030000300003000030000300003000030000300003000030000300003000030000300003000030000300003
010900000c3550f3551335518351183551b3050030500305003050030500305003050030500305003050030500305003050030500305003050030500305003050030500305003050030500305003050030500305
01100000223551b355000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005
0109000016155221551d1050010500105001050010500105001050010500105001050010500105001050010500105001050010500105001050010500105001050010500105001050010500105001050010500105
001000001f15522105000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005
011000001b15500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005
__music__
00 03074344
01 05024c44
00 00024644
00 08024644
00 09024644
00 0a024644
00 0b024c44
00 0b020c44
00 4b020c44
00 05020c44
00 00020c44
00 08020c44
00 09020c44
00 0d020c44
02 4d020c44

