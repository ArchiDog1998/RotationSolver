using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVAutoAttack.Combos.CustomCombo
{
    public enum ComboAuthor
    {
        None,
        Armolion,
        NiGuangOwO,
        fatinghenji,
    }

    public static class ComboAuthorExtension
    {
        public static string ToName(this ComboAuthor author)
        {
            switch (author)
            {
                default:
                case ComboAuthor.None:
                    return "无作者，欢迎自荐或推荐";
                case ComboAuthor.Armolion:
                    return "汐ベMoon";
                case ComboAuthor.NiGuangOwO:
                    return "逆光";
                case ComboAuthor.fatinghenji:
                    return "玖祁";
            }
        }
    }
}
