# Character

The last source of information is character. Very important.

## Basic

| Method         | Description                                                  |
| -------------- | ------------------------------------------------------------ |
| IsBoss         | Is character a boss? Max HP exceeds a certain amount.        |
| IsDying        | Is character a dying? Current HP is below a certain amount. It is for running out of resources. |
| GetHealthRatio | Get the target's current HP percentage.                      |

## Status

| Method           | Description                              |
| ---------------- | ---------------------------------------- |
| WillStatusEndGCD | Will any of status be end after a while? |
| WillStatusEnd    | Will any of status be end after a while? |
| HasStatus        | Has one status right now.                |

