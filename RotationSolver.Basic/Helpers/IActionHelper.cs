using Dalamud.Logging;
using System.IO;

namespace RotationSolver.Basic.Helpers;

public static class IActionHelper
{
    public static SortedList<uint, float> AnimationLockTime = new SortedList<uint, float>();

    public static async void GetAnimationLockTimeAsync(string directory)
    {
#if DEBUG
        directory = @"E:\OneDrive - stu.zafu.edu.cn\PartTime\FFXIV\RotationSolver\Resources";
        if (!Directory.Exists(directory)) return;
#endif
        var filename = $"{nameof(AnimationLockTime)}.json";
        var filePath = Path.Combine(directory, filename);
        if (File.Exists(filePath))
        {
            var str = File.ReadAllText(filePath);
            AnimationLockTime = JsonConvert.DeserializeObject<SortedList<uint, float>>(str);
        }
        else
        {
            try
            {
                var client = new HttpClient();
                var str = await client.GetStringAsync($"https://raw.githubusercontent.com/ArchiDog1998/RotationSolver/main/Resources/{nameof(AnimationLockTime)}.json");
                File.WriteAllText(filePath, str);

                AnimationLockTime = JsonConvert.DeserializeObject<SortedList<uint, float>>(str);
            }
            catch (Exception ex)
            {
                PluginLog.Warning(ex, "Failed to download the animation lock time.");
            }
        }

    }

    public static ActionID[] MovingActions = new ActionID[]
    {
        ActionID.EnAvant,
        ActionID.Plunge,
        ActionID.RoughDivide,
        ActionID.Thunderclap,
        ActionID.Shukuchi,
        ActionID.Intervene,
        ActionID.CorpsACorps,
        ActionID.HellsIngress,
        ActionID.HissatsuGyoten,
        ActionID.Icarus,
        ActionID.Onslaught,
        ActionID.SpineShatterDive,
        ActionID.DragonFireDive,
    };

    internal static bool IsLastGCD(bool isAdjust, params IAction[] actions)
    {
        return IsLastGCD(GetIDFromActions(isAdjust, actions));
    }
    internal static bool IsLastGCD(params ActionID[] ids)
    {
        return IsActionID(DataCenter.LastGCD, ids);
    }

    internal static bool IsLastAbility(bool isAdjust, params IAction[] actions)
    {
        return IsLastAbility(GetIDFromActions(isAdjust, actions));
    }
    internal static bool IsLastAbility(params ActionID[] ids)
    {
        return IsActionID(DataCenter.LastAbility, ids);
    }

    internal static bool IsLastAction(bool isAdjust, params IAction[] actions)
    {
        return IsLastAction(GetIDFromActions(isAdjust, actions));
    }
    internal static bool IsLastAction(params ActionID[] ids)
    {
        return IsActionID(DataCenter.LastAction, ids);
    }

    public static bool IsTheSameTo(this IAction action, bool isAdjust, params IAction[] actions)
        => action.IsTheSameTo(GetIDFromActions(isAdjust, actions));

    public static bool IsTheSameTo(this IAction action, params ActionID[] actions)
    {
        if (action == null) return false;
        return IsActionID((ActionID)action.AdjustedID, actions);
    }

    private static bool IsActionID(ActionID id, params ActionID[] ids) => ids.Contains(id);

    private static ActionID[] GetIDFromActions(bool isAdjust, params IAction[] actions)
    {
        return actions.Select(a => isAdjust ? (ActionID)a.AdjustedID : (ActionID)a.ID).ToArray();
    }
}
