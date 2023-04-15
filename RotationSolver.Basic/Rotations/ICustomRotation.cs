using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.Basic.Rotations;

public interface ICustomRotation : ITexture
{
    ClassJob Job { get; }
    ClassJobID[] JobIDs { get; }

    string GameVersion { get; }
    string RotationName { get; }
    IRotationConfigSet Configs { get; }
    MedicineType MedicineType { get; }

    IBaseAction[] AllBaseActions { get; }
    IAction[] AllActions { get; }
    PropertyInfo[] AllBools { get; }
    PropertyInfo[] AllBytes { get; }

    MethodInfo[] AllTimes { get; }
    MethodInfo[] AllLast { get; }
    MethodInfo[] AllGCDs { get; }

    IAction ActionHealAreaGCD { get; }
    IAction ActionHealAreaAbility { get; }
    IAction ActionHealSingleGCD { get; }
    IAction ActionHealSingleAbility { get; }
    IAction ActionDefenseAreaGCD { get; }
    IAction ActionDefenseAreaAbility { get; }
    IAction ActionDefenseSingleGCD { get; }
    IAction ActionDefenseSingleAbility { get; }
    IAction ActionMoveForwardGCD { get; }
    IAction ActionMoveForwardAbility { get; }
    IAction ActionMoveBackAbility { get; }
    IAction EsunaStanceNorthGCD { get; }
    IAction EsunaStanceNorthAbility { get; }
    IAction RaiseShirkGCD { get; }
    IAction RaiseShirkAbility { get; }
    IAction AntiKnockbackAbility { get; }
    bool TryInvoke(out IAction newAction, out IAction gcdAction);

    /// <summary>
    /// This is an <seealso cref="ImGui"/> method for display the rotation status on Window.
    /// </summary>
    void DisplayStatus();
}
