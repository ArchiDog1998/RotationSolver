# Rotation Information

So we need to fill the blank in the methods mentioned [before](RotationDev/rotation.md). But what data do we need to organize the [logic](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/boolean-logical-operators) with? First of all, there is the information in rotation.

## Player

| Property      | Description                                                 |
| ------------- | ----------------------------------------------------------- |
| Player        | This is the player.                                         |
| Level         | The level of the player.                                    |
| HasSwift      | Does player have swift cast, dual cast or triple cast.      |
| HasTankStance | Does player have grit, royal guard,  iron will or defiance. |



## Target

| Property      | Description                    |
| ------------- | ------------------------------ |
| Target        | The player's target.           |
| IsTargetDying | Shortcut for Target.IsDying(); |
| IsTargetBoss  | Shortcut for Target.IsBoss();  |



## Status

| Property | Description |
| -------- | ----------- |
| InCombat           | Is in combat.                                                |
| IsMoving           | Check the player is moving, such as running, walking or jumping. |
| HasHostilesInRange | Is there any hostile target in the range? 25 for ranged jobs and healer, 3 for melee and tank. |
| IsFullParty        | Whether the number of party members is 8.                    |
| InBurst            | Is in burst right now? Usually it used with team support actions. |



## Job Gauge

Job gauge is a little bit complex. And it depends on the certain job.

If there are some time stuffs, usually two methods are available.

| Method         | Description                                    |
| -------------- | ---------------------------------------------- |
| XXXEndAfter    | Is the thing still there after several seconds |
| XXXEndAfterGCD | Is the thing still there after several gcds.   |



## Record(Not Recommend)

| Property            | Description                                                  |
| ------------------- | ------------------------------------------------------------ |
| RecordActions       | Actions successfully released. The first one is the latest one. |
| TimeSinceLastAction | How much time has passed since the last action was released. |

In methods, we always have two parameters.

isAdjust: Check for adjust id not raw id.

actions or ids: True if any of this is matched.

| Methods           | Description                                   |
| ----------------- | --------------------------------------------- |
| IsLastGCD         | Check for GCD Record.                         |
| IsLastAbility     | Check for ability Record.                     |
| IsLastAction      | Check for action Record.                      |
| CombatElapsedLess | Check  how long the battle has been going on. |





