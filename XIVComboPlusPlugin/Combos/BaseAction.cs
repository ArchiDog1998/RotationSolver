using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionCategory = Lumina.Excel.GeneratedSheets.ActionCategory;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace XIVComboPlus.Combos
{
    internal class BaseAction
    {
        private bool _isFriendly;
        internal Action Action { get; }
        internal IconReplacer.CooldownData CoolDown => Service.IconReplacer.GetCooldown(ActionID);
        private uint _actionType => Action.ActionCategory.Value.RowId;
        internal byte Level => Action.ClassJobLevel;
        internal uint ActionID => Action.RowId;
        private bool IsAbility => _actionType == 4 || _actionType == 1;
        private bool IsSpell => _actionType == 2;
        private bool IsWeaponskill => _actionType == 4;
        internal short CastTime => (short)(Action.Cast100ms * 100);
        internal virtual uint MPNeed
        {
            get
            {
                if(Action.PrimaryCostType == 3 || Action.PrimaryCostType == 4)
                {
                    return Action.PrimaryCostValue * 100u;
                }
                else if(Action.SecondaryCostType == 3 || Action.SecondaryCostType == 4)
                {
                    return Action.SecondaryCostValue * 100u;
                }
                return 0;
            }
        }
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
        /// 使用这个技能不能有的Buff。
        /// </summary>
        internal ushort[] BuffsCantHave { get; set; } = null;

        /// <summary>
        /// 如果有一些别的需要判断的，可以写在这里。True表示可以使用这个技能。
        /// </summary>
        internal Func<bool> OtherCheck { get; set; } = null;

        internal BaseAction(uint actionID, bool isFriendly = false)
        {
            Action = Service.DataManager.GetExcelSheet<Action>().GetRow(actionID);
            _isFriendly = isFriendly;
        }

        public bool TryUseAction(byte level, out uint action, uint lastAct = 0, bool mustUse = false, bool Empty = false)
        {
            action = ActionID;

            //等级不够。
            if (level < this.Level) return false;

            //MP不够
            if (Service.ClientState.LocalPlayer.CurrentMp < this.MPNeed) return false;

            //没有前置Buff
            if(BuffsNeed != null)
            {
                bool findFuff = false;
                foreach (var buff in BuffsNeed)
                {
                    if (HaveStatus(FindStatusSelfFromSelf(buff)))
                    {
                        findFuff = true;
                        break;
                    }
                }
                if(!findFuff) return false;
            }

            //如果有不能拥有的Buff的话，就返回。
            if(BuffsCantHave != null)
            {
                foreach (var buff in BuffsCantHave)
                {
                    if (HaveStatus(FindStatusSelfFromSelf(buff)))
                    {
                        return false;
                    }
                }
            }

            //如果是能力技能，而且没法释放。
            byte charge = Action.MaxCharges;
            if (charge < 2 && CoolDown.IsCooldown) return false;
            if (CoolDown.CooldownElapsed / CoolDown.CooldownTotal < 1f / charge) return false;

            //已有提供的Buff的任何一种
            if (BuffsProvide != null)
            {
                foreach (var buff in BuffsProvide)
                {
                    if (HaveStatus(FindStatusSelfFromSelf(buff))) return false;
                }
            }

            //如果必须要用，那么以下的条件就不用判断了。
            if (mustUse) return true;

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
            if(OtherIDsCombo != null) comboActions = comboActions.Union(OtherIDsCombo).ToArray();
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
                foreach (var debuff in TargetStatus)
                {
                    if (EnoughStatus(FindStatusTargetFromSelf(debuff))) return false;
                }
            }

            //如果是能力技能，还没填满。
            if (!Empty && IsAbility && CoolDown.CooldownRemaining > 5) return false;

            //如果是个法术需要咏唱，并且还在移动，也没有即刻相关的技能。
            if (TargetHelper.IsMoving && IsSpell && Action.Cast100ms > 0)
            {
                bool haveSwift = false;
                foreach (var buff in CustomCombo.GeneralActions.Swiftcast.BuffsProvide)
                {
                    if (HaveStatus(FindStatusSelfFromSelf(buff)))
                    {
                        haveSwift = true;
                        break;
                    }
                }
                if (!haveSwift) return false;
            }

            //用于自定义的要求没达到
            if (OtherCheck != null && !OtherCheck()) return false;

            //如果是个范围，并且人数不够的话，就算了。
            if (!TargetHelper.ActionGetATarget(Action, _isFriendly)) return false;

            return true;
        }

        internal static bool EnoughStatus(Status status)
        {
            return StatusRemainTime(status) > 5.5f;
        }
        //internal static bool HaveStatus(Status[] status)
        //{
        //    foreach (var sta in status)
        //    {
        //        if (HaveStatus(sta)) return true;
        //    }
        //    return false;
        //}
        internal static bool HaveStatus(Status status)
        {
            return StatusRemainTime(status) != 0f;
        }
        internal static float StatusRemainTime(Status status)
        {
            return status?.RemainingTime ?? 0f;
        }

        /// <summary>
        /// 找到任何对象附加到自己敌人的状态。
        /// </summary>
        /// <param name="effectID"></param>
        /// <returns></returns>
        internal static Status FindStatusTarget(ushort effectID)
        {
            return FindStatus(effectID, Service.TargetManager.Target, null);
        }

        /// <summary>
        /// 找到任何对象附加到自己身上的状态。
        /// </summary>
        /// <param name="effectID"></param>
        /// <returns></returns>
        internal static Status FindStatusSelf(ushort effectID)
        {
            return FindStatus(effectID, Service.ClientState.LocalPlayer, null);
        }

        /// <summary>
        /// 找到玩家附加到敌人身上的状态。
        /// </summary>
        /// <param name="effectID"></param>
        /// <returns></returns>
        internal static Status FindStatusTargetFromSelf(ushort effectID)
        {
            GameObject currentTarget = Service.TargetManager.Target;
            PlayerCharacter localPlayer = Service.ClientState.LocalPlayer;
            return FindStatus(effectID, currentTarget, localPlayer != null ? new uint?(localPlayer.ObjectId) : null);
        }

        /// <summary>
        /// 找到自己附加到自己身上的状态。
        /// </summary>
        /// <param name="effectID"></param>
        /// <returns></returns>
        internal static Status FindStatusSelfFromSelf(ushort effectID)
        {
            PlayerCharacter localPlayer = Service.ClientState.LocalPlayer,
                            localPlayer2 = Service.ClientState.LocalPlayer;
            return FindStatus(effectID, localPlayer, localPlayer2 != null ? new uint?(localPlayer2.ObjectId) : null);
        }

        private static Status FindStatus(ushort effectID, GameObject obj, uint? sourceID)
        {
            if (obj == null)
            {
                return null;
            }
            BattleChara val = (BattleChara)obj;
            if (val == null)
            {
                return null;
            }
            foreach (Status status in val.StatusList)
            {
                if (status.StatusId == effectID && (!sourceID.HasValue || status.SourceID == 0 || status.SourceID == 3758096384u || status.SourceID == sourceID))
                {
                    return status;
                }
            }
            return null;
        }
    }
}
