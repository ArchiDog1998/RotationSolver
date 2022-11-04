using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVAutoAttack.Combos.CustomCombo
{
    public abstract partial class CustomCombo
    {
        /// <summary>
        /// 在起手的状态
        /// </summary>
        protected bool InStartState { get; private set; }

        private void UpdateStartState()
        {
            if (InCombat)
            {
                if (InStartState && ShouldEndStartState())
                {
                    InStartState = false;
                }
            }
            else
            {
                InStartState = true;
            }
        }

        /// <summary>
        /// 是否应该结束起手了
        /// </summary>
        /// <returns>true为要结束起手</returns>
        private protected virtual bool ShouldEndStartState() => false;

        /// <summary>
        /// 有什么是需要每一帧进行更新数据用的，放这里。如果有自定义字段，需要在此函数内全部更新一遍。
        /// </summary>
        private protected virtual void UpdateInfo() { }

        /// <summary>
        /// 如果有Override就一定要返回一点字符串！
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal virtual string OnCommand(string args) => string.Empty;
    }
}
