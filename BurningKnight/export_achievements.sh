cp Content/Animations/achievements.ase .

rm -rf achievement_sprites
mkdir achievement_sprites
aseprite -b achievements.ase --scale 4 --save-as achievement_sprites/{slice}.png

cd achievement_sprites
mkdir gray

for file in ./*
do
  convert "$file" -colorspace Gray "gray/$file"
done

cd ..
ls -l achievement_sprites
rm achievements.ase
