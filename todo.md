# TODO

* lock shop rooms, that you didnt save npcs yet
* lock access to the shop area, if you didnt save any npc yet
* npc should reward you with emeralds (when it dissappears, it leaves an emerald on the stand)
* teach controls
* remove lamp
* hearts, keys and coins should emit some light, maybe batteries
* ui string should support drawing small icons, shop npc's in the hall should say smth "gotta bring some [ic emerald]!", hinting that you need to collect emeralds
* overhaul dead screen

* poison, thorns, ice, fire, charm rings
* buffs display on mobs (like frozen and poisoned, charmed)
* Enemies / rooms drop rewards (bombs, keys, coins)

* tile lighting: figure it out
* dont forget little bursts of light for guns and stuff
* lava should be drawn over the shadows
* particles for lava like in pd
* fix lavafall colors
* fix fire particle changing the render state so much
* try wfc (wave function collapse) for procgen???

### sfx

better mob hurt
prop break
wall crawelr sfx
dummy sfx?
turret sfx

key pickup
bomb pickup
coin pickup
battery pickup

open lock louder

door open/close louder
door unlock/lock with enemies

spikes popping up

roll
gobbo hurt/heal

# todo

* check if audio settings works
* flashes (like lighting bolt zaps)
* rain
* when bomb explodes, disable lighting for a bit>

## audio

* switching between levels (dark out) should fade in/out music
* hide music disks around
* lowpass filter and somewhat responsible music? :(

## general

*!!! entrance to the shop can be blocked by a prop
*!!! pad treasure room still can spawn stands on stands

* ?!!! secret rooms seem to corrupt level sometimes
* ?!!! sometimes generation can fail, then go ok, game appears, but generation is still going in bg :OOO
* !!! generation needs hard testing

* idol room update, make them smaller, idol also will keep on spawning mobs with each load

* place items in oldman hall
* room cleared effect
* picking up hearts should give more feedback
* Low health indicator

* bk types:
 + slime
 + king
 + bandit
 
* breadcrumbs of where bk room is (alg for detecting shortest path is there)
* breadcrumbs for bk type 
* things, that watch after you, like in metroid. dungeon is dead at first, but gets more alive and alive??
* improve epic spawn
* curse mechanics (you get curse points, basically difficulty points, at 9 or 10 bk spawns, like he was before (chasing you all the time, except battle))
* juice up pistons (entity + tile mix for rendering, we need inbetweens for sure)
* menu screen needs shadows
* ellipses seem to be 1 tile less width and height??
* parallaxed decor in the chasms
* buff icons
* spice up bullets (different textures for different enemies)
* we need more secret room layouts

### future bk ideas

* worm
* ghost

### beta bugs

* openal crash
* bk missile projectiles have no sprite??? wrong depth, maybe?

### todo

* shields
* game intro

* Tile interactions
 * burning terrain
 * ember after burnt stuff
 * liquids should have smaller hitbox
 
* Buffs
 * poison (green poison drops dropping from you, some slowness)
 * slowness
 * speed
 * charmed hearts particles
 * frozen
 * buff icons?
 
* gases??

### bk himself

* Learning to avoid the last attack that killed him
* way to skip the boss battle but get harder game
* difficulty progression through the fight
* Learning to avoid the last attack that killed him

* steam cloud
* implement stats
* push hearts and batteries and coins around, if you cant have more
* Animate lamp up and down to make it more alive
* Crash report window
* lighting bolts
* figure out what to do with classes? (like potatoo, it doesnt do anything for melee, etc)

### items

* hats
 discord hat, obtain by joining the discord (discord rpc fix me)
 night vision hat
 Diving hat
 A flipped bucket
 glowing hat (glowing mushroom)
 tinfoil hat
 headless
 null texture
 terminal head
 hat trader hat
 
* animated hats
* separate hat sprite for when you duck

* kills random enemy (except boss)
* tps random enemy to another room (except boss)
* discord rod should damage everything it tourches from point where you were to where you tp
* existing orbitals should be smaller
* orbital, that deals damage to enemies
* torch, has a good light radius, emits fire particles (you drop it if you get hit)
* map, reveals all tiles
* boomerang (or throwing axe)
* sword that shoots projectiles, when you are full hp
* item, that scales you damage to health (negative and positive one)
* ball on chain, melee weapon (or passive item?), drags behind you, you spin it around with movement to hurt enemies
* item that makes your bullets warp through screen
* marshmallow orbital, can be light up
* battery buddy
* d1
* d4
* d2
* d8
* d24
* pig: gives you more money the deeper flor you use it on (single use active item)
* matches (sets you on fire)
* item that makes bombs explode faster
* sharp blade
* dull blade
* stick bombs
* homing bombs
* wallet buddy (collects money)
* roll damages enemies
* teleport to random room
* teleport to prev room
* egg: spawns a familiar after a while?
* egg: needs to be cracked
* there are options
* curse of unknown items

#### Polish

* Vegan mode: press 20 times to unlock all content?
* bones and props in the walls
* Broken variant for walls that do not break
* Explosion dust in the whole explosion radius, goes from black to white and floats up super fast
* Animate items/creatures falling into the chasm?
* Add sparks
* Think about blur around edges
* Reflections in the water
* Splashes on the water
* Walls after explosion should be "a bit more broken?"
* rework lava, should be like water in chasm or smth
* Props that react to music beats

### Mechanics

* Each level should have its own mechanic, except the starting one (running out of water, turn based movement, etc, allow to choose different roots)
* Explode item stand to replace the item with trash? :thinking:

### Ideas

* fire trap, could use a simular sprite to pd/spikes, emits fire when you stand on it, some just emit on timer
* cool bonus for killing bk without dropping a single white chunk (health chunk, aka non stop kill)
* make bk himself sign!!!!
* Random events (rain, blood moon, etc), ways to make them more or less likely
* Signs, you can write/read from them
* Hub house: long church like building with a huge door in the end, and statues for achievements. Door opens when you finish all achievements. Might also have 1-2 more doors for some amount of achievements complete. Unlocks a new area, maybe new npcs and items
* item combo -> transformation
* use cota tracks as disks to feature in the game and change in the gramaphone
* bee hive room in the jungle (has tons of bees inside, has a special wall (wall b textured as honeycomb), maybe even with a miniboss?)

### enemies

* enemy that deals insane knockback to you
* enemy with insane knockback modifier
* skull, that shoots missiles, that go through wall (terraria dungeon)
* (can be with thief sprite) caster, that shoots 3 penetrating the walls missiles, then tp's (terraria dungeon)
* buffer (etg)
* enemy with waterbolt?
* one, that just runs around all the walls and is unkillable (terraria dungeon)
* man eater (wine plant, but in jungle??)
* man eater, but on chain? for dungeon
* snipper (nt)
* enemy, that moves as insane as sk, but not as much

+ slime that shoots to you, when lands
+ burning slime
+ maggot on the floor
+ maggot nest: spawns maggots?
+ scorpion
+ worm: pops up, waits, fires bullet hell at you, hides, goes to another place, repeat
+ turtle

* Zelda like digging enemy ??
* bee hive and bee for forest biome
* Diagonal fly

* dragon (has a sprite)
* thief (has a sprite)
* skeleton (has a sprite)
* king servant?? (has a sprite)
* knight??? (has a sprite)

#### NPCs

* https://twitter.com/MateCziner/status/1107173510877720577
* https://twitter.com/128_mhz/status/1107158705772978176
* trash goblin (gaz)
* elon (wizard, has a sprite)

### Rooms

* Challenge room
* Cursed room
* alg for connection rooms that makes a really maze like path but with no dead ends
* take one of items treasure room (how does that work with mimics, tho??)
* rooms with preasure plates
* rooms with switches
* trap rooms