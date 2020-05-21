## PICOEMU

This is the repository for an implementation of a PICO-8 Emulator!

PICO-8 is a Fantasy Console created by [Zep](https://twitter.com/lexaloffle). You should [DEFINITELY check it out](https://www.lexaloffle.com/pico-8.php) before using this. PICOEMU in no way, shape or form replaces this wonderful little console, it's super awesome to use and it has a [great community](https://www.lexaloffle.com/bbs/?cat=7).
 
This emulator keeps the memory layout but without the CPU and memory limits so that we can expand on PICO-8 games as much as we want to. It is also possible to export games made with PICO-8 to other platforms such as Xbox, PS4 and Switch (given that you have the proper SDK for it).

If you find a bug, want something to change or would like to add a feature, you can create a bug of send me a message on [twitter](https://twitter.com/MatheusMortatti)! Feel free to create pull requests after the bug or feature is discussed!

### Usage

Refer to the [Wiki Page](https://github.com/mmortatti/pico8-emulator/wiki/Pico8-Emulator-General-Usage) for information about usage.


Flip = graphics.Flip;

			// Music
			Music = audio.Music;
			Sfx = audio.Sfx;

			Print = graphics.Print;

			// Math
			Max = Math.Max;
			Min = Math.Min;
			Mid = ((x, y, z) => Math.Max(Math.Min(Math.Max(x, y), z), Math.Min(x, y)));
			Floor = Math.Floor;
			Ceiling = Math.Ceiling;
			Cos = (x => Math.Cos(2 * x * Math.PI));
			Sin = (x => -Math.Sin(2 * x * Math.PI));
			Atan2 = ((dx, dy) => 1 - Math.Atan2(dy, dx) / (2 * Math.PI));
			Sqrt = Math.Sqrt;
			Abs = Math.Abs;
			Band = (x, y) => Util.FixedToFloat(Util.FloatToFixed(x) & Util.FloatToFixed(y));
			Bor = (x, y) => Util.FixedToFloat(Util.FloatToFixed(x) | Util.FloatToFixed(y));
			Bxor = (x, y) => Util.FixedToFloat(Util.FloatToFixed(x) ^ Util.FloatToFixed(y));
			Bnot = x => Util.FixedToFloat(~Util.FloatToFixed(x));
			Shl = (x, n) => Util.FixedToFloat(Util.FloatToFixed(x) << n);
			Shr = (x, n) => Util.FixedToFloat(Util.FloatToFixed(x) >> n);
			Lshr = (x, n) => Util.FixedToFloat((int) ((uint) Util.FloatToFixed(x)) >> n); // Does Not Work I think

			// Graphics
			Line = graphics.Line;
			Rect = graphics.Rect;
			Rectfill = graphics.Rectfill;
			Circ = graphics.Circ;
			Circfill = graphics.CircFill;
			Pset = graphics.Pset;
			Pget = graphics.Pget;
			Sset = graphics.Sset;
			Sget = graphics.Sget;
			Palt = graphics.Palt;
			Pal = graphics.Pal;
			Clip = graphics.Clip;
			Spr = graphics.Spr;
			Sspr = graphics.Sspr;
			Map = graphics.Map;
			Mget = memory.Mget;
			Mset = memory.Mset;
			Fillp = memory.Fillp;

			// Memory related
			Cls = memory.Cls;
			Peek = memory.Peek;
			Poke = memory.Poke;
			Peek2 = memory.Peek2;
			Poke2 = memory.Poke2;
			Peek4 = memory.Peek4;
			Poke4 = memory.Poke4;
			Fget = memory.Fget;
			Fset = memory.Fset;
			Camera = memory.Camera;
			Memcpy = memory.Memcpy;
			Memset = memory.Memset;
			Color = memory.Color;

			//
			// Now, fill the lua API properly.
			//

			// Graphics
			interpreter.AddFunction("line", Line);
			interpreter.AddFunction("rect", Rect);
			interpreter.AddFunction("rectfill", Rectfill);
			interpreter.AddFunction("circ", Circ);
			interpreter.AddFunction("circfill", Circfill);
			interpreter.AddFunction("pset", Pset);
			interpreter.AddFunction("pget", Pget);
			interpreter.AddFunction("sset", Sset);
			interpreter.AddFunction("sget", Sget);
			interpreter.AddFunction("palt", Palt);
			interpreter.AddFunction("pal", Pal);
			interpreter.AddFunction("clip", Clip);
			interpreter.AddFunction("spr", Spr);
			interpreter.AddFunction("sspr", Sspr);
			interpreter.AddFunction("map", Map);
			interpreter.AddFunction("mget", Mget);
			interpreter.AddFunction("mset", Mset);
			interpreter.AddFunction("fillp", Fillp);

			// Memory related
			interpreter.AddFunction("cls", Cls);
			interpreter.AddFunction("peek", Peek);
			interpreter.AddFunction("poke", Poke);
			interpreter.AddFunction("peek2", Peek2);
			interpreter.AddFunction("poke2", Poke2);
			interpreter.AddFunction("peek4", Peek4);
			interpreter.AddFunction("poke4", Poke4);
			interpreter.AddFunction("fget", Fget);
			interpreter.AddFunction("fset", Fset);
			interpreter.AddFunction("camera", Camera);
			interpreter.AddFunction("memcpy", Memcpy);
			interpreter.AddFunction("memset", Memset);
			interpreter.AddFunction("reload", (Func<int, int, int, string, object>) Reload);
			interpreter.AddFunction("cstore", (Func<int, int, int, string, object>) Cstore);
			interpreter.AddFunction("cartdata", (Func<string, object>) Cartdata);
			interpreter.AddFunction("dget", (Func<int, object>) Dget);
			interpreter.AddFunction("dset", (Func<int, double, object>) Dset);
			interpreter.AddFunction("color", Color);

			// Math
			interpreter.AddFunction("max", Max);
			interpreter.AddFunction("min", Min);
			interpreter.AddFunction("mid", Mid);
			interpreter.AddFunction("flr", Floor);
			interpreter.AddFunction("ceil", Ceiling);
			interpreter.AddFunction("cos", Cos);
			interpreter.AddFunction("sin", Sin);
			interpreter.AddFunction("atan2", Atan2);
			interpreter.AddFunction("sqrt", Sqrt);
			interpreter.AddFunction("abs", Abs);
			interpreter.AddFunction("rnd", (Func<double?, double>) Rnd);
			interpreter.AddFunction("srand", (Func<int, object>) Srand);
			interpreter.AddFunction("band", Band);
			interpreter.AddFunction("bor", Bor);
			interpreter.AddFunction("bxor", Bxor);
			interpreter.AddFunction("bnot", Bnot);
			interpreter.AddFunction("shl", Shl);
			interpreter.AddFunction("shr", Shr);
			interpreter.AddFunction("lshr", Lshr); // Does Not Work I think

			// Controls
			interpreter.AddFunction("btn", (Func<int?, int?, object>) Btn);
			interpreter.AddFunction("btnp", (Func<int?, int?, object>) Btnp);

			interpreter.AddFunction("flip", Flip);

			// Music
			interpreter.AddFunction("music", Music);
			interpreter.AddFunction("sfx", Sfx);

			// Misc
			interpreter.AddFunction("time", (Func<double>) Time);

			interpreter.AddFunction("print", Print);
			interpreter.AddFunction("printh", (Func<object, object>) Printh);

			interpreter.AddFunction("menuitem", (Func<int, string, object, object>) Menuitem);
			interpreter.AddFunction("import", (Func<string, bool, object>) Import);
			interpreter.AddFunction("export", (Func<string, object>) Export);

			interpreter.RunScript(@"
                function all(collection)
                   if (collection == nil) then return function() end end
                   local index = 0
                   local count = #collection
                   return function ()
                      index = index + 1
                      if index <= count
                      then
                         return collection[index]
                      end
                   end
                end

                function tostr(x)
                    if type(x) == ""number"" then return tostring(math.floor(x*10000)/10000) end
                    return tostring(x)
                end

                function tonum(x)
                    return tonumber(x)
                end

                function add(t,v)
                    if t == nil then return end
                    table.insert(t,v)
                end

                function del(t,v)
                    if t == nil then return end
                    for i = 0,#t do
                        if t[i] == v then
                            table.remove(t,i)
                            return
                        end
                    end
                end

                function foreach(t,f)
                    for e in all(t) do
                        f(e)
                    end
                end

                function string:split(sep)
                   local sep, fields = sep or "":"", {}
                   local pattern = string.format(""([^%s]+)"", sep)
                   self: gsub(pattern, function(c) fields[#fields+1] = c end)
                   return fields
                end

                function b(s)
                    local o = 0
                    local n = s:split('.')
                    for d in n[1]:gmatch('[01]') do
                        o = o*2 + tonumber(d)
                    end
                    if n[2] then
                        div = 2
                        for d in n[2]:gmatch('[01]') do
                            o = o + tonumber(d) / div
                            div = div * 2
                        end
                    end

                    return o
                end

                t = type
                function type(f)
                    if not f then
                        return 'nil'
                    end
                    return t(f)
                end

                cocreate = coroutine.create
                coresume = coroutine.resume
                costatus = coroutine.status
                yield = coroutine.yield
                sub = string.sub
                ");