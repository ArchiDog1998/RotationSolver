using ECommons.GameHelpers;
using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.Basic.Traits;
/// <summary>
/// The trait thing.
/// </summary>
public class BaseTrait : IBaseTrait
{
    private readonly Trait? _trait;

    /// <summary>
    /// Has enough level.
    /// </summary>
    public bool EnoughLevel => Player.Level >= Level;

    /// <summary>
    /// The level of this trait.
    /// </summary>
    public byte Level => _trait?.Level ?? 1;

    /// <summary>
    /// 
    /// </summary>
    public uint IconID { get; }

    /// <summary>
    /// 
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 
    /// </summary>
    public string Description => Name;

    /// <summary>
    /// 
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public uint ID { get; }

    /// <summary>
    /// Create by id.
    /// </summary>
    /// <param name="traitId"></param>
    public BaseTrait(uint traitId)
    {
        ID = traitId;
        _trait = Service.GetSheet<Trait>().GetRow(traitId)!;
        Name = _trait?.Name ?? string.Empty;
        IconID = (uint)(_trait?.Icon ?? 0);
    }
}
