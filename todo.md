# TODO

* !!! secret rooms seem to corrupt level sometimes
* !!! sometimes generation can fail, then go ok, game appears, but generation is still going in bg :OOO

* a way to sort imgui windows
* when creature dies, its not visible, seems like regular hit
* room cleared effect
* halo gives 2 heart containers, but rendering seems wrong (it doesnt show up like a full heart)
* remove lamp mechanics?
* mimic attacks
* shop keeper ai
* juice up pistons (entity + tile mix for rendering, we need inbetweens for sure)
* signs of where bk room is
* special tiles for bk room?
* show seed in pause menu
* make lamps breabkable (and improve them, as suggested in discord)
* break statue to skip bk, dont spawn it anymore
* menu screen needs shadows
* redo bullet death animation: pause, flash white for a split second, then gone

* castle 2 mobs:
 + slime that shoots to you, when lands
 + burning slime
 + bandit mutant: homing missiles?

### rooms

* take one of items treasure room (how does that work with mimics, tho??)
* rooms with preasure plates
* rooms with switches
* trap rooms

### beta bugs

* openal crash
* max hp resets
* bk missile still has no sprite???

### todo

* shields
* game intro
* maggots are boring
* sparks

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
 
* lil boo
* gases

* animation data on when player head moves up and down 
 * move his weapon with it
 * introduce single sprite hats

* NPCs:
 * shopkeeper (spelunky)
 * granny
 * oldman smth?

* Room improvements/ideas:
 * Use lava in the terrain
 * trap only room
  + trap controllers

### on windows

* darkness shader should be applied to all screen and not just game
* get treasure music in

### bk himself

* Learning to avoid the last attack that killed him
* way to skip the boss battle but get harder game
* difficulty progression through the fight
* Learning to avoid the last attack that killed him
* slime pattern
* bouncy pattern -> another transformation?

* Boss door
* Healthbar art

* EEEEEEEEEEEEEEEEEEEEEPIC intro
 * more epic stuff in the spawn trigger, some fire or smth, dissappears with the start of the shake
 * cool particles when tile gets removed
 
* make player unhittable and remove projectiles/enemies from the room on bk death?

### Before josh

* wip sign and placement
* steam cloud

* !!! treasure room design
* bk missile projectiles have no sprite??? wrong depth, maybe?

### can wait

* nicer preboss room
* room rewards (batteries and stuff)
* implement stats
* push hearts and batteries and coins around, if you cant have more
* unlockable items
* iron hearts, half a heart spawning, etc
* update art for chest
* Animate lamp up and down to make it more alive
* Crash report window
* stats window for multiple file
* selling items?
* explode gold tiles for coins
* settings

### items

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

### fixes

* camera follow items that are on screen a bit?
* item can make some items less/more luckyly to spawn
* items that can spawn only if you dont have em
* save orbitals
* Animate enemy death
* ^^^^^^^ Effects when player is low hp
* ui description banner bg
* lighting bolts

### Before v0.0.0.4

* figure out what to do with classes? (like potatoo, it doesnt do anything for melee, etc)
* Enemies / rooms drop rewards (bombs, keys, coins)
* fix nodes requiring to hit enter to set their name

### Important

* Zelda like digging enemy
* Diagonal fly
* Work out room layout for hub
* Allow to place entities with editor
* Allow to remove entities with editor
* Rooms that can spawn only in certant biomes
* noise wall room (only possible with path finding)
* Implement settings

#### Polish

* bones and props in the walls
* Broken variant for walls that do not break
* Explosion dust in the whole explosion radius, goes from black to white and floats up super fast
* Weapons in player hands should drop shadows
* Wall shadow of player should include weapons?
* Low health indicator
* Animate items/creatures falling into the chasm?
* Flash frame for guns
* Make bullets fly out from the right place
* Add sparks
* Think about bloom shader
* Think about blur around edges
* Gore
* Reflections in the water
* Splashes on the water
* Walls after explosion should be "a bit more broken?"
* rework lava, should be like water in chasm or smth
* Props that react to music beats

### Mechanics

* Each level should have its own mechanic, except the starting one (running out of water, turn based movement, etc, allow to choose different roots)
* Explode item stand to replace the item with trash? :thinking:

### Ideas

* cool bonus for killing bk without dropping a single white chunk (health chunk, aka non stop kill)
* make bk himself sign!!!!
* Random events (rain, blood moon, etc), ways to make them more or less likely
* Signs, you can write/read from them
* In-game settings (in lobby) with a sign "We also have normal settings, if you press esc..., but who needs them!?"
* Hub house: long church like building with a huge door in the end, and statues for achievements. Door opens when you finish all achievements. Might also have 1-2 more doors for some amount of achievements complete. Unlocks a new area, maybe new npcs and items
* item combo -> transformation
* use cota tracks as disks to feature in the game and change in the gramaphone

### enemies

* bee hive and bee for forest biome

#### NPCs

* https://twitter.com/MateCziner/status/1107173510877720577
* https://twitter.com/128_mhz/status/1107158705772978176
* trash goblin (gaz)

### Special rooms

* Challenge room
* Cursed room
* Room with transmutation well (current weapon slot)
* alg for connection rooms that makes a really maze like path but with no branches