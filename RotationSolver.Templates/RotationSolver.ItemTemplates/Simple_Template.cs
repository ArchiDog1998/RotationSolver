namespace RotationSolver.ItemTemplates;

// Link to the Ratation source code
[SourceCode(Path = "%Branch/FilePath to your sourse code% eg. main/DefaultRotations/Melee/NIN_Default.cs%")]
// The detailed or extended description links of this Ratation, such as loop diagrams, recipe urls, teaching videos, etc.,
// can be written more than one
[LinkDescription("%Link to the pics or just a link%", "%Description about your rotation.%")]
[YoutubeLink(ID ="%If you got a youtube video link, please add here, just video id!%")]


// Change this base class to your job's base class. It is named like XXX_Base.
public class Simple_Template : AST_Base 
{
    //Change this to the game version right now.
    public override string GameVersion => "6.48";

    // Change this to your custom Rotation name
    public override string RotationName => "$safeitemname$";

    //GCD actions here.
    protected override bool GeneralGCD(out IAction act)
    {
        throw new NotImplementedException();
    }

    //0GCD actions here.
    protected override bool AttackAbility(out IAction act)
    {
        return base.AttackAbility(out act);
    }
}
