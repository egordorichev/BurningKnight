pico-8 cartridge // http://www.pico-8.com
version 19
__lua__
function gk(qq,ow,ny)qq[ow]=qq[ow]or {}
add(qq[ow],ny)end
function lm(e,qv,...)local fn=e and e[qv]
if type(fn)=="function" then
return fn(e,...)end
return fn
end
function qw(o,na)for k,v in pairs(na or {})do
o[k]=v
end
return o
end
function ma(o)
return qw({},o)end
function hb(qq,na,go)
return function(rm)for i,p in pairs(na)do
rm[p]=hu(go,p)and r[rm[i]]or rm[i]
end
qq[rm.n_]=rm
end
end
function qp(y1,y2,...)rectfill(0,y1,127,y2 or y1,...)end
function fc(x1,y1,x2)rect(x1,y1,x2+1,y1+12,0)rectfill(x1+1,y1+1,x2,y1+11,5)spr(70,x1,y1,1,2)spr(71,x2-2,y1,1,2)end
function jn(t,x,y,c,a)
if a then x = x - a*4*#t end
for _,d in pairs(el)do
print(t,x+d.x,y+d.y,band(d.c,c))end
end
function ke(qz,pw)local f={}
for e in all(qz)do
if pw(e) then add(f,e) end
end
return f
end
function og(qz,ky)local m={}
for e in all(qz)do
add(m,ky(e))end
return m
end
function hu(qz,jx)for e in all(qz)do
if e==jx then return true end
end
end
function kn(a,b)local t={}
local kz=function(e)add(t,e)end
foreach(a,kz)foreach(b,kz)
return t
end
function nr(fn,a)
return fn
and fn(a[1],a[2],a[3],a[4],a[5])or a
end
function ob(qs,na)local jq,s,n,lr=
{},1,1,0
fh(qs,function(c,i)local sc,qh=sub(qs,s,s),i+1
if c=="(" then
lr = lr + 1
elseif c==")" then
lr = lr - 1
elseif lr==0 then
if c=="=" then
n,s=sub(qs,s,i-1),qh
elseif c=="," and s<i then
jq[n]=sc=='"'and sub(qs,s+1,i-2)or sub(qs,s+1,s+1)=="("and nr(pu[sc],ob(sub(qs,s+2,i-2)..","))or sc~="f"and band(sub(qs,s,i-1)+0,0xffff.fffe)s=qh
if type(n)=="number" then n = n + 1 end
elseif sc~='"'and c==" " or c=="\n" then
s=qh
end
end
end)
return qw(jq,na)end
function fh(qs,fn)local rs={}
for i=1,#qs do
add(rs,fn(sub(qs,i,i),i))end
return rs
end
hp="abcdefghijklmnopqrstuvwxyz0123456789 ().,=-+_/\"'?%\n"function unstash(nk)local s=""repeat
local i=peek(nk)s=s..sub(hp,i,i)
nk = nk + 1
until i==0
return s
end
ks={}
function ks:jt(rj)rj=ob(rj or "")rj.jg,rj.nw,ks[rj.fx or ""]=
self,{__index=rj},rj
return setmetatable(rj,{__index=self,__call=function(self,ob)ob=setmetatable(ma(ob),rj.nw)local ko,jl=rj
while ko do
if ko.ov and ko.ov~=jl then
jl=ko.ov
jl(ob)end
ko=ko.jg
end
return ob
end
})end
le={}
le.__index=le
function le:__add(b)
return v(self.x+b.x,self.y+b.y)end
function le:__sub(b)
return v(self.x-b.x,self.y-b.y)end
function le:__mul(m)
return v(self.x*m,self.y*m)end
function le:__div(d)
return v(self.x/d,self.y/d)end
function le:__unm()
return v(-self.x,-self.y)end
function le:ql(v2)
return self.x*v2.x+self.y*v2.y
end
function le:oy()
return self/self:rg()end
function le:oe()
return v(-self.y,self.x)end
function le:rg()
return sqrt(#self)end
function le:__len()
return self:ql(self)end
function v(x,y)
return setmetatable({x=x,y=y
},le)end
function qn(fq,l_)
return v(cos(l_),sin(l_))*fq
end
function qt(xl,yt,xr,yb)
return function(p)
return mid(xl,xr,p.x)==p.x
and mid(yt,yb,p.y)==p.y
end
end
pu={b=qt,v=v,br=qp,ra=jn,rf=fc,s=spr}
function cq()local a=0x5000
for p=0,15 do
for c=0,15 do
poke(a,bor(sget(p,c),c==3 and 0x80))
a = a + 1
end
end
end
function ec(no)memcpy(0x5f00,0x5000+shl(flr(no),4),16)end
el=ob([[
o(x=-1,y=-1,c=0),
o(x=0,y=-1,c=0),
o(x=1,y=-1,c=0),
o(x=-1,y=0,c=0),
o(x=1,y=0,c=0),
o(x=-1,y=1,c=0),
o(x=0,y=1,c=0),
o(x=1,y=1,c=0),
o(x=0,y=0,c=15),
]])function jd()hs,cu,by={},{},{}
mz,jh,pc=
rh(),lu(),ui()end
function mc(e)add(hs,e)for p in all(bj)do
if e[p] then gk(cu,p,e) end
end
for t in all(e.pd)do
gk(by,t,e)end
return e
end
function he(e)del(hs,e)for p in all(bj)do
if e[p] then del(cu[p],e) end
end
for t in all(e.pd)do
del(by[t],e)end
end
bj=ob([[
"kd","ev",
"hv",
"hn","hl",
]])function df()for _,qi in pairs(hs)do
local fn=qi[qi.mb]
if fn then
fn(qi,qi.t)end
if qi.oo then
he(qi)end
qi.t = qi.t + 1
end
end
km=ks:jt([[
mb="ox",t=0,
]])km.ov=mc
function km:lj(mb)self.mb,self.t=mb,0
end
cm=ob([[0.5,1,1,1,1,1,1,1,1,0,0,0,0,0,0,]])
function dh(ow)
local fl={}
for _,qi in pairs(cu[ow])do
gk(fl,qi.er,qi)
end
for o=1,15 do
if ow=="kd" and fl[o] then mz:nd(cm[o]) end
for _,qi in pairs(fl[o]or {})do
qi[ow](qi,qi.qk)
end
end
end
lu=km:jt([[
l=o(),r=o(),qk=v(0,0),
]])function lu:ov()poke(0x5f2d,1)end
function lu:ox()self.qk=v(stat(32),stat(33))bx(self.l,1)bx(self.r,6)end
function bx(rd,pz)local on,jf=
band(stat(34),pz)>0,rd.on
rd.on,rd.il,rd.hh=
on,on and not jf,jf and not on
end
function gi(e,mp,rp)
return e.lh((e.er>=10 and rp or mp)-e.qk
)end
ui=km:jt([[
qe=58,
er=15,
kj=o(),
gz=b(10,10,118,118),
]])function ui:jv()local kj=self.kj
self.oz=jh.qk+mz.qk
self:hx()if #kj==0 or hu(kj,self.ns)then
self:bf("l")end
self:bf("r")if jh.l.il and self.kj==kj then
self:cl()end
self.gh=self.ns
end
function ui:ev()spr(self.rheld and 9 or self.ns and self.ns.qe or self.qe,jh.qk.x,jh.qk.y)end
function ui:hx()self.io,self.ns={}
for e in all(cu.hv)do
if lm(e,"hv",self.oz,jh.qk)then
gk(self.io,e.er,e)self.ns=e
end
end
(self.gh or {}).mu=false
(self.ns or {}).mu=true
poke(0x5f80,self.ns
and self.ns.ft
or max(peek(0x5f80)-1,0))end
function ui:bf(bt)local b,ml=
jh[bt],bt.."held"if b.il then
local om=self:kr(bt.."down",self.oz,jh.qk)if om and type(om.jq)=="table" then
self[ml]=om.jq
end
end
local held=self[ml]
lm(held,"ha",self.oz,jh.qk)if b.hh and held then
local pp=self:kr("gy",held)lm(held,"ey",pp and pp.ks)self[ml]=nil
end
end
function ui:kr(qv,...)for ng=15,0,-1 do
for h in all(self.io[ng])do
local jq=lm(h,qv,...)if jq then
return {ks=h,jq=jq}
end
end
end
end
function ui:cl()og(self.kj,he)self.kj={}
end
lc=km:jt([[
er=11,
]])lc.hv=gi
function lc:ov()self.nf=nf(lm(self,"nf"))self.nb=self.nf.w+2
self.lh=qt(0,-1,self.nb,11)self.hy=self.hy and nf(self.hy)self.jj=self.jj and jj(self.jj)end
function lc:ox()if self.qd then
local d=self.qd-self.qk
self.qk = self.qk + (#d<=1 and d or d*0.4)
end
end
function lc:ev()if self.jj and self.mu then
self.jj:qa(127,1)end
end
function lc:kd(p)local qx,dn,c=
p.x+self.nb-1,p.y+9,self.mx
rect(p.x-1,p.y,qx+1,dn+1,1)rectfill(p.x,p.y,qx,dn,c and (self.mu and 12 or 13)or 5)rectfill(p.x,p.y,qx,p.y,c and 6 or 5)self.nf:qa(p+v(1,2))local c=self.hy
if c then
rectfill(qx+3,p.y+1,qx+c.w+4,dn,c.bg or 2)c:qa(p+v(self.nb+3,2))end
end
function lc:ldown()qo(self.mx and 60)lm(self,"mx")
return true
end
function nu(p,bs)if p.fd then
bs=kn({{nf={p.fd,c=13}}},bs)end
qo(p.eo or 62)local qk=p.ho or p.qk
local bp=qk+v(0,0)pc:cl()pc.kj=og(bs,function(b)b.qk,b.qd,b.er=
qk,bp,p.ch
bp = bp + p.fr
return lc(b)end)
if p.fy then pc.kj={} end
end
nf=ks:jt()function nf:ov()local w,s,fs,f,fw=0,0
self.fs=og(self,function(f)f,fw,fs=self:mr(f)local x=w
w = w + fw+fs
return {fn=f,d=v(x,0)}
end)self.w=w-fs
end
function nf:qa(p,a)
if a then p = p - v(a*self.w,0) end
for f in all(self.fs)do
f.fn(p+f.d)end
end
function nf:mr(f)local w,rl,qj,fn=6,0
if type(f)=="table" then
if f.qm then f={f.qm,5} end
f,w,rl,qj=f[1],f[2],f[3]or rl,f[4]
end
if type(f)=="string" then
local c=self.c or 6
return function(p)jn(f,p.x+2,p.y+1,c)end,#f*4+3,1
elseif f then
fn=function(p)ec(rl)spr(f,p.x,p.y)ec()end
end
return fn or function()end,w,qj or 2
end
pu.ld=nf.qa
jj=ks:jt()function jj:ov()self.ls=og(self,function(ln)
return nf(type(ln)=="string" and {ln}or ln)end)end
function jj:qa(od,a)local h=#self*8+2
local od = od - h*a
local dn=od+h
fillp(0xf0f)qp(od,dn,0x10)fillp()for l in all(self.ls)do
l:qa(v(64,od+2),0.5)
od = od + 8
end
end
gg=km:jt([[
er=14,
hv=1,
]])function gg:ldown()self.oo=lm(self,"mx")
return true
end
function qo(no)
if no and not h_ then sfx(no,3) end
end
function ie(oh,b)
return function(me)local v=rnd(1-abs(b))+max(b,0)
b = b + (0.5-v)*oh
return v
end
end
function mi(d,c,k_)if d>=0 then
d="+"..d
else
c=8
end
return d..(k_ or ""),c
end
function fj(e)
return abs(e.qk.x-mz.qk.x-64)<=76 and abs(e.qk.y-mz.qk.y-64)<=76
end
function lo(p,e)e.lo=max(0,e.lo-0.1)
return p+qn(e.lo,e.t*0.2)end
function ja(p,kv)local mm,mg=32767
for o in all(kv)do
local d=#(p-o.p)if p~=o.p and d<mm then
mg,mm=o,d
end
end
return mm,mg
end
bg=km:jt([[
er=1,
hv=1,
pb=o(0,0,0,72,73,74,75,88,89,90,91,88,91,89,91),
]])function bg:ov()local vs,vh,ou,m=
{},0,ie(0.1,0)for y=0,31 do
for x=0,31 do
vh=ou()*8-4+
(vh+(vs[x]or 1))*0.5
vs[x]=vh
mset(x,y,rnd()<0.004
and 122+rnd(5)or bg.pb[flr(vh+rnd(4))])end
end
end
function bg:kd()map(0,0,0,0,32,32)fillp(0xa5a5)for xy=3,262,32 do
rectfill(xy,0,xy,262,1)rectfill(0,xy,262,xy)end
fillp()rect(0,0,262,262,5)end
function bg:ldown(mp)
return (#(by.hj or {})>0 and ne or hj)()end
function bg:rdown(mp,rp)self.md=rp
return self
end
function bg:ha(mp,rp)mz.v=self.md-rp
self.md=rp
end
kk=km:jt([[
pd=o("kk","oi","prober"),
mb="unknown",
hc=1,ht=1,
er=6,
lh=b(-7,-7,7,7),
lo=0,
la=o(2,3,9,12),
ej=o(
o("unhappy",c=8),
o("content",c=3),
o("prosperous",c=9),
o("rich",c=12),
),
cw=o(12,36,24,25),
kg=o(lp=0),
fr=v(0,11),ch=9,
em=o(f,3,0,-1),
]])kk.hv=gi
function kk:ov()self.nq={}
if self.hq then
self:nj(gc[self.hq])end
end
function kk:hk()self.qb=
kk.iz()<0.45
and di.ki
or kk.nt()self.fd=self.qb.d
self:lj(self.qb.mb or "known")self:jz()end
function kk:ldown()if self.mb=="known" then
self:ek()
return true
elseif self.mb=="mn" then
return jm({qu=self})end
end
function kk:ek()nu(self,og(bm(self),function(b)local pj=b:pj()
return {nf=lm(b,"nf"),hy=pj>0 and {pj.."$",c=13},mx=function()if gn:mk(pj)then
self:hd(b)end
end,jj=b:jj()}
end
))end
function kk:ff()local bs={{nf=
self.n_ or
kk.ej[self.mh]
}}
bs[1].hy=self.lk
and {mi(self.lk).."$",bg=self.lk>=0 and 1}
local ir={"receives:"}
local ll=ke(self.kh,function(i)for j=1,i.fz do
add(ir,i)end
return i.fz==0
end)if ll[1]then
add(bs,{nf=kn({"needs:"},ll)})else
if self.mh<4 and #self.kh>0 and self.kg.qm~=r.w then
add(bs,{nf=kn({"wants more:"},self.rt
and {self.kh[#self.kh]}
or self.kh
)})end
end
add(bs,ir[2]and {nf=ir})if self.kg.lp>0 then
local lb=hu(kk.cw,self.kg.qm)and {"makes:",tostr(self.kg.lp),self.kg}
or {"exports:",#self.ca.."/"..self.kg.lp,self.kg}
add(bs,{nf=lb})end
for i=2,#bs do
add(bs[i].nf,{false,1})end
nu(self,bs)end
function kk:fi()
return self.hc and self.mb=="mn"end
function kk:hd(hq)local fm=2
if hq.ct then
self.oo=true
self=ks[hq.ct]({p=self.p,qk=self.qk,id=self.id
})elseif hq.fg then
self.qb=di[hq.fg]
else
self:lj("mn")self:nj(hq)self.fo=true
fm=pn("autoassemblers",3,2)end
gn:oc(fm)qo(hq.qo or 55)self:jz()end
function kk:nj(hq)self.hf,self.fd=
hq
self.kh=og(hq.i,ma)self.kg={qm=hq.o,lp=0}
end
function kk:kw(sw)add(self.nq,sw)
if sw:ea(self).fo then self.fo=true end
self:jz()end
function kk:jz()self.lo=1.25
end
function kk:ki(t)self.oo=t>60
end
function kk:gy(s)if self.mb=="mn" then
if s.qu==self then
self:ff()else
return true
end
end
end
function kk:cj(mo)local t=mo.t
if t.qm==r.e
and not self.ii then
for i in all(self.kh)do
if i.fz==0 and i.qm~=r["?"]then
del(self.kh,i)break
end
end
self.kh,self.ii=
kn({{qm=r.e}},self.kh),1
end
for i in all(self.kh)do
if i.qm==r["?"]and hu(self.ba,t.qm)then
i.qm=t.qm
end
if t.qm==i.qm and i.qm~=r["?"]then
mo:kw(t:kz(self))
return
end
end
end
function kk:ib()local ib=self.ca or {}
local qm,lp=
self.kg.qm,self.kg.lp
for w in all(self.nq)do
local mo={t=trade({mj={self},rq={w},qm=qm
}),lp=lp-#ib,kw=function(self,a)if not hu(ib,a)then
add(ib,a)
self.lp = self.lp - 1
end
end
}
if mo.lp==0 then break end
w:ea(self):cj(mo)end
return ib
end
function kk:hn()if self.lk and self.lk~=0 and fj(self)then
ic({qk=self.qk,mi(self.lk,10,"$")})end
end
function kk.eu(is,o,ku,m_,d
)
if not o then return end
local l,js,ni=
{},#is>=3 and 1 or 0,o==r.w and {25,23}or {o}
local ph=ku*#ni>=3
for i in all(is)do
add(l,{i.qm,3-js,i.fz~=0 and 0 or 7
})end
if #is>0 then
add(l,{false,d+js})add(l,(d==1 or ph)and {35,2+d}or {32,5})end
if ku>5 then
add(l,tostr(ku))add(l,{false,-4})l.c=13
ku=1
end
for qg in all(ni)do
for i=1,max(ku,1)do
add(l,{qg,6,(i<=ku and i>m_)and 0 or 7,ph and -4 or -3
})end
add(l,kk.em)end
return nf(l)end
function kk:hl()self.nf=kk.eu(self.kh,self.kg.qm,self.kg.lp,#self.ca,0
)end
function kk:kd(p)
if not fj(self) then return end
p=lo(p,self)if self.mb=="ki" and rnd(30)<self.t-30 then
return
end
circfill(p.x,p.y,7,0)ec(self.rl)palt(3,false)palt(0,true)spr(self.qb and self.qb.s or self.s or 43,p.x-4,p.y-4,2,2)ec()if self:fi()then
circ(p.x,p.y,6,kk.la[self.mh])spr(self.mh+1,p.x-3,p.y-6)end
lm(self.nf,"qa",p+v(0,7),0.5)if self.gj then
fillp(0xaaaa.8)circfill(p.x,p.y,sqrt(self.gj.eb)*128,2)fillp()self.gj=nil
end
end
function bq(self,mp,rp)self.qk,self.p=mp,mp/128
local o_,c=ja(self.p,by.oi)self.ez=
o_<self.eb and c or
ke(by.jm,function(sw)
return be(sw.qu.p,sw.ro.p,self.p,0.0032)end)[1]or
not ui.gz(rp)and {}
if self.ez then self.ez.gj=self end
end
function dd(self)if self.mb=="kf" then
local mp,t=jh.qk
if self.ez then
t=self.ez.qs
if not t then spr(31,mp.x-3,mp.y-8) end
elseif self.pj then
t=self.pj.."$"end
jn(t or "",mp.x,mp.y-8,8,0.5)end
end
function eg(self)if not self.ez
and gn:mk(self.pj)then
self:lj(self.dk or "ox")gn:oc(self.fm or 0)qo(self.ik or 54)
return true
else
qo(57)self.oo=true
end
end
function lq(self,t)
self.qk.y = self.qk.y + sin(t*0.005)*0.06
end
ms=km:jt([[
pd=o("ms"),
er=14,
qr=5,
]])function ms:ov()
if kx then kx.oo=true end
kx=self
end
function ms:ev()local t=self.t/self.qr-10
if t>=0 and lm(self,"kq")then
self.t,t=self.qr*10,0
end
local lt=-t^3/9
bi(self,lt)camera()if t>10.5 then
self.oo,kx=true
end
end
function bi(qc,lt)for e in all(qc)do
if lt then
camera(lt,0)lt=-lt
end
nr(pu[e.fn],e)end
end
trade=ks:jt()function trade:to()
return self.mj[#self.mj]
end
function trade:kz(oi,jm)
return trade({mj=kn(self.mj,{oi}),rq=kn(self.rq,{jm}),qm=self.qm
})end
function trade.nw.__eq(l,r)
return l.mj[1]==r.mj[1]
and l:to()==r:to()end
jm=km:jt([[
pd=o("jm"),
mb="kf",
er=2,
fm=f,ik=48,
eb=0.004,
jo=o(0,0,7,7,10,10,9,9,9,9,9,9,9,9,4,4,4,2,2,1),
mq=20,
]])function jm:ov()qo(50)self.ts,self.g_={},{}
end
function jm:ha(mp)self.ro=pc.ns.gy and pc.ns or {qk=mp,p=mp/128}
self.pj=
dj(self.qu,self.ro)self.mv=
self.qu.mv or self.ro.mv
self.ez=self:cg()if self.ez then
self.ez.gj=
self
end
self.er=
self.mv and 2 or 4
end
function jm:ey(ro)if not ro
or not eg(self)then
if stat(19)==50 then qo(-1) end
self.oo=true
return
end
self.mq,self.ro=1,ro
self.qu:kw(self)ro:kw(self)gn:oc(1)end
function jm:cg()local sp,dp=self.qu.p,self.ro.p
if #(sp-dp)>pn("space folding",0.5,1.08)then
return {qs="-too far-"}
end
for w in all(self.qu.nq)do
if w:ea(self.qu)==self.ro then return w end
end
local dm=kn(ke(by.oi,function(n)
return be(sp,dp,n.p,0.0032)end),ke(by.jm,function(sw)
return sw.mv==self.mv and
cn(sp,dp,sw.qu.p,sw.ro.p)end))
return dm[1]
end
function jm:ea(kk)
return kk==self.qu and self.ro or self.qu
end
function jm:ce(f,t)if rnd()<0.08 and self.g_[t.id]then
local d=t.qk-f.qk
local v=d:oy()add(self.ts,{qk=f.qk+v:oe(),v=v,l=d:rg()})end
end
function jm:kt(l,h,s,c)local qu,ro=
self.qu.qk+qn(1.5,self.t/126),self.ro.qk+qn(1.5,self.t/176)for dx=l,h,s do
for dy=l,h,s do
line(qu.x+dx,qu.y+dy,ro.x+dx,ro.y+dy,c)end
end
end
function jm:kd()if fj(self.qu)or fj(self.ro)then
local ju=
self.jo[self.mq]
if self.mq<20 then self.mq = self.mq + 1 end
local c,bc,tc=
(self.ez or self.gj)and 2 or self.mv and 0 or ju,self.mv and ju or 0,self.mv and 5 or 13
self:kt(-1,2,3,bc)self:kt(0,1,1,c)for _,t in pairs(self.ts)do
t.qk = t.qk + t.v
t.l = t.l - 1
if t.l<=0 then del(self.ts,t) end
local x,y=t.qk.x,t.qk.y
rectfill(x,y,x+1,y+1,0)pset(x,y,tc)end
self:ce(self.qu,self.ro)self:ce(self.ro,self.qu)end
self.gj=nil
end
function jm:ev()
if self.qu~=self.ro then dd(self) end
end
function gp(self)self.rr={}
end
function ep(self,n)while rnd()<n do
add(self.rr,self:lx())
n = n - 1
end
end
function ei(self)
if self.er<10 and not fj(self) then return end
local cx,cy,rc,ri,rk,rn,cs=
self.qk.x,self.qk.y,self.rc,self.ri,self.rk,self.rn,self.jo
for _,p in pairs(self.rr)do
local pv=p.v
p.p = p.p + pv
p.v.x=rc*pv.x+rk*pv.y
p.v.y=ri*pv.x+rn*pv.y
if p.a then p.v = p.v + p.a end
p.l = p.l - 0.14
if p.l<=1 then
del(self.rr,p)else
pset(cx+p.p.x,cy+p.p.y,cs[flr(p.l)])end
end
ep(self,self.fb)end
hj=km:jt([[
pd=o("hj","prober"),
fo=1,
dk="mn",
pj=0,fm=f,
er=2,
eb=0.025,
nx=0.06,jy=9,
pe=3,px=1,
fb=1,
jo=o(1,1,5,13,6),
rc=1.02,rk=-0.0525,
ri=0.0525,rn=1.02,
de=2,
]])hj.ha=bq
hj.ey=eg
hj.ev=dd
hj.ov=gp
hj.kd=ei
function hj:lx()local a=(flr(rnd(self.pe))+rnd(0.5))/self.pe+self.t*0.001
local p=qn(self.de+rnd(),a)local v=p*self.nx
return {p=p,v=self.px and v:oe()or v,l=self.jy
}
end
function fv(hm,p_,r)local nt,ol={},{}
for e in all(hm)do
for n=1,e.r*p_ do
add(nt,e)end
end
return function()local e
repeat
e=nt[flr(r()*#nt+1)]
until not hu(ol,e)del(nt,e)ol[3-#nt%3]=e
return e
end
end
function bu(n)for id=1,n do
local p
repeat
p=v(rnd(2.75)+0.125,rnd(2.75)+0.125
)until ja(p,by.kk)>0.05
kk({id="p"..id,p=p,qk=p*128})end
kk.nt,kk.iz=
fv(di,2,ie(0.5,dg.fa)),ie(0.8,0.5)end
ne=km:jt([[
pd=o("ne"),
mb="kf",eb=0,
dk="nn",
spawn_snd=52,ik=49,
pk=0,
pj=3,fm=1,
er=5,
kp=o(7,7,6,13,5,1,1,1,0),
iv=o(qs="-too far-"),
]])ne.ha=bq
ne.ev=dd
function ne:ov()qo(53)self.pi=ke(hs,function(p)
return p.fo
end)end
function ne:ha(...)bq(self,...)local o_
o_,self.ja=ja(self.p,self.pi)if o_>pn("space folding",0.19,0.27)then
self.ez=ne.iv
end
end
function ne:kf(t)self.pk=pn("space folding",29,35)*
(min(t/15,1)+sin(t/40)*0.1)end
function ne:ey()self.oo=
self.t<15 or not eg(self)end
function ne:nn(t)local sr=self:dz(t)/128+0.031
for pt in all(by.kk)do
if pt.mb=="unknown"and (pt.p-self.p):rg()<sr then
pt:hk()end
end
self.oo=t>25
end
function ne:kd(p)if self.mb=="kf" then
if not self.ez then
circ(p.x,p.y,self.pk,1)end
local cp=self.ja.qk
line(cp.x,cp.y,p.x,p.y,1)spr(7,p.x-4,p.y-4)else
for dt=0,8,2 do
if self.t>dt then
local sr=self:dz(self.t-dt)circ(p.x,p.y,sr,self.kp[flr(sr/self.pk*8)])end
end
end
end
function ne:dz(t)
return min(sqrt(t/15),1)*self.pk
end
ef=ob("\"\74\65\78\",\"\70\69\66\",\"\77\65\82\",\"\65\80\82\",\"\77\65\89\",\"\74\85\78\",\"\74\85\76\",\"\65\85\71\",\"\83\69\80\",\"\79\67\84\",\"\78\79\86\",\"\68\69\67\",\"\85\78\68\",\"\68\85\79\",\"\84\69\82\",")cb=ob([[
o(26,86,0,fn="br"),
o(51,59,1,fn="br"),
o(76,77,1,fn="br"),
o(24,25,1,fn="br"),
o(33,41,1,fn="br"),
o("",score=0,qk=v(64,21),c=5),
o(55,"planets:   ",
score=1,qk=v(64,34)),
o(19,"population:",
score=2,qk=v(64,43)),
o(24,"technology:",
score=3,qk=v(64,52)),
o(o(27,5),o(f,0),f,o(25,5),"happiness",
score=4,qk=v(64,61)),
o("total:","",
score=5,qk=v(64,74)),
o(score=6,qk=v(64,82)),
]])nz=km:jt([[
er=10,
dw=o(
o(42,43,1,fn="br"),
o("",64,40,6,0.5,fn="ra"),
),
gu=o(
o("last year!",64,48,9,0.5,fn="ra"),
o("2 years remain",64,48,2,0.5,fn="ra"),
f,f,
o("5 years remain",64,48,2,0.5,fn="ra"),
),
db=o(
ho=v(0,11),fr=v(0,11),ch=10,
),
hz=o(
o(126,127,1,fn="br"),
o(0,1,1,fn="br"),
o(8,118,34,fn="rf"),
o(0,-3,26,fn="rf"),
o(81,-3,117,fn="rf"),
),
np=o(
o(f,13,121,9,fn="ra"),
o(f,37,121,2,fn="ra"),
o(f,13,2,9,fn="ra"),
o(f,29,2,2,fn="ra"),
o(f,59,2,2,fn="ra"),
o(f,85,2,13,fn="ra"),
o(f,99,2,13,fn="ra"),
),
dv=1,
]])function nz:hl()local cf=mi(gn.fp)local np={gn.ok.."$",cf,gn.ix,"+"..gn.pa[r.k]+1,gn.happiness.."%",ef[gn.mt],gn.yr
}
for i,p in pairs(np)do
nz.np[i][1]=p
end
ee()end
function nz:kd()bi(kn(nz.hz,nz.np))if self.gv then
self.gv = self.gv + 0.2
local h=min(self.gv,3.2)^2
qp(0,h,0)qp(127-h,127)end
end
function nz:hn(yr)local iw,nv=
nz.dw,nz.gu[3426-yr]
iw[2][1],iw[3]=
"year "..yr,nv or nil
if nv then
local t=90
function iw.kq()
t = t - 1
return t>0
end
end
ms(iw)end
function nz:ox()if gn.gw and not self.gv then
gg()lc(ob([[
er=15,
qk=v(51,100),
nf=o(" again? "),
]],{mx=_init}))lc(ob([[
er=15,
qk=v(39,100),
nf=o(o(10,7)),
]],dr))self.cr,self.gv,self.er=
true,0,13
end
if self.cr and kx~=self.dv then
local s=lg()local iw=og(cb,function(st)if st.score then
st[3]=tostr(s[st.score])if st.score==0 then
st[1]=(gn.gw or "current score").." ("..dg.n_..")"end
if st.score==6 then
for i=1,5 do
st[i]=i<=s[6]and 56 or 57
end
end
return {nf(st),st.qk,0.5,fn="ld"}
else
return st
end
end)iw.qr=3
function iw.kq()
return self.cr
end
self.dv=ms(iw)end
end
dr={mx=function()jr.cr = not jr.cr
return true
end
}
rt=kk:jt([[
pd=o("rt","kk","oi","prober"),
hq=1,n_=o("laboratory",c=13),
mb="kf",dk="mn",
hc=f,rt=1,
pj=15,fm=2,
eb=0.025,
mh=1,
s=174,
ba=o(28,18,20,21,22,48),
]])rt.ha=bq
rt.mn=lq
rt.ev=dd
rt.ey=eg
iy=km:jt([[
lh=b(0,0,9,8),
er=12,
id=0,
qk=v(999,999),
eh=v(118,120),
]])iy.hv=gi
function iy:hl()if not self.kl and oa(self.km.q_)then
self.qk,self.kl=
iy.hi,true
iy.hi = iy.hi - v(10,0)
end
end
function iy:ldown(mp)
iy.id = iy.id + 1
return self.km({id="s"..iy.id
})end
function iy:kd(p)if self.kl then
palt(3,false)spr(self.km.s,p.x,p.y)if self.mu then
dd(self.km)self.jj:qa(0,0)end
palt(3,true)end
end
ic=km:jt([[
er=9,
l=0,v=v(0,-0.34),
]])function ic:ov()self.qk=self.qk or
jh.qk+mz.qk-v(0,5)end
function ic:ox()
self.qk = self.qk + self.v
self.l = self.l + 0.1
self.oo=self.l>=5
end
function ic:kd(p)ec(self.l)jn(self[1],p.x,p.y,self[2],0.5)ec()end
function db()local bs,qy,gb=
{},{},#gn.invented
local tl=gn.es
local rb=e_[tl-1]or 0
local bs=og(b_(true),function(t)
return {nf={t.qb,t.n_,c=6},hy={{24,5},tostr(ga(t))},jj=t.pr,mx=t.ex and function()gn:jw(t)end
}
end
)if gb>0 then
add(bs,{nf={30,gb.." invented",c=3},jj=gn.invented
})end
nz.db.fd=
"level "..tl.." ("..(gb-rb).."/"..(e_[tl]-rb)..")"nu(nz.db,bs)end
ia=ob([[
o(
qk=v(118,0),
nf=o(o(15,7)),
),o(
qk=v(0,0),
nf=o(o(37,7)),
),o(
qk=v(47,0),
nf=o(o(25,7)),
),o(
qk=v(0,117),
nf=o(o(8,7)),
),
]])ia[1].mx=function()gn.mt=1
gn:hn()end
ia[2].mx=db
ia[2].hl=function(self)self.nf=nf({#b_()>0 and 24 or 37
})end
ia[3].mx=function()jr.cr=true
gg(dr)end
gt=ob([[
o(nf=o(31,"restart     ")),
o(nf=o(42,"toggle music")),
o(nf=o(40,"toggle sfx  ")),
ho=v(0,107),fr=v(0,-11),
ch=14,
]])ia[4].mx=function()for i,fn in pairs({_init,function()music(-sgn(stat(20)))end,function()h_=not h_ end
})do
gt[i].mx=fn
end
nu(gt,gt)end
gs=rt:jt([[
pd=o("gs","kk","oi","prober"),
q_="ascension",
hq=2,rt=f,
n_=o("ascension oj",c=13),
pj=60,
s=166,
nx=0.12,jy=9,
pe=5,px=1,
fb=0.5,
jo=o(1,13,12,12,12,13,13,5,1,1,1),
de=7.5,
rc=0.98,rk=-0.07,
ri=0.07,rn=0.98,
]])gs.ov=gp
gs.lx=hj.lx
function gs:kd(p)if self.mh and self.mh>=2 then
ei(self)end
kk.kd(self,p)end
pq=gs:jt([[
fx="pq",
pd=o("kk","oi","prober"),
hq=3,n_=o("protostar",c=13),
mb="mn",s=98,
nx=0.06,jy=9,
pe=8,px=f,
nh=v(1,0),
de=2,
fb=1,
jo=o(1,2,4,9,10,7),
rc=1.02,rk=-0.0525,
ri=0.0525,rn=1.02,
]])function pq:ov()gd(ob([[
rc=0.97,rk=-0.1255,
ri=0.1255,rn=0.97,
jo=o(1,2,8,14,14,14,14,8,8,2,2,1,1),
i_=-0.009,nx=9,lw=1.5,
]],{qk=self.qk+self.nh}))end
mw=gs:jt([[
pd=o("kk","oi","prober"),
q_="void synthesis",
hq=5,s=66,n_=o("synthesizer",c=13),
pj=40,
gx=o(28,20,21,22),
nx=0.12,jy=9,
pe=5,px=1,
fb=0.3,
jo=o(1,1,5,5,3,3,3,5,5,1),
de=5,
rc=0.98,rk=-0.07,
ri=0.07,rn=0.98,
]])function mw:kw(sw)local cp=sw:ea(self)for i in all(cp.kh)do
if i.fz==0
and hu(self.gx,i.qm)then
self.kg.qm,self.gx=
i.qm,{}
end
end
kk.kw(self,sw)end
pm=rt:jt([[
pd=o("kk","oi","prober"),
q_="trade league",rt=f,pm=1,
hq=6,s=68,n_=o("trading hub",c=13),
pj=20,rl=15,
ba=o(28,48,18,20,21,22),
]])function pm:mn(t)lq(self,t)if self.mh>=2 then
self.rl=8+t%42/6
end
end
gd=kk:jt([[
fx="gd",
pd=o(),
hc=f,
ca=o(),
lh=b(0,0,-1,-1),
er=3,
fb=0,
rc=0.9,rk=0,
ri=0,rn=0.9,
jo=o(1,2,4,9,9,10,10,7,7,7,7,7),
i_=-0.06,nx=6,lw=3,
]])function gd:ov()gp(self)ep(self,140)mz.lo=3
end
function gd:lx()local re=rnd(1.5)+0.5
if rnd()<0.6 then re=flr(re/0.5)*0.5 end
local p=qn(re,rnd())
return {p=p,v=p*self.lw,a=p*self.i_,l=rnd(self.nx)+self.nx
}
end
function gd:kd(p)local pt
for i=1,3 do
pt=self.rr[i]
if pt then
circ(p.x,p.y,pt.p:rg(),self.jo[flr(pt.l/2)])end
end
ei(self)self.oo=not pt
end
py=rt:jt([[
pd=o("kk","oi","prober"),
hq=4,n_=o("processor",c=13),
rt=f,s=100,
pj=15,
ht=0.5,
]])oj=rt:jt([[
pd=o("oj","oi","prober"),
pj=10,fm=1,
lh=b(-4,-4,5,5),
s=170,
jb=o(170,170,170,168),
eb=0.01,
q_="slipgates",
]])function oj:gy(s)
return s.qu~=self and #self.nq<3
end
function oj:ldown()
return self:gy({})and jm({qu=self})end
function oj:hl()self.s=self.jb[#self.nq+1]
end
function oj:cj(mo)local t=mo.t
for w in all(self.nq)do
if mo.lp>0 and not hu(t.rq,w)then
local gl=ma(mo)gl.t=t:kz(self,w)w:ea(self):cj(gl)mo.lp=gl.lp
end
end
end
ge=oj:jt([[
mv=1,
q_="infraspace",
pj=25,
s=78,jb=o(78,78,78,76),
]])rh=km:jt([[
v=v(0,0),
lo=0,
p=v(192,192),
qk=v(192,192),
po=o(v(-1,0),v(1,0),v(0,-1),v(0,1)),
]])function rh:ox()local ih=v(0,0)for b=0,3 do
if btn(b)or btn(b,1)then
ih = ih + self.po[b+1]*3
end
end
self.p = self.p + self.v
self.v = self.v + (ih-self.v)*0.2
self.p.x=mid(-64,320,self.p.x)self.p.y=mid(-64,320,self.p.y)self.qk=lo(self.p,self)end
function rh:nd(fq)local cp=self.qk*(fq or 1)camera(cp.x,cp.y)end
gq=hj:jt([[
er=10,qk=v(64,28),
ho=v(34,56),
ch=11,
fr=v(0,11),
eo=-1,
fy=1,
fb=5,
jo=o(0,1,1,2,4,9,10,9,4,2,1,1,1),
rc=1.02,rk=-0.007,
ri=0.007,rn=1.02,
pe=5,de=15,
nx=0.0067,px=1,
jy=17,
lh=b(-40,78,40,98),
qc=o(
o(192,0,11,16,4,fn="s"),
o(172,20,107,2,2,fn="s"),
o("need help?",40,110,13,fn="ra"),
o("slipways.net/help",40,116,5,fn="ra"),
),
]])gq.hv=gi
function gq:ov()for i,d in pairs(bo)do
d.mx=ew
end
nu(self,bo)end
function gq:kd(p)ei(self)bi(self.qc)end
eq=ob([[
2,2,5,5,7,7,10,10,14,14,14,19,19,19,
]])function ee()local ps=#ke(by.kk,kk.fi)/2
for i,pl in pairs(eq)do
local nk=0x3101+4*pl
poke(nk,peek(nk)%128+(ps<i and 128 or 0))end
end
function _init()cq()jd()iy.hi,kx=
iy.eh
gq()music(0)end
function ew(nl)dg=nl
jd()bg()bu(90)gn,jr=iu(),nz()og(ia,lc)local hr={rt,py,oj,ge,pm,mw,gs
}
for i,b in pairs(hr)do
iy({km=b,jj=jj(du[i])})end
gn:jv()end
function _update60()df()pc:jv()end
function _draw()cls()ec()dh("kd")dh("ev")end
bo=ob([[
o(nf=o(49,"forgiving   "),n_="forgiving",
trade=7,fa=-0.99),
o(nf=o(50,"reasonable  "),n_="reasonable",
trade=6,fa=-0.6),
o(nf=o(51,"challenging "),n_="challenging",
trade=5,fa=-0.4),
o(nf=o(52,"tough       "),n_="tough",
trade=4,fa=-0.2),
]])iu=ks:jt()function iu:ov()qw(self,ob([[
ok=100,fp=0,
ix=0,es=1,
invented=o(),
mt=1,yr=3401,
]]))end
function iu:oc(hw)local ka=pn("time compression",12,15)
self.mt = self.mt + hw
self:jv()if self.mt>ka then
self.mt = self.mt - ka
self:hn()end
end
function iu:hn()
self.ok = self.ok + self.fp
self.ix = self.ix + self.pa[r.k]+1
self.yr = self.yr + 1
self:jv()for e in all(cu.hn)do
e:hn(self.yr)end
end
function iu:mk(kc)if self.ok<kc then
ic({"-no money-",8})
return
end
self.ok = self.ok - kc
if kc~=0 then
ic({mi(-kc,10,"$")})end
return true
end
function iu:jv()repeat
bb()until not bd()self.pa,self.fu=
bv(),ck()local ds=0
for p in all(by.kk)do
p.lk=flr(cv(p)-
co(p))
ds = ds + p.lk
end
self.fp=flr(ds*pn("skill implants",1,1.15))- cz()self.happiness=
bh()self.gw=
self.yr>=3426 and "final score"or self.fp<=0 and self.ok<3 and "bankrupt"for e in all(cu.hl)do
e:hl()end
end
function ci(je)local p={}
for _,qm in pairs(r)do
p[qm]=je
end
return p
end
function bv()local p=ci(0)for pt in all(by.kk)do
if pt.kh then
p[pt.kg.qm] = p[pt.kg.qm] + pt.kg.lp
end
end
return p
end
function ck()local d=ci(1)for l in all(by.rt)do
d[l.kh[#l.kh].qm] = d[l.kh[#l.kh].qm] - 0.15
end
return d
end
bn,bw=
6,0.0977
function dj(nm,to)local d_=
(#(to.p-nm.p)/bw)^0.75
return flr(bn*bk()*max(0.5,d_))end
function bk()
return max(1,sqrt(#(by.jm or {}))*0.27-0.1
)end
function bb()for pt in all(by.kk)do
pt.cc={}
end
for pt in all(by.kk)do
pt.ca=pt:ib()for e in all(pt.ca)do
for i,w in pairs(e.rq)do
w.g_[e.mj[i+1].id]=true
end
add(e:to().cc,e)end
end
end
function bd()local jc
for pt in all(by.kk)do
local jp=dl(pt)if not pt.mh or jp>pt.mh then
if pt.mh==1 and pt.hf.dc then
add(pt.kh,{qm=pt.hf.dc})end
pt.mh,jc=
jp,true
end
if pt.mh~=0 then
local pa=pt.hf:pa()pt.kg.lp=
pa[min(pt.mh,#pa)]+
(pt.ii or 0)end
end
return jc
end
function dl(pt)
if pt.mb~="mn" then return 0 end
local ji=1
for i in all(pt.kh)do
i.fz=
#ke(pt.cc,function(im)
return im.qm==i.qm
end)
ji = ji * i.fz
end
if ji==0 then return 1 end
if pt.pm then
return min(#ke(pt.kh,function(i)
return i.fz>0
end),3)end
if not pt.hc then
local li=pt.kh[#pt.kh]
return li and min(li.fz+1,4)or 2
end
return min(4,1+mid(#pt.ca,#pt.cc,1))end
fe=ob([[1,0,1,2,]])
function co(pt)
return pt.rt
and #by.rt
or pt:fi()
and fe[pt.mh]
or 0
end
function cz()
local lp=#ke(by.kk,kk.fi)
return flr(0.18*lp^2)
end
da=
ob([[-0.333,0,0.25,0.5,]])
function cv(pt)
local total=0
for e in all(pt.ca)do
local os=e:to()
if os.hc then
total = total + dg.trade*pt.ht*(1+da[pt.mh]+da[os.mh])
end
end
if pt.kg.qm==r.w then
total = total + pt.kg.lp*dg.trade*1.5
end
return total
end
bz=ob([[0,100,200,400,]])
function lg()
local dq=0
for pt in all(by.kk)do
if pt:fi()then
dq = dq + bz[pt.mh]
end
end
local fk=
gn.pa[r.p]*40
local en=
flr((#gn.invented*1.2)^2)*10
local total=flr((
dq+
fk+
en
)*0.01*gn.happiness)
return {
dq,
fk,
en,
gn.happiness.."%",
total,
min(flr(total/2000),5)
}
end
c_=ob([[-5,0,1,1,]])
function bh()
local h=100+2*(
gn.pa[r.h]+
gn.pa[r.w]
)
for pt in all(by.kk)do
if pt:fi()then
h = h + c_[pt.mh]
end
end
return h
end
r,ij=ob(unstash(8192)),ob(unstash(8259))di=ob([[
o("ki",45,0,mb="ki"),
o("e",160,4.5,"earth-like"),
o("f",128,6,"forgeworld"),
o("m",134,4,"mineral"),
o("o",130,3,"ocean"),
o("r",140,2.5,"remnant"),
o("x",162,3,"xeno"),
o("j",136,2,"jungle"),
o("i",138,2,"iceball"),
o("s",142,2,"barren"),
o("g",96,4,"gas giant"),
]])foreach(di,hb(di,ob([[
"n_","s","r","d",
]])))hg=ob(unstash(8383))et=ob(unstash(8494))function bm(kk)
return ke(gc,function(b)if oa(b.q_)then
local ly
fh(b.nc,function(k)ly=ly or k==kk.qb.n_
end)
return ly
end
end)end
hq=ks:jt([[
na=o("n_","nc","f_","jk","i","o","iq","dc"),
go=o("o","dc"),
gr=o(f,2),
]])function hq:ov()hb(gc,hq.na,hq.go)(self)if self.ip then
self.nf={self.n_,32,{self.ip,8}}
end
self.i=fh(self.i or "",function(c)
return {qm=r[c]}
end)end
function hq:nf()local l=
kk.eu(self.i,self.o,max(self:pa()[1],1),0,1
)add(l,hq.gr)add(l,self.n_)
return l
end
function hq:jj()
if self.ip then return end
local it
for i in all(self.i)do
it=(it and it.."," or "")..ij[tostr(i.qm)]
end
return {{it,13,ij[tostr(self.o)]}}
end
function hq:pa()local qf=hg[self.jk]
return self.iq and oa(self.iq)and hg[qf.mf]
or qf
end
function hq:pj()local pj=self.f_
if self.iq and oa(self.iq)then
pj = pj * 1.5
end
if not self.ip and oa("nanomaterials")then
pj = pj * 0.85
end
return flr(pj)end
gc,q_={}
for rm in all(et)do
if type(rm)=="string" then
q_=rm
else
rm.q_=q_
add(gc,hq(rm))end
end
du=ob(unstash(9807))lz=ob(unstash(10457))foreach(lz,hb(lz,ob([["n_","lv","qb","pj","pr",]]),
{"qb"}
))
e_=ob([[2,4,7,12,]])
function iu:jw(t)
self.ix = self.ix - ga(t)
add(self.invented,t.n_)
if self.es<4
and #self.invented>=e_[self.es]then
self.es = self.es + 1
end
self:jv()
end
function b_(bl)
return ke(lz,function(t)
t.ex=ga(t)<=gn.ix
return t.lv==gn.es
and not oa(t.n_)
and (t.ex
or bl)
end)
end
function oa(n_)
return not n_ or hu(gn.invented,n_)
end
function pn(n_,kb,mf)
return oa(n_)
and mf or kb
end
function ga(t)
return flr(t.pj*max(0.55,
gn.fu[t.qb]))
end
function cn(p1,p2,q1,q2)
local oq1,oq2,op1,op2=
ed(p1,p2,q1),ed(p1,p2,q2),
ed(q1,q2,p1),ed(q1,q2,p2)
return oq1 and oq2 and op1 and op2
and oq1~=oq2 and op1~=op2
end
function ed(a,b,c)
local j_=
(b.y-a.y)*(c.x-b.x)-
(b.x-a.x)*(c.y-b.y)
return j_~=0 and sgn(j_)
end
function be(a,b,c,gf)
local d=b-a
local my=d:ql(c-a)/#d
return my>0.01
and my<0.99
and #(a+d*my-c)<gf
end
__gfx__
00000000000000003000000300000003307700033007700330000033333333330000000333000033331133333305033333330303333333333333333333000333
111000211111111130488803077766033077000330700a03050d0503333003330ddd55033076760331551333330703333330e0e0333003333300033330666033
2211002122222222304882030dd555033066570330a00a0300c7c003330760330000000330777760157c50330007000333008880000090333006003306757d03
333110213333333330400003066d0d03306656030a0330a00d777d03dd0660dd0dd555030767776015cc5033076776d030aa08030999ff033066603306557d03
42211021444444443020113305511103306656030a03309000c7c00311500511000000033077776031551033307777600a44a033000090333006003306777d03
5511102155555555302033333333333330dd5d0309033090050d0503330650330dd55503330776033300050333077603094a9033333003333300033330ddd033
66d51085666666663333333333333333333333333333333330000033333003330000000333000003333330333307760330990333333333333333333333000333
776d108d777777763333333333333333333333333333333333333333333333333333333333333333333333333300000333003333333333333333333333333333
88221021a00490903033303333000333330003333300033330000033330003333300033330030033000000033333333330003333333333333333333333000333
942210219a0049000d0005030007000330ddd033307e20330076100330aaa0333077903307e07e03077777033333333307a00033300300333333303330888033
a942108509a0049000d050030f888f030d77b5030ae221030f7d1a030a4449030777a9030e88e80307ddd7033333d3d30a407a033080803333330b0308008203
bb3310854049002007776d03008820030d7bb5030e22e103047d12030a49a90309aa9903028882030777770333333d3330904403330803330030b00308080203
ccd51021240490000d6d6d0330dd50330dbb5033302e22030f6d1a03094aa903309990333028203307ddd7033333d3d33309403330202033070b003308800203
d5511021dddddddd06666d0330d050333055033333012003005510033099903330d55033330203330666660333333333330403333003003330b0033330222033
ee82108504909a000000000330000033330033333330003330000033330003333000003333303333000000033333333333000333333333333300333333000333
f9421085004909a03333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333
33333333300000333300333333333333330003333300033333000003330000033330033330001003330000330000000000000000001111100000000000000000
33300333050d0503307033330033333330dd003330ddd03330756d1330ab5d130007033304249403330a90330001110000000000011000110000000000000000
0000503300c7c003077000330d0333333000d0330d773d030757dd130bb5cd1307660633029aaa13330900330015551000000000110110011000000000000000
015555030d777d037777603305503333300500330d333d0306555113055cd5130666033304a7771330090333015dd50100000000101000001000000000000000
0000503300c7c003067000330503333333000333303330330657651306c515130556063319a777230aa90333015d550100000000100000001000000000000000
33300333050d0503306033330033333333050333305110330d56dd130cdbb1130005033304a77723099403330155500100000000100000001000000000000000
33333333300000333300333333333333330003333000003301111113011111133330033300112223300033330010001000000000110000011000000000000000
33333333333333333333333333333333333333333333333333333333333333333333333333333333333333330001110000000000011000110000000000000000
33000003333333333333333333333333311133331111133330000033330003333330333333303333003333330000000000000000001111100000000000000000
3049940333333333311133331111133330703333171a13330209020330adc0333307033333020333060333330000000000000000000000000000000000000000
044449033111333330a03333070a0333111113331100033300a7a0030ab5cd030077a00000221000076033330000000000000000000000000000000000000000
04ff490330a0333330003333000003330a0903330a090333097779030d5ccd037777aaa022221210077603330000000000000000000000000000000000000000
04ff4403300033333090333350905333000003330000033300a7a0030ccc5b0309aaa90301111103077760330000000000000000000000000000000000000000
0444403335553333300033333000333350905333509053330209020330ddb03307949a0302101103000700330000000000000000000000000000000000000000
00000333333333333555333335553333300033333000333330000033330003330a000a0301000103330060330000000000000000000000000000000000000000
33333333333333333333333333333333355533333555333333333333333333330033300300333003333000330000000000000000000000000000000000000000
00007000000000000000700000000000015555510000000033003333003333330000000000000000000000000000000000017100000000000001710000000000
007561d000000000007561d0000000001566dd55100000003055333355033333000000000000000000100000000000000001d100000000000001d10000000000
076511d500000000076511d500000000115555511000000005553333555033330000000000000000000000000000000001000001000000000100000100000000
0512221100000000051111110000000005111115000000000555333355503333000100000000000000000000000000006502220560000000650ccc0560000000
002eae2000000000001b7b10000000001607060d000000000555333355503333000000000000000000000000000000000502880500000000050c770500000000
0089798000000000013777310000000051777661100000000555333355503333000000000000000000000000010000000002880000000000000c770000000000
0089a98000000000013a7a31000000005d1111151000000005553333555033330000000000000000000000000000000005700075000000000570007500000000
008e7e8000000000003b7b300000000015dd55511000000005553333555033330000000000000000000000000000000000105050000000000010505000000000
070222050000000007055505000000001c55551b1000000005553333555033330000000001000000000000000000000000000000000000000000000000000000
076616d500000000076616d50000000001ef89a10000000005553333555033330000001000000000000000000010000000000000000000000000000000000000
00650d500000000000650d5000000000001111100000000005553333555033330000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000030553333550333330000000000000000000000100000001000000000000000000000000000000000
00000000000000000000000000000000000000000000000033003333003333330000000000000100100000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000033333333333333330000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000033333333333333330100000000000000000000000000100000000000000000000000000000000000
00000000000000000000000000000000000000000000000033333333333333330000000000000000000000000000000000000000000000000000000000000000
0001222101000000000001000000000000076d000000000031244403315abd03315ddd0331155503311222033015550330555503306660333000000330ddd033
0024f9e2101000000200020002000000007000d00000000014ffff0315ab5c031dcccc031577760312eee80305babd030577770306000d03221010030d000503
01499e44212000000042494240000000d707000d500000002ff77f035ab5cd035cc77c03177777031e8821031bddbb03577bbb0370ccc0509e210203d0d50050
02efffe4490000000029aaa920000000d711111d500000004f79f903555cdd03dc77cc03577676031228ee035abdba0357bbb60370c77050e4421403d0500050
02ef9ee7a0000000104a777a4010000050ddddd0500000004fffff035cc55d03dccccd0357776d03128e88035bdadd0357b6bb0370c77050fe449003d0000050
4249977f21000000529a777a9250000010000000100000004f9f99035cdbb5035cdcdd035776660328e822035dbd5d0357bb660306000503ee7a000305000503
09a7aa9421000000004a777a40000000003bb3300000000000000003000000030000000300000003000000030000000300000003305550330000000330555033
00224422100000000759aaa91d000000000311000000000033333333333333333333333333333333333333333333333333333333330003333333333333333333
000222210000000000725552600000000000000000000000301555033076d003366ddd03301d1003000000000100000010000000000000000000000030d0d503
00000000000000000100676001000000000000000000000005756d0307000d03d55555031000001300000101100110000000000000000000000001000c000503
0000000000000000000001000000000000000000000000001757dd03707000d3d111110350ccc053000100100011000110011000000110000100000050000003
00000000000000000000000000000000000000000000000056555103711111d36070600350c770530010010001100000001001001010001000011000000c0003
000000000000000000000000000000000000000000000000565765030ddddd037777660300c77003001000000100000000100000001000010111000050c77003
0000000000000000000000000000000000000000000000005d56dd0300bb5003d111110357000753000100000000000100010010000100000100000050070003
00000000000000000000000000000000000000000000000000000003000000030000000301050503010000000000000000000100000000000000001000000003
00000000000000000000000000000000000000000000000033333333333333333333333333333333100000000000010000000010000000000000000033333333
0015551000000000005ddd50000000000000000000000000001222100000000000155510000000000015551000000000005dd410000000000024441000000000
05756d51000000000dccccd500000000000000000000000002eee8210000000005bab3b10000000005777651000000000d565f410000000004ffff4100000000
1757dd15100000005cc77cc51000000000000000000000001e882110000000001b33bb3500000000177777650000000057d55f94000000002ff77f9400000000
565551d510000000dc77ccd51000000000000000000000001228ee82100000005ab3ba5510000000577676d510000000d655f944100000004f79f99410000000
5657655510000000dccccdc5100000000000000000000000128e8822100000005b3a33311000000057776dd510000000d5ff9472100000004fffff9210000000
5d56dd15100000005ccddc5510000000000000000000000028e822121000000053b3535110000000577666d51000000049994755100000004f9f994210000000
15d56551100000001ddcd551000000000000000000000000188282210000000013353511000000001566dd510000000014426d110000000014f9942100000000
01555511000000000155551000000000000000000000000001222210000000000155111000000000015555100000000001222510000000000144221000000000
00111110000000000001110000000000000000000000000000011100000000000001110000000000000111000000000000011100000000000001110000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
005abd500000000000533310000000000000000000000000000606d010000000050070050000000005007005000000000000000000000000000dd50000000000
03ab3cd500000000037777b1000000000242022000000000070d0d550100000000056100000000000005610000000000000000e8820000000007760000000000
5ab3cdd5100000005777bb3d0000000027f92ff20000000000c00050c0100000006000d000000000006000d000000000000000ea820000000d07760500000000
b33cddb310000000377bb6b3100000004f94ff9410000000d500000c001000000702220600000000070ccc060000000000000022220000006dd06055d0000000
dcc55d111000000037b6bb331000000049427422100000000000c000501000000702e80600000000070c77060000000000000067cd00000076001006d0000000
5cdbb5551000000037bbbd35100000004f97f94210000000d50c770d501000000702880600000000070c7706000000000000cd5c5dcd00000667776d00000000
1ddb3b31000000001bbd33510000000014f4422100000000d500700d501000000060006000000000006000600000000000000dc6c5d000000066666000000000
0151331000000000013351100000000001442100000000000500000d010000000d0ddd0d000000000d0ddd0d00000000000000c6c50000000600000d00000000
000111000000000000011100000000000001100000000000000d55000000000000001000000000000000100000000000000000cdc50000000066ddd000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000ea8060d08a82000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001220701d02222000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000008110c010d0000100000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000008821000000211100000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000008821900109221100000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000008221401104221100000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000002110200002110000000000000000000
33333333333333333333333333333333333333333333333333333333333499994923333333333333333333333333333333333333333333333333333333333333
333333333333333333333333333333333333333333333333333333334aa999949444423333333333333333333333333333333333333333333333333333333333
3333333333333333333333333333333333333333333333333333334aaa4000000024444233333333333333333333333333333333333333333333333333333333
33333333333333333333333333333333333333333333333333333aa4000000000000024423333333333333333333333333333333333333333333333333333333
3333333333333333333333333333333333333333333333333333aa00000000000000000442333333333333333333333333333333333333333333333333333333
33333333333333333333333333333333333333333333333333344000000000000000000044233333333333333333333333333333333333333333333333333333
33333333333333333333333333333333333333333333333333000000000000000000000002203333333333333333333333333333333333333333333333333333
33333333333333000000000000000000000033000000000000000000000000000000000000000000000000000000333000000300000000003333333333333333
3333333333333007777777766600777d000033077770077761777d00007750000000077500000777766d000777d03330776d000766ddddd00333333333333333
33333333333300776ddddddddd00776d0100330777d07776505666d0007750000000077d0000766ddd66d00666d033306ddd106dd55555550333333333333333
3333333333330776d0000000000776d01003300766d076650006666d057650000000076d000666d000d66d066dd003300ddd50dd500000000333333333333333
333333333330011000111111100766d0100330766d0666501110666d0d6650000000066d100ddd00110ddd00ddd503330ddd5011101111110333333333333333
333333333330777d0000000000666d01003330666d01110000001110066d500000000d6d5011000100011100ddd5003300ddd50dd60000000003333333333333
3333333333307666666666d000666d0100330011100775000000777507dd10007600056d50665010000076500ddd5000006dd500dd6666dddd00033333333333
3333333333300555555566650111001003330777607665000007665007d50007dd50006d507d500000007dd50ddd5177617dd5010ddddddddd51003333333333
3333333333301000000056650776501003330766d07666661676d5010655007dddd500d5507dd67716776dd500dddd16d51dd500100000000555100333333333
3333333333300111111011107665010033300766506ddd5505551011011106dd5ddd5011107ddddd16dddddd5005551dd5155000011111111011100033333333
3333333300000000000776507665000000006dd506ddd5000000011006656dd500ddd5dd10d55555055555dd5000000ddd50010000000000000d551033333333
333333300777777777766506ddd6777766506dd506dd50155055110006dd5550110d555510d5100000000055510155155d50010330ddddddddd5551033333333
333333007666d6dddddd5006dddddddddd506dd506dd50111011100006555101111055551055101110111055510011105551000330d555555555551033333333
3333330655555555555501d555555555555055550555500000000000055110110011011110111011101110111100000011110033301111111111110033333333
33333300000000000000110000000000000000000000000000000000000001100001100000000000000000000000000000000033300000000000001033333333
33333301000010111111101111515155555055550555503300000000055151000000111110111000000000111100333011110333301111110100001033333333
33333301000101111111001111111111111011110111103332000000011110000000011110111000000000111100333011110333301111101000010033333333
33333300000000000000000000000000000000000000003334400000000000000000000000000003333300000000333000000333300000000000000033333333
33333300000000000000000000000000000000000000003333440000000000000000000000000003333300000000333000000333300000000000000033333333
33333333333333333333333333333333333333333333333333344000000000000000000011133333333333333333333333333333333333333333333333333333
33333333333333333333333333333333333333333333333333332200000000000000000222333333333333333333333333333333333333333333333333333333
33333333333333333333333333333333333333333333333333333222000000000000012223333333333333333333333333333333333333333333333333333333
33333333333333333333333333333333333333333333333333333312221000000012222133333333333333333333333333333333333333333333333333333333
33333333333333333333333333333333333333333333333333333333122222222222213333333333333333333333333333333333333333333333333333333333
33333333333333333333333333333333333333333333333333333333333122222213333333333333333333333333333333333333333333333333333333333333
__label__
66666666615555555555555555501111111111111111111666666666111111111111111111111111105555555555555555555555555555555555516666666661
ddddddddd15500000555555555501111000001111111111ddddddddd11000000000000011111111110555555555555555500000000000000055551ddddddddd1
ddd000ddd15509990555555555500000020200000000001dd00d00dd1002220202020200005111044055000000000000550ddd0d0d0ddd0d055551ddd000ddd1
dd0ddd0dd15509090555555555500020020200000000001d07e07e0d100202020200020000011104f0550ddd0d0d0dd005000d0d0d0d0d0d000551dd06660dd1
d0d77dd0d15509090555555555500222022200000000001d0e88e80d100222022200200000011104f05500d00d0d0d0d0550dd0ddd0d0d0ddd0551d06757d0d1
d0ddddd0d15509090555555555500020000200000000001d0288820d100002000202000000010104405500d00d0d0d0d05000d000d0d0d0d0d0551d06557d0d1
dd0ddd0dd15509990555555555500000000200000000001dd02820dd10000200020202000100000000550dd000dd0d0d050ddd050d0ddd0ddd0551d06777d0d1
dd05110dd15500000555555555500000000000000000001ddd020ddd10000000000000001111000000550000500000000500000500000000000551dd0ddd0dd1
dd00000dd15555555555555555000000000000000000001dddd0dddd10000000000000011100000000055555555555555555555555555555555551ddd000ddd1
ddddddddd10000000000000000001010101010101010101ddddddddd10101010101011111000101010000000000000000000000000000000000001ddddddddd1
11111111110000000001000000000000000000000000001111111111100000000000111100000000000100000000000000000000000000000001011111111111
00000000000000000000000000000000000000000000000000000000000000000001d00000000000000000000000000000000000000000000000000000000000
00000000000000000001000000000000000000000000000000010000000000000111000000000000000100000000000000000000000000000101000000001000
00000000000000000000000000000000000000000000000000000000000000001111000000000000000000000000000000000000000000000000000000000000
00000000000000000001000000000000000000000000000000010000000000011100000000000000000100000000000000000000000000000001000000000000
00000000000000000000000000000000000000000000000000000000000000111000000000000000000000000000000000000000000000000000000000100000
0000000000000000000100000000000000000000000000000001000000001d010000000000000000000100000000000000000000000000000001000000000000
000000000000000000000000000000000000000000010000000000000001d0000000000000000000000000000000000000010000000000000000000000000010
00000000000000000001000000000000000000000000000000010000001100000000000000000000000100000000000000000000000000000001000000000000
00000000000000000000000000000000000000000000000000000000111100000000000000000000000000000000000000000000000000000000000000000000
000000000000000000010000000000000000000000000000000100011d0000000000000000000000000100000000000000000000000000000001000000001000
00000000000000000000000000000000000000000000000000000011100000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000001000000000000000000000000000000000111000000000000000000000000000100000000000000000000000000000001000000000000
00000000000000000000000000000000000000000000000000011110000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000001000000000000000000000077000000111100000000000000000000000000000100000000000000000000000000000001000000000000
00000000000000000000000000000000000000009077000001110000000000000000000000010000000000000000000000000000000000000000000000000010
00000000000000000001000000000000000000090066570900110000000000000000000000000000000100000000000000000000000000000001000010000000
00000000000000000000000000000000000000905066560090000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000001000000000000000009017066560109010000000000000000000000000000000100000000000000000000000000000001000000000000
000000000000000000000000000000000000090560dd5d0109000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000001000000000000000009056576555109010000000000000000000000000000000100000000000000000000000000000001000000000000
0000000000000000000000000000000000000905d56dd15109000000000000000000000000000000001000000000000000000000000000000000000000000000
00000000000000000001000000000000000009015d56551109000000000000000000000000000000000100000000000000000000000000000001000000000000
0000000000000000000000000000000111d010901555511090100000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000001111d0110010090111110900111100000000000000000000000000000100000000000000000000000000000001000000000000
00000000000000000000111111111100000000009000009000111111000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000011111111111100000000000000999990000000111111000000000000000000000000100000000000000000000000000000001000000000000
0000000001111111111110000000000000000000100000000000000000111d000000000000000000000000000000000000000000000000000000000000000000
00001111111d00110000000000000007e20d00050000000101040499401110000000000000000000000100000000000000000000000000000001000000000000
1111111d0100000000000000000000ae2200d05000d0001010404444900011111100000000000000000000000000000000000000001000000000000000000000
111110000000000000010000000000e22e07776d0055001010404ff4900000111111000000000000000100000000000000000000000000000001000000000000
00000010101010101010101010101002e20d6d6d0050101010404ff4401010000111110000101010101010101010101010101010101010101010101010101010
000000000000000000010000000000001206666d00000010104044440000000000011111d0000000000100000000000000000000000000000001000000000000
00000000000000000000000000000000000000000000000000000000000000000000011100100000000000000000000000000000000000000100000000000000
00000000000000000001000000000000000011000000000000010000000000000000000011111100000100000000000000000000000000000001000000000000
00000000000000000000000000000000000111000000000000000000000000000000000000111111000000000000000000000000000000000000000000000000
00000000000000000001000000000000000110000000000000010000000000000000000000000111110000000000000000000000000000000001000000000000
00000000000000000000000000000000001110000000000000000000000000000000000000000001111110000010000000000000000000000000000000000000
30000000000000000001000000000000001100000000000000010000000000000000000000000000011111100000000000000000000000000001000000000000
30000000000000000000000000000000011100000000000000000000000000000000000000000000000011111100000000000000000100000000000000000000
30000000000000000001000000000000011000000000000000010000000000000000000000000000000000111111000000000000000000000001000000000000
30000000000000000000000000000000001000000000000000000000000000000100000000000000000000000111110000000000000000000000000001000000
30110000000000000001000000000077000000000000000000010000000000000000000000000000000100000001111110000000000000000001000000000000
01111111100000000000000000009077000000000000000000000000000000000000000000000000000000000000011111100000000000000000000000000000
0001111111111100d001000000090066570900000000000000010000000000000000000000000000000100000000000011111d00000000770000000000000000
00000000111111110000000000903066560090000000000000000000001000000000001000000000000000000000000000111001000090770000000000000000
0000000000000111111111100905a066560109000000000000010000000000000000000000000000000100000000000000001111100900665709000000000011
000000000000000001111111090b30dd5d01090111111110000000000000000000000000000000000000000000000000000000011090d0665600900000011111
0ddd00000000000000000011090dcc55d11109011111111111111111111111111111111111000000000000000000000000000000090570665600090111111110
d77b500000000000000000000905cdbb55510900000d001111111111111111111111111111111111111111111111111111111000090d60dd5d01090111110000
d7bb500000000000000100000901ddb3b31009000000000000000000000000000d000000011111111111d0111111111111111111090d5ff94721090100000000
dbb500000000000000000000009015133100900000000000000000000000000000000000000000000000000000000000000d0111090499947551090000000000
05500000000000000001000000090011100900100d0000000001000000000000000000000000000000010000000000000000000009014426d110090000000000
00000000000000000000000001009000009001111000000000000000000000000000001000100000000000000000000000000000009012225100901000000000
00000000000000000001000011100999990000111110000000010000000000000000000000000000000100000000000000000000000900111009000000000000
000000000000000000000001000000000000000000001100dd000000000000000000000000000000000000000000000000000000000090000090000000000000
0000000000000000007a0000499400011000000000d0001100010000000000000000000000000000000100000000000000000000000009999900000000000000
000000000000000000a40704444900d0105050505111501111000000000000000000000000000000010000000000000000000000000000000000000000000000
000000000000000000090404ff49005500000000011100011111000000000000010000000000000000010000000000000000007007a000000100101010001000
000000000000000000109404ff440050d0010101011100000111111000000000000000000000000000000000000000000000f8880a407a00d000000001010000
00000000000000000110400444400000000101010101000000011111100000000000000000000000000100000000000000000882009044005500d0d0ddd51000
00000000000000011110000000000000110000000000000000000011111000000000000000000000001000000000000000000dd5010940005000101015151000
000000000000001111000000000000000110000000000000000100001111110d0000000000000000000100000000000000000d05010400000000505055551000
101010101010111110001010101010100110101010101010101010100011111000001010101010101010101010101010101d0000000000101010000000000010
10000000000011100001000000000000011000000000000000010000000001111100000000000000000100000000000000000111000000000001000000000000
10000000000111000000000000000000011100000000000000000000000000011111100000000000000000000000000000d01110000000000000000000000000
100000000d01100000010000000000000011000000000000000100000000000001111110d0000000000100000000000000001100000000000001000000000000
000000001001000000000000000000000d0100000000000000000000000000000000111100d000000000000000000000001d0000000000000000000000000000
00000001110000000001000000000000000100000000000000010000000000001000001111000000000100000000000000100000000000000001000000000000
00000011100000000000000000000000001100000000000000000000000000500501000011111100000000000770000001110000000000000010000000000000
d50000010000000000010000000000000011100000000000000100000000d0005500000000011111000100090770000011100000000000000001000000000000
76000000000000000000000000000000000110000000000000000001000000000000010100000111110000900665709001000000000000000000000000000000
7605000000000000000100000000000000011000000000000001000000060000006d000000000001111109030665600900000000000000000001000000000000
6055d0000000000000000000000000000001100000000000000000001100000000600000000000000110905a0665601090000000010000000000000000000000
1006d00000000000000100000000000000011100000000000001001000000000000d000000000000000090b30dd5d01090000000000000000001000000000000
776d0000000000000000000000000000000d0100000000000000000005000000000d000000000000000090dcc55d111090000000000000000000000000000000
666000000000000000010000000000000000010000000000000100000050006000000000000000000000905cdbb5551090000000000000000001000000000000
000d00011000000000000000000000000000110000000000000000000000dd6000050000000000000000901ddb3b310090100000000000100000000000000000
ddd00001111000000001000000000000000011000000000000010000000000000010000000000000011109015133100900000000000000000001000000000000
00000000111110000000000000000000000011100000000000000000000000000000000000000011111100900111009000000010000000000000001000000000
000000000011111000010000000000000000011000000000000100000000000001000000001111d0110000090000090000000000000000001001000000000000
00000000000001111000000000000000000001100000000000000000000000000000000111111100000000009999900000000000000000000000000001000000
0000007070779011111100000000000000000110000000000001000000010000000011111d000000000000000000000000000000010000000001000000000000
0d0007070777a90011111d00000000000000011100000000000000000000000011111d010000000d00050ddd0000000d0d07a000000000000000000000000000
0550090909aa99000011100d00000000000000110000000000010000000001111111100000000000d050d77b500d0005050a407a000000000001000000000000
050000909099900000001110000000000000000000000000000000000011111110000000000000077760d7bb5005501010109044000000100000000000000000
000000d0d0d5500000000011111000000000000000000000000000111111d000000000000000000d6d60dbb50005000001010940000000000001000000000000
00000000000000000000000011111000000007776600000000011111111000000000000000000006666d05500000000001010400000000000000000000000000
0000000000000000000100000011111000030dd5550300001111111d000000000000000000000000000000000000000000000000000000000001000000000000
000000000000000000000000000011111030066d0d00301111110000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000001000000000011030505511101030110d00000000000000000000000000000000100000100000000000000010000000001000000000000
00000000000000000000000000000000030dc77ccd51030000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000001000000000000030dccccdc51030000010000000000000000000000000000000100000000000000000000000000000001000000000000
000000000000000000100000000000000305ccddc551030000000000000000000000000000000000000000000000000000000000000000000000000000100000
000000000000000000010000000000000301ddcd5510030000010000000000000000000000000000000100000000000000000000000000000001000000000000
10101010101010101010101010101010103015555100301010101010101010101010101010101010101010101010101010100000000000001010101010101010
00000000000000000001000000000000000300111003000000010000000000000000000000000000000100000000000000000001555100000001000000000000
0000000000000000000000000000000000003000003000000000000000000000000000000000000000000000000000000000005bab3b10000000000000000000
000000000000000000010000000000000000033333000000000100000000000000000000000000000001000000000000000001b33bb350000001000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000005ab3ba551000000000000000000
0000000000000000000100000000000070000000001010ddd00100000000000000000000000000000001000000000000000005b3a33311000001000000000000
000000000000000000000000001000f888f00d0001010d77b500000000000000000000000000000000000000000000000000053b353511000000000000100000
000000000000000000010000000000088200055001010d7bb5010000000000000000000000000000000100000000000000000133535110000001000000000000
0000000000000000000000000000000dd500050001010dbb50000000000000000000000000000000000000000000000000000015511100000000000000000000
0000000000000000000100000000000d050000000010105500010000000000000000000000000000000100000000000000000000111000000001000000000000
00000000010000000000000000000000000000000000000000000000000000000000000015551000000000000000000000000000000000000000000000000000
000000000000000000010000000000000000000000000000000100000000000000000005756d5100000100000000000000000000000000000001000000000000
66666666610000000000000000000000000000000000000000000000000000000000001757dd1510000000000000000000000000000000000000000000000000
ddddddddd1000000000000000000000000000000000000000001000000000000000000565551d510000100000000000000000000000000000001000000000000
d0000000d15555555555555555555555550000000000000000000000000000000000005657655510000000000000000000000000000000000000000000000000
d0ddd550d15500000000000005555555555000000000000000010000000000000000005d56dd1510000100000000000000000000000000076d0000000dd50000
d0000000d155090909990999055555555550000002220222000000000000000000000015d56551100000000000000000000000000001007000d0000007760000
d0dd5550d155090900090990055555555550002000020202000100000000000000000001555511000001000000000000000000000000d707000d000d07760500
d0000000d155099900990099055555555550022200020222000000000000000000000000111110000000000000000000000000000000d711111d006dd0605500
d0dd5550d15500090009099905555555555000200002020200010000000000000000000000000000000100000000000000000000000050ddddd0007600100600
d0000000d15555090999009005555555555000000002022200000000000000000000000000000000000000000000000000000000000010000000000667776d00
ddddddddd155550000000000555555555550111111000000011111111111111111111111111111111111111111111111111111111111003bb330110066666011
11111111115555555555555555555555555011111111111111111111111111111111111111111111111111111111111111111111111100031100110600000d11

__gff__
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000200000000000001000100000000000000000000000000010001000001000000000100000000010101000000000000000000000001000000000000
0002000001000001010000000101010100000000010000000100000000010101000001000101000201000100000000000000010001010000010001000000000000000000000001000201000101000001000000000000010000010000000000010000000000000001000100000000000100010100000000010000000000000001
__map__
33062a1d23290f2a1d1c29022a1c2329102a1c2429072a1f2329142a1d1d290c2a1d1b29172a1c1d290b2a1d1f29312a1e2129012a1e1e29052a201f29082a1d202900331d232a2f060f0f042f291d1c2a2f0f12052f291c232a2f020f14132f291c242a2f10050f100c052f291f232a2f070f0f04132f291d1d2a2f14050308
2f291d1b2a2f0f1207010e0903132f291c1d2a2f0c15181512192f291d1f2a2f130309050e03052f29201f2a2f050e051207192f291d202a2f0a0f192f290033062b2a0f261b291c291d291e290d062a2f062c2f272933062c2a0f261b291d291e291f2729332b2a0f261c291d291e290d062a2f2c2f2729332c2a0f261d291e
291f27293312142a0f261b291c291e2920290d062a2f12142c2f27293312142c2a0f261b291d2920292327293300330f262f2f292f2f291b292f12142f292f10312f292f0b2f292f131510051208150d010e2501092f27290f262f2f292f2f291b292f062c2f292f102f292f082f27290f262f2f292f2f291b292f2b2f292f2f
292f052f27290f262f2f292f2f291b292f062b2f292f0c2f292f062f292f18050e0f060f0f04132f27290f262f2f292f2f291b292f062b2f292f2f292f312f27290f262f2f292f2f291b292f062b2f292f31312f292f172f2906292f312f27290f262f0e010e0f140503082f292f062f2923292f2b2f292f0f2f292f142f2729
0f262f020f14132f292f062f2923292f2c2f292f0f142f292f022f27290f262f070f0f04132f292f062f291c1b292f2c2f292f0f022f292f072f27290f262f070104070514132f292f062f291c1b292f2c2f292f0f142f292f072f27290f262f08091605170f120c042f292f052f2920292f2c2f292f062f292f102f2906292f
072f27290f262f030f0c0f0e192f292f0f2f291c1b292f2b2f292f0f062f292f102f292f07050e0525120517120914090e072f292f072f27290f262f030f0c0f0e192f292f182f291c1b292f2b2f292f14062f292f102f292f07050e0525120517120914090e072f292f072f27290f262f0d090e052f292f0d2f2920292f2b2f
292f102f292f0f2f27290f262f13141209102f292f122f2920292f2b2f292f10062f292f0f2f27290f262f0107120f170f120c042f292f052f2920292f2b2f292f022f292f062f292f18050e0f060f0f04132f292f0c2f27290f262f010c0701052f292f0f2f2923292f2b2f292f022f292f062f292f18050e0f060f0f04132f
292f0c2f27290f262f02120505042f292f0f2f2920292f2c2f292f102f292f0c2f27290f262f02120505042f292f182f2920292f2b2f292f022f292f0c2f27290f262f08150e142f292f0a2f2920292f2b2f292f102f292f0c2f27290f262f0409130d010e140c052f292f122f2920292f2b2f292f10062f292f142f27290f26
2f13030116050e07052f292f122f2920292f2b2f292f10062f292f022f27290f262f140f151209130d2f292f0a2f291c1b292f062b2f292f10072f292f172f27290f262f0c15181512192f292f052f291c1b292f062b2f292f14072f292f172f27292f18050e0f060f0f04132f290f262f0601120d2f292f180a2f291c1b292f
2b2f292f022f292f062f2906292f0c2f27292f07050e0513050504132f290f262f080102091401142f292f13092f291c1d292f2b2f292f0c062f292f102f2906292f072f27290f262f07050e050601120d2f292f13092f291c1d292f2b2f292f0c022f292f062f27292f0412090c0c020f14132f290f262f0518030116011405
2f292f0d2f291c1d292f2c2f292f022f292f0f2f27290f262f05180301160114052f292f09072f291c1d292f2b2f292f022f292f0f2f27292f07050e0525120517120914090e072f290f262f15100c0906142f292f062f291d1b292f2b2f292f0c2f292f102f27292f1205100c09030114090f0e2f290f262f0d050308010e09
1a052f292f0d12130f090a182f291d1b2909102a1e232906072a2f062f27292f02090f0d05250801030b090e072f290f262f1405121201060f120d2f292f1809130f0a2f291d1b2909102a1e242906072a2f052f27292f1314011202091214082f290f262f030f0c0c011013052f292f070d2f291e1b2909102a1f1c2903142a
2f10112f29110f2a1f2027292f07050f08011216051314090e072f290f262f080112160513142f292f05060d0f12180a0913072f292b1d1b2909102a1d1e2903142a2f07042f29110f2a1f21272900330f260f262f2b0c01022b2f29032a2427290f262f0d010b05132f291d1f292f130309050e0305251708050e2513151010
0c0905042f27290f262f170914082f291c24292f010e0425010e19251205130f151203052f27290f262f010404090e07250d0f1205250f062514080114251205130f151203052f27290f262f090e03120501130513250f15141015142f272927290f260f262f2b060f0f042510120f030513130f122b2f29032a2427290f262f
030f0e16051214132f291d1b292f090e140f2f291d23272927290f260f262f2b130c0910070114052b2f29032a2427290f262f030f0e0e0503141325151025140f251e25130c0910170119132f272927290f260f262f2b090e061201070114052b2f29032a2427290f262f030f0e0e0503141325151025140f251e25130c0910
170119132f27290f262f120f15140513251408120f15070825090e06120107011405132f27290f262f03010e2503120f1313250f1408051225130c0910170119132f272927290f260f262f2b14120104090e07250815022b2f29032a2427290f262f07050e0512011405132f291d20291d1e292f0f0e030525190f1525030f0e
0e0503142f27290f262f1d250f12251e25040906060512050e14251205130f15120305132f272927290f260f262f2b13190e14080513091a05122b2f29032a2427290f262f03010e2510120f160904052f291d23291d1b291d1c291d1d27290f262f140f250125100c010e05142514080114250e050504132509142f27292729
0f260f262f2b011303050e13090f0e25070114052b2f29032a2427290f262f010303051014132f291c24292f010e04251415120e13251408050d2f27290f262f090e140f2f291d20292f08011010090e0513132f2729272900330f262f131001030525060f0c04090e072f291c292f142f291f290f262f0c0f0e070512251201
0e07052f292f0f0e25130c09101701191325010e042510120f0205132f2727290f262f07050e0513050504132f291c292f0c2f2920290f260f262f1513052f291c1b1d291c1b20292f100c010e0514132f27290f262f140f25130514140c052f291c24292f010e04250601120d2f291d23272727290f262f0e010e0f0d011405
1209010c132f291c292f0f2f2921290f260f262f030f0c0f0e091a090e0725100c010e0514132f27290f262f0205030f0d0513251c203225030805011005122f272727290f262f0412090c0c020f14132f291c292f022f2922290f260f262f0215090c04250d090e0513251513090e072f291c2327290f262f0f0e2f291c1b21
291c1b20291c1c1b292f100c010e0514132f272727290f262f18050e0f060f0f04132f291d292f0c2f291c1b290f260f262f010c0c0f17250601120d090e072f291d23292f0f0e2f291c1b22291c1b2327290f262f090d10120f16052f291d23292f0f1514101514250f062f291c1b1e291c1b1f291c1c24272727290f262f13
0c091007011405132f291d292f142f291c1d290f260f262f0215090c042f291c1b24292f130c091007011405132f27290f262f140f25051814050e0425190f151225130c0910170119132f272727290f262f07050f08011216051314090e072f291d292f0f2f291c1f290f260f262f04051314120f1925050d10141925100c01
0e0514132f27290f262f060f1225010e25090e1314010e14251d1b2f291d1e292f020f0f13142f272727290f262f1412010405250c05010715052f291d292f142f291c21290f260f262f0215090c042f291c1d1b292f14120104090e07250815021325140f2f27290f262f1415120e251d2b1e25040906060512050e14251205
130f15120305132f27290f262f090e140f2f291d20292f08011010090e05131325010e042f291d1e292f0d0f0e05192f272727290f262f1205100c09030114090f0e2f291d292f022f291c23290f260f262f1415120e25010e19250f062f291c1b1d291c1b1f291c1b20291c1b21291c1b22291c1b2327290f262f090e140f2f
291c1c23292f060f120705170f120c04132f272727290f262f0115140f011313050d020c0512132f291e292f022f291c24290f260f262f030f0c0f0e091a090e0725100c010e0514132f27290f262f14010b0513251d250d0f0e14081325090e1314050104250f06251e2f272727290f262f130b090c0c25090d100c010e1413
2f291e292f022f291d1d290f260f262f0501120e251c2032250d0f12052f291d1e292f090e030f0d052f27292f06120f0d2514120104052f2727290f262f1314011202091214082f291e292f0f2f291d20290f260f262f030f0c0c011013052f291c1c1b291c1b21292f090e140f2513140112132f27290f262f140f250d010b
052f29201f292f140801142506090c0c1325010e19250e0505042f27290f262f010e0425090e031205011305132510120f04150314090f0e2f272727290f262f02090f0d05250801030b090e072f291e292f0c2f291d23290f260f262f1405121201060f120d2f291c1b1d291c1b1f291c1b20291c1b22291c1b2327290f262f
090e140f2f291c1b1e292f05011214082b0c090b0525170f120c04132f272727290f262f090e06120113100103052f291e292f142f291e1c290f260f262f0215090c042f291c1d1c292f090e06120107011405132f27290f262f14080114250c051425130c09101701191325100113132f27290f262f150e040512250f140805
1225130c0910170119132f272727290f262f131510051208150d010e2501092f291f292f142f291e1f290f262f0c010213250e0f172507050e05120114052f290f262f1d2e202e232f291d1f292f0201130504250f0e250c0516050c2f272727290f262f160f09042513190e1408051309132f291f292f0f2f291e23290f260f
262f0215090c042513190e14080513091a0512132f27290f262f14080114250312050114052f291d23291d1b291d1c291d1d272727290f262f07050e0525120517120914090e072f291f292f0c2f291f1d290f260f262f030f0c0f0e090513251909050c04250d0f12052f291c2427290f262f190f152503010e2515100c0906
142f291d1b291c1e291c2427290f262f1513090e072f291c1c23292f060f120705170f120c04132f272727290f262f14090d0525030f0d1012051313090f0e2f291f292f022f291f21290f260f262f01040413251e250d0f0e14081325140f25050103082f291c20292f190501122f272727290f262f011303050e13090f0e2f
291f292f012f29201b290f260f262f0215090c042f291c1d22292f011303050e13090f0e2507011405132f27290f262f140f251415120e2f291c24292f090e140f2f291d20292f08011010090e0513132f2727272900000000000000000000000000000000000000000000000000000000000000000000000000000000000000
__sfx__
0110001e1f7741f74518774187101a7741a7451f7741f74518711187441a7741a7451f7741f745187741d0161f7741f7451a7741a74518774187451a7741a710227111f77418774187451a7741a7451f7041f705
011000140c174000550c1740c1751f0341f0440c174180550c1000c1000c17400575131451f1350c174000750c1740c1750c1000c1000c1000c1000c1000c1000c1000c1000c1000c1000c1000c1000c1000c100
010800200c6150c6050c6150c605186150c6050c6150c605306150c6050c6150c605186150c6050c6150c6050c6150c6050c6150c605186150c6050c6150c605306150c6050c6150c605186150c6050c6150c615
010714200a7700a7700a7700a7700a7700a7700b7710b7700c7710c7700c7700c7700c7700c7700c7700c7700c7700c7700c7700c7700c7600c7600c7600c7600c7600c7600c7600c7600c7600c7600c7600c760
010c10201052511545187501875018750187501875018750187501875018750187501876018760187601876018770187701877018770187601876018760187601876018750187501875018750187501876018760
010716181851018510185201852018530185301854018540185501855018560185601855018550185401854018530185301852018520185101851018514185151850018500185001850018500185001850018500
01051f201853018530185301853018520185201851518530185301853018520185201851518530185301853018520185151853018530185201851518530185201851518520185101852518515185151851518500
0110000518735187101f71218735187152670518705187051f7051f70526705267051f7051f7052670526705267051f70518705187051a7051a7051f7051f7050070500705007050070500705007050070500705
0194000012b4412b350d52512b4412b3012b350dd4512b4412b450652512b4412b4012b450d52512b4412b450d52510b4410b4010b450b53510b4410b451052510b4410b4010b450b52510b4410b4010b4510525
018100201052517c2017c2514d3414d2014d2517d3417d2514c2014c2214c2017d3417d2514c2414c2014c251972417c2417c2514d3414d2014d1517d3417d2514c2014c2214c2517d3417d2514c2014c2014c25
019400201cc141cc101cc1519d3419d251cd341cd201cd1519c2419c2219c251cd341cd2519c2019c2019c25155051cc201cc2519d3419d2019d251cd341cd2519c2019c2219c251cd341cd2519c2419c2019c25
012000090a51511515185150a515115151a5150a515115151a5171a5051d5050e505055050e5050e505005051a5050a505115051a5051a5050a505115051a5050a505115051a5050a5051a5050a505115051a505
0180000010d2500a2016e2400a2016e2400a2000a2000a2016e2000a200fe2000a2016d1000a2016e2000a200fd2400a2016e2400a2016e2400a2000a2000a2016d2400a200fe2000a200ad2000a200fd2000a20
01ac00000cb440cb400cb400cb450cb440cb400cb400cb450cb440cb400cb400cb450cb440cb400cb400cb450cb440cb400cb400cb450cb440cb400cb400cb450cb440cb400cb400cb450cb440cb400cb400cb45
01ac0000107041cb241cb201cb201cb201cb25000001cb241cb201cb201cb201cb25000001cb241cb201cb201cb201cb2510d341cb241cb201cb201cb201cb25000001cb241cb201cb201cb201cb201cb2510d34
01ac000026b041ab0426b2426b2026b2026b2026b251ab0523b2423b2023b2023b2023b2524b0026b2426b2026b2026b2026b2524b0023b2423b2023b2023b2023b2524b002db242db202db202db202db202db25
01ac00001ab0410d442ab242ab202ab202ab202ab251cb00175241752017520175201752518b002ab242ab202ab202ab202ab2518b0026b2426b2026b2026b2026b2518b0021b2421b2021b2021b2021b2021b25
018000002082420810208242081020824208152082420810228242281022824228102282422815228242281020824208102082420810208242081520824208102782427810278242781022824228102282422810
01ac000013b4413b4013b400252513b4413b4013b4013b4513b4413b4013b400252513b4413b4013b4013b4513b4413b4013b4013b4513b4413b4013b400252513b4413b4013b4013b4513b4413b4013b4013b45
01ac0000105051ab241ab201ab201ac201ac2510d341ab141ab101ab101ac201ac2512d341cb141cb101cb101cc201cc2510d341cb241cb201cb201cb201cb2510d341ab241ab201ac201ac201ac201ac2510524
01ac000026b041252426b2426b2226b251ac201ac2510d4423b1417c2017c2017c2217c2217c251ac201ac201ac2526b1426b2026b2517c2017c2523b2423b2023b25125242db242db252ab242ab202ab222ab25
01ac0000105251cb241cb2221b2421b2510d341eb241eb201eb201eb201eb201eb2510d441fb241fb201fb201fb201fb2510d441eb241eb221eb201eb201eb2510d4421b2421b2021b2021b2021b2221b2510524
01ac000026b042db142db151ec201ec201ec221ec251ab052db242db252ab242ab202ab222ab251ac002fb242fb252ab242ab202ab222ab2517c052db242db251ec201ec201ec221ec252ab042db242db202db25
01ac000026b040e50226b1426b1226b101ac101ac151ab0523b1417c1017c1017c1217c1524b001ac101ac101ac1526b1426b1026b1517c1017c1523b1423b1023b15125142db142db152ab142ab102ab122ab15
01ac00000cb440cb4507d350cb440cb400cb450b5250cb440cb450b5250cb440cb400cb450b5250cb440cb40075250cb440cb400cb450e5250cb440cb45075250cb440cb400cb450e5250cb440cb400cb4507525
018000201182400a2024b1400a201482400a2000a2000a201182400a2024b1400a201681500a2000a2000a201182400a2024b1400a201681500a2000a2000a201182400a2024b1400a201681500a2000a2000a20
01ac000010b4410b450b52510b4410b4010b450d52510b4410b451052510b4410b4010b45105250eb440eb40095250eb440eb400eb45105250eb440eb45095250eb440eb400eb450e5250eb440eb400eb4509525
0180032010d2517c2017c250ad200ad200bd200bd200bd2016e2016e200bd200bd200bd2016d2016d200bd200bd200bd2016e2416e200bd200bd200bd2016d2016d200bd200bd200ad200ad200bd200bd2016d20
012000221d5151f515225152451526514295142951524514245102451224515295002e5142e5102e5152950029514295122951229515265142651026512265152b5142e5142e5102e512225142e5142e51529500
018000200db440db45085250db440db400db4508d45015250fb440fb45035250fb440fb400fb450ad450352511b4411b451151511b4411b4011b450cd450552514b4414b450852514b4414b4014b4214b3508525
01400322147141471014710197141b7141b710227142271022710227102271022712227151d71420714207102071020710257142771427710277102e7142e7102e7102e7102e7152c7142c7102c7150fe340fe35
0180032010d1517c1017c150ad100ad100bd100bd100bd100ad100ad100bd100bd100bd1016d1016d100bd100bd100bd100ad100ad100bd100bd100bd1016d1016d100bd100bd100ad100ad100bd100bd1016d10
01800020087250db4508e45010250de440102508d4501125030350fe35031240fb45030240fe350ae250353511b450502511e350502511e34050250cd45051240802514e350802514b4414b4514e3514e2508114
018000201491014910149101492516910169101691016925089100891008910089250a9100a9100a9100a9251491014910149101492516910169101691016925089100891008910089250a9100a9100a9100a925
0110001818a1518a1518a1518a1518a1518a1518a1518a1518a1518a1518a1518a1518a1518a1518a1518a1518a1518a1518a1518a1518a1518a1518a1518a1518a0018a0018a0018a0018a0018a0018a0018a00
01800000208242081022824225152282422810278241651529824298102c824275152782427810278241b51529824298102c8242251527824278102782416515208242081022824275152282422810278241b515
018000201391013910139101392516910169101691016925079100791009910099100a9100a9100a9100a92514910149101491014925139101391013910139250891008910089100892507910079100791007925
01800000228242251522824295152282422515278241651522824245151f824265152282422515278241651529824275152c824265152782422515278241651529824275152c8242951527824225152782416515
0180000e0f9100f91011910119101391013910159101591016910169100c9100c9100e9100e9100e9000e9050f9000f9000f9000f905000000000000000000000000000000000000000000000000000000000000
018000102281427814298143381426714227141d7141a7141f7141f710298142e814298142e814267142271429804275052c804265052780422505278041650529804275052c8042950527804225052780416505
0180000e109101091012910129101491014910169101691017910179100d9100d9100f9100f9100f9000f9050f9000f9000f9000f905000000000000000000000000000000000000000000000000000000000000
0180001023814288142a8143481427714237141e7141b71420714207102a8142f8142a8142f814277142371429804275052c804265052780422505278041650529804275052c8042950527804225052780416505
012000090b51512515195150b515125151b5150b515125151b5171b5051e5050f505065050f5050f505015051a5050a505115051a5051a5050a505115051a5050a505115051a5050a5051a5050a505115051a505
012000221ef3020f3023f3025f3027f302af302af3025f3025f3025f3025f3025f35237142371023712237151e7141e7101e7121e7151b7141b7101b7121b7152071423714237102371017f3023f3023f3023f25
0120040d2e5102e5122e5152e5050a51511515185150a515115151a5150a515115151a5171a5051d5050e505055050e5050e50500505000000000000000000000000000000000000000000000000000000000000
010500003f62039620306102424329610162431f6100f24300243035430224305543032430654304243075430524308543072430a543092330c5330a2330d5330b2230e5230c2230f5230d213105130e21311513
010500003f62039620306102424329610162431f6100f2431961009243136100524311610032430e610012430c61001240096100124006610012300461001230026100122002610012200061001210005143d521
0120172038b1438b1038b1038b153db143db103db123db1233b1433b1033b1033b1536b1436b1036b1036b1538f2436f2034f2033f2031f2036f2233f2523f2023f202af202af2231f2033f202af202af202af20
0103000022625001450d6250414500234004450024401425012340144402224024350224403425032340344504224044350424404234044250422404224044150421408405082040840508204084050820408405
010700003003030020300123001230714300203001230012300153002030012300143002030012307140000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010400060074400755007640074500754007650070400705007040070500704007050020400405002040040500204004050020400405002040040500204004050020400405002040040500204004050020400405
018000000f9100f92513910139250f9100f925139101392510910109251491014925109101092514910149250f9100f92513910139250f9100f92513910139251091010925149101492510910109251491014925
0140001510910109100601512910129100801514910149100a01516910169100b0150b9100b910010150d9100d910030150f9100f910040150d9000d9000b9000b90014900149001290012900000000000000000
010200002e5142e5152e5142e5152e5242e5252e5242e5252e5342e5352e5342e5352e5442e5452e5442e5452e5542e5452e5442e5452e5442e5452e5442e5452e5342e5352e5342e5252e5242e5252e5142e515
0104000019433014213062024620186100c6100003106021010110703102021080110303109021040110a031050210b011060210d011070310e021080110f03109021100110a0311270100501061010050100000
01040000013303062030610080000b041070310302102011010110002100031000430c5241173516524187351d5242273524524297352e5243c735355243a7353060008001050010300101300306003060008001
0180000029e1429f252be142bf2529e1429f252be142bf252ae142af252ce142cf252ae142af252ce142cf2529e1429f252be142bf2529e1429f252be142bf252ae142af252ce142cf252ae142af252ce142cf25
010800000734501345000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
014000201df201ff301df201ff301df201ff301df201ff3024f2026f3024f2024f3524f2026f3024f2024f351ef2020f301ef2020f301ef2020f301ef2020f3025f2027f3025f2025f3025f2027f3025f2025f30
0180000023f1018a153000518a153000518a153000518a153000518a153000518a153000518a153000518a153000518a153000518a153000518a153000518a153000518a153000518a153000518a153000518a15
0102000019045000001e0450000023045000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
014000200f9100f925030151391013925070150f9100f925030151391013925070150f9100f9100f9250301510910109250401514910149250801510910109250401514910149250801510910109101092504015
00020000016100d6111c61131611146110c61108611056110261501601016050c600116001a600006000060000600006000060000600006000060000600006000000000000000000000000000000000000000000
01010000006000c7000c600137001860018700306001f7003c6002b70518600187000c60013700006000c70000600006000060000600006000060000600006000060000600006000060000600006000060000600
__music__
00 0d0e0f44
01 0d0e1044
02 12131044
00 18151444
01 1a151644
02 08090a44
01 1d1b1e44
02 201b1e44
00 200c1e44
01 210c1144
02 21191144
00 21110b22
01 21230b22
00 21230b22
02 24250b22
00 26270b22
00 26270b22
00 26271c22
00 26271c22
02 21112c22
01 28292b22
00 33383a22
00 34292f22
00 3d383a22
00 28292f22
00 33383a22
00 34292b22
02 3d383a22
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
01 21110b22

