# CanUse

It is a complex method which used everywhere. In one word, it will check a lot of things.

Level, Enabled, Action Status, MP, Player Status, Coll down, Combo, Moving (for casting), Charges, Target, etc.

Check the source code [here](https://github.com/ArchiDog1998/RotationSolver/blob/dae05a0777ed567ac4f7512244887fe7e7cc9f2a/RotationSolver/Actions/BaseAction/BaseAction_ActionInfo.cs#L54).

## Usage

High-damage actions always have multiple restrictions.  Use them at first, then AOEs, then single targets. DOT actions always have some target status, so use it above.

## Param

some param you can use here.

### mustUse

AOE action will be used on a single target.

Moving actions will skip checking for distance. 

Skip for StatusProvide and TargetStatus checking.

### emptyOrSkipCombo

Use up all charges, without keeping one.

Do not need to check the combo.

### skipDisable

Skip the disable for emergency use. Please always set this to false.
