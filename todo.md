# TODO

### Castle enemies
#### To sprite

* Dragon
* King servant

#### To implement

* Slime
  Jumps around, when sees player jumps to it. Might have a better version, that spits out bullets, when lands.
* King
  Decide AI, spawns king servants
* King servant
  Decide AI
* Dragon
  Slowly walks (or sits in one place), fires bullet hell
* Knight
  Decide AI
* Ranged knight
  Decide AI
* Bandit
  Wanders, shoots into you sometimes
* Wall slime
  Move variants, 3 shoot? aim shot? 3 in a row shot?
* Clown
  Gets close, plants a bomb, runs

### Important

* Death animations
* Broken variant for walls that do not break
* Explosion dust in the whole explosion radius, goes from black to white and floats up super fast
* redo wallB sprite (green?)
* lamp rendering
* weapons in player hands should drop shadows
* Start working on blood?
* Fix C and D tiles in castle, update dirt with less holes and more like its on top and not below
* Enemies / rooms drop rewards (bombs, keys, coins)

* Work out room layout for hub
* Allow to place entities with editor
* Allow to remove entities with editor
* Rooms that can spawn only in certant biomes
* Debug command to show room type

* Show item description, when you pick them up [UiItemDescription]

!!!
* ask dad for neural net for generating room layouts
!!!

!!!
* Path finding
* noise wall room (only possible with path finding)
!!!

* Cap max hearts on player
* Animate gaining hearts from the lamp
* Move all text to ui layer, so that it's smaller/can be at higher resolution (also move lamp pickup hearts there)
* Pause the game, while console is open, but do not bring pause menu up?
* Implement settings
* Missing texture slice
* Set game icon

### Polish

* Low health indicator
* Animate items/creatures falling into the chasm?
* Animate healing
* Wall a -> wall b tiling
* Far away tiles (darker version of the wall)
* In wall decor, like nuclear throne
* Water / liquid shader still has a 2-3 frames of other liquid overlapping
* Item pickup fx convert to -> "+ item name (count)"
* Animate item going to inventory
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

* Implement overworld
* Infinite overworld?
* Add health component to chest
* Add explodable component to chest?
* Explode item stand to replace the item with trash? :thinking:
* Remove half hearts?
* Breakable walls (and bedrock, then)
* Level interaction
 + Liquid collision
 + Falling into chasms (items, enemies, player)
 + Rolling over chasm
* Chest react to explosion/hitting

### Ideas

* Signs, you can write/read from them
* In-game settings (in lobby) with a sign "We also have normal settings, if you press esc..., but who needs them!?"

### Special rooms

* Challenge room
* Shop room
* Cursed room

### Clean up

* Move BurningBuff.Id somewhere else (other name), cause it hides Id field from Buff class

### Bugs

* Game will crash, if you die without saving
* Ui inventory sometimes wont show hearts