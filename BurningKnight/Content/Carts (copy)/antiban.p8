pico-8 cartridge // http://www.pico-8.com
version 19
__lua__
-- antiban
-- by @egordorichev

--[[ todo
sfx
music
]]

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
 --m()
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
 reload(0x2000,0x2000,0x1000)
 reload(0x1000,0x1000,0x1000)
 
 entity_reset()
 collision_reset()
 
 e_add(level({
  base=v(g_index%8*16,flr(g_index/8)*16),
  size=v(16,16)
 }))
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
 draw_order=2,
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
 local f=e.bold and ospr or spr
 gs=e
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
 if (gs.anti) pal(10,8)
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
   ent.vel+=v(0,w)
  end
 end
end

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

function c_check(box,tags)
 local fake_e={pos=v(box.xl,box.yt)} 
 for tag in all(tags) do
  for o in c_potentials(fake_e,tag) do
   local oc=c_collider(o)
   if oc and not o.nocol and box:overlaps(oc.b) then
    return oc.e
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
 for e in all(entities_with.feetbox) do  
  local fb=e.feetbox
  if fb then
   fb=fb:translate(e.pos)
   local support=c_check(fb,{"walls"})
-- ) support=nil
   e.supported_by=support
   if support and support.vel then
    e.pos+=support.vel
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
 if(not o) o=5--sget(97,c)
 
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
 g_level=self
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
    if(not e.norem)mset(xx,yy,0)
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
 palt(12,true)
 map(self.base.x,self.base.y)
 palt(12,false)
end
 
solid=static:extend({
 tags={"walls","cor"},
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

box=entity:extend({
 tags={"box"},
 draw_order=3
})

box:spawns_from(18,48)

function box:init()
 self.
 sprite={idle={self.tile}}
end

pr=entity:extend({
 draw_order=3
})

pr:spawns_from(32)

function pr:render()
 spr(self.tile,self.pos.x,self.pos.y+cos(self.t*0.01)*2.5)
end

function pr:init()
 self.x,self.y=
  self.pos.x/8+g_level.base.x,
  self.pos.y/8+g_level.base.y
  
end

function pr:idle()
 if self.t==0 then
  
 mset(self.x,self.y,34)
 end

 if mget(self.x,self.y)==0 then
  self.done=true
  shk=4
 end
end
-->8
-- player

guy=entity:extend({
 sprite={
  idle={2,3,4,5,6,delay=6},
  move={8,9,10,11,delay=6},
  flips=true
 },
 tags={"guy"},
 draw_order=4
})

guy:spawns_from(1,7)

function guy:init()
 self.bold=true
 self.t=rnd(32)
 self.anti=self.tile==7
end

function guy:idle()
 local mx,my=0,0
 
 if self.buf then
  mx,my=self.buf[1],self.buf[2]
  self.buf=nil
 else
  if (btnp(⬅️)) mx=-1
  if (btnp(➡️)) mx=1
  if (btnp(⬆️)) my=-1
  if (btnp(⬇️)) my=1
 end
 
 if mx~=0 or my~=0 then
  if mx~=0 then
   my=0
  end
  
  if self.anti then
   mx*=-1
   my*=-1
  end
 
  if(g_index<16)g_moves+=1
  self:become("move")
  sfx(10)
  self.vx=mx
  self.vy=my
  self.sx=self.pos.x+mx*8
  self.sy=self.pos.y+my*8
  self.ox=self.pos.x
  self.oy=self.pos.y
  
  local x,y=self.sx/8,self.sy/8
  local t=mget(x+g_level.base.x,y+g_level.base.y)
  self.blk=fget(t,0)
  self.pl=nil
 	
  self.ext=t==16
  
  local bx=self.pos.x-mx*8
  local by=self.pos.y-my*8
  
  local a=entities_tagged["box"]
  
  if a then
   for b in all(a) do
    if b.pos.x==self.sx and b.pos.y==self.sy then
     self.blk=true
    elseif b.pos.x==bx and b.pos.y==by then
     self.pl=b
    end
   end
  end
  
  for b in all(entities_tagged["guy"]) do
   if b~=self and b.pos.x==self.sx and b.pos.y==self.sy then
    self.blk=true
   elseif b~=self and b.sx==self.sx and b.sy==self.sy then
    self.blk=true
   end
  end
 end
end

function guy:move()
 self.flipped=self.vx<0

 if self.t==0 then
  local x,y=self.ox/8+g_level.base.x,self.oy/8+g_level.base.y
  onunset(x,y)
  
  if self.pl then
   local x,y=self.pl.pos.x/8+g_level.base.x,
   self.pl.pos.y/8+g_level.base.y
   onunset(x,y)
  end
 end
 
 local mx,my=0,0
 
 if (btnp(⬅️)) mx=-1
 if (btnp(➡️)) mx=1
 if (btnp(⬆️)) my=-1
 if (btnp(⬇️)) my=1
 
 if mx~=0 or my~=0 then
  if(mx~=0) my=0
  self.buf={mx,my}
 end

 local p=self.t*(1/8)
 
 if (self.blk and p>=0.5) p=1-p
 
 self.pos.x=self.ox+(self.sx-self.ox)*p
 self.pos.y=self.oy+(self.sy-self.oy)*p
 
 local dn=self.t>=8
 
 if dn then
  if self.blk then
   self.pos.x=self.ox
   self.pos.y=self.oy
  else
   self.pos.x=self.sx
   self.pos.y=self.sy
  end
  
  self:become("idle")
 self.t=rnd(32)
 
  if self.ext then
   g_index+=1
   fade()
   shk=3
   sfx(14)
   restart_level()
   return
  end
  
  local x,y=self.pos.x/8+g_level.base.x,self.pos.y/8+g_level.base.y
  onset(x,y)
 end
 
 if self.pl then
  self.pl.pos.x=self.pos.x-self.vx*8
  self.pl.pos.y=self.pos.y-self.vy*8
 
  if dn then
   local x,y=self.pl.pos.x/8+g_level.base.x,
   self.pl.pos.y/8+g_level.base.y
   onset(x,y)
  end
 end
end

function onset(x,y)
 local t=mget(x,y)
 
 if t==34 then
  mset(x,y,0)
  sfx(12)
  return
 end
 
 if t==20 or t==21 then
  mset(x,y,t==20 and 36 or 37)
  local act=t==20
  shk=3
  sfx(11)
  
  if not act then
   local b
   for tx=g_level.base.x,g_level.base.x+16 do
    for ty=g_level.base.y,g_level.base.y+16 do
     if mget(tx,ty)==21 then
      b=true
      break
     end   
    end
    if(b) break
   end
   
   act=not b
  end
  
  if act then
   for tx=g_level.base.x,g_level.base.x+16 do
    for ty=g_level.base.y,g_level.base.y+16 do

     if mget(tx,ty)==19 then
      mset(tx,ty,35)
      shk=5
     end   
    end
   end
  end
 end
end

function onunset(x,y)
 local t=mget(x,y)  
  
 if t==37 then
  mset(x,y,21)
  sfx(15)
  
  for tx=g_level.base.x,g_level.base.x+16 do
   for ty=g_level.base.y,g_level.base.y+16 do
    if mget(tx,ty)==35 then
     mset(tx,ty,19)
    end   
   end
  end
 end
end
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
 do_movement()
 do_collisions()
 do_supports()
end

function ingame.draw()
 cls()
 if shk>0 then
  camera(rnd(shk)-shk/2,rnd(shk)-shk/2)
  shk-=0.5
 end
 r_render_all("render")
 
 if btnp(❎) then
  fade()
  sfx(13)
  restart_level()
 end
 camera()
 
 if g_index==1 or fnd() then
  coprint("❎ to restart ",120,7)
 elseif g_index==0 then
  coprint("⬅️➡️⬆️⬇️    ",120,7)
 elseif g_index==16 then
  coprint("^you won!",12+sin(g_time*0.01)*2.5,7)
  oprint(g_moves.." turns",2,120,7)
 end
end

function fnd()
 for p in all(entities_tagged["guy"]) do
  if p.state~="move" then
   local px,py=p.pos.x/8,p.pos.y/8
 
   local fnd=false
   for xx=-1,1 do
    for yy=-1,1 do
     if not chk(px,py,xx,yy) then
      fnd=true
      break
     end
    end
    if(fnd) break
   end
   
   if fnd then
    return false
   end
  else
   return false
  end
 end
 
 return true
end

function chk(px,py,xx,yy)
 if abs(xx)+abs(yy)==1 then
  local x,y=px+xx,py+yy

  local t=mget(x+g_level.base.x,y+g_level.base.y)
  if fget(t,0) then
   return true
  end
  
  local a=entities_tagged["box"]
  
  if a then
   for b in all(a) do
    if b.pos.x==x*8 and b.pos.y==y*8 then
     return true
    end
   end
  end
  
  for b in all(entities_tagged["guy"]) do
   if b~=self and b.pos.x==x*8 and 
    b.pos.y==y*8 then
    
    return true
   elseif b~=self and b.sx==x*8 and b.sy==y*8 then
    return true
   end
  end
 else
  return true
 end
 
 return false
end

menu={}

function menu.update()

end

function menu.draw()

end

__gfx__
00000000eee00eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee00eeeeeeaaeeeeeeeeeeeeeeeeeeeeeeaaeee00000000000000000000000000000000
00000000ee0aa0eeeeeaaeeeeeeeeeeeeeeeeeeeeeeaaeeeeeeaaeeeee0880eeeeeaaeeeeeeaaeeeeeeaaeeeeeeaaeee00000000000000000000000000000000
00700700ee0aa0eeeeeaaeeeeeeaaeeeeeeaaeeeeeeaaeeeeeeaaeeeee0880eeeeaaaaeaeeaaaaeeeeeaaeeeeeeaaeee00000000000000000000000000000000
00077000e0aaaa0eeeaaaaeeeeaaaaeeeeeaaeeeeeeaaeeeeeaaaaeee088880eeaaaaaaeeaaaaaaaeeaaaaeeeeaaaaee00000000000000000000000000000000
000770000aaaaaa0eaaaaaaeeaaaaaaeeeaaaaeeeeaaaaeeeeaaaaee08888880aeaaaaeeeaaaaaeeeaaaaaaeeaaaaaae00000000000000000000000000000000
00700700a0aaaa0aaeaaaaeaaeaaaaeaaaaaaaaaeaaaaaaeeaaaaaae80888808eeaeeeaeeeaeeaeeeeaaaaaeeaaaaaea00000000000000000000000000000000
0000000000a00a00eeaeeaeeeeaaaaeeeeaaaaeeaeaaaaeaaeaeeaea00800800eaeeeeeaeeaeeeaeeeaeeaeeeeaeeaee00000000000000000000000000000000
00000000e0a00a0eeeaeeaeeeeaeeaeeeeaeeaeeeeaeeaeeeeaeeaeee080080eeeeeeeeeeeeeeeaeeeeaaeeeeaeeeaee00000000000000000000000000000000
00000000566666659aaaaaa904444440000000000000000000000000000000005666666556666665000000000000000000000005500000000000000000000000
09aaaaa966666666aaaaaaaa41222214003bb3000028820000000000000000006666666666666666000000000000000000000066660000000000000000000000
0a00000a66666666aaaaaaaa4244442403bbbb300288882000000000000000006666666666666666000000000000000000000666666000000000000000000000
0a0a000a66666666aaaaaaaa424444240bbb3bb00888288000000000000000006666666666666666000000000000000000006666666600000000000000000000
0a0a000a66666666aaaaaaaa424444240bb3bbb00882888000000000000000006666666666666666000000000000000000066666666660000000000000000000
0a0a090a66666666aaaaaaaa4244442403bbbb300288882000000000000000006666666666666666000000000000000000666666666666000000000000000000
0a00000a66666666aaaaaaaa42444424013bb3100128821000000000000000006666666666666666000000000000000006666666666666600000000000000000
09aaaaa9566666659aaaaaa942444424001111000011110000000000000000005666666666666665000000000000000056666665566666650000000000000000
0aa00aa0155555510ee00ee004444440000000000000000000000000000000005666666666666665000000000000000056666665566666650000000000000000
09a99a90555555550eeeeee042000024000000000000000000000000000000006666666666666666000000000000000006666666666666600000000000000000
288aa88255555555eeeeeeee40000004001331000012210000000000000000006666666666666666000000000000000000666666666666000000000000000000
888aa88855555555eeeeeeee40000004013333100122221000000000000000006666666666666666000000000000000000066666666660000000000000000000
02299220555555550eeeeee040000004033313300222122000000000000000006666666666666666000000000000000000006666666600000000000000000000
088aa880555555550eeeeee040000004033133300221222000000000000000006666666666666666000000000000000000000666666000000000000000000000
088aa880555555550eeeeee040000004013333100122221000000000000000006666666666666666000000000000000000000066660000000000000000000000
028aa820155555510eeeeee040000004001331000012210000000000000000005666666556666665000000000000000000000005500000000000000000000000
0aa00aa0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
09a99a90000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
1ddaadd1000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
dddaaddd000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
01199110000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0ddaadd0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0ddaadd0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
01daad10000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000eeeaaeeeeeeeeeeeeeeeeeeeeeeaaeee00000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000eeeaaeeeeeeaaeeeeeeaaeeeeeeaaeee00000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000eeaaaaeaeeaaaaeeeeeaaeeeeeeaaeee00000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000eaaaaaaeeaaaaaaaeeaaaaeeeeaaaaee00000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000aeaaaaeeeaaaaaeeeaaaaaaeeaaaaaae00000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000eeaeeeaeeeaeeaeeeeaaaaaeeaaaaaea00000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000eaeeeeeaeeaeeeaeeeaeeaeeeeaeeaee00000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000eeeeeeeeeeeeeeaeeeeaaeeeeaeeeaee00000000000000000000000000000000
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
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
11111111111111111111111111111111000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000110000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
11111111111111111111111111111111000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
11111111111111111111111111111111000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
11111111111111111111111111111111000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
11111111000000000000001111111111000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
11111100000000000000000011111111000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
11111100000300020003000011111111000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
11111100000000000000000011111111000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
11111100000200100002000011111111000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
11111100000000000000000011111111000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
11111100000300020003000011111111000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
11111100000000000000000011111111000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
11111111000000000000001111111111000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
11111111111111111111111111111111000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
11111111111111111111111111111111000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
11111111111111111111111111111111000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
__label__
56666665566666655666666556666665566666655666666556666665566666655666666556666665566666655666666556666665566666655666666556666665
66666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666
66666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666
66666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666
66666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666
66666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666
66666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666
56666665566666655666666556666665566666655666666556666665566666655666666556666665566666655666666556666665566666655666666556666665
56666665566666650000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000005666666556666665
66666666666666600000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000666666666666666
66666666666666000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000066666666666666
66666666666660000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000006666666666666
66666666666600000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000666666666666
66666666666000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000066666666666
66666666660000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000006666666666
56666665500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000556666665
56666665000000000000000556666665500000000000000050000000000000005000000000000000500000000000000000000000500000000000000056666665
66666666000000000000006666666666660000000000000066000000000000006600000000000000660000000000000000000000660000000000000066666666
66666666000000000000066666666666666000000000000066600000000000006660000000000000666000000000000000000000666000000000000066666666
66666666000000000000666666666666666600000000000066660000000000006666000000000000666600000000000000000000666600000000000066666666
66666666000000000006666666666666666660000000000066666000000000006666600000000000666660000000000000000000666660000000000066666666
66666666000000000066666666666666666666000000000066666600000000006666660000000000666666000000000000000000666666000000000066666666
66666666000000000666666666666666666666600000000066666660000000006666666000000000666666600000000000000000666666600000000066666666
56666665000000005666666556666665566666650000000056666665000000005666666500000000566666650000000000000000566666650000000056666665
56666665000000005666666500000000566666650000000056666665500000005666666500000000566666655666666500000000566666650000000056666665
66666666000000006666666600000000666666660000000066666666660000006666666600000000666666666666666000000000666666660000000066666666
66666666000000006666666600000000666666660000000066666666666000006666666600000000666666666666660000000000666666660000000066666666
66666666000000006666666600000000666666660000000066666666666600006666666600000000666666666666600000000000666666660000000066666666
66666666000000006666666600000000666666660000000066666666666660006666666600000000666666666666000000000000666666660000000066666666
66666666000000006666666600000000666666660000000066666666666666006666666600000000666666666660000000000000666666660000000066666666
66666666000000006666666600000000666666660000000066666666666666606666666600000000666666666600000000000000666666660000000066666666
56666665000000005666666500000000566666650000000056666665566666655666666500000000566666655000000000000000566666650000000056666665
56666665000000005666666556666665566666650000000056666665566666655666666500000000566666650000000000000000566666650000000056666665
66666666000000006666666666666666666666660000000066666666066666666666666600000000666666660000000000000000666666660000000066666666
66666666000000006666666666666666666666660000000066666666006666666666666600000000666666660000000000000000666666660000000066666666
66666666000000006666666666666666666666660000000066666666000666666666666600000000666666660000000000000000666666660000000066666666
66666666000000006666666666666666666666660000000066666666000066666666666600000000666666660000000000000000666666660000000066666666
66666666000000006666666666666666666666660000000066666666000006666666666600000000666666660000000000000000666666660000000066666666
66666666000000006666666666666666666666660000000066666666000000666666666600000000666666660000000000000000666666660000000066666666
56666665000000005666666556666665566666650000000056666665000000055666666500000000566666650000000000000000566666650000000056666665
56666665000000005666666500000000566666650000000056666665000000005666666500000000566666655666666500000000566666650000000056666665
66666666000000006666666000000000066666660000000066666660000000000666666600000000066666666666666000000000066666660000000066666666
66666666000000006666660000000000006666660000000066666600000000000066666600000000006666666666660000000000006666660000000066666666
66666666000000006666600000000000000666660000000066666000000000000006666600000000000666666666600000000000000666660000000066666666
66666666000000006666000000000000000066660000000066660000000000000000666600000000000066666666000000000000000066660000000066666666
66666666000000006660000000000000000006660000000066600000000000000000066600000000000006666660000000000000000006660000000066666666
66666666000000006600000000000000000000660000000066000000000000000000006600000000000000666600000000000000000000660000000066666666
56666665000000005000000000000000000000050000000050000000000000000000000500000000000000055000000000000000000000050000000056666665
56666665000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000056666665
66666666000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000066666666
66666666000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000066666666
66666666000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000066666666
66666666000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000066666666
66666666000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000066666666
66666666000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000066666666
56666665000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000056666665
56666665000000000000000000000005500000000000000000000005566666655000000000000000500000000000000050000000000000000000000056666665
66666666000000000000000000000066660000000000000000000066666666666600000000000000660000000000000066000000000000000000000066666666
66666666000000000000000000000666666000000000000000000666666666666660000000000000666000000000000066600000000000000000000066666666
66666666000000000000000000006666666600000000000000006666666666666666000000000000666600000000000066660000000000000000000066666666
66666666000000000000000000066666666660000000000000066666666666666666600000000000666660000000000066666000000000000000000066666666
66666666000000000000000000666666666666000000000000666666666666666666660000000000666666000000000066666600000000000000000066666666
66666666000000000000000006666666666666600000000006666666666666666666666000000000666666600000000066666660000000000000000066666666
56666665000000000000000056666665566666650000000056666665566666655666666500000000566666650000000056666665000000000000000056666665
56666665000000000000000056666665000000050000000056666665000000005666666500000000566666655000000056666665000000000000000056666665
66666666000000000000000066666666000000660000000066666666000000006666666600000000666666666600000066666666000000000000000066666666
66666666000000000000000066666666000006660000000066666666000000006666666600000000666666666660000066666666000000000000000066666666
66666666000000000000000066666666000066660000000066666666000000006666666600000000666666666666000066666666000000000000000066666666
66666666000000000000000066666666000666660000000066666666000000006666666600000000666666666666600066666666000000000000000066666666
66666666000000000000000066666666006666660000000066666666000000006666666600000000666666666666660066666666000000000000000066666666
66666666000000000000000066666666066666660000000066666666000000006666666600000000666666666666666066666666000000000000000066666666
56666665000000000000000056666665566666650000000056666665000000005666666500000000566666655666666556666665000000000000000056666665
56666665000000000000000056666665566666655000000056666665566666655666666500000000566666655666666556666665000000000000000056666665
66666666000000000000000066666666666666606600000066666666666666666666666600000000666666660666666666666666000000000000000066666666
66666666000000000000000066666666666666006660000066666666666666666666666600000000666666660066666666666666000000000000000066666666
66666666000000000000000066666666666660006666000066666666666666666666666600000000666666660006666666666666000000000000000066666666
66666666000000000000000066666666666600006666600066666666666666666666666600000000666666660000666666666666000000000000000066666666
66666666000000000000000066666666666000006666660066666666666666666666666600000000666666660000066666666666000000000000000066666666
66666666000000000000000066666666660000006666666066666666666666666666666600000000666666660000006666666666000000000000000066666666
56666665000000000000000056666665500000005666666556666665566666655666666500000000566666650000000556666665000000000000000056666665
56666665000000000000000056666665000000055666666556666665000000005666666500000000566666650000000056666665000000000000000056666665
66666666000000000000000066666666000000666666666066666660000000000666666600000000666666600000000006666666000000000000000066666666
66666666000000000000000066666666000006666666660066666600000000000066666600000000666666000000000000666666000000000000000066666666
66666666000000000000000066666666000066666666600066666000000000000006666600000000666660000000000000066666000000000000000066666666
66666666000000000000000066666666000666666666000066660000000000000000666600000000666600000000000000006666000000000000000066666666
66666666000000000000000066666666006666666660000066600000000000000000066600000000666000000000000000000666000000000000000066666666
66666666000000000000000066666666066666666600000066000000000000000000006600000000660000000000000000000066000000000000000066666666
56666665000000000000000056666665566666655000000050000000000000000000000500000000500000000000000000000005000000000000000056666665
56666665000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000056666665
66666666000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000066666666
66666666000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000066666666
66666666000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000066666666
66666666000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000066666666
66666666000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000066666666
66666666000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000066666666
56666665000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000056666665
56666665000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000056666665
66666666000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000066666666
66666666000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000066666666
66666666000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000066666666
66666666000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000066666666
66666666000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000066666666
66666666000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000066666666
56666665000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000056666665
56666665500000000000000000000000000000000000000000000000000000055000000000000000000000000000000000000000000000000000000556666665
6666666666000000000000000000000000000000000aa0000000000000000066660000000000000009aaaaa90000000000000000000000000000006666666666
6666666666600000000000000000000000000000000aa000000000000000066666600000000000000a00000a0000000000000000000000000000066666666666
666666666666000000000000000000000000000000aaaa00000000000000666666660000000000000a0a000a0000000000000000000000000000666666666666
66666666666660000000000000000000000000000aaaaaa0000000000006666666666000000000000a0a000a0000000000000000000000000006666666666666
6666666666666600000000000000000000000000a0aaaa0a000000000066666666666600000000000a0a090a0000000000000000000000000066666666666666
666666666666666000000000000000000000000000a00a00000000000666666666666660000000000a00000a0000000000000000000000000666666666666666
566666655666666500000000000000000000000000a00a000000000056666665566666650000000009aaaaa90000000000000000000000005666666556666665
56666665566666655000000000000000000000000000000000000005566666655666666550000000000000000000000000000000000000055666666556666665
66666666666666666600000000000000000000000000000000000066666666666666666666000000000000000000000000000000000000666666666666666666
66666666666666666660000000000000000000000000000000000666666666666666666666600000000000000000000000000000000006666666666666666666
66666666666666666666000000000000000000000000000000006666666666666666666666660000000000000000000000000000000066666666666666666666
66666666666666666666600000000000000000000000000000066666666666666666666666666000000000000000000000000000000666666666666666666666
66666666666666666666660000000000000000000000000000666666666666666666666666666600000000000000000000000000006666666666666666666666
66666666666666666666666000000000000000000000000006666666666666666666666666666660000000000000000000000000066666666666666666666666
56666665566666655666666500000000000000000000000000000005000000050000000500000005000000000000000000000000566666655666666556666665
56666665566666655666666556666665566666655666666007777700077777000777770007777700566666655666666556666665566666655666666556666665
66666666666666666666666666666666666666666666666077755770775577707775777077555770666666666666666666666666666666666666666666666666
66666666666666666666666666666666666666666666666077500770770057707750577077000770666666666666666666666666666666666666666666666666
66666666666666666666666666666666666666666666666077700770770077707700077077707770666666666666666666666666666666666666666666666666
66666666666666666666666666666666666666666666666057777750577777505777775057777750666666666666666666666666666666666666666666666666
66666666666666666666666666666666666666666666666005555500055555000555550005555500666666666666666666666666666666666666666666666666
66666666666666666666666666666666666666666666666600000006000000060000000600000006666666666666666666666666666666666666666666666666
56666665566666655666666556666665566666655666666556666665566666655666666556666665566666655666666556666665566666655666666556666665

__gff__
0000000000000000000000000000000000010001000000000101000001010000000100000000000001010000010100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
__map__
1111111111111111111111111111111111111819111111111111111111111111111111111111111111111121111111111111111111111111111111111111111111111111111111111121111111111111111111111111111111111111111111111111111111111121111111111111111111111111111111111111111111111111
112d0f0f0f0f0f0f0f0f0f0f0f0f2c1111112829112111111111111111111111111111111111111111111111111118191118191111112111111111000011111111111111111111111111111111000011111111000000110000110000000011111111111111111111111111111111111111111111000000111111111111111819
11001c111d0f1d0f1d0f1d0f0f1d0f1111111111111111111111111100000011111121111100001100000011111128291128291100001111111111000011111111111100111111111111111111000011111111001000110000110011110011111100000011111111111111111100001111111111001000110000111111112829
11001100110f111d110f112d0f110f1111111111111111111111211100000011111121111100001100000011111111111111110000000011111111111111111111111111110000000011110011111111111111000000111111110011110011111100210011111111001111111100001111111111000000110000110011111111
11001111110f112c110f110f0f110f1111000011000000111111111100000011111111111118191100000011112111111111000010000000111111111111111111111111110000000013000000111111111111111311111111110000000011111100000011110000000000111111111111111111110011111111111111111111
110f2d0f2c0f2d0f2c0f2c2d0f2c0f1111000011001000111111111111111111110000001128291111111111112111111111000000000000111111111111111111211111000001150011001000111111110000000000000011111111111111111118191111110000120000111111111111111111121212000012120011111111
110f0f0f0f0f0f0f0f0f0f0f0f0f0f1111111111000000111100001111111111110010001111111111111111111111111111111311111111110000001100111111111111000000000011000000111111110001000000000011110000110011111128291111000012000000001111111111111111120012121212121211001111
110f0f1c1d0f1c111d0f1d0f1d0f0f1111110011111211110000001100111111110000001200120011111111000011111100000000111111110000001111111111111100000000000011111111111111110000001212121111110000111111111111111111001500001500000011111121111111000012000000001211111111
110f0f111c0f1100110f111d110f0f1111111111110011000000001111111111111111110012001200001111000011111100001400111111110000001111111111111100001200000000111111111111111111111112120011111111181921111111001111111100001101000011111111111111000012001212001211000011
110f0f112d1d1111110f112c110f0f1111111111000000010000111111111111111121111100000000001111111111111100000000000000111111111111111111111111000000000000111100000011112111110000000011111111282911111111111111111100000000000011111111000011000000120000120011000011
110f0f111c2d2d0f2c0f2d0f2c0f0f1111111111000000000000111111211111111111110000000000001100111111111111000001000000111111111111111111211111110000000011111100000011111111111100150011111111111111111100001111111111000000001111111111000011120012120012001211111111
110f0f0f0f0f0f0f0f0f0f0f0f0f0f1111111111000000000011111111111111110011110000010000111111111111111111110000000011111111111111111111111111111111111111111100000011110000111100000011111100000011111100001111111111131111111111001111111111121212120012001211111111
110f0f0f0f0f0f0f0f0f0f0f0f0f0f1111211111110000001111111100001111111111111100000011111100000011111111111819111111111111110000111111110000111118191100001111111111110000111111111111111100000011111111111111110000000000111111111111111111001200120000011211111111
111d0f0f0f010f1c1d0f100f0f0f1c1111111111111111111111111100001111110000111111111111111100000011111111112829111111111111110000111111110000111128291100001100111111111111111111111111111100000011111100001111000010000000111100001111111819111111111111111111111111
11111d0f0f0f1c11191d0f0f0f1c111111111111211111111111111111111111110000111819111111111100000011111111111819111121111111111111111111111111111111111111111111111111111100111111111121111111111111111100001111110000001111111100001111112829111111110011110011111111
1111111111111111111111111111111111111111111111111111211111111111111111112829111111111111111111111111112829111111111118191111111111111111111111111111111111111111111111111111181911111111111111111111111111111111111111111111111111111111111111111111111111111111
1111112111111111111111111111111111211111111111111111111111111111111111111111111111211111111111111121111111111111111128291111111111111111111111111111111111111111111111111111282911111111111111111111111111111111111111111111111111111111111111111111111111111111
1111111111111111111111111111111111110000110000001111000011111111110000111111211111000000111111110000001100000011111111111111111111001111000000000011110000111121111111111100001111111111111111111111211111111111181911000011111111111111001111001111111111111111
1111000000000000000000000000111111110000110010001111000011002111110000110011111111000000181911110000001100100011111100112111111111111100000100000000110000111111110000001100001111111400001111111111111111111111282911000011211111111111111111111111111111111111
1111001500111111111111001500111111111111110000001111282911111111111111111111111111000000282911110000001100000011111111110000111111111100000000000012111111111111110007001111110011000000120011111100001111111111111111111111111111000000000000111111000000111111
1100001111111111112111111100111111111115111113111100001111111111111111111111111111111111111111212829111111131121111111110000111111111111112111001212000011001111110000001111111112001212000011111100001101000000000000011111001111001111001100111111000000111111
1100000000121211110000111100001111211100110000000000001111001111000011111500000011001300101111111111111100000000000000111111111819112111111111121200000011112111111311131121111112001200120000112111111100111111001111001111111111010012000032110011310032111111
1100001100121211000000001100001111111100110000150012001111111819000011111111110011001111111111111111000000000000000000001111112829111111000012120000150011111111110000001111111100000000120012111111111100111511001511001100001111001100110011111111111111111111
1100001100000000000000000000001111111100110001000001001100002829111111000000110011001100000011111111000000000700000100000011111111001100000000000000000011001111111212001212111112121200120012111100001100110011000000001100001111310000000011000011111111000011
1111000000000000010000111100001111111100110000001100001100001111110011000100000011000000010011111111000000000000000000000000111111111100000000000000111111111111110012000000111100001200000012111100001100110000120000001111111111111311110011310011110011000011
1111001111000000000011111100001111111100110000001100111111111111111111000000111211001100000011111111111111000000150000000000111111111111000000121211111111111111111212001200111101000012110011111111111100110000000000001118191819000000110011111111111111111111
1111001111111111111111111100111111111100111100001111111111111121111111001111110011211111110011111121111111110000001111110000111111110013000012121211111121211111111112121200111200000012001111111100211100110000210011111128292829001000110000111111000000111111
1111001111111100101111111100111111111100111112111111211111111111111111001215000000000000150011111111111111111111111111110000111111110011111112000000001111111111111112001212111112121200111111111111111107111111111111110000001111310032110000000000001500000011
1111001500111113111111001500111118191100000000111111110000001111111111000011000011000011111111111111000000111111111111000000111111000000111100000700000011111111181911101112111111111111111111111111111100001111110000110000001118191111110011001112150000003211
1111000000000000001100000000111128291111111111110000110000001111111111111111111111111111000011112111000000110000111111000011111111001000111100000000000011000011282911111111110000111121110000111111111111001100110000110000001128290000113100321111111111111111
1111111111111111111111111111111111001111211111110000110000001111110011111111211111111111000011111111000000110000110011111121111111000000111111110000001111000011110011111111110000110011110000111110001315001118191111111111111118190032111111111111111111111111
1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111112111111111111111111111111111111128291111111111111128291111111111111111111111111111
__sfx__
010100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010800000e0150e005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005
010400000c65500000106550000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
01060000163551b355000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005
011000001245500400004000040000400004000040000400004000040000400004000040000400004000040000400004000040000400004000040000400004000040000400004000040000400004000040000400
010a0000113251332516321163251b305183050030500305003050030500305003050030500305003050030500305003050030500305003050030500305003050030500305003050030500305003050030500305
010400001065510655306050000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
