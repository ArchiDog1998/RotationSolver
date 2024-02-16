# Rotation Part

We just started a simple 123 rotation. Obviously, it doesn't solve the complex combats. In this part, lets see how many methods can we override, and how to use them. That is the major work for the rotation developers.

## GCD

General Cooldown Actions, which contains Weapon Skill and Spell.

But in Rotation Solver, not only actions above can add into these methods. It means, when GCD is cooled down, will choose one of them to use.

For example. BLM is a special job. In many case it needs to use `Triplecast` after `Fire4`. But when fire 4 is finished, GCD is cooled down. So `Triplecast`  is a case that need to be used in GCD.

The code that defines these methods is [here](https://github.com/ArchiDog1998/RotationSolver/blob/78ede8c386e3c37708b3cb15f259ccf4b839caaf/RotationSolver/Rotations/CustomRotation/CustomRotation_GCD.cs#L79-L109).

### EmergencyGCD

This is a method with the highest priority, even higher than raise, heal and defense. So it is rarely used. I only use it on RDM. Because I don't want to use `Verraise` when I am burst with Scorch, etc.

```c#
    private protected override bool EmergencyGCD(out IAction? act)
    {
        act = null; return false;
    }
```

### MoveGCD

Only use when player input the macro with `Move Forward`.

``` c#
    private protected override bool MoveGCD(out IAction? act)
    {
        act = null; return false;
    }
```

### HealSingleGCD

When need to use spell to heal one player. 

``` c#
    private protected override bool HealSingleGCD(out IAction? act)
    {
        act = null; return false;
    }
```

### HealAreaGCD

When need to use spell to heal multiple players. 

``` c#
    private protected override bool HealAreaGCD(out IAction? act)
    {
        act = null; return false;
    }
```

### DefenseSingleGCD

When need to use spell to defense for one player.

``` c#
    private protected override bool DefenseSingleGCD(out IAction? act)
    {
        act = null; return false;
    }
```

### DefenseAreaGCD

When need to use spell to defense for multiple players.

``` c#
    private protected override bool DefenseAreaGCD(out IAction? act)
    {
        act = null; return false;
    }
```

### GeneralGCD

Just normal GCD. Always for attack.

``` c#
    private protected override bool GeneralGCD(out IAction? act)
    {
        act = null; return false;
    }
```



## Ability

If GCD is not cooled down, Rotation Solver will find an action from abilities.

`abilitiesRemaining` means how many abilities will be used before next GCD. 

The code that defines these methods is [here](https://github.com/ArchiDog1998/RotationSolver/blob/78ede8c386e3c37708b3cb15f259ccf4b839caaf/RotationSolver/Rotations/CustomRotation/CustomRotation_Ability.cs#L251-L306).

### EmergencyAbility

This is a method with the highest priority. And only this method has `nextGCD` parameter.  The logic `before` is there. You may want to use `IsTheSameTo` method to check what is the next action.

``` c#
    private protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        act = null; return false;
    }
```

Here is the example for MCH.

``` c#
    private protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        if (nextGCD.IsTheSameTo(true, ChainSaw))
        {
            if (Reassemble.CanUse(out act, emptyOrSkipCombo: true)) return true;
        }
        return base.EmergencyAbility(abilitiesRemaining, nextGCD, out act);
    }
```

### MoveForwardAbility

Only use when player input the macro with `Move Forward`.

``` c#
    private protected override bool MoveForwardAbility(out IAction? act)
    {
        act = null; return false;
    }
```

### MoveBackAbility

Only use when player input the macro with `Move Back`.

``` c#
    private protected override bool MoveBackAbility(out IAction? act)
    {
        act = null; return false;
    }
```

### HealSingleAbility

When need to use ability to heal one player. 

``` c#
    private protected override bool HealSingleAbility(out IAction? act)
    {
        act = null; return false;
    }
```

### HealAreaAbility

When need to use ability to heal multiple players. 

``` c#
    private protected override bool HealAreaAbility(out IAction? act)
    {
        act = null; return false;
    }
```

### DefenceSingleAbility

When need to use ability to defense for one player. 

``` c#
    private protected override bool DefenceSingleAbility(out IAction? act)
    {
        act = null; return false;
    }
```

### DefenceAreaAbility

When need to use ability to defense for multiple players. 

``` c#
    private protected override bool DefenceAreaAbility(out IAction? act)
    {
        act = null; return false;
    }
```

### GeneralAbility

Just normal abilities. No need for hostile target.

``` c#
    private protected override bool GeneralAbility(out IAction? act)
    {
        act = null; return false;
    }
```

### AttackAbility

Need for hostile target in attack range. Ranged roles are 25, others are 3.

``` c#
    private protected override bool AttackAbility(out IAction? act)
    {
        act = null; return false;
    }
```

## Specials

One more thing...

### CountDownAction

When counting down, you can actions on time.

``` c#
    private protected override IAction CountDownAction(float remainTime)
    {
        return null;
    }
```

For example of PLD.

``` c#
    private protected override IAction CountDownAction(float remainTime)
    {
        if (remainTime <= 2 && HolySpirit.CanUse(out var act)) return act;

        if (remainTime <= 15 && DivineVeil.CanUse(out act)) return act;

        return base.CountDownAction(remainTime);
    }
```

