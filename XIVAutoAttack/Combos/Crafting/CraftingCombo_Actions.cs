using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Actions.BaseCraftAction;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.Crafting
{
    internal abstract partial class CraftingCombo
    {
        /// <summary>
        /// 制作
        /// </summary>
        public static BaseCraftAction BasicSynthesis { get; } = new(100001, level => level >= 31 ? 1.2 : 1);

        /// <summary>
        /// 加工
        /// </summary>
        public static BaseCraftAction BasicTouch { get; } = new(100002, level => 1);

        /// <summary>
        /// 精修
        /// </summary>
        public static BaseCraftAction MastersMend { get; } = new(100003);

        /// <summary>
        /// 仓促
        /// </summary>
        public static BaseCraftAction HastyTouch { get; } = new(100355, level => 1);

        /// <summary>
        /// 高速制作
        /// </summary>
        public static BaseCraftAction RapidSynthesis { get; } = new(100363, level => level >= 63 ? 5 : 2.5);

        /// <summary>
        /// 观察
        /// </summary>
        public static BaseCraftAction Observe { get; } = new(100010);

        /// <summary>
        /// 秘诀
        /// </summary>
        public static BaseCraftAction TricksoftheTrade { get; } = new(100371);

        /// <summary>
        /// 俭约
        /// </summary>
        public static BaseCraftAction WasteNot { get; } = new(4631)
        {
            BuffsProvide = new StatusID[] { StatusID.WasteNot, StatusID.WasteNot2 }
        };

        /// <summary>
        /// 崇敬
        /// </summary>
        public static BaseCraftAction Veneration { get; } = new(19297);

        /// <summary>
        /// 中级加工
        /// </summary>
        public static BaseCraftAction StandardTouch { get; } = new(100004, level => 1.25);

        /// <summary>
        /// 阔步
        /// </summary>
        public static BaseCraftAction GreatStrides { get; } = new(260)
        {
            BuffsProvide = new StatusID[] { StatusID.GreatStrides },
        };

        /// <summary>
        /// 改革
        /// </summary>
        public static BaseCraftAction Innovation { get; } = new(19004)
        {
            BuffsProvide = new StatusID[] { StatusID.Innovation },
        };

        /// <summary>
        /// 最终确认
        /// </summary>
        public static BaseCraftAction FinalAppraisal { get; } = new(19012)
        {
            BuffsProvide = new StatusID[] { StatusID.FinalAppraisal},
        };

        /// <summary>
        /// 长期俭约
        /// </summary>
        public static BaseCraftAction WasteNot2 { get; } = new(4639)
        {
            BuffsProvide = WasteNot.BuffsProvide,
        };

        /// <summary>
        /// 比尔格的祝福
        /// </summary>
        public static BaseCraftAction ByregotsBlessing { get; } = new(100339, level =>
        {
            var player = Service.ClientState.LocalPlayer;
            if (player == null) return 0;
            return player.StatusStack(false, StatusID.InnerQuiet) * 0.2 + 1;
        });

        /// <summary>
        /// 集中加工
        /// </summary>
        public static BaseCraftAction PreciseTouch { get; } = new(100128, level => 1.5);

        /// <summary>
        /// 坚信
        /// </summary>
        public static BaseCraftAction MuscleMemory { get; } = new(100379, level => 3);

        /// <summary>
        /// 模范制作
        /// </summary>
        public static BaseCraftAction CarefulSynthesis { get; } = new(100203, level => 1.5);

        /// <summary>
        /// 掌握
        /// </summary>
        public static BaseCraftAction Manipulation { get; } = new(4574);

        /// <summary>
        /// 俭约加工
        /// </summary>
        public static BaseCraftAction PrudentTouch { get; } = new(100227, level => 1);

        /// <summary>
        /// 注视制作
        /// </summary>
        public static BaseCraftAction FocusedSynthesis { get; } = new(100235, level => 2);

        /// <summary>
        /// 注视加工
        /// </summary>
        public static BaseCraftAction FocusedTouch { get; } = new(100243, level => 1.5);

        /// <summary>
        /// 闲静
        /// </summary>
        public static BaseCraftAction Reflect { get; } = new(100387, level => 1);

        /// <summary>
        /// 坯料加工
        /// </summary>
        public static BaseCraftAction PreparatoryTouch { get; } = new(100299, level => 2);

        /// <summary>
        /// 坯料制作
        /// </summary>
        public static BaseCraftAction Groundwork { get; } = new(100403, level => 3);

        /// <summary>
        /// 精密制作
        /// </summary>
        public static BaseCraftAction DelicateSynthesis { get; } = new(100323, level => 1);

        /// <summary>
        /// 集中制作
        /// </summary>
        public static BaseCraftAction IntensiveSynthesis { get; } = new(100315, level => 4);

        /// <summary>
        /// 工匠的神速技巧
        /// </summary>
        public static BaseCraftAction TrainedEye { get; } = new(100283);

        /// <summary>
        /// 上级加工
        /// </summary>
        public static BaseCraftAction AdvancedTouch { get; } = new(100411, level => 1.5);

        /// <summary>
        /// 俭约制作
        /// </summary>
        public static BaseCraftAction PrudentSynthesis { get; } = new(100427, level => 1.8);

        /// <summary>
        /// 工匠的神技
        /// </summary>
        public static BaseCraftAction TrainedFinesse { get; } = new(100435, level => 1);

        /// <summary>
        /// 设计变动
        /// </summary>
        public static BaseCraftAction CarefulObservation { get; } = new(100395);

        /// <summary>
        /// 专心致志
        /// </summary>
        public static BaseCraftAction HeartandSoul { get; } = new(100419);
    }
}
