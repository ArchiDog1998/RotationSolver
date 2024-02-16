# Set Up

## Software

First you need to install the [Visual Studio](https://visualstudio.microsoft.com/). Please Check the `.NET desktop development`  under Workloads, `Class Designer` (optional)under Individual components.

![.NET desktop development](assets/image-20230122152037534.png)

![Class Designer](assets/image-20230122152134510.png)

## Project

Now, you need to add an extension for rotation develop. Download the `RotationSolver Templates` from `Manage Extensions`

![Template](assets/1680587392644.png)

Create a new project from the template and name it as what you want.

![Create Project](assets/1680587583305.png)

Update the Packages to the newest one.

![Update Nuget](assets/1680587915851.png)

Change the DalamudLibPath to your own.

![Change the Path](assets/1680588115090.png)

## Rotation

Right click the role you want to add. And then add a new item.

![1680588432784](assets/1680588432784.png)

Find the rotation template called `Simple Rotation`, and named it with `Job Abbreviation`\_`Rotation Name`. In this example, I create a WAR rotation and named with Test. So I call it WAR_Test.

![Create a rotation](assets/1680588677659.png)

And then, you'll see some thing like this.

``` c#
namespace RotationTest.Tank
{
    [LinkDescription("$Your link description here, it is better to link to a png! this attribute can be multiple! $")]
    [SourceCode("$If your rotation is open source, please write the link to this attribute! $")]
	[Rotation("WAR_Test", CombatType.PvE, GameVersion = "6.35")]
    // Change this base class to your job's base class. It is named like XXX_Base.
    public class WAR_Test : WarriorRotation
    {
        //GCD actions here.
        protected override bool GeneralGCD(out IAction? act)
        {
            throw new NotImplementedException();
        }

        //0GCD actions here.
        protected override bool AttackAbility(out IAction? act)
        {
            throw new NotImplementedException();
        }
    }
}
```



`Attack Ability` is for using the ability that can attack the mobs.

`General GCD` is similar to `Attack Ability` but is Weapon Skill or Spell.

So we need a rotation like `HeavySwing` -> `Maim` -> `StormsPath`/`StormsEye`.

And we know that 3 need 2 to use, 2 need 1 to use. We write like 321. And always put Effect of Time actions in front.

Let's just change some code like this, and you'll get a 123/4 rotation!

``` c#
namespace RotationTest.Tank;

[LinkDescription("$Your link description here, it is better to link to a png! this attribute can be multiple! $")]
[SourceCode("$If your rotation is open source, please write the link to this attribute! $")]
[Rotation("WAR_Test", CombatType.PvE, GameVersion = "6.35")]
// Change this base class to your job's base class. It is named like XXX_Base.
public class WAR_Test : WarriorRotation
{
    //GCD actions here.
    protected override bool GeneralGCD(out IAction? act)
    {
        if (StormsEyePvE.CanUse(out act)) return true;
        if (StormsPathPvE.CanUse(out act)) return true;
        if (MaimPvE.CanUse(out act)) return true;
        if (HeavySwingPvE.CanUse(out act)) return true;
        return false;
    }

    //0GCD actions here.
    protected override bool AttackAbility(out IAction? act)
    {
        act = null;
        return false;
    }
}
```

We will discuss the Action in the future, so let's just do it.

## To the Plugin

Change the Configuration from `Debug` to `Release` and then build it!

![Change to Release](assets/image-20230404141821517.png)![Build](assets/image-20230404141847740.png)

You'll see some output things in the Output, please copy the directory of this `dll`.

![Out put](assets/1680589498456.png)

In the game, add this directory to your [Rotation Dev](extra-rotations.md) folder. After clicking the `Update rotations` button, you'll see your rotation!

## Use your own rotation in Game

Let's go to the game and select your own rotation.

![1680965887006](assets/1680965887006.png)

When you use `Smart` or `Manual`, the rotation will be like this.

![rotations](assets/image-20230408222335027.png)
