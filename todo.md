# TODO

### FIXMEE!!

* No lighting
* Fix interacting with dialogs (and think what to do with input block?)
* Make node editing possible in-game
* Fix -1 level madness

### Before v0.0.0.4

* Item editor
* Fix player spawns
* Dialog for old man
* Dont let the player exit until he has a lamp
* Put lamps and weapons in hall
* Old man -> bk when you take a lamp

* Show item description, when you pick it up [UiItemDescription]
* Animate bullet death
* Implement coins
* Proper animation for gaining an item (for gobbo.ase obv)
* When player uses active item, animate him using it (like in isaac)

* BK him self
 + Healthbar
 + Animation and spawning
 + Finishing room hits him
 + He spawns the enemies
 + Player picking up hearts will heal bk
 + Opening and buying heals bk
 + Somehow make him a bit annoying and scary?? But also super cool to fight, you should feel like not worthy, until you get well equipped
 
* Fix player reading 1 byte less than expected (some component is broken? or maybe not added) (and other entities, like a lot in prefabs!!)

* Animate enemy death
* ^^^^^^^ Effects when player is low hp

* Enemies / rooms drop rewards (bombs, keys, coins)

* ^^^^ same for picking up item

* Figure out lamp mechanics, display it on player
* ^^^^^^ whats up with Burning Knight? gotta think a lot

* Add PICO-8 prototype room
* Make WIP sign
* Add at least one NPC house

* Add a few weapons for melee, magic and ranged
* Add a few artifacts

* Implement speed changing
* Audio speed should depend on engine speed

* Crash report/fail loading stuff

* Ui banner (level depth report and stuff)

#### Polish

* Explosion dust in the whole explosion radius, goes from black to white and floats up super fast
* Weapons in player hands should drop shadows
* Wall shadow of player should include weapons?

### Important

* Gobbo tombstone impl (appears on next run too, gives random item)
* Zelda like digging enemy
* Diagonal fly
* Broken variant for walls that do not break
* Props that react to music beats
* Work out room layout for hub
* Allow to place entities with editor
* Allow to remove entities with editor
* Rooms that can spawn only in certant biomes
* Debug command to show room type

!!!
* ask dad for neural net for generating room layouts
!!!

!!!
* noise wall room (only possible with path finding)
!!!

* Cap max hearts on player
* Animate gaining hearts from the lamp
* Move all text to ui layer, so that it's smaller/can be at higher resolution (also move lamp pickup hearts there)
* Pause the game, while console is open, but do not bring pause menu up?
* Implement settings

### Polish

* Low health indicator
* Animate items/creatures falling into the chasm?
* Animate healing
* Wall a -> wall b tiling
* Far away tiles (darker version of the wall)
* In wall decor, like nuclear throne
* Water / liquid shader still has a 2-3 frames of other liquid overlapping
* Item pickup fx convert to -> "+ item name (count)"
* Flash frame for guns
* Make bullets fly out from the right place
* Bullet shells
* Add sparks
* Think about bloom shader
* Think about blur around edges
* Blood
* Gore
* Show black sprite over obstacles, when player view is blocked by something?
* Adjust door locks, they are a bit weird
* Animate hearts in ui
* Reflections in the water
* Splashes on the water
* Walls after explosion should be "a bit more broken?"
* Creatures break high grass, when they walk over it
* Breaking high grass should emit particles (might drop seeds?)

### Mechanics

* Each level should have its own mechanic, except the starting one (running out of water, turn based movement, etc, allow to choose different roots)
* Add health component to chest
* Add explodable component to chest?
* Explode item stand to replace the item with trash? :thinking:
* Level interaction
 + Falling into chasms (items, enemies)
* Chest react to explosion/hitting

### Ideas

* Random events (rain, blood moon, etc), ways to make them more or less likely
* Signs, you can write/read from them
* In-game settings (in lobby) with a sign "We also have normal settings, if you press esc..., but who needs them!?"
* Hub house: long church like building with a huge door in the end, and statues for achievements. Door opens when you finish all achievements. Might also have 1-2 more doors for some amount of achievements complete. Unlocks a new area, maybe new npcs and items

#### NPCs

* https://twitter.com/MateCziner/status/1107173510877720577
* https://twitter.com/128_mhz/status/1107158705772978176

### Special rooms

* Challenge room
* Shop room
* Cursed room

### Clean up

* Move BurningBuff.Id somewhere else (other name), cause it hides Id field from Buff class
