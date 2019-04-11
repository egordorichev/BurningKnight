# TODO

### Important

* Hide secret and unexplored rooms
* Make secret rooms bombable
* Make mobs get removed after death
* Fix connection rooms with 1 door
* Solid body component for chest
* Guns should not collide with items (bullets)
* Fix chest without lock not interacting
* Fix bug with 0 size level

* Work out room layout for hub
* Allow to place entities with editor
* Allow to remove entities with editor
* Loop and castle builders
* Rooms that can spawn only in certant biomes
* Debug command to show room type

* Show item description, when you pick them up [UiItemDescription]

!!!
* Path finding
* Mobs do not get removed after death
!!!

* Cap max hearts on player
* Shadows (cool)
* Animate gaining hearts from the lamp
* Move all text to ui layer, so that it's smaller/can be at higher resolution (also move lamp pickup hearts there)
* Pause the game, while console is open, but do not bring pause menu up?
* Implement settings
* Missing texture slice
* Set game icon

### Polish

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