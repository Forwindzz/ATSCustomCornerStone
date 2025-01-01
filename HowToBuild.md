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
    <SteamPath>Your steam path</SteamPath>
    <StormPath>ATS Game Path</StormPath>
    <BepInExPath>Mod Profile path</BepInExPath>
  </PropertyGroup>
</Project>
```

Here is an example:
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

You can run it with Steam, but I am not clear about how to attach the debugger...

# About Code

There are some customized code that may not focus on compatitablity.
I also write my own modules, that independent from API (which might move to API in the future)
It is not recommend to use my mod as a library, please suggest the API to add corresponding functions \_(:з」∠)\_

Code Aspect:
- Custom Save system
- Custom Service system
- Custom Availability for conerstone

Compatibility information: 
This mod may break compatibility, 

- **Set Operation**: directly set the value: `=` `Set()`. This will break compatibility if other mod also set this value
    - Modify `GoodModel.eatable` value
	- Modify Trader cornerstone price 

- **Relative**: modify based on the existing value: `+=` `-=` `*=` `/=` `Add()` `Remove()`). This will break compatibility if other mod use set to change the value. But it could be compatitable if they use relative method to update the value.
	- Modify Hearth upgrade requirement 
	- Modify `DecorationModel`: Decoration points 
    - Modify `RelicModel`: Tier effects 

Also you may encounter some meaningless code, the project are based on this repository:
(Shush's First ATS mod)[https://github.com/Shushishtok/AtS-my-first-mod]
I might remove them later...