using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVAutoAttack.Combos.CustomCombo
{
    public abstract partial class CustomCombo<TCmd> where TCmd : Enum
    {
        protected TCmd LastCommand { get; private set; } = default(TCmd);

        DateTime _lactCommandTime = DateTime.Now;
        protected TimeSpan LastCommandElapsed => DateTime.Now - _lactCommandTime;

        public Dictionary<string, string> CommandShow
        {
            get
            {
                var dict = new Dictionary<string, string>();
                foreach (var pair in CommandDescription)
                {
                    dict[pair.Key.ToString()] = pair.Value;
                }
                return dict;
            }
        }

        protected virtual SortedList<TCmd, string> CommandDescription { get; } = new SortedList<TCmd, string>();


        /// <summary>
        /// 有什么是需要每一帧进行更新数据用的，放这里。如果有自定义字段，需要在此函数内全部更新一遍。
        /// </summary>
        private protected virtual void UpdateInfo() { }

        /// <summary>
        /// 如果有Override就一定要返回一点字符串！
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public string OnCommand(string args)
        {
            foreach (TCmd value in Enum.GetValues(typeof(TCmd)))
            {
                var name = Enum.GetName(typeof(TCmd), value);

                if (string.IsNullOrEmpty(name)) continue;

                if(name == args)
                {
                    LastCommand = value;
                    _lactCommandTime = DateTime.Now;

                    if (!CommandDescription.TryGetValue(value, out var desc) || string.IsNullOrEmpty(desc))
                        desc = value.ToString();
                    return $"成功执行\"{desc}\"";
                }
            }

            return string.Empty;
        }
    }
}
