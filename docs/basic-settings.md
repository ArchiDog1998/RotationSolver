# Basic Usage

It is always good to know or control the `Rotation Solver`. These are several tips for you.

## Macros

After you have installed `Rotation Solver`, you should first check the help tab within the settings window in order to figure out the macros the addon can use.

This will show you the basic Auto-Manual-Cancel macros that you can easily copy using the right click so you can paste them into a macro button to set up on your hotbar along with other sub macros that you can use during the Auto to force Rotation Solver to do things.

You can also with these commands along with the [QolBar](https://github.com/UnknownX7/QoLBar) addon.

![Macro Helps](assets/image-20230227112230914.png)

There is a description of the different macros and their usage if you wish to use them. During the Auto usage, the addon does all of them automatically except anti-knockback and raise when Swiftcast is on cooldown.

Macros are divided into two categories. One is permanent while the addon is active and the other is for a set duration within that.

### State

This kind of macro will set a no-time-limit state. It contains three macros.

| Macro            | Description                                                  |
| ---------------- | ------------------------------------------------------------ |
| /rotation Auto   | Start the addon in Auto mode (auto-targeting) when out of combat or when combat starts, otherwise switch the target according to the set condition. |
| /rotation Manual | Start the addon in manual mode. You need to choose the target manually. This will bypass any  Hostile target filtering conditions that you have set up and will start attacking immediately once something is targeted. |
| /rotation Cancel | Stop the addon. Always remember to turn it off when not in use! |

### Special Duration

This type of macro will activate a window for some special action to use. The duration of it is set in the `Param` – `Basic` tab.

| Macro                      | Description                                                  |
| -------------------------- | ------------------------------------------------------------ |
| /rotation EndSpecial       | To end this special duration before the set time.            |
| /rotation HealArea         | Open a window to use AoE heal.                               |
| /rotation HealSingle       | Open a window to use single heal.                            |
| /rotation DefenseArea      | Open a window to use AoE defense.                            |
| /rotation DefenseSingle    | Open a window to use single defense.                         |
| /rotation MoveForward      | Open a window to move forward.                               |
| /rotation MoveBack         | Open a window to move back.                                  |
| /rotation EsunaStanceNorth | Open a window to use Esuna, tank stance actions or True North. |
| /rotation RaiseShirk       | Open a window to use Raise or Shirk.                         |
| /rotation AntiKnockback    | Open a window to use knockback-penalty actions.              |
| /rotation Burst            | Open a window to burst.                                      |



