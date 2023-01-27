# CanUse

It is a complex method which used everywhere. In one word, it will check a lot of things.

Level, Enabled, Action Status, MP, Player Status, Coll down, Combo, Moving (for casting), Charges, Target, etc.

Check the source code [here](https://github.com/ArchiDog1998/RotationSolver/blob/dae05a0777ed567ac4f7512244887fe7e7cc9f2a/RotationSolver/Actions/BaseAction/BaseAction_ActionInfo.cs#L54).

## Usage

So the action with high damage always have multiple restricts.  Use them at first, then aoe, then single. Dot actions always have some target status, so use it above.

## Param

some param you can use here.

### mustUse

AOE only need one target to use.

Moving action don't need to have enough distance to use. 

Skip for StatusProvide and TargetStatus checking.

### emptyOrSkipCombo

Use all charges, no keeping one.

Do not need to check the combo.

### skipDisable

Skip the diable for emergency use. Please always set this to false.