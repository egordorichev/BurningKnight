pico-8 cartridge // http://www.pico-8.com
version 19
__lua__
time_max=600--450--900
force={}
paliers={2,5,10,15}
pool_start={2,3,4,5,6,7,11,12}
force_nxt=nil

function _init()
 t=0
 hisc=0
 power=0
 logs={}
 ents={}
 --init_game()
 init_menu()
end

function mod(n)
 return (t%n)/n
end

function init_menu()
 sfx(1)
 vl=0
 
 t=0
 ents={}
 upd=function()
  t=t+1
  if vl>32 then
   init_game()
  end
 end
 
 dr=function()
  pal(8,13)
  if t>36 and vl==0 then
   n=max(40-t,0)
   cl= n==0 and 7 or 1
   line(36,7-n,91,7-n,cl)
   line(36,27+n,91,27+n,cl)
  end
  if t>40 then
   pal(8,sget(16+min((t-40)/2,7),2))
   
   cx=64
   cy=80

   for i=0,15 do
    an =i/16
    c=cos(i/16+t/50)
	   r = 30+c*12+max((70-t)*4,0)+vl*2
    ray=6+c*3-vl/2
    cl=sget(22-c*6,2)

    circ(cx+cos(an)*r,cy+sin(an)*r,ray,cl)
   end

   
   if mod(vl>0 and 2 or 16)<.5 and vl<24 then
    print("press <x> to start",28,120,8)
   end
   if vl>0 then
    vl+=1
    
   elseif btnp(5) then
    vl=1
    sfx(6)
    music(-1,1000)
   end
   
  end  
  
  hdy=min(t*6-712,0)-vl*vl

  camera(0,hdy)
   --
   for x=32,96 do for y=48,100 do
    pix=pget(x,y)
    if pix>0 then
     for i=0,2 do pix=dark(pix) end
     pset(x,y,pix)
    end
   end end
   --
   print("--objectives--",37,50,13)
   for i=1,4 do
    cl=1
    y=52+i*8
    pal()
    if power>=i then
     apal(2)
    end
    sspr(16,3,4,5,38,y)
    print(gdd(paliers[i]),44,y,7)
    pal()
    if power<i then apal(1) end
    print(powers_str[i],56,y,14)
   end
   pal()
   print("hi-score:",38,94,13)
   print(gdd(hisc),80,94,7)
   sspr(16,3,4,5,74,94)
   
  camera()
   
  dx=(cos(mod(32)))*max(80-t*2,0)
  sspr(56,48,58,16,36+dx,8-vl*2)

  if t==40 then
   music(1,3000)
   sfx(2)
  end
 end
 
end

function dark(pix)
 return sget(48+pix,3)
end

function gdd(n)
 n=""..n
 if #n<2 then n="0"..n end
 return n
end

function init_game()
 music(8)
 fade=40
 drk=-1
 upd =upd_game
 dr=dr_game
 water_y=120
 bgx=0
 cvx=0
 cmx=0
 rr=0
 wlk_dist=0
 wlk=0
 cmx=0
 scr_shk=0
 reset_counter()
 flags=0
 start=0
 gmo=nil
 lvl={0,1}
 flaws={}
 for x=0,128 do for y=16,32 do
  mset(x,y,0)
 end end
 
 -- birds
 init_birds()
 
 -- build_pool
 pool={}
 for n in all(pool_start) do add(pool,n) end
 if power>=2 then add(pool,13) end
 scan(0)
 scan(1)
 
 -- hero
 hero=mke(16,8,80)
 hero.phys=true
 hero.we=0.5
 hero.upd=upd_hero
 hero.dr=dr_hero
 hero.cst=16

 -- 
 for n in all(force) do
  add_flaw(n)
 end
 build_lvl()

end

function dr_hero(e)
 if e.shield or (e.csh and t%4<2 ) then
	 
	 n=-flr(abs(e.vx))
	 local flp=e.sns==-1
	 x=e.x+3+n
			
	 if flp then
	  x-=6+2*n
	 end
	 autopal()
	 spr(22,x,e.y,1,1,flp)
 end
end

function init_birds()
 for i=1,5 do
  local cc=i/5
  local c=rnd()
  b=mke(0,rnd(128),rnd(128))
  b.vy=-cc/3
  b.dp=-1
  b.eternal=true
  b.fr=0
  
  b.upd=function(b)
   
   b.vx=cos(t/(100+cc*250))*cc/2
   b.vy=sin(t/(100+c*200))*cc/2
   b.x-=cvx*(0.25+cc/4)
   b.x=b.x%128
   b.y=b.y%128
  end
  b.dr=function(e)
  
   fr=0
   if e.vy<0 then
    fr=flr((t/12)%2)
   end
  
   sspr(24+fr*3,3,3,3,e.x,e.y)
   
  end
 end
end

function announce(fr)
 
 if fr<8 then fr=8 end
 local str="no desc"
 for i=0,16 do
  if mget(i,0)==fr then
   str=flaws_str[i+1]
  end
 end
 
 ww=10+8+#str*4
 hh=8
 local e=mke(0,(128-ww)/2,128)
 e.dp=3
 e.dr=function(e)
  rectfill(x+1,y+1,x+ww+1,y+11,1)
  rectfill(x,y,x+ww,y+10,13)
  for i=0,1 do
  	x=e.x+1-i
  	y=e.y+1-i  
   pal()
   if i==0 then apal(1) end
   print(str,x+14,y+3,7)
   spr(fr,x+1,y+1)
  end
 end
 mvt(e,e.x,115,2)
 function vanish()
  mvt(e,e.x,128,2)
  e.life=40
 end
 wt(48,vanish)
 

end


function build_lvl()
 hero.shield=power>=4
 
 k=0
 for n in all(lvl) do
  for x=0,15 do for y=2,15 do
   tl=mget(x+n*16,y)
   
   if fget(tl,7) then
    
    local e=spawn(tl,(x+k*16)*8,y*8,n,x..","..y)
 
    if fget(tl,2) and e then
     tl=1

    else
     tl=0
    end
 
   end
   mset(x+k*16,y+16,tl)
  end end
  k+=1
 end
end

function in_pool(fr)
 for n in all(pool) do
  if n==fr then return true end
 end
 return false
end
function flw(fr)
 for n in all(flaws) do
  if n==fr then return true end
 end
 return false
end

function spawn(fr,x,y,at,uid)
 if in_pool(fr) then return nil end
 for e in all(ents) do
  if e.uid == uid then
   return nil
  end
 end
 
 local e=mke(fr,x,y)
 e.uid=uid
 e.ox=x
 e.oy=y
 e.flyer=fr==19
 e.bad=fget(fr,1)
 e.spikes=fr==48
 if e.flyer then
  e.phys=true
  e.dr=function(e)
   spr(20,e.x,e.y+4)
  end
 end
 if e.fr==35 then
  e.ddx=0
  e.dr=function()
   spr(36,e.x+e.ddx,e.y)
   spr(35,e.x,e.y)
  end
 end
 
 if fr==24 then
  e.lps=1
  e.spikes=true
  e.immune=true
 end
 
 if fr==50 then
  e.upd = upd_fish
 elseif fr==32 then -- flag
  if at!=start then
   flag=e 
  else
   kl(e)
  end
 else
  e.upd=upd_mon
 end
 
 if fr==43 then
  e.upd=upd_spitter
 end
 -- fx
 if is_in(e) then
  e.cspawn=16
 end
 
 return e
 
end

function upd_fish(e)
 c=((t+e.x*2)%100)/100
 e.y=128-cos(c)*48
 e.fr=50
 if e.y<100 then
  e.fr+=(t/4)%2
 end
end

function upd_mon(e)

 -- walk
 if e.fr==48 or e.fr==38 then

   walk(e)
 
 end
 
 -- fly
 if e.flyer then
  if is_in(e) then
   fly(e)
  end
 end
 
 -- canon
 if e.fr==35 then
  e.ddx*=0.75
  tt=(t+e.x)%120
  if tt%60==0 then
   sfx(12)
   p=mke(37,e.x,e.y)
   p.sns=tt==0 and -1 or 1
   p.vx=p.sns
   p.bad=true
   e.ddx=-p.vx*2
   p.phys=true
   p.proj=true
   p.cgh=8
   p.oncol=function(p)
    explo(p,4)
    kl(p)
   end
  end
 end
 

 
end

function upd_spitter(e)
  
  dx=hero.x-e.x
  dy=hero.y-e.y
  
  if e.cdn and e.cdn>34 then
   if e.cdn==54 then
    e.fr=46
    sfx(26)
    --fireball
    p=mke(59,e.x,e.y)
    p.bad=true
    p.proj=true
    p.phys=true
    p.spikes=true
    p.immune=true
    p.lps=2
    p.oncol=function(e) 
     explo(e,6)
     kl(e) 
    end
    p.vx=e.sns*2
   end
   if e.cdn==44 then
    e.fr=45
   end

  elseif is_in(e) and not e.cdn and  abs(dy)<4 and dx*e.sns>0 then
   e.cdn=64
   e.fr=45
  else
   e.fr=43
   walk(e)
  end

end


function fly(e)
  
 if (e.ox+t)%200==0 then
  e.fr=21
  
  -- bottle
  function f()
   sfx(22)
   e.fr=19
   local p=mke(54,e.x,e.y)
   p.we=0.25
   p.vy=-3
   p.vx=sgn(hero.x-e.x)*2
   p.phys=true
   p.bad=true
   p.proj=true
   p.oncol=function(p)
    sfx(21)
    xpl(p,32,0)
   end
  end
  wt(24,f)
 end
 e.frict = 0.95
 lim=.12
 dx=hero.x-e.x
 if abs(dx)<64 then
  e.vx+=mid(-lim,dx/100,lim)
  e.vy+=(max(hero.y-48,0)-e.y)/100
 end
 e.vy*=0.9
end

function xpl(e,x,y)
 
 for i=0,3 do
  
  local p=mke(0,e.x+2,e.y+2)
  imp(p,i/4+0.15,1)
  p.vx+=rnd()*e.vx
  p.vy-=rnd(2)
  p.we=0.15
  p.bad=true
  p.proj=true
  p.sz=3
  p.life=8+rand(8)
  p.dr=function(p)
   sspr(x+4*i,y,4,4,p.x,p.y)
  end
 end
 kl(e)
 
 
end

function mvt(e,x,y,tt)
 e.twc=0
 e.sx=e.x
 e.sy=e.y
 e.tx=x
 e.ty=y
 dx=x-e.sx
 dy=y-e.sy
 e.spc=tt
 if tt>0 then
  e.spc=tt/sqrt(dx*dx+dy*dy)
 end
 
 
end

function dist(a,b)
 dx=a.x-b.x
 dy=a.y-b.y
 return sqrt(dx*dx+dy*dy)
end


function walk(e)
 dx=0.5*e.sns
 gr=pcol(e.x+dx,e.y+e.sz) and pcol(e.x+dx+7,e.y+e.sz)
 if ecol(e,dx) or not gr  then
  e.sns*=-1
 end
 e.x+=0.5*e.sns
end


function upd_hero(e)
 
 if e.vy>4 then
  e.vy*=0.95
 end
 if not e.cfz then
  e.vx*=0.8
 end
 e.water=e.y+8 > gwy(e.x)
 if e.water then
  e.vx*=0.5
  e.vy*=0.5
 end
 
 
 e.running=false
 ngr=nil
 if ecol(hero,0,1) then
  ngr=mget((hero.x+4)/8,17+hero.y/8)
 end
 if e.ground and not ngr and power>1 then
  e.burning=true
 end
 e.ground=ngr
 
 if e.ground and e.burning then
  e.burning=false
 end
 
 if btn(0) then hmov(-1) end
 if btn(1) then hmov(1) end
 if btnp(2) then jump() end
 
 -- check ground
 check_faller(e) 

 
 -- check death
 for a in all(ents) do
  if a.bad and dcol(a,e)  then 
   if not e.ground and e.vy>0 and e.y<a.y and not a.spikes then
     sfx(11)
     p=stun(a)
     p.vx=hero.vx*0.5
     e.vy=-5
   elseif not e.cim then
    if a.proj and a.vx*e.sns<0 and (e.shield or e.csh) then
     hero.vx=a.vx
     hero.cfz=20
     hero.shield=false
     hero.csh=28
     p=stun(a)
     p.vx=-a.vx/2
     sfx(31)
    else
     die()
     a.killer=true
    end
   end
  end
  if a.hcol then a.hcol(a) end
 end
 -- check fall
 if e.y>125 then
  die()
  return
 end

 -- check flag
 if dcol(e,flag) then
  local f=flag
  level_up()
  kl(f)
  function pop()
   local p=mkp(48,f.x+rand(8),f.y+7,1)
   --p.we=-rnd(0.5)
   p.vy=-rnd(8)
   p.frict=0.85
   p.life=8+rand(8)
   p.dp=0
  end
  for i=0,16 do wt(i,pop) pop() end
 end

end

function splash(x)
 local p=mkp(24,x,gwy(x),2)
 p.we=0.1+rnd()/10
 p.vy=-rnd(2)
 return p
end

function stun(e)
 sfx(32)
 kl(e)
 local p=mke(e.fr,e.x,e.y)
 p.vy=-2
 p.we=0.5
 p.life=40
 p.stun=true
 return p
end

function mkp(fx,x,y,sc)
 sc=sc or 1
 local p=mke(0,x,y)
 p.life=8+rnd(8)
 p.dr=function()
  sspr(fx+8-p.life/2,0,1,1,p.x,p.y,sc,sc)
 end
 return p
end

function burn()
 
 hero.cim=8
 hero.burning=false
 explo(hero,5)
 
 if flw(13) then
  hero.churt=20
  count-=60
  if count<1 then count=1 end
 end
 
 ray=16
 for e in all(ents) do
 	if not e.immune and e.bad and dist(e,hero)<24 then
   p=stun(e)
   imp(p,atan2(dx,dy),3)
   p.fir=true   
  end
 end

 
end


function explo(from,mx)
 scr_shk=8
 sfx(15)
 local e=mke(0,from.x,from.y)
 e.life=8
 e.dp=0
 e.dr=function(e)
  c=1-e.life/8
  circ(e.x+4,e.y+3,6+sqrt(c)*16,10)
  circ(e.x+4,e.y+3,4+sqrt(c)*4,7)  
 end 

 for i=0,mx-1 do
  local p=mkp(16,from.x+rnd(8), from.y+rnd(8))
  imp(p,rnd(),rnd(2))
 end
end

function die()
 music(-1,1000)
 gmo=16
 sfx(9)
end

function check_faller(e)

 for i=0,1 do 
	 local px=flr((e.x+i*7)/8)
	 
	 for k=0,8 do
	  local py=max(flr(e.y/8+1)-k,0)
	  tl=mget(px,py+16)
	  if fget(tl,1) and flw(tl) then
	   sfx(29)
	   mset(px,py+16,1)
	   local e=mke(tl,px*8,py*8+1)
	   e.wfl=true
	   function fall()
	    sfx(30)
		   mset(px,py+16,0)
		   e.wfl=false
		   e.we=0.5
		   e.vy=1
		   e.life=40
		   e.bad=true
		   e.spikes=true
	   end
	   wt(16,fall)
	  end
	 end
 end
end


function power_up()
 power+=1
 rr=paliers[power]

end

function reset_counter()
 count=time_max
 if power>=3 then count+=150 end
end

function level_up()
 flags+=1
 hisc=max(flags,hisc)
 if power<4 and flags==paliers[power+1] then
  power_up()
 end
 reset_counter()
 start=1-start
 --sfx(8)
 sfx(42)
 --clean ents
 for e in all(ents) do
  if not is_in(e) and not e.eternal then
   kl(e)
  end
 end
 
 --
 if force_nxt then
  add_flaw(force_nxt)
  force_nxt=nil
 elseif #pool>0 then
  n=pool[rand(#pool)+1]
  add_flaw(n)
 end
	build_lvl()
end

function is_in(e)
 px=e.x-cmx
 return px>=-8 and px<128
end

function add_flaw(n)


 announce(n)
 add(flaws,n)
 del(pool,n)
 
 -- insert shark
 if n==12 then
  e=mke(0,0,120)
  e.sns=-1
  e.eternal=true
  e.frict=0.9
  e.upd=upd_shark
  e.dr=function(e)
   flp=e.sns==-1
   -- body
   x=e.x
   if e.sns==-1 then x+=8 end
   spr(26,x,e.y,3,1,flp)
   spr(26,x,e.y+8,3,1,flp,true)
   -- head
   x=e.x+24
   if e.sns==-1 then
    x=e.x
   end
   
   df=0
   
   if e.cc then
    df=flr(-sin(e.cc)*2+.5)
   end
   spr(29+df,x,e.y,1,1,flp)
   spr(29+df,x,e.y+8,1,1,flp,true)
   --eye
   circfill(x+2,y+6-df*2,1,13)
   
  end
  e.hcol=function(e)
 	 hdx=e.x+16-(hero.x+4)
	  hdy=e.y+8-(hero.y+4)
	  if abs(hdx)<18 and abs(hdy)<9 then
	   die()
	  end  
  end

  
 end
 
 
 -- insert_lvl
 if n<8 then
  pos=1+rand(#lvl-1)
  insert(lvl,pos,n)
  scan(n)
  for e in all(ents) do
   if e.x>pos*128 then
    e.x+=128
   end
  end  
  cmx=hero.x-64
 end
end

function upd_shark(e)

 e.vx+=e.sns*.25
 dx=hero.x-e.x
 dy=hero.y-e.y
 if abs(dx)>64 then
  e.sns = sgn(dx)
 end
 
 e.y=water_y+get_tide(e.x)-abs(cos(t/200)*8) 
 wy=gwy(x-cmx)
 


 e.cc=nil
 if e.cjmp  then
  e.cc=max((e.cjmp-96)/64,0)
  e.y+=sin(e.cc)*24
  
 elseif abs(dx)<32 and abs(dy)<32 and dx*e.sns>0 then
  e.cjmp=128
  sfx(25)
 end
 
 -- water drop
 if abs(e.y+7-wy)<4 then
  x=e.x+12+e.sns*14+rnd(8)
  
  p=splash(x)
  p.vx=-e.vx
  
 end

end

function scan(lvl)
 for x=0,15 do for y=2,15 do
  tl=mget(lvl*16+x,y)
  if tl!=32 and ( fget(tl,7) or fget(tl,1) ) and not flw(tl) and not in_pool(tl) then
   add(pool,tl)
  end
 end end
end
function insert(a,pos,el)
 for i=0,#a do
  n=#a-i
  if n==pos then
   a[n+1]=el
   return
  else
   a[n+1]=a[n]
  end

 end
 
end

function rand(n)
 return flr(rnd(n))
end

function dcol(a,b)
 box=a.x<b.x+b.sz and a.x+a.sz>b.x and
    a.y<b.y+b.sz and a.y+a.sz>b.y
 if not box then return false end

 if a.fr==0 or b.fr==0 then
  return true
 end
 
 for i=0,63 do 
  x=i%8
  y=flr(i/8)
  if gpix(a,x,y)>0 then
   bx=x-(b.x-a.x)
   by=y-(b.y-a.y) 
   if gpix(b,bx,by) > 0 then
    return true
   end
  end  
 end
 return false
end

function gpix(e,x,y)
 if e.sns==-1 then 
  x=7-x 
 end
 if x<0 or y<0 or x>=8 or y>=8 then
  return 0
 end
 
 return sget((e.fr%16)*8+x,flr(e.fr/16)*8+y)
end


function jump()
 if hero.ground or hero.burning then
  sfx(7)
  hero.vy = -5
  if power>1 then
   hero.burning = not hero.burning
   if not hero.burning then
    burn()
   end
  end
 end
end

function hmov(d)
 if hero.ground then
  wlk_dist+=abs(hero.vx)
  if hero.water then
   p=splash(hero.x+rnd(8))
   p.vx=rnd()*hero.vx*2
  end
  lim=hero.water and 4 or 8
  
  if wlk_dist>lim then
   wlk_dist-=lim
   wlk+=1
   bss=13
   
   local g=hero.ground
   if g==65 then
    bss=17
   end
   if g==70 or g==74 then
    bss=19
   end   
   if hero.water then
    bss=27
   end
   sfx(bss+wlk%2)
  end
 end

 hero.running=true
 spd=0.3
 if power>0 then spd=0.5 end
 if hero.cfz then 
  spd*=1-hero.cfz/20
 end
 hero.vx+=d*spd
 hero.sns=d
end

function mke(fr,x,y)
 e={fr=fr,x=x,y=y,vx=0,vy=0,
  sz=8,we=0,frict=1,sns=1,n=0,
  dp=1,lps=8,
 }
 add(ents,e)
 return e
end

function upe(e)
 
 -- counters
 for v,n in pairs(e) do
  if sub(v,1,1)=="c" then
   n-=1
   if n==0 then
    e[v]=nil
   else
    e[v]=n
   end
  end
 end 
 
 --move
 if e.phys and not e.cgh then
  pmove(e)
 else
  e.x+=e.vx
  e.y+=e.vy
 end
 
 e.vy+=e.we
 e.vx*=e.frict
 e.vy*=e.frict 
 
 if e.upd then e.upd(e) end
 
 -- tweening
 if e.twc then
  e.twc=min(e.twc+e.spc,1)
  c=e.twc
  e.x=e.sx+c*(e.tx-e.sx)
  e.y=e.sy+c*(e.ty-e.sy)
  if e.twc==1 then
   e.twc=nil
  end
 end
 
 if e.life then
  e.life-=1
  if e.life<= 0 then
   kl(e)
  end
 end
 
 if e.cspawn and t%3==0 then
  p=mke(52,e.x,e.y)
  p.x+=rnd(8)-4
  p.y+=rnd(8)-4
  p.we=-0.1
  p.life=8+rand(10)
  p.twinkle=true
 end
 
end



function pmove(e)
 col=false
 e.x+=e.vx
 sgx=sgn(e.vx)
 while ecol(e) do
  e.x-=sgx
  e.vx=0
 	col=true
 end
 e.y+=e.vy
 sgy=sgn(e.vy)
 while ecol(e) do
  e.y-=sgy
  e.vy=0
  col=true
 end
 if col and e.oncol then e.oncol(e) end

end

function ecol(e,dx,dy)
 dx = dx or 0
 dy = dy or 0
 a={0,0,1,0,0,1,1,1}
 for i=0,3 do
  x=e.x+a[1+i*2]*(e.sz-1)+dx
  y=e.y+a[2+i*2]*(e.sz-1)+dy
  if pcol(x,y) then
   return true
  end
 end
 return false
 
end

function pcol(x,y)
 if y<0 or x<0 or x>=#lvl*16*8 then
  return false
 end
 tl=mget(x/8,16+y/8)
 if x<0 then return true end
 return fget(tl,0)
end



function dre(e)
 if e.dp != depth then return end
 flp=e.sns==-1
 x=e.x
 y=e.y
 fr=e.fr
 
 -- running
 if e.running then
  if e.ground then
   fr+=(t/5)%2
  else
   fr+=1
  end
 end

 --
 if e.wfl then
  y = y+((t+e.x/8)/2)%2
 end
 if e.twinkle then
  fr=fr+t%2
 end
 
 
 if e.flyer then
  y=y+flr(cos(mod(16))+0.5)
 end
 
 -- loop
 loop=0
 while fget(fr+loop,3) do loop+=1 end
 if loop>0 then
  fr=fr+(t/e.lps)%loop
 end
 
 -- jump
 if fget(fr,0) then
  y-=1
 end
 
 -- map
	autopal()
	if e==hero or e.killer then
	 pal()
	end
	
 if e.burning then
  pal(13,9+t%2)
  pal(5,8+t%2)
  
  cl=10+t%2
  if cl==11 then cl=7 end
  pal(14,cl)
 end
 
 if e.fir then
  for i=0,15 do pal(i,sget(48+i,4+mod(4)*3)) end
 end
	
 spr(fr,x,y,e.sz/8,e.sz/8,flp,e.stun)

 if e.dr then e.dr(e) end
	--if e.burning then
  --spr(40,x,y+5.5+cos(mod(5)))
	--end
end

function wt(n,func)
 e=mke(0,0,0)
 e.life=n
 e.dth=func
end

function kl(e)
 del(ents,e)
 if e.dth then e.dth() end
end

function upd_game()

 -- game over
 if gmo then
  gmo-=1
  if gmo==0 then
   sfx(10)
   gmo=nil
   kl(hero)
   for i=0,7 do
    e=mke(52,hero.x,hero.y)
    imp(e,i/8,2)
    e.frict=0.95
    e.life=40
    e.twinkle=true
   end
   wt(36,init_menu)
  end
  return
 end
 
 -- chrono
 count-=1
 if count%30==0 and count>=-30 and count<=90 then
  if count==-30 then  die()
  elseif count==0 then 
   sfx(24)
   hero.churt=8
  else 
   sfx(23)
   hero.churt=8
  end
 end
 
 -- water
 wty=flw(11) and 110 or 120
 water_y += (wty-water_y)*0.1 
 
 t+=1
 foreach(ents,upe)
end

function _update()
 if fade then
  fade=mid(0,fade+drk*2,40)
  if fade==40 or fade==0 then
   fade=nil
  end
 end
 if upd then upd() end
end

function imp(e,an,spd)
 e.vx+=cos(an)*spd
 e.vy+=sin(an)*spd
end

function autopal()
 pal()
 if gmo then
  for i=0,15 do
   pal(i,sget(16+i,1))
  end
 end
 
 --
 if fade then
  for i=0,15 do
   pal(i,sget(8+i,72+fade*8/40))
  end
 end  

 
end

function get_tide(x)
 tide=4
 if flw(11) then tide+=4 end
 return cos(t/300)*tide
end
function gwy(x)
 local n=cos((cmx*1.5+x)/48)*cos(t/40)*3
 local wy=water_y+get_tide(x-cmx)+n
 if hero.cst then wy+=hero.cst end
 return wy
end
function dr_game()
 autopal()
 
 --bg
 rectfill(0,0,127,127,14)

 map(48,48,bgx/2-16)
 depth=-1
 foreach(ents,dre)
 
 map(0,48,bgx)
 ncmx=hero.x-64
 cvx=ncmx-cmx
	--bgx+=(cmx-ncmx)/2
	bgx-=cvx/2
 cmx=ncmx
 
 camera(cmx,scr_shk)
 scr_shk*=-0.25
 
 depth=0
 foreach(ents,dre)
 
 --
 map(0,16,0,0)
 depth=1
 foreach(ents,dre)
 
 -- game over
 if gmo then
  c=gmo/16
  ray=c*c*128
  circ(hero.x+4,hero.y+4,ray,7)
 end
 
 -- inter
 y=0
 if hero.cst then
  y=hero.cst 
 end
 camera(0,y)
 pal()
 for i=0,1 do
  camera(i,y+i)
  pal()
  if i==0 then apal(2) end
  sspr(16,3,4,5,3,2)
  print(flags,9,2,7)
  tpx=110
  if hero.churt then
   tpx+=(t%2)*2-1
  end
  --if count>0 and count<90 	and count%30 >20 then
  -- tpx+=(t%2)*2-1
  --end
  str=""..max(flr(count/30,0)+1,0)
  if #str<=1 then str="0"..str end
  sspr(20,3,4,5,tpx,2)
  print(str,tpx+6,2,count<90 and 8+6*(t%2) or 7)
 
  -- unlock
  if power<4 then
   rectfill(48,2,80,6,7)
   rectfill(49,3,79,5,2)
   c=(flags-rr)/(paliers[power+1]-rr)
   rectfill(49,3,49+30*c,5,8)
   str=powers_str[power+1]
   print(str,65-#str*2,9,7)
  end
  
 end
 camera()

 
 
 
 -- ocean
 for x=0,127 do

  first=2
		by=gwy(x)
  for y=by,128 do
   pix=pget(x,y)
   if first>0 then
    for i=1,first do
     pix=sget(48+pix,1)
    end
    first-=1
   end
   pset(x,y,sget(48+pix,2))
  end
 end
 
 --
 depth=3
 foreach(ents,dre) 
 
 --dx=cmx%16
 --for i=0,18 do
 -- fr=112+(t/2)%6
 -- flp=i%2==1
 -- if (t/2)%12 < 7 then flp = not flp end
 -- spr(f0r,i*8-dx,120,1,1,flp)
 --end 
 
end

function _draw()
 cls()



 
 if dr then dr() end
 
 -- log
 color(7)
 cursor(0,8)
 for n in all(logs) do
  print(n)
 end
 
end

function apal(n)
 for i=0,15 do pal(i,n) end
end

function log(n) 
 add(logs,n)
 if #logs>18 then
  del(logs,logs[1])
 end
end

powers_str={
 "speed up",
 "bomb jump",
 "xtra time",
 "shield"
}
flaws_str={
 "island extention",
 "flying drunkard",
 "razor discs",
 "double-sided canons",
 "carefree pangolins",
 "armoured beetles",
 "starving piranhas",
 "weak bridges",
 "rusty platforms",
 "high tides",
 "mischevious shark",
 "potatoes spitter",
 "time costing bomb",
 "todo",
 "todo",
}


__gfx__
00000000000000007777a98877777ab30bb0bb30eeb00b0077ffccdd55555555000000000123456789abcdef0008800000000000000000000000000000000000
0000000000000000011ddd67d6766d67b333bb30883303301d8b9677ea7a66b7000000001d8b9677ea7a66f700888800fff0000000aaaa900000000000000000
000000000000000077ffee882211dd6600000b3023330000113333babbabb3ba000b3b00144444a799a9a9a7088888800efff0000ad6d6d90000000000000000
000000000000000098889a94d0d000000000003000000000001151df2893d5de000888000000000000000000000880000eeeff000a6717790000000006666600
0000000000000000988877660d00d00000000000000000002222889a89a9989a00088b3b0000000000000000b00ee00b0eeeeff00ad117d90000000066666070
000000000000000098880760000d0d00000000000000000022222289289882893b3b888800000000000000003bb22bb30eeeeef00a6777790000000066607000
0000000000000000900077660000000000000000000000002222222822822228888888880000000000000000333223330eeeeef009d7d7690000000060700000
000000000000000090009a9400000000000000000000000000000000000000008888888800000000000000001331133100000000009999900000000070000000
05ddeed505ddeed5000ff0000003b00000000000b7ee7b000000000000000000007d6d0000d7d600000000000000000000000000000000000000000000000000
005deed0005deed000000ff00003b00000000000338833b4000077770000000006dddd6007ddddd0000000000000000000000000000000000000000000000000
0052dd200052dd200000000000399b000776d660aa88330000006999000000007d66666ddd66666dfff0000000000000ffff0000000000000000000000000000
0052dd200052dd2000ffff000349a4b0d777777d995494b00000694400000000dd6dd6667d6dd66d00ffff00000000000eeef00000000000000000000fffff00
0028888000288880000000003099a90b777777762299a90b00006944000000006d6dd67ddd6dd67100eeeff0000000000eeeef000000000000fff000fffff070
022877800228778000000ff00229928077d77d76222992800000694400000000dd6666716d66667d000eeeff000000fffffffffffffe0000ffffff00fff07000
022888805228888d000ff0009a22289a77d77d660222289a0000d9440000000006d677d006667710000eeeeff00ffffffffffffeffffff00fffff070f0700000
00500d00000000000000000099228899d666666d0222889900000ddd0000000000d1d10000dd1d00000eeeeeffffffffffffffeffffffff0f707000070000000
098880080980088809088800000000006d6666d6d0ddd22000000000000000008000000800000fffeee000000444400000444400044440000444400000ffee00
098772880982877809877728000760007677776761666882000000000000f00f800000080000ffffeeed0000444444000444444044444400447447000fffeee0
09888d78098d7878098888d8000660006d6666d6d1ddd2220000f00f0000ffff980000890000ffeeeeed0000447447000447447044744700444444400ffeeee0
0987727809828778098777d80076d600ddddddddd1ddd11100bbffff00bbf2f2998888990000eee77edd000044444440044444444444444044422200ffe77eee
09888d78098d7888098888d8006dd600d1dddd1dd0ddd1104bbbf2f20bbb9fff0a9999a00000ee7577dd0000244ffff00244ffff2442220024422200ff7557dd
0900028009028000098000280766666000000000000000000bb39fff4bb339ff00aaaa000000de7777d500002229999002229999222ffff022288800ee7557ed
090000000900000009000000066666600000000000000000043119ff043111990000000000ffeee77dd5dd00229999000229999222999900229ffff00ed77ed0
09000000090000000900000006666660000000000000000090409099490009000000000000ffe5555555dd002000200020000000200020002000200000eeed00
0000000000000000007cc0000000000000000000000000000000000000004000000000000ffedd555555ddd00288820000000000000000000000000000000000
0000000000000000077cdc0000007c0000fff00000000000000000000000b00000000000fffdddd5555dddee8899982002888200002888200028882000000000
00000000000d0000077ccc007288cdc00fffff000000000000000000000bb30000000000ffeddddeedddeeee89aaa88028899820028998820289988200000000
000d00000d0d06000ccccc0067ccccc00ff7ff00000f000077ee7b000007b300000000000deeeddeeddeeed089a1198089a11980089119980891199800000000
0d0d06000d2d86000dcccc000ddccc000fffff0000000000bb88bbb4000e880000000000eeddeeeeeeeedd5d8991198089a1198008911a9808911a9800000000
0d2d8600dddd666000dcc00000ddc00000fff0000000000033883300000e880000000000eeedddeeeedd5ddd2889982089aa9880088aaa98088aaa9800000000
dddd666002222200000c0000000c00000000000000000000000000000007b300000000000ddeeddd5555ddd00288820028999820028999880289998800000000
022292900222929000dcc00000dcc0000000000000000000000000000007b30000000000000dddddd55dd0000000000008888200002888200028882000000000
777777763333bbbb2222228e0000000330000000004444006666666d1111111211281111000f000066666d7dd000000d000000000000000033333333d0000000
777777663333bbbb228e2288000033033033000004ffff4066ddd6d5111112221122122803f3f30066ddd6d5ddd0dddd0000000000dd000d33333333dd000000
7766666d222288882288222200003333333300004f9999f46d5ddd551111121111111222033333006d5d6d55dd000ddd00000000000ddd0d333333333d0dd000
7766666d2222888e2222288e03003333333300304f9ff9f46dddddd5111111111111122200bb30006dddddd5dddd0ddd000000000000dddd3333333333dd0000
776666dd28e2888828e2288803333333333333304f9ff9f46dddddd522111111128e1111003330006dddddd5dddddddd0dd000000dddd33333333333333ddd00
766666dd288228882882288800333333333333004f9999f466ddd6d512111111128811110003000066d5d6d5ddd00ddd00dd00dddddd3333333333333ddd3dd0
66dddddd2228e2222228e222003333333333330004ffff406d5ddd551111111112221111b3030bb06d56dd55dddddddd0dd3ddd0000dd33333333333dddddd00
6ddddddd2228822222288222333333333333333300444400d555555511111111111111110333b300d5d55555ddddddddd333333d0ddd33333333333333333ddd
2277677776666622277766620000000000000000ff000000d0000006000000ff0000006ddd000000ddd33d33111111110000000000000000ddddddddd0000000
277766777766666277666776000000000000000024000000dd00006d00000024000006ddddd00000dd333ddd111111110000000000dd000ddddddddddd000000
77777667776666667666766d30003000000000009a0000000dd006d00000009a00006dd00ddd0000d3333ddd1111111100000000000ddd0ddddddddddd0dd000
77766666666d66666666666d30030030000000002290000000dd6d00000009220006dd0000ddd0003333333d11111111000000000000dddddddddddddddd0000
7776666666d6666666666ddd030303000000000024090000006ddd0000009024006dd000000ddd003333d333111111110dd000000ddddddddddddddddddddd00
6666666ddd6666666666dd6d03030300000300009a00900006d00dd00009009a06dd00000000ddd033333dd31111111100dd00ddddddddddddddddddddddddd0
6666666666677666266dd6dd0030303000030030229009006d0000dd009009226dd0000000000ddd33333333111111110dddddd0000ddddddddddddddddddd00
6666666666d666dd22666dd2003030300303030024090090d000000d09009024dd000000000000dd3333333311111111dddddddd0ddddddddddddddddddddddd
66666666dd6666dd007766002222233b00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000ddddddd
6666666dd66ddddd07666776233b22830000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000ddddd
666666dddddd6ddd7666766d22832223000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000dddddd
66666dd6ddd766dd6666666d2223233b0000000000000000000000008880000888000dddddddddd0ddd0000ddd0dddddddd000dddd000ddd0000000000dddddd
6666dd66dd666ddd66666ddd23b228330000000000000000000000008880000888000dddddddddd0ddd0000ddd0ddddddddd00dddd000ddd000000000dd0dddd
6666d666dd66dddd6666dd6d388228880000000000000000000000008880000888000dddddddddd0ddd0000ddd0dddddddddd0ddddd00ddd000000000d00dd0d
2666666dddddddd2666dd6dd3223b2220000000000000000000000008880000888000000dddd0000ddd0000ddd0ddd000dddd0ddddd00ddd000000000000dd0d
226ddddddddddd22d6666ddd222882220000000000000000000000008880000888000000dddd0000ddd0000ddd0ddd000dddd0dddddd0ddd0000000000000d00
000000000000000000000000000000000000000f00000000000000008880000888000000dddd0000ddd0000ddd0ddddddddd00dddddd0ddd00000000ddddddd0
0000000000000000000000000000000f000000000000000f000000008880000888011110dddd0000ddd0000ddd0ddddddddd00dddddddddd00000000ddddd000
000000000000000000000000000000ff0000000000000000000000008880000888011110dddd0000ddd0000ddd0dddddddd000ddd0dddddd00000000dddddddd
00000000000000000000000f000000ff0000000f00000000000000008880000888000000dddd0000ddd0000ddd0ddd00ddd000ddd0dddddd00000000ddddddd0
00000000000000ff000000ff00000fff000000ff000000ff000000008888008888000000dddd0000dddd00dddd0ddd000ddd00ddd00ddddd00000000dddd0000
ffffffff00ffffff0000ffff000fffff0000ffff00ffffff000000008888888888000000dddd0000dddddddddd0ddd000ddd00ddd00ddddd00000000d0ddd000
ffffffffffffff990ffffff900fffff90ffffff9fffffff9000000000888888880000000dddd00000dddddddd00ddd0000ddd0ddd000dddd00000000d000dd00
99999999ff999999fffff999ffffff99ffffff99fff99999000000000088888800000000dddd000000dddddd000ddd0000ddd0ddd000dddd0000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
33333ddd0123456789abcdef00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
3333dd33001111d62893d58e00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
33ddedd30000005d1281512800000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
3333ddd3000000150120101200000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
3deedd33000000010010000100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
33dd3333000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
dddd3333000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
dd333333000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
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
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000046
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
0000000000000000000000000000000000c4d4f40000000000000000000000000000000000000000000000000000000000000000000000c50000000000000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000e4e4
000000000000000000000000000000c4d4e4e4e4f4000000000000000000000000000000000000000000000000000000000000000000d5e5f5c5000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000e4e4e4
00000000000000000000000000c4d4e4e4e4e4e4e4f4c4000000000000000000000000000000000000000000000000000000000000d5e5e5e5e5f500c5000000
000000000000000000000000000000c5000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000d4a5e4e4e4e4e4e4e4e4e4f4c40000c4d4f40000d4f400000000000000000000000000000000000000f6e5e5e5e5e5b4e5f50000
000000000000000000000000c5d5b4e5f5c500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000c4d409a5e4e4e4e4e4e4e4e4e4e4e4f4d4a5e4e4f4d4e4e40000000000000000000000000000000000000000f6e5e5e5e5e5e5f70000
0000000000000000000000d5e5e5e5e5e5e5f5000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000d409e4e4e4e4e4e4e4e4e4e4e4e4e4e40909e4e4e40909e4e4f400000000000000000000000000000000d5f5c5d5e5e5e5e5e5e5f50000
00000000000000000000d5e5e5e5e5e5e5e5e5f50000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000d4e4e4a5e4e4e4e4e4e4e4e4e4e4e4e409e4e4e4090909e4e4e4e4000000000000000000000000000000d5e5e5e5e5e5e5e5e5e5e5e5f5c5
d5f50000000000000000d5e5e5e5e5e5e5e5e5e5f5000000000000d5f50000000000000000000000000000000000000000000000000000000000000000000000
0000d4f40000000009e409a5e4e4e4e4e4e4e4e4e4e4e4e4e4e4e4e4090909e4e4e4e4f4000000d4f4000000000000000000f6e5e5e5e5e5e5e5e5e5e5e5e5e5
e5e5f5c50000000000d5e5e5e5e5e5e5e5e5e5e5e5b4f5000000d5e5e5b4f5c50000000000000000000000000000000000000000000000000000000000000000
d4e4e4e4f40000d4e4a5e4e4e4e4e4e4e4e4e4e4e4e4e4e4e4e4e4e4e4e4e4e4e4e4e4e4f400d4e4e4f4d4f40000000000d5e5e5e5e5e5e5e5e5e5e5e5e5e5e5
e5e5e5e5f50000d5e5e5e5e5e5e5e5e5e5e5e5e5e5e5e5f500d5e5e5e5e5e5e50000000000000000000000000000000000000000000000000000000000000000
__label__
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
ee9888ee777eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee777777777777777777777777777777777eeeeeeeeeeeeeeeeeeeeeeeeeeee9a94ee888e88eeeeeeee
ee98882e7272eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee7822222222222222222222222222222272eeeeeeeeeeeeeeeeeeeeeeeeeee77662e8282e82eeeeeee
ee98882e7272eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee7822222222222222222222222222222272eeeeeeeeeeeeeeeeeeeeeeeeeeee7622e8282e82eeeeeee
ee92222e7272eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee7822222222222222222222222222222272eeeeeeeeeeeeeeeeeeeeeeeeeee7766ee8282e82eeeeeee
ee92eeee7772eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee7777777777777777777777777777777772eeeeeeeeeeeeeeeeeeeeeeeeeee9a942e8882888eeeeeee
eee2eeeee222eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee222222222222222222222222222222222eeeeeeeeeeeeeeeeeeeeeeeeeeee2222ee222e222eeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee77e777e777e777e77eeeeee7e7e777eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee7e22727272227222727eeeee72727272eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee777e777277ee77ee7272eeee72727772eeeeeeeeeeeeeededeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee2727222722e722e7272eeee72727222eeeeeeeeeeeeeeedeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee77e272ee777e777e7772eeeee77272eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee22ee2eee222e222e222eeeeee22e2eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeededeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeedeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee9e888eeeeeeeeeeee5ddeed5eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee9877728eeeeeeeeeee5deedeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee98888d8eeeeeeeeeee52dd2eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee98777d8eeeeeeeeeee52dd2eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee98888d8eeeeeeeeeee28888eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee98eee28eeeeeeeeee228778eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee9eeeeeeeeeeeeeeee228888eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee9eeeeeeeeeeeeeeeee5eedeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee6666666d6666666d6666666d6666666d6666666deeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee66ddd6d566ddd6d566ddd6d566ddd6d566ddd6d5eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee6d5ddd556d5ddd556d5ddd556d5ddd556d5ddd55eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee6dddddd56dddddd56dddddd56dddddd56dddddd5eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee6dddddd56dddddd56dddddd56dddddd56dddddd5eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee66ddd6d566ddd6d566ddd6d566ddd6d566ddd6d5eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee6d5ddd556d5ddd556d5ddd556d5ddd556d5ddd55eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeed5555555d5555555d5555555d5555555d5555555eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeedeeeeee6eeeeeeeeeeeeeeeedeeeeee6deeedee6eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeddeeee6deeeeeeeeeeeeeeeeddeeee6dddeddd6deeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeddee6deeeeeeeeeeeeeeeeeeddee6dddddd36dddeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeedd6deeeeeeeeeeeeeeeeeeeedd6deedddd6dddeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeddeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee6dddeeeeeeeeeeeeeeeddeee6dddddd36ddd3dddeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeeddeeddeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee6deeddeeeeeeeeeeeeeeedde6dddddd36d33ddd3ddeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eeddddddeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee6deeeeddeeeeeeeeeeeeedd36ddeeedd6d33ddddddeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
eddddddddeeeeeeeeeeeeeeeeeeeeeeeeeeeeeedeeeeeedeeeeeeeeeeeed333d33dedddd333333d3dddeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
edddddddddeeeeeeeeeeeeeeeeeeeeeeeeeeeeedeeeeee6eeeeeeeeeeee3333d3333336d33333363333deeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
dddddddddddeeeeeeeeeeeeeeeeeeeeeeeeeeeeddeeee6deeeeeeddeeed3333dd33336ddd33336d3333ddeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
dddddddddddeddeeeeeeeeeeeeeeeeeeeeeeeeeeddee6deeeeeeeeddded33333dd336d33dd336d333333deddeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
dddddddddddddeeeeeeeeeeeeeeeeeeeeeeeeeeeedd6deeeeeeeeeedddd333333dd6d3333dd6d33333333ddeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
dddddddddddddddeeeddeeeeeeeeeeeeeeeeeeeee6ddddeeeeeedddd3333333336ddd33336ddd333333333dddeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
ddddddddddddddddeeeddeeddeeeeeeeeeeeeeee6deedddeedddddd3333333336d33dd336d33dd333333ddd3ddeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
dddddddddddddddeeeddddddeeeeeeeeeeeeeee6deeeddddddeeeedd33333336d3333dd6d3333dd3333ddddddeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
dddddddddddddddddddddddddeeeeeeeeeeeeeedeeed33d333deddd33333333d333333dd333333d333333333dddeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
ddddddddddddddddddddddddddeeeeeeeeeeeeedeee33363333333333333333d3333336d333333666666d7d66666d7d66666d7deeeeeeeeeeeeeeeeeeeeeeeee
dddddddddddddddddddddddddddeeeeeeeeeedddded336d3333333333333333dd33336ddd33336d66ddd6d566ddd6d566ddd6d5eeeeeeeeeeeeeeeeeeeeeeeee
dddddddddddddddddddddddddddeddeeeeeeeeddddd36d333333333333333333dd336d33dd336d36d5d6d556d5d6d556d5d6d55eeeeeeeeeeeeeeeeeeeeeeeee
dddddddddddddddddddddddddddddeeeeeeeeeedddd6d33333333333333333333dd6d3333dd6d336dddddd56dddddd56dddddd5eeeeeeeeeeeeeeeeeeeeeeeee
dddddddddddddddddddddddddddddddeeeeedddd36ddd333333333333333333336ddd33336ddd336dddddd56dddddd56dddddd5eeeeeeeeeeeeeeeeeeeeeeeee
ddddddddddddddddddddddddddddddddedddddd36d33dd3333333333333333336d33dd336d33dd366d5d6d566d5d6d566d5d6d5eeddeeeeeeeeeeeeeeeeeeeee
dddddddddddddddddddddddddddddd3dddeeeed6d3333dd33333333333333336d3333dd6d3333dd6d56dd556d56dd556d56dd55dddeeeeeeeeeeeeeeeeeeeeee
dddddddddddddddddddddddddddd333333dedddd333333d3333333333333333d333333dd333333dd5d55555d5d55555d5d55555333deeeeeeeeeeeeeeeeeeeee
ddddddddddddddd6666666d6666666d6666666d6666666d3333333333333333d3333336d3333336d333333633333333333333333333deeeeeeeeeeeeeeeeeeee
ddddddddddddddd66ddd6d566ddd6d566ddd6d566ddd6d53333333333333333dd33336ddd33336ddd33336d33333333333333333333ddeeeeeeeeeeeeeeeeeee
ddddddddddddddd6d5ddd556d5ddd556d5ddd556d5ddd5533333333333333333dd336d33dd336d33dd336d3333333333333333333333deddeeeeeeeeeeeeeeee
ddddddddddddddd6dddddd56dddddd56dddddd56dddddd5333333333333333333dd6d3333dd6d3333dd6d333333333333333333333333ddeeeeeeeeeeeeeeeee
ddddddddddddddd6dddddd56dddddd56dddddd56dddddd53333333333333333336ddd33336ddd33336ddd3333333333333333333333333dddeeeddeeeeeeeeee
ddddddddddddddd66ddd6d566ddd6d566ddd6d566ddd6d5333333333333333336d33dd336d33dd336d33dd3333333333333333333333ddd3ddeeeddeeddeeeee
ddddddddddddddd6d5ddd556d5ddd556d5ddd556d5ddd5533333333333333336d3333dd6d3333dd6d3333dd33333333333333333333ddddddeeedd3dddeeeeee
dddddddddddddddd5555555d5555555d5555555d55555553333333333333333d333333dd333333dd333333d3333333333333333333333333dddd333333deeeee
ddddddddddddddddddd33363dddddd33d333333d33333363333333333333333d3333336d333333633333333333333333333333333333333333333333333deeee
ddddddddddddddddddd336ddd33dd333ddd3333dd33336d3333333333333333dd33336ddd33336d33333333333333333333333333333333333333333333ddeee
ddddddddddddddddddd36ddedd3d3333ddd33333dd336d333333333333333333dd336d33dd336d3333333333333333333333333333333333333333333333dedd
ddddddddddddddddddd6d33ddd33333333d333333dd6d33333333333333333333dd6d3333dd6d333333333333333333333333333333333333333333333333dde
dddddddddddddddd36dddeedd333333d3333333336ddd333333333333333333336ddd33336ddd3333333333333333333333333333333333333333333333333dd
ddddddddddddddd36d33ddd333333333dd3333336d33dd3333333333333333336d33dd336d33dd3333333333333333333333333333333333333333333333ddd3
dddddd3dddddddd6d33dddd33333333333333336d3333dd33333333333333336d3333dd6d3333dd33333333333333333333333333333333333333333333ddddd
eddd333333dddddd333dd3d3333333333333333d333333d3333333333333333d333333dd333333d3333333333333333333333333333333333333333333333333
edd33333ddd3333d33333363333333333333333d333333633333333333333333333bbbb3333bbbb3333333333333333333333333333333333333333333333333
ddd3333dd333333dd33336d3333333333333333dd33336d33333333333333333333bbbb3333bbbb333333333333333333333333333333333333333333333333d
ddd33ddedd333333dd336d333333333333333333dd336d3333333333333333322228888222288883333333333333333333333333333333333333333333333dde
ddd3333ddd3333333dd6d33333333333333333333dd6d3333333333333333332222888e2222888e333333333333333333333333333333333333333333333333d
3333deedd333333336ddd333333333333333333336ddd33333333333333333328e2888828e28888333333333333333333333333333333333333333333333deed
33333dd3333333336d33dd3333333333333333336d33dd3333333333333333328822888288228883333333333333333333333333333333333333333333333dd3
333dddd333333336d3333dd33333333333333336d3333dd33333333333333332228e2222228e22233333333333333333333333333333333333333333333dddd3
333dd3333333333d333333d3333333333333333d333333d3333333333333333222882222228822233333333333333333333333333333333333333333333dd333
33333333333ddd3dd33333633333333333333333333bbbb3333bbbb3333bbbb2222228e2222228e33333333333333333333333333333333333333333ddd33333
33333333333dd33dddd336d33333333333333333333bbbb3333bbbb3333bbbb228e2288228e22883333333333333333333333333333333333333333dd3333333
33333333333d3333ddd36d33333333333333333222288882222888822228888228822222288222233333333333333333333333333333333333333ddedd333333
33333333333333333dd6d3333333333333333332222888e2222888e2222888e2222288e2222288e3333333333333333333333333333333333333333ddd333333
333333333333333d36ddd33333333333333333328e2888828e2888828e2888828e2288828e228883333333333333333333333333333333333333deedd3333333
33333333333333336d33dd33333333333333333288228882882288828822888288228882882288833333333333333333333333333333333333333dd333333333
3333333333333336d3333dd33333333333333332228e2222228e2222228e2222228e2222228e222333333333333333333333333333333333333dddd333333333
333333333333333d333333d33333333333333332228822222288222222882222228822222288222333333333333333333333333333333333333dd33333333333
333bbbb3333bbbb3333bbbb3333bbbb3333bbbb3333bbbb2222228e22776777766666222222228e3333333333333333333333333333333333333333333333333
333bbbb3333bbbb3333bbbb3333bbbb3333bbbb3333bbbb228e22882777667777666662228e22883333333333333333333333333333333333333333333333333
22288882222888822228888222288882222888822228888228822227777766777666666228822223333333333333333333333333333333333333333333333333
222888e2222888e2222888e2222888e2222888e2222888e2222288e77766666666d66662222288e3333333333333333333333333333333333333333333333333
8e2888828e2888828e2888828e2888828e2888828e2888828e228887776666666d6666628e228883333333333333333333333333333333333333333333333333
88228882882288828822888288228882882288828822888288228886666666ddd666666288228883333333333333333333333333333333333333333333333333
228e2222228e2222228e2222228e2222228e2222228e2222228e22266666666666776662228e2223333333333333333333333333333333333333333333333333
228bbbbbbbbbbbbb22882222228822222288222222882222228bbbbaaaaaaaaa6d666dd2228822233333333333333333333aaaaaaaaaaaaa3333333333333333
bbbbbbbbbbbbbbbbbbbbbbabb22228e2222228e222bbaaabbbbbbbaaaaaaaaabaaaaaaaba7766623333bbbb333aaaaaaaaabbbbbbbbbbbbbaaaaaaaaa3333333
bbb33bb33bb33bb3bbbbbbbbbbabbbbbbbabbbbbaaabbbbbbba33b3bbbbbbb33aabbbbbaaaaaaaaaaaaaaaaaaabbbbbbbbb3333333333333bbbbbbbbbaaaaaaa
3bb333333bb333333bb333333bbbbbbbbbbbbbbbbb3333333b33333bbbbbb333333b333abaaaaabbbbbbbbbbbb3333333333333333333333333333333bbbbbbb
3333bbb33333bbb33333bbb33333bbb33333bbb3333333b3333333bbbbbb33b333abb33bbbbbbb33333bbbb33333333333333333333333333333333333333333
bb33bbb3bb33bbb3bb33bbb3bb33bbb3bb33bbb33b33b3333b33b33bbbb33bb33bbb333bbbbb3333bb3bbbb33333333333333333333333333333333333333333
bb33bbb3bb33bbb3bb33bbb3bb33bbb3bb33bbb3bb33bbb3bb33bbbbbbb3bbb33bb3333bbbb33b33bb33bbb33333333333333333333333333333333333333333
33bb333333bb333333bb333333bb333333bb3333333b3333333b3333bbbbbb3333333333bb33b33333bb33333333333333333333333333333333333333333333
33bb333333bb333333bb333333bb333333bb333333bb333333bb33333b333333333333333bbb333333bb33333333333333333333333333333333333333333333

__gff__
0001000000000000000000000000000000010082001200008a080000800000008808088400028a8a0000008a0b0202008a0a828200000a0a000000080808080000010100000301000000030000000000010101000000000000000000000000000101010100000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
__map__
08131823263032454a0b0c2b0d00000000000000000000000000000000000000000000000000000000000000000000000000000000005e5e5e0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000004a4a4a00000000000000000000000026000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000002000000000000000000000000000580000005900000000000000000000464646464a4a4a00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000130000000000464646464600000000002600000058000000000059002b0000000000000000000000000000000000000000000000002300000000000000000000000000002b0000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000056000056560000001846464646461800000000001846464a0000000000000000000000000000000000000000000046464600000000000000000000000000414100000000000046464a4a461800000000000000000000000000260000000000000000000000300000
000000000000000000000000000000000000000000000000560000565600000000005600000059000000000058005600000000230000000000000000000000000000000000000000000000000026000000000000000042424400490000000000000000000000000000000000000000004646460000004a464a00000046464600
00000000000000000000000000000000000000000000260056000056564a4a4a00005600000000590000005800005600000046464600000000000000000000000000000000000000000000004a4a4618000000434426424241414100000000000000000000006200000000000000000000560000000000560000000000560000
0020000000000000000000000000000000000000184646464600005656560000000056000000000046464600000056000000005600000000414100000000000000000000000000575500000000000000000041414141424248474200000049000000004141414141000000000000000000560000000000560000000000560000
4141410000000000000000000000000000000000005600005600005656000000000056000043443046464600000056000000005600000000424200434400000000000000000045414145000000000000000042484747474747474700004141000000004242424242000000000000000000560000000000560000000000560000
42424241434455000000000000000057000000000056000056000041410000004141410000414141414141000026560000000056002b0000424241414100000000000000004500424200450000000000000047474747474747474700004242443000624242424141414141260000004d4f560000000000560000000000560000
6342634241414145454545454545454141414100005600004141414242000000424242474742634250514241414141414141414141414141505152634141414155000045450000634200004545000057003047474747474742425051414142414141414142634242634241414141414141414118181846414618181841414141
6363634252424200000000000000006342424141414141414142505142000000426342474763424260615051424263425242424263425051606142424263424241454500000000636300000000454541414142424242424250516061524242424242426342424263636342424242424242424141414141424141414141424242
5263505142634253543200003200536363524242424242426363606152413254424242474763634242526061426363634242424263636061424242526363634242540032000000526353540000005463634242524242425260615252525242424242636363425051636363424263424242424242424242424242424242424242
__sfx__
000500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
012800000742413441134421344500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000200
01100000242551f3751f3251f315004020c400004020c4000c0000c00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
011000000c0750c122376130a2250c07516215376131b215110551300013055131151105011112130551a2250c0550000037613000000c0550000037613182251605500000130550000011055000000f05500000
01200000272142721027221272202723127232272322723226230262302623226232222302223022232222321f2301f2301f2321f2321f2211f2221f2221f2221d2201d2201d2221d2221f2101f2102221022210
012000001f2301f2301f2321f2321f2321f2321f2321f2321d2201d2201d2221d2221f2101f210222102221000000000000000000000000000000000000000000000000000000000000000000000000000000000
011000002735522255273252222527315222150000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
011000002b13137000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010c00001f0501c050000001f05021050000002305024050240502400028052280210000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010c00003525013231000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
01180000130531f2551f2251f2151f2051f205000001f205000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
011000002d13100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
011000001f04300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010400000463500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010400001c63500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
01100000100531f6672b6203761000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010a00000465500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010400003462434625000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010400001c6241c625000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010800001d62534425344050000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010800001d6252b4251d6053440500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
011000003735530325000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
011000001d3411d325000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
011000002945200000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
011000003045230422304120000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010800001365407645136541f6521f6442b6321f6241f614000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010c00001f67413153000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
01040000100541c0211c0050000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
01040000130541f021000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010800001803518015000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010400001f11407111000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
011800003745500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
01100000072511f221000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0110000018335183321f1351f43518335242321f3351f31518335242351f1351b4321a335162321a3351a315183350c2321f1351f43518335242321f3351f31518335242351f1351b4321a335162321a3351a315
011000000c053000000000034625356150000035615000000c053000000000000000346250000000000000000c053000000000034625356150000035615000000c05300000000000000034625000000000000000
0110000014335143321f1351f43518335242321f3351f31514335142351f1351b4321a335162321a3351a31514335143321f1351f43518335242321f3351f31514335142351f1351b4321a335162321a3351a315
011000001833500000000001f3351d3350000018335183051b3350000000000000001d3351d3051f335000002033520315000001b3051b3351b3151b30500000223352231500000000001d3351d3150000000000
011000001833500000000001f3351d3350000018335183051b3350000000000000001d3351d3051f3350000020335203051b3051b3351b3052030520335000001f33522305000001a3351d3051d3050000000000
011000001b0201b0201b0201b0201802118020180201802018010180101801500000000000000000000000001d0201d0201d0201d020180211802018020180201801018010180150000000000000000000000000
011000001f0201f0201f0201f0201f0221f0221f0221f0221f0121f0121f0121f0151f0021f0021f0021f0021d0201d0201d0201d0201d0201d0201d0201d0201d0101d0101d0101d0151d0001d0001d0001d000
011000001b0201b0201b0201b0201f0211f0201f0221f0221f0221f0221f0111f0102202022020240202402022020220201d0201d0202002020020200222002220011200101d0201d0201f0201f0201f0221f022
01100000180201802018020180201802218022180221802218011180101801018010180001800018000180001d0211d0201d0201d0201d0221d0221d0221d0221d0111d0101d0101d0101d0001d0001d0001d000
011000001b2751b2151f2751f2251f2251f2051b00000005243052420524105000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
__music__
01 03424344
00 03424344
00 03044244
02 03054344
00 41424344
00 41424344
00 41424344
00 41424344
01 25224344
01 24224344
01 21224344
00 21224344
00 23224344
00 23224344
00 21222644
00 21222744
00 23222844
02 23222944

