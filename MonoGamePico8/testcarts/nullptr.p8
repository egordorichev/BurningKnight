pico-8 cartridge // http://www.pico-8.com
version 21
__lua__
-- nullptr
-- by @egordorichev

cartdata("nullptr")
poke(24365,1)

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
 record=dget(0)
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
 
 g_time=0
 g_coins=0
 entity_reset()
 collision_reset()
 
 --[[g_level=e_add(level({
  base=v(g_index%8*16,flr(g_index/8)*16),
  size=v(16,16)
 }))]]
 
 e_add(guy({
  pos=v(60,96)
 }))
 
 for i=1,20 do
  e_add(star())
 end
 
 music(0)
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
 local frm_index=flr((e.gt and g_time or e.t)/delay) % #frames + 1
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

star=entity:extend({
 draw_order=0
})

local starpal={0,1,5}

function star:init()
 self.pos=v(
  rnd(128),rnd(128)
 )
 
 self.depth=rnd(#starpal)
end

function star:idle()
 self.lx=self.pos.x
 self.ly=self.pos.y
 
 self.pos.y+=self.depth*0.5
 
 if self.ly>128 then
  self.pos.y=-6
  self.ly=self.pos.y
  self.pos.x=rnd(128)
  self.lx=self.pos.x
 end
end

function star:render()
 if not self.lx then
  return
 end

 line(self.pos.x,self.pos.y,self.lx,self.ly,
  starpal[flr(self.depth)+1])
end

bullet=entity:extend({
 sprite={
  idle={14,13,15,delay=5}
 },
 gt=true,
 draw_order=4,
 hitbox=box(0,0,4,4),
 collides_with={"guy"},
 tags={"bullet"}
})

function bullet:init()
 sfx(17)
end

function bullet:collide(o)
 if (self.done or o.done) return
 o:hurt()
 self.done=true
end

function bullet:idle()
 if self.pos.y>=132 then
  self.done=true
 end
end


shot=entity:extend({
 sprite={
  idle={29,30,delay=5}
 },
 gt=true,
 draw_order=5,
 hitbox=box(0,0,2,5),
 collides_with={"mob"}
})

function shot:collide(o)
 o:hurt()
 self.done=true
end

function shot:idle()
 if self.pos.y<=-5 then
  self.done=true
 end
end

pickup=entity:extend({
 hitbox=box(0,0,8,8),
 collides_with={"guy"}
})

function pickup:init()
 self.vel=v(0,0.5)
 self.sprite={idle={self.tile}}
end

local clrs={0,3,11,3}
function pickup:render()
 local x,y=self.pos.x,self.pos.y
 

 local rn=clrs[flr(g_time/10)%#clrs+1]
 for i=1,15 do pal(i,rn) end
 palt(0,true)
 
 for xx=-2,2 do
  for yy=-2,2 do
   if(abs(xx)+abs(yy)==2)spr(self.tile,x+xx,y+yy)
  end
 end
 
 for i=1,15 do pal(i,0) end
 palt(0,false)
 
	for xx=-1,1 do
  for yy=-1,1 do
   if(abs(xx)+abs(yy)==1)spr(self.tile,x+xx,y+yy)
  end
 end
 
 r_reset()
 spr(self.tile,x,y)
end

function pickup:idle()
 if rnd()<0.1 then
  e_add(part({
   pos=v(self.pos.x+4,self.pos.y+4),
   vel=v(rnd(1)-0.5,-0.2),
   r=4,
   c=clrs[flr(rnd(#clrs))+1]
  }))
 end
 
 self.pos.x+=cos(self.t/50)*0.4
 
 if self.pos.y>=132 then
  self.done=true
 end
end

function pickup:collide()
 if(self.done) return
 self.done=true
 shk=6
 
 for i=1,20 do
  e_add(part({
   pos=v(self.pos.x+4,self.pos.y+4),
   vel=v(rnd(8)-4,rnd(8)-4),
   r=4,
   c=rnd(8)+7
  }))
 end
 sfx(16)
 self:oncol()
end

mushroom=pickup:extend({
 tile=68
})

function mushroom:oncol()
 if(g_guy.state=="idle")g_guy:become("big")
end

gmushroom=pickup:extend({
 tile=73
})

function gmushroom:oncol()
 g_guy:become("ship")
end

bomb=pickup:extend({
 tile=74
})

function bomb:oncol()
 if entities_tagged["mob"] then
	 for m in all(entities_tagged["mob"]) do
	  m:die()
	 end
 end
 
 if entities_tagged["bullet"] then
	 for b in all(entities_tagged["bullet"]) do
	  b.done=true
	 end
 end
end


fireflower=pickup:extend({
 tile=71
})

function fireflower:oncol()
 g_guy:become("fire")g_guy.fireal=180
end

starp=pickup:extend({
 tile=70
})

function starp:oncol()
 g_guy.inv=300
end

coin=pickup:extend({
 tile=69
})

function coin:oncol()
 g_coins+=1
end
-->8
-- guy

guy=entity:extend({
 sprite={
  idle={64,65,66,67,delay=10},
  big={1,2,3,4,delay=15},
  fire={80,81,82,83,delay=15},
  ship={86}
 },
 hitbox={
  idle=box(2,1,5,5),
  big=box(1,1,7,7),
  fire=box(1,1,7,7),
  ship=box(0,0,8,8)
 },
 tags={"guy"}
})

function guy:init()
 self.vel=v(0,0)
 self.ac=v(0,0)
 g_guy=self
 self.hrt=0
 self.fireal=0
 self.inv=0
 self.lpt=0
end

function guy:hurt()
 --if(true) return
 
 if(self.hrt>0 or self.inv>0)return
 
 shk=10
 sfx(29)
  
 if self.state~="idle" then
  self:become("idle")
  self.hrt=60
  return
 end

 if(self.done) return
 self.done=true
 
 current=g_time
 record=max(record,g_time)
 music(-1)
 sfx(30)
 g_time=0
 --restart_level()
 dset(0,record)
 
 local am=8
 for i=1,am do
 local s=0.5
  local a=i/am
  e_add(bullet({
	  pos=v(self.pos.x+2,self.pos.y+2),
	  vel=v(cos(a)*s,sin(a)*s)
	 }))
 end
end

function guy:render()
 local x,y=self.pos.x+4,self.pos.y+4
 if self.ac.x~=0 then
  line(x,y,x+self.ac.x*-8,y,10)
  line(x,y,x+self.ac.x*-6,y,9)
  
  if rnd()<0.2 then
   e_add(part({
	   pos=v(x,y),
	   vel=v(self.ac.x*-0.5,rnd(0.5)-0.2),
	   r=3,
	   c=rnd()>0.7 and 10 or 4
	  }))
  end
 end
 
 if self.ac.y~=0 then
  line(x,y,x,y+self.ac.y*-8,10)
  line(x,y,x,y+self.ac.y*-6,9)
  
  if rnd()<0.2 then
   e_add(part({
	   pos=v(x,y),
	   vel=v(rnd(0.5)-0.2,self.ac.y*-0.5),
	   r=3,
	   c=rnd()>0.7 and 10 or 4
	  }))
  end
 end

 if self.inv>0 then
	 local r=12+cos(g_time/100)*4
	 local am=10
	 local rn=g_time/10%8+7
	 
	 for i=1,am do
	  local a=i/am+t()*0.5
	  local x,y=self.pos.x+4+cos(a)*r,
	   self.pos.y+4+sin(a)*r
	   
	  circfill(x,y,2,0)
	  circ(x,y,1,rn)
	 end
	 
	 if entities_tagged["mob"] then
	  for e in all(entities_tagged["mob"]) do
	   if (e.pos-self.pos):len()<=r+4 then
	    e:die()
	   end
	  end
	 end
	 
	 if entities_tagged["bullet"] then
	  for e in all(entities_tagged["bullet"]) do
	   if (e.pos-self.pos):len()<=r+2 then
	    e.done=true
	   end
	  end
	 end
  for i=1,15 do pal(i,rn) end self.hrt-=1
  self.inv-=1
  if self.inv<60 and self.inv%10<5 then return end
  if(self.invt==0) self.hrt=15
 elseif self.hrt>0 then
  for i=1,15 do pal(i,7) end self.hrt-=1
  if self.hrt%10<5 then return end
 end
 
 spr_render(self)
end

function guy:big()
 self:idle()
end

function guy:idle()
 local s=self.state=="ship" and 1/8 or 0.05
 self.vel*=0.93
 self.ac=v(0,0)
 
 if(btn(⬅️)) self.vel.x-=s self.ac.x=-1
 if(btn(➡️)) self.vel.x+=s self.ac.x+=1
 if(btn(⬆️)) self.vel.y-=s self.ac.y=-1
 if(btn(⬇️)) self.vel.y+=s self.ac.y+=1
 
 if self.pos.x<0 then
  self.pos.x=0
  self.vel.x=max(0,self.vel.x)
 elseif self.pos.x>120 then
  self.pos.x=120
  self.vel.x=min(0,self.vel.x)
 end
 
 if self.pos.y<0 then
  self.pos.y=0
  self.vel.y=max(self.vel.y,0)
 elseif self.pos.y>120 then
  self.pos.y=120
  self.vel.y=min(self.vel.y,0)
 end 
 
 self.lpt+=1
 if self.lpt>15 then
  self.lpt=0
  e_add(part({
   pos=v(self.pos.x+4,self.pos.y+4),
   vel=v(rnd(0.5)-0.2,2),
   r=4,
   c=rnd()>0.7 and 10 or 9
  }))
 end
end

function guy:fire()
 self:idle()
 
 if btnp(❎) then
  self.vel.y+=0.25
  sfx(27)
  
  e_add(shot({
	  pos=v(self.pos.x+3,self.pos.y+3),
	  vel=v(0,-1.5)
	 }))
	 
	 e_add(part({
   pos=v(self.pos.x+4,self.pos.y),
   vel=v(rnd(0.5)-0.2,-1),
   r=4,
   c=rnd()>0.7 and 10 or 9
  }))
 end
end

function guy:ship()
 self.vel*=0.9
 self:idle()
end
-->8
-- enemies

mob=entity:extend({
 collides_with={"guy"},
 tags={"mob"},
 maxhp=1
})

function mob:init()
 self.hp=self.maxhp
 self.hrt=0
 if(not self.vel)self.vel=v(0,0)
end

function mob:render()
 if(self.hrt>0) for i=1,15 do pal(i,7) end self.hrt-=1
 spr_render(self)
end

function mob:hurt()
 if self.hp<=0 then return end
 self.hp-=1
 shk=2
 self.hrt=15
 
 sfx(28)
  
 if self.hp<=0 then
  self:die()
 end
end

function mob:die()
 shk=6
 self.done=true
 for i=1,10 do
  self.vel.y-=0.25
  e_add(part({
   pos=v(self.pos.x+4,self.pos.y+4),
   vel=v(rnd(2)-1,rnd(2)-1),
   r=rnd(2)+5,
   c=rnd()>0.7 and 8 or 2
  }))
 end
end

function mob:bound()
 if self.pos.y>=132 then
  self.done=true
 end
 
 self.pos.y=mid(self.pos.y,-8,134)
 
 if self.pos.x<-8 or self.pos.x>134 then
  self.done=true
 end
end


function mob:collide(o)
 if o:is_a("guy") then
  if o.inv>0 then
   self:die()
  else
   o:hurt()
  end
 end
end

xwing=mob:extend({
 sprite={
  idle={
   17,18,19,20,delay=15
  },
  attack={21,22,17,18,19,delay=15}
 },
 hitbox=box(0,0,8,8),
 maxhp=5
})

function xwing:idle()
 self:bound()
 if self.tar==nil then
  self.tar=v(rnd(120),rnd(48))
 end
 
 local dx,dy=self.tar.x-self.pos.x,
  self.tar.y-self.pos.y
  
 local d=sqrt(dx*dx+dy*dy)
 
 if d<=5 then
	 self:become("attack")
	 self.tar=nil
	else
	 local s=d*20
	 self.vel+=v(dx/s,dy/s)
	 self.vel*=0.95
 end
end

function xwing:attack()
 self:bound()
	 self.vel*=0.9
	 
 if self.t>=30 and not self.fr then
  self.fr=true
  local vl=1
  
	 e_add(bullet({
	  pos=v(self.pos.x-1,self.pos.y+3),
	  vel=v(0,vl)
	 }))

	 e_add(bullet({
	  pos=v(self.pos.x+5,self.pos.y+3),
	  vel=v(0,vl)
	 }))
	 
	 self.vel.y-=1
 end
 
 if self.t>=60 then
  self:become("idle")
  self.fr=false
 end
end

rocket=mob:extend({
 sprite={
  idle={
   33
  },
  prep={34,35,delay=10},
  fire={34,35,delay=5}
 },
 maxhp=3,
 hitbox=box(1,1,7,6)
})

function rocket:init()
 self.vel=v(0,0.5)
end

function rocket:idle()
 self:bound()
 
 local dx,dy=g_guy.pos.x-self.pos.x,
  g_guy.pos.y-self.pos.y
  
 local d=sqrt(dx*dx+dy*dy)
 
 local s=d*20
 self.vel.x*=0.95
 self.vel.x+=dx/s
 
 if abs(dx)<=4 and self.pos.y>=16 then
  self:become("prep")
  sfx(19)
 end
end

function rocket:prep()
 self.vel*=0.8
 
 if self.t>=30 then
  self:become("fire")
 end
end

function rocket:fire()
 self:bound()
 self.vel.y=min(5,self.vel.y+0.2)

 if self.t%5==0 then
  e_add(bullet({
	  pos=v(self.pos.x+2,self.pos.y+2),
	  vel=v(0,0.5)
	 }))
 end
end

spok=mob:extend({
 sprite={
  idle={
   52,51,50,49,
   delay=15
  },
  attack={53,54,52,51,50,delay=15}
 },
 hitbox=box(0,0,8,8),
 maxhp=2
})

function spok:init()
 self.vel=v(rnd()*0.5-0.25,0.25)
end

function spok:idle()
 self:bound()
 if self.t>=60 then
  self:become("attack")
 end
end

function spok:attack()
 self:bound()
 if self.t>=30 and not self.fr then
  local dx,dy=g_guy.pos.x-self.pos.x,
  g_guy.pos.y-self.pos.y
  
  self.fr=true
  
  local s=0.5
  local a=atan2(dx,dy)
  
  e_add(bullet({
	  pos=v(self.pos.x+2,self.pos.y+2),
	  vel=v(cos(a)*s,sin(a)*s)
	 }))
 end 
 
 if self.t>=60 then
  self.fr=false
  self:become("idle")
 end
end

ufo=mob:extend({
 sprite={
  idle={
   7,8,9,10,delay=10
  },
  attack={
   11,12,7,7,7,delay=15
  }
 },
 maxhp=3,
 hitbox=box(1,1,7,5)
})

function ufo:init()
 self.pass=0
 self.vel=v(0,0.25)
 self.pos.x=rnd(64)+32
end

function ufo:idle()
 self:bound()
 if self.t>=60 then
  self.pass+=1
  self.vel=v(0,0)
  self:become("attack")
 end
end

function ufo:attack()
 if self.t>=30 and not self.fr then
  for i=1,5 do
   local a=i/5+(self.pass%2==0 and 0 or 1/10)
   local s=1
   
	  e_add(bullet({
		  pos=v(self.pos.x+2,self.pos.y+2),
		  vel=v(cos(a)*s,sin(a)*s)
		 }))
  end
  self.fr=true
 end
 
 if self.t>=60 then
  self.fr=false
  self:become("idle")
  local s=0.5
  
  self.vel=v(   
   self.pass%2==1 and
    (self.pass%4<2 and s or -s) or 0,
   self.pass%2==0 and s/2 or 0
  ) 
 end
end

moon=mob:extend({
 sprite={
  idle={
   23,24,25,26,27,delay=10
  },
  attack={28,23,delay=15}
 },
 hitbox=box(0,0,8,8)
})

function moon:init()
 self.ty=rnd(32)+16
 self.bullet=0
end

function moon:idle()
 self:bound()
 if self.tar==nil then
  self.tar=v(rnd(120),self.ty)
 end
 
 local dx,dy=self.tar.x-self.pos.x,
  self.tar.y-self.pos.y
  
 local d=sqrt(dx*dx+dy*dy)
 
 if d<=5 then
	 self:become("attack")
	 self.tar=nil
	else
	 local s=d*20
	 self.vel+=v(dx/s,dy/s)
	 self.vel*=0.95
 end
end

function moon:attack()
 self.vel*=0.8
 self:bound()
	 
 if self.t>=15 and self.t%6==0 then
	 if self.bullet~=4 and self.bullet~=3 then
	  e_add(bullet({
		  pos=v(self.pos.x+2,self.pos.y+2),
		  vel=v(0,1)
		 }))
	 end
	 
	 self.bullet+=1 
	 self.vel.y-=0.4
 end
 
 if self.t>=60 then
  self.bullet=0
  self:become("idle")
 end
end

indicator=mob:extend({
 sprite={
  idle={
   39,40,delay=10
  },
 },
 maxhp=7,
 hitbox=box(1,1,8,7)
})

function indicator:init()
 self.vel=v(0,1)
end

function indicator:idle()
 self:bound()
 
 if self.t%48==0 then
  e_add(bullet({
	  pos=v(self.pos.x+2,self.pos.y+2),
	  vel=v(0,0.5)
	 }))
	 
	 e_add(bullet({
	  pos=v(self.pos.x+2,self.pos.y+2),
	  vel=v(-0.5,0.5)
	 }))
	 
	 e_add(bullet({
	  pos=v(self.pos.x+2,self.pos.y+2),
	  vel=v(0.5,0.5)
	 }))

 end
end

printer=mob:extend({
 sprite={
  idle={
   41
  },
 },
 maxhp=3,
 hitbox=box(1,1,7,7)
})

local txts={
"♥","…","░","∧","☉",
"◆","★","✽","●","웃",
"⌂","▥","🐱","ˇ","▒",
"♪","😐","die"
}

function printer:init()
 self:pick()
 self.pos=v(0,0)
end

function printer:pick()
 self.txt=txts[flr(rnd(#txts)+1)]
 self.tm=#self.txt*32
end

function printer:idle()
 self:bound()
 if self.t<self.tm then
 self.pos.x+=5/4
  if self.t%4==0 then
	  cls()
	  print(self.txt,0,0,7)
	  
	  local x=self.t/4
	  --self.pos.x+=5
	  self.pos.y+=1
	  
	  for y=0,5 do
	   if pget(x,y)==7 then
	    e_add(bullet({
				  pos=v(self.pos.x,self.pos.y+self.t*0.25+y*5),
				  vel=v(0,0.5)
				 }))
	   end
	  end
	 end
 elseif self.t>=self.tm+90 then
  self:pick()
  self.t=-1
 end
end

invader=mob:extend({
 sprite={
  idle={
   55,56,57,58,59,delay=5
  },
  attack={
   60,61,55,56,57,delay=15
  }
 },
 maxhp=3,
 hitbox=box(1,1,7,7)
})

function invader:init()
 self.lft=rnd()>0.5
end

function invader:idle()

 self:bound()
 if self.tar==nil then
  self.tar=v(self.lft and (rnd(16)+8) or (120-rnd(16)),rnd(120))
 end
 
 local dx,dy=self.tar.x-self.pos.x,
  self.tar.y-self.pos.y
  
 local d=sqrt(dx*dx+dy*dy)
 
 if d<=5 then
	 self:become("attack")
	 self.tar=nil
	else
	 local s=d*20
	 self.vel+=v(dx/s,dy/s)
	 self.vel*=0.95
 end
end

function invader:attack()
 self:bound()
	 self.vel*=0.9
	 
 if self.t>=30 and not self.fr then
  self.fr=true
  local vl=self.pos.x>64 and -1 or 1
  
	 e_add(bullet({
	  pos=v(self.pos.x-2,self.pos.y+2),
	  vel=v(vl,0)
	 }))
	 
	 self.vel.x-=vl*0.5
 end
 
 if self.t>=60 then
  self:become("idle")
  self.fr=false
 end
end


eye=mob:extend({
 sprite={
  idle={
   96
  },
 },
 hitbox=box(1,1,7,7)
})

function eye:idle()
 self:bound()
 if self.tar==nil then
  self.tar=v(rnd(120),rnd(120))
 end
 
 local dx,dy=self.tar.x-self.pos.x,
  self.tar.y-self.pos.y
  
 local d=sqrt(dx*dx+dy*dy)
 
 if d<=5 then
	 self.tar=nil
	else
	 local s=d*40
	 self.vel+=v(dx/s,dy/s)
	 self.vel*=0.95
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
-- states

ingame={}

local delay=0

local alw={
rocket,indicator
}

local pool={
 {30,eye,1},
 {0,spok,1},
 {10,xwing,1},
 {20,invader,1},
 {30,ufo,1.2},
 {40,indicator,1.5},
 {50,moon,1},
 {60,printer,0.8}
}

menuitem(1,"difficulty up",function()
 g_time=50*60
end)
menuitem(2,"give shroom",function()
 g_guy:become("big")
end)
menuitem(3,"give ship",function()
 g_guy:become("ship")
end)
menuitem(4,"give flower",function()
 g_guy:become("fire")
end)

local spick=0
local started=false

function ingame.update()
 
 if started and not g_guy.done then
  delay-=1
  
 g_guy.fireal=max(0,g_guy.fireal-1)
 
  local n=not entities_tagged["mob"]
	 if delay==0 or 
	  n or 
	  #entities_tagged["mob"]==0 then

	  if n or #entities_tagged["mob"]<5 then 
			 local pl={}
			 
			 for p in all(pool) do
			  if g_time/60>=p[1] then
			   add(pl,{p[2],p[3]})
			  end
			 end
			 
			 if #pl>0 then
			  local sm=0
			  
			  for p in all(pl) do
			   sm+=p[2]
			  end
			  
			  local r=rnd(sm)
			  sm=0
			  
			  for p in all(pl) do
			   sm+=p[2]
			   if sm>=r then
				   e_add(p[1]({
						  pos=v(rnd(120),-8)
						 }))
			    break
			   end
			  end
	  	end
		 elseif rnd()<0.1 then
		  e_add(alw[flr(rnd(#alw))+1]({
	 	  pos=v(rnd(120),-8)
		  }))
		 end
	  
	  delay=50
	 end
	
		spick+=1
		if spick>=500 then
		 spick=rnd(100)
		 local pl={starp,bomb,fireflower}
		 
		 if g_guy.state=="idle" then
		  add(pl,mushroom)
		 --else
		 -- add(pl,coin)
		 end
		 
		 if g_guy.state~="ship" and g_guy.state~="fire" then
		  add(pl,gmushroom)
		 end
		 
		 local p=pl[flr(rnd(#pl))+1]({
		  pos=v(rnd(120),-8)
		 })
		 
		 e_add(p)
		end
	end
 
 e_update_all()
 do_movement()
 do_collisions()
 do_supports()
end

local s=""
local d=false
local tm=0

function rectfill(x1,y1,x2,y2) 
 for x=x1,x2 do
  for y=y1,y2 do
   pset(x,y,sget(121,8+sget(121,8+pget(x,y))))
  end
 end
end

function ingame.draw()
 cls()
  
 local y=g_time-record
 
 if record>0 and y>0 and y<134 then
  print("record!",1,y-7,8)
  line(0,y,127,y,8)
 end
 
 if shk>0.1 then
  shk-=0.5
  camera(rnd(shk)-shk/2,rnd(shk)-shk/2)
 else
  camera()
 end
 
 r_render_all("render")
 
 if g_guy.done then
	 rectfill(0,58,127,67,1)
	 coprint("^r^i^p",60,7)
	 
	 rectfill(0,108,127,117,1)
	 coprint("^p^r^e^s^s ❎ ",110,7)
 
  if btnp(❎) then
   fade()
   restart_level()
  end
 else
  if(g_guy.fireal>0)rectfill(0,108,127,117,1) coprint("^p^r^e^s^s ❎ ^t^o ^f^i^r^e ",110,7)
 end
 
 if not started then
  rectfill(0,58,127,67,1)
	 coprint("⬅️➡️   ^n^u^l^l^p^t^r   ⬆️⬇️    ",60,7)
	 
	 rectfill(0,108,127,117,1)
	 coprint("^p^r^e^s^s ❎ ",110,7)
	 g_time=0
	 if(btnp(❎)) fade() started=true
 else
	 print(flr((g_guy.done and current or g_time)/60),1,1,7)
 end
end

menu={}

function menu.update()

end

function menu.draw()
 cls()
 if stat(30) then
  local c=stat(31)
  
  if c>=" " and c<="z" then
   s=s..c
   
   if s=="nothing" then
    d=true
   end
  elseif c=="\8" then
   s=sub(s,1,#s-1)
  end
 end

 color(6)
 print("attenpt to call 'starship'") 
 print("(a null value)\n")
 print("type 'nothing' to start")
 print("> "..s)
 
 if not d and t()%0.5<0.25 then
	 local x,y=(#s+2)*4+1,24
	 rectfill(x,y,x+3,y+4,8)
 end
 
 if d then
  tm+=1
  print("booting...")
  
  if tm>=30 then
   fade()
   state=ingame
   g_time=0
  end
 end
end

__gfx__
00000000eee00eeeeeeeeeeeeeeeeeeeeee00eee0000000000000000eee00eeeeeeeeeeeeeeeeeeeeee00eeeeee00eeeeee00eeef77feeee1dd1eeee8998eeee
00000000ee0c70eeeee00eeeeee00eeeee0c70ee0000000000000000000cc00000e00e00eee00eeeee0cc0ee000cc000000cc0007777eeeed77deeee9aa9eeee
00700700000dc0000e0c70e0ee0c70eee00dc00e00000000000000001ddccdd11d0cc0d1000cc00000dccd00566cc665d77cc77d7777eeeed77deeee9aa9eeee
0007700067777776600dc006000dc000077777700000000000000000dddccddddddccddd1ddccdd11ddccdd1666cc666777cc777f77feeee1dd1eeee8998eeee
0007700000ffff0007f77f706777777660ffff0600000000000000005dddddd55ddccdd5dddccdddddddddddd666666d67777776eeeeeeeeeeeeeeeeeeeeeeee
00700700e077770ee0ffff0e00ffff0000f77f0000000000000000000555555005dddd505dddddd55d5555d50dddddd006666660eeeeeeeeeeeeeeeeeeeeeeee
00000000e070070ee077770ee077770ee077770e0000000000000000e000000ee055550e0555555005000050e000000ee000000eeeeeeeeeeeeeeeeeeeeeeeee
00000000e060060ee060060ee060060ee060060e0000000000000000eeeeeeeeee0000eee000000ee0eeee0eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
0000000010e00e01100dd001100dd00110e00e0150e00e05d0e00e0d9aaaaaa99aa00aa99a0000a9000aa00000aaaa00a777777a7feeeeeea9eeeeee01000000
00000000d00dd00dd0dddd0dd0dddd0dd00dd00d6006600670077007aaccaaaaaac99aaaaa9999aa499ca99449ccaa9477cc777777eeeeeeaaeeeeee11000000
00000000d1dddd1dd1dddd1dd1dddd1dd1dddd1d656666567d7777d7aaccaaaaaacc9aaaaacc99aaaacca9aaaaccaaaa77cc777777eeeeeeaaeeeeee20000000
0000000051dddd15515dd515515dd51551dddd15d566665d6d7777d6aadd99aaaadc99aaaacc99aaaacd99aaaadd99aa77ddaa7777eeeeeeaaeeeeee31000000
00000000d05dd50dd015510dd015510dd05dd50d60d66d0670677607a900009aa90d409aa9dd449aa9d0049aa900009a7a0000a7f7eeeeee9aeeeeee45000000
000000006015510660011006600c7006601c7106605dd50670d66d07a0eeee0aa0e00e0aa000000aa00ee00aa0eeee0a70eeee07eeeeeeeeeeeeeeee51000000
000000000e0c70e00e0c70e00e0dc0e00e0dc0e00e0c70e00e0c70e09a0ee0a99a0ee0a99a0ee0a99a0ee0a99a0ee0a9a70ee07aeeeeeeeeeeeeeeee65000000
00000000ee0dc0eeee0dc0eeeee00eeeeee00eeeee0dc0eeee0dc0ee090ee090090ee090090ee090090ee090090ee0900a0ee0a0eeeeeeeeeeeeeeee76000000
00000000e9aee9aee12ee12eef7eef7e0000000000000000000000000d02820d0c08980c00000000e09aa90e0000000000000000000000003bb3eeee82000000
000000000880088002200220077007700000000000000000000000000d11d11d0cddcddc0888888009adda90000000000000000000000000baabeeee94000000
0000000008888880022222200777777000000000000000000000000008017108090d7d09080888809adddda9000000000000000000000000baabeeeea9000000
0000000002888820012222100f7777f0000000000000000000000000e001d100e00dcd0008888080addc7dda0000000000000000000000003bb3eeeeb3000000
00000000e087c80ee027c20ee077c70e000000000000000000000000ee01710eee0d7d0e08888880add1cdda000000000000000000000000eeeeeeeecd000000
00000000e08cd80ee02cd20ee07cd70e000000000000000000000000ee01d10eee0dcd0e088008809adddda9000000000000000000000000eeeeeeeed1000000
00000000e028820ee012210ee0f77f0e000000000000000000000000ee02820eee08980e0888888009adda90000000000000000000000000eeeeeeeee8000000
00000000ee0000eeee0000eeee0000ee000000000000000000000000eee080eeeee090ee00000000e09aa90e000000000000000000000000eeeeeeeef4000000
0000000000eeee0000e00e0000e00e0000eeee0000eeee0000eeee00070ee070070ee070070ee070e0eeee0ee00ee00e090ee090080ee0800000000000000000
000000008200002882022028820220288200002894000049a900009ae0700700e0700700e070070e0700007007700770e0900900e08008000000000000000000
000000008802208888022088880220888802208899044099aa0990aa0677776006700760e070070ee077770ee077770e04999940028888200000000000000000
000000008812218888122188881221888812218899144199aa1991aa677777766777777606777760067777600677776049999994288888820000000000000000
000000008812218888199188881991888819918899144199aa1991aa770770777707707767777776677777766707707699099099880880880000000000000000
0000000022099022220990222200002222099022440aa04499077099777777777707707777077077770770777777777799999999888888880000000000000000
00000000220000222200002222000022220000224400004499000099700000077007700777777777777007777700007790000009800000080000000000000000
0000000000eeee0000eeee0000eeee0000eeee0000eeee0000eeee00677007766770077667700776677007766770077649900994288008820000000000000000
eee00eeeeeeeeeeeeeeeeeeeeee00eeeee29a9eeeea79eeeeeea7eeeee888eee00000000ee3bb3eeeeeeeeae0000000000000000000000000000000000000000
ee0c70eeeee00eeeeee00eeeee0c70eee49aa89eea9479eeeeea7eeee8aaa8ee00000000e3bbb73eeeeee9ee0000000000000000000000000000000000000000
ee0dc0eeee0c70eeee0c70eeee0dc0ee488aaaa4a9aa479eaaaa777a8a070a8e00000000377bbbb3ee1595ee0000000000000000000000000000000000000000
e067760ee06dc60ee06dc60eee0770ee9889a8a9a9aa479ee90aa09ee8aaa8ee00000000b77bb7bbe15ddd5e0000000000000000000000000000000000000000
ee0ff0eeee0ff0eeee0770eee06ff60e49999994a9aa479eee9aa9eeee888eee000000003bbbbbb3e55dddde0000000000000000000000000000000000000000
e060060ee060060ee06ff60ee067760ee48ff84ea9aa4a9ee9aaaa9eeee3ebee00000000e33ff33ee55ddd5e0000000000000000000000000000000000000000
ee0ee0eeee0ee0eeee0000eeee0000eeeef77feeea94a9eee99ee99eeeebbeee00000000eef77feee155551e0000000000000000000000000000000000000000
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee6776eeeeaa9eee9eeeeee9eeebeeee00000000ee6776eeee1551ee0000000000000000000000000000000000000000
eee00eeeeeeeeeeeeeeeeeeeeee00eee000000009aa9eeeee028820e000000000000000000000000000000000000000000000000000000000000000000000000
ee0c70eeeee00eeeeee00eeeee0c70ee00000000499aeeee028c7820000000000000000000000000000000000000000000000000000000000000000000000000
000dc0000e0c70e0ee0c70eee00dc00e00000000499aeeee08ccc780000000000000000000000000000000000000000000000000000000000000000000000000
68888886600dc006000dc0000888888000000000499aeeee08dccc80000000000000000000000000000000000000000000000000000000000000000000000000
00222200082882806888888660222206000000002449eeee288dc882000000000000000000000000000000000000000000000000000000000000000000000000
e088880ee022220e002222000028820000000000eeeeeeee88888888000000000000000000000000000000000000000000000000000000000000000000000000
e080080ee088880ee088880ee088880e00000000eeeeeeee88888888000000000000000000000000000000000000000000000000000000000000000000000000
e060060ee060060ee060060ee060060e00000000eeeeeeee88200288000000000000000000000000000000000000000000000000000000000000000000000000
ee0000ee000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
e028820e000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
02677620000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
08700780000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
08700780000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
02677620000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
e028820e000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
ee0000ee000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
__label__
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000005000000000000000500000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000005000000000000000500000000000
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
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111110000000100000001111111111110000000000010001000000000000011111111111100000001000000011111111111111111111111
11111111111111111111100777770007777700111111111110770070707010701077707770777011111111111007777700077777001111111111111111111111
1111111111111111111110777dd77077dd77701111111111107d707070701070107d70d7d07d70111111111110777d777077ddd7701111111111111111111111
111111111111111111111077d007707700d77011111111111070707070701070107770070077d011111111111077d0d770770007701111111111111111111111
1111111111111111111110777007707700777011111111111070707070700070007dd007007d7011111111111077000770777077701111111111111111111111
1111111111111111111110d77777d0d77777d01111111111107070d77077707770700007007070111111111110d77777d0d77777d01111111111111111111111
11111111111111111111100ddddd000ddddd00111111111110d0d00dd0ddd0ddd0d0110d00d0d01111111111100ddddd000ddddd001111111111111111111111
11111111111111111111110000000100000001111111111110000000000000000000110000000011111111111100000001000000011111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000001000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000050000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000050000000000000000000000000000000000000000000000
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
000000000000000000000000000000000000000000000000000000000000000c7000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000006dc600000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000ff000000000000000000000000000000000000000000000000000000000000000
00500000000000000000000000000000000000000000000000000000000000600600000000000000000000000000000000000000000000000000000000000000
00500000000000000000000000000000000000000000000000000000000000005000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000100001000000000000000000000000000000000000000000100000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000100000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111000000000000000000000111100000001111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111077707770777007700770111007777700111111111111111111111111111111111111111111111111
1111111111111111111111111111111111111111111111107d707d707dd07dd07dd0111077d7d770111111111111111111111111111111111111111111111111
111111111111111111111111111111111111111111111110777077d07700777077701110777d7770111111111111111111111111111111111111111111111111
1111111111111111111111111111111111111111111111107dd07d707d00dd70dd70111077d7d770111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111070007070777077d077d01110d77777d0111111111111111111111111111111111111111111111111
111111111111111111111111111111111111111111111110d010d0d0ddd0dd00dd0011100ddddd00111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111000100000000000000001111100000001111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000

__sfx__
01100020004550240000452004320041202400044450444302455044020245202432024120240009445094430445500402044520443204412044000c4450c4430945507402094520943209412074020e4450e443
011000200e3050c3050c3050c3750e3050c3750c3550c3250e3050c3050c3050e3751c3050e3750e3550e3250e3050e3350e305103750c3051037510355103251030510335103051537513305153751537315375
011000200c06310003306650e0030c06310003306650c0030c0631000330665130030c0631000330665306650c0631000330665306650c06310003306650e0030c0631000330665306650c063100033066530665
011000200040502400004020047202305004720c3050c3050e3050c3050c3050247204305024720e3050e3050445500402044520443204412044000c4450c44309455074020945209433094120e4430e4430e443
011000200e3050c3050c3050c3750e3050c3750c3550c3250e3050c3050c3050e3751c3050e3750e3550e3250e3050e3350e305103750c3051037510355103251030510335103051337513305133751335513325
01100020004550240000452004320041202400044450444302455044020245202432024120240007445074430445500402044520443204412044000c4450c4430745507402074520743207412074020e4450e443
0110000013435183751a30518375183350c4750e4050c4750c4351a3751c3051a3751a3350e475104050e4750e4351c375183051c3751c335104750c40510475104351f3751f3051f3751f335134751340513475
0110000015435183751a30518375183350c4750e4050c4750c4351a3751c3051a3751a3350e475104050e4750e4351c375183051c3751c335104750c4051047510433213751f3052137521335154731f40515473
01100020002750220500275002750027502205042750427502275042050227502275022750220507275072750427500205042750427504275042050c2750c2750727507205072750727507275072050e2750e275
01100020002750220500275002750027502205042750427502275042050227502275022750220507275072750427500205042750427504275042050c2750c2750927507205092750927509275072050e2750e275
011000001333513335183751a30518375183350c3750e3050c3750c3351a3751c3051a3751a3350e375103050e3750e3351c375183051c3751c335103750c30510375103351f3751f3051f3751f3351337513305
011000001547515472183751837318371183750c405183050c4750c4731a3751a3051a3051a3750e4731a3750e4750e4721c3751c3731c3711c375104051c30510475104731f37521305213051f3751f4731f375
011000001f475154051f472183751837318371183751a3751c3750c4750c4031a3751a3051a3051a3750e4731a3750e4750e4021c3751c3731c3011c375104051c37510475104031f3751c375213051f3751f473
01100000214751540521472183751837318371183751a3751c3750c4750c4031a3751a3051a3051a3750e4731a3750e4750e4021c3751c3731c3011c375104051c3751047510403213751c375213052137521473
011000201f475183011f472183771837218301183051a3751c3750c4751c3051a3051c3751c3751a3050e4721a3050e4051f4721c3771c3761f4771c3751f4751c37510405183721f3751c305183031f3751f473
01100020214751840121472214771847218401184051a4751c4750c4751c4051a4051c4751c4751a4050e4721a4050e405214721c4771c4761f4771c475214751c4751040518472214751c405184032147521473
010800001135213352163521b3521f3551b3021f30216302183021b30200002000020000200002000020000200002000020000200002000020000200002000020000200002000020000200002000020000200002
010300002b04624006000060000600006000060000600006000060000600006000060000600006000060000600006000060000600006000060000600006000060000600006000060000600006000060000600006
010a00001807624075000002407600000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010a00002417618155001000c17500100001000010000100001000010000100001000010000100001000010000100001000010000100001000010000100001000010000100001000010000100001000010000100
0104000018134131340f1340c1340c1040c1040f10400104001040010400104001040010400104001040010400104001040010400104001040010400104001040010400104001040010400104001040010400104
010c00000c07312003000030000300003000030000300003000030000300003000030000300003000030000300003000030000300003000030000300003000030000300003000030000300003000030000300003
010900000c3550f3551335518351183551b3050030500305003050030500305003050030500305003050030500305003050030500305003050030500305003050030500305003050030500305003050030500305
01100000223551b355000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005
0109000016155221551d1050010500105001050010500105001050010500105001050010500105001050010500105001050010500105001050010500105001050010500105001050010500105001050010500105
001000001f15522105000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005
011000001b15500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005
010900002b55624006180001800000000000060000600006000060000600006000060000600006000060000600006000060000600006000060000600006000060000600006000060000600006000060000600006
010600001825524273002002420000200002000020000200002000020000200002000020000200002000020000200002000020000200002000020000200002000020000200002000020000200002000020000200
010700001835124374003012430100301003010030100301003010030100301003010030100301003010030100301003010030100301003010030100301003010030100301003010030100301003010030100301
01200000103750e3751f3750c3351f3051f305133050e3050e3050e3051c305183051c3051c30510305103050c3050c3051a3051c3051a3051a3050e3050c3050c3050c3051a3051c3051a3051a3050e30513305
__music__
01 02034144
01 02050444
00 02000144
00 02050644
00 02000744
00 02080b44
00 02090c44
00 02090d44
00 02090e44
02 02090f44
00 41424c44
00 48424c44
00 49424c44
00 02054644
02 02004744

