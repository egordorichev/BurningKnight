pico-8 cartridge // http://www.pico-8.com
version 19
__lua__
--upward
--by matthias

a=true
function _init()
b()
c()
d()
if(e()) f.g="piracy"
h()
i()
j(f.k)
end
function _update()
if(f.g=="start"or f.g=="start_downward") l()
if(f.g=="play") then
foreach(m,n)
foreach(o,p)
q()
r()
s()
end
end
function _draw()
if(f.g=="start"or f.g=="start_downward") t()
if(f.g=="play") u()
if(f.g=="screenshot") v()
if(f.g=="piracy") w()
end
function b()
poke(0x5f2d,1)
end
function c()
palt(0,false)
palt(15,true)
end
function x()
print("level: "..tostr(f.k),0,0,0)
print("entities: "..count(m),0,6,0)
print("x: "..tostr(y.z),0,12,0)
print("y: "..tostr(y.ba),0,18,0)
end
function bb(bc,bd)
rectfill(0,56,127,72,bd)
print(bc,64-#bc*2,62,7)
end
function be(bf,bg,bh,bi)
local bj={
bk=bf,
z=bg,
ba=bh,
spr=bi,
bl=0}
return bj
end
function n(bm)
bm.bk(bm)
end
function bn(bm)
spr(bm.spr,bm.z,bm.ba,1,1,bm.bo==-1)
end
function bp(bq)
bq.z+=bq.br
bq.ba+=bq.bs
end
function bt(bq)
if(bq.bs<0) then
bq.bs+=bq.bu
if(bv(bq)) bq.bs=0
end
if(bq.bs>=0) then
bq.bs+=bq.bw
if(bq.bs>bq.bx) bq.bs=bq.bx
if(not bq.by and bz(bq)) then
if(not bq.ca) then
bq.ca=true
sfx(1)
local cb=cc(bq.z,bq.ba-(bq.ba%8)+7,rnd(.2)*-1-.2,0,1.2,0,7)
local cd=cc(bq.z+7,bq.ba-(bq.ba%8)+7,rnd(.2)+.2,0,1.2,0,7)
add(o,cb)
add(o,cd)
end
bq.bs=0
bq.ba=bq.ba-(bq.ba%8)
end
end
end
function ce(bq,cf,cg,ch,ci,cj)
if(bq.ba<0) return false
ck=fget(mget(flr((bq.z+cf+cl*8)/8),flr((bq.ba+cg+cm*8)/8)),cj)
cn=fget(mget(flr((bq.z+ch+cl*8)/8),flr((bq.ba+ci+cm*8)/8)),cj)
return ck or cn
end
function co(bq,cj)
local cf=-bq.cp
local ch=7+bq.cq
local cg=8
local ci=8
return ce(bq,cf,cg,ch,ci,cj)
end
function bz(bq)
return co(bq,0)
end
function cr(bq)
return co(bq,1)
end
function cs(bq)
return co(bq,2)
end
function ct(bq)
return co(bq,4) and mget(flr((bq.z+cl*8)/8),flr((bq.ba+16+cm*8)/8))!=0
end
function cu(bq,ba)
local cf=-bq.cp
local ch=7+bq.cq
local cg=ba
local ci=ba
local cj=3
return ce(bq,cf,cg,ch,ci,cj)
end
function bv(bq)
return cu(bq,0)
end
function cv(bq)
return cu(bq,-8)
end
function cw(bq,z)
local cf=z
local ch=z
local cg=8
local ci=8
local cj=0
return not ce(bq,cf,cg,ch,ci,cj)
end
function cx(bq)
return cw(bq,8)
end
function cy(bq)
return cw(bq,-1)
end
function cz(bq)
return cw(bq,-1) and cw(bq,8)
end
function da(bq,z)
local cf=z
local ch=z
local cg=0
local ci=7
local cj=3
return ce(bq,cf,cg,ch,ci,cj)
end
function db(bq)
return da(bq,-1)
end
function dc(bq)
return da(bq,8)
end
function dd(de,df)
if((de.z-de.cp<=df.z+8+df.cq) and
(de.z+8+de.cq>=df.z-df.cp) and
(de.ba-de.dg<=df.ba+8+df.dh) and
(de.ba+8+de.dh>=df.ba-df.dg)) then
return true
else
return false
end
end
function di()
m={}
for dj=0,15 do
for dk=0,15 do
dl=dk+cl
dm=dj+cm
if(mget(dl,dm)>=16 and mget(dl,dm)<=19) then
dn=0 dp=16
if(mget(dl,dm)==18 or mget(dl,dm)==19) then
dn=4
dp=18
end
if(not f.dq) then
local dr=ds(dk*8+dn,dj*8)
add(m,dr)
end
mset(dl,dm,dp)
end
if(mget(dl,dm)==29) then
local dt=du(dk*8,dj*8)
add(m,dt)
end
if(mget(dl,dm)==31) then
local dt=dv(dk*8,dj*8)
add(m,dt)
end
if(mget(dl,dm)==44) then
local dw=dx(dk*8,dj*8)
add(m,dw)
end
if(mget(dl,dm)==40) then
local dw=dy(dk*8,dj*8)
add(m,dw)
end
if(mget(dl,dm)>=59 and mget(dl,dm)<=63) then
dz=1
ea=59
if(mget(dl,dm)==62 or mget(dl,dm)==63) then
dz=-1
ea=62
end
if(not f.dq) then
local eb=ec(dk*8,dj*8,dz)
add(m,eb)
end
mset(dl,dm,ea)
end
if(mget(dl,dm)>=54 and mget(dl,dm)<=58) then
dz=1
ea=54
if(mget(dl,dm)==57 or mget(dl,dm)==58) then
dz=-1
ea=57
end
if(not f.dq) then
local eb=ed(dk*8,dj*8,dz)
add(m,eb)
end
mset(dl,dm,ea)
end
if(mget(dl,dm)==48 or mget(dl,dm)==49 or mget(dl,dm)==39) then
local dp=39
if(not f.dq) then
local ee=ef(dk*8,dj*8)
add(m,ee)
dp=48
end
mset(dl,dm,dp)
end
if(mget(dl,dm)==20 or mget(dl,dm)==21) then
if(not f.dq) then
local eg=eh(dk*8,dj*8)
add(m,eg)
end
mset(dl,dm,20)
end
if(mget(dl,dm)>=36 and mget(dl,dm)<=38) then
local dp=38
if(not f.dq) then
local ei=ej(dk*8,dj*8)
add(m,ei)
dp=36
end
mset(dl,dm,dp)
end
if(mget(dl,dm)==22 or mget(dl,dm)==23) then
if(not f.dq) then
local eg=ek(dk*8,dj*8)
add(m,eg)
end
mset(dl,dm,22)
end
if(mget(dl,dm)>=25 and mget(dl,dm)<=27) then
local dp=27
if(not f.dq) then
local ei=el(dk*8,dj*8)
add(m,ei)
dp=25
end
mset(dl,dm,dp)
end
if(mget(dl,dm)==7) then
local em=en(dk*8,dj*8)
add(m,em)
end
if(mget(dl,dm)==13 or mget(dl,dm)==14) then
local eo=ep(dk*8+4,dj*8)
add(m,eo)
mset(dl,dm,13)
end
if(mget(dl,dm)==200) then
local eq=er(dk*8,dj*8)
add(m,eq)
end
end
end
end
function es(et)
for eu=1,count(et) do
if(et[eu].ev=="enemy_black_right") then
local eb=ec(et[eu].z,et[eu].ba,1)
add(m,eb)
end
if(et[eu].ev=="enemy_black_left") then
local eb=ec(et[eu].z,et[eu].ba,-1)
add(m,eb)
end
if(et[eu].ev=="enemy_white_right") then
local eb=ed(et[eu].z,et[eu].ba,1)
add(m,eb)
end
if(et[eu].ev=="enemy_white_left") then
local eb=ed(et[eu].z,et[eu].ba,-1)
add(m,eb)
end
if(et[eu].ev=="broken_ground") then
local ew=ef(et[eu].z,et[eu].ba)
add(m,ew)
mset(flr(et[eu].z/8+cl),flr(et[eu].ba/8+cm),48)
end
if(et[eu].ev=="ms_blob") then
local ew=ex(et[eu].z,et[eu].ba)
add(m,ew)
end
end
end
function e()
local ey={
0,
'',
'127.0.0.1',
'localhost',
'lexaloffle.com',
'www.lexaloffle.com',
'pocketfruit.itch.io',
'playpico.com',
'www.playpico.com'
}
local ez=stat(102)
local fa=true
for de in all(ey) do
if(ez==de) fa=false
end
if(sub(ez,-14)=='.ssl.hwcdn.net') fa=false
return fa
end
function w()
if(not fb) fb=stat(102)
cls()
print("you are playing this game from",0,0,8)
print("an unauthorized website.",0,6,8)
print("please visit",0,18,8)
print("pocketfruit.itch.io",0,24,7)
print("to play a legitimate copy.",0,30,8)
print("please help to avoid piracy",0,42,8)
print("and properly support the author",0,48,8)
print("of this game.",0,54,8)
print("thank you.",0,66,8)
print("domain: "..fb,0,78,9)
end
function d()
f={}
f.k=1
f.fc=31
f.fd=0
f.fe=0
f.ff=0
f.fg=0
f.fh=nil
f.g="start"
f.dq=false
f.x=false
f.fi=false
f.fj=-48
f.fk=false
f.fl=0
f.fm=false
f.bl=0
end
function h()
if(a) then
cartdata("mtths_upward")
a=false
end
f.k=dget(0)
if(f.k>15) f.g="start_downward"
f.fe=dget(1)
if(f.k==nil or f.k==0) fn()
end
function fo()
dset(0,f.k)
dset(1,f.fe)
end
function fn()
f.k=1
dset(0,f.k)
f.fe=0
dset(1,f.fe)
end
function u()
rectfill(0,0,127,127,7)
map(cl,cm)
foreach(m,bn)
if(y.eo) spr(14,y.z-3,y.ba-2)
spr(y.spr,y.z,y.ba)
foreach(o,fp)
if(fq.fr and not fq.fs) bb("oh yeah",8)
if(fq.ft) bb("oh no",0)
if(fq.fu) bb("there must be a way",0)
if(fq.fv and fq.bl<=75) bb("you got medicine",8)
if(fq.fv and fq.bl>75) bb("this is heavy...",8)
if(fq.fw) fq.fx+=1
if(fq.fw and fq.fx>30 and not fq.fu) then
y.bb="   stuck? push ⬅️+➡️"
y.fy=90
fq.fw=false
fq.fx=0
end
if(f.fm) spr(216,4,4)
if(y.bb and not fq.fu) then
fz=y.bb
if(f.k!=31) rectfill(y.z-#fz*2+3,y.ba-8,y.z+#fz*2+3,y.ba-8+4,7)
print(fz,y.z-#fz*2+3,y.ba-8,0)
end
if(fq.ga) bb("the end",0)
if(fq.gb) then
rectfill(0,73,127,89,7)
local fz="deaths: "..tostr(f.fd)
print(fz,64-#fz*2,75,0)
local fz="time: "..f.fh
print(fz,64-#fz*2,83,0)
fz="made with ♥ by matthias falk"
print(fz,64-#fz*2-2,119,6)
end
if(f.fi) x()
end
function t()
rectfill(0,0,127,127,7)
map(cl,cm)
if(y.eo and f.k==15) spr(14,y.z-3,y.ba-2)
spr(y.spr,y.z,y.ba)
local cg=f.fj*1
if(cg>8) cg=8
local ci=f.fj*1
if(ci>16) ci=16
if(f.g=="start_downward") rectfill(22,0+cg,105,16+cg,0)
if(f.g=="start") sspr(0,96,64,16,32,1+cg)
if(f.g=="start_downward") sspr(0,112,80,16,24,1+cg)
fz="jump: 🅾️/⬆️"
if(f.g=="start_downward") fz="down: 🅾️/⬇️"
if(f.fj>60 and f.fl%3==0) then
rectfill(64-#fz*2-3,76,64+#fz*2-3,80,7)
print(fz,64-#fz*2-3,76,0)
end
if(f.k>1 and f.fj>60 and not f.fk) then
fz="new game? push ❎❎❎"
rectfill(64-#fz*2-3,100,64+#fz*2-3,104,7)
print(fz,64-#fz*2-3,100,6)
end
fz="made with ♥ by matthias falk"
print(fz,64-#fz*2-2,135-ci,6)
end
function l()
if(f.fj==0) music(1)
f.fj+=1
if(not f.fk and f.fj>60 and(f.g=="start"and(btnp(2) or btnp(4) or stat(34)==1) or(f.g=="start_downward"and(btnp(3) or btnp(4) or stat(34)==1)))) then
music(-1)
f.fk=true
sfx(8)
end
if(f.fk) then
f.fl+=1
if(f.fl>90) then
if(f.g=="start_downward") then
f.dq=true
y.gc="downward"
end
f.g="play"
f.fj=-64
f.fl=0
f.fk=false
menuitem(1,"retry level",gd)
end
end
if(f.k>1 and f.fj>60 and btnp(5)) fq.ge+=1
if(f.k>1 and f.fj>60 and fq.ge==3) then
fn()
f.k=1
f.dq=false
f.g="start"
j(f.k)
music(-1)
f.fk=true
sfx(8)
end
end
function v()
rectfill(0,0,127,127,7)
map(48,32)
sspr(0,96,64,16,32,56)
fz="made with ♥ by matthias falk"
print(fz,64-#fz*2-2,119,6)
end
function s()
if(f.k==6 and y.z>126) gf()
if((stat(34)==1 and stat(32)<16 and stat(33)<16) or btn(0) and btn(1)) then
if(not fq.fr and not fq.ft and not fq.fu and not fq.gg) gd()
end
if(f.x and btnp(0)) gh()
if(f.x and btnp(1)) gf()
if(fq.fr) then
f.bl+=1
if(f.bl>60) fq.fs=true
if(f.bl>75 and fq.gi==false) then
if(cs(y)) then
y.z+=1
else
fq.gi=true
gj(y.gk)
end
end
if(fq.fs and(y.ba<-24 or y.z>127)) gf()
end
if(y.ba>127) then
if(f.dq and not fq.ft and not fq.fu and not(f.k>=24 and f.k<=27)) then
gf()
else
gl()
end
end
if(f.k>=24 and f.k<=27 and y.z<-2) gf()
if(fq.ft) then
f.bl+=1
if(f.bl>30) then
j(f.k)
f.fd+=1
end
end
if(fq.fv) then
fq.bl+=1
if(fq.bl>180) then
fq.fv=false
fq.bl=0
fq.gm=true
end
end
if(fq.gm) then
fq.gm=false
fq.bl=0
f.g="start_downward"
end
if(fq.gg) then
if(fq.gn==1) then
f.bl+=1
y.gc="cutscene"
if(f.bl==30) y.z=64
if(f.bl==60) then
y.eo=false
y.spr=6
local eo=ep(y.z,y.ba-8)
eo.gc="use"
add(m,eo)
end
end
if(fq.gn==2) then
f.bl+=1
if(f.bl==30) y.spr=2
if(f.bl==90) fq.go.spr=10
if(f.bl==120) fq.go.gc="idle"
if(f.bl>150 and f.bl<180) y.bb="♥  "
if(f.bl==240) fq.go.gc="moving"
if(f.bl>240 and y.z<140) y.z+=.5
if(y.z==140) then
fq.go.br=0
fq.go.gc="idle"
end
if(f.bl==360) fq.ga=true
if(f.bl==420) fq.gb=true
if(f.bl>420 and(btnp(4) or stat(34)==1)) then
music(-1)
fn()
_init()
end
end
end
if(stat(34)==1) f.fm=true
end
function i()
gp={}
local gq={
cl=0,cm=0,
gr=60,gs=96,
}
add(gp,gq)
local gt={
cl=16,cm=0,
gr=60,gs=96,
}
add(gp,gt)
local gu={
cl=32,cm=0,
gr=60,gs=96,
}
add(gp,gu)
local gv={
cl=48,cm=0,
gr=28,gs=96,
m={
{ev="enemy_white_right",
z=64,ba=56},
{ev="enemy_white_left",
z=80,ba=32}
}
}
add(gp,gv)
local gw={
cl=64,cm=0,
gr=4,gs=8,
}
add(gp,gw)
local gx={
cl=80,cm=0,
gr=4,gs=80,
}
add(gp,gx)
local gy={
cl=96,cm=0,
gr=4,gs=80,
}
add(gp,gy)
local gz={
cl=112,cm=0,
gr=60,gs=96,
}
add(gp,gz)
local ha={
cl=0,cm=16,
gr=92,gs=100,
}
add(gp,ha)
local hb={
cl=16,cm=16,
gr=20,gs=100,
}
add(gp,hb)
local hc={
cl=32,cm=16,
gr=60,gs=100,
}
add(gp,hc)
local hd={
cl=48,cm=16,
gr=60,gs=100,
}
add(gp,hd)
local he={
cl=64,cm=16,
gr=16,gs=100,
}
add(gp,he)
local hf={
cl=80,cm=16,
gr=24,gs=100,
}
add(gp,hf)
local hg={
cl=96,cm=16,
gr=60,gs=100,
}
add(gp,hg)
local hh={
cl=80,cm=16,
gr=60,gs=4,
dq=true
}
add(gp,hh)
local hi={
cl=64,cm=16,
gr=104,gs=4,
dq=true,
m={
{ev="enemy_black_right",
z=16,ba=32},
{ev="enemy_white_right",
z=48,ba=32},
{ev="enemy_black_right",
z=80,ba=32},
{ev="enemy_white_right",
z=60,ba=80},
}
}
add(gp,hi)
local hj={
cl=48,cm=16,
gr=88,gs=4,
dq=true,
m={
{ev="enemy_white_right",
z=48,ba=80},
{ev="broken_ground",
z=24,ba=88},
{ev="enemy_white_right",
z=24,ba=80},
}
}
add(gp,hj)
local hk={
cl=32,cm=16,
gr=80,gs=4,
dq=true,
m={
{ev="enemy_white_right",
z=48,ba=80},
{ev="broken_ground",
z=32,ba=64},
{ev="broken_ground",
z=40,ba=64},
{ev="broken_ground",
z=48,ba=64},
{ev="broken_ground",
z=88,ba=64}
}
}
add(gp,hk)
local hl={
cl=16,cm=16,
gr=60,gs=4,
dq=true,
m={
{ev="enemy_white_left",
z=104,ba=56}
}
}
add(gp,hl)
local hm={
cl=0,cm=16,
gr=20,gs=4,
dq=true,
m={
{ev="enemy_white_left",
z=48,ba=80}
}
}
add(gp,hm)
local hn={
cl=112,cm=0,
gr=92,gs=4,
dq=true,
m={
{ev="enemy_white_right",
z=32,ba=56},
{ev="enemy_white_left",
z=88,ba=80}
}
}
add(gp,hn)
local ho={
cl=96,cm=0,
gr=60,gs=4,
dq=true
}
add(gp,ho)
local hp={
cl=112,cm=16,
gr=60,gs=4,
dq=true
}
add(gp,hp)
local hq={
cl=0,cm=32,
gr=120,gs=80,
dq=true,
m={
}
}
add(gp,hq)
local hr={
cl=16,cm=32,
gr=120,gs=80,
dq=true,
m={
{ev="enemy_white_right",
z=6,ba=32}
}
}
add(gp,hr)
local hs={
cl=32,cm=32,
gr=112,gs=32,
dq=true,
m={
{ev="enemy_white_left",
z=120,ba=32}
}
}
add(gp,hs)
local ht={
cl=32,cm=0,
gr=112,gs=32,
dq=true,
m={
{ev="enemy_white_left",
z=120,ba=32},
{ev="enemy_white_right",
z=52,ba=56},
{ev="enemy_black_right",
z=48,ba=80}
}
}
add(gp,ht)
local hu={
cl=16,cm=0,
gr=60,gs=4,
dq=true,
m={
{ev="enemy_black_left",
z=64,ba=32},
{ev="enemy_black_right",
z=48,ba=56}
}
}
add(gp,hu)
local hv={
cl=0,cm=0,
gr=60,gs=4,
dq=true
}
add(gp,hv)
local hw={
cl=48,cm=32,
gr=60,gs=4,
dq=true,
m={
{ev="ms_blob",
z=56,ba=104}
}
}
add(gp,hw)
hx()
end
function j(k)
cl=gp[k].cl
cm=gp[k].cm
gr=gp[k].gr
gs=gp[k].gs
hx()
hy(gr,gs)
f.dq=false
if(gp[k].dq) then
f.dq=true
y.gc="downward"
y.eo=true
end
di()
if(gp[k].m) es(gp[k].m)
end
function hx()
fq={}
fq.fr=false
fq.ft=false
fq.fu=false
fq.fs=false
fq.gi=false
fq.hz=false
fq.ia=false
fq.ib=false
fq.ic=false
fq.id=0
fq.bl=0
fq.gm=false
fq.fv=false
fq.gg=false
fq.gn=1
fq.ga=false
fq.gb=false
fq.go=nil
fq.ie=0
fq.ig=false
fq.fw=false
fq.fx=0
fq.ge=0
fq.m={}
o={}
f.bl=0
end
function r()
if(fq.ib) then
fq.bl+=1
if(fq.bl>3) then
fq.ib=false
fq.bl=0
end
end
if(fq.ic) then
fq.id+=1
if(fq.id>15) then
fq.ic=false
fq.id=0
end
end
if(f.k==24 and fq.ig==false) then
fq.ie+=1
if(fq.ie>120) then
fq.ig=true
local eb=ed(120,32,-1)
add(m,eb)
end
end
end
function gd()
gl()
fq.fu=true
end
function gf()
f.k=mid(1,f.k+1,f.fc)
f.fe+=time()-f.ff
fo()
j(f.k)
f.ff=time()
end
function gh()
f.k=mid(1,f.k-1,f.fc)
j(f.k)
end
function hy(z,ba)
y={}
y.gc="idle"
y.ih=false
y.ii=false
y.by=false
y.z=z
y.ba=ba
y.cp=-1
y.cq=-1
y.dg=-1
y.dh=0
y.br=0
y.bs=0
y.bu=.4
y.bw=.6
y.bx=4
y.gk=-5
y.ij=-5
y.ca=true
y.ik=0
y.bl=0
y.il=0
y.spr=1
y.eo=false
y.bb=nil
y.fy=0
end
function q()
y.ih=btnp(2) or btnp(4) or(stat(34)==1 and stat(32)>16 and stat(33)>16)
y.ii=btnp(3) or btnp(4) or(stat(34)==1 and stat(32)>16 and stat(33)>16)
if(y.gc!="cutscene") im()
if(y.gc=="idle") io()
if(y.gc=="downward") ip()
if(y.gc=="dead") iq()
bp(y)
if(y.br!=0) y.br*=.8
if(y.gc!="dead") bt(y)
if(y.ik>0) y.ik-=1
end
function im()
if(y.bs<0) then
y.bl+=.7
y.spr=(y.bl/3)+3
y.spr=mid(3,y.spr,5)
y.ca=false
elseif(y.bs>0) then
y.bl-=1
y.spr=(y.bl/3)+5
y.spr=mid(3,y.spr,5)
y.ca=false
else
y.bl=0
y.spr=2
end
if(y.gc=="dead") y.spr=6
end
function io()
if(y.ih and bz(y) and y.bs==0) gj(y.gk)
if(not btn(2) and y.bs<y.ij) y.bs=y.ij
ir()
if((btn(0) or btn(1)) and f.k<5) then
y.bb="i can only jump"
y.fy=30
end
if(y.fy>0) y.fy-=1
if(y.fy==0) y.bb=nil
end
function ip()
if(y.il>0) y.il-=1
if(y.il==0) y.by=false
if(y.ii and ct(y) and y.bs==0) then
y.by=true
y.il=5
end
ir()
end
function gj(is)
sfx(0)
if(not cv(y)) then
y.bs=is
else
y.bs=-1
end
end
function ir()
if(cr(y) and y.bs==0) y.z-=1
if(cs(y) and y.bs==0) y.z+=1
end
function gl()
if(not fq.ft==true) then
fq.ft=true
sfx(4)
y.gc="dead"
y.bs=-2.5
end
end
function iq()
y.bs+=.4
if(y.bs>4) y.bs=4
y.ba+=y.bs
end
function ds(bg,bh)
local dr=be(
it,
bg,bh,17)
dr.cp=0
dr.cq=0
dr.dg=0
dr.dh=0
add(m,dr)
end
function it(bm)
if dd(y,bm) then
if(not fq.fr and y.gc!="dead") then
fq.fr=true
y.gc="won"
local cb=cc(bm.z,bm.ba+4,-2,.4,2.4,9,9)
local cd=cc(bm.z+7,bm.ba+4,2,.4,2.4,9,9)
add(o,cb)
add(o,cd)
local cb=cc(bm.z,bm.ba+4,-1,.2,2,9,7)
local cd=cc(bm.z+7,bm.ba+4,1,.2,2,9,7)
add(o,cb)
add(o,cd)
del(m,bm)
music(0)
end
end
end
function dx(bg,bh)
return be(
iu,
bg,bh,44)
end
function iu(bm)
bm.bl+=.5
bm.spr=(bm.bl/4)%4+44
end
function dy(bg,bh)
return be(
iv,
bg,bh,40)
end
function iv(bm)
bm.bl+=.5
bm.spr=(bm.bl/4)%4+40
end
function du(bg,bh)
local iw=be(
ix,
bg,bh,29)
iw.iy=-1
return iw
end
function ix(bm)
bm.bl+=1
if(bm.bl>30) then
if(bm.iy==-1) then
local iz=ja(bm.z-4,bm.ba,bm.iy)
add(m,iz)
else
local iz=ja(bm.z+4,bm.ba,bm.iy)
add(m,iz)
end
bm.bl=0
sfx(7)
end
end
function dv(bg,bh)
local iw=du(bg,bh)
iw.spr=31
iw.iy=1
return iw
end
function ja(bg,bh,bo)
if(bo==-1) then
jb=28
jc=-2
jd=0
je=-3
if(f.k==14) jc=-1
else
jb=30
jc=2
jd=-3
je=0
if(f.k==14) jc=1
end
local iz=be(
jf,
bg,bh,jb)
iz.jg="bullet"..tostr(rnd(128))
iz.jh=jc
iz.cp=jd
iz.cq=je
iz.dg=-2
iz.dh=-2
return iz
end
function jf(bm)
if(dd(bm,y)) gl()
bm.z+=bm.jh
if(bm.z<-10 or bm.z>138) then
for ji in all(m) do
if(ji.jg==bm.jg) del(m,ji)
end
end
end
function ef(bg,bh)
local ee=be(
jj,
bg,bh,49)
ee.jk=false
return ee
end
function jj(bm)
if(y.ba+8==bm.ba and(y.z-y.cp<bm.z+8) and(y.z+8+y.cq>bm.z)) bm.jk=true
if(bm.jk) then
bm.bl+=1
bm.spr=49+(bm.bl*.125)
if(bm.spr>54) bm.spr=54
if(bm.bl>40) then
bm.jk=false
mset(flr(bm.z/8+cl),flr(bm.ba/8+cm),39)
del(m,bm)
end
end
end
function eh(bg,bh)
local eg=be(
jl,
bg,bh,21)
eg.cp=0
eg.cq=0
eg.dg=0
eg.dh=0
return eg
end
function jl(bm)
if dd(bm,y) then
fq.hz=true
fq.ib=true
local cb=cc(bm.z,bm.ba+4,-1,0,2,0,7)
local cd=cc(bm.z+7,bm.ba+4,1,0,2,0,7)
add(o,cb)
add(o,cd)
del(m,bm)
sfx(5)
end
end
function ej(bg,bh)
return be(
jm,
bg,bh,37)
end
function jm(bm)
if(fq.hz) then
local cb=cc(bm.z+4,bm.ba,0,-2,2,0,7)
local cd=cc(bm.z+4,bm.ba+7,0,2,2,0,7)
add(o,cb)
add(o,cd)
mset(flr(bm.z/8+cl),flr(bm.ba/8+cm),38)
del(m,bm)
end
end
function ek(bg,bh)
local eg=be(
jn,
bg,bh,23)
eg.cp=0
eg.cq=0
eg.dg=0
eg.dh=0
return eg
end
function jn(bm)
if dd(bm,y) then
fq.ia=true
fq.ib=true
local cb=cc(bm.z,bm.ba+4,-1,0,2,0,0)
local cd=cc(bm.z+7,bm.ba+4,1,0,2,0,0)
add(o,cb)
add(o,cd)
del(m,bm)
sfx(5)
end
end
function el(bg,bh)
return be(
jo,
bg,bh,26)
end
function jo(bm)
if(fq.ia) then
local cb=cc(bm.z+4,bm.ba,0,-2,2,0,0)
local cd=cc(bm.z+4,bm.ba+7,0,2,2,0,0)
add(o,cb)
add(o,cd)
mset(flr(bm.z/8+cl),flr(bm.ba/8+cm),27)
del(m,bm)
end
end
function en(bg,bh)
local em=be(
jp,
bg,bh,7)
em.cp=-1
em.cq=-1
em.dg=-4
em.dh=0
return em
end
function jp(bm)
if(dd(bm,y)) gl()
end
function ep(bg,bh)
local eo=be(
jq,
bg,bh,14)
eo.br=0
eo.bs=0
eo.cp=0
eo.cq=0
eo.dg=0
eo.dh=0
eo.gc="normal"
eo.jr=false
return eo
end
function jq(bm)
if(bm.gc=="use") then
bm.bs=0
if(bm.ba>92) then
bm.bs=-.25
else
bm.jr=true
end
if(bm.jr) then
bm.bl+=1
if(bm.bl>60) then
js(bm)
sfx(20)
fq.gn=2
f.bl=0
del(m,bm)
end
end
bp(bm)
end
if(bm.gc=="normal") then
if(dd(bm,y)) then
fq.fv=true
y.gc="none"
y.eo=true
js(bm)
sfx(20)
music(11)
del(m,bm)
end
end
end
function js(bm)
local cb=cc(bm.z,bm.ba+4,-2,0,3,8,9)
local cd=cc(bm.z+7,bm.ba+4,2,0,3,8,9)
add(o,cb)
add(o,cd)
local cb=cc(bm.z,bm.ba+4,-1,0,2,8,9)
local cd=cc(bm.z+7,bm.ba+4,1,0,2,8,9)
add(o,cb)
add(o,cd)
local cb=cc(bm.z,bm.ba+4,-3,0,1,8,9)
local cd=cc(bm.z+7,bm.ba+4,3,0,1,8,9)
add(o,cb)
add(o,cd)
end
function ex(bg,bh)
local go=be(
jt,
bg,bh,12)
go.cp=0
go.cq=0
go.dg=-4
go.dh=0
go.gc="lying"
return go
end
function jt(bm)
if(dd(bm,y) and not fq.gg) then
fq.gg=true
local ju=f.fe+time()-f.ff
local jv=flr(ju/60)
local jw=ju-flr(ju/60)*60
f.fh=tostr(jv).." min "..tostr(jw).." sec"
fq.go=bm
music(7)
end
if(bm.gc!="lying") then
bm.bl+=.5
bm.spr=(bm.bl/2%2)+10
end
if(bm.gc=="moving") then
bm.z+=.5
if(not fq.ic) then
fq.ic=true
sfx(6)
local cb=cc(bm.z,bm.ba+7,-.5,0,2,0,7)
add(o,cb)
end
end
end
function er(bg,bh)
local eq=be(
jx,
bg,bh,200)
eq.cp=-1
eq.cq=-1
eq.dg=-4
eq.dh=0
eq.jk=false
return eq
end
function jx(bm)
if(dd(bm,y) and y.gc!="dead") then
gj(-7)
bm.spr=201
bm.jk=true
end
if(bm.jk) then
bm.bl+=1
if(bm.bl>10) then
bm.spr=200
bm.jy=false
bm.bl=0
end
end
end
function ec(bg,bh,iy)
local eb=be(
jz,
bg,bh,60)
eb.ev="black"
eb.cp=-1
eb.cq=-1
eb.dg=-2
eb.dh=0
eb.br=.5*iy
eb.ka=0
eb.bs=0
eb.bw=.6
eb.bx=4
eb.bo=iy
eb.kb=false
eb.ca=true
return eb
end
function kc(bm)
bm.bl+=.5
bm.spr=(bm.bl/2)%2+60
end
function ed(bg,bh,iy)
local eb=ec(bg,bh,iy)
eb.bk=jz
eb.ev="white"
eb.spr=55
return eb
end
function kd(bm)
bm.bl+=.5
bm.spr=(bm.bl/2)%2+55
end
function jz(bm)
if(bm.ev=="black") kc(bm)
if(bm.ev=="white") kd(bm)
if(not cz(bm) and bm.bs==0) then
if((not fq.ib and cx(bm)) or dc(bm)) bm.bo=-1
if((not fq.ib and cy(bm)) or db(bm)) bm.bo=1
bm.br=.5*bm.bo
else
bm.br=0
end
if(bm.bs>0) bm.ca=false
bp(bm)
bt(bm)
if(dd(bm,y)) then
if(y.ba+7<bm.ba+5 and y.bs>0 and not bm.kb and y.gc!="dead") then
bm.kb=true
gj(-2)
y.ik=10
ke=0
if(bm.ev=="white") ke=7
local cb=cc(bm.z,bm.ba+6,-1,0,2.4,0,ke)
local cd=cc(bm.z+7,bm.ba+6,1,0,2.4,0,ke)
add(o,cb)
add(o,cd)
if(bm.ev=="white"and f.k<5) fq.fw=true
del(m,bm)
end
if(bm.ev=="black"and not bm.kb and y.ik==0) gl()
if(bm.ev=="white"and y.bs==0) then
if(bm.z>y.z and bm.br<0) y.br=bm.br
if(bm.z<y.z and bm.br>0) y.br=bm.br
if(not fq.ic) then
fq.ic=true
sfx(6)
local kf=0
if(bm.bo==-1) kf=8
local cb=cc(bm.z+kf,bm.ba+7,-.5*bm.bo,0,2,0,7)
add(o,cb)
end
end
end
end
function cc(z,ba,br,bs,kg,kh,ke)
ki={}
ki.z=z
ki.ba=ba
ki.br=br
ki.bs=bs
ki.kg=kg
ki.kh=kh
ki.ke=ke
return ki
end
function p(kj)
kj.z+=kj.br
kj.ba+=kj.bs
kj.kg-=.1
if(kj.kg<=0) del(o,kj)
end
function fp(kj)
circfill(kj.z,kj.ba,kj.kg,kj.ke)
circ(kj.z,kj.ba,kj.kg,kj.kh)
end
__gfx__
00000000ffffffffffffffffff0000ffff0000ffff0000ffff0000fffffffffff00000000000000fffffffffffffeeffffffffffffffffffffffffffffffffff
00000000ffffffffff0000fff000000ff070070ff070070ff070070fffffffff0777777777777770ffffeeffff0000fffffffffffffffffffff00fffffffffff
00700700fffffffff000000ff070070ff070070ff000000ff070070fffffffff0777777007777770ff0000fff0eeee0ffffffffffffffffffff00fffff7ff7ff
00077000fffffffff070070ff070070ff000000ff007700ff000000fffffffff0000000770000000f0eeee0ff07ee70fffffffffffffffffff0770fffff77fff
00077000fffffffff070070ff000000ff007700ff000000ff007700ff0fff0ff0777777777777770f07ee70ff07ee70fff000000fffffffff079970ffff77fff
00700700fffffffff000000ff007700ff000000ff000000ff007700ff0fff0ff0777777777777770f07ee70ff0eeee0ff0eeeee0ffffffff07988970ff7ff7ff
00000000fffffffff007700ff000000ff000000ff000000ff000000f070f070f0777777777777770f0eeee0ff0e77e0f0e7eeee0ffffffff07988870ffffffff
00000000ffffffff00000000f000000ffffffffffffffffff000000f070f070f07777777777777700ee77ee0f0eeee0f0e7eeee0ffffffff00000000ffffffff
fffffffffff0fffffffffffffffffff0fffffffffffffffffffffffffffffffffffffffffffffffff000000fffffffffffffffffffffffffffffffffffffffff
fffffffffff00ffffffffffffffffff0fffffffff000ffffffffffffffffffffffffffffffffffff00000000ffffffffffffffffffff00f0ffffffff0f00ffff
fffffffff000000ffffffffffffff000ffffffff07770ffffffffffff00fffffffffffffffffffff00077000fffffffff0000f00ffff070000f0000f0070ffff
ffffffff00999990ffffffffffff0099ffffffff0707000fffffffff0770ffffffffffffffffffff00700700ffffffff00000fffffff0700fff000000070ffff
ffffffff09999990ffffffffffff0999ffffffff07777770ffffffff07700000ffffffffffffffff00777700ffffffff00000fffffff0700fff000000070ffff
ffffffff09999990ffffffffffff0999ffffffff07707070ffffffff0770f0f0ffffffffffffffff00777700fffffffff0000f00ffff00f000f0000f0f00ffff
fffffffff099990ffffffffffffff099ffffffff07707070fffffffff00ff0f0ffffffffffffffff00000000ffffffffffffffffffffffffffffffffffffffff
ffffffffff0000ffffffffffffffff00fffffffff00f0f0ffffffffffffffffffffffffffffffffff000000fffffffffffffffffffffffffffffffffffffffff
f000000ff0000000000000000000000ffffffffff000000ffffffffffffffffff000770ff007700ff077000ff070070ff077000ff007700ff000770ff070070f
07777770077777777777777777777770ffffffff07777770ffffffffffffffff0000000000000000000000000000000000000000000000000000000000000000
07777770077777700777777007777770ffffffff07700770ffffffffffffffff0000700000007000000070000000700000070000000700000007000000070000
f000000ff000000ff000000ff000000fffffffff07077070ffffffffffffffff0007000000070000000700000007000000007000000070000000700000007000
ffffffffffffffffffffffffffffffffffffffff07000070ffffffffffffffff0000700000007000000070000000700000070000000700000007000000070000
ffffffffffffffffffffffffffffffffffffffff07000070fffffffffffffffff000000ff000000ff000000ff000000ff000000ff000000ff000000ff000000f
ffffffffffffffffffffffffffffffffffffffff07777770ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff
fffffffffffffffffffffffffffffffffffffffff000000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff
fffffffff000000ff000000ff000000ff000000ff000000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff
ffffffff07777770077777700707070000f0f0ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff
ffffffff0707070007070700f0f0f0ffffffffffffffffffffffffffff0000ffffffffffffffffffff0000ffffffffffff0000ffffffffffffffffffff0000ff
fffffffff0f0f0fff0f0f0fffffffffffffffffffffffffffffffffff077770fff0000fffffffffff077770ffffffffff000000fff0000fffffffffff000000f
fffffffffffffffffffffffff0fff0fffffffffffffffffffffffffff077000ff077770ffffffffff000770ffffffffff000770ff000000ffffffffff077000f
fffffffffffffffff0f0f0fffffffffff0fff0fffffffffffffffffff077770ff077000ffffffffff077770ffffffffff000000ff000770ffffffffff000000f
fffffffffffffffffff0fffff0f0f0fffffffffff0fff0fffffffffff077770ff077770ffffffffff077770ffffffffff000000ff000000ffffffffff000000f
fffffffffffffffffffffffffff0fffff0f0f0fffffffffffffffffff077770f07777770fffffffff077770ffffffffff000000f00000000fffffffff000000f
ff00000000000000000000ffff000000000fff00000f0000000000ffff077777ffff077777707777777707777770ffff7770ffff777770ff7777777007777777
f0777777777777777777770ff07777777770f077777077777777770ff0777777ffff077777707777777707777770ffff7770ffff7777770f7777777007777777
0777777777777777777777700777777777770777777077777777777007777777ffff077777707777777707777770ffff7770ffff777777707777777007777777
0777777777777777777777777777777777777777777777777777777007777777fffff0077770777777770777700fffff770fffff777777707777777007777777
0777777777777777777777777777777777777777777777777777777007777777ffff077777700077770007777770ffff70ffffff777777707770777007770777
0777777777777777777777777777777777777777777777777777777007777777ffff077777707707707707777770ffff770fffff777777707770777007770777
0777777777777777777777777777777777777777777777777777777007777777ffff077777707770077707777770ffff7770ffff777777707770770ff0770777
0777777777777777777777777777777777777777777777777777777007777777ffff077777707770077707777770ffff7770ffff77777770777000ffff000777
07777777ffffffffffffff0000ffffffffff07777770ffff7777777007777777ffff077777777777777777777770ffffffff0777777777707770ffffffff0777
07777777fffffffffffff077770fffffffff07777770ffff7777777007777777ffff077777777777777777777770ffffffff0777777777707770ffffffff0777
07777777ffffffffffff07777770ffffffff07777770ffff7777777007777777ffff077777777777777777777770ffffffff0777777777707770ffffffff0777
07777777ffffffffffff07777770ffffffff07777770ffff77777770f0777777ffff077777777777777777777770fffffffff0777777770f7770ffffffff0777
07777777ffffffffff000777777000ffffff07777770ffff77777770ff077777ffff077777777777777777777770ffffffffff07777770ff7770ffffffff0777
07777777ff0f0ffff07770777707770fffff07777770ffff77777770f0777777ffff077777777777777777777770fffffffff0777777770f7770ffffffff0777
07777777f07070ff0777777777777770fffff077770fffff7777777007777777fffff0777077777777777707770fffffffff077777777770770ffffffffff077
07777777ff070fff0777777777777770ffffff0770ffffff7777777007777777ffffff00077777777777777000ffffffffff07777777777000ffffffffffff00
07777777ffffff00077777777777777000ffffff70ffffff7777777007777777ffffff070777777777777770ffffffffffffffff777777707777777777777777
07777777fffff0777077777777777707770fffff770fffff7777777007777777fffff0770777777777777770ffffffffffffffff777777707777777777777777
07777777ffff077777777777777777777770ffff7770ffff7777777007777777ffff0777f07770777707770fffffffffffffffff777777707777777777777777
07777777ffff077777777777777777777770ffff7770ffff77777770f0077777ffff0777ff000777777000ffffffffffffffffff7777700f7777777777777777
07777777ffff077777777777777777777770ffff7770ffff7777777007777777ffff0777ffff07777770ffffffffffffffffffff777777707777777007777777
07777777ffff077777777777777777777770ffff7770ffff7777777007777777ffff0777ffff07777770ffffffffff0f0fffffff777777707777777007777777
f0777777ffff077777777777777777777770ffff7770ffff7777770f07777777ffff0777fffff077770ffffffffff07070ffffff777777707777777007777777
ff077777ffff077777777777777777777770ffff7770ffff777770ff07777777ffff0777ffffff0000ffffffffffff070fffffff777777707777777007777777
7777777000ffffff777000ffff000777ffffff000777777777777777077777777777777777777777777777777777777777777777777777707770777007770777
77777770770fffff7770770ff0770777fffff0770777777777777777077777777777777777777777777777777777777777777777777777707770777007770777
777777707770ffff7770777007770777ffff07770777777777777777077777777777777777777777777777777777777777777777777777707770770ff0770777
777777707770ffff7770777007770777ffff0777077777777777777707777777777777777777777777777777777777777777777777777770777000ffff000777
777777777770ffff7777777007777777ffff07777777777777777777077777777777777777777777777777777777777777777777777777707770ffffffff0777
777777777770ffff7777777007777777ffff07777777777777777777077777777777777777777770077777777777077777707777777777707770ffffffff0777
777777777770ffff7777777007777777ffff07777777777777777777f0777777777777777777770ff07777777770f077777077777777770f7770ffffffff0777
777777777770ffff7777777007777777ffff07777777777777777777ff00000000000000000000ffff000000000fff00000f0000000000ff7770ffffffff0777
676767676765f0f0f065f0f0f0f0f0f067676767e7f0f0f0f035f09677b7c70567676700000067676767000000676767f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
8787878787a6f0f0f06717f0f0f0f0f067676767e5f0f082f0f035f0f0f0f09667676767676767676767676767676767f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
f0f0f0f0f0f0f0f0f0f094f0f0f0f0f067678282828282f082828235f0f0f0f067676767676767676767676767676767f0f0f0f0f0f0158090f0f0f0f0f0f0f0
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
f0f0f0f0f0f0f0f0f0f0671464f0f0f0878787e5f0f0f0f0f0f0f0f035f0f0f0878787c787878797a7878787b787b787f0f0f0f0f0f0046765f0f0f0f0f0f0f0
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
f0f0f0f0f0f0828282f0f0f0651515f0671565f0f0f0f0f0f0f0f0f065f0f0f0f0f015f0f0b6c6f0f0f0f0f0f0f0b6c6f0f0f0f0f0f005676515f0f0f0f0f0f0
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
f0f0f0f0f0f0f0f0f0f0f0f0041414351414146470828282c2c2c2f0d5f0f0f014441414141414141414141414541414f0f0f0f0f00444141464f0f0f0f0f0f0
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
f0f0f0f0f0f0f0f0f0f0f0f0056767676767676764f070f0f0f0f0f0f035f0f000000000000000000000000000000000f0f0f0f0f00567676765f0f0f0f0f0f0
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
f0f0f0f08282c2c2c2c2f0f0f46767676767676765f002f0f0f0f0f0f065f0f067676767676767676767676767676767f0f0f0f0f07567151565f0f0f0f0f0f0
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
f0f015707070f0f0f0f0f0f0f567676767676767e4f0828cc2c28cf0f065f0f067676767676767676767676767676767f0f0f0f025056704541464f0f0f0f0f0
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
f0f004141464f0f08cf0f0f086676767676767e6e5f0f002f0f002f0f0d5f0f067676767676767676767676767676767f0f0f025670567056767d5f0f0f0f0f0
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
f0f005676765f0f002f0f0f037676767676767658282f0f0f0f0f0f0f065f0f067676767676767676767676767676767f0f0256767056705b6c66535f0f0f0f0
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
82828282828282707082828282828282676767657070f0f0f08c82828282828267676767676767676767676767676767f0f005156705042434646567352535f0
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
f0f0056767d6f08090f0f0f005676767676782821232f0f0f002f0f0f0d6f0f067676767676767676767676767676767f0041464257605676765656725676746
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
f0f076676766f00565f0f0f075676767707070e500f0f0f0f0f0f0f0f0a6f0f067676767676767676767676767676767f00567256705056767656525676767b4
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
f0f0966767d48c828282f0f005676767041464f000f0f0f0f08c828282f0f0f06767676767676767676767676767676714141414141414141414141414141414
14141477777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
f0f0f0056765020565f0f0f07667676706c2c2c2c2c2c28cf002f0a6f0f0f0f06767676767676767676767676767676700000000000000000000000000000000
00000077777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
0000000000000000000000000000000000000000000000000000000000000000ffffffff70000007777777777777777777777777777777777777777777777777
0000000000000000000000000000000000000000000000000000000000000000ffffffff70777707777777777777777777777777777777777777777777777777
0000000000000000000000000000000000000000000000000000000000000000ffffffff77000077777777777777777777777777777777777777777777777777
0000077700777007777700077707700770007777700007777700077777700000ffffffff77700777777777777777777777777777777777777777777777777777
00007777707770777777770777077007700777777770777777770777777770007000000777700777777777777777777777777777777777777777777777777777
00077777777770777007770777077007700777007770777007770777007770007077770777700777777777777777777777777777777777777777777777777777
00000777007770777007770777077007700777007770777007770777007770007700007777700777777777777777777777777777777777777777777777777777
00000777007770777777770777077007700777777770777777770777007770007770077777700777777777777777777777777777777777777777777777777777
0000077700777077777700077707700770077777777077777700077700777000ffffffff77777777777777777777777777777777777777777777777777777777
0000077700777077700000077707700770077700777077707777077700777000fff6ff6f77777777777777777777777777777777777777777777777777777777
0000077777777077700000077777777770077700777077700777077777777000ff66f66f77777777777777777777777777777777777777777777777777777777
0000077777777077700000077777777770077700777077700777077777777000f666666f77777777777777777777777777777777777777777777777777777777
0000007777700077700000007777777700077700777077700777077777700000f666666f77777777777777777777777777777777777777777777777777777777
0000000000000000000000000000000000000000000000000000000000000000ff66f66f77777777777777777777777777777777777777777777777777777777
0000000000000000000000000000000000000000000000000000000000000000fff6ff6f77777777777777777777777777777777777777777777777777777777
0000000000000000000000000000000000000000000000000000000000000000ffffffff77777777777777777777777777777777777777777777777777777777
00000000000000000000000000000000000000000000000000000000000000000000000000000000777777777777777777777777777777777777777777777777
00000000000000000000000000000000000000000000000000000000000000000000000000000000777777777777777777777777777777777777777777777777
00000000000000000000000000000000000000000000000000000000000000000000000000000000777777777777777777777777777777777777777777777777
00777777000077777000777077007700077777000777077007700077777000077777000777777000777777777777777777777777777777777777777777777777
00777777770777777770777077007700777777770777077007700777777770777777770777777770777777777777777777777777777777777777777777777777
00777007770777007770777077007700777007770777077007700777007770777007770777007770777777777777777777777777777777777777777777777777
00777007770777007770777077007700777007770777077007700777007770777007770777007770777777777777777777777777777777777777777777777777
00777007770777007770777077007700777007770777077007700777777770777777770777007770777777777777777777777777777777777777777777777777
00777007770777007770777077007700777007770777077007700777777770777777000777007770777777777777777777777777777777777777777777777777
00777007770777007770777077007700777007770777077007700777007770777077770777007770777777777777777777777777777777777777777777777777
77777770770777777770777777777700777007770777777777700777007770777007770777777770777777777777777777777777777777777777777777777777
07777707770777777770777777777700777007770777777777700777007770777007770777777770777777777777777777777777777777777777777777777777
00777077000077777000077777777000777007770077777777000777007770777007770777777000777777777777777777777777777777777777777777777777
00000000000000000000000000000000000000000000000000000000000000000000000000000000777777777777777777777777777777777777777777777777
00000000000000000000000000000000000000000000000000000000000000000000000000000000777777777777777777777777777777777777777777777777
00000000000000000000000000000000000000000000000000000000000000000000000000000000777777777777777777777777777777777777777777777777
__label__
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777700000000000000777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777077777777777777077777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777077777700777777077777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777000000077000000077777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777077777777777777077777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777070777077777777777777077777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777770707077077777777777777077777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777070777077777777777777077777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777000000777777777777777077777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777770777777777777777777777077777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777707777777777777777777777077777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777707777777777777777777777077777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777707777777777777777777777077777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777707777777777777777777777077777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777707777777777777777777777077777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777707777777777777777777777077777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777707777777777777777777777077777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777707777777777777777777777077777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777707777777777777777777777077777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777707777777777777777777777077777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777707777777777777777777777077777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777707777777777777777777777077070777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777707777777777777777777777070707077777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777707777777777777777777777077070777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777700000000077700000000000000000000000077777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777077777777707077777777777777777777777707777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777770777777777770777777777777777777777777770777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777770777777777777777777777777777777777777770777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777770777777777777777777777777777777777777770777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777770777777777777777777777777777777777777770777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777770777777777777777777777777777777777777770777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777770777777777777777777777777777777777777770777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777770777777777777777777777777777777777777770777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777770777777777777777777777777777777777777770777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777770777777777777777777777777777777777777770777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777770777777777777777777777777777777777777770777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777770777777777777777777777777777777777777770777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777770777777777777777777777777777777777777770777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777770777777777777777777777777777777777777770777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777770777777777777777777777777777777777777770777777777777777777777777777777777777777777777777
77777777777777777777777777777777000000000000000000000000000000000000000000000000000000000000000077777777777777777777777777777777
77777777777777777777777777777777000000000000000000000000000000000000000000000000000000000000000077777777777777777777777777777777
77777777777777777777777777777777000000000000000000000000000000000000000000000000000000000000000077777777777777777777777777777777
77777777777777777777777777777777000007770077700777770007770770077000777770000777770007777770000077777777777777777777777777777777
77777777777777777777777777777777000077777077707777777707770770077007777777707777777707777777700077777777777777777777777777777777
77777777777777777777777777777777000777777777707770077707770770077007770077707770077707770077700077777777777777777777777777777777
77777777777777777777777777777777000007770077707770077707770770077007770077707770077707770077700077777777777777777777777777777777
77777777777777777777777777777777000007770077707777777707770770077007777777707777777707770077700077777777777777777777777777777777
77777777777777777777777777777777000007770077707777770007770770077007777777707777770007770077700077777777777777777777777777777777
77777777777777777777777777777777000007770077707770000007770770077007770077707770777707770077700077777777777777777777777777777777
77777777777777777777777777777777000007777777707770000007777777777007770077707770077707777777700077777777777777777777777777777777
77777777777777777777777777777777000007777777707770000007777777777007770077707770077707777777700077777777777777777777777777777777
77777777777777777777777777777777000000777770007770000000777777770007770077707770077707777770000077777777777777777777777777777777
77777777777777777777777777777777000000000000000000000000000000000000000000000000000000000000000077777777777777777777777777777777
77777777777777777777777777777777000000000000000000000000000000000000000000000000000000000000000077777777777777777777777777777777
77777777777777777777777777777777000000000000000000000000000000000000000000000000000000000000000077777777777777777777777777777777
77777777777777777777777777777700777777770777777777777777077777777777777777777777777777707777777777777777777777777777777777777777
77777777777777777777777777777077777777770777777777777777077777777777777777777777777777707777777777777777777777777777777777777777
77777777777777777777777777770777777777770777777777777777077777777777777777777777777777707777777777777777777777777777777777777777
77777777777777777777777777770777777777770777777777777777077777777777777777777777777777077777777777777777777777777777777777777777
77777777777777777777777777000777777777770777777777777777077777777777777777777777777770777777777777777777777777777777777777777777
77777777777777777777777770777077777777770777777777777777077777777777777777777777777777077777777777777777777777777777777777777777
77777777777777777777777707777777777777770777777777777777077777777777777777777777777777707777777777777777777777777777777777777777
77777777777777777777777707777777777777770777777777777777077777777777777777777777777777707777777777777777777777777777777777777777
77777777777777777777770077777777777777770777777777777777077777777777777777777777777777700077777777777777777777777777777777777777
77777777777777777777707777777777777777770777777777777777077777777777777777777777777777707707777777777777777777777777777777777777
77777777777777777777077777777777777777770777777777777777077777777777777777777777777777707770777777777777777777777777777777777777
77777777777777777777077777777777777777770777777777777777077777777777777777777777777777707770777777777777777777777777777777777777
77777777777777777700077777777777777777770777777777777777077777777777777777777777777777707770007777777777777777777777777777777777
77777777777777777077707777777777777777770777777777777777077777777777770707777777777777707707770777777777777777777777777777777777
77777777777777770777777777777777777777770777777777777777077777777777707070777777777777707777777077777777777777777777777777777777
77777777777777770777777777777777777777770777777777777777077777777777770707777777777777707777777077777777777777777777777777777777
77777777777777770777777777777777777777770777777777000000000000777700000000000077777777707777777700777777777777000077777777777777
77777777777777770777777777777777777777770777777770777777777777077077777777777707777777707777777777077777777770777707777777777777
77777777777777770777777777777777777777770777777707777777777777700777777777777770777777707777777777707777777707777770777777777777
77777777777777770777777777777777777777770777777707777777777777777777777777777770777777707777777777707777777707777770777777777777
77777777777777770777777777777777777777770777777707777777777777777777777777777770777777707777777777700077770007777770007777777777
77777777777777770777777777070777777777770777777707777777777777777777777777777770777777707777777777077707707770777707770777777777
77777777777777770777777770707077777777770777777707777777777777777777777777777770777777707777777777777770077777777777777077777777
77777777777777770777777777070777777777770777777707777777777777777777777777777770777777707777777777777770077777777777777077777777
77777777770000000000000000000077777777000777777707777777777777777777777777777770777777707777777777777700777777777777777700777777
77777777707777777777777777777707777770770777777707777777777777777777777777777770777777707777777777777077777777777777777777077777
77777777077777777777777777777770777707770777777707777777777777777777777777777770777777707777777777770777777777777777777777707777
77777777077777777777777777777770777707777007777707777777777777777777777777777770777777707777777777770777777777777777777777707777
77777777077777777777777777777770770007770777777707777777777777777777777777777770777777707777777777000777777777777777777777707777
77777777077777777777777777777770707770770777777707777777777777777777777777777770777777707777777770777077777777777777777777707777
77777777077777777777777777777770077777770777777707777777777777777777777777777770777777707777777707777777777777777777777777707777
77777777077777777777777777777770077777770777777707777777777777777777777777777770777777707777777707777777777777777777777777707777
77777777077777777777777777777700777777770777777707777777777777777777777777777770777777707777770077777777777777777777777777707777
77777777077777777777777777777077777777770777777707777777777777777777777777777770777777707777707777777777777777777777777777707777
77777777077777777777777777770777777777770777777707777777777777777777777777777770777777707777077777777777777777777777777777707777
77777777077777777777777777770777777777770777777707777777777777777777777777777770777777707777077777777777777777777777777770077777
77777777077777777777777777000777777777770777777707777777777777777777777777777770777777707700077777777777777777777777777777707777
77777777077777777777777770777077777777770777777707777777777777777777777777777770777777707077707777777777777777777777777777707777
77777777077777777777777707777777777777770777777707777777777777777777777777777770777777700777777777777777777777777777777777707777
77777777077777777777777707777777777777770777777707777777777777777777777777777770777777700777777777777777777777777777777777707777
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777766676667667766677777676766676667676777777667667777776667676777776667666766676667676766676667766777776667666767776767777777
77777766676767676767777777676776777677676777777666667777776767676777776667676776777677676776776767677777776777676767776767777777
77777767676667676766777777676776777677666777777666667777776677666777776767666776777677666776776667666777776677666767776677777777
77777767676767676767777777666776777677676777777766677777776767776777776767676776777677676776776767776777776777676767776767777777
77777767676767666766677777666766677677676777777776777777776667666777776767676776777677676766676767667777776777676766676767777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777
77777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777

__gff__
0000000000000000111100000000000000000000000000000009090000000000111111110909000013000000150000001111111111110000000000000000000011111111111111000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000080808080808080000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
__map__
7676767676560f0f0f0f50767676767676764d0f0f0f0f0f0f0f0f0f0f4776767676767676767676766d0f0f0f0f67760f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f4f7654767676560f4f76764e0f0f0f0f4f767676767676765f787c794f76764e0f0f0f0f4f76
7676767676560f130f0f5076767676767676560f0f0f0f51110f0f0f0f5076767676767676767676764e0f130f0f57760f0f0f0f0f0f0f0f0f0f0f510f0f110f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f5f6f76767676560f5f797a5e0f130f0f5f767676767676764e150f0f5f6f6e5e0f130f0f5f6f
76767676764e0f21230f4f7676767676767676710f0f0f21230f0f0f747676767676767676767676765e0f21230f57760f0f0f0f0f0f0f0f0f0f0f402c2c2c2c2c2c2c310f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f4f765f767656510f0f0f0f0f21230f0f7f76767676766e5e31310f0f4f4e0f0f21230f0f7f
767676766e5e0f0f0f0f5f6f76767676767676720f0f0f0f0f0f0f0f737676767676767676767676560f0f0f0f0f777b0f0f0f0f0f0f0f0f0f0f0f5076767676765e0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f5f7878787676461f0f0f0f0f0f0f0f0f5f6f767676767e0f0f0f0f0f5f5e0f0f25250f0f73
76767676560f0f510f0f0f50767676767676764e0f0f6b6c3f0f0f0f4f7676767676767676767676560f0f6b6c0f0f500f0f0f0f0f0f0f0f6b6c0f50767676766a0f0f0f51150f0f0f0f0f0f0f0f0f0f0f0f0f0f0f510f51510f0f0f0f0f5f78787d1f0f0f0f0f0f6b6c0f74767676766e5e0f0f0f0f0f0f0f0f0f0f0f0f7476
767676764e0f212222230f4f7676767676766e5e0f0f212222230f0f5f6f76767676767676767676560f0f21222340410f0f0f0f0f0f0f0f404145467676766a0f0f40314631310f0f0f0f0f0f0f0f0f0f0f0f0f4041444141460f0f0f0f0f0f0f0f0f0f0f0f212222230f5c767676765d0f0f31310f0f0f0f0f0f31310f5c76
7676766e5e0f0f0f0f0f0f5f6f76767676767e0f0f0f0f0f0f0f0f0f0f7f7676767676787b797a787d0f0f0f0f0f67760f0f0f0f0f0f0f0f5776765676766a0f0f0f6776560f0f0f0f0f0f0f0f0f0f0f0f0f0f0f5076767676560f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f5f6f767676660f0f0f0f0f0f0f0f0f0f0f0f0f4876
7676767e0f0f0f0f0f6b6c0f7f7676767676720f0f0f3c0f6b6c0f0f0f73767676765d0f0f37250f150f510f0f0f57760f0f0f0f0f0f510f5076765d76560f0f0f5276375d6b6c0f0f0f0f0f0f0f0f0f0f0f0f0f50767676766a0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f7f7676765e0f0f0f37510f0f0f0f6b6c0f0f4876
7676764b0f2122222222230f487676767676560f0f2122222222230f0f50767676764e0f21222222222222230f0f4f760f0f0f0f0f0f40414146765d76560f0f52404141414146250f0f0f0f0f0f0f0f0f0f0f0f6976767676530f0f0f0f0f0f0f0f0f0f0f0f0f21230f0f0f5f787640710f0f0f212222222222222222235c76
7676764c0f0f0f0f0f0f0f0f5c767676767676710f0f0f0f0f0f0f0f747676767676710f0f0f0f0f0f0f0f0f0f7441760f0f0f0f52535776766d766a765d0f0f696776767676560f0f0f0f0f0f0f0f0f0f0f0f0f5276767676560f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f5f1d77720f0f0f0f0f0f0f0f0f0f0f0f0f7376
76766e5e0f0f0f0f0f0f0f0f5f6f767676766e5e0f0f0f0f0f0f0f0f5f6f767676764c0f0f0f3c0f6b6c0f0f0f5c76760f0f3751505276767656765376560f0f0f6076767676560f0f370f0f0f0f110f0f0f0f0f5076767676560f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f4f1d70710f0f0f0f0f0f0f0f0f3a0f747576
76764e0f0f0f212222230f0f0f4f7676767676710f0f0f21230f0f0f74767676766e5e0f0f0f212222230f0f0f5f6f760f0f40414544414676567656766d0f0f0f47766b6c4041444146250f522c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c2c764c0f0f20313131212222230f737676
7a785e0f0f0f0f0f0f0f0f0f0f5f78797676764c0f0f0f0f0f0f0f0f5c767676764e0f0f0f0f0f0f0f0f0f0f0f0f4f760f0f5776767676567656765676560f0f0f5076404650767676562c0f677676760f0f0f0f6076767676560f0f0f0f0f0f0f0f0f0f510f0f0f0f0f0f0f0f0f5f6f76720f0f0f0f0f0f0f0f0f0f74577676
0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f767676720f0f0f0f0f0f0f0f7376767676720f0f0f0f0f0f0f0f0f0f0f0f73766b6c50767676766d766a515676560f0f0f50520f6a5376767651460f507676760f0f6b6c47767676766d0f0f0f0f0f0f0f0f5241460f0f0f0f0f0f0f0f74414f7641710f0f0f6b6c0f0f0f0f73677676
0f0f0f0f0f0f0f21230f0f0f0f0f0f0f767676764171212222237441767676767676460f0f0f212222230f0f0f4076764146212222234041414141414141415352414141414141534041462c2c2c7640454141454676767676560f0f0f0f0f0f0f5276766d510f0f0f0f6b6c0f73765c76762c2c2c2c212222230f5150757676
0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f7676767676720f0f0f0f7376767676767676660f0f0f0f0f0f0f0f0f0f607676766d0f0f0f0f6776767676767676767676767676767676765376560f507676577676767676467676766d0f0f0f0f0f0f0f60767676460f0f0f0f40424341765c4676404141460f0f0f0f524141414141
4e0f0f0f0f4f767676767678787676767676767676560f0f0f0f4f76767676767676767676797a7d0f0f0f0f777676767676767c7c7878787b7d0f0f0f777c760f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f7676767878786a0f0f697d0f0f6776760f00000000000f0f0f0f00000000000f0f69767676560f0f0f0f000000000000
4b0f130f0f5f6f7676764e0f0f4f767676767676764e51130f0f5f797a7b6f76767676764e0f0f0f0f0f110f0f50767676764e0f150f0f0f0f0f0f110f0f0f4f0f0f150f0f0f0f0f0f0f0f0f0f110f0f76764e0f0f0f0f130f0f0f0f0f4f76760f0f0f0f52530f0e0f0f0f0f0f0f0f0f0f0f697676560f0f0f0f7f7676767676
4b0f21230f0f697c7b785e0f0f5f767676767676765e212222230f0f0f0f5776767676785e0f0f0f0f2122230f50767676765e0f0f0f0f0f0f0f0f200f0f0f480f0f2c2c0f0f0f0f0f0f0f0f2122230f766e5e0f0f0f0f21230f0f150f5f6f760f0f0f527676530f0f0f0f0f0f0f0f0f0f0f0f69787d0f0f0f0f5f797a787878
550f0f0f0f0f0f0f0f0f0f0f0f0f4f76767676764e0f0f0f0f0f0f0f150f4f6f76787b7d0f0f0f0f0f2525250f577676766d0f0f0f0f0f0f0f0f2525250f0f730f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f76560f0f0f0f0f25250f0f0f0f0f57760f0f5276767676530f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f
650f0f0f3a0f0f0f0f3f0f0f150f5f76767678785e0f3c0f0f0f0f0f0f0f5f6f764e0f0f0f0f150f0f0f510f0f777676767d0f0f0f0f0f0f0f0f0f513a0f74750f0f510f3c0f510f370f510f3c0f510f76661f0f0f0f0f0f0f0f0f0f0f0f50760f0f507676767676530f0f0f0f0f0f0f0f510f0f0f0f0f0f0f0f0f0f510f510f
4b4044462525252525250f0f310f0f4f76765e0f0f0f250f08090f0f0f0f0f7f764b0f0f0f31310f0f2122230f0f5776764b0f0f28280f170f252521230f5f6f0f08092525080925250809252508090f764d2c2c2c2c2c2c2c2c2c310f0f57760f5276767676404141460f0f0f0f0f0f5241444145460f2c2c2c0f4041444141
5550765d0f0f0f0f0f0f0f0f0f0f0f5c764e0f0f0f0f0f0f505d0f0f0f0f0f5c6e5e0f0f0f0f0f0f0f0f0f0f0f0f6776765b0f0f0f0f0f0f0f0f0f0f0f0f0f4f0f57560f0f696d0f0f506a0f0f50660f765d0f0f0f0f252525250f0f0f1d507652767676767650767656530f0f0f0f0f7676767676660f0f0f0f0f6776767676
65507656510f0f0f0f0f0f0f0f0f0f486e5e0f510f0f510f67560f0f3a510f5c560f6b6c370f0f0f0f0f0f0f0f0f577676710f0f0f0f0f0f0f0f0f0f0f0f0f480f506d0f0f616d3c0f5053370f504d0f76560f0f0f0f0f0f0f0f0f0f0f0f507676765253525350765156765352530f0f76767676764d0f0f0f0f0f5076767676
55504045460f0f0f0f0f0f31310f0f547e0f2122222222222222222222230f735671080931313131313131310f404545764c0f2c2c2c2c2c0f08091a1a0f0f730f08092525080925250809252508090f76660f282828282828282828280f67767676505276404145414146765376530f7676767676560f2828280f5776767676
65506776560f0f0f0f0f0f0f0f0f0f684c0f0f0f0f0f0f0f0f0f0f0f0f0f7476764b67560f0f0f0f0f250f0f0f577676765b0f0f0f0f0f0f0f505d0f0f0f0f570f57660f0f676a0f0f69560f0f67560f764d0f252525252525250f0f0f0f6776765253507650767676765676567676537676767676560f0f0f0f0f5076767676
72675076560f0f0f6b6c510f0f0f0f484b0f0f0f0f0f0f0f0f0f0f0f0f0f48767672505d0f0f0f0f0f250f510f4f767676710f0f0f0f0f0f5157563a0f0f74760f674d0f0f67530f0f52560f0f506d0f76560f0f0f0f0f0f0f0f0f0f0f0f50765276527676577651767666765676767676767676766d0f0f0f0f0f5076767676
56414141414243414141414145460f5c720f2c2c0f0f2c2c0f0f2c2c310f48767676465621222222222222230f5f7676764571313131222222222222230f73760f40444141414541414243414145460f76560f2c2c2c2c2c2c2c2c2c2c0f577676765076404144414141414676537676282828282828280f2c2c2c2c2c767676
41467676767676767676767676560f545d0f0f0f0f0f0f0f0f0f0f0f0f0f5f7676765d6a0f0f0f0f0f0f0f0f0f7476767676720f0f0f0f0f0f0f0f0f0f0f57760f50767676767676767676767676560f764e0f0f0f0f0f0f0f0f0f0f0f0f507676527676677676767676765d767653767676767676560f0f0f0f0f4f76767676
767671767676767676767676765607685d0f0f0f0f0f0f0f0f0f0f0f0f0f7376767676710f0f6b6c0f0f0f0f0f737676767676460f0f0f0f0f0f0f0f517476760f50767676766b6c765151767651560f6e5e0f0f510f07070707070707074f765276767650767676766b6c6d7676765376767676764e0f0f0f0f0f5f76767676
414441414145414243462122222340455d0f21232c2c0f0f2c2c0f0f0f7476767676764b0f0f212222230f0f747676767676767644710f212352454142434141462020204045414144414142434145417d0f2122230f21222222222222235f784141414141414621234041414141414176767676765e0f51c828282828282876
0000000000000000006d0f0f0f0f50765d0f0f0f0f07070707070707074076764141454144460f0f0f0f4044457676767676767676720f0f0f690000000000006d0f0f0f6700000000000000000000000f0f0f0f0f0f0f0f0f0f0f0f0f0f0f0f767676767676560f0f507640467676767676767676710f40460f0f0f50767676
__sfx__
0101000012050130501505016050170501a0501a05000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010100000805005050050500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010800000c050180000c040180000c030180000c0201800018050180001f000200000c050180000c05018000180000c0000c040000000c030000000c020000001805000000000000000000000000000000000000
010800000c043000000c7000c6000c6000000000000000000c6450000000000000000000000000000000000000000000000c700000000c04300000000000c6000c64500000000000000000000000000000000000
01010000240502305022050200501f0501c0501b0501905017050150501305012050100500e0500c0500b05000000000000000000000000000000000000000000000000000000000000000000000000000000000
010400002403032000300202e000181002e0003200034000320003100032000340000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010800000c6100c6000c6100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000400000c13014050100500c05000000000000000010000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010c0000183651d3551f35524355183551d3451f34524345183451d3351f33524335183351d3251f32524325183251d3151f31524315183151d3151f315243152830028300283002830028300283000000000000
011000000c043000000c0430c0000c6450000000000000000c600000000c043000000c6450c0430c600000000c0430c0000c0430c0000c6450000000000000000c600000000c043000000c6450c0430c60000000
011000000c0751300016000130001605513000110551300011055130000c000180000c000130000c0000c055110751300011075130000f055130000f055130000c05513000110001300011000130001100013000
011000002402300000240132401324000240002401300000240230000024013240130000024013000000000024023240132401324013240130000024013000002402300000240132401300000240130000000000
01100020182000f0001f3301300018320180001f3201f0001d33000000000000000000000000000c0002400029000000001d3302400018320240001d320000001f33000000000000000000000000000000000000
01100000182000f0001f3301300018320180001f3201f0001d33000000000000000000000000000c0002400029000000001d3302400018320240001b320000001833000000000000000000000000000000000000
01100018182351d2251f225182351d2251f225182351d2251f225182351d2251f225182351d2251f225182351d2251f225182351d2251f225182351d2251f225182001d2001f200182001d2001f2002200000000
01100018182351d2251f225182351d2251f225182351d2251f225182351d2251f225182351d2251f225182351d2251f225182251d2151f215182151d2151f215182001d2001f200182001d2001f2002200000000
001000182403000000000002903000000000002b030000003a0002e03000000000002d0300000000000290002a000290002903000000212001f200000000000000000000001f2000000000000000000000000000
011000182403000000000002b030000000000030030000003a0002e00000000000002d0000000000000290002a000290002900000000212001f200000000000000000000001f2000000000000000000000000000
011000182405000000000002b050000000000030050000003a0002e00000000000002d0000000000000290002a000290002900000000212001f200000000000000000000001f2000000000000000000000000000
010c0000245002e000181002e00032000340003200031000320003400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
000400002403032000300202e000181002e0003200034000320003100032000340000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
01100000182000f0002b3301340024320180002b3202b0002933000000000000000000000000000c0002400029000000002933024000243202400027320000002433000000000000000000000000000000000000
__music__
04 02034344
00 090a4b4c
00 090a4b4c
00 090a0b44
00 090a0b44
01 090a0b0c
02 090a0b0d
00 0e4f5013
00 0e4f5013
01 0e105013
02 0e115013
04 090a4315

