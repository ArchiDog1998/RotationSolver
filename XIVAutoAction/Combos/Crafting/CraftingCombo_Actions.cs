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
        public static BaseCraftAction BasicSynthesis { get; } = new(100001, CraftActionType.Progress, level => level >= 31 ? 1.2 : 1);

        /// <summary>
        /// 加工
        /// </summary>
        public static BaseCraftAction BasicTouch { get; } = new(100002, CraftActionType.Quality, level => 1);

        /// <summary>
        /// 精修
        /// </summary>
        public static BaseCraftAction MastersMend { get; } = new(100003, CraftActionType.Other);

        /// <summary>
        /// 仓促
        /// </summary>
        public static BaseCraftAction HastyTouch { get; } = new(100355, CraftActionType.Quality, level => 1);

        /// <summary>
        /// 高速制作
        /// </summary>
        public static BaseCraftAction RapidSynthesis { get; } = new(100363, CraftActionType.Progress, level => level >= 63 ? 5 : 2.5);

        /// <summary>
        /// 观察
        /// </summary>
        public static BaseCraftAction Observe { get; } = new(100010, CraftActionType.Other);

        /// <summary>
        /// 秘诀
        /// </summary>
        public static BaseCraftAction TricksoftheTrade { get; } = new(100371, CraftActionType.Other);

        /// <summary>
        /// 俭约
        /// </summary>
        public static BaseCraftAction WasteNot { get; } = new(4631, CraftActionType.Status)
        {
            BuffsProvide = new StatusID[] { StatusID.WasteNot, StatusID.WasteNot2 }
        };

        /// <summary>
        /// 崇敬
        /// </summary>
        public static BaseCraftAction Veneration { get; } = new(19297, CraftActionType.Status);

        /// <summary>
        /// 中级加工
        /// </summary>
        public static BaseCraftAction StandardTouch { get; } = new(100004, CraftActionType.Quality, level => 1.25, 100002);

        /// <summary>
        /// 阔步
        /// </summary>
        public static BaseCraftAction GreatStrides { get; } = new(260, CraftActionType.Status)
        {
            BuffsProvide = new StatusID[] { StatusID.GreatStrides },
        };

        /// <summary>
        /// 改革
        /// </summary>
        public static BaseCraftAction Innovation { get; } = new(19004, CraftActionType.Status)
        {
            BuffsProvide = new StatusID[] { StatusID.Innovation },
        };

        /// <summary>
        /// 最终确认
        /// </summary>
        public static BaseCraftAction FinalAppraisal { get; } = new(19012, CraftActionType.Progress)
        {
            BuffsProvide = new StatusID[] { StatusID.FinalAppraisal },
        };

        /// <summary>
        /// 长期俭约
        /// </summary>
        public static BaseCraftAction WasteNot2 { get; } = new(4639, CraftActionType.Status)
        {
            BuffsProvide = WasteNot.BuffsProvide,
        };

        /// <summary>
        /// 比尔格的祝福
        /// </summary>
        public static BaseCraftAction ByregotsBlessing { get; } = new(100339, CraftActionType.Quality, level =>
        {
            var player = Service.ClientState.LocalPlayer;
            if (player == null) return 0;
            return player.StatusStack(false, StatusID.InnerQuiet) * 0.2 + 1;
        });

        /// <summary>
        /// 集中加工
        /// </summary>
        public static BaseCraftAction PreciseTouch { get; } = new(100128, CraftActionType.Quality, level => 1.5);

        /// <summary>
        /// 坚信
        /// </summary>
        public static BaseCraftAction MuscleMemory { get; } = new(100379, CraftActionType.Quality, level => 3);

        /// <summary>
        /// 模范制作
        /// </summary>
        public static BaseCraftAction CarefulSynthesis { get; } = new(100203, CraftActionType.Progress, level => 1.5);

        /// <summary>
        /// 掌握
        /// </summary>
        public static BaseCraftAction Manipulation { get; } = new(4574, CraftActionType.Status);

        /// <summary>
        /// 俭约加工
        /// </summary>
        public static BaseCraftAction PrudentTouch { get; } = new(100227, CraftActionType.Quality, level => 1);

        /// <summary>
        /// 注视制作
        /// </summary>
        public static BaseCraftAction FocusedSynthesis { get; } = new(100235, CraftActionType.Progress, level => 2);

        /// <summary>
        /// 注视加工
        /// </summary>
        public static BaseCraftAction FocusedTouch { get; } = new(100243, CraftActionType.Quality, level => 1.5);

        /// <summary>
        /// 闲静
        /// </summary>
        public static BaseCraftAction Reflect { get; } = new(100387, CraftActionType.Quality, level => 1);

        /// <summary>
        /// 坯料加工
        /// </summary>
        public static BaseCraftAction PreparatoryTouch { get; } = new(100299, CraftActionType.Quality, level => 2);

        /// <summary>
        /// 坯料制作
        /// </summary>
        public static BaseCraftAction Groundwork { get; } = new(100403, CraftActionType.Progress, level => 3);

        /// <summary>
        /// 精密制作
        /// </summary>
        public static BaseCraftAction DelicateSynthesis { get; } = new(100323, CraftActionType.Progress, level => 1);

        /// <summary>
        /// 集中制作
        /// </summary>
        public static BaseCraftAction IntensiveSynthesis { get; } = new(100315, CraftActionType.Progress, level => 4);

        /// <summary>
        /// 工匠的神速技巧
        /// </summary>
        public static BaseCraftAction TrainedEye { get; } = new(100283, CraftActionType.Quality);

        /// <summary>
        /// 上级加工
        /// </summary>
        public static BaseCraftAction AdvancedTouch { get; } = new(100411, CraftActionType.Quality, level => 1.5, 100004);

        /// <summary>
        /// 俭约制作
        /// </summary>
        public static BaseCraftAction PrudentSynthesis { get; } = new(100427, CraftActionType.Progress, level => 1.8);

        /// <summary>
        /// 工匠的神技
        /// </summary>
        public static BaseCraftAction TrainedFinesse { get; } = new(100435, CraftActionType.Quality, level => 1);

        /// <summary>
        /// 设计变动
        /// </summary>
        public static BaseCraftAction CarefulObservation { get; } = new(100395, CraftActionType.Other);

        /// <summary>
        /// 专心致志
        /// </summary>
        public static BaseCraftAction HeartandSoul { get; } = new(100419, CraftActionType.Other);
    }
}
