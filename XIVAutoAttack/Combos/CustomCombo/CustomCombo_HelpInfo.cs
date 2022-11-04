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
