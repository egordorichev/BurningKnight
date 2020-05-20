pico-8 cartridge // http://www.pico-8.com
version 19
__lua__
-- one room dungeon by trasevol_dog 
-- #ld37 - theme was 'one room'

-- post-jam updated


objs={
 to_update={},
 to_draw={},
 
 enemies={},
 tiles={}
}

for i=0,4 do objs.to_draw[i]={} end

col_drk={[0]=0,0,1,1,2,1,5,6,2,4,9,3,1,1,8,9}


tileshin={}
for x=0,31 do
 tileshin[x]={}
 for y=0,31 do
  tileshin[x][y]=8
 end
end

roomw=7

firstinit=true
function _init()
 music(0,0,7)
 if firstinit then
  cls()
  init_anim_info()
  init_spawn_table()
  init_clear_colors()
  init_titlescreen()
 
  for i=1,50 do draw_title_clear() end
 
  --- titlescreen
  add_shake(8)
  sfx(13)
  while not btnp(5) do
   t+=0.01
   
   draw_titlescreen()
   flip()
  
   update_shake()
  end
  
  memcpy(0x1000,0x4300,0x1000)
 end
 firstinit=false
 
 music(-1)
 
 sfx(12)
 
 cls(6)
 flip()
 flip()
 cls(7)
 flip()
 flip()
 cls(6)
 flip()
 flip()
 cls(5)
 flip()
 flip()
 cls(1)
 flip()
 flip()
 cls(0)
 flip()
 
 music(8,0,4)
 
 objs.to_update={}
 for i=0,4 do objs.to_draw[i]={} end
 objs.enemies={}
 objs.tiles={}

 roomx=3.5*8
 roomy=3.5*8
 roomt=0
 
 camx=roomx-64
 camy=roomy-64
 
 for x=0,roomw do
 for y=0,roomw do
  mset(x,y,mget(x+32,y))
 end
 end
 
 player=create_player(roomx,roomy)
 loset=0.2
 
 tiletopress=3
 create_tiles(roomx,roomy)
 
 diff=0
 hppickup=0
 
 create_explosion(roomx,roomy,8,7)
end

t=0
function _update()
 t+=0.01
 roomt+=0.01
 
 update_objects()
 update_camera()
 
 for en in group("enemies") do
  local col=collide_objgroup(en,"enemies")
  if col then
   local a=atan2(col.x-en.x,col.y-en.y)
   en.vx-=0.5*cos(a)
   en.vy-=0.5*sin(a)
  end
  
  if player and collide_objobj(en,player) then
   local a=atan2(player.x-en.x,player.y-en.y)
   en.vx-=0.5*cos(a)
   en.vy-=0.5*sin(a)
  end
 end
 
 
 if not player then
  local ol=loset
  loset=max(loset-0.01,0)
  if loset==0 and ol>0 then
   add_shake(32)
   sfx(13)
   music(6,0,4)
  end
  
  if loset==0 and btnp(5) then _init() end
 end
end

function _draw()
 xmod=camx+shkx
 ymod=camy+shky
 
 camera(0,0)
 
 --rect(0,0,127,127,8)
 draw_clear()
 
 camera(xmod,ymod)
 --draw_room(64+16*cos(t),64+16*sin(t))
 draw_room(roomx,roomy)
 draw_objects()

 pal(14,0)
 spr(88,roomx-roomw*4-4,roomy-roomw*4,8,7) 
 pal(14,14)
 
 camera(0,0)
 draw_gui()
end



--- updates ---

function take_damage(d)
 if player and player.inv==0 then
  player.hp=max(player.hp-d,0)
  player.whiteframe=true
  player.inv=0.3
  add_shake(2)
  sfx(7)
 end
end

function update_player(p)
 p.animt+=0.01
 
 if p.state=="run" and p.animt%0.03<0.01 and animt_to_step(p.animt,"player","run")==2 then
  sfx(17)
 end
 
 p.inv=max(p.inv-0.01,0)
 p.rhppu=max(p.rhppu-0.01,0)
 
 col=collide_objgroup(p,"enemies")
 if col and col.attacking then
  take_damage(1)
 end
 
 if p.ded then
  create_explosion(p.x,p.y,16,7)
  add_shake(16)
  delete_registered(p)
  player=nil
  music(-1)
  sfx(16)
 end
 
 if p.hp<=0 then
  p.ded=true
 end
 
 p.vx*=p.dec
 p.vy*=p.dec
 
 if btn(0) then p.vx-=p.acc end
 if btn(1) then p.vx+=p.acc end
 if btn(2) then p.vy-=p.acc end
 if btn(3) then p.vy+=p.acc end
 
 p.vx=sgn(p.vx)*min(abs(p.vx),p.spdcap)
 p.vy=sgn(p.vy)*min(abs(p.vy),p.spdcap)
 
 p.x+=p.vx
 p.y+=p.vy
 
 stay_in_box(p,
  roomx-roomw*4,
  roomy-roomw*4-2,
  roomx+roomw*4-1,
  roomy+roomw*4-3)
 
 local newstate
 if abs(p.vx)>0.1 or abs(p.vy)>0.1 then
  newstate="run"
 else
  newstate="idle"
 end
 
 if newstate~=p.state then
  p.state=newstate
  p.animt=0
 end

 if abs(p.vx)>0 then
  p.faceleft=(p.vx<0)
 end
 
 if tiletopress<=0 and btnp(5) then
  if     btn(0) then move_room(0)
  elseif btn(1) then move_room(1)
  elseif btn(2) then move_room(2)
  elseif btn(3) then move_room(3)
  end
 end
end


function update_slime(s)

 s.vx*=s.dec
 s.vy*=s.dec

 if s.state=="idle" then
  s.animt+=0.01
  
  if s.animt>=0.2 then
   s.state="jump"
   s.animt=0
   if player then
    s.aimx=player.x
    s.aimy=player.y
   end
  end
 else
  if animt_to_step(s.animt,"slime","jump")<anim_get_steps("slime","jump")-1 then
   s.animt+=0.01
  elseif not s.attacking then
   s.vz=-s.jump
   s.attacking=true
   local a=atan2(s.aimx-s.x,s.aimy-s.y)
   s.vx=s.spdcap*cos(a)
   s.vy=s.spdcap*sin(a)
   s.faceleft=(s.vx<0)
  else
   s.z+=s.vz
   s.vz+=0.5
   
   if s.z>=0 then
    s.z=0
	s.vz=0
	s.animt=rnd(0.5)
	s.state="idle"
	s.attacking=false
   end
  
  end
 
 --- you stopped here
 end
 
 s.x+=s.vx
 s.y+=s.vy
 
 stay_in_box(s,
  roomx-roomw*4,
  roomy-roomw*4-2,
  roomx+roomw*4-1,
  roomy+roomw*4-3)

end


function update_chomp(s)
 s.animt+=0.01
 
 local a
 if player and s.animt%0.1<0.01 then
  s.aimx=player.x
  s.aimy=player.y
 end
 
 a=atan2(s.aimx-s.x,s.aimy-s.y)
 
 s.vx+=s.acc*cos(a)
 s.vy+=s.acc*sin(a)
 
 s.x+=s.vx
 s.y+=s.vy
 
 s.vx*=s.dec
 s.vy*=s.dec
 
 if sqr(s.x-s.rx)+sqr(s.y-s.ry)>sqr(s.reach) then
  a=atan2(s.rx-s.x,s.ry-s.y)
  s.vx+=1*cos(a)
  s.vy+=1*sin(a)
 end
 
 if player then
  s.faceleft=(player.x<s.x)
 end
 
 stay_in_box(s,
  roomx-roomw*4,
  roomy-roomw*4-2,
  roomx+roomw*4-1,
  roomy+roomw*4-3)
 
end


function update_skele(s)
 s.animt+=0.01
 
 local step=animt_to_step(s.animt,"skele")
 if (not s.attacking) and step==0 then
  s.attacking=true
  create_arrow(s.x,s.y,s.aimx,s.aimy)
  sfx(18)
 elseif step==anim_get_steps("skele")-4 and player then
  s.aimx=player.x
  s.aimy=player.y
 elseif step==1 then
  s.attacking=false
 end
 
 s.x+=s.vx
 s.y+=s.vy
 
 s.vx*=s.dec
 s.vy*=s.dec
 
 if player then
  s.faceleft=(player.x<s.x)
 end
 
 stay_in_box(s,
  roomx-roomw*4,
  roomy-roomw*4-2,
  roomx+roomw*4-1,
  roomy+roomw*4-3)
end

function update_arrow(ar)
 ar.x+=ar.vx
 ar.y+=ar.vy
 
 if stay_in_boxb(ar,
     roomx-roomw*4,
     roomy-roomw*4-2,
     roomx+roomw*4-1,
     roomy+roomw*4-3) then
  delete_registered(ar)
  sfx(19)
 end
end


function update_bat(s)
 s.animt+=0.01
 
 local a
 if player then
  a=atan2(player.x-s.x,player.y-s.y)
 else
  a=rnd(1)
 end
 
 s.vx+=s.acc*cos(a)
 s.vy+=s.acc*sin(a)
 
 s.vx=sgn(s.vx)*min(abs(s.vx),s.spdcap)
 s.vy=sgn(s.vy)*min(abs(s.vy),s.spdcap)
 
 s.x+=s.vx
 s.y+=s.vy
 
 s.vx*=s.dec
 s.vy*=s.dec
 
 if player then
  s.faceleft=(s.vx<0)
 end
 
 stay_in_box(s,
  roomx-roomw*4,
  roomy-roomw*4-2,
  roomx+roomw*4-1,
  roomy+roomw*4-3)
end


function update_thrower(s)
 s.animt+=0.01
 
 local step=animt_to_step(s.animt,"thrower")
 if (not s.attacking) and step==anim_get_steps("thrower")-1 then
  s.attacking=true
  local a=rnd(1)
  local spd=0.1+rnd(0.3)
  create_bomb(s.x,s.y,spd*cos(a),spd*sin(a))
  sfx(25)
 elseif step==0 then
  s.attacking=false
 end
 
 s.x+=s.vx
 s.y+=s.vy
 
 s.vx*=s.dec
 s.vy*=s.dec
 
 stay_in_box(s,
  roomx-roomw*4,
  roomy-roomw*4-2,
  roomx+roomw*4-1,
  roomy+roomw*4-3)
end

function update_bomb(s)
 s.animt+=0.01
 
 if s.animt>0.5 and s.animt%0.04<0.01 then sfx(15) end
 
 s.z+=s.vz
 s.vz+=0.5
 if s.z>=-1 then
  s.z=0
  if abs(s.vz)>1 then
   s.vz*=-0.5
  else
   s.vz=0
  end
  s.vx*=s.dec
  s.vy*=s.dec
 end
 
 local step=animt_to_step(s.animt,"bomb")
 if step==anim_get_steps("bomb")-1 then
  sfx(14)
  create_explosion(s.x,s.y,s.reach)
  if player then
   if sqr(player.x-s.x)+sqr(player.y-s.y)<sqr(s.reach+1) then
    take_damage(2)
   end
  end
  delete_registered(s)
 end
 
 s.x+=s.vx
 s.y+=s.vy
 
 stay_in_box(s,
  roomx-roomw*4,
  roomy-roomw*4-2,
  roomx+roomw*4-1,
  roomy+roomw*4-3)
end


function update_shapeshift(s)
 s.animt+=0.01
 s.kt-=0.01
 
 if s.name~="shapeshift" then
  s.shape_update(s)
  s.shape_update(s)
  
  if s.kt<0 then
   s.name="shapeshift"
   s.kt=0.1+rnd(0.08)
   s.state=nil
   s.acc=0
   s.dec=0
   s.spdcap=4
   s.z=0
   s.attacking=true
   s.draw=draw_self
  end
 elseif s.kt<0 then
  s.kt=0.20+rnd(0.10)
  local k=rnd(7)
  if k<1 then
   s.name="slime"
   s.state="idle"
   s.shape_update=update_slime
   s.draw=draw_self
   s.animt=0
   s.acc=0.2
   s.dec=0.8
  elseif k<2 then
   s.name="chomp"
   s.shape_update=update_chomp
   s.draw=draw_chomp
   s.animt=0
   s.rx=s.x
   s.ry=s.y
   s.acc=0.05
   s.dec=0.95
  elseif k<3 then
   s.name="skele"
   s.shape_update=update_skele
   s.draw=draw_self
   s.animt=0
   s.acc=0.1
   s.dec=0.8
  elseif k<4 then
   s.name="bat"
   s.shape_update=update_bat
   s.draw=draw_self
   s.animt=0
   s.acc=0.1
   s.dec=0.97
   s.spdcap=1.1
  elseif k<5 then
   s.name="zombie"
   s.shape_update=update_bat
   s.draw=draw_self
   s.animt=0
   s.acc=0.1
   s.dec=0.8
   s.spdcap=0.2
  elseif k<6 then
   s.name="ghost"
   s.shape_update=update_bat
   s.draw=draw_self
   s.animt=0
   s.acc=0.1
   s.dec=0.9
   s.spdcap=2
  elseif k<7 then
   s.name="thrower"
   s.shape_update=update_thrower
   s.draw=draw_self
   s.animt=0
   s.acc=0.1
   s.dec=0.8
   s.spdcap=3
  end
 else
  s.x+=s.vx
  s.y+=s.vy
  
  stay_in_box(s,
   roomx-roomw*4,
   roomy-roomw*4-2,
   roomx+roomw*4-1,
   roomy+roomw*4-3)
 end
end

function update_tile(ti)
 if ti.pressed then
  ti.animt=min(ti.animt+0.01,7*anim_info.tile.shine.dt)
  return
 end
 
 if player and collide_objobj(ti,player) then
  ti.pressed=true
  mset(flr(ti.x/8)%32,flr(ti.y/8)%32,25+flr(rnd(5)))
  tiletopress-=1
  if tiletopress<=0 then
   sfx(6)
  else
   sfx(5)
  end
 end
end

function update_hptile(ti)
 if player and collide_objobj(ti,player) then
  player.hp=min(player.hp+ti.hp,player.hpmax)
  player.rhppu=0.2
  if player.hp==player.hpmax then
   player.lhppu="hp max"
  else
   player.lhppu="+"..ti.hp.." hp"
  end
  player.whiteframe=true
  mset(flr(ti.x/8)%32,flr(ti.y/8)%32,41)
  delete_registered(ti)
  sfx(11)
 end
end

function update_spawntile(ti)
 if roomt>0.1 and ((not player) or (not collide_objobj(ti,player))) then
  -- spawn
  
  if diff%9>8.8 then
   create_shapeshift(ti.x,ti.y)
  else
   local lvl=min(flr(diff)+1,10)
   local pool=zone_spawns[lvl]
   local tospawn=pool[flr(rnd(#pool))+1]
   local foo=enemy_spawns[tospawn]
   foo(ti.x,ti.y)
  end
  
  mset(flr(ti.x/8)%32,flr(ti.y/8)%32,31)
  delete_registered(ti)
  sfx(10)
 end
end

function update_smoke(s)
 s.x+=s.vx
 s.y+=s.vy
 
 s.vx*=0.95
 s.vy=0.95*s.vy+0.05*(-1)
 
 s.r-=0.05
 if s.r<0 then
  delete_registered(s)
 end
end

function move_room(d)

 sfx(9)

 add_shake(8)
 
 diff+=0.25
 roomt=0

 local dx,dy=0,0
 
 if d==0 then
  dx=-1
 elseif d==1 then
  dx=1
 elseif d==2 then
  dy=-1
 else
  dy=1
 end
 
 for ti in group("tiles") do
  delete_registered(ti)
 end
 
 if diff%9==0 then
  local rmx=roomx+dx*roomw*8
  local rmy=roomy+dy*roomw*8
  local mx=flr(rmx/8-roomw*0.5)%32
  local my=flr(rmy/8-roomw*0.5)%32
  for x=0,roomw do
  for y=0,roomw do
   if diff>9 and mget(x+39,y)==40 then
    mset((mx+x)%32,(my+y)%32,42)
   else
    mset((mx+x)%32,(my+y)%32,mget(x+39,y))
   end
  end
  end
  
  music(16,500,4)

  create_tiles(rmx,rmy)
  
  diff=diff-0.01
 elseif diff%9>0.23 and diff%9<0.25 then
  local rmx=roomx+dx*roomw*8
  local rmy=roomy+dy*roomw*8
  local mx=flr(rmx/8-roomw*0.5)%32
  local my=flr(rmy/8-roomw*0.5)%32
  for x=0,roomw do
  for y=0,roomw do
   mset((mx+x)%32,(my+y)%32,mget(x+46,y))
  end
  end

  create_tiles(rmx,rmy)
  
  player.hpmax+=1
  music(8,500,4)
  
  diff=flr(diff)
 else
  gen_room(roomx+dx*roomw*8,roomy+dy*roomw*8,diff)
  if diff%9==0.25 then
   music(24,500,4)
  elseif diff%9==3 then
   music(32,500,4)
  elseif diff%9==6 then
   music(48,500,4)
  end
 end
 
 player.state="againstwall"
 if abs(dx)==1 then
  player.x=roomx+dx*(roomw*4-5)
  player.animt=0
 else
  player.y=roomy+dy*(roomw*4+1)-3
  player.animt=1
 end
 
 _draw()
 for i=0,4 do
  flip()
 end
 
 local orx,ory,nrx,nry
 orx=roomx
 ory=roomy
 nrx=roomx+dx*roomw*8
 nry=roomy+dy*roomw*8
 
 local i=0
 while roomx~=nrx or roomy~=nry do
  i+=1
  
  roomx+=dx
  roomy+=dy
  player.x+=dx
  player.y+=dy
  
  camx+=0.5*dx
  camy+=0.5*dy
  
  enm=false
  for en in group("enemies") do
   if en.ded then
    if en.update==update_shapeshift then
	 create_explosion(en.x,en.y,20,en.c)
	 flip()
	else
     create_explosion(en.x,en.y,10,en.c)
	end
	
	delete_registered(en)
	sfx(8)
   else
    local burn=stay_in_boxb(en,
     roomx-roomw*4,
     roomy-roomw*4-2,
     roomx+roomw*4-1,
     roomy+roomw*4-3)
    
    if burn then
     en.whiteframe=true
	 en.ded=true
    end
	 
    enm=enm or burn
   end
  end
  
  if diff%9==0.25 then
   --_draw()
  end
  
  if enm or i%16==0 then
   update_shake()
   camera(camx+shkx,camy+shky)
   local ax,ay=orx-roomw*4,ory-roomw*4
   rectfill(ax,ay,ax+roomw*8-1,ay+roomw*8-1,0)
   _draw()
   flip()
  end
 end
 
 add_shake(8)
 
 camx-=8*dx
 camy-=8*dy
end

function gen_room(rmx,rmy,dif)
 local mx=flr(rmx/8-roomw*0.5)%32
 local my=flr(rmy/8-roomw*0.5)%32

 zone=flr(diff/3)
 
 local loopin=false
 if zone>=3 then
  zone=zone%3
  loopin=true
 end
 
 local tileanc=zone*8
 
 for x=0,roomw-1 do
 for y=0,roomw-1 do
  local c=0
  if rnd(10)<1 then c=4+flr(rnd(4))
  else c=flr(rnd(4))
  end
  
  if loopin and rnd(15)<2 then
   c+=flr(rnd(3))*8
  else
   c+=tileanc
  end
  
  mset((mx+x)%32,(my+y)%32,c)
 end
 end
 
 -- hp tile
 if rnd(1)<1 then
  local x=(mx+flr(rnd(7)))%32
  local y=(my+flr(rnd(7)))%32
  
  if mget(x,y)<24 then
   if diff<9 then
    mset(x,y,40)
   elseif diff<18 then
    if rnd(4)<1 then
	 mset(x,y,40)
	else
	 mset(x,y,42)
	end
   else
    hppickup+=1
	if hppickup%2<1 then
     if rnd(4)<1 then
	  mset(x,y,40)
	 else
	  mset(x,y,42)
	 end
	else
     mset(x,y,41)
	end
   end
  end
 end
 
 -- spawn tiles
 local enems=flr(min(2+diff*0.5,min(5+flr(diff/9),9)))
 
 while enems>0 do
  local x=(mx+flr(rnd(7)))%32
  local y=(my+flr(rnd(7)))%32
  
  if mget(x,y)<24 then
   mset(x,y,30)
   enems-=1
  end
 end
 
 -- unlock tiles
 local topress=flr(min(3+diff*1.5,7*7-10))
 while topress>0 do
  local x=(mx+flr(rnd(7)))%32
  local y=(my+flr(rnd(7)))%32
  
  if mget(x,y)<24 then
   mset(x,y,24)
   topress-=1
  end
 end
 
 create_tiles(rmx,rmy)
end

camx=0 camy=0
shkx=0 shky=0
camrx=0 camry=0
function update_camera()
 local ocamx,ocamy=camx,camy
 
 camx=lerp(roomx-64,camx,0.1)
 camy=lerp(roomy-64-4,camy,0.1)
 
 camrx=camx-ocamx
 camry=camy-ocamy

 update_shake()
end


function add_shake(p)
 local a=rnd(1)
 shkx+=p*cos(a)
 shky+=p*sin(a)
end

function update_shake()
 if abs(shkx)<0.5 and abs(shky)<0.5 then
  shkx=0
  shky=0
 else
  shkx*=-0.6-rnd(0.2)
  shky*=-0.6-rnd(0.2)
 end
end



--- draws ---

function draw_player(p)
 if p.inv%0.04>0.02 then
  return
 end

 local foo=function(p)
            draw_anim(p.x,p.y-1,p.name,p.state,p.animt,p.faceleft)
           end
 
 draw_outline(foo,0,p)
 foo(p)
end

function draw_chomp(s)
 pal(1,0)
 for i=0,4 do
  local x,y
  x=lerp(s.x,s.rx,i/5)
  y=lerp(s.y,s.ry,i/5)
  spr(160,x-4,y-4)
 end
 pal(1,1)
 draw_self(s)
end

-- this func works only if there's a name field
function draw_self(s)
 local foo=function(s)
            local state=s.state or "only"
			local z=s.z or 0
            draw_anim(s.x,s.y-1+z,s.name,state,s.animt,s.faceleft)
           end
 local c=0
 if s.whiteframe then c=7 end
 draw_outline(foo,c,s)
 if s.whiteframe then all_colors_to(7) end
 foo(s)
 if s.whiteframe then all_colors_to() s.whiteframe=false end
end

function draw_arrow(ar)
 local foo=function(ar)
  line(ar.x,ar.y,ar.x-ar.vx,ar.y-ar.vy,4)
  pset(ar.x,ar.y,6)  
 end
 
 draw_outline(foo,0,ar)
 foo(ar)
end

function draw_bomb(b)
 if b.animt>0.5 then
  circ(b.x,b.y,b.reach,8+(b.animt*100)%2)
 end
 draw_self(b)
end

function draw_tileshine(ti)
 if not ti.pressed then return end
 
 draw_anim(ti.x,ti.y,"tile","shine",ti.animt)
end

function draw_room(x,y)

 local ax=x-roomw*4
 local ay=y-roomw*4
 
 rectfill(ax-7,ay-11,ax+roomw*8-1+7,ay+roomw*8-1+4,0)
 
 
 local mx=flr(ax/8)
 local my=flr(ay/8)
 local rx=mx*8-ax
 local ry=my*8-ay
 
 if rx==33 and ry==0 then
 
  for x=0,roomw-1 do
  for y=0,roomw-1 do
   spr(mget((mx+x)%32,(my+y)%32),ax+x*8+rx,ay+y*8+ry)
  end
  end
 
 else
 
  for x=0,roomw do
  for y=0,roomw do
   local s=mget((mx+x)%32,(my+y)%32)
   local sx=s%16*8
   local sy=flr(s/16)*8
   
   local xx=ax+x*8+rx
   local yy=ay+y*8+ry
   local xw,yh=8,8
   
   if x==0 then
    local oxx=xx
    xx=max(xx,ax)
    xw-=xx-oxx
    sx+=xx-oxx
   end
   if y==0 then
    local oyy=yy
    yy=max(yy,ay)
    yh-=yy-oyy
    sy+=yy-oyy
   end
   if x==roomw then
    local xx2=min(xx+8,ax+roomw*8)
    xw=xx2-xx
   end
   if y==roomw or y==roomw-1 then
    local yy2=min(yy+8,ay+roomw*8-2)
    yh=yy2-yy
   end
   
   sspr(sx,sy,xw,yh,xx,yy)
  end
  end
 
 end
 
 spr(72,roomx-roomw*4-4,roomy-roomw*4-8,8,8)
end

function draw_smoke(s)
 circfill(s.x,s.y,s.r,s.c)
end

function draw_explosion(e)
 if e.p<1 then
  circfill(e.x,e.y,e.r,0)
 elseif e.p<2 then
  circfill(e.x,e.y,e.r,7)
  if e.r>4 then
   for i=0,2 do
    local x=e.x+rnd(2.2*e.r)-1.1*e.r
    local y=e.y+rnd(2.2*e.r)-1.1*e.r
    local r=0.25*e.r+rnd(0.5*e.r)
    create_explosion(x,y,r,e.c)
   end
   
   for i=0,2 do
    create_smoke(e.x,e.y,1,e.c)
   end
  end
 elseif e.p<3 then
  circ(e.x,e.y,e.r,7)
 
  delete_registered(e)
 end
 
 e.p+=1
end

function init_clear_colors()
clrcols={
[0]={0,1,2,4,9,8},
{0,1,2,4,9},
{0,1,12,13,7},
--{0,1,2,8,9,10},
{0,2,8,9,10},
{0,2,8,14,7}
}
for j=0,#clrcols do
 local plt=clrcols[j]
 local nplt={}
 for i=1,#plt do
  nplt[plt[i]]=plt[i%#plt+1]
 end
 clrcols[j]=nplt
end
end

function draw_clear(k)
 for i=0,999 do
  local x,y=rnd(128),rnd(128)
  local c=pget(x+camrx,y+camry)
  
  
  zone=flr(diff/3)
  plt=clrcols[min(zone,3)+1]
  if plt[c] then
   if rnd(1.1)>=1 then
    c=plt[c]
   end
  else
   c=0
  end
  
  local a=atan2(x-64,y-64)

  circ(x+2*cos(a),y+2*sin(a),1,c)
 end
end

function draw_title_clear()
 for i=0,799 do
  local x,y=rnd(128),rnd(128)
  local c=pget(x+camrx,y+camry)
  
  plt=clrcols[0]
  if plt[c] then
   if rnd(1.2)>=1 then
    c=plt[c]
   end
  else
   c=0
  end
  
  local a=atan2(x-64,y-64)+0.15

  circ(x+2*cos(a),y+2*sin(a),1,c)
 end
end

function draw_gui()

 if player then
  if tiletopress<=0 then
   camera(xmod,ymod)
   local x1,y1,x2,y2
   x1=roomx-roomw*4-8
   x2=roomx+roomw*4+7
   y1=roomy-roomw*4-8-4
   y2=roomy+roomw*4+7-4  
   rect(x1-2,y1-2,x2+2,y2+2,0)
   local k=flr(2*cos(t*4))
   rect(x1+k,y1+k,x2+0.9999-k,y2+0.9999-k,6)
   camera(0,0)
   
   if diff==0 then
    draw_text("hold ⬅️ / ➡️ / ⬆️ / ⬇️    ",64,112+flr(4*cos(t*2)+0.5))
    draw_text("and press ❎ ",64,112+8+4*cos(t*2))
   end
  end
  
  draw_healthbar(3,3,0)
  draw_progression(124,3,2)
  
  if diff==9 then
   local y=112+flr(4*cos(t*2)+0.5)
   if tiletopress>5 then
    draw_text("!! well done !!",64,y,1,true)
   elseif tiletopress>4 then
    draw_text("!!! you beat the game !!!",64,y,1,true)
   elseif tiletopress>3 then
    draw_text("but more adventures await...",64,y,1,true)
   elseif tiletopress>2 then
    draw_text("in the...",64,y,1,true)
	add_shake(1)
   elseif tiletopress>1 then
    draw_text("!!! one room !!!",64,y,1,true)
	add_shake(2)
   elseif tiletopress>0 then
    draw_text("!!!! dungeon !!!!",64,y,1,true)
	add_shake(4)
   end
  end
  
  if player.rhppu>0 then
   draw_text(player.lhppu,4,20,0,true)
  end
  
 elseif loset==0 then
  --draw_healthbar(64,16,1)
  draw_text("g a m e   o v e r",64,38,1,true)
  draw_text("score: "..(flr(diff*4)),64,50,1,true)
  draw_progression(65+flr(2*cos(t*4)),60,1)
  if t%0.2<0.15 then
   draw_text("press ❎ to try again ",64,64+24,1,true)
  end
 end
end

function draw_healthbar(x,y,al)
 local w=48
 if al==1 then x-=w/2
 elseif al==2 then x-=w end
 
 rectfill(x-1,y-1,x+w+1,y+10+2,0)
 rect(x,y,x+w,y+10,7)
 line(x,y+11,x+w,y+11,13)
 
 if player then
  local x2=x+1+(w-2)*player.hp/player.hpmax
  rectfill(x+1,y+1,x2,y+9,2)
  line(x+2,y+2,x2-1,y+2,8)
 end
 
 local txt
 if player then
  txt=player.hp.."/"..player.hpmax
 else
  txt="0/4"
 end
 draw_text(txt,x+24,y+6)
end

function draw_progression(x,y,al)
 local w=58
 if al==1 then x-=w/2
 elseif al==2 then x-=w end
 
 --[[
 local foo=function(ox,oy)
   for i=0,diff/9 do
    spr(200,66+ox,3+oy,8,1)
	oy+=5
   end
   
   if player then
    spr(216,66+ox+ceil((diff/9)%1*57)-3,-1+oy)
   else
    spr(217,66+ox+ceil((diff/9)%1*57)-3,1+oy)
   end
  end
 
 pal(7,0)
 for x=-1,1 do
 for y=-1,2 do
  foo(x,y)
 end end
 pal(7,13)
 foo(0,1)
 pal(7,7)
 foo(0,0)
 --]]
 
 local s
 if player then s=216 else s=217 end
 
 local foo=function(ox,oy)
  for i=0,diff/9 do
   spr(200,x+ox,y+oy,8,1)
   oy+=5
  end
  spr(s,x+ox+ceil((diff/9)%1*57)-3,y-2+oy)
 end
 
 pal(7,0)
 for xx=-1,1 do
 for yy=-1,2 do
  foo(xx,yy)
 end end
 pal(7,13)
 foo(0,1)
 pal(7,7)
 foo(0,0)
end

function draw_text(str,x,y,al,extra)
 local al=al or 1

 if al==1 then x-=#str*2-1
 elseif al==2 then x-=#str*4 end
 
 y-=3
 
 if extra then
  print(str,x,y+3,0)
  print(str,x-1,y+2,0)
  print(str,x+1,y+2,0)
  print(str,x-2,y+1,0)
  print(str,x+2,y+1,0)
  print(str,x-2,y,0)
  print(str,x+2,y,0)
  print(str,x-1,y-1,0)
  print(str,x+1,y-1,0)
  print(str,x,y-2,0)
 end
 
 print(str,x+1,y+1,13)
 print(str,x-1,y+1,13)
 print(str,x,y+2,13)
 print(str,x+1,y,7)
 print(str,x-1,y,7)
 print(str,x,y+1,7)
 print(str,x,y-1,7)
 print(str,x,y,0)

end

function draw_outline(draw,c,arg)
 all_colors_to(c)
 
 camera(xmod-1,ymod)
 draw(arg)
 camera(xmod+1,ymod)
 draw(arg)
 camera(xmod,ymod-1)
 draw(arg)
 camera(xmod,ymod+1)
 draw(arg)
 
 camera(xmod,ymod)
 all_colors_to()
end

function init_titlescreen()
 local yy=1
 pal(7,13)
 sspr(0,96,63,26,0,yy+1,126,52)
 sspr(0,96,63,26,1,yy+2,126,52)
 sspr(0,96,63,26,2,yy+1,126,52)
 
 pal(7,7)
 sspr(0,96,63,26,0,yy,126,52)
 sspr(0,96,63,26,2,yy,126,52)
 sspr(0,96,63,26,1,yy-1,126,52)
 sspr(0,96,63,26,1,yy+1,126,52)
 
 pal(7,1)
 sspr(0,96,63,26,1,yy,126,52)
 pal(7,7)
 
 memcpy(0x4300,0x1000,0x1000)
 memcpy(0x1000,0x6000,0x1000)
 
 cls()
end

function draw_titlescreen()
  draw_title_clear()
  
  xmod=shkx
  ymod=shky
  camera(shkx,shky)
  
  local yy=64-32
  
  --[[
  pal(7,13)
  sspr(0,96,63,26,0,yy+1,126,52)
  sspr(0,96,63,26,1,yy+2,126,52)
  sspr(0,96,63,26,2,yy+1,126,52)
  
  pal(7,7)
  sspr(0,96,63,26,0,yy,126,52)
  sspr(0,96,63,26,2,yy,126,52)
  sspr(0,96,63,26,1,yy-1,126,52)
  sspr(0,96,63,26,1,yy+1,126,52)
  
  pal(7,0)
  sspr(0,96,63,26,1,yy,126,52)
  pal(7,7)
  --]]
  
  local foo=function()
    spr(128,0,yy-1,16,8)
   end
   
  draw_outline(foo,0)
  pal(1,0)
  foo()
  pal(1,1)
  
  draw_text("by trasevol_dog",1,yy+59,0,true)
  draw_text("#ld37",128,yy+59,2,true)
  
  foo=function() draw_anim(70,yy-6,"player","idle",t) end
  draw_outline(foo,0)
  foo()
  
  foo=function(x,y) draw_anim(x,yy,"player","run",t,(yy>64)) end
  for i=-4,4 do
   local xx
   xx=64+i*16+(t*40)%16
   yy=4
   draw_outline(foo,0,xx)
   foo(xx)
   xx=64+i*16-(t*40)%16
   yy=123
   draw_outline(foo,0,xx)
   foo(xx)
  end
  
  if t%0.25<0.15 then
   draw_text("press ❎ to start",64,108,1,true)
  end
  
end


--- creates ---

function create_player(x,y)
 local p={
  inv=0,
  hp=3,
  hpmax=4,
  rhppu=0,
  lhppu="",
  x=x or 64,
  y=y or 64,
  vx=0,
  vy=0,
  acc=0.5,
  spdcap=1,
  dec=0.8,
  w=4,
  h=4,
  name="player",
  state="idle",
  faceleft=true,
  animt=0,
  draw=draw_player,
  update=update_player,
  regs={objs.to_update,objs.to_draw[2]}
 }
 
 register_to_regs(p)
 
 return p
end

function init_spawn_table()

 zone_spawns={
  [1]={"zombie","zombie","slime","slime","skele"},
  [2]={"zombie","bat","skele","slime"},
  [3]={"bat","bat","skele","skele","slime","zombie"},
  [4]={"skele","ghost","ghost","bat"},
  [5]={"skele","ghost","chomp","chomp","bat"},
  [6]={"bomb","chomp","ghost","skele"},
  [7]={"bomb","bat","bat","skele","chomp"},
  [8]={"thrower","bat","skele","chomp"},
  [9]={"thrower","thrower","bat","bat","bat","chomp"},
  [10]={"zombie","bat","bomb","skele","ghost","chomp","slime","thrower"}
 }

 enemy_spawns={
  slime=create_slime,
  chomp=create_chomp,
  skele=create_skele,
  bat=create_bat,
  zombie=create_zombie,
  ghost=create_ghost,
  bomb=create_bomb,
  thrower=create_thrower
 }
end

function create_slime(x,y)
 local en={
  c=11,
  x=x,
  y=y,
  vx=0,
  vy=0,
  z=0,
  vz=0,
  acc=0.2,
  spdcap=4,
  dec=0.8,
  jump=2,
  w=4,
  h=3,
  attacking=false,
  aimx=0,
  aimy=0,
  name="slime",
  state="idle",
  faceleft=false,
  animt=rnd(0.05),
  draw=draw_self,
  update=update_slime,
  regs={objs.to_update,objs.to_draw[2],objs.enemies}
 }
 
 register_to_regs(en)
end

function create_chomp(x,y)
 local en={
  c=6,
  x=x,
  y=y,
  rx=x,
  ry=y,
  vx=0,
  vy=0,
  acc=0.05,
  spdcap=3,
  dec=0.95,
  reach=16,
  w=4,
  h=4,
  aimx=0,
  aimy=0,
  attacking=true,
  name="chomp",
  faceleft=false,
  animt=rnd(1),
  draw=draw_chomp,
  update=update_chomp,
  regs={objs.to_update,objs.to_draw[2],objs.enemies}
 }
 
 register_to_regs(en)
end

function create_skele(x,y)
 local en={
  c=6,
  x=x,
  y=y,
  vx=0,
  vy=0,
  acc=0.1,
  spdcap=3,
  dec=0.8,
  w=4,
  h=4,
  aimx=0,
  aimy=0,
  attacking=false,
  name="skele",
  faceleft=false,
  animt=0.04+rnd(0.12),
  draw=draw_self,
  update=update_skele,
  regs={objs.to_update,objs.to_draw[2],objs.enemies}
 }
 
 register_to_regs(en)
end

function create_arrow(x,y,aimx,aimy)
 local aim=atan2(aimx-x,aimy-y)
 
 local ar={
  c=4,
  x=x,
  y=y,
  vx=6*cos(aim),
  vy=6*sin(aim),
  w=2,
  h=2,
  attacking=true,
  draw=draw_arrow,
  update=update_arrow,
  regs={objs.to_update,objs.to_draw[2],objs.enemies}
 }

 register_to_regs(ar)
end

function create_bat(x,y)
 local en={
  c=13,
  x=x,
  y=y,
  vx=0,
  vy=0,
  acc=0.1,
  spdcap=1.1,
  dec=0.97,
  w=2,
  h=2,
  attacking=true,
  name="bat",
  faceleft=false,
  animt=rnd(1),
  draw=draw_self,
  update=update_bat,
  regs={objs.to_update,objs.to_draw[2],objs.enemies}
 }
 
 register_to_regs(en)
end

function create_zombie(x,y)
 local en={
  c=3,
  x=x,
  y=y,
  vx=0,
  vy=0,
  acc=0.1,
  spdcap=0.2,
  dec=0.8,
  w=4,
  h=4,
  attacking=true,
  name="zombie",
  faceleft=false,
  animt=rnd(1),
  draw=draw_self,
  update=update_bat,
  regs={objs.to_update,objs.to_draw[2],objs.enemies}
 }
 
 register_to_regs(en)
end

function create_ghost(x,y)
 local en={
  c=6,
  x=x,
  y=y,
  vx=0,
  vy=0,
  acc=0.1,
  spdcap=2,
  dec=0.9,
  w=2,
  h=2,
  attacking=true,
  name="ghost",
  faceleft=false,
  animt=rnd(1),
  draw=draw_self,
  update=update_bat,
  regs={objs.to_update,objs.to_draw[2],objs.enemies}
 }
 
 register_to_regs(en)
end

function create_bomb(x,y,vx,vy)
 local en={
  c=6,
  x=x,
  y=y,
  vx=vx or 0,
  vy=vy or 0,
  z=0,
  vz=-3-rnd(0.5),
  dec=0.8,
  w=2,
  h=2,
  reach=12,
  attacking=false,
  name="bomb",
  faceleft=(rnd(2)<1),
  animt=rnd(0.12),
  draw=draw_bomb,
  update=update_bomb,
  regs={objs.to_update,objs.to_draw[2],objs.enemies}
 }
 
 register_to_regs(en)
end

function create_thrower(x,y)
 local en={
  c=8,
  x=x,
  y=y,
  vx=0,
  vy=0,
  acc=0.1,
  spdcap=3,
  dec=0.8,
  w=4,
  h=4,
  attacking=false,
  name="thrower",
  faceleft=(rnd(2)<1),
  animt=rnd(0.16),
  draw=draw_self,
  update=update_thrower,
  regs={objs.to_update,objs.to_draw[2],objs.enemies}
 }
 
 register_to_regs(en)
end

function create_shapeshift(x,y)
 local en={
  c=8,
  x=x,
  y=y,
  vx=0,
  vy=0,
  acc=0,
  spdcap=4,
  dec=0,
  w=4,
  h=4,
  z=0,
  vz=0,
  jump=2,
  reach=16,
  aimx=0,
  aimy=0,
  attacking=true,
  name="shapeshift",
  faceleft=(rnd(2)<1),
  animt=0,
  kt=0.10+rnd(0.08),
  draw=draw_self,
  update=update_shapeshift,
  regs={objs.to_update,objs.to_draw[2],objs.enemies}
 }
 
 register_to_regs(en)
end

function create_tile(x,y)
 local ti={
  x=x,
  y=y,
  w=8,
  h=8,
  pressed=false,
  animt=0,
  draw=draw_tileshine,
  update=update_tile,
  regs={objs.to_update,objs.to_draw[1],objs.tiles}
 }
 
 register_to_regs(ti)
 
 return ti
end

function create_spawntile(x,y)
 local ti={
  x=x,
  y=y,
  w=24,
  h=24,
  update=update_spawntile,
  regs={objs.to_update,objs.tiles}
 }
 
 register_to_regs(ti)
 
 return ti
end

function create_hptile(x,y,hp)
 local ti={
  x=x,
  y=y,
  w=8,
  h=8,
  hp=hp,
  update=update_hptile,
  regs={objs.to_update,objs.tiles}
 }
 
 register_to_regs(ti)
 
 return ti
end

function create_tiles(rmx,rmy)
 local mx=flr(rmx/8-roomw*0.5)%32
 local my=flr(rmy/8-roomw*0.5)%32
 
 tiletopress=0
 for x=0,6 do
 for y=0,6 do
  local c=mget((mx+x)%32,(my+y)%32)
  if c==24 then
   tiletopress+=1
   create_tile(rmx-roomw*4+x*8+4,rmy-roomw*4+y*8+4)
  elseif c==30 then
   create_spawntile(rmx-roomw*4+x*8+4,rmy-roomw*4+y*8+4)
  elseif c==40 then
   create_hptile(rmx-roomw*4+x*8+4,rmy-roomw*4+y*8+4,2)
  elseif c==42 then
   create_hptile(rmx-roomw*4+x*8+4,rmy-roomw*4+y*8+4,1)
  end
 end
 end
end


function create_explosion(x,y,r,c)
 local e={
  x=x,
  y=y,
  r=r,
  p=0,
  c=c or 6,
  draw=draw_explosion,
  regs={objs.to_draw[4]}
 }
 
 register_to_regs(e)
end

function create_smoke(x,y,spd,c)
 local a=rnd(1)
 local spd=0.75*spd+rnd(0.5*spd)
 
 if rnd(2)<1 then c=col_drk[c] end
 
 local s={
  x=x,
  y=y,
  vx=spd*cos(a),
  vy=spd*sin(a),
  c=c,
  r=1+flr(rnd(3)),
  update=update_smoke,
  draw=draw_smoke,
  regs={objs.to_update,objs.to_draw[3]}
 }
 
 register_to_regs(s)
end


--- anim ---

function init_anim_info()
 anim_info={
  player={
   idle={
    sprites={32,32,33,33,32,32,33,33,32,32,33,32,33,34,35,36,37,38,39},
    dt=0.04
   },
   run={
    sprites={48,49,50,51},
    dt=0.03
   },
   againstwall={
    sprites={52,53},
	dt=1
   }
  },
  tile={
   shine={
    sprites={56,57,58,59,60,61,62,63},
	dt=0.03
   }
  },
  slime={
   idle={
    sprites={64,65,66,64},
	dt=0.04
   },
   jump={
    sprites={67,68,69},
	dt=0.04
   }
  },
  chomp={
   only={
    sprites={70,71},
	dt=0.04
   }
  },
  skele={
   only={
    sprites={96,97,96,97,98,99,100,101,101,102,102,102,103},
	dt=0.04
   }
  },
  bat={
   only={
    sprites={112,113,114,115},
	dt=0.03
   }
  },
  zombie={
   only={
    sprites={116,117,118,119},
	dt=0.04
   }
  },
  thrower={
   only={
    sprites={128,129,130,129,130,131,132,131,132,133,133,133,134,135},
	dt=0.04
   }
  },
  ghost={
   only={
    sprites={144,145,146,147,148,149,150,151},
	dt=0.04
   }
  },
  bomb={
   only={
    sprites={80,81,80,81,80,81,82,83,84,83,84,83,84,85,87,86,86,86,87,86,86,87,86,86},
	dt=0.03
   }
  },
  shapeshift={
   only={
    sprites={161,162,163,164,165,166,167},
	dt=0.03
   }
  }
 }
end

function draw_anim(x,y,char,state,t,xflip)
 local sinfo=anim_info[char][state]
 local spri=sinfo.sprites[flr(t/sinfo.dt)%#sinfo.sprites+1]

 local wid=sinfo.width or 1
 local hei=sinfo.height or 1

 local xflip=xflip or false

 spr(spri,x-wid*4,y-hei*4,wid,hei,xflip)

end

function animt_to_step(animt,char,state)
 local state=state or "only"
 return flr(animt/anim_info[char][state].dt)%#anim_info[char][state].sprites
end

function anim_get_steps(char,state)
 local state=state or "only"
 return #anim_info[char][state].sprites
end



--- collisions ---

function collide_objgroup(obj,group)
 for obj2 in all(objs[group]) do
  if obj2~=obj then
   local bl=collide_objobj(obj,obj2)
   if bl then
    return obj2
   end
  end
 end

 return false
end

function collide_objobj(obj1,obj2)
 return (abs(obj1.x-obj2.x)<(obj1.w+obj2.w)/2
     and abs(obj1.y-obj2.y)<(obj1.h+obj2.h)/2)
end

function stay_in_box(obj,x1,y1,x2,y2)
 obj.x=clamp(obj.x,x1+obj.w/2,x2-obj.w/2)
 obj.y=clamp(obj.y,y1+obj.w/2,y2-obj.h/2)
end

function stay_in_boxb(obj,x1,y1,x2,y2)
 local orx,ory=obj.x,obj.y
 obj.x=clamp(obj.x,x1+obj.w/2,x2-obj.w/2)
 obj.y=clamp(obj.y,y1+obj.w/2,y2-obj.h/2)
 return (obj.x~=orx or obj.y~=ory)
end


--- objects handling ---

function update_objects()
 local uobjs=objs.to_update
 
 for obj in all(uobjs) do
  obj.update(obj)
 end
end

function draw_objects()
 for i=0,4 do
  local dobjs=objs.to_draw[i]
 
  --sorting objects by depth
  for i=2,#dobjs do
   if dobjs[i-1].y>dobjs[i].y then
    local k=i
    while(k>1 and dobjs[k-1].y>dobjs[k].y) do
     local s=dobjs[k]
     dobjs[k]=dobjs[k-1]
     dobjs[k-1]=s
     k-=1
    end
   end
  end
 
  --actually drawing
  for obj in all(dobjs) do
   obj.draw(obj)
  end
 end
end

function register_to_regs(o)
 for reg in all(o.regs) do
  add(reg,o)
 end
end

function delete_registered(o)
 for reg in all(o.regs) do
  del(reg,o)
 end
end

function group(name) return all(objs[name]) end



--- utilities ---

function all_colors_to(c)
 if c then
  for i=0,15 do
   pal(i,c)
  end
 else
  for i=0,15 do
   pal(i,i)
  end
 end
end

function ceil(a) return flr(a+0x.ffff) end
function lerp(a,b,i) return i*a+(1-i)*b end
function clamp(a,mi,ma) return min(max(a,mi),ma) end
function sqr(a) return a*a end

__gfx__
99494494994944949499949466d66d6d9944994494949944949949949449499477c7c77c7c77c7cc77c7c77c66d6d66d77c7c77c7cc77c7c77c77c7c66dd6d6d
9444444494444444444444426dddddd1444444424444444244444442946664427cccccccccccccc17c777cc16dddddd1ccccccccccccccccccccccc16dddddd1
444444424444444294444444ddddd1d194844444944444429446644446ddd142ccccccc17cccc1ccc777ccc76dddddd17cccc7c17cccccc17ccc1cccddddd1d1
9444444294244442944424426dddd6dd48e844429444c442946dd14296d114447ccccccccc1cc7cc777ccc77dddddddd7c7c7b7c7c7c7ccc7cc1d7c16dd116dd
444444444494424444424942ddddddd194844842944c7c4446ddd142441444447cccccc17c7cccc177ccc7716d1dddd1c7b7c7c1ccc7ccc1cc1dd7c1dd166dd1
944444449444494494449444dddddddd94448e824444c4424411144494444642cccccccc7cccccc17ccc77ccdd6dddddcc7ccccc7c7c7ccc7cc77ccc6d6ddddd
9444444294444442444444426dddddd144444844944444449444444494446d147cccccc17ccccccc7cc77cc16dddddd17cccccc17cccccc1ccccccc16dddddd1
424224224242242242242422d1d1d11142442242422422424244242242424142c1c11c11c11cc1c1c1771c11d11d1d11c1c11c11c1c11c11c1c11c11d11d1d11
99889898989899889899889866d66d6d98998988989989889989899866d66d6d7777777677777778777777797777777b7777777c7777777d2288882211dddd11
9888888888888882988888886dddddd18888888288888882988888886dddddd17666666d78888882799999947bbbbbb37cccccc17dddddd1282222221d111111
888888829888828888882882ddddddd19889888298888a8288822882dd1d1dd1766dd66d78822882799449947bb33bb37cc11cc17dd11dd182222220d1111110
9888888898888982988298886ddddddd889a98888888aea8982aa9886d616ddd76d6676d78288782794997947b3bb7b37c1cc7c17d1dd7d182022020d1011010
888888828828888288298882dddd1dd19889888298a88a82982aaa92dd161dd176d6676d78288782794997947b3bb7b37c1cc7c17d1dd7d182222220d1111110
988888888898888898988882dddd6ddd888888888aea8888882aaa98dd6d6ddd7667766d78877882799779947bb77bb37cc77cc17dd77dd12222220211111101
9888888298888882988888886dddddd19888888298a88882988999826dddddd17666666d78888882799999947bbbbbb37cccccc17dddddd1282022021d101101
882288228228282282228282d1d1d111882828228828282282822822d1d1d1116ddddddd8222222294444444b3333333c1111111d11111112202002211010011
00000000000000000000000000000000000000000000000000000000000000007766767677667676776676760000000000000000994944940020220088888882
000770000000000000707707000007700070770700077000007077070000000076d2276d76d1176d76d2276d0000000000000000944444440002400082000020
00077000000770000070770700000770007077070007700000707707000770006dd287d66dd117d66dd2e7d60000000000000000444444420004400080222280
00777700000770000007777000000770000777700007700000077770700770076222822d6111111d6222e22d0000000000000000944444420056650080022080
0707707007777770000077000000077000007700000770000000770007777770728888867111111672eeeee60000000000000000444444440606d06080222280
070770707007700700777700000077700000777700077700007777000007700067728776677117766772e7760000000000000000944444440505505082028820
000707000007770000000700000007000000700000007000000007000007770076d2876676d1176676d2e7660000000000000000944444420006060082082820
00700700007707000000070000000700000070000000700000000700007707006d6d66dd6d6d66dd6d6d66dd00000000000000004242242200d00d0020000000
00000000000000000000700000000000000000770707707000000000000000007777777700000000777600007777777600000677000000060000000000000000
00000000000000000007700000077070000000770707707000000000000000007777777700000000760000007777600007777777000000070000000000000000
00077000070770000007700000077070000000770077770000000000000000007777777700000000700000007776000007777776000000070000000000000000
00077000070770000007700000777700000000770007700000000000000000007777777700000000600000007760000007777770000000670000000000000000
77777777007777000007700007077000000000770007700000000000000000007777777700000000000000007600000007777770000006770000000600000000
00077000000770700007700007077000000000770770070000000000000000007777777700000000000000007000000067777770000067770000000700000000
07700700007770700007700000070770000000070000070000000000000000007777777700000000000000007000000077777770000677770000006700000000
00000700000070000007000000700000000000000000000000000000000000007777777700000000000000006000000077600000677777770000677700000000
00000000000000000000000000000000000000000000000000000000000d66d000d6d66d66d6d66dd6d66d66d6d66d6d66d666d6d66d6d66dd6d66d6d66d6d00
00000000000000000000000000000000000000000000000000000000006dd10006dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd60
000000000000000000000000000000000000000000007b0000d66d0006dd10000dd6666d666d666d666d66d66d6d666d66d66d666d66d6d666d666d6666d6dd0
00000000000000000007b0000000000000000000000733b006dddd6006dd000006d66dd16dd16dd16dd16d16d6d16dd16d16d16dd16d6d16dd16dd16ddd16d60
0077bb000007b00000733b000077bb00007b0000007333b006dd11d00ddd000006d6d111d111d111d111d11d1111d111d11d11d111d1111d111d111d11116d60
073333b000733b0000733b00073333b00733b00000b33b000dd6d6600ddd600006dd666d66d666d66d666d666d6666d666d6666d66d666d66666d666d666dd60
0b3333b00b3333b000b33b000b3333b0b3333b00000bb00006dddd10001dd6000ddd6dd16d16dd16d16dd16dd16ddd16dd16ddd16d16dd16dddd16dd16d1ddd0
00bbbb0000bbbb00000bb00000bbbb000bbbbb0000000000001d11000001111006d6d111d11d111d11d111d111d1111d111d1111d11d111d11111d111d116d60
0000000000000000000000000000000000000000000000000000000000000000edd6100000000000000000000000000000000000000000000000000000016dde
0000000000000000000000000000000000000000000000000000000000000000e6dd00000000000000000000000000000000000000000000000000000000dd6e
0000000000000000000000000000000000000000000000000000000000000000edd6000000000000000000000000000000000000000000000000000000006dde
0000590000005800000057000000800000009000000070000000000000000000e6d6000000000000000000000000000000000000000000000000000000006d6e
0001100000011000000110000001100000011000000110000001100000077000e6d6000000000000000000000000000000000000000000000000000000006d6e
0016110000161100001611000016110000161100001611000016110000777700eddd00000000000000000000000000000000000000000000000000000000ddde
0011110000111100001111000011110000111100001111000011110000777700e6d6000000000000000000000000000000000000000000000000000000006d6e
0001100000011000000110000001100000011000000110000001100000077000edd6000000000000000000000000000000000000000000000000000000006dde
0000000000000000000000000000000000000100000000000000000000000000e6dd00000000000000000000000000000000000000000000000000000000dd6e
0077000000000000007700000000010000775010007751000000000000775100e6d6000000000000000000000000000000000000000000000000000000006d6e
0067005100770000006700510077501000675010006500100077510000650010edd6000000000000000000000000000000000000000000000000000000006dde
0676650100670051067765010067501006765416067444160065001006444410e6d6000000000000000000000000000000000000000000000000000000006d6e
7070571006766501707057100676541607705010777500100644441077750010eddd00000000000000000000000000000000000000000000000000000000ddde
7067010070705710076701000770501000675010006751007775001000675100e6d6000000000000000000000000000000000000000000000000000000006d6e
0061700070677100006170000067501000607100006070000067510000607000e6d6000000000000000000000000000000000000000000000000000000006d6e
0600600006616000060060000660610006006000060060000660600006006000edd6000000000000000000000000000000000000000000000000000000006dde
0dd00d0000000d00000000000000000000000000000000000000000000005500edd6000000000000000000000000000000000000000000000000000000006dde
01dd0d20000d0d200dd00d0000000d0000005500000055000000550000053000e6d6000000000000000000000000000000000000000000000000000000006d6e
0011d1d000ddd1d001dd0d20000d0d2000053000000530000005300000033000e6d6000000000000000000000000000000000000000000000000000000006d6e
000011000dd111000011d1d000ddd1d000033000000330000003300000d63d36eddd00000000000000000000000000000000000000000000000000000000ddde
0001100000101100000011000dd1110000d63d3600d63d0000d63d30020d6000e6d6000000000000000000000000000000000000000000000000000000006d6e
00000000000010000001100000101100020d6000020d6036020d600600011300e6d6000000000000000000000000000000000000000000000000000000006dde
0000000000000000000000000000100000011300000113000001130000010010edd6000000000000000000000000000000000000000000000000000000006d6e
0000000000000000000000000000000003100100003101000031010000300000edd600000000000000000000000000000000000000000000000000000000dd6e
0020020000200200002002000000000000000000000000000000000000200200e6dd000000000000000000000000000000000000000000000000000000006d6e
0028820000228200002822000020020000200200002002000020020000288200e6d600000000000000000000000000000000000000000000000000000000dd6e
0008800000088000000880000022820000282200002882000028820020088002e6d6000000000000000000000000000000000000000000000000000000006dde
0822228000222200002222000008800000088000000880000008800008222280edd6000000000000000000000000000000000000000000000000000000006d6e
2008800208088080080880800022220000222200002222000822228000088000e6dd00000000000000000000000000000000000000000000000000000000ddde
0001100002011020020110200808808008088080280880822008800200011000edd6000000000000000000000000000000000000000000000000000000006d6e
0002010000020100000201000201112002211020002111000021110000200100e6d6000000000000000000000000000000000000000000000000000000006d6e
0010010000100100001001000012010000101100001001000010010000100100edd6000000000000000000000000000000000000000000000000000000006dde
0706707000067000070670700006700000000000000000000000000000000000e6dd00000000000000000000000000000000000000000000000000000000dd6e
0606706070067007060670607006700707067070000670000706707000067000edd6000000000000000000000000000000000000000000000000000000006dde
0077670006776760007767000677676006067060700670070606706070067007edd6000000000000000000000000000000000000000000000000000000006dde
0006700000667000000670000066700000776700067767600077670006776760e6dd00000000000000000000000000000000000000000000000000000000dd6e
00d600000dd60000d0d600000dd60000d0067000006670000006700000667000edd6000000000000000000000000000000000000000000000000000000006dde
d000000000000000000000000000000000d600000dd60000d0d600000dd60000e6d6000000000000000000000000000000000000000000000000000000006d6e
0000000000000000000000000000000000000000000000000000000000000000e6d6000000000000000000000000000000000000000000000000000000006d6e
0000000000000000000000000000000000000000000000000000000000000000eddd00000000000000000000000000000000000000000000000000000000ddde
0000000060606666020220020303333306666660770770071011110160066606e6d6000000000000000000000000000000000000000000000000000000006d6e
000110000777707066666060777707000000000040440000000000000d0000ddedd6000000000000000000000000000000000000000000000000000000006dde
0016d100111000000080888860066666666660600ddddd0d1101100066660600edd600000000000000000000000000000000000000000000000000000000dd6e
016116100ddddd0d0000000000000000dddd0d0d0005505500dd00d010101110e6d6000000000000000000000000000000000000000000000000000000006d6e
01d11d1000000000002222020bb0bbbb07007777110100112220020200707077e6d6000000000000000000000000000000000000000000000000000000006dde
001d61000000111110101110d0ddddd010111101000000000000000020220202eddd000000000000000000000000000000000000000000000000000000006d6e
000110006606660006660600000000000dd0ddd0707077778088000811111111e6d600000000000000000000000000000000000000000000000000000000ddde
000000000dddd0d0d0ddddd03303330300000000060006660d0d0ddd77077077edd6000000000000000000000000000000000000000000000000000000006d6e
0000000000000000000000000000000000000000000000000000000000000000e6d6000000000000000000000000000000000000000000000000000000006d6e
0000000000000000000000000000000000000000000000000000000000000000e6dd00000000000000000000000000000000000000000000000000000000dd6e
0000000000000000000000000000000000000000000000000000000000000000edd6000000000000000000000000000000000000000000000000000000006dde
0000000000000000000000000000000000000000000000000000000000000000e6d6000000000000000000000000000000000000000000000000000000006d6e
0000000000000000000000000000000000000000000000000000000000000000eddd00000000000000000000000000000000000000000000000000000000ddde
0000000000000000000000000000000000000000000000000000000000000000e6d6d66d666d666d66d666d666d66d6666d66d6666d66d66d66d666d666d6d6e
0000000000000000000000000000000000000000000000000000000000000000edddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddde
0000000000000000000000000000000000000000000000000000000000000000ee6d66d6d66d6d66d6d66d6d66d666d66d6d6d66d666d6d66d6d66d66d66d6ee
07777000770007700777777000000000777770000777700007777000770007707777777777777777777777777777777777777777777777777777777777700000
77007700777007700770000000000000770007007700770077007700777077707700000700000700000770000070000070000077000007000007000007700000
77007700770707700770000000000000770007007700770077007700777777707700000000000000000770000000000000000077000000000000000007700000
77007700770707700777700000000000770007007700770077007700777777700000000000000000000000000000000000000000000000000000000000000000
77007700770707700770000000000000777770007700770077007700770707700000000000000000000000000000000000000000000000000000000000000000
77007700770707700770000000000000770077007700770077007700770007700000000000000000000000000000000000000000000000000000000000000000
77007700770077700770000000000000770077007700770077007700770007700000000000000000000000000000000000000000000000000000000000000000
07777000770007700777777000000000770077000777700007777000770007700000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
77777700077000770077000770007777770007777777000777777000770007700007700000777700000000000000000000000000000000000000000000000000
77777770077000770077000770077777777007777777007777777700770007700077770007777770000000000000000000000000000000000000000000000000
77000770077000770077700770077000077007700000007700007700777007700077770007077070000000000000000000000000000000000000000000000000
77000770077000770077700770077000077007700000007700007700777007700077770007777770000000000000000000000000000000000000000000000000
77000770077000770077700770077000077007700000007700007700777007700000000000777700000000000000000000000000000000000000000000000000
77000770077000770077770770077000000007700000007700007700777707700000000000707700000000000000000000000000000000000000000000000000
77000770077000770077770770077000000007700000007700007700777707700000000000000000000000000000000000000000000000000000000000000000
77000770077000770077770770077000000007777700007700007700777707700000000000000000000000000000000000000000000000000000000000000000
77000770077000770077777770077007777007777700007700007700777777700000000000000000000000000000000000000000000000000000000000000000
77000770077000770077077770077007777007700000007700007700770777700000000000000000000000000000000000000000000000000000000000000000
77000770077000770077077770077000077007700000007700007700770777700000000000000000000000000000000000000000000000000000000000000000
77000770077000770077077770077000077007700000007700007700770777700000000000000000000000000000000000000000000000000000000000000000
77000770077000770077007770077000077007700000007700007700770077700000000000000000000000000000000000000000000000000000000000000000
77000770077000770077007770077000077007700000007700007700770077700000000000000000000000000000000000000000000000000000000000000000
77777770077777770077000770077777777007777777007777777700770007700000000000000000000000000000000000000000000000000000000000000000
77777700007777700077000770007777770007777777000777777000770007700000000000000000000000000000000000000000000000000000000000000000
__label__
00000000000000000000001000000001000001010000000000000000001000100000000010100000000000000111010100000042110001010000001101010011
00000000001000100000070000000000000010101110000000000000000000000000000001000000000007010010001000000702200000100000070110000000
00770700000107000077000000000000007701000001100000770700000000000077000000000000007707091000000000770702100407000077070100001000
00770000000100000077000101000000007700001000010000770000010000000077000000000000007700099000000000770001714040000077000110010000
77777770001010077777777010000007777777700010000777777770101701077777777000000007777777709000000777777770100440077777777001000007
00770000000100000077000111000000007700000101000000770000110110000077000420000100007700091000240000770001994444000077000000100000
77007000001000007700700001000000770070000000000077007000411111007700704242004010770070010102024077007009919440407700709001010090
00007000110000000000700400100000004070402000400000107000044190100000702422141402000070491410202200007009991144010020704000100990
20110101000002020222014042011012004404224200940401210000484949410912011121214142204904014142022200140299991414121212049011001919
11911810010020212021221421211121222442442429494442120809808294149494111121011444424420401410209221212929291041412122424111112191
09008188101102001002119142911142924222224242942229298040982818114944411110100104442214049440010212194242920404902214224411211212
10000888811110100000092991991414442422222224222291989484008181911444411101000110494229000104101011119122220044020212122211121121
94000408411111010000099919119044844222222422422021981944080912114114111120001110949220001010010101111222222024402021212221111122
49491044111111101009989981888448442822224249940200110110429090021111911202000111094242040101000012109121212202000212122211111212
84214111111111010090998898889844422224222429992001001101242900012119101120120111129922102010100101010012121922200021212211111442
22121191111111108909989499448499420229442242922220410010121120101201410101212011292921220201201010101111212402121002142111114444
02219911111111199890894948944999902082222222292222141001011102010014142101121212122202212012100101010111110440210101101211114442
20098914191221989988449984899998228828922222221222412110111020101001422212111111221222002211100011010011101001411014412111141422
00800801999999898888899998949982882882229222222121221211111101011100022121211411112100000011111010100001210011211444421201124222
88008022999999980088989999994448288999222192291222222111111110101000000112114141111010000001110001010010110002121444102110012211
88080202298998900808899999994224299992222211911222222111111010000000001021211441111101000001110000100000000001214411012111022221
80800020999980090090998819992422929992222121111121221111110101000000000102111114111110100001111000100000000011111141120210102210
08000008999908002909898108999929249112221221111141112211110010000100000000411110111101010000110001010000000001111010122000002001
00000080898980808100411080899292411111122111112414121222200000000000707707042101011210111000010001001000000001211001011100200010
88000001909098081414442100881020141111212111121141211112000000000000707707021010101121011102100000000000000202110100011112020100
00000018090100814144444201110100011181121211211111111111000000000000077770201100010110101120220000001001000021101010021000210010
00018181001000121149442111111000121111222111111110011111101000000000007700210100001111011112222000000100000212101010000000100000
01101810100840212011011191111111212212122211111100001011010100000000007777011010000111101101221000000000000021000100000000000010
18010108080484420101110411111411222220221211111020000101001011000000007000111101000001000010110000000000000011100000000000000101
00801080898844842010104041144110122200002221211102142411011111100000007000111110000000000001111000000000000001010000001000000010
10000000000448489000000020400001002000000000000011414242110111000000000000000110000000000000111000000000000000000000010000000000
00077777777044890777700001077770000777777777777001141421211000100777777777700000000777777770011100077777777000000777700000077770
00700000000704007000070010700007007000000000000702111012110101007000000000070000007000000007001100700000000700007000070000700007
07700000000770007000077000700007007000000000000700210101101010007000000000077000077000000007701007700000000770007000077007700007
70000777700007007000000700700007007000077777777d00001011110100107000077777700700700007777000070070000777700007007000000770000007
700007dd70000700700000077070000700700007ddddddd00000001111110100700007dddd700700700007dd70000700700007dd700007007000000770000007
70000700700007007000077007700007007000070000000200022211111110107000070000700700700007007000070070000700700007007000000000000007
70000700700007007000077007700007007000077770120020222221110001007000070000700700700007007000070070000700700007007000000000000007
70000700700007007000077007700007007000000007094004242102100000007000070000700700700007007000070070000700700007007000000000000007
70000700700007007000077007700007007000000007022422414211010100007000077777700700700007007000070070000700700007007000000000000007
7000070070000700700007700770000700700007777d029221442121101010007000000000077d00700007007000070070000700700007007000077007700007
7000070070000700700007700770000700700007ddd0912214044210110101007000000000077000700007007000070070000700700007007000077007700007
7000070070000700700007700770000700700007000221002144040401101000700007777000070070000700700007007000070070000700700007d77d700007
7000070070000700700007700770000700700007002212200010424012100200700007dd70000700700007007000070070000700700007007000070dd0700007
7000070070000700700007d770000007007000070000000211211024210122207000070070000700700007007000070070000700700007007000070000700007
70000777700007007000070d70000007007000077777777021121010201012207000070070000700700007777000070070000777700007007000070000700007
d770000000077d0070000700d7700007007000000000000702212101010101007000070070000700d770000000077d00d770000000077d007000070220700007
0d7000000007d000700007000d7000070070000000000007022220101010101070000700700007000d7000000007d0000d7000000007d0007000070000700007
00d77777777d0000d7777d0000d7777d00d777777777777d0202022111000010d7777d00d7777d0000d77777777d000000d77777777d0000d7777d0000d7777d
000dddddddd000100dddd000100dddd0000dddddddddddd040002202100000000dddd0000dddd000000dddddddd00000100dddddddd000000dddd000010dddd0
00000000000000000000000100000000000000000000000000001021000000000000000000000000000000000000000000000000000000000000000000000000
07777777777770101007777010000777700007777011000777701100077777777777701000077777777777777000000777777777777000000777700001077770
70000000000007010070000700007000070070000701007000070100700000000000070000700000000000000700007000000000000700007000070000700007
70000000000007701070000700007000070070000700107000070007700000000000077000700000000000000700077000000000000770107000070000700007
70000000000000070070000700007000070070000700007000070070000000000000000700700000000000000700700000000000000007007000070000700007
70000000000000070070000700007000070070000770007000070070000000000000000700700000000000000700700000000000000007007000077000700007
70000777777000070070000700007000070070000007007000070070000777777770000700700007777777777d00700007777777700007007000000700700007
700007dddd70000700700007000070000700700000070070000700700007dddddd70000700700007ddddddddd000700007dddddd700007007000000700700007
70000700007000070070000700007000070070000007007000070070000700000070000700700007000000000000700007000000700007007000000700700007
70000700007000070070000700107000070070000007007000070070000700001070000700700007011000000010700007011110700007007000000700700007
70000701007000070070000700007000070070000007007000070070000700000070000700700007010000000000700007001110700007007000000700700007
70000700107000070070000700007000070070000007707000070070000700000070000700700007000000000000700007000010700007007000000770700007
700007010070000700700007000070000700700000000770000700700007010000d7777d00700007000000001010700007000020700007007000000007700007
7000070000700007007000070000700007007000000007700007007000070000000dddd000700007000000010000700007000110700007007000000007700007
70000702007000070070000700007000070070000000077000070070000700000010000000700007000000000000700007001010700007007000000007700007
70000700007000070070000700007000070070000000077000070070000700000001000000700007777770000000700007001110700007007000000007700007
70000700007000070070000700007000070070000000077000070070000700100000000000700000000007000000700007000110700007007000000007700007
70000700007000070070000701007000070070000000077000070070000701077777777000700000000007000000700007001010700007007000000007700007
70000700007000070070000700107000070070000000000000070070000700700000000700700000000007000020700007000100700007007000000000000007
70000700107000070070000701007000070070000000000000070070000700700000000700700000000007000020700007001010700007007000000000000007
7000070110700007007000070000700007007000077000000007007000070070000000070070000777777d010120700007000100700007007000077000000007
70000700107000070070000700007000070070000770000000070070000700700000000700700007ddddd0101010700007001010700007007000077000000007
70000700007000070070000700207000070070000770000000070070000700d77770000700700007000001010100700007011100700007007000077000000007
700007000070000700700007021070000700700007700000000700700007000ddd70000700700007000000100110700007011110700007007000077000000007
70000700007000070070000701007000070070000770000000070070000700000070000700700007000001001010700007001110700007007000077000000007
70000700007000070070000700007000070070000770000000070070000700000070000700700007000000010100700007001100700007007000077000000007
700007000070000700700007010070000700700007d770000007007000070000007000070070000700000010000070000701110070000700700007d770000007
7000070000700007007000070000700007007000070d700000070070000700000070000700700007000000000000700007001000700007007000070d70000007
70000700007000070070000700007000070070000700700000070070000700000070000700700007000000000000700007000000700007007000070070000007
70000777777000070070000777777000070070000700700000070070000777777770000700700007777777777000700007777777700007007000070070000007
70000000000000070070000000000000070070000700d770000700700000000000000007007000000000000007007000000000000000070070000700d7700007
700000000000000700700000000000000700700007000d700007007000000000000000070070000000000000070070000000000000000700700007000d700007
700000000000077d00d77000000000077d00700007000070000700d7700000000000077d00700000000000000700d7700000000000077d007000070000700007
70000000000007d0010d700000000007d0007000070000700007000d70000000000007d0007000000000000007000d70000000000007d0107000070000700007
d777777777777d000000d7777777777d0000d7777d0100d7777d0000d777777777777d0000d77777777777777d0000d777777777777d0000d7777d0000d7777d
0dddddddddddd00000000dddddddddd000000dddd010100dddd000000dddddddddddd000000dddddddddddddd000000dddddddddddd000000dddd000000dddd0
00000000000000000000000000000000000000000001010000000000000000000000000101200000000000000101001000000000000201000000000000000000
07770707010007770777077700770777070700770700100007700077007701010000011112121102000001111211111214042244222070707000770077707770
70007070700070007000700077007000707077007070000070070700770070100000101100202120100000112121122122222242420707070707007700070007
707070707000d70770707070707770777070707070700010707070707077d0110400010100020011410001011211214222222224990700070707070777077707
7007700070010707700770007000700770707070707000007070707070770001409090100004010414140011112114142222224242070707070707077007d707
70707770700007077070707077707077700070707077077770707070707070111919092044404044444002012211214222242424240700070777070777070707
70007000701107077070707070077000770770077000700070007007700070011291910104440444441440222222422222424224940707070007000700070707
d777d777d0110d7dd7d7d7d7d77dd777dd7dd77dd777d777d777d77dd777d0000121122021104444044444222124242222242449490d7d7d777d777d777d0d7d
0ddd0ddd001100d00d0d0d0d0dd00ddd00d00dd00ddd0ddd0ddd0dd00ddd000011120222144144000024442112424422224248948480d0d0ddd0ddd0ddd010d0
00000000020200000000000000000000000000001000000000000000000000011110242444441440002044422224491844944988888808010000000400022104
00100000121010100010000000010000000000010100000000000000000010001110224444444200020204222222918189448898898888881121114444044242
00100000212101010101000000000000001000101100000000000000000110000101201444148020022042242221291819984889989188821222144440400444
01010001111210111010100000000000000111011000000000010020101112000011111141810802924424424412114199898998998812222220024208004444
00102001111021110010000000000000000011010201000000001202211121202011111198108049292242444144111119981989881822221200402421444444
01022001010111202001000000000000000012101020111000000122221112110101101119990911122124244441411118119108108222222114014244142440
00222000101111120000000011001000000002211002111100001012210111111210100111919111111212422444022121211000002222401041141141422214
00122000141012140011000011211100101022211020111110000102121202112122220012091119192120224424100112020000222244144909411412122241
01110101242221110011000102121101000000002000110000000000010000002222200020000012920000002000000010001002222424249499994191212119
01110111224221110001000000211110077707770777007700770000107777700201077700770101207707770777077707770120222242424999949419111912
00101012124222140000101001411110700070007000770077007000070000070110700077007011070070007000700070007002022214242419414191411124
0011011124242120000001011414111070707070707770777077d000700707007010d707707070007077d707707070707707d000022121120121141114142221
010111121142222100000010014111107000700770077000700070007000700070000707707070007000770770007007d7070000001211011112414449444412
20111211111212121000000010011110707770707077d77077707000700707007000070770707000d77077077070707077070000101111111111444494944141
00111121212224211200000100001110707d7070700070077007d000d7000007d00007077007d0007007d7077070707077070001011111011114144429421414
01011112122241411100001020012110d7d0d7d7d777d77dd77d00000d77777d00000d7dd77d0000d77d0d7dd7d7d7d7dd7d0010111111110111414102202184
001111112222241110000101102212110d000d0d0ddd0dd00dd0000010ddddd0000000d00dd000100dd000d00d0d0d0d00d00001101010181101141000020898
01101111221222411100101101012110100000000000200000000000110000000000000000001101000000000000101010000010111101808010002000009989
10011112422124414111101110201111010000000000000000000000101020200000000001010111101000000111010111010101241000080000100100144490
01011111242244491111222221010120101000001100020000000200010012000000000000100110011110001199109111102422440090808880000114404449
10141114121224949112122224101021110100001220122000000000100000100000000000001010011120011119994941122222424409888828041241429494
11114221442244494111211221410022011110002202012200001100001001000000000110001100001102020111119411112222242848229480422224292999
21442222412144441111111114140022214101000420122000011000110100000020200000010010000000202001111040102222229921994941111222122944
12442228121240494111111111400002241410110144111001101100011212010002220200001101000000020000100000101120099111199401111112021040
24442221804000041111111120000002224111101040401001111000102000101222422220000002100000201100000001011112001011100000011120000000
92949288040000004101111407000000220411110700000100111101070000010014042227000000000000011710000000010220270001000000101107000000
09294898070770000490111007077000274012120007700001111110070770000101412207077000070000010107700000002022070770000701001107077000
07929489000770000091111000077000000421210007700010111141000770000011010000077000070001100007700010201211000770001000000200077000
00004840777777770919111077777777000242207777777701071410777777770000101077777777001010007777777702010110777777770110000077777777
00000481000770000091912100077000001422220007700000000014000770000000010000077000010101200007700011101111000770000000000000077000
00001499007007700009121200700770111102422070077000000001007007700100000000700770001012120070077011011111007007700010000000700770
00014040107000000001902140700000011104222070000000000000107000001010010000700000000001202070000000110010007000000101000010700000
00000401000000001010100101010000101010022200000000000100010000010110101100000000100000121100011110000001100110110000000000000001

__map__
01020001050301080808080808080000000000000000000000000000000000000102000105030118181818181818010a1201150b01000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0200051804000008080808080808000000000000000000000000000000000000020005180400001828181e1828181018051807180e000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
030400020001000808080808080800000000000000000000000000000000000003040002000100181818181818180904000200000c000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
01000600020000080808080808080000000000000000000000000000000000000100060002000018181e181e181801032a282a0600000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000010000000108080808080808000000000000000000000000000000000000000001000000011818181818181807000100040017000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0218042800180508080808080808000000000000000000000000000000000000021804280018051818182818181802180018021814000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0003000000040208080808080808000000000000000000000000000000000000000300000004021818181818181816130000000f02000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
1010101010101010000000040000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000006000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000006070000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000400000000000700000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
__sfx__
010e00000d5560355612556085560f556035560f55603556125550655212555065520d555015520d555015521455514555085520855216555165550a5520a552125551255506552065520f5550f5550355203552
010e00001275212042120321202212752120421203212022147521404214032140221475214042140321402212752120421203212022127521204212032120221675216042160321602216752160421603216022
010e0000125551255206555065520d5550d5520155501552145551455208555085520f5550f5520355503552125551255206555065521455514552085550855216555165520a5550a5520f5550f5520355503552
010e0000125560655612556065560d556015560d55601556145550855214555085520f555035550f552035521255606556125560655614555085551455208552165560a556165560a5560f555035550f55203552
010e00000d7560f75612756147560f7560f7560f7560f756127521204212032120220d7520d0420d0320d0221475214042140321402216752160421603216022127521204212032120220f7520f0420f0320f022
0108000012343295451b5550f50500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010800000e3431c343355353055512000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010800002a64501635000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010a00001b3432a635336350000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
011000003364300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
011000000243200000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0108000028555295552d5552300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010e000011333103230e3130c31300313003130000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010a00000f36303353103230430311303073030430300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
01080000243631b363276530335310323073030430300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0106000026045000001a0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010a00001b343123332763533625276151b6150f61503615040060430304006000000000000000000000000004006043030400600000000000000000000000000000000000000000000000000000000000000000
010200000061300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010300001605300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010600001004300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
01100000010550104501035010250605506045060350602501055010450103501025030550304503035030250105501045010350102508055080450803508025010550104501035010250a0550a0450a0350a025
011000000a0550a0450a0350a0250a0550a0450a0350a025080550804508035080250605506045060350602503055030450303503025060550604506035060250105501045010350102503055030450303503025
01100000060550604506035060250601506015060050600508055080450803508025080150801503005030050a0550a0450a0350a025030550304503035030250105501045010350102501015010150a0050a005
011000000605506045060350602506055060450603506025060150601501005010050305503045030350302503055030450303503025030150301508005080050105501045010350102501015010150100501005
010e000001555035550655508555035550355503555035550655506555065550655501555015550155501555085550855508555085550a5550a5550a5550a5550655506555065550655503555035550355503555
010400000313112131125050000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010c000012347061370354706335124450613506245033350f4450323503545034350f4450333503245031350d3450123501345014370d3470113701545014351434508437083470833514245084370814508535
010c000012147062350624706337123470653706245063350f2450313703545033350f14703235033450313512245063350634506337124470633706445065351124705335054450553511247053350524505035
010c0000122470613706345062351254506435064470633512145064350634506237124450653506145062350f4450333503345034350f145032370344703137162470a5370a3450a435162450a3350a1450a435
010c0000162450a1350a3470a537144470a1370a2450a33514547084350814508237144470853508445085371434508135082470853714145084370824508535161450a1350a2450a3350d145012370144701535
011000000805508045080350802508015080150300503005010050100501005010050100501005000000000006055060450603506025060150601500000000000000000000000000000000000000000000000000
01100000030550304503035030250301503015030050300501005010050100501005010050100500000000000a0550a0450a0350a025060550604506035060250305503045030350302503015030150300503005
0107000025055030350605508035220550a0350a0550a035270550303503055030350f0550303503055030352a05506035060550603506055060350605506035250550103501055010350d055010350105501035
010700002c055080350805508035200550803508055080352e0550a0350a0550a035160550a0350a0550a0352a0550603506055060351e055060350605506035270550303503055030350f055030350305503035
010700002005508035080550803520055080350805508035220550a0350a0550a035160550a0350a0550a0351e0550603506055060351e0550603506055060351b0550303503055030350f055030350305503035
010700001405508035080550803514055080350805508035160550a0350a0550a0350a0550a0350a0550a03512055060350605506035120550603506055060350f05503035030550303503055030350305503035
0107000012055060350605506035120550603506055060351b0550303503055030350f0550303503055030352005508035080550803508055080350805508035220550a0350a0550a035160550a0350a0550a035
01070000220550a0350a0550a035160550a0350a0550a0351e0550603506055060351205506035060550603525055010350105501035190550103501055010351b0550303503055030350f055030350305503035
0107000025055010350105501035190550103501055010352a05506035060550603512055060350605506035270550303503055030351b0550303503055030352c05508035080550803514055080350805508035
010700002e0550a0350a0550a0352e0550a0350a0550a0352e0550a0350a0550a035160550a0350a0550a03527055030350305503035270550303503055030352a05506035060550603512055060350605506035
010700002c0450802208042080222004408024080440802414042080220804208022080440802408044080242e0450a0220a0420a022220440a0240a0440a024160420a0220a0420a0220a0440a0240a0440a024
010700002a0450602206042060221e0440602406044060241204206022060420602206044060240604406024270450302203042030221b0440302403044030240f0420302203042030220f044030240304403024
01070000120420602206042060221e04406024060440602403042030220304203022270440302403044030241404208022080420802220044080240804408024270450302203042030221b044030240304403024
010700002e0450a0220a0420a022220440a0240a0440a024200420802208042080221404408024080440802412042060220604206022120440602406044060240f04203022030420302203044030240304403024
010700000f042030220304203022030440302403044030240f0420302203042030220304403024030440302420042080220804208022140440802408044080242e0450a0220a0420a022220440a0240a0440a024
010700002004208022080420802220044080240804408024220420a0220a0420a022160440a0240a0440a02420042080220804208022200440802408044080242a04506022060420602212044060240604406024
010700002c0450802208042080222c0440802408044080242e0450a0220a0420a0222e0440a0240a0440a024270450302203042030221b0440302403044030242a0450602206042060221e044060240604406024
01070000270450302203042030221b044030240304403024140420802208042080221404408024080440802412042060220604206022120440602406044060240f0420302203042030220f044030240304403024
01070000060421e0221e0421e022050441d0241d0441d024030421b0221b0421b022020441a0241a0441a0240104219022190421902200044180241804418024060421e0221e0421e022070441f0241f0441f024
010700000804220022200422002209044210242104421024090422102221042210220a04422024220442202408042200222004220022060441e0241e0441e024050421d0221d0421d022030441b0241b0441b024
01070000030421b0221b0421b022020441a0241a0441a0240104219022190421902201044190241904419024020421a0221a0421a022040441c0241c0441c024050421d0221d0421d022060441e0241e0441e024
01070000070421f0221f0421f02208044200242004420024080422002220042200220904421024210442102408042200222004220022070441f0241f0441f024050421d0221d0421d022030441b0241b0441b024
01070000030421b0221b0421b0221b0421b0221b0421b022020441a0241a0441a0241a0441a0241a0441a02401042190221904219022190421902219042190220004418024180441802418044180241804418024
010700000104219022190421902218042180221804218022030441b0241b0441b0241a0441a0241a0441a024060421e0221e0421e0221d0421d0221d0421d022080442002420044200241f0441f0241f0441f024
010700000a042220222204222022090442102421044210240a042220222204222022090442102421044210240a042220222204222022070441f0241f0441f024060421e0221e0421e022030441b0241b0441b024
01070000030421b0221b0421b022030441b0241b0441b024060421e0221e0421e022040441c0241c0441c024030421b0221b0421b022030441b0241b0441b024060421e0221e0421e022050441d0241d0441d024
01070000330450f025030420302203044030240304403024340451002504042040220404404024040440402436045120250604206022060440602406044060243704513025070420702207044070240704407024
010700003a045160250a0420a0220a0440a0240a0440a0240a0420a0220a0420a0220a0440a0240a0440a0240a0420a0220a0420a0220a0440a0240a0440a0240a0420a0220a0420a0223a0450a0243a0450a024
010700003904515025090420902209044090240904409024090420902209042090220904409024090440902409042090220904209022090440902409044090240904209022090420902239045090243904509024
010700003804514025080420802208044080240804408024080420802208042080220804408024080440802408042080220804208022080440802408044080240804208022080420802238045080243804508024
010700003604512025060420602206044060240604406024330450f025030420302203044030240304403024310450d025010420102201044010240104401024300450c025000420002200044000240004400024
01070000320450e025020420202202044020240204402024020420202202042020220204402024020440202402042020220204202022020440202402044020240204202022020420202232045020243204502024
01070000330450f025030420302203044030240304403024030420302203042030220304403024030440302403042030220304203022030440302403044030240304203022030420302233045030243304503024
010700003404510025040420402204044040240404404024040420402204042040220404404024040440402404042040220404204022040440402404044040240404204022040420402234045040243404504024
__music__
01 40430004
01 41420102
00 41420102
00 41420103
02 41420103
00 41424341
01 41424d1e
02 41424d1f
01 41424d16
00 41424d17
00 41424d14
00 41424d15
00 41424d15
00 41424d17
00 41424d17
02 41424d14
01 41424d1a
00 41424d1a
00 41424d1c
00 41424d1b
00 41424d1a
00 41424d1d
00 41424d1c
02 41424d1d
01 41426120
00 41426321
00 41426322
00 41426323
00 41426324
00 41426325
00 41426326
02 41426327
01 41426328
00 41426329
00 4142632a
00 4142632b
00 4142632c
00 4142632d
00 4142632e
02 4142632f
01 41426330
00 41426331
00 41426332
00 41426333
00 41426334
00 41426335
00 41426336
02 41426337
01 41426338
00 41426339
00 4142633a
00 4142633b
00 4142633c
00 4142633d
00 4142633e
02 4142633f
00 4142637f

