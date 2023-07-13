using ECommons.GameHelpers;
using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.Basic.Traits;

/// <summary>
/// The trait thing.
/// </summary>
public class BaseTrait : IBaseTrait
{
    private Trait _trait;

    /// <summary>
    /// Has enough level.
    /// </summary>
    public bool EnoughLevel => Player.Level >= Level;

    /// <summary>
    /// The level of this trait.
    /// </summary>
    public byte Level => _trait.Level;

    /// <summary>
    /// Create by id.
    /// </summary>
    /// <param name="traitId"></param>
    public BaseTrait(uint traitId)
    {
        _trait = Service.GetSheet<Trait>().GetRow(traitId);
    }
}
