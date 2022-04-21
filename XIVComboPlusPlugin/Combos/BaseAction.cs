using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace XIVComboPlus.Combos
{
    internal class BaseAction
    {
        internal const byte GCDCooldownGroup = 58;

        private bool _isFriendly;
        internal Action Action { get; }
        internal IconReplacer.CooldownData CoolDown => Service.IconReplacer.GetCooldown(Action.CooldownGroup);
        internal byte Level => Action.ClassJobLevel;
        internal uint ActionID => Action.RowId;
        private bool IsGCD { get; }
        internal short CastTime => (short)(Action.Cast100ms * 100);

        internal EnemyLocation SayoutText { get; set; } = EnemyLocation.None;

        internal virtual uint MPNeed { get; }

        /// <summary>
        /// 咏唱时间
        /// </summary>
        private int Cast100 => Action.Cast100ms - (HaveStatusSelfFromSelf(ObjectStatus.LightSpeed) ? 25 : 0);
        /// <summary>
        /// 如果之前是这些ID，那么就不会执行。
        /// </summary>
        internal uint[] OtherIDsNot { private get; set; } = null;

        /// <summary>
        /// 如果之前不是这些ID中的某个，那么就不会执行。
        /// </summary>
        internal uint[] OtherIDsCombo { private get; set; } = null;
        /// <summary>
        /// 给敌人造成的Debuff,如果有这些Debuff，那么不会执行。
        /// </summary>
        internal ushort[] TargetStatus { get; set; } = null;
        /// <summary>
        /// 使用了这个技能会得到的Buff，如果有这些Buff中的一种，那么就不会执行。 
        /// </summary>
        internal ushort[] BuffsProvide { get; set; } = null;

        /// <summary>
        /// 使用这个技能需要的前置Buff，有任何一个就好。
        /// </summary>
        internal ushort[] BuffsNeed { get; set; } = null;

        /// <summary>
        /// 如果有一些别的需要判断的，可以写在这里。True表示可以使用这个技能。
        /// </summary>
        internal Func<bool> OtherCheck { get; set; } = null;

        internal BaseAction(uint actionID, bool isFriendly = false)
        {
            this.Action = Service.DataManager.GetExcelSheet<Action>().GetRow(actionID);
            _isFriendly = isFriendly;
            this.IsGCD = Action.CooldownGroup == GCDCooldownGroup;

            if (Action.PrimaryCostType == 3 || Action.PrimaryCostType == 4)
            {
                this.MPNeed = Action.PrimaryCostValue * 100u;
            }
            else if (Action.SecondaryCostType == 3 || Action.SecondaryCostType == 4)
            {
                this.MPNeed = Action.SecondaryCostValue * 100u;
            }
            else
            {
                this.MPNeed = 0;
            }
        }

        public bool TryUseAction(byte level, out BaseAction action, uint lastAct = 0, bool mustUse = false, bool Empty = false)
        {
            action = this;

            //等级不够。
            if (level < this.Level) return false;

            //MP不够
            if (Service.ClientState.LocalPlayer.CurrentMp < this.MPNeed) return false;

            //没有前置Buff

            if(BuffsNeed != null)
            {
                if (!HaveStatusSelfFromSelf(BuffsNeed)) return false;
            }

            //已有提供的Buff的任何一种
            if (BuffsProvide != null)
            {
                if (HaveStatusSelfFromSelf(BuffsProvide)) return false;
            }


            //如果是能力技能，而且没法释放。
            var cool = CoolDown;
            if (!IsGCD && cool.IsCooldown)
            {
                byte charge = Action.MaxCharges;
                if (charge < 2) return false;
                if (cool.CooldownElapsed < Action.Recast100ms / 10f) return false;
            }

            //如果必须要用，那么以下的条件就不用判断了。
            if (mustUse)
            {
                return true;
            }

            if (IsGCD)
            {
                //如果有输入上次的数据，那么上次不能是上述的ID。
                if (OtherIDsNot != null)
                {
                    foreach (var id in OtherIDsNot)
                    {
                        if (lastAct == id) return false;
                    }
                }

                //如果有Combo，有LastAction，而且上次不是连击，那就不触发。
                uint[] comboActions = Action.ActionCombo.Row == 0 ? new uint[0] : new uint[] { Action.ActionCombo.Row };
                if (OtherIDsCombo != null) comboActions = comboActions.Union(OtherIDsCombo).ToArray();
                bool findCombo = false;
                foreach (var comboAction in comboActions)
                {
                    if (comboAction == lastAct)
                    {
                        findCombo = true;
                        break;
                    }
                }
                if (!findCombo && comboActions.Length > 0) return false;

                //目标已有充足的Debuff
                if (TargetStatus != null)
                {
                    if (EnoughStatusTargetFromSelfOne(TargetStatus)) return false;
                }

                //如果是个法术需要咏唱，并且还在移动，也没有即刻相关的技能。
                if (Cast100 > 0 && TargetHelper.IsMoving)
                {
                    if (!HaveStatusSelfFromSelf(CustomCombo.GeneralActions.Swiftcast.BuffsProvide)) return false;
                }
            }
            else
            {
                //如果是能力技能，还没填满。
                if (!Empty && cool.CooldownRemaining > 5) return false;
            }

            //用于自定义的要求没达到
            if (OtherCheck != null && !OtherCheck()) return false;

            //如果是个范围，并且人数不够的话，就算了。
            if (!TargetHelper.ActionGetATarget(Action, _isFriendly)) return false;

            return true;
        }

        /// <summary>
        /// 找到玩家附加到敌人身上的状态。
        /// </summary>
        /// <param name="effectID"></param>
        /// <returns></returns>
        internal static float[] FindStatusTargetFromSelf(params ushort[] effectIDs)
        {
            if(Service.TargetManager.Target is BattleChara chara)
            {
                return FindStatusFromSelf(chara, effectIDs);
            }
            else return new float[0];
        }


        internal static bool HaveStatusSelfFromSelf(params ushort[] effectIDs)
        {
            return FindStatusSelfFromSelf(effectIDs).Length > 0;
        }
        internal static float[] FindStatusSelfFromSelf(params ushort[] effectIDs)
        {
            return FindStatusFromSelf(Service.ClientState.LocalPlayer, effectIDs);
        }
        internal static bool EnoughStatusTargetFromSelfOne(params ushort[] effectIDs)
        {
            var result = FindStatusTargetFromSelf(effectIDs);
            if (result.Length == 0) return false;
            return result.Min() > 5.5f;
        }
        private  static float[] FindStatusFromSelf(BattleChara obj, ushort[] effectIDs)
        {
            uint[] newEffects = effectIDs.Select(a => (uint)a).ToArray();
            return FindStatusFromSelf(obj).Where(status => newEffects.Contains(status.StatusId)).Select(status => status.RemainingTime).ToArray();
        }

        private static Status[] FindStatusFromSelf(BattleChara obj)
        {
            if (obj == null) return new Status[0];

            return  obj.StatusList.Where(status => status.SourceID == Service.ClientState.LocalPlayer.ObjectId && status.RemainingTime != 0).ToArray();
        }

        //internal static Status FindStatus(ushort effectID, GameObject obj, uint? sourceID)
        //{
        //    if (obj == null)
        //    {
        //        return null;
        //    }
        //    BattleChara val = (BattleChara)obj;
        //    if (val == null)  return null;

        //    if (val.StatusList == null) return null;
        //    foreach (Status status in val.StatusList)
        //    {
        //        if (status.StatusId == effectID && (!sourceID.HasValue || status.SourceID == 0 || status.SourceID == 3758096384u || status.SourceID == sourceID))
        //        {
        //            return status;
        //        }
        //    }
        //    return null;
        //}
    }
}
