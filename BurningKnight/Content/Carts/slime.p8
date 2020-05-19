pico-8 cartridge // http://www.pico-8.com
version 19
__lua__
--slime bubble bro
--by guerragames

--
one_frame=1/60
epsilon=.001

t=0

max_air_time=6000

game_finished_once=0
game_finished_last=0
game_finished=false

--
cartdata("slimebubblebro")

map_x=dget(0)
map_y=dget(1)
game_time=dget(2)
player_score=dget(3)
player_deaths=dget(4)
game_finished_once=dget(5)
game_finished_last=dget(6)

--
function save_game()
 dset(0,map_x)
 dset(1,map_y)
 dset(2,game_time)
 dset(3,player_score)
 dset(4,player_deaths)
 dset(5,game_finished_once)
 dset(6,game_finished_last)
end

--
function reset_game()
 game_finished_once=0
 reset_score()
end

--
function reset_score()
 map_x,map_y,game_time,player_score,player_deaths,game_finished_last=0,0,0,0,0,0,0
 save_game()
 run()
end

--
function next_i(l,i)
 i+=1
 if(i>#l)i=1
 return i
end

--
function nl(s)
 local a={}
 local ns=""
 
 while #s>0 do
  local d=sub(s,1,1)
  if d=="," then
   add(a,ns+0)
   ns=""
  else
   ns=ns..d
  end
  
  s=sub(s,2)
 end
 
 return a
end

--
function create_button(btn_num)
 return 
 {
  time_since_press=100,
  time_held=0,
  button_number=btn_num,

  button_init=function(b)
   b.time_since_press=100
   b.time_held=0
  end,

  button_update=function(b)
   b.time_since_press+=one_frame

   if btn(b.button_number) then
    if b.time_held==0 then
     b.time_since_press=0
    end
  
    b.time_held+=one_frame
   else
    b.time_held=0
   end
  end,

  button_consume=function(b)
   b.time_since_press=100
  end,
 }
end

jump_button=create_button(5)
shoot_button=create_button(4)

--
function mag(x,y)
  local d=max(abs(x),abs(y))
  local n=min(abs(x),abs(y))/d
  return sqrt(n*n+1)*d
end

--
function normalize(x,y)
  local m=mag(x,y)
  return x/m,y/m,m
end

--
align_c=0
align_l=1
align_r=2

function print_outline(t,x,y,c,bc,a)
  local ox=#t*2 
  if a==align_l then
   ox=0
  elseif a==align_r then
   ox=#t*4
  end
  local tx=x-ox
  color(bc)
  print(t,tx-1,y)print(t,tx-1,y-1)print(t,tx,y-1)print(t,tx+1,y-1)
  print(t,tx+1,y)print(t,tx+1,y+1)print(t,tx,y+1)print(t,tx-1,y+1)
  print(t,tx,y,c)
end

--
function time_to_text(time)
 local mins=flr(time/60)
 local secs=flr(time%60)
 
  if mins >= 100 then
   return "99:59"
  elseif mins > 0 then
   if secs < 10 then
    return mins..":0"..secs
   else
    return mins..":"..secs
   end
  else
   return ""..secs
  end
end

--
cam_shake_x=0
cam_shake_y=0
cam_shake_damp=0
screen_flash_timer=0
screen_flash_color=7

--
function screenflash(duration,color)
 screen_flash_timer=duration
 screen_flash_color=color
end

--
function screenshake(max_radius,damp)
 local a=rnd()
 cam_shake_x=max_radius*cos(a)
 cam_shake_y=max_radius*sin(a)
 cam_shake_damp=damp
end

--
function update_screeneffects()
 cam_shake_x*=cam_shake_damp+rnd(.1)
 cam_shake_y*=cam_shake_damp+rnd(.1)
 
 if abs(cam_shake_x)<1 and abs(cam_shake_y)<1 then
  cam_shake_x=0
  cam_shake_y=0
 end
 
 camera(cam_shake_x,cam_shake_y)
 
 if screen_flash_timer>0 then
  screen_flash_timer-=one_frame
 end
end

--
function round(x) return flr(x+.5) end
 
function map_coords(x,y)
 return map_x+flr(round(x)/8)%16,map_y+flr(round(y)/8)%16
end

function s_floor(x,y)
 local mx,my=map_coords(x,y)
 local val,val2=mget(mx,my),mget(mx,my-1)
 return fget(val,0) and not fget(val2,2)
end

function s_ceil(x,y)
 local mx,my=map_coords(x,y)
 local val,val2=mget(mx,my),mget(mx,my+1)
 return fget(val,2) and not fget(val2,0)
end

function s_lwall(x,y)
 local mx,my=map_coords(x,y)
 local val,val2=mget(mx,my),mget(mx+1,my)
 return fget(val,3) and not fget(val2,1)
end

function s_rwall(x,y)
 local mx,my=map_coords(x,y)
 local val,val2=mget(mx,my),mget(mx-1,my)
 return fget(val,1) and not fget(val2,3)
end

--
function collision_checks(x,y,vx,vy,ws,we,hs,he)
 local new_x,new_y=x,y
 
 local on_floor,on_ceiling,on_lwall,on_rwall=false,false,false,false
 
 local nvx,nvy,vel_mag=normalize(vx,vy)
 
 local keep_looking=true
 
 while keep_looking do
  local temp_x,temp_y=new_x,new_y
  
  if vel_mag>epsilon then
   local i_vx=(vel_mag>=1) and nvx or (vel_mag*nvx) 
   local i_vy=(vel_mag>=1) and nvy or (vel_mag*nvy)
        
    if not on_floor and not on_ceiling then
     if i_vy>0 then
      if s_floor(new_x+ws+1,new_y+he+1) and not s_floor(new_x+ws+1,new_y+he-1) or 
         s_floor(new_x+we-1,new_y+he+1) and not s_floor(new_x+we-1,new_y+he-1) then
       on_floor=true
       temp_y=round(temp_y)
       nvy=0
       i_vy=0
      end
     else
      if s_ceil(new_x+ws+1,new_y+hs-1) and not s_ceil(new_x+ws+1,new_y+hs+1) or
         s_ceil(new_x+we-1,new_y+hs-1) and not s_ceil(new_x+we-1,new_y+hs+1) then
       on_ceiling=true
       temp_y=round(temp_y)
       nvy=0
       i_vy=0
      end
     end
    end
    
    if not on_rwall and not on_lwall then
     if i_vx > 0 then
      if s_rwall(new_x+we+1,new_y+hs+1) and not s_rwall(new_x+we-1,new_y+hs+1) or
         s_rwall(new_x+we+1,new_y+he-1) and not s_rwall(new_x+we-1,new_y+he-1) then
       on_rwall=true
       temp_x=round(temp_x)
       nvx=0
       i_vx=0
      end
     else
      if s_lwall(new_x+ws-1,new_y+hs+1) and not s_lwall(new_x+ws+1,new_y+hs+1) or
         s_lwall(new_x+ws-1,new_y+he-1) and not s_lwall(new_x+ws+1,new_y+he-1) then
       on_lwall=true
       temp_x=round(temp_x)
       nvx=0
       i_vx=0
      end
     end
    end
    
    --[[]]
    if abs(i_vy)>epsilon and abs(i_vx)>epsilon and not on_floor and not on_ceiling and not on_lwall and not on_rwall then
    if not on_floor and not on_ceiling then
     if i_vy > 0 then
      if s_floor(new_x+ws-1,new_y+he+1) and not s_floor(new_x+ws+1,new_y+he-1) or 
         s_floor(new_x+we+1,new_y+he+1) and not s_floor(new_x+we-1,new_y+he-1) then
       on_floor=true
       temp_y=round(temp_y)
       nvy=0
       i_vy=0
      end
     else
      if s_ceil(new_x+ws-1,new_y+hs-1) and not s_ceil(new_x+ws+1,new_y+hs+1) or
         s_ceil(new_x+we+1,new_y+hs-1) and not s_ceil(new_x+we-1,new_y+hs+1) then
       on_ceiling=true
       temp_y=round(temp_y)
       nvy=0
       i_vy=0
      end
     end
    end
    
    if not on_floor and not on_ceiling and not on_rwall and not on_lwall then
     if i_vx > 0 then
      if s_rwall(new_x+we+1,new_y+hs-1) and not s_rwall(new_x+we-1,new_y+hs+1) or
         s_rwall(new_x+we+1,new_y+he+1) and not s_rwall(new_x+we-1,new_y+he-1) then
       on_rwall=true
       temp_x=round(temp_x)
       nvx=0
       i_vx=0
      end
     else
      if s_lwall(new_x+ws-1,new_y+hs-1) and not s_lwall(new_x+ws+1,new_y+hs+1) or
         s_lwall(new_x+ws-1,new_y+he+1) and not s_lwall(new_x+ws+1,new_y+he-1) then
       on_lwall=true
       temp_x=round(temp_x)
       nvx=0
       i_vx=0
      end
     end
    end
    end
    --]]
    
    if not on_floor and not on_ceiling then
     temp_y+=i_vy
    end
  
    if not on_rwall and not on_lwall then
     temp_x+=i_vx
    end    
  
    vel_mag-=1
  else
   keep_looking=false
  end

  new_x,new_y=temp_x,temp_y
  
  if on_floor or on_ceiling then
   new_y=round(new_y)
  end
 
  if on_rwall or on_lwall then
   new_x=round(new_x)
  end
 end
 
 return {x=new_x,y=new_y,floor=on_floor,lwall=on_lwall,rwall=on_rwall,ceil=on_ceiling}
end

--
function boss_collision_checks(e)
 local new_x,new_y=e.x+e.vx,e.y+e.vy
 local on_floor,on_lwall,on_rwall,on_ceiling=false,false,false,false
 
 if new_x-e.size<0 then
  new_x=e.size
  on_lwall=true
 elseif new_x+e.size>127 then
  new_x=127-e.size
  on_rwall=true
 end
 
 if new_y-e.size<0 then
  new_y=e.size
  on_ceiling=true
 elseif new_y+e.size>127 then
  new_y=127-e.size
  on_floor=true
 end
 return {x=new_x,y=new_y,floor=on_floor,lwall=on_lwall,rwall=on_rwall,ceil=on_ceiling}
end

--
function check_edge_warps(x,y)
  if x+6<1 then
   x=122
  elseif x+2>126 then
   x=-2
  elseif y+6<1 then
   y=122
  elseif y+2>126 then
   y=-2
  end
  
  return x,y
end

--
sbs={}
sbs_next=1

for i=1,22 do
 local sb={}
 sb.timer=0
 add(sbs,sb)
end

--
function sbs_launch(x,y,score,timer)
 local sb=sbs[sbs_next]
 
 sb.x,sb.y=x,y
 sb.score_text=(score>0) and "+"..score or ""..score
 sb.bcolor=(score>0) and 10 or 8
 sb.timer=timer
 
 sbs_next=next_i(sbs,sbs_next)
end

--
function sbs_update()
 for i=1,#sbs do
  local sb=sbs[i]
  
  if sb.timer>0 then
   sb.timer-=one_frame
   sb.y-=.5
  end
 end
end

--
function sbs_draw()
 for i=1,#sbs do
  local sb=sbs[i]
  if sb.timer>0 then
   local blink=(sb.timer%.2)>.1
   print_outline(sb.score_text,sb.x,sb.y,blink and 7 or sb.bcolor,1)
  end
 end
end

--
exps={}
exps_next=1

exps_colors=nl("3,3,11,11,3,11,11,7,10")

exps_level_small={intensity=2,time=.2}
exps_level_medium={intensity=2,time=.4}
exps_level_hard={intensity=4,time=.8}

for i=0,40 do
 local ex={}
 
 ex.t=0
 ex.particles={}
 
 for j=0,30 do
  local p={}
  add(ex.particles,p)
 end
 
 add(exps,ex)
end

--
function exps_spawn(px,py,level)
 local ex=exps[exps_next]
 
 for k,p in pairs(ex.particles) do
  local an=rnd()
  local b=rnd(.15)
  local ra=level.intensity+rnd(.2)

  p.x,p.y,p.vx,p.vy=px,py,ra*cos(an)*sin(b),ra*sin(an)*sin(b)
  
  p.max_t=level.time+rnd(level.time)
  p.t=p.max_t
 end
 
 ex.active=true
 ex.t=2*level.time
 ex.level=level
 
 exps_next=next_i(exps,exps_next)
end

--
function exps_update()
 for k,ex in pairs(exps) do
  if ex.active then
   if ex.t>0 then
    ex.t-=one_frame
    
    for k,p in pairs(ex.particles) do
     p.vx*=.9
     p.vy*=.9
     p.vy+=.05
     p.x+=p.vx
     p.y+=p.vy
     p.t-=one_frame
    end

   else
    ex.active=false
   end
  end
 end
end

--
function exps_draw()
 pink_color_check()
 for k,ex in pairs(exps) do
  if ex.active then
   for k,p in pairs(ex.particles) do
    local s=p.t/p.max_t
    local c=exps_colors[1+flr((#exps_colors-1)*s)]
    circfill(p.x,p.y,ex.level.intensity*s,c)
   end
  end
 end
 pal()
end

--
bullets={}
bullets_next=1

for i=1,100 do
 local b={}
 b.p={}
 b.vx,b.vy=0,0
 b.t=0
 b.s=4
 for j=1,4 do
  b.p[j]={}
 end
 add(bullets,b)
end

--
function bullets_spawn(x,y,vx,vy,s,t)
 local b=bullets[bullets_next]
 
 b.active=true
 b.vx,b.vy=vx,vy
 b.t=t
 b.s=s
 
 b.p[1].x,b.p[1].y=x,y
 
 for j=#b.p,2,-1 do
  b.p[j].x,b.p[j].y=b.p[1].x,b.p[1].y
 end

 bullets_next=next_i(bullets,bullets_next)
end

--
function bullets_collision_checks()
 for k,b in pairs(bullets) do
  if b.active then
   local x,y=b.p[1].x,b.p[1].y
   if b.vy>1 and s_floor(x+b.vx,y+b.vy) then
    b.active=false
   elseif b.vx<-epsilon and s_lwall(x+b.vx,y+b.vy) then
    b.active=false
   elseif b.vx>epsilon and s_rwall(x+b.vx,y+b.vy) then
    b.active=false
   end
   
   if not b.active then
    exps_spawn(x,y,exps_level_small)
   end
  end
 end
end

--
function bullets_check_edge_warps(b)
 if b.x<0 then
  b.x=127
 elseif b.x>127 then
  b.x=0
 elseif b.y<0 then
  b.y=127
 elseif b.y>127 then
  b.y=0
 end
end

--
function bullets_update()
 bullets_collision_checks()

 for k,b in pairs(bullets) do
  if b.active then
   b.t-=one_frame
   
   if b.t<=0 then
    b.active=false
   else
    
    bullets_check_edge_warps(b.p[1])
    
    b.p[1].x+=b.vx
    b.p[1].y+=b.vy
    
    b.vy+=.2
    if b.vy>4 then
     b.vy=4
    end
    
    for j=#b.p,2,-1 do
     b.p[j].x,b.p[j].y=b.p[j-1].x,b.p[j-1].y
    end
   end
  end
 end
end

--
function bullets_draw()
 pink_color_check()
 for k,b in pairs(bullets) do
  if b.active then
   local size_scale=1
   if b.t<.1 then
    size_scale=b.t/.1
   end
  
   for j=#b.p,1,-1 do
    circfill(b.p[j].x,b.p[j].y,1+b.s*size_scale*(1-j/#b.p),3)
   end
   for j=#b.p,1,-1 do
    circfill(b.p[j].x,b.p[j].y,1+b.s*size_scale*(1-j/#b.p)-1,11)
   end
  end
 end
 pal()
end

--
player={}

player_idle_anim=nl("57,57,57,57,57,58,58,58,58,58")
player_run_anim=nl("8,8,8,8,9,9,9,9")
player_start_anim={58}
player_launch_anim={58}
player_arc_anim={43}
player_wall_anim={43}

player_anim=player_start_anim

player_spawn_x=7*8
player_spawn_y=14*8

--
player_init=function()
 player_anim_index=1
 player_anim_flip=false
 player_anim_loops=true
 player_max_vx=1
 player_gravity=.5
 player_max_vy=3
 player_air_time=max_air_time
 player_exploding=0
 
 player_score_mul=0
 
 player_reset()
end

--
function player_is_in_launch_anim()
 return player_anim==player_launch_anim
end

--
function player_set_anim(anim,loops,flips)
 if player_anim!=anim then
  player_anim=anim
  player_anim_index=1
 end
 
 player_anim_loops=loops
 player_anim_flip=flips
end

--
function player_loco_anim(cr)
 if cr.lwall or cr.rwall then
  if abs(player_vx)>.1 and player_anim!=player_wall_anim then
   player_set_anim(player_wall_anim,true,player_anim_flip)
   return
  end
 end
 
 player_set_anim(abs(player_vx)>.1 and player_run_anim or player_idle_anim,true,player_anim_flip)
end

--
function player_anim_advance(cr)
 player_anim_index+=1
 
 if player_anim_index>#player_anim then
  player_anim_index=1
  
  if player_is_in_launch_anim() then
   player_vy=-4.5
   player_gravity=.5
  end
  
  if (not player_anim_loops) player_loco_anim(cr)
 end
end

--
function player_update()
 
 player_touching_death=false
 
 if player_respawning>0 then
  player_respawning-=one_frame
 end
 
 if player_died then
  if player_exploding>0 then
   player_exploding-=one_frame
   
   if player_exploding<0 then
    player_exploding=0
    exps_spawn(player_x,player_y+2,exps_level_hard)
    exps_spawn(player_x+4,player_y+6,exps_level_hard)
    exps_spawn(player_x+8,player_y+2,exps_level_hard)
    player_respawn()
   end
  end

  return
 end
 
 if level_transition>0 and level_transition<1 then
  local speed=4*sin(-.25*(1-level_transition))
  local nvx,nvy,mag=normalize(player_spawn_x-player_x,player_spawn_y-player_y)

  if mag<speed then
   player_x,player_y=player_spawn_x,player_spawn_y
  else
   player_x+=speed*nvx
   player_y+=speed*nvy
  end
  
  return
 end

 player_x,player_y=check_edge_warps(player_x,player_y)
 
 local collision_result=collision_checks(player_x,player_y,player_vx,player_vy,1,6,3,7)
 player_x=collision_result.x
 player_y=collision_result.y
 
 player_anim_advance(collision_result)
 
  player_air_time+=one_frame
  if player_air_time>max_air_time then
   player_air_time=max_air_time
  end

  if not collision_result.floor then
   player_vy+=player_gravity
  
   if player_vy>player_max_vy then
    player_vy=player_max_vy
   end
   
   if (abs(player_vy)>.1)player_set_anim(player_arc_anim,true,player_anim_flip)
  end

  if collision_result.floor then
   if player_air_time>.1 then
    exps_spawn(player_x+4,player_y+8,exps_level_small)
   end
   player_air_time=0
   player_loco_anim(collision_result)
  end
  
  if btn(0) then
   player_vx=-player_max_vx
   player_anim_flip=true
   
   if collision_result.floor and not player_is_in_launch_anim() then
    player_set_anim(player_run_anim,true,player_anim_flip)
   end
  elseif btn(1) then
   player_vx=player_max_vx
   player_anim_flip=false
   if collision_result.floor and not player_is_in_launch_anim() then
    player_set_anim(player_run_anim,true,player_anim_flip)
   end
  else
   if player_air_time>.1 then
    player_vx*=.9
   else
    player_vx*=.5

    if player_vx<.1 then
     if collision_result.floor and not player_is_in_launch_anim() then
      player_set_anim(player_idle_anim,true,player_anim_flip)
     end
    end
    
    if abs(player_vx)<epsilon then
     player_vx=0
    end
   end
  end

  
  if player_air_time<.1 then  
   if jump_button.time_since_press<.2 then
     sfx(1)
     player_vx,player_vy=0,0
     player_gravity=0
     
     jump_button:button_consume()
     player_air_time=max_air_time
     player_set_anim(player_launch_anim,false,player_anim_flip)
   end
  end

  -- shooting bullets
  if shoot_button.time_since_press<.2 then
   shoot_button:button_consume()
   local ox=player_anim_flip and 0 or 8
   local a=.025
   
   local bvx=(player_anim_flip and -3 or 3)*cos(a)
   local bvy=3*sin(a)
   
   sfx(0)
   bullets_spawn(player_x+ox,player_y+2,bvx,bvy,2,.25)
  end
end

--
function player_killed()
 player_touching_death=true
 
 if player_respawning<=0 and not player_died then
  player_deaths+=1
  
  local score_penalty=player_deaths
  sbs_launch(player_x+4,player_y,-10*score_penalty,1)
  player_score-=score_penalty
  
  player_died=true
  player_exploding=.3
  screenshake(6,.7)
  screenflash(.05,8)
  sfx(3)
 end
end

--
function player_reset()
 player_x,player_y=player_spawn_x,player_spawn_y
 player_vx,player_vy=0,0
 player_gravity=.5
 player_died=false
 player_exploding=0
 player_respawning=1
end

--
function player_respawn()
 player_reset()
 player_respawning=2
end

--
function player_sspr(spr_index,size,ysize)
 sspr(spr_index%16*8,flr(spr_index/16)*8,8,8,player_x-size,player_y-ysize,8+size*2,8+size*2,player_anim_flip)
end

-- 
function pink_color_check()
 if game_finished_once==1 then
  pal(3,8)pal(11,14)
 end
end

--
function player_draw()
 pink_color_check()
 
 local spr_index=player_anim[player_anim_index]
 
 if level_transition>0 and level_transition<1 then
  local size=24
  
  if level_show_previous then
   size=24*sin(-.5*(1-level_transition))
  else
   size=42*sin(-.25-.25*(1-level_transition))
  end
 
  spr_index=57
  player_sspr(spr_index,size,2*size+1)
 elseif player_respawning>1.7 then
  local size=16*sin(-.5*(2-player_respawning)/.3)
  
  spr_index=57
  player_sspr(spr_index,size,2*size+1)
 elseif player_respawning>0 then
  local blink=player_respawning%.1>.05
  if blink then
   spr(spr_index,player_x,player_y,1,1,player_anim_flip)
  end
 elseif player_died then
  if player_exploding>0 then
   spr_index=59
   local size=16*sin(-.5*(.3-player_exploding)/.3)
   
   player_sspr(spr_index,size,size)
  end
 else
  spr(spr_index,player_x,player_y,1,1,player_anim_flip)
 end
 
 pal()
end

--
expq={}
expq_delay=0

function expq_push(e)
 if e.active and e.exploding<=0 then
  e.slimed_index=enemy_max_slime_level
  e.exploding=enemy_explosion_time
  add(expq,e)
 end
end

function expq_pop()
 local e=expq[1]
 if e then
  expq_delay=.2
  del(expq,e)
 else
  player_score_mul=0
 end
 return e
end

function expq_update()
 expq_delay-=one_frame
 
 if expq_delay<=0 then
  local e=expq_pop()
  if e then
   if e.isboss then
    player_score+=e.type.scorevalue
    sbs_launch(e.x,e.y,10*e.type.scorevalue,2)
    for i=0,8 do
     exps_spawn(e.x+e.size-rnd(2*e.size),e.y+e.size-rnd(2*e.size),exps_level_hard)
    end
    screenshake(6,.7)
    screenflash(.1,7)
   else
    player_score_mul+=1
    player_score+=player_score_mul
    sbs_launch(e.x+4,e.y,10*player_score_mul,.5+.1*player_score_mul)
    exps_spawn(e.x+4,e.y+4,exps_level_hard)
    screenshake(6,.7)
    screenflash(.025,7)
   end
   
   sfx(2)
  end
 end
end

--
function enemy_ledge_behavior_turn(e)
 if e.x+4>e.last_ledge_x then
  e.vx=e.max_vx
  e.anim_flip=false
 else
  e.vx=-e.max_vx
  e.anim_flip=true
 end
end

--
function enemy_shooter_init(e,wait,rndwait)
 e.shoot_bullet_time=wait+rnd(rndwait)
 e.bullet_timer=0
end

--
function enemy_shooter_spread(e,speed,count,angle)
 if e.bullet_timer<e.shoot_bullet_time then
  e.bullet_timer+=one_frame
   
  if e.bullet_timer>=e.shoot_bullet_time then
   e.bullet_timer=0
    
   if e.x+4>player_x+4 then
    angle+=.5
   end
   
   ebs_shoot_spread(e.x+2,e.y+2,speed,count,angle)
  end
 end
end

--
function enemy_shooter_aimed(e)
 if e.bullet_timer<e.shoot_bullet_time then
  e.bullet_timer+=one_frame
   
  if e.bullet_timer>=e.shoot_bullet_time then
   e.bullet_timer=0
   ebs_shoot_aimed(e.x+4,e.y+4,1,1,0)
  end
 end
end

--
function enemy_shooter_draw(e)
 if e.bullet_timer>=e.shoot_bullet_time-.2 then
  local o=(e.vx>0) and -1 or 0
  circfill(e.x+4+o,e.y+4,6,8)
  circ(e.x+4+o,e.y+4,6,10) 
 end
  
 enemy_default_draw(e)
end

--
function enemy_spawner_init(e,time,spawn_type,max_minions)
 e.spawn_enemy_time=time
 e.spawn_timer=0
 e.spawn_type=spawn_type
 e.max_minions=max_minions
end 

--
function enemy_spawner_update(e)
 if e.spawn_timer<e.spawn_enemy_time then
  e.spawn_timer+=one_frame
   
  if e.spawn_timer>=e.spawn_enemy_time then
   e.spawn_timer=0
   
   if enemies_active_count-1<e.max_minions then
    enemy_spawn(e.spawn_type,e.x,e.y)
   end
  end
 end
end 

--
enemy_types={}

enemy_types[1]=
{
 idle_anim=nl("1,1,1,1,1,1,1,1,1,1,2,2,2,2,2,2,2,2,2,2,"),
 vel_x=.1,
 
 init=function(e)
  enemy_shooter_init(e,2,1)
 end,
 
 update=function(e)
  enemy_shooter_aimed(e)
 end,
 
 ledge_behavior=enemy_ledge_behavior_turn,
 draw=enemy_shooter_draw,
}

enemy_types[5]=
{
 idle_anim=nl("6,6,6,6,6,5,5,5,5,5"),
 vel_x=.1,
 
 init=function(e)
  enemy_shooter_init(e,2,1)
 end,
 
 update=function(e)
   enemy_shooter_spread(e,1,8,0)
 end,
 
 ledge_behavior=enemy_ledge_behavior_turn,
 draw=enemy_shooter_draw,
}

enemy_types[10]=
{
 idle_anim=nl("11,11,11,11,11,10,10,10,10,10"),
 vel_x=.2,
 
 init=function(e)
  enemy_shooter_init(e,2,1)
 end,
 
 update=function(e)
   enemy_shooter_spread(e,1,1,0)
 end,
 
 ledge_behavior=enemy_ledge_behavior_turn,
 draw=enemy_shooter_draw,
}

enemy_types[13]=
{
 idle_anim=nl("13,13,13,13,13,14,14,14,14,14"),
 vel_x=.1,
 
 alert_anim=nl("13,13,13,15,15,15,15,15,15,15"),

 ledge_behavior=enemy_ledge_behavior_turn,
 
 init=function(e)
  e.keep_alert=0
 end,
 
 update=function(e)
  e.keep_alert-=one_frame
  if e.keep_alert<0 then
   e.keep_alert=0
  end

  if e.keep_alert<=0 then
   if e.y+2>player_y+8 or e.y+6<player_y then
    e.max_vx=.1
    e.vx=e.anim_flip and -.1 or .1
    e.anim=e.type.idle_anim
   else
    e.max_vx=1
    e.vx=e.anim_flip and -1 or 1
    e.anim=e.type.alert_anim
    e.keep_alert=1
   end
  end
 end,
}

enemy_types[21]=
{
 idle_anim=nl("21,21,21,21,21,22,22,22,22,22"),
 vel_x=0,
 
 update=function(e)
  if e.slimed_index<=0 then
   if rnd()>.95 then
    if s_floor(e.x+4,e.y+9) then
     if abs(e.vy)<epsilon then
      e.vy=-4.2
      e.anim_flip=player_x<e.x
      e.vx=e.anim_flip and -.6 or .6
     end
    end
   else
    if s_floor(e.x+4,e.y+9) then
     e.vx=0
    end
   end
  end
 end,
 
 ledge_behavior=function(e)end,
}


enemy_types[27]=
{
 idle_anim=nl("28,28,28,28,28,27,27,27,27,27"),
 vel_x=.1,
 
 init=function(e)
  e.gravity=0
  enemy_shooter_init(e,2,1)
 end,
 
 ledge_behavior=function(e)end,
 
 update=function(e)
  enemy_shooter_aimed(e)
  
  if e.slimed_index<1 then
   if rnd()>.99 then
    e.vx,e.vy=e.vy,e.vx 
   end
  end
 end,
 
 draw=enemy_shooter_draw,
}

enemy_types[32]=
{
 idle_anim=nl("32,32,32,32,32,33,33,33,33,33"),
 vel_x=.5,
 
 ledge_behavior=function()end,
}

enemy_types[35]=
{
 idle_anim=nl("36,36,36,36,36,35,35,35,35,35"),
 vel_x=.2,
 
 init=function(e)
  e.max_vy=.25
  e.vy=e.max_vy
  e.timer=0
  e.gravity=0
 end,
 
 ledge_behavior=function(e)end,
 
 update=function(e)
  e.timer-=one_frame
  
  if e.timer<=0 then
   e.timer=.5+rnd(.5)
   
   local nvx,nvy,mag=normalize(player_x-e.x,player_y-e.y)

   e.vx=nvx*e.max_vy
   e.vy=nvy*e.max_vy
  end
 end,
}

enemy_types[37]=
{
 idle_anim=nl("38,38,38,38,38,37,37,37,37,37"),
 vel_x=.5,
 
 init=function(e)
  e.max_vy=.5
  e.vy=e.max_vy
  e.gravity=0
 end,
 
 ledge_behavior=function(e)end,
 
 update=function(e)
  if e.slimed_index<1 then
   if e.collision_result.floor then
    e.vy=-e.max_vy
   elseif e.collision_result.ceil then
    e.vy=e.max_vy
   elseif abs(e.vy)<.01 then
    e.vy=e.max_vy
   end
  end
 end,
}

enemy_types[41]=
{
 idle_anim=nl("41,41,41,41,41,42,42,42,42,42,"),
 vel_x=.3,
 
 init=function(e)
  e.jump_cooldown=0
 end,
 
 update=function(e)
  
  e.jump_cooldown-=one_frame
  
  if e.jump_cooldown<=0 then
   if e.slimed_index<=0 then
    if rnd()>.99 then
     if s_floor(e.x+4,e.y-1) then
      e.vy=-4.2
      e.jump_cooldown=1
     end
    end
   end
  end
 end,
 
 ledge_behavior=function(e)
  if e.slimed_index<=0 then
   if rnd()>.9 then 
    if e.jump_cooldown<=0 then
     e.vy=-4.2
     e.jump_cooldown=1
    end
   end
  end
 end,
}

enemy_types[48]=
{
 idle_anim=nl("49,49,49,49,49,48,48,48,48,48"),
 vel_x=.5,
 
 ledge_behavior=enemy_ledge_behavior_turn,
}

--
function make_boss(ia,sa,ss,ms,bc,bw,sv)
 return {
 idle_anim=ia,
 slimed_anim=sa,
 spr_s=ss,
 vel_x=.2,
 max_s=ms,
 bcount=bc,
 bwait=bw,
 scorevalue=sv,
 
 init=function(e)
  e.gravity=0
  e.isboss=true
  e.angle=0
  e.size=8
  e.damt=0
  enemy_spawner_init(e,5,48,10)
  enemy_shooter_init(e,e.type.bwait,0)
 end,
 
 ledge_behavior=function()end,
 
 take_damage=function(e,dam)
  e.damt=.05
  e.size+=dam
  if e.size>e.type.max_s then
   e.size=e.type.max_s
   
   for k,ae in pairs(enemies) do
    enemy_explode(ae)
   end
  end
 end,
 
 update=function(e)
  e.angle+=.01

  enemy_shooter_spread(e,.5,e.type.bcount,e.angle)
  enemy_spawner_update(e)
  
  e.damt=max(0,e.damt-one_frame)
  
  if e.collision_result.ceil then
   e.vy=e.type.vel_x
  elseif e.collision_result.floor then
   e.vy=-e.type.vel_x
  end
 
  if rnd()>.99 then
   e.vx,e.vy=e.vy,e.vx 
  end
 end,
 
 draw=function(e)
  if e.damt>0 then
   pal(7,8)
   pal(6,8)
  end
  
  local shake=3*(max(.5,e.size/e.type.max_s)-.5)/.5
  
  local sprite=e.anim[e.anim_index]
  sspr(sprite%16*8,flr(sprite/16)*8,e.type.spr_s,e.type.spr_s,e.x-e.size+1+rnd(shake),e.y-e.size+1+rnd(shake),e.size*2,e.size*2,e.anim_flip)
  pal()
 end,
}
end

enemy_types[54]=make_boss(nl("54,54,54,54,54,54,54,54,55,55,55,55,55,55,55,55"),{54},7,16,4,1.5,20)
enemy_types[52]=make_boss(nl("52,52,52,52,52,52,52,52,53,53,53,53,53,53,53,53"),{52},7,24,6,1,40)
enemy_types[25]=make_boss(nl("25,25,25,25,25,25,25,25,26,26,26,26,26,26,26,26"),{25},7,32,8,.8,80)
enemy_types[23]=make_boss(nl("23,23,23,23,23,23,23,23,46,46,46,46,46,46,46,46"),{54},16,42,8,.5,200)

--
function enemy_default_draw(e) 
 spr(e.anim[e.anim_index],e.x,e.y,1,1,e.anim_flip)
end

--

enemy_max_slime_level=6
enemy_explosion_time=.2
enemies={}

for i=0,40 do
 add(enemies,{})
end

--
function enemies_init()
 enemies_next=1
 enemies_active_count=0
 
 enemies_boss=nil
 
 for k,e in pairs(enemies) do
  e.active=false
  e.x,e.y=0,0
  e.type_i=48
  e.type=enemy_types[e.type_i]
  e.anim_index=1
  e.anim=e.type.idle_anim
  e.max_vx=e.type.vel_x
  e.vx,e.vy=e.max_vx,0  
  e.anim_flip=false
  e.slimed_index=0
  e.last_slimed=0
  e.invulnerable=0
  e.exploding=0
  e.isboss=false
 end
end

--
function enemy_spawn(spr,x,y)
 if enemies_active_count>=#enemies then
  return
 end
 
 enemies_active_count+=1
 
 local e=enemies[enemies_next]
 while e.active do
  enemies_next=next_i(enemies,enemies_next)
  e=enemies[enemies_next]
 end

 e.active=true
 e.type_i=spr
 e.type=enemy_types[e.type_i]
   
 e.anim=e.type.idle_anim
 e.slimed_index=0
 
 e.x,e.y=x,y
 
 e.max_vx=e.type.vel_x
 e.max_vy=nil
 e.vx=(e.x<64) and -e.max_vx or e.max_vx  
 e.vy=0
 
 e.last_ledge_x,e.last_ledge_y=0,0
 
 e.anim_flip=(e.vx<0)

 e.gravity=nil
 
 if(e.type.init)e.type.init(e)
 
 if(e.isboss)enemies_boss=e
end

--

function slimed_enemy_check_freeze(eb)
 if eb.slimed_index>1 then
  local ebx,eby=eb.x,eb.y
  for k,e in pairs(enemies) do
   if e.active and e!=eb then
    if not e.isboss then
     local ex,ey=e.x,e.y
   
     local mags=mag(ex-ebx,ey-eby)
   
     local bubble_size=eb.slimed_index*4
     if mags<bubble_size then
      if e.slimed_index<=0 then
       e.slimed_index=1
      end
     end
    end
   end
  end
 end 
end

--
function enemy_check_collateral(eb)
 local ebx,eby=eb.x+4,eb.y+4
 local bubble_size=eb.slimed_index*4
 
 for k,e in pairs(enemies) do
  if e.active and e!=eb then
   if e.isboss then
     local mags=mag(e.x-ebx,e.y-eby)
     if mags<bubble_size+e.size then
      e.type.take_damage(e,1.5)
     end
   else
    if e.slimed_index>=1 then
     enemy_explode(e)
    else
     local ex,ey=e.x+4,e.y+4
     local mags=mag(ex-ebx,ey-eby)
    
     if mags<bubble_size then
      enemy_explode(e)
     end
    end
   end
  end
 end 
end

--
function slimed_enemy_check_player_collision(e)
 local px,py=player_x,player_y
 local ex,ey=e.x,e.y
   
 local nvx,nvy,mag=normalize(ex-px,ey-py)
   
 local bubble_size=4+e.slimed_index*4
 if mag<bubble_size then
  e.vx=nvx*(bubble_size-mag)*.2
 end 
end

--
function enemy_movement(e)
 e.x,e.y=check_edge_warps(e.x,e.y)
 
 if e.isboss then
  e.collision_result=boss_collision_checks(e)
 else
  e.collision_result=collision_checks(e.x,e.y,e.vx,e.vy,0,7,0,7)
 end
 
 e.y=e.collision_result.y
 
 e.x=e.collision_result.x
 if e.collision_result.rwall then
  e.vx=-e.max_vx
  e.anim_flip=true
 elseif e.collision_result.lwall then
  e.vx=e.max_vx
  e.anim_flip=false
 end
 
 if e.collision_result.rwall or e.collision_result.lwall or
    e.collision_result.floor or e.collision_result.ceil then
  e.last_collision_x=e.collision_result.x
  e.last_collision_y=e.collision_result.y
 end
 
 if e.type.ledge_behavior then
  if s_floor(e.x+4,e.y+9) then
   if e.vx>0 and not s_floor(e.x+7,e.y+9) then
    e.last_ledge_x,e.last_ledge_y=e.x+7,e.y+9
    e.type.ledge_behavior(e)
   elseif e.vx<0 and not s_floor(e.x-1,e.y+9) then
    e.last_ledge_x,e.last_ledge_y=e.x-1,e.y+9
    e.type.ledge_behavior(e)
   end
  end
 end
end

--
function enemy_check_player(e)
 if e.isboss then
  local mags=mag(e.x-player_x-4,e.y-player_y-4)
  if mags<e.size+4 then
   player_killed()
  end  
 else
  if player_x+6<e.x or
     player_x>e.x+8 or
     player_y+6<e.y or
     player_y>e.y+8 then
  else
   player_killed()
  end
 end
end

--
function enemy_explode(e)
 expq_push(e)
end

--
function enemy_player_bullet_collision(b,e)
 local bp=b.p[1]
 
 if e.isboss then
  local mags=mag(e.x-bp.x,e.y-bp.y)
  if mags>e.size+3 then
   return false
  end
 else
  if bp.x+1<e.x or
     bp.x-1>e.x+8 or
     bp.y+1<e.y or
     bp.y-1>e.y+8 then
   return false
  end
 end
 
 return true
end

--

function enemy_update_nonexploding(e)
 -- slimed movement
 if e.slimed_index>=1 then
  e.x+=e.vx
  e.y+=e.vy
   
  if abs(e.vx)>0 then
   e.vx*=.8
  end

  if e.vx>0 then
   if s_rwall(e.x+8+e.vx,e.y+4) then
    e.vx=0
   end
  elseif e.vx<0 then
   if s_lwall(e.x+e.vx,e.y+4) then
    e.vx=0
   end
  end
 end

 -- gravity
 if e.collision_result and not e.collision_result.floor then
  local g=e.gravity or .5
  local max_vy=e.max_vy or 4
      
  if e.slimed_index>=1 then
   max_vy=.5
   g=.5
  end
      
  e.vy+=g
      
  if e.vy>max_vy then
   e.vy=max_vy
  end
 else
  local has_gravity=(e.slimed_index>=1 or not e.gravity or e.gravity>0)
  if has_gravity and e.vy>0 then
   e.vy=0
   e.y=flr(e.y)
  end
 end

 -- advance animation
 e.anim_index=next_i(e.anim,e.anim_index)
   
 if e.invulnerable>0 then
  e.invulnerable-=one_frame
 end
   
 -- slimed cooldown
 if e.slimed_index>=1 then
  e.last_slimed-=one_frame
  if e.last_slimed<=0 then
   e.slimed_index-=1
   e.last_slimed=2
     
   if e.slimed_index<=0 then
    e.vx=e.anim_flip and -e.max_vx or e.max_vx
   end
  end
    
  if e.slimed_index>0 then
   slimed_enemy_check_player_collision(e)
     
   slimed_enemy_check_freeze(e)
  end
 end

end

--
function enemies_update()
 expq_update()
 
 local enemies_exploding=(#expq>0)
 
 for k,e in pairs(enemies) do
  if e.active then
   if not enemies_exploding then
    enemy_update_nonexploding(e)
   end
   
   if not enemies_exploding and e.exploding>0 then
    e.exploding-=one_frame
    if e.exploding<=0 then
     
     enemy_check_collateral(e)
     
     enemies_active_count-=1
     e.active=false
     
     if enemies_active_count<=0 then
      level_goto_next()
     end
    end
   end
   
   if e.exploding<=0 then
    for k2,b in pairs(bullets) do
     if b.active then
      if enemy_player_bullet_collision(b,e) then
       b.active=false
       
       if(e.type.take_damage)e.type.take_damage(e,.1)
      
       if e.invulnerable<=0 then
        e.invulnerable=.2
      
        if not e.isboss then
         e.slimed_index+=1
         e.vx,e.vy=0,0
         e.last_slimed=(e.slimed_index>=enemy_max_slime_level-1) and 6 or 2
        end
        
        if not enemies_exploding and e.slimed_index>=enemy_max_slime_level then
         enemy_explode(e)
        else
         exps_spawn(b.p[1].x,b.p[1].y,exps_level_medium)
         screenshake(2,.7)
        end
       else
        exps_spawn(b.p[1].x,b.p[1].y,exps_level_small)
       end
      end
     end
    end
   end
  end
 end
 
 if not enemies_exploding then
  for k,e in pairs(enemies) do
   if e.active then
    enemy_movement(e)
   
    if e.slimed_index<1 then
     if(e.type.update)e.type.update(e)
     enemy_check_player(e)
    end
   end
  end
 end
end

--
function enemy_draw_bubble(e,sx,sy)
 pink_color_check()
 circfill(e.x+4+sx-e.slimed_index*2,e.y+sy+8-e.slimed_index*2.5,e.slimed_index/1.5,7)
 circ(e.x+sx+3+1,e.y+sy+4,e.slimed_index*4-1,7)
 if e.slimed_index>=3 then
  circ(e.x+sx+3,e.y+sy+4,e.slimed_index*4-1,7)
  circ(e.x+sx+3+2,e.y+sy+4,e.slimed_index*4-1,7)
 end
 circ(e.x+sx+4,e.y+sy+4,e.slimed_index*4,11)
 pal()
end

--
function enemies_draw() 
 for k,e in pairs(enemies) do
  if e.active and e.exploding<=0 then
   if e.slimed_index<enemy_max_slime_level-1 then   
    if e.slimed_index<=0 then
     if e.type.draw then
      e.type.draw(e) 
     else
      enemy_default_draw(e)
     end
    end
   end
  end
 end

 for k,e in pairs(enemies) do
  if e.active and e.exploding<=0 then
   if e.slimed_index<enemy_max_slime_level-1 then   
    if e.slimed_index>0 then
     spr(e.type.slimed_anim or e.type.idle_anim[1],e.x,e.y)
      
     local blink=(e.slimed_index!=1 or e.last_slimed>1 or e.last_slimed%.1>.05)
      
     if blink then
      enemy_draw_bubble(e,0,0)
     end
    end
   end
  end
 end

 for k,e in pairs(enemies) do
  if e.active then
   if e.slimed_index>=enemy_max_slime_level-1 or e.exploding>0 then   
     local shakex,shakey=1-rnd(2),1-rnd(2)
     spr(e.type.slimed_anim or e.type.idle_anim[1],e.x+shakex,e.y+shakey)
     enemy_draw_bubble(e,shakex,shakey)
   end
  end
 end
end

--
ebs={}
ebs_next=1
ebs_blink=0
ebs_blink_on=true

for i=1,100 do
 add(ebs,{})
end

function ebs_reset()
 for k,eb in pairs(ebs)do
  eb.active=false
 end
end

function ebs_make_bullets(x,y,speed,count,inc_a,start_a)
 sfx(4)
 local ia=start_a
 for i=1,count do
  local eb=ebs[ebs_next]
  eb.x,eb.y=x,y
  eb.velx,eb.vely=speed*cos(ia),speed*sin(ia)
  eb.active=true
  ebs_next=next_i(ebs,ebs_next)
  
  ia-=inc_a
 end 
end

--
function ebs_check_player(eb)
 if player_x+6<eb.x+1 or
    player_x>eb.x+3 or
    player_y+6<eb.y+1 or
    player_y>eb.y+3 then
 else
  eb.active=false
  player_killed()
 end
end

--
function ebs_update()
 if ebs_blink<=0 then
  ebs_blink=.05
  ebs_blink_on=not ebs_blink_on
 else
  ebs_blink-=one_frame
 end

 for k,eb in pairs(ebs)do
  if eb.active then
   if eb.y<-8 or eb.x<-8 or eb.y>142 or eb.x>142 then
    eb.active=false
   else
	   eb.y+=eb.vely
    eb.x+=eb.velx

    ebs_check_player(eb)
    
    for k,e in pairs(enemies) do
     if e.active and e.slimed_index>=1 then
      local ex,ey=e.x+4,e.y+4
   
      local mags=mag(ex-eb.x,ey-eb.y)
      
      local bubble_size=e.slimed_index*4+1
      if mags<bubble_size then
       eb.active=false
       exps_spawn(eb.x,eb.y,exps_level_small)
      end
     end
    end
   end
  end
 end
end

--
function ebs_draw()
 pal()

 if ebs_blink_on then
  pal(8,9)
  pal(10,8)
 end
  
 for k,eb in pairs(ebs)do
  if eb.active then
   spr(3,eb.x,eb.y)
  end
 end
 
 pal()
end

function ebs_shoot_aimed(x,y,speed,count,angle)
 local to_player_x,to_player_y=player_x-x,player_y-y
 local nx,ny,mag=normalize(to_player_x,to_player_y)
 local a=atan2(nx,ny)
 ebs_make_bullets(x,y,speed,count,angle/count,a)
end

function ebs_shoot_spread(x,y,speed,count,angle)
 ebs_make_bullets(x,y,speed,count,1/count,angle)
end

--
level_transition=0
level_show_previous=false
level_show_title=0
level_number=0
next_map_x=map_x
next_map_y=map_y

--
function level_goto(mx,my)
 level_number=flr(mx/16)+flr(my/16)*8
 
 if level_number<0 then
  level_number=0
 elseif level_number>31 then
  level_number=31
 end

 level_transition=.99
 level_show_previous=false
 next_map_x=level_number%8*16
 next_map_y=flr(level_number/8)*16
end

--
function level_goto_next()
 if level_number<32 then
  level_transition=2
  level_show_previous=true
  level_number+=1
  next_map_x=level_number%8*16
  next_map_y=flr(level_number/8)*16
  
  if level_number>=32 then
   player_spawn_x=6*8
   player_spawn_y=8*8
  end  
 end
end

--
function level_init()
 for i=map_x,map_x+15 do
  for j=map_y,map_y+15 do
   local tile_spr=mget(i,j)
   if fget(tile_spr,7) then
    enemy_spawn(tile_spr,(i-map_x)*8,(j-map_y)*8)
   end
  end
 end
end

--
function level_update()
 if level_transition>0 then
  level_transition-=one_frame
  
  if level_transition<1 and level_transition+one_frame>1 then
   sfx(5)
  end
  
  if level_transition<=0 then
   level_transition=0
   map_x=next_map_x
   map_y=next_map_y
   
   save_game()
   game_view.start()
   
   if level_number>=32 then
    game_finished=true
    game_finished_last=1
    game_finished_once=1
    level_number=0
    map_x,map_y=0,0
    save_game()
   end

   level_show_title=2
  end
 end
 
 if level_show_title>0 then
  level_show_title-=one_frame
 end
end

--
function level_draw()
 if level_transition>0 then
  local offsety=-128*sin(.25-level_transition/4)
  map(next_map_x,next_map_y,0,offsety-128,16,16,0x10)
  if(level_show_previous)map(map_x,map_y,0,offsety,16,16,0x10)
 end
end

--
function level_title_draw()
 if level_show_title>0 then
  local a,b=map_y/16+1,map_x/16+1
  if(b==8)b="boss"
  print_outline("floor "..a.."-"..b,64,64,7,1)
 end
end

--
function ge_sspr(spx,spy,x,y,s,f)
 sspr(spx,spy,8,8,x-s,y-s,s*2,s*2,f)
end

--
function game_end_draw()
 for i=0,999 do
  pset(rnd(128),rnd(128),rnd(3))
 end
 
 if t%(14*one_frame*2)<one_frame then
  player_anim_flip=not player_anim_flip
  exps_spawn(rnd(128),rnd(128),exps_level_hard)
 end
 
 local spr_index=57
 local spr_index2=44
 
 local spr1x,spr1y=spr_index%16*8,flr(spr_index/16)*8
 local spr2x,spr2y=spr_index2%16*8,flr(spr_index2/16)*8
 
 local tover2=t*.5

 for a=0,1,.03 do
  local r=90*(a)
  local x,y=64+r*cos(a*tover2),64+r*sin(a*tover2)
  local size=r*.2
  if a%.06>=.03 then
   ge_sspr(spr1x,spr1y,x,y,size,player_anim_flip)
  else
   ge_sspr(spr2x,spr2y,x,y,size,not player_anim_flip)
  end
 end
 
 ge_sspr(spr1x,spr1y,48,56,16,player_anim_flip)
 ge_sspr(spr2x,spr2y,80,56,16,not player_anim_flip)
 
 print_outline("congratulations!",64,74,7,1)
 print_outline("you rescued slimette!",64,84,7,1)
end

--
music(0)

--
function _init()
 menuitem(1,"reset progress!?",reset_game)

 if game_finished_last==1 then
  reset_score()
 end

 t=0
 
 jump_button:button_init()
 shoot_button:button_init()

 player_init()
 enemies_init()
 ebs_reset()
end

--
fe_view={}

local title="06ee010e010001e002ee010e010003ee010e010002ee010004ee010e01e002ee050024ee01300133010302ee0130013301e002ee0130013301e0010e013301e003ee0130010301ee010e053301e022ee010e01b3017701370100010e01730177010301ee010e01730177010301300177010302ee010e0173013701e0013003bb017b0177010322ee013002bb017b0133010001b301bb013701e0013001bb017b0103013001bb013701e001ee013001bb0137010001b305bb013701e020ee010e01b303bb0177013302bb013701e0013002bb013701b301bb013701e001ee013001bb017b010301b305bb013701e020ee013005bb013702bb013b0100013002bb013701b302bb0103010001b301bb017b013302bb023302bb010321ee013001bb013b013302bb013702bb013b0100013002bb013b01b302bb013b013303bb013301bb013b0200023301e020ee010e013001bb0103010001b301bb013b02bb013b0100013002bb013b01b307bb013301bb013b040021ee010e013001bb013b0100013001bb013b02bb013b020001b301bb013b01b307bb013301bb013b01e0020022ee010e013002bb013301000133010301b301bb013b020001b301bb013b01b307bb013301bb013b01e024ee010e010001b302bb0103020001b301bb010301ee0100013001bb013b01b301bb013b02bb013b02bb013301bb013b01e024ee010e0100013002bb013b01e0010001b301bb010301ee0100013001bb013b01b301bb013b01b301bb013302bb013301bb013b020024ee020001b302bb0103010001b301bb010301ee010e013001bb013b01b301bb013b01b301bb013302bb013302bb023301e023ee010e0100013002bb0137010001b301bb010301ee010e013001bb013b01b301bb013b01b301bb013302bb013302bb017b0177010324ee020001b301bb017b010301b301bb010301ee010e013001bb013b01b301bb013b0130013b013002bb013304bb013701e022ee0300013001bb017b010301b301bb010301ee010e013001bb013b01b301bb013b01000103013002bb013304bb010318ee010e020007ee010e013301030100013002bb013701b3013b01e0010e0100013001bb013b01b301bb013b0200013002bb013302bb023301e017ee0100013001bb013b010006ee0130017b0137020001b301bb013701b3013b01e001300103013001bb013b01b301bb013b01e0010e013002bb013301bb013b020017ee010001b303bb013b01e005ee013001bb017b0103010001b301bb013b01b3013b010001730137013001bb013b01b301bb013b01e0010e013002bb013301bb013b010017ee010e01b301bb017b017701b701bb010304ee010e013001bb017b0103010001b301bb013b01b3013b013001bb017b013301bb013b01b301bb013b01e0010e013002bb013301bb013b01e017ee010002bb027701b701bb013b01e003ee010e013002bb0103010001b301bb013b01b3013b013001bb017b013301bb013b01b301bb013b01e0010e013002bb013301bb013b01e016ee010e01b301bb017b01b703bb013b01e003ee010e013002bb013b010001b301bb013b01b3013b01b302bb013301bb013b01b301bb013b01e0010e013002bb013301bb013b01e001ee020013ee013001b301bb01b704bb013b01e003ee010e013003bb013302bb010301b3013b03bb013301bb013b01b301bb013b01e0010e013002bb013301bb013b0200023301e012ee013007bb013b01e003ee010e010001b305bb010301b303bb0133013001bb013b01b301bb013b01e0010e013002bb013302bb0233017b0177010311ee010e013307bb013b01e003ee010e010001b304bb013b010001b302bb01330100013001bb013b01b301bb013b01e0010e013002bb010301b305bb013701e010ee013001b307bb013b01e004ee0100013004bb01e3010001b301bb013b0100010e013001bb013b01b301bb013b01e0010e013002bb010301b305bb013701e010ee013008bb013301e004ee0200013302bb013b01e0010001b301bb010301ee010e010001b30103013001bb010301ee010e010001b3013b0100013005bb010310ee010e013307bb0133010305ee010e02000233010301e001000130013301e001ee010e0100013002000133010001e0010e0100013001030200053301e010ee010e013306bb023301e006ee02000130010301000103013001030130010301ee013001030100010302000133010301ee010001300103010e01300103010001300133010011ee010e013304bb0333010308ee010001b301370130013701b3013701b30137010001b30137013001370100013001bb013701e0010e01b30137010001b30137010001b3017b010311ee010e013301b301bb013b0333010301e008ee013001bb017b0133013b01b3013b01bb017b013301bb017b0133013b010001b301bb017b0103013001bb017b013301bb017b013302bb013701e00cee010e010001e002ee013004330103010001e009ee0130013b01b3023b01b3023b01b3023b01b3023b010001b3023301e00130013b01b3023b01b3023b01b3013b01e00bee010e01300133010302ee010e0333010001e00aee010e0130013b01b3023b01b3023b01b3023b01b3023b010001b301330100010e0130013b01b3023b01b3023b01b3013b01e00bee013001b301bb013b01e002ee03000cee010e013002bb0133013b01b3013b02bb013302bb0133013b010001b301bb0103010e013002bb013302bb0133013b01b3013b01e00aee010e01b301bb017701bb010311ee010e0130013b01b3023b01b3023b01b3023b01b3023b010001b301330100010e0130013b01b3023b01b3023b01b3013b01e00aee013001bb027701bb013b01e010ee010e0130013b01b3023b01b3023b01b3023b01b3023b013301b3023301000130013b01b3023b01b3023b01b3013b01e00aee013001bb017702bb013b01e010ee010e013002bb013302bb013302bb013302bb013302bb01b302bb0103013002bb0133013b01b3013b02bb013b01e009ee010e01b3017b03bb013b01e010ee010e010001b3013b010001b3013b010001b3013b010001b3013b010001b3013b013001bb013b020001b3013b0130013b01b3010301b301bb01030aee010e01b304bb013b01e010ee010e010001300103010001300103010001300103010001300103010001300103010001330103010e01000130010301000103013001000130013301e00aee013005bb013301e011ee020001e0020001e0020001e0020001e0050001e001ee09000bee013004bb013b010312ee010e01e001ee010e01e001ee010e01e001ee010e01e001ee010e01e001ee010001e002ee010e01e001ee01e0010e01ee010e01000cee013003bb013b013301e038ee010e01b302bb013b0133010339ee010e01b302bb0133010301e02bee030001e00aee010e01b301bb0133010301e02bee010e013302bb010b020008ee010e0133013b013301e02cee0130013305bb010007ee010e023301032cee010e023306bb020001e005ee0130010301e02cee010e013301b308bb010b010004ee010e01e02dee013001330bbb01e032ee013001b30bbb010b01e030ee010e013301b30cbb010b01002fee010e01330fbb01e02eee013001b30fbb010b2dee010e013311bb01e02cee013012bb010b2bee010e01b313bb01e02aee01300dbb013b013305bb010b29ee010001b30dbb0133017701b705bb01e027ee010e013307bb013301b304bb013b0173027705bb01e027ee013001b306bb0133017301b704bb013b0173027705bb010b26ee010e013307bb0133027704bb013b037701b704bb010b26ee010e01b306bb013b0173027701b703bb0133037701b704bb010b26ee013001b306bb013b0173027701b703bb0133037701b705bb01e024ee010e013307bb013b0173037702bb013b0173047705bb01e00cee010e010003ee010e04000eee010e013307bb013b047702bb013b0173047705bb01e00bee010e01b001bb010002ee0130013303bb01000dee013001b307bb0133047701b701bb013b0173047705bb01e00aee010e01b003bb01e0010e013301b304bb01000cee013001b307bb0133047701b701bb013b0173047705bb010b09ee010e01b004bb01e00130013302bb027701b701bb01000aee010e013308bb0133047701b701bb013b0173047705bb010b09ee01b002bb017b017701bb010b0130013302bb047701bb010009ee0130013308bb0133047701b701bb013b0173037701b705bb010b08ee010e02bb017b027701b7010b0130013304bb027701b701bb01e008ee0130013308bb0133047701b701bb013b0173037701b705bb010b08ee01b001bb017b017701b702bb01030130013301b305bb017701bb010b04ee020001ee010e023308bb0133047701b702bb0133037701b705bb010307ee010e02bb017701b703bb01030130013301b305bb017b01b701bb01e002ee010e02bb01e0010e023308bb013b047701b702bb0133037706bb010307ee01b001bb017b01b703bb013b0103010e023306bb017b01b7010b02ee013001bb01b7010b010e023308bb013b0173037701b702bb013b037706bb010301ee010e010003ee010e02bb01b704bb013b0103010e033306bb017b01bb01e001ee013001bb017b010b010e023308bb013b0133037703bb013b0173017701b706bb010301ee01b001bb01e002ee01b001bb017b05bb013301e001ee0100033301b305bb01b7010b01ee010e01b3017b010b010e023309bb0133037703bb013b0133017701b305bb013b0103010e017b01b7010301ee010e02bb01b704bb013b013301e002ee0100043304bb017b01bb01e001ee013002bb0100023309bb013b0173027704bb023306bb013b01e0010e01b701bb010301ee01b007bb0133010304ee0100043305bb010b01ee010e01b301bb010002330abb0133017701b70cbb013301e001b002bb0103010e07bb013b013301e005ee0100043304bb010b02ee013001bb010002330abb013b013301b30cbb013301e001b001bb013b01e001b006bb013b0133010307ee0100033301b304bb01e001ee010e01b30100023318bb013b013301e001b001bb0103010e07bb023301e008ee0100033301b303bb01e001ee010e01b3010b023301b317bb013b0103010e01bb013b01e0010e06bb023301000aee0100033303bb010b02ee0130010b0130013301b317bb01330103010e01bb010301ee01b005bb013b013301030cee0100033302bb010b02ee0130010b0130023317bb01330103010e013b01e0010e05bb013b023301e00dee0100023301b302bb01e001ee0130010b0130023316bb013b013301e001b0010301ee01b004bb033301000fee0100023301b301bb01e001ee010e010b0130023301b315bb0133010301ee01b001e0010e04bb0333010011ee0130023301bb010b01ee010e010b010e033314bb013b0133010301ee013001e001b003bb0333010012ee010e0130013301b301bb01e0010e0103010e033301b313bb023301e0010e010b01ee01b002bb0333010014ee010e023301bb01e001ee01b00100043312bb013b0133010301ee010e0103010e02bb013b0233010010ee020004ee0130013301bb01e001ee01b001e00130033301b310bb013b0233010302ee01e001b002bb0233010310ee010e02bb010003ee010e013301b3010b01ee013001e0013004330fbb013b033301e003ee01b001bb0233010301e010ee01b0017b017701bb010003ee01300133010b01ee010e01ee013005330dbb0433010303ee010e01bb013b0133010301e011ee01b002bb017701bb01e002ee010e013301b301e002ee010e063301b309bb063301e003ee01b001bb0133010301e012ee013004bb010b01e002ee013001b301e003ee0130083305bb013b0633010303ee010e01bb013b013301e001ee010e010010ee010e023301b302bb010b02ee010e0133010b03ee0130143301e003ee010e01bb0133010001ee010001b001bb010010ee02000130013301b301bb010002ee0130010b03ee010e01301233010304ee01b0013b010301ee010e01bb017b017701bb01e011ee010e0100013001b301bb01e001ee010e010304ee010e1133010301e003ee010e01bb010001e0010e01b003bb01b7010b13ee010e013001b3010b01e001ee013001e004ee0130103301e004ee01b0010b01ee010e01b005bb010314ee010e013001b3010b01ee010e05ee010e01300e33010005ee010e01e0010e01b005bb013b010315ee010e013001b301e007ee010e01300c33010008ee01b002bb0433010301e016ee010e0130010b08ee010e01300a33010008ee010e01bb0233040001e018ee010e01b301e008ee010e01300733020009ee01b0013302001eee0100010b09ee010e07000aee010e0133010021ee01b001e01aee0130010322ee010e010b1aee010e01e023ee01e0feeefeeefeeefeeefeeefeee39ee0100"

function hex_to_num(str)
	return ("0x"..str)+0
end

function load_rle()
	local index=0
	for i=1,#title,4 do
		local count=hex_to_num(sub(title,i,i+1))
		local col=hex_to_num(sub(title,i+2,i+3))
		for j=1,count do
   poke(index,col)
		 index+=1
		end
	end
end

function clear_sprsheet()
	reload(0x0,0x0,0x2000)
end

--
fe_view.start=function()
 _init()
 load_rle()
end

--
fe_view.update=function()
 
 if front_end_fade_in>0 then
  front_end_fade_in-=one_frame
 else
  front_end_fade_in=0
  
  if btn(4) then
   current_view=game_view
   current_view.start()
   level_goto(map_x,map_y)
   player_x,player_y=64,74
  end
 end
end

--
fe_view.draw=function()
 cls()

 local fade=max(0,front_end_fade_in-1)
 local ft=-fade/.5
 local scale=ft*ft
 
 for a=0,1,1/5 do
  local s,c=sin(a-t/8),cos(a-t/8)
  for r=4,100*(1-2*fade),2 do
   local q=r/25-t
   local x,y,size=64+r*c+r/4*cos(q),64+r*s+r/4*sin(q),1+r/12
   circ(x+1,y+1,size,10)circfill(x,y,size,1)circ(x,y,size,5)
  end
 end
 
 palt(14,true)palt(0,false)
 pink_color_check()
 spr(0,0,-128*scale+3+3*sin(t/one_frame/21),16,16)
 pal()
 palt()
 
 if front_end_fade_in<.5 then
  if (t%.4<.3)print_outline("press \142 to start!",64,92,7,1)
 end

 print_outline("guerragames 2017",64,120,7,2)
end

front_end_fade_in=1.5


--
game_view={}

--
game_view.start=function()
 clear_sprsheet()
 
 jump_button:button_init()
 shoot_button:button_init()

 player_reset()
 enemies_init()
 ebs_reset()
 level_init() 
end

--
game_view.update=function()
 if(not game_finished)game_time+=one_frame
 
 player_update()
 bullets_update()
 
 if level_transition<=0 then
  enemies_update()
 end
 
 ebs_update()
 exps_update()
 sbs_update()
 level_update()
end

--
function score_draw()
 print_outline("scr:"..player_score..(player_score==0 and "" or "0"),2,2,7,2,align_l)
 print_outline("deaths:"..player_deaths,127,2,7,2,align_r)
 print_outline("-"..time_to_text(game_time).."-",64,2,7,2)
end

--
game_view.draw=function()
 if game_finished then
  exps_draw()
  game_end_draw()
  score_draw()
  return
 end 

 cls()
 
 if level_transition>0 and level_transition<1 then
  level_draw()
  player_draw()
  score_draw()
  if(level_transition>.3 and level_transition%.3<.2)print_outline("game saved!",64,64,7,8)
  return
 end

 if screen_flash_timer>0 then
  cls(screen_flash_color)
  return
 end
  
  map(map_x,map_y,0,0,16,16,0x10)
 
  score_draw()
 
  enemies_draw()
  player_draw()
  bullets_draw()
  exps_draw()
  ebs_draw()
  sbs_draw()
  
  level_title_draw()
end

--
current_view=fe_view
current_view.start()

--
function _update60()
 t+=one_frame
 
 update_screeneffects()
 jump_button:button_update()
 shoot_button:button_update()
 current_view.update()
end

--
function _draw()
 current_view.draw()
end

__gfx__
00000000007770000000000008800000000bb0000000000000000000000bb000000000000000000000000000000000000000000000000000000000004aa99aa4
0000000007777700007770008aa8000000b3b30000ddd0000000000000003a0000000000000000000b000b0000000000000000000099940000099940aaaaaaaa
0000000007070700077777008aa800002b2832620dcc6d0000dddd0000009a600003bbb0000000000bbbbb000b000b000000000009999940009999944aa99aa4
0000000000777000070707000880000088888888dcccc6d00dccc6d000009aa0003bbbbb0003bbb00b8b8b000bbbbb0000000000099a9a400099a9a404999940
0000000000060000007770000000000088888888dcccccd0dccccc6d00009aa003bb7b7b003bbbbb00bbb0000b8b8b0000000000099999400099999400999940
0000000007776600000600000000000088888888d5ccccd0dccccccd0009aa903bbb7b7b03bb7b7b0b3b3b0000bbb00000000000009994000009994000999400
00000000006770000777660000000000288888820d5ccd00d5cccccdaaaaaa003bbbbbbb3bbb7b7b00333000003b300000000000949940009409940004944000
000000000660070000776000000000000288882000ddd0000dddddd009aa900003bbbbb003bbbbbb00b0b0000b333b0000000000099400000999400099400000
0009990000000000000000b3000000000777aa000000000000000000002800000008200007777700077777000000000000000000000000000000000000000000
00098800000999000000088600aa7000000700000000000000000000028000000000820066070660660706600c000c00ccccccc000000000000aaaa000000000
00099900000988000000858b00090000009aaa00009aaa0000000000288000676000882070070070700700700ccccc000c8c8c0000aaaa00000944a000000000
98889889000999000008888b009aaa0009a0a0a009a5a5a0009aaa00888267777762888070878070700700700c8c8c0000ccc00000944a00000924a000aaaa00
00099000000890000088858b09a0a0a009a0a0a009aaaaa009a5a5a02888877777888820777777707777777000ccc0000cdcdc0000924a000009999000944a00
0088889000999900088588b309a0a0a009aaaaa009a555a009aaaaa0028877777778820067070760670707600cdcdc0000ddd000009999000004400000924a00
9900000900088000b8888b3009aaaaa0009aaa0009a555a009aa5aa00002277777220000060606000606060000ddd00000c0c000022442200024220022999922
00000000000990003bbbb300009aaa0000000000009aaa00009aaa00006282777282600000000000000000000c000c0000000000022002200024220022444422
00000090000000000003b00000000000000067770000000000000000067288272882760000dddd00000000000000000000000800008eee800020000000002000
9009999909090090000032600006777000067877000000000550055007728882888277000dd1d1d000dddd00003bb0000000008008eeeee80280000000008200
900091910900999900dd02200067877000677787000550005665566506772882882776000dddddd00dd1d1d003b7b700008ee8088ee7eeee0280006760008200
09009999090091910020dd006777787000775776005dd500565dd565006762272267600000dddd000dddddd003b7b70008eeee008e777e7e2880677777608820
09999480090099990dd2d2000675776007567760055dc550505dc50500000777770000000006600600dddd0003bbbb008ee7e7e08ee7eeee8882777777728880
09999900099994800d2020007567760000657600566556650005500000007676767000006dddddd60006600003bbbb008ee7e7e08eeeeeee2888877777888820
940004900999940000dd00000656700006070000560000650000000000076070706700006000000000655d0003bbbb008eeeeee08eeeeeee0288677777688200
00000000094940000d2000000070600000000000500000050000000000060070700600000000000000060660003bb00008eeee0008eeeee00067227772276000
000000000d000000005555000000000070000070700000700677760006777600077777000000000000000000003bbb0000000000000008000677282728277600
0d000000d00dd0dd00588800005555006777776067777760677777606777776066070660000000000000000003bbbbb000000800000000800777288288277700
0d0dd0ddd00fdddf00555506005588006997996069979960799798707897997070070070003bb000000000003bb7bbbb00000080008ee8080677722722777600
d00fdddf0d0d1d1d06cccc6000555500789798707987897079879970799789707087807003bbbb00003bbb003b777b7b000ee80808eeee000067676767676000
d00d1d1d0dddfff060dddd0000cccc6677777770777777707777777077777770777777703bb7b7b003bbbbb03bb7bbbb08eeeee08ee7e7e00000077777000000
0dddfff00dddd07000cccc6606dddd0067676760676767606777776067777760670707603bb7b7b03bb7b7bb3bbbbbbb8ee7e7ee8ee7e7e00000767676700000
0dddd070f4004f006600000006cccc0007000700000700000606060006060600060606003bbbbbb03bb7b7bb3bbbbbbb8ee7e7ee8eeeeee00000707070700000
0f4f4000000000000000000000660600000000000000000000000000000000000000000003bbbb0003bbbbb003bbbbb008eeeee008eeee000000707070700000
0dddddd0ddddddd00999999009990990033033303303303011000010110000100100004100000550000100001001001000010001000100010000000000000000
d555555dd555d55d94444449944494493333133363333333001012d1001012d1144001240050511500050001000012d100051005000510050000000000000000
d522225d5222525d9422124942124249035365531355353310010120000101200220044200100011100000001d01012110005100100051000000000000000000
d520025d2000225d09414449411212297163163056513530d1100000000010001001022000000000501000002201001050100000501000000000000000000000
d52025dd0020025d9422124941114129761111671110115721000000000101000144010105500000005100102210100000510000005000100000000000000000
d520025d2252225d9411149042424249075116571511157710000000000001d04022404051100050000500501d0101d010051000000000510000000000000000
dd52025d55d5555d94211249449444497651157067517557000000000000d22d2444212111000010100000000001022d51000000000000050000000000000000
d520025dddddddd00942124999099990761101677077077010000000000001211222101000110000000000001010012100000000000000000000000000000000
d52025dddddddddd9421124999990999761011573303303300000000888888880100044100000550000000011100001000000000000000010000000000000000
d520025d5d555d55944224904444944476511567633633360d0000008d6d8d8d140002240050511000100005001012d100000000000000050000000000000000
dd52025d252225229421124942124242075155705535635122d000006d218d610000004200000000100001001001012010000000000000000000000000000000
d520025d020002000942444911012141716116175135151001000000d12d8d10000000200000000050000000d100100050100000000000000000000000000000
d52025dd000000009422124910001020705105671051001000010000211d60100000001000000000000000002101010000510000000100100000000000000000
d520025d00000000941114900000101076551670000510500d101000100202d0000000005000005010010000101001d010051001100510510000000000000000
dd52025d100010109421124900000010075115670100000022d010001201022d0000000011000010510000100001d22d51000005510000050000000000000000
d520025d011100000944144900000000761101570000010012100101021010210000000000100000000000001010012100000100000001000000000000000000
d52025dd0000055094211249010104417611116700010001000000011012d0100100000000000550100000000000000000000000000000000000000000000000
d520025d055051159442249014414224765015700005100500000001001022d11440000005505100500001000000000000000000000000000000000000000000
dd52025d01101111942112490220214207515657100051000000001012d102210220000001101000000000000000000000100040000000000000000000000000
d520025d000001100942444911011020705115675010000000000000222d01001001000000000000000000000000000000110020000000000000000000000000
d52025dd055500009422124904440111711011700051001000000010222d10d10144000005550000000000000000000010441110000000000000000000000000
d522225d5111505094211249422240400751155710051051000101d0122202d04022404051100000000000010000000040224041000000000000000000000000
d555555d111110109442244924412121765717575100000500d002211221022d2444212111000000010000050000000021412121000000000000000000000000
0dddddd00111000009999990122101100770777000000100122d0010021010211222101001000000000000000000000012210111000000000000000000000000
0ddddddddddddddd0999099909990999033033033303303388888888000000000000004100000550000005500000050001010001100004410000000000000000
d555d555d555d555944494449444944433133536633333368d6d8d8d000000000000012400505115005051150550510014414001100042240000000000000000
d5225222522252229422424242124242335633515603603661118161000000000000044200100111001011110110110002202001100021420000000000000000
d5202000200020009421212121122121311131111101563110108010000000000000022000000010000001100000010011011000000110200000000000000000
d5200020002000209421412111114121071011100111111100006000000000000000010100000000005500000555000004440100004401110000000000000000
d5222252225222529422424242424242751111511615610100001000000000000000004000000050011150505111505042224000002240400000000000000000
d55555d555d555d59444449444944494767107666667667600000000000000000004012100000010111110101111101024412100244121210000000000000000
0ddddddddddddddd0999990999099909070770777770770700000000000000000012101000000000011100000111000012210110122101100000000000000000
c500a4a6a6a6a4a600a400a4a6a4a6a4565656c556a656565656a6c55656c556d5d5a6d4a6a4c4c4d5c5d5d5d5c4d4c545d5565656565656565656565656c545
45a6565656a6a6c5a55656a4a6a6a44546a4d456d4d45656a4a45656d456a54646020000d456d4a4d5d456c400000246000000c500c50000d500d5c50000d500
a6b1d5c456a4a4c5b1d5a456a6b1d5a5a45656565656c40000d456d45656565656c5d452d5c5c55252d4d5c452d4d5564556565656c400100000d45656565645
455656a6d5a6a65656a6a4a5d5a6a445a4a5a45656a456a4a5a4a45656a4a4a4a5555544445656c4d5d45644445555a5c500d5d45656c5d5c45656c400d556d4
c5d5a5c400a600a400a600d0d456c4a4a5c45000d4565555555556c40050d4565656c5d5c4c5d5d4d4d5c5c4d5c4565645c40010d4555555555555c41000d445
4556a6a5a5d4a4a4a4c456a656565645a4a5c400005000d4c400500000d456d4a556d546465656c4d5d4d5464656a6a4c4a6565656d456a4d456c4c4d5d456a4
4444a400555555555555555500a4444456c5475400d456565656c4004754d5a557575754c5d5d4a6d5d5c5d54757575745575754565656565656565647575745
45c5a656a5d4a5a6a5a4565656c5a6455656a455555555a4a455555555a456565656d4d556c55644445656d55656a5d54455555556c500565600d55655555544
454556c500d00000a60000a4d556454556565647575400d4c40047575456565645d5565656c5c50202d5d5565656c54545d5565656c4d456c4d456565656c545
4555555555c4a60000a6d45555555545000010d456c400000000d456c4100000a5d5d5c40000024646020000d45656a5460000d5565656d456565656c5000046
454556c40055555555555500a4564545a4c55656564754d5564754565656565646565656565655555555565656565646455656c40202020202020202d4565645
46a4d5c40200101010100002d456a546555555565656555555555656a4555555a556564444555556565555444456d556d5d456555555c40000d455555556d4c5
46465656c5c5a60000a400d5a556464656565656c5565656a656a656d456c556d55656c400d456565656c400d456565646565656555555555555555556565646
a5a456565555555555555555565656a55656c400000000d4c400000000d45656d45656464656c450d4a5564646d556c4565656c50000009100000000d5565656
d5d556a5a5a54444444456a5a5a5d5d5a4c55656565656565656c4d4565656d45656565555555656565655555556565656c4d4d4c4565656565656c4d456d456
a456c556a5a5a556a4d5a456c556a5a6a5a5a555555555a5a555555555a5a5a54456c45656d556555556565656c456444455555656c500000000d55656555544
d456c4d0d4a54646464656c4d0d456565656c450d456c40202d456c450d45656c5d50010d5565656565656c510d5c500000000000000d45656c4000000000000
a45656c556a4a4a456a556a456a5a6a60010d4a4a4c400000000d4a556c4100046020000d4a5a556c5a556c4000002464600d5c5a4a5c50000d5565656c50046
55554444a5a4c5a400d556564444555557575754d5565555555556c5475757575757575456565656565656564757575757575754555555565655555547575757
c400d45656a65656a5a4a65656c400d455555656d4a5555555555656a4a4555556555544445656d556c5564444555556d55656555555560000565555555656c5
00d44646a4c4d000a400d4d54646c40057545656565656c4a65656565656475745d556565656c40202d456565656c54545d556565656c40000d456565656c545
445555d556445000a6504456565555440010d4c4000000d4c4000000d4c4100056a45646465656c4d4565646465656a556a456c50000560000560000d556c556
555500d5a400555555550056c500555556a4d456c400d45656c400d4c45656564656565656565555555556565656564645565656565655555555565656565645
45a556c4d4455555555545c4d4565645555556564455555656555544a4a45555a556d45656c456444456d456a4d4a400445555565600560000560056d4555544
00d55656a4c500a400a4d5d4a4c4c40056a4c456555555565655555556a6c556d55656c400d456565656c400d45656c5455656c400d456565656c400d4565645
45565655554600565600465555c556455656c40045a4c40000d4a44500d4a4a4a55656c40000024646020000d4a4d4a44600d55656d556c5d556c556a4c50046
5555a4c40000d5a456a40000d4005555a6565656d45656565656d456d45656565656565555555656565655555556565645565655555556565656555555565645
45c4d4565656565656a456a4a4c4d445a4a456444556a4555556564544a5a4d456d4564444555556565555444456c456d55656565555555656555555565656c5
c500a5475754c5a4c556475754c50000c40002d45656c40000d45656c40200d45656c40000d400c5c400d40000d4565645c40000d45656c4d45656c40000d445
455555a6a5c400a60000d4a4a55555450000d44545c400000000d44545c40000d4a4a546465656c4d456564646c500c456a4a556c500d55656c500d5c556a4a4
57575757575757575757575757575757575754d55656475757545656c547575756c4475757575757575757575754d45646575757545656475456564757575746
4556a656a6475757575754a65656a5455757575757575757575757575757575744565656c5565655555656c55656564444565655555555555555555555565644
0074b576647476b564b476766474b5b566767676b5767664007476767676b564b5767676b55076b5767650b57676b5b500747676b57664007476767664000000
00000000000000000000000000000000b5b57664107476b5767664107476b5b5b5767676b57676b5767676b57676b5b5b5b5b56464b5b564646466b5747474b5
a0007464a00074b5b4b47664a0007476767676b4b4b5766566b47676b4767665b57575757575757575757575757575b475757500747600000074640000757575
00757500007575000075750000757500b57676657575757575757575667676b4767676b4b4b576b5b4b4b5b5b4b476766767b564646474b574747474b5b56767
7575666575756664a00074767575666475757564b175757576b564b174757575b57650767676767676b57676507676b476766400517464000000640051747676
00746400007464000074640000746400b57676767664106474107476767676b47676b57575757575757575757576767664b5b564b574646464b5b56665b56464
7464b4667464746575756664746476b57676b465667676767675756676b576b5b57575757575757575757575757575b476747575757565000000757575757476
750065757566007575b4007575006575b57676757575756566757575757676b47575b576b5b4b4b5b5b5b4b5b4b575756467676774746474b564646667676764
7664a0007400a000746464a00074b57676b564b17476b464b17476b464b17476b5b5b4b5b476b476507676b476b4b5b476640074767676765166767676660074
64020074640200746400027464000274b5b5b4b564507476b564507476b4b5b476b5b4000074b4b5b5b5640000b4b57674b56464b57474747464646474656400
647475756676757566b5767575666474767575757676757566b5757575667676b5b4757575757575757575757575b46475757565747675757575767676757575
0075756600757566b5757500657575b4b5b5757575757575757575757575b564b57675756300b5b5b5b50063757576766767b57474656400747464b564b56767
76767464b4b47464a00074746464a00076767676b5b576b4b4b4b57676b5767674b47676b5b576b4b4b4b5507676b40064517476657464746474647464517476
0074640200746400007464000274640074b47664a07476b4b4b464a07476b400757576650000b5b4b4b4000066b5757565b5b5747464007100b464b5b5b56464
64a00074b5b576b47575647476b5757576b464b17464b174757564b174b5757500b4b475757575757575757575b4640000757575746474640000000075757564
7566007575b40075750065757500647500b4b465757576767676757576b4640076b4b4b5b5b5b4b5b5b4b5b5b5b476b5b56767676767650066646767676767b5
74757566647476b57464a00074b5746475757566766566b5b4b56566b5b576760074b4b576507676b4b57676b4b4650066767664007476645100006600747400
64000074640002746402007464000074007464a07476640000747664a0746500757575b4b5b564746474b4b5b5757575b5746464b56466b4b46466b4b465b5b5
76746464a00074766474757566b476b564007476b4767676b4767575766400740066b475757575757575757575b4b40075757574006675757575747665757575
b5757566b575750074757500007575640066b47575640000000074757566b40076767676b4b565666566b4b5767676b56767b4b466b47464666466b466666767
64b5647475756664a0007464767676766566b4767664007476767676766566b500b5b4767676b5767676507676b4b46500747664517474766474007451747674
007464020074640000746400027464000064a07464000000000000b464a07465757576767676b5b5b5b576767676757564b46464b466b46664b4666665666664
7464a0007464b5767575666474b4b5b5b4757575b4657575757564007575757666b5757575757575757575757575b4b400007575757500740000757575750074
7566007575006575757466757566007566b57575b400000000000074757566b4b476b476746474647464746476b4b47674676767b46666666466b46467676766
b576757566b4b576746464a000747676b476b476b4b4b47676b4656676767676b5b57676b4b4b45076b4b4767676b4b400667664007476000066766466640000
6400007464000074640000746400007464a0747665650000000000667664a074b47575640000000000000000747575766574646565b4667474666666b4656464
76647464747676b4b4b476757566767600747675757575b4b575757575766400b57575757575757575757575757575b475757565000075757575007474757575
65757564747575647475756400757564657575767665650000006676767575b4767676b56665666566656665767676b5676765b46665b466b4b4666665b46767
7666b4b466b4b476b5b564746474766466b4b4b5767664007476767676766566b57676b576767676b57676767676b4b474746474656664647474766566766474
00746400007464000074640000746400b57676b576767676767676767676b4b47676757575767676767676757575b4b564b4b464b465b466b46664b46566b464
67676767676767676767676767676767b4b4757575757575757575757575b4b4b57575757575757575757575757575b476757575667675757575767675757576
7500747575006475750074757500747575757575757676757576767575757575b4b5b5b576b475757575b476b47676b465676766b46667676767b4b466676764
__label__
11111111511111111151111111151111111111111115a1111111115a000000000000000000000000000000000000051111155111115515a00000000000000051
11111115111111111151111111115111111111111151a1111111115a000000000000000000000000000000000000005111511111111150a00000000000000051
1111111511111111151111111111511111111111115a11111111150a000000000000000000000000000000000000005115111111111115000000000000000051
1111115111111111151111111111155111111111551a1111111115a0000000000000000000000000000000000000000551111111111111500000000000000005
111111511111100005111110001111a55000115500a11111111000a00000000000000000000000000000000000000000511111111111115a0000000000000005
11111151111103333011110333011111033305103305511111033000003333333333000000000000000000000000555511111111111111150000000000000000
5111115111103b777300103777301110377730037730a5555037730003bbbbbbb77730000000000000000000000511151111111111111115a000000000000005
051111511103bbbbb733003bbb730103bbb73003bb7301aa03bb73003bbbbbbbbbbb730000000000000000000051111511111111111155555000000000000005
00551115103bbbbbbb7733bbbb730103bbbb733bbb73011103bbb7303bbbbbbbbbbb730000000000000000000555551511111111115511111550000000000051
000a555503bbbbbbbbbb73bbbbb30003bbbb733bbbbb30003bbbb733bbbb3333bbbb300000000000000000005111115511111111151111111115000000000051
00000aaa03bbb333bbbb73bbbbb30003bbbbb33bbbbbb333bbbbbb33bbb300003333000000000000000000051111111551111111511111111111500000000051
0000000003bb30003bbbb3bbbbb30003bbbbb33bbbbbbbbbbbbbbb33bbb300000000000000000000000000511111111151111115111111111111150055555051
0000000003bbb30003bbb3bbbbb300003bbbb33bbbbbbbbbbbbbbb33bbb300000000000000000000000005111111111115111115111111111111155511111551
0000000003bbbb330033303bbbb300003bbbb33bbbbbbbbbbbbbbb33bbb300000000000000000000000005111111111115511151111111111111151111111115
00000000003bbbbb3000003bbb30000003bbb33bbbb3bbbbb3bbbb33bbb300000000000000000000000005111555551115a55151111111111111511111111111
000000000003bbbbb301003bbb30050003bbb33bbbb33bbb33bbbb33bbb300000000000000000000000005115111115115aaa551111111111115111111111111
0000000000003bbbbb30003bbb30511003bbb33bbbb33bbb33bbbb33bbbb33330000000000000000000005151111111515a00051111111111115111111111115
00000000000003bbbb73003bbb30111003bbb33bbbb33bbb33bbbb33bbbbb7773000000000000000000000511111111150a00051111111111151111111111115
000000000000003bbbb7303bbb30111003bbb33bbbb303b303bbbb33bbbbbbbb7300000000000000000005111111111115000005111111111151111111111115
0000000000000003bbb7303bbb30111003bbb33bbbb3003003bbbb33bbbbbbbb3000000000000000000005111111111115a00005111111111150000011111115
0000000033300003bbbb733bb301100003bbb33bbbb3000003bbbb33bbbb33330000000000000000000005111111555555a00000511111110003bbb300111115
00000003b77300003bbb733bb301033003bbb33bbbb30a0003bbbb33bbb300000000000000000000000005111115111115a00000051111003bbbbbbbb3011111
00000003bbb730003bbbb33bb300377303bbb33bbbb3000003bbbb33bbb3000000000000000000000000051111511111115000000555103bbbb7777bbb301111
00000003bbb730003bbbb33bb303bbb733bbb33bbbb3000003bbbb33bbb300000000000000000000000000511511111111150000051a00bbbb77777bbbb30111
00000003bbbb30003bbbb33bb303bbb733bbb33bbbb3000003bbbb33bbb30000000000000000000000000005511111111115555505103bbbb77bbbbbbbb30111
00000003bbbbb3003bbbb33bb33bbbbb33bbb33bbbb3055003bbbb33bbb30000000000000000000000000000511111111151111155033bbb7bbbbbbbbbb30115
00000003bbbbbb33bbbb303bb3bbbbbb33bbb33bbbb3011003bbbb33bbb3000033330000000000000000000051111111151111111503bbbbbbbbbbbbbbb30551
000000003bbbbbbbbbbb303bbbbbbb3303bbb33bbbb3011003bbbb33bbbb3333b7773000000000000000000051111111511111111033bbbbbbbbbbbbbbb30aaa
000000003bbbbbbbbbb3003bbbbb330003bbb33bbbb30a1003bbbb303bbbbbbbbbbb730000000000000000005111111511111111033bbbbbbbbbbbbbbbb30a1a
0000000003bbbbbbbb30003bbbb3000003bbb33bbbb30a1003bbbb303bbbbbbbbbbb73000000000000000000051111151111111103bbbbbbbbbbbbbbbb330115
000000000033bbbbb300003bbb300000003b3003bb301a50003bb30003bbbbbbbbbb30000000000005550000005111151111111033bbbbbbbbbbbbbb33301115
0000000000003333300000033300000000030000330005500003300000333333333300000000000555115550000511151111111033bbbbbbbbbbbb3333011150
0000000000000330003003300330000330003000003330510003305003300003330000000000005551511515000055551111111033bbbbbbbb3333333011150a
0000000000003b7303733b733b73003b7303730003bb7305103b73003b73003bb730000000005511155515a150000aaa51111110333bbbb333333330011150a0
000000000003bbb733b33bb3bbb733bbb733b3003bbbb73003bbb733bbb733bbbb73000000005111115aa5a15a00000000011111033333333330000555550a00
000000000003b33bb3b33bb3b33bb3b33bb3b3003b33330103b33bb3b33bb3b33bb30000000511115555a1a15a000003333011115033333300050a00aaaaa000
000000000003b33bb3b33bb3b33bb3b33bb3b3003b33001003b33bb3b33bb3b33bb300000005115511155a151a50033bbbb305551a0000001150a00000000000
000000000003bbbb33b33bb3bbbb33bbbb33b3003bbb301003bbbb33bbbb33b33bb300000005115111115a51a1503bbb77bb30aaa1555555550a000000000000
000000000003b33bb3b33bb3b33bb3b33bb3b3003b33001003b33bb3b33bb3b33bb3000000005511111555aa1103bb7777bbb301111aaaaaaaa0000000000000
000000000003b33bb3b33bb3b33bb3b33bb3b3333b33330003b33bb3b33bb3b33bb3000000005511155111551503bb77bbbbb3011115a0000000000000000000
000000000003bbbb33bbbb33bbbb33bbbb33bbbb3bbbbb3003bbbb33b33bb3bbbbb300000000051115111115503bb7bbbbbbb3011150a0000000000000000000
0000000000003bb3003bb3003bb3003bb3003bb303bbb300003bb303b33b303bbb3000000000005151111115103bbbbbbbbbb301155a00000000000000000000
000000000000033000033000033000033000033000333000000330003003000333000000000000555111115103bbbbbbbbbb330550aa00000000000000000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000a5111115103bbbbbbbbb330aaaa0000000000000000000000
000000000000000000000000000000000000000000000000000000051011100000000000000000000511115103bbbbbbb33300a0000000000000000000000000
00000000000000000000000000000000000000000000000000000000511150a00000000000000000055111503bbbbbb333305a00000000000000000000000000
0000000000000000000000000000000000000000000000000000000055555a00000000000000000000a555103bbbbb333000aa00000000000000000000000000
000000000000000000000000000000000000000000000000000000005100000000000000000000000005aaa03bbb33300aaa0000000000000000000000000000
000000000000000000000000000000000000000000000000000000055033bbbbb0000000000000000055511033b3330000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000510333bbbbbbbbbb0000000000051115503333300000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000503333bbbbbbbbbbbb00000000051115aa0330000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000550333bbbbbbbbbbbbbbbbbb000551115a00000000000000000000000000000000000000000
0000000000000000000000000000000000000000000000000000550333bbbbbbbbbbbbbbbbbbbbbb015550a00000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000055033bbbbbbbbbbbbbbbbbbbbbbbb00aaa000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000050333bbbbbbbbbbbbbbbbbbbbbbbbbb000000000000000000000000000000000000000000000
50005555500000000000000000000000000000000000000000000033bbbbbbbbbbbbbbbbbbbbbbbbbbbbbb000000000000000000000000000000000000000000
1555111115000555550000000000000000000000000000000000033bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb00000000000000000000000000000000000000000
111511111150511111500000000000000000000000000000000033bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb0000000000000000000000000000000000000000
1111511111151111111500000000000000000000000000000003bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb000000000000000000000000000000000000000
11115a111115a11111115555500055500055500000000000003bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb00000000000000000000000000000000000000
111115111115a11111115a1115051115051115055500000003bbbbbbbbbbbbbbbbbbbbbbbbbbb333bbbbbbbbbbb0000000000000000000000000000000000000
111115a11115a11111115a111151111151111151115000003bbbbbbbbbbbbbbbbbbbbbbbbbbb33777bbbbbbbbbbb000000000000000000000000000000000000
111115a55115a11111115a11111511115a11115a11155033bbbbbbbbbbbbbb333bbbbbbbbbb3377777bbbbbbbbbb000000000000000000000000000000000000
111115a11551a11111115a111115a1115a11115a1115033bbbbbbbbbbbbb33377bbbbbbbbbb3377777bbbbbbbbbbb00000000000000000000000000000000000
111115a11115111111151a111115a1150a11151a111033bbbbbbbbbbbbbb337777bbbbbbbbb37777777bbbbbbbbbb00000000000000000000000000000000000
111151a1111151111151a1111115a550a05550a111503bbbbbbbbbbbbbb33777777bbbbbbb337777777bbbbbbbbbb00000000000000000000000000000000000
11115a1111115a55551a11111115a15a000aaa0555033bbbbbbbbbbbbbb33777777bbbbbbb337777777bbbbbbbbbbb0000000000000000000000000000000000
11151a11111115aaaaa111111151a11500000000a033bbbbbbbbbbbbbbb337777777bbbbb33777777777bbbbbbbbbb0000000000000000000000000000000000
15500000000015a511151111151a1111500000000033bbbbbbbbbbbbbbb377777777bbbbb33777777777bbbbbbbbbb050000000000000000000000000bbb0000
510333bbbbbb00a51111555551a111115a000000033bbbbbbbbbbbbbbb33777777777bbbb33777777777bbbbbbbbbb0a50000000000000555555500bbbbbbb00
a0333bbbbbbbbb0055511aaaaa1111115a000000033bbbbbbbbbbbbbbb33777777777bbbb33777777777bbbbbbbbbbb05a0000000000051111100bbbbbbbbb00
0333bbbb77777bbb00155151111111115a00000033bbbbbbbbbbbbbbbb33777777777bbbb33777777777bbbbbbbbbbb05a00000000005111110bbbbbb777bbb0
0333bbbb77777777bb005a51111111115a00000333bbbbbbbbbbbbbbbb33777777777bbbb3377777777bbbbbbbbbbbb05a0000000555551110bbbbb777777bb0
0333bbbbbbbb77777bbb05a5111111155a00000333bbbbbbbbbbbbbbbb33777777777bbbb3377777777bbbbbbbbbbbb0aa000000511111510bbbb7777bbbbb30
03333bbbbbbbbbbb77bbb0a5511111000000003333bbbbbbbbbbbbbbbb33777777777bbbbb337777777bbbbbbbbbbb300a00000511111110bbbb777bbbbbbb30
03333bbbbbbbbbbbb77bbb05155550bbbb00003333bbbbbbbbbbbbbbbbb3777777777bbbbb33777777bbbbbbbbbbbb30a00000511111110bbbb77bbbbbbbb330
a03333bbbbbbbbbbbbb77bb011aa03bb7bb0003333bbbbbbbbbbbbbbbbb3377777777bbbbbb3777777bbbbbbbbbbbb3000000051111110bbbb7bbbbbbbbbb330
a0333333bbbbbbbbbbbbb7bb011103bbb7b0003333bbbbbbbbbbbbbbbbb333777777bbbbbbb337777bbbbbbbbbbbbb30000bbb0555510bbbb7bbbbbbbbbb3301
a0003333333bbbbbbbbbbb7bb011103bb7b0003333bbbbbbbbbbbbbbbbbb33777777bbbbbbb333773bbbbbbbbbbbb33000b77b301110bbbb7bbbbbbbbbb33301
a0510033333333bbbbbbbbb7bb011103bbbb003333bbbbbbbbbbbbbbbbbbb3377777bbbbbbbb3333bbbbbbbbbbbbb300007bbb30110bbbbbbbbbbbbbbb333011
0051110033333333bbbbbbbbbbb011103bbb003333bbbbbbbbbbbbbbbbbbbb33777bbbbbbbbbbbbbbbbbbbbbbbbb33000bbbbb3010bbbbbbbbbbbbbbb3330155
000511110033333333bbbbbbbbb0555a03bb003333bbbbbbbbbbbbbbbbbbbbb3333bbbbbbbbbbbbbbbbbbbbbbbbb33000bbbb3010bbbbbbbbbbbbbb333301111
0055111111003333333bbbbbbbbb0aaa003b003333bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb333000bbb3010bbbbbbbbbbbbbb3333011111
005151111111003333333bbbbbbb05a0003bb033333bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb330a0bbb30110bbbbbbbbbbbb333300111111
0511155111115500333333bbbbbbb0a00003b003333bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb333050bb30110bbbbbbbbbbbb3333011111111
a51111a555551aa500333333bbbbb0000003b0033333bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb333010b30110bbbbbbbbbbb333330111111111
55111111aaaaa115a00033333bbbbb000003b0033333bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb333010b30050bbbbbbbbb333333001111111111
5a11111111111115a0000033333bbb000000b00333333bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb3330110b0500bbbbbbbb33333300111111111111
5a11111111111115a00000033333bbb00000b000333333bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb333301103050bbbbbbb3333330015111111111111
5a51111111111150a000000003333bbb000030003333333bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb33330110b0550bbbbb333333001515115555511111
5a5111111111115a00000000003333bb00000b0033333333bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb3333051103050bbbbb3333300511151551111155111
1a151111111115000000000000033111111111111111111111bbbb1111111bbbb111111111bbbb11111111111111111111111b333330115a1115111111111511
a1115511111550bbbb00000000003177717771777117711771bbb117777711bbb177711771bbb1177177717771777177711713333001115a1151111111111151
a1111a55555a0bb777bb00000000017171717171117111711133b177111771bbb117117171bbb1711117117171717117111713300111115a1511111111111115
1111111aaaaa0bbbbb77bb0000000177717711771177717771333177171771bbbb1711717133317771171177717711171b1710011155555a1511111111111115
5111111150a003bbbbbbbbb000000171117171711111711171333177111771bbbb17117171333111711711717171711713111111100011155111111111111111
151111150a000033333bbbbbb00001713171717771771177113331177777113333171177113331771117117171717117131711000bbb00115111111111111111
15555550a00000000003333bbb0001110111111111111111133333111111133333111111133331111311111111111111101110bbb777bb015111111111111111
50aaaaaa00000000000000033bbb0000003000000000003333333333333333333333333333333333300000000000bb0000000bbbbbbb7bb05111111111111111
0a0000000000000000000000033bb00000030000000000033333333333333333333333333333333300000000000bb000000bbbbbbbbbbb305111111111111111
a0000000000000000000000000033bb000000000000000000333333333333333333333333333330000000000000000000bbbbbbbbbbbb3301511111111111115
0000000000000000000000000000033b000000000000000550033333333333333333333333330000000000000000000bbbbb3333333330011511111155555115
00000000000000000000000000000003b0000000000000051150033333333333333333333300000000000000000000bb33330000000001111551115511111550
000000000000000000000000000000003b00000000000051151110033333333333333300000000000000000000000b3300000005111111115115151111111115
0000000000000000000000000000000000b000000000005115111110000000000000001150000000000000000000330000000000511111151111511111111111
00000000000000000000000000000000000b000000000555551111111115a111115a11115a000000000000000003300000000000051111511115111111111111
000000000000000000000000000000000000b00000005111115111111115a111115a11115a000000000000000000000000000000005555511115111111111111
000000000000000000000000000000000000000000051111111511111115a111151a11115a000000000000000000000000000000000aaa511151111111111111
000000000000000000000000000000000000000000511111111151111151a11151a111115a500000000000000000000000000000000000511151111111111111
00000000000000000000000000000000000000000051111111115a11150a55551a1111151a5a0000000000000000000000000000000000511151111111111111
00000000000000000000000000000000000000000051111111115a5550a0aaaaa1111151a15a0000000000000000000000000000000000051151111111111111
00000000000000000000000000000000000000005555511111115aaaaa0000005555551a115a0000000000000000000000000000000000005151111111111111
00000000000000000000000000000000000000051111151111115a511155000051aaaaa1115a0000000000000000000000000000000000000515111111111155
00000000000000000000000000000000000000511111115111151a111115a00055111111155a0000000000000000000000000000000000000055111111111511
0000000000000000000000000000000000000511111111151151a111111155555151111151aa000000000000000000000000000000000000000a511111115111
00000000000000000000000000000000000051111111111155aa511111551115511555551a5a0000000000000000000000000000000000000000051111151111
0000000000000000000000000000000000005111111111115aa05555555111515111aaaaa15a0000000000000000000000000000000000000000005511151111
0000000000000000000000000000000000005111111111115a0551111155115115111111150a0000000000000000000000000000000000000000000a55511111
0000000000000000000000000000000000005111111111115a511111111151555551111150a0000000000000000000000000000000000000000000000a511111
00000000000000000000000000000000222222222222222222222222222222222222222222220002222222222222222200000000000000000000000000511111
00000000000000000000000000000002277272727772777277727772277277727772777227720002777277727722777200000000000000000000000000511111
00000000000000000000000000000002722272727222727272727272722272727772722272220002227272722722227200000000000000000000000000511111
00000000000000000000000000000002722272727722772277227772722277727272772277720002777272722720027200000000000000000000000000051111
00000000000000000000000000000002727272727222727272727272727272727272722222720002722272722722027200000000000000000000000000051111
00000000000000000000000000000002777227727772727272727272777272727272777277220002777277727772027200000000000000000000000000005111
00000000000000000000000000000002222222222222222222222222222222222222222222215002222222222222022200000000000000000000000000000511
00000000000000000000000000000000005555511111111111115a111111151a11111115a1111500000000000000000000000000000000000000000000000055
00000000000000000000000000000000551111155111111111115a11111115a111111115a11115a000000000000000000000000000000000000000000000000a

__gff__
008080000080800080808080008080808080000000808080008080808000000080800080808080000080808080808000808000008080808080808080808000001b1d1b1d1b1d10101010101010101e1e1a111a111a1110111010101010101e1e1e101e101e1010101010101e101e1e1e17151715171511001010101010101e1e
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
__map__
0000707171717171717171717141000050000049490000000059494959495950500000494900000000594949594959505069004949000000005949495949595050690049496900590059494959495950506900494969000079597969594979505000004969796900795949795949595040717171717171717171717171717140
4070416161590000005959616170414050495961614949004961614961495950505961616161614949616161616149505061614969494961617949494961615050616161616161616169616161616150506161616161590059616161616161505059616161616161616161596161615050616161616161616161616161616150
5061616161614900594961616161615050616159590d594961490d59596149505059014961616a0159496161490159505049496900000029290000007949495050007961690a796161690a7961590050500100796161615949496161690001505061616159616901796161616161615050496161616149595949616161614950
50616149616159494961614959496150505961497b51515151515149594961505051515151497b5151617b5151515150506949697b51515151515149494979505071717171417a61617b70717171715050717171417a616161617b707171715050690100007970717141690000017950507b5151517a7b5151497b5151517a50
5049593059496161616149593059495050515151496149595949617b51515150506a596159495961306a59596161595050694949490079616149007979494950505979796161616930796161616159505059496161617b51517a61496149595050415151517a616161617b515151705050616161616161595949496161616150
50707171414961616161497071714150506149006149590d0d5949616161495050515149597b51515151615959515150504969515151510000515151517961505059616161617b51517a61616161615050496161616161495949616161614950500079616159690d0d79615961610050507b51517a7b51515151497b51514950
5049616161617b51517a6161616161505059615951515151515151514949595050616159405961616161614059616150506900000000290000290000000079505061690a79693079614961690a7900505069000a797b515151517a690a007950507b5151517071717171415151517a5050596161614959365949616161495950
506161616161493059496161616149505051514959616161596161617b51515050515151605151596a5151605151515050515151494951515151497b51515150507171417a7b515151517a7b70717150507171417a6149615961617b70717150504969007961616161616169007961505051517a61495151515149497b515150
5059616161707171717141616161595050596151515151597b515151514949505061616161617b5151616161616149505061616900290079690029007961615050596161616179616930796161615950507171415151517a7b51515170717150504151517a6169000079617b5151705050616159596161496161495959496150
50494961616161494949616161494950505949615949616161496149614900505051516159496161615961617b5151505049497b515151497b51515149494950506161617b5151515151517a61616150505949616169000d0d007961614959505049616161000d00000d007961614950507b5151517a7b51517a7b5151514950
505151517a4949593059497b51515150505151490d5949614959590d595151505059306a494940616140616a59306a505069007961690000000079614900795050690a796169306161616179690a79505049616900707171717141007961495050617b515170717171714151517a615050596161594959615961615961615950
506161497b5151515151517a61496150504961615151515151515151616149505051515151515049615051515151515050515151497b51515151497b515151505071417a617b515151517a617b70715050617b51517a614959617b51517a61505000007961616100000061616900005050497b5151517a61617b5151517a5950
5049305959496161616149595930495050590d495949616161616161490d005050615930596150616150494930596a5050616929000079616169000029796150505979615961616159616161616159505061690a00796161496161690a0079505051517a7a5961496161497b7b51515050595949616161616161616161495950
5051515151517a61497b5151515151505051515151514959497b5151515151505051515151516061496051515151515050617b51515151497b5151515149615050617b5151517a61617b5151517a695050404040404079616169404040404050505930796161615949616159693059505051517a7b5151497b51517a7b515150
6070416161494959005959496170416050594961495959004949614959494950506a6a616161595961496149596a6a505069000000007969000000000000795050007961796161795961617969005950507171507150596159595071507171505070717171416900797970717171415050494961495959594959594949614950
0000707171717171717171717141000060717171717171717171717171717160507171717171717171717171717171505071717171717171717171717171715050717171717171717171717171717150507171606060717171716060607171505071717171717171717171717171715050717171717171717171717171717150
5273737373737373737373737373735252737373737373737373737373737352527373737373737373737373737373525273737373737373737373737373735252737373737373737373737373737352527373737373737373737373737373525200787d63636800786358635800005263634858635863634858635863635863
5263680058007863486858000058785252636358234863484868582348586852522558635863486348584848485825525258784848637c587858634863635852524858635858585863637c58587d63525273737373437d7d78586c587273735252207d635863636c636363636800205263637c535353537d7c535353537d4848
52486363687848637d635830307863525268484858486878636363686363635252586363636363636348636348686c52526363636358305863486363583078525263636368017d635863582558636352527373437d6c63636363636c6c727352525353580578586363637d586853535263486363580000000000005863634863
52535353535342425353535353537d52525353534242426368424242426363525263635872435353537d48635848485252537d7c42535353535353535353535252637c72737373437d637c7d635868525273437c6358635801586363636c72525263687c535353537d586c05786363524253537d7c535353537d585353535342
5258786368305252683058636363635252634863626262686362626262634852526363486363634863584858487c7d52525863635258635858055858786348525278634848637c7273437d636363685252436c63237c535353535353236c585252580078636c63637c535353537d635262586358000000340000000058635862
52535353535362527c53535353535352525353536363634848786363634858525258486348587c53535372437d635852527c537d5263637c535353537d636352520a586363586363487c53537d635852527d586363636c486c48686c6363635252635863636368636368636c7863585263637c535353537d7c535353537d5863
52685800307878526363636368305852522358486363487858787858485823525225586363636348634863636358255252634848526358305863635830587852525353537d6325686378486363580a5252637c5353537d637d7c5353537d585252636c63786c6c636358580a7c53535263486800000000000000000000786368
52535353535353624242535353537d52524848727373435878727343535353525268487c72435353537d584863486c5252637c535253535353535353535353525263636363687d634863637c72737352520a5868486c630158636c4868580a52526378780a58587c535353537d636c5242535353537d7c535353537d58535342
52687858005800305252683058636352524863727373436878727343636378525258636348636348636348637c636352525863635258635863580558786363525263637863787273437d635872737352525353537d7c7c53537d7d6353535352526325535353537d63686c786325635262586800000000000000000000786362
52535353535353536252685353535352526358585848636363634858535353525258635848587c53535372437d485852527c537d5263637c535353537d58585252587c5353537d637c53537d7c7273525268486c63636363486363636c486c5252786363686c785863635863636c635263637c535353537d58535353537d6358
52686348636830784852587868307852526878235863685878484863784858525225586363634863584863635858255252684858526363635830305863587852520a586363635858636358255868585252637c5353537d637d7c5353537d58525253537d63580a78637863636363635268636368000000000000000078636863
52535353535353535362424253537d52525353534242425863424242424858525268487c72435353537d63487d58785252537d63525353535353535353535352525353537d6363636358637d78636352527d6323686c635801586c236c7d4252527d6c7c53535353780a5858637d63524253537d7c535353537d7c5353535342
526863487d63637d6830525200786352525848586262626378626262625823525263636348486348485848486348485252636863626348685805587878486352526358636342426363636363636358525242587c5353535353537d6348425252526363636c6c6c7d7c535353537d635262586348680000000000007863635862
52535353535353535353626278637d52525353536348634863634863634858525263635848587c53535372437d586352527c537d7c63637c535353537d63635252686378425252687d424268587842525252427c686c486c6c487d5842525252527858586363636363686c7d7c53535263637c535353537d58535353537d6363
5263636368007d6363636368007d7d525268587863634868584863786858785252684848584858486358634863637c5252586348486358635858484863635852524242425252527c7d52524242425252525252425863635863484242525252525258007d58635863636358635800585263585863635863585863586363486358
6273737373737373737373737373736252737373737373737373737373737352527373737373737373737373737373525273737373737373737373737373735252626262626262535362626262626252526262625353535353536262626262525200007273737373737373734300005242535353537d7c535353537d7c535342
__sfx__
00010000197701b7602076020760237601f760167600e770097600675004720027100270002700017000170002700017000170001700067000570005700057000570006700067000670007700047000470004700
00010000157701a7601c7601f7601f760207601c76020770237602475025760217501e7502c730277301f71002700017000170001700067000570005700057000570006700067000670007700047000470004700
010300001a7571b7671d7771e7771f77720777207772077720777207771f7671d7571c7471c7371b7371a7271a7171971719707197071e7071d70705707057070570706707067070670707707047070470704707
00040000313252e3352c3552936526375233751f3751d3751a3751636513345103450d3350b3350b3250b3250d31511315183251e335253452a3552e3652f3752f3752d3652a34525325233051e3051a30517305
01010000155401a5401c5501f5501f550205501c55020550235502454025540215301e5302c520275101f51002500015000150001500065000550005500055000550006500065000650007500045000450004500
000500001e5711d5711957115571105710d5510b5410a5310a5210a5110a5110a5110a5110b5110d5210e5211153114531185311b5411e55122551235612657127571295712a5712c5612d5512e5412e53117500
00010000157701a7601c7601f7601f760207601c76020770237602475025760217501e7502c730277301f71002700017000170001700067000570005700057000570006700067000670007700047000470004700
00010000157701a7601c7601f7601f760207601c76020770237602475025760217501e7502c730277301f71002700017000170001700067000570005700057000570006700067000670007700047000470004700
00010000157701a7601c7601f7601f760207601c76020770237602475025760217501e7502c730277301f71002700017000170001700067000570005700057000570006700067000670007700047000470004700
010e00000e42611426154261a4261142615426184261d4260e42611426154261a42613426174261a4261f4260e42611426154261a42615426184261c426214260e42611426154261a4261142615426184261d426
010e000026455264551a4051a405294551d40529405294552645529455264552b4052b4552b4051a4052b455264551d405264552d4552f405294052d455264552845529455264552940529455294552845529455
010e000026333263031a3031a3032863328603263031d30326333263332130326333286331d3031a3031d303263332630326333263332863321303283032830326333263032b3032633328633286032863328633
010e00002d4552d4552b4552d455294552645529405264552b4552b455294552b4552845524455264052445524455264552645528455264552b4552b455264552845529455264552940529455294552845529455
010e000026552285522f502305522f5522f5522950228552265521a5022d5522850228552305022f5522d552265522f5522850230552285022f55229552285522655226552305022855230552295022f55229552
010e000026554265542655426554285542855428554285542655426554265542655429554295542955429554265542655426554265542b5542b5542b5542b5542655426554265542655429554295542955429554
010e00000e0520e0020e052100520e0520e0020e052110520e0520e0020e052100520e052150020e052130520e0520e0020e052100520e0520e0020e052150520e052130020e052100520e0520c0521005211052
010e00000e0520e0520e0520e05213052130520e0520e05211052100520e052130520e0520e0520e0520e0521305213052150520e0520e0521305213052150520e0520e0520e052130520e052110520e0520e052
010e00003255532505325553250537555375053255532505355553455532555375553255532505325553250537555375553955532555325553755537555395553255532555325553755532555355553255532505
010e000032542325423c54232542325423254237542325423b5423254232542395423254232542375423254232542325423c5423254232542325423554232542395423754235542345423b542395423754235542
010e000032745327453b745327453274539745327053b70532745327453974532745327453b745327053270532745327453574530745307453474532705357053074530745347453074534745357453474532745
010e00001a2221d222212221d2221a2221d222212221d222182221c2221f2221c222182221c2221f222182221d2222122224222212221d2222122224222212221c2221f222232221f2221c222232221f2221c222
010e00001a3551a3051a3051d3551d305213551d3551a30518355213051a3051c3551d3051f3551c3551c305153551d3051d305153551a30511355103551d3051335515355113551335510355113550e35510355
010e000026333263031a303263332863328603263332630326333263032633326333286331d303263331d303263332630326303263032863321303263332633326333263032b3032633328633286332863328633
010e0000265512655526555265552f55529551285552d55526555265552f5512d55528555265552d555245552655126555265552d555285552b5552655126555305552b5552f555295552d55126555285552b555
010e0000265542655526554265552655426552265522655529554295552955429555295542955229552295552b5542b5552b5542b5552b5542b5522b5522b5552d5542d5522d5522d5552d5542b5522955228555
010e0000263312633526335263352f331293352d33526331263352633526335263352d331283352f3353033528331293352f335283352d335263312633526335303352f3352d335293352d335263313033526335
010e00000e3050e3750e3751d3051d305133751337513305153750e3050e3050e3750e30515375153751c305173751d3051d3050e3751a30513375133751d305153750e305133050e3751130515375113751a375
010e000032035320323b0053203532032350353703535035390353903234005390353903237035350353703535035350323b00535035350323703534035350353403535035370353903539032370353503534035
010e000026025280252d0252602526025280252d0252602526025280252d0252602526025280252d0252602528025280252d0252f02528025280252d0252f0252802528025290252d0252802528025290252d025
010e00000e14210142181420e14218142171420e1421714213142151421d142131421d1421c1421a1421c1421c1421f142211421c142211421f1421d1421f1421a1421d1421f1421c142211421f1421d1421c142
010e000026333263031a3031a3032863328603263031d3032633326303263032633328633263331a3031d30326333263032633326333286332130326333263332633328633263332633328633286332863328633
010e000026333263031a3031a3032863328603263031d3032633326303263032633328633263331a3031d30326333263032630326333286332130326333263332633328633263332633328633263032630326303
010e0000095550c55510555095550c55513555105550c555105551355517555135551755518555175551355511555155551855515555185551c5551855515555075550b5550e5550b5550e555115550e5550b555
010e00001535517355153551530515355133551130517355113551035518355173050e35511355113051535517355153551535515305173551530515355133551535510355113551030515355173551535515305
010e00002d33530335323352d33530335323352d3352b3352d33530335323352d33530335323352d3352f3352d33530335323352d33530335323352d3352b3352d33530335323353533537335353353433532335
010e0000095420b5420c5420b5420e5420c5420b54209542095420b5420c5420b542105420e5420c5420b542095420b5420c5420b5420e5420c5420b5420954209542105420e5420c54211542105420e5420c542
010e0000092550b2550c2050b2550e2550c2050b25509255092550b2550c2550b255102050e2550c2550b255092550b2550c2050b2550e2550c2050b2550925509255102550e2050c25511205102550e2550c255
010e0000095550c55510555095550c55513555105550c555105551350517505135051750518505175051350511555155551855515555185551c5551855515555075550b5050e5050b5050e555115050e5050b505
010e00002633326303286032633328633286032860318553263332630326303263332863326333185533050326333263032633326333286332860328633185532633328633263332633328633263031855330503
001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010c00000c4000c4000c4000c4000c400180000c400000000c400000000c400000000c400000000c400000000c400000000c400000000c400000000c400000000c400000000c400000000c400000000c40000000
010c00000c4000c4000c4000c4000c400180000c400000000c400000000c400000000c400000000c400000000c400000000c400000000c400000000c400000000c400000000c400000000c400000000c40000000
__music__
01 490b6644
00 094c0b66
00 090a0b66
00 090c0b44
00 090d0b44
00 090e0b44
00 090f0b44
00 09100b44
00 09110b44
00 09120b44
00 09130b44
00 14151644
00 14171644
00 14181644
00 14191644
00 141a1644
00 141b1644
00 141c1644
00 141d1644
00 20611f44
00 20211f44
00 20221f44
00 20231f44
00 20241f44
00 20641f44
02 60255f44
03 26424344
00 49424b44
00 41424344
00 41424344
00 41424344
00 41424344
00 41424344
00 41424344
00 41424344
00 41424344
00 41424344
00 41424344
00 41424344
00 41424344
01 490b6644
00 094c0b66
01 090a0b66
00 090a0b66
00 090c0b44
00 090c0b44
00 14151644
00 14151644
00 14171644
02 14171644

