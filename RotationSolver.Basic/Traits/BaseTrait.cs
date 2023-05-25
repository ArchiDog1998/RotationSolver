using ECommons.GameHelpers;
using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.Basic.Traits;

public class BaseTrait : IBaseTrait
{
    private Trait _trait;
    public bool EnoughLevel => Player.Level >= Level;
    public byte Level => _trait.Level;

    public BaseTrait(uint traitId)
    {
        _trait = Service.GetSheet<Trait>().GetRow(traitId);
    }
}
