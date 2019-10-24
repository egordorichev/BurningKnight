namespace Pico8Emulator {
	public static class Api {
		public const string All = @"
function all(c)
 if c==nil then return function() end end
 local i =0
 local cn=#c

 return function()
  i=i+1
  if i<=cn then
   return c[i]
  end
 end
end

function tostr(x)
 if type(x)==""number"" then return tostring(flr(x*10000)/10000) end
 return tostring(x)
end

function tonum(x)
 return tonumber(x)
end

function add(t,v)
 if t~=nil then
  table.insert(t,v)
 end
end

function del(t,v)
 if t~=nil then
  for i=0,#t do
   if t[i]==v then
    table.remove(t,i)
    return
   end
  end
 end
end

function count(a)
 return #a
end

function foreach(t,f)
 for e in all(t) do
  f(e)
 end
end

local tp=type
function type(f)
 if not f then return ""nil"" end
 return tp(f)
end

cocreate=coroutine.create
coresume=coroutine.resume
costatus=coroutine.status
yield=coroutine.yield

sub=string.sub

⬅=0
➡️=1
⬆=2
⬇=3
❎=4
🅾️=5";
	}
}