using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using System;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Data;
using XIVAutoAttack.SigReplacers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Helpers.IActionHelper;
//using static XIVAutoAttack.Combos.RangedMagicial.BLMCombo.BLMCombo.Actions;
using System.Threading;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Combos.CustomCombo;

namespace XIVAutoAttack.Combos.RangedMagicial.BLMCombo
{
    /// <summary>
    /// 黑魔帮助类
    /// </summary>
    internal sealed partial class BLMCombo : JobGaugeCombo<BLMGauge>
    {
        private static double Mpyupan = 0;
        private static double MPNextUpInCurrGCD = 0;
        internal static double MPYuPanDouble = 0;


        internal static bool IsPolyglotStacksMaxed => Actions.Xenoglossy.EnoughLevel ? JobGauge.PolyglotStacks == 2 : JobGauge.PolyglotStacks == 1;
        internal static bool HasFire => Player.HaveStatus(ObjectStatus.Firestarter);
        internal static bool HasThunder => Player.HaveStatus(ObjectStatus.Thundercloud);
        internal static bool TargetHasThunder => Target.HaveStatus(ObjectStatus.Thunder, ObjectStatus.Thunder2, ObjectStatus.Thunder3, ObjectStatus.Thunder4);

        internal static Watcher.ActionRec[] RecordSpells => Watcher.RecordActions.Where(b => b.action.ActionCategory.Value.RowId == 2).ToArray();
        internal static bool IsOldSpell(int count, IAction action) => RecordSpells[count].action.RowId == action.ID;
        internal static bool TargetThunderWillEnd(float time) => Target.WillStatusEnd(time, false, ObjectStatus.Thunder, ObjectStatus.Thunder2, ObjectStatus.Thunder3, ObjectStatus.Thunder4);
        /// <summary>
        /// 计算魔法的咏唱或GCD时间
        /// </summary>
        /// <param name="GCDTime"></param>
        /// <param name="isSpell"></param>
        /// <param name="isSpellTime"></param>
        /// <returns></returns>
        internal static double CalcSpellTime(double GCDTime, bool isSpell = true, bool isSpellTime = true) => CooldownHelper.CalcSpellTime(GCDTime, isSpell, isSpellTime);
        /// <summary>
        /// 计算咏速是否可以打几个火4
        /// </summary>
        /// <param name="f4Count"></param>
        /// <returns></returns>
        internal static bool CanF4Number(int f4Count, bool hasFire = true) => (hasFire ? CalcSpellTime(2500, isSpell: false) : -550) + CalcSpellTime(2800) * f4Count + CalcSpellTime(3000, isSpellTime: false) <= 15000;
        
        /// <summary>
        /// 当前经过几个GCD到冰阶段时异言数量
        /// </summary>
        /// <param name="isInIce"></param>
        /// <returns></returns>      
        internal static int HaveXeCounts(int GCDcount)
        {
            var count = 0;
            //当前异言数量
            count += JobGauge.PolyglotStacks;
            //冰阶段可用异言数量
            if (JobGauge.EnochianTimer < GCDTime * GCDcount - 1)
            { 
                count++;
            }
            return count;
        }

        /// <summary>
        /// 当前经过几个GCD到冰阶段时是否有雷可用
        /// </summary>
        /// <param name="isInIce"></param>
        /// <returns></returns>      
        internal static bool HaveT3InIce(int GCDcount)
        {
            if (!HasThunder) return false;

            if (!TargetThunderWillEnd((float)((GCDTime * GCDcount / 1000) + 5))) return false;

            if (!Player.WillStatusEnd((float)(GCDTime * GCDcount / 1000), false, ObjectStatus.Thundercloud)) return true;
            return false;
        }

        /// <summary>
        /// 未来观测卡跳蓝(三连咏唱)
        /// </summary>
        /// <returns></returns>
        internal static bool BenignMp()
        {
            if (inOpener || HaveSwift || HasFire || JobGauge.IsParadoxActive || Actions.Triplecast.ChargesCount == 0) return false;

            //双星灵时悖论后到星灵前时间
            var tatolTime = (float)(GCDTime * 2 - 0.7) / 1000;

            if (tatolTime > (6 - (ActionUpdater.MPUpdateElapsed + (float)GCDTime / 1000 * 3) % 3) && F4RemainingNumber() == 1)
            {
                if (HaveXeCounts(3) > 1 || HaveXeCounts(3) == 1 && HaveT3InIce(3)) return true;
            }

            if (tatolTime > (6 - (ActionUpdater.MPUpdateElapsed + (float)GCDTime / 1000 * 2) % 3) && F4RemainingNumber() == 0)
            {
                {
                    if (HaveXeCounts(2) > 1 || HaveXeCounts(2) == 1 && HaveT3InIce(2)) return true;
                }
            }
            if (tatolTime > (6 - (ActionUpdater.MPUpdateElapsed + (float)GCDTime / 1000 * 1) % 3) && IsLastSpell(true, Actions.Despair))
            {
                {
                    if (HaveXeCounts(1) > 1 || HaveXeCounts(1) == 1 && HaveT3InIce(1)) return true;
                }
            }

            return false;
        }

        /// <summary>
        /// GCD间隔总时间
        /// </summary>
        internal static double GCDTime => CalcSpellTime(2500, isSpell: false);

        /// <summary>
        /// 当前火状态还能打几个火4
        /// </summary>
        /// <returns></returns>
        internal static byte F4RemainingNumber()
        {
            if (!JobGauge.InAstralFire) return 0;
            var mpCount = (byte)((Player.CurrentMp - 800) / Actions.Fire4.MPNeed);
            var timeCountDe = (byte)((JobGauge.ElementTimeRemaining - CalcSpellTime(3000)) / CalcSpellTime(2800));
            var timeCountPe = (byte)((JobGauge.ElementTimeRemaining - CalcSpellTime(2500)) / CalcSpellTime(2800));
            if (JobGauge.IsParadoxActive) return Math.Min(mpCount, timeCountPe);
            else return Math.Min(mpCount, timeCountDe);
        }

        

        private static double MPYuCe(int count)
        {
            //双星灵蓝量预测(两瞬发情况下)
            MPYuPanDouble = 0;

            if (MPNextUpInCurrGCD >= Mpyupan && MPNextUpInCurrGCD <= CalcSpellTime(2500, false))
            {
                MPYuPanDouble += 3200;
            }
            if (MPNextUpInCurrGCD > CalcSpellTime(2500, false))
            {
                MPYuPanDouble += 4700;
            }

            if (MPNextUpInCurrGCD + 3000 > CalcSpellTime(2500, false) && MPNextUpInCurrGCD + 6000 > CalcSpellTime(2500, false) * 3 - 0.8)
            {
                MPYuPanDouble += 4700;
            }
            if (MPNextUpInCurrGCD + 6000 < CalcSpellTime(2500, false) * 3 - 0.8)
            {
                MPYuPanDouble += 4700;
            }
            if (MPYuPanDouble > 10000) MPYuPanDouble = 10000;
            return MPYuPanDouble;
        }
    }
}
