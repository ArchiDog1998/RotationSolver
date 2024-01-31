using RotationSolver.Basic.Traits;

namespace RotationSolver.Basic.Rotations;

partial class CustomRotation
{
    internal static void LoadActionSetting(ref IBaseAction action)
    {
        //TODO: better target type check. (NoNeed?)
        //TODO: better friendly check.
        //TODO: load the config from the configuration.
    }

    #region Role Actions
    static partial void ModifyAddlePvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.Addle];
        setting.TargetStatusFromSelf = false;
    }

    static partial void ModifySwiftcastPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = StatusHelper.SwiftcastStatus;
    }

    static partial void ModifyEsunaPvE(ref ActionSetting setting)
    {
        setting.TargetType = TargetType.Dispel;
    }

    static partial void ModifyLucidDreamingPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Player.CurrentMp < 6000 && InCombat;
    }

    static partial void ModifySecondWindPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Player?.GetHealthRatio() < Service.Config.HealthSingleAbility && InCombat;
    }

    static partial void ModifyRampartPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = StatusHelper.RampartStatus;
    }

    static partial void ModifyBloodbathPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Player?.GetHealthRatio() < Service.Config.HealthSingleAbility && InCombat && HasHostilesInRange;
    }

    static partial void ModifyFeintPvE(ref ActionSetting setting)
    {
        setting.TargetStatusFromSelf = false;
        setting.TargetStatusProvide = [StatusID.Feint];
    }

    static partial void ModifyLowBlowPvE(ref ActionSetting setting)
    {
        setting.CanTarget = o =>
        {
            if (o is not BattleChara b) return false;

            if (b.IsBossFromIcon() || IsMoving || b.CastActionId == 0) return false;

            if (!b.IsCastInterruptible || ActionID.InterjectPvE.IsCoolingDown()) return true;
            return false;
        };
    }

    static partial void ModifyPelotonPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () =>
        {
            if (!NotInCombatDelay) return false;
            var players = PartyMembers.GetObjectInRadius(20);
            if (players.Any(ObjectHelper.InCombat)) return false;
            return players.Any(p => p.WillStatusEnd(3, false, StatusID.Peloton));
        };
    }

    static partial void ModifyIsleSprintPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.Dualcast];
    }
    #endregion

    #region PvP

    static partial void ModifyStandardissueElixirPvP(ref ActionSetting setting)
    {
        setting.ActionCheck = () => !HasHostilesInMaxRange
            && (Player.CurrentMp <= Player.MaxMp / 3 || Player.CurrentHp <= Player.MaxHp / 3)
            && !IsLastAction(ActionID.StandardissueElixirPvP);
    }

    static partial void ModifyRecuperatePvP(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Player.MaxHp - Player.CurrentHp > 15000;
    }

    static partial void ModifyPurifyPvP(ref ActionSetting setting)
    {
        setting.TargetType = TargetType.Dispel;
    }

    static partial void ModifySprintPvP(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.Sprint_1342];
    }

    #endregion
    private protected virtual IBaseAction? Raise => null;
    private protected virtual IBaseAction? TankStance => null;

    private protected virtual IBaseAction? LimitBreak1 => null;
    private protected virtual IBaseAction? LimitBreak2 => null;
    private protected virtual IBaseAction? LimitBreak3 => null;

    public virtual IAction[] AllActions => 
    [
        .. AllBaseActions,
        .. Medicines,
        .. MpPotions,
        .. HpPotions,
        .. AllItems,
    ];

    public virtual IBaseTrait[] AllTraits { get; } = [];

    PropertyInfo[] _allBools;
    public PropertyInfo[] AllBools => _allBools ??= GetType().GetStaticProperties<bool>();

    PropertyInfo[] _allBytes;
    public PropertyInfo[] AllBytesOrInt => _allBytes ??= GetType().GetStaticProperties<byte>().Union(GetType().GetStaticProperties<int>()).ToArray();

    PropertyInfo[] _allFloats;
    public PropertyInfo[] AllFloats => _allFloats ??= GetType().GetStaticProperties<float>();
}
