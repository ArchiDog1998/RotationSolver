using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using Dalamud.Utility;
using XIVComboPlus.Attributes;

namespace XIVComboPlus.Combos;

public abstract class CustomCombo
{
    #region Job

    internal abstract uint JobID { get; }

    internal abstract string JobName { get; }

    internal struct GeneralActions
    {
        internal static readonly BaseAction
            //混乱
            Addle = new BaseAction(7560u),

            //即刻咏唱
            Swiftcast = new BaseAction(7561u)
            {
                BuffsProvide = new ushort[]
            {
                ObjectStatus.Swiftcast1,
                ObjectStatus.Swiftcast2,
                ObjectStatus.Swiftcast3,
                ObjectStatus.Triplecast,
                ObjectStatus.Dualcast,
                ObjectStatus.Acceleration,
            }
            },

            //康复
            Esuna = new BaseAction(7568)
            {
                OtherCheck = () => TargetHelper.WeakenPeople.Length > 0,
            },

            //醒梦（如果MP低于6000那么使用）
            LucidDreaming = new BaseAction(7562u)
            {
                OtherCheck = () => Service.ClientState.LocalPlayer.CurrentMp < 6000,
            },

            //伤腿
            LegGraze = new BaseAction(7554)
            {
                BuffsProvide = new ushort[]
                {
                    13,564,1345,
                },
            },

            //内丹
            SecondWind = new BaseAction(7541)
            {
                OtherCheck = () => (double)Service.ClientState.LocalPlayer.CurrentHp / Service.ClientState.LocalPlayer.MaxHp < 0.6,
            },

            //伤足
            FootGraze = new BaseAction(7553);

    }
    #endregion

    #region Combo
    protected internal abstract uint[] ActionIDs { get; }
    public abstract string ComboFancyName { get; }

    public abstract string Description { get; }

    public virtual string[] ConflictingCombos => new string[0];
    public virtual string ParentCombo => string.Empty;

    public virtual bool SecretCombo => false;
    /// <summary>
    /// 优先级，越大就使用到的概率越高！
    /// </summary>
    public virtual byte Priority => 0;
    public bool IsEnabled
    {
        get => Service.Configuration.EnabledActions.Contains(ComboFancyName);
        set
        {
            if (value)
            {
                Service.Configuration.EnabledActions.Add(ComboFancyName);
            }
            else
            {
                Service.Configuration.EnabledActions.Remove(ComboFancyName);
            }
        }
    }

    #endregion
    protected static bool IsMoving => TargetHelper.IsMoving;
    protected static bool HaveTargetAngle => TargetHelper.GetObjectInRadius(TargetHelper.HostileTargets, 25).Length > 0;

    protected static PlayerCharacter LocalPlayer => Service.ClientState.LocalPlayer;
    protected static GameObject Target => Service.TargetManager.Target;
    protected static bool CanInsertAbility => !LocalPlayer.IsCasting && Service.IconReplacer.GetCooldown(141u).CooldownRemaining > 0.67;
    protected CustomCombo()
    {
    }

    internal bool TryInvoke(uint actionID, uint lastComboActionID, float comboTime, byte level, out uint newActionID)
    {
        newActionID = 0u;
        if (!IsEnabled)
        {
            return false;
        }
        if (!ActionIDs.Contains(actionID))
        {
            return false;
        }
        uint num2 = Invoke(actionID, lastComboActionID, comboTime, level);
        if (num2 == 0 || actionID == num2)
        {
            return false;
        }
        newActionID = num2;
        return true;
    }

    protected abstract uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level);

}
