pico-8 cartridge // http://www.pico-8.com
version 19
__lua__
-- curse of the lich king
-- created by @johanpeitz
-- audio by @gruber_music

-- special thanks to all testers 
-- at the megadome discord

-- sorry for uncommented code,
-- had to strip it out to make
-- the whole thing fit! 😐
--  / johan

function _init()
m_name, m_anim, m_col, m_prop, m_hp, m_atk, m_range, m_sight, m_depth, m_itemchance = explode("hero,a rat,a ghoul,pot,chest,shelves,door,door,altar,gate,lever,gate,spikes,an eyeball,an imp,box,a troll,pot,a skeleton,anvil,body,a toxic rat,a ghost,^raq'zul,well"),explodeval("240,192,224,29,13,15,4,5,25,7,31,9,47,228,244,212,208,27,196,24,11,216,200,248,22"),explodeval("8,8,12,7,7,7,7,7,7,7,7,7,7,14,14,9,3,7,7,7,7,11,7,1,7"),explodeval("0,0,0,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,0,1,1,0,0,0,1"),explodeval("6,1,6,1,1,1,1,1,1,1,1,1,1,10,10,15,22,1,12,1,1,4,12,45,1"),explodeval("3,1,1,0,0,0,0,0,0,0,0,0,5,0,1,0,3,0,2,0,0,0,1,4,0"),explodeval("1,1,1,1,0,0,0,0,0,0,0,0,0,4,1,1,1,1,1,0,0,1,1,4,0"),explodeval("4,3,3,0,0,0,0,0,0,0,0,0,0,5,3,3,3,0,4,0,0,3,4,5,0"),explodeval("0,1,2,0,0,0,0,0,0,0,0,0,3,5,3,99,6,0,4,0,0,4,7,99,0"),explodeval("0,0,0,4,10,2,0,0,0,0,0,0,0,0,0,0,0,4,0,0,4,0,0,0,0")
i_name, i_type, i_spr, i_atk, i_heal, i_hpmax, i_depth = explode("apple,carrot,bread,soup,steak,vial,potion,stick,dagger,scimitar,battleaxe,spear,broadsword,club,sword"),explodeval("0,0,0,0,0,0,0,1,1,1,1,1,1,1,1"),explodeval("0,0,0,0,0,0,0,76,92,100,120,104,96,108,124"),explodeval("0,0,0,0,0,0,0,1,2,6,3,7,8,5,4"),explodeval("1,2,5,10,15,0,0,0,0,0,0,0,0,0,0"),explodeval("0,0,0,0,0,1,3,0,0,0,0,0,0,0,0"),explodeval("1,2,3,4,5,2,4,1,2,6,3,7,8,5,4")
i_trait,i_status={"","cursed ","holy "},explode" , of blight, of confusion, of the basilisk, of the leech"
dirx,diry=explodeval"-1,1,0,0,-1,1,1,-1",explodeval"0,0,-1,1,1,-1,1,-1"
dpal,lpal=explodeval"0,0,1,5,1,1,13,6,4,4,9,3,13,5,1,1",explodeval"1,5,4,11,9,13,7,7,14,10,7,7,7,6,15,7"
_glyphs,_kerning,fdat={},{},[[  0000.0004! 0000.824c" 0000.0168# 0000.0204$ 0001.7490% 0002.1508& 0000.2f40' 0000.004c( 0008.9252) 0005.248a* 0000.5540+ 0000.2e80, 0004.8004- 0000.0e00. 0000.8004/ 0004.a5200 0001.5b501 0001.24d22 0003.95183 0001.c5184 0002.4f685 0001.c6786 0001.56507 0001.29388 0001.55509 0001.4d50: 0000.8204; 0004.8204< 0002.22a0= 0000.71c0> 0000.a888? 0001.0518@ 0001.1b50^a0002.fb50^b0001.d758^c0003.1270^d0001.db58^e0003.9678^f0000.b278^g0003.5270^h0002.df68^i0000.924c^j0005.2492^k0002.d768^l0003.9248^m0002.dfe8^n0002.db58^o0001.5b50^p0000.bb58^q0011.5b50^r0002.bb58^s0001.c470^t0001.24b8^u0003.5b68^v0001.5b68^w0002.ff68^x0002.d568^y000a.6b68^z0003.9538[ 000c.925a\ 0012.2448] 000d.249a^^0000.0150_ 0003.8000` 0000.008aa 0003.5b80b 0001.dac8c 0001.1282d 0003.5ba0e 0001.1e80f 0004.b252g 000e.7b80h 0002.dac8i 0000.920cj 0005.2412k 0002.d748l 0000.924cm 0002.dec0n 0002.dac0o 0001.5a80p 0005.dac0q 0013.5b80r 0000.9740s 0001.a2c2t 0001.12cau 0003.5b40v 0001.5b40w 0002.fb40x 0002.a540y 000e.6b40z 0001.94c2{ 0019.22b0| 0004.924c} 000d.2898~ 0000.1f00^*0000.7bc0]]
for i=0,#fdat/11 do
local p=1+i*11
local char=sub(fdat,p,p+1)
_glyphs[char],_kerning[char]=
tonum("0x"..sub(fdat,p+2,p+10)),4
end
swap_state(title_state)
end
function level_init()
make_player()
pl.depth,pl.win,inventory=1,false,{}
inventory.items,sleep=0,0
give_item(make_item(8),true)
give_item(make_item(1),true)
hud=add_window(0,115,{" "},128)
hud.hud=true
make_new_level()
music"2"
if pl.depth==1 then
add_modal(10,10,
explode"^chills run down your spine as,you enter the ^lich ^king's lair.,,^you have travelled light but,hopefully you will find more,supplies hidden down here.,,^rumor has it ^raq'zul resides on,the 8th floor. ^descend his lair,and destroy him!"
,108)
end
end
function make_new_level()
phase,hash,entities,particles,floaters,fog,floater_delay,num_rooms=0,{},{},{},{},{},0,2+pl.depth*2
for x=0,127 do
fog[x]={}
for y=0,31 do
fog[x][y]=2
hash[y]={}
end
end
generate_level(num_rooms)
camtx,camty,move_cam=pl.tx,pl.ty,false
update_fog()
if (pl.depth>1) show_alert("^floor "..pl.depth,nil,60).delay=30
end
function make_entities()
for i=0,127 do
for j=0,31 do
local tile=mget(i,j)
if tile==17 then
pl.tx,pl.ty=i,j
add(entities,pl)
else
local id=indexof(m_anim,mget(i,j))
if id>0 then
if m_prop[id]==1then
tm=make_prop(id,i,j)
mset(i,j,1)
else
tm=make_mob(id,i,j)
mset(i,j,1)
end
add(hash[tx2hash(tm.tx)],tm)
end
end
end
end
end
function make_player(tx,ty)
pl=make_e(1,{
mob=true,
fs=2,
weapon=nil,
tx=tx, ty=ty,
logic=player_logic
})
end
function player_logic(e)
if e.hp>0 then
if e.paralyzed>0 then
phase+=1
move_anim(e,0,0,0)
else
if e.confused>0 then
next_btn=intrnd(4)
end
if next_btn>=0 and next_btn<4 then
if move_entity(e,dirx[next_btn+1],diry[next_btn+1]) then
update_fog()
phase+=1
end
elseif next_btn==5 then
show_inventory()
end
end
next_btn=-1
else
if e.hit_count==0 then
e.hit_count=-1
add_modal(32,32,{
"",
"^killed by "..pl.hit_by.name,
"on floor "..pl.depth..".",
""
},nil,function ()
fadeout()
swap_state(title_state)
end)
end
end
end
function make_prop(id,tx,ty)
local p=make_e(id,{
prop=true,
tx=tx, ty=ty,
walkable=false
})
p.block_los = id==7 or id==8
if id==5 or id==9 or id==20 or id==25 then
p.uf=function(p)
if chance(0.03) and not p.used then
add_rising_particle(p,explodeval"5,6,13,7,12")
end
update_entity(p)
end
end
if id==13 then
p.trap,p.walkable=true,true
p.logic=function(e)
local m=get_entity(e.tx,e.ty,"mob")
if m then
change_entity_hp(m,e,-e.atk)
e.logic=nil
e.frames[1]-=1
sfx"54"
end
end
end
return p
end
function make_mob(id,tx,ty)
m=make_e(id,{
mob=true,
fs=4,
dir=chance"0.5" and 1 or -1,
tx=tx, ty=ty,
logic=wait_logic
})
if id==15 or id==24 then
m.after_attack=blink
end
return m
end
function confused_logic(e)
local options={}
for i=1,4 do
if is_floor(mget(e.tx+dirx[i],e.ty+diry[i])) then
add(options,i)
end
end
if #options>0 then
local d=rnd_elem(options)
move_entity(e,dirx[d],diry[d])
e.fc,e.frame=0,0
else
end
if e.confused==0 then
e.logic=chase_logic
end
end
function paralyzed_logic(e)
if e.paralyzed==0 then
e.logic=chase_logic
wait_logic(e)
end
end
function flee_logic(e)
local bdist,bdir=0,0
for i=1,4 do
local dstx=e.tx+dirx[i]
local dsty=e.ty+diry[i]
if is_walkable(dstx,dsty,"move") then
local dist=distance(pl.tx,pl.ty,dstx,dsty)
if dist>bdist then
bdir,bdist=i,dist
end
end
end
if bdir>0 then
move_entity(e,dirx[bdir],diry[bdir])
e.fc,e.frame=0,0
end
local dist=distance(e.tx,e.ty,pl.tx,pl.ty)
if (dist>e.sight) e.logic=wait_logic
end
function chase_logic(e)
e.attacked=false
local cansee=false
if los(e.tx,e.ty,pl.tx,pl.ty,e.sight) then
e.gx,e.gy=pl.tx,pl.ty
cansee=true
end
local dist=distance(e.tx,e.ty,pl.tx,pl.ty)
local perp=e.tx-pl.tx==0 or e.ty-pl.ty==0
if dist<=e.range and perp and cansee then
if e.id==24 then
if dist==1 then
e.atk=5
change_entity_hp(e,e,2)
else
e.atk=1
end
end
attack_entity(e,pl)
e.attacked=true
local dx=ssgn(pl.tx-e.tx)
local dy=ssgn(pl.ty-e.ty)
move_anim(e,dx,dy,4)
if e.id==22 then
pl.poisoned+=3
elseif e.id==23 and chance"0.5" then
pl.paralyzed=2
end
if e.range>1 then
if e.id==14 or (e.id==24 and dist>1) then
pl.confused+=2
sfx"55"
local x,y=e.tx,e.ty
for i=0,dist,0.125 do
add_particle(
(x+i*dx)*8+3,
(y+i*dy)*8+3,
0,0,2+rnd(5),
explodeval("8,9,10,7")
)
end
sleep=20
end
end
else
local bdist,bdir=999,intrnd"4"+1
for i=1,4 do
local dstx,dsty=e.tx+dirx[i],e.ty+diry[i]
if is_walkable(dstx,dsty,"move") then
dist=distance(e.gx,e.gy,dstx,dsty)
if dist<bdist then
bdir,bdist=i,dist
end
end
end
move_entity(e,dirx[bdir],diry[bdir])
e.fc,e.frame=0,0
if e.tx==e.gx and e.ty==e.gy then
sfx"56"
add_floater("?",e,10)
e.logic=wait_logic
end
end
end
function wait_logic(e)
if (distance(e.tx,e.ty,pl.tx,pl.ty)>e.sight) return
if los(e.tx,e.ty,pl.tx,pl.ty,e.sight) then
sfx"57"
add_floater("!",e,10)
e.gx,e.gy,e.logic=pl.tx,pl.ty,chase_logic
if (e.id==16) e.logic=flee_logic
end
end
function move_anim(e,dx,dy,dist)
e.mt,e.odist,e.ox,e.oy,e.dx,e.dy=0,dist,dist*dx,dist*dy,dx,dy
if dist!=0 then
e.fc,e.frame=0,0
end
if e.confused>0 then
e.dir=rnd_elem({1,-1})
else
if (dy==0) e.dir=dx<0 and -1 or 1
end
end
function tx2hash(tx)
return flr(tx/8)
end
function addhash(e)
add(hash[tx2hash(e.tx)],e)
end
function delhash(e)
del(hash[tx2hash(e.tx)],e)
end
function move_entity(e,dx,dy)
local dstx,dsty=e.tx+dx,e.ty+dy
local tile=mget(dstx,dsty)
if is_walkable(dstx,dsty,"move") then
delhash(e)
e.tx,e.ty=dstx,dsty
addhash(e)
move_anim(e,dx,dy,-8)
sfx(62+e.t%2)
return true
elseif e==pl then
move_anim(e,dx,dy,3)
if interact(e,dstx,dsty) then
return true
end
sfx"37"
end
return false
end
function is_walkable(tx,ty,mode)
local tile=mget(tx,ty)
if mode=="los" then
if not fget(tile,1) then
local e=get_entity(tx,ty)
if (not e) return true
if (e.block_los) return false
return true
end
return false
elseif mode=="move" then
if not fget(tile,0) then
local e=get_entity(tx,ty,"mob")
if not e then
e=get_entity(tx,ty)
if (not e) return true
if (e.walkable) return true
end
end
return false
end
return false
end
function get_entity(tx,ty,typ)
local arr=hash[tx2hash(tx)]
for e in all(arr) do
if e.hp>0 and e.tx==tx and e.ty==ty then
if typ and not e[typ] then
else
return e
end
end
end
return nil
end
function interact(mob,tx,ty)
local e=get_entity(tx,ty,"mob")
if (e) return attack_entity(mob,e)
e=get_entity(tx,ty)
if (not e) return false
if (e.used) return false
if e.id==20 then
if not tried_anvil then
tried_anvil=true
return add_modal(22,28,explode"^a mighty anvil...,,^spend a turn on this,hefty piece to bestow,a wepaon with special,powers - if you have,the right items.")
end
if pl.weapon==nil then
show_alert("^equip weapon to alter...")
return false
elseif pl.weapon.trait!=1 or pl.weapon.status!=1 then
show_alert("^weapon must be untampered...")
return false
end
local foods={}
for i=1,8 do
local itm=inventory[i]
if itm and itm.type==0 and itm.trait==2 then
add(foods,itm)
end
end
if #foods==0 then
show_alert("^cursed food required...")
return false
end
discard_item(rnd_elem(foods),true)
if chance(1-0.05*pl.weapon.atk) then
sfx"32"
sleep=24
enchant_item(pl.weapon,true)
show_alert(item_name(pl.weapon),1).delay=30
else
sfx"38"
sleep=10
show_alert(pl.weapon.name.." shattererd!")
discard_item(pl.weapon,true)
end
end
if e.id==25 then
if not tried_well then
tried_well=true
return add_modal(22,28,explode"^a shallow pool...,,^the strangely clear water,has the power to free,a weapon from its curse.,^it only costs a turn")
end
if not pl.weapon or pl.weapon.trait!=2 then
show_alert("^dip a cursed weapon in the pool...")
return false
else
show_alert("^"..pl.weapon.name.." is no longer cursed!")
sleep,e.frames[1],e.used,pl.weapon.trait=10,21,true,1
sfx"46"
end
end
if e.id==9 then
if not tried_altar then
tried_altar=true
return add_modal(22,28,explode"^a holy altar...,,^the book on the table has,the answers you need.,^spend a turn here to,identify a mysterious item.")
end
local unids={}
for i=1,8 do
local itm=inventory[i]
if itm and not itm.identified then
add(unids,itm)
end
end
if #unids>0 then
local itm=rnd_elem(unids)
sfx(45+itm.trait)
sleep,itm.identified=10,true
show_alert(item_name(itm),3)
else
show_alert("^nothing to identify...")
return false
end
end
if e.id==7 or e.id==8 then
sfx(61)
delhash(e)
del(entities,e)
end
for id in all(explodeval"4,5,6,18,21") do
if e.id==id and not e.used then
local itmid,csnd
if id==5 then
csnd,itmid=49,get_depth_item(explodeval("8,9,10,11,12,13,14,15"))
elseif id==6 then
csnd=60
if e.hasitem then
itmid=get_depth_item({6,7})
end
else
csnd=id==21 and 60 or 50
if e.hasitem then
itmid=get_depth_item(explodeval("1,2,3,4,5"))
elseif chance"0.1" then
addhash(make_mob(chance(0.7) and 2 or 22,tx,ty))
end
end
if itmid then
if inventory.items<8 then
give_item(make_item(itmid))
e.used=true
else
show_alert("^can't carry any more...")
return sfx"37"
end
else
e.used=true
end
if e.used then
sfx(csnd)
e.frames[1]-=1
if (id==4 or id==18 or id==21) e.walkable=true
end
end
end
return true
end
function get_depth_item(selection)
local pool={}
for id in all(selection) do
if (i_depth[id]<=pl.depth) add(pool,id)
end
return rnd_elem(pool)
end
function win_game()
music"-1"
sfx"6"
for i=1,50 do
flip()
end
fadeout()
music"23"
add_modal(8,24,
explode("^as the ^lich king lets out a final;scream, something shifts in the;air. ^you feel lighter, as if an;invisible burden has lifted.;;^raq'zul is defeated and you;return to the surface knowing;that the world has become;a better place.",";")
,112,function ()
pl.win=false
music"-1"
fadeout()
swap_state(title_state)
end
)
end
function add_floater(str,e,c,delay)
if (delay) floater_delay+=delay
add(floaters,{
str=str,
e=e,
dy=1,
oy=0,
c=c,
life=25,
delay=floater_delay
})
if (not delay) floater_delay+=2
end
function level_update()
if sleep==0 then
if phase==0 then
pl.logic(pl)
elseif phase==2 then
if mget(pl.tx,pl.ty)==16 then
sfx"53"
fadeout()
pl.depth+=1
return make_new_level()
elseif pl.win then
win_game()
else
for e in all(entities) do
if not e.trap and e!=pl and e.hp>0 and e.logic then
update_entity_status(e)
e.logic(e)
end
end
for e in all(entities) do
if (e.trap and e.logic) e.logic(e)
end
phase+=1
if pl.confused>0 or pl.paralyzed>0 then
sleep=20
end
end
end
if phase==3 then
local atk,move_on=false,true
for e in all(entities) do
if (e.mt<1) move_on=false
if (e.attacked) atk=true
end
if (not atk or move_on) phase+=1
end
else
sleep-=1
end
for e in all(entities) do
if e.uf(e) then
if e==pl and phase==1 then
phase+=1
end
end
end
if phase>3 then
sort(entities,"ty")
phase=0
update_entity_status(pl)
end
for p in all(particles) do
if (p.uf) p.uf(p)
p.life-=1
if (p.life<=0) del(particles,p)
end
if (floater_delay>0) floater_delay-=1
for f in all(floaters) do
if f.delay>0 then
f.delay-=1
else
f.oy-=f.dy
f.dy*=0.8
f.life-=1
if (f.life<=0) del(floaters,f)
end
end
end
function level_draw()
local dz=2
if pl.tx<camtx-dz then
camtx-=1
move_cam=true
elseif pl.tx>camtx+dz then
camtx+=1
move_cam=true
elseif pl.ty<camty-dz then
camty-=1
move_cam=true
elseif pl.ty>camty+dz then
camty+=1
move_cam=true
end
if move_cam then
camera((camtx-8)*8+pl.ox+4,
(camty-8)*8+pl.oy+4)
else
camera((camtx-8)*8+4,
(camty-8)*8+4)
end
if (pl.ox==0 and pl.oy==0) move_cam=false
cls()
fmap(camtx-8,camty-8)
for e in all(entities) do
e.df(e)
end
for p in all(particles) do
pset(p.x,p.y,rnd_elem(p.c))
end
for f in all(floaters) do
if f.delay==0 then
pr(f.str,
f.e.tx*8-tlen(f.str)/2+4+f.e.ox,
f.e.ty*8-4+f.oy+f.e.oy,
f.c,1)
end
end
camera()
spr(174,1,0,2,2)
local col,btny=12,6
if inventory.items==8 then
btny+=sin(time())+1
col=8
end
if pl.hp>0 and not modal_top then
pr("[*]",16,7,5 )
pr("[*]",16,btny,col )
end
end
last_pal=0
function set_pal(p)
if (p==last_pal) return
if not p then
pal()
pal(14,1)
else
for i=1,16 do
pal(i-1,p[i])
end
end
last_pal=pal
end
function fmap(tx,ty)
for x=tx,min(127,tx+16) do
for y=ty,min(31,ty+16) do
if x>=0 and y>=0 then
local fv=fog[x][y]
if fv==1 then
set_pal(dpal)
elseif fv==0 then
set_pal()
end
local tile=mget(x,y)
if fv!=2 and tile>0 then
spr(tile,x*8,y*8)
end
end
end
end
pal()
fillp()
end
function los(x1,y1,x2,y2,sight)
local frst,sx,sy,dx,dy=true
local dist=distance(x1,y1,x2,y2)
if (dist>sight) return false
if (dist==1) return true
if x1<x2 then
sx,dx=1,x2-x1
else
sx,dx=-1,x1-x2
end
if y1<y2 then
sy,dy=1,y2-y1
else
sy,dy=-1,y1-y2
end
local err,e2=dx-dy,nil
while not(x1==x2 and y1==y2) do
if not frst then
if not is_walkable(x1,y1,"los") then
return false
end
end
frst,e2=false,err+err
if e2>-dy then
err-=dy
x1+=sx
end
if e2<dx then
err+=dx
y1+=sy
end
end
return true
end
function inbounds(tx,ty)
if (tx<0 or ty<0 or tx>=127 or ty>=31) return false
return true
end
function update_fog()
local r,px,py=pl.sight+1,pl.tx,pl.ty
for ty=py-r,py+r do
for tx=px-r,px+r do
if inbounds(tx,ty) then
if fog[tx][ty]==0 then
local d=distance(px,py,tx,ty)
if (d<r+1) fog[tx][ty]=1
end
if los(px,py,tx,ty,pl.sight) then
fog[tx][ty]=0
end
end
end
end
end
level_state={
init=level_init,
update=level_update,
draw=level_draw
}
function round(x)
return flr(x+0.5)
end
function chance(x)
return rnd()<x+0
end
function add_params(src,dst)
for k,v in pairs(src) do
dst[k]=v
end
end
function strchr(str,c)
for i=1,#str do
if (sub(str,i,i)==c) return i
end
return -1
end
function indexof(a,e)
for i=1,#a do
if (a[i]==e) return i
end
return -1
end
function ssgn(x)
return x<0 and -1 or x>0 and 1 or 0
end
function distance(ax,ay,bx,by)
local dx,dy=ax-bx,ay-by
return sqrt(dx*dx+dy*dy)
end
function sort(a,p)
for i=1,#a do
local j = i
while j > 1 and a[j-1][p] > a[j][p] do       a[j],a[j-1] = a[j-1],a[j]
j = j - 1
end
end
end
function intrnd(r)
return flr(rnd(max(0,r)))
end
function rnd_elem(a)
return a[intrnd(#a)+1]
end
function explode(s,delim)
if (not delim) delim=","
local retval,lastpos={},1
for i=1,#s do
if sub(s,i,i)==delim then
add(retval,sub(s, lastpos, i-1))
i+=1
lastpos=i
end
end
add(retval,sub(s,lastpos,#s))
return retval
end
function explodeval(_arr)
return toval(explode(_arr))
end
function toval(_arr)
local _retarr={}
for _i in all(_arr) do
add(_retarr,flr(_i+0))
end
return _retarr
end
function tlen(str)
local l,i=0,1
while i<=#str do
local char=sub(str,i,i)
if char=="^" then
char=sub(str,i,i+1)
i+=1
else
char=char.." "
end
l+=_kerning[char]
i+=1
end
return l
end
function pr(str,x0,y0,c1,c2)
local x1,i=x0,1
while i<=#str do
local char=sub(str,i,i)
if char=="\n" then
y0+=7
x1=x0
else
if char=="^" then
char=sub(str,i,i+1)
i+=1
else
char=char.." "
end
local px,k=_glyphs[char],4
for j=1,2 do
px=shr(px,1)
if (band(px,0x0.0001)>0) k-=j
end
for y=0,5 do
for x=0,2 do
px=shr(px,1)
if band(px,0x0.0001)>0 then
pset(x1+x,y0+y,c1)
if (c2) pset(x1+x,y0+y+1,c2)
end
end
end
x1+=k
_kerning[char]=k
end
i+=1
end
end
next_btn, state, next_state, change_state ,fade_progress,fade_pal= -1, {}, {}, false,1,explodeval"0,0,1,5,1,1,13,6,4,4,9,3,13,5,4,4"
function swap_state(_s)
next_state,change_state =_s,true
end
function _update()
if next_btn<0 then
for i=0,5 do
if (btnp(i)) next_btn=i
end
end
if change_state then
state, change_state, windows = next_state, false,{}
state.init()
end
if update_windows() then
state.update()
end
if fade_progress>0 then
fade_progress-=0.075
end
end
function update_fade()
for i=0,15 do
local col,k=i,6*fade_progress
for j=1,k do
col=fade_pal[col+1]
end
pal(i,col,1)
end
end
function fadeout()
while fade_progress<1 do
fade_progress=min(fade_progress+0.05,1)
update_fade()
flip()
end
end
function _draw()
state.draw()
camera()
draw_windows()
update_fade()
end
function make_e(id,params)
local e={
id=id,
name=m_name[id],
col=m_col[id],
sight=m_sight[id],
range=m_range[id],
hp=m_hp[id],
hpmax=m_hp[id],
atk=m_atk[id],
hasitem=chance(m_itemchance[id]/10),
lvl=1,
poisoned=0,
confused=0,
paralyzed=0,
t=0,
mt=0,
tx=0, ty=0,
ox=0, oy=0,
dx=0, dy=0,
odist=0,
dir=1,
xp=0,
hit_count=0,
df=draw_entity,
uf=update_entity,
fc=0,
fs=0,
frame=0,
frames={m_anim[id]},
}
if (params) add_params(params,e)
if e.mob then
for i=1,3 do
add(e.frames,m_anim[id]+i)
end
end
add(entities,e)
return e
end
function xp_req(lvl)
local xp=0
for i=1,lvl do
local ii=i+1
xp+=ii*ii-4
end
return xp
end
function give_xp(e,xp)
e.xp+=xp
floater_delay+=10
add_floater("+"..xp,e,12)
while e.xp>=xp_req(e.lvl+1) do
level_up(e)
end
end
function level_up(e)
sfx"52"
e.lvl+=1
e.atk+=1
e.hpmax+=3
e.hp=pl.hpmax
floater_delay+=10
add_floater("^level up",e,12,5)
add_floater("^a^t^k +1",e,12,25)
add_floater("^max ^h^p &+3",e,12,25)
add_floater("^h^p refilled!",e,12,25)
end
function get_dmg(e)
return e.atk+(e.weapon and e.weapon.atk or 0)
end
function attack_entity(a,d)
if change_entity_hp(d,a,-get_dmg(a)) then
if a==pl then
give_xp(a,1)
if d.id==16 then
local itm=make_item(1+intrnd"5",3)
itm.identified=true
give_item(itm)
elseif d.id==24 then
pl.win,d.frames,d.fc,d.frame=    true,{252},0,0
end
end
sfx"58"
else
sfx"59"
end
if a.weapon then
local aws=a.weapon.status
if aws==2 then
d.poisoned+=3
elseif chance"0.5" and aws==3 and d.id!=24 then
d.confused+=5
d.logic=confused_logic
elseif chance"0.3" and aws==4 and d.id!=24 then
d.paralyzed+=2
d.logic=paralyzed_logic
elseif chance"0.3" and aws==5 then
change_entity_hp(a,a,1)
end
if a.weapon.trait==2 then
a.weapon.atk-=1
if a.weapon.atk<=0 then
local itm=a.weapon
show_alert(itm.name.." shattered!")
itm.trait=1
unequip_item(itm,true)
discard_item(itm,true)
sfx"38"
end
end
end
return true
end
function change_entity_hp(de,se,dmg)
de.hp=min(de.hp+dmg,de.hpmax)
de.hit_count,de.hit_by=10,se
if dmg!=0 then
if dmg>0 then
add_floater("+"..dmg,de,11,10)
else
add_floater(""..dmg,de,8)
end
end
if dmg<0 and se.tx then
for i=1,8 do
add_particle(
de.tx*8+2+rnd(4),
de.ty*8+3+rnd(2),
ssgn(de.tx-se.tx)*(1+rnd(1)),
ssgn(de.ty-se.ty)*(0.3+rnd(0.3)),
5+rnd(5),
{de.col},
function(p)
p.x+=p.dx
p.dx*=0.8
p.y+=p.dy
p.dy+=0.1
end
)
end
end
if de==pl and pl.hp<=0 then
sfx"51"
music"24"
pl.hit_count,pl.fs,pl.frames=100,5,explodeval"232,232,232,232,232,232,232,232,233,234,235"
end
return de.hp<=0
end
function blink(e)
if (e.hp<=0) return
for i=0,7 do
add_rising_particle(e,explodeval"15,15,14,7")
end
delhash(e)
local moved=false
while not moved do
local dx,dy=intrnd"7"-3,intrnd"7"-3
local dstx,dsty=e.tx+dx,e.ty+dy
if is_floor(mget(dstx,dsty)) and is_walkable(dstx,dsty,"move") then
e.tx+=dx
e.ty+=dy
e.logic,moved=chase_logic,true
end
end
addhash(e)
end
function add_particle(x,y,dx,dy,life,c,uf)
add(particles,{
x=x,y=y,
dx=dx or 0,
dy=dy or 0,
life=life,
c=c,
uf=uf
})
end
function update_entity_status(e)
if e.poisoned>0 then
e.poisoned-=1
change_entity_hp(e,e,-1)
e.hit_by={name="poison"}
end
if e.paralyzed>0 then
e.paralyzed-=1
end
if e.confused>0 then
e.confused-=1
end
end
function update_entity(e)
if (e.mt<=1) e.mt+=0.125
if e==pl then
if e.mt<=1 then
e.ox, e.oy = e.dx*e.odist*(1-e.mt), e.dy*e.odist*(1-e.mt)
end
else
e.ox*=0.5
e.oy*=0.5
end
if e.hit_count>0 then
e.hit_count-=1
else
if e.hp<=0 then
delhash(e)
del(entities,e)
end
end
if e.t%8==0 and e.poisoned>0 then
add_rising_particle(e,explodeval"3,11,10,5")
end
if e.mt>=1 and e.attacked then
if e.after_attack then
e.after_attack(e)
e.attacked=false
end
end
update_entity_anim(e)
return e.mt>=1
end
function add_rising_particle(e,cols)
add_particle(
e.tx*8+1+rnd(6)+e.ox,
e.ty*8+rnd(6)+e.oy,
0,-0.1-rnd(0.2),
30+rnd(20),
cols,
function(p)
p.y+=p.dy
end
)
end
function update_entity_anim(e)
e.t+=1
e.fc+=1
if e.fc > e.fs then
e.frame+=1
e.fc=0
if e.frame >= #e.frames then
e.frame-=1
end
end
end
function draw_entity(e)
if (fog[e.tx][e.ty]==2) return
local x,y=e.tx*8+e.ox,e.ty*8+e.oy
if fog[e.tx][e.ty]==1 then
set_pal(dpal)
end
if e.hit_count>0 and e.t%3==0 then
set_pal(lpal)
end
spr(e.frames[1+e.frame],
x, y,
1, 1,
e.dir==-1)
pal()
if e.weapon and e.hp>0 then
spr(e.weapon.spr+e.frame,
x+e.dir, y,
1, 1,
e.dir==-1)
end
if e.hp>0 then
local indicator=""
if (e.confused>0) indicator="?"
if (e.paralyzed>0) indicator="..."
pr(indicator,x+6-2*#indicator,y-6,10)
end
end
windows,modal_top,modal_stack={},nil,{}
s_time=0
function show_alert(txt,hdr,life)
local w=add_window(0,24,{txt})
w.x,w.hdr=63-w.w/2,hdr
w.life=life or 60
return w
end
function add_modal(x,y,lines,width,on_close)
local w=add_window(x,y,lines,width)
w.btn,w.on_close,modal_top=true,on_close,w
add(modal_stack,w)
end
function add_window(x,y,lines,width)
local w={x=x,y=y,lines=lines}
w.h,w.w,w.delay=#lines*7+5,41,0
for l in all(lines) do
w.w=max(w.w,tlen(l)+7)
end
if (width) w.w=width
add(windows,w)
return w
end
function close_modal_top()
if (modal_top.header) modal_top.header.life=1
modal_top.life=1
del(modal_stack,modal_top)
modal_top=modal_stack[#modal_stack]
end
function update_windows()
if modal_top then
if modal_top.cursor then
if next_btn==2 then
modal_top.cursor-=1
s_time=0.25
sfx"34"
elseif next_btn==3 then
modal_top.cursor+=1
s_time=0.25
sfx"34"
elseif next_btn==5 then
if modal_top.on_select then
modal_top.on_select()
end
end
if modal_top then
modal_top.cursor=mid(1,modal_top.cursor,#modal_top.lines)
end
end
if next_btn==4 then
sfx"35"
if (modal_top.on_close) modal_top.on_close()
close_modal_top()
end
next_btn=-1
end
return modal_top==nil
end
function draw_windows()
for w in all(windows) do
if w.delay==0 then
local wx,wy,ww,wh=w.x,w.y,w.w,w.h
if w.on_close then
spr(pl.win and 160 or 144,wx+ww/2-20,wy-10,5,1)
end
rectfill(wx,wy,wx+ww-1,wy+wh-1,1)
rect(wx,wy+1,wx+ww-1,wy+wh-1,2)
rect(wx,wy,wx+ww-1,wy+wh-2,9)
spr(118,wx,wy)
local twy=wy+3
local cx=w.cursor and 4 or 0
if (w.cursor) wx+=8
for i=1,#w.lines do
local c=7
if w.cursor==i then
c=10
spr(119,wx-5,twy)
end
local sx,tw,www=0,tlen(w.lines[i]),ww-4-cx*3
if tw>www and w.cursor==i and w==modal_top then
local diff=(www-tw)/2
sx=diff+round(diff*sin(s_time))
s_time+=0.008
end
clip(wx+cx,wy+3,ww-4-cx*3,wh-6)
pr(w.lines[i],4+wx+sx,twy,c,2)
clip()
twy+=7
end
if w.hdr then
local hdrs=explodeval"151,4,167,5,183,3"
spr(hdrs[w.hdr],wx+3,wy-1,hdrs[w.hdr+1],1)
end
if w.hud then
spr(149,7,118)
pr(pl.hp.."/"..pl.hpmax,17,118,7)
spr(165,44,118,2,1)
pr(""..get_dmg(pl),58,118,7)
spr(181,74,118,2,1)
pr(pl.lvl.." ("..pl.xp.."/"..xp_req(pl.lvl+1)..")",86,118,7)
end
if w.life and w.delay==0 then
if (w.cursor) w.cursor=999
w.life-=1
if w.life<0 then
local diff=w.h/4
w.y+=diff/2
w.h-=diff
if (w.h<8) del(windows,w)
end
else
if w.btn then
pr("^close (c)",wx+ww-32-cx*2,wy+wh+1,12,5)
end
end
else
w.delay-=1
end
end
end
function unequip_item(itm,skip_snd)
if (not itm) return true
if (itm.trait==2) then
sfx"42"
show_alert(itm.name.." is stuck!",nil,60)
return false
end
if (not skip_snd) sfx"39"
pl.hpmax-=itm.hpmax
itm.equipped,pl.weapon=nil,nil
return true
end
function discard_item(itm,skip_snd)
if itm.equipped then
if not unequip_item(itm) then
return
end
end
if (not skip_snd) sfx"41"
inventory.items-=1
for i=1,8 do
if inventory[i]==itm then
inventory[i]=nil
return
end
end
end
function on_item_select()
local itm,cmd,spend_turn=inventory[modal_stack[1].cursor],modal_top.lines[modal_top.cursor],false
if cmd=="discard" then
discard_item(itm)
elseif cmd=="use" then
use_item(pl,itm)
discard_item(itm,true)
spend_turn=true
elseif cmd=="equip" then
use_item(pl,itm)
elseif cmd=="unequip" then
unequip_item(itm)
end
close_modal_top()
modal_stack[1].lines=get_inventory_text()
if spend_turn then
close_modal_top()
phase+=1
move_anim(pl,0,0,0)
end
end
function on_inv_select()
local itm=inventory[modal_top.cursor]
if itm then
sfx"33"
add_modal(
modal_top.x+8,
modal_top.y+3+modal_top.cursor*6,{
"use",
"discard"
},
50)
if itm.type==1 then
modal_top.lines[1]=itm.equipped and "unequip" or "equip"
end
modal_top.on_select=on_item_select
modal_top.btn=nil
end
end
function show_inventory()
sfx"36"
add_modal(16,12,get_inventory_text(),96)
modal_top.cursor,modal_top.on_select=1,on_inv_select
modal_top.header=add_window(modal_top.x,
modal_top.y-10,
{"^b^a^c^k^p^a^c^k"})
end
function item_name(itm)
local str="mysterious "..itm.name
if itm.identified then
str=i_trait[itm.trait]..itm.name..i_status[itm.status]
end
return str
end
function get_inventory_text()
local lines={}
for i=1,8 do
local itm=inventory[i]
if itm then
local str=item_name(itm)
if (itm.equipped) str="$ "..str.." "
add(lines,str)
else
add(lines,"###")
end
end
return lines
end
function bless_item(itm)
itm.trait=3
if (itm.heal>0) itm.heal+=1+flr(itm.heal*rnd(0.5))
if (itm.hpmax>0) itm.hpmax+=1+flr(itm.hpmax*rnd(0.5))
if (itm.atk>0) itm.atk+=1+flr(itm.atk*rnd(0.5))
end
function curse_item(itm)
itm.trait=2
itm.heal=0
itm.hpmax=0
end
function enchant_item(itm)
itm.status,itm.atk=intrnd(4)+2,flr(itm.atk*0.7)
end
item_count=0
function make_item(id,trait)
item_count+=1
local itm={
ic=item_count,
name=i_name[id],
spr=i_spr[id],
type=i_type[id],
atk=flr(i_atk[id]+rnd(0.2*i_atk[id])),
heal=i_heal[id],
hpmax=i_hpmax[id],
trait=1,
status=1,
identified=pl.depth<=2 and true or chance"0.2"
}
if not trait then
if chance"0.1" and pl.depth>=4 then
trait=3
elseif chance"0.4" and pl.depth>=2 then
trait=2
end
end
if trait==3 then
bless_item(itm)
elseif trait==2 then
curse_item(itm)
end
if pl.depth>3 and itm.type==1 and chance(trait==2 and 0.3 or 0.1) then
enchant_item(itm)
end
return itm
end
function use_item(e,itm)
if itm.type==0 then
floater_delay+=20
sleep=30
if itm.hpmax!=0 then
add_floater("&+"..itm.hpmax,e,11)
e.hpmax+=itm.hpmax
end
sfx(42+itm.trait)
if itm.trait==2 then
e.hit_by=itm
local r=rnd()
if r<0.2 then
itm.heal=5
elseif r<0.4 then
e.poisoned+=3
show_alert("^b^l^e^h! ^poisonous!").delay=20
elseif r<0.6 then
e.confused+=4
show_alert("^feeling dizzy...").delay=20
elseif r<0.8 then
e.paralyzed+=3
show_alert("^g^l^u^p! ^can't move!").delay=20
else
show_alert("^ouf... ^it's rotten!").delay=20
itm.heal=-1
end
end
change_entity_hp(e,itm,itm.heal)
elseif itm.type==1 then
if unequip_item(e.weapon) then
e.weapon=itm
itm.equipped,itm.identified=true,true
sfx"40"
end
end
end
function get_free_slot()
for i=1,8 do
if (inventory[i]==nil) return i
end
return nil
end
function give_item(itm,hide_msg)
local slot=get_free_slot()
if slot then
if (not hide_msg) show_alert(item_name(itm),5)
inventory[slot]=itm
inventory.items+=1
end
return slot
end
function title_init()
reload()
t,story=0,"  ^to become immortal, the lich\n king ^raq'zul casts a powerful\n   spell. ^draining the world\n     of life and happiness.\n\n  ^brave souls descend into his\n     lair, but none return.\n\n  ^now it is your turn. ^defeat\n ^raq'zul, break the curse, and\n       end the suffering!"
music"0"
end
function title_update()
t+=1
if (t>768) t=0
if next_btn>=4 then
sfx"53"
fadeout()
swap_state(level_state)
end
next_btn=-1
end
function title_draw()
cls""
map(112,0)
spr(139,44,27,5,2)
clip(0,50,128,33)
pr(story,16,86-t/7,6,2)
clip()
fillp"0b0101101001011010.1"
rect(0,50,127,82,0)
fillp()
pr("(*) ^embark",46,104,12,5)
pr("^created by @^johan^peitz\n^audio by @^gruber_^music      v1.2",26,115,5,1)
end
title_state={
init=title_init,
update=title_update,
draw=title_draw
}
floor_weights=explodeval("0,0,0,1,2,2,3,3,3")
function generate_level(num_rooms)
rooms,dropped,removed,size={},0,0,num_rooms
mobs_placed,mobs_to_place=0,xp_req(pl.depth+1)-xp_req(pl.depth)
mobs_per_room=flr(0.5+mobs_to_place/size)-1
t_floor,t_wall,t_door_h,t_door_v=1,48,4,5
memset(0x2000,0,0xfff)
make_room("entry",5+intrnd"3",5+intrnd"3")
make_room"storage"
for i=#rooms,size-1 do
if chance"0.1" then
make_room"storage"
else
make_room"n/a"
end
end
size=#rooms
room_pool={}
add(room_pool, pl.depth<8 and "exit" or "boss")
for i=1,pl.depth/2 do
add(room_pool,"treasure")
end
if (pl.depth>=3) add(room_pool,"shrine")
if (pl.depth>=4) then
add(room_pool,"smithy")
add(room_pool,"pool")
end
while #rooms>0 or #room_pool>0 do
if #rooms==0 and #room_pool>0 then
make_room(room_pool[1])
del(room_pool, room_pool[1])
end
for r in all(rooms) do
if not place_room(r) then
local dirs=explodeval"1,2,2,2,3,4"
local rd=rnd_elem(dirs)
r.x+=dirx[rd]
r.y+=diry[rd]
r.dir=rd
else
del(rooms,r)
end
end
end
while mobs_placed<=mobs_to_place do
place_mob(intrnd"128",intrnd"32")
end
mobs_to_place=mobs_placed+pl.depth-3
while mobs_placed<=mobs_to_place do
place_mob(intrnd"128",intrnd"32",212)
end
auto_tile(48)
make_entities()
wall_fix()
end
function make_room(typ,w,h)
local r=add(rooms,{
x=12,
y=12,
w=5+intrnd(7),
h=5+intrnd(5),
oob=0,
dir=0,
typ=typ,
floor=0
})
r.floor=rnd_elem(floor_weights)
if w then
r.w,r.h=w,h
end
if typ=="boss" then
r.w,r.h=11,8
end
return r
end
function mset_flr(x,y,tile)
if is_floor(mget(x,y)) then
mset(x,y,tile)
return true
end
return false
end
function place_room(r)
local rx,ry,rw,rh=r.x,r.y,r.w,r.h
if ry<0 or ry+rh>31 or
rx<0 or rx+rw>127 then
r.oob+=1
if r.oob>5 then
add(room_pool,r.typ)
del(rooms,r)
dropped+=1
end
return false
end
r.oob=0
for tx=rx+1,rx+rw-2 do
for ty=ry+1,ry+rh-2 do
if mget(tx,ty)!=0 then
return false
end
end
end
local bg={}
for tx=rx,rx+rw-1 do
bg[tx]={}
for ty=ry,ry+rh-1 do
bg[tx][ty]=mget(tx,ty)
end
end
for tx=rx,rx+rw-1 do
for ty=ry,ry+rh-1 do
local tile=t_floor
if r.floor>0 then
tile=get_floor(28+4*r.floor)
end
if tx==rx or ty==ry or
tx==rx+rw-1 or
ty==ry+rh-1 then
tile=t_wall
end
mset(tx,ty,tile)
end
end
if chance"0.4" then
for i=1,rnd(r.w/2) do
local x,y=get_rnd_pos(r)
local tle=27+2*intrnd"2"
if (chance"0.1") tle=11
mset_flr(x,y,tle)
end
end
if pl.depth>=m_depth[13] then
if chance"0.2" then
for i=1,1+rnd(r.w/2) do
local x,y=get_rnd_pos(r)
mset_flr(x,y,47)
end
end
end
rcx,rcy=rx+rw/2,ry+rh/2
if r.typ=="entry" then
rset"17"
mset(rcx-1,rcy-1,t_wall)
mset(rcx-1,rcy,t_wall)
elseif r.typ=="boss" then
place_piece(27,rx+2,ry+2)
place_piece(0,rx+6,ry+3)
rset"248"
elseif r.typ=="exit" then
rset"16"
elseif r.typ=="pool" then
rset"22"
elseif r.typ=="smithy" then
rset"24"
mset(rcx-1,rcy,23)
elseif r.typ=="shrine" then
rset"25"
mset(rcx+1,rcy,71)
elseif r.typ=="storage" then
for i=1,rnd(r.w) do
local x,y=get_rnd_pos(r)
mset(x,y,27+2*intrnd"2")
end
for i=1,r.w-2 do
if chance"0.5" then
mset(rx+i,ry+1,15)
end
end
elseif r.typ=="treasure" then
for i=1,1+rnd(r.w) do
local x,y=get_rnd_pos(r)
mset(x,y,47)
end
rset"13"
elseif rw>=6 and rh>=6 then
local amnt=flr((rw-2)/5)
for ix=0,amnt-1 do
place_piece(2*pl.depth-2+intrnd"14",
rx+1+intrnd"2"+ix*4,
ry+2+intrnd(r.h-7))
end
end
local prev_mobs_placed=mobs_placed
if r.typ!="entry" then
for i=1,mobs_per_room do
local x,y=get_rnd_pos(r)
place_mob(x,y)
end
end
dc,doors=0,get_doors(rx+rw-1,ry,rx+rw-1,ry+rh-1)
door_helper()
doors=get_doors(rx,ry,rx,ry+rh-1)
door_helper()
doors=get_doors(rx,ry+rh-1,rx+rw-1,ry+rh-1)
door_helper()
doors=get_doors(rx,ry,rx+rw-1,ry)
door_helper()
if dc==0 and r.typ!="entry" then
for tx=rx,rx+rw-1 do
for ty=ry,ry+rh-1 do
mset(tx,ty,bg[tx][ty])
end
end
add(room_pool,r.typ)
mobs_placed=prev_mobs_placed
removed+=1
return true
end
return true
end
function door_helper()
dc+=#doors
place_a_door(doors)
end
function rset(id)
mset(rcx,rcy,id)
end
function place_piece(p,tx,ty)
for x=0,2 do
for y=0,2 do
local op,np=sget(p*3+x,64+y)
if op==15 then
np=t_wall
else
np=op+63
end
if np!=t_wall and chance(pl.depth/10) then
np+=16
end
if op>0 then
mset(x+tx,y+ty,np)
end
end
end
end
function place_mob(x,y,id)
local mobs={}
for i=2,#m_depth do
if m_prop[i]==0 and m_depth[i]<=pl.depth then
add(mobs,m_anim[i])
end
end
if (not id) then
id=rnd_elem(mobs)
end
if mset_flr(x,y,id) then
mobs_placed+=1
end
end
function get_rnd_pos(r)
return r.x+1+rnd(r.w-2),r.y+1+rnd(r.h-2)
end
function get_floor(offset)
if (chance(0.2)) return t_floor
return offset+rnd_elem({0,1,2})
end
function place_a_door(doors)
local door=rnd_elem(doors)
if (not door) return
mset(door.x,door.y,chance(0.7) and door.tile or t_floor)
del(doors,door)
end
function get_doors(x1,y1,x2,y2)
local ds={}
local sx,sy,dist
if x1==x2 then
dist,sx,sy=y2-y1+1,0,1
else
dist,sx,sy=x2-x1+1,1,0
end
for i=1,dist do
local t1,t2=mget(x1+sy,y1+sx),mget(x1-sy,y1-sx)
if is_floor(t1) and is_floor(t2) then
add(ds,{x=x1,y=y1,tile=(sx==1 and t_door_h or t_door_v)})
end
x1+=sx
y1+=sy
end
return ds
end
function is_floor(id)
if (id==1 or id==2 or (id>=32 and id<=44)) return true
return false
end
function wall_fix()
for i=0,127 do
for j=0,31 do
local t1,t2=mget(i,j),mget(i,j+1)
if fget(t1,7) then
if (t2==1) mset(i,j+1,2)
for ii=32,40,4 do
if (t2>=ii and t2<=ii+2) mset(i,j+1,ii+3)
end
if (t2==0) mset(i,j+1,3)
end
end
end
end
function auto_tile(st)
for i=0,127 do
for j=0,31 do
if mget(i,j) ==st then
local nt,bm=0,explodeval"2,4,1,8"
for d=1,4 do
if (fget(mget(i+dirx[d],j+diry[d]),7)) nt+=bm[d]
end
mset(i,j,nt+st)
end
end
end
end
__gfx__
0000000000000000dd5dd5dddd5dd5dd000000000004400000000000000000000001100000066000000000000000000000000000000000004444444499999999
00000000000000001111111111111111000000000004400000000000000000000001100000066000000000000000000000000000000000004444444499999999
00d00d000000000015551555155515552444444400044000d0000006d666d6660001100000066000000000000000000000022240000000002222222244444444
000660000000000011111111111111112444444400044000d0000006d666d66600011000000dd0000000000000000000000222440099940024151112483d2224
0006600000000000000000000000000012222222000440005000000d5ddd5ddd0000000000066000002dd50000466d0000022240099944402222222244444444
00d00d0000010000000100001101110111222222000440005000000d55dd5ddd00000000000660001225dd5d244d66240d6d55500d6d55502140000243e11114
0000000000000000000000000000000011122222000440005000000d555d5ddd00000000000660000205004d040d009404442220044422202145511243edd224
0000000000000000000000000010001011112222000220005000000d55555ddd00055000000dd000100020002000400000000000000000002222222244444444
ddddddddd60000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
d111111dd60000000000000000000000000000000000000000000000000000000d6666d0000000000000000000000000000000000dd000000000000000004000
d5d1111dd65d0000000000000000000000000000006dddd0006dddd0000000006d6666d67866668700000000000ddd0000000000d1150dd00400000000004000
d5d0000d115d00000000000000000000000000000655555d0655555d004444405dddddd5786446870000000000d11150000000005d51d115004ddd0000dd4d00
d5d1500d115d15000000000000000000000000000611111d06cccccd0041114000dddd00786666870000d000005d551000d00000d6d55d5100d41d0000d14d00
d5d1500d111115000000000000000000000000000d6666d50d6666d5004444400065560062dddd26000d550000d6dd500d550d000550d6d500dddd0000dddd00
d5d1501d111115000000000000000000000000000dd555550dd55555002222200066660062dddd26050000000005550000000550000005500055550000555500
dddddddd1111110000000000000000000000000000dddd5000dddd500022222000dddd0062222226000000000000000000000000000000000000000000000000
000000000000000000000000dd5dd5dd000000000000000000000000dd5dd5dd000000000000000000000000dd5dd5dd00000000000000000000000000000000
00000110000000000000000011111111000000000000000000000000111111110111000001100000000000001111111100000000000000000000000000000000
00001001000000000000000015551555000000000001000000001100155515550111000001101110001111001555155500000000000000000000000000000000
0000000000300000000000301111111100010000001110000001111011111111000000000000111000111100111111110000000000000000000f000f00000000
0000000000303000030003300000000000111000011111000001110000011100000001100000000000111100000000000000000000000000f004000f00000000
001100000000000000330300000110000111110000111000000010000011111000011110001110000000000000011110000000000000000040020040000d0000
010010000000000000030000001001000011000000010000000000000001110000011000001110000000000000011110000000000000000004000020000000d0
0000000000000000000000000000000000000000000000000000000000001000000000000000000000000000000000000000000000000000020000000d000000
11e1e1e1e0000001e1e1e1e1e000000111e1e1e1e0000001e1e1e1e1e000000111e1e1e1e0000001e1e1e1e1e000000111e1e1e1e0000001e1e1e1e1e0000001
1000000e1000000e0000000e0000000e100000001000000000000000000000001000000e1000000e0000000e0000000e10000000100000000000000000000000
e0000001e00000010000000100000001e0000000e00000000000000000000000e0000001e00000010000000100000001e0000000e00000000000000000000000
1000000e1000000e0000000e0000000e100000001000000000000000000000001000000e1000000e0000000e0000000e10000000100000000000000000000000
e000000ee000000e0000000e0000000ee0000000e00000000000000000000000e0000001e00000010000000100000001e0000000e00000000000000000000000
55555555555555555555555555555555555555555555555555555555555555551000000e1000000e1000000e1000000e1000000e1000000e1000000e1000000e
5666d6665666d666d666d666d666d6665666d6665666d666d666d666d666d666e0000001e0000001e0000001e0000001e0000001e0000001e0000001e0000001
55555555555555555555555555555555555555555555555555555555555555551000000e1000000e1000000e1000000e1000000e1000000e1000000e1000000e
00000000000000000000600000000000006666000000000000000000000a000000000000000000000000050500a0000000000000000000000000000000000000
0000000000040000044464400000400000666600004000000000000000090000000550000660002000d000d00090000000000000000000000000300000000000
0000000000040000044494400000400000dddd0066466660000000000066d000005dd5004dd44224444444d400d00a0000000000000030000000040000000300
01111111000244400444444004442000000550006666666066d666600005000000dddd0044444444444445d505d5090000000000000004000000400000000040
01010101000244400444444004442000000dd000ddddddd06dddd660000d000000d55d00222222224d44445400500d0000000030000040000009000000000400
01010101000222200222222002222000006dd6002222222066d66660000d000000dddd00200000022252252500005d5000009440000900000000000000009000
011111110002002002000020020020000066660020000020ddddddd0005d500000dddd0020000002205000020000050000000000000000000000000000000000
0000000000000000020000200000000000dddd000000000000000000000500000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000600000000000000000000000000000000000000000000000050500e0000000000000000000000000000000000000
000000000000044000440000044000000006d000000000000000000000050000000000000200000000d000d00080000000000000000000000000000000000000
00000000000004000044444000400000000dd00000066660000000000066d0000050000042442600424444d200d00e0000000000000000000000060000000000
01111111000004400444444004400000000dd000666666606d6666600005000005d5000044444d4424554d4405d5080000000000000006000000600000000000
01010101004442000444444000244400000d50006666ddd0dd511160000d00000ddd0000222244444d445d5400500d0000000000000060000009000000000000
01000101004442000222444000244400006dd600dddd22205dd56660000d00000d5d5000200022222222252200005d5000009660000900000000000000009660
011111110022222002002220022222000066660022200020d5dd5dd0005d50000dddd05020000002200050520000050000000000000000000000000000000000
0000000000000000020000200000000000d5dd0000000000005d0000000500000000000000000000000000000000000000000000000000000000000000000000
00000000000000700000000000000007000000000000000000000600000000000000000000000000000006000000000000000000000000000000000000000000
00000070000007600000000000000076000000000000060000000600000000600000000000000600000004000000006000000000000024000000044000000240
00000670000007000000067700000070000000000000060000006600000000600000000600000400000040000000004000000440000044000000242000000440
0000076000007600000d776000000760000000060000660000006000000006600000004000004000000040000000040000000440000042000002420000000420
0000d700000d70000009d0000000d700000000660000600000090000000006000000040000004000000900000000040000002420000240000009000000002400
00009d000009d0000000000000009d00000096000009000000000000000090000000900000090000000000000000900000009400000900000000000000009000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000400000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000077a7aa9a000000000000000000000000000000000000000000000000000000000000070000000000
00000000000000000000000000000000000000000000000072222222067777600000000000000000000006000000000000000000000007000000060000000070
000000000000000000000000000000000000000000000000a0000000677760000000000000000600000004600000006000000007000006000000700000000060
00000000000000000000000000000000000000000000000090000000677760000000000000000460000040000000004600000060000070000000600000000700
000000000000000000000000000000000000000000000000a0000000d77600000000000600004000000900000000040000000600000060000009000000000600
00000000000000000000000000000000000000000000000090000000000000000000944600090000000000000000900000009000000900000000000000009000
00000000000000000000000000000000000000000000000090000000000000000000000600000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000090000000000000000000000000000000000000000000000000000000000000000000000000000000
f0ff0fffff0ff0f2340000005050f0505000080f0f00fff07070000000000000970008a80007000000c000000770000000000000000000000070070070000000
000000000f0000f000230ac00100f0f1f860000010f0f00000007c0b00509990094520000b50996065f500007880707070700770070000700780077077000700
f0fffff0ff0ff0f2340000005050f0000000050f0ff000ff07000000c0000001000000001000000040f000007000707077807780777007870770078078707770
999aaa999aaa999aaa999aaa999aaa999aaa999aaa999aaa999aaa999aaa999aaa999aaa999aaa999aaa00007000707078008770788007070780070070707880
cccccccccccc000000000000cccccccccccc000000000000cccccccccccc000000000000cccccccccccc00008770877070007780870008780700087070708700
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000880088080008800080000800700008080800800
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000700000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000077000000000700000700000800000000007000
000000700000000000777000000000000000700070707aa000000000176617611766117117661766176110000077007700007700000770077000070000078000
0070078000070000007777700077000000077000aaa0aaa000000000161116611661176611611661161610000078008800007700000770778077077700770000
0077770077070070007788770088077007777000a2a0a22000000000166616161666161611611666166110000770077007707770000877770088087870780700
00877807870700700087008707707870788780002020200000000000000000000000000000000000000000000770077077807887000077877007707070700770
00078077070777700007700707707770707700000000000000000000000000000000000000000000000000007770087077008007000077087707708070770870
00770077070877800008777707707880877700000000000000000000000000000000000000000000000000007777708087770008000087008707800080877780
00780087780088000000877808708700088700000000000000000000000000000000000000000000000000008887770008880000000007000808000000077800
00800008800000000000088000800800000800000000000000000000000000000000000000000000000000000008880000000000000008000000000000088000
000700000000000000700000000007000000770007007aa070700000171761176616611766171766171766176110000011111111111111110000000000000000
00777007007700007777007700000700700077007aa02a20a7200000161616166116161161161661161661161610000011111111111111110000000220000000
0087700700880777877807870700087070077800a2a00a00a2a00000161661166616161161161611161666166110000011111111111111110000002442000000
00087007077077880770070707770077700770002020020020200000000000000000000000000000000000000000000011111111111011110000024444200000
00007707077077000770077707787087700780000000000000000000000000000000000000000000000000000000000011111111111111110000244444420000
00008777077087000870087807708007700800000000000000000000000000000000000000000000000000000000000011111111111011110000244444442000
00000878087008770087008008700077807000000000000000000000000000000000000000000000000000000000000011111111101010110004424644442200
00000080008000880008000000800088008000000000000000000000000000000000000000000000000000000000000011111111100010100044446544424200
000000000000000000000000000000000000000070070707000000001766176617171761176110000000000000000000011011000010100009444d4244242200
0000000000000000000000000000000000000000a00a7a0a00000000166116161616161616161000000000000000000000000000101000100994444422422100
0000000000000000000000000000000000000000aa02a20aa0000000161116661666161616611000000000000000000010111010001000000299444444221000
00000000000000000000000000000000000000002200200220000000000000000000000000000000000000000000000000080000000000100029944442210000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000111a1110000000000002994422100000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000040000000000000000299221000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001040100000000000000029210000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000
00000000000000000000000000000000000000000000000000077000000000000000000000000000000000000000000000000000000000000000000a00000000
000000000000000000000000000000000000000000077000000670d0000770000000000000000ff000000ff00000ff0000044000000440240004400800044000
000000000000000000000000000000000007700d000670d0000700d00006700d0000ff00000099f0000ff9f000009f000002f0070002f0440002f0090002f030
000004000000000000000000000000000006700d007700d000440d000077000d00009f00000ff00000fff900000ff00000770006007700420077004000ff0004
004424e0444404000004400000440400077700d004470d0000226d00044760d0000ff00000fff9f000f90f0000fff9000767607007676240076760400f9f9040
04422200002244e004422440044224e0446766d002276d0000770000022706d000fff9f000f900000f900000f0f90f000f4409600f4409000f4409000f770900
40200000000002000020204e4020200022776000007060000076000000606000f0f900000f900000f00000000f90000000402000004020000040200000706000
00000000000000000000000000000000660006000600600000060000060060000f900000f000000000000000000000000200200002002000020020000f009000
0000dd00000000000000dd0000000dd000000000000000000000000000000000000000000000000000000000000000000000000000e800000000009000050500
0043530000000dd000435300000435300000000000000000000000000000000000000000000000000000000000000000000660000e8760000005d09000058000
033433000034353003343300003433300000000000000000000000000000000000000000000000000000000000000000000df0000806d007000d904000088000
303343500333433030334350033343000000499000449990004499000000000000000300000000000000000000000000004400060065000700ddd04000ddd000
30444405304444503044440530444450004496d00055d6d00055d69000449990003353f03333030000033000003303000424206006d6d0700d5d50400d5d5000
003442503034420500344200303442050055d4400022444000224dd00055d6d003355500005533f003355330033553f00f550900065506700f5dd590085dd540
003000500030050000035000003005000022400000011100000004400022444030500000000005000050503f30505000005020000060d00000ddd04000ddd000
005000000500050000055000050005000000000000000000000000000000000000000000000000000000000000000000020020000600d0000200204005ddd000
0000cc0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000008000000dd00000000000000000
0000dc000000cc00000000000000cc000007700000000000000000000000000000440000000000000000000000000000000cc090000435300004400000000000
00cc4dd00cc0dcd00000cc00000cdc00007fef00000fe0000000000000077000f42f0900000044000000044000000000000dc040003433300002f00000053300
0c044000000c400000ccdc0000c44d00006e1e00007e1e0000077000007fef000777600000672f00000002f00000000000222040033343000077000000033300
00022200000440000c0420d000c22d00000fe000006fef00007fef00006e1e000067000006770000004777000047760002121040304444500767600000533500
00200d000022020000022200002020000000000000077000006e1e00000fe000004400000446700024467600244677240c1225d0303442050f44090000333350
0d0000000d000d0000d20d0000d0d0000011110000111100001fe100001111000042200024200f0004006000040600f400222040003005000040200005333330
0000000000000000000000000000000000000000000000000000000000000000020000000000000020000f002000f0000d00d040050005000200200000000000
00000000000000000004400000000000eee088220ee002200e0000200ee0022000060d00000060d000060d0000060d000000000000060d000030300000000000
00000000000440000002f0000004400000e8880000e0880000e0020000e088000000670400006704000670000006700000000000000670000003530000000000
024440000442f000004700000042f000088880008008880000e0880080088800000077020000770200077004000770400000000000077000003dd00000000000
0022f0000077000004770000047700008080200008888000800888000888800000dddd02000ddd0200ddd00200ddd0200000000000ddd0000022200000000000
07770000076700000077900007676000000000000080200008888000008020000605dd5d00d5dd5d005d50020d5d5020000000000d5d50000212100000330300
f06769000f44900000f400000f440900000000000000000000802000000000000005dd020065dd0200ddd55d065dd5d0d00d5dd0065dd5d00d122500033553f0
0044200000402000004200000040200000111000001110000011100000111000005ddd02000ddd02006dd00200ddd020077ddd6d00ddd0000022200030505000
220002000200200000020000020020000000000000000000000000000000000005ddd500005ddd0000ddd50205ddd0206670055d05ddd0000500500000000000
__label__
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000080000080000000000000880000008080000000000000000000000000000008000000000000000000000000000000000000
00000000000000000000000000000080080088000880880000828008002088088000880080800800880080088008808800000000000000000000000000000000
00000000000000000000000000000080828082808280828000808088808082028000828088208880820888082808208200000000000000000000000000000000
00000000000000000000000000000080808080808080808000882082208080082000808082008220280822080808002800000000000000000000000000000000
00000000000000000000000000000080282080802880808000820028008028088000882080002800880280080802808800000000000000000000000000000000
00000000000000000000000000000820020020200220202000200002002002022000820020000200220020020200202200000000000000000000000000000000
00000000000000000000000000000200000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000777700000000000000000000000000000000000000000000007700007700007700000000000000000000000000000000000000
00000000000000000000000000777700000000000000000000000000000000000000000000007700007700007700000000000000000000000000000000000000
00000000000000000000000077888800770077007700770000777700007700000000770000778800007777007777000000770000000000000000000000000000
00000000000000000000000077888800770077007700770000777700007700000000770000778800007777007777000000770000000000000000000000000000
00000000000000000000000077000000770077007777880077778800777777000077887700777700007788007788770077777700000000000000000000000000
00000000000000000000000077000000770077007777880077778800777777000077887700777700007788007788770077777700000000000000000000000000
00000000000000000000000077000000770077007788000088777700778888000077007700778800007700007700770077888800000000000000000000000000
00000000000000000000000077000000770077007788000088777700778888000077007700778800007700007700770077888800000000000000000000000000
00000000000000000000000088777700887777007700000077778800887700000088778800770000008877007700770088770000000000000000000000000000
00000000000000000000000088777700887777007700000077778800887700000088778800770000008877007700770088770000000000000000000000000000
00000000000000000000000000888800008888008800000088880000008800000000880000770000000088008800880000880000000000000000000000000000
00000000000000000000000000888800008888008800000088880000008800000000880000770000000088008800880000880000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000770000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000770000000000000000000000000000000000000000000000000000
00000000000000000000000000007777000000000000000000770000000000770000000000880000000000000000000077000000000000000000000000000000
00000000000000000000000000007777000000000000000000770000000000770000000000880000000000000000000077000000000000000000000000000000
00000000000000000000000000007777000077770000000077770000000000777700007777000000007700000000007788000000000000000000000000000000
00000000000000000000000000007777000077770000000077770000000000777700007777000000007700000000007788000000000000000000000000000000
00000000000000000000000000007788000088880000000077770000000000777700777788007777007777770000777700000000000000000000000000000000
00000000000000000000000000007788000088880000000077770000000000777700777788007777007777770000777700000000000000000000000000000000
00000000000000000000000000777700007777000077770077777700000000887777777700008888008877887700778800770000000000000000000000000000
00000000000000000000000000777700007777000077770077777700000000887777777700008888008877887700778800770000000000000000000000000000
00000000000000000000000000777700007777007777880077888877000000007777887777000077770077007700770000777700000000000000000000000000
00000000000000000000000000777700007777007777880077888877000000007777887777000077770077007700770000777700000000000000000000000000
00000000000000000000000077777700008877007777000088000077000000007777008877770077770088007700777700887700000000000000000000000000
00000000000000000000000077777700008877007777000088000077000000007777008877770077770088007700777700887700000000000000000000000000
00000000000000000000000077777777770088008877777700000088000000008877000088770077880000008800887777778800000000000000000000000000
00000000000000000000000077777777770088008877777700000088000000008877000088770077880000008800887777778800000000000000000000000000
00000000000000000000000088888877777700000088888800000000000000000077000000880088000000000000007777880000000000000000000000000000
00000000000000000000000088888877777700000088888800000000000000000077000000880088000000000000007777880000000000000000000000000000
00000000000000000000000000000088888800000000000000000000000000000088000000000000000000000000008888000000000000000000000000000000
00000000000000000000000000000088888800000000000000000000000000000088000000000000000000000000008888000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000011100000000000000000000000000000000cc0000000000
0000000000000000000000000000000000000000000000000011110000000000000000000000000001110000000000000000000000000000000cdc0000000000
000000000000000000000000000000000000000000000000001111000000000000000000000000000000000000000000000000000000000000c44d0000000000
000000000000000000000000000000000000000000000000001111000000000000000000000000000000011000000000000000000000000000c22d0000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000111100000000000000000000000000020200000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000001100000000000000000000000000000d0d00000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000011111111d60000000000000000000000000a0000111111110000000000000000000060000000000000000000
000000000000000000000000011000000000000010000001d6000000000000000000000000090000100000010110000000040000044464400000000000000000
000000000000000000000000011011100000000010000001d65d000000000000000000000066d000100000010110111000040000044494400000000000000000
000000000000000000000000000011100000000010000001115d0000000000000000000000050000100000010000111000024440044444400000000000000000
000000000000000000000000000000000000000010000001115d15000000000000000000000d0000100000010000000000024440044444400000000000000000
000000000000000000000000001110000000000010000001111115000001000000000000000d0000100000010011100000022220022222200000000000000000
000000000000000000000000001110000000000010000001111115000000000000000000005d5000100000010011100000020020020000200000000000000000
00000000000000000000000000000000000000001000000111111100000000000000000000050000100000010000000000000000020000200000000000000000
00000000000000000000000011111111111111111000000100000000000000000000000000000000100000010000000000000000000000000000000000000000
00000000011100000000000010000000000000000000000100000000000000000111000001110000100000010000000000000000011100000111000000000000
00000000011100000000000010000000000000000000000100000000001111000111000001110000100000010000000000111100011100000111000000000000
00000000000000000000000010000000000000000000000100000000001111000000000000000000100000010000000000111100000000000000000000000000
00000000000001100000000010000000000000000000000100000000001111000000011000000110100000010000000000111100000001100000011000000000
00000000000111100000000055555555555555555555555500010000000000000001111000011110100000010001000000000000000111100001111000000000
0000000000011000000000005666d666d666d666d666d66600000000000000000001100000011000100000010000000000000000000110000001100000000000
00000000000000000000000055555555555555555555555500000000000000000000000000000000100000010000000000000000000000000000000000000000
000000000000000000000000dd5dd5dddd5dd5dddd5dd5dd00000000000000000000000000000000100000011111111111111111000000001111111100000000
00000000000000000dd0000011111111111111111111111101100000000770000004400001100000100000000000000000000001000000001000000100000000
0000000000000000d1150dd0155515551555155515551555011011100006700d700f200001101110100000000000000000000001244444441000000100000000
00000000000000005d51d115111111111111111111111111000011100077000d6000770000001110100000000000000000000001244444441000000100000000
0000000000000000d6d55d5100000000000000000000000000000000044760d00706767000000000100000000000000000000001122222221000000100000000
00000000000000000550d6d500010000000111100001000000111000022706d0069044f000111000555555555555555555555555112222225555555500000000
000000000000000000000550000000000001111000000000001110000060600000020400001110005666d666d666d666d666d666111222225666d66600000000
00000000000000000000000000000000000000000000000000000000060060000002002000000000555555555555555555555555111122225555555500000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000dd5dd5dddd5dd5dddd5dd5dd00000000dd5dd5dd00000000
00000000000000000000000000000000000000000111000000000000011000000110000001100000111111111111111111111111011100001111111100000000
00000000000000000000000000111100001111000111000000000000011011100110111001101110155515551555155515551555011100001555155500000000
00000000000000000000000000111100001111000000000000000000000011100000111000001110111111111111111111111111000000001111111100000000
0000000000d000000000000000111100001111000000011000000000000000000000000000000000000000000000000000000000000001100000000000000000
000000000d550d000000000000000000000000000001111000010000001110000011100000111000000100000001000000011110000111100001000000000000
00000000000005500000000000000000000000000001100000000000001110000011100000111000000000000000000000011110000110000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000077000011100000000000000000000011100000000000001100000000000000110000001100000000000000000000000000000
000000000000000000000000007fef00011100000011110000111100011100000000000001101110001111000110111001101110000000000000000000000000
000000000000000001111111006e1e00000000000011110000111100000000000000000000001110001111000000111000001110000000000000000000000000
000000000000000001010101000fe000000001100011110000111100000001100000000000000000001111000000000000000000000000000000000000000000
00000000000100000101010100000000000111100000000000000000000111100001000000111000000000000011100000111000000500000000000000000000
00000000000000000111111100111100000110000000000000000000000110000000000000111000000000000011100000111000000000500000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000050000000000000000000000
00000000000000000000000000000000111111111111111111111111111111111111111100000000000000000dd0000000000000000000000000000000000000
00000000000000000000000000000000100000000000000000000000000000000000000101110000011000000353400000000000000000000111000000000000
00000000000000000000000000000000100000000000000000000000000000000000000101110000011011100333430000000000000000000111000000000000
00000000000000000000000000000000100000000000000000000000000000000000000100000000000011100034333000999400000000000000000000000000
00000000000000000000000000000000100000000000000000000000000000000000000100000110000000000544440309994440000000000000011000000000
0000000000000000000000000000000055555555555555555555555510000001555555550001111000111000502443030d6d5550000000000001111000000000
000000000000000000000000000000005666d666d666d666d666d66610000001d666d66600011000001110000050030004442220000000000001100000000000
00000000000000000000000000000000555555555555555555555555100000015555555500000000000000000050005000000000000000000000000000000000
00000000000000000000000000000000dd5dd5dd99999999dd5dd5dd10000001dd5dd5dd00000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000111111119999999911111111100000011111111100000000000000000111000000000000000000000000000000000000
00000000000000000000000000000000155515554444444415551555100000011555155500000000000000000111000000000000001111000000000000000000
0000000000000000000f000f0000000011111111483d222411111111100000011111111100000000000000000000000000000000001111000000000000000000
0000000000000000f004000f00000000000000004444444400000000100000010000000000440400000000000000011000000000001111000000000000000000
000000000000000040020040000000000001111043e11114000100005555555500011110044224e0000000000001111000000000000000000000000000000000
000000000000000004000020000000000001111043edd224000000005666d6660001111040202000000000000001100000000000000000000000000000000000
00000000000000000200000000000000000000004444444400000000555555550000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000dd5dd5dd0000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000111111110000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000111100000000000000000000000000155515550000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000111100000000000000000000000000111111110000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000111100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000100000000000000000000000000000001000000000000000000000000000000000000
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

__gff__
0000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000008383838383838383838383838383838300010101010101010101010100000000000101010101010101010101000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
__map__
30303030303030303030000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000acacacacacacacacadadacacacacacac
30181916300f0f0f0f30303030303030303030303030303030303030303030303030303030303030303030000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000acacacacacacadadbdbdadadadadacac
30010101302f2f1d1b30010101d40101010130300101010101010101010101d40101010130010101010130000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000acacacacacadbdbd0000bdbdbdbdadac
303001303001010101050101010101010101010101d4010101d40101010101010101010105010101d01030000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000adadacacadbd0000000000000000bdad
300101c03001010b0130300430303030300101010130300430303030300430303030300430010101010130000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000bdbdadacbd0000000000b0b0000000bd
300111010501010101300f01012f011b3030300430300101010101f43001011d01013001303030303030300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000bdac000000000000000000000000
30010101301d010101300101012f01c03001010101300101013001013001011d01e430010101011d300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000ad000000000000000000000000
30303030301b0b011d301b01012f010130c001c001300101443044013001303044443056c4460156300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000bd000000000000000000000000
3030303030303004303030303030303030010101c0301d01013001013001e430e401301d010101c430000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
300f0f0f0f0f0f010f1b1b1b1b1b1b300dd801011630f41d011617f43001013001013001440d440130000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
301b1b1b0101010101010d010d010d3030303001303030303030303030303030303030010101010130000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
301b1b1b0119011718010d010d010d30e00f010101e001300000000000000000000030303030303030000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
301b1b1b0101010101010d010d010d3001010141425301300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
301b1b1b010101011b1b1b1b1b1b1b3042010000e00101300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
3030303016013004303030303030303030303030303030300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000000303030300101010105290128012a2a3000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000300101013044443028292a280129300000000000000000000000000000cc00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000301b01013001013029562a2956283000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000301d1b013001c8302929012829013000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000003030011d1d3030303028472a2947293000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000030c8000000300000302a0128f829013000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000003030303030300000302846010156293000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000303030303030303000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0030000000000000000030000030004142430000000000004400444430004400440000000047003000300000303030004600460000000000000000000000000048460000004749470000004600000000000000000000000000000000000000000000000000000000000000000000000000000000000000003030303030303030
303030003030303000003030303000000000414200494b0000400000300030403047450000000000400030003000000000000000464b004a00004400484848000048434441000000004a440048484500450000000000000000000000000000000000000000000000000000000000000000000000000000003028c4282a2f1d30
00300000300000300000000000000041424300000000000044004444300000000000000000440030003030000000303000460000000000004b000000000000400000000000000000400000000000000043000000000000000000000000000000000000000000000000000000000000000000000000000000302a302a29302830
acacac000000acacac000000acacac000000acacac000000acacac000000acacac000000acacac000000acacac000000acacac000000acacac000000acacac000000acacac000000acacac000000acacac00000000000000000000000000000000000000000000000000000000000000000000000000000030291b29f8282a30
d5d5d5d5d5d5d5d5d5d5d5d5000000000000000000000000d5d5d5d5d5d5d5d5d5d5d5d5000000000000000000000000d5d5d5d5d5d5d5d5d5d5d5d5000000000000000000000000d5d5d5d5d5d5d5d5d500000000000000000000000000000000000000000000000000000000000000000000000000000030282a56461d2a30
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000301b2928c4282830
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000003047282a28294730
c8c8c8c8c8c80000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000003030303030303030
__sfx__
012000000c1700c1700c0600c0500c1450c0300c0200c010001000010000100001000010000100001000010000100001000010000100001000010000100001000010000100001000010000100001000010000100
012000000c0700c0700c1600c0500c1400c1350c0200c110000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
01100d101a0441b7341f024247341b0441f73424024277341f04424734270242b734300441b7341f0242473400004000040000400004000040000400004000040000400004000040000400004000040000400004
011000010c15000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
01060000247742476124751247412b7742b7413277432761327513274232735000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
01070000247542b73432714267542d73434714287542f734367142a75431734387143972039711397123971500700007000070000700007000070000700007000070000700007000070000700007000070000700
010800001ad451ad451fd3524d351ed251ed2523d1528d151ed451ed4523d3528d3522d2522d2527d152cd1522d4022d4027d302cd3026d2026d202bd1030d1026d4726d472bd3730d372ad362ad362fd3634d36
012000200c033137021370213702246151670216702167020c033197021970219702246151c7021c7020c0330c033137021370213702246151670216702167020c033197021970219702246150c0331c7020c033
014000000c8500c8410c8310c8210f9500f9410f9310f921089500894108831088210a8500a9410a8350a0210c8500c8410c8310c8210f9500f9410f93103712089500894108831088210a8500a9410a8350a021
014000001b7401b7111b7121b7151f7401f7111f7121f7151d7401d7111d7121d7151a7401a7111a7121a715187401871118712187151674016711167150f7400f7110f7120f7120f71511740117111171211715
0140000018a4418a4418a4418a4418a4418a4518a4418a4518a4418a4418a4418a4418a4418a4518a4418a4518a4418a4418a4418a4418a4418a4518a4418a4518a4418a4418a4418a4418a4418a4518a4418a45
0140000018a1418a1018a1018a1018a1018a1018a1018a1518a1418a1018a1018a1018a1018a1018a1018a1518a1424a1024a1018a1018a1024a1024a1018a1518a1424a1224a1218a1218a1224a1224a1218a15
014000001b7401b7111b7121b7151f7401f7111f7121f715227402271122712227151a7401a7111a7121a715247402471124712247152274022711227151b7401b7111b7121b7121b7151d7401d7111d7121d715
0114000002b7002b4002b311d71405b7005b4502b4502b701c704107140e514157141a71410714117141571402b7002b4002b311d71405b7005b4502b4502b701c704107140e514157141d714217141d7140c733
011400000c03311714157141a7141071411714157140c0331071411714157141a7141071411714157141a7140c03311714157141a7141071411714157140c0331071411714157141a7141071411714157170e714
0114000005b7505b4005b210fb400fb4511b0502b450eb501c7040f7140e514157141a7140f714117141571402b7002b450cb501571411b4011b350eb4502b701c704107140e514157141d714217141d7140c733
011400000c03311714157141a7140f71411714157140c0330f71411714157141a7140f71411714157141a7140c03311714157141a7141071411714157140c0331071411714157141a7141071411714157170e714
011400001d7201d7111d7121d7151071411714157140c033217202171121712217151071411714157141a7141f7201f7111f7121f7151071411714157140c0331c7201c7111c7121c7151071411714157140c033
0114000002b7002b4102b111d71405b7005b4502b4502b701a714107140e51405b751a71405b7005b4105b210ab600ab300ab111d7140ab600ab350ab150cb600cb35107140e5140ab751a7140cb600cb310cb11
011400001a7201a7111a7121a7151071411714157140c033187201871118712187151071411714157141a7141172011711117121171510714117141571413720137201371113712137151071411714157140c033
0114000002b7002b4102b111d71405b700504502b4505b7005b4105b210e51405b751a71405b700504505b210ab600ab300ab111d7140ab600a0350ab150cb600cb310cb110e5140ab751a7140cb60000450cb11
011400001d7201d7111d7121d7151071411714157140c033217202171121712217151071411714157141a714247202471124712247151071411714157140c0331c7201c7111c7121c7151071411714157140c033
01140000267202671126712267151071411714157140c033247202471124712247151071411714157141a7141d7201d7111d7121d7151071411714157141f7201f7121f7121f7151a7141071411714157140c033
0114000002d2402d2102d212171405d4005d2502d450c03302d1002d2102d310e514157141a714107141171402d2402d2102d212171405d4005d2502d450c03302d1002d2102d310e514157141a7141071411714
0114000007b7007b4107b111d71407b700704507b4502b7002b4102b210e51405b751a71402b700204502b210ab600ab300ab111d7140ab600a0350ab150cb600cb310cb110e5140ab751a7140cb60000450cb11
01140000227202271122712227151071411714157140c033217202171121712217151071411714157141a7141a7201a7111a7121a71510714117141571418720187111871118712187151071411714157140c033
0114000002b7002b4102b111871402b700204502b450ab700ab410ab210e51405b751a7140ab700a0450ab2007b6007b3007b111d71407b600703507b1509b6009b3109b110e5140ab751a71409b600904509b11
01140000297202971129712297151071411714157140c03326720267112671226715107141171415714117142672026711267122671510714117141571425720257112571125712257151071411714157140d033
014000001e7401e7311e7211e711217402173121721217111a7401a7401a7311a7301a7211a7201a7111a71223740237312372123711217402173121721217111a7401a7311a7211c7401c7411c7311c7211c711
014000000e8400e8300e8200e8100c8400c8300c8200c8100b9400b9300b9200b9100c8400c8300c8200c8100e8400e8300e8200e8100c8400c8300c8200c8100b9400b9300b9200b9300a8400a8300994009930
013000001d7401d7311d7211d7121a7401a7311a7211a7121a7401a7311a7211a712197401973119721197111a7401a7401a7311a7301a7211a7201a7111a7101a7101a7101a7101a7101a7151a7001a7001a700
0130000013b5013b31077300772514b5014b31087300872515b5015b31097300672109b5009b31097300472102b5002b5002b4102b4002b3102b3002b2102b2002b1002b1002b1002b1002b151ab001ab001ab00
011800003f255030003f255030003f25518d7418d7018d7018d7018d7018d0018d0018d0000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000000
01030000245552b535325150000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005
01010000265352b535005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500
000100003f620010203f620014203f6210120000000000000162101421016210142101621012000b6000060000600006000060000600006000060000600006000060000600006000060000600000000000000000
000200000733010621176112361132610000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010200000c07006751037350070000700007000070000700007000070000700007000070000700007000070000700007000070000700007000070000700007000070000700007000070000700007000070000700
01020000012732e673012732b6630b263016632125323653012531b64301243146330d233016331c2332362301223156231e22312613012130161322203016031b2030160313203016032a203146030120301603
010200003f620091133f6110911300000000000b6300b1330b6110b11300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010200000b6400b1130b6110b1130b600000003f630092233f6110911300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010600000c57405551005310000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
01030000182301841018310185121801512220122150a2300a4100a3100a0150a7150000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010600000b07012741127350c07013741137350d07014741147350f0701674116735182001840018300185021800512200122050a2000a4000a3000a0050a70500000000000d0001400014005000000000000000
010600000b07012741127350c07013741137350d07014741147350f0701674116735182401842018310185121801512230122150a2400a4200a3100a0150a715007000d700147001470500700007000070000700
010600000b07012741127350c07013741137350d07014741147350f0701674116735267442673126721267212d7442d711347443474134731347223471500000000000d000140001400500000000000000000000
010600001a5501a0111a0111a01521550210112101121015285502801128011280151f0001f0021f0021f002260042600126001260012600126002260022600226002260051f000240001a0001c0001f00024000
0105000018250184211831118512180121871512350120150a2500a4110a3110a0151a0001c0001f0002400000000000000000000000000000000000000000000000000000000000000000000000000000000000
010800001a7241a7711a7611a7511a7111a7111a7121a7122172421771217612175121711217122171221712287242877128761287512a7412a7322a7222a7122a7122a7151f000245001a0001c7001f50024700
000600002d6551f65500000055630e3540f3311002111725123000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000200001f533356333b203336330123339603232333263338203316330143335603013332f6332f2030132318123286030c52327623212031e62302523176031a1130e6130b2030761301013036032471301613
010f00002d27321363164530c3430733303323013130d50309503075031550300003000030000300003000031d303123031b0030000300003000030000300003153030b3031a7031f5031b003217031d50322003
0005000011574160741357418074155641a064165641b064185641d0541a7541f5541b054217541d544220441f744245342103426734220342772424024297140070400704007040070400704007040070400704
0004000014173007000c1000000000000000001016300700000000000000000000000b15300700000000000000000000000614300700000000000000000000000313300700000000000000000000000112300700
01010000122303f6501b6300862000003000000000000000036200327003620032700000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00010000182761e623182661d623182561e623182561d623182561e623182461d623182461e623182461d623182361e623182361d623182361e625182261d625182261e615182161d615182161e615182161d615
010100001b5611e06125061010001a0511d0512405100000197411c7412374100700187301b731227310050000000000000000000000000000000000000000000000000000000000000000000000000000000000
0001000024340270512e0510a50025541280412f0410b0002672129721307210c700277112a711317110d00000000000000000000000000000000000000000000000000000000000000000000000000000000000
01020000071540f163163730b22332643216331c6231861315613136130e6130a613187551a5551c7551554517745195451273514535167350f52511725135250c7150e515107150050000000000000000000000
01020000071540f163163730b22332643216331c6231861315613136130e6130a61304600000000000000000000000b1010710105101031010110100000000000000000000000000000000000000000000000000
0006000009630106400e6300160009600106000e60006600000001660012600116000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000300001e640000001260000000000000a6600463000000000000260003600000000000000000000000000000000076000000000000000000000000000000000000000000000000000000000000000000000000
010100000f1450c100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010100000c14500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
__music__
01 08090a0b
02 080c0a0b
01 0d0e4344
00 0d0e4344
00 0f104344
00 0f104344
00 0d0e4344
00 0d0e4344
00 0f104344
00 0f104344
00 12114344
00 14135244
00 12155544
00 14165344
00 0d0e4344
00 0d0e4344
00 0f104344
00 0f104344
00 12154344
00 14165244
00 18195344
02 1a1b5944
00 06474744
03 1c1d0744
04 1e1f4344

