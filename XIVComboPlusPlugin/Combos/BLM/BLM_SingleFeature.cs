using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Enums;
using System.Linq;
using XIVComboPlus;
using XIVComboPlus.Combos;

namespace XIVComboPlus.Combos.BLM;

internal class BLM_SingleFeature : BLMCombo
{
    public override string ComboFancyName => "单个目标GCD";

    public override string Description => "替换火1为持续的GCD循环！";

    protected internal override uint[] ActionIDs => new uint[] { Actions.Fire.ActionID };

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        uint act;
        if (IsMoving)
        {
            //如果在移动并且有目标。
            if (HaveValidTarget)
            {
                if (Actions.Xenoglossy.TryUseAction(level, out act)) return act;
                if (Actions.Triplecast.TryUseAction(level, out act)) return act;
                if (GeneralActions.Swiftcast.TryUseAction(level, out act)) return act;
            }
            //如果在移动，但是没有目标。
            else
            {
                if (Actions.UmbralSoul.TryUseAction(level, out act))
                {
                    if (level < Actions.Paradox.Level)
                    {
                        return act;
                    }
                    else
                    {
                        if (JobGauge.UmbralIceStacks > 2 && JobGauge.UmbralHearts > 2)
                        {
                            return act;
                        }
                    }
                }
                return Actions.Transpose.ActionID;
            }
        }

        if (CanAddAbility(level, out act)) return act;
        if (MantainceState(level, lastComboMove, out act)) return act;
        if (AttackAndExchange(level, out act)) return act;
        return actionID;
    }

    private bool AttackAndExchange(byte level, out uint act)
    {
        if (JobGauge.InUmbralIce)
        {
            if (Actions.Blizzard4.TryUseAction(level, out act)) return true;
            if (Actions.Blizzard.TryUseAction(level, out act)) return true;
        }
        else if (JobGauge.InAstralFire)
        {
            //如果没蓝了，就直接冰状态。
            if (Service.ClientState.LocalPlayer.CurrentMp == 0)
            {
                if (AddUmbralIceStacks(level, out act)) return true;
            }
            //如果蓝不够了，赶紧一个绝望。
            if (Service.ClientState.LocalPlayer.CurrentMp < Actions.Fire4.MPNeed + Actions.Despair.MPNeed)
            {
                if (Actions.Despair.TryUseAction(level, out act)) return true;
            }

            //如果MP够打一发伤害。
            if (Service.ClientState.LocalPlayer.CurrentMp >= AttackAstralFire(level, out act))
            {
                return true;
            }
            //否则，转入冰状态。
            else
            {
                if (AddUmbralIceStacks(level, out act)) return true;
            }
        }

        act = 0;
        return false;
    }

    /// <summary>
    /// In AstralFire, maintain the time.
    /// </summary>
    /// <param name="level"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private uint AttackAstralFire(byte level, out uint act)
    {
        uint addition = level < Actions.Despair.Level ? 0u : 800u;

        //如果通晓满了，就放掉。
        if (IsPolyglotStacksFull)
        {
            if (Actions.Xenoglossy.TryUseAction(level, out act)) return addition;
            if (Actions.Foul.TryUseAction(level, out act)) return addition;
        }

        if (Actions.Fire4.TryUseAction(level, out act)) return Actions.Fire4.MPNeed + addition;
        if (Actions.Paradox.TryUseAction(level, out act)) return Actions.Paradox.MPNeed + addition;
        //如果有火苗了，那就来火3
        if (BaseAction.HaveStatus(BaseAction.FindStatusSelfFromSelf(ObjectStatus.Firestarter)))
        {
            act = Actions.Fire3.ActionID;
            return addition;
        }
        if (Actions.Fire.TryUseAction(level, out act)) return Actions.Fire.MPNeed + addition;
        return uint.MaxValue;
    }

    /// <summary>
    /// 保证冰火都是最大档数，保证有雷，如果条件允许，赶紧转火。
    /// </summary>
    /// <param name="level"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool MantainceState(byte level, uint lastAct, out uint act)
    {
        if (JobGauge.InUmbralIce)
        {
            if (HaveEnoughMP && (JobGauge.UmbralHearts == 3 || level < 58))
            {
                if (AddAstralFireStacks(level, out act)) return true;
            }

            if (AddUmbralIceStacks(level, out act)) return true;
            if (AddUmbralHeartsSingle(level, out act)) return true;
            if (AddThunderSingle(level, lastAct, out act)) return true;
        }
        else if (JobGauge.InAstralFire)
        {
            if (AddAstralFireStacks(level, out act)) return true;
            if (AddThunderSingle(level, lastAct, out act)) return true;
        }
        else
        {
            //没状态，就加个冰状态。
            if (AddUmbralIceStacks(level, out act)) return true;
        }

        return false;
    }

    private bool AddUmbralIceStacks(byte level, out uint act)
    {
        //如果冰满了，就别加了。
        act = 0;
        if (JobGauge.UmbralIceStacks > 2)return false;

        //试试看冰3
        if (Actions.Blizzard3.TryUseAction(level, out act)) return true;

        //试试看冰1
        if (Actions.Blizzard.TryUseAction(level, out act)) return true;

        act = Actions.Transpose.ActionID;
        return true;
    }

    private bool AddAstralFireStacks(byte level, out uint act)
    {
        //如果火满了，就别加了。
        act = 0;
        if (JobGauge.AstralFireStacks > 2) return false;

        if(Service.ClientState.LocalPlayer.CurrentMp < 5000)
        {
            if(AddUmbralIceStacks(level, out act)) return true;
        }

        //试试看火3
        if (Actions.Fire3.TryUseAction(level, out act)) return true;

        //如果时间够火1
        if (JobGauge.ElementTimeRemaining > 2500 && level >= Actions.Blizzard3.Level)
        {
            if (Actions.Fire.TryUseAction(level, out act)) return true;
        }
        else
        {
            if (Actions.Blizzard3.TryUseAction(level, out act)) return true;
        }

        act = Actions.Transpose.ActionID;
        return true;
    }

    private bool AddThunderSingle(byte level, uint lastAct, out uint act)
    {
        //试试看雷1
        if (Actions.Thunder.TryUseAction(level, out act, lastAct)) return true;

        return false;
    }

    private bool AddUmbralHeartsSingle(byte level, out uint act)
    {
        //如果满了，或者等级太低，没有冰心，就别加了。
        act = 0;
        if (JobGauge.UmbralHearts == 3 || level < 58) return false;

        //冰4
        if (Actions.Blizzard4.TryUseAction(level, out act)) return true;

        return false;
    }

}
