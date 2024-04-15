
# [![](Images/Logo.gif)](https://archidog1998.github.io/RotationSolver/#/) **Rotation Solver**

![Github Latest Releases](https://img.shields.io/github/downloads/ArchiDog1998/RotationSolver/latest/total.svg?style=for-the-badge)
![Github All Releases](https://img.shields.io/github/downloads/ArchiDog1998/RotationSolver/total.svg?style=for-the-badge)
![](https://img.shields.io/codefactor/grade/github/ArchiDog1998/RotationSolver?longCache=true&style=for-the-badge)
![Github License](https://img.shields.io/github/license/ArchiDog1998/RotationSolver.svg?label=License&style=for-the-badge)
![Github Commits](https://img.shields.io/github/commits-since/ArchiDog1998/RotationSolver/latest/main?style=for-the-badge)

It's time to learn something new, this plugin will be available at this link on the game version 7.0!

```
https://raw.githubusercontent.com/ArchiDog1998/DalamudPlugins/main/pluginmaster.json
```

> [!IMPORTANT]
>
> My friends have suggested not to make this plugin fully open source, so the base library will no longer be fully public. 
>
> If you want to see the source code, please support me in ko-fi and send me a private message on your GitHub mail!

Three ways to get it:
- Try the community version of [Rotation Solver Reborn](https://github.com/FFXIV-CombatReborn/RotationSolverReborn) (not mine). 
- Build by yourself on the release tap.
- Buy the dev version for $2 in [ko-fi](https://ko-fi.com/s/7cf5ff0de3) to help you to build.


## Brief

> Analyses combat information in every frame and finds the best action.

This means almost all the information available in one frame in combat, including the status of all players in the party, the status of any hostile targets, skill cooldowns, the MP and HP of characters, the location of characters, casting status of the hostile target, combo, combat duration, player level, etc.

Then, it will highlight the best action on the hot bar, or help you to click on it.

It is designed for `general combat`, not for savage or ultimate. Use it carefully.

## Compatibility

literally, `Rotation Solver` helps you to choose the target and then click the action. So any plugin that changes these will affect its decision. 

- [XIVCombo](https://github.com/daemitus/XIVComboPlugin)
- [ReAction](https://github.com/UnknownX7/ReAction)
- etc...

NOTICE: It can't use with [`Block Targeting Treasure Hunt Enemies`](https://github.com/Caraxi/SimpleTweaksPlugin/blob/7e94915afa17ea873d48be2c469ebdaddd2e5200/Tweaks/TreasureHuntTargets.cs) in [Simple Tweaks](https://github.com/Caraxi/SimpleTweaksPlugin). 

I don't know why. I just used the [GetIsTargetable](https://github.com/aers/FFXIVClientStructs/blob/c554a586c4649a472433734b45c59a4bc4979ead/FFXIVClientStructs/FFXIV/Client/Game/Object/GameObject.cs#L71) Method in [FFXIVClientStructs](https://github.com/aers/FFXIVClientStructs). If anybody knows why, please tell me.

## Want to contribute?

- Create a fork
- Make your changes
- Test the changes
- Create a PR and point it to main

## How to build

- Build `RotationSolver.SourceGenerators`
- Restart your IDE.
- Build the whole solution!

## Links

If you have any questions about usage, please check the [Wiki](https://archidog1998.github.io/RotationSolver/#/).

The rotations definitions are [here](https://github.com/ArchiDog1998/FFXIVRotations).

[![Crowdin](https://badges.crowdin.net/badge/light/crowdin-on-dark.png)](https://crowdin.com/project/rotationsolver)

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/B0B0IN5DX)
