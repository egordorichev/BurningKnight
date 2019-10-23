# bug && feedback update

* whuuuut, bk can be a metroid vania now???:  * the artifact give you access to some stuff, that you didnt have access to, but bk is on your tail

* check, what happens, when you die in tutorial

* bk:
 * attacks
 * the entrance room also has an exit, point players face to it, so that they know what to do

* Implement basic loop
* Allow to use images in ui string, get some gamepad and keyboard buttons going

* still has chasms in lvl 1
* explosion leftovers should be rendered under the chasm
* fixme: betatesters wont get the new key bindings added
* Ui hearts are not noticable
* Hold melee weapon to throw it 
* Enemies should only spawn when player enters, and get removed, when you to away
* rework dialog look? dialog system with good old popup like in old versions of bk for long dialogs?
* seed input remove/rework, it doesnt affect the run?
* Props in shop can block exit

* we need inventory in pause menu, like in revita
* player can respawn without a weapon or with op weapon from last run
* +hp up when items mod smth your stat on your head
* windows has two builds on steam?
* make enemies items and players fall into chasm
* reflecting bullets should give them a high speed

# TODO

* Tiny autoaim, but disable it in the settings
* get discord npc somewhere to spawn
* Hints on loading screen
* make music update run in a separate thread
* throwing music discs as weapons
* resprite bk
* curse getting animation

* curse of unknown (do not know hp)
* curse of lost (rooms get forgotten)   
* curse of ??? (do not know consumable counts)
* curse of ??? (items hidden)
* more prefixes

* discord rpc where??
* items that give curse
* item that makes your bullet size change over time (both ways (2 items))

* player stats:
 + use rate
 + speed
 + range mod


* troll bombs
* buffs display on mobs (like poisoned, charmed)
* hitting frozen enemy should let it slide a lot more around the room than usual
* Enemies / rooms drop rewards (bombs, keys, coins)

* draw the duck frames for hats
* mother slime body is not removed!!!!

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

## audio

* switching between levels (dark out) should fade in/out music
* hide music disks around
* lowpass filter and somewhat responsible music? :(

## general

+ lava:
 * tile lighting: figure it out
 * dont forget little bursts of light for guns and stuff
 * lava should be drawn over the shadows
 * particles for lava like in pd
 * fix lavafall colors
 * fix fire particle changing the render state so much

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

* implement stats
* lighting bolts
* figure out what to do with classes? (like potatoo, it doesnt do anything for melee, etc)

### items

* sword with projectiles
* sword with projectiles only when full hp
* item, that makes bomb explode on touching enemy
* brain (slow down enemy bullets near you)
* shield weapon (hurting/not hurting variants)
* single use item, that kills all enemies
* the ring
* ring of gold
* wings
* laser pointer
* star
* zoom
* poison, charisma (minor sale), charm rings
* Mines
* your bullets break enemy bullets

* weapons, that are not super good, but insta kill some enemy (for example, gunner drops gunner wand, it insta kills gunners, if you hit them with it)

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
 
* double the items on floor
* animated hats
* separate hat sprite for when you duck

* makes bomb explode really soon
* kills random enemy (except boss)
* tps random enemy to another room (except boss)
* discord rod should damage everything it tourches from point where you were to where you tp
* existing orbitals should be smaller
* orbital, that deals damage to enemies
* map, reveals all tiles
* boomerang (or throwing axe)
* sword that shoots projectiles, when you are full hp
* item, that scales you damage to health (negative and positive one)
* ball on chain, melee weapon (or passive item?), drags behind you, you spin it around with movement to hurt enemies
* item that makes your bullets warp through screen
* marshmallow orbital, can be light up
* item, that gives your items autouse 
* battery buddy
* d1
* charms
* d2
* ethernal d6: d6 with less charge needed but has chance to remove the item xd
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

* secret seeds
* tiles from isaac, that require key to open (basically have doors in some rooms, with pickups behind em)

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

* enemy, that reflects your bullets with melee arc
* enemy, that stands still, shoots laser at you and gets pushed back

* enemy, that dies from you touching it, does no contact damage, 
 runs away from you and shoots, tons of hp (hard to kill without touching)
 
* insanly hard enemy, that doesnt spawn, it's a statue at first. but if the statue breaks, the enemy appears (like it was a stone statue, but unstoned) and kicks your butt
 
* creeper
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

### Rooms

* Room, where you press lever/button to open/close chasm
* Challenge room
* Cursed room
* rooms with preasure plates
* rooms with switches
* trap rooms
* room, where you have an item blocked by bricks (aka chasms, but you cant roll through em), from isaac. has a small chance to spawn tho (pretty annoying), has no enemies

### things I promissed to do

* secret seeds
* daily runs
* local coop up to 4 players
* twitch support
* steam cloud