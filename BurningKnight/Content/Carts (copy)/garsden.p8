pico-8 cartridge // http://www.pico-8.com
version 19
__lua__
--gar's den
--by trasevol_dog


function _init()
 init_anim_info()
 drk=parse"0=0,0,1,1,2,1,5,13,2,4,9,3,1,1,2,4"
 t,shkx,shky,camx,camy,xmod,ymod=0,0,0,0,0,0,0
 
 if not started then
  music(4)
  darkout()
  darkin()
  repeat
   _draw()
   flip()
   t+=0.01
  until btnp(4)
  started=true
 end
 
 objs=parse[[
to_update={},

hittable={},
collect={},
needtile={},

doors={},
stairs={},
plants={},
enemies={}
]]
 
 for i=0,4 do
  objs["to_draw"..i]={}
 end
 
 music(-1)
 sfx(26)
 darkout()

 init_plants()init_enemies()init_props()init_items()init_inventory()

 curlvl,invcur,levels=1,1,{}
 create_player()
 
 player.x,player.y=generate_level()

 camx,camy,floatxts=player.x,player.y,{}
 
 local tilx,tily=tile(player.x,player.y)
 mset(tilx,tily,16)
 
 for i=-1,1 do
  create_prop(tilx+i*1.5,tily-1.5+abs(i),"bc"..i)
 end
 
 uncover_room(player.x,player.y)
 
 music(0)
 darkin()
 sfx(27)
end

function _update()
 t+=0.01

 camx=lerp(camx,player.x+12*player.vx,0.05)
 camy=lerp(camy,player.y+12*player.vy,0.05)
 
 if player.hp<=0 then
  player.vx,player.vy,player.state,player.animt=0,0,"slash",0
  if btnp(4) and t>0.2 then
   _init()
  end
  
  return
 end

 for txt in all(floatxts) do
  txt.y-=txt.t
  txt.t-=0.1
  if txt.t<0 then
   del(floatxts,txt)
  end
 end
 
 update_shake()
 update_objects()
 
-- if btnp(4,1) then
--  for p in group("plants") do
--   p.t=p.td-1
--  end
-- end
end

function _draw()
 oxmod,oymod,xmod,ymod,ar=xmod,ymod,flr(camx-64+shkx),flr(camy-64+shky),parse"{-1,0},{1,0},{0,-1},{0,1}"

 camera()
 
 vxmod,vymod,texts=xmod-oxmod,ymod-oymod,{}
 for i=0,149 do
  local x,y=rnd128(),rnd128()
  local c=chance(5) and 0 or band(pget(x+3*vxmod,y+3*vymod)+rnd(1.001),1)
  local dif=pick(ar,4)
  circ(x+dif[1],y+dif[2],1,c)
 end
 
 if not started then
  draw_text("trasevol_dog presents",64,4,1,true,1,0)
  spr(202,40,16,6,4)
  circfill(32,56,6,0)
  draw_anim(32,56,"gar","dance",t)
  circfill(96,56,6,0)
  draw_anim(96,56,"player","idle",t,true)
  
  draw_text("🅾️ / z ~ action + slash ",60,72,1,true,13,1)
  draw_text("❎ / x ~ inventory control ",66,82,1,true,13,1)
  draw_text("⬅️➡️⬆️⬇️ ~ move around    ",50,92,1,true,13,1)
  
  draw_text("press 🅾️ / z to start ",64,120)
  
  return
 end
 
 if player.hp<=0 then
  draw_text("you died",64,32)
  local str
  if win then
   str="as a winner"
  else
   str="you didn't defeat gar"
  end
  draw_text(str,64,48)
  draw_text("press 🅾️ / z to try again ",64,96)
  
  camera(xmod,ymod)
  
  player:draw()
  return
 end
 
 palt(0,false)
 draw_room()
 palt(0,true)
 all_colors_to()

 camera(xmod,ymod)
 
 if target then
  spr(226,target[1]*8-1,target[2]*8-1,2,2)
 end

 draw_objects()
 all_colors_to()
 
 palt(0,false)
 
 local amx,amy=tile(xmod,ymod)
 for x=amx,amx+16 do
 for y=amy,amy+16 do
  if peek(0x4300+(y-amy)*17+x-amx)==2 then
   local m=mget(x,y)
   local s=48+(m+x+y)%4
   spr(s,x*8,y*8-4)
  end
 end
 end
 
 palt(0,true)

 for data in all(texts) do
  draw_text(data[1],data[2],data[3],1,true,data[4],data[5])
 end
 
 for txt in all(floatxts) do
  local c1,c2=txt.c1,txt.c2
  if txt.t>1.8 then c1,c2=drk[c1],drk[c2] end
  draw_text(txt.str,txt.x,txt.y,1,true,c1,c2)
 end

 camera()
 draw_inventory()
 
 if win and t<4 then
  draw_text("you win the game!",64,32)
  draw_text("this is your den now!",64,48)
  draw_text("!!! thank you for playing !!!",64,96)
 end
 
 --draw_text(""..stat(1),1,4,0,true)
 --draw_text(""..stat(0),1,12,0,true)
end



function update_player(p)
 update_animt(p)
 
 p.inv=max(p.inv-.01,0)
 
 local movx,movy=0,0
 if btn(5) then
  if btnp(0) then invcur=(invcur-2)%invplaces+1 end
  if btnp(1) then invcur=(invcur)%invplaces+1 end
 else
  if btn(0) then movx-=p.acc p.faceleft=true end
  if btn(1) then movx+=p.acc p.faceleft=false end
  if btn(2) then movy-=p.acc end
  if btn(3) then movy+=p.acc end
 end
 
 target=nil
 local data=inventory[invcur]
 if data[2]>=1 then
  if data[1].target=="tile" then
   local f=data[1].tilef
   
   local x,y
   local check=function() return tile_flag(x,y,f) and (not data[1].tilecheck or data[1].tilecheck(x,y)) end
   
   local list,xmul=parse"{6,0},{-2,0},{0,-6},{0,6}",p.faceleft and -1 or 1
   
   for ofs in all(list) do
    x,y=tile(p.x+xmul*ofs[1],p.y+ofs[2])
    
    if check() then
     target={x,y}
     break
    end
   end
   
  end
 end
 
 if btnp(4) and not player.pressin then
  if data[2]>=1 and (target or not data[1].ntarg) then
   local used=data[1].use and data[1].use(target,data[1].hp)

   if used and data[1].consumable then
    data[2]-=1
   end
  end
  
  sfx(24)
  
  p.state="slash"
  p.animt=0
 end
 
 player.pressin=btn(4)
 
 
 update_movement(p,movx,movy,true)
 
 if p.state=="slash" then
  local step,first=anim_step(p)
  if step==1 and first then
   local hit={x=p.x,y=p.y,h=20,w=14}
   if p.faceleft then hit.x-=6 else hit.x+=6 end
   local list=all_collide_objgroup(hit,"hittable")
   
   for o in all(list) do
    o:hit()
   end
   
   p.vx*=0
   p.vy*=0
  end
 end
 
 
 local tx,ty=tile(p.x,p.y)
 local m=mget(tx,ty)
 if fget(m,7) and not p.staircol then
  sfx(26)
  darkout()
 
  cover_room(p.x,p.y)
  
  local mapdata={}
  for x=0,127 do
  for y=0,31 do
   mapdata[y*128+x]=mget(x,y)
  end
  end
  
  levels[curlvl]={
   map=mapdata
  }
  
  for k,grp in pairs(objs) do
   for o in all(grp) do
    if not o.floor then
     o.floor=curlvl
    end
   end
  end
 
  if m==52 then curlvl+=1
  elseif m==53 then curlvl-=1
  end
  
  local data=levels[curlvl]
  if data then
   mapdata=data.map
   for x=0,127 do
   for y=0,31 do
    local mm=mapdata[y*128+x]
    mset(x,y,mm)
    if (m==52 and mm==53) or (m==53 and mm==52) then
     p.x,p.y=untile(x,y)
    end
   end
   end
  else
   p.x,p.y=generate_level()
  end
  
  if curlvl==4 then
   music(4)
  else
   music(-1)
  end
  
  camx,camy,p.vx,p.vy=p.x,p.y,0,0
  
  p.floor=curlvl
  uncover_room(p.x,p.y)
  sfx(27)
  
  darkin()
 end
 p.staircol=fget(m,7)
 
 
 local newstate
 if p.state=="slash" and anim_step(p)<3 then
  newstate="slash"
 elseif abs(p.vx)>0.1 or abs(p.vy)>0.1 then
  newstate="run"
  local step,one=anim_step(p)
  if step==3 and one then sfx(16) end
 else
  newstate="idle"
 end
 
 if newstate~=p.state then
  p.state,p.animt=newstate,0
 end
end

function update_plant(p)
 local tilx,tily=tile(p.x,p.y)
 local til,tt=mget(tilx,tily),0.01

 if til>=2 and til<=4 and rnd(4)<3 then
  tt+=0.04
 end
 
 p.t+=tt
 if p.t%(p.td/p.spk)<tt then
  if on_floor(p) and chance(70) then
   mset(tilx,tily,1)
  end
  
  if p.t>p.td then
   group_add("hittable",p)
  end
 end
 
 p.t=min(p.t,p.td+0.1)
end

function update_enemy(en)
 if en.hidden then return end

 update_animt(en)
 en.faceleft=en.x>player.x
 
 if en.updatetyp=="fireball" then
  en.x+=en.vx
  en.y+=en.vy
  if map_col(en) then
   en:hit()
  end
  en.faceleft=en.vx<0
 else
  
  if en.name=="gar" then
   local step,one=anim_step(en)
   
   if en.state=="teleport" then
    if step==4 and one then
     particles(en.x,en.y,6,en.colors)
     repeat
      en.x,en.y=untile(flrnd(32),flrnd(32))
     until dist(en.x,en.y,player.x,player.y)>64 and tile_flag(en.x/8,en.y/8,6)
     
     en.state="idle"
    end
   else
    if step>=14 and step<27 and en.animt%0.1<0.01 then
    
     for i=0,5 do
      local a=i/6+en.animt*.2
      local xx,yy=0.5*cos(a),0.5*sin(a)
      create_enemy(en.x+16*xx,en.y+16*yy,"fireball",xx,yy)
     end
     sfx(18)
     
    elseif step==34 and one and dist(en.x,en.y,player.x,player.y)<48 then
     en.state="teleport"
    end
   end

  end
 
  local mvx,mvy=0,0
  
  if en.knock==0 then
   local upd
   if en.updatetyp=="follow" then
    upd=update_bat
   elseif en.updatetyp=="slime" then
    upd=update_slime
   end
   
   mvx,mvy=upd(en)
   
   if en.name=="mage" then
    local step,one=anim_step(en)
    if step==7 and one then
     create_enemy(en.x+16*mvx,en.y+16*mvy,"fireball",mvx*2,mvy*2)
     sfx(18)
    end
    
    mvx,mvy=-mvx,-mvy
   end
  else
   en.knock=max(en.knock-1,0)
  end
  
  update_movement(en,mvx,mvy,true,true)
 end

 local cole,colp=collide_objgroup(en,"enemies"),collide_objobj(en,player)
 if cole then
  local a=atan2(en.x-cole.x,en.y-cole.y)
  cole.vx-=en.rep*cos(a)
  cole.vy-=en.rep*sin(a)
 end
 
 if colp and en.dmg and player.inv==0 then
  player.hp-=en.dmg
  player.inv=0.2
  
  local a=atan2(player.x-en.x,player.y-en.y)
  
  player.vx+=6*cos(a)
  player.vy+=6*sin(a)
  
  sfx(31)
  
  if player.hp<=0 then
   t=0
   sfx(35)
  end
 end
end

function update_bat(en)
 local a=atan2(player.x-en.x,player.y-en.y)
 return en.acc*cos(a),en.acc*sin(a)
end

function update_slime(en)
 if en.z<-0.5 then
  en.animt-=0.01
 else
  en.vx*=0.8
  en.vy*=0.8
 end
 
 en.z+=en.vz
 en.vz+=0.5
 
 if en.z>0 then
  en.vz,en.z=0,0
 end
 
 local step,one=anim_step(en)
 if step==en.jstep and one then
  en.vz=en.jump
  sfx(34)
  return update_bat(en)
 end
 
 return 0,0
end

function update_collect(s)
 s.t+=0.01

 local mvx,mvy=0,0
 if s.t>0.15 then
  local a=atan2(player.x-s.x,player.y-s.y)
  mvx,mvy=0.4*cos(a),0.4*sin(a)
 end
 
 update_movement(s,mvx,mvy,true,true)
 
 s.z+=s.vz
 s.vz-=0.4
 
 if s.z<0 then
  s.z=0
  s.vz*=-0.5
  
  if s.vz<1.2 then
   s.vz=0
  end
 end
 
 if s.t>0.03 and collide_objobj(s,player) then
  if add_item(s.item) then
   create_text(player.x,player.y-6,"+"..items[s.item].name,12,1)
   deregister_object(s)
   sfx(19)
  end
 end
end

function update_door(d)
 d.t=abs(d.t+0.01)

 if d.hidden or not on_floor(d) then return end

 local prev=d.colplayer
 d.colplayer=collide_objobj(d,player)
 
 if prev and not d.colplayer then
  darken_room(player.x,player.y,0)
 end
end

function update_particle(s)
 s.t+=0.01
 update_movement(s,0,0)
 
 s.z+=s.vz
 s.vz-=0.4
 
 if s.z<0 then
  s.z=0
  s.vz*=-0.5
  
  if s.vz<1.2 then
   s.vz=0
  end
 end
 
 local tx,ty=tile(s.x,s.y)
 if s.t>0.8 or tile_flag(tx,ty,0) then
  deregister_object(s)
 end
end

function update_animt(o)
 o.animt+=0.01
end

function add_money(v)
 player.money=min(player.money+v,9999)
 add_shake(1)
 particles(player.x,player.y,3,parse"7,10,9")
end

function add_item(name,num)
 local num=num or 1

 local done=false
 for i=1,invplaces do
  local data=inventory[i]
  if data[1]==items[name] then
   data[2]+=num
   return true
  end
 end

 for i=1,invplaces do
  local data=inventory[i]
  if data[2]==0 then
   data[1],data[2]=items[name],num
   return true
  end
 end
 
 return false
end

function sell_item(s)
 local data=inventory[invcur]
 if data[2]>0 then
  data[2]-=1
  local p=data[1].price
  add_money(p)
  
  create_text(player.x,player.y-6,"+"..p.."$",11,3)
  
  sfx(29)
  s.stand.item=data[1].ref
 end
end

function use_hoe(targ)
 local x,y=targ[1],targ[2]
 mset(x,y,1)
 
 x,y=untile(x,y)
 particles(x,y,4+flrnd(2),parse"4,4,9,2")
 sfx(20)
end

function water_tile(targ)
 local x,y=targ[1],targ[2]
 mset(x,y,2+flrnd(3))
 
 x,y=untile(x,y)
 particles(x,y,2,parse"12,12,12,7")
 sfx(21)
end

function plant_seed(targ)
 local data,x,y=inventory[invcur],untile(targ[1],targ[2])
 create_plant(x,y,data[1].plant)
 particles(x,y,3,parse"11,3,4,9,2")
 sfx(22)
 return true
end

function eat(target,hp)
 local str
 
 if not hp then
  player.hpmax+=1
  player.hp+=1
  create_text(player.x,player.y-6,"+1 hp max",14,2)
  sfx(30)
  return true
 end

 local mhp=function() return player.hp>=player.hpmax end

 if mhp() then return false end
 
 player.hp+=hp
 
 str="+"..hp.." hp"
 if mhp() then
  player.hp,str=player.hpmax,"hp max"
 end
 create_text(player.x,player.y-6,str,11,3)

 sfx(30)
 return true
end

function hit_door(d)
 while collide_objobj(d,player) do
  if d.horiz then
   player.y+=sgn(player.vy)
  else
   player.x+=sgn(player.vx)
  end
 end

 sfx(25)
 add_shake(1)
 
 door_change(d,player.x,player.y)
end

function door_change(d,fx,fy)
 d.open=not d.open
 mset(d.tilx,d.tily,d.closesprite-30+(d.open and 2 or 0))

 local x,y
 if d.horiz then
  x,y=d.x,d.y+sgn(d.y-fy)*8
 else
  x,y=d.x+sgn(d.x-fx)*8,d.y
 end
 
 if d.open then
  if d.t>50 then
   local x1,y1,x2,y2=get_room(x,y)
   if x2-x1>32 and y2-y1>32 and mget(x/8,y/8)~=28 then
    local k=curlvl+flrnd(2)
    for i=1,k do
     create_enemy(lerp(x1,x2,.25+rnd(.5)),lerp(y1,y2,.25+rnd(.5)),pick(spawntable[curlvl]))
    end
    sfx(17)
   end
  end
 
  uncover_room(x,y)
 else
  cover_room(x,y)
  d.t=0
  
  d.hidden=false
 end
end

function stand_buy(s)
 if s.item then
  local data=items[s.item]
  if player.money>=data.price then
   create_collect(s.x,s.y,s.item)
   player.money-=data.price
   
   create_text(player.x,player.y-6,"-"..data.price.."$",14,2)
   
   if not s.infi then
    s.item=nil
   end
   
   sfx(28)
   particles(player.x,player.y,3,parse"7,10,9")
  end
 end
end

function uncover_room(x,y)
 local x1,y1,x2,y2=get_room(x,y)
 x,y=(x1+x2)*0.5,(y1+y2)*0.5
 
 for o in group("to_draw2") do
  if in_box(o.x,o.y,x1,y1,x2,y2) and on_floor(o) then
   local prev=o.hidden
   o.hidden=false
   
   if o.is_door and o.open and prev then
    if o.horiz then
     uncover_room(o.x,o.y+sgn(o.y-y)*8)
    else
     uncover_room(o.x+sgn(o.x-x)*8,o.y)
    end
   end
  end
 end
end

function cover_room(x,y)
 local x1,y1,x2,y2=get_room(x,y)
 x,y=(x1+x2)*.5,(y1+y2)*.5
 
 for o in group("to_draw2") do
  if in_box(o.x,o.y,x1,y1,x2,y2) and on_floor(o) then
   local prev=o.hidden
   o.hidden=true
   
   if o.is_door and o.open and not prev then
    if o.horiz then
     cover_room(o.x,o.y+sgn(o.y-y)*8)
    else
     cover_room(o.x+sgn(o.x-x)*8,o.y)
    end
   end
  end
 end
end

function darken_room(x,y,k)
 local x1,y1,x2,y2=get_room(x,y)
 x,y=(x1+x2)*0.5,(y1+y2)*0.5
 
 for o in group("doors") do
  if in_box(o.x,o.y,x1,y1,x2,y2) and on_floor(o) then
   if o.is_door and o.open and not o.done then
   
    if k>=2 then
     door_change(o,x,y)
    else
     o.done=true
     if o.horiz then
      darken_room(o.x,o.y+sgn(o.y-y)*8,k+1)
     else
      darken_room(o.x+sgn(o.x-x)*8,o.y,k+1)
     end
     o.done=nil
    end
    
   end
  end
 end
end

function is_free(x,y)
 for o in group("needtile") do
  local tilx,tily=tile(o.x,o.y)
  if tilx==x and tily==y then
   return false
  end
 end
 return true
end

function end_game()
 sfx(36)
 win,t=true,0
end

function tile_flag(x,y,f) return fget(mget(x,y),f) end

function collect_fruit(s)
 create_collect(s.x,s.y,s.fruit)
 group_rem("hittable",s)
 particles(s.x,s.y,4+flrnd(2),parse"1,11")
 sfx(23)
 
 if s.again then
  s.t=s.td/2
 else
  deregister_object(s)
 end
end

function damage(en,noeffect)
 local a=atan2(player.x-en.x,player.y-en.y)
 en.vx-=4*cos(a)
 en.vy-=4*sin(a)
 en.knock,en.whiteframe=8,true
  
 local dmg,data=1,inventory[invcur]
 if data[2]>=1 then
  dmg=data[1].damage or 1
 end
 
 dmg=round((0.75+rnd(0.5))*dmg)
 
 if not en.noeffect then
  create_text(en.x,en.y-6,-dmg,14,2)
  sfx(33)
 end
 
 en.hp-=dmg
 if en.hp<0 then
  particles(en.x,en.y,6,en.colors)

  if en.name=="gar" then
   music(-1)
  end
  
  if en.noeffect then
   add_shake(1.5)
   sfx(37)
  else
   add_shake(4)
   sfx(32)
  end
  
  if en.loot then
   create_collect(en.x,en.y,en.loot)
  end
  
  deregister_object(en)
 else
  add_shake(2)
 end
end

function destroy_prop(p)
 deregister_object(p)
 
 local data=props[p.name]
 if chance(data.lootchance) then
  create_collect(p.x,p.y,pick(data.loot))
 end
 
 particles(p.x,p.y,3+flrnd(2),data.cols)
 sfx(32)
 add_shake(1)
end

function add_shake(p)
 local a=rnd(1)
 shkx+=p*cos(a)
 shky+=p*sin(a)
end

function update_shake()
 if abs(shkx)+abs(shky)<0.5 then
  shkx,shky=0,0
 else
  shkx*=-0.5-rnd(0.2)
  shky*=-0.5-rnd(0.2)
 end
end

function on_floor(o)
 return not o.floor or o.floor==curlvl
end



function draw_player(p)
 if p.inv%0.04>0.02 then return end
 
 draw_self(p)
 
 if p.state=="slash" then
  local n,s,x,y,flp=anim_step(p),72,p.x-3,p.y-8,false
  
  if n==0 then s=-2 end
  if n==2 then s+=2 end
  if p.faceleft then
   x-=10
   flp=true
  end
  
  spr(s,x,y,2,2,flp,false)
  
  local data=inventory[invcur]
  if data[2]>=1 then
   local sp,ox,oy=255,3,6
   if n==0 then
    sp,ox,oy=data[1].sprite,6,-6
   else
    local s=data[1].sprite
    local sx,sy=s%16*8,flr(s/16)*8
    
    for y=0,7 do
    for x=0,7 do
     sset(127-x,120+y,sget(sx+y,sy+x))
    end
    end
   end
   
   if p.faceleft then ox=-ox end
   local foo=function()
    spr(sp,p.x+ox-4,p.y+oy-4,1,1,p.faceleft)
   end
   draw_outline(foo)
   foo()
   
  end
 end
end

function draw_self(s)
 local foo=function(s)
            local state=s.state or "only"
			local z=s.z or 0
            draw_anim(s.x,s.y-1+z,s.name,state,s.animt,s.faceleft)
           end
 local c
 if s.whiteframe then c=7 end
 draw_outline(foo,c,s)
 if s.whiteframe then all_colors_to(7) end
 foo(s)
 if s.whiteframe then all_colors_to() s.whiteframe=false end
end

function draw_fireball(s)
 pal(11,0)
 draw_anim(s.x,s.y,"fireball","only",s.animt,s.faceleft)
 pal(11,11)
end

function draw_plant(p)
 local s=p.sp+(p.t*p.spk)/p.td
 local foo=function()
  spr(s,p.x-4,p.y-8)
 end
 
 local c
 if p.highlight then
  c,p.highlight=7,false
 else
  c=0
 end
 draw_outline(foo,c)
 
 if p.palmap then
  apply_palmap(p.palmap)
 end
 foo()
end

function draw_shopkeeper(s)
 draw_self(s)
 local hit={x=s.x,y=s.y,w=14,h=14}
 if collide_objobj(hit,player) then
  local data=inventory[invcur]
  if data[2]>=1 then
   add(texts,{"sell:"..data[1].price.."$",s.x,s.y-10,11,3})
  end
 end
end

function draw_stand(s)
 local foo=function()
  spr(44,s.x-4,s.y-6)
 end
 draw_outline(foo)
 foo()
 
 if s.item then
  local foo=function()
   spr(items[s.item].sprite,s.x-4,s.y-11+2*cos(s.animt))
  end
  draw_outline(foo)
  foo()

  local hit={x=s.x,y=s.y,w=14,h=14}
  if collide_objobj(hit,player) then
   local data=items[s.item]
   add(texts,{data.name..": "..data.price.."$",s.x,s.y+2,10,4})
  end
 end
end

function draw_prop(p)
 local foo=function()
  spr(p.s,p.x-4,p.y-6)
 end
 
 draw_outline(foo)
 foo()
end

function draw_door(d)
 local xx,yy=0,0
 if d.open then xx,yy=4,3 end
 local foo=function()
  spr(d.open and d.opensprite or d.closesprite,d.x+xx-4,d.y+yy-8)
 end
 draw_outline(foo)
 foo()
end

function draw_collect(s)
 local foo=function() spr(s.s,s.x-4,s.y-s.z-4) end
 
 if s.t<0.03 then
  draw_outline(foo,7)
  all_colors_to(7)
  foo()
  all_colors_to()
 else
  draw_outline(foo)
  foo()
 end
end

function draw_particle(s)
 local y=s.y-s.z
 circ(s.x,y,1,drk[s.c])
 pset(s.x,y,s.c)
end

function draw_room()
 local dx,dy,amx,amy=flr(xmod%8),flr(ymod%8),tile(xmod,ymod)
 
 memset(0x4300,0,289) --17*17
 
 local px,py=tile(player.x,player.y)
 local uncover
 uncover=function(x,y,k,xm,ym)
  local sx,sy=x-amx,y-amy

  local a=0x4300+sy*17+sx
  if peek(a)>0 or k>2 or not in_box(sx,sy,0,0,17,17) then
   return
  end
  
  local m=mget(x,y)
  spr(m,sx*8-dx,sy*8-dy)
  
  poke(a,fget(m,0) and 2 or 1)
  
  if fget(m,5) and (xm~=0 or ym~=0) then
   if xm~=0 and ym~=0 then
    poke(a,0)
   else
    k+=1
    darken(k>1)
    uncover(x+xm,y+ym,k,xm,ym)
    k-=1
    if k>0 then darken(k>1) else all_colors_to() end
   end
  elseif not fget(m,4) then
   for xx=-1,1 do
   for yy=-1,1 do
    uncover(x+xx,y+yy,k,xx,yy)
   end
   end
  end
 end
 
 uncover(px,py,0,0,0)
end

function draw_inventory()
 color()
 rectfill(41,102,87,111)
 circfill(41,111,9)
 circfill(87,111,9)
 rectfill(0,110,127,127)


 local x=7

 for i=1,invplaces do
  local s
  if i==invcur then
   x+=1
   s=224
  else
   s=192
  end
  spr(s,x-8,112,2,2)
 
  local data=inventory[i]
  
  if data[2]~=0 then
   spr(data[1].sprite,x-4,116)
   
   if data[2]>1 then
    draw_text(""..data[2],x+4,124)
   end
  end
  
  if i==invcur then
   x+=1
  end
  
  x+=14
 end
 
 local str
 local data=inventory[invcur]
 if data[2]==0 then
  str="nothing"
 else
  str=data[1].name
 end
 draw_text(str,64,107)
 
 pal(11,0)

 draw_text("floor "..curlvl.."/4",127,5,2,true)
 
 spr(194,93,8,2,2)
 draw_text(player.money,110,17,0,true)
 
 for i=0,player.hpmax-1 do
  local s
  if i+1<=player.hp then
   s=200
  elseif i+.5<=player.hp then
   s=46
  else
   s=232
  end
  spr(s,i*13,-1,2,2)
 end
 
 pal(11,11)
end

function draw_text(str,x,y,al,extra,c1,c2)
 str=""..str
 local al=al or 1
 local c1=c1 or 7
 local c2=c2 or 13

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
 
 print(str,x+1,y+1,c2)
 print(str,x-1,y+1,c2)
 print(str,x,y+2,c2)
 print(str,x+1,y,c1)
 print(str,x-1,y,c1)
 print(str,x,y+1,c1)
 print(str,x,y-1,c1)
 print(str,x,y,0)

end

function draw_outline(draw,c,arg)
 local c=c or 0

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



function create_player()
 player=merge_arrays(parse[[
vx=0,
vy=0,
acc=0.7,
spdcap=1.4,
dec=0.7,
w=4,
h=4,
name=player,
state=idle,
faceleft=true,
staircol=true,
animt=0,
money=0,
hp=3,
hpmax=3,
inv=0.1,
regs={to_update,to_draw2}
]],{
  draw=draw_player,
  update=update_player,
 })
 
 register_object(player)
end

function create_plant(x,y,name)
 local p=merge_arrays(parse[[
w=6,
h=8,
t=0,regs={to_update,to_draw2,plants,needtile}
]],{
  x=x+rnd(2)-1,
  y=y+rnd(2),
  update=update_plant,
  draw=draw_plant,
  hit=collect_fruit
 })
 
 merge_arrays(p,plants[name])
 
 register_object(p)
end

function create_shopkeeper(x,y,stand)
 local s=merge_arrays(parse[[
w=6,
h=6,
name=shopkeeper,
animt=0,
hidden=true,
regs={to_update,to_draw2,hittable}
]],{
  x=x,
  y=y,
  update=update_animt,
  draw=draw_shopkeeper,
  hit=sell_item,
  stand=stand,
 })
 
 register_object(s)
end

function create_stand(x,y,item,infi)
 local s=merge_arrays(parse[[
w=4,
h=3,
animt=%1,
hidden=true,
regs={to_update,to_draw2,hittable,doors}
]],{
  x=x,
  y=y,
  item=item,
  infi=infi,
  update=update_animt,
  draw=draw_stand,
  hit=stand_buy
 })
 
 register_object(s)
 
 return s
end

function create_enemy(x,y,name,vx,vy)
 local en=merge_arrays(parse[[
animt=%0.2,
knock=0,
regs={to_update,to_draw2,hittable,enemies}
]],{
  x=x,
  y=y,
  vx=vx or 0,
  vy=vy or 0,
  update=update_enemy,
  draw=name=="fireball" and draw_fireball or draw_self,
  hit=damage
 })
 
 merge_arrays(en,enemies[name])
 
 register_object(en)
end

function create_prop(x,y,name)
 local x,y=untile(x,y)
 local p=merge_arrays(parse[[
hidden=true,
w=6,
h=6,
regs={to_draw2,hittable,needtile}
]],{
  x=x,
  y=y,
  s=props[name].sprite,
  name=name,
  draw=draw_prop,
  hit=destroy_prop
 })
 
 register_object(p)
end

function create_door(tilx,tily,horiz)
 local d=merge_arrays(parse[[
is_door=true,
hidden=true,
open=false,
colplayer=false,
t=999,
horiz=false,
w=4,
h=8,
opensprite=54,
closesprite=55,
regs={to_update,to_draw2,hittable,needtile,doors}
]],{
  tilx=tilx,
  tily=tily,
  update=update_door,
  draw=draw_door,
  hit=hit_door
 })
 
 if horiz then
  merge_arrays(d,parse[[
horiz=true,
w=8,
h=4,
opensprite=55,
closesprite=54
]])
 end
 
 d.x,d.y=untile(tilx,tily)
 
 register_object(d)
end

function create_collect(x,y,item)
 local a,spd=rnd(1),2+rnd(2)
 
 local s=merge_arrays(parse[[
w=6,
h=6,
spdcap=3,
dec=0.8,
z=2,
vz=%3,
t=0,
regs={to_update,to_draw2,collect}
]],{
  x=x,
  y=y,
  vx=spd*cos(a),
  vy=spd*sin(a),
  s=items[item].sprite,
  item=item,
  update=update_collect,
  draw=draw_collect,
 })
 
 register_object(s)
 
 return s
end

function create_text(x,y,str,c1,c2)
 local txt={
  t=2,
  x=x,
  y=y,
  str=str,
  c1=c1,
  c2=c2
 }
 
 add(floatxts,txt)
end

function particles(x,y,n,clrs)
 for i=1,n do
  create_particle(x,y,pick(clrs))
 end
end

function create_particle(x,y,c)
 local a,spd=rnd(1),2+rnd(5)
 
 local s=merge_arrays(parse[[
w=2,
h=2,
spdcap=3,
dec=0.8,
z=%2,
vz=%2.5,
t=%0.4,
regs={to_update,to_draw1}
]],{
  x=x,
  y=y,
  c=c,
  vx=spd*cos(a),
  vy=spd*sin(a),
  update=update_particle,
  draw=draw_particle
 })
 
 register_object(s)
end



function init_plants()
 plants=parse[[berrytree={fruit=berries,td=180,sp=128,spk=7,again=true},bigberrytree={fruit=bigberry,td=180,sp=128,spk=7,palmap={12=8,1=2},again=true},berryberrytree={fruit=berryberry,td=180,sp=128,spk=7,palmap={12=13,1=2},again=true},carrot={fruit=carrot,td=120,sp=144,spk=3,palmap={2=4,14=9,15=10}},beet={fruit=beet,td=120,sp=144,spk=3},potato={fruit=potato,td=120,sp=144,spk=3,palmap={2=4,14=2,15=9,7=10}},heartree={fruit=heart,td=100,sp=160,spk=6},swordtree={fruit=sword,td=120,sp=176,spk=7,palmap={6=4,13=2}},betterswordtree={fruit=bettersword,td=120,sp=176,spk=7,palmap={11=14,3=2}},bestswordtree={fruit=bestsword,td=120,sp=176,spk=7,palmap={11=10,3=9}}]]
end

function init_enemies()
 enemies=parse[[bat={w=3,h=3,acc=0.1,dec=0.97,spdcap=1.3,hp=3,dmg=0.5,rep=0.2,loot=wildberry,name=bat,updatetyp=follow,colors={13,13,13,13,1}},skeleton={w=6,h=6,acc=0.3,dec=0.8,spdcap=0.6,hp=6,dmg=1,rep=2,loot=bone,name=skeleton,updatetyp=follow,colors={6,7,7,9}},golem={w=6,h=6,acc=0.3,dec=0.5,spdcap=1.5,hp=25,dmg=2,rep=3,loot=marbles,name=golem,updatetyp=follow,colors={13,6,13,6,1}},slime={w=4,h=4,acc=3,dec=0.99,spdcap=3,hp=4,dmg=0.5,rep=1,loot=slime,name=slime,updatetyp=slime,jump=-2,jstep=16,z=0,vz=0,colors={11,11,11,3}},bigslime={w=6,h=6,acc=2,dec=0.99,spdcap=2,hp=9,dmg=1,rep=1,loot=slime,name=bigslime,updatetyp=slime,jump=-1,jstep=7,z=0,vz=0,colors={11,11,11,3}},mage={w=6,h=6,acc=0.4,dec=0.8,spdcap=0.6,hp=8,rep=1,loot=powder,name=mage,updatetyp=follow,colors={8,10,2}},fireball={w=2,h=2,hp=0,dmg=1,rep=1,noeffect=true,name=fireball,updatetyp=fireball,colors={8,9,10}},gar={w=6,h=6,acc=0,dec=0.9,spdcap=2,hp=320,rep=1,loot=winningring,name=gar,state=idle,updatetyp=follow,colors={8,8,10,10,14}}]]

 spawntable=parse[[
{bat,slime,slime},
{bat,skeleton,bigslime,bigslime},
{bat,skeleton,golem,golem,mage,mage}
]]
end

function init_props()
 props=parse[[bush={sprite=40,lootchance=20,loot={berryseed,wildberry},cols={3,11}},bush2={sprite=56,lootchance=20,loot={berryseed,wildberry},cols={3,11}},crate={sprite=41,lootchance=40,loot={berries},cols={4,9}},vase1={sprite=57,lootchance=30,loot={berries,slime,berryseed,carrotseed},cols={6,7,11}},vase2={sprite=58,lootchance=30,loot={berries,slime,berryseed,carrotseed},cols={6,7,14}},vase3={sprite=59,lootchance=30,loot={berries,slime,berryseed,carrotseed},cols={6,7,9}},vase4={sprite=60,lootchance=30,loot={berries,slime,berryseed,carrotseed},cols={6,7,12}},bc-1={sprite=41,lootchance=200,loot={hoe},cols={4,9}},bc0={sprite=41,lootchance=200,loot={watcan},cols={4,9}},bc1={sprite=41,lootchance=200,loot={woodsword},cols={4,9}}]]
end

function init_items()
 items=parse[[hoe={name=hoe,sprite=136,target=tile,tilef=1,ntarg=true,use=hoe,price=50},watcan={name=watering can,sprite=137,target=tile,tilef=2,ntarg=true,use=watcan,price=50},woodsword={name=wooden sword,sprite=138,price=50,damage=2},sword={name=sword,sprite=158,price=100,damage=4},bettersword={name=better sword,sprite=159,price=150,damage=8},bestsword={name=best sword,sprite=175,price=250,damage=16},slime={name=slime,sprite=140,price=3},powder={name=magic dust,sprite=141,price=10},marbles={name=marbles,sprite=142,price=12},bone={name=bone,sprite=143,price=5},berryseed={name=berry seeds,sprite=171,plant=berrytree,use=seed,price=5},bigberryseed={name=bigberry seeds,sprite=172,plant=bigberrytree,use=seed,price=15},berryberryseed={name=berryberry seeds,sprite=173,plant=berryberrytree,use=seed,price=50},carrotseed={name=carrot seeds,sprite=168,plant=carrot,use=seed,price=4},beetseed={name=beet seeds,sprite=169,plant=beet,use=seed,price=13},potatoseed={name=potato seeds,sprite=170,plant=potato,use=seed,price=45},swordseed={name=sword seed,sprite=185,plant=swordtree,use=seed,price=150},betterswordseed={name=better sword seed,sprite=186,plant=betterswordtree,use=seed,price=500},bestswordseed={name=best sword seed,sprite=187,plant=bestswordtree,use=seed,price=2000},heartseed={name=life seeds,sprite=184,plant=heartree,use=seed,price=200},wildberry={name=wildberries,sprite=189,hp=0.5,consumable=true,use=fruit,price=2},berries={name=berries,sprite=152,hp=1,consumable=true,use=fruit,price=12},bigberry={name=bigberry,sprite=153,hp=2,consumable=true,use=fruit,price=30},berryberry={name=berryberry,sprite=154,hp=3,consumable=true,use=fruit,price=100},carrot={name=carrot,sprite=155,hp=1,consumable=true,use=fruit,price=14},beet={name=beet,sprite=156,hp=2,consumable=true,use=fruit,price=43},potato={name=potato,sprite=157,hp=3,consumable=true,use=fruit,price=125},heart={name=life fruit,sprite=174,consumable=true,use=fruit,price=500},winningring={name=ring of the winner,sprite=139,consumable=false,use=endgame,price=9999}]]

 local uses={
  hoe=use_hoe,
  watcan=water_tile,
  seed=plant_seed,
  fruit=eat,
  endgame=end_game
 }
 
 for k,it in pairs(items) do
  if it.use=="seed" then
   it.tilecheck=is_free
   
   merge_arrays(it,parse[[
target=tile,
consumable=true,
ntarg=true,
tilef=3
]])
  end
 
  it.use=uses[it.use]
  it.ref=k
 end
end

function init_inventory()
 invplaces,inventory=9,{}
 for i=1,invplaces do
  inventory[i]=parse[[,0]]
 end
end

function generate_level()
 for o in group"hittable" do
  if not o.floor then
   deregister_object(o)
  end
 end

 local rectfilll=function(x1,y1,x2,y2,c)
  for xx=x1,x2 do
  for yy=y1,y2 do
   mset(xx,yy,c)
  end
  end
 end
 
 if curlvl==4 then
  rectfilll(0,0,127,31,36)
 
  for x=9,22 do
   mset(x,8,32+flrnd(4))
   for y=9,22 do
    mset(x,y,16+flrnd(8))
   end
  end
  
  mset(12,12,32)
  mset(12,19,33)
  mset(19,12,34)
  mset(19,19,35)
  create_enemy(128,128,"gar")
  mset(16,11,53)
  return untile(16,11)
 end

 rectfilll(0,0,127,31,2)
 rectfilll(1,1,126,30,0)

 local x,y=48+flrnd(32),8+flrnd(16)
 local sx,sy,dirs,nrooms,dir=x,y,{{-1,0},{1,0},{0,-1},{0,1}},1,nil
 
 rectfilll(x-2,y-2,x+2,y+2,3)
 
 while nrooms<32 do
  mset(x,y,4)
  
  if chance(50) or not dir then
   dir=pick(dirs)
  end
  
  local px,py=x,y
  x+=dir[1]
  y+=dir[2]
  
  local c=mget(x,y)
  
  if c==2 then
   x=px
   y=py
   dir=nil
  elseif c==1 then
   x+=dir[1]
   y+=dir[2]
  elseif c==0 then
   local b=true
   local n=sgn(dir[1])*sgn(dir[2])
   for xx=-dir[2],dir[2]+2*dir[1],n do
   for yy=-dir[1],dir[1]+2*dir[2],n do
    b=b and mget(x+xx,y+yy)==0
   end
   end
  
   if b then
    mset(x,y,1)
    x+=dir[1]
    y+=dir[2]

    local ar,arr,arrr={x,y,x,y},{-1,-1,1,1},parse"0,0,0,0"
    
    repeat
     local n=flrnd(4)+1
     ar[n]+=arr[n]
     
     local b
     for xx=ar[1]-1,ar[3]+1 do
     for yy=ar[2]-1,ar[4]+1 do
      b=b or mget(xx,yy)>1
     end
     end
     
     if b or arrr[n]>5 then
      ar[n]-=arr[n]
      arr[n]=0
     end
     
     arrr[n]+=1
     
     if chance(15) then
      arr[n]=0
     end
    
     b=true
     for i=1,4 do b=b and arr[i]==0 end
    until b
    
    rectfilll(ar[1],ar[2],ar[3],ar[4],3)
    nrooms+=1
   else
    x=px
    y=py
    dir=nil
   end
  end
 end
 
 local maaap={}
 for y=0,31 do
 for x=0,127 do
  local c,a,v=mget(x,y),y*128+x

  if c==1 then v=3
  elseif c<=2 then v=0
  else v=c-2 end
  maaap[a]=v
 end
 end
 
 for y=0,31 do
 for x=0,127 do
  local a=y*128+x
  local c=maaap[a]
  if c==0 then
   local k=0
   for xx=x-1,x+1 do
   for yy=y-1,y+1 do
    if in_box(xx,yy,1,1,127,31) and maaap[yy*128+xx]>0 then
     k+=1
    end
   end
   end

   if k>0 then
    if y<31 and maaap[a+128]~=0 then
     mset(x,y,32+flrnd(4))
    else
     mset(x,y,36)
    end
   else
    mset(x,y,0)
   end
  elseif c==2 and rnd(4)<3 then
   mset(x,y,8+flrnd(3))
   local squa=true
   for xx=0,1 do for yy=0,1 do
    local m=mget(x-xx,y-yy)
    squa=squa and m>=8 and m<=11
   end end

   if squa and chance(50) then
    for i=0,3 do
     mset(x-i%2,y-flr(i/2),15-i)
    end
   end
  elseif c==3 then
   if maaap[a-1]==0 and maaap[a+1]==0 then
    mset(x,y,24)
    create_door(x,y,true)
   elseif maaap[a-128]==0 and maaap[a+128]==0 then
    mset(x,y,25)
    create_door(x,y)
   end
   
  else
   mset(x,y,16+flrnd(8))
  end
 end
 end
 
 local i,x1,y1,x2,y2=0
 repeat
  repeat
   x,y=rnd128(),rnd(32)
  until not tile_flag(x,y,4)
  i+=1
  if i>499 then return generate_level() end
  
  x,y=untile(x,y)
  x1,y1,x2,y2=get_room(x,y)
  x1,y1=tile(x1+8,y1+8)
  x2,y2=tile(x2-12,y2-12)
 until x2-x1>4 and y2-y1>4
 
 rectfilll(x1,y1,x2,y2,28)
 for xx=x1,x2 do
  if tile_flag(xx,y1-1,0) then
   mset(xx,y1-1,38+xx%2)
  end
 end

 local x,y=flr(.5*x1+.5*x2),flr(.5*y1+.5*y2)
 
 mset(x,y,29)
 mset(x,y+2,29)
 local s=create_stand(x*8+4,y*8+20)
 create_shopkeeper(x*8+4,y*8+4,s)
 
 local k,ar,arr=1,parse"{heartseed,swordseed},{heartseed,betterswordseed},{heartseed,bestswordseed}",parse"{carrotseed,berryseed},{beetseed,bigberryseed},{potatoseed,berryberryseed}"
 for i=0,49 do
  local x,y,b=x1+flrnd(x2-x1+1),y1+flrnd(y2-y1+1)
  
  for yy=-1,1 do
   b=b or tile_flag(x,y+yy,4)
  for xx=-1,1 do
   local m=mget(x+xx,y+yy)
   b=b or m==29 or m==24 or m==25 
  end
  end
  
  if not b then
   mset(x,y,29)
   x,y=untile(x,y)
   if k<3 then
    create_stand(x,y,ar[curlvl][k])
   else
    create_stand(x,y,arr[curlvl][k%2+1],true)
   end
   
   k+=1
  end
 end

 for i=0,299 do
  local x,y=flrnd(128),flrnd(32)
  local m=mget(x,y)
  
  if is_free(x,y) then
   if m>=8 and m<16 then
    create_prop(x,y,pick(parse"crate,crate,vase1,vase2,vase3,vase4"))
   elseif m>=16 and m<24 then
    create_prop(x,y,chance(50) and "bush" or "bush2")
   end
  end
 end

 local i=0 
 while mget(x1,y1)~=16 or not is_free(x1,y1) or tile_flag(x1,y1+1,0) or dist(x1,y1,sx,sy)<32 do
  i+=1
  if i>999 then return generate_level() end

  x1,y1=flrnd(128),flrnd(32)
 end
 mset(x1,y1,52)

 mset(sx,sy,53)
 
 return untile(sx,sy) 
end

function get_room(x,y)
 x,y=tile(x,y)
 local lx,hx,ly,hy=x,x,y,y
 
 while tile_flag(lx,y,6) do lx-=1 end
 while tile_flag(hx,y,6) do hx+=1 end hx+=1
 while tile_flag(x,ly,6) do ly-=1 end
 while tile_flag(x,hy,6) do hy+=1 end hy+=1
 
 return lx*8,ly*8,hx*8,hy*8
end



function init_anim_info()
 anim_info=parse[[
player={
idle={
sprites={64,65,66,67,68,69,70,69,68,67,66,65},
dt=0.03
},
run={
sprites={80,81,82,83,84,85},
dt=0.02
},
slash={
sprites={86,87,87,87,87},
dt=0.02
}
},
shopkeeper={
only={
sprites={98,98,96,98,97,98,98,99,100,101,100,99,98,97},
dt=0.06
}
},
bat={
only={
sprites={124,125,126,127},
dt=0.02
}
},
skeleton={
only={
sprites={104,105,106,107},
dt=0.03
}
},
golem={
only={
sprites={120,121,122,123},
dt=0.04
}
},
slime={
only={
sprites={76,77,78,79,76,77,78,79,76,77,78,79,92,92,92,93,93},
dt=0.03
}
},
bigslime={
only={
sprites={108,109,110,111,94,94,94,95,110,111},
dt=0.03
}
},
mage={
only={
sprites={112,113,114,115,112,113,114,115},
dt=0.03
}
},
fireball={
only={
sprites={116,117,118,119},
dt=0.02
}
},
gar={
idle={
sprites={
196,197,198,199,212,
196,197,198,199,212,
213,214,215,228,229,
229,229,229,229,229,
229,229,229,229,229,
229,229,229,230,231,
196,197,198,199,212},
dt=0.03
},
teleport={
sprites={244,245,246,247,247},
dt=0.03
},
dance={
sprites={196,197,198,199,212,231,231,212,199,198,197,196,231,231},
dt=0.03
}
}
]]
end

function draw_anim(x,y,char,state,t,xflip)
 local sinfo=anim_info[char][state]
 local spri,wid,hei,xflip=sinfo.sprites[flr(t/sinfo.dt)%#sinfo.sprites+1],sinfo.width or 1,sinfo.height or 1,xflip or false

 spr(spri,x-wid*4,y-hei*4,wid,hei,xflip)
end

function anim_step(o)
 local state=o.state or "only"
 local info=anim_info[o.name][state]
 
 local v=flr(o.animt/info.dt%#info.sprites)
 
 return v,(o.animt%info.dt<0.01)
end



function update_movement(s,nx,ny,map_walls,bounce)
 s.vx*=s.dec
 s.vy*=s.dec
 
 s.vx+=nx
 s.vy+=ny
 
 s.vx=mid(s.vx,-s.spdcap,s.spdcap)
 s.vy=mid(s.vy,-s.spdcap,s.spdcap)
 
 if map_walls then
  local ox,oy=s.x,s.y
  
  s.x+=s.vx
  if map_col(s) then
   s.x=ox
   
   if bounce then
    s.vx*=-0.6
   else
    s.vx*=0.2
   end
  end
  
  s.y+=s.vy
  if map_col(s) then
   s.y=oy
   
   if bounce then
    s.vy*=-0.6
   else
    s.vy*=0.2
   end
  end
  
 else
  s.x+=s.vx
  s.y+=s.vy
 end
end



function collide_objgroup(obj,groupname)
 for obj2 in group(groupname) do
  if obj2~=obj and not obj2.hidden and on_floor(obj2) and collide_objobj(obj,obj2) then
   return obj2
  end
 end

 return false
end

function collide_objobj(obj1,obj2)
 return (abs(obj1.x-obj2.x)<(obj1.w+obj2.w)/2
     and abs(obj1.y-obj2.y)<(obj1.h+obj2.h)/2)
end

function all_collide_objgroup(obj,groupname)
 local list={}
 for obj2 in group(groupname) do
  if obj2~=obj and not obj2.hidden and on_floor(obj2) and collide_objobj(obj,obj2) then
   add(list,obj2)
  end
 end
 return list
end

function map_col(obj)
 local d,x1,y1,x2,y2=collide_objgroup(obj,"doors"),bounding_box(obj)
 
 return ((d and not d.open) or fget(pmget(x1,y1),0)
      or fget(pmget(x1,y2),0)
      or fget(pmget(x2,y1),0)
      or fget(pmget(x2,y2),0))
end

function pmget(x,y)
 return mget(x/8,y/8)
end

function bounding_box(obj)
 local wh,hh=obj.w/2,obj.h/2
 return obj.x-wh,obj.y-hh,obj.x+wh,obj.y+hh
end



function update_objects()
 local uobjs=objs.to_update
 
 for obj in all(uobjs) do
  obj.update(obj)
 end
end

function draw_objects()
 for i=0,4 do
  local dobjs=objs["to_draw"..i]
 
  --sorting objects by depth
  for i=2,#dobjs do
   if dobjs[i-1].y>dobjs[i].y then
    local k=i
    while(k>1 and dobjs[k-1].y>dobjs[k].y) do
     local s=dobjs[k]
     dobjs[k],dobjs[k-1]=dobjs[k-1],s
     k-=1
    end
   end
  end
 
  --actually drawing
  for obj in all(dobjs) do
   if not obj.hidden and on_floor(obj) then
    obj.draw(obj)
   end
  end
 end
end

function register_object(o)
 for reg in all(o.regs) do
  add(objs[reg],o)
 end
end

function deregister_object(o)
 for reg in all(o.regs) do
  del(objs[reg],o)
 end
end

function group_add(name,o)
 add(objs[name],o)
 add(o.regs,name)
end

function group_rem(name,o)
 del(objs[name],o)
 del(o.regs,name)
end

function group(name) return all(objs[name]) end



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

function apply_palmap(pmap,undo)
 if undo then
  for c1,c2 in pairs(pmap) do
   pal(c1,c1)
  end
 else
  for c1,c2 in pairs(pmap) do
   pal(c1,c2)
  end
 end
end

function darken(double,scrn)
 if double then
  for i=0,15 do
   pal(i,drk[drk[i]],scrn)
  end
 else
  for i=0,15 do
   pal(i,drk[i],scrn)
  end
 end
end

function darkout()
 darken(false,1)
 flip()
 flip()
 
 darken(true,1)
 flip()
 flip()
 
 cls()
 flip()
 flip()
end

function darkin()
 _draw()
 flip()
 flip()
 
 darken(false,1)
 flip()
 flip()
 
 for i=0,15 do
  pal(i,i,1)
 end
end

nums={}
for i=0,9 do nums[""..i]=true end
function parse(str,ar)
 local c,lc,ar,field=1,1,{}
 
 while c<=#str do
  local char=sub(str,c,c)
  
  if char=='{' then
   local sc,k=c+1,0
   while sub(str,c,c)~='}' or k>1 do
    char=sub(str,c,c)
    if char=='{' then k+=1
    elseif char=='}' then k-=1 end
    c+=1
   end
   local v=parse(sub(str,sc,c-1))
   if field then
    ar[field]=v
   else
    add(ar,v)
   end
   lc=c+2
   c+=1
  elseif char=='=' then
   field,lc=sub(str,lc,c-1),c+1
  elseif char==',' or c==#str then
   if c==#str then c+=1 end
   local v,vb=sub(str,lc,c-1),sub(str,lc+1,c-1)
   local fc=sub(v,1,1)
   if nums[fc] then v=v*1
   elseif fc=='%' then v=rnd(vb)
   elseif v=='true' then v=true
   elseif v=='false' then v=false
   end
   
   if field then
    if nums[sub(field,1,1)] then field=field*1 end
    ar[field]=v
   else
    add(ar,v)
   end
   
   field,lc=nil,c+1
  elseif char=='\n' then
   lc+=1
  end
  c+=1
 end
 
 return ar
end

function in_box(x,y,x1,y1,x2,y2)
 return x>=x1 and x<x2 and y>=y1 and y<y2
end

function chance(a) return rnd(100)<a end

function merge_arrays(ard,ars)
 for k,v in pairs(ars) do
  ard[k]=v
 end
 return ard
end

function rnd128() return rnd(128) end
function flrnd(a) return flr(rnd(a)) end
function pick(ar,k) k=k or #ar return ar[flr(rnd(k))+1] end

function dist(x1,y1,x2,y2) return sqrt(sqr(x2-x1)+sqr(y2-y1)) end

function tile(x,y) return flr(x/8),flr(y/8) end
function untile(x,y) return x*8+4,y*8+4 end

function ceil(a) return flr(a+0x.ffff) end
function round(a) return flr(a+0.5) end
function lerp(a,b,i) return (1-i)*a+i*b end
function clamp(a,mi,ma) return min(max(a,mi),ma) end
function sqr(a) return a*a end


__gfx__
0000000022422424224224242422424422424224000000000000000000000000944449449444944447749674977477649949449499449444446dd6d476666d42
00000000299499492444449444944449244444490000000000000000000000004476764244767744766d666d766d766d947767676447764294776767766dd762
0000000044222429474c242924224c2447c42429000000000000000000000000476666d24766d7d2766d766d76d76666476666666d7666d446666dd66d676664
00000000299499442444444424444444444944440000000000000000000000004666d664976d76d49d624dd26676666d966666666d6666644766d66766d666d2
0000000042224229422442c947c42229222417c900000000000000000000000047667662466766624774477476666d6d9766d666d76666d29766677666d76664
0000000024994994244424442444494924444444000000000000000000000000476666d4476666d2766d76666666676646667666766d66d4966666666d4766d4
0000000022242249221c7c292247c4244c724c24000000000000000000000000946d6d42446d6d447666766d7666666d4766666d76d676624476dd6d4444dd42
000000004949949949449499499449494944494900000000000000000000000044242422442242424d624dd24d6d6dd296666667666766d44242422442422422
949949449494949499499494949499449949499499494494944994949949499499494994949764949949499494976494aaa97776aaaaaaa90000000000000000
444444424444444494444442944444424444444294444442444444429444444294444442446d7d4494444442446d7d44a994766da44444940000000000000000
9444444494444442444444424424444494444b4244448444944444444444444446776764947d764246776764947d7642a994766da47776a40000000000000000
4444444294444242944424449494444444b443449448e8424446444294446d427dd00dd694700d427dddddd6947d7d4294446ddda4766da40000000000000000
944444424444494444424942444424429434b44244448444446d14429444d1446770077d44600d446777777d446d7d447776aaa9a4766da40000000000000000
94444444944444424444944494449444944434449444444294414444444444449d6dd6d4947d76429d6dd6d4947d7642766da994a46ddda40000000000000000
444444424444444494444442444444424444444294444442944444429444444294444442446d7d4494444442446d7d44766da994a9aaaaa40000000000000000
4244242242242242442424224242242242242442424224224242442242442422422424224246d242422424224246d2426ddd9444944444440000000000000000
00000000000000000000000000000000000000000000000000000000000000000000300099999994999999949999999400000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000307bb030924242429444444242222222000000000000000000000bb00bb00000
000000000000000000000000000000000000000000000000000000000000000007bbbb0099999994422222229242424209999940000000000000b77bb77b0000
00000000000000000000000000000000000000000000000000000000000000000bb7bb034222222292424242929292920924242000000000000b7bb77bb7b000
1dd1dd1dd1d1d11dd1d1dd1ddd11d1dd0000000000000000414141144144141100bbb3b0924242429494949292929292099999400000000000b7b7ebbbbb7b00
66d6d66d666d666d66d6666d666d6d6d00000000000000009947e89999476d990bb33b3094949492999999949494949204222220000000000b7b7e82bbbbb7b0
6d6d16d16dd16dd16d16ddd16dd16dd10000000000000000442e82944426d2940034330099999994944444429999999409200420000000000b7be882bbbbb7b0
d1111d11d111d111d11d1111d11dd111000000000000000022282242222d22420002400042222222422222224222222200000000000000000b7be8882bbbb7b0
1111111111111111111111111111111190000004477dd76409999940000920000000000000000000000000000000000000766600000000000bd7b8882bbb7db0
1000001110010011111010111001111101000000961111d292429492000420000007b000007766000077660000000000000dd0000000000000bd7b882bb7db00
101111d1101111d110111111101111110dd11d50477dd7649242429200094000000bb300000dd000000dd0000077660000766c0000000000000bd7b2bb7db000
101111d1111111d1111111d1111111110510011096d11dd2929497d200042000007b3b00000bb00007eeee20000dd00007cccc10000000000000bd7bb7db0000
101111d11011111110111111111111d1066556d0477dd76294429d1200794d0003b3b3b000766600076666d0007666000ccccc100000000000000bd77db00000
101111d1101111d111111111101111110d51155096d11dd24242424200d42100003b3b0007bbb3300eeee2100799994000c66d0000000000000000bddb000000
11ddddd111d1ddd1111dd1d111111dd1077dd760977dd76292949492000940000b343330066ddd100066d100099994200066d100000000000000000bb0000000
1111111111111111111111111111111106d11dd046d11dd242222222000920000002400000dd1100000d1000066dd110000d1000000000000000000000000000
00000700000000000000000000000000000000000000000000700000000000000000000000000000000000000000000000000000000000000000000000000000
00077700000770700000000000000000000000000707700000777000000000000000070777700000000000000700000000000000000000000000000000000000
00077700000770700007700000077000000770000707700000777000000000000000000077777000000000000000770000000000000000000000000000000000
00077000007777000007700700077000700770000077770000077000000000000000000007777700000000000000007000330000000330000000330000000000
007770000707700007777770777777770777777000077070000777000000000000000000007777000000000000000000037733000037b30000337b3000333300
00777000070770007007700000077000000770070007707000077700000000000000000000777770000000000000000703bbbb30037bbb300377bb300377bb30
00770700000707000007070000070700000707000007070000070700000000000000000000077770000000000000000703bb333003b33b300333bb3003b33b30
00700700007007000070070000700700007007000070070000700700000000000000000000077770000000000000000000333300003333000033330000333300
00000000000000000000000000000000000000000000000000000000000000000000000000077770000000000000000700000000000000000000000000033330
0007700000077000000770000000000000000000000000000007707000000000000000000077777000000000000000070000000000033300033300000337bbb3
0007700000077000000770000007700000077000000770000007707007077000000000007777777000000000000000770000000000377b30377b3000377bbbb3
77777777077777000077700000077000000770000007700007777700070770000000007777777700000000000000007000000000037bbb3037bbb3003bbb33b3
0007700070077077070777700077700000777700077777007007700000777700000007777777770000000000000007700333000003bb33003bbbbb303bb33bb3
000777700007770007077000007777000707770070077077000777000007707000000777777770000000000000007700377b3000003300003b33bbb303bbb330
000700000770007007700700077007000777707000077000000700700770707000000777777700000000007000777000333bb3000000000003b333b300333000
00700000000000000000007000000700000070000007000000070000000070000000007777000000000000077770000003333300000000000033333000000000
00076000000760000007600000076000000760000007600000000000002002000003000000030000000300000003000000000000000000000000000000000000
767676767676767676767676767676767676767676767676000000000022f20000a9900000a9900000a9900000a9900000000000000000000000000000000000
a004400aa004400aa004400aa004400aa004400aa004409400000000000ff0000094400000944077009440007794400003333000003333000003333000000000
0022e2000222e2000022e2000022e2000022e2240022e2200000000002e49e20076766770067660077676670006766003777b33003777b300337bbb303333330
02022020040220200202202002022024020220000202200000000000f089a80f7007000007070000000700070007007037bbbbb337bbbbb3377bbbb33777bbb3
0409a3400009a3400409a3400409a3000409a3000409a30000000000008a7800000677000706776000067000000670703b33bbb33bb33bb33bbb33b33bbbbbb3
0003030000030030000300300003003000030030000300300000000008898a80000600600006000006600700006670003b3333b33b3333b33b3333b33b3333b3
00003000000030000000300000003000000030000000300000000000029229200006000000060000000006000000600003333330033333300333333003333330
00000000000000000000000000000000000bb00000bbbb0000bbbb0000bbbb000000000000000000000000000000000000000000000000000dd00d0000000d00
000ff000000ff000000ff000000ff00000b88b000b8888b00b8888b00b2882b0000d600000000000000d6000000d60000dd00d0000000d0001dd0d20000d0d20
f00ff00f000ff000000ff000000ff0000b8a98b0b889a88bb899998bb289982b0001d000000d60000001d0000001d00001dd0d20000d0d200011d1d000ddd1d0
08a88980f8a8898f08a88980f8a8898fb8a7a98bb89a7a8bb89aa98bb89aa98b001dd1d00001d0000d1dd1000d1dd1000011d1d000ddd1d0000011000dd11100
009a9a00009a9a00f09a9a0f009a9a00b89aa98bb89aa98bb89a7a8bb8a7a98b0d0d600d0d1dd1d0d00d60d0d00d60d0000011000dd111000001100000101100
000882000008800000088000000880000b8998b0b889988bb899a98bb28a982b0d01dd01d00d600d1001d0d01001ddd000011000001011000000000000001000
0002211001222200002222000022200000b88b000b8888b00b8888b00b2882b0010101001001dd010011d0100001101000000000000010000000000000000000
00111000001111100011110000111000000bb00000bbbb0000bbbb0000bbbb000010000000110100000010000001000000000000000000000000000000000000
007c0000007c0000007c0000007c0000b0bb3000b0bb3700b0bb3700b0bb7cc0000066400000000000000094000000000000000000d000000000000000000000
000400000004000000040000000400000b3300000b3300000b330c000b33cc1000001d6007600076000009420007e00000000000006000000007600000007700
0002000000020000000200000b020b000b320b000b320b000b3200000b32c110000042d6711606d600009421007e880000033000d676d0000006d000000076d0
000400000004000000040000003430000034300007343000073430007cc43000000420d666666d11000942100007a0000037b300116110000001176000076dd0
000400000004000000040b0b0b04b30b0b04b30b0b04b3070c04b307cc14b7cc004200116666d1000494210000700a00003b330000d000d0076006d00776d110
0002000000020b0000020b3000020b3000020b3000020b3000020b3cc1120cc104200000d66d10000142100000a009000013310000100d7d06d00110076d1000
000200000002b00000020b3000020b3000020b3000020b3000020b3000020c11420000001111000004140000000a900000011000000001d10110000001dd0000
0004b0000004b3000004b3000004b3000004b3000004b3000004b3000004b3002000000000000000410100000000000000000000000000100000000000110000
0000000000000000000000000000000000000000b0bb37c0b0bb7cc0b0bb370000000000000000000003b00000b0b30000b0b3000000000000000076000000e8
00000000000000000000000000000000000000000b330c100b33cc100b337c1000007c00000b3000007d7d00001331000b0330300000fff00000076d00000e82
00000000000000000000000000000000000000000b3200000b32c1100b32010007c0cc00007ee80000d2d200000a9000007fe2000fff9949000076d10000e821
000000000000000000000000007b7b00000000007c3430007cc43000073430000cc0110007ee888007d7d7d000a9940007feee20ff49999400076d10000e8210
000000000000000000007b0007b3b33000000000c104b37ccc14b7cc7c14b370011007c00ee888200d2d2d20009994000feefe20f99992440476d10006e82100
0000000000000000007bb300000b30000000000000020bc1c1120cc1010207c1007c0cc00e888220017d7d10002a920001efe21094994442014d100001621000
00094000000b300000b3b300007fe2000000000000020b3000020c1100020b1000cc01100182221000d2d20000094000001e2100244422200414000004160000
0094420000944200000b300000fe2200000000000004b3000004b3000004b3000011000000111100001111000002200000011000022200004101000041010000
000000000000000000000000000000000000000007b000000bb007b00000000033000000330000009400000077000000ee000000dd00000007e007e0008008a9
0000000000000000000000000000000007b000007bbb07b0b7eb388300000000a9030000e2030000490300007c000094e8000094d20000947e88e88800088a9a
00000000000000000000000007b000007bb307b0bbbb3bb3be888823000000009a01b0002e01b0009401b000cc0940428809404222094042e88888880808a8a8
0000000000000000007b00000bb30bb0bbbbbbb3bbbbbbb3b888882300000000110010b3110010b3110010b311042011110420111104201188888882008a8a11
000000000000000000b30b000bb3bb3003b3bb3003b3bb300388823000000000000000110000001100000011000119400001194000011940188888210da9a800
00000000000b0b0000b3b30000b3b30000b3b30000b3b30000b223000000000000000b0000000b0000000b000000042000000420000004200188221001da1100
00094000000b3000000b3000000b3000000b3000000b3000000b30000000000000003100000031000000310000000110000001100000011000122100041d0000
009442000094420000b3330000b3330000b3330000b3330000b33300000000000000100000001000000010000000000000000000000000000001100041010000
00000000000000000000000000000000000000000000000000b00b000b0403b000000000000000000000000000000000000b3000000b3b300000000000000000
0000000000000000000000000000000000000000000b0b0000b4300000b430000000e080000000000000000000000000007a790000b0b0030000000000000000
00000000000000000000000000000000000b0b00000b3000000b30b00b6dd3b00000882000000000000000000000000007a79a9007d003000000000000000000
000000000000000000000000000b0b00000b3000000b30b000b333000bb3b300000012100000400000006000000060000a7aa9907d2d07d00000000000000000
0000000000000000000b0b00000b3000000b30b000b333000bb3b30000b3b3000e0801000076420000ee620000a9620007aa9a40d2d27d2d0000000000000000
0000000000000b00000b3000000b300000b333000bb3b30000b3b30000b3b3b008820000001121000011d1000011d1000179a4101d21d2d20000000000000000
00094000000b3000000b3000000b300000b3330000b3b30000b3b3b000b3b30001210000000010000000100000001000001a410001101d210000000000000000
00944200009442000093320000b3330000b3b30000b3b30000b3b30000b3b3000010000000000000000000000000000000011000000001100000000000000000
00000000000000000000000000000000002002000000000000000000002002000000000000000000000000000000000000000000000000000000000000000000
0000770000770000000000bbbb0000000022f20000200200002002000022f20000000bb00bb000000000aaaaaa0000000aaaaaa000aaaaaaaaa0000000000000
0007dd0000dd700000000b7777b00000f20ff0000022f2000022f200000ffe200000b77bb77b0000000aaaa999a00000aaaa999a00aaa9999999000000000000
007d00000000d7000000b7bbbb7b000000e49e2f02eff00000effe00f2e4980f000b7bb77bb7b00000aaa94499990000aa94499900a999444999400776000000
0070000000000700000b7b7777b7b0000089a800f0849e2ff284982f0089a80000b7b7ebb7eb7b0000aa94009994000aa9400a99400994000a9940076d000000
00d0000000000d0000b7b7aaaaab7b00008a7800088a7800008a7800008a78800b7b7e88e882b7b00aaa40000a94000aa9400a99400a940000a940776d000000
00000000000000000b7b7aa99aa9b7b008898a8008898a8008898a8008898a800b7be8888882b7b00aa940000944000aa40000a9400a940000a940766d000000
00000000000000000b7b7a9aaaa9b7b0029229200292292002922920029229200b7be8888882b7b00aa400000000000aa99aaa99400a94000a99406dd0776000
00000000000000000b7b7aaaa7a9b7b0002002000000000000000000000000000bd7b888882b7db00a94000aaaa9900a99999999400a99aaa999400007766600
00000000000000000b7b7aa77aa9b7b00022f20000200200002002000020020000bd7b8882b7db000a99400aa999900a99444499400a9999999400007766dd00
00700000000007000bd7baaaaa9b7db0000ff02f0022f2000022f2000022f200000bd7b22b7db0000a99400a999940a994000099940a994449940000776d0000
007000000000070000bd7b9999b7db00f2e49e00000ff000000ff000000ff0f00000bd7bb7db000000999400a99400a9940000a9940a940009994000766d0000
00d7000000007d00000bd7bbbb7db0000089a800f2e49e2f02e49e2002e49e2000000bd77db0000000a999aa994400994000000a940a94000a9940000766d000
000d77000077d0000000bd7777db0000008a78000089a8000f89a8f00f89a800000000bddb00000000099999444000a94000000a940a940000a9400000666d00
0000dd0000dd000000000bddddb0000008898a80088a7a80088a7a80088a7a800000000bb000000000004444440000944000000944094400009440000007dd00
0000000000000000000000bbbb00000002922920029929200299292002992920000000000000000000000000000000000000000000000000000000007776dd00
000777000077700007777777700000000000000000200200002002000020020000000000000000000000000000000000000000000000000000000000766dd000
0077660000667700700000000700000000200200f022f20f0022f2000022f20000000bb00bb00000000000000bbbb990000009bbbbbb9400000000000ddd0000
0776dd0000dd677070000000070000000022f200020ff020f00ff00f000ff0000000b77bb77b000000000000b3b34444b0009993b33442009bb00b0b00000000
076d00000000d67070000000070000000f0ff0f000e49e0002e49e20f2e49e2f000b7bb77bb7b000000000009b32244433009942322222099b3b00bb30000000
0760000000000670700000000700000002e49e200089a8000089a8000089a80000b7bbbbbbbb7b0000000000933002244b3094203000000993b320b320000000
0dd0000000000dd070000000070000000089a800088a7880088a7880008a78000b7bbbbbbbbbb7b000000000942300024b30b420000000094b34209320000000
00000000000000007000000007000000088a7a8008898a8008898a8008898a800b7bbbbbbbbbb7b00000000094b0000093203b499940000944b4209420000000
00000000000000007000000007000000029929200292292002922920029229200b7bbbbbbbbbb7b000000000942300009320b344442000094234429420000000
00000000000000007000000007000000002002000020020000200200002002000bd7bbbbbbbb7db00000000094b0000094209342222000094294429420000000
000000000000000007777777700000000022f2f00022f2000022f2000022f20000bd7bbbbbb7db00000000009423000094209420000000094209429420000000
07600000000006700000000000000000000ff020000ff00f000ff000000ff000000bd7bbbb7db0000000000094b000094420b420000000094209444430000000
0760000000000670000000000000000002e49e0000e49e2000e49e0000e49e000000bd7bb7db000000000000942300094220b3200000000942094443b0000000
07760000000067700000000000000000f089a8000289a8000289a82f0289a82000000bd77db0000000000000942000b4420093499bbb94094200944b30000000
0d776600006677d00000000000000000008a78800f8a788000fa780000fa78f0000000bddb0000000000000094449b342200944443344209420094b3b0000000
00d7770000777d00000000000000000008898a8008898a8008898a8008898a800000000bb000000000000000944443b22000022222b222094200943b30000000
000ddd0000ddd000000000000000000002922920029229200292292002922920000000000000000000000000022222320000000000300004220003b300000000
__label__
00000000000000000000000111011101110011011101010011010100000110101100110101011101110111001101110110011100110000000000000000000000
00000000000000000000001000100010001100100010101100101000001001010011001010100010001000110010001001100011001010100000000000000000
00000000000000000000000101101010101011101110101010101010001010101010110000101010101011101110111010110110110001010000000000000000
00000000000000000000000101100110001000100110101010101010001010101010110000100010011001100010011010110110001000100000000000000000
00000000000000000000000101101010101110101110001010101101111010101010101000101110101011011010111010110101101000000000000000000000
00000000000000000000000101101010101001100011011001100010001000100110001000101010101000100110001010110110010000000000000000000000
00000000000000000000000010010101010110011100100110011101110111011001110000010001010111011001110101001001100000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010100000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000
00000000000000000000000000000100000000000000000000000000000000000000000000101000000000000000001000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000010000000000000000000101000000000000000000000000000000
00000000000000000000000000000000000000001000000001000000000000000000000000101010000000000000000010000000000000000000000000000000
00000000000000000000000000000000000001010000aaaaaa1100000aaaaaa000aaaaaaaaa10101000000000000000000000000000000000000000000000000
0000000000000000000000000000000000001010000aaaa999a11000aaaa999a00aaa99999991010100000000000000000000000000000000000000000000000
000000000000000000000000000000000001011110aaa94499991000aa94499900a9994449994107760000000000000000000000000000000000000000000000
000000000000000000000010100000000000111111aa94009994000aa9410a99401994101a9940176d1000000000000000000000000000000000000000000000
00000000000000000000000101000000000011111aaa40001a94100aa9401a99400a940000a940776d0000000000000000000000000000000000000000001000
00000000000000000000000010000000010110100aa940000944010aa41101a9411a940001a940766d0000100000000000000000000000000000000000000100
00000000000000000000000000000000001011001aa400000010101aa99aaa99401a94000a99416dd07761110000000000000000000000000000000000000000
00000000000000000000000000000000010101010a94000aaaa9910a99999999410a99aaa9994111077666100000000000000000000000000000000000000000
00000000000000000000000000000000001010101a99410aa999901a99444499400a9999999400107766dd100000000000000000000000000000000000000000
00000000000000000000000000000000010101001a99400a999941a994110199940a994449941111776d01010101000000000000000000000000000000000000
000000000000000000000000000000000010100001999411a99401a9941010a9940a940119994111766d00100010000000000000000000000000000000000000
000000000000000000000000000000000000000000a999aa994400994100010a940a94001a9941111766d0000000000000000000000000000000000000000000
000000000000000000000000000000000000000000199999444000a94000000a940a940000a9411010666d000000000000000000000000000000000000000000
000000000000000000000000000000000000000000014444440000944100000944094400009441010017dd100000000000000000000000000000000000000000
000000000000000000000000000000000000000000001000000000000011100010010000000010107776dd010000000000000000000000000000000000000000
00000000000000000000000000000000000000000001000000011000101111110111000000010101766dd0100000000000000000000000000000000000000000
0000000000000000000000000000000000000000001010001bbbb990011119bbbbbb9410001010101ddd01000000000000000010000000000000000000000000
000000000000000000000000000000000000000000010001b3b34444b1119993b33442009bb11b0b110110100000000000000000000000000000000000000000
0000000000000000000000000000000000000000000000019b32244433119942322222099b3b00bb301001000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000933012244b3194203010001993b320b3210000100000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000942300024b30b421110000194b342093200001000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000010094b0000093213b499940010944b42194200000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000942300019321b3444420001942344294200000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000100000094b00001942193422221010942944294200000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000100194230001942094200010111942094294200000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000194b000094420b4200101011942194444300000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000001942310194220b3201110110942194443b10000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000001942101b4421093499bbb94194201944b301000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000001194449b342211944443344209420094b3b10000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000001944443b22011022222b222094200943b300000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000022222320000000000311004220113b3101000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000001010000011111010100000000000000000000000000000000000000000000
00000000000010000000000000000000000000000000000000000000000000000000100000001100101000000000000000000000000000000000000000000000
00000000000101000000000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000
00000000000010100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000020020000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000022f20000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000001000000000f20ff00000000000000000000000000000000000000000000000000000000000000770000000000000000000000000000000
000000000000000001010000000000e49e2f00000000000000000000000000000000000000000000000000000000000770000000000000000000000000000000
00000000000000000010100000000089a80000000000000000000000000000000000000000000000000000000000777777770000000000000000000000000000
0000000000000000000100000000008a780000000000000000000000000000000000000000000000000000000000000770000000000000000000000000000000
000000000000000000000000000008898a8000000000000000000000000000000000000000000000000000000000007070000000000000000000000000000000
00000000000000000000000000000292292000000000000000000000000000000000000000000000000000000000007007000000000000000000000000000000
00000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000101000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000001000000000000000000010100000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000100000000000000000001000000100000000000000000000000000000000000000000000001000000000000000000000000
00000000000000001000000010001010000000000000001010000001000000000000000000000000000000000000000010000010100000000000000000000000
00000000000001010001010101010100000000000000000001000000000000000000000000000000000000000000000101000111000000000000000000000000
00000000000010100010101010001000000001000000000000000000000001000000000000000000000000000100000010100111000000000000000000000000
00000000000001000000010101000010000000100000000000000000000000100000001000000000000000000010000000000010100000000000000000000000
0000000000000010ddddd01010100d00000ddd0000000000000ddd00dd0ddd0ddd00dd0dd000000001000000dd0d000ddd00dd0d0d0000000000000000000000
000000000000010d00000d010100d0d000d000d000000d0000d000dd00d000d000dd00d00d000010d000000d00d0d0d000dd00d0d0d000000000001000000000
00000000000000d00ddd00d0100d0d10001dd0d0000dd0d000d0d0d0dd1d0d1d0dd0d0d0d0d0010d0d0000d0ddd0d0d0d0d0ddd0d0d000000001010000000000
00000000000000d00d0d00d0100d0d00000d0d1000d000d000d000d0d10d0d0d0dd0d0d0d0d000d000d000d000d0d0d000d000d000d000000010101000000000
00000000000000d00ddd00d0010d0d0000d0dd0000d0dd1000d0d0d0dd0d0d0d0dd0d0d0d0d0001d0d10101dd0d0ddd0d0ddd0d0d0d000000101010000000000
000000000000001d00000d1010d0d10010d000d0001d110010d0d0dd00dd0dd000d00dd0d0d01001d10110d00dd000d0d0d00dd0d0d001000010001000000000
0000000010000001ddddd101101d1010001ddd1000010000001d1d11dd11d11ddd1dd11d1d1000001010101dd11ddd1d1d1dd11d1d1000000000000100000000
00000001010100001111101110010100010111011010100000010100110010011101100101000000010101011001110101011001010011000000001000000000
00000000101010000000001110000000000000100101000000001011000000100000001010100001001010000100000000000000000111000000000000000000
00000000000000000000011000000010100000000000000000000010010000000010010000110010000000000001000000000000000000000000001000000000
0000000000000000ddddd01100000d01010d0d0001000000000ddd0dd00d0d0ddd0dd00ddd00dd0ddd0d0d000000dd00dd0dd00ddd0ddd00dd0d000000000000
000000000000000d00000d000000d0d000d0d0d010100d0000d000d00dd0d0d000d00dd000dd00d000d0d0d0000d00dd00d00dd000d000dd00d0d00000000000
00000000000000d00d0d00d0000d0d1000d0d0d0010dd0d0001d0dd0d0d0d0d0ddd0d0dd0dd0d0d0d0d0d0d010d0ddd0d0d0d0dd0dd0d0d0d0d0d00000000000
00000000000010d000d000d0000d0d00001d0d1000d000d0100d0dd0d0d0d0d00dd0d0dd0dd0d0d00dd000d010d0d1d0d0d0d0dd0dd00dd0d0d0d00000000000
00000000000000d00d0d00d0000d0d0110d0d0d000d0dd10010d0dd0d0d000d0ddd0d0dd0dd0d0d0d0ddd0d010d0ddd0d0d0d0dd0dd0d0d0d0d0dd0100000000
000000000000001d00000d1000d0d10100d0d0d0001d110000d000d0d0dd0dd000d0d0dd0dd00dd0d0d000d0001d00d00dd0d0dd0dd0d0d00dd000d000000000
0000000000000001ddddd100001d1010101d1d1001010010101ddd1d1d11d11ddd1d1d11d11dd11d1d1ddd100001dd1dd11d1d11d11d1d1dd11ddd1000000000
00000000000000001111101000010101010101000000110101011101010010011101010010011001010111000000110110010100100101011001110000000000
01000000010100010000000000101000001010100000001010100000100100000010101101000000000000000000000000001010001010000000000000000000
00000001000000000000010000000000000001000000000001000001000000000010110000100000000000100000000000000101000100000000010000000000
00000010ddddd000ddddd010ddddd000ddddd01000001000100ddd00dd0d0d0ddd01000ddd0ddd00dd0d0d0dd00dd01000001010000000000010100000000000
0000000d00000d0d00000d0d00000d0d00000d0010000d0100d000dd00d0d0d000d000d000d000dd00d0d0d00dd00d0000010100000000000001010000000000
000000d000dd00d00dd000d000d000d00ddd00d0010dd0d010d000d0d0d0d0d0dd1010d0d0d0d0d0d0d0d0d0d0d0d0d000001000010000000000100000000000
000000d00d1d00d00d1d00d00d1d00d00d1d00d010d000d000d0d0d0d0d0d0d00d0100d000d00dd0d0d0d0d0d0d0d0d000000000100000000000000000000000
000000d000dd00d00dd000d00ddd00d000d000d000d0dd1000d0d0d0d0d000d0dd0000d0d0d0d0d0d0d0d0d0d0d0d0d000000000000000000000000000000000
0010101d00000d1d00000d1d00000d1d00000d10101d110000d0d0d00d1d0dd000d010d0d0d0d0d00d1d00d0d0d000d000000000000000000000000000000000
01010101ddddd101ddddd101ddddd101ddddd10101010000001d1d1dd101d11ddd10101d1d1d1d1dd101dd1d1d1ddd1000000000000000000000000000000000
00101010111110101111101011111010111110101010100000010101101010011101010101010101101011010101110000000000000000000000000000000000
00010100000001010000000100000000000000010101000000101010010101000010101000101010000100101010000000000000000000000000000000000000
00001000000000100000000000100000000000001000000000000001011010110001010000010001000001000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000100101010000000000101000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000001000000000000000001010100000000001010000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000101010000000000000000000001000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000010101000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000010100000000000000000000000000000000000001000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000001000000000000000000000000000000000000000100000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00010000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00101000000000000000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000100000000000000101000000000000100000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000001010000000000000010000000000001110000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000010010100000000000000110000000000010111001000000000000000000000000000000
10000000000000000010000000000000000000000000000000000000100101010000000000001000000000000001110110100000000000000000000000000000
00000000000000000000000000000000000000000000010001000010010010001000000000000100000000000001111001010010000000000000000000000000
00000000000000000010000000000000000000000000001010000100101010000000000000001101000000000101110000101000000000000000000000000000
00000000000000000001000000000000001000010000000111001010010101010000001011001010100000001010101000010000010000000000000000000000
00000000000000000000101000001001011100101000011111010101011010101001011111101101000000010000010001101000101000000000000000000000
00000000000000000001010777077717771177117700101177777110111107010017771111177700770000107707770777177707770000000000000000000000
00000000100000000000007000700070007700770070011700000701011070711170007111700077007001070070007000700070007110000000000000000000
000000010100000000000070707070707770777077d1117007770070111707d011d7707111d707707071107077d707707070707707d111000000000000000000
0000000010000000000000700070077007700070007011700707007111170701011707d1011707707071107000770770007007d7071110000000000000000000
0000000000000000000000707770707077d770777070017007770071010707001170770011170770707011d77077077070707077070101000000000000000000
0000000000000000000000707d7070700070077007d011d7000007d000707d11117000700117077007d1117007d7077070707077071010000000000000000000
0000000000000000000000d7d1d7d7d777d77dd77d10111d77777d0000d7d11011d777d1001d7dd77d1101d77d1d7dd7d7d7d7dd7d0101000000000000000000
00000000000000000000000d011d1d1ddd0dd01dd1010111ddddd010011d1101011ddd000101d11dd110001dd111d00d1d1d1d10d01010000000000000000000
00000001000000000000000000111111110000011010101111100101011110101101110010101011110101011010101011111101000000000000000000000000
00001010000000000000000000011111100000010101010111000010111110111010100100010000101010101111110101101000000000000000000000000000
00000000000000000000000000101001000000000010101100000101011111010101010000000000000100001111111000010000000000000000000000000000
00000000000000000000000000010010000000000000010010000010101110001010000000000000001010010111110000100000000000000000000000000000

__gff__
114c4848480000004040404040404040424242424242424210102020404000001111111111111111000000000000000000000000c0c00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000064a80000000000000000000000000000001c000000000000a8a8000000000000a864000000000000a8a80000000000001c00a88da88d
__map__
1113080000081210141109000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
12130a090008131316120a080000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
141715090009090a111210090000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
1113160800090b0a171416090a00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
17150b0a080b1617101610120a00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
1717131512151516161011110b08090a00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
1114161612111608081110151215101700000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
1315171115151215131113121316101100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
1015171712151616131315141313171400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
1314141017171315131314101211101300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
1114141017121415171215151712101300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
1016131217161510111315121216161400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
1317101512121211121112151612121500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
1616121012161017121214151211121700000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
1514151114141517101214151115121400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
1014171514141314151114131512151400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
__sfx__
0110000003065030550304503035080650805508045080350a0650a0550a0450a0350c0650c0550c0450c0350c0650c0550c0450c0350e0650e0550e0450e0350706507055070450703507065070550704507035
011000000305503045030350302506055060450603506025010550104501035010250005500045000350002500055000450003500025020550204502035020250505505045050350502504055040450403504025
011000000606506055060450603506025060150601506005000000000000000000000000000000000000000003065030550304503035030250301503015000000000000000000000000000000000000000000000
011000000a0650a0550a0450a0350a0250a0150a01500005000050000500005000050000500005000050000503065030550304503035030250301503015000050000500005000050000500005000050000500005
010800200f7530000300003000000330500000033050000014753073031400307303147530000300003000030f75300003000030000300003000030000300003147530c003147530c003147530c0030000300003
010800200f7530000300003000000330500000033050000014703073031400307303147030000300003000030f75300003000030000314753000030000300003147530c003147530c003147530c0031475300003
010800200f7530000300003000000f7030000003305000000f753073031400307303147030000300003000030f75300003000030000300003000030000300003147530c003147530c003147530c0030000300003
010800000c1530e5520c1210e1210c1530e5520c1210e121101531155210151111511015311552101511115113153155521315115151131531555213151151510e153105520e151101510c1530e5520c1510e151
0108000011153135521115113151111531355211151131511515317552151511715115153175521515117151181531a552181511a151181531a552181511a1511515317552111511315110153115520c1510e151
010800000e153105520e151101510e151101510e151101511315315552131511515113151151511315115151181531a552181511a151181511a151181511a1511c1531d5521c1511d1511c1511d1511c1511d151
010800001c1531d5521c1511d1511c1511d1511c1511d1511a1531c5521a1511c1511a1511c1511a1511c151181531a552181511a151181511a151181511a1511115313552111511315111151131511115113151
010800000c1530e1520c1210e1210c1530e1520c1210e121101531115210151111511015311152101511115113153155521315115151131531555213151151510e153105520e151101510c1530e5520c1510e151
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010200000f71300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010c00000615506055061250602500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
01080000105531c721117110971124000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010c00002d5552f5552f5152f5052f5052f5050000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
01080000193230f555125150000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
01080000193231b5551e5150000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0108000019323275552a5150000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0108000019323275551e5150000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010800003c6230d612000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010600001733303655173050000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0110000015753137531d753107530e743187330c7230c713000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
011000001575300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
01080000010550f0551e0551e02500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
01080000190550f055120550602500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010c0000341552a1352a1252a11500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010c00000f35300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010c0000366431a6450d0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010800002735300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010800001a15324000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010c0000293531c0551505523005260552305524055180450c035180250c015000150000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010b00001a0550e1451a0350e0251c055101451c035100251d055111451d035110251f055131451f0351302523055171452303517025240551814524035180252405518145240351804524035181252401518015
010800001b1252a515000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
__music__
01 02044344
00 03054344
00 02044344
04 03444344
01 07064344
00 07064344
00 08064344
00 07064344
00 09044344
00 0a044344
00 0a064344
02 09064344

