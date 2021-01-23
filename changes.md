* Reworked the whole projectile system
* Blue bullet slime doesn't shoot after death anymore
* Dino is a bit more angry now
* Halo buffed

* Projectile light/poof effects wont work after there are 99 projectiles in game
* Mob projectiles are capped at 199
* Removed Tags.Torch => Tags.MobProjectile
* Removed Tags.Gramaphone => Tags.PlayerProjectile 
* Projectiles wont be able to hit mobs/paintings/tnt in rooms that there are no players in
* All projectiles will die after some time now
* The game will give up trying to process past time if it is more than 0.25 of a second
* If player has more than 69 projectiles old projectiles will start to break
* Improved missile
* Added bounce cap of 8
* Killing shopkeeper gives you 3 scoure points now
* Boss bullets will become scourged on 1st loop
* Enemy bullets will become scourged on 2nd loop
* Shopkeeper mood wont change if you are not in his room
* Added "always cook second" to the loading screen jokes
* Death/win screen will now have a note if the run was seeded
* The player now actually drops all the items he collected on the run when dying (it was meant to work like that forever but was broken)
* Some movement speed tweaks
* Player sprite is now tilted slightly while moving
* Ctrl+V works on beetroot now
* Click to place bombs with bomb lamp
* Removed aura from not reflectable projectiles
* Added battery sfx
* Fixed black bars not resizing (aka ui target not updating its size)
* Mobs killed with explosive lamp will drop bombs on death
* Unhitatble creatures wont get buffs from projectiles
* Hopefully fixed ice level bug (rude but still)