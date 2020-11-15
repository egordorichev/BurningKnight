# Burning Knight 

Available on [Steam](https://store.steampowered.com/app/851150/Burning_Knight/) and [itch.io](https://egordorichev.itch.io/bk).

Before you look at the code: hear me out. Yes, some of it is not the best. Yes, some of it can be redone and improved.
But I have to admit, I came so close to dropping this whole project so many times, I still have no idea, how I made it through May of 2020.
Anyway. Here it is. The source code of C# branch of Burning Knight. **For java branch see [this repo](https://github.com/egordorichev/BurningKnightJava).**

###### (Yes, this is only half of the work, the game was initially written in Java and then rewritten in C#, this code base is relatively nice compared to the Java one).

This project really quickly moved from the "nice and relaxing" to "constant bugfixing and stress" type of project.
You can clearly see me going mad in some places (do not look at the pull requests I beg you).
But its all in the past now. Sadly, the game has done really poorly on the Steam, even tho I've tried my best with marketing and everything else.
I'm still really really proud of the game, it's THE project I can point people to and say: I've made this. That's one of the reasons why I wanted to open the source of this monster.

There are not a lot of open source games, that have been released on Steam. So there is not that much where you can go to learn from real released games. Of course, you can decompile games, but it will never compare to the actual source.
I believe in the power open source. I'm opening this project up with hope, that it might turn out to be useful to someone. So yeah, forgive me my code decisions in some places.

Anyway, onto the juicy stuff.

The game has a lot of developer tools in it, but to gain access to them, you must build the solution in Debug configuration.
This is really important, because **music & sfx wont load in Debug configuration, but dev tools will be enabled**. The assets are dropped in favor of quicker loading time (there is still no hotswap support for Mono on Linux).

To access the dev tools, you shall press F1 while being in InGameState (while you are normally playing and not watching a loading screen).
A panel with a bunch of checkboxes should appear, that show different dev tools. Have fun!

##### Why did you merge pull requests into release branch all the time?

You see, this is how I've set up Github Actions CI. The tool went online in the middle of the first summer of development of the C# branch, and it was such a huge help.
Before that, I had to compile all the builds for beta testing by hand, but after 3 days of internal screaming I was able to get the CI working, and from that point I was able just to merge
my dev branch into release, and 10 minutes later press a few buttons on Itch/Steam to release the new builds.

##### Building

Hey, so I saw a bunch of people complain online about no building instructions. I couldn't be surprised more, since you just open the .sln file in your C# IDE of choice and compile & run the Desktop project. But just in case anyone is still curious, here you go.  

Or if you preffer to do it from the terminal: install the packages:

```bash
nuget restore
```

Debug configuration (disabled sfx & music but has dev tools enabled):

```bash
msbuild
cd Desktop/bin/Debug/
mono Desktop.exe
```

Release configuration (same as on Steam) is a bit more tricky. You gotta install MonoGame Content Pipeline tool and compile the assets (raw sfx -> .xnb).
If you are on Linux, you can get it via:

wget https://github.com/MonoGame/MonoGame/releases/download/v3.7.1/monogame-sdk.run
chmod +x monogame-sdk.run
sudo ./monogame-sdk.run
Otherwise, find a binary on the monogame website. After that open BurningKnight/Content/Content.mgcb in the tool and hit build.

![](https://user-images.githubusercontent.com/7851390/96409702-5cb22700-11ee-11eb-8586-0afe349f1473.png)

If you are on Linux, ignore shader compilation errors, prebuild shaders (via a Windows machine) are already in the repo.
After that build the sources and run:


```bash
msbuild /p:Configuration=Release
cd Desktop/bin/Release/
mono Desktop.exe
```
