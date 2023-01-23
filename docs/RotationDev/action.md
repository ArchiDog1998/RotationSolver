# Action

Another very important source of information is action.

## Basic

| Property    | Description            |
| ----------- | ---------------------- |
| EnoughLevel | EnoughLevel for using. |

## Target

| Property      | Description                    |
| ------------- | ------------------------------ |
| Target        | The action's target.           |
| IsTargetDying | Shortcut for Target.IsDying(); |
| IsTargetBoss  | Shortcut for Target.IsBoss();  |

## CoolDown

| Property       | Description             |
| -------------- | ----------------------- |
| IsCoolingDown  | Is action cooling down. |
| CurrentCharges | Current charges count.  |
| MaxCharges     | Max charges count.      |

| Method               | Description                                  |
| -------------------- | -------------------------------------------- |
| ElapsedAfterGCD      | Has it been in cooldown for so long?         |
| ElapsedAfter         | Has it been in cooldown for so long?         |
| WillHaveOneChargeGCD | Will have at least one charge after a while? |
| WillHaveOneCharge    | Will have at least one charge after a while? |
