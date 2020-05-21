pico-8 cartridge // http://www.pico-8.com
version 19
__lua__
-- corrupted space	
-- by @egordorichev

cartdata("ldjam42")

menuitem(1,"restart lvl",function()
 if(state==menu) return
 tween(ingame,{size=0},0.3).onend=restart_level
end)

menuitem(5,"reset progress",function()
 tween(ingame,{size=0},0.3).onend=function()
  reset_progress()
  tween(ingame,{size=256},0.3,"quad_in")
 end
end)

menuitem(4,"lowres",function()
 lowrez=not lowrez
 poke(0x5f2c,lowrez and 3 or 0)
end)

menuitem(2,"toggle music",function()
 g_mute_music=not g_mute_music
 if g_mute_music then
  music(-1)
 else
  play_music(state==menu and 0 or 7+rnd(6))
 end
 dset(4,g_mute_music and 1 or 0)
end)
menuitem(3,"toggle sfx",function()
 g_mute_sfx=not g_mute_sfx
 dset(5,g_mute_sfx and 1 or 0)
end)

function saveval(id)
 return dget(id) or 0
end

lowrez=false
if(lowrez) poke(0x5f2c,3)

local osfx=sfx
function sfx(id)
 if (not g_mute_sfx) osfx(id)
end

function play_music(id)
 if (not g_mute_music) music(id)
end

function reset_progress()
 for i=0,127 do
  dset(i,0)
 end
 _init()
end

function _init()

 g_time,state,g_index=
 0,menu,saveval(0)
 
 g_sec,g_min,g_hour=saveval(1),
  saveval(2),saveval(3)
  
 g_mute_music=saveval(4)==1
 g_mute_sfx=saveval(5)==1
 g_show_stats=saveval(6)==1
 g_deaths=saveval(7)
 	  
 shk=0
 
end

function _update60()
 if btnp(🅾️,1) then
  g_show_stats=not g_show_stats
  dset(6,g_show_stats and 1 or 0)
 end

 state.update()
 tween_update(1/60)
 g_time+=1
 
 if g_time%60==0 and not g_won then
  g_sec+=1
  if g_sec>=60 then
   g_min+=1
   g_sec=0
   if g_min>=60 then
    g_min=0
    g_hour+=1
   end
  end
  dset(1,g_sec)
  dset(2,g_min)
  dset(3,g_hour)
 end
end

function _draw() 
 state.draw()
end

function restart_level()
 dset(0,g_index)

 reload(0x2000,0x2000,0x1000)
 reload(0x1000,0x1000,0x1000)
 
 entity_reset()
 collision_reset()

 g_ldone=false
 g_tcor=0
 g_cor=0
 g_follow={}
 g_won=false
 g_cora={}
 
 for x=0,15 do
  g_cora[x]={}
 end
 
 g_level=e_add(level({
  base=v(g_index%8*16,flr(g_index/8)*16),
  size=v(16,16)
 }))
 
 ingame.size=0
 tween(ingame,{size=256},0.3,"quad_in")
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
 self.corrupt_lvl=
  ((self.tile>=1 and self.tile<=5) or
  (self.tile==17 or (self.tile>=19 and self.tile <=21)) or
  (self.tile>=33 and self.tile<=35)
  or self.tile==48) and self.tile
 
 and 2 or 0
 self.can=self.corrupt_lvl==2
 
 if self.can then
  self.needed=true
 end
 
 g_cora[flr(self.pos.x/8)][flr(self.pos.y/8)]=self
end

function solid:clean()
 self.to_clean=true
end

cors={42,41,40,39,23,6}

function solid:render()
 if self.can then
  local x,y=self.pos.x,self.pos.y

  pal(14,4)
  palt(14,false)
  palt(12,true)
  spr(self.tile,x,y) 
  palt(12,false)
  palt(14,true) 
 
  if self.corrupt_lvl>1 then
   local s=flr(min(5,(self.corrupt_lvl-1)*6+1))
   spr(cors[s+1],x+1,y+1)
  end
  
  if(g_won) return
 
  if self.to_clean then
   self.corrupt_lvl-=0.04
   
   if self.corrupt_lvl<=1 then
    self.corrupt_lvl=0
    self.to_clean=false
   end
  else
   self.corrupt_lvl=
    min(self.
    corrupt_lvl+0.001,2)
  end
 end
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

turret=entity:extend({
 
})

turret:spawns_from(70,71)

function turret:init()
 self.flip=self.tile==71
end

function turret:idle()
 if self.t>=40 then
  self.t=0
  shk+=1
  sfx(25)
  local x=(self.flip and 2 or 6)+self.pos.x
  parts(x,self.pos.y+4,2,3,7)
  local b=bullet({
   vel=v(self.flip and -1 or 1,0),
   pos=v(x-2,self.pos.y+2)
  })
  
  e_add(b)
 end
end

function turret:render()
 spr(self.tile,self.pos.x
  +(self.t<=7 and 2 or 0)*(self.flip and 1 or -1),self.pos.y)
end

bullet=entity:extend({
 hitbox=box(0,0,4,4),
 sprite={idle={72}},
 collides_with={"guy","walls","bbox"}
})

function bullet:idle()
 if self.pos.x<-2 then
  self.pos.x=126
 elseif self.pos.x>126 then
  self.pos.x=-2
 elseif self.pos.y<-2 then
  self.pos.y=126
 elseif self.pos.y>126 then
  self.pos.y=-2
 end
end

function bullet:collide(o)
 if o:is_a("guy") then
  o:die() 
  self.done=true
 elseif o:is_a("bbox") then
  o.hp-=1
  if(o.hp<=0) o.done=true sfx(33) 
  parts(o.pos.x+4,o.pos.y+4,5,4,15)
  self.done=true
  parts(self.pos.x+2,self.pos.y+2,2,2,5)
 elseif o:is_a("walls") then
  self.done=true
  parts(self.pos.x+2,self.pos.y+2,2,2,5)
 end
end

follow=entity:extend({
 draw_order=4
})

function follow:pick()
 self.after=#g_follow<=1 and g_guy or g_follow[#g_follow-1]
end

function follow:init()
 self.vel=v(0,0)
end

function follow:start()
 add(g_follow,self)
 self:pick()
end

function follow:onreach()

end

function follow:update()
 self.vel*=0.9

 if self.tar then
  self.did=true
  local d=self.tar.pos-self.pos
  local l=d:len()
  
  if l<=4 then
   self.done=true
   del(g_follow,self)
   self:onreach()
  end
  
  local s=0.3
  self.vel.x+=d.x/l*s
  self.vel.y+=d.y/l*s
 else
  if not self.after or self.after.done or self.after.did then
   self:pick()
   return
  end
  
  local d=self.after.pos-self.pos
  local l=d:len()
  
  if l>12 then
   local s=0.2
   self.vel.x+=d.x/l*s
   self.vel.y+=d.y/l*s
  end
 end
end

key=follow:extend({
 sprite={idle={73,80,81,82,delay=30}},
 hitbox=box(0,0,8,8),
 collides_with={"guy"}
})

key:spawns_from(73)

function key:collide(o)
 if (self.cl) return
 parts(self.pos.x+4,self.pos.y+4,4,5,10)
 self.cl=true
 self:start()
 sfx(26)
end

function key:idle()
 if not self.cl then
  self.pos.y+=cos(self.t/120)/8
 else
  self:update()
  if(self.tar) return
  
  for e in all(entities_tagged["door"]) do
   if not e.busy and (e.pos-self.pos):len()<24 then
    self.tar=e
    e.busy=true
   end
  end
 end
 
 if self.t%60==0 or rnd()<0.005 then
  e_add(spark({
   pos=v(self.pos.x+2+rnd(4)-4,self.pos.y+rnd(8)-4)
  }))
 end
end

function key:render()
 for i=1,15 do pal(i,0) end
 for xx=-1,1 do
  for yy=-1,1 do
   if abs(xx)+abs(yy)==1 then
    spr_render(self,nil,xx,yy)
   end
  end 
 end
 r_reset()
 spr_render(self)
end

function key:onreach()
 parts(self.tar.pos.x+4,self.tar.pos.y+4,3,5,4)
 self.tar:become("fin")
 shk=6
 sfx(27)
end

door=entity:extend({
 hitbox=box(0,0,8,8),
 tags={"walls","door"}
})

door:spawns_from(68,69,67)

function door:fin()
 shk=2
 if(self.t>=60) sfx(28) parts(self.pos.x+4,self.pos.y+4,10,6,4) self.done=true shk=10
end

function door:collide()
 return c_push_out
end

function door:render()
 spr(self.tile,self.pos.x,self.pos.y)
end

swap=entity:extend({
 sprite={idle={7},hi={54}},
 hitbox=box(0,0,8,8),
 collides_with={"guy"}
})

swap:spawns_from(7)

function swap:collide(o)
 if (o.lt<10) return
 o.vert=not o.vert
 o.lt=0
 sfx(30)
 o.vel.y=0
 parts(o.pos.x+4,o.pos.y+4,4,4,7)
 self:become("hi")
 return c_push_out
end

function swap:hi()
 if(self.t>=10) self:become("idle")
end

bbox=entity:extend({
 hitbox=box(0,0,8,8),
 tags={"walls","bbox"}
})

bbox:spawns_from(83,84)

function bbox:init()
 self.hp=2
end

function bbox:collide(o)
 return c_push_out
end

function bbox:render()
 spr(82+self.hp,self.pos.x,self.pos.y)
end

platform=entity:extend({
 sprite={idle={86},width=2},
 hitbox=box(0,0,16,6),
 tags={"walls"},
 collides_with={"walls"}
})

platform:spawns_from(86,87)

function platform:init()
 self.up=self.tile==87
 self.pos.y+=1
 self.vel=v(0,0)
end

function platform:idle()
 if (self.t>=90) self:become("move") self.up=not self.up
end

function platform:move()
 self.vel.y=(self.up and -1 or 1)*0.5
end

function platform:collide(o)
 if(self.state=="idle") return c_push_out

 if o:is_a("walls") and o~=self then
  --self.done=true
  self:become("idle")
  sfx(34)
  return c_move_out
 end
 
 return c_push_out
end

spikes=entity:extend({
 tags={"walls"},
 collides_with={"guy"}
})

spikes:spawns_from(89,90,91,94)

function spikes:init()
 self.hitbox=(self.tile==89 
  or self.tile==91) and box(0,0,8,5) or box(0,3,8,8)
end

function spikes:collide(o)
 if o:is_a("guy") then
  o:die()
 end
end

function spikes:render()
 for i=1,15 do pal(i,0) end
 for xx=-1,1 do 
  spr(self.tile,self.pos.x+xx,self.pos.y+(xx==0 and 1 or 0)*((self.tile==90 or self.tile==94) and -1 or 1))
 end
 r_reset()
 spr(self.tile,self.pos.x,self.pos.y)
end

hplt=platform:extend({

})

hplt:spawns_from(88,104)

function hplt:init()
 self.dw=self.tile==88
 self.vel=v(0,0)
end

function hplt:idle()
 if (self.t>=90) self:become("move") self.dw=not self.dw
end

function hplt:move()
 self.vel.x=(self.dw and -1 or 1)*0.5
end

lock=entity:extend({
 hitbox=box(0,0,8,8),
 tags={"walls"}
})

lock:spawns_from(74,75)

function lock:init()
 self.sec=self.tile==74
end

function lock:render()
 if self.sec then
  spr(g_guy.vert and 75 or 74,self.pos.x,self.pos.y)
 else
  spr(g_guy.vert and 74 or 75,self.pos.x,self.pos.y) 
 end
end

function lock:collide()
 if ((self.sec and not g_guy.vert) or (not self.sec and g_guy.vert)) return c_push_out
end

pad=entity:extend({
 hitbox=box(0,7,8,8),
 collides_with={"guy"},
 sprite={idle={76},up={77}}
})

pad:spawns_from(76)

function pad:up()
 if(self.t>10) self:become("idle")
end

function pad:collide(o)
 if self.state=="idle" then
  self:become("up")
  o.vel.y=-4.2
  o.pos.y=self.pos.y-7
  sfx(35)
 end
end

function pad:idle()
 if self.t%60==0 or rnd()<0.005 then
  e_add(spark({
   pos=v(self.pos.x+rnd(8)-4,self.pos.y+3)
  }))
 end
end

fly=entity:extend({
 sprite={idle={92,93,delay=15}}
})

fly:spawns_from(92,93)

function fly:init()
 self.t=rnd(128)
 self.sx=self.pos.x
 self.sy=self.pos.y
end

function fly:idle()
 local t=self.t/4
 self.pos.x=self.sx+cos(t/53)*6*sin(t/74)
 self.pos.y=self.sy+sin(t/89)*7*cos(t/32)
end

-- todo:

-- lowrez mode menu

-- bugs:
-- something wrong with count
-->8
-- player

guy=entity:extend({
 sprite={
  idle={64},
  move={64,65,64,66,delay=7},
  flips=true,
  spawn={46}
 },
 draw_order=5,
 tags={"guy"},
 collides_with={"walls"},
 hitbox=box(2,0,6,8),
 feetbox=box(2,-1,6,9)
})

guy:spawns_from(64)

function guy:init()
 g_guy=self

 self.vel=v(0,0)
 self.vert=false
 self.cx=self.pos.x
 self.cy=self.pos.y
 self.rad=0
 self.tar_rad=0
 self.clr=11
 self.lt=0
 self.blkt=0
 self:become("spawn")
 sfx(29) 
end

function guy:die()
 if(self.state=="death") return
 self:become("death")
 sfx(24)
 g_deaths+=1
 dset(7,g_deaths)
 self.vel.x=0
 self.vel.y=0
 cls(8)
 
 shk=4
 for i=0,5 do
  flip()
 end
end

function guy:death()
 if(self.t==30 or self.t==31) shk=10 self.tar_rad=0 sfx(23)
 if self.t==60 then
  tween(ingame,{size=0},0.3).onend=restart_level
 end
 self:dorad()
end

function guy:spawn()
 self:dorad()
 if (self.t>=30) sfx(31) self.blkt=60 cls(7) flip() flip() self:become("idle") shk=4 self.tar_rad=16
end

function guy:win()
 if(self.state=="won") return
 self:become("won")
 self.vel.x=0
 self.vel.y=0
 self.tar_rad=0
 fade()
 sfx(32)
end

function guy:won()
 self:dorad()
 if self.t==30 then
  tween(ingame,{size=0},0.3).onend=function()
    g_index+=1
   restart_level()
  end
 end
end

function guy:idle()
 self:move()
end

function guy:flip()
 sfx(22)
 self.vert=not self.vert
 parts(self.pos.x+4,self.pos.y+4,3,4,self.clr)
end

function guy:move()
 self.lt+=1

 if self.pos.x<-4 then
  self.pos.x=124
 elseif self.pos.x>124 then
  self.pos.x=-4
 elseif self.pos.y<-4 then
  self.pos.y=124
 elseif self.pos.y>124 then
  self.pos.y=-4
 end

 self.vel.y+=(self.vert and -1 or 1)*0.2
 if (btnp(❎) and self.supported_by) self:flip()
 if (btnp(⬇️) and self.supported_by and self.vert) self:flip()
 if (btnp(⬆️) and self.supported_by and not self.vert) self:flip()
 local s=0.5--self.supported_by and 0.5 or 0.35
 self.vel.x*=0.3

 if btn(⬅️) then
  self.vel.x-=s
 end
 
 if btn(➡️) then
  self.vel.x+=s
 end
 
 self:become(abs(self.vel.x)>0.1 and "move" or "idle")
 local r=self.rad
 local rr=r*r
 local vc=v(self.cx,self.cy)
 local sx,sy=flr(self.cx/8+0.5),flr(self.cy/8+0.5)
 for xx=flr(-r/8),flr(r/8+0.5) do
  for yy=flr(-r/8),flr(0.5+r/8) do
   local x,y=xx+sx,yy+sy
   if x<0 or x>15 or y<0 or y>15 then
   else
    local c=g_cora[x][y]
    if c and #(c.pos-vc)<rr then
     c:clean()
    end
   end
  end
 end
 
 --[[
 local r=self.rad*self.rad
 for e in all(entities_tagged["cor"]) do
  if #(e.pos-vc)<r then
   e:clean()
  end
 end]]

 if not self.was_supported and self.supported_by then
  parts(self.pos.x+4,self.pos.y+4,3,3,7)
 end
 self.was_supported=self.supported_by~=nil

 self:dorad()
end

function guy:dorad()
 local s=10
 self.cx+=(self.pos.x-self.cx)/s
 self.cy+=(self.pos.y-self.cy)/s
 
 self.rad+=(self.tar_rad-self.rad)/10
end

function guy:render()
 self.blkt-=1
 
 if not self.no_render and (self.blkt<=0 or self.blkt%20>10) then
  spr_render(self)
 end

 local am=10
 local r=max(0,self.rad-2)--+cos(g_time/100)
 local c2=sget(97,self.clr)
 local c3=sget(97,c2)

 for i=1,am do
  local a=i/am+g_time/150
  local x=cos(a)*r+self.cx+4
  local y=sin(a)*r+self.cy+4
  spr(22,x-2,y-2)
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

ingame={size=256}

function ingame.update()
 if g_ldone then
  g_guy:win()
 end
 
 g_cor=0
 
 local calc=g_tcor==0
 
 for e in all(entities_tagged["cor"]) do
  if e.corrupt_lvl>=1 then
   g_cor+=1
   if(calc) g_tcor+=1
  end
 end
    
 if g_tcor>0 and g_cor<=0 then
  if g_index>=30 then
   g_won=true
  else
   g_ldone=true
  end
 end

 if g_guy.state=="death" or g_guy.state=="won" then
  g_guy.t+=1
  g_guy[g_guy.state](g_guy)
 else
  e_update_all()
  do_movement()
  do_collisions()
  do_supports()
 end
end

local ex,ey
function ingame.draw()
 local cx,cy=0,0
 if shk>0.5 then
  shk-=0.2
  cx,cy=rnd(shk)-shk/2,rnd(shk)-shk/2
 end

 cls() 
 if lowrez then
  local s=10
  if(btn(⬅️,1)) ex-=s
  if(btn(➡️,1)) ex+=s
  if(btn(⬆️,1)) ey-=s
  if(btn(⬇️,1)) ey+=s
  
  ex=mid(-64,64,ex)
  ey=mid(-64,64,ey)
  
  ex+=(-ex)/10
  ey+=(-ey)/10
  
  
  camera(mid(0,64,g_guy.pos.x-28+cx+ex),mid(0,64,g_guy.pos.y-28+cy+ey))
 else
  camera(cx,cy)
 end
 
 if ingame.size<256 then
  local s=ingame.size/2
  local x,y=g_guy.pos.x+4,g_guy.pos.y+4
  clip(x-s,y-s,s*2,s*2)
 end

 rectfill(0,0,127,55,12)
 rectfill(0,104,127,127,0)
 map(112,48,0,48,16,8)
 
 r_render_all("render")
 
 if g_index==0 then
  coprint("⬅️➡️ ^move  ",25,(btn(⬅️) or btn(➡️)) and 6 or 5)
  coprint("❎ ^toggle gravity ",96,btn(❎) and 6 or 5)
  coprint("⬆️⬇️ ^change gravity  ",106,(btn(⬇️) or btn(⬆️)) and 6 or 5)
 elseif g_won and g_index==30 then
  local y=32
  local c=g_time%60>30 and 1 or 0
  coprint("◆★✽●♥웃♥       ",y,10-c)
  coprint("^you won!",y+10,9+c)
  coprint("◆★✽●♥웃♥       ",y+20,10-c)
  coprint("⧗ "..pad(g_hour)..":"..pad(g_min)..":"..pad(g_sec).." ",y+40,6)
  coprint("★ ^thanks for playing! ★  ",y+60,7)
  coprint("😐 "..g_deaths.." deaths ",y+50,8)
  coprint("^press ❎ to continue ",y+80,12-c)
  if (btnp(❎)) sfx(36)  tween(ingame,{size=0},0.3).onend=function() reset_progress() end
 end
 
 camera()
 r_render_all("render_hud")
 
 --if btn(🅾️) then
 if(g_show_stats and not g_won) oprint(pad(g_hour)..":"..pad(g_min)..":"..pad(g_sec).." lvl "..(g_index+1),2,2,7)
 if(btn(🅾️))oprint(stat(1),2,12,stat(1)>=1 and 8 or 11)
end

function pad(v)
 v=v..""
 while #v<2 do
  v="0"..v
 end
 return v
end

--[[
g_pat={
  0b1111111111111111.1,
  0b0111111111111111.1,
  0b0111111111011111.1,
  0b0101111111011111.1,
  0b0101111101011111.1,
  0b0101101101011111.1,
  0b0101101101011110.1,
  0b0101101001011110.1,
  0b0101101001011010.1,
  0b0001101001011010.1,
  0b0001101001001010.1,
  0b0000101001001010.1,
  0b0000101000001010.1,
  0b0000001000001010.1,
  0b0000001000001000.1
}--]]

menu={}

function menu.update()

end

function menu.draw()
 cls(0)
 if ingame.size<256 then
  local s=ingame.size/2
  clip(64-s,64-s,s*2,s*2)
 end

 rectfill(0,0,127,55,12)
 rectfill(0,104,127,127,1)
 local x=g_time/2%128
 map(112,48,x,48,16,7)
 map(112,48,x-128,48,16,7)
 
 r_reset()
 local s=16
 local y=cos(g_time/120)*2.5
 local yy=sin(g_time/160)*1.5
 sspr(0,48,123,16,3,32+yy+1+s)
 sspr(0,48,123,16,3,32+yy+s)
 coprint("^press ❎ ",70+y+s,11+(g_time%60>30 and 1 or 0))
 print(smallcaps("@egordorichev"),2,121,13)

 for i=1,17 do
  spr((g_time+i*10)%20>10 and 66 or 64,(i*8-8+g_time/2)%136-8,22+yy-3+cos(g_time/60+i/6)*2.5+s)
  spr((g_time+i*10)%20>10 and 
   66 or 64,(i*8-8-g_time/2)%136-8,49+yy+3-cos(g_time/60+i/6)*2.5+s)
 end

 spr(g_time%30>15 and 66 or 64,118,118)
 spr(g_time%30<=15 and 66 or 64,2,2)
 spr(g_time%30>15 and 66 or 64,118,2)

 if(btnp(❎)) music(-1) sfx(36)  tween(ingame,{size=0},0.3).onend=function() play_music(g_index==0 and 7 or 7+rnd(6)) state=ingame restart_level() end
 if(btn(🅾️)) oprint(stat(1),2,2,stat(1)>=1 and 8 or 11)
end


 -- do not forget about 
 m()
 play_music(0)
 -- ➡️➡️⬆️☉🅾️∧░⬆️➡️☉🅾️∧
__gfx__
00000000cc01010101010101010101cceeeeeeeeeeeeeeee101101ee677777756c6c6c6cf6f6f6f67f7f7f7f7f7f7f7f01000000eeeeeeeeeeeeeeeeeeeeeeee
00000000c13b3b3b3b3b3b3b3b3b3b0ceeeeeeeeeeeeeeee028820ee7eeeeee6c6c6c6c66f6f6f6ff7f7f7f7f7f7f7f710000000eeeeeeeeeeeeeeeeeeedeeee
0070070003beeeeeeeeeeeeeeeeee3b1eeeeeeeeeeeeeeee18a981ee7e0110e66c6c6c6cf6f6f6f67f7f7f7f7f7f7f7f21000000eeeeeeeeeeedeeeeeee6eeee
000770001beeeeeeeeeeeeeeeeeeee30eeeeeeeeeeeeeeee189a81ee7e1ee1e6c6c6c6c66f6f6f6ff7f7f7f7f7f7f7f731000000eeedeeeeeed6deeeed676dee
0007700003eeeeeeeeeeeeeeeeeeeeb1eeeeeeeeeeeeeeee028820ee7e1ee1e666666666ffffffff777777777777777742000000eeeeeeeeeeedeeeeeee6eeee
007007001beeeeeeeeeeeeeeeeeeee30eeeeeeeeeeeeeeee101101ee7e0110e666666666ffffffff777777777777777751000000eeeeeeeeeeeeeeeeeeedeeee
0000000003eeeeeeeeeeeeeeeeeeeeb1eeeeee2332eeeeeeeeeeeeee7eeeeee666666666ffffffff777777777777777765000000eeeeeeeeeeeeeeeeeeeeeeee
000000001beeeeeeeeeeeeeeeeeeee30eeeeee3003eeeeeeeeeeeeee5666666166666666ffffffff77777777777777777d000000eeeeeeeeeeeeeeeeeeeeeeee
4444444403eeeeee44444444eeeeeeb1eeeeee3003eeeeeee010eeeeeeeeeeee777777777bbbbbb7777777771111111182000000eeedeeeeeee6eeeeeee7eeee
444444441beeeeee44444444eeeeee30eeeeee2332eeeeee0ab30eeee10101ee77777773bbbbbbbb377777771111111194000000eee6eeeeeee7eeeeeee6eeee
4424444403eeeeee44444444eeeeeeb1eeeeeeeeeeeeeeee1b3b1eeee02820ee77bb777bbbb33bbbb77bbbb711111111a9000000eee7eeeeeee6eeeeeeedeeee
444444441beeeeee44444444eeeeee30eeeeeeeeeeeeeeee03b10eeee18a81ee7bbbb7bbb333333bbbbbbbbb11111111b3000000d67676de676d676e76ded67e
4444422403eeeeee44444444eeeeeeb1eeeeeeeeeeeeeeeee010eeeee02820eebbbbbbbbb3333333bbbb33bb11111111c1000000eee7eeeeeee6eeeeeeedeeee
422442241beeeeee44444444eeeeee30eeeeeeeeeeeeeeeeeeeeeeeee10101eebb33bbbb333333333333333311111111d5000000eee6eeeeeee7eeeeeee6eeee
4224444403eeeeee44444444eeeeeeb1eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee33333333333113333333333311111111e2000000eeedeeeeeee6eeeeeee7eeee
444444441beeeeee44444444eeeeee30eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee33113333331111333333113311111111f4000000eeeeeeeeeeeeeeeeeeeeeeee
4444444403eeeeeeeeeeeeeeeeeeeeb17666666d766d766d6dddddd5eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee0101010100000000eee6eeeeeeeeeeee00000000
444444441beeeeeeeeeeeeeeeeeeee306dddddd56dd56dd5d5555551e1010eeeeeeeeeeeeeeeeeeeeeeeeeee1010101000000000eeedeeeeeeeeeeee00000000
4422444403eeeeeeeeeeeeeeeeeeeeb16d155dd56dd56dd5d56dd551e0280eeeee101eeeee82eeeeeeeeeeee0101010100000000eeeeeeeeeeeeeeee00000000
442244441beeeeeeeeeeeeeeeeeeee306d5dd6d5d551d551d5d55151e1821eeeee080eeeee28eeeeeee8eeee10101010000000006deeed6eeeeeeeee00000000
4444442403eeeeeeeeeeeeeeeeeeeeb16d5dd6d5766d766dd5d55151e0010eeeee101eeeeeeeeeeeeeeeeeee0000000000000000eeeeeeeeeeeeeeee00000000
444444441b3eeeeeeeeeeeeeeeeeeb306dd667d56dd56dd5d5511051eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee0000000000000000eeedeeeeeeeeeeee00000000
44424444c0b3b3b3b3b3b3b3b3b3b31c6dddddd56dd56dd5d5555551eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee0000000000000000eee6eeeeeeeeeeee00000000
44444444cc10101010101010101010ccd5555551d551d55151111110eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee0000000000000000eeeeeeeeeeeeeeee00000000
c101010ce1e1e1eee1e1e1eeeee1e1eeeeeeeeeeeeeeeeeeabbbbbb9000000000000000000000000000000000000000000000000000000000000000000000000
1b3b3b30e3e3e3eee3eee3eeeee3e3eeeeeeeeeeeeeeeeeebeeeeee3000000000000000000000000000000000000000000000000000000000000000000000000
03eeeeb1ebe3eeeeeeeeebeeeeeee3eeeeeeeeeeeeeeeeeebe3bb3e3000000000000000000000000000000000000000000000000000000000000000000000000
1beeee30eee3eeeeeeeeeeeeeeeee3eeeebeeeeeeeeeeeeebebeebe3000000000000000000000000000000000000000000000000000000000000000000000000
03eeeeb1eee3eeeeeeeeeeeeeeeee3eeee3eeeeeeeeeebeebebeebe3000000000000000000000000000000000000000000000000000000000000000000000000
1beeee30eeebeeeeeeeeeeeeeeeeebeee3eebeeeeeebe3eebe3bb3e3000000000000000000000000000000000000000000000000000000000000000000000000
03b3b3b1eeeeeeeeeeeeeeeeeeeeeeeee3ee3eebebe3e3eebeeeeee3000000000000000000000000000000000000000000000000000000000000000000000000
c010101ceeeeeeeeeeeeeeeeeeeeeeeee3ee3e3ee3e3e3ee93333331000000000000000000000000000000000000000000000000000000000000000000000000
eeeeeeeeeeeeeeeeeeeeeeeee000000e0000000010494901eeeeeeeeeeeeeeee1001eeeeeefaa7eee111111ee111111eeeeeeeeeeeeeeeee0000000000000000
eee00eeeeee00eeeeee00eee094949409494949409242420ee00000ee00000ee0780eeeeee9e9aee165555d01d0000d0eeeeeeeee000000e0000000000000000
ee0ff0eeee0ff0eeee0ff0ee042424204242424204242420e02882200228820e0820eeeeee9eeaee15655d1010100150eeeeeeee094444200000000000000000
ee0ff0eeee0ff0eeee0ff0ee0424242042424242042424200288710ee01788201001eeeeee499fee1557d11010011550eeeeeeeee060050e0000000000000000
e06cc10ee06cd10ee06cd10e040424204042424204242420e02888200288820eeeeeeeeeeeee4eee155d01101001d550eeeeeeeeee0dd0ee0000000000000000
e0ccd10ee0cdd1f00fcdd1f0040424204042424204042420e028810ee018820eeeeeeeeeeeee9eee15d1101010155d50eeeeeeeee060050e0000000000000000
e0f49f0e0f94920ee094920e042424204242424204042420e028810ee018820eeeeeeeeeee9afeee1d1111001d555560e000000eee0dd0ee0000000000000000
e0c0010ee00c10ee0c000010e000000e00000000042424200288282002828820eeeeeeeeee49aeeee000000ee000000e09444420e060050e0000000000000000
eeeeaeeeee7aafeeeee7eeeee111111ee111111e00000000e11111111111111e000000006dd56dd5eeeeeeeed5515555e00ee00eee0000eeeeeeeeee00000000
eeee9eeeeea9e9eeeeeaeeee197779f01a7777f000b00b0017fafafafafafa700080080066d566d5eeeeeeeedd51d55107600670e067560eeeeeeeee00000000
eeee9eeeeeaee9eeeeeaeeee174f4f9017ffff900b0000b01f494949494949f008000080765e765eeeeeeeee6d1edd1ee017510e07051070eeeeedee00000000
eeee9eeeeef994eeeeeaeeee17f4ff9017ffff90bbbbbbbb192414040414249088888888e6eee6eee6eee6eeedee6d1eee0510eee0e00e0eedeeedee00000000
eeee4eeeeee4eeeeeee4eeee1742ff9017ffff90bbbbbbbb1a4949494949494088888888e6eee6eee6eee6eeedeeedeeeee00eeeeeeeeeeeedee6d1e00000000
eeee9eeeeee9eeeeeee9eeee19ff4f9017ffff900b0000b0e00000000000000e08000080eeeeeeee765e765eeeeeedeeeeeeeeeeeeeeeeee6d1edd1e00000000
eeeefeeeeeefa9eeeeeaeeee1f9992401f99994000b00b00eeeeeeeeeeeeeeee00800800eeeeeeee66d566d5eeeeeeeeeeeeeeeeeeeeeeeedd51d55100000000
eeeeaeeeeeea94eeeeefeeeee000000ee000000e00000000eeeeeeeeeeeeeeee00000000eeeeeeee6dd56dd5eeeeeeeeeeeeeeeeeeeeeeeed551555500000000
eeee0000eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee00eeeeeeee0000eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
ee0022290eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee0eeeeeeeeeeeeeeee0220eeeeee0122200eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
e022222990eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee010eeeeeeeeeeeeeee0220eeeee011222220eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
e0220002990ee000eeee00e00e00e00e00eee00ee00e00eee01100eee000eeeeee000220eeee0110000220e00e00eeeee0000eeeee000eeeee000eeeeeeeeeee
0220eee090ee01110ee0110990110220220e09900220220e0111110e02220eeee0220220eeee011000e00e0220220eee099990eee01110eee09990eeeeeeeeee
0220eeee0ee0111110e0111990122220220e09900222221001111100222220ee02222220eeee01112200ee02221120e09992990e0111110e0999920eeeeeeeee
0120eeeeee0221011100111000222000220e029002220211001100022000990011102220eeeee01122220e022201110e000099002100110099000220eeeeeeee
0110eeee0e0220e01100110ee0220ee0220e02900220e01100110e0222229900110e0220eeeeee000222200220e0110ee0229900220e00e099992220eeeeeeee
0110eee0900220e01100110ee0220ee0220e02200220e01100110e0222229900110e0220eeeee00ee002200220e0110e02202900220e00e099992220eeeeeeee
e0110002290222011100110ee0990ee0220011200222022100110002200000e011102220eeee0290000220022202110022002900220022009900000eeeeeeeee
e011122220e0222210e0220ee0990ee02211112002222220e02221002229990e01122220eeee099922220e02222210e02222990e0222220e09999220eeeeeeee
ee0011220eee02220ee0220ee0990eee011101200220220eee02220e029990eee0110220eeeee0099990ee0220220eee02220990e02220eee099920eeeeeeeee
eeee0000eeeee000eeee00eeee00eeeee000e00e022000eeeee000eee0000eeeee00e00eeeeeeee0000eee022000eeeee000e00eee000eeeee0000eeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee0990eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee0920eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee0990eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee0990eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee00eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee00eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
42524252625242625242426270427042212121212121425262422121212121212121214252425262424262525242526242425242424242424242424221212121
21425242626242425242424252212121524252425242426242426252426242525242625242426242425242425242426242424270704221212142214262424221
4242420000000000000000420000005242222250212152b595522121212121214022226295b595b5959595b5b595b552524295b595b5b5b54242524201212102
214295b54252b5954262959542225021426242524295b59595b595b542424242520042006200420042005200620042525295b500004221622121214295954221
42000000000000009400005200940042520023122222320000622222222250213113000000000000000000000000006242620000750000000000004221212121
21620000000000005242000000231121524285000000000000000000860052426200000000004300000000000000744242000000004221212121014200005221
62040000c50000000000004200000042420094000033000000520013330011213100000000000000000000000000004242000000000000000094006221212121
21310000534300001131000043001121624200000000000000000000000042424200030003000300030003004200424262000000005221212121216200c56221
424200000000004242420042000000626200000000000000004200c5000011214242625200000000000000000000005242000000000000000000004221622101
21310000103000001131000010205121420400000000000000000000004242426200430033000000230013000000424242000400006202212121215200005221
42640000000000005400003500c500424242624242426200004200000000115231000095000000000000000000c5004262006500000065004252625221212121
0142000011420000113100005221212142524262520000c500000000000074424200030062000300030042000300426242004200005242526242425200004221
4242425242624252425252620000005252524242424252000054004300531121310453000000000000000000000000524200a5a5a5a5a5a54242424221212152
21420000116200001131000011214221215285000000000000000000860062424200230000001300000000c53300524252940000005222222222224200004221
01214022222222502142424200000052212121212142420000422020202051214120203000000000000000000000006242004242426242426242524242424242
21310000113100001131000011212121213100000000000000000000004252424200030003000300030003000300624242000000003423001333000000001121
21213100132300422222425200000042212121212142620000112122222222502222223265000000000000000000004242004295959595959500000000000042
4032000011310000623100001102212121310052a5e5a5a5a5a5e5e5626242526200000000c53300000033005300424262850000003400430000530000001121
42424242000000420023114270700062426242424242520000123200331300110000000000006500000000000000000052000400000000000000000000000042
31130000113100004231000011212101213100424242524242624242425242424200030003000300030062000300424252000000004220202020204200004221
52640054000000450000113100000052424252526242420000330000000000114243000000000000650000000043004242424265004262650042426244425242
3104000011310000113100001121212121310011212121212121212121425242420033005300000013004300230062424200c500005242624242426200005221
42624242000010420043113100000062425295b595b5426500000043000000115220204200000000000065006220205200004200000000000000004200420000
41300000113100001232000011212121216200110121524221212102216242525200030003005200030003000300425262000000006221212121216200006221
21212131000011212020513100000062310000000000000010202020202020512121216200000000000000005221212143004200000000000062944200545300
21620000426200000033000062422121214200112121425221214252215242426200000043000000000023000000426252000094005221210121215200004221
214221310000122222222232000000423100040043530000112121212101212121212142000000000000000042212121202042a5e5a5a5e5e542424242622020
21520000624200000000000052422102213100122222222222225262222222504200420003000300030003000300424242000000004221422121026200005221
212121310053000013000000000000524120202020202020512121622121212121212162a5e5a5a5e5e5a5e54221212102214252426242424242425252424252
21426500426265001030650042622121423100004300230000530033005300114204000033000000000000001300526242a5e5000042212121212152e5e54221
21022141202020202020205270707062212121210221212121212121212121212121214252526262524242524221212121214242212121212121212102212152
2142a5a55242e5e55262a5e542422121214120204252422020202020202020515262624262424252424262524242424242426270704221012121214242625221
21212121212121212121212121212121215262212121214252214022222222502102212121212121212121212121212152422222522222425222222222622250
21212121216221212121022121212121212102212121402222320011212121022121012121216200004242527070622180808080808080808080808080808080
21212121216242210221212121212121216252212121212102213100330013115262524252625242426242426262526242330004432300000000535300130011
21402222222222222222222222222250214022222222620000000011212121214022222222505200004221620000622190909090909090909090909090909090
212121012142622121212121212102212121212121212121212131000400001162420000000000b5000000950094426231005220202052b4b452202020300011
21310000002300130000003300330011213133002300420094c500112121012131003323001142000062426200945221a0a0a0a0a0a0a0a0a0a0a0a0a0a0a0a0
2121212121212121212121212121212121212101212140222222425242b4b45242520000c5000000000000000000744231001140222262b4b442222250310011
2131004300000000005300430000001121310000040062435353001121212121310000045311520000422142000052218191a18191a18191a18191a18191a181
42707052222222222222222222222250212121212121310023000000000000116242040000a5000000e500000000425231005242003300000000002352310062
0141202020202020202020202052a45221310000424242202030c4110221212131000010205162000011426200004221b1b1b1b1b1b1b1b1b1b1b1b1b1b1b1b1
3100000000130033000000330023001152426242526242000052a5e5a50000114242424242524242424242426200426262004252000000000000000011310011
2162525242624242524242524242a4422131000052524222225262522222225031000011526242000011014200006221b1b1b1b1b1b1b1b1b1b1b1b1b1b1b1b1
3104000000000000000000000000001142640000000000000042524242a4a4524221014221214221214202214200524231001131c50010202030000062520062
215295b59595b59595b59595b5b500112131000062424200230000540033131131000012625252424442424200001121b1b1b1b1b1b1b1b1b1b1b1b1b1b1b1b1
52a4a462425242624252424262b4b452624252424262420053000043000000625242424262524252426242424244424242001232000052425242000052620052
21420000000000000000000000000011213100c500000000000000520000001131000000950000000000000000001121b2b2b2b2b2b2b2b2b2b2b2b2b2b2b2b2
31000000000000000000000000000011212121212121412020202020300000424242526242222222422222224200626262002300000000746400000013530052
214200000053000000000000c500531121310000000000a5e5a50062005343113100c5004353000000000000c500112142624242425242424242426242524242
3100430053000000000000530000001140222222222222222222222262b4b45262420000b5330000951323009500425242001030000052425242000010300042
21620000102030000000000000001051213100000042424242420052202020513100004220205242445200000000112152426252424252525242524262625252
41202020205242b4b462422062707042312323003300000000130000000000115242000000000000000000000000426262001131000000000000c50042310011
21420000622142000000000000006221214200005242212121310011212121213100005202216200006242620000112142524242525262425242626252525252
21212121216242b4b452422121212121310000e5a500a50000e500e5a5000011424243530000a5005343e5000043424231001131005300000000430052310011
216200005221420000c5000000006221214200005242212121310011212121215200006221216200004221420000622152624252524252424252524242425262
2121212121310000009552212142522131000042625242424242625242a4a4425242202042524220202042422020625231001152202042b4b452202051310011
21620000422142000000000000004202215200004262212102420062212101216200004242625200004242620000422142424242424252426252524252425262
21210121213100430000422121524221310000000000000000000000000000116242524262424262424252624242524231001222222252b4b462226222320011
215204c4422162a5c4c4a5c4c4a56221216200004242212121520042212121214200005221214200006221529400622142424242624252424252524252424252
2121212121412020205262212121212131004353a5a500e5e500e500a50053112121212121212121212121212121210252430023002300005300135300430062
215242624221425242624262425242212142c4a542522121216200622121212152c4c45221214200004242420000522162426252425242525252625242526242
21622121212121212121212121022121412020204242624242526242422020512121212162422121210121212121212142524220202020425242622020202051
02212121012121212121212121212121215242426242212121520042212121216242526242526200006201427070422142425242424242625242425252524242
__label__
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
ccccc00cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc00ccccc
cccc0ff0cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc0ff0cccc
cccc0ff0cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc0ff0cccc
ccc06cc10cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc06cd10ccc
ccc0ccd10ccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc0fcdd1f0cc
ccc0f49f0cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc094920ccc
ccc0c0010ccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc0c000010cc
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
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccc00cccccccccccccccccccccccccccccccccccccc00cccccccccccccccccccccccccccccccccccccccccccccc00cccccccccccccccccccccccccccccccccc
ccc0ff0ccccc00cccccccccccccccccccccc00ccccc0ff0ccccc00cccccccccccccccccccccccccccccc00ccccc0ff0ccccc00cccccccccccccccccccccccccc
ccc0ff0cccc0ff0cccccccccccccccccccc0ff0cccc0ff0cccc0ff0cccccccccccccccccccccccccccc0ff0cccc0ff0cccc0ff0ccccccccccccccccccccccccc
cc06cc10ccc0ff0ccccccccccccc00ccccc0ff0ccc06cc10ccc0ff0ccccc00cccccccccccccc00ccccc0ff0ccc06cc10ccc0ff0ccccc00cccccccccccccc00cc
cc0ccd10cc06cc10ccccccccccc0ff0ccc06cc10cc0ccd10cc06cc10ccc0ff0cccccccccccc0ff0ccc06cc10cc0ccd10cc06cc10ccc0ff0cccccccccccc0ff0c
cc0f49f0cc0ccd10cccc00ccccc0ff0ccc0ccd10cc0f49f0cc0ccd10ccc0ff0ccccc00ccccc0ff0ccc0ccd10cc0f49f0cc0ccd10ccc0ff0ccccc00ccccc0ff0c
cc0c0010cc0f49f0ccc0ff0ccc06cc10cc0f49f0cc0c0010cc0f49f0cc06cc10ccc0ff0ccc06cc10cc0f49f0cc0c0010cc0f49f0cc06cc10ccc0ff0ccc06cc10
cccccccccc0c0010ccc0ff0ccc0ccd10cc0c0010cccccccccc0c0010cc0ccd10ccc0ff0ccc0ccd10cc0c0010cccccccccc0c0010cc0ccd10ccc0ff0ccc0ccd10
cccccccccccccccccc06cc10cc0f49f0cccccccccccccccccccccccccc0f49f0cc06cc10cc0f49f0cccccccccccccccccccccccccc0f49f0cc06cc10cc0f49f0
cccccccccccccccccc0ccd10cc0c0010cccccccccccccccccccccccccc0c0010cc0ccd10cc0c0010cccccccccccccccccccccccccc0c0010cc0ccd10cc0c0010
cccccccccccccccccc0f49f0cccccccccccccccccccccccccccccccccccccccccc0f49f0cccccccccccccccccccccccccccccccccccccccccc0f49f0cccccccc
cccccccccccccccccc0c0010cccccccccccccccccccccccccccccccccccccccccc0c0010cccccccccccccccccccccccccccccccccccccccccc0c0010cccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
ccccccc0000ccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc00cccccccc0000cccccccccccccccccccccccccccccccccccccccccc
ccccc0022290cccccccccccccccccccccccccccccccccccccccccc0cccccccccccccccc0220cccccc0122200cccccccccccccccccccccccccccccccccccccccc
c6c60222229906c6c6c6c6c6c6c6c6c6c6c6c6c6c6c6c6c6c6c6c010c6c6c6c6c6c6c6c02206c6c60112222206c6c6c6c6c6c6c6c6c6c6c6c6c6c6c6c6c6c6c6
6c6c02200029906c000c6c600c00600c00600c6c006c00600c6c01100c6c000c6c6c6000220c6c60110000220c00600c6c6c00006c6c60006c6c60006c6c6c6c
c6c02200000900c01110c60110990110220220c09900220220c0111110c02220c6c602202206c6c01100000000220220c6c0999906c6011106c6099906c6c6c6
6c60220c6c000c0111110c0111990122220220609900222221001111100222220c602222220c6c6011122000602221120c0999299060111110609999206c6c6c
66601206666060221011100111000222000220602900222021100110002200099001110222066660011222206022201110000009900210011009900022066666
66601106666060220001100110000220000220602900220001100110002222299001100022066666000022220022000110600229900220000009999222066666
66601106660900220601100110660220660220602200220601100110602222299001106022066666000000220022060110602202900220600609999222066666
66600110002290222011100110660990660220011200222022100110002200000001110222066660290000220022202110022002900220022009900000066666
6f6f01112222000222210002206f09906f02211112002222220002221002229990001122220f6f609992222000222221000222299000222220009999220f6f6f
f6f60001122006002220060220f60990f600111012002202200600222000299900f001102206f6f000999900f0220220060022209900022200f009992006f6f6
6f6f600000006f6000006f00006f00006f60000000002200006f6000006000000f6f0000000f6f6f0000000f602200006f600000000f00000f6f0000006f6f6f
f6f6f6f00006f6f60006f6f006f6f006f6f6000600f0990006f6f60006f60000f6f6f00600f6f6f6f60000f6f0920006f6f6000600f6f000f6f6f00006f6f6f6
fffffffffffffffffffffffffffffffffffffffffff0990ffffffffffffffffffffffffffffffffffffffffff0990fffffffffffffffffffffffffffffffffff
fffffffffffffffffffffffffffffffffffffffffff0000ffffffffffffffffffffffffffffffffffffffffff0000fffffffffffffffffffffffffffffffffff
ffffffffffffffffffffffffffffffffffffffffffff00ffffffffffffffffffffffffffffffffffffffffffff00ffffffffffffffffffffffffffffffffffff
ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff
f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7
7f7f7f7f7f7f7f7f7f007f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f007f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f7f007f7f7f7f7f7f7f7f7f7f
f7f7f7f7f700f7f7f0ff07f7f700f7f7f7f7f7f7f7f7f7f7f7f7f7f7f700f7f7f0ff07f7f700f7f7f7f7f7f7f7f7f7f7f7f7f7f7f0ff07f7f700f7f7f7f7f7f7
7f7f7f7f70ff0f7f70ff0f7f70ff0f7f7f7f7f7f7f7f7f7f7f7f7f7f70ff0f7f70ff0f7f70ff0f7f7f7f7f7f7f7f7f7f7f7f7f7f70ff0f7f70ff0f7f7f7f7f7f
7700777770ff077706cc107770ff077777007777777777777700777770ff077706cc107770ff077777007777777777777700777706cc107770ff077777007777
70ff077706cc10770ccd107706cc107770ff07777777777770ff077706cc10770ccd107706cc107770ff07777777777770ff07770ccd107706cc107770ff0777
70ff07770ccd10770f49f0770ccd107770ff07777700777770ff07770ccd10770f49f0770ccd107770ff07777700777770ff07770f49f0770ccd107770ff0777
06cc10770f49f0770c0010770f49f07706cc107770ff077706cc10770f49f0770c0010770f49f07706cc107770ff077706cc10770c0010770f49f07706cc1077
0ccd10bb0c001077777777770c0010770ccd10bb70ff07770ccd10770c0010bb777777770c0010770ccd10bb70ff07770ccd107777bbbbbb0c0010770ccd1077
0f49f0bbb377777777777777377777770f49f0bb06cc10770f49f0773bbbbbbbb3777777777777770f49f0bb06cc10770f49f0773bbbbbbbb37777770f49f077
0c0010bbbb77bbbb777bb777b77bb7770c0010bb0ccd10bb0c001077bbbb33bbbb77bbbb777bb7770c0010bb0ccd10bb0c001077bbbb33bbbb77bbbb0c001077
bb333333bbbbbbbbb7bbbb7bb7bbbb7bbb3333330f49f0bbb7bbbb7bbb333333bbbbbbbbb7bbbb7bbb3333330f49f0bbb7bbbb7bbb333333bbbbbbbbb7bbbb7b
bb3333333bbbb33bbbbbbbbbbbbbbbbbbb3333330c00103bbbbbbbbbbb3333333bbbb33bbbbbbbbbbb3333330c00103bbbbbbbbbbb3333333bbbb33bbbbbbbbb
b3333333333333333bb33bbbbbb33bbbb3333333333333333bb33bbbb3333333333333333bb33bbbb3333333333333333bb33bbbb3333333333333333bb33bbb
33331133333333333333333333333333333311333333333333333333333311333333333333333333333311333333333333333333333311333333333333333333
33311113333331133331133333311333333111133333311333311333333111133333311333311333333111133333311333311333333111133333311333311333
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111000001111111111111111111100000001111111111111111111111111111111111111111111111111
111111111111111111111111111111111111111111111110ccc0000000000000000011100ccccc00111111111111111111111111111111111111111111111111
111111111111111111111111111111111111111111111110c1c0ccc0ccc00cc00cc01110cc1c1cc0111111111111111111111111111111111111111111111111
111111111111111111111111111111111111111111111110ccc0c1c0cc10c110c1101110ccc1ccc0111111111111111111111111111111111111111111111111
111111111111111111111111111111111111111111111110c110cc10c10010c010c01110cc1c1cc0111111111111111111111111111111111111111111111111
111111111111111111111111111111111111111111111110c000c1c0ccc0cc10cc1011101ccccc10111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111010101010111011001100111001111100111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111000100000000000000001111100000001111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111110011111
1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111110ff01111
111d111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111110ff01111
11d1d1ddd1ddd11dd1ddd1dd111dd1ddd1ddd1ddd1d1d1ddd1d1d11111111111111111111111111111111111111111111111111111111111111111106cd10111
11d1d1dd11d111d1d1d1d1d1d1d1d1d1d11d11d111d1d1dd11d1d111111111111111111111111111111111111111111111111111111111111111110fcdd1f011
11d111d111d1d1d1d1dd11d1d1d1d1dd111d11d111ddd1d111ddd111111111111111111111111111111111111111111111111111111111111111111094920111
111dd1ddd1ddd1dd11d1d1dd11dd11d1d1ddd1ddd1d1d1ddd11d1111111111111111111111111111111111111111111111111111111111111111110c00001011
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111

__gff__
0001010101010000000000000000000001010101010100000000000000000000010101010101010000000000000000000100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
__map__
1212121212121220121212121212121225262422222226242424222222242624121212121004230000210512121212121212121212122012121212121212121212121210121212121220122424242424242405252524222222252224222225261212121212252412121212121012121225252425242424252426242425252426
1012121212121212121212121212121225000000330000252600003200310026121212121213000000331112121212121204222222051212121204222222051212121212122512201212122500310024240011130032000034000035000031241204222222051212121212121212121226000000000024242500000000000025
12121212121212121212241212121212260025242526001113002526242400242012121212130000000011121220241212130032001112121212133300311112262526121212121212121224000040242400111300012424022425240225001112133200001126122425242624242412250049000000472624005c0000490025
1212121212121212121212122612121213002612122600111300261212250011121212121213003400001112121212121213000000112612101213005c00111225252422220525262222052424002524250024130011252522222222051300241213004900210512240031003300241225000000000024262400000000000024
12121212121212121212121212121212135c26122025001113002612122600111212121212140202020215121212121212134000001112121212130000351112242625003121242400312122230024262400111300111300000000311113002612130000000011122600005c0000261226000000000024242400242425242426
1212121204222222222222051212121213001112121300111300111212130011121204222222051212042222220512121214030001151212121214030001151213003500000000320000003300004724240021230025230024252600242600251213350000001112240000000000240525000000002424000000250000002424
1212121213320000320033112012121213001112121300111300111212130011222223003100111212130031002122221212130011121212121212130011101213002425240202022625020202022425260000325c00000047262400242400261025020300001112242535000000321124000000002424442600000000004726
1220121213005c000000001112121212130011241213001113001112121300110032005c000011121013005c003331001212130021242524262624230011121226002522222222222426222222222224250001030001030024242400252400241212121300001112262426240000001124004000000000002400240024442426
1212121213000000000000111212121213001112121300111300111212130011000000000000111212130000000000001212130032470024240046000011121213003400003200000000350000350024240011130011133400252600252400110422222300001112121212130000001125242624252424252400000024002424
1212121213340040350000111212101213001110121300111300111210130011000035000040111212130034000000001012130001242424252424030011121214020202262424252602020202030026130025240011260202242500262400111300313300002612201212130000001104222222051024242625242425000011
1212121214020202020202151212121213001112121300212300112012130011020202030001151212140202020202021212130011122012121212130011121204222404252424262522222222230025130026250021222222262500111300111300000000001112120422230000002413330000212222220504222223000011
12121212121212121212121212121212130011121226003200342612121300111212121300212222222205101212121212121300111212121212121300111212250011130032001113310000330000241300111334003400003300002526001113005c0000011512121300310000001113000000320031001113310000000011
1212241212121212121212121212121213001112122524262524261212130011121212130000003200331112122512121212130021222222222222230011121226001114020300111300012624000024130011140202022524242502252600111300000000212205121300005c000011130000005c00000011130049005c0011
121212121212121212121212121225262600212222222222222222222223002612241213000000005c001112122012121212133400310032000000003511121224002124252600212300212424000026130021242222222624252422222300111300000000003321222300000000002626240000000000002123000000000011
1212121212121212121212121212262525350033003400400032000000310025121210133400350000001112121212122412140202020202020202020215121224340000000035000034001113340024244000000000330000340000000035241300400034000000450000343400001124463400000026004500350000000011
1212121212121212121212121212121224262402020202020202020202242625121212140202030000011512121212121212121212121210121212122512121226242626240202022402021514020226262402242402020202020224020224251425020202020202020202020202021524252425262524020202020202020215
0000000000000000000000000000000025240000212300000000212300002424002426242524252426242425242424242524000000000000000000000000262424242400000000000034000000242424042222262400002426000000002426120707070724070707240707070707240712250707070724121212122607072412
0000242400000000005c00002526000022230000320000403400003100002122002424242424242425240000000000002424242600000000000000002425242424000034000103000001030035000025133100242400002424005c00002424120000000024000000240000000000250012130000000026222222222400001112
000000000000000000000000000000000033000000000102020300000000320000000024252400242400005c00000000002400000000350000000034000024002402020202151402021514020202022413000025240000000000000000242612004000002400004924000000000024001213005c000000330033000000001120
0000000000000000000000000000000000000000000021222223005c0000000000000026244000000000242600350000002600000102030000010203000024001204222222050422220504222222051013000025240000000000000000242412242400002400000024000024240024241213340035000001020203005c001112
2424000035000000003500000000242403000000000000320031000000000001262400242424244424252424020226020024000021222300002122230000000020130000322513000011130000331112130000242400000000000024242524240000000024000024240000242400000012140202030001151212130000001112
0000000102020300000102020300000023003400000034000000000000340021121300111224240024242522222222050024000032313340003200310000000022230000002123000021230000002122130000242400000000000024242424242425240024000000250000242425242612101212130021222205130000001112
0000001104222300002122051300000033000103000001020203000001030032201300210513003500002400320000114924000000000001030000000000000000310000000032000000310000000000134424242407070707070724240000242424240025242400240000242424242412042222240000330011133400351112
0000001113000035003332241300000000001113000011121213000011130000121300311113000103002400010300110024005c00000021230000000000000000000000000000000000000000000000130000000000000000000000000049240000240024242500260000002424000012133100000040000011260300011512
0000002625000102020300111300000000001113000011101213000011130000121300001113002123002500111300110024000000340031000034350000005c000000000000000000000000005c0000130000000000000000000000000000240000240000000000240000002424490012130025350000000025242300111212
0000002526002122222300111300000000002523000021222223000025263500121300002513000000342400111300110025000001030000000001030000000000000000000000000000000000000000130000002407070707070724242424240000240000242400242424002425000012130024020202020224260000111212
005c00111334003300400011130000000300003200000031330000002625020212130000111402020202260011130011002400002123000000002123000000000000000000000000000000000000000013005c000000000000000024242424242400262424242400000024002424242404230024240512120423000025241212
0000001114020202020202151300000024000000000035000034000021222222222300001104222222222500261300210026000000310000000000330000240000000000070707070707070707000000130000000000000000000024121012120000000000000000000024000000000013000000001112121333000011122612
0000002122222222222222222300000000005c00000001020203000000003200003300001113003300000000111300330024000000000034000000000000240000000000000000000000000000000000130000000000002424252424121212122425240000242444242524252424252413005c00001112121300000011121212
0000000031000000003300000000000000000000000021222223000000000000000000002524004934000000111300002424442424000001030000000000242400403400000000000000000000003500130040000000002424242424222222052524260000262444242424242624262413000000001120121300000011121212
0000000000000000000000000000000000340000350000320033000000003400000035002624020202020202151402022400000025000021230000002424242502020300000000000000000000010202242424242600000000000000330031110000000000242400000024242600000024020202022412122407070726121012
0000000000000000000000000000000002030000010300000000010300000102000102242625121212121212121012122402020224000000330000000000242412122400000000000000000000241212242425252400000000000000002426240707070707242402020224252407070712121212121210121212121212121212
__sfx__
011600200f3320f335133221633513332133350c3320f335113321133513322113350f3320f3350f3321133516332163350f3220a3350f3320f33513332113351333213335183221b33516332163351333216331
011800200c0030c0030c0630300324605030030c063030030c003030030c0630c00330605030030c0630c00324605030030c0630300324605030030c063030030c003030030c0630c00303003030030c06324604
011800200a1540a1520a1510a1550f1340f1320f1320f13507154071520715207145131441314113142131450f1240f1210f1210f1250a1340a1320a1320a1350315403152031520315507144071410714207145
011800200a1540a15207151071550f1340f132111321113507154071520515205145131441314116142161450f1240f1210c1210c1250a1340a1320c1320c1350315403152051520515507144071410514205145
011800200c0030c0030c0633062524605030030c063030030c003030030c0633062530624306250c0630c00324605030030c0633062524605030030c063030030c003030030c0633062530624306250c06324604
001800200a1540c15207152051550f13413132111320f135071540a1520515203145131441814216142131450f124111220c122071250a1340f1320c1320a13503154071520515203155071440a1420514203145
002000200c0430c335346150c3350c0430c33534615346150c0430a3350a3350f043346150a3350f0430a3350f0430533534615053350c0430533534615053350c04303335346150f04307335073350c0430a335
002000201f1341f1321f1321f135181341813218132181351d1341d1321d1321d135161341613216132161351b1341b1321b1321b13513134131321313213135181341813218132181351b1321d1321f13222135
002000201f1341f1321f1321f1321f1321f1321f1321f1351d1341d1321d1321d1321d1321d1321d1321d1351b1341b1321b1321b1321b1321b1321b1321b135181341813218132181321b1321d1321f13222135
002000201f1341f131221311f13518134181321b132181351d1341d1321f1321d135161341613118131161351b1341b1311d1311b1351313413132161321313518134181321b132181351b1321d1322413222135
002000200c0430c305346150c3050c0430c30534615346150c0430a3050a3050f043346150a3050f0430a3050f0430530534615053050c0430530534615053050c01303305346150f04307305073050c0430a305
004000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0120002018424134221642213425114240c4220f4220c425164241142213422114250f4240a4220c4220a425134240f422114220f4250c424074220a42207425114240c4220f4220c42513422114221842216425
01200020241271f035221271f0361d127180351b12718036221271d0351f1271d0361b1271603518127160361f1271b0351d1271b036181271303516127130361d127180351b127180361f1271d0352412722036
012000200c404074020a40207405054040040203402004050a4040540207402054050340400402004020040507404034020540203405004040040200402004050540400402034020040507402054020c4020a405
012000200c425074250a42507425054250042503425004250a4250542507425054250342500425004250042507425034250542503425004250042500425004250542500425034250042507425054250c4250a425
01200020243221f321223221f3221d321183271b32718326223221d3211f3221d3221b3211632718327163261f3221b3211d3221b322183211332716327133261d322183211b322183221b3211d3271f32722327
012000201f3221f221222211f32218322182211b221183221d3221d2211f2211d322163221622118221163221b3221b2211d2211b3221332213221162211332218322182211b221183221b3221d2212422122322
002000200c435074350a43507435054350043503435004350a4350543507435054350343500435004350043507435034350543503435004350043500435004350543500435034350043507435054350c4350a435
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
01050000053620a362103020e30200300003000030000300003000030000300003000030000300003000030000300003000030000300003000030000300003000030000300003000030000300003000030000300
010600002236018360133600c36007360033600330003300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010700000a47207471074750040500405004050040500405004050040500405004050040500405004050040500405004050040500405004050040500405004050040500405004050040500405004050040500405
01040000132560a256072060320607206072060020600206002060020600206002060020600206002060020600206002060020600206002060020600206002060020600206002060020600206002060020600206
01040000163521b3650a3020f30200302003020030200302003020030200302003020030200302003020030200302003020030200302003020030200302003020030200302003020030200302003020030200302
010900000c36511365033050330500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005
010e000005364073620a3620f36116365133650000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005
010b000005354073620c37511302183021b3020030200302003020030200302003020030200302003020030200302003020030200302003020030200302003020030200302003020030200302003020030200302
010800000c2540f251132520020000200002000020000200002000020000200002000020000200002000020000200002000020000200002000020000200002000020000200002000020000200002000020000200
010a0000113650c365133650030500305003050030500305003050030500305003050030500305003050030500305003050030500305003050030500305003050030500305003050030500305003050030500305
0110000003335073420f3621f3711b37511305163051f3051b3052e30500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005
010700001f67518675136750f67500605006050060500605006050060500605006050060500605006050060500605006050060500605006050060500605006050060500605006050060500605006050060500605
011000000c07300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010800000c364113050c3721137500304003040030400304003040030400304003040030400304003040030400304003040030400304003040030400304003040030400304003040030400304003040030400304
010b00000c47500400004000040000400004000040000400004000040000400004000040000400004000040000400004000040000400004000040000400004000040000400004000040000400004000040000400
__music__
00 01024344
00 01024344
01 04034344
00 04034344
00 04054344
02 04054344
01 46484344
00 4b0a4344
01 06080b44
00 06070b44
00 06090b44
00 060c0b44
00 060c0b0d
00 060f0b0d
00 060f0b10
02 06120b11

