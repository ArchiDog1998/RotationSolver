namespace RotationSolver.Basic.Data;
using Action = Lumina.Excel.GeneratedSheets.Action;

/// <summary>
/// Action Record
/// </summary>
/// <param name="UsedTime">Action used time.</param>
/// <param name="Action">The action.</param>
public record ActionRec(DateTime UsedTime, Action Action);

/// <summary>
/// Time that got damage.
/// </summary>
/// <param name="ReceiveTime">the time.</param>
/// <param name="Ratio">ratio of hp.</param>
public record DamageRec(DateTime ReceiveTime, float Ratio);
