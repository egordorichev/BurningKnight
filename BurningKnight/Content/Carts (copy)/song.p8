pico-8 cartridge // http://www.pico-8.com
version 19
__lua__
-- 07. not a creature was stirring
-- by kittenm4ster for advent 2018


-- global constants
fps = 60
keyboard_y = 95
key_count = 29
key_pos_type = {
  [0] = {22, 1},
  {25, 4},
  {28, 2},
  {31, 4},
  {34, 3},
  {40, 1},
  {43, 4},
  {46, 2},
  {49, 4},
  {52, 2},
  {55, 4},
  {58, 3},
}
for i = 12, key_count - 1 do
  local src = key_pos_type[i - 12]
  key_pos_type[i] = {
    src[1] + 42,
    src[2]
  }
end
key_sx = {
  110,
  116,
  122,
  105, -- black
}
top_platform_y = 14
snowman_start = {22, 22}
snowman_speed = .9
headdrop_x = 24
rope_gravity = 0.2
rope_tightness = .8
rope_damping = .50
curtain_min_y = -33
curtain_max_y = keyboard_y
title_fade_len = 6
title_fade_table = {
   [0] = {2,  2,  1,  1,  1},
   [3] = {2,  5,  5,  3,  3},
   [7] = {8, 14, 14, 14, 15},
   [9] = {2,  4,  4,  4,  9},
  [10] = {8,  8,  9,  9, 10},
  [11] = {8,  4,  5,  5, 11},
  [12] = {2, 13, 13, 13, 12},
  [15] = {8,  8, 14, 14, 15},
}
switcher = {y = 128}
switchers = {
  {18, 0, 101, 2, 2},
  {18, 15, 152},
  {18, 22, 152},
  {18, 27, 1, 1, 2, true},
}
stripe_fill = {0x1248, 0x8124, 0x4812, 0x2481}

function _init()
  channelnote = {}
  songpos = {prevticks = 0}
  notesubs = {}
  melodynotes = {}
  actors = {}
  coroutines = {}

  snowflakes = {}
  for i = 1, 256 do
    local s = {}
    init_snowflake(s)
    s.y = rnd_int(-32, keyboard_y)
    add(snowflakes, s)
  end

  rope = new_rope()
  lightsn = 0
  lightstimer = timer.new(fps / 8)

  snowman_body = add_actor({
    x = -9,
    y = 87,
    anims = {
      default = anim.new(1, 1,
        {95, 95, 79, 79, 95, 111, 111, 0, 0, 111, 95, 95, 95, 95}, 4, false)
    }
  })
  snowman_head = add_actor({
    x = -8,
    y = 83,
    anims = {
      default = anim.new(1, 1, {33}, 0, false),
      hop = anim.new(1, 1,
        {33, 16, 16, 16, 16, 16, 35, 49, 51, 127, 127, 51, 49, 35}, 4, false),
      slump = anim.new(1, 1, {33, 16}, 4, false),
    }
  })

  init_bell_ringers()
  init_bigringers()
  init_ballet()

  tubamouse = add_actor({
    x = -30,
    y = 85,
    targetx = 55,
    w = 12,
    anims = {
      default = anim.new(2, 2, {160}, 0, false),
      run = anim.new(1, 2, {128, 129, 130, 131, 132}, 4, true),
      stand = anim.new(1, 2, {128}, 0, false),
      play = anim.new(2, 2, {162, 164, 162, 160}, 3, false)
    },
    pal = {[2] = 7}
  })
  tuba = {
    visible = true,
    x = 60,
    y = 80,
    sx = 5,
    sy = 81,
    sw = 10,
    sh = 15,
    palt = {1, 3, 6, 7, 8, 14}
  }

  horsemouse1 = new_horsemouse(-30, 87)
  horsemouse2 = new_horsemouse(-30, 87)
  horse = add_actor({
    x = -48,
    y = 79,
    w = 34,
    anims = {
      default = anim.new(5, 2, {64, 96}, 999, true)
    }
  }, true)

  snowboarder = add_actor({
      x = -56,
      y = 86,
      anims = {
        default = anim.new(1, 2, {234, 235}, 4, true),
        fall = anim.new(2, 2, {170, 236}, 4, true)
      },
      pal = {[9] = 7, [6] = -1, [7] = -1, [13] = -1}
  })
  particles = {}

  cat = {pawx = 83, pawy = -16}
  curtainy = curtain_max_y
  titles = {}

  add_coroutine(run_snowman_head)
  add_coroutine(run_snowman_body)
  add_coroutine(run_cat)
  add_coroutine(run_fallingbell)
  add_coroutine(run_curtain)
  add_coroutine(run_tubamouse)
  add_coroutine(run_tuba)
  add_coroutine(run_switchers)

  add(horse.actions, run_horse)
  if stat(100) ~= 'back to calendar' then
    menuitem(5, 'load calendar', function()
      load('#pico8adventcalendar2018')
    end)
  end
end

function add_coroutine(f)
  add(coroutines, cocreate(f))
end

function follow_body(head, dropx)
  local body = snowman_body

  set_anim(head, 'hop')
  head.y = body.y - 4

  repeat
    if head.x < body.x then
      head.x += (snowman_speed * .5)
      head.x = min(head.x, body.x)
    end

    if dropx then
      head.x = min(head.x, dropx)
    end

    if body.anim:is_done() and (songpos.n + 1) % 4 == 0 then
      head.anim:reset()
    end

    yield()
  until dropx and head.x == dropx
end

function run_snowman_head()
  local head, body = snowman_head, snowman_body

  wait_until(snowman_start)

  follow_body(head, headdrop_x)

  head.maxy = 87
  head.vx = 0
  head.vy = 0

  tubabox = {x = tubamouse.x + 5, y = tubamouse.y + 2, w = 11, h = 2}
  head.w = 6
  head.h = 8

  local gravity = .015
  local airxfriction = .99
  local states = {
    fall = 0,
    wait_for_kick = 1,
    fly_to_tuba = 2,
    wait_for_tuba = 3,
    fly_to_body = 4,
  }
  local state = states.fall

  while true do
    if state == states.wait_for_kick and kicker.readytokick then
      kicker.anim = kicker.anims.kick

      if kicker.anim.i == 3 then
        state = states.fly_to_tuba
        head.vx = .58
        head.vy = -1.1
        head.anim = head.anims.default
      end
    end

    if state == states.fly_to_tuba then
      if overlap(head, tubabox) then
        head.y = tubabox.y - 8
        head.vy = 0
        head.vx = 0
        state = states.wait_for_tuba
      end
    end

    if state == states.wait_for_tuba and tubaisblowing then
      state = states.fly_to_body
      head.vx = .29
      head.vy = -.75
    end

    if state == states.fly_to_body and head.y >= body.y - 4 then
      break
    end

    head.vx *= airxfriction
    head.vy += gravity

    head.x += head.vx
    head.y += head.vy

    if state == states.wait_for_tuba then
      head.y = tubabox.y - 8
    end

    head.y = min(head.y, head.maxy)

    if state == states.fall and head.y == head.maxy then
      state = states.wait_for_kick
      head.anim = head.anims.slump
    end

    yield()
  end

  follow_body(head)
end

function run_snowman_body()
  wait_until(snowman_start)

  while snowman_body.x < 128 do
    if snowman_body.anim:get_index() == 0 then
      snowman_body.x += snowman_speed
    end

    if snowman_body.anim:is_done() and (songpos.n + 1) % 4 == 0 then
      snowman_body.anim:reset()
    end

    yield()
  end
end

function new_horsemouse(x, y)
  return add_actor({
    x = x,
    y = y,
    anims = {
      default = anim.new(1, 1, {152}, 0, false),
      run = anim.new(1, 1, {152, 153, 154, 155, 156}, 4, true)
    }
  })
end

function new_ballerina(x, y)
  return add_actor({
    x = x,
    y = y,
    pal = {[1] = 1},
    anims = {
      default = anim.new(1, 2, {1}, 0),
      pointe = anim.new(1, 2, {2}, 0),
      pointewalk = anim.new(1, 2, {3, 4}, 5, true),
      pointewalkmanual = anim.new(1, 2, {3, 4}, 999, true),
      slump = anim.new(1, 2, {5, 6, 7, 6}, 8),
      unslump = anim.new(1, 2, {5, 1}, 4),
      turn = anim.new(1, 2, {8, 9, 10, 11, 12, 13, 14, 15}, 4),
      leap = anim.new(2, 2,
        {32, 34, 36, 36, 38, 40}, 8, false,
        {[32] = {1}, [34] = {1}, [36] = {2, 1}},
        {[32] = 1, [34] = 2}),
      hop = anim.new(1, 2, {42, 42, 43, 44, 45, 45, 45, 44, 43}, 3),
      kick = anim.new(1, 2, {32, 71, 34, 34, 34, 34, 34, 34, 34, 71, 32}, 5),
    },
    set_flipx = function(self, flipx)
      if (not not self.flipx) ~= flipx then
        self.flipx = flipx
        self.x += (3 * (self.flipx and 1 or -1))
      end
    end
  },
  true)
end

function new_bell(x, y)
  return add_actor({
    x = x,
    y = y,
    anims = {
      default = anim.new(1, 1, {133}, 0, false),
      ring = anim.new(
        1,
        1,
        {133, 134, 190, 136, 133, 137, 138, 139, 133, 140, 133},
        2,
        false,
        {[134] = {2}, [190] = {2}})
    }
  })
end

function new_ringer(x, y)
  return add_actor({
    x = x,
    y = y,
    anims = {
      default = anim.new(2, 2, {101}, 0, false),
      run = anim.new(2, 2, {101, 107, 109, 75, 77}, 4, true),
      ring = anim.new(2, 2, {105, 103, 101}, 4, false)
    }
  }, true)
end

function new_bigringer(x, y)
  return add_actor({
    x = x,
    y = y,
    anims = {
      default = anim.new(2, 2, {69}, 0, false),
      walk = anim.new(2, 2, {166, 168}, 8, true),
      ring = anim.new(3, 2, {72, 69}, 8, false, {[69] = {2}})
    }
  }, true)
end

function wait(n, skippable)
  for n = 1, n do
    yield()
    if skippable and (btnp(4) or btnp(5)) then
      break
    end
  end
end

function add_actor(props, enablecr)
  local actor = add(actors, props)

  actor.anim = actor.anims.default

  if enablecr then
    actor.actions = {}
    add(coroutines, cocreate(
        function()
          while true do
            local action = actor.actions[1]
            if action then
              action()
              del(actor.actions, action)
            end

            yield()
          end
        end
    ))
  end

  return actor
end

-- compensate for bug where stats don't change in sync
function poll_songpos()
  local pat = stat(24)
  local n = stat(21)
  local ticks = stat(26)

  if pat ~= songpos.pat and n ~= songpos.n then
    songpos.pat = pat
    songpos.n = n

    if songpos.prevticks > 0 and ticks >= songpos.prevticks then
      songpos.ticks = 0
      songpos.waitingforticksync = true
    end
  elseif pat == songpos.pat and n ~= songpos.n then
    songpos.n = n
  end

  if songpos.waitingforticksync then
    if ticks < songpos.prevticks then
      songpos.waitingforticksync = false
    end
  else
    songpos.ticks = ticks
    songpos.prevticks = ticks
  end
end

-- @param songcoords an array of {pattern, note}
function wait_until(songcoords)
  while songpos.pat < songcoords[1] do
    yield()
  end
  while songpos.n < songcoords[2] and songpos.pat == songcoords[1] do
    yield()
  end
end

function walk_tween(actor, args)
  local destx = args[1]
  local ticksrc = args[2]
  local tickdst = args[3]
  local flipxoverride = args[4]

  local flipx = flipxoverride
  if flipx == nil then
    flipx = destx < actor.x
  end
  actor:set_flipx(flipx)

  actor.anim = actor.anims.pointewalk

  local t = tween.new(actor.x, destx, ticksrc, tickdst, ease_linear)
  while actor.x ~= destx do
    local ticks = songpos.ticks

    actor.x = t:update(ticks)

    if ticks >= tickdst then
      break
    end
    yield()
  end
  actor.anim = actor.anims.default
end

function ease_linear(t, b, c, d)
  return ((t / d) * c) + b
end

function ease_in_quad(t, b, c, d)
  t = t / d
  return c * (t ^ 2) + b
end

function ease_out_quad(t, b, c, d)
  t = t / d
  return -c * t * (t - 2) + b
end

function hop_on_beat(actor, args)
  local dir = args[1]
  local destx = args[2]

  actor.anim = actor.anims.hop
  if dir == -1 then
    if not actor.flipx then
      actor.x += 3
    end
    actor.flipx = true
  end
  actor.vx = 0

  local sub = note_subscribe(
    function (ch, s, n, note)
      return ch == 1 and (n + 2) % 4 == 0
    end,
    function()
      actor.anim:reset(true)
      actor.vx = 0
    end)

  repeat
    local i = actor.anim.i

    if i == 4 and actor.vx == 0 then
      actor.vx = .94
    end

    actor.vx *= .89

    if i == #actor.anim.indexes then
      actor.vx = 0
    end

    actor.x += (actor.vx * dir)
    if dir == 1 then
      actor.x = min(actor.x, destx)
    else
      actor.x = max(destx, actor.x)
    end

    yield()
  until actor.x == destx

  note_unsubscribe(sub)
end

function walk_leap(actor, args)
  local walkpattern = args[1]
  local dir = args[2]

  actor:set_flipx(dir == -1)
  actor.anim = actor.anims.pointe

  wait_until({walkpattern, 0})
  actor.anim = actor.anims.pointewalkmanual

  local sub = note_subscribe(
    function (ch, s, n, note, p)
      return p == walkpattern and ch == 1 and note.volume > 0
    end,
    function() actor.anim:update(true) end)
  repeat
    actor.x += (.1 * dir)
    yield()
  until songpos.n >= 15
  note_unsubscribe(sub)

  actor.anim = actor.anims.leap
  actor.anim:reset()
  if actor.flipx then
    actor.x -= 8
  end

  repeat
    local i = actor.anim.i
    if i >= 3 and i <= 5 then
      actor.x += (.3 * dir)
    end
    yield()
  until actor.anim:is_done()

  actor.anim = actor.anims.default
  if actor.flipx then
    actor.x += 8
  end
end

function note_subscribe(filter, callback)
  return add(notesubs, {filter = filter, callback = callback})
end

function note_unsubscribe(sub)
  del(notesubs, sub)
end

function prop_all(t, props)
  for _, item in pairs(t) do
    for k, v in pairs(props) do
      item[k] = v
    end
  end
end

function add_action(actor, start, f, args)
  add(actor.actions,
    function()
      if start then
        wait_until(start)
      end
      f(actor, args)
    end)
end

function init_ballet()
  local y = keyboard_y - 11
  local b1 = new_ballerina(-20, y)
  local b2 = new_ballerina(-10, y)
  local mice1 = {b1, b2}
  for i, b in pairs(mice1) do
    local destx1 = 2 + ((i - 1) * 10)
    local destx2 = 44 + ((i - 1) * 10)

    add_action(b, {14, 15}, walk_tween, {destx1, 14 * 15, 14 * 28})
    add_action(b, {15, 12}, walk_leap, {16, 1})
    add_action(b, {18, 0}, walk_tween, {destx2, 0, 14 * 15})
    add_action(b, {19, (i - 1) * 4}, spin_after_fairy)
  end

  local b3 = new_ballerina(128)
  local b4 = new_ballerina(138)
  local mice2 = {b3, b4}
  prop_all(mice2, {y = y, flipx = true})
  for i, b in pairs(mice2) do
    local destx1 = 108 + ((i - 1) * 10)
    local destx2 = 64 + ((i - 1) * 10)

    add_action(b, {15, 0}, walk_tween, {destx1, 0, 7 * 24})
    add_action(b, {16, 28}, walk_leap, {17, -1})
    add_action(b, {18, 0}, walk_tween, {destx2, 0, 14 * 15, false})
    add_action(b, {19, (i + 1) * 4}, spin_after_fairy, {b == b4})
  end

  local fairy1 = new_ballerina(-8, y)
  kicker = fairy1
  local fairy2 = new_ballerina(128, y)
  fairy2.flipx = true
  prop_all({fairy1, fairy2}, {pal = {[1] = 1, [15] = 12}})
  local start = {18, 0}
  add_action(fairy1, start, fairy_solo, {24, 1})
  add_action(fairy2, start, fairy_solo, {97, -1})

  -- make silly mouse stand up
  add_action(b4, {20, 0}, function()
    set_anim(b4, 'unslump')
  end)

  local catch_up = function(actor, args)
    local other = args[1]
    walk_tween(actor, {other.x + 10, 14 * 20, 14 * 28})
  end

  local everyone = {fairy1, b1, b2, b3, b4, fairy2}
  for b in all(everyone) do
    if b == b4 then
      add_action(b, {20, 1}, walk_leap, {20, -1})
      add_action(b, {20, 20}, catch_up, {b3})
    else
      add_action(b, {19, 28}, walk_leap, {20, -1})
    end

    add_action(b, {20, 28}, walk_leap, {21, 1})
    add_action(b, {21, 24}, set_anim, 'pointe')
    add_action(b, {21, 28}, set_anim, 'default')
  end

  local destx = -8
  local leftgroup = {b2, b1, fairy1}
  for b in all(leftgroup) do
    add_action(b, {22, 0}, walk_tween, {destx, 0, 14 * 28})
    destx -= 10
  end

  local destx = 128
  local rightgroup = {b3, b4, fairy2}
  for b in all(rightgroup) do
    add_action(b, {22, 0}, walk_tween, {destx, 0, 14 * 28})
    destx += 10
  end

  for i, b in pairs({fairy1}) do
    local x = -(10 * 2) + (i * 10)
    add_action(b, {22, 29}, function(actor)
      actor.x = x
      actor.flipx = false
    end)
    add_action(b, {24, 9}, hop_on_beat, {1, headdrop_x - 6})
    add_action(b, {24, 30}, function(actor) actor.readytokick = true end)
    add_action(b, {27, 27}, do_anim, 'turn')
    add_action(b, nil, hop_on_beat, {-1, -28})
  end

  local destx = 40
  for b in all(leftgroup) do
    add_action(b, {35,  0}, function(a, args) a.x = args[1] end, {destx - 50})
    add_action(b, {36,  0}, walk_tween, {destx, 0, 14 * 27})
    add_action(b, {37,  0}, do_anim, 'turn')
    add_action(b, {37, 16}, do_anim, 'turn')
    add_action(b, {37, 24}, set_anim, 'pointe')
    add_action(b, {37, 28}, set_anim, 'default')
    add_action(b, {38,  0}, do_anim, 'hop')
    add_action(b, {38,  4}, do_anim, 'hop')
    add_action(b, {38,  8}, walk_tween, {destx + 10, 14 * 8, 14 * 13})
    destx -= 10
  end

  local destx = 80
  for b in all(rightgroup) do
    add_action(b, {35,  0}, function(a, args) a.x = args[1] end, {destx + 50})
    add_action(b, {36,  0}, walk_tween, {destx, 0, 14 * 27})
    add_action(b, {37,  b == b4 and 10 or 8}, do_anim, 'turn')
    add_action(b, {37, 24}, set_anim, 'pointe')
    add_action(b, {37, 28}, set_anim, 'default')
    add_action(b, {38,  0}, do_anim, 'hop')
    add_action(b, {38,  4}, do_anim, 'hop')
    add_action(b, {38,  8}, walk_tween, {destx - 10, 14 * 8, 14 * 13})
    destx += 10
  end

  local destx = -10
  local runstarts = {19, 16, 18, 17, 16, 14}
  for i, b in pairs(everyone) do
    add_action(b, {38, runstarts[i]}, function(actor)
      actor.anim = actor.anims.pointewalk
      local vx = 0
      while actor.x > destx do
        vx -= .1
        vx = max(-1, vx)
        actor.x += vx
        yield()
      end
    end)
  end
end

function set_anim(actor, animkey)
  actor.anim = actor.anims[animkey]
  actor.anim:reset()
end

function do_anim(actor, animkey)
  actor.anim = actor.anims[animkey]
  actor.anim:reset()
  repeat yield() until actor.anim:is_done()
  actor.anim = actor.anims.default
end

function spin_after_fairy(actor, args)
  local silly = args and args[1]

  set_anim(actor, 'turn')
  if silly then
    actor.anim.timer.length = 2
    actor.anim.loop = true
    while in_song_range({19, 0}, {19, 23}) do
      if actor.x < 120 then
        actor.x += 0.05
      end
      yield()
    end
    actor.anim.loop = false
    set_anim(actor, 'slump')
  else
    repeat yield() until actor.anim:is_done()
    set_anim(actor, 'default')
  end
end

function fairy_solo(actor, args)
  local destx = args[1]
  local dir = args[2]

  for i = 1, 4 do
    actor.anim = actor.anims.pointewalk
    while stat(21) % 8 ~= 0 do
      actor.x += (.17 * dir)
      if dir == -1 then
        actor.x = max(destx, actor.x)
      else
        actor.x = min(actor.x, destx)
      end
      yield()
    end

    set_anim(actor, 'turn')
    repeat
      actor.x += (.17 * dir)
      if dir == -1 then
        actor.x = max(destx, actor.x)
      else
        actor.x = min(actor.x, destx)
      end
      yield()
    until actor.anim:is_done()
  end
  set_anim(actor, 'default')
end

function run_tubamouse()
  repeat
    yield()
  until curtainy <= 64

  tubamouse.anim = tubamouse.anims.run
  while tubamouse.x < tubamouse.targetx do
    local dist = tubamouse.targetx - tubamouse.x
    local vx = dist * .1
    tubamouse.x += min(vx, .5)
    if dist < .5 then
      tubamouse.x = tubamouse.targetx
    end
    yield()
  end
  tubamouse.anim = tubamouse.anims.stand
  wait(fps / 2)
  tubamouse.y -= 6
  tubamouse.anim = tubamouse.anims.default
  tubamouse.hastuba = true
  tuba.visible = false

  wait(fps / 2)
  music(2)

  wait_until({14, 0})
  tubastage = {
    x1 = tubamouse.x + 3,
    y1 = tubamouse.y + 16,
    x2 = tubamouse.x + 14,
    y2 = keyboard_y
  }
  for y = tubamouse.y, tubamouse.y - 14, -.1 do
    tubamouse.y = y
    tubastage.y1 = y + 16
    yield()
  end
  tuba.y = tubamouse.y + 1

  wait_until({38, 24})

  tubamouse.hastuba = false
  tubamouse.anim = tubamouse.anims.run
  tubamouse.y += 6
  tubamouse.flipx = true
  tubamouse.vy = 0
  for x = tubamouse.x, -8, -1 do
    tubamouse.x = x
    if tubamouse.x < tubamouse.targetx - 3 then
      tubamouse.vy += .1
    end
    tubamouse.y += tubamouse.vy
    tubamouse.y = min(tubamouse.y, keyboard_y - 10)
    yield()
  end
end

function run_tuba()
  wait_until({38, 24})
  tuba.visible = true

  wait_until({38, 26})
  tuba.rotate = true
  tuba.x -= 7
  tuba.y += 12

  local ty = tween.new(
    tuba.y, tuba.y + 11,
    songpos.ticks, 14 * 31,
    ease_in_quad)
  repeat
    tuba.y = ty:update(songpos.ticks)
    yield()
  until ty:is_done()
end

function apply_palette(p)
  for k, v in pairs(p) do
    if v == -1 then
      palt(k, true)
    else
      pal(k, v)
    end
  end
end

function run_horse()
  -- enter from left, push tubamouse, exit to right
  wait_until({7, 0})
  move_horse(-40, 128, 1, 14 * 50, function(x)
    if x + horse.w > tubamouse.x then
      tubamouse.x = x + horse.w
    end
  end)

  -- enter from right, put tubamouse back
  wait_until({9, 0})
  move_horse(128 + tubamouse.w, tubamouse.targetx + tubamouse.w, -1, 14 * 15,
    function(x) tubamouse.x = horse.x - tubamouse.w end)

  set_anim(horsemouse1, 'default')
  set_anim(horsemouse2, 'default')
  wait(12)

  -- exit to right
  move_horse(tubamouse.targetx + tubamouse.w, 128, -1, 14 * 31)

  wait_until({30, 0})

  -- enter from left, pulling snowboarder
  snowboarder.hasline = true
  move_horse(-37, 184, 1, 14 * 52, function(x)
    snowboarder.x = x - 24
    add_particle(snowboarder.x + 2, snowboarder.y + 9, -1)
  end)
  snowboarder.hasline = false

  -- snowboarder: do a sick jump from the right
  wait_until({32, 0})
  set_anim(snowboarder, 'fall')
  local vx, vy, maxy, landed = -1, -0.6, keyboard_y - 10, false
  snowboarder.x = 128
  snowboarder.y = 21
  while snowboarder.x > -32 do
    vy += .09
    snowboarder.x += vx
    snowboarder.y += vy
    if snowboarder.y >= maxy then
      snowboarder.y = maxy
      landed = true
    end
    if snowboarder.x <= 47 then
      snowboarder.anim = snowboarder.anims.default
      snowboarder.flipx = true
      snowboarder.y = maxy + 1
    end
    if landed then
      add_particle(snowboarder.x + 5, snowboarder.y + 9, 1)
    end
    yield()
  end
end

function add_particle(x, y, vx)
    add(particles, {
      x = x,
      y = y,
      vx = vx,
      vy = -(rnd_int(1, 10) / 10),
      ttl = fps * 2
    })
end

function move_horse(src, dst, dir, tickdst, callback)
  local flipx = dir == -1

  horse.flipx = flipx
  horsemouse1.flipx = flipx
  horsemouse2.flipx = flipx
  horsemouse1.offsetx = flipx and 15 or 17
  horsemouse2.offsetx = flipx and 7 or 25
  horse.x = src

  set_anim(horsemouse1, 'run')
  set_anim(horsemouse2, 'run')
  horsemouse2.anim:update(true)

  local wrappedcallback = function(x)
    horsemouse2.x = x + horsemouse2.offsetx
    horsemouse1.x = x + horsemouse1.offsetx
    if callback then
      callback(x)
    end
  end

  move_to_target(horse, 'x', dst, tickdst, ease_linear, wrappedcallback)
end

function move_to_target(actor, axis, target, tickdst, easefunc, callback)
  local t = tween.new(actor[axis], target, songpos.ticks, tickdst, easefunc)
  local startpat = songpos.pat
  repeat
    local ticks = songpos.ticks
    if songpos.pat > startpat then
      ticks += (14 * 32)
    end

    local v = t:update(ticks)

    actor[axis] = v
    if callback then
      callback(v)
    end
    yield()
  until t:is_done()
end

function init_bell_ringers()
  local h = 16
  local positions = {
    {x = -18, targetx =   3, y = 45, dir =  1, pitch = 36},
    {x = 130, targetx = 109, y = 45, dir = -1, pitch = 41},
    {x = -18, targetx =   3, y = 13, dir =  1, pitch = 43},
    {x = 130, targetx = 109, y = 13, dir = -1, pitch = 48},
    {
      x = 50,
      targetx = 50,
      y = -h * 2,
      targety = top_platform_y - h,
      dir = 1,
      pitch = 53
    }
  }

  local cr = function(ringer)
    local waitoffset = flr(((ringer.pitch - 36) / 17) * 4)
    for pat in all({10, 33}) do
      wait_until({pat - 1, 20 - waitoffset})

      -- move out to target position
      set_anim(ringer, 'run')
      local oldx = ringer.x
      move_to_target(ringer, 'x', ringer.targetx, 14 * (28 - waitoffset),
        ease_out_quad)
      ringer.anim = ringer.anims.default
      wait_until({pat + 1, 8 + waitoffset})

      -- go back offscreen
      set_anim(ringer, 'run')
      move_to_target(ringer, 'x', oldx, 14 * (16 + waitoffset), ease_in_quad)
    end
  end

  local cr5 = function(ringer)
    local inity, i = ringer.y, 1
    for pat in all({10, 33}) do
      wait_until({pat, 14})

      local vy = 0
      while ringer.y < ringer.targety do
        vy += .04
        ringer.y += vy
        ringer.y = min(ringer.y, ringer.targety)
        yield()
      end

      if i == 2 then
        wait_until({38, 3})
      else
        wait_until({pat + 2, 28})
      end

      -- jump up
      local vx, vy = 0, -1.8
      if i == 2 then
        -- jump away from cat
        set_anim(ringer, 'run')
        vx = -0.3
        vy = -.85
        ringer.x -= 5
        ringer.flipx = true
      end
      while ringer.y > -16 do
        vy += .04
        ringer.x += vx
        ringer.y += vy
        yield()
      end

      ringer.y = inity
      i += 1
    end
  end

  for i, pos in pairs(positions) do
    local dir = pos.dir

    local belloffsetx = 10 * dir
    if dir == -1 then
      belloffsetx += 8
    end
    local belly = pos.targety and pos.targety + 4 or pos.y + 3
    local bell = new_bell(pos.targetx + belloffsetx, belly)
    if i == 5 then
      fallingbell = bell
    end
    bell.flipx = dir == -1

    local ringer = new_ringer(pos.x, pos.y)
    add_props(ringer, pos)
    ringer.flipx = dir == -1

    if i == 5 then
      add(ringer.actions, function() cr5(ringer) end)
    else
      add(ringer.actions, function() cr(ringer) end)
    end

    local ringfunc = function()
      set_anim(bell, 'ring')
      set_anim(ringer, 'ring')
    end

    note_subscribe(
      function(ch, s, n, note, p)
        return (p == 10 or p == 33) and ch == 1
           and note.volume > 0 and note.pitch == pos.pitch
           and note.effect ~= 2
      end,
      ringfunc)

    if i == 5 then
      note_subscribe(
        function(ch, s, n, note, p)
          return (p == 11 or p == 34) and n == 0
        end,
        ringfunc)
    end
  end
end

function init_bigringers()
  local cr = function(m)
    local exitdir = -m.dir
    local initial = {x = m.x, dir = m.dir, flipx = m.flipx, firstring = true}

    for pat in all({10, 33}) do
      add_props(m, initial)
      wait_until({pat, 0})

      -- move out to target position
      m.anim = m.anims.walk
      move_to_target(m, 'x', m.targetx, 9 * 14, ease_out_quad)

      m.anim = m.anims.default
      wait_until({pat + 1, 8})

      -- failsafe: turn in correct direction even if ringing was skipped
      m.dir = exitdir
      m:set_flipx(exitdir == -1)

      -- go back offscreen
      m.anim = m.anims.walk
      if m.dir == -1 then
        m.x += 8
      end
      local tickdst = (16 * 14) + (dir == -1 and 2 or 0)
      move_to_target(m, 'x', initial.x, tickdst, ease_in_quad)
    end
  end

  local trigger1 = function(ch, s, n, note, p)
    return (p == 10 or p == 33) and ch == 2
      and note.pitch == 51 and note.effect ~= 2
  end
  local trigger2 = function(ch, s, n, note)
    return s == 37 and (n == 0 or n == 2)
  end

  local br1 = new_bigringer(-16, 79)
  br1.targetx = 14
  br1.dir = 1
  br1.flipx = false
  local br2 = new_bigringer(128, 79)
  br2.targetx = 98
  br2.dir = -1
  br2.flipx = true
  local bigringers = {br1, br2}
  for br in all(bigringers) do
    add(br.actions, function() cr(br) end)

    br.firstring = true

    br.set_flipx = function(self, flipx)
      if self.flipx ~= flipx then
        self.flipx = flipx
        self.x += (15 * (self.flipx and -1 or 1))
      end
    end

    br.ringfunc = function()
      set_anim(br, 'ring')
      if br.firstring then
        if br.dir == -1 then
          br.x -= 8
        end
      else
        br:set_flipx(not br.flipx)
      end
      br.firstring = false
    end
  end

  note_subscribe(trigger1, br1.ringfunc)
  note_subscribe(trigger2, br2.ringfunc)
end

function add_props(t, props)
  for k, v in pairs(props) do
    t[k] = v
  end
end

function _update60()
  poll_songpos()
  for ch = 0, 3 do
    detect_note_starts(ch)
  end

  for cr in all(coroutines) do
    if costatus(cr) ~= 'dead' then
      assert(coresume(cr))
    else
      del(coroutines, cr)
    end
  end

  update_tuba()

  erase_things_for_snow()
  update_snow()
  update_particles()

  for _, a in pairs(actors) do
    a.anim:update()
  end

  update_rope(rope)

  update_titles()

  if stat(24) <= 0 and curtainy >= keyboard_y / 2 then
    if lightstimer:update() then
      lightstimer:reset()
      lightsn += 1
      lightsn %= 32
      change_lights(lightsn)
    end
  end
end

function erase_things_for_snow()
  rectfill(20, 15, 107, 40, 0) -- xmas lights
  pset(59, tubamouse.y + 7, 0) -- below hat pom
  draw_top_platform()
end

function draw_top_platform()
  spr(172, 48, top_platform_y - 5)
  spr(53, 56, top_platform_y - 5)
end

function new_rope()
  local nodes = {}

  local count = 20
  local x1 = 20
  local x2 = 108
  local x, y = x1, 15
  for i = 1, count do
    add(nodes,
      {
        x = x,
        y = y,
        vx = 0,
        vy = 0,
        light = {}
      })
    x += (x2 - x1) / (count - 1)
  end

  local light_colors = {8, 10, 11, 12, 14}
  local ci = 1
  local on = true
  local mod = 4
  for i = 2, #nodes do
    if i % 1 == 0 then
      on = not on
    end

    nodes[i].prv = nodes[i - 1]
    if i + 1 <= #nodes then
      nodes[i].nxt = nodes[i + 1]
    end
    nodes[i].light = {
      c = light_colors[ci],
      on = on,
      mod = mod
    }

    ci += 1
    if ci > #light_colors then
      ci = 1
    end

    if i % 2 == 0 then
      mod += 4
      if mod > 8 then
        mod = 4
      end
    end
  end

  nodes[#nodes].nxt = nodes[#nodes]
  nodes[#nodes].fixed = true

  return nodes
end

function update_rope(rope)
  for i = 2, #rope do
    local node = rope[i]
    if not node.fixed then
      local target = {
        x = node.prv.x + (node.nxt.x - node.prv.x) / 2,
        y = node.prv.y + (node.nxt.y - node.prv.y) / 2
      }
      local dx = (target.x - node.x) * rope_tightness
      local dy = (target.y - node.y) * rope_tightness

      dx -= (node.vx * rope_damping)
      dy -= (node.vy * rope_damping)

      node.vx += dx
      node.vy += dy

      node.vy += rope_gravity

      node.x += node.vx
      node.y += node.vy
    end
  end
end

function draw_rope()
  for i = 1, #rope - 1 do
    local a = rope[i]
    local b = rope[i + 1]

    line(a.x, a.y, b.x, b.y, 3)

    if i > 1 then
      circfill(a.x, a.y + 2, 1, a.light.on and a.light.c or 1)
    end
  end
end

function update_tuba()
  tubamouse.blowbox = {
    x = tubamouse.x + 4,
    y = tubamouse.y - 14,
    w = 13,
    h = 16
  }
  if tubamouse.anim.i == 3 or tubamouse.anim ~= tubamouse.anims.play then
    tubaisblowing = false
  end
end

function _draw()
  cls()

  for _, s in pairs(snowflakes) do
    pset(flr(s.x), flr(s.y), s.c)
  end

  if tubastage then
    fillp(stripe_fill[flr(tubastage.y1 % 4) + 1])
    rectfill(tubastage.x1, tubastage.y1, tubastage.x2, tubastage.y2, 0xd1)
    fillp()
    map(16, 0, tubastage.x1 - 6, tubastage.y1 - 5, 3, 1)
  end

  map(0, 0, 0, 0, 64, 64)
  draw_top_platform()
  draw_keyboard()
  draw_switchers()

  for _, p in pairs(particles) do
    pset(p.x, p.y, 7)
  end

  for i = 1, #actors do
    draw_actor(actors[i])
  end

  if snowboarder.hasline then
    line(snowboarder.x + 7, snowboarder.y + 4, horse.x - 1, horse.y + 5, 12)
  end

  if tuba.visible then
    draw_tuba()
  end

  draw_cat()
  draw_rope()
  draw_curtain()
  draw_titles()
end

function update_particles()
  for _, p in pairs(particles) do
    p.ttl -= 1
    if p.ttl <= 0 then
      del(particles, p)
    else
      p.vy += .05
      p.vx *= .99

      p.x += p.vx
      p.y += p.vy
    end
  end
end

function draw_switchers()
  if not keyboardon then
    spr(188, 24, 96)
  end

  for i = 1, 2 do
    local xo, yo = 0, 0
    if i == 1 then
      for c = 1, 15 do
        pal(c, 5)
      end
      xo, yo = -2, -1
    end

    for s in all(switchers) do
      if i == 2 then
        if s[6] then
          pal(15, 12)
        end
        pal(1, s[6] and 1 or 0)
      end
      spr(s[3], s[1] + xo, switcher.y + s[2] + yo, s[4] or 1, s[5] or 1)
    end
    pal()
  end
end

function draw_tuba()
  pal(2, 7)
  if tuba.rotate then
    draw_rot_90_ccw(tuba.sx, tuba.sy, tuba.sw, tuba.sh, tuba.x, tuba.y,
      tuba.palt)
  else
    for _, c in pairs(tuba.palt) do
      palt(c, true)
    end
    sspr(tuba.sx, tuba.sy, tuba.sw, tuba.sh, tuba.x, tuba.y)
  end
  pal()
end

function run_switchers()
  wait_until({2, 0})
  move_to_target(switcher, 'y', 93, 14 * 20, ease_out_quad)

  wait_until({2, 24})
  switchers[1][3] = 103
  keyboardon = true
  sfx(7, 3, 31)
  wait(5)
  switchers[1][3] = 101

  move_to_target(switcher, 'y', 128, 14 * 40, ease_in_quad)
end

function run_fallingbell()
  wait_until({38, 6})

  local vx, vy, maxy = -.6, 0, top_platform_y - 6

  while fallingbell.y < maxy do
    vy += .2
    vy = min(vy, 2)

    fallingbell.x += vx
    fallingbell.y += vy
    fallingbell.y = min(fallingbell.y, maxy)

    yield()
  end
end

function run_cat()
  wait_until({38, 0})

  -- extend a little
  move_cat_arm(
    57, 61,
    -16, 7,
    14 * 2, 14 * 4, ease_out_quad)

  -- retract
  move_cat_arm(
    cat.pawx, 60,
    cat.pawy, -16,
    songpos.ticks, 14 * 8, ease_in_quad)

  -- go out farther
  move_cat_arm(
    79, 86,
    -16, 23,
    14 * 11, 14 * 15, ease_out_quad)

  -- make lights fall
  rope[#rope].fixed = false
  rope_gravity *= 2
  rope_tightness *= 1.2

  -- retract quickly
  move_cat_arm(
    cat.pawx, 94,
    cat.pawy, -32,
    songpos.ticks, 14 * 18, ease_in_quad)

  -- go out one last time
  move_cat_arm(
    cat.pawx, 74,
    -16, 23,
    14 * 24, 14 * 30, ease_out_quad)
end

function move_cat_arm(xsrc, xdst, ysrc, ydst, ticksrc, tickdst, easefunc)
  local tx = tween.new(xsrc, xdst, ticksrc, tickdst, easefunc)
  local ty = tween.new(ysrc, ydst, ticksrc, tickdst, easefunc)
  repeat
    cat.pawx = tx:update(songpos.ticks)
    cat.pawy = ty:update(songpos.ticks)
    yield()
  until tx:is_done()
end

function draw_cat()
  -- draw arm extension
  local sh = 3
  for y = cat.pawy - (sh * 10), cat.pawy - 1, sh do
    sspr(115, 29, 9, sh, cat.pawx + 3, y, 9, sh, false, true)
  end

  -- draw paw
  sspr(112, 16, 16, 16, cat.pawx, cat.pawy, 16, 16, false, true)
end

function draw_keyboard()
  palt(0, false)
  palt(4, true)

  rectfill(1, keyboard_y, 126, keyboard_y, 6)
  pset(21, keyboard_y, 13)
  pset(89, keyboard_y, 13)
  sspr(0, 96, 128, 127, 0, keyboard_y + 1)
  sspr(22, 104, 42, 24, 64, 104) -- dup. some keys to cover up other sprites

  palt()

  for _, note in pairs(melodynotes) do
    if note and note.pitch then
      draw_key(note)
    end
  end
end

function move_curtain(vd)
  local vy = 0
  repeat
    vy += vd
    vy = mid(-1, vy, 1.5)
    curtainy += vy
    curtainy = mid(curtain_min_y, curtainy, curtain_max_y)
    yield()
  until curtainy == curtain_min_y or curtainy == curtain_max_y
end

function fadeout_titles(waitlen, skippostwait)
  wait(waitlen * fps, true)
  prop_all(titles, {d = -.1})
  if not skippostwait then
    wait(fps * .5)
  end
end

function run_curtain()
  wait(fps * .66)
  local t = add_title({'', '',
    'kittenm4ster',
    '',
    'in association with',
    'the pico-8 advent project',
    '',
    'presents...'})
  t.logo = true
  fadeout_titles(6)

  add_title({
    'not a creature was stirring',
    '',
    '',
    '', '', '', ''})
  wait(fps * 4, true)
  add_title({
    '',
    '',
    '(...except squeaky whiskerson',
    'and his',
    'marvelous musical mice!)',
    '',
    '♪⌂♪   ',
  })

  fadeout_titles(6, true)
  move_curtain(-0.1)

  wait_until({38, 26})
  move_curtain(.15)
  wait(fps * .75)

  add_title({'the end'})
  fadeout_titles(6)

  add_title({
    'made by andrew anderson',
    'for the pico-8 advent project',
    '2018-12-07',
    '',
    'with choreography assistance',
    'from',
    'aubrianne anderson',
  })
  fadeout_titles(12)

  add_title({
    'music adapted from',
    '"sleigh ride"',
    'composed by leroy anderson',
    'in 1948'})
  fadeout_titles(10)

  add_title({
    'squeaky whiskerson',
    'would like to thank',
    'whoever left the',
    'old keyboard in the attic'
  })
  fadeout_titles(12)

  add_title({'merry xmas!'})
end

function draw_curtain()
  if curtainy >= -33 then
    local i = 0
    for x = 0, 127, 8 do
      local offsety = 0
      offsety = ceil((x + 1) / 8) * 2

      local maxy = keyboard_y - 7
      maxy += ((i % 2 == 0) and 1 or 0)

      for y = ((curtainy + offsety - 127) % 8) - 8, curtainy + offsety, 8 do
        spr(189, x, min(y, maxy))
      end
      i += 1
    end
  end
end

function draw_key(note)
  local i = note.pitch - 24
  if in_song_range({17, 30}, {19, 21}) or in_song_range({21, 30}, {23, 1}) then
    i += 12
  end
  if note.ch == 1 and (in_song_range({19, 23}, {19, 31})
      or in_song_range({38, 26}, {38, 29})) then
    -- don't draw silly slide notes
    return
  end
  if in_song_range({24, 0}, {26, 0}) then
     i -= 12
  end

  i %= key_count

  local key = key_pos_type[i]
  local keytype = key[2]
  local x = key[1]
  local y = keyboard_y + 12
  local w = keytype == 4 and 4 or 5
  local h = 20
  local sx = key_sx[keytype]

  sspr(sx, 64, w, h, x, y)
end

function in_song_range(a, b)
  if ((songpos.pat == a[1] and songpos.n >= a[2]) or songpos.pat > a[1]) and
     (songpos.pat < b[1] or (songpos.pat == b[1] and songpos.n <= b[2])) then
    return true
  end
end

function draw_actor(actor)
  pal(1, 0)

  if actor.pal then
    apply_palette(actor.pal)
  end

  local s = actor.anim:get_index()
  local offsetx = actor.anim:get_offset()
  if actor.flipx then
    offsetx *= -1
  end
  local sw, sh = actor.anim:get_size()
  if actor.flipx and sw ~= actor.anim.w then
    offsetx += 8 * (actor.anim.w - sw)
  end

  spr(
    s,
    actor.x + offsetx,
    actor.y,
    sw,
    sh,
    actor.flipx)
  pal()
end

function detect_note_starts(ch)
  local n = stat(20 + ch) -- note index
  if n ~= channelnote[ch] then
    -- <= 31 is to compensate for bug where faster patterns overflow
    if n >= 0 and n <= 31 then
      local s = stat(16 + ch) -- sfx index
      if s ~= -1 then
        local note = get_note(s, n)
        note_started(ch, s, n, note)
        channelnote[ch] = n
      end
    elseif ch > 0 then
      melodynotes[ch] = nil
    end
  end
end

-- @param ch channel index
-- @param s sfx index
-- @param n note index
-- @param note note object
function note_started(ch, s, n, note)
  for _, sub in pairs(notesubs) do
    if sub.filter(ch, s, n, note, songpos.pat) then
      sub.callback()
    end
  end

  if note.instrument == 2 and not note.issfxinst and note.volume > 0
     and tubamouse.hastuba then
    set_anim(tubamouse, 'play')
    tubaisblowing = true
  end

  if note.instrument == 7 and note.issfxinst and note.volume > 0 then
    if n % 2 == 0 then
      horse.anim:update(true)
    end
  end

  if ch == 1 or (ch > 0 and songpos.pat >= 15 and songpos.pat <= 21) then
    if note.instrument == 1 and note.volume > 0 then
      note.ch = ch
      melodynotes[ch] = note
    else
      melodynotes[ch] = nil
    end
  end

  if ch == 1 then
    change_lights(n)
  end
end

function change_lights(n)
  for i = 2, #rope - 1 do
    local node = rope[i]
    if n % node.light.mod == 0 then
      node.light.on = not node.light.on
    end
  end
end

function get_note(sfx, n)
  local addr = 0x3200 + (68 * sfx) + (2 * n)
  local byte1 = peek(addr)
  local byte2 = peek(addr + 1)
  local inst_r = shr(band(byte1, 0b11000000), 6)
  local inst_l = shl(band(byte2, 0b00000001), 2)

  return {
    addr = addr,
    byte1 = byte1,
    byte2 = byte2,
    pitch = band(byte1, 0b00111111),
    volume = shr(band(byte2, 0b00001110), 1),
    effect = shr(band(byte2, 0b01110000), 4),
    instrument = bor(inst_l, inst_r),
    issfxinst = band(byte2, 0b10000000) > 0
  }
end

function init_snowflake(s)
  s.y = 0
  s.x = rnd_int(0, 127)
  s.w = 1
  s.h = 1
  s.vx = rnd_int(1, 3) / 12
  if rnd_int(0, 1) == 1 then
    s.vx *= -1
  end
  s.vy = rnd_int(3, 8) / 10
  s.maxv = rnd_int(2, 9) / 10
  s.c = rnd_choice({5, 7, 7})
  s.collidable = (s.c == 7)
  s.landed = false
end

function snow_hit(c)
  return c ~= 0 and c ~= 7 and c ~= 5
end

function update_snow()
  for _, s in pairs(snowflakes) do
    local ignorehits = false
    if  curtainy >= s.y - 32 then
      ignorehits = true
    end

    if s.collidable and not ignorehits and
       snow_hit(pget(flr(s.x), flr(s.y))) then
      s.ttl = 0
    end

    if s.landed and s.ttl <= 0 then
      init_snowflake(s)
    end

    s.vy += .05

    local hit = false
    if s.collidable and not ignorehits then
      local cx = pget(flr(s.x + s.vx), flr(s.y))
      local cy = pget(flr(s.x), flr(s.y + s.vy))
      hit = snow_hit(cx) or snow_hit(cy)
    end

    if (hit or s.y > 125) then
      s.y = flr(s.y)
      s.vx = 0
      s.vy = 0

      if not s.landed then
        s.landed = true
        s.ttl = rnd_int(fps, fps * 4)
        if flr(s.y) == keyboard_y - 1 then
          s.ttl = fps
        end
      end
    end

    if s.landed then
      s.ttl -= 1
    end

    if tubaisblowing then
      if overlap(s, tubamouse.blowbox) then
        local dist = max(1, tubamouse.blowbox.y + tubamouse.blowbox.h - s.y)
        s.vy -= (1.2 / dist)
        local xdir = (s.x < tubamouse.blowbox.x + 6 and -1 or 1)
        s.vx += rnd(2) * (.1 / dist) * xdir
      end
    end

    s.vy = mid(-s.maxv, s.vy, s.maxv)
    s.vx = mid(-s.maxv, s.vx, s.maxv)

    s.x += s.vx
    s.y += s.vy
  end
end

function overlap(a, b)
  return (a.x + a.w > b.x
    and a.x < b.x + b.w
    and a.y + a.h > b.y
    and a.y < b.y + b.h)
end

function add_title(lines)
  return add(titles, {lines = lines, v = 0, d = .05})
end

function update_titles()
  for _, t in pairs(titles) do
    t.v += t.d
    t.v = mid(0, t.v, 1)
    if t.v == 0 then
      del(titles, t)
    end
  end
end

function fade(v)
  for c = 0, 15 do
    pal(c, get_fade_col(c, v))
  end
end

function get_fade_col(c, v)
  local i = flr(v * title_fade_len) + 1
  if i < title_fade_len and title_fade_table[c] then
    return title_fade_table[c][i]
  end

  return c
end

function draw_logo(fadev)
  local x, y, bg = 58, 9, get_fade_col(0, fadev)

  -- draw black border/background
  for c = 0, 15 do
    pal(c, bg)
  end
  for a = -1, 1 do
    for b = -1, 1 do
      spr(232, x + a, y + b, 2, 2)
    end
  end
  pal()

  fade(fadev)
  spr(232, x, y, 2, 2)
  pal()
end

function draw_titles()
  for t in all(titles) do
    local y = ((keyboard_y / 2) - ((#t.lines * 8) / 2))

    if t.logo then
      draw_logo(t.v)
    end

    fade(t.v)
    for line in all(t.lines) do
      cprint(line, 64, y, 7, 0)
      y += 8
    end
    pal()
  end
end

-- animation class
anim = {}
anim.__index = anim

function anim.new(w, h, indexes, delay, loop, indexwh, indexoffset)
  local obj = {
    indexes = indexes,
    i = 1,
    timer = timer.new(delay),
    loop = loop or false,
    w = w,
    h = h,
    indexwh = indexwh or {},
    indexoffset = indexoffset or {}
  }

  setmetatable(obj, anim)
  return obj
end

function anim:update(force)
  if self.timer:update() or force then
    self.timer:reset()

    if self.i < #self.indexes then
      self.i += 1
    elseif self.loop then
      self:reset()
    end
  end
end

function anim:get_index()
  return self.indexes[self.i]
end

function anim:get_offset()
  return self.indexoffset[self:get_index()] or 0
end

function anim:get_size()
  local wh = self.indexwh[self:get_index()]
  if wh then
    return wh[1], wh[2] or self.h
  end

  return self.w, self.h
end

function anim:reset()
  self.i = 1
  self.timer:reset()
end

function anim:is_done()
  return (self.i == #self.indexes and self.timer.value == 1)
end

-- timer class
timer = {}
timer.__index = timer
function timer.new(frames)
  local obj = {
    length = frames,
    value = frames
  }

  setmetatable(obj, timer)
  return obj
end

function timer:reset()
  self.value = self.length
end

function timer:update()
  if self.value > 1 then
    self.value -= 1
  else
    return true
  end
end

-- tween class
tween = {}
tween.__index = tween

function tween.new(src, dst, ticksrc, tickdst, func)
  local obj = {
    src = src,
    dst = dst,
    val = src,
    ticksrc = ticksrc,
    tickdst = tickdst,
    ticktotal = tickdst - ticksrc,
    func = func
  }

  setmetatable(obj, tween)
  return obj
end

function tween:is_done(ticks)
  return self.val == self.dst
end

function tween:update(ticks)
  ticks = mid(0, ticks - self.ticksrc, self.ticktotal)
  self.val = self.func(ticks, self.src, self.dst - self.src, self.ticktotal)

  return self.val
end

function rnd_int(min, max)
  return flr(rnd((max + 1) - min)) + min
end

function rnd_choice(t)
  return t[flr(rnd(#t)) + 1]
end

function cprint(s, x, y, c, b)
  local w = max(0, (#s * 4) - 1)
  bprint(s, x - flr(w / 2), y, c, b)
end

function table_has_value(t, value)
  for _, v in pairs(t) do
    if v == value then
      return true
    end
  end
end

function draw_rot_90_ccw(sx, sy, sw, sh, dx, dy, skipcolors)
  for y = sy, sy + sh - 1 do
    for x = sx, sx + sw - 1 do
      local c = sget(x, y)
      if c ~= 0 and not table_has_value(skipcolors, c) then
        pset(dx + y - sy, dy - x - sx + (sw * 2), c)
      end
    end
  end
end

function bprint(s, x, y, c, b)
  for yo = -1, 1 do
    for xo = -1, 1 do
      if not (yo == 0 and xo == 0) then
        print(s, x + xo, y + yo, b)
      end
    end
  end
  print(s, x, y, c)
end

__gfx__
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
06777700000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
67777770000000000006606600066066000660660000000000000000000000000006606600066066000066660006606600066660000660660006606600066066
67777770000660660006161600061616000616160066066000000000000000000006161600061616000066660006666600066660000616160006161600061616
6777777000061616000066e0000066e0000066e0006161600000000000000000000066e0000066e0000066e0000066600000e6600000e66000006e60000066e0
67777770000066e00006fff60006fff60006fff600066e0000660660000000000006ff6000006ff60006fff60006fff60006fff00006ff600006fff60006fff6
066666000006fff60000fff00000fff00000fff0006fff6000616160006606600000fff00000fff00000fff00000fff00000fff00000fff00000fff00000fff0
000000000000fff0e0077777e0077777e0077777000fff00e0066e000e616160e00777770e07777700e77e770007ee7700077e7e000777770007777700e77777
00000000e00777770eeef0f00eeef0700eee70f0e07777700e6fff600e066e000eeef0f000eef0f0000eeff00000fef00000ffe00000fefe000efef0000ef0f0
000000000eeef0f00000707000007070000070700efe00f007efff07076fff670000707000007070000007700000707000007700000070700000707000007070
00000000000770770000707000007000000000700770007707f777f707f777f70000707000007070000007700000707000007700000070700000707000007070
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00777000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
06171700000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
06799700000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000090009000000
000000000000000000000000000000000006606600000000000000000000000000000000000000000000000000000000000000000006606600009e909e900000
00000000000000000000000000000000000616160000000000066066000000000000000000000000000000000000000000066066000616160009eee9eee90000
00660660000000000660660000777000000066e00000000000061616000000000006606600000000000000000006606600061616000066e00009eee9eee90000
006161600077700006161600061717000ee6fff6000000000e0066e00000000000061616000000000006606600061616000066e00006fff600999e999e999000
00066e00061717000066e00006799700000efff00000000000e6fff600000000000066e00000000000061616000066e00e06fff60eeefff009ee9999999ee900
000fff0006799700e6fff6000067700077f77777f7700000000efff0000000000006fff600000000000066e00e06fff600eefff077f7777709eee99999eee900
006fff60006770000efff00700000000000000000000000077f77777000000000000fff0000000000ee6fff600eefff077f77777000000f009eee9eee9eee900
e077777000000000077777f70000000000000000000000000000000f000000000e07777700000000000efff077f77777000000f000000070099e9eeeee9e9900
0eef0f000000000000f00000007770000000000000000000000000077000000000eef0f00000000077f77777000000f000000070000000700099eeeeeee99000
00770770007770000770000006171700000000000000000000000000000000000007707700000000000000770000007700000070000000000009eeeeeee90000
000000000617170000000000067997000000000000000000000000000000000000000000000000000000000000000000000000000000000000099ee9ee990000
00000000067997000000000000677000000000000000000000000000000000000000000000000000000000000000000000000000000000000009999999990000
00000000006770000000000000000000ddddddddddddd00000000000000000000000000000000000000000000000000000000000000000000009999999990000
00000000000000000000000000000000111111111111000000000000000000000000000000000000000000000000000000000000000000000009999999940000
00000000000000000000000000000000ddddddddddd0000000000000000000000000000000000000000000000000000000000000000000000009999994490000
000000000000000000000000000000000000000000000090000000000000000000000000000a0000000000000000000000000000000000000000000000000000
000000000000000000000000000000040400000000009aaaaa70000000000000000000000000a09a000000000000000000000000000000000000000000000000
00660660000000000000000000000ff44f000000000009aaa70000000000000000000000000a009aa00000000000000000000000000000000000000000000000
0061616000000000000000000000ff4444f00000000009aaa700000006606600000000000000009aaa0000000000000000000000000000000000000000000000
00066e0000000000000000000000f44477440000000009aaa70000000616160000000000000009aaaaa900000000000000000000000000000000000006777700
888332060000088000000000000ff444714440000000009a700000000066e00000000000000009aaaa7700000000000444000000000000000000000067777770
08632330000082880000444833338444444445000000009a70000000e6fff6000000000000009aaaa70000000000000040000000000000044400000067777770
088233200008802800f44448333384444444550000000009000000000efff0000000000000009aaa70000a000000000040000000000000004000000066666660
08883233200880000ff44448333384444004500000000004000000000777770000000000000049a70000a0000006606640000000000000004000000000000000
0888883233288000ff0444448888444440000000006606640000000000f00f00000660660004000000000a000006161640000000000660664000000000000000
0888888888888000ff0444444444444440000000006161640000000007700770000616160040000000000000000066e040000000000616164000000000000000
008888888888860ff0044444444444444000000000066e040000000000000000000066e004000000000000000008888540000000000866e04000000006777700
0006888868806060000044444444444400000000000888540000000000000000000888854000000000000000e000336640000000000088854000000067777770
0006000060006060000000000000000000000000e083366400000000000000000e00336600000000000000000ee3333040000000e00033664000000067777770
00600006000600600000000000000000000000000ee33304000000000000000000e33340000000000000000000000003000000000ee333304000000067777770
66666666666666000000000000000000000000000003030000000000000000000000003000000000000000000000000000000000000000300000000006666600
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
00660660000000000000000000000004040000000000000000000000000000000000000000000000000000000000000000000000000000000000000006777700
00616160000000000000000000000ff44ff000000000000000000000000000000000000000000000000000000000000000000000000000000000000067777770
00066e0000000000000000000000ff44440000000000000000000000000000000000000000000000000000000000000000000000000000000000000067777770
88833260000008800f004440000ff444714400000000000000000000000000000000000000000000000000000000000000000000000000044400000067777770
0863233000008288fff444483333f444774440000000000444000000000000000440000000000000000000000000000444000000000000004000000067777770
0882332000088028ff04444833338444444445000000000040000000000000000044400000000000000040000000000040000000000000004000000006666600
088832332008800ff004444833338444444455000000000040000000000000000040000000000000000004000000000040000000000660664000000000777000
088888323328800ff004444488884444400450000006606640000000000066066400000000006606600040400006606640000000000616164000000006171700
088888888888800f0004444444444444400000000006161640000000000061616400000000006161600400000006161640000000000066e04000000006799700
0088888888888600000044444444444440000000000066e0400000000000066e040000000000066e00400000000066e040000000000088854000000000677000
00068888688060600000044444444444000000000000888540000000000008885400000000008888540000000000888540000000000833664000000000000000
0006000060006060000000000000000000000000e0083366400000000e008336600000000e00033660000000e008336640000000eeee33334000000000000000
00600006000600600000000000000000000000000eee33304000000000ee33334000000000ee3334000000000eee333340000000000300000000000000000000
66666666666666000000000000000000000000000000303000000000000000030000000000000003000000000000300000000000000000000000000000000000
000000000000000000000000000000000000000000009000000009000000000000009000000900000009000000009000000090000bbbbd88800008800008888d
00000000000000000008880000888800000000000009a70000009a70000000000009a700009a7000009a70000009a7000009a7000bbbbd88800008800008888d
0088880000888800008088800700888000888800009aaa700009aaa700000000009aaa7009aaa70009aaa700009aaa70009aaa700bbbbd88800008800008888d
0700888007008880070677760006777607008880009aaa700009aaa700000000009aaa7009aaa70009aaa700009aaa70009aaa700bbbbd88800008800008888d
0006777600067776000616160006161600067776009aaa700009aaa700000000009aaa7009aaa70009aaa700009aaa70009aaa700bbbbd88800008800008888d
0006161600061616000066e0000066e00006161609aaaaa7009aaaaa7000000009aaaaa79aaaaa709aaaaa7009aaaaa709aaaaa70bbbbd88800008800008888d
000066e0000066e000063336e0063336000066e000009000000090000000000000090000000090000000090000000900000900000bbbbd88800008800008888d
e0063336e0063336eeee33330ee33330e006333600000000000000000000000000000000000000000000000000000000000000000bbbbd88800008800008888d
0eee33300eee333300030000000000030ee3333000ddddd0000000000000000000000000000000000006606600066066000000000bbbbd88800008800008888d
00003030000030000000000000000000000000300d00000dd00000000000000000066066000660660006161600061616000660660b77bd88800008800008888d
0000000000000000000000000000000000000000d000dd000dd0000000dd00000006161600061616000066e0000066e0000616160bbbbd88800008800008888d
0000000000000000000000000000000000000000d00d00d0000dd0000d00d000000066e0000066e00006666600066666000066e000000d88888d88888d88888d
0000000000000000000000000000000000000000d00000d000000dd00000d0000006666600066666000044400e0044400006666600000d88888d88888d88888d
00000000000000000000000000000000000000000d000d000000000dd00d00000e0044400e0044400eee444500e544400e00444000000d88888d88888d88888d
000000000000000000000000000000000000000000ddd000000000000dd0000000ee444000ee4445000500000000000500e5444000000d88888d88888d88888d
0000000000000000000000000000000000000000ddddddddddddddddddddd000000050500000500000000000000000000000005000000d88888d88888d88888d
000000000000000000000aaaaaaaaaaa00000aaaaaaaaaaa00000090000000000000009000000000000bbb00000000000000000000000d88888d88888d88888d
000000aaaaaaaaa0000000ffffffff20000000ffffffff2000009aaaaa70000000009aaaaa70000000499940880000000000000000000d88888d88888d88888d
0000000ffffff2000000000f2ffff2000000000f2ffff200000009aaa7000000000009aaa700000000414148000000000000000000000d88888d88888d88888d
0000000f2ffff20000000000f2ff200000000000f2ff2000000009aaa7000000000009aaa7000000000e4480000000000000000000000ddddddddddddddddddd
00000000f2ff200000000000ffff200000000000ffff2000000009aaa7000000000009aaa70000000c488800e000000000000000000000000000000000000000
00000000ffff2000000000000ff20000000000000ff200000000009a700000000000009a70000000cccaaa0e00000000000ddddd000000000000000000000000
000000000ff20000000700000ff20000000070000ff200000000009a700000000000009a700000000ccca4e00000000000001111000000000000000000000000
008888000ffaaa00000080000ffaaa00000080000ffaaa000000000900000000000000090000000000ccca000000000000000ddd000000000000000000000000
070088800ff40a20000088000ff40a20000088000ff40a2000000004000000000000000400000000000ccc000000000066666666888222820000090000000000
000077700f4f4a20000088800f4f4a20000088800f4f4a20006606640000000000660664000000000000ccc000000000556660608882228200009a7000000000
000616160aaa4a20000677760aaa4a20000677760aaa4a200061616400000000006161640000000000000c000000000055662266888222820009aaa700000000
000666e6af4a4a2000061616af4a4a2000061616af4a4a2000066e040000000000066e0400000000000000000000000066662255888222820009aaa700000000
00006aaa0f4aaa2000006aaa0f4aaa2000006aaa0f4aaa2000088854000000000008885400000000000000000000000066662266888222820009aaa700000000
e0063aa60f4f4f2000063aa60f4f4f2000063aa60f4f4f20e083366400000000e08336640000000000000000000000006666666688822282009aaaaa70000000
0eee33300ff4ff20eeee33300ff4ff20eeee33300ff4ff200ee33334000000000e33330400000000000000000000000066666666888222820009000000000000
0000303000fff2000000303000fff2000000303000fff200000300000000000000000300000000000000000000000000dddddddd888222820000000000000000
666666666666666666666d666666666666666666666666666666666666666666666666666666666666666666d666666666666666666666666666666666666666
666565656565656565666d66886660606060666660606060666666666666666666666666666666666666666d6666666666666666666666666ddddddddddddddd
665656565656565656566d6688666622666666666666662266666611666cc66699666aa666ee6661166666d666ee666aa66611666cc66666d666666666666666
666565656565656565666d666666552255555666555555225666611166ccc6699966aaa66eee661116666d666eee66aaa6611166ccc6666d6666663333366666
665656565656565656566d66666666226666666666666622666611166ccc6699966aaa66eee661116666d666eee66aaa6611166ccc6666d66666633333336666
666565656565656565666d66666666666666666666666666666611666cc66699666aa666ee666116666d6666ee666aa66611666cc6666d666666663333366666
665656565656565656566d666666666666666666666666666666666666666666666666666666666666d6666666666666666666666666d6666666666666666666
6665656565656565656666dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd66666666666666666666
6656565656565656565666666666666666666666666666666666666666666666000000ddddd0000000000000000000000ddddd00666666666666666666666666
66656565656565656566666666666666666666666666666666666666666666660000000ddd000000000000000000000dd00000d0666666666666666666666666
665656565656565656566555555555555555555555555555555555555555555500000000000000000000dd0000000dd000dd000d555555555555555555556666
666565656565656565666d77700007700007777d77700007700007700007777d0000000000000000000d00d0000dd0000d00d00d7d77700007700007777d6666
665656565656565656566d77700007700007777d77700007700007700007777d0000000000000000000d00000dd000000d00000d7d77700007700007777d6666
666565656565656565666d77700007700007777d77700007700007700007777d00000000000000000000d00dd000000000d000d07d77700007700007777d6666
665656565656565656566d77700007700007777d77700007700007700007777d000000000000000000000dd000000000000ddd007d77700007700007777d6666
666565656565656565666d77700007700007777d77700007700007700007777d0000000000000000000ddddddddddddddddddddd7d77700007700007777d6666
665656565656565656566d77700007700007777d77700007700007700007777d0088800088800000000bbb00000bbb00000bbb008077700007700007777d6666
666565656565656565666d77700007700007777d77700007700007700007777d08200808002800000049994000499940004999408077700007700007777d6666
665656565656565656566d77700007700007777d77700007700007700007777d00820888028000008041414000414140004141480077700007700007777d6666
666565656565656565666d77700007700007777d77700007700007700007777d7eee77777ffff00008844e0088844e00000e44800077700007700007777d6666
665656565656565656666d77706607706607777d77706607706607706607777d8888fffffeeee00000488840004888400c488800e077706607706607777d6666
666565656565656566666d77700007700007777d77700007700007700007777d88777777777ee000000aaa00000aaa00cccaaa0e0077700007700007777d6666
665656565656565666566d77777d77777d77777d77777d77777d77777d77777d88777777777ee000eeaaaaa0eeaaaaa00ccca4e00077777d77777d77777d6666
666565656565656665666d77777d77777d77777d77777d77777d77777d77777d99777222777dd00000a000a000a000a000ccca000077777d77777d77777d6666
665656565656566656566d77777d77777d77777d77777d77777d77777d77777d99777772777dd0000ccccccc0ccccccc000ccc000077777d77777d77777d6666
666565656565666565666d77777d77777d77777d77777d77777d77777d77777d99777772777dd00000000000000000000000ccc00077777d77777d77777d6666
665656565656665656566d77777d77777d77777d77777d77777d77777d77777d99777772777dd000000000000000000000000c000077777d77777d77777d6666
666565656566656565666d77777d77777d77777d77777d77777d77777d77777d99777772777dd0000000000000000000000000000077777d77777d77777d6666
665656565666565656566d77777d77777d77777d77777d77777d77777d77777daa777777777cc0000000000000000000000000000077777d77777d77777d6666
666565656665656565666d77777d77777d77777d77777d77777d77777d77777daa777777777cc0000000000000000000000000000077777d77777d77777d6666
666666666666666666666dddddddddddddddddddddddddddddddddddddddddddaa993333311cc00000000000000000000000000000dddddddddddddddddd6666
4666666666666666666666666666666666666666666666666666666666666666aaaabbbbbcccc000000000000000000000000000006666666666666666666664
__label__
888222828882228288822282888222828882228288882888222820000000000000000040333e0288222822282882228288822282888222828882228288822282
8882228288822282888222828882228288822282888828882228200000000000000000466330e288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000050000004588800288222822282882228288822282888222828882228288822282
888222828882228288822282888222828882228288882888222820000000000000000040e6680288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000004606060288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000700000000004660660288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000004000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000009000000288222822282882228288822282888222828882228288822282
8882228288822282888222828882228288822282888828882228200000000000000007a900000288222822282882228288822282888222828882228288822282
8882228288822282888222828882228288822282888828882228200000000000000007a900000288222822282882228288822282888222828882228288822282
888222828882228288822282888222828882228288882888222820070000000000007aaa90000288222822282882228288822282888222828882228288822282
888222828882228288822282888222828882228288882888222820000000050000007aaa90000288222822282882228288822282888222828882228288822282
888222828882228288822282888222828882228288882888222820000000000000007aaa90000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000007aaaaa9000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000900000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000070000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000070000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000700000000500000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000700000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282005000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000070000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000700000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000070000000005000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282660000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282707000700000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282977000000000000000700000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282907000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282770000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000070000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000050000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000700000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000007000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282007000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000005000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000050000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000070000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282005000000000000007000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000000000000700288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000000700000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000700aaaaaaaaa000000000288222822282882228288822282888222828882228288822282
888222828882228288822282888222828882228288882888222820000000ffffff70000000000288222822282882228288822282888222828882228288822282
888222828882228288822282888222828882228288882888222820000000f7ffff70000000000288222822282882228288822282888222828882228288822282
8882228288822282888222828882228288822282888828882228200000000f7ff700000000000288222822282882228288822282888222828882228288822282
8882228288822282888222828882228288822282888828882228200000000ffff700000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000000000ff7000000000000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282008888000ffaaa0066066000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282070088800ff40a7061616000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000077700f4f4a700e660000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000606060aaa4a706fff6000288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282000666e6af4a4a700fff0000288222822282882228288822282888222828882228288822282
8882228288822282888222828882228288822282888828882228200006aaa0f4aaa707777700e288222822282882228288822282888222828882228288822282
88822282888222828882228288822282888222828888288822282e0063aa60f4f4f700f0feee0288222822282882228288822282888222828882228288822282
888222828882228288822282888222828882228288882888222820eee33300ff4ff7007070000288222822282882228288822282888222828882228288822282
888222828882228288822282888222828882228288882888222820000303000fff70007070000288222822282882228288822282888222828882228288822282
88822282666666668882228266666666888222826666688822282666666666666666666666666288222826666666666688822282666666668882228266666666
666565656565656565666d665566606060606666660606060666666666666666666666666666666666666666d666666666666666666666666ddddddddddddddd
665656565656565656566d6655662266666666666666662266666611666cc66699666aa666ee6661166666d666ee666aa66611666cc66666d666666666666666
666565656565656565666d666666225555555666555555225666611166ccc6699966aaa66eee661116666d666eee66aaa6611166ccc6666d6666663333366666
665656565656565656566d66666622666666666666666622666611166ccc6699966aaa66eee661116666d666eee66aaa6611166ccc6666d66666633333336666
666565656565656565666d66666666666666666666666666666611666cc66699666aa666ee666116666d6666ee666aa66611666cc6666d666666663333366666
665656565656565656566d666666666666666666666666666666666666666666666666666666666666d6666666666666666666666666d6666666666666666666
6665656565656565656666dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd66666666666666666666
66565656565656565656666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666
66656565656565656566666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666
66565656565656565656655555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555556666
666565656565656565666d77700007700007777d77700007700007700007777d77700007700007777d77700007700007700007777d77700007700007777d6666
665656565656565656566d77700007700007777d77700007700007700007777d77700007700007777d77700007700007700007777d77700007700007777d6666
666565656565656565666d77700007700007777d77700007700007700007777d77700007700007777d77700007700007700007777d77700007700007777d6666
665656565656565656566d77700007700007777d77700007700007700007777d77700007700007777d77700007700007700007777d77700007700007777d6666
666565656565656565666d77700007700007777d77700007700007700007777d77700007700007777d77700007700007700007777d77700007700007777d6666
665656565656565656566d77700007700007777d77700007700007700007777d77700007700007777d77700007700007700007777d77700007700007777d6666
666565656565656565666d77700007700007777d77700007700007700007777d77700007700007777d77700007700007700007777d77700007700007777d6666
665656565656565656566d77700007700007777d77700007700007700007777d77700007700007777d77700007700007700007777d77700007700007777d6666
666565656565656565666d77700007700007777d77700007700007700007777d77700007700007777d77700007700007700007777d77700007700007777d6666
665656565656565656666d77706607706607777d77706607706607706607777d77706607706607777d77706607706607706607777d77706607706607777d6666
666565656565656566666d77700007700007777d77700007700007700007777d77700007700007777d77700007700007700007777d77700007700007777d6666
665656565656565666566d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d6666
666565656565656665666d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d6666
665656565656566656566d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d6666
666565656565666565666d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d6666
665656565656665656566d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d6666
666565656566656565666d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d6666
665656565666565656566d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d6666
666565656665656565666d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d77777d6666
666666666666666666666ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd6666
06666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666660

__map__
00000000000000d8d900000000000000ac343500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
95969700000000000000000000dadbdc00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
3435000000000000000000000000ac3400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
95969700000000000000000000dadbdc00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
3435000000000000000000000000ac3400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
__sfx__
010800001d55624556295562655600500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000000
010800001d55622556295562655600500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000000
010800002255624556275562955600500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000000000000
010800002155624556275562955600500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000500005000050000000
010800001d55620556255562955600000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010800001d55621556245562955600000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010700001f55622556275562b5561d556225562655629556000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
0105000032355000000000000000000002b300323002b30000305003050030500305003050030500305003050030500305003050030500305003050030500305003050030500305003050030500305003053f615
010e00000522000200002000020000220002000020000200052200020000200002000022000200002000020005220002000020000200002200020000200002000522000200002000020000220002000020000200
011000000522000200002000020000220002000020000200052200020000200002000022000200002000020005220002000020000200002200020000200002000522000200002000020000200002000522000200
010e000035122291002b12200200291202612026122221202412024122261202412024122211201f1201f1221d1201d1221d1221d1221d1221d1221d1221d1221d1221d122221202412024122261202712027122
010e00000a220000001885018855052200000018950189550c2200000018a5018a55052200000018b5018b550a220000001885018855052200000018950189550c2200000018a5018a55052200000018b5018b55
010e00000a220002000020000200002000020005220002000a220002000020000200002000020005220002000a220002000020000200002000020005220002000a22000100291220c100291220c1002912200100
010e000029122291002b12200100291202612022122221002412224100261220010024120211201f1221f1001d1201d1221d1221d1221d1221d1221d1221d12200100001001f1202112022120241202612027120
010e000029122291002b12200100291202612024120221202412200100241202612024120221201f1221f10022120221222212222122221222212222122221220010000100291220c100291220c1002912200100
010e00000a220000001885018855052200000018950189550c2200000018a5018a55052200000018b5018b550a22000000188501885505220000001885018855012200000018c5018c55002200000018d5018d55
010e00001511015112151121511213110131121311213112161101611216112161121511015112151121511215110151121511215112131101311213112131121611016112161121611215110151121511215112
010e00001511015112151121511213110131121311213112161101611216112161121511015112151121511216110161121111011112131101311211110111121411214112111101111215112151121111011112
010e00000a220000001885018855052200000018950189550c2200000018a5018a55052200000018b5018b550a2200000018e5018e5018e4018e45052001880018e5018e5018e4018e450a20018d000000000000
010e000029122291002b12200100291202612024120221202412200100241202612024120221201f1221f1002212022122221222212222122221222212222122221222212200000000002b1002a0002a1202b120
010e000032122000002a1202b12032122000002a1202b120321220000034122000003112031122311223112200000000003212200000311202d1202a120000002f1202f1222f1222f1222f1222f1222f1222f122
010e00001511015112151121511213110131121311213112161101611216112161121511015112151121511216110161120000000000000000000005220000000a22011100111000000005220111000a22016100
010e0000042201980019800198050b220010001990019905042200000000000000000922000000000000000002220000000000000000092200000000000000000222000000000000000009220000000000000000
010e0000042201980019800198050b220010001990019905042200000000000000000922000000000000000002220000000000000000000000000009220000000222000000000000000009220000000922000000
010e000013110131121311213112131121311213112131121311213112171101711215110151121311013112121101211212112121121211212112121121211212112121120e1100e11212110121121711017112
010e000018f450000011f4511f4318f450000011f4511f4318f450000511f4500f0518f450000511f450000518f450000511f450000518f450000511f4528f0518f450000511f450000518f450000511f4500000
010e000018f450000011f4500f0518f450000511f450000518f450000511f4500f0518f450000511f450000518f450000511f4511f4318f450000511f0028f0511f4511f4318f450000518f000000511f4511f43
010e00001311013112131121311213112131121311213112131121311217110171121511015112131101311212110121121211212112121121211212112121121211212112121121211200000000000000000000
010e00002b1000010031122001002f1202b120281202b1202f1202f1223212232122311203112234122341222d1202d1222d1222d1222d1222d1222d1222d1222d1222d1222d1222d12200100001002812029120
010e00000222017800178001780509220000001790017905022200000000000000000722000000000000000000220000000000000000072200000000000000000022000000000000000007220000000000000000
010e00003012200000281202912030122000002812029120301220000032122000002f1202f1222f1222f122000000000030122000002f1202b12028120000002d1202d1222d1222d1222d1222d1222d1222d122
010e000011110111121111211112111121111211112111121111211112151101511217110171121d1101d1121c1101c1121c1121c1121c1121c1121c1121c1121c1121c112131101311215110151121c1101c112
010e00001b1101b1121b1121b1121b1121b1121b1121b1121b1121b1121b1121b1121b1121b1121b1121b1121d1101d11200000000001f1101f11200000000002111021112335303351233530335123353033512
010e00000022000200002000000000000000000c220002000022000200002000000000000000000c2200020005220002000020000200002200020000200002000522000200002000020000200002000522000200
010e0000005000050030530305222b530295302453024522295302b53030530305222b5302b5222953024530295302b5303053030522355303552230530305222953029512355303551235530355123553035512
010e00001611016112161121611216112161121611216112161121611216112161121611216112161121611215110151120000000000161101611200000000001811018112000000000000000000000000000000
010e000000220325003250015100052201310013100131000b220161001984017800198430000006220151000b220151001984017800198431310006220131000b2201f52024520285202a52000000000000b220
010600003253035530325223552232512355150000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
010e000029122291002b12200100291202612024120221202412200100241202612024120221201f1221f1002212022122221222212222122221222212222122221222212200000000002b1002a0000000000000
010700000922009220000000000000000000000000000000000000000000000000000222002220000000000009220092200000000000000000000000000000000222002220000000000002220022200000000000
010e000000000000001f5201f5221e5200000000000000001f5201f5221e520000000000000000241102411000000000000000000000000000000000000000000000000000000000000000000000000000000000
010e00000000000000245202452224522000000000000000245202452224522000000000000000261102811000000000000000000000000000000000000000000000000000000000000000000000000000000000
010e000000000000002752027522265200000000000000002752027522265200000000000000002a1102b11000000000000000000000000000000000000000000000000000000000000000000000000000000000
010e0000261120010226112241022611200102261120010226112001022612200102261220010226122001022a1322410200102001023f6003f60023122231222311223112231122311200102000001e1101f110
010e00002a112041022a112041022a112041022a112041022a112041022a122041022a122041022a122041022f132000020000000000000003f60028122281222611126112261122611200002000022311224112
010e000007220002000000000000022200000000000000000722000200000000000002220000000000000000072200000000000000000000000000022200000007220000000b220000000e220000000222000000
010e00002d112031022d112031022d112031022d112031022d112031022d122031022d122031022d12203102321320000200000000003f6353f6052b1222b1222a1112a1122a1122a11200000000000000000000
010e00002111200102211121f102211120010221112001022111200102211220010221122001022112200102261321f10200102001023a6003a60023122231221f1111f1121f1121f11200000000000000000000
010e0000261120010226112001022611200102261120010226112001022612200102261220010226122001022a132000020000000000000003b60026122261222311123112231122311200000000000000000000
010e00002a112001022a112001022a112001022a112001022a112001022a122001022a122001022a122001022f1320000200000000003e6353c6052a1222a1222811128112281122811200000000001712018120
010e00000822000000000000000002220000000000000000092200000000000000000a2200000000000000000b2200000000000000000b2200000000000000000422000000000000000001220010000100000000
010e0000062200000000000000000622000000000000000001220010000000000000062200000000000000000b22000000000000000008220000000000000000092200000000000000000e220000000222000000
010e00001a12018120171201712517120171221a1201a12218120181221f1201f1221e1201e1221e1221c1201a1201a1222412024122231202312221120211221f1201f1221f1221f1221f1221f1251e1201c120
010e00001e1201c1201b1201b1251b1201b1221c1201c1221e1201e122281202812227120271222512025122231202312223122231222312223122000003012032121000002d1220000026122000002411024110
010e00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000002611028110
010e00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000002a1102b110
010e00001a12018120171201712517120171221a1201a12218120181221f1201f1221e1201e1221e1221c1201a1201a1222412024122231202312221120211221f1201e1201c1201e1201f120211202312025120
010e00000222018000000000000009220000000000000000022201800000000000000922000000000000000002220180000000000000092200000000000000000222000000000000000002220000000000000000
010e0000261202612232120321222d1202b12026120261222b1202b1222d1202d12200000000000000000000000000000032120321222d1202b12026120261222b1202b1222d1202d12200000000000000000000
010e000000000000003512035122301202e12029120291222e1202e122301203512030120301222e120291202e12030120351203512230120301222e1202e1222912029122351200000035120000003512000000
011000003512035122301220000030122000003012200000301203012229122010002912201000291220000029120291223012201000301220100030122000003012030122351220100035122010003512200000
010e00000b22000000000000000000000000000000000000042200000000000000000422000000000000000009220000000000000000000000000000000000000222000000000000000007220000000000000000
010e00002a5222a1002c5222c1002a5202752023520235222f1222f10231122311002f1202c120281202810028522281002a5222a100285202552021520215222d1222d1022f1222f1022d1202a1202612026122
010e00002b122000002d122000002b12028120241222410223122000002a122000002a122000002a122000002a122000003112201000311220100031122000003112226102341213b1212d121271210000023122
__music__
00 48424344
00 49424344
00 4a0c7c44
00 0b0d1044
00 0f0e1144
00 0b0d1044
00 12131544
00 16141819
00 171c1b1a
00 1d1e1f19
00 21222023
00 0b0d1025
00 0f0e1144
00 0b0d1044
00 12261544
00 2728292a
00 2d2b2c2e
00 2d2f3031
00 32344344
00 33353637
00 2d2b2c2e
00 2d2f3031
00 32384344
00 393a4344
00 083b4344
00 093c4344
00 0b0a1044
00 0f0e1144
00 0b0d1044
00 12131544
00 16141819
00 171c1b1a
00 1d1e1f19
00 21222023
00 0b0d1025
00 0f0e1144
00 0b0d1044
00 3d3e4344
00 243f4344

