using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic;
using RotationSolver.Data;
using RotationSolver.Updaters;

namespace RotationSolver.SigReplacers;

internal sealed class IconReplacer : IDisposable
{
    private delegate ulong IsIconReplaceableDelegate(uint actionID);

    private delegate uint GetIconDelegate(IntPtr actionManager, uint actionID);

    private delegate IntPtr GetActionCooldownSlotDelegate(IntPtr actionManager, int cooldownGroup);

    public static TargetHostileType RightNowTargetToHostileType
    {
        get
        {
            if (Service.Player == null) return 0;
            var id = Service.Player.ClassJob.Id;
            return GetTargetHostileType(Service.GetSheet<ClassJob>().GetRow(id));
        }
    }

    public static TargetHostileType GetTargetHostileType(ClassJob classJob)
    {
        if (Service.Config.TargetToHostileTypes.TryGetValue(classJob.RowId, out var type))
        {
            return (TargetHostileType)type;
        }

        return classJob.GetJobRole() == JobRole.Tank ? TargetHostileType.AllTargetsCanAttack : TargetHostileType.TargetsHaveTarget;
    }

    /// <summary>
    /// https://github.com/attickdoor/XIVComboPlugin/blob/master/XIVComboPlugin/IconReplacerAddressResolver.cs
    /// </summary>
    [Signature("E8 ?? ?? ?? ?? 84 C0 74 4C 8B D3", DetourName = nameof(IsIconReplaceableDetour))]
    private readonly Hook<IsIconReplaceableDelegate> isIconReplaceableHook;

        private readonly Hook<GetIconDelegate> getIconHook;

    private IntPtr actionManager = IntPtr.Zero;

    public IconReplacer()
    {
        unsafe
        {
            getIconHook = Hook<GetIconDelegate>.FromAddress((IntPtr)ActionManager.MemberFunctionPointers.GetAdjustedActionId, GetIconDetour);
        }
        SignatureHelper.Initialise(this);

        getIconHook?.Enable();
        isIconReplaceableHook?.Enable();
    }

    public void Dispose()
    {
        getIconHook?.Dispose();
        isIconReplaceableHook?.Dispose();
    }

    internal ActionID OriginalHook(ActionID actionID)
    {
        return (ActionID)getIconHook.Original.Invoke(actionManager, (uint)actionID);
    }

    internal uint OriginalHook(uint actionID)
    {
        return getIconHook.Original.Invoke(actionManager, actionID);
    }

    private unsafe uint GetIconDetour(IntPtr actionManager, uint actionID)
    {
        this.actionManager = actionManager;
        return RemapActionID(actionID);
    }

    internal static ActionID KeyActionID => ActionID.Repose;

    private uint RemapActionID(uint actionID)
    {
        PlayerCharacter localPlayer = Service.Player;

        if (localPlayer == null || actionID != (uint)KeyActionID || Service.Config.NeverReplaceIcon)
            return OriginalHook(actionID);

        return ActionUpdater.NextAction?.AdjustedID ?? OriginalHook(actionID);
    }

    private ulong IsIconReplaceableDetour(uint actionID)
    {
        return 1uL;
    }
}
