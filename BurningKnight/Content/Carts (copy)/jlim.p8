pico-8 cartridge // http://www.pico-8.com
version 19
__lua__
-- jlim game
-- by @egordorichev

local osfx,omusic=sfx,music
function sfx(...)
 if allow_sfx then
  osfx(...)
 end
end
function music(...)
 if allow_music then
  omusic(...)
 end
end


cartdata("ldjam439")

palettes={
{0,1,13},
{0,3,11},
{1,2,8},
{5,13,6},
{0,1,2},
{2,4,15},
{4,9,10},
{5,13,12},
{7,13,1},
{0,1,3}
}

pli=max(dget(5) or 0, 1)
plttt=palettes[pli]

allow_music=dget(16)~=1
allow_sfx=dget(17)~=1

menuitem(1,"next palette",function()
 
 pli+=1
 if pli>#palettes then
  pli=1
 end
  sfx(12)
 dset(5,pli)
 plttt=palettes[pli]
end)
menuitem(2, "reset progress",function()
 for i=0,64 do
	 dset(i,0)
 end
  sfx(12)
 g_won=false
 g_guy=nil
 fade()
 _init()
end)

function _init()
 g_time,state,g_index
  =
 0,menu
 ,dget(63),false
 
 mnadded=false
 poke(0x5f2d,1)
 music(g_index==0 and 0 or flr(rnd(16)))
 g_num_keys=dget(0) or 0
 
 entity_reset()
 collision_reset()

 shk=0
 -- do not forget about 
 -- m()
 -- *in the end of the cart
 -- ➡️➡️⬆️☉🅾️∧░⬆️➡️☉🅾️∧
end

function _update60()
 state.update()
 tween_update(1/60)
 g_time+=1
 
 if btnp(❎,1) then
  pli+=1
  if pli>#palettes then
   pli=1
  end
  sfx(12)
  dset(5,pli)
  plttt=palettes[pli]
 end
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
 g_index=mid(0,31,g_index)
 dset(63,g_index)
 local g=g_guy

 reload(0x1000,0x1000,0x1000)
 reload(0x2000,0x2000,0x1000)

 entity_reset()
 collision_reset()
 g_mob=0
 g_mv={}
 
 g_level=e_add(level({
  base=v(g_index%8*16,flr(g_index/8)*16),
  size=v(16,16)
 }))
 
 if g and not g.dd then
 e_add(guy({pos=v(g.pos.x,g.pos.y)})) 
 elseif not g_guy or (g and g.dd) then
 e_add(guy({pos=v(dget(60),dget(62))})) 
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
 palt(14,true)
 palt(11,true)
 pal(0,plttt[1])
 pal(1,plttt[2])
 pal(13,plttt[3])
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
  if not e.dd then
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
   if not (support and support.ignn) then
   e.supported_by=support
   if support and support.vel then
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
 if(not o) o=1--sget(97,c)
 
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
function fade(n)
 if(not n)shk=3
 for i=1,#pltt do
  cls(pltt[i])
  flip()-- flip()
 end
end
-->8
-- level

function block_type(blk)
 if (fget(blk,0) or fget(blk,2) or fget(blk,3)) return solid
 if (fget(blk,1)) return support
end

level=entity:extend({
 draw_order=1
})

function level:init()
 local b,s=self.base,self.size
 for y=s.y-1,0,-1 do
  for x=s.x-1,0,-1 do
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
 map(self.base.x,self.base.y)
end
 
solid=static:extend({
 tags={"walls"},
 draw_order=3
})

function solid:init()
 local dirs={v(-1,0),v(1,0),v(0,-1),v(0,1)}
 local allowed={}
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
 
 if self.needed then
  self.hitbox=fget(self.tile,2) and box(0,0,8,4) 
  or (fget(self.tile,3) and
  box(0,4,8,8)
  or box(0,0,8,8))
 end
end

function solid:collide(e)
 if(not e.dd) return c_push_out,self.allowed
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
 tags={"door"},
 collides_with={"guy"},
 hitbox=box(1,0,7,16),
 draw_order=0
})

door:spawns_from(80,87)

function door:collide()
 if(self.m==0) return c_push_out
end

function door:open()
 self.m=self.t*0.2
 if(self.t==1)sfx(17)
 if self.m>=8 then
  self.m=8
  if (not self.e) self.done=true
 end
end

function door:idle()
 if self.e and g_mob>0 and 
 (g_guy.pos.x>self.pos.x+8 or g_guy.pos.x<self.pos.x-8) then
  self:become("close")
 end
end

function door:close()
 self.m=8-self.t*0.2
 if self.m<=0 then
  self.m=0
 end
 if(g_mob==0) self:become("open")
end

function door:init()
 self.m=0
 if dget(32+g_index)==1 then
  self.done=true
 end
 self.e=self.tile==87
 if self.e then
  self.m=16
 end
end

function door:render()
 if self.state=="open" then
  pal(0,plttt[2])
 end
 spr(self.tile,self.pos.x,self.pos.y-self.m)
 spr(self.tile+16,self.pos.x,self.pos.y+8+self.m)
end

key=entity:extend({
 hitbox=box(0,0,8,8),
 sprite={idle={81}},
 collides_with={"guy"}
})

key:spawns_from(81)

function key:init()
 self.vel=v(0,0)
 self.start=self.pos.y
 
 if(not self.id) self.id=2
 self.tt=60
end

function key:collide(o)
 if self.tar then return end
 
 self.tar=o
 sfx(12)
 self:become("follow")
 dset(1+g_index,1)
 g_num_keys+=1
 dset(0,g_num_keys)
 self.tt=0
end

function key:render()
 if self.state=="follow" then
  pal(0,plttt[2])
 end
 spr_render(self)
end

function key:idle()
 if dget(1+g_index)==1 then
  self.done=true
 end
 
 self.pos.y=self.start+
 cos(self.t*0.01)*1.5
end

function key:follow()
 local dx=self.tar.pos.x-self.pos.x
 local dy=self.tar.pos.y-self.pos.y
 local d=sqrt(dx*dx+dy*dy)
 
 if d>12+12*self.id then
  self.vel.x+=dx/d*0.2
  self.vel.y+=dy/d*0.2
 end
 
 self.vel.x*=0.9
 self.vel.y*=0.9
 
 local a=entities_tagged["door"]
 if a then
 for e in all(a) do
  if not e.e and e.state~="open" then
  local dx=e.pos.x-self.pos.x
  local dy=e.pos.y-self.pos.y
  local d=sqrt(dx*dx+dy*dy)
 
  if d<4 then
   self.done=true
   g_num_keys-=1
   dset(0,g_num_keys)
   e:become("open")
   dset(32+g_index,1)
  elseif d<(64) then
   self.vel.x+=dx/d
   self.vel.y+=dy/d
  end
 end
 end
 end
end

spike=entity:extend({
 collides_with={"guy"}
})

function spike:init()
 if self.tile~=29 then
  self.hitbox=box(0,0,8,4)
 else
  self.hitbox=box(0,6,8,8)
 end
end

spike:spawns_from(23,39)

function spike:render()
 if(self.t%30>15) pal(1,plttt[1]) pal(0,plttt[2])
 spr(self.tile,self.pos.x,self.pos.y)
end

function spike:collide(o)
 o:die(self.t~=29)
end

bullet=entity:extend({
 hitbox=box(0,0,5,2),
 sprite={idle={72}},
 draw_order=0,
 collides_with={"mob","walls","door"}
})

function bullet:collide(o)
 if(o.ignn) return
 if o:is_a("mob") then
  o:die()
 end
 
 parts(self.pos.x,self.pos.y,3,3,0)
 self.done=true
end

function bullet:idle()
 if self.pos.y>128 or self.pos.y<8 then
  self.done=true
 end
end

mob=entity:extend({
 tags={"mob"},
 hitbox=box(0,0,8,8),
 collides_with={"guy"}
})

function mob:collide(o)
 if o:is_a("guy") then
  o:die()
 end
end

function mob:die()
 self.hp-=1
 self.tt=30
 sfx(16)
 if self.hp<=0 then
  self.done=true
  g_mob-=1
  sfx(15)
 end
end

function mob:init()
 self.hp=2
 self.tt=0
 g_mob+=1
end

function mob:render()
 if self.tt>0 and self.tt%15>7 then
  pal(1,plttt[1])pal(0,plttt[2])
 end
 self.tt-=1
 spr_render(self)
end

slime=mob:extend({
 sprite={idle={82,83,delay=10}}
})

slime:spawns_from(82)

zomb=mob:extend({
 sprite={idle={84,85,86,85,delay=10}}
})

zomb:spawns_from(84)

function zomb:init()
 self.dr=-1
end

function zomb:idle()
 self.pos.x+=self.dr*0.25
 
 local x,y=g_level.base.x
   +flr(self.pos.x/8+0.5),
  g_level.base.y
   +flr(self.pos.y/8)
 
 if (not fget(mget(x,y+1),0) and not fget(mget(x,y+1),1))
 or fget(mget(x+self.dr,y),0) then
  
  self.dr*=-1
 end
end

pwr=entity:extend({
 sprite={idle={97}},
 collides_with={"guy"},
 hitbox=box(0,0,8,8)
})

pwr:spawns_from(97)

function pwr:init()
 self.start=self.pos.y
 if dget(2)==1 then
  self.done=true
 end
end

function pwr:collide(o)
 o.db=true
 dset(2,1)
 sfx(17)
 fade(1)
 self.done=true
 parts(self.pos.x,self.pos.y,4,4,0)
end

function pwr:idle()
 self.pos.y=self.start+cos(self.t*0.01)*1.5
end

fall=entity:extend({
 sprite={idle={73},
 fl={74,75,76,delay=8},
 dead={16},
 new={77,78,delay=8}},
 tags={"walls"},
 hitbox=box(0,2,8,5)
})

fall:spawns_from(73)

function fall:fl()
 if self.t>=23 then
  self:become("dead")
 end
end

function fall:new()
 if self.t>=15 then
  self:become("idle")
  self.ignn=false
 end
end

function fall:dead()
  self.ignn=true
 if self.t>=60 then
  self:become("new")
 end
end

function fall:collide(o) 
 if o:is_a("guy") and self.state=="idle" then
  self:become("fl")
  return c_push_out
 end
end

tggle=entity:extend({
 hitbox=box(0,0,8,8),
 tags={"walls"},
 sprite={idle={88},gone={89}}
})

tggle:spawns_from(88,89)

function tggle:init()
 if self.tile==89 then
  self.ignn=true
  self:become("gone")
 end
end

function tggle:gone()
 if self.t>=50 then
  self.ignn=false
  self:become("idle")
 end
end

function tggle:idle()
 if self.t>=50 then
  self.ignn=true
  self:become("gone")
 end
end

function tggle:collide()
 if self.state=="idle" then
  return c_push_out
 end
end

mv=entity:extend({
 sprite={
  idle={90},
  off={91}
 },
 hitbox=box(0,0,8,8),
 tags={"walls"}
})

mv:spawns_from(90,91)

function mv:init()
 add(g_mv,self)
 self.id=#g_mv
 if self.tile==91 then
  self:become("off")
 end
end

function mv:idle()
 self.ignn=false
end

function mv:off()
 self.ignn=true
end

function mv:collide(o)
 if self.state=="idle" and o:is_a("guy") then
  self:become("off")
  if self.id==#g_mv then
   g_mv[1]:become("idle")
  else
   g_mv[self.id+1]:become("idle") 
  end
 end
end

ou=entity:extend({
 sprite={idle={24}},
 hitbox=box(0,0,8,3),
 draw_order=0,
 collides_with={"guy","walls"}
})

ou:spawns_from(24)

function ou:init()
 self.vel=v(0,0)
end

function ou:idle()
 if abs(g_guy.pos.x-self.pos.x)
 <12 then
  self:become("fall")
  self.weight=0.1
 end
end

function ou:collide(o)
 if o:is_a("guy") and self.state=="fall" then
  o:die()
 elseif o:is_a("walls") then
  self:become("did")
 end
end

fly=entity:extend({
 sprite={idle={98,99,100,101,102,
 delay=7}}
})

fly:spawns_from(98)

function fly:init()
 self.t=rnd(3)
 self.start=v(self.pos.x,self.pos.y)
end

function fly:idle(t)
 t=t*0.01
 self.pos=v(
  self.start.x+cos(t*0.1)*sin(t*0.32)*8,
  self.start.y+cos(t*0.12)*sin(t*0.4)*8
 )
end

ty=entity:extend({
})

function ty:init()
 self.start=self.pos.y
 self.fn=self.sfx and cos or sin
end

function ty:idle(t)
 self.pos.y=self.start+self.fn(t*0.01)*1.5
end


function ty:render()
 local md=0
 if mx>=self.pos.x and mx<=self.pos.x+8 and my>=self.pos.y and my<=self.pos.y+8 then
  if mbb then self.pos.y=self.start end
  pal(1,plttt[1])
  if mbb and not lmbb then
   if self.sfx then
    allow_sfx=not allow_sfx
    dset(17,allow_sfx and 2 or 1)
   else
    if(allow_music) music(-1)
    allow_music=not allow_music
    
    if(allow_music) music(0)
    
    dset(16,allow_music and 2 or 1)
   end
   sfx(12)
  end
 end
 local chk=0
 if self.sfx then
  chk=allow_sfx and 0 or 32
 else
  chk=allow_music and 0 or 32
 end
 spr(
 (self.sfx and 26 or 27)+chk,self.pos.x,self.pos.y+md)
end
-->8
-- player

guy=entity:extend({
 sprite={
  idle={64,65,66,65,delay=10},
  move={67,68,69,68,delay=10},
  fall={70},
  jump={71},
  dead={71},
  flips=true
 },
 weight=0.1,
 collides_with={"walls"},
 tags={"guy"},
 hitbox=box(2,1,6,8),
 feetbox=box(2,8,6,8.1)
})

guy:spawns_from(64)

function guy:init()
 if g_guy~=nil then
  g_guy.done=true
 end

 self.sec=true
 g_guy=self
 self.vel=v(0,0)
 self.db=dget(2)==1
 
 for i=1,g_num_keys do
  e_add(key({
   tar=self,
   id=i,
   pos=v(self.pos.x,self.pos.y)
  })):become("follow")
 end
end

function guy:sv()
 dset(60,self.pos.x)
 dset(62,self.pos.y)
 restart_level()
end

function guy:die(tt)
 if self.dd then return end
 if self.state~="dead" then
  self:become("dead")
  fade(true)
 end
 
 sfx(11)
 
 self.dd=true
 self.vel.y=tt and 0.1 or -1.5 
 self.vel.x=0
end

function guy:dead()
 if self.t>30 then
  self.done=true
  fade()
  restart_level()
 end
end

function guy:idle()
 if self.dd then return end
 local speed,m=0.3
 local s=self.supported_by
 if not g_won and not btn(🅾️) then
 if (btn(⬅️)) m=true self.vel.x-=speed
 if (btn(➡️)) m=true self.vel.x+=speed
 if (self.db and (btn(❎) or btnp(⬆️)) and self.sec) then
  self.sec=false
  self.vel.y=-2.5
  sfx(14)
  parts(self.pos.x,self.pos.y,5,4,0)
 e_add(bullet({
   pos=v(self.pos.x+(
   self.flipped and -3 or 3),
   self.pos.y+2),
   vel=v(self.flipped 
   and -3 or 3,0)
  }))
 end 

 if s and self.vel.y>0 then
  if self.supported_by.collide then
   self.supported_by:collide(self)
  end
  sfx(13)
  self.vel.y=-2.3
  self.sec=true
  e_add(bullet({
   pos=v(self.pos.x+(
   self.flipped and -3 or 3),
   self.pos.y+2),
   vel=v(self.flipped 
   and -3 or 3,0)
  }))
 end
 end
 
 if self.pos.x>122 and self.vel.x>0 then
  g_index+=1
  self.pos.x=-1
  self:sv()
  return
 elseif self.pos.x<-2 and self.vel.x<0 then
  g_index-=1
  self.pos.x=121
  self:sv()
  return
 elseif self.pos.y>124 and self.vel.y>0 then
  g_index+=8
  self.pos.y=-3
  self:sv()
  return
 elseif self.pos.y<-4 and self.vel.y<0 then
  g_index-=8
  self.pos.y=110
  self:sv()
  return
 end
 
 self.was_supported_by=s
 self.vel.x*=0.8
 
 if self.vel.y>0 and not s then
  self:become("fall")
 elseif self.vel.y<0 then
  self:become("jump")
 elseif m then
  self:become("move")
 else
  self:become("idle") 
 end
end

guy.fall=guy.idle
guy.move=guy.idle
guy.jump=guy.idle
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
 if not mnadded then
  mnadded=true
   e_add(ty({
  sfx=true,
  pos=v(11,4.8)*8,
  draw_order=10
 }))
 e_add(ty({
  sfx=false,
  pos=v(12,4.8)*8,
  draw_order=10
 }))
 end
 
 if btn(❎) then
  sfx(12)
  restart_level()
  state=ingame
  fade()
  mnadded=false
 end
 
 lmbb=mbb
 mx,my,mbb=stat(32),stat(33),
 stat(34)==0
 
 e_update_all()
end

function menu.draw()
 cls(plttt[3])
 r_reset()
 map(80,16)
 r_render_all("render")
 spr(13,mx,my)
 coprint("^press ❎",95+cos(g_time*0.01)*1.5,13)
 coprint("^by @egordorichev",121,13)
end

ingame={}

function ingame.update()
 e_update_all()
 do_movement()
 do_collisions()
 do_supports()
 
 if btnp(🅾️,1) then
  g_guy=nil
  fade()
  restart_level()
 end
end

function ingame.draw()
 cls(plttt[3])
 fillp(0b111101111011110.1)
 rectfill(0,0,127,127,plttt[2])
 fillp()
 rectfill(8,8,119,119,plttt[3])
 r_render_all("render")
 
 if g_index==8 then
  coprint("^press ^q to change palette!",114+cos(g_time*0.01)*2.5,13)
 end
 
 if g_index==5 and g_guy.db then
  coprint("^press ❎ to doublejump!",120+cos(g_time*0.01)*2.5,13)
 elseif g_index==17 and g_guy.pos.y>64 then
  if(not g_won) sfx(18) music(-1)
  g_won=true
  local y=24+cos(g_time*0.01)*1.5
  coprint("★★★★    ",y,13)
  coprint("^you won!",y+10,13)
  coprint("★★★★    ",y+20,13) 
  
  coprint("^thanks for playing!",y+40,13) 
  coprint("^find more games",y+50,13)
  coprint("like this one on",y+60,13)
  coprint("egordorichev.itch.io",y+70,13)

  coprint("^press ❎ to continue ",y+90,13) 
  if btnp(❎) then
   sfx(12)
   for i=0,64 do
    dset(i,0)
   end
   g_won=false
   g_guy=nil
   fade()
   _init()
  end
 end
 

 
 --oprint(pli,3,3,7)
end

m()
__gfx__
33333333eeeeeeeeeeeeeeeeeeeeeeee0000000000000000e110000e000000000000000000000000eeeeeee00eeeeeee0115024511eeeeeeeeeeeeeeeeeeeeee
33333333eeeeeeeeeeeeeeeeeeeeeeeee00000000000000eee1100ee000000000000000000000000eeeeee0000eeeeee132d149d1d1eeeeeeeeeeeeeeeeeeeee
33333333eeeeeeeeeeeeeeeeeeeeeeeeee000000000000eeeee00eeeee00eeeeeeeeeeeeeee00eeeeeeee000000eeeeedb862fac1dd1eeeeeeeeeeeeeeeeeeee
33333333eee1eeeeeeeeeeeeeeeeeeeeeee0000000000eeeeee10eeee00eeeeeeeeeeeeeeeee00eeeeee00000000eeeeeeeeeeee1ddd1eeeeeeeeeeeeeeeeeee
33333333eee0eeeee1eeeeeee1eeee1eeeee00000000eeeeeee10eee00eeeeeeeeeeeeeeeeeee00eeee0000000000eeeeeeeeeee1dddd1eeeeeeeeeeeeeeeeee
33333333e1e0eeeee0eee1eee0eeee0eeeeee000000eeeeeeee00eee0eeeeeeeeeeeeeeeeeeeee00ee000000000000eeeeeeeeee1dd111eeeeeeeeeeeeeeeeee
33333333e0e0ee1eee0e0eeee0ee1e0eeeeeee0000eeeeeeeee00eeeeeeeeeeeeeeeeeeeeeeeeee0e00000000000000eeeeeeeeee11d1eeeeeeeeeeeeeeeeeee
33333333e0e0e00eee0e0e1ee00e0e0eeeeeeee00eeeeeeeeee00eeeeeeeeeeeeeeeeeeeeeeeeeee0000000000000000eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeee0000000000000000000000000111111111111110eee00eeeeeeeeeee00000000eeeeeeeeeeee11eeeeeeeeee1e1e1e1eeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeee00000010000000000000000011ddd1dddddddd11eee10eeeeeeeeeeee000000eeeeeeeeeeeee111eee11111ee1e1e1e1eee0eeeeeeee0eeeeeeeeeee
eeeeeeee0000010000100000000000001d11111111111111eee10eeeee0eee0eee0000eeeeeeeeeeeeee1eeeee1eee1e1e1e1e1eee00eeeeeeee00eeeeeeeeee
eeeeeeee0100000001010100000000001d11111111111111eee10eeee010e010eee00eeeeeeeeeeeeeee1eeeee1eee1ee1e1e1e1e000000ee000000eeeeeeeee
eeeeeeee0010000000101000000000001d11111111111111eee00eeee010e010eeeeeeeeeeeeeeeeeeee1eeeee1eee1e1e1e1e1eee00eeeeeeee00eeeeeeeeee
eeeeeeee0100000000000100000000001d11111111111111eee00eee01110111eeeeeeeeeeeeeeeeee111eeeee1eee1ee1e1e1e1eee0eeeeeeee0eeeeeeeeeee
eeeeeeee0000000000000000000000001d11111111111111eee10eee01110111eeeeeeeeeeeeeeeee1111eee111e111e1e1e1e1eeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeee0000000000000000000000001d11111111111111eee10eee11111111eeeeeeeeeeeeeeeee111eeee111e111ee1e1e1e1eeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeee0e0e0e0e0e0e0e0e10e0e0e1d11111111111111eee10eee11111111eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeee1e0eee1e0eee0e1ee0eee1e1111111111111101eee00eee01110111eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee000eeeee000eeeeeeeeeee
eeeeeeeeeee0eeeee0eee1eeee1eeeee1d11111111111011eee1eeee01110111eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee0eee0eee0eee0eeeeeeeeee
eeeeeeeeeee1eeeee1eeeeeeeeeeeeee1d11111111111101eeeeeeeee010e010eeeeeeee0000000000000000eeeeeeeeeeeeeeeee0eeeeeee0eee0eeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee1111111111111011eeeeeeeee010e010eeeeeeeee00000000000000eeeeeeeeeeeeeeeeee0eeeeeee0e0e0eeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee1d11111111010101eeee0eeeee0eee0eeeeeeeeee00000000000000eeeeeeeeeeeeeeeeee0eee0eee0ee0eeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee1111111110101011eee10eeeeeeeeeeeeeeeeeeee00000000000000eeeeeeeeeeeeeeeeeee000eeeee00e0eeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee0111111111111110eee10eeeeeeeeeeeeeeeeeeee00000000000000eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeee0111111001111110011111100dd11dd0eeeeeeeeeee00eeeeeeeeeeeeeeeeeeeeeeeeeee0eee11ee0eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeee11dddd1111d1dd1111d1dd11d110d110eeeeeeeeeee00eeeeeeeeeeeeeeeeeeeeeeeeeeee0ee111ee011111eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeee1d1111111d1111011d111101d110d110eeeeeeeeeee00eeeeeeeeeeeeeeeeeeeeeeeeeeeee0e1eeeee0eee1eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeee1d1111011d1111011d10110110001001eeeeeeeeeee10eeeeeeeeeeeeeeeeeeeeeeeeeeeeee01eeeee10ee1eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeee111110111d1111011d11d1011dd10dd1eeeeeeeeeee10eeeeeeeeeeeeeeeeeeeeeeeeeeeeeee0eeeee1e0e1eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeee1d1101011d1111011d111101d110d110eeeeeeeeeee00eeeeeeeeeeeeeeeeeeeeeeeeeeeee1110eeee1ee01eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeee111010111100011111000011d110d110eeeeeeeeee1100eeeeeeeeeeeeeeeeeeeeeeeeeee1111e0e111e110eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeee01111110011111100111111000011000eeeeeeeee110000eeeeeeeeeeeeeeeeeeeeeeeeee111eee0111e1110eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee111111ee00000000ee000eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eee00eeeeeeeeeeeeeeeeeeeeee00eeeeee00eeeeee00eeeeee00eeeeee00eee111111ee0000000000000000ee00eeeeeeeeeeeeee0000eee000000eeeeeeeee
eee00eeeeee00eeeeee00eeeeee00eeeeee00eeeeee00eee0ee00ee00ee00ee0eeeeeeee000000000000000000000000eeeeeeeeeee00eeeee0000eeeeeeeeee
ee0000eeee0000eeeee00eeeee0000e0ee0000ee0e0000eee000000ee000000eeeeeeeeee000000e0000000000000000eee00eeeeeeeeeeeeeeeeeeeeeeeeeee
e000000e000000000e0000e0e000000e00000000e000000eee0000eeee0000eeeeeeeeeeeeeeeeeee0eee00e00ee0000eeee0eeeeeeeeeeeeeeeeeeeeeeeeeee
0e0000e0ee0000eee000000e0e0000eeee0000eeee0000e00e0000e0ee0000eeeeeeeeeeeeeeeeeeeeeeeeeee0eee00e00eeeeeeeeeeeeeeeeeeeeeeeeeeeeee
ee0000eeee0000eeee0000eeee0000eeee0000eeee0000eee000000eee0000eeeeeeeeeeeeeeeeeeeeeeeeeeeeeee0ee00eee00eeeeeeeeeeeeeeeeeeeeeeeee
ee0ee0eeee0ee0eeee0ee0eee0eeee0eee0ee0eee0eeee0eeeeeeeeee0eeee0eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee00eeeeeeeeeeeeeeeeeeeeeeeee
e111111eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee111111e000000000e0e0e0e00000000bbbbbbbbeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
e111111eeeeeeeeeeeeeeeeeee1111eeeee00eeeeee00eeeeee00eeee111111e00000000eeeeeee000eeee00beeeeeebeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
e111111e0000eeeeee1111eee111111eee0110eeee0110eeee0110eee111111e000000000eeeeeee0eeeeee0beeeeeebeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
e111111e0e000000e111111ee111111eee01100eee0110eee00110eee11ee11e00000000eeeeeee00eeeeee0beeeeeebeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
e111111e0ee0ee0011111111e111111ee0111110e011110e0111110ee11ee11e000000000eeeeeee0eeeeee0beeeeeebeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
e111111e0000eee011111111e111111ee011110e01111110e011110ee111111e00000000eeeeeee00eeeeee0beeeeeebeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
e11ee11eeeeeeeee11111111e111111e0111110ee011110ee0111110e111111e000000000eeeeeee00eeee00beeeeeebeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
e11ee11eeeeeeeeee111111ee111111ee0100010e010010e0100010ee11ee11e00000000e0e0e0e000000000bbbbbbbbeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
e11ee11eeee00eeeeeeeeeeeeeeeeeeeee0000eeeeeeeeee0eeeeee0e11ee11eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
e11ee11eee0110eeeeeeeeeeeeeeeeeee000000ee000000ee000000ee111111eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
e111111ee011110e00eeee00e000000e0ee00ee00e0000e0ee0000eee111111eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
e111111e01111110e000000ee000000eeeeeeeeeeee00eeeeee00eeee11ee11eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
e111111e00011000ee0000ee0ee00ee0eeeeeeeeeeeeeeeeeeeeeeeee11ee11eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
e111111eee0110eeeee00eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee111111eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
e111111eee0110eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee111111eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
e111111eee0000eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee111111eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
31314231313131313141513143233131313131233131313131314151312331312111113101010140313131313131313131313131313111313131313133313131
41515001403143313131425231312131312331313343313131113131313113313131314151313150010101014151313131313131425250010101014043311313
31314331313131433142523131415131313131313131311331314252313131313131415101010101120122403143313111315001220122323212014031313131
42520101013131313131313131313331133131313131313111314151313131313131314252312101012601014252313111311331433101010101010140313131
31313131313150010101014031425231133150602212220122220132604031313131425201010110010126010131313131500115010101010101010132314151
31500101a03131315060322212403131500101010140312331314252313101403113313131313101010101014113313143312131315001260110200101133121
43313150010101260101010101014031313101610101010101010101610143313121011201014151013001010113723131010101013020013001200101404252
310101a03131503201610101010160120101150101010140314331213131b0013131415131315001010101013113313131313150220101010190500101313131
313131010101010101010101010101313150016101010101010101016101313131310101010142523131b0200101013131b00120a03131808031310101014031
31b0a0315022010101630130010163302010010101010101313123312331313131314252313101201030100140131341513123010101301001200130a0415121
31315001010101301001450110010131500101610101010101010101610131312131010101010140113131310101013143313131432150010113310101010131
21313131010101010131313131313131313113b02020300131233131311131311331313131500131313131010131314252211301904151313131311331425211
41510101010180808080808080010175010101610101010101010101610113313131010101010101313143330101013131312133500101010131310101010131
21313150010101010140313131312131333131312331500131313131314151313131313131010131324031010131313131315001014252312150011222403131
4252010101010101010101010120100101010161010101010101010161013131433101010101010132403111700101314151500101010101a02350010101a031
31503201010101b50101122240313131313150120132010150010160404252313131213150010133431031010140233121500110450130323201010101014151
3131010101010145300101010140334331b0016101010101010101016101313131310101010101010101403101010131425201010110a0313150010101012141
51010126010101010101010101123260221201010101010101010161012240313123313101260140333131010101314151010140313131315001010101404252
3131010101808080800101010101313131310161010101012601010161014151312301010101010101010131b001013131310110a03131415101010101013342
5201010101b501010101010101010161010101012601010101010161010101403131315001010101403150010101404252010101120101320101260101014031
13310101010101010101010101014031313101610101010101010101610142523131010101010101010101403170024011508090313111425201010101013123
31010101010101010101010101010162010101010101010101201063010101011232600101010101013201010101016075010101012030014501010145010113
1331b001010101102010014501010131415101610101010101010101610131313131b00101010101010101011201010101010101120132120101010101013131
3101b50101010101010101010101016101010101010101010133312331b020010101631030200101201030203010206301103001a031133131b0010180809031
31313101010180808080808080808031425201610101010101010101610131233131312030100101010101200110010101010101010101010101010101303131
3301010101b5010101b50101a5010162b50101b50101a50101403131312121312131313331709031213131312131313131213141513121433131010101010131
313131010101010101010101010101313123b06310300192a201201063a03131312131313131717171713131312331b0202001010101014530200130a0433123
3101010101010101010101013010206330010101010101010101050122403123313131333101013131313133313141513113314252312150127501010120a031
2131310101102010201010102010a043313131314331313131314151313131313121313131213131415131313123313131433121237171133131313131313131
1371717171717171717171711343313113717171717113b0102001012601403131415131317090314151313131314252313131313143310101010110a0311331
31313170809031313131311331313131313131313131314331314252313131333131312331313131425231313131313131313131313141513143312131313131
313133313131313331313121313131313131233131314151312331b0010101313142523131012031425231313131313131314151313111010133415131333131
41513170809031312331313131233131133113314331333131433131312331313143313131313131313131313131333131313131313142523141513131313141
51313131314151313131313133313131314321313131425231213131010101213131313131709031313131213131313131314252433121010140425231313121
42523101010140313131313131313131313150128140315081404350814031233131313131433131313131313131313131314350321240313142525022124042
52500122124252011232404151312131415121313143502240313150010101403131333131200131433131313131333141513131314151010101401133213131
31313110010101818181818181814031315001010101120101013201010131313131313131313131313133314151313143313101260101014031310101010160
12010101013212010101014252312121425231333150010101320101010101014031434331709031313141514343313142523131114252010101012232403133
31133170800101010101010101010131130101010101010101010101010132010181124031313131313131314252313131315001010101010123430101010161
01010101010101010101014031311331313131312101010101010101010101260132403133010131313142523131314331313113313121012020010101013131
3131310101012601010101010101014050010101010101010101010101010101010101018122812212403131313131433131b00101010101a031310101010161
01010101010101010101010122403131315001010501010101010101010101010101013131709031313131315012126012322240313311311333b00101013331
213131013001010101010101010101010101010101010101010101010101012601010101010101010101018122403131233131b0203010013131500101010161
01010101260101010101010101013141510101260101010101011001013020300101011331013041513143120101016101010101324013313141510101011131
31313170800101010101010101010101010101012601010101010101010101010101010101010101010101010101814031313331415131313133010126010161
0101010101010101010101010101404252010101403131313131214151313121b001013113709042523131010101016301010101010112224042520101013111
31313101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010122403131425233313150010101010161
01010101010101010101010101010131110101010101010101014042523113313101012131302031313321010101a011b0010101010101010122120101014151
31415110010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101013295012232228501010101010161
01010101010101010101010101010131310101010101010101010132403111313301013111709031313131010101334331010101012601010101010101014252
31425231b02001010101010101010101010101010101010101010101010101010101010101010101010101010101010101010195010101018501010101013063
10300101010101010101010101010131310101010101010101010101014031313101013331200131431311010101313121019501010101010101010101013311
313131313131b0300101010101010101010101010101010101010101010130102030102010200101010101010101010101010195010126018501010101013143
33310101010101010101010101010113500101010101010101010101010131433101013331709031313131010101314151010101010101010101010101013131
3131311331313131b01001010101010101010101010101010101010101013131415123313331b00120302030010101010101b595010101018501010101014331
4151010101010101010101010101016001010101010101010101010101a033313101013143010133312143010101314252010101850195018501010101011133
31313131313141513131b0103001102010010101300101010101010120013131425231313131313143312331b001203001010195940101b58594010101014031
425201850195019401850194018501610101010101010101010101010131312131010140317090313143500101a0213111010101010101010101950101013131
313131313131425231313131313131313101010123010101940101011301313131333131313123313131313131333141510101950101010185010101a5010131
315001010101010101010101010101633001850101010101010101010113313131b0010160010112601201010121314331010101010101010101011020302111
31313131134323313131311323313131237171717171717171717171717143313131313131313131415131333131314252717171717171717171717171717141
51717171717171717171717171712331437171717171717171717171713331333131b0306310302063102030a041513113717171717171717171711311313141
51313131313131313131313131415131313131313131313131313131313123313131313141513131425231313131313131313131313131313131313131313142
52311331311331313131415131313131311131433123313121314331313131414331333131213131333143313142523121313321314151313133213141513142
__label__
0000000000000000000000000000000001111111111111100000000001111110000000000000000000000000000000000dd11dd00dd11dd00000000000000000
0000000000000000000000000000000011ddd1dddddddd110000000011d1dd1100000000000000000000000000000000d110d110d110d1100000000000000000
000000000000000000000000000000001d11111111111111000000001d11110100000000000000000000000000000000d110d110d110d1100000000000000000
000000000000000000000000000000001d11111111111111000000001d1111010000000000000000000000000000000010001001100010010000000000000000
000000000000000000000000000000001d11111111111111000000001d111101000000000000000000000000000000001dd10dd11dd10dd10000000000000000
000000000000000000000000000000001d11111111111111000000001d11110100000000000000000000000000000000d110d110d110d1100000000000000000
000000000000000000000000000000001d11111111111111000000001100011100000000000000000000000000000000d110d110d110d1100000000000000000
000000000000000000000000000000001d1111111111111100000000011111100000000000000000000000000000000000011000000110000000000000000000
000000000111111000000000000000001d1111111111111100000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000011d1dd110000000000000000111111111111110100000000000000000000000000000000000000000000000000000000000000000000000000000000
000000001d11110100000000000000001d1111111111101100000000000000000000000000000000000000000000000000000000000000000000000000000000
000000001d11110100000000000000001d1111111111110100000000000000000000000000000000000000000000000000000000000000000000000000000000
000000001d1111010000000000000000111111111111101100000000000000000000000000000000000000000000000000000000000000000000000000000000
000000001d11110100000000000000001d1111111101010100000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000110001110000000000000000111111111010101100000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000011111100000000000000000011111111111111000000000000000000000000000000000000000000000000000000000000000000000000000000000
111111100000000000000000d0d0d0d0d10d0d0dd0d0d0d0d0d0d0d0d10d0d0d0000000001111110000000000000000001111110011111100000000000000000
dddddd11000000000000000dd1d0ddd1dd0ddd1dd0ddd0d1d1d0ddd1dd0ddd1dd000000011dddd11000000000000000011d1dd1111d1dd110000001000000000
1111111100000000000000ddddd0dddddd1dddddd0ddd1ddddd0dddddd1ddddddd0000001d11111100000000000000001d1111011d1111010000010000000000
111111110000000000000dddddd1ddddddddddddd1ddddddddd1ddddddddddddddd000001d11110100000000000000001d1011011d1011010100000000000000
11111111000000000000dddddddddddddddddddddddddddddddddddddddddddddddd00001111101100000000000000001d11d1011d11d1010010000000000000
1111111100000000000dddddddddddddddddddddddddddddddddddddddddddddddddd0001d11010100000000000000001d1111011d1111010100000000000000
111111110000000000dddddddddddddddddddddddddddddddddddddddddddddddddddd0011101011000000000000000011000011110000110000000000000000
11111111000000000dddddddddddddddddddddddddddddddddddddddddddddddddddddd001111110000000000000000001111110011111100000000000000000
1111111100000000ddddddddddddddddddddddddddddddddddddddddddddddddddddddddd0d0d0d0d0d0d0d00000000000000000000000000000000000000000
1111110100000000ddddddddddddddddddddddddddddddddddddddddddddddddddddddddd0ddd0d1d1d0ddd1d000000000000000000000000000000000000000
1111101100000000ddddddddddddddddddddddddddddddddddddddddddddddddddddddddd0ddd1ddddd0dddddd00000000000000000000000000000000000000
1111110100000000ddddddddddddddddddddddddddddddddddddddddddddddddddddddddd1ddddddddd1ddddddd0000000000000000000000000000000000000
1111101100000000dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd000000000000000000000000000000000000
1101010100000000ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd00000000000000000000000000000000000
1010101100000000dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd0000000000000000000000000000000000
1111111000000000ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd000000000000000000000000000000000
0000000000000000ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd0d0d0d0d10d0d0d0000000000000000
000000000000000dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd1d0ddd1dd0ddd1dd000000000000000
00000000000000ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd0dddddd1ddddddd00000000000000
0000000000000dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd1ddddddddddddddd0000000000000
000000000000dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd000000000000
00000000000ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd11ddddddddddddddddddddddd00000000000
0000000000dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd111ddddddddddddddddddddddd0000000000
000000000ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd1ddddd11111dddddddddddddddd000000000
00000000dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd1ddddd1ddd1ddddddddddddddddd00000000
00000000dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd1ddddd1ddd1ddddddddddddddddd00000000
00100000dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd111ddddd1ddd1ddddddddddddddddd00000000
01010100ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd1111ddddd1ddd1ddddddddddddddddd00000000
00101000ddddddddddddddddd1dddd1dd1ddddddddddddddddddddddddddddddd1ddddddddddddddddddddddd111dddd111d111ddddddddddddddddd00000000
00000100ddddddddddddddddd0dddd0dd0ddd1ddddddddddddddddddddddddddd0ddd1dddddddddddddddddddddddddd111d111ddddddddddddddddd00000000
00000000ddddddddddddddddd0dd1d0ddd0d0ddddddddddddddddddddddddddddd0d0ddddddddddddddddddddddddddddddddddddddddddddddddddd00000000
00000000ddddddddddddddddd00d0d0ddd0d0d1ddddddddddddddddddddddddddd0d0d1ddddddddddddddddddddddddddddddddddddddddddddddddd00000000
00000000dddddddddddddddd0000000000000000dddddddd0ddddddddddddddd00000000dddddddd0dddddddddddddddddddddddddddddd0dddddddd01111110
00000000ddddddddddddddddd000000000000000dddddddd00dddddddddddddd00000000dddddddd00dddddddddddddddddddddddddddd00dddddddd11d1dd11
00000000dddddddddddddddddd00000000000000dddddddd000ddddddddddddd00000000dddddddd000dddddddddddddddddddddddddd000dddddddd1d111101
00000000ddddddddddddddddddd0000000000000dddddddd0000dddddddddddd00000000dddddddd0000dddddddddddddddddddddddd0000dddddddd1d101101
00000000dddddddddddddddddddd000000000000dddddddd00000ddddddddddd00000000dddddddd00000dddddddddddddddddddddd00000dddddddd1d11d101
00000000ddddddddddddddddddddd00000000000dddddddd000000dddddddddd00000000dddddddd000000dddddddddddddddddddd000000dddddddd1d111101
00000000dddddddddddddddddddddd0000000000dddddddd0000000ddddddddd00000000dddddddd0000000dddddddddddddddddd0000000dddddddd11000011
00000000ddddddddddddddddddddddd000000000dddddddd00000000dddddddd00000000dddddddd00000000dddddddddddddddd00000000dddddddd01111110
00000000dddddddddddddddddddddddd00000000dddddddd00000000dddddddd00000000dddddddd000000000dddddddddddddd000000000dddddddd00000000
00000000dddddddddddddddddddddddd00000000dddddddd00000000ddddddddd000000ddddddddd0000000000dddddddddddd0000000000dddddddd00000000
00000000dddddddddddddddddddddddd00000000dddddddd00000000dddddddddd0000dddddddddd00000000000dddddddddd00000000000dddddddd00000000
00000000dddddddddddddddddddddddd00000000dddddddd00000000ddddddddddd00ddddddddddd000000000000dddddddd000000000000dddddddd00000000
00000000dddddddddddddddddddddddd00000000dddddddd00000000dddddddddddddddddddddddd0000000000000dddddd0000000000000dddddddd00000000
00000000dddddddddddddddddddddddd00000000dddddddd00000000dddddddddddddddddddddddd00000000000000dddd00000000000000dddddddd00000000
00000000dddddddddddddddddddddddd00000000dddddddd00000000dddddddddddddddddddddddd000000000000000dd000000000000000dddddddd00000000
00000000dddddddddddddddddddddddd00000000dddddddd00000000dddddddddddddddddddddddd00000000000000000000000000000000dddddddd00000000
00000000dddddddd0ddddddddddddddd00000000dddddddd00000000dddddddd00000000dddddddd00000000000000000000000000000000dddddddd00000000
00000000dddddddd00dddddddddddddd00000000dddddddd00000000dddddddd00000000dddddddd00000000d00000000000000d00000000dddddddd00000000
00000000dddddddd000ddddddddddddd00000000dddddddd00000000dddddddd00000000dddddddd00000000dd000000000000dd00000000dddddddd00000000
00000000dddddddd0000dddddddddddd00000000dddddddd00000000dddddddd00000000dddddddd00000000ddd0000000000ddd00000000dddddddd00000000
00000000dddddddd00000ddddddddddd00000000dddddddd00000000dddddddd00000000dddddddd00000000dddd00000000dddd00000000dddddddd00000000
00000000dddddddd000000dddddddddd00000000dddddddd00000000dddddddd00000000dddddddd00000000ddddd000000ddddd00000000dddddddd00000000
00000000dddddddd0000000ddddddddd00000000dddddddd00000000dddddddd00000000dddddddd00000000dddddd0000dddddd00000000dddddddd00000000
00000000dddddddd00000000dddddddd00000000dddddddd00000000dddddddd00000000dddddddd00000000ddddddd00ddddddd00000000dddddddd00000000
01111110dddddddd000000000000000000000000dddddddd00000000dddddddd00000000dddddddd00000000dddddddddddddddd00000000dddddddd00000000
11d1dd11ddddddddd0000000000000000000000ddddddddd00000000dddddddd00000000dddddddd00000000dddddddddddddddd00000000dddddddd00000000
1d111101dddddddddd00000000000000000000dddddddddd00000000dddddddd00000000dddddddd00000000dddddddddddddddd00000000dddddddd00000000
1d111101ddddddddddd000000000000000000ddddddddddd00000000dddddddd00000000dddddddd00000000dddddddddddddddd00000000dddddddd00000000
1d111101dddddddddddd0000000000000000dddddddddddd00000000dddddddd00000000dddddddd00000000dddddddddddddddd00000000dddddddd00000000
1d111101ddddddddddddd00000000000000ddddddddddddd00000000dddddddd00000000dddddddd00000000dddddddddddddddd00000000dddddddd00000000
11000111dddddddddddddd000000000000dddddddddddddd00000000dddddddd00000000dddddddd00000000dddddddddddddddd00000000dddddddd00000000
01111110ddddddddddddddd0000000000ddddddddddddddd00000000dddddddd00000000dddddddd00000000dddddddddddddddd00011000dddddddd00000000
00000000ddddddddddddddddd10d0d0dddddddddddddddddd0d0d0d0ddddddddd10d0d0dddddddddd0d0d0d0ddddddddddddddddd0d1d1d0dddddddd01111110
00000000dddddddddddddddddd0ddd1dddddddddddddddddd0ddd0d1dddddddddd0ddd1dddddddddd1d0ddd1ddddddddddddddddd1d1dd11dddddddd11d1dd11
00000000dddddddddddddddddd1dddddddddddddddddddddd0ddd1dddddddddddd1dddddddddddddddd0ddddddddddddddddddddddd1ddd1dddddddd1d111101
00000000ddd1ddddddddddddddddddddddddddddddddddddd1ddddddddddddddddddddddddddddddddd1ddddddddddddddddddddddd1dddd1ddddddd1d111101
00000000ddd0ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd1dd111ddddddd1d111101
00000000d1d0dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd11d1dddddddd1d111101
00000000d0d0dd1ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd11000111
00000000d0d0d00ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd01111110
00000000000000000ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd00000000
000000000000000000dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd00000000
0000000000000000000ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd00100000
00000000000000000000dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd01010100
000000000000000000000dddddddddddddddddddddddddddd00000dddddddddddddddddddd0000000ddddddddddddddddddddddddddddddddddddddd00101000
0000000000000000000000ddddddddddddddddddddddddddd0ddd00000000000000000ddd00ddddd00dddddddddddddddddddddddddddddddddddddd00000100
00000000000000000000000dddddddddddddddddddddddddd0d1d0ddd0ddd00dd00dd0ddd0dd1d1dd0dddddddddddddddddddddddddddddddddddddd00000000
000000000000000000000000ddddddddddddddddddddddddd0ddd0d1d0dd10d110d110ddd0ddd1ddd0dddddddddddddddddddddddddddddddddddddd00000000
000000000111111000000000ddddddddddddddddddddddddd0d110dd10d10010d010d0ddd0dd1d1dd0ddddddddddddddddddddddddddddddddddddd001111111
0000000011dddd1100000000ddddddddddddddddddddddddd0d000d1d0ddd0dd10dd10ddd01ddddd10dddddddddddddddddddddddddddddddddddd0011ddd1dd
000000001d11111100000000ddddddddddddddddddddddddd010d01010111011001100ddd001111100ddddddddddddddddddddddddddddddddddd0001d111111
000000001d11110100000000ddddddddddddddddddddddddd000d0000000000000000ddddd0000000dddddddddddddddddddddddddd1dddddddd00001d111111
000000001111101100000000ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd0ddddddd000001d111111
000000001d11010100000000ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd1d0dddddd0000001d111111
000000001110101100000000ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd0d0dd1dd00000001d111111
000000000111111000000000ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd0d0d00d000000001d111111
00000000000000000dd11dd00dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd0000000000dd11dd01d111111
0000000000000000d110d11000dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd0000000000d110d11011111111
0000000000100000d110d110000dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd00000000000d110d1101d111111
0000000001010100100010010000dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd000000000000100010011d111111
00000000001010001dd10dd100000dddd1ddddddddddddddddddddddddddddddddddddddddddddddddddddddd1ddddddddd00000000000001dd10dd111111111
0000000000000100d110d110000000ddd0ddd1ddddddddddddddddddddddddddddddddddddddddddddddddddd0ddd1dddd00000000000000d110d1101d111111
0000000000000000d110d1100000000ddd0d0ddddddddddddddddddddddddddddddddddddddddddddddddddddd0d0dddd000000000000000d110d11011111111
00000000000000000001100000000000dd0d0d1ddddddddddddddddddddddddddddddddddddddddddddddddddd0d0d1d00000000000000000001100001111111
00000000011111111111111000000000000000000dddddddddddddddddddddddddddddddddddddddddddddd00000000000000000000000000111111000000000
0000000011ddd1dddddddd11000000000000000000ddddddddddddddddddddddddd00ddddddddddddddddd0000000000000000000000000011dddd1100000000
000000001d111111111111110000000000000000000dddddddddddddddddddddddd00dddddddddddddddd0000000000000000000000000001d11111100000000
000000001d1111111111111100000000000000000000dddddddddddddddddddddd0000ddddd1dddddddd00000000000000000000000000001d11110100000000
000000001d11111111111111000000000000000000000dddd1dddd1dd1ddddddd000000dddd0ddddddd000000000000000000000000000001111101100000000
000000001d111111111111110000000000000000000000ddd0dddd0dd0ddd1dd0d0000d0d1d0dddddd0000000000000000000000000000001d11010100000000
000000001d1111111111111100000000000000000000000dd0dd1d0ddd0d0ddddd0000ddd0d0dd1dd00000000000000000000000000000001110101100000000
000000001d11111111111111000000000000000000000000d00d0d0ddd0d0d1ddd0dd0ddd0d0d00d000000000000000000000000000000000111111000000000
000000001d1111111111111100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000111111111111110100000000ddd0000000000d0000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000001d1111111111101100000000d1d0d0d00000d1d0ddd0ddd00dd0ddd0dd000dd0ddd0ddd0ddd0d0d0ddd0d0d000000000001000000000000000000000
000000001d1111111111110100000000dd10ddd00000d0d0dd10d110d1d0d1d0d1d0d1d0d1d01d10d110d0d0dd10d0d000000000010101000000000000000000
00000000111111111111101100000000d1d011d00000d010d100d0d0d0d0dd10d0d0d0d0dd100d00d000ddd0d100ddd000000000001010000000000000000000
000000001d1111111101010100000000ddd0ddd000001dd0ddd0ddd0dd10d1d0dd10dd10d1d0ddd0ddd0d1d0ddd01d1000000000000001000000000000000000
00000000111111111010101100000000111011100000011011101110110010101100110010101110111010101110010000000000000000000000000000000000
00000000011111111111111000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000

__gff__
0000000004040002020208080000000000010101010100000000000000000000000000000101000000080800000000000001010101000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
__map__
2513331312131314153313131324251331311314151313131313131313111313131313132425131313111313141513331313131312133313131313131331131313131313131313131313242513131415133113131213131313141513131313333313131332341313133413131113341311131331132425311311131224251214
1313132322220424251313342727131313131324251021221004341313131313131311131333102106211004242513111331311313320523042413321313131313121313131415131313321313132425131313130522211004242505101010101010101010101010101004131313131313141513131313131313131313111324
3411051010101013131333101010311313131305101010621010131313131314151333130521101026101010102113131313130521221010100623210413311332130523042425131305212110102104131205101010101010102310101010101010101010101010101010041331131313242505212323220431052304131313
132110101062100413110510101013141513051010101010010a131311131324251305211010101036101010101031131332221051101010101610101021041313051010101013132110101010101010222110101010101010101010101114341313131415121310101010101313131113052210101010101023101010220431
130b10101010101022221010101013242513101010100a311313131313131313132110101003101415101010101004131313031010101010021610101010101313101010101013131061101002100102031010101010101010101010100432251313122425131310101010100413133313101010511010101001021010101013
141510101010101d1e1010101010131213131010030a13131312131313341313131062100809132425101010101010133413051010101008081610101010105723101010101013120b01030214151313130b101010101049101010101010221313131313131305101010101010131313130b0310101002010a13120b10101011
242510024010101010101010101032131332101014151313331313111313131313101010101034131310101010101013131310101062101010161010100103100310106210100413131313132425133313321010101010101010101010101013121331130510101010101010101415131314150b020a13131311121310101011
13131313130b101010106210101013341305101024252123100413131313131305101010020a131313101010101010141513540210101010101610101008091313131010101010311333131313131213141510491049101010101307101010131313131310101010101010101024251313242513131314151213130510101012
1112311313330310101010101010131313101010100610101010210610222110101010080913133205101010101010242513070808101010101310105403013312131010101010211006040523041313242510101010101010100410101002131313130510101010101010101013131313131312323224251313121062101013
131313131213130b101010101010041305102d10101610101010102662101003020203100a1113131010621010100a1313130b10011054030a1210100808091313321049101010101016101010101331130510101010101010101010101014151312131010101010101010101013133132121113131313131313051010101013
1321041313331305101010101010105010101010101610101010101610011415131334131333131310101010101034131314151008080809320510101010011313131010101010101016101010101313121010101062101010101010100924251313131010101010101010101013133313121305102321212223101010100a12
131010212210061010101002030301101010020303361010101010360a132425131213131305210610101010101013331324250b1010101006101010010a13131313171010491049101610101010133205101010101010101010101010101313131313101010106210101010101313131313130b101010101010101010031313
130b10511010161010100a13131331131313331213133117171731131313131333051021231010161010101010103312133112130b5410013601030a131313131214151010101010103602101010130510101010101010101010101049103113111113101010101010104910101313111113311310101010101010010a131415
13320b0301033603030a13141513121313131313131313131314151313311113131010101010101610101010101013131313131313131313131415131313313213242510101010100a13330b101050101010104910104910104910101010121313131310101010101010101010131313131111130b10101010100a3113132425
1313133113131313131111242513131313131113131333131324251313131334131010100354023603101002020a13131113131313133313132425131213131313133217171717173212131301030210021033171717171717171717171714151313131010101010171717171713131332131314150b03101002131313133212
13131415133413331313131313131313131313141513131311131313331313131310100a33131113130b0a33131311131313131313131331121313131313131213131332131311131313131307093233323313141513131132321331131324251331131010101010131313131314151313121324251307101009141513131313
1313242513131313131313131313131313121324251313121113131333131313321010131334131213341313121334131313131213111313131313311313111313311313131334131312131310023413131313121415133213131313343413131313131010101010131213331324251313111313130510101010242512321334
1313131313131313131313131313131313131313131313052223041312131313131010321313312727272733131313131313141513331313311113131314151313121334131313133313133307091313131312122425131313131313131313131313131010101010041313131213131313133213051010101010041331131313
1313121313131313121313311313131313321313052322101010102104131331131010130510221010101004141513131313242505232206041234272724252204131313133411133413051010101314151305212322212304311313333311131312131062101010101313131313121314151313101049101010101010100411
1313131313321313131313131313131313131205101010015410101010220413131010231010101010101010242513341313132110101016102310101010061010232321101004131305101008091224251310101010101010222104131313131415131010101010101313131313131324251305101010101010101010101013
1313131313051010041313131314151313131310101010121310080810101014151010101010101010101010211004311312131010101036101010101010161010101010101010131310101010101334130510101010101010101010212304132425131010101010101313141513333213141510101010101049101010101031
1313131313101010102104131324251313141554011010231010101010030324250b10030110621002105410100210133112310b101033130b1010101010361010101010101010133310101010021213121010030210101002101010101010131313131010171710101313242513131313242510106210101010101010101013
131305210610101010101021041313131324251307081020100354030a331415131333070810100a3211311307080913131313131311111313341313131213130b10101010101012120b0210080913131310100413100b1013100b10100a10331313131010131310101313131313131313131310101010101010101010104912
131302101610106210101010100610041305062210101020041313331313242512331310101010131415130510101014153113131213131313133113331331131415100102010a13131113101010331313101010131013101810130b0a1310131331131010131310101312131313131332130510101010491010101010101013
131331101610101010101010101610105710161062101010101010102306041305062210101010042425051010101024251334131313312731131314151313122425111313131313131305100110131313100b101310131013101304051310131313131010131310101313111312131331131010101017171717104910101013
1313310b16101010101010101036021001103610101008101010101010161057102610101010101006231010101010131313133305220610101021242513131313131113331334120523101008091313321004130510131013101310101310131313131010131310100413131313131113131010101004131305101010101013
1313131336101010101010100a1312131313130b101010101010101010361001103610101062101016101010101010502350222210102610101010061022230431131333131213051010621010103413130110231010221023102110102110321313051010131310101050101332311314151010101010101010101010491013
12131313131010511010100213131313131332130b541010100110100a3413131313101010101010161010101010021002100110105436021010102610101010230413131311131010101003100a131213130b101010101010101010101010121313101010131317101010101313131324251049101010101010101010101031
131313131307080808080931111311141513131313131313133213131113141513120b101010101036015410030a13341415131313341313320b1036100210101010311313130510101010080913131213311310101010101010101010010a141513105110131313131313101313131313131010101010101010101010101013
1313131313331732171734131213112425131313131334131313131313132425131313101010100a111313321314151324251313131313131313131233130b021010061021061010101010100a3411131312340b02101010101010020a1334242513101010131313131305101312311313131051104910101010101049101012
131313131313131313131313131313131313311313131313131313121313131334133101101010131415131213242513131313131213131334133113131313130b103601023610031054100a1313133113141513130b030240010a13131331133313171717111313051010101313131314151002100103101010013217171713
1313131313311313131313131213131313131313131313131313131313133413131313131010091324251213131313131332131133131113331313131333131213131313111313133313141512131113132425131313131313131313131213131313131313131313101010101313131324251334141513101008091313111313
__sfx__
0118002006e3106e310b0330b03306e3106e312f61506e000be332f6150de050be332fe052f6352fe050be3306e3106e310b0330b03306e3106e312f61506e000be332f61506e020be332fe042f6142f6150be33
0118002017f1417f1217f1117f1517f1417f1515f1417f151cf141cf121cf111cf151cf141cf1517f141cf151af141af121af111af151af141af1517f141af1515f1415f1215f1112f1512f1412f1512f1415f15
0118002017f240bf2217f2117f2517f2417f2215f2117f251cf2410f221cf211cf251cf241cf220bf211cf251af240ef221af211af251af241af220bf211af2515f2409f2215f2112f2512f2412f2206f2115f25
011800200b325003250b3250b3250b3250b325093250b32510325043251032510325103251032500325103250e325023250e3250e3250e3250e325003250e3250932500325093250632506325063250032509325
011800200e5171a5110e5171a5111a5111a5110e5151a51106517125110651712511125111251106515125110b517175110b5171751117511175110b51117511105171c511105171c5111c5111c511105151c511
011800200e0121a01212112171121501217012121121a1120601212012091120e1120b0120e01209112121120b012170120e1121211210012120120e11217112100121c012121121711215112171121a1121e112
010200200c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c0720c072
010200200c1700c1700c1700c1700c1700c1700c1700c1700c1700c1700c1700c1700c1700c1700c1700c1700c1700c1700c1700c1700c1700c1700c1700c1700c1700c1700c1700c1700c1700c1700c1700c170
01180020020550e055061350b135090550b055061350e1350005506055001350213500035020550015506135000550b05502135061350405506055021350b1350405510055061350b135091550b1550e13512135
0118002012e3112e31170331703312e3112e313b63512000170333b63519505170333b6053b6353b6051703312e3112e31170331703312e3112e313b63512000170333b63512002170333b6043b6343b63517033
010c000406e7106e7006e7106e7000005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005
000d00001a1550c155071550115503105031050010500105001050010500105001050010500105001050010500105001050010500105001050010500105001050010500105001050010500105001050010500105
000800001335500305003050030500305003050030500305003050030500305003050030500305003050030500305003050030500305003050030500305003050030500305003050030500305003050030500305
010400000c31518305183150000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005
010300001125513255162551d25500205002050020500205002050020500205002050020500205002050020500205002050020500205002050020500205002050020500205002050020500205002050020500205
01050000112311d231000000000100001000010000100001000010000100001000010000100001000010000100001000010000100001000010000100001000010000100001000010000100001000010000100001
011000000c21100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
01080000131151311516115181151d115271150010500105001050010500105001050010500105001050010500105001050010500105001050010500105001050010500105001050010500105001050010500105
0110000016355183551b3551d35524332243350030500305003050030500305003050030500305003050030500305003050030500305003050030500305003050030500305003050030500305003050030500305
011800200be430be450be450ee450ee432661509e430be4510e4310e452661513e4313e45266152661510e450ee43266150ee4510e4510e43266150be432661509e4309e452661509e430be45266142661509e45
0118002017325003220b3250b421174220b325093220b3251c3250432210325104211c422103250e322103251a325023220e3250e4211a4220e325153220e3251532500322093250642112422063250032209325
0118002017322003251712117121174220b32510121101211c3250432510121101211c422103250e1210e1211a325023250e1210e1211a4250e32515121151211532500325121211212112425063251712117121
011800201740117401174211742110401104011042110421104011040110421104210e4010e4010e4210e4210e4010e4010e4210e421154011540115421154211240112401124211242117401174011742117421
011800201712510125171051710510125171251010510105101250e12510105101050e125101250e1050e1050e125151250e1050e1051512512125151051510512125171251210512105171250b1251710517105
011800201730517305173251732510305103051032510325103051030510325103250e3050e3050e3250e3250e3050e3050e3250e325153051530515325153251230512305123251232517305173051732517325
011800201742117421175211752110421104211052110521104211042110521105210e4210e4210e5210e5210e4210e4210e5210e521154211542115521155211242112421125211252117421174211752117521
0118002017427234271752717527104271c4271052710527104271c42710527105270e4271a4270e5270e5270e4271a4270e5270e52715427214271552715527124271e427125271252717427234271752717527
011800200b425174220b5250b5210442510422045250452104425104220452504521024250e4220252502521024250e422025250252109425154220952509521064251242206525065210b425174220b5250b521
__music__
01 13414344
00 13414344
00 13014344
00 13014344
00 13024344
00 13024344
00 13034544
00 13034544
00 13144544
00 13144544
00 13154544
00 13154544
00 13161544
00 13161544
00 13181744
00 13181744
00 13191844
00 13191844
00 131a1844
00 131a1844
00 131b1844
00 131b1844
00 135a1844
02 135a1844

