# publisher build stuff

* fix {
 + bugs {
  - maximized window bug on windows
  - game sometimes doesn't exit after closing
 }

 + visual {
  - some weirdness is going on with bullet death particles?
  - screenshake is trash
  - the weapons go crazy when you point directly up or down
  - doors are hard to notice
 }

 + tutorial {
  - make the door after the sword a wall type, that you need to break by using your sword on it
 }

 + you can roll out of the idol room
 + remove autodeath from wallcrawlers but make sure rooms dont spawn where you cant access walls or they wont spawn in it

 + gamepad {
  - check on windows, this might be linux issue?
  - gamepad doesn't seem to function properly without hotplugging it
  - rumble doesn't always work
 }
}

* room {
 + special props on top of special rooms, like shop boss and treasure
 + cleared effect
 + clear reward. Spawns in the center, destroying the tiles. 
}

+ make bk not float in the center of actions in the rooms
+ charge up animation for old king before he spews out the heads

* change desert gen to be more like archvale and nt, open?

* bk: {
 + better into boss going anim
 + better getting out of boss
 + fix how saving works with capturing ths bosses
}

* ui {
 + change how black bars work, all of that ingame ui that is pause stuff should render as a second time ui layer (and stop hiding ui elements if the game is just paused)
 + player should say "Daddy? What did they do to you?!!" when he sees the tombstone
}

* audio {
 + bk fire sound (when near)
 + slow down music when you die
}

+ hub {
 + allow to layer in stuff for speical events
 + lock away the rest of the hub on the first time you enter it after tutorial?
}

* twitter {
 + showcase desert area
 + showcase granny/dm rooms
}

##


* spawn chests/pickups in the levels

* duck interaction
* Implement basic loop after you defeat the boss
* Hints on loading screen  
* curse getting animation

* curse of unknown (do not know hp)
* curse of lost (rooms get forgotten)   
* curse of ??? (do not know consumable counts)
* curse of ??? (items hidden)
* more prefixes

* Enemies / rooms drop rewards (bombs, keys, coins)

* dog or some pet, that you can pet, send to can you pet the dog tweet account

## general

* lava: {
 + tile lighting: figure it out
 + dont forget little bursts of light for guns and stuff
 + lava should be drawn over the shadows
 + particles for lava like in pd
 + fix lavafall colors
}

* Low health indicator
* juice up pistons (entity + tile mix for rendering, we need inbetweens for sure)
* shields

#### Polish

* special door textures for shop and treasure room, maybe even boss room 
* animate items on player head better, also dont stack em, display count 
* animate item pickup / appearance
* Broken variant for walls that do not break
* Explosion dust in the whole explosion radius, goes from black to white and floats up super fast
* Add sparks
* Splashes on the water
* Props that react to music beats
* player should go up (like jumping arc) when he is rolling

### Ideas

* disco floor (secret location or just secret room?)
* blue hearts, at least em

* teleport tile mechanic for some level
* make chests brekable (the ones that you dont have a key to open)
* Unique saving experience for each NPCs (and save phrases)
* enemies/rooms/other stuff that doesn't happen/appear every run

* jungle rooms should be a tad smaller than the rest of the game?
* special secret music with 10% chance?
* a special reward for completing the level fast

* tiles from isaac, that require key to open (basically have doors in some rooms, with pickups behind em)
* Random events (rain, blood moon, etc), ways to make them more or less likely
* Hub house: long church like building with a huge door in the end, and statues for achievements. Door opens when you finish all achievements. Might also have 1-2 more doors for some amount of achievements complete. Unlocks a new area, maybe new npcs and items
* item combo -> transformation
* bee hive room in the jungle (has tons of bees inside, has a special wall (wall b textured as honeycomb), maybe even with a miniboss?)
* lore through shopkeeper (like in messager)?

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

* secret seeds
* daily runs
* local coop up to 4 players
* twitch support
* steam rpc

### Machine Ideas

* Takes two weapons gives new 
* Buys weapons from you
* Health chest, can be mimic 
* Rework well into different structs
* machine: gives random stat up
