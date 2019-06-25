cd ~/Lens/BurningKnight/Content/
"C:/Program Files (x86)/MSBuild/MonoGame/v3.0/Tools/MGCB.exe" "Test2.mgcb" /platform:DesktopGL
cd ~/Lens/
"C:/Program Files (x86)/Microsoft Visual Studio/2017/Community/MSBuild/15.0/Bin/MSBuild.exe" ./Lens.sln /property:Configuration=Release
# ~/Downloads/butler.exe push ./Desktop/bin/Release/ egordorichev/bkd:windows

# cd ~/Lens/BurningKnight/Content/
# "C:/Program Files (x86)/MSBuild/MonoGame/v3.0/Tools/MGCB.exe" "Test2.mgcb" /platform:MacOSX
# cd ~/Lens/
# "C:/Program Files (x86)/Microsoft Visual Studio/2017/Community/MSBuild/15.0/Bin/MSBuild.exe" ./Lens.sln /property:Configuration=Release
# ~/Downloads/butler.exe push ./DesktopMac/bin/Release/ egordorichev/bkd:mac

# cd ~/Lens/BurningKnight/Content/
# "C:/Program Files (x86)/MSBuild/MonoGame/v3.0/Tools/MGCB.exe" "Test2.mgcb" /platform:Linux
# cd ~/Lens/
# "C:/Program Files (x86)/Microsoft Visual Studio/2017/Community/MSBuild/15.0/Bin/MSBuild.exe" ./Lens.sln /property:Configuration=Release
# cp ~/Lens/DesktopLinux/Kick/* -rf ~/Lens/DesktopLinux/bin/Release/
# ~/Downloads/butler.exe push ./Desktop/bin/Release/ egordorichev/bkd:linux

# cd ~/Lens/