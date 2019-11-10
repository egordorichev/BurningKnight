# esty stuff

* fade out projectile when it dies, or maybe scale
* implement speed and fire rate

* portal

* give item tag to chests

* spawn chests/pickups in the levels
* change how black bars work, all of that ingame ui that is pause stuff should render as a second time ui layer (and stop hiding ui elements if the game is just paused)
* teleportation animation on player
* special room just with a vending machine

* !! you can start with no weapon, if you continue run, you have no weapon (same with active item?)
* !! game sometimes doesnt exit even after window is closed
* keyboard controls dont show up if you have gamepad connected (show both)
* enemy bullets are hard tell from player bullets, in desert they are hard to spot, cause floor is red too
* light up room after finishing it? overall, some room cleared effect (maybe like a torch that you light up when everyone ded)
* some effects when you clear the whole level
* props in the shop can be unaccessable due item stands being too close 
* animate mob spawning
* text on your head when you get +hp, etc, so that its all clear

* maps should not reveal secret rooms
* seed select with gamepad (use buttons a b x y or make a proper input dialog)

* second lvl deset enemies

* auto rotate projectiles after they bounce (stuff like arrows from amurs bow looks weird)

* fix laser pointer and nozzle
* look into maximized bug
* Allow to use images in ui string, get some gamepad and keyboard buttons going

* we need inventory in pause menu, like in revita
* player can respawn without a weapon or with op weapon from last run
* make enemies items and players fall into chasm

* dark mage/granny rooms

* duck interaction
* Implement basic loop
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

* bomb item should have a chance to ignire
* hitting frozen enemy should let it slide a lot more around the room than usual
* Enemies / rooms drop rewards (bombs, keys, coins)

## general

+ lava:
 * tile lighting: figure it out
 * dont forget little bursts of light for guns and stuff
 * lava should be drawn over the shadows
 * particles for lava like in pd
 * fix lavafall colors
 * fix fire particle changing the render state so much

* Low health indicator
* juice up pistons (entity + tile mix for rendering, we need inbetweens for sure)
* spice up bullets (different textures for different enemies)
* we need more secret room layouts
* shields
 
#### Polish

* special door textures for shop and treasure room, maybe even boss room 
* animate items on player head better, also dont stack em, display count 
* animate item pickup / appearance
* Broken variant for walls that do not break
* Explosion dust in the whole explosion radius, goes from black to white and floats up super fast
* Add sparks
* Reflections in the water
* Splashes on the water
* Props that react to music beats

### Ideas

* blue hearts, at least em

* teleport tile mechanic for some level
* breakable chests
* Unique saving experience for each NPCs (and save phrases)
* enemies/rooms/other stuff that doesn't happen/appear every run

* jungle rooms should be a tad smaller than the rest of the game?
* special secret music with 10% chance?
* a special reward for completing the level fast

* tiles from isaac, that require key to open (basically have doors in some rooms, with pickups behind em)
* fire trap, could use a simular sprite to pd/spikes, emits fire when you stand on it, some just emit on timer
* Random events (rain, blood moon, etc), ways to make them more or less likely
* Hub house: long church like building with a huge door in the end, and statues for achievements. Door opens when you finish all achievements. Might also have 1-2 more doors for some amount of achievements complete. Unlocks a new area, maybe new npcs and items
* item combo -> transformation
* bee hive room in the jungle (has tons of bees inside, has a special wall (wall b textured as honeycomb), maybe even with a miniboss?)

### Rooms

* falling floor room
* minigame rooms from rogue legacy
* crawled space
* Room, where you press lever/button to open/close chasm
* Challenge room
* Cursed room
* trap rooms
* more secret rooms
* room, where you have an item blocked by bricks (aka chasms, but you cant roll through em), from isaac. has a small chance to spawn tho (pretty annoying), has no enemies
* new secret room designs

* secret level ith tons of shopkeepers, aka dark market from spelunky

### things I promissed to do

* angel/devil rooms, I dont like the theme tho, rename into smth else
* secret seeds
* daily runs
* local coop up to 4 players
* twitch support
* steam cloud

### Machine Ideas

* Takes two weapons gives new 
* Buys weapons from you
* Health chest, can be mimic 
* Rework well into different structs