using Dalamud.Game.ClientState.Objects.Types;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.CustomCombo
{
    public abstract partial class CustomCombo<TCmd> where TCmd : Enum
    {

        /// <summary>
        /// 在倒计时的时候返回这个函数里面的技能。
        /// </summary>
        /// <param name="remainTime">距离战斗开始的时间(s)</param>
        /// <returns>要使用的技能</returns>
        public virtual IAction CountDownAction(float remainTime) => null;

        /// <summary>
        /// 一些非常紧急的GCD战技，优先级最高
        /// </summary>
        /// <param name="lastComboActionID"></param>
        /// <param name="act"></param>
        /// <returns></returns>
        public virtual bool EmergercyGCD(out IAction act)
        {
            act = null; return false;
        }
        /// <summary>
        /// 常规GCD技能
        /// </summary>
        /// <param name="lastComboActionID"></param>
        /// <param name="act"></param>
        /// <returns></returns>
        public abstract bool GeneralGCD(out IAction act);

        public virtual bool MoveGCD(out IAction act)
        {
            act = null; return false;
        }

        /// <summary>
        /// 单体治疗GCD
        /// </summary>
        /// <param name="lastComboActionID"></param>
        /// <param name="act"></param>
        /// <returns></returns>
        public virtual bool HealSingleGCD(out IAction act)
        {
            act = null; return false;
        }

        /// <summary>
        /// 范围治疗GCD
        /// </summary>
        /// <param name="level"></param>
        /// <param name="lastComboActionID"></param>
        /// <param name="act"></param>
        /// <returns></returns>
        public virtual bool HealAreaGCD(out IAction act)
        {
            act = null; return false;
        }

        public virtual bool DefenseSingleGCD(out IAction act)
        {
            act = null; return false;
        }
        public virtual bool DefenseAreaGCD(out IAction act)
        {
            act = null; return false;
        }
    }
}
