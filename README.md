# Against The Storm - My First Mod

This is my first attempt at a mod in Against The Storm.

This mod is based on Shush's Tutorial:
https://github.com/Shushishtok/AtS-my-first-mod/blob/master/Getting%20Started.md#changing-existing-stuff

Still in exploration and development, the code might be messy. 
I am new to Harmony 

There is still some codes from Shush's Tutorial, and it is not playable now. (I have not removed all of them, and still orgnize my code)

# Build

Requires:
- BepInEx
- API Mod 

Links of API Mod: [ThunderStone](https://thunderstore.io/c/against-the-storm/p/ATS_API_Devs/API/), [GitHub](https://github.com/JamesVeug/AgainstTheStormAPI)

Create file `Directory.Build.props` in the repository root

Set up your `Steam` `Against The Storm` `BepInEx` path like this:
```xml
<Project>
  <PropertyGroup>
    <SteamPath>D:\Program Files (x86)\Steam\steam.exe</SteamPath>
    <StormPath>E:/Program Files/Steam/steamapps/common/Against the Storm</StormPath>
    <BepInExPath>C:/Users/Cirno/AppData/Roaming/Thunderstore Mod Manager/DataFolder/AgainstTheStorm/profiles/Default</BepInExPath>
  </PropertyGroup>
</Project>
```

Then open it with `VS2022` and build it!

You can run it with Steam, but I am not clear about how to attach the debugger
