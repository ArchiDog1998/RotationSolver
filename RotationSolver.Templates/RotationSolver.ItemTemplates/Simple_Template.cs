namespace RotationSolver.ItemTemplates;

// RotationDesc here is used to display burst actions, indicating that if 'Auto Burst' is not turned on, the actions will not be released
[RotationDesc(ActionID.Lightspeed)]
// Link to the Ratation source code
[SourceCode(Path = "main/DefaultRotations/Melee/NIN_Default.cs")]
// The detailed or extended description links of this Ratation, such as loop diagrams, recipe urls, teaching videos, etc.,
// can be written more than one
[LinkDescription("https://www.thebalanceffxiv.com/img/jobs/nin/earlymug3.png")]
[LinkDescription("https://www.thebalanceffxiv.com/img/jobs/nin/nininfographicwindows.png")]
[LinkDescription("https://docs.google.com/spreadsheets/u/0/d/1BZZrqWMRrugCeiBICEgjCz2vRNXt_lRTxPnSQr24Em0/htmlview#",
    "Under the “Planner (With sample section)”")]

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
        throw new NotImplementedException();
    }
}
