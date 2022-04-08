using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Enums;
using System.Linq;
using XIVComboPlus;
using XIVComboPlus.Combos;

namespace XIVComboPlus.Combos.BLM;

internal class BLM_Feature : BLMCombo
{
    public override string ComboFancyName => "黑魔GCD";

    public override string Description => "替换火1为持续的GCD循环，自动判断群攻还是单体！";

    protected internal override uint[] ActionIDs => new uint[] { Actions.Fire.ActionID };

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        uint act;
        if (IsMoving)
        {
            //如果在移动并且有目标。
            if (HaveTargetAngle)
            {
                if (Actions.Flare.TryUseAction(level, out act)) return act;
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
                if (JobGauge.ElementTimeRemaining < 10000)
                    return Actions.Transpose.ActionID;
            }
        }


        if (MantainceState(level, lastComboMove, out act)) return act;
        if (CanAddAbility(level, out act)) return act;
        if (AttackAndExchange(level, out act)) return act;
        return GeneralActions.Addle.ActionID;
    }

    private bool AttackAndExchange(byte level, out uint act)
    {
        //如果通晓满了，就放掉。
        if (IsPolyglotStacksMaxed && JobGauge.EnochianTimer < 10000)
        {
            if (Actions.Foul.TryUseAction(level, out act)) return true;
            if (Actions.Xenoglossy.TryUseAction(level, out act)) return true;
            if (Actions.Foul.TryUseAction(level, out act, mustUse: true)) return true;
        }

        if (JobGauge.InUmbralIce)
        {
            //如果没有火苗且单体有悖论，那打掉！
            if (!BaseAction.HaveStatus(BaseAction.FindStatusSelfFromSelf(ObjectStatus.Firestarter)) && 
                JobGauge.IsParadoxActive && Actions.Blizzard.TryUseAction(level, out act)) return true;

            if (Actions.Freeze.TryUseAction(level, out act)) return true;
            if (Actions.Blizzard2.TryUseAction(level, out act)) return true;

            //给我攻击！
            if (JobGauge.PolyglotStacks > 0)
            {
                if (Actions.Foul.TryUseAction(level, out act)) return true;
                if (Actions.Xenoglossy.TryUseAction(level, out act)) return true;
                if (Actions.Foul.TryUseAction(level, out act, mustUse: true)) return true;
            }

            if (Actions.Blizzard4.TryUseAction(level, out act)) return true;
            if (Actions.Blizzard.TryUseAction(level, out act)) return true;
        }
        else if (JobGauge.InAstralFire)
        {
            //如果蓝不够了，赶紧一个绝望。
            if (level >= 58 && JobGauge.UmbralHearts < 2)
            {
                if (Actions.Flare.TryUseAction(level, out act)) return true;
            }
            if (Service.ClientState.LocalPlayer.CurrentMp < Actions.Fire4.MPNeed + Actions.Despair.MPNeed)
            {
                if (Actions.Despair.TryUseAction(level, out act)) return true;
            }

            //试试看火2
            if (Actions.Fire2.TryUseAction(level, out act)) return true;

            //再试试看核爆
            if (Actions.Flare.TryUseAction(level, out act)) return true;


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
            bool hasFire = BaseAction.HaveStatus(BaseAction.FindStatusSelfFromSelf(ObjectStatus.Firestarter));

            if (LocalPlayer.CurrentMp > 9000 && (JobGauge.UmbralHearts == 3 || level < 58))
            {
                if (AddAstralFireStacks(level, lastAct, out act)) return true;
            }
            else if (LocalPlayer.CurrentMp >= 7200 && hasFire)
            {
                if (AddAstralFireStacks(level, lastAct, out act)) return true;
            }

            if (AddUmbralIceStacks(level, out act)) return true;
            if (AddUmbralHeartsSingle(level, out act)) return true;
            if (AddThunderSingle(level, lastAct, out act)) return true;
        }
        else if (JobGauge.InAstralFire)
        {
            //如果没蓝了，就直接冰状态。
            if (Service.ClientState.LocalPlayer.CurrentMp == 0 && XIVComboPlusPlugin.LastAction != Actions.Manafont.ActionID)
            {
                if (AddUmbralIceStacks(level, out act)) return true;
            }

            if (AddAstralFireStacks(level, lastAct, out act)) return true;
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
        if (JobGauge.UmbralIceStacks > 2 && JobGauge.ElementTimeRemaining > 4000) return false;

        //试试看冰2
        if (Actions.Blizzard2.TryUseAction(level, out act)) return true;

        //如果在火状态，切有火苗的话
        if (BaseAction.HaveStatus(BaseAction.FindStatusSelfFromSelf(ObjectStatus.Firestarter)) && (JobGauge.PolyglotStacks > 0 || 
            Service.ClientState.LocalPlayer.CurrentMp > 800))
        {
            if (JobGauge.InAstralFire)
            {
                //就变成冰状态！
                if (CanInsertAbility && Actions.Transpose.TryUseAction(level, out act)) return true;

                //创造内插的状态！
                if (JobGauge.PolyglotStacks > 0)
                {
                    if (Actions.Foul.TryUseAction(level, out act)) return true;
                    if (Actions.Xenoglossy.TryUseAction(level, out act)) return true;
                    if (Actions.Foul.TryUseAction(level, out act, mustUse: true)) return true;
                }

                //加个能力技？
                if (CanAddAbility(level, out act)) return true;

                //试试看冰3
                if (Actions.Blizzard3.TryUseAction(level, out act)) return true;

            }
        }
        else
        {
            //加个能力技？
            if (CanAddAbility(level, out act)) return true;

            //如果有冰悖论，那就上啊！
            if (JobGauge.UmbralIceStacks > 1 && JobGauge.IsParadoxActive && Actions.Blizzard.TryUseAction(level, out act)) return true;

            //试试看冰3
            if (Actions.Blizzard3.TryUseAction(level, out act)) return true;

            //试试看冰1
            if (Actions.Blizzard.TryUseAction(level, out act)) return true;
        }



        return false;
    }

    private bool AddAstralFireStacks(byte level, uint lastaction, out uint act)
    {
        //如果火满了，就别加了。
        act = 0;
        if (JobGauge.AstralFireStacks > 2 && JobGauge.ElementTimeRemaining > 5100) return false;

        if(Service.ClientState.LocalPlayer.CurrentMp < 5000 && lastaction != Actions.Manafont.ActionID)
        {
            if(AddUmbralIceStacks(level, out act)) return true;
        }

        //试试看火2
        if (Actions.Fire2.TryUseAction(level, out act)) return true;

        //如果在冰状态，且有火苗的话。
        if(BaseAction.HaveStatus(BaseAction.FindStatusSelfFromSelf(ObjectStatus.Firestarter)) && JobGauge.InUmbralIce)
        {
            //就变成火状态！
            if (CanInsertAbility && Actions.Transpose.TryUseAction(level, out act)) return true;

            //创造内插的状态！
            if(JobGauge.PolyglotStacks > 0)
            {
                if (Actions.Foul.TryUseAction(level, out act)) return true;
                if (Actions.Xenoglossy.TryUseAction(level, out act)) return true;
                if (Actions.Foul.TryUseAction(level, out act, mustUse: true)) return true;
            }
        }

        //加个能力技？
        if (CanAddAbility(level, out act)) return true;

        //试试看火3
        if ((JobGauge.InUmbralIce || JobGauge.AstralFireStacks == 1) && Actions.Fire3.TryUseAction(level, out act)) return true;

        //如果时间够火1，并且如果是90级有悖论，那只打悖论出来。
        if (JobGauge.ElementTimeRemaining > 2500 &&((level == 90 && JobGauge.IsParadoxActive) || level < 90))
        {
            if (Actions.Fire.TryUseAction(level, out act)) return true;
        }
        else
        {
            if ((lastaction != Actions.Fire.ActionID || lastaction != 25797) && AddUmbralIceStacks(level, out act)) return true;
        }

        //(level == 90 && JobGauge.IsParadoxActive) || level < 90

        return false;
    }

    private bool AddThunderSingle(byte level, uint lastAct, out uint act)
    {
        //试试看雷2
        if (Actions.Thunder2.TryUseAction(level, out act, lastAct)) return true;

        //加个能力技？
        if (CanAddAbility(level, out act)) return true;

        //试试看雷1
        if (Actions.Thunder.TryUseAction(level, out act, lastAct)) return true;

        return false;
    }

    private bool AddUmbralHeartsSingle(byte level, out uint act)
    {
        //如果满了，或者等级太低，没有冰心，就别加了。
        act = 0;
        if (JobGauge.UmbralHearts == 3 || level < 58) return false;

        //冻结
        if (Actions.Freeze.TryUseAction(level, out act)) return true;

        //加个能力技？
        if (CanAddAbility(level, out act)) return true;

        //冰4
        if (Actions.Blizzard4.TryUseAction(level, out act)) return true;

        return false;
    }

}
