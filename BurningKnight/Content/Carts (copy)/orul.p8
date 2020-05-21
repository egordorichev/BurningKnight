pico-8 cartridge // http://www.pico-8.com
version 19
__lua__
-- ourl
-- by @egordorichev

local osfx=sfx
c1=0
c2=5
c3=6
plt=1
plts={
{0,5,6},
{1,13,12},
{9,10,7},
{0,1,13},
{0,3,11},
{0,1,3},
{0,1,2},
{2,4,9},
{2,8,15}
}

function sfx(id)
 if (not g_mute_sfx) osfx(id)
end

function play_music(id)
 if (not g_mute_music) music(id)
end

function _init()
 cls()
 --[[spr(68,48,48,4,4)
 coprint("^rexcellent ^games",84,6)
 for i=0,90 do
  flip()
 end]]

 g_time,state,g_index=
 0,ingame,
 0
 
 music(0)

 shk=0
 restart_level()
 g_deaths=0
 g_sec=0
 -- ∧⌂웃⬇️♥★█⬅️⬇️♥⌂★█⬅️
 --m()
end

function _update60()
 state.update()
 tween_update(1/60)
 g_time+=1
 
 if g_index>0 and g_guy and not g_guy.done then
  g_sec+=1/60
 end
 
 if btnp(❎,1) then
  plt=(plt)%#plts+1
  c1=plts[plt][1]
  c2=plts[plt][2]
  c3=plts[plt][3]
 end
end

function _draw() 
 state.draw()
end

function restart_level()
 g_index=mid(0,31,g_index)
 reload(0x2000,0x2000,0x1000)
 reload(0x1000,0x1000,0x1000)
 
 entity_reset()
 collision_reset()
 
 g_level=e_add(level({
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

function r_reset()
 pal()
 palt(0,false)
 palt(14,true)
 pal(0,c1)
 pal(5,c1)
 pal(1,c2)
 pal(13,c3)
 pal(2,c3)
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
    if e:is_a("guy") then
     g_guy.pos+=support.vel
     g_guy2.pos+=support.vel
    else
     e.pos+=support.vel
    end
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
 o=c2--if(not o) o=sget(97,c)
 
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
 if (fget(blk,2)) return sup
 if (fget(blk,3)) return sdown
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
    mset(b.x+x,b.y+y,1)
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
 map(self.base.x,self.base.y,0,0,16,8)
end
 
solid=static:extend({
 tags={"walls"},
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

sup=solid:extend({
 hitbox=box(0,0,8,4),
})

sdown=solid:extend({
 hitbox=box(0,4,8,8),
})

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

spike=entity:extend({
 collides_with={"guy"}
})

spike:spawns_from(22,38)

function spike:init()
 local up=self.tile==38
 self.hitbox=up and box(0,0,4,8) or box(0,5,8,8)
end

function spike:render()
 if self.t%30>14 then
  if self.pos.y>=64 then
   pal(0,c1)
   pal(1,c3)
   pal(2,c2)
  else
   pal(0,c2)
   pal(1,c1)
   pal(2,c1)
  end
 end
 
 spr(self.tile,self.pos.x,self.pos.y)
end

function spike:collide(o)
 g_guy=nil
 g_guy2=nil
 restart_level()
 sfx(17)
 fade()
 g_deaths+=1
 shk=2
end

platform=entity:extend({
 sprite={
  idle={23},
  width=2
 },
 tags={"walls"},
 collides_with={"walls","guy"},
 hitbox=box(0,0,16,8)
})

platform:spawns_from(23,39)

function platform:init()
 self.dir=-1
 self:become("move")
 self.vel=v(0,0)
 self.sprite.idle[1]=self.tile
end

function platform:idle()
 self.vel.x=0
 if self.t>60 then
  self:become("move")
 end
end

function platform:move()
 self.vel.x=self.dir
end

function platform:collide(o)
 if o:is_a("walls") then
  if self.state~="idle" then
   self.dir*=-1
   self:become("idle")
  end
  return c_move_out
 end
 
 return c_push_out
end

invis=entity:extend({
 tags={"walls"},
 hitbox=box(0,0,8,8)
})

function invis:render()
 local dx=g_guy.pos.x-self.pos.x
 local dy=g_guy.pos.y-self.pos.y
 local dx2=g_guy2.pos.x-self.pos.x
 local dy2=g_guy2.pos.y-self.pos.y
 local d=min(sqrt(dx*dx+dy*dy),sqrt(dx2*dx2+dy2*dy2))
 if d<16 then
  spr(d<12 and 26 or 27,self.pos.x,self.pos.y)
 end
end

function invis:collide()
 return c_push_out
end

invis:spawns_from(26)

swap=entity:extend({
 tags={"walls"},
 sprite={
  idle={42},
  hid={43}
 },
 hitbox=box(0,0,8,8)
})

function swap:init()
 self:become(self.tile==42 and "idle" or "hid")
end

function swap:collide()
 if self.state=="idle" then
  return c_push_out
 end
end

function swap:idle()
 if(self.t>60) self:become("hid")
end

function swap:hid()
 if(self.t>60) self:become("idle")
end

swap:spawns_from(42,43)

jump=entity:extend({
 tags={"walls","jump"},
 sprite={
  idle={58},
  hid={59}
 },
 hitbox=box(0,0,8,8)
})

function jump:init()
 self:become(self.tile==58 and "idle" or "hid")
end

function jump:collide()
 if self.state=="idle" then
  return c_push_out
 end
end

function jump:tog()
 if(self.state=="hid") self:become("idle") return
 self:become("hid")
end

jump:spawns_from(58,59)

ex=entity:extend({
 sprite={idle={15,63,delay=30}}
})

ex:spawns_from(15,63)

function ex:render()
 self.vert=self.pos.y>=64
 if self.t%30>14 then
  if self.vert then
   pal(0,c1)
   pal(1,c3)
   pal(2,c2)
  else
   pal(0,c2)
   pal(1,c1)
   pal(2,c1)
  end
 end
 spr_render(self,self.pos)
end
-->8
-- guy

guy=entity:extend({
 sprite={
  move={10,11,12,delay=5},
  idle={2,3,4,5,6,delay=5},
  jump={8},
  fall={9,25,41,delay=5},
  flips=true
 },
 tags={"guy"},
 collides_with={"walls"},
 hitbox=box(1,0,7,8)
})

function guy:init()
 self.sec=self.tile==7
 self.vel=v(0,0)
 local w=0.1
 
 if self.sec then
  if g_guy2 then self.done=true return end
  g_guy2=self
  self.vert=true
  self.weight=-w
  self.feetbox=box(1,-0.5,7,0.5)
 else
  if g_guy then self.done=true return end
  g_guy=self
  self.feetbox=box(1,7.5,7,8.5)
  self.weight=w
 end
end

function guy:render(w)
 if(w) return
 self:sup()
 if self.sec then
  spr_render(self,self.pos)  
 else
  spr_render(self,self.pos) 
 end
end

function guy:idle()
 self:move()
end

function guy:move()
 local sp=0.7
 if btn(⬅️) then
  self.vel.x-=sp
  self:become("move")
 end
 
 if btn(➡️) then
  self.vel.x+=sp
  self:become("move")
 end
 
 self.vel.x*=0.6
 
 if abs(self.vel.x)<=0.1 then
  self.vel.x=0
  if(self.supported_by) self:become("idle")
 end
 
 if (btnp(❎) or btnp(⬆️)) and self.supported_by then
  local s=1.8
  self:become("jump")  
  local a=entities_tagged.jump
  sfx(16)
  if a and not gt then
   gt=true
   for e in all(a) do
    e:tog()
   end
  end
  
  self.vel.y=self.sec and s or -s
 end
end

function guy:jump()
 self:air_move()
 
 if self.sec then
  if self.vel.y<0 then
   self:become("fall")
  end
 else
  if self.vel.y>0 then
   self:become("fall")
  end
 end
end

function guy:fall()
 self:air_move()
end

function guy:air_move()
 if self.supported_by then
  self:become("idle")
  return
 end
 
 local sp=0.6
 if btn(⬅️) then
  self.vel.x-=sp
 end
 
 if btn(➡️) then
  self.vel.x+=sp
 end
 
 self.vel.x*=0.6
end

function guy:sup()
 if self.sec then
  if self.pos.y<=64 then
   self.pos.y=64
   self.vel.y=0
   self.supported_by={}
  end
 else
  if self.pos.y>=56 then
   self.pos.y=56
   self.vel.y=0
   self.supported_by={}
  end
 end
end

guy:spawns_from(2,7)
-->8

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
 cls(c3)
 
 if shk>0.1 then
  camera(rnd(shk)-shk/2,rnd(shk)-shk/2)
  shk-=0.3
 else
  camera()
 end
 
 r_reset()
 rectfill(0,64,127,127,0)
 pal(0,c2)
 pal(1,c3)
 pal(12,c2)
 pal(13,c1)
 map(g_level.base.x,
  g_level.base.y+8,0,64,16,8)
 r_reset()
 r_render_all("render")

 if g_index~=18 and g_guy and g_guy2 then
  if abs(g_guy.vel.x)<abs(g_guy2.vel.x) then
   g_guy2.vel.x=g_guy.vel.x
   g_guy2.pos.x=g_guy.pos.x
  else
   g_guy.vel.x=g_guy2.vel.x
   g_guy.pos.x=g_guy2.pos.x
  end
  
  if g_guy.vel.y<-g_guy2.vel.y then
   g_guy2.vel.y=-g_guy.vel.y
   g_guy2.pos.y=128-g_guy.pos.y-8
  elseif g_guy.vel.y>-g_guy2.vel.y then
   g_guy.vel.y=-g_guy2.vel.y
   g_guy.pos.y=(128-g_guy2.pos.y)-8
  end
 elseif not g_guy2.done then
  if g_guy2.pos.x>124 then
   g_guy2.done=true
   g_guy.done=true
   music(-1)
   fade()
  end
 end
 
 pal(0,c2)
 if(not g_guy2.done)g_guy2:render()
 r_reset()
 if(not g_guy.done)g_guy:render()
 
 if g_index%8<7 and g_guy.pos.x>=124 then
  local g1=g_guy
  local g2=g_guy2
  g1.pos.x-=128
  g2.pos.x-=128
  g_index+=1
  restart_level()
  e_add(g1)
  e_add(g2)
  return
 end
 
 if g_index>0 and g_guy.pos.x<=-4 then
  local g1=g_guy
  local g2=g_guy2
  g1.pos.x+=128
  g2.pos.x+=128
  g_index-=1
  restart_level()
  e_add(g1)
  e_add(g2)
  return
 end

 if g_index<23 and g_guy2.pos.y>=124 then
  local g1=g_guy
  local g2=g_guy2
  g1.pos.y+=64
  g2.pos.y-=64
  g_index+=1
  restart_level()
  e_add(g1)
  e_add(g2)
  fade()
  return
 end
 
 gt=false
 
 if g_guy.done and g_index==18 then
  coprint("^oh no",60,c3)
  coprint("^thank you",68,c3)
  
  coprint(g_deaths.." fails",80,c3)
  coprint(flr(g_sec+0.5).." seconds",88,c3)
  
  if(btnp(🅾️)) g_guy=nil g_guy2=nil _init()
 end
 
 oprint("^level "..(g_index+1),1,1,c3)
 if(btn(🅾️)) print(stat(1),1,9,7)
end

menu={}

function menu.update()

end

function menu.draw()

end

__gfx__
00000000ddddddddeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee0ee0eeeeeeeeeeeeeeeeeeeee00eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee1eee1
00000000ddddddddeee00eeeeeeeeeeeeeeeeeeeeee00eeeeee00eeeee0ee0ee0ee00ee00ee00ee0eee00ee0eee00eeeeee00eeeeee0eeeeeeee0eeeee1eee1e
00700700ddddddddeee00eeeeee00eeeeee00eeeeee00eeeeee00eee0e0000e0e0e00e0ee0e00e0eee00000eeee00eeeeee00eeeee00eeeeeeee00eee1eee1ee
00077000ddddddddee0000eeee0000eeeee00eeeeee00eeeee0000eee000000eee0000eeee0000eee00000ee0e00000eee0000eee000000ee000000e1eee1eee
00077000dddddddde000000ee000000ee000000eee0000eeee0000eeee0000eeee0000ee0e0000e00e0000eee00000e000000000ee00eeeeeeee00eeeee1eee1
00700700dddddddd0e0000e00e0000e00e0000e000000000e000000eeee00eeeee0000eee000000eee0eee0eee0000eeee0000eeeee0eeeeeeee0eeeee1eee1e
00000000dddddddd0e0ee0e0ee0ee0eeee0000eeee0000ee0e0ee0e0eee00eeee0eeee0eeeeeeeeee0eeeee0eee0ee0eee0ee0eeeeeeeeeeeeeeeeeee1eee1ee
00000000ddddddddee0ee0eeee0ee0eeee0ee0eeee0ee0eeee0ee0eeeeeeeeee0eeeeee0eeeeeeeeeeeeeeeeeeee00eeee0ee0eeeeeeeeeeeeeeeeee1eee1eee
eeeeeeeeeeeeeeeeeeeeeeee555555550000000dd0000000eeeeeeee1000000000000001eeeeeeee01111110eeeeeeee00000000eeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeee55555555000000dddd000000eeeeeeee0000000000000000eee00eee1eeeeee1eeeeeeee00000000ee0eee0e0000e0ee0e000eee
eeeeeeeeeeeeeeeeeeeeeeee5555555500000dddddd00000eee0eeee0000000000000000eee00eee1eeeeee1ee0110ee00000000ee00e00e0ee0e0ee0e0eeeee
ee0eeeeeeeeeeeeeeeeeeeee555555550000dddddddd0000ee010e0e0000000000000000000000001eeeeee1ee1ee1ee00000000ee0e0e0e0ee0e0ee0e00eeee
ee0eeeeeeee0eeeeeee0eeee55555555000dddddddddd000ee01001000000000000000000e0000e01eeeeee1ee1ee1ee00000000ee0eee0e0ee0e0ee0e0eeeee
ee0e0eeeeee0eeeeeee0ee0e5555555500dddddddddddd00e01110100000000000000000e000000e1eeeeee1ee0110ee00000000ee0eee0e0000ee00ee000eee
ee0e0eeee0e0eeeeeee0e0ee555555550dddddddddddddd0001110100000000000000000eeeeeeee1eeeeee1eeeeeeee00000000eeeeeeeeeeeeeeeeeeeeeeee
ee0e0e0ee0e0e0eee0e0e0e055555555dddddddddddddddd111111111000000000000001eeeeeeee01111110eeeeeeee00000000eeeeeeeeeeeeeeeeeeeeeeee
ee0e0e0ee0e0e0eee0e0e0e0ccccccccdddddddddddddddd222222221222222222222221eeeeeeee011111101e1e1e1e00000000eeeeeeeeeeeeeeeeeeeeeeee
ee0e0eeee0e0eeeeeee0e0eecccccccc0dddddddddddddd0112221212222222222222222eee00eee11111111eeeeeee100000000eeeeeeeeeeeeeeeeeeeeeeee
ee0e0eeeeee0eeeeeee0ee0ecccccccc00dddddddddddd00e12221212222222222222222eee00eee111111111eeeeeee00000000ee0eee0e0000ee00ee000eee
ee0eeeeeeee0eeeeeee0eeeecccccccc000dddddddddd000ee1211212222222222222222e000000e11111111eeeeeee100000000ee0eee0e0ee0e0ee0e0eeeee
ee0eeeeeeeeeeeeeeeeeeeeecccccccc0000dddddddd0000ee121e1e222222222222222200000000111111111eeeeeee00000000ee0e0e0e0ee0e0ee0e00eeee
eeeeeeeeeeeeeeeeeeeeeeeecccccccc00000dddddd00000eee1eeee22222222222222220000000011111111eeeeeee100000000ee00e00e0ee0e0ee0e0eeeee
eeeeeeeeeeeeeeeeeeeeeeeecccccccc000000dddd000000eeeeeeee2222222222222222eeeeeeee111111111eeeeeee00000000ee0eee0e0000e0ee0e000eee
eeeeeeeeeeeeeeeeeeeeeeeecccccccc0000000dd0000000eeeeeeee1222222222222221eeeeeeee01111110e1e1e1e100000000eeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee0000000000000000000000000000000000000000011111101e1e1e1e00000000eeeeeeeeeeeeeeeee1eee1ee
ee0000eee000eee00ee0eee0eee000e00000eeee00000000000000000000000000000000000000001eeeeee1eeeeeee100000000eeeeeeeeeeeeeeee1eee1eee
e0eeee0ee0ee0e0ee0e0eee0eee0eeeee0eeeeee00000000000000000000000000000000000000001e1111e11e1e1eee00000000eee0eeeeeeee0eeeeee1eee1
e0eeee0ee0ee0e0ee0e0eee0eee00eeee0eeeeee00000000000000000000000000000000000000001e1111e1eeeee1e100000000ee00eeeeeeee00eeee1eee1e
e0eeee0ee000ee0000e0eee0eee0eeeee0eeeeee00000000000000000000000000000000000000001e1111e11e1eeeee00000000e000000ee000000ee1eee1ee
e0ee0e0ee0eeee0ee0e0eee0eee0eeeee0eeeeee00000000000000000000000000000000000000001e1111e1eee1e1e100000000ee00eeeeeeee00ee1eee1eee
ee0000eee0eeee0ee0e000e000e000eee0eeeeee00000000000000000000000000000000000000001eeeeee11eeeeeee00000000eee0eeeeeeee0eeeeee1eee1
eeeeee0eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee000000000000000000000000000000000000000001111110e1e1e1e100000000eeeeeeeeeeeeeeeeee1eee1e
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee000000000000000000000000000000000000000000000000eeeeee0eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee00000000
ee0e0eeee0000e0ee0e0eee0e000eeee000000000000000000000000000000000000000000000000ee0000eee0eeee0ee0e000e000e000eee0eeeeee00000000
ee0e0eeeeee0ee0ee0e00e00e0e0eeee000000000000000000000000000000000000000000000000e0ee0e0ee0eeee0ee0e0eee0eee0eeeee0eeeeee00000000
eee0eeeeeee0ee0ee0e0e0e0e000eeee000000000000000000000220000000000000000000000000e0eeee0ee000ee0000e0eee0eee0eeeee0eeeeee00000000
ee0e0eeee0e0ee0ee0e0eee0e0eeeeee000000000000000000000082000000000000000000000000e0eeee0ee0ee0e0ee0e0eee0eee00eeee0eeeeee00000000
ee0e0eeeee0eeee00ee0eee0e0eeeeee000000002000000000000088820000000000000000000000e0eeee0ee0ee0e0ee0e0eee0eee0eeeee0eeeeee00000000
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee000000000200000000000088880000000000000000000000ee0000eee000eee00ee0eee0eee000e00000eeee00000000
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee000000000082000000000088882000000000000000000000eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee00000000
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee000020000028200000000288888200000000000000000000000000000000000000000000000000000000000000000000
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee000288000002800000000888888800000000000000000000000000000000000000000000000000000000000000000000
ee0e0eeeee0eeee00ee0eee0e0eeeeee000888800000282000002888888820000000000000000000000000000000000000000000000000000000000000000000
ee0e0eeee0e0ee0ee0e0eee0e0eeeeee000888880000028200028888888880000000000000000000000000000000000000000000000000000000000000000000
eee0eeeeeee0ee0ee0e0e0e0e000eeee002888888000002820288888888880000000000000000000000000000000000000000000000000000000000000000000
ee0e0eeeeee0ee0ee0e00e00e0e0eeee008888888800000288888888888882000000000000000000000000000000000000000000000000000000000000000000
ee0e0eeee0000e0ee0e0eee0e000eeee008888888880000008888888888888000000000000000000000000000000000000000000000000000000000000000000
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee008888888888000808888888888888000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000008888888888888808888888888888000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000008888888888888808888888888888000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000008888888200000000000028888882000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000002888820020000000000000288880000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000888800080000000000800088880000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000888802888200000008080088820000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000288800080000800800800088800000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000088800020008008000000088800000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000028820000000000000000288200000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000002882002888888882002882000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000288888888888888888800000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000028888888888888882000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000288888888888200000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000288888820000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
31313131313131313131313131313131101010101010101010101010101010101010101010101010101010101010105231313131313131313131313131313131
31313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131
31411010121010101210102210105131101010101010101010101010101010101010101010101010101010101010523131101010101010101010101010101031
31101010101010101010101010101031311010101010101010101010101010313110101010101010101010101010103131101010101010101010101010101031
31101010101010101010101010101031101010101010101010101010101010101010101010101010101010101052313131101010101010101010101010101031
31101010101010101010101010101031311010101010101010101010101010313110101010101010101010101010103131101010101010101010101010101031
311010101010a110a110a110a1101031101010101010101010101010101010101010101010101010101010105231313131101010101010101010101010101031
31101010101010101010101010101031311010101010101010101010101010313110101010101010101010101010103131101010101010101010101010101031
31a1a110101010101010101010101031101010101010101010101010101010101010101010101010101011523131313131101010101010101010101010101031
31101010101010101010101010101031311010101010101010101010101010313110101010101010101010101010103131101010101010101010101010101031
311010101010101010101010101010f0101010101010101010101010101010101010101010101010105231313131313131101010101010101010101010101031
31101010101010101010101010101031311010101010101010101010101010313110101010101010101010101010103131101010101010101010101010101031
311010101010101010101010101010f0101010101010101010101010101010311010101010101010103131313131313131101010101010101010101010101031
31101010101010101010101010101031311010101010101010101010101010313110101010101010101010101010103131101010101010101010101010101031
312010102101101010102110101010f0102010102110101010101010100110f01020100110101010103131313131313131101010101010101010101010101031
31101010101010101010101010101031311010101010101010101010101010313110101010101010101010101010103131101010101010101010101010101031
327000513232323232412200513200f0007000002200000000000000000200f0007000020000000000000000000000f032000000000000000000000000000032
32000000000000000000000000000032320000000000000000000000000000323200000000000000000000000000003232000000000000000000000000000032
320000005132323241000000003200f000000000000000000000000000000032000000000000000000000000000000f032000000000000000000000000000032
32000000000000000000000000000032320000000000000000000000000000323200000000000000000000000000003232000000000000000000000000000032
320000000022001200000000003200f000000000000000000000000000000000000000000000000000000000000000f032000000000000000000000000000032
32000000000000000000000000000032320000000000000000000000000000323200000000000000000000000000003232000000000000000000000000000032
3200000000000000000000000032003200000000000000000000000000000000000000000000000000000000000000f032000000000000000000000000000032
32000000000000000000000000000032320000000000000000000000000000323200000000000000000000000000003232000000000000000000000000000032
3200000000000000000000000022003200000000000000000000000000000000000000000000000000000000000000f032000000000000000000000000000032
32000000000000000000000000000032320000000000000000000000000000323200000000000000000000000000003232000000000000000000000000000032
3200000000000000000000000000003200000000000000000000000000000000000000000000000000000000000000f032000000000000000000000000000032
32000000000000000000000000000032320000000000000000000000000000323200000000000000000000000000003232000000000000000000000000000032
3242000011000000110000210000523200000000000000000000000000000000000000000000000000000000000000f032000000000000000000000000000032
32000000000000000000000000000032320000000000000000000000000000323200000000000000000000000000003232000000000000000000000000000032
3232323232323232323232323232323200000000000000000000000000000000000000000000000000000000000000f032323232323232323232323232323232
32323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232
31313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131
31313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131
31101010101010101010101010101031311010101010101010101010101010313110101010101010101010101010103131101010101010101010101010101031
31101010101010101010101010101031311010101010101010101010101010313110101010101010101010101010103131101010101010101010101010101031
31101010101010101010101010101031311010101010101010101010101010313110101010101010101010101010103131101010101010101010101010101031
31101010101010101010101010101031311010101010101010101010101010313110101010101010101010101010103131101010101010101010101010101031
31101010101010101010101010101031311010101010101010101010101010313110101010101010101010101010103131101010101010101010101010101031
31101010101010101010101010101031311010101010101010101010101010313110101010101010101010101010103131101010101010101010101010101031
31101010101010101010101010101031311010101010101010101010101010313110101010101010101010101010103131101010101010101010101010101031
31101010101010101010101010101031311010101010101010101010101010313110101010101010101010101010103131101010101010101010101010101031
31101010101010101010101010101031311010101010101010101010101010313110101010101010101010101010103131101010101010101010101010101031
31101010101010101010101010101031311010101010101010101010101010313110101010101010101010101010103131101010101010101010101010101031
31101010101010101010101010101031311010101010101010101010101010313110101010101010101010101010103131101010101010101010101010101031
31101010101010101010101010101031311010101010101010101010101010313110101010101010101010101010103131101010101010101010101010101031
31101010101010101010101010101031311010101010101010101010101010313110101010101010101010101010103131101010101010101010101010101031
31101010101010101010101010101031311010101010101010101010101010313110101010101010101010101010103131101010101010101010101010101031
32000000000000000000000000000032320000000000000000000000000000323200000000000000000000000000003232000000000000000000000000000032
32000000000000000000000000000032320000000000000000000000000000323200000000000000000000000000003232000000000000000000000000000032
32000000000000000000000000000032320000000000000000000000000000323200000000000000000000000000003232000000000000000000000000000032
32000000000000000000000000000032320000000000000000000000000000323200000000000000000000000000003232000000000000000000000000000032
32000000000000000000000000000032320000000000000000000000000000323200000000000000000000000000003232000000000000000000000000000032
32000000000000000000000000000032320000000000000000000000000000323200000000000000000000000000003232000000000000000000000000000032
32000000000000000000000000000032320000000000000000000000000000323200000000000000000000000000003232000000000000000000000000000032
32000000000000000000000000000032320000000000000000000000000000323200000000000000000000000000003232000000000000000000000000000032
32000000000000000000000000000032320000000000000000000000000000323200000000000000000000000000003232000000000000000000000000000032
32000000000000000000000000000032320000000000000000000000000000323200000000000000000000000000003232000000000000000000000000000032
32000000000000000000000000000032320000000000000000000000000000323200000000000000000000000000003232000000000000000000000000000032
32000000000000000000000000000032320000000000000000000000000000323200000000000000000000000000003232000000000000000000000000000032
32000000000000000000000000000032320000000000000000000000000000323200000000000000000000000000003232000000000000000000000000000032
32000000000000000000000000000032320000000000000000000000000000323200000000000000000000000000003232000000000000000000000000000032
32323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232
32323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232
__label__
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
06000000000000000000000006600000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
06000666060606660600000000600000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
06000660060606600600000000600000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
06000600066606000600000000600000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
06660666006006660666000006660000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000066666666660000000000000006000000000000000600000000000000066666666666666660000000066666666000000000000000000000000
00000000000000666666666666000000000000006600000000000000660000000000000066666666666666600000000066666666000000000000000000000000
00000000000006666666666666600000000000006660000000000000666000000000000066666666666666000000000066666666000000000000000000000000
00000000000066666666666666660000000000006666000000000000666600000000000066666666666660000000000066666666000000000000000000000000
00000000000666666666666666666000000000006666600000000000666660000000000066666666666600000000000066666666000000000000000000000000
00000000006666666666666666666600000000006666660000000000666666000000000066666666666000000000000066666666000000000000000000000000
00000000066666666666666666666660000000006666666000000000666666600000000066666666660000000000000066666666000000000000000000000000
00000000666666666666666666666666000000006666666600000000666666660000000066666666600000000000000066666666000000000000000000000000
00000000666666660000000066666666000000006666666600000000666666660000000066666666000000000000000066666666000000006000000000000000
00000000666666660000000066666666000000006666666600000000666666660000000066666666000000000000000066666666000000006600000000000000
00000000666666660000000066666666000000006666666600000000666666660000000066666666000000000000000066666666000000006660000000000000
00000000666666660000000066666666000000006666666600000000666666660000000066666666000000000000000066666666000000006666000000000000
00000000666666660000000066666666000000006666666600000000666666660000000066666666000000000000000066666666000000006666600000000000
00000000666666660000000066666666000000006666666600000000666666660000000066666666000000000000000066666666000000006666660000000000
00000000666666660000000066666666000000006666666600000000666666660000000066666666000000000000000066666666000000006666666000000000
00000000666666660000000066666666000000006666666600000000666666660000000066666666000000000000000066666666000000006666666600000000
00000000666666666666666666666666000000006666666666666666666666660000000066666666000000000000000066666666666666666666666600000000
00000000066666666666666666666660000000006666666666666666666666600000000066666666000000000000000006666666666666666666666000000000
00000000006666666666666666666600000000006666666666666666666666000000000066666666000000000000000000666666666666666666660000000000
00000000000666666666666666666000000000006666666666666666666660000000000066666666000000000000000000066666666666666666600000000000
00000000000066666666666666660000000000006666666666666666666600000000000066666666000000000000000000006666666666666666000000000000
00000000000006666666666666600000000000006666666666666666666000000000000066666666000000000000000000000666666666666660000000000000
00000000000000666666666666000000000000006666666666666666660000000000000066666666000000000000000000000066666666666600000000000000
00000000000000066666666660000000000000006666666666666666600000000000000066666666000000000000000000000006666666666000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000066666666660606060666666666666666666666666666666666666666660000000000000000000000000000000000000000000000660666066
00000000000000666666666666606066666666666666666666666666666666666666666666000000000000000000000000000000000000000000006606660666
00000000000006666666666666606606666666666666666666666666666666666666666666600000000000000000000000000000000000000000066666606660
00000000000066666666666666606666666666666666666666666666666666666666666666660000000000000000000000000000000000000000666666066606
00000000000666666666666666666666666666666666666666666666666666666666666666666000000000000000000000000000000000000006666660666066
00000000006666666666666666666666666666666666666666666666666666666666666666666600000000000000000000000000000000000066666606660666
00000000066666666666666666666666666666666666666666666666666666666666666666666660000000000000000000000000000000000666666666606660
00000000666666666666666666666666666666666666666666666666666666666666666666666666000000000000000000000000000000006666666666066606
00000000666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666660666066
00000000666666666666666666666666666066666606660600006066060006666666066666666666666666666666666666666666666666666666666606660666
00000000666666666666666666666666660066666600600606606066060666666666006666666666666666666666666666666666666666666666666666606660
00000000666666666666666666666666600000066606060606606066060066666000000666666666666666666666666666666666666666666666666666066606
00000000666666666666666666666666660066666606660606606066060666666666006666666666666666666666666666666666666666666666666660666066
00000000666666666666666666666666666066666606660600006600660006666666066666666666666666666666666666666666666666666666666606660666
00000000666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666606660
00000000666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666066606
00000000666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666660666066
00000000066666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666606660666
00000000006666666660066666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666606660
00000000000666666600006666666666660666666666666666666666666666666666666666666666666666666606666666666666666666666666666666066606
00000000000066666000000666666666660666666666666666666666666666666660666666666666666666666606666666666666666666666666666660666066
00000000000006660600006066666666660606666666666666666666666666666660666666666666666666666606066666666666666666666666666606660666
00000000000000666606606666666666660606666666666666666666666666666060666666666666666666666606066666666666666666666666666666606660
00000000000000066606606666666666660606066666666666666666666666666060606666666666666666666606060666666666666666666666666666066606
55555555555555500050050000000000005050500000000000000000000000000505050000000000000000000050505000000000000000000000000000600060
55555555555555000050050000000000005050000000000000000000000000000505000000000000000000000050500000000000000000000000000000060006
55555555555550005055550500000000005050000000000000000000000000000005000000000000000000000050500000000000000000000000000060006000
55555555555500000555555000000000005000000000000000000000000000000005000000000000000000000050000000000000000000000000000006000600
55555555555000000055550000000000005000000000000000000000000000000000000000000000000000000050000000000000000000000000000000600060
55555555550000000005500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000060006
55555555500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000060006000
55555555000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000006000600
55555555000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000600060
55555555000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000060006
55555555000000000000000000000000000500000050005055550055005550000000500000000000000000000000000000000000000000000000000060006000
55555555000000000000000000000000005500000050005050050500505000000000550000000000000000000000000000000000000000000000000006000600
55555555000000000000000000000000055555500050505050050500505500000555555000000000000000000000000000000000000000000000000000600060
55555555000000000000000000000000005500000055055050050500505000000000550000000000000000000000000000000000000000000000000000060006
55555555000000000000000000000000000500000050005055550500505550000000500000000000000000000000000000000000000000000000000060006000
55555555000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000006000600
55555555000000000000000000000000000000000000000000000000000000000000000000000000555555555555555555555555555555550000000000600060
55555555500000000000000000000000000000000000000000000000000000000000000000000005555555555555555555555555555555555000000000060006
55555555550000000000000000000000000000000000000000000000000000000000000000000055555555555555555555555555555555555500000060006000
55555555555000000000000000000000000000000000000000000000000000000000000000000555555555555555555555555555555555555550000006000600
55555555555500000000000000050000000000000000000000000000000000000000000000005555555555555555555555555555555555555555000000600060
55555555555550000000000000050050000000000000000000000000000000000000000000055555555555555555555555555555555555555555500000060006
55555555555555000000000000050500000000000000000000000000000000000000000000555555555555555555555555555555555555555555550060006000
55555555555555500000000005050505000000000000000000000000000000000000000005555555555555555555555555555555555555555555555006000600
55555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555
55555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555
55555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555
55555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555
55555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555
55555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555
55555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555
55555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555
55555555555555500000000005555555555555550000000000000000055555555555555500000000555555555555555555555550000000000555555555555555
55555555555555000000000000555555555555550000000000000000005555555555555500000000555555555555555555555500000000000055555555555555
55555555555550000000000000055555555555550000000000000000000555555555555500000000555555555555555555555000000000000005555555555555
55555555555500000000000000005555555555550000000000000000000055555555555500000000555555555555555555550000000000000000555555555555
55555555555000000000000000000555555555550000000000000000000005555555555500000000555555555555555555500000000000000000055555555555
55555555550000000000000000000055555555550000000000000000000000555555555500000000555555555555555555000000000000000000005555555555
55555555500000000000000000000005555555550000000000000000000000055555555500000000555555555555555550000000000000000000000555555555
55555555000000000000000000000000555555550000000000000000000000005555555500000000555555555555555500000000000000000000000055555555
55555555000000005555555500000000555555550000000055555555000000005555555500000000555555555555555500000000555555550000000055555555
55555555000000005555555500000000555555550000000055555555000000005555555500000000555555555555555500000000555555555000000055555555
55555555000000005555555500000000555555550000000055555555000000005555555500000000555555555555555500000000555555555500000055555555
55555555000000005555555500000000555555550000000055555555000000005555555500000000555555555555555500000000555555555550000055555555
55555555000000005555555500000000555555550000000055555555000000005555555500000000555555555555555500000000555555555555000055555555
55555555000000005555555500000000555555550000000055555555000000005555555500000000555555555555555500000000555555555555500055555555
55555555000000005555555500000000555555550000000055555555000000005555555500000000555555555555555500000000555555555555550055555555
55555555000000005555555500000000555555550000000055555555000000005555555500000000555555555555555500000000555555555555555055555555
55555555000000000000000000000000555555550000000055555555000000005555555500000000055555555555555500000000555555555555555555555555
55555555500000000000000000000005555555550000000555555555000000055555555500000000005555555555555500000000555555555555555555555555
55555555550000000000000000000055555555550000005555555555000000555555555500000000000555555555555500000000555555555555555555555555
55555555555000000000000000000555555555550000055555555555000005555555555500000000000055555555555500000000555555555555555555555555
55555555555500000000000000005555555555550000555555555555000055555555555500000000000005555555555500000000555555555555555555555555
55555555555550000000000000055555555555550005555555555555000555555555555500000000000000555555555500000000555555555555555555555555
55555555555555000000000000555555555555550055555555555555005555555555555500000000000000055555555500000000555555555555555555555555
55555555555555500000000005555555555555550555555555555555055555555555555500000000000000005555555500000000555555555555555555555555
55555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555
55555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555
55555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555
55555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555
55555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555
55555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555
55555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555
55555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555

__gff__
0000000000000000000000000000000000000001040400000000000000000000000000010808000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
__map__
131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313130f0f0f13131313131313131313
1314001513151315130025130013131313131313131313131313131313131313131313131313140101151313131313131313131313131313131313131313131313142001151313131313131401221513131401012201010101200120010115131301122020010101010120012001131313131311010101200101010101220113
1300130013001300130013130013151313140121011513131313131313131313131313140101010101010122011513131313142001010115131314010101010f0101010101011513131401010101010f0101010101010101010101010101010f0101130101011301010101010101011313131313010101010117010101010113
1324002513000025130013132400251313010101010113131313131313131313131422010130013132333401010113131313010101010101200101010112010f0201010101010120010101010101010f0212010101010101010101010101010f0201010101011313130101010101131313131313010101010101010101010113
1313131313131313131313131313131314010101010115131313131313131313140101010101010101011101010115131314010101010101010101010113131313130101010101010101010101010113131301010101171301010101010101131301010113131313130101011301221313131314010101010101010101010113
1314012201010101011513131313140f0101014041424301010120010101010f0101010101010101012513240101010f0101010101010101010101010101151313130101010101010101010101010113131301010101012201010101010101131301010101131313130101011301131313142101010101010101010101010113
130101010d1d1e1f0e0101010101010f0101010101010101010101010101010f0101010101010101011313132401010f0101011001010112010101010101011313130101010101010101010101010113131301010101010101010101010101131301252401131313140125131301010f02010101010101010101010101010113
1324020110010101110101100101010f0102251324010101120101010101100f1101021201011101251313131301010f0102251316161613240101010101011313131616161616161616161616161613131316161616161616161616161616131301131301220101010113131301010f01010101011201011001011101111013
2314070020000000210000200000000f0007152314000000220000000000200f2100072323232300002200000000000f0007152326262623232324000000002323232626262626262626262626262623232326262626262626262626262626232300232300120000000023232300000f00000000002200002000002100232323
230000003d2d2e2f3e0000000000000f0000000000000000000000000000000f0000001523232300000000000000000f0000002000000022152323232324002323230000000000000000000000000023232300000000000000000000000000232300151400232323240015231400000f07000000000000000000000000152323
2324001200000000002523232323240f0000005051525300000010000000000f0000000023231400000000000000000f0000000000000000002323141523232323230000000000000000000000000023232300000000001200000000000000232300000000000000230012000000232323241100000000000000001100001523
2323232323232323232323232323232324000000000025232323232323232323240000000000000000000000000025232324000000000000000022000023232323230000000000002700000000000023232300000000002300002700000000232323240000000000232323002300122323232324000000000000002323000023
231400152300001523002323140015232300000000002323232323232323232323241200004a004b4c4d4e00000023232323000000000000100000000022000f0700000000000010000000000000000f0722000000000000000000000000000f0700230000000000000000000000232323232323000000000000000000000023
2300230023002300230023230023242323240011002523232323232323232323232323240000000000000012002523232323241000000025232324000000000f0000000000002523232400000000000f0000000000000000000000000000000f0000230000002300232300000000002323232323000000000000000000000023
2324002523252325230015230123232323232323232323232323232323232323232323232323240000252323232323232323232323232323232323232323232323241000252323232323232400122523232400001200000000100010000025232300221010000000232310001000232323232321000000100000000000120023
232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323230f0f0f23232323232323232323
131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313130101011313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313131313130f0f0f0f131313131313131313
13131313131314010101010101011513131401200101010101151422010115131313140101200101010122010120010f01012013010101130122010122011513130122010101220101012001010101131313131313140120010120011513131313131313142a22010101012a0101010f0101011a01011a1a011a010101011513
1313131313140101010101010101010f0101010101010101010101010101010f0101010101010101010101010101010f02010113010101132a2a1313010101131301011a1a1a1a1a01010101011a1a131313131401010101010101010115131313131420012a01010101012a0101010f0101011a01011a01011a011a01010113
1313131401010101010101010112010f02010101010101010101011a0101010f100201010101010101010101012b0113131a01132401251301011314010101131301011a0101011a0101011a011a1a131313140101010101010101010101151313130101012a01010101012a0101010f0201011a01011a01011a011a01010113
131401010101010101010101011313131313011a01010101010101010101131313130101010101012b010101010101131301011313131313010113010101011313010101011a011a01010101010101131314010212010101010101010101011313140101012a01010101012a010113131324011a010101010101011a01010113
1301010101010101010101010115131313140101010101011a0101010101151313130101010101010101012a010101131301011513131313010113010101011313010101011a1a1a010101011a011a131301011313013a010101013a0101010f0f010101012a0101013a0101010115131313011a0101011a01011a1a01010113
13010101010101010101010101012213130101010101010101010101010101131314010101012a01010101010101011313010101012201012a2a13240101010f0f0101010101010101011a011a01010f0f01251313010101010101010101120f0f02013a0101010101010101010101131313011a1a1a1a1a1a1a1a1a01010113
1302011001010101110101010101011313161616161616161616161616161613131616161616161616161616161616131301100101010112010113130112010f0f0201101a1a0101011201011a1a010f0f0113131316161616161616161313131313161616161616161616161616161313130101010111010101010112012513
23071a2000000000210000000000002323262626262626262626262626262623232626262626262626262626262626232300200000000022000023230022000f0f071a200000001a1a2200000000000f0f0023232326262626262626262323232323262626262626262626262626262323230000000021000000000022001523
230000001a000000000000000000122323000000000000000000000000000023230000002b00000000000000000000232300000000120000000023140000000f0f001a000000000000001a000000000f0f00152323000000000000000000220f0f07000000003b0000000000000000232323000000000000000000002b2b0023
230000000000001a000000000025232323240000000000000000000000002523230000000000000000000000000000232300002523232323000023000000002323000000001a1a1a001a1a0000001a2323000023230000003b3b00000000000f0f00000000000000000000000000252323230000000000000000000000000023
232400000000000000001a000023232323230000001a000000000000001a232323002a0000000000000000000000002323000023232323232b2b230000000023231a0000001a001a001a0000000000232324000722000000000000000000002323240000000000002b0000003b00232323140000002b000000000000002a2a23
2323232400000000000000000022000f0700000000000000000000000000000f20070000000000000000000000000023231a00231400152300002324000000232300001a00000000001a001a000000232323240000000000000000000000252323230000000000002b00000000002b0f07000000000000002a00000000000023
2323232323240000000000000000000f0000000000000000000000000000000f0000000000000000000000000000000f070000230000002300002323000000232300001a0000000000000000000000232323232400000000000000000025232323232410000000002b00000000002b0f000000002a000000000000002b2b0023
23232323232324000000000000002523232400100000000000252412000025232323240000100000000012000010000f00001023000000230012000012002523230012000000120000001000000000232323232323240010000010002523232323232323240012002b00001200002b0f00000000000000000000000000000023
232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323230000002323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323230f0f0f0f232323232323232323
__sfx__
001800001010310153101031015316105101031015310073101031015316105100731015316100100731015310103100731610510153001001610010073101531010310153161001015310153100731015310153
001800200f305101531010310153346253460510153100730a3051015334605100731015316100100731015305305100730530510153346243462510073101530a3051015311305101530f305100731015310153
011800200a3350a3350f4340a4350f3350f3350c4340f4350c3350c335074340c4350a33505335074350a4350a3350a3350f4340a4350f3350f3350c4340f4350733507335074340c4350a3350c335074350a435
011800100a3350a3370f3350a3320f3310f3370c3350f3320c3310c337073350c3320a33105337073350a33500305003050030500305003050030500305003050030500305003050030500305003050030500305
011800201133213332114320c3350a4360a3360f4350f3310c4320c3320f432113350c4360a3360a4350f431163321643216332134350a33605436073350a4310c33216432163320f43507336074360a3350a431
01180000072350a2350a2350a2350523505235052350a2350a2350a2350723505235052350a2350a2350723507235072350a2350a23507235072350a2350723505235072350723505235052350a2350723507235
001800200f305101531010310153346253460510153100730a3051015334605100731015316100100731015305305100730530510153346243462510073101530a3051015311305101530f305100731015310153
011800200f10211152101021115211152101021115211152161020c152101020c1520c152161020c1520c152111020f152111020f1520f1520f1020f1520f152161020a152111020a1520f1020a1020a1520a152
011800200c175111050c17511105111050c1750c105111050f1750c1050f1750c1050c1050f1750c1050c105161750f105161750f1050f10516175161050f1050717516105071751610507175071751610516105
0118002011775107051177511775107051177511775167050c775107050c7750c775167050c7750c775117050f775117050f7750f7750f7050f7750f775167050a775117050a7750f7050a7050a7750a7750a705
011800200f30511355103051135511355103051135511355163050c355103050c3550c355163050c3550c355113050f355113050f3550f3550f3050f3550f355163050a355113050a3550f3050a3050a3550a355
0118002011155101051115511155101051115511155161050c155101050c1550c155161050c1550c155111050f155111050f1550f1550f1050f1550f155161050a155111050a1550f1050a1050a1550a1550a105
01180000071050a1050a1640a1650510005100051640a1650a1000a1000716405165051000a1000a1640716507100071000a1640a16507100071000a1640716505100071000716405165051000a1000716407165
01180000070550a0550a0050a0050505505055050050a0050a0550a0550700505005050550a0550a0050700507055070550a0050a00507055070550a0050700505055070550700505005050550a0550700507005
0118002011365103051136511365103051136511365163050c365103050c3650c365163050c3650c365113050f365113050f3650f3650f3050f3650f365163050a365113050a3650f3050a3050a3650a3650a305
011800200f20511245102051124511245102051124511245162050c245102050c2450c245162050c2450c245112050f245112050f2450f2450f2050f2450f245162050a245112050a2450f2050a2050a2450a245
011000001611516105001050010500105001050010500105001050010500105001050010500105001050010500105001050010500105001050010500105001050010500105001050010500105001050010500105
010600001d310133100a3100531003310033100a30000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
011800201110213102111340c1350a1060a1060f1340f1350c1020c1020f134111350c1060a1060a1340f135161021610216134131350a10605106071340a1350c10216102161340f13507106071060a1340a135
01180020113251332113073130730a3250a32137615130730c3250c32113073130730c3250a3213761513073163251632113073130730a3250532137615130730c32516321130731307307325073213761437615
0118002011175101051117511105111721117511105111751817518105181750c10518172181750c105181750a1750a1050a175111050a1720a1750f1050a1751317513105131750f10513172131750a10513175
0118002011175101051117511105111721117511105111751817518105181750c10518172181750c105181750a1750a1050a175111050a1720a1750f1050a1751317513105131750f10513172131750a10513175
011800201114511145111051114511105111451114511105111451114511105111451110511145111451110511145111451110511145111051114511145111051114511145111051114511105111451114511105
011800200a3350a3050f4350a4050f3350f3050c4350f4050c3350c305074350c4050a33505305074350a4050a3350a3050f4350a4050f3350f3050c4350f4050733507305074350c4050a3350c305074350a405
011800201332513305163250c3251b3050f3251b3050c3251632516305163251632513305133251330516325183251d305163251632522305163251f30513325133251d305113250f325183050c3251630516325
01180020224251f4251f4052242518425274051b4252740518425224252240522425224251f4051f4251f40522425244252940522425224252e405224252b4051f4251f425294051d4251b425244051842516405
__music__
01 00424344
00 01174344
00 01024344
00 01034344
00 01430444
00 06450344
00 06074544
00 06450844
00 0a090044
00 0a090844
00 00090844
00 00090a44
00 010e0f44
00 010e0f44
00 12010744
00 010e4644
00 01140f44
00 01140e44
00 01151644
02 010f5644
00 414f5644

