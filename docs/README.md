# RotationSolver⚔️

> Analyses PvE combat information every frame and finds the best action.

This means almost all the information available in one frame in combat, including the status of all players in the party, the status of any hostile targets, skill cooldowns, the MP and HP of characters, the location of characters, casting status of the hostile target, combo, combat duration, player level, etc.

Then, it will highlight the best action on the hot bar, or help you to click on it.

It is designed for `general combat`, not for savage or ultimate. Use it carefully.

## Installation

After installing [Dalamud](https://goatcorp.github.io/), input `/xlplugins` command  in the chat box. Go to`Settings` -> `Experimental` -> `Custom Plugin Repositories`, and add the URL here, check the box and in the end, save it.

```
https://raw.githubusercontent.com/ArchiDog1998/Dalamud_Plugins/main/pluginmaster.json
```

![Add Url](assets/image-20230129154207892.png)

Go back to the Installed Plugins. Find the `Rotation Solver` in the `All Plugins` list, and install it.

![Install Plugin](assets/image-20230129155343199.png)

## Quick Start

Open the `Rotation Solver Settings`, Find `Help` Tab, and left click the `/rotation Auto` to start the rotation. Then it is done.

![Start the Rotation](assets/image-20230129160200852.png)

What? Nothing happened? Fine. If your job is not a `Tank` role, Rotation Solver only attacks the mobs that have already been engaged by default. So, you need to right-click the target into `Auto Attack`  mode to make the target engaged in combat with your character.
