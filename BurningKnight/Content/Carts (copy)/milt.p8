pico-8 cartridge // http://www.pico-8.com
version 19
__lua__
-- milt
-- by @egordorichev

daynumber="8"
::_::
if (btnp()>0) goto donewithintro
cls()
f=4-abs(t()-4)
for z=-3,3 do
 for x=-1,1 do
  for y=-1,1 do
   b=mid(f-rnd(.5),0,1)
   b=3*b*b-2*b*b*b
   a=atan2(x,y)-.25
   c=8+(a*8)%8
   if (x==0 and y==0) c=7
   u=64.5+(x*13)+z
   v=64.5+(y*13)+z
   w=8.5*b-abs(x)*5
   h=8.5*b-abs(y)*5
   if (w>.5) rectfill(u-w,v-h,u+w,v+h,c) rect(u-w,v-h,u+w,v+h,c-1)
  end
 end
end

if rnd()<f-.5 then
 ?daynumber,69-#daynumber*2,65,2
end
 
if f>=1 then
 for j=0,1 do
  for i=1,f*50-50 do
   x=cos(i/50)
   y=sin(i/25)-abs(x)*(.5+sin(t()))
   circfill(65+x*8,48+y*3-j,1,2+j*6)
  end
 end
  
 for i=1,20 do
  ?sub("pico-8 advent calendar",i),17+i*4,90,mid(-1-i/20+f,0,1)*7
 end
end
 
if (t()==8) goto donewithintro

flip()
goto _
::donewithintro::

music(0)
cartdata("milt")
--dset(0,3)

function _init()
 g_time,state,g_index,g_won=
  0,menu,
 dget(0) or 0
 ,false
 entity_reset()
 collision_reset()

  
 shk=0
 
 -- do not forget about 
 -- m()
 -- ➡️➡️⬆️☉🅾️∧░⬆️➡️☉🅾️∧
end

function _update60()
 state.update()
 tween_update(1/60)
 g_time+=1
end

function _draw()
 if shk>0.5 then
  shk-=0.2
  camera(rnd(shk)-shk/2,rnd(shk)-shk/2)
 else
  camera()
 end
 
 state.draw()
end

function restart_level()
 fade()
 reload(0x1000,0x1000,0x1000)
 reload(0x2000,0x2000,0x1000)
 entity_reset()
 collision_reset()
 dset(0,g_index)

 e_add(level({
  base=v(g_index%8*16,flr(g_index/8)*16),
  size=v(16,16)
 }))
 
 for i=1,16 do
  e_add(snow({
   pos=v(0,0)
  }))
 end
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
  local ml,mv=32767
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

function spr_render(e)
 local s,p=e.sprite,e.pos

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
 (sp.x),(sp.y),w,h,flip_x)

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
  for ent in all(drawables[o]) do
   r_reset(prop)
   if not ent.done then ent[prop](ent,ent.pos) end
  end
 end
end

function r_reset(prop)
 pal()
 palt(0,false)
 palt(12,true)
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

function c_push_out(oc,ec,allowed_dirs,e,o)
 local sepv=ec.b:sepv(oc.b,allowed_dirs)
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
 if(not o) o=sget(97,c)
 
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
 --if (fget(blk,0)) return solid
 --if (fget(blk,1)) return support
end

level=entity:extend({
 draw_order=1
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
    if(not e.norem)mset(xx,yy,e.dtile)
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
 map(self.base.x,self.base.y)
end
 
solid=static:extend({
 tags={"walls"},
 hitbox=box(0,0,8,8),
 draw_order=3
})

function solid:bad()
 return self.lava and (
 (flr(nt_time/4 )
  +self.pos.x/8)%(g_index>=23 and (g_index==31 and 2 or 6) or 7)==0) 
end

function solid:render()
 if(g_red) return

 if self:bad() then
  self.t+=1
  rect(self.pos.x,self.pos.y,self.pos.x+7,self.pos.y+7,0)
  for x=self.pos.x+1,self.pos.x+6 do
   for y=self.pos.y+1,self.pos.y+6 do
    pset(x,y,get(x,y,20))
   end 
  end
 end
 
 if self.tile==5 and rnd()>0.99 then
  mset(self.map_pos.x,self.map_pos.y,0)
 end
end

function solid:init()
 self.t=0
 local dirs={v(-1,0),v(1,0),v(0,-1),v(0,1)}
 local allowed={}
 self.lava=fget(self.tile,3)
 local needed=self.lava
 for i=1,4 do
  local np=self.map_pos+dirs[i]
  allowed[i]=
   block_type(mget(np.x,np.y))
    ~=solid
  needed=needed or allowed[i]
 end
 self.allowed=allowed
 self.needed=needed
end

function solid:collide(e)
 if e:is_a("guy") 
 and self:bad() and self.t>=10 then
  e:die()
 end 

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

target=entity:extend({
 dtile=4
})

target:spawns_from(3)

function target:render()
 spr(self.tile,
 self.pos.x,
 self.pos.y+cos(g_time*0.01)*1.2)
end

snow=entity:extend({
 draw_order=5
})

function snow:init()
 if self.pos.x==0 then
 self.pos.x=rnd(128)
 else
 self.pos.x=-4 
 end
 self.pos.y=rnd(146)-16
 self.sz=rnd(3)+1
 self.s=self.pos.y
 self.mv=rnd(0.5)+1.5
 self.md=rnd(0.5)+1.5
 self.t=rnd(1000)
end

function snow:render()
 self.pos.x+=self.sz*0.5
 self.pos.y=self.s+cos(self.md*self.t
 *0.005)*self.mv*16
 
 if self.pos.x>132 then
  self:init()
 end
 rectfill(self.pos.x,
  self.pos.y,
  self.pos.x+self.sz,
  self.pos.y+self.sz,6)
end

key=entity:extend({
 dtile=10,
 tags={"key"}
})

key:spawns_from(24)

function key:render()
 spr(self.tile,self.pos.x,self.pos.y+cos(g_time*0.01)*1.2)
end
-->8
-- player

guy=entity:extend({
 sprite={idle={64,65,66,67,68,69,70,
 delay=5},
 move={80,81,82,83,84,85,86,delay=5},flips=true}
})

guy:spawns_from(64,16)

function guy:init()
 self.mx,self.my=0,0
 self.lmx,self.lmy=0,0
end

function guy:idle()
 if btnp(⬅️) then
  self.mx=-1
 elseif btnp(➡️) then
 	self.mx=1
 elseif btnp(⬆️) then
  self.my=-1
 elseif btnp(⬇️) then
  self.my=1
 end
 
 if self.mx~=0 or self.my~=0 then
  local mx,my=g_level.base.x+self.pos.x/8+self.mx,
   g_level.base.y+self.pos.y/8+self.my
   
  if fget(mget(mx,my),0)==true then
   self.mx=0
   self.my=0
   sfx(32)
   shk=2
   return
  end
  sfx(31)
  self:become("move")
 end
end

function guy:move()
 local x,y=self.pos.x,self.pos.y
 
 if x%8==0 and y%8==0 then
  local mx,my=g_level.base.x+x/8,
   g_level.base.y+y/8
  local ttt=mget(mx,my)
  
  if ttt==4 then
   g_index+=1
   sfx(34)
   restart_level()
   return
  elseif ttt==10 then
   sfx(32)
   for e in all(entities_tagged["key"]) do
    e.done=true
   end
   mset(mx,my,0)
   for x=0,15 do
    for y=0,15 do
     local xx,yy=g_level.base.x+x,
     g_level.base.y+y
     if mget(xx,yy)==25 then
      mset(xx,yy,0)
     end
    end
   end
  end
  
  local tt=mget(mx+self.mx,my+self.my)
  
  if fget(tt,0)==true then
   --[[if tt==2 then
    restart_level()
    return  
   end]]
   
   self.lmx,self.lmy=self.mx,self.my
   self.mx=0
   self.my=0
   sfx(33)
   self:become("idle")
   return
  end
  
  local t
  
  if self.lmx==0 and
   self.lmy==0 then
   
   if self.mx~=0 then
    t=7
   else
    t=22
   end
  else
   --printh(self.mx.." "..self.my.." lm "..self.lmx.." "..self.lmy)
   if self.mx~=0 then
    if self.lmy==0 then
     t=7
    elseif self.lmy==1 then
     t=self.mx==1 and 38 or 23
    else
     t=self.mx==1 and 6 or 8
    end
   else
    if self.lmx==0 then
     t=22
    elseif self.lmx==1 then
     t=self.my==1 and 8 or 23
    else
     t=self.my==1 and 6 or 38
    end
   end
   self.lmx=0
   self.lmy=0
  end
  
  mset(mx,my,t)
 end
 
 local s=g_index==0 and 1 or 2
 self.pos.x+=self.mx*s
 self.pos.y+=self.my*s
end
-->8
-- fx

part=entity:extend({
 draw_order=10
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

function part:render_hud()
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
   t.progress+=dt
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

menu={}

function menu.update()
 cls(0)
 e_update_all()
 do_movement()
 if not sn then
  sn=true
  for i=1,16 do
   e_add(snow({
    pos=v(0,0)
   }))
  end
 end
 
 if btnp(❎) then
  sn=false
  sfx(34)
  state=ingame
  restart_level()
 end
end

function menu.draw()
 map(112,48)
 r_render_all("render")
 coprint(("^press ❎"),8*10+cos(g_time*0.01)*1.5,7)
 coprint(("^by egordorichev"),120,7)
end

ingame={}

function ingame.update()
 if rst then
  restart_level()
  rst=false
  return
 end

 e_update_all()
 do_movement()
 do_collisions()
 if btnp(❎) then
  sfx(32)
  
  if g_index==16 then
   for i=0,63 do
    dset(i,0)
   end
   _init()
  else
  rst=true
  end
 end
end

function ingame.draw()
 cls()
 r_render_all("render")

 if g_index==1 then
  coprint("^press ❎ to restart",32+77,12)
 elseif g_index==16 then
  local y=32
  coprint("★★★★    ",y+7,10)
  coprint("^you won!",y+17,9)
  coprint("★★★★    ",y+27,10)
  
  coprint("^thanks for playing!",y+67,11)
  coprint("^press ❎",y+77,12)
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
 if(not o) o=sget(97,c)
 
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

function oline(x1,y1,x2,y2,c)
 for x=-1,1 do
  for y=-1,1 do
   line(x1+x,y1+y,x2+x,y2+y,c)
  end
 end
end

function noprint(s,x,y,c)
 s=smallcaps(s)
 prnt(s,x+4-#s*2,y,c)
end

cls()
m()
__gfx__
000000007777777700000000c070070ccccccccc0000000000000000000000000000000011111111000000000000000001000000000000000000000000000000
0000000077777777000000000a9009a0cccccccc0000000000055555555555555555500016666661000000000000000010000000000000000000000000000000
007007007777777700000000288a7882cccccccc0000000000500000000000000000050016666661000000000000000021000000000000000000000000000000
000770007777777700000000888aa888cccccccc0000000005000000000000000000005016666661000000000000000031000000000000000000000000000000
00077000777777770000000002299220cccccccc0000000005000000000000000000005016666661000000000000000042000000000000000000000000000000
007007007777777700000000088aa880cccccccc0000000005000000000000000000005016666661000000000000000051000000000000000000000000000000
000000007777777700000000088aa880cccccccc0000000005000055555555555500005016666661000000000000000065000000000000000000000000000000
000000007777777700000000028aa820cccccccc000000000500005000000000050000501111111100000000000000007d000000000000000000000000000000
ccc00ccc00000007700000000007700000077777777770000500005005000050cccccccc7aaaaaa7000000000000000082000000000000000000000000000000
cc0880cc00000077770000000077770000777777777777000500005055000050c000cccca999999a000000000000000094000000000000000000000000000000
cc0880cc000007777770000007777770077777777777777005000050000000500888000ca444999a0000000000000000a9000000000000000000000000000000
c088880c0000777777770000777777777777777777777777050000500000005008082880a4a4244a0000000000000000b3000000000000000000000000000000
088888800007777777777000777777777777777777777777050000500000005008880280a444924a0000000000000000c1000000000000000000000000000000
8088880800777777777777007777777707777777777777700500005000000500c000c00ca999999a0000000000000000d5000000000000000000000000000000
0080080007777777777777707777777700777777777777000500005055555000cccccccca999999a0000000000000000e2000000000000000000000000000000
c080080c77777777777777777777777700077777777770000500005000000000cccccccc7aaaaaa70000000000000000f0000000000000000000000000000000
00000000777777777777777777777777000770000000000005000050000000000000000000000000000000000000000000000000000000000000000000000000
05555550077777777777777077777777007777000000000005000055000000000000000000000000000000000000000000000000000000000000000000000000
05000050007777777777770077777777077777700000000005000000000000000000000000000000000000000000000000000000000000000000000000000000
05000050000777777777700077777777777777770000000005000000000000000000000000000000000000000000000000000000000000000000000000000000
05000050000077777777000077777777777777770000000005000000000000000000000000000000000000000000000000000000000000000000000000000000
05000050000007777770000007777770077777700000000000500000000000000000000000000000000000000000000000000000000000000000000000000000
05555550000000777700000000777700007777000000000000055555000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000077000000000077000000770000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
09999990088888800000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
09000090080000800000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
09000090080000800000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
09000090080000800000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
09000090080000800000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
09999990088888800000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
ccc00cccccccccccccccccccccccccccccc00cccccc00cccccc00ccc000000000000000000000000000000000000000000000000000000000000000000000000
cc0880ccccc00cccccc00cccccc00ccccc0880cccc0880cccc0880cc000000000000000000000000000000000000000000055555555555555555500000000000
cc0880cccc0880cccc0880cccc0880cccc0880cccc0880cccc0880cc000000000000000000000000000000000000000000500000000000000000050000000000
c088880cc088880ccc0880cccc0880cccc0880ccc088880cc088880c000000000000000000000000000000000000000005000000000000000000005000000000
088888800888888000888800c088880cc088880cc088880c08888880000000000000000000000000000000000000000005000000000000000000005000000000
80888808808888088888888808888880088888800888888080888808000000000000000000000000000000000000000005000000000000000000005000000000
00800800008008000088880080888808808888088080080880800808000000000000000000000000000000000000000005000055555555555500005000000000
c080080cc080080cc080080c00800800008008000080080000800800000000000000000000000000000000000000000005000050000000000500005000000000
ccc00ccccc0880cccc0880cccc0880cccc0880ccccc00cccccc00ccc000000000000000000000000000000000000000005000050050000500000000000000000
cc0880cccc0880cc0c0880cc0c0880cc0c0880cccc0880cccc0880cc000000000000000000000000000000000000000005000050550000500000000000000000
cc0880c0cc0880cc808888008088880c8088880c00888800cc0880cc000000000000000000000000000000000000000005000050000000500000000000000000
c0888808008888000888888808888880088888808888888800888800000000000000000000000000000000000000000005000050000000500000000000000000
0888888088888888c0888800c0888808c08888080088880088888888000000000000000000000000000000000000000005000050000000500000000000000000
8088880c00888800c080080c0800008008000080c080080c80888800000000000000000000000000000000000000000005000050000005000000000000000000
0080080cc080080c080cc08080cccc0880cccc08080c080c0080080c000000000000000000000000000000000000000005000050555550000000000000000000
cc0880ccc080080cc0cccc0c0cccccc00cccccc0c0ccc0ccc08080cc000000000000000000000000000000000000000005000050000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000005000050000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000005000055000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000005000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000005000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000005000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000500000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000055555000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001010
10101010101010101010101010101010100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001010
10101010101010101010101010101010100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001010
10101010101010101010101010101010100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001010
10101022000000000000000012101010100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001010
10101000300000000000003000101010100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001010
10101000000000000000000000101010100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001010
10101000300000000000003000101010100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001010
10101000000000000000000000101010100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001010
10101000300000000000003000101010100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001010
10101000000000000000000000101010100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001010
10101000300000000000003000101010100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001010
10101021000000000000000011101010100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001010
10101010101010101010101010101010100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001010
10101010101010101010101010101010100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001010
10101010101010101010101010101010100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
10101010101010101010101010101010101010101010101010000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010101010101010101010101010101010
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010101022000000121010101010101010
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010220000000000000000001210101010
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010000000000000000000000000001210
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010003100003100420031004110510010
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010001021111000000010000010000010
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010001012221000310010000010000010
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010003200003200320032000032001110
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010000000000000000000000000001010
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010000000000000000000000000111010
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010210000000000000000000000101010
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010100000000000000000000000101010
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010102101000000000000003011101010
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010101010210000000000111010101010
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010101010101010101010101010101010
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010101010101010101010101010101010
__label__
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777000000000000000000000000777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777770000000000000000000000000077777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777700000000000000000000000000007777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777000000000000000000000000000000777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777770000000000000000000000000000000077777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777700000000000000000000000000000000007777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777000000000000000000000000000000000000777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777770000000000000000000000000000000000000077777777777777777777777777777777777777777777777777777777777777777
77777777777777770000000000000000000000000000000000000000000000000000000000000000000000007777777777777777777777777777777777777777
77777777777777700000000000000000000000000000000000000000000000000000000000000000000000000777777777777777777777777777777777777777
77777777777777000000000000000000000000000000000000000000000000000000000000000000000000000077777777777777777777777777777777777777
77777777777770000000000000000000000000000000000000000000000000000000000000000000000000000007777777777777777777777777777777777777
77777777777700000000000000000000000000000000000000000000000000000000000000000000000000000000777777777777777777777777777777777777
77777777777000000000000000000000000000000000000000000000000000000000000000000000000000000000077777777777667777777777777777777777
77777777770000000000000000000000000000000000000000000000000000000000000000000000000000000000007777777777667777777777777777777777
77777777700000000000000000000000000000000000000000000000000000000000000000000000000000000000000777777777777777777777777777777777
77777777000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000007777777777777777
77777777000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000777777777777777
77777777000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000077777777777777
77777777000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000007777777777777
77777777000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000777777777777
77777777000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000077777777777
77777777000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000007777777777
77777777000000000000000000000000000000000000000000000000000000000000000000000660000000000000000000000000000000000000000777777777
77777777000000000007700000000000000000000007700000000000000770000000000000077660000000000007777777777777777770000000000077777777
77777777000000000077770000000000000000000077770000000000007777000000000000777700000000000077777777777777777777000000000077777777
77777777000000000777777000000000000000000777777000000000077777700000000007777770000000000777777777777777777777700000000077777777
77777777000000007777777700000000000000007777777700000000777777770000000077777777000000007777777777777777777777770000000077777777
77777777000000007777777700000000000000007777777700000000777777770000000077777777000000007777777777777777777777770000000077777777
77777777000000007777777700000000000000007777777700000000077777700000000077777777000000000777777777777777777777700000000077777777
77777777000000007777777700000000000000007777777700000000007777000000000077777777000000000077777777777777777777000000000077777777
77777777000000007777777700000000000000007777777700000000000770000000000077777777000000000007777777777777777770000000000077777777
77777777000000007777777770000000000000077777777700000000000000000000000077777777000000000000000077777777000000000000000077777777
77777777000000007777777777000000000000777777777700000000000000000000000077777777000000000000000077777777000000000000000077777777
77777777000000007777777777700000000007777777777700000000000000000000000077777777000000000000000077777777000000000000000077777777
77777777000000007777777777770000000077777777777700000000000000000000000077777777000000000000000077777777000000000000000077777777
77777777000000007777777777777000000777777777777700000000000000000000000077777777000000000000000066677777000000000000000077777777
77777777000000007777777777777700007777777777777700000000000000000000000077777777000000000000000066677777000000000000000077777777
77777777000000007777777777777770077777777777777700000000000000000000000077777777000000000000000066677777000000000000000077777777
77777777000000007777777777777777777777777777777700000000000000000000000077777777000000000000000077777777000000000000000077777777
77777777000000007777777777777777777777777777777700000000000770000000000077777777000000000000000077777777000000000000000077777777
77777777000000007777777707777777777777707777777700000000007777000000000077777777000000000000000077777777000000000000000077777777
77777777000000007777777700777777777777007777777700000000077777700000000077777777000000000000000077777777000000000000000077777777
77777777000000007777777700077777777770007777777700000000777777770000000077777777000000000000000077777777000000000000000077777777
77777777000000007777777700007777777700007777777700000000777777770000000077777777000000000000000077777777000000000000000077666777
77777777000000007777777700000777777000007777777700000000777777770000000077777777000000000000000077777777000000000000000077666777
77777777000000007777777700000077770000007777777700000000777777770000000077777777000000000000000077777777000000000000000077666777
77777777000000007777777700000007700000007777777700000000777777770000000077777777000000000000000077777777000000000000000077777777
77777777000000007777777700000000000000007777777700000000777777770000000077777777000000000000000077777777000000000000000777777777
77777777000000007777777700000000000000007777777700000000777777770000000077777777000000000000000077777777000000000000007777777777
77777777000000007777777700000000000000007777777700000000777777770000000077777777000000000000000077777777000000000000077777777777
77777777000000007777777700000000000000007777777700000000777777770000000077777777000000000000000077777777000000000000777777777777
77777777000000007777777700000000000000007777777700000000777777770000000077777777000000000000000077777777000000000007777777777777
77777777000000000777777000000000000000000777777000000000077777700000000007777770000000000000000007777770000000000077777777777777
77777777000000000077770000000000000000000077770000000000007777000000000000777700000000000000000000777700000000000777777777777777
77777777000000000007700000000000000000000007700000000000000770000000000000077000000000000000000000077000000000007777777777777777
77777777000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000007777777777777777
77777777000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000007777777777777777
77777777000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000007777777777777777
77777777000000000000000000000000000000000000066600000000000000000000000000000000000000000000000000000000000000007777777777777777
77777777000000000000000000000000000000000000066600000000000000000000000000000000000000000000000000000000000000007777777777777777
77777777000000000000000000000000000000000000066600000000000000000000000000000000000000000000000000000000000000007777777777777777
77777777000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000007777777777777777
77777777000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000007777777777777777
66677777000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000077777777777777777
66677777000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000777777777777777777
66677777000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000007777777777777777777
66677777000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000077777777777777777777
66677777000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000777777777777777777777
77777777000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000007777777777777777777777
77777777000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000077777777777777777777777
77777777000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000777777777777777777777777
77777777700000000000000000000000000006666000000000000000000000000000000000000000000000000000000000000000777777777777777777777777
77777777770000000000000000000000000006666000000000777000000000000000000000077777000000000000000006600000777777777777777777777777
777777777770000000000000000000000000066660000000007d707770777007700770000077d7d7700000000000000006600000777777777777777777777777
7777777777770000000000000000000000000666600000666077707d7077d07dd07dd00000777d77700000000000000000000000777777777777777777777777
777777777777700000000000000000000000000000000066607dd077d07d00d070d070000077d7d7700000000000000000000000777777777777777777777777
7777777777777700000000000000000000000000000000000070007d70777077d077d00000d77777d00000000000000000000000777777777777777777777777
77777777777777700000000000000000000000000000000000d000d0d0ddd0dd00dd0000000ddddd000000000000000000000000777777777777777777777777
77777777777777770000000000000000000000000000000000000000000000000000000000000000000000000000000000000000777777777777777777777777
77777777777777770000000000000000000000000000000000000000000000000000000000000000000000000000000000000000777777777777777777777777
77777777777777770000000000000000000000000000000000000000000000000000000000000000000000000000000000000000777777777777777777777777
77777777777777770000000000000000000000000000000000000000000000000000000000000000000000000000000000000000777777777777777777777777
77777777777777770000000000000000000000000000000000000000000000000000000000000000000000000000000000000000777777777777777777777777
77777777777777770000000000000000000000000000000000000000000000000000000000000000000000000000000000000000777777777777777777777777
77777777777777770000000000000000000000000000000000000000000000000000000000000000000000000000000000066600777777777777777777777777
77777777777777770000000000000000000000000000000000000000000000000000000000000000000000000000000000066600777777777777777777777777
77777777777777770000000000000000000000000000000000000000000000000000000000000000000000000000000000066600777777777777777777777777
77777777777777777000000000000000000000000000000000000000000000000000000000000000000000000070070000000007777777777777777777777777
77777777777777777700000000088000000000000000000000000000000000000000000000000000000000000a9009a000000077777777777777777777777777
7777777777777777777000000008800000000000000000000000000000000000000000000000000000000000288a788200000777777777777777777777777777
7777777777777777777700000088880000000000000000000000000000000000000000000000000000000000888aa88800007777777777777777777777777777
77777777777777777777700008888880000000000000000000000000000000000000000000000000000000000229922000077777777777777777777777777777
7777777777777777777777008088880800000000000000000000000000000000000000000000000000000000088aa88000777777777777777777777777777777
7777777777777777777777700080080000000000000000000000000000000000000000000000000000000000088aa88007777777777777777777777777777777
7777777777777777777777770080080000000000000000000006666000000000000000000000000000000000028aa82077777777777777777777777777777777
77777777777777777777777777777777700000000000000000066660000000000000000000000000000000077777777777777777777777777777777777777777
77777777777777777777777777777777770000000000000000066660000000000000000000000000000000777777777777777777777777777777777777777777
77777777777777777777777777777777777000000000000000066660000000000000000000000000000007777777777777777777777777777777777777777777
77777777777777777777777777777777777700000000000000000000000000000000000000000000000077777777777777777777777777777777777777777777
77777777777777777777777777777777777770000000000000000000000000000000000000000000000777777777777777777777777777777777777777777777
77777777777777777777777777777777777777000000000000000000000000000000000000000000007777777777777777777766666777777777777777777777
77777777777777777777777777777777777777700000000000000000000000000000000000000000077777777777777777777766666777777777777777777777
77777777777777777777777777777777777777770000000000000000000000000000000000000000777777777777777777777766666777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777766666777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777766666777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777700000777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777707770000077700000000000000000000700000000000000000000000000007777777777777777777777777777777777
77777777777777777777777777777777707d70707077707770777007707770770007707770777077707070777070707777777777777777777777777777777777
777777777777777777777777777777777077d07770777077d07dd07d707d707d707d707d70d7d07dd0707077d070707777777777777777777777777777777777
77777777777777777777777777777777707d70dd7077707d007070707077d07070707077d00700700077707d0077707777777777777777777777777777777777
77777777777777777777777777777777707770777077707770777077d07d7077d077d07d70777077707d707770d7d07777777777777777777777777777777777
7777777777777777777777777777777770ddd0ddd07770ddd0ddd0dd00d0d0dd00dd00d0d0ddd0ddd0d0d0ddd00d007777777777777777777777777777777777
77777777777777777777777777777777700000000077700000000000000000000000000000000000000000000000077777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777

__gff__
0001000000000101010000000000000000010101010101010001000000000000000101010100010000000000000000000000000000000000000000000000000000000000000000000000000001010100000000000000000000000000010100000000000000000000000000000100000000000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
__map__
0101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101
0101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101220000210101220021010101010101010101010101010101010101010101010101010101010101010101010101010101010101012201010122010101010101010101010101010101010101010101
0101010101010101010101010101010101010101010101010101010101010101010101220000000000000021010101010101000000000000000003010101010101010101010101010101010101010101010101010101010101010101010101010122002101010001010100010101010101010101220021010101010101010101
0101010122000000210101010101010101010101010101010101010101010101010101000000000000000010010101010101001101010101010101010101010101012210010101220000000000002101010101010101010101010101010101010100000001011101010111010101010101010101000000010101010101010101
0101010100000000000021010101010101010101010101012200210101010101010101000011120000000011010101010101000122000000000010010101010101010000010101000000000000000001010101010101010101010101010101010100110021010101010101220000210101010101001300012221012221010101
0101010100000000000000210101010101010101221021220003000101010101010101000001010011010001010101010101000100001300000011010101010101010000010101001300000313000001012200000000000000000021010101010100010000000000000000000000000101010122002300230000000000010101
0101010100000000000000110101010101010101000000000000000101010101010101000021220001010001010101010101000100002300000001010101010101010000210122000100110101000001010000000000000000000000010101010100010000111500110101010112000101010100000000000010000000010101
0101012200000000000011010101010101010101000000000000000101010101010101120000000001010001010101010101000112000000130001010101010101011200000000000100010101000001010011010101010101150000010101010100010000230000210101010101000101010100011200130000000311010101
0101014000000000000301010101010101010101000000000000000101010101010101011200001101220021010101010101002101010101220001010101010101010101120000000100210101000001010001010101220000000000010101010100011200000010000000010101000101010100210100010000001401010101
0101011200000000000021010101010101010101000000000000000101010101010101010101010101000300010101010101000000000000000001010101010101010101010013002300000101000001010001222101030011011200010101010100010101011500001300210122000101010100000000010000000000210101
0101012200000000000000210101010101010101120000000000110101010101010101010101010101120011010101010101120000000000001101010101010101010101220001000000110101000001010001000023000001010100210101010100210101220000000100000300000101010112000000230013002400000101
0101010000000000000000000101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101000021010101010122000001010001000000000021012200000101010100000000000000110100000000000101010101001300000001000000110101
0101011200000000000000110101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101120000000000000000000001010023001101150000000000110101010100140101010101012200111200110101010101002101010122001101010101
0101010101011200001101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101120000001101010000002122000024001101010101010100000000000000000000010101010101010101000000000000000101010101
0101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101011200100000000000000101010101010112000000000000000011010101010101010101120000000000110101010101
0101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101
0101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101
0101010101010101010101010101010101010101012200000000000021010101010101010101010101010101010101010101010122000021010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101012200210101010101010122000021010101010101010101
0101010101010101010101010101010101010101220300140101150011010101010101010101010101010101010101010101010100000000000000000021010101010101010101010101010101010101010101010101220021010101010101010101100000000000000000000000210101010100000000000000000018010101
0101010101010101010101010101010101220000000000000000000021010101010101010101010101000101010101010101010100110101010101120000010101010101010101010101010101010101010101010101000000010101010101010101150000000013000000111200000101010100110101010101010101010101
0101012200000000000021010101010101000014011500110115000000002101010101010101010122000101010101010101012200010000000021010000010101220000000000000000001000000101010101010101002400210101010101010101000014120001001300010100110101010100011000000021010101010101
0101010000000000000000000000210101120000010000010100000013000001010101010101221000002101010101010122000000010014150003230011010101120000000000000014010101010101010101010101000000000003010101010101001300230001000100010100010101010100211500000011010101010101
0101010000000010000000000000110101010000230014010100240001000001010101220000000000000000000101010100001300010000000000000001010101220018141200000000000000002101010101010122000000140101010101010101000100000023000110010100010101012200000000000021010101010101
0101010000000000000000001919010101010000000000180100000001000001010101000000000000000000030101010112100100010024001401120001010101000000000100000000000000000001010101010000000000000000002101010101000100000000002101012200010101010000000000000000000010010101
0101010000000000000000001903010101010000000010000100001401000001010101121000000000001300000101010101010100010000000001010001010101000000100100000000000000030001010101010115001401010112000001010122002300000000000000000000010101011200000024000014010101010101
0101010000000000000000001919010101010013001300002300000001000001010101010101010112000101010101010101100100211500130001010001010101000000001900000000130000000001010101010000001001001001000001010112000000000000000000000011010101010101011200000000190001010101
0101011800000000000000000011010101010001000100000019240023001101010101010101010101000101010101010101002300000000230001010001010101000000000112000011011200001101010101220024000001001422000001010101011200001112001101120001010101010101010100001011010001010101
0101011200000011010101010101010101220001002101011500000000000101010101010101010101010101010101010101000000000000000001010001010101120000110101010101010101010101010101000000000001000000001101010101010101010101010101010301010101010101010100240001010001010101
0101010101010101010101010101010101000001000000000000110115000101010101010101010101010101010101010101121101010101120021220001010101010101010101010101010101010101010101001401150001121101010101010101010101010101010101010101010101010101010100000001010001010101
0101010101010101010101010101010101000023001401010101012200002101010101010101010101010101010101010101010101010101010000000001010101010101010101010101010101010101010101000000000001010101010101010101010101010101010101010101010101010101010112001101010301010101
0101010101010101010101010101010101120000000000000000000000001101010101010101010101010101010101010101010101010101011200001101010101010101010101010101010101010101010101120000001101010101010101010101010101010101010101010101010101010101010101010101010101010101
0101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101
__sfx__
012000200c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c072
011000200c8030c80530615138050c8030c8053061513805118031180530615138051180311805306153061513803138053061513805138031380530615138050f8030f805306053061530605306153060430615
011000200c8730c87530615138050c8730c875306151380511873118753061513805118731187530615306150f8730f87530615138050f8730f87530615138051387313875306150780213873138753061430615
012000201300513005130051300513005130051300513005110051100511005110051100511005110051100516005160051600516005160051600516005160050f0050f0050f0050f00511005130051600518005
011000201331213312133121331213312133121331213312113121131211312113121131211312113121131216312163121631216312163121631216312163120f3120f3120f3120f31211312133121631218312
011000200f3150f3120f3150f312133121331513312133120c3120c3120c3150c3121131211315113121131213312133121331513312163121631516312163150c3150c3120c3150c31211315133151631518315
011000200cf430cf453062513f050cf430cf453062513f0511f4311f453062513f0511f4311f4530625306250ff430ff453062513f050ff430ff453062513f0513f4313f453062507f0213f4313f453062430625
011000200c1720c1720c1720c1720c1720c1720c1720c1720c1720c1720c1720c1720c1720c1720c1720c1720c1720c1720c1720c1720c1720c1720c1720c1720c1720c1720c1720c1720c1720c1720c1720c172
011000200f3150f3120c3150f31213312133150f312133120c3120c312073150c31211312113150c3121131213312133120c31513312163121631511312163150c3150c312073150c31211315133150f31518315
011000200f3150f3120c3120f31213312133120f312133150c3150c312073120c31211312113120c3121131513315133120c31213312163121631211312163150c3150c312073120c31211312133120f31218315
0110002018313183151831318313183131831518313183131d3131d3151d3131d3131d3131d3151d3131d3131b3131b3151b3131b3131b3131b3151b3131b3131f3131f3151f3131f3131f3131f3151f3131f313
01100020111171311716117181171b1171611713117111170f1170f117111171111716117181171b1171d1171f1171d1171b1171811716117161171311711117111171311716117181171b117181171611713117
01100020115351353516135181351b5351653513135111350f5350f535111351113516535185351b1351d1351f5351d5351b1351813516535165351313511135115351353516135181351b535185351613513135
01100020113071330716317183171b3071630713317113170f3070f307113171131716307183071b3171d3171f3071d3071b3171831716307163071331711317113071330716317183171b307183071631713317
01100020115271352716127181271b5271652713127111270f5270f527111271112716527185271b1271d1271f5271d5271b1271812716527165271312711127115271352716127181271b527185271612713127
01100020242352723524235242351f235222352423524235292352b235292352923524235272352923529235272352b2352723527235222352423527235272352b2352e2352b2352423527235292352b2352b235
01100020182351b2351823518235132351623518235182351d2351f2351d2351d235182351b2351d2351d2351b2351f2351b2351b23516235182351b2351b2351f235222351f235182351b2351d2351f2351f235
01100020184241b4241842418424134241642418424184241d4241f4241d4241d424184241b4241d4241d4241b4241f4241b4241b42416424184241b4241b4241f424224241f424184241b4241d4241f4241f424
01100020184121b4121841218412134121641218412184121d4121f4121d4121d412184121b4121d4121d4121b4121f4121b4121b41216412184121b4121b4121f412224121f412184121b4121d4121f4121f412
011000202432527325243252432513325163251832518325293252b3252932529325183251b3251d3251d325273252b325273252732516325183251b3251b3252b3252e3252b325243251b3251d3251f3251f325
011000200c4220f4220c4220c4221f42222422244222442211422134221142211422244222742229422294220f422134220f4220f422224222442227422274221342216422134220c42227422294222b4222b422
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010300001b3551f355003050030500305003050030500305003050030500305003050030500305003050030500305003050030500305003050030500305003050030500305003050030500305003050030500305
00060000243551d355000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005
0103000030635246351b6351860500605006050060500605006050060500605006050060500605006050060500605006050060500605006050060500605006050060500605006050060500605006050060500605
000900000c355133551b35524355183551f3550000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
__music__
00 01404040
00 02034340
01 02040340
00 02050340
00 06080340
00 0a090603
00 0a090b03
00 06090b03
00 060a0c03
00 060a0d03
00 06090e03
00 06090a03
00 060a0f03
00 060a1003
00 060f1103
00 060f1203
00 06131403
00 06131403
02 06531403
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000
00 00000000

