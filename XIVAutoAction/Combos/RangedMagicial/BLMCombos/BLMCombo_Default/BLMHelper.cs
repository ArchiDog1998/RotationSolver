//#if DEBUG

//using System;
//using System.Linq;
//using XIVAutoAttack.Actions;
//using XIVAutoAttack.Combos.Basic;
//using XIVAutoAttack.Data;
//using XIVAutoAttack.Helpers;
//using XIVAutoAttack.Updaters;
//using static XIVAutoAttack.Combos.RangedMagicial.BLMCombo_Default.BLMCombo_Default;
//using static XIVAutoAttack.SigReplacers.Watcher;

//namespace XIVAutoAttack.Combos.RangedMagicial.BLMCombo_Default;

///// <summary>
///// 黑魔帮助类
///// </summary>
//internal sealed partial class BLMCombo_Default : BLMCombo_Base<CommandType>
//{
//    internal static double TsPointElapsed = 0;
//    internal static double TsPointRemain = 0;
//    internal static double MPNextUpInCurrGCD = 0;
//    internal static double MPYuPanDouble = 0;
//    internal static float MpUpdateRemainInIceFirst = 0;


//    internal static float MpUpdateRemain => 3 - ActionUpdater.MPUpdateElapsed;
//    internal static bool TargetHasThunder => Target.HasStatus(true, StatusID.Thunder, StatusID.Thunder2, StatusID.Thunder3, StatusID.Thunder4);
//    /// <summary>
//    /// GCD间隔总时间
//    /// </summary>
//    internal static double GCDTime => CalcSpellTime(2500, isSpell: false);
//    internal static float GCDTimes => (float)CalcSpellTime(2500, isSpell: false) / 1000;
//    internal static float TwoGCDTSTime => (float)(GCDTime * 2 - 800) / 1000;
//    internal static bool MpTwoInIce => MpUpdateRemain > (3 - GCDTime / 1000) && MpUpdateRemain < (GCDTime / 1000 - 0.8);
//    internal static ActionRec[] RecordSpells => RecordActions.Where(b => b.action.ActionCategory.Value.RowId == 2).ToArray();
//    internal static bool IsOldSpell(int count, IAction action) => RecordSpells[count].action.RowId == action.ID;
//    internal static bool TargetThunderWillEnd(float time) => Target.WillStatusEnd(time, false, StatusID.Thunder, StatusID.Thunder2, StatusID.Thunder3, StatusID.Thunder4);
//    /// <summary>
//    /// 几个GCD后跳两次蓝所用时间
//    /// </summary>
//    /// <param name="GCDCount"></param>
//    /// <returns></returns>
//    internal static float MpBackGCDYuceDouble(int GCDCount) => (float)(6 - (ActionUpdater.MPUpdateElapsed + (float)GCDTime / 1000 * GCDCount) % 3);
//    internal static float MpBackGCDRYuceDouble(int GCDCount) => (float)(6 - (ActionUpdater.MPUpdateElapsed + ActionUpdater.WeaponRemain + (float)GCDTime / 1000 * GCDCount) % 3);
//    /// <summary>
//    /// 几个GCD后能否在两个GCD之间(星灵前)跳两次蓝
//    /// </summary>
//    /// <param name="GCDCount"></param>
//    /// <returns></returns>
//    internal static bool MpBackGCDCanDouble(int GCDCount) => (float)(GCDTime * 2 - 750) / 1000 > MpBackGCDYuceDouble(GCDCount);
//    internal static bool MpBackGCDRCanDouble(int GCDCount) => (float)(GCDTime * 2 - 750) / 1000 > MpBackGCDRYuceDouble(GCDCount);



//    internal static float MpUpdateAfterGCD(int GCDCount)
//    {
//        return (ActionUpdater.MPUpdateElapsed + (ActionUpdater.WeaponRemain + GCDCount * GCDTimes) % 3) % 3;
//    }

//    /// <summary>
//    /// 多少秒后MP跳蓝已经过时间
//    /// </summary>
//    /// <param name="time"></param>
//    /// <returns></returns>
//    internal static float MpUpdateAfterElapsed(float time)
//    {
//        return (ActionUpdater.MPUpdateElapsed + time) % 3;
//    }

//    /// <summary>
//    /// 多少秒后MP跳蓝剩余时间
//    /// </summary>
//    /// <param name="time"></param>
//    /// <returns></returns>
//    internal static float MpUpdateAfterRemain(float time)
//    {
//        return 3 - MpUpdateAfterElapsed(time);
//    }

//    internal static float MPUpInCurrGCD => (float)(6 - (ActionUpdater.MPUpdateElapsed - ActionUpdater.WeaponElapsed <= 0 ? ActionUpdater.MPUpdateElapsed - ActionUpdater.WeaponElapsed + 3 : ActionUpdater.MPUpdateElapsed - ActionUpdater.WeaponElapsed) % 3);



//    /// <summary>
//    /// 计算魔法的咏唱或GCD时间
//    /// </summary>
//    /// <param name="GCDTime"></param>
//    /// <param name="isSpell"></param>
//    /// <param name="isSpellTime"></param>
//    /// <returns></returns>
//    internal static double CalcSpellTime(double GCDTime, bool isSpell = true, bool isSpellTime = true) => CooldownHelper.CalcSpellTime(GCDTime, isSpell, isSpellTime);

//    /// <summary>
//    /// 计算咏速是否可以打几个火4
//    /// </summary>
//    /// <param name="f4Count"></param>
//    /// <returns></returns>
//    internal static bool CanF4Number(int f4Count, bool hasFire = true) => (hasFire ? CalcSpellTime(2500, isSpell: false) : -550) + CalcSpellTime(2800) * f4Count + CalcSpellTime(3000, isSpellTime: false) <= 15000;

//    /// <summary>
//    /// 当前经过几个GCD到冰阶段时异言数量
//    /// </summary>
//    /// <param name="isInIce"></param>
//    /// <returns></returns>      
//    internal static int HaveXeCounts(uint GCDcount)
//    {
//        var count = 0;
//        //当前异言数量
//        count += PolyglotStacks;
//        //冰阶段可用异言数量
//        if (EnchinaEndAfter(GCDcount * GCDTimes - 1))
//        {
//            count++;
//        }
//        return count;
//    }

//    /// <summary>
//    /// 当前经过几个GCD到冰阶段时是否有雷可用
//    /// </summary>
//    /// <param name="isInIce"></param>
//    /// <returns></returns>      
//    internal static bool HaveT3InIce(int GCDcount)
//    {
//        if (!HasThunder) return false;

//        if (!TargetThunderWillEnd((float)((GCDTime * GCDcount / 1000) + 5))) return false;

//        if (!Player.WillStatusEnd((float)(GCDTime * GCDcount / 1000), false, StatusID.Thundercloud)) return true;
//        return false;
//    }

//    /// <summary>
//    /// 未来观测卡跳蓝(三连咏唱)
//    /// </summary>
//    /// <returns></returns>
//    internal static bool BenignMp()
//    {
//        if (HaveSwift || HasFire || IsParadoxActive || Triplecast.CurrentCharges == 0) return false;

//        //双星灵时悖论后到星灵前时间
//        if (MpBackGCDCanDouble(3) && F4RemainingNumber() == 1)
//        {
//            if (HaveXeCounts(4) > 1 || HaveXeCounts(4) == 1 && HaveT3InIce(4)) return true;
//            return true;
//        }

//        if (MpBackGCDCanDouble(2) && F4RemainingNumber() == 0)
//        {
//            //if (tatolTime > (6 - (ActionUpdater.MPUpdateElapsed + (float)GCDTime / 1000 * 1 + CalcSpellTime(2800)) % 3)) return false;
//            if (HaveXeCounts(3) > 1 || HaveXeCounts(3) == 1 && HaveT3InIce(3)) return true;
//            return true;

//        }
//        if (MpBackGCDCanDouble(1) && IsLastGCD(true, Despair))
//        {
//            if (HaveXeCounts(2) > 1 || HaveXeCounts(2) == 1 && HaveT3InIce(2)) return true;
//            return true;

//        }
//        return false;
//    }



//    /// <summary>
//    /// 当前火状态还能打几个火4
//    /// </summary>
//    /// <returns></returns>
//    //internal static byte F4RemainingNumber(int count)
//    //{
//    //    if (!InAstralFire) return 0;
//    //    var mpCount = (byte)((Player.CurrentMp - 800) / Fire4.MPNeed);
//    //    var timeCountDe = (byte)((ElementTimeRemaining - CalcSpellTime(3000)) / CalcSpellTime(2800));
//    //    var timeCountPe = (byte)((ElementTimeRemaining - CalcSpellTime(2500)) / CalcSpellTime(2800));
//    //    if (IsParadoxActive) return Math.Min(mpCount, timeCountPe);
//    //    else return Math.Min(mpCount, timeCountDe);
//    //}

//    //internal static bool F4RemainingNumber(int count)
//    //{
//    //    if (!InAstralFire) return false;
//    //    var mpCount = (Player.CurrentMp - 800) / Fire4.MPNeed >= count;

//    //    var timeCountDe = !ElementTimeEndAfter((float)(CalcSpellTime(2800) * count + CalcSpellTime(3000)) / 1000);
//    //    var timeCountPe = !ElementTimeEndAfter((float)(CalcSpellTime(2800) * count + CalcSpellTime(2500)) / 1000);

//    //    if (IsParadoxActive) return mpCount && timeCountPe;
//    //    else return mpCount && timeCountDe;
//    //}

//    private static double MPYuCe(int count = 2)
//    {
//        //天语经过时间(秒)
//        var gcdTime = GCDTime / 1000;

//        if (InAstralFire && Transpose.IsCoolDown && (DateTime.Now - RecordActions[0].UsedTime).Milliseconds < 100)
//        {
//            MPNextUpInCurrGCD = (3 - (ActionUpdater.MPUpdateElapsed - ActionUpdater.WeaponElapsed)) % 3;
//            TsPointElapsed = ActionUpdater.WeaponElapsed - 0.1;// - (15000 - JobGauge.ElementTimeRemaining);
//        }

//        //双星灵蓝量预测(两瞬发情况下)
//        MPYuPanDouble = 0;

//        if (MPNextUpInCurrGCD >= TsPointElapsed && MPNextUpInCurrGCD <= gcdTime)
//        {
//            MPYuPanDouble += 3200;
//        }
//        if (MPNextUpInCurrGCD > gcdTime)
//        {
//            MPYuPanDouble += 4700;
//        }
//        if (MPNextUpInCurrGCD + 3 > gcdTime && MPNextUpInCurrGCD + 6 > gcdTime * 3 - 0.75)
//        {
//            MPYuPanDouble += 4700;
//        }
//        if (count == 1) return MPYuPanDouble;
//        else
//        {
//            if (MPNextUpInCurrGCD + 6 < gcdTime * 3 - 0.75)
//            {
//                MPYuPanDouble += 9400;
//            }
//            if (MPYuPanDouble > 10000) MPYuPanDouble = 10000;

//            return MPYuPanDouble;
//        }

//    }
//}
//#endif