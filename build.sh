cd ~/Lens/BurningKnight/Content/
"C:/Program Files (x86)/MSBuild/MonoGame/v3.0/Tools/MGCB.exe" "Test2.mgcb" /platform:Windows 
cd ~/Lens/
"C:/Program Files (x86)/Microsoft Visual Studio/2017/Community/MSBuild/15.0/Bin/MSBuild.exe" ./Lens.sln /property:Configuration=Release
cd ~/Lens/
## cd ~/Lens